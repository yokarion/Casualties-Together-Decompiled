using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using KrokoshaCasualtiesUtils;
using LiteNetLib;
using LiteNetLib.Utils;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace KrokoshaCasualtiesMP;

public class MinigameMPManager : KrokoshaScavSingleton
{
	public class MinigameSession
	{
		public bool minigame_ended;

		public ushort session_id;

		public byte minigame_type;

		public float maxdist = 19f;

		public bool object_required;

		public GameObject obj;

		public readonly HashSet<NetPlayer> involved_players = new HashSet<NetPlayer>();

		public readonly HashSet<NetPlayer> all_players_that_ever_were_involved = new HashSet<NetPlayer>();

		public IReadOnlyList<knetid> InvolvedPlayersClientIds(bool ignorehost = false)
		{
			List<knetid> list = involved_players.Select((NetPlayer p) => p.clientId).ToList();
			if (ignorehost)
			{
				list.Remove((ushort)0);
			}
			return list;
		}

		public virtual void Init()
		{
		}

		public virtual void Update()
		{
		}

		public virtual void FixedUpdate()
		{
		}

		public virtual void OnPlayerJoin(NetPlayer plr)
		{
		}

		public virtual void OnPlayerLMBDown(NetPlayer plr, Vector2 aleged_handPos)
		{
		}

		public virtual void OnPlayerLMBUp(NetPlayer plr, Vector2 aleged_handPos)
		{
		}
	}

	public abstract class _LimbMedicalMinigameSession : MinigameSession
	{
		public Limb limb => obj.GetComponent<Limb>();

		public override void Init()
		{
			object_required = true;
			maxdist = SharedMain.max_player_interaction_distance * 1.7f;
		}
	}

	public class ShrapnelMinigameSession : _LimbMedicalMinigameSession
	{
		public readonly List<Vector2> shrapnel_locations = new List<Vector2>
		{
			new Vector2(-248f, -316.85f),
			new Vector2(-143f, -316.85f),
			new Vector2(0f, -316.85004f),
			new Vector2(116f, -316.85004f),
			new Vector2(208f, -316.85f)
		};

		public readonly List<(double, NetPlayer)> last_shrapnel_movement = new List<(double, NetPlayer)>();

		private float synctimer;

		public static List<NetPlayer> client_shrapnel_owners = new List<NetPlayer> { null, null, null, null, null };

		public override void Init()
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			base.Init();
			int num = shrapnel_locations.Count() - base.limb.shrapnel;
			for (int i = 0; i < shrapnel_locations.Count(); i++)
			{
				Vector2 val = shrapnel_locations[i];
				val += Random.insideUnitCircle * 20f;
				if (num > 0)
				{
					num--;
					val += Vector2.up * 2500f;
				}
				shrapnel_locations[i] = val;
				last_shrapnel_movement.Add((0.0, null));
			}
		}

		public override void FixedUpdate()
		{
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			synctimer -= Time.fixedDeltaTime;
			if (!(synctimer <= 0f))
			{
				return;
			}
			synctimer = 0.065f;
			NetDataWriter writer = Net.CreateWriter(10054);
			writer.Put(new LimbNetId(base.limb));
			writer.Put((ushort)shrapnel_locations.Count());
			for (int i = 0; i < shrapnel_locations.Count(); i++)
			{
				Vector2 value = shrapnel_locations[i];
				(double, NetPlayer) value2 = last_shrapnel_movement[i];
				writer.Put(value);
				if ((Object)(object)value2.Item2 == (Object)null)
				{
					writer.Put(false);
				}
				else
				{
					writer.Put(true);
					writer.Put((ushort)value2.Item2.clientId);
				}
				if (value2.Item1 < Time.realtimeSinceStartupAsDouble && (Object)(object)value2.Item2 != (Object)null)
				{
					value2.Item2 = null;
					last_shrapnel_movement[i] = value2;
				}
			}
			Net.Server_SendToClients((DeliveryMethod)4, in writer, (IEnumerable<knetid>)InvolvedPlayersClientIds());
		}

		[ClientReceiver(10054, false)]
		private static void ClientReceiver_MinigameSchrapnelLocations(knetid _, ref NetDataReader reader)
		{
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			ShrapnelMinigame_Update_MultiplayerPatch.last_packet_was_a_change = false;
			reader.Get(out LimbNetId result);
			ushort num = default(ushort);
			reader.Get(ref num);
			if (!Minigame.op_Implicit(MinigameBase.main.currentMinigame))
			{
				return;
			}
			Minigame currentMinigame = MinigameBase.main.currentMinigame;
			ShrapnelMinigame val = (ShrapnelMinigame)(object)((currentMinigame is ShrapnelMinigame) ? currentMinigame : null);
			if (val == null || !result.TryGetNetBodyAndLimb(out var _, out var _))
			{
				return;
			}
			List<RectTransform> value = Traverse.Create((object)val).Field("objects").GetValue<List<RectTransform>>();
			RectTransform value2 = Traverse.Create((object)val).Field("currentlyHeld").GetValue<RectTransform>();
			bool flag = default(bool);
			for (int i = 0; i < num; i++)
			{
				reader.Get(out Vector2 result2);
				RectTransform val3 = value[i];
				if ((Object)(object)value2 != (Object)(object)val3)
				{
					if (val3.anchoredPosition != result2)
					{
						ShrapnelMinigame_Update_MultiplayerPatch.last_packet_was_a_change = true;
					}
					val3.anchoredPosition = result2;
				}
				reader.Get(ref flag);
				if (flag)
				{
					reader.Get(out knetid result3);
					client_shrapnel_owners[i] = NetPlayer.GetNetPlayerFromClientId(result3);
				}
				else
				{
					client_shrapnel_owners[i] = null;
				}
			}
		}

