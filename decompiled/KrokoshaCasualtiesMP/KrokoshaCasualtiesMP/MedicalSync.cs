using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using KrokoshaCasualtiesUtils;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

internal class MedicalSync : KrokoshaScavSingleton
{
	private static bool stolenformp_lastHeartBeating = false;

	internal static ECGVisualizer woundview_ecg = null;

	private static Queue<NetBody> server_tosend_queue = new Queue<NetBody>();

	private static int _mischealthsync_cur_body = 0;

	private static int _healthsync_cur_body = 0;

	public const float SyncFrequencyHealth = 0.91245323f;

	public float timer_HealthUpdateSyncClients;

	public static bool IsRefusingHelp(Body b)
	{
		if (!b.aboveMedicalCutoff)
		{
			return b.conscious;
		}
		return false;
	}

	private void LateUpdate()
	{
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		Util.IsInWorld();
		if (Util.IsInWoundView())
		{
			Body body = WoundView.view.body;
			if (!Object.op_Implicit((Object)(object)body))
			{
				return;
			}
			AudioSource component = ((Component)WoundView.view).GetComponent<AudioSource>();
			bool flag = false;
			try
			{
				if ((Object)(object)woundview_ecg == (Object)null)
				{
					woundview_ecg = ((Component)WoundView.view).gameObject.GetComponentInChildren<ECGVisualizer>(true);
				}
				bool followBody = woundview_ecg.followBody;
				if (body.IsBodyLocal())
				{
					woundview_ecg.followBody = true;
				}
				else
				{
					woundview_ecg.followBody = false;
					woundview_ecg.writeHeight = body.GetECGHeight(woundview_ecg.timeToUpdate - 0.028f);
				}
				if (woundview_ecg.followBody != followBody)
				{
					for (int i = 0; i < woundview_ecg.width; i++)
					{
						for (int j = 0; j < woundview_ecg.height; j++)
						{
							Color32 val = woundview_ecg.pixelGrid[i, j];
							val.a = 0;
							woundview_ecg.pixelGrid[i, j] = val;
						}
					}
				}
			}
			catch (Exception ex)
			{
				log.error("WOUNDVIEW ECG OVERRIDE: " + ex.ToString());
			}
			if (!body.alive)
			{
				stolenformp_lastHeartBeating = false;
				Util.WoundViewShowAlertText(Lang.Get("ww_deceased", false));
			}
			else
			{
				if (stolenformp_lastHeartBeating && body.inCardiacArrest)
				{
					stolenformp_lastHeartBeating = false;
					if (!Util.IsUnchipped() && !body.IsBodyLocal())
					{
						Sound.Play("flatline", Vector2.zero, true, false, (Transform)null, 0.8f, 1f, true, true);
					}
				}
				stolenformp_lastHeartBeating = !body.inCardiacArrest;
				if (SharedMain.CheckIfDispersionPunishmentProtocolRuleIsActive())
				{
					NetPlayer netPlayer = NetPlayer.LOCAL_PLAYER;
					NetBody netBody = default(NetBody);
					if (UIInGame.SPECTATOR_MODE && ((Component)body).TryGetComponent<NetBody>(ref netBody))
					{
						netPlayer = netBody.plr;
					}
					if (Util.IsBodyLocal(body) && (Object)(object)netPlayer != (Object)null)
					{
						if (SharedMain.CheckIfShouldActivateDispersionPunishmentProtocol(netPlayer, out var max_recorded_dist, out var _, out var _))
						{
							int num = (int)Time.unscaledTime % 3;
							flag = true;
							Util.WoundViewShowAlertText(num switch
							{
								0 => Lang.Get("healthchipalert_antidispersion1", false), 
								1 => Locale.GetOther("healthchipradalert2"), 
								2 => Lang.Get("healthchipalert_antidispersion3", false), 
								_ => Locale.GetOther("healthchipradalert2"), 
							});
						}
						else if (!Util.WoundViewAlertIsShown() && max_recorded_dist > Util.MetersToTiles(KrokoshaScavMultiplayer.rules.ScatterPunishDistance) * 0.8f)
						{
							Util.WoundViewShowAlertText(((int)Time.unscaledTime % 2) switch
							{
								0 => Lang.Get("healthchipalert_antidispersion_warn", false), 
								_ => Lang.Get("healthchipalert_antidispersion3", false), 
							});
						}
					}
				}
			}
			component.volume = ((body.alive || flag) ? 1 : 0);
		}
		else
		{
			stolenformp_lastHeartBeating = false;
		}
	}

