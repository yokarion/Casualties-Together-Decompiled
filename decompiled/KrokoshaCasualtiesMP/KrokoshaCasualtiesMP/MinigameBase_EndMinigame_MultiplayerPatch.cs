using System;
using System.Collections.Generic;
using HarmonyLib;
using KrokoshaCasualtiesUtils;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(MinigameBase), "EndMinigame")]
public static class MinigameBase_EndMinigame_MultiplayerPatch
{
	private static bool Prefix(MinigameBase __instance)
	{
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		MinigameMPManager.Client_DeleteAllRemoteHands();
		MinigameMPManager.client_i_know_my_minigame_session = false;
		if (KrokoshaScavMultiplayer.network_system_is_running)
		{
			try
			{
				if (Minigame.op_Implicit(__instance.currentMinigame))
				{
					Minigame currentMinigame = __instance.currentMinigame;
					SelfHarmMinigame val = (SelfHarmMinigame)(object)((currentMinigame is SelfHarmMinigame) ? currentMinigame : null);
					if (val != null)
					{
						KrokoshaScavMultiplayer.Client_SendSimpleMessageToServer((ushort)10076, (byte)((val.cutsDone != 5) ? (val.isEye ? 1u : 2u) : 0u));
					}
					else if (KrokoshaScavMultiplayer.is_client)
					{
						Minigame currentMinigame2 = __instance.currentMinigame;
						KeypadMinigame val2 = (KeypadMinigame)(object)((currentMinigame2 is KeypadMinigame) ? currentMinigame2 : null);
						if (val2 != null)
						{
							if (val2.current == val2.match && !Object.op_Implicit((Object)(object)val2.toDestroy))
							{
							}
						}
						else
						{
							Minigame currentMinigame3 = __instance.currentMinigame;
							LockpingMinigame val3 = (LockpingMinigame)(object)((currentMinigame3 is LockpingMinigame) ? currentMinigame3 : null);
							if (val3 != null)
							{
								if (val3.lockProgress >= 1f && Object.op_Implicit((Object)(object)val3.toDestroy) && val3.toDestroy.health == 0f && NetObjectRegistry.TryGetSyncInfoOrRegister(((Component)val3.toDestroy).gameObject, out var si))
								{
									KrokoshaScavMultiplayer.Client_SendSimpleMessageToServer((ushort)10078, (ushort)si.syncId, true);
								}
							}
							else if (__instance.currentMinigame is SyringeMinigame)
							{
								Limb listen_to_limb = WaterContainerItem_Inject_MultiplayerPatch.listen_to_limb;
								NetBody component = ((Component)listen_to_limb.body).GetComponent<NetBody>();
								if (listen_to_limb.shrapnel == 1 && listen_to_limb.pain > 20f && NetObjectRegistry.TryGetSyncInfo(((Component)Minigame.game.currentItem).gameObject, out var si2))
								{
									SyringeMinigame_Update_MultiplayerPatch.Client_SendCurrentInjectedAmount(force_sound: true);
									KrokoshaScavMultiplayer.Client_SendSimpleMessageToServer((ushort)10077, si2.syncId, component.netId, Util.GetLimbIndex(listen_to_limb));
								}
							}
							else
							{
								Minigame currentMinigame4 = __instance.currentMinigame;
								DislocationMinigame val4 = (DislocationMinigame)(object)((currentMinigame4 is DislocationMinigame) ? currentMinigame4 : null);
								if (val4 != null)
								{
									if (KrokoshaScavMultiplayer.is_client && !val4.limb.dislocated)
									{
										NetDataWriter writer = Net.CreateWriter(10056);
										writer.Put(new LimbNetId(val4.limb));
										Net.Client_Send((DeliveryMethod)2, in writer);
									}
								}
								else
								{
									Minigame currentMinigame5 = __instance.currentMinigame;
									AmputationMinigame val5 = (AmputationMinigame)(object)((currentMinigame5 is AmputationMinigame) ? currentMinigame5 : null);
									if (val5 != null)
									{
										if (KrokoshaScavMultiplayer.is_client && val5.cutProgress >= 1f && NetObjectRegistry.TryGetSyncInfo((Component)(object)AmputationMinigame_PhysicsUpdate_MultiplayerPatch.cutter_item, out var si3))
										{
											NetDataWriter writer2 = Net.CreateWriter(10063);
											writer2.Put((ushort)si3.syncId);
											writer2.Put(new LimbNetId(val5.limb));
											Net.Client_Send((DeliveryMethod)0, in writer2);
										}
										AmputationMinigame_PhysicsUpdate_MultiplayerPatch.cutter_item = null;
									}
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				log.error("EndMinigame patch: " + ex.ToString());
			}
			NetDataWriter writer3 = Net.CreateWriter(10075);
			writer3.Put(true);
			Net.Client_Send((DeliveryMethod)2, in writer3);
		}
		return true;
	}

	[ServerReceiver(10076)]
	private static void Server_SelfHarmMinigameMinigameEnd(knetid clientId, ref NetDataReader reader)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		byte b = default(byte);
		reader.Get(ref b);
		if (NetPlayer.TryGetNetPlayerAndBodyFromClientId(clientId, out var plr, out var body) && body.alive && body.totalHappiness < -50f)
		{
			Limb limb = null;
			YOU_SHOULD_KILL_YOURSELF_NOOOW.SelfHarm_Copypasted(b, body, ref limb);
			NetDataWriter writer = Net.CreateWriter(10064);
			writer.Put(b);
			writer.Put(new LimbNetId(limb));
			Net.Server_SendToClients((DeliveryMethod)0, in writer, (IEnumerable<knetid>)ServerMain.AllClientIdsExceptHost);
			MedicalSync.Server_QueueSendCharacterHealth(plr.playerbody, force: true);
		}
	}

	public static bool CanAmputate(this Limb limb)
	{
		if (limb.body.IsDeadOrCriticallyDying() || KrokoshaScavMultiplayer.rules.AmputateHealthyPlayers)
		{
			return CombatStuff.CheckIfItsOkeyToDismemberThatLimb(limb);
		}
		bool flag = false;
		for (int i = 9; i < 15; i++)
		{
			if (limb.body.limbs[i].dismembered)
			{
				flag = true;
				break;
			}
		}
		if (limb.infectionAmount > 60f && Array.IndexOf(limb.body.limbs, limb) > 2 && (!flag || !limb.isLegLimb))
		{
			return true;
		}
		return false;
	}

	[ServerReceiver(10063)]
	private static void Server_AmputationMinigame_success(knetid clientId, ref NetDataReader reader)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out knetid result);
		reader.Get(out LimbNetId result2);
		if (!MedicalSync.Server_CheckCanHealerUseItemForLimb(clientId, result2.bodyNetId, result2.limbId, result, out var healersci, out var target_nb, out var item_si))
		{
			return;
		}
		Limb limb = result2.GetLimb();
		if (limb.CanAmputate() && limb.muscleHealth < 70f)
		{
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(Vector2.op_Implicit(((Component)limb).transform.position), $"S: AmputationMinigame_success {healersci} -> {target_nb} {item_si}");
			}
			AmputationMinigame_Update_MultiplayerPatch.AmputationMinigame_FinishAmputateCopypasted(limb);
			if (limb.isHead)
			{
				Body body = healersci.body;
				body.happiness -= 5f;
			}
		}
	}

	[ServerReceiver(10056)]
	private static void Server_DislocationFixSuccess(knetid clientId, ref NetDataReader reader)
	{
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out LimbNetId result);
		if (!NetPlayer.TryGetNetPlayerAndNetBodyFromClientId(clientId, out var plr, out var pb) || !result.TryGetNetBodyAndLimbSafe(out var _, out var limb))
		{
			return;
		}
		MinigameMPManager.MinigameSession sessionThisPlayerIsInvolvedIn = MinigameMPManager.GetSessionThisPlayerIsInvolvedIn(plr);
		if (sessionThisPlayerIsInvolvedIn != null && sessionThisPlayerIsInvolvedIn is MinigameMPManager.DislocationMinigameSession dislocationMinigameSession && (Object)(object)dislocationMinigameSession.obj != (Object)null && (Object)(object)dislocationMinigameSession.limb == (Object)(object)limb && limb.dislocated && limb.dislocationTimer < 10f)
		{
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(Vector2.op_Implicit(((Component)limb).transform.position), $"S: DislocationFixSuccess {pb} -> {result}");
			}
			limb.UnDislocate();
		}
	}

	[ServerReceiver(10077)]
	private static void Server_SyringMinigame_BrokeNeedle(knetid clientId, ref NetDataReader reader)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out knetid result);
		reader.Get(out knetid result2);
		byte b = default(byte);
		reader.Get(ref b);
		if (!MedicalSync.Server_CheckCanHealerUseItemForLimb(clientId, result2, b, result, out var healersci, out var target_nb, out var item_si))
		{
			return;
		}
		Limb val = target_nb.body.limbs[b];
		if (val.shrapnel != 1)
		{
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(Vector2.op_Implicit(((Component)healersci.body).transform.position), $"S: broke needle oops {healersci.playername} -> {target_nb.playername} {item_si}");
			}
			if (!healersci.is_local)
			{
				val.shrapnel = Math.Max(val.shrapnel, 1);
				val.pain += 20f;
			}
			ServerMain.Server_AnnounceSound(Vector2.op_Implicit(((Component)target_nb.body).transform.position), "bullethit", ServerMain.GetListOfClientIdsExceptThis(clientId));
		}
	}

	[ServerReceiver(10078)]
	private static void Server_LockpickingMinigame_Finish_success(knetid clientId, ref NetDataReader reader)
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out knetid result);
		if (NetPlayer.TryGetNetPlayerAndNetBodyFromClientId(clientId, out var plr, out var pb))
		{
			Openable val = default(Openable);
			if (pb.body.conscious && NetObjectRegistry.TryGetSyncInfo(result, out var si) && si.go.TryGetComponent<Openable>(ref val) && KM.dist2dsqrcheck((Component)(object)pb.body, (Component)(object)si.go.transform, 20f) && !val.isKeypad)
			{
				si.building.health = 0f;
				ServerMain.Server_AnnounceSound(si.position, "unlock", ServerMain.AllClientIds);
			}
			else
			{
				plr.Server_DoAlertSingle(Lang.MarkMsgAsLocaleKey("serverdeny_lockpick"));
			}
		}
	}
}
