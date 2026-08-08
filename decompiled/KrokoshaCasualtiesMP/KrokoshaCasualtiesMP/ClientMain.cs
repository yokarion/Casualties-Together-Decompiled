using System;
using System.Collections.Generic;
using System.Diagnostics;
using KrokoshaCasualtiesUtils;
using LiteNetLib;
using LiteNetLib.Utils;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public class ClientMain : MonoBehaviour
{
	internal class SavedHingeJointState : MonoBehaviour
	{
		public Vector3 a;

		public Quaternion b;

		public Vector3 aa;

		public Quaternion bb;

		public bool useLimits;

		public float limitsmax;

		public float limitsmin;

		public float referenceAngle;

		public float h;

		public float l;

		public Rigidbody2D connectedBody;

		public HingeJoint2D hinge;
	}

	public static int LOCAL_PING = -1;

	public static float AdaptiveSyncTimerDelta = 0.03f;

	public static float ServerPerformanceScale = 1f;

	public static int SERVER_FPS = 60;

	public static int SERVER_TPS = 60;

	public static byte MY_CONNECTION_QUALITY = 0;

	public static ServerWorldState serverWorldState = new ServerWorldState
	{
		server_is_generating_world = true
	};

	public const float PUSH_VELOCITY = 15f;

	private static float _ragddistmarg = 0.0025000002f;

	private static float _ragdangvelqua = 100f;

	private static float _raglimbrangequa = 7f;

	private bool _sendPlayerUIStateNextFrame;

	private double _last_sync_time;

	private Vector2 last_movedir_thing = Vector2.zero;

	private Vector2Int16 last_looktarget;

	internal static Dictionary<string, Action<string, NetDataReader>> ClientCustomCommandsDict = new Dictionary<string, Action<string, NetDataReader>>();

	internal static bool _last_reminderpack_while_generating_received = false;

	internal static NetBodySyncPacket _last_reminderpack_while_generating;

	public static float RTT_IN_SECONDS_with_margin => (float)LOCAL_PING * 0.00401f;

	public static bool server_is_generating_world
	{
		get
		{
			return serverWorldState.server_is_generating_world;
		}
		set
		{
			serverWorldState.server_is_generating_world = value;
		}
	}

	public static bool IsInTemporaryWorldPause()
	{
		if (!SharedMain.local_world_is_generating)
		{
			if (KrokoshaScavMultiplayer.is_client)
			{
				return server_is_generating_world;
			}
			return false;
		}
		return true;
	}

	public static void DoVerboseObjInfo(Component itemorbuild, ref string ttname, ref string ttdesc)
	{
		if (!log.verbose || (Object)(object)itemorbuild == (Object)null)
		{
			return;
		}
		ttname += "<color=#ffffff>";
		if (NetObjectRegistry.TryGetSyncInfo(itemorbuild, out var si))
		{
			ttname += $" (netID: {si.syncId})";
			ttdesc = $"{ttdesc}\nLAST SYNC: {Math.Round(Time.realtimeSinceStartupAsDouble - si.last_update_time, 1)}\nRELAX: {si.relaxed_combat_sync}";
		}
		else if (NetObjectRegistry.ObjectCanBeIgnoredForNetwork(itemorbuild.gameObject))
		{
			ttname += " (netID: IGNORED)";
		}
		else
		{
			ttname += " (netID: NONE??? BUG!!!!!)";
		}
		BodyGetterOverrider bodyGetterOverrider = default(BodyGetterOverrider);
		if ((Object)(object)itemorbuild != (Object)null && itemorbuild.TryGetComponent<BodyGetterOverrider>(ref bodyGetterOverrider))
		{
			string text = "UNKNOWN";
			Body body = bodyGetterOverrider.GetBody();
			NetBody netBody = default(NetBody);
			if (body.IsBodyLocal())
			{
				text = "LOCAL";
			}
			else if ((Object)(object)body != (Object)null && ((Component)body).TryGetComponent<NetBody>(ref netBody))
			{
				text = ((object)netBody).ToString();
			}
			ttdesc = ttdesc + "\nBODY OVR: " + text;
		}
		Item val = (Item)(object)((itemorbuild is Item) ? itemorbuild : null);
		if (val != null)
		{
			ttdesc = ttdesc + "\nitem ID: " + val.id;
		}
		BuildingEntity val2 = (BuildingEntity)(object)((itemorbuild is BuildingEntity) ? itemorbuild : null);
		if (val2 != null)
		{
			ttdesc = ttdesc + "\nbuilding ID: " + val2.id;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		if (Net.running && Net.is_client)
		{
			ServerPerformanceScale = Mathf.Clamp(Mathf.Lerp((float)SERVER_FPS, (float)SERVER_TPS, 0.5f) * Time.fixedDeltaTime, 0.01f, 1f);
		}
		else
		{
			ServerPerformanceScale = 1f;
		}
		AdaptiveSyncTimerDelta = Time.unscaledDeltaTime * ServerPerformanceScale;
		Util.GetCursorWorldPos();
		NetPlayer lOCAL_PLAYER = NetPlayer.LOCAL_PLAYER;
		if (!Net.is_connected || !((Object)(object)lOCAL_PLAYER != (Object)null))
		{
			return;
		}
		float num = (float)(Time.realtimeSinceStartupAsDouble - _last_sync_time);
		bool flag = num * ServerPerformanceScale >= 0.1f;
		lOCAL_PLAYER.camerapos = Vector2.op_Implicit(((Component)Camera.main).transform.position);
		if (Util.IsWorldGenerated())
		{
			Body body = lOCAL_PLAYER.body;
			if ((Object)(object)body != (Object)null)
			{
				if (!flag && body.alive)
				{
					bool flag2 = body.moveDir != Vector2.zero;
					Vector2Int16 vector2Int = (Vector2Int16)body.targetLookPos;
					if (flag2 || !body.standing)
					{
						flag = num >= 0.035f;
					}
					if (!flag)
					{
						if (vector2Int != last_looktarget)
						{
							flag = num >= 0.065f;
						}
						else if (last_movedir_thing == Vector2.zero)
						{
							if (flag2)
							{
								flag = true;
							}
						}
						else if (body.moveDir == Vector2.zero)
						{
							flag = true;
						}
					}
					last_movedir_thing = body.moveDir;
					last_looktarget = (Vector2Int16)body.targetLookPos;
				}
				if (flag)
				{
					Client_SendCharacterSyncPacket();
				}
			}
		}
		if (!flag)
		{
			return;
		}
		_last_sync_time = Time.realtimeSinceStartupAsDouble;
		_sendPlayerUIStateNextFrame = !_sendPlayerUIStateNextFrame;
		if (Util.IsInWoundView())
		{
			if ((Object)(object)WoundView.view.body != (Object)null && WoundView.view.body.TryGetNetBody(out var nb))
			{
				lOCAL_PLAYER.woundViewTargetNetBodyId = nb.netId;
			}
			else
			{
				lOCAL_PLAYER.woundViewTargetNetBodyId = ushort.MaxValue;
			}
		}
		else
		{
			lOCAL_PLAYER.woundViewTargetNetBodyId = 0;
		}
		if (_sendPlayerUIStateNextFrame)
		{
			NetDataWriter writer = Net.CreateWriter(10001);
			writer.Put(Vector2.op_Implicit(((Component)Camera.main).transform.position));
			writer.Put(Util.GetCursorPos());
			writer.Put(Con.IsConsoleOpen());
			writer.Put(!Application.isFocused);
			writer.Put(Chat.CHAT_textbox_input_focused);
			writer.Put(Util.IsInCraftingMenu());
			writer.Put((Object)(object)PlayerCamera.main != (Object)null && PlayerCamera.main.tradeMenu.activeSelf);
			writer.Put(lOCAL_PLAYER.woundViewTargetNetBodyId);
			Net.Client_Send((DeliveryMethod)4, in writer);
		}
	}

	public static void Client_SendCharacterSyncPacket()
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		if (!NetPlayer.TryGetLocalNetBody(out var nb))
		{
			Plugin.log.LogError((object)"BRO WTFFFFFFF IS THIS !!?!??!?!?!?!??!?!?!?!??!?!!?!?!?!??.");
			return;
		}
		ClientToServer_NetBodySyncPacket value = new ClientToServer_NetBodySyncPacket(nb);
		NetDataWriter writer = Net.CreateWriter(10002);
		writer.Put(value);
		if (value.IncludeRagData() && nb.alive)
		{
			WriteRagdollPacket(ref writer, nb.body);
		}
		writer.CompressWriter();
		Net.Client_Send((DeliveryMethod)4, in writer);
	}

	public static void WriteRagdollPacket(ref NetDataWriter writer, in Body body)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		_ = writer.Length;
		Limb[] limbs = body.limbs;
		foreach (Limb val in limbs)
		{
			writer.Put(val.rb.position);
			writer.Put(val.rb.rotation);
			writer.Put<Vector2_4byte_200>(val.rb.velocity);
			writer.Put(NewCoolerObjectPacketWriteReadSystem.QuantizeFloatToShort(val.rb.angularVelocity, _ragdangvelqua));
		}
		_ = writer.Length;
	}

	public static void ReadRagdollPacket(ref NetDataReader reader, Body body)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		bool flag = (Object)(object)body != (Object)null;
		Vector2 position = body.GetUpperTorso().GetPosition();
		Limb[] limbs = body.limbs;
		float num = default(float);
		short quantizedValue = default(short);
		foreach (Limb val in limbs)
		{
			reader.Get(out Vector2 result);
			reader.Get(ref num);
			reader.Get(out Vector2_4byte_200 result2);
			reader.Get(ref quantizedValue);
			if (flag && !val.dismembered && Vector2.SqrMagnitude(val.rb.position - result) > _ragddistmarg)
			{
				if (Net.is_server && (!num.IsFinite() || !(Vector2.SqrMagnitude(result - position) < 100f)))
				{
					flag = false;
					continue;
				}
				((Component)val).transform.position = Vector2.op_Implicit(result);
				val.rb.rotation = num;
				val.rb.velocity = result2;
				val.rb.angularVelocity = NewCoolerObjectPacketWriteReadSystem.UnquantizeFloatFromShort(quantizedValue, _ragdangvelqua);
			}
		}
	}

	private void LateUpdate()
	{
	}

	[ServerReceiver(10001)]
	private static void Server_PlayerCameraPos(knetid clientId, ref NetDataReader reader)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out Vector2 result);
		reader.Get(out Vector2 result2);
		if (!result.IsFinite() || !NetPlayer.TryGetPlayerFromClientId(clientId, out var plr))
		{
			return;
		}
		reader.Get(ref plr.is_in_cmd);
		reader.Get(ref plr.is_alttab);
		reader.Get(ref plr.is_chatting);
		reader.Get(ref plr.is_crafting);
		bool flag = default(bool);
		reader.Get(ref flag);
		reader.Get(ref plr.woundViewTargetNetBodyId);
		if (plr.woundViewTargetNetBodyId != 0)
		{
			if ((Object)(object)plr.body != (Object)null && NetBody.TryGetNetBodyFromId(plr.woundViewTargetNetBodyId, out var nb))
			{
				Component ba = (Component)(object)nb;
				Component bb = (Component)(object)plr.body;
				if (KM.dist2dsqrcheck(in ba, in bb, 30f))
				{
					goto IL_00c1;
				}
			}
			plr.woundViewTargetNetBodyId = 0;
		}
		goto IL_00c1;
		IL_00c1:
		if (result2 != plr.cursorpos)
		{
			plr.server_plrstate.last_cursormove = Time.realtimeSinceStartupAsDouble;
			plr.cursorpos = result2;
		}
		plr.is_trading = false;
		plr.camerapos = result;
		if ((Object)(object)plr.body != (Object)null)
		{
			if (plr.body.moveDir != Vector2.zero)
			{
				plr.is_alttab = false;
				plr.is_chatting = false;
			}
			if (plr.body.conscious && flag)
			{
				plr.is_trading = true;
			}
		}
	}

	[ServerReceiver(10024)]
	private static void ServerReceiver_PlayerNameRequest(knetid clientId, ref NetDataReader reader)
	{
		reader.Get(out knetid result);
		if (NetPlayer.TryGetPlayerFromClientId(result, out var plr))
		{
			plr.Server__ResponsePlayerName(clientId);
		}
	}

	public static bool ItemCanBeUsedOnThisBody(Item item, Body target)
	{
		if (CombatStuff.ItemIsUsedForAttacking(item))
		{
			return false;
		}
		if (item.Stats.wearable)
		{
			return true;
		}
		if (item.id == "dynamite")
		{
			return false;
		}
		if (item.id == "autopump")
		{
			return true;
		}
		if (item.id == "mp3player")
		{
			return false;
		}
		return true;
	}

	internal static void _PLRINT_Piggyback(NetBody target)
	{
		if (!target.IsPiggybackable() || !((double)target.body.overEncumberance < 0.5))
		{
			return;
		}
		NetBody localNetBodyNullable = NetPlayer.GetLocalNetBodyNullable();
		if ((Object)(object)localNetBodyNullable != (Object)null)
		{
			target = target.GetPiggybackStackTop();
			int num = localNetBodyNullable.CountPiggybackStackUp();
			if (target.CountPiggybackStackDown() + num + 1 <= KrokoshaScavMultiplayer.rules.PiggybackMaxStack && localNetBodyNullable.StartPiggyback(target))
			{
				KrokoshaScavMultiplayer.Client_SendSimpleMessageToServer((ushort)10031, (ushort)target.netId, true);
			}
		}
	}

	internal static void _PLRINT_Carry(NetBody target)
	{
		NetBody localNetBodyNullable = NetPlayer.GetLocalNetBodyNullable();
		if (!KrokoshaScavMultiplayer.is_client || target.StartPiggyback(localNetBodyNullable, check_distance: true))
		{
			target.SetNetIgnoreTime();
			KrokoshaScavMultiplayer.Client_SendSimpleMessageToServer((ushort)10033, (ushort)target.netId, true);
		}
	}

	internal static void _PLRINT_Inventory(NetBody target)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (KrokoshaScavMultiplayer.rules.NoInventoryLock || !target.body.conscious)
		{
			InvButton_get_body_MultiplayerPatch.focused_body = target.body;
			InvButton_get_body_MultiplayerPatch.focused_body_position_smooth = Vector2.op_Implicit(Camera.main.WorldToScreenPoint(((Component)InvButton_get_body_MultiplayerPatch.focused_body).transform.position));
			PlayerCamera.main.radialOpen = true;
		}
	}

	public static void Do_The_Modifier_Announcement(string layerPrefix, string layerDescription)
	{
		string text = Locale.GetOther("layer") + " " + (WorldGeneration.world.biomeDepth + 1) + "\n" + WorldGeneration.world.biomeTitles[WorldGeneration.world.biomeDepth];
		if (!string.IsNullOrEmpty(layerPrefix))
		{
			string text2 = "<color=\"orange\">" + layerPrefix + "</color> ";
			text = Locale.GetOther("layer") + " " + (WorldGeneration.world.biomeDepth + 1) + "\n" + text2 + WorldGeneration.world.biomeTitles[WorldGeneration.world.biomeDepth];
			string text3 = "<color=\"orange\">" + layerDescription + "</color>";
			((MonoBehaviour)PlayerCamera.main).StartCoroutine(PlayerCamera.main.DoAlertDelayed(text3, false, 6f));
		}
		PlayerCamera.main.DoAlert(text, true);
	}

	internal static void _InitializePlayerCharacter()
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)ServerMain.CHARACTER_PREFAB == (Object)null))
		{
			return;
		}
		GameObject val = GameObject.Find("Experiment");
		if ((Object)(object)val != (Object)null)
		{
			HingeJoint2D[] componentsInChildren = val.GetComponentsInChildren<HingeJoint2D>();
			foreach (HingeJoint2D val2 in componentsInChildren)
			{
				ComponentHolderProtocol.AddComponent<HingeJointState>((Object)(object)val2);
				SavedHingeJointState savedHingeJointState = ComponentHolderProtocol.AddComponent<SavedHingeJointState>((Object)(object)val2);
				savedHingeJointState.a = ((Component)val2).transform.localPosition;
				savedHingeJointState.b = ((Component)val2).transform.localRotation;
				savedHingeJointState.aa = ((Component)((Joint2D)val2).connectedBody).transform.localPosition;
				savedHingeJointState.bb = ((Component)((Joint2D)val2).connectedBody).transform.localRotation;
				savedHingeJointState.useLimits = val2.useLimits;
				JointAngleLimits2D limits = val2.limits;
				savedHingeJointState.limitsmax = ((JointAngleLimits2D)(ref limits)).max;
				limits = val2.limits;
				savedHingeJointState.limitsmin = ((JointAngleLimits2D)(ref limits)).min;
				savedHingeJointState.referenceAngle = val2.referenceAngle;
				savedHingeJointState.hinge = val2;
				savedHingeJointState.connectedBody = ((Joint2D)val2).connectedBody;
			}
			ServerMain.CHARACTER_PREFAB = Object.Instantiate<GameObject>(val);
			ServerMain.CHARACTER_PREFAB.SetActive(false);
		}
		else if (KrokoshaScavMultiplayer.network_system_is_running)
		{
			KrokoshaScavMultiplayer.DoMultiplayerStatusMessageError("PLAYER CHARACTER NOT FOUND!!!!!!!!!!!!!!!! RESTART THE GAME! RESTART !!!!!!!!!!!!!!!!!!!!");
		}
	}

	public static void RegisterClientReceiver(ushort name, KrokoshaScavMultiplayer.KrokoshaHandleNamedMessageDelegate receiver, bool ignorehost = false)
	{
		KrokoshaScavMultiplayer.all_network_receivers_from_attributes.Add(new ClientReceiverAttribute(name, ignorehost), receiver);
	}

	internal static void _RegisterClientReceivers()
	{
		foreach (KeyValuePair<KrokoshaNetworkMessageReceiverAttribute, KrokoshaScavMultiplayer.KrokoshaHandleNamedMessageDelegate> item in KrokoshaScavMultiplayer.all_network_receivers_from_attributes)
		{
			if (item.Key.GetReceiverType() != KrokoshaNetworkMessageReceiverAttribute.ReceiverType.Client)
			{
				continue;
			}
			ClientReceiverAttribute attr = (ClientReceiverAttribute)item.Key;
			Net.RegisterClientReceiver(attr.GetMessageId(), delegate(knetid clientId, ref NetDataReader reader)
			{
				if (Net.is_host && attr.ignore_host)
				{
					if (log.verbose)
					{
						KrokoshaScavMultiplayer.DoMultiplayerStatusMessageError($"DEV: I AM HOST, I CANT DO THIS SHIT: {attr.GetMessageName()}\n{new StackTrace()}");
					}
					else
					{
						log.error("DEV: I AM HOST, I CANT DO THIS SHIT: " + attr.GetMessageName());
					}
					return;
				}
				try
				{
					item.Value(clientId, ref reader);
				}
				catch (Exception arg)
				{
					Plugin.log.LogError((object)$"ERROR: Client receiver: {attr.GetMessageName()}\n{arg}");
				}
			});
		}
	}

	[ClientReceiver(10004, true)]
	private static void ClientReceiver__PlayerDamageBlockRelay(knetid _, ref NetDataReader reader)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out knetid result);
		reader.Get(out Vector2Int result2);
		float num = default(float);
		reader.Get(ref num);
		if (NetBody.TryGetNetBodyFromId(result, out var nb) && !nb.IsBodyLocal())
		{
			BlockDamage blockDamage = WorldGeneration.world.GetBlockDamage(result2);
			if (blockDamage == null)
			{
				WorldGeneration.world.DamageBlock(result2, num, true, false, false);
				return;
			}
			float num2 = num - blockDamage.damage;
			WorldGeneration.world.DamageBlock(result2, num2, !Mathf.Approximately(blockDamage.damage, num), false, false);
		}
	}

	[ClientReceiver(10005, true)]
	private static void ClientReceiver__CurrentPingOfAllPlayers(knetid _, ref NetDataReader reader)
	{
		bool flag = default(bool);
		float num = default(float);
		while (true)
		{
			reader.Get(ref flag);
			if (!flag)
			{
				break;
			}
			reader.Get(out knetid result);
			reader.Get(ref num);
			if (NetPlayer.TryGetPlayerFromClientId(result, out var plr))
			{
				plr.ping = num;
				if (plr.is_local)
				{
					LOCAL_PING = (int)(num * 1000f);
				}
			}
		}
	}

	[ClientReceiver(10006, false)]
	private static void ClientReceiver__AnnounceDoAlert(knetid _, ref NetDataReader reader)
	{
		string message = default(string);
		reader.Get(ref message);
		Lang.MsgTryTranslateIfItsLocaleKey(ref message);
		bool flag = default(bool);
		reader.Get(ref flag);
		if (Object.op_Implicit((Object)(object)PlayerCamera.main))
		{
			log.l("Received DoAlert: " + message + " ");
			PlayerCamera.main.DoAlert(message, flag);
		}
	}

	[ClientReceiver(10171, false)]
	private static void ClientReceiver__CustomLogMessage(knetid _, ref NetDataReader reader)
	{
		string msg = default(string);
		reader.Get(ref msg);
		bool flag = default(bool);
		reader.Get(ref flag);
		if (flag)
		{
			KrokoshaScavMultiplayer.DoMultiplayerStatusMessageError(msg);
		}
		else
		{
			KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog(msg);
		}
	}

	[ClientReceiver(10007, true)]
	private static void ClientReceiver__SetBlock(knetid _, ref NetDataReader reader)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)WorldGeneration.world == (Object)null || WorldChunkSync.instantiatingWorld)
		{
			return;
		}
		reader.Get(out Vector2Int result);
		ushort num = default(ushort);
		reader.Get(ref num);
		bool flag = default(bool);
		reader.Get(ref flag);
		if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
		{
			DebugHelp.OnNetEvent(WorldGeneration.world.BlockToWorldPos(result), $"C: SetBlock {num} {flag} ");
		}
		if (flag)
		{
			WorldGeneration.world.SetBlockNoUpdate(result, num);
		}
		else
		{
			WorldGeneration.world.SetBlock(result, num);
		}
		if (num == 0)
		{
			BlockDamage blockDamage = WorldGeneration.world.GetBlockDamage(result);
			if (blockDamage != null)
			{
				blockDamage.DestroySprite();
				WorldGeneration.world.blockDamages.Remove(blockDamage);
			}
		}
	}

	[ClientReceiver(10008, true)]
	private static void ClientReceiver__CreateExplosionEffect(knetid _, ref NetDataReader reader)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Expected O, but got Unknown
		reader.Get(out Vector2 result);
		float velocity = default(float);
		reader.Get(ref velocity);
		float range = default(float);
		reader.Get(ref range);
		WorldGeneration_CreateExplosion_MultiplayerPatch.ForceCreateExplosionEffect(new ExplosionParams
		{
			position = result,
			velocity = velocity,
			range = range
		});
	}

	[ClientReceiver(10009, true)]
	private static void ClientReceiver__PlayerJumpRelay(knetid _, ref NetDataReader reader)
	{
		reader.Get(out knetid result);
		if (NetPlayer.TryGetNetPlayerAndNetBodyFromClientId(result, out var _, out var pb))
		{
			pb.body.Jump();
		}
	}

	[ClientReceiver(10178, true)]
	private static void ClientReceiver__forcespectator(knetid _, ref NetDataReader reader)
	{
		if (!UIInGame.SPECTATOR_MODE)
		{
			log.l("CLIENT: received command from server to enable spectator mode");
			UIInGame.StartSpectatorMode();
		}
	}

	[ClientReceiver(10185, true)]
	private static void ClientReceiver__ClientCustomCommand(knetid _, ref NetDataReader reader)
	{
		RunCustomCommand(reader.GetString(), reader);
	}

	internal static void RunCustomCommand(string com, NetDataReader reader)
	{
		if (ClientCustomCommandsDict.TryGetValue(com, out var value))
		{
			try
			{
				value(com, reader);
			}
			catch (Exception ex)
			{
				log.error(ex.Message);
			}
		}
	}

	[ClientReceiver(10011, true)]
	private static void ClientReceiver__GoBackToMainMenu(knetid _, ref NetDataReader reader)
	{
		KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog("Received announcement to go to main menu.");
		KrokoshaScavMultiplayer.showMultiplayerMenu = true;
		PlayerCamera.main.ToMainMenu();
	}

	[ClientReceiver(10012, true)]
	private static void ClientReceiver__PlayerPointFingerAtRelay(knetid _, ref NetDataReader reader)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out knetid result);
		reader.Get(out Vector2 result2);
		byte type = default(byte);
		reader.Get(ref type);
		if (NetPlayer.TryGetPlayerFromClientId(result, out var plr))
		{
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(result2, $"C: PlayerPointFingerAtRelay {plr}");
			}
			plr.PointFingerAt(result2, type);
		}
	}

	[ClientReceiver(10014, true)]
	private static void ClientReceiver__CurrentWorldStateSync(knetid _, ref NetDataReader reader)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out SerializableRandomState result);
		bool flag = default(bool);
		reader.Get(ref flag);
		server_is_generating_world = !flag;
		Random.state = result.State;
	}

	[ClientReceiver(10015, true)]
	private static void ClientReceiver__Announce_LayerModifier(knetid _, ref NetDataReader reader)
	{
		ushort num = default(ushort);
		reader.Get(ref num);
		LayerModifier lm = LayerModifier.availableModifiers[num];
		Plugin.log.LogInfo((object)string.Format("CLIENT: Received layer modifier: {0}  index: {1}", "lm", lm.modifierIndex));
		Util.CallLambdaWhenWorldGenerates((Action)delegate
		{
			lm.Initialize(WorldGeneration.world);
			lm.active = true;
			if (log.verbose)
			{
				log.l($"Applied layer modifier {((object)lm).GetType()}");
			}
			Body body = PlayerCamera.main.body;
			string other = Locale.GetOther("layermodifier" + lm.modifierIndex);
			string other2 = Locale.GetOther("layermodifier" + lm.modifierIndex + "dsc");
			WorldGeneration.world.layerPrefix = other;
			WorldGeneration.world.layerDescription = other2;
			Do_The_Modifier_Announcement(other, other2);
		});
	}

	[ClientReceiver(10016, true)]
	private static void ClientReceiver__Announce_RegenerateWorld(knetid _, ref NetDataReader reader)
	{
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		ushort num = default(ushort);
		reader.Get(ref num);
		WorldGeneration world = WorldGeneration.world;
		if (KrokoshaScavMultiplayer.is_client && Object.op_Implicit((Object)(object)world))
		{
			if (world.generatingWorld)
			{
				((MonoBehaviour)world).StopCoroutine("GenerateWorld");
				world.generatingWorld = false;
				KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog("Received RegenerateWorld announcement. (CANCELLING GenerateWorld !!!)");
			}
			else
			{
				KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog("Received RegenerateWorld announcement.");
			}
			PlayerCamera.main.body.forceWalk = false;
			world.savePanel.SetActive(false);
			bool flag = num == 1;
			world.ResetLayerModifiers();
			world.doPod = flag;
			((MonoBehaviour)world).StartCoroutine("RegenerateWorld", (object)flag);
			if (flag)
			{
				Sound.Play("drillpoduse", Vector2.zero, true, false, (Transform)null, 1f, 1f, true, false);
			}
		}
	}

	[ClientReceiver(10181, false)]
	private static void ClientReceiver__CreateInstance(uint _, ref NetDataReader reader)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out Vector2 result);
		float num = default(float);
		reader.Get(ref num);
		reader.Get(out var result2, oneByteChars: true);
		if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
		{
			DebugHelp.OnNetEvent(result, "C: CreateInstance " + result2);
		}
		Utils.Create(result2, result, num);
	}

	[ClientReceiver(10017, false)]
	private static void ClientReceiver__PlayThisSoundHere(knetid _, ref NetDataReader reader)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out Vector2 result);
		reader.Get(out var result2, oneByteChars: true);
		if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
		{
			DebugHelp.OnNetEvent(result, "C: PlayThisSoundHere " + result2);
		}
		Sound.Play(result2, result, false, true, (Transform)null, 1f, 1f, false, false);
	}

	[ClientReceiver(10172, false)]
	private static void ClientReceiver__ServerInfoUpdate(knetid _, ref NetDataReader reader)
	{
		Net.cur_server_info.Deserialize(reader);
	}

	[ClientReceiver(10018, true)]
	private static void ClientReceiver__PlayerBarkRelay(knetid _, ref NetDataReader reader)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out knetid result);
		if (NetPlayer.TryGetNetPlayerAndBodyFromClientId(result, out var plr, out var body) && !plr.is_local)
		{
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(plr.pos, "C: Bark");
			}
			body.eatTime = 0f;
			((Component)body).GetComponent<PantSound>().Bark();
			if (body.eatTime == 0.5f)
			{
				body.eatTime = 0.45f;
			}
		}
	}

	[ClientReceiver(10019, true)]
	private static void ClientReceiver__SingleCharacterPositionsSync(knetid _, ref NetDataReader reader)
	{
		reader.Get(out knetid result);
		reader.Get(out NetBodySyncPacket result2);
		if (!NetBody.TryGetNetBodyFromId(result, out var nb))
		{
			return;
		}
		if (nb.is_player && nb.is_local)
		{
			if (Util.IsGeneratingWorld() || !Util.IsInWorld())
			{
				_last_reminderpack_while_generating_received = true;
			}
			_last_reminderpack_while_generating = result2;
			nb.OnReceiveSyncPacket(in result2, force: true);
		}
		else
		{
			nb.OnReceiveSyncPacket(in result2);
		}
	}

	[ClientReceiver(10179, true)]
	private static void ClientReceiver__ragdollpack(knetid _, ref NetDataReader reader)
	{
		reader.Get(out knetid result);
		if (NetBody.TryGetNetBodyFromId(result, out var nb))
		{
			reader = reader.DecompressReader();
			ReadRagdollPacket(ref reader, nb.body);
		}
	}

	[ClientReceiver(10010, true)]
	private static void ClientReceiver__GenerateWorld_SeedAnnounce(knetid _, ref NetDataReader reader)
	{
		reader.Get(out LastBeforeGenerationState result);
		KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog("GenerateWorld_SeedAnnounce, received worldgen seed.");
		WorldGeneration_GenerateWorld_MultiplayerPatch.client_firstworldgenparams_are_used = false;
		WorldGeneration_GenerateWorld_MultiplayerPatch.firstworldgenparams = result;
	}

	[ClientReceiver(10021, true)]
	private static void ClientReceiver_StartGameAnnouncement(knetid _, ref NetDataReader reader)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out ServerMain.StartGameAnnouncementPacket result);
		reader.Get(out var result2, oneByteChars: true);
		KrokoshaScavMultiplayer.rules = result.rules;
		KrokoshaScavMultiplayer.ApplyGameRules();
		WorldGeneration_GenerateWorld_MultiplayerPatch.firstworldgenparams.randomstate = result.randomstate;
		WorldGeneration_GenerateWorld_MultiplayerPatch.client_firstworldgenparams_are_used = true;
		Random.state = result.randomstate;
		try
		{
			WorldgenPatches.ReadRunSettings(result2);
			result.prefs.ApplyPrefs();
		}
		catch (Exception ex)
		{
			log.error(ex.ToString());
		}
		SaveSystem.loadedRun = false;
		PreRunScript val = Object.FindObjectOfType<PreRunScript>();
		if ((Object)(object)val != (Object)null)
		{
			KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog("### STARTING GAME (RECEIVED ANNOUNCEMENT FROM SERVER) ###");
			ScavGameStartPatch.starting_game = true;
			((MonoBehaviour)val).StartCoroutine(val.WaitLoad());
			KrokoshaScavMultiplayer.showMultiplayerMenu = false;
		}
		else if (Util.IsInWorld())
		{
			log.warn("Received StartGameAnnouncement but im in the world already... bruh");
		}
		else
		{
			log.warn("Received StartGameAnnouncement but theres no PreRunScript, what???");
		}
	}

	[ClientReceiver(10020, true)]
	private static void ClientReceiver__RunSettingsSync(knetid _, ref NetDataReader reader)
	{
		SaveSystem.loadedRun = false;
		reader.Get(out WorldgenPatches.RunPrefs result);
		reader.Get(out var result2, oneByteChars: true);
		result.ApplyPrefs(abide_rules: true, apply_tutorial: false);
		PreRunScript val = Object.FindObjectOfType<PreRunScript>();
		if (Object.op_Implicit((Object)(object)val))
		{
			val.UpdateAllSettingDisplays();
		}
		WorldgenPatches.ReadRunSettings(result2);
	}

	[ClientReceiver(10022, true)]
	private static void ClientReceiver__SetTimeScaleRelay(knetid _, ref NetDataReader reader)
	{
		ushort num = default(ushort);
		reader.Get(ref num);
		if ((Object)(object)PlayerCamera.main != (Object)null && !KrokoshaScavMultiplayer.rules.DisableTimeManipulation)
		{
			log.l("SetTimeScale: Received SetTimeScale from server.");
			PlayerCameraSetTimeScalePatch.force = true;
			PlayerCamera.main.SetTimeScale((SpeedType)num, true, false);
		}
	}
}