	public static bool TryGetNetBodyAndLimb(knetid netBodyId, byte limbId, out NetBody nb, out Limb limb)
	{
		if (NetBody.TryGetNetBodyFromId(netBodyId, out nb) && limbId < nb.body.limbs.Count())
		{
			limb = nb.body.limbs[limbId];
			return true;
		}
		limb = null;
		return false;
	}

	public static bool Server_CheckCanHealerUseItemForLimb(knetid healer_CId, knetid target_bodyNetId, byte limb_id, knetid item_syncid, out NetPlayer healersci, out NetBody target_nb, out SyncInfo item_si)
	{
		healersci = null;
		target_nb = null;
		item_si = null;
		float num = SharedMain.max_player_interaction_distance * 2.5f;
		if (NetPlayer.TryGetNetPlayerAndBodyFromClientId(healer_CId, out healersci, out var body) && body.conscious && TryGetNetBodyAndLimb(target_bodyNetId, limb_id, out target_nb, out var _) && NetObjectRegistry.TryGetSyncInfo(item_syncid, out item_si) && item_si.IsItem() && item_si.item.Stats.usableOnLimb && ItemSync.CheckIfBodyReachThisItem(item_si.item, body, num))
		{
			return Util.DoFullInteractionCheck(body, target_nb.body, do_effect: false, num, check_obstruction: false);
		}
		return false;
	}