		[ServerReceiver(10069)]
		private static void ServerReceiver_ShrapnelMinigame_MovingShrapnel(knetid clientId, ref NetDataReader reader)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			byte b = default(byte);
			reader.Get(ref b);
			reader.Get(out Vector2 result);
			if (!NetPlayer.TryGetNetPlayerAndNetBodyFromClientId(clientId, out var plr, out var _) || !result.IsFinite())
			{
				return;
			}
			MinigameSession sessionThisPlayerIsInvolvedIn = GetSessionThisPlayerIsInvolvedIn(plr);
			KM.v2clampSquare(ref result, 524f);
			if (sessionThisPlayerIsInvolvedIn == null || !(sessionThisPlayerIsInvolvedIn is ShrapnelMinigameSession shrapnelMinigameSession) || b >= shrapnelMinigameSession.shrapnel_locations.Count)
			{
				return;
			}
			(double, NetPlayer) value = shrapnelMinigameSession.last_shrapnel_movement[b];
			if (!((Object)(object)value.Item2 == (Object)(object)plr) && !(value.Item1 < Time.realtimeSinceStartupAsDouble))
			{
				return;
			}
			value.Item1 = Time.realtimeSinceStartupAsDouble + 0.15000000596046448;
			value.Item2 = plr;
			shrapnelMinigameSession.last_shrapnel_movement[b] = value;
			Vector2 val = shrapnelMinigameSession.shrapnel_locations[b];
			if (!(result.y > 35f))
			{
				((Vector2)(ref result))._002Ector(val.x, result.y);
			}
			if (result.y < -364f)
			{
				((Vector2)(ref result))._002Ector(result.x, -364f);
			}
			shrapnelMinigameSession.shrapnel_locations[b] = result;
			int num = 0;
			foreach (Vector2 shrapnel_location in shrapnelMinigameSession.shrapnel_locations)
			{
				if (shrapnel_location.y < 34f)
				{
					num++;
				}
			}
			shrapnelMinigameSession.limb.shrapnel = num;
			if (shrapnelMinigameSession.limb.shrapnel != num && shrapnelMinigameSession.limb.TryGetNetBody(out var nb))
			{
				MedicalSync.Server_QueueSendCharacterHealth(nb);
			}
			if (num == 0)
			{
				Server_EndMinigame(sessionThisPlayerIsInvolvedIn, actually_delete_session: true);
			}
		}
	}

	public class DislocationMinigameSession : _LimbMedicalMinigameSession
	{
		public Vector2 bonepos;

		public Vector2 boneVelocity;

		public const float dislocation_timer_minigame_end_treshholddddddddddd = 3f;

		private static float disl_timer_tolerance = 1.4f;

		private float synctimer;

		public static Vector2 FinishSpot => DislocationMinigame.FinishSpot;

		public static float CalculateDislocationTimer(Vector2 bone_anchoredPosition)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return Mathf.Clamp(Vector2.Distance(bone_anchoredPosition, FinishSpot) * 0.2f, 0f, 100f);
		}

		public static void BonePositionTick(ref Vector2 bone_anchoredPosition, ref Vector2 boneVelocity)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			bone_anchoredPosition += boneVelocity * Time.fixedDeltaTime * 3.5f;
			boneVelocity = Vector2.Lerp(boneVelocity, Vector2.zero, Time.fixedDeltaTime * 3.5f);
			bone_anchoredPosition = new Vector2(Mathf.Clamp(bone_anchoredPosition.x, -500f, 900f), Mathf.Clamp(bone_anchoredPosition.y, -500f, 500f));
		}

		public override void Init()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			base.Init();
			bonepos = FinishSpot + DislocationMinigame.GetRandomPointOnRightQuarter() * (base.limb.dislocationTimer * 5f);
		}

		public override void FixedUpdate()
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			synctimer -= Time.fixedDeltaTime;
			BonePositionTick(ref bonepos, ref boneVelocity);
			base.limb.dislocationTimer = CalculateDislocationTimer(bonepos);
			if (base.limb.dislocationTimer < 3f + disl_timer_tolerance)
			{
				base.limb.UnDislocate();
				base.limb.dislocationTimer = 0f;
				MedicalSync.Server_QueueSendCharacterHealth(base.limb.GetNetBody(), force: true);
				Server_EndMinigame(this, actually_delete_session: false);
				obj = null;
			}
			else if (synctimer <= 0f)
			{
				synctimer = 0.06234456f;
				NetDataWriter writer = Net.CreateWriter(10055);
				writer.Put(new LimbNetId(base.limb));
				writer.Put(bonepos);
				writer.Put(boneVelocity);
				Net.Server_SendToClients((DeliveryMethod)4, in writer, (IEnumerable<knetid>)InvolvedPlayersClientIds());
			}
		}

		[ClientReceiver(10055, false)]
		private static void ClientReceiver_MinigameDislocationBonepos(knetid _, ref NetDataReader reader)
		{
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			ShrapnelMinigame_Update_MultiplayerPatch.last_packet_was_a_change = false;
			reader.Get(out LimbNetId _);
			reader.Get(out Vector2 result2);
			reader.Get(out Vector2 result3);
			if (Minigame.op_Implicit(MinigameBase.main?.currentMinigame) && MinigameBase.main.currentMinigame is DislocationMinigame)
			{
				DislocationMinigame_CheckForHit_MultiplayerPatch.just_received_new_packet = true;
				DislocationMinigame_CheckForHit_MultiplayerPatch.lastreceived_bonepos = result2;
				DislocationMinigame_CheckForHit_MultiplayerPatch.lastreceived_boneVelocity = result3;
			}
		}
	}

	public abstract class _UnlockingMinigameSession : MinigameSession
	{
		public BuildingEntity building => obj.GetComponent<BuildingEntity>();

		public Openable openable => obj.GetComponent<Openable>();

		public override void Init()
		{
			object_required = true;
			UsableObject uo = default(UsableObject);
			if (obj.TryGetComponent<UsableObject>(ref uo))
			{
				maxdist = KeypadMinigame_CheckCasts_MultiplayerPatch.GetUsabilityDistanceMP(uo) * 1.1f;
			}
		}
	}

	public class KeypadMinigameSession : _UnlockingMinigameSession
	{
		public string current_input = "";

		public string match { get; protected set; }

		public override void Init()
		{
			base.Init();
			if (string.IsNullOrEmpty(base.openable.code))
			{
				base.openable.code = KeypadMinigame.GenerateCode();
			}
			match = base.openable.code;
		}

		public override void OnPlayerJoin(NetPlayer plr)
		{
			if (!plr.is_local)
			{
				KrokoshaScavMultiplayer.Server_SendSimpleMessageToOneClient((ushort)10070, plr.clientId, match);
			}
			AnnounceCurrentInput();
		}

		public override void OnPlayerLMBDown(NetPlayer plr, Vector2 aleged_handPos)
		{
		}

		public void AnnounceCurrentInput()
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			NetDataWriter writer = Net.CreateWriter(10057);
			writer.Put(current_input, oneByteChars: true);
			Net.Server_SendToClients((DeliveryMethod)0, in writer, (IEnumerable<knetid>)InvolvedPlayersClientIds());
		}

		[ClientReceiver(10057, false)]
		private static void ClientReceiver_MinigameKeypadCurrentInput(knetid _, ref NetDataReader reader)
		{
			reader.Get(out var result, oneByteChars: true);
			Minigame currentMinigame = MinigameBase.main.currentMinigame;
			KeypadMinigame val = (KeypadMinigame)(object)((currentMinigame is KeypadMinigame) ? currentMinigame : null);
			if (val != null)
			{
				val.current = result;
			}
		}

		[ClientReceiver(10070, true)]
		private static void ClientReceiver_MinigameKeypadCode(knetid _, ref NetDataReader reader)
		{
			client_i_know_my_minigame_session = true;
			reader.Get(out var result, oneByteChars: true);
			Minigame currentMinigame = MinigameBase.main.currentMinigame;
			KeypadMinigame val = (KeypadMinigame)(object)((currentMinigame is KeypadMinigame) ? currentMinigame : null);
			if (val != null)
			{
				val.match = result;
				Openable val2 = default(Openable);
				if (((Component)val.toDestroy).TryGetComponent<Openable>(ref val2))
				{
					val2.code = result;
				}
				((TMP_Text)((Component)Minigame.game.spawnedMiniGame.GetChild(0).GetChild(0)).GetComponent<TextMeshProUGUI>()).text = KeypadMinigame.AddDashes(result);
			}
		}
	}

	private class KrokoshaMinigameRemotePeerHand : MonoBehaviour
	{
		public static readonly List<KrokoshaMinigameRemotePeerHand> all_instances = new List<KrokoshaMinigameRemotePeerHand>();

		public Vector2 handVelocity = Vector2.zero;

		public NetPlayer plr;

		public RectTransform handTransform;

		public Image handSprite;

		private void OnDestroy()
		{
			all_instances.Remove(this);
		}

		private void Awake()
		{
			handTransform = ((Component)this).GetComponent<RectTransform>();
			handSprite = ((Component)this).GetComponent<Image>();
			all_instances.Add(this);
		}

		private void Update()
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)plr == (Object)null)
			{
				Object.Destroy((Object)(object)((Component)this).gameObject);
				return;
			}
			float deltaTime = Time.deltaTime;
			Vector2 minigame_mousepos = plr.minigame_mousepos;
			float num = 85f;
			float num2 = 0.25f;
			float num3 = 5f;
			if ((Object)(object)plr.body != (Object)null)
			{
				num3 += plr.body.skills.STRFrom10 * 0.3f;
				num3 *= plr.body.consciousness * 0.01f;
				num2 *= plr.body.consciousness * 0.01f;
			}
			float num4 = 1.5f;
			Vector2 val = Vector2.ClampMagnitude((minigame_mousepos - handTransform.anchoredPosition) * num2, num);
			handVelocity = Vector2.Lerp(handVelocity, val, num3 * deltaTime);
			handVelocity = Vector2.Lerp(handVelocity, Vector2.zero, deltaTime * num4);
			RectTransform obj = handTransform;
			obj.anchoredPosition += handVelocity * deltaTime * 120f;
			handTransform.anchoredPosition = Vector2.Lerp(handTransform.anchoredPosition, plr.minigame_handpos, deltaTime);
			((Transform)handTransform).eulerAngles = new Vector3(0f, 0f, handVelocity.x * 0.5f);
			RectTransform obj2 = handTransform;
			Body body = plr.body;
			((Transform)obj2).localScale = new Vector3((body != null && body.handSlot == 0) ? 1f : (-1f), 1f, 1f);
			handSprite.sprite = MinigameBase.main.handSprites[plr.minigame_handsprite];
		}

		private void OnGUI()
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			if (MinigameBase.main.currentMinigame != null)
			{
				Vector3 position = ((Transform)handTransform).position;
				position = Vector2.op_Implicit(new Vector2(position.x, (float)Screen.height - position.y));
				GUI.Label(new Rect(position.x, position.y, 200f, 20f), plr.playername);
			}
		}
	}

	public static readonly List<MinigameSession> active_sessions = new List<MinigameSession>();

	private static ushort idcounter;

	public static double client_dislocation_mg_ignoretime = 0.0;

	public static bool client_i_know_my_minigame_session = false;

	private static byte last_sent_handimage = 2;

	private static double last_sent_handpos = 0.0;

	private static TwoWayDictionary<Type, byte> minigame_ids;

	private void OnGUI()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		if (!log.verbose)
		{
			return;
		}
		Minigame val = MinigameBase.main?.currentMinigame;
		if (val == null)
		{
			return;
		}
		ShrapnelMinigame val2 = (ShrapnelMinigame)(object)((val is ShrapnelMinigame) ? val : null);
		if (val2 != null)
		{
			List<RectTransform> objects = val2.objects;
			for (int i = 0; i < 5; i++)
			{
				Vector3 position = ((Transform)objects[i]).position;
				position = Vector2.op_Implicit(new Vector2(position.x, (float)Screen.height - position.y));
				NetPlayer netPlayer = ShrapnelMinigameSession.client_shrapnel_owners[i];
				GUI.Label(new Rect(position.x, position.y, 200f, 20f), UnityObjectUtility.ToSafeString((Object)(object)netPlayer));
			}
		}
	}

	public static MinigameSession Server_GetSessionForObject<T>(GameObject obj, byte minigame_type, NetPlayer plr_that_requested) where T : MinigameSession, new()
	{
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		MinigameSession minigameSession = active_sessions.FirstOrDefault((MinigameSession session) => (Object)(object)session.obj == (Object)(object)obj && session.minigame_type == minigame_type);
		if (minigameSession == null)
		{
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(Vector2.op_Implicit(obj.transform.position), "S: GetSessionForObject, CREATING NEW SESSION");
			}
			minigameSession = new T
			{
				obj = obj,
				session_id = idcounter++,
				minigame_type = minigame_type
			};
			active_sessions.Add(minigameSession);
			minigameSession.Init();
		}
		else if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
		{
			DebugHelp.OnNetEvent(Vector2.op_Implicit(obj.transform.position), "S: GetSessionForObject, SESSION ALREADY EXISTS");
		}
		if ((Object)(object)plr_that_requested != (Object)null)
		{
			minigameSession.involved_players.Add(plr_that_requested);
			minigameSession.all_players_that_ever_were_involved.Add(plr_that_requested);
			minigameSession.OnPlayerJoin(plr_that_requested);
		}
		return minigameSession;
	}

	public static MinigameSession GetSessionThisPlayerIsInvolvedIn(NetPlayer plr_that_requested)
	{
		return active_sessions.FirstOrDefault((MinigameSession session) => session.involved_players.Contains(plr_that_requested) && plr_that_requested.minigame_is_in_a_minigame && plr_that_requested.minigame_session == session.session_id && plr_that_requested.minigame_current_type == session.minigame_type);
	}

	public static bool TryGetSessionThisPlayerIsInvolvedIn(NetPlayer plr_that_requested, out MinigameSession session)
	{
		session = GetSessionThisPlayerIsInvolvedIn(plr_that_requested);
		if (session == null)
		{
			return false;
		}
		return true;
	}

	public static bool TryGetSessionThisPlayerIsInvolvedIn<T>(NetPlayer plr_that_requested, out T session) where T : MinigameSession
	{
		MinigameSession sessionThisPlayerIsInvolvedIn = GetSessionThisPlayerIsInvolvedIn(plr_that_requested);
		if (sessionThisPlayerIsInvolvedIn != null && sessionThisPlayerIsInvolvedIn is T val)
		{
			session = val;
			return true;
		}
		session = null;
		return false;
	}

	public static void Client_SendHandPos()
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		byte localHandSpriteIndex = GetLocalHandSpriteIndex();
		if (last_sent_handimage != localHandSpriteIndex)
		{
			KrokoshaScavMultiplayer.Client_SendSimpleMessageToServer((ushort)10073, localHandSpriteIndex);
			last_sent_handimage = localHandSpriteIndex;
		}
		if (Time.realtimeSinceStartupAsDouble - last_sent_handpos > 0.05999999865889549)
		{
			last_sent_handpos = Time.realtimeSinceStartupAsDouble;
			Vector2 localMousePosMinigameSpace = GetLocalMousePosMinigameSpace();
			KrokoshaScavMultiplayer.Client_SendSimpleMessageToServer((ushort)10074, Minigame.game.handPos, localMousePosMinigameSpace, reliable: false);
		}
		KeyCode bind = KeyBinds.GetBind("attack");
		bool keyDown = Input.GetKeyDown(bind);
		bool keyUp = Input.GetKeyUp(bind);
		if (keyDown)
		{
			KrokoshaScavMultiplayer.Client_SendSimpleMessageToServer((ushort)10071, Minigame.game.handPos);
		}
		if (keyUp)
		{
			KrokoshaScavMultiplayer.Client_SendSimpleMessageToServer((ushort)10072, Minigame.game.handPos);
		}
	}

	[ServerReceiver(10071)]
	private static void ServerReceiver_MinigameOnLMBDown(knetid clientId, ref NetDataReader reader)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out Vector2 result);
		if (NetPlayer.TryGetNetPlayerAndNetBodyFromClientId(clientId, out var plr, out var pb))
		{
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(plr.pos, $"S: MinigameOnLMBDown {pb}");
			}
			MinigameSession sessionThisPlayerIsInvolvedIn = GetSessionThisPlayerIsInvolvedIn(plr);
			if (sessionThisPlayerIsInvolvedIn != null && !plr.minigame_lmb_down)
			{
				plr.minigame_lmb_down = true;
				plr.minigame_handpos = result;
				sessionThisPlayerIsInvolvedIn.OnPlayerLMBDown(plr, result);
			}
		}
	}

	[ServerReceiver(10072)]
	private static void ServerReceiver_MinigameOnLMBUp(knetid clientId, ref NetDataReader reader)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out Vector2 result);
		if (NetPlayer.TryGetNetPlayerAndNetBodyFromClientId(clientId, out var plr, out var pb))
		{
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(plr.pos, $"S: MinigameOnLMBUp {pb}");
			}
			MinigameSession sessionThisPlayerIsInvolvedIn = GetSessionThisPlayerIsInvolvedIn(plr);
			if (sessionThisPlayerIsInvolvedIn != null && plr.minigame_lmb_down)
			{
				plr.minigame_lmb_down = false;
				plr.minigame_handpos = result;
				sessionThisPlayerIsInvolvedIn.OnPlayerLMBUp(plr, result);
			}
		}
	}

	public static Vector2 GetLocalMousePosMinigameSpace()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = Vector2.op_Implicit(Input.mousePosition);
		val -= Vector2.op_Implicit(((Component)MinigameBase.main.minigameScreen).transform.position);
		val /= PlayerCamera.uiScale;
		KM.v2clampSquare(ref val, 524f);
		return val;
	}

	public static byte GetLocalHandSpriteIndex()
	{
		if (MinigameBase.main?.currentMinigame != null)
		{
			Image component = ((Component)MinigameBase.main.handTransform).GetComponent<Image>();
			return (byte)Array.IndexOf(MinigameBase.main.handSprites, component.sprite);
		}
		return 2;
	}

	public static bool Server_ValidateHandSprite(byte index, NetBody npc)
	{
		if ((Object)(object)MinigameBase.main == (Object)null)
		{
			return false;
		}
		if (index >= MinigameBase.main.handSprites.Length)
		{
			return false;
		}
		if (npc.body.limbs[5].dismembered)
		{
			if (index != 10)
			{
				return false;
			}
		}
		else if (index == 10)
		{
			return false;
		}
		return true;
	}

	public static bool TryGetMinigameTypeFromId(byte mg, out Type minigame)
	{
		return minigame_ids.TryGetBySecond(mg, out minigame);
	}

	public static byte GetMinigameTypeId(Type mg)
	{
		return minigame_ids[mg];
	}

	private void Awake()
	{
		minigame_ids = GenerateInheritedClassesIdDict(typeof(Minigame));
		client_dislocation_mg_ignoretime = Time.realtimeSinceStartupAsDouble;
	}

	private void CleanUnusedSessions()
	{
		active_sessions.RemoveAll(delegate(MinigameSession sess)
		{
			if ((Object)(object)sess.obj == (Object)null)
			{
				Server_EndMinigame(sess, actually_delete_session: false);
				return true;
			}
			return sess.involved_players.Count <= 0;
		});
	}

	private void Update()
	{
		if (Util.IsInWorld() && Minigame.op_Implicit(MinigameBase.main?.currentMinigame) && client_i_know_my_minigame_session)
		{
			Client_SendHandPos();
		}
		if (!KrokoshaScavMultiplayer.is_server)
		{
			return;
		}
		CleanUnusedSessions();
		foreach (MinigameSession active_session in active_sessions)
		{
			active_session.Update();
		}
	}

	private void FixedUpdate()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if (!KrokoshaScavMultiplayer.is_server)
		{
			return;
		}
		foreach (MinigameSession active_session in active_sessions)
		{
			if ((Object)(object)active_session.obj != (Object)null)
			{
				foreach (NetPlayer involved_player in active_session.involved_players)
				{
					if (!KM.dist2dsqrcheck(involved_player.pos, Vector2.op_Implicit(active_session.obj.transform.position), active_session.maxdist) && involved_player.minigame_is_in_a_minigame)
					{
						Server_ForceEndMinigameForPlayer(involved_player);
						break;
					}
				}
			}
			else if (active_session.object_required)
			{
				Server_EndMinigame(active_session, actually_delete_session: false);
				continue;
			}
			active_session.FixedUpdate();
		}
	}

	public static TwoWayDictionary<Type, byte> GenerateInheritedClassesIdDict(Type base_type)
	{
		List<Type> list = (from t in AppDomain.CurrentDomain.GetAssemblies().SelectMany(delegate(Assembly a)
			{
				try
				{
					return a.GetTypes();
				}
				catch (ReflectionTypeLoadException ex)
				{
					return ex.Types.Where((Type t) => t != null);
				}
			})
			where t.IsClass && !t.IsAbstract && base_type.IsAssignableFrom(t)
			select t).ToList();
		list.Sort((Type a, Type type) => a.Name.CompareTo(type.Name));
		TwoWayDictionary<Type, byte> twoWayDictionary = new TwoWayDictionary<Type, byte>();
		byte b = 0;
		foreach (Type item in list)
		{
			twoWayDictionary.Add(item, b++);
		}
		return twoWayDictionary;
	}

	public static void Server_EndMinigame(MinigameSession sess, bool actually_delete_session)
	{
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		if (sess.minigame_ended)
		{
			return;
		}
		sess.minigame_ended = true;
		List<knetid> list = new List<knetid>();
		foreach (NetPlayer involved_player in sess.involved_players)
		{
			list.Add(involved_player.clientId);
		}
		NetDataWriter writer = Net.CreateWriter(10058);
		writer.Put(sess.session_id);
		writer.Put(sess.minigame_type);
		DeliveryMethod delivery = (DeliveryMethod)2;
		IEnumerable<knetid> clientIds = list;
		Net.Server_SendToClients(in delivery, in writer, in clientIds);
		if (actually_delete_session)
		{
			active_sessions.Remove(sess);
		}
	}

	public static void Server_PlayerExitedMinigame(NetPlayer plr)
	{
		plr.minigame_current_type = byte.MaxValue;
		plr.minigame_currentItem = null;
		plr.minigame_targetobject = new AnyObjectNetId();
		foreach (MinigameSession active_session in active_sessions)
		{
			active_session.involved_players.Remove(plr);
		}
	}

	public static void Server_ForceEndMinigameForPlayer(NetPlayer plr)
	{
		Server_PlayerExitedMinigame(plr);
		KrokoshaScavMultiplayer.Server_SendSimpleMessageToOneClient((ushort)10058, (ushort)plr.clientId, (ushort)(knetid)(ushort)0, true);
	}

	public static void Server_SendCurrentInvolved(NetPlayer plr, MinigameSession session)
	{
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		NetDataWriter writer = Net.CreateWriter(10059);
		HashSet<knetid> hashSet = new HashSet<knetid> { plr.clientId };
		if (session == null)
		{
			writer.Put(idcounter++);
			writer.Put(0);
			writer.Put((ushort)1);
			writer.Put((ushort)plr.clientId);
			writer.Put((byte)2);
		}
		else
		{
			writer.Put(session.session_id);
			writer.Put(session.minigame_type);
			List<NetPlayer> list = new List<NetPlayer>();
			foreach (NetPlayer involved_player in session.involved_players)
			{
				list.Add(involved_player);
				hashSet.Add(involved_player.clientId);
			}
			writer.Put((ushort)list.Count());
			foreach (NetPlayer item in list)
			{
				writer.Put((ushort)item.clientId);
				writer.Put(item.minigame_handsprite);
				if (writer.Length > 1000)
				{
					break;
				}
			}
		}
		Net.Server_SendToClients((DeliveryMethod)0, in writer, (IEnumerable<knetid>)hashSet.ToList());
	}

	[ClientReceiver(10059, false)]
	private static void ClientReceiver_MinigameInvolvedPlayers(knetid _, ref NetDataReader reader)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (Minigame.game.currentMinigame == null)
		{
			return;
		}
		client_i_know_my_minigame_session = true;
		ushort minigame_session = default(ushort);
		reader.Get(ref minigame_session);
		byte minigame_current_type = default(byte);
		reader.Get(ref minigame_current_type);
		ushort num = default(ushort);
		reader.Get(ref num);
		NetPlayer lOCAL_PLAYER = NetPlayer.LOCAL_PLAYER;
		lOCAL_PLAYER.minigame_session = minigame_session;
		if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
		{
			DebugHelp.OnNetEvent(lOCAL_PLAYER.pos, $"C: involved plrs count: {num}");
		}
		Client_DeleteAllRemoteHands();
		byte minigame_handsprite = default(byte);
		for (int i = 0; i < num; i++)
		{
			reader.Get(out knetid result);
			reader.Get(ref minigame_handsprite);
			if (NetPlayer.TryGetPlayerFromClientId(result, out var plr) && (Object)(object)plr != (Object)(object)lOCAL_PLAYER)
			{
				plr.minigame_session = minigame_session;
				plr.minigame_current_type = minigame_current_type;
				plr.minigame_handsprite = minigame_handsprite;
				Client_CreateARemoteHand(plr);
			}
		}
	}

	public static void Client_DeleteAllRemoteHands()
	{
		foreach (KrokoshaMinigameRemotePeerHand all_instance in KrokoshaMinigameRemotePeerHand.all_instances)
		{
			Object.Destroy((Object)(object)((Component)all_instance).gameObject);
		}
	}

	private static void Client_CreateARemoteHand(NetPlayer whos_hand_is_this)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		KrokoshaMinigameRemotePeerHand orAddComponent = ComponentHolderProtocol.GetOrAddComponent<KrokoshaMinigameRemotePeerHand>((Object)(object)Object.Instantiate<GameObject>(((Component)MinigameBase.main.handTransform).gameObject));
		orAddComponent.plr = whos_hand_is_this;
		((Transform)orAddComponent.handTransform).SetParent(((Transform)MinigameBase.main.handTransform).parent);
		((Transform)orAddComponent.handTransform).localScale = ((Transform)MinigameBase.main.handTransform).localScale;
		((Graphic)orAddComponent.handSprite).color = new Color(1f, 1f, 1f, 0.3f);
	}

	[ServerReceiver(10073)]
	private static void ServerReceiver_MinigameHandSprite(knetid clientId, ref NetDataReader reader)
	{
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		byte b = default(byte);
		reader.Get(ref b);
		if (NetPlayer.TryGetNetPlayerAndNetBodyFromClientId(clientId, out var plr, out var pb) && pb.body.alive)
		{
			if (!Server_ValidateHandSprite(b, pb))
			{
				Plugin.log.LogWarning((object)$"SUS: bogus handsprite index: {b}");
				b = 2;
			}
			MinigameSession minigameSession = active_sessions.FirstOrDefault((MinigameSession session) => session.involved_players.Contains(plr) && plr.minigame_is_in_a_minigame);
			if (minigameSession != null)
			{
				plr.minigame_handsprite = b;
				NetDataWriter writer = Net.CreateWriter(10060);
				writer.Put((ushort)clientId);
				writer.Put(b);
				Net.Server_SendToClients((DeliveryMethod)0, in writer, (IEnumerable<knetid>)minigameSession.InvolvedPlayersClientIds());
			}
		}
	}

	[ServerReceiver(10074)]
	private static void ServerReceiver_MinigameHandpos(knetid clientId, ref NetDataReader reader)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out Vector2 result);
		reader.Get(out Vector2 result2);
		if (NetPlayer.TryGetNetPlayerAndNetBodyFromClientId(clientId, out var plr, out var pb) && pb.body.alive)
		{
			if (result2.x > 524f)
			{
				result2.x = 524f;
			}
			if (result2.y > 524f)
			{
				result2.y = 524f;
			}
			if (result2.x < -524f)
			{
				result2.x = -524f;
			}
			if (result2.y < -524f)
			{
				result2.y = -524f;
			}
			plr.minigame_handpos = result;
			plr.minigame_mousepos = result2;
			MinigameSession minigameSession = active_sessions.FirstOrDefault((MinigameSession session) => session.involved_players.Contains(plr) && plr.minigame_is_in_a_minigame);
			if (minigameSession != null)
			{
				NetDataWriter writer = Net.CreateWriter(10061);
				writer.Put((ushort)clientId);
				writer.Put(result);
				writer.Put(result2);
				Net.Server_SendToClients((DeliveryMethod)4, in writer, (IEnumerable<knetid>)minigameSession.InvolvedPlayersClientIds());
			}
		}
	}

	[ClientReceiver(10061, false)]
	private static void ClientReceiver_MinigameHandposRelay(knetid _, ref NetDataReader reader)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out knetid result);
		reader.Get(out Vector2 result2);
		reader.Get(out Vector2 result3);
		if (NetPlayer.TryGetNetPlayerAndNetBodyFromClientId(result, out var plr, out var pb) && !pb.is_local)
		{
			plr.minigame_handpos = result2;
			plr.minigame_mousepos = result3;
		}
	}

	[ClientReceiver(10060, false)]
	private static void ClientReceiver_MinigameHandSpriteRelay(knetid _, ref NetDataReader reader)
	{
		reader.Get(out knetid result);
		byte minigame_handsprite = default(byte);
		reader.Get(ref minigame_handsprite);
		if (NetPlayer.TryGetNetPlayerAndNetBodyFromClientId(result, out var plr, out var pb) && !pb.is_local)
		{
			plr.minigame_handsprite = minigame_handsprite;
		}
	}

	[ClientReceiver(10058, false)]
	private static void ClientReceiver_MinigameForceEnd(knetid _, ref NetDataReader reader)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
		{
			DebugHelp.OnNetEvent(NetPlayer.LOCAL_PLAYER.pos, "C: Server told me to stop the minigame.");
		}
		MinigameBase main = MinigameBase.main;
		if (main != null)
		{
			main.EndMinigame();
		}
	}

	[ServerReceiver(10062)]
	private static void ServerReceiver_MinigameStart(knetid clientId, ref NetDataReader reader)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		byte b = default(byte);
		reader.Get(ref b);
		reader.Get(out knetid result);
		reader.Get(out AnyObjectNetId result2);
		if (!minigame_ids.TryGetBySecond(b, out var key) || !NetPlayer.TryGetNetPlayerAndNetBodyFromClientId(clientId, out var plr, out var pb))
		{
			return;
		}
		Server_PlayerExitedMinigame(plr);
		if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
		{
			DebugHelp.OnNetEvent(plr.pos, $"S: MinigameStart {pb}: {key}, objnet:{result2}");
		}
		if (log.verbose)
		{
			log.l($"SERVER: MinigameStart {plr}: {key.Name}, objnet:{result2}");
		}
		plr.minigame_current_type = b;
		if (ItemSync.TryGetItemSyncInfo(result, out var si))
		{
			if (!ItemSync.CheckIfBodyReachThisItem(si, pb.body))
			{
				log.sus($"{plr} is trying to use {si} but they cant even reach it");
			}
			plr.minigame_currentItem = si.item;
			if (si.IsAED())
			{
				si.item.battery.DrainCharge(0.01f);
				ServerMain.Server_AnnounceSound(si.position, "aedstart", ServerMain.GetListOfClientIdsExceptThis(plr.clientId));
			}
		}
		else
		{
			plr.minigame_currentItem = null;
		}
		MinigameSession minigameSession = _Server_CreateMinigameSession(plr, key, b, result2, ref reader);
		if (minigameSession != null)
		{
			plr.minigame_session = minigameSession.session_id;
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(Vector2.op_Implicit(minigameSession.obj.transform.position), "S: involved plrs " + string.Join(" ", minigameSession.involved_players), Color.cyan, 4f);
			}
			Server_SendCurrentInvolved(plr, minigameSession);
		}
	}

	private static void Server_DenyMinigame(NetPlayer plr)
	{
		Server_ForceEndMinigameForPlayer(plr);
		plr.Server_DoAlertSingle(Lang.MarkMsgAsLocaleKey("minigame_deny_server"));
	}

	private static MinigameSession _Server_CreateMinigameSession(NetPlayer plr, Type realtype, byte realtype_id, AnyObjectNetId objnet, ref NetDataReader reader)
	{
		MinigameSession result = null;
		NetBody playerbody = plr.playerbody;
		bool flag = realtype == typeof(CPRMinigame);
		if (realtype == typeof(AEDMinigame) || flag)
		{
			if (!objnet.TryGetNetBody(out var nb) || !KM.dist2dsqrcheck((Component)(object)playerbody.body, (Component)(object)nb.body, 20f) || (flag && !CPRHandler.IsCPRBeingPerformedOnThisBody_IsThisGuyOrNobody(nb.body, plr)))
			{
				Server_DenyMinigame(plr);
				return result;
			}
		}
		else if (realtype == typeof(ShrapnelMinigame) || realtype == typeof(DislocationMinigame))
		{
			if (!objnet.TryGetNetBodyAndLimb(out var nb2, out var limb) || !KM.dist2dsqrcheck((Component)(object)playerbody.body, (Component)(object)nb2.body, 20f))
			{
				Server_DenyMinigame(plr);
				return result;
			}
			if (realtype == typeof(ShrapnelMinigame) && limb.shrapnel > 0)
			{
				result = Server_GetSessionForObject<ShrapnelMinigameSession>(((Component)limb).gameObject, realtype_id, plr);
			}
			else
			{
				if (!(realtype == typeof(DislocationMinigame)) || !limb.dislocated)
				{
					Server_DenyMinigame(plr);
					return result;
				}
				result = Server_GetSessionForObject<DislocationMinigameSession>(((Component)limb).gameObject, realtype_id, plr);
			}
		}
		else if (realtype == typeof(LockpingMinigame) || realtype == typeof(KeypadMinigame))
		{
			Openable val = default(Openable);
			if (!objnet.TryGetSyncInfo(out var si) || !si.go.TryGetComponent<Openable>(ref val) || !KM.dist2dsqrcheck((Component)(object)playerbody.body, (Component)(object)si.go.transform, 20f))
			{
				Server_DenyMinigame(plr);
				return result;
			}
			if (!(realtype == typeof(LockpingMinigame)) || val.isKeypad)
			{
				if (!(realtype == typeof(KeypadMinigame)) || !val.isKeypad)
				{
					Server_DenyMinigame(plr);
					return result;
				}
				result = Server_GetSessionForObject<KeypadMinigameSession>(si.go, realtype_id, plr);
			}
		}
		plr.minigame_targetobject = objnet;
		return result;
	}

	[ServerReceiver(10075)]
	private static void ServerReceiver_MinigameEnd(knetid clientId, ref NetDataReader reader)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (NetPlayer.TryGetNetPlayerAndNetBodyFromClientId(clientId, out var plr, out var pb))
		{
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(plr.pos, $"S: MinigameEnd {pb}");
			}
			plr.minigame_lmb_down = false;
			MinigameSession sessionThisPlayerIsInvolvedIn = GetSessionThisPlayerIsInvolvedIn(plr);
			Server_PlayerExitedMinigame(plr);
			if (sessionThisPlayerIsInvolvedIn != null)
			{
				Server_SendCurrentInvolved(plr, sessionThisPlayerIsInvolvedIn);
			}
		}
	}
}