	public static void Server_SendCharacterHealth(NetBody nb, bool force = false)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if (force || CanSyncHealth())
		{
			Body body = nb.body;
			NetDataWriter writer = Net.CreateWriter(10127);
			writer.Put((ushort)nb.netId);
			writer.Put(new CharacterHealthStateSyncPacket(body));
			writer.Put(false);
			writer.CompressWriter();
			Net.Server_SendToClients((DeliveryMethod)4, in writer, (IEnumerable<knetid>)ServerMain.AllClientIdsExceptHost);
		}
	}

	public static void Server_QueueSendCharacterHealth(NetBody nb, bool force = false)
	{
		Con.ConFailIfNetworkIsRunningAndIsClient();
		if (force || (CanSyncHealth() && !nb._SERVER_healthsync_skip_frame))
		{
			nb._SERVER_healthsync_skip_frame = true;
			server_tosend_queue.Enqueue(nb);
		}
	}

	public static bool CanSyncHealth()
	{
		if (ServerMain._DEV_ENABLE_HP_SYNC)
		{
			return true;
		}
		return false;
	}

	private IEnumerator HealthUpdateSyncClients()
	{
		if (Util.IsWorldGenerated() && KrokoshaScavMultiplayer.network_system_is_running && CanSyncHealth())
		{
			List<NetBody> tosync = new List<NetBody>();
			int bytecounter = 0;
			int fullpacksize = Marshal.SizeOf(typeof(CharacterHealthPainkillerStateSyncPacket)) + 8 + 2;
			NetDataWriter writer = Net.CreateWriter(10128);
			Painkillers val = default(Painkillers);
			while (true)
			{
				bool flag = false;
				if (_mischealthsync_cur_body >= NetBody.all_instances.Count())
				{
					_mischealthsync_cur_body = 0;
					flag = true;
				}
				if (bytecounter + fullpacksize >= 1220)
				{
					flag = true;
				}
				if (flag)
				{
					break;
				}
				NetBody netBody = NetBody.all_instances[_mischealthsync_cur_body];
				_mischealthsync_cur_body++;
				if (!netBody._SERVER_healthsync_skip_frame && !((Object)(object)netBody.body == (Object)null) && ((Component)netBody.body).TryGetComponent<Painkillers>(ref val))
				{
					tosync.Add(netBody);
					bytecounter += fullpacksize;
					yield return null;
				}
			}
			timer_HealthUpdateSyncClients = -5f;
			yield return null;
			tosync.RemoveAll((NetBody x) => (Object)(object)x.body == (Object)null);
			if (tosync.Count > 0)
			{
				writer.SetPosition(2);
				writer.Put((ushort)tosync[0].netId);
				writer.Put(new CharacterHealthPainkillerStateSyncPacket(tosync[0].body));
				for (int num = 1; num < tosync.Count; num++)
				{
					writer.Put(true);
					writer.Put((ushort)tosync[num].netId);
					writer.Put(new CharacterHealthPainkillerStateSyncPacket(tosync[num].body));
				}
				writer.Put(false);
				Net.Server_SendToClients((DeliveryMethod)4, in writer, (IEnumerable<knetid>)ServerMain.AllClientIdsExceptHost);
				tosync.Clear();
			}
			while (_healthsync_cur_body < NetBody.all_instances.Count())
			{
				NetBody netBody2 = NetBody.all_instances[_healthsync_cur_body];
				_healthsync_cur_body++;
				if (netBody2._SERVER_healthsync_skip_frame)
				{
					netBody2._SERVER_healthsync_skip_frame = false;
					_healthsync_cur_body++;
					continue;
				}
				if (!netBody2.body.alive)
				{
					netBody2._SERVER_healthsync_skip_frame = !netBody2._SERVER_healthsync_skip_frame;
				}
				Server_SendCharacterHealth(netBody2);
				yield return null;
			}
			_healthsync_cur_body = 0;
		}
		yield return null;
		timer_HealthUpdateSyncClients = 0f;
	}

	private void Start()
	{
	}

	protected void Update()
	{
		if (KrokoshaScavMultiplayer.is_client)
		{
			return;
		}
		if (server_tosend_queue.Count > 0)
		{
			NetBody netBody = server_tosend_queue.Dequeue();
			if ((Object)(object)netBody != (Object)null)
			{
				netBody._SERVER_healthsync_skip_frame = true;
				Server_SendCharacterHealth(netBody, force: true);
			}
		}
		timer_HealthUpdateSyncClients += Time.unscaledDeltaTime;
		if (timer_HealthUpdateSyncClients > 0.91245323f)
		{
			timer_HealthUpdateSyncClients = -2 * NetBody.all_instances.Count;
			((MonoBehaviour)this).StartCoroutine(HealthUpdateSyncClients());
		}
	}

	[ClientReceiver(10128, true)]
	private static void ClientReceiver__HealthPainkillerSync(knetid _, ref NetDataReader reader)
	{
		reader.Get(out knetid result);
		reader.Get(out CharacterHealthPainkillerStateSyncPacket result2);
		Body bodyFromClientId = NetPlayer.GetBodyFromClientId(result);
		if ((Object)(object)bodyFromClientId != (Object)null)
		{
			result2.Apply(bodyFromClientId);
		}
		bool flag = default(bool);
		while (true)
		{
			reader.Get(ref flag);
			if (flag)
			{
				reader.Get(out knetid result3);
				reader.Get(out CharacterHealthPainkillerStateSyncPacket result4);
				Body bodyFromClientId2 = NetPlayer.GetBodyFromClientId(result3);
				if ((Object)(object)bodyFromClientId2 != (Object)null)
				{
					result4.Apply(bodyFromClientId2);
				}
				continue;
			}
			break;
		}
	}

	[ClientReceiver(10127, true)]
	private static void ClientReceiver__HealthSync(knetid _, ref NetDataReader reader)
	{
		reader = reader.DecompressReader();
		reader.Get(out knetid result);
		reader.Get(out CharacterHealthStateSyncPacket result2);
		if (NetBody.TryGetNetBodyFromId(result, out var nb))
		{
			result2.Apply(nb);
		}
	}

	[ClientReceiver(10025, true)]
	private static void ClientReceiver__VomiterVomit(knetid _, ref NetDataReader reader)
	{
		reader.Get(out knetid result);
		bool isblood = default(bool);
		reader.Get(ref isblood);
		Body bodyFromClientId = NetPlayer.GetBodyFromClientId(result);
		if ((Object)(object)bodyFromClientId != (Object)null)
		{
			Vomiter_Vomit_MultiplayerPatch.ForceNoWarning(bodyFromClientId.vomiter, isblood);
		}
	}

	[ServerReceiver(10125)]
	private static void ServerReceiver_WoundSpecialAction(knetid clientId, ref NetDataReader reader)
	{
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		if (!CanSyncHealth())
		{
			return;
		}
		reader.Get(out LimbNetId result);
		if (NetPlayer.TryGetNetPlayerAndBodyFromClientId(clientId, out var plr, out var body) && plr.body.conscious && result.TryGetNetBodyAndLimbSafe(out var nb, out var limb))
		{
			Body body2 = nb.body;
			if ((Object)(object)body != (Object)(object)body2)
			{
				Component ba = (Component)(object)plr.body;
				Component bb = (Component)(object)body2;
				if (!KM.dist2dsqrcheck(in ba, in bb, SharedMain.max_player_interaction_distance * 1.3f) || !Util.AccurateRaycastInteractionCheckObstruction(plr.body, body2, do_effect: false))
				{
					return;
				}
			}
			if (!plr.is_local)
			{
				PlayerCamera_WoundSpecialAction_MultiplayerPatch.OG_WoundSpecialAction(plr.body, limb);
			}
			log.devevent($"S: WoundSpecialAction {plr} -> {result}   ", Vector2.op_Implicit(((Component)limb).transform.position));
			Server_QueueSendCharacterHealth(nb);
		}
		else
		{
			log.sus(ServerMain.GetPlayerFullDebugString(clientId) + " WoundSpecialAction on an invalid limb");
		}
	}

	[ServerReceiver(10126)]
	private static void ServerReceiver_ApplyWoundItem(knetid clientId, ref NetDataReader reader)
	{
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		if (!CanSyncHealth())
		{
			return;
		}
		reader.Get(out LimbNetId result);
		reader.Get(out knetid result2);
		if (!NetPlayer.TryGetNetPlayerAndNetBodyFromClientId(clientId, out var plr, out var _) || !plr.body.conscious || !result.TryGetNetBodyAndLimbSafe(out var nb, out var limb) || !ItemSync.TryGetItemSyncInfo(result2, out var si))
		{
			return;
		}
		if (!ItemSync.CheckIfBodyReachThisItem(si, plr.body, 20f, check_obstruction: true))
		{
			Plugin.log.LogWarning((object)$"Server receives: PlayerCamera.ApplyWoundItem FAILED: CANT REACH {plr} -> {result} si:{si}  ");
		}
		else if (PlayerCamera_ApplyWoundItem_MultiplayerPatch.DoApplyWoundItemChecks(plr.body, limb, si.item))
		{
			NetDataWriter writer = Net.CreateWriter(10129);
			writer.Put((ushort)clientId);
			writer.Put(result);
			writer.Put((ushort)result2);
			if (log.verbose)
			{
				log.l($"Server receives: PlayerCamera.ApplyWoundItem {plr.playername} -> {result} si:{si}  ");
			}
			Net.Server_SendToClients((DeliveryMethod)0, in writer, (IEnumerable<knetid>)ServerMain.GetListOfClientIdsExceptThisAndHost(clientId));
			MinigameBase_StartMinigame_MultiplayerPatch.ignore_next_mgstart_cuz_its_not_me = !Util.IsBodyLocal(plr.body);
			PlayerCamera_ApplyWoundItem_MultiplayerPatch.ForceApplyWoundItem(plr.body, limb, si.item);
			MinigameBase_StartMinigame_MultiplayerPatch.ignore_next_mgstart_cuz_its_not_me = false;
			Server_QueueSendCharacterHealth(nb);
		}
		else
		{
			Plugin.log.LogWarning((object)$"Server receives: PlayerCamera.ApplyWoundItem FAILED {plr} -> {result} {si}  ");
		}
	}

	[ClientReceiver(10129, true)]
	private static void ClientReceiver__ApplyWoundItem_Relay(knetid _, ref NetDataReader reader)
	{
		reader.Get(out knetid result);
		reader.Get(out LimbNetId result2);
		reader.Get(out knetid result3);
		if (result2.TryGetNetBodyAndLimb(out var _, out var limb) && NetPlayer.TryGetNetPlayerAndBodyFromClientId(result, out var plr, out var body) && plr.is_local && NetObjectRegistry.TryGetSyncInfo(result3, out var si))
		{
			MinigameBase_StartMinigame_MultiplayerPatch.ignore_next_mgstart_cuz_its_not_me = true;
			PlayerCamera_ApplyWoundItem_MultiplayerPatch.ForceApplyWoundItem(body, limb, si.item);
			MinigameBase_StartMinigame_MultiplayerPatch.ignore_next_mgstart_cuz_its_not_me = false;
		}
	}
}
