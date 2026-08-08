using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KrokoshaCasualtiesUtils;
using LiteNetLib;
using LiteNetLib.Utils;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public class ServerMain : MonoBehaviour
{
	public struct StartGameAnnouncementPacket : INetSerializeByMemcpy
	{
		public bool isreal;

		public WorldgenPatches.RunPrefs prefs;

		public State randomstate;

		public KrokoshaMultiplayerGameRules rules;
	}

	public static Dictionary<string, PlayerSavedState> server_lastplayerstates = new Dictionary<string, PlayerSavedState>();

	public static float AVERAGE_PING = 0f;

	public static int CURRENT_TPS = 0;

	public static int CURRENT_FPS = 0;

	public static byte AVG_CONNECTION_QUALITY = 0;

	internal static StartGameAnnouncementPacket LAST_STARTGAME_ANNOUNCEMENT_PACKET = new StartGameAnnouncementPacket
	{
		isreal = false,
		prefs = default(WorldgenPatches.RunPrefs).ReadPrefs(),
		randomstate = Random.state,
		rules = KrokoshaScavMultiplayer.rules
	};

	public static bool _DEV_ENABLE_HP_SYNC = true;

	public static GameObject CHARACTER_PREFAB = null;

	private static List<knetid> _real_AllClientIds = new List<knetid>();

	private static List<knetid> _real_AllClientIdsExceptHost = new List<knetid>();

	private static List<NetPlayer> _real_AllPlayersExceptHost = new List<NetPlayer>();

	private double tpscalc_lasttime;

	private int tpscalc_framecounter;

	private double fpscalc_lasttime;

	private int fpscalc_framecounter;

	public const float SyncFrequency = 0.09f;

	public const float SyncFrequencyRare = 5f;

	public const float SyncFrequencySecond = 1f;

	internal static float timer_UpdateSyncClientsToClients = 0f;

	internal static float timer_RareUpdateSyncClients = 0f;

	internal static float timer_SecondUpdateSyncClients = 1f;

	public static float _ded_server_switch_counter = 0f;

	internal static Dictionary<string, Action<NetPlayer, string, string[]>> ServerClientCustomCommandsDict = new Dictionary<string, Action<NetPlayer, string, string[]>>();

	public static Body CHARACTER_PREFAB_BODY => ((Component)CHARACTER_PREFAB.transform.GetChild(0)).GetComponent<Body>();

	public static IReadOnlyList<knetid> AllClientIds => _real_AllClientIds;

	public static IReadOnlyList<knetid> AllClientIdsExceptHost => _real_AllClientIdsExceptHost;

	public static IReadOnlyList<NetPlayer> AllPlayersExceptHost => _real_AllPlayersExceptHost;

	public static void Server_Announce_GAME_START()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		WorldgenPatches.RunPrefs prefs = default(WorldgenPatches.RunPrefs).ReadPrefs();
		NetDataWriter writer = Net.CreateWriter(10021);
		LAST_STARTGAME_ANNOUNCEMENT_PACKET = new StartGameAnnouncementPacket
		{
			isreal = true,
			prefs = prefs,
			randomstate = Random.state,
			rules = KrokoshaScavMultiplayer.rules
		};
		writer.Put(LAST_STARTGAME_ANNOUNCEMENT_PACKET);
		writer.Put(WorldgenPatches.CompileRunSettings(), oneByteChars: true);
		if (prefs.tutorial)
		{
			Chat.Server_ChatAnnouncement("Host is starting the tutorial.");
		}
		else
		{
			Chat.Server_ChatAnnouncement("Host is starting game.");
		}
		KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog("### STARTING GAME ###");
		Net.Server_SendToClientsVeryReliable(in writer, (IEnumerable<knetid>)AllClientIdsExceptHost);
	}

	public static void Server_SendMPLogMessage(in string msg, in bool is_error = false, IReadOnlyList<knetid> targets = null)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		NetDataWriter writer = Net.CreateWriter(10171);
		writer.Put(msg);
		writer.Put(is_error);
		DeliveryMethod delivery = (DeliveryMethod)2;
		IEnumerable<knetid> clientIds = targets ?? AllClientIds;
		Net.Server_SendToClients(in delivery, in writer, in clientIds);
	}

	public static void _Server_OnServerNameChange(IReadOnlyList<knetid> targets = null)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		NetDataWriter writer = Net.CreateWriter(10172);
		writer.Put<NetPublicServerInfo>(Net.MY_SERVER_INFO);
		DeliveryMethod delivery = (DeliveryMethod)2;
		IEnumerable<knetid> clientIds = targets ?? AllClientIdsExceptHost;
		Net.Server_SendToClients(in delivery, in writer, in clientIds);
		if (Net.is_playing_with_steam)
		{
			KSteam.Server_UpdateLobbyData();
		}
	}

	public static void Server_InstantiateInstance(Vector2 pos, float rot, string name, IReadOnlyList<knetid> to_who)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		NetDataWriter writer = Net.CreateWriter(10181);
		writer.Put(pos);
		writer.Put(rot);
		writer.Put(name, oneByteChars: true);
		DeliveryMethod delivery = (DeliveryMethod)0;
		IEnumerable<knetid> clientIds = to_who;
		Net.Server_SendToClients(in delivery, in writer, in clientIds);
	}

	public static void Server_AnnounceAlert(in string msg, bool important, bool reliable = true, IReadOnlyList<knetid> targets = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (KrokoshaScavMultiplayer.is_dedicated_server)
		{
			Util.DoAlert(in msg, in important);
		}
		NetDataWriter writer = Net.CreateWriter(10006);
		writer.Put(msg);
		writer.Put(important);
		Net.Server_SendToClients((DeliveryMethod)((!reliable) ? 4 : 0), in writer, (IEnumerable<knetid>)(targets ?? AllClientIds));
	}

	public static bool ForceTalkerSayAndAnnounce(in SyncInfo si, in string msg, IReadOnlyList<knetid> targets = null)
	{
		if (Util.TryGetTalkerOnObject(si.go, out var talker, out var _))
		{
			talker.ForceNoSpeechImpairment(msg, resetTalkTimer: true);
			return TalkerSayAnnounce(in si.syncId, (byte)0, in msg, targets);
		}
		return false;
	}

	public static bool ForceTalkerSayAndAnnounce(in NetBody pb, in string msg, IReadOnlyList<knetid> targets = null)
	{
		if (Util.TryGetTalkerOnObject(((Component)pb).gameObject, out var talker, out var _))
		{
			talker.ForceNoSpeechImpairment(msg, resetTalkTimer: true);
			return TalkerSayAnnounce(in pb.netId, (byte)1, in msg, targets);
		}
		return false;
	}

	public static bool TalkerSayAnnounce(in knetid id, in byte type, in string msg, IReadOnlyList<knetid> targets = null)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		NetDataWriter writer = Net.CreateWriter(10030);
		writer.Put((ushort)id);
		writer.Put(type);
		writer.Put(msg);
		DeliveryMethod delivery = (DeliveryMethod)0;
		IEnumerable<knetid> clientIds = targets ?? AllClientIds;
		Net.Server_SendToClients(in delivery, in writer, in clientIds);
		return true;
	}

	public static IEnumerator HeyPlayerJustJoinedGiveHimASpawnLocationOkay(knetid clientId)
	{
		NetPlayer plr;
		NetBody pb;
		while (!NetPlayer.TryGetNetPlayerAndNetBodyFromClientId(clientId, out plr, out pb) || !Util.IsWorldGenerated() || !Body_PlaceBody_MultiplayerPatch.has_spawn_location)
		{
			if ((Object)(object)plr != (Object)null)
			{
				plr.ResetEntropy();
			}
			yield return null;
		}
		if (!((Object)(object)plr != (Object)null))
		{
			yield break;
		}
		plr.ResetEntropy();
		if (pb.body.alive || !KrokoshaScavMultiplayer.rules.LateJoinSpectate)
		{
			if (plr.server_plrstate.did_give_spawn_location_from_a_save)
			{
				plr.server_plrstate.did_give_spawn_location_from_a_save = false;
			}
			else if (!KrokoshaScavMultiplayer.rules.LayerFinishKeepXOffset || WorldGeneration.world.totalTraveled <= 0 || plr.late_joined)
			{
				LateSpawnLocation(pb);
			}
			else
			{
				pb.SetBodyPosition(Util.PlaceBody_FindSpawnLocation(plr.server_plrstate.layer_transition_x_offset));
			}
		}
		if (!pb.IsBodyLocal())
		{
			log.l($"SERVER: Sending a spawn location for {pb} to {((Component)pb).transform.position} ");
			pb.Server_RemindPlayersCurrentState();
		}
		plr.server_plrstate.SaveLocationSnapshot();
		WorldChunkSync.singleton.timer_TilemapSync = 10f;
	}

	public static int GetAlivePlayerCountPercent(float scale)
	{
		int count = NetPlayer.AllLivingPlayers.Count;
		if (count == 0)
		{
			return 1;
		}
		return Mathf.Clamp(Mathf.CeilToInt((float)count * scale), 1, count);
	}

	public static int GetNumberOfPlayersRequiredToFinishLayer()
	{
		return GetAlivePlayerCountPercent((float)(int)KrokoshaScavMultiplayer.rules.LayerFinishPlrPercent * 0.01f);
	}

	public static bool CheckIfEnoughPeopleAreAtLayerFinish()
	{
		int current;
		int required;
		return CheckIfEnoughPeopleAreAtLayerFinish(out current, out required);
	}

	public static bool CheckIfEnoughPeopleAreAtLayerFinish(out int current, out int required)
	{
		if (KrokoshaScavMultiplayer.rules.CheckIfLayerContinueIsDisabled())
		{
			current = 0;
			required = 1000;
			return false;
		}
		return CheckIfCanFinishLayerCustomCondition((NetPlayer plr) => plr.pos.y < (float)(0L - (long)WorldGeneration.world.halfHeight) + 3.1f, out current, out required);
	}

	public static bool CheckIfShouldStartRadlineForStragglers(byte minimum = 1)
	{
		if (KrokoshaScavMultiplayer.rules.StragglerRadlinePercent < 100)
		{
			CheckIfEnoughPeopleAreAtLayerFinish(out var current, out var required);
			int num = 0;
			if (NetPlayer.AllLivingPlayers.Count > 1 && required > 0 && current != NetPlayer.AllLivingPlayers.Count && current >= minimum)
			{
				num = GetAlivePlayerCountPercent((float)(int)KrokoshaScavMultiplayer.rules.StragglerRadlinePercent * 0.01f);
				if (num > 0 && current >= num)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool CheckIfCanFinishLayerCustomCondition(Func<NetPlayer, bool> func, out int current, out int required)
	{
		required = GetNumberOfPlayersRequiredToFinishLayer();
		current = 0;
		foreach (NetPlayer allLivingPlayer in NetPlayer.AllLivingPlayers)
		{
			if ((!Net.is_server || allLivingPlayer.server_plrstate.is_loaded_in) && func(allLivingPlayer))
			{
				current++;
				if (current >= required)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static void LateSpawnLocation(NetBody b)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		NetBody playerBodyHostOrAnyoneAlive = GetPlayerBodyHostOrAnyoneAlive(b.plr, b);
		if ((Object)(object)playerBodyHostOrAnyoneAlive != (Object)null && playerBodyHostOrAnyoneAlive.body.alive && IsInWorld(playerBodyHostOrAnyoneAlive.pos))
		{
			b.SetBodyPosition(playerBodyHostOrAnyoneAlive.pos);
			return;
		}
		if ((Object)(object)RadiationLine.line != (Object)null)
		{
			Body_PlaceBody_MultiplayerPatch.spawnlocation.y = Mathf.Min(Body_PlaceBody_MultiplayerPatch.spawnlocation.y, ((Component)RadiationLine.line).transform.position.y);
		}
		((Component)b).transform.position = Vector2.op_Implicit(Body_PlaceBody_MultiplayerPatch.spawnlocation);
	}

	public static bool IsInWorld(in Vector2 pos)
	{
		if (Mathf.Abs(pos.x) < 512f)
		{
			return Mathf.Abs(pos.y) < 512f;
		}
		return false;
	}

	public static NetBody GetPlayerBodyHostOrAnyoneAlive(NetPlayer for_who = null, NetBody except = null)
	{
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		NetBody netBody = null;
		NetBody netBody2 = null;
		if (!KrokoshaScavMultiplayer.is_dedicated_server)
		{
			Body localBody = Util.GetLocalBody();
			netBody = ((localBody != null) ? ((Component)localBody).GetComponent<NetBody>() : null);
		}
		if ((Object)(object)netBody == (Object)null || !netBody.body.conscious)
		{
			foreach (KeyValuePair<Body, NetPlayer> item in NetPlayer.BodyToPlayerDict)
			{
				if (item.Key.conscious && (Object)(object)except != (Object)(object)item.Value.playerbody && (Object)(object)for_who != (Object)(object)item.Value && IsInWorld(Vector2.op_Implicit(((Component)item.Key).transform.position)) && item.Value.server_plrstate.did_give_spawn_location && item.Value.server_plrstate.is_loaded_in && !((Object)(object)item.Key == (Object)(object)Util.GetLocalBody()))
				{
					if (KrokoshaScavMultiplayer.rules.Teams && (Object)(object)for_who != (Object)null && item.Value.IsInSameTeamAs(for_who))
					{
						netBody2 = item.Value.playerbody;
					}
					return item.Value.playerbody;
				}
			}
		}
		if ((Object)(object)netBody2 != (Object)null)
		{
			return netBody2;
		}
		return netBody;
	}

	public static bool CheckIfEveryoneAliveIsActuallyConscious()
	{
		foreach (KeyValuePair<Body, NetPlayer> item in NetPlayer.BodyToPlayerDict)
		{
			if (item.Key.alive && !item.Key.conscious)
			{
				return false;
			}
		}
		return true;
	}

	public static List<knetid> GetListOfClientIdsExceptThis(knetid exclude)
	{
		List<knetid> list = new List<knetid>();
		foreach (NetPlayer value in NetPlayer.ClientIdToPlayerDict.Values)
		{
			NetPlayer component = ((Component)value).GetComponent<NetPlayer>();
			if ((Object)(object)component != (Object)null && (ushort)exclude != (ushort)component.clientId)
			{
				list.Add(component.clientId);
			}
		}
		return list;
	}

	public static List<knetid> GetListOfClientIdsExceptThisAndHost(knetid exclude)
	{
		List<knetid> list = new List<knetid>(_real_AllClientIdsExceptHost);
		list.Remove(exclude);
		return list;
	}

	public static List<knetid> GetListOfClientIdsExceptThisAndHost(NetBody nb)
	{
		List<knetid> list = new List<knetid>(_real_AllClientIdsExceptHost);
		if (nb.is_player)
		{
			list.Remove(nb.player.clientId);
		}
		return list;
	}

	public static bool CheckIfAnyoneIsMoving()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		foreach (NetPlayer allLivingPlayer in NetPlayer.AllLivingPlayers)
		{
			if (allLivingPlayer.body.conscious && allLivingPlayer.body.moveDir != Vector2.zero)
			{
				return true;
			}
		}
		return false;
	}

	public static bool CheckIfAnyoneFinishedLayer(out NetPlayer who)
	{
		foreach (KeyValuePair<Body, NetPlayer> item in NetPlayer.BodyToPlayerDict)
		{
			if (item.Key.alive && item.Key.conscious && DidHeFinishTheLayer(item.Key))
			{
				who = item.Value;
				return true;
			}
		}
		who = null;
		return false;
	}

	public static bool DidHeFinishTheLayer(Body body)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		if (((Component)body).transform.position.y < (float)(0L - (long)WorldGeneration.world.halfHeight) + 3.1f)
		{
			return true;
		}
		return false;
	}

	public static bool CheckIfEveryoneIsSleeping()
	{
		bool result = false;
		foreach (KeyValuePair<Body, NetPlayer> item in NetPlayer.BodyToPlayerDict)
		{
			if (item.Key.alive)
			{
				if (item.Key.consciousness > 20f)
				{
					return false;
				}
				result = true;
			}
		}
		return result;
	}

	public static void ResetPlayersFinishedStatus()
	{
		foreach (NetPlayer value in NetPlayer.ClientIdToPlayerDict.Values)
		{
			value._tutorial_finished = false;
			value.finishedLayer = false;
		}
	}

	public static string GetPlayerFullDebugString(Body body)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)body == (Object)null)
		{
			return "BODY(NULL)";
		}
		if (body.TryGetNetBody(out var nb))
		{
			return ((object)nb).ToString();
		}
		return $"BODY({((Object)body).name}, pos: {Vector2.op_Implicit(((Component)body).transform.position)})";
	}

	public static string GetPlayerFullDebugString(knetid cid)
	{
		if (NetPlayer.TryGetPlayerFromClientId(cid, out var plr))
		{
			return ((object)plr).ToString();
		}
		return $"PLR(ID:{cid}, UNKNOWN)";
	}

	public static bool TryGetPlayerFromPartialName(string partialname, out NetPlayer player, bool body_required = false)
	{
		if (partialname.StartsWith("id:") && ushort.TryParse(partialname.Substring(3), out var result) && NetPlayer.TryGetPlayerFromClientId(result, out player) && (!body_required || (Object)(object)player.body != (Object)null))
		{
			return true;
		}
		foreach (NetPlayer value in NetPlayer.ClientIdToPlayerDict.Values)
		{
			if (value.playername == partialname && (!body_required || (Object)(object)value.body != (Object)null))
			{
				player = value;
				return true;
			}
		}
		foreach (NetPlayer value2 in NetPlayer.ClientIdToPlayerDict.Values)
		{
			if (value2.playername.StartsWith(partialname, StringComparison.OrdinalIgnoreCase) && (!body_required || (Object)(object)value2.body != (Object)null))
			{
				player = value2;
				return true;
			}
		}
		player = null;
		return false;
	}

	public static NetBody GetBodyForCommandOnCursor(Vector2 cursorpos, bool only_players = false)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		Body bodyOnPos = Util.GetBodyOnPos(cursorpos);
		NetBody netBody = ((bodyOnPos != null) ? ((Component)bodyOnPos).GetComponent<NetBody>() : null);
		(NetBody, float) nearestBody = NetBody.GetNearestBody(cursorpos);
		if ((Object)(object)netBody == (Object)null)
		{
			(netBody, _) = nearestBody;
		}
		if ((Object)(object)netBody != (Object)null)
		{
			if (only_players && !netBody.is_player)
			{
				return null;
			}
			return netBody;
		}
		return null;
	}

	public static NetBody RelaxedGetBodyForCommand(string plrname_or_macro, bool allow_macros = true, bool only_players = false, NetBody nbCaller = null, bool randomExcludeCaller = false)
	{
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		if (string.IsNullOrEmpty(plrname_or_macro))
		{
			return null;
		}
		string text = plrname_or_macro.ToLower();
		if (StringUtility.StartsWith(plrname_or_macro, '@') && (allow_macros || text == "@c"))
		{
			if (text.StartsWith("@m") && (Object)(object)NetPlayer.LOCAL_PLAYER != (Object)null)
			{
				return NetPlayer.LOCAL_PLAYER?.playerbody;
			}
			if (text.StartsWith("@r"))
			{
				if (only_players)
				{
					List<NetBody> list = NetPlayer.BodyToPlayerDict.Keys.Select((Body x) => ((Component)x).GetComponent<NetBody>()).ToList();
					if (randomExcludeCaller && (Object)(object)nbCaller != (Object)null)
					{
						list.Remove(nbCaller);
					}
					if (list.Count == 0)
					{
						return null;
					}
					NetBody netBody = list[Random.Range(0, list.Count)];
					if (netBody == null)
					{
						return null;
					}
					return ((Component)netBody).GetComponent<NetBody>();
				}
				List<NetBody> list2 = Object.FindObjectsOfType<NetBody>().ToList();
				if (randomExcludeCaller && (Object)(object)nbCaller != (Object)null)
				{
					list2.Remove(nbCaller);
				}
				if (list2.Count == 0)
				{
					return null;
				}
				return list2[Random.Range(0, list2.Count)];
			}
			if (text.StartsWith("@c"))
			{
				return GetBodyForCommandOnCursor(Util.GetCursorWorldPos());
			}
		}
		else
		{
			if (TryGetPlayerFromPartialName(plrname_or_macro, out var player, body_required: true))
			{
				return player.playerbody;
			}
			if (!only_players)
			{
				foreach (NetBody all_instance in NetBody.all_instances)
				{
					if (all_instance.bodyname == plrname_or_macro)
					{
						return all_instance;
					}
				}
				foreach (NetBody all_instance2 in NetBody.all_instances)
				{
					if (all_instance2.bodyname.StartsWith(plrname_or_macro, StringComparison.OrdinalIgnoreCase))
					{
						return all_instance2;
					}
				}
			}
		}
		return null;
	}

	public static Tuple<bool, string> _PerformActionOnPlayersByName(string plrname, Action<NetPlayer> func, bool allow_macros = true, NetPlayer callerPlr = null, bool require_body = false, bool randomExcludeCaller = false, bool onlyOne = false)
	{
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		if (string.IsNullOrEmpty(plrname))
		{
			return null;
		}
		string text = plrname.ToLower();
		if (StringUtility.StartsWith(plrname, '@') && (allow_macros || text == "@c"))
		{
			if (!onlyOne && text == "@a")
			{
				foreach (NetPlayer value in NetPlayer.ClientIdToPlayerDict.Values)
				{
					if (!require_body || !((Object)(object)value.body == (Object)null))
					{
						func(value);
					}
				}
				return Tuple.Create(item1: true, "Success");
			}
			if (!onlyOne && text == "@o")
			{
				foreach (NetPlayer value2 in NetPlayer.ClientIdToPlayerDict.Values)
				{
					if ((!require_body || !((Object)(object)value2.body == (Object)null)) && (!((Object)(object)callerPlr != (Object)null) || !((Object)(object)callerPlr == (Object)(object)value2)))
					{
						func(value2);
					}
				}
				return Tuple.Create(item1: true, "Success");
			}
			if (text.StartsWith("@m") && (Object)(object)NetPlayer.LOCAL_PLAYER != (Object)null && (!require_body || (Object)(object)NetPlayer.LOCAL_PLAYER.body != (Object)null))
			{
				func(NetPlayer.LOCAL_PLAYER);
				return Tuple.Create(item1: true, "Success");
			}
			if (text.StartsWith("@r"))
			{
				NetPlayer netPlayer = null;
				List<NetPlayer> list = new List<NetPlayer>(NetPlayer.ClientIdToPlayerDict.Values).ToList();
				if (randomExcludeCaller && (Object)(object)callerPlr != (Object)null)
				{
					list.Remove(callerPlr);
				}
				if (list.Count > 0)
				{
					netPlayer = list[Random.Range(0, list.Count)];
				}
				if ((Object)(object)netPlayer != (Object)null)
				{
					func(netPlayer);
					return Tuple.Create(item1: true, "Success");
				}
				return Tuple.Create(item1: false, "Nothing was found.");
			}
			if (text.StartsWith("@c"))
			{
				Body bodyOnPos = Util.GetBodyOnPos(Util.GetCursorWorldPos());
				NetBody netBody = ((bodyOnPos != null) ? ((Component)bodyOnPos).GetComponent<NetBody>() : null);
				if ((Object)(object)netBody == (Object)null || !netBody.is_player)
				{
					netBody = NetBody.GetNearestBody(Util.GetCursorWorldPos()).Item1;
				}
				if ((Object)(object)netBody != (Object)null)
				{
					if (!netBody.is_player)
					{
						return Tuple.Create(item1: false, "Body is not player.");
					}
					func.DynamicInvoke(netBody.plr);
					return Tuple.Create(item1: true, "Success");
				}
				return Tuple.Create(item1: false, "Nobody was found at cursor.");
			}
			return Tuple.Create(item1: false, "Unknown macro.");
		}
		if (TryGetPlayerFromPartialName(plrname, out var player))
		{
			func.DynamicInvoke(player);
			return Tuple.Create(item1: true, "Success");
		}
		return Tuple.Create(item1: false, "Player not found");
	}

	public static Tuple<bool, string> _PerformActionOnBodiesByName(string plrname, Action<NetBody> func, bool allow_macros = true, bool only_players = true, NetBody nb_to_exclude_for_random = null)
	{
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		if (string.IsNullOrEmpty(plrname))
		{
			return null;
		}
		if (StringUtility.StartsWith(plrname, '@') && (allow_macros || plrname == "@c"))
		{
			if (plrname == "@a")
			{
				foreach (NetPlayer value in NetPlayer.BodyToPlayerDict.Values)
				{
					func(value.playerbody);
				}
				return Tuple.Create(item1: true, "Success");
			}
			if (plrname.StartsWith("@m") && (Object)(object)NetPlayer.LOCAL_PLAYER != (Object)null && (Object)(object)NetPlayer.LOCAL_PLAYER.body != (Object)null)
			{
				func(NetPlayer.LOCAL_PLAYER.playerbody);
				return Tuple.Create(item1: true, "Success");
			}
			if (plrname.StartsWith("@r"))
			{
				NetBody netBody = null;
				if (only_players)
				{
					List<NetBody> list = NetPlayer.BodyToPlayerDict.Keys.Select((Body x) => ((Component)x).GetComponent<NetBody>()).ToList();
					if ((Object)(object)nb_to_exclude_for_random != (Object)null)
					{
						list.Remove(nb_to_exclude_for_random);
					}
					if (list.Count > 0)
					{
						netBody = list[Random.Range(0, list.Count)];
					}
				}
				else
				{
					List<NetBody> list2 = new List<NetBody>(NetBody.all_instances);
					if ((Object)(object)nb_to_exclude_for_random != (Object)null)
					{
						list2.Remove(nb_to_exclude_for_random);
					}
					if (list2.Count > 0)
					{
						netBody = list2[Random.Range(0, list2.Count)];
					}
				}
				if ((Object)(object)netBody != (Object)null)
				{
					func(netBody);
					return Tuple.Create(item1: true, "Success");
				}
				return Tuple.Create(item1: false, "Nothing was found.");
			}
			if (plrname.StartsWith("@c"))
			{
				Body bodyOnPos = Util.GetBodyOnPos(Util.GetCursorWorldPos());
				NetBody netBody2 = ((bodyOnPos != null) ? ((Component)bodyOnPos).GetComponent<NetBody>() : null);
				if ((Object)(object)netBody2 == (Object)null)
				{
					netBody2 = NetBody.GetNearestBody(Util.GetCursorWorldPos()).Item1;
				}
				if ((Object)(object)netBody2 != (Object)null)
				{
					if (only_players && !netBody2.is_player)
					{
						return Tuple.Create(item1: false, "Body is not player.");
					}
					func.DynamicInvoke(netBody2);
					return Tuple.Create(item1: true, "Success");
				}
				return Tuple.Create(item1: false, "Nobody was found at cursor.");
			}
			return Tuple.Create(item1: false, "Unknown macro.");
		}
		if (TryGetPlayerFromPartialName(plrname, out var player, body_required: true))
		{
			func.DynamicInvoke(player.playerbody);
			return Tuple.Create(item1: true, "Success");
		}
		return Tuple.Create(item1: false, "Player not found");
	}

	internal static void _UpdateSpecialPlayerLists()
	{
		_real_AllClientIdsExceptHost.Clear();
		_real_AllPlayersExceptHost.Clear();
		_real_AllClientIds.Clear();
		if (!Net.running)
		{
			return;
		}
		foreach (NetPlayer value in NetPlayer.ClientIdToPlayerDict.Values)
		{
			_real_AllClientIds.Add(value.clientId);
			if ((Object)(object)value != (Object)null && !value.is_host)
			{
				_real_AllClientIdsExceptHost.Add(value.clientId);
				_real_AllPlayersExceptHost.Add(value);
			}
		}
	}

	private void FixedUpdate()
	{
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		tpscalc_framecounter++;
		double realtimeSinceStartupAsDouble = Time.realtimeSinceStartupAsDouble;
		if (realtimeSinceStartupAsDouble - tpscalc_lasttime >= 1.0)
		{
			tpscalc_lasttime = realtimeSinceStartupAsDouble;
			CURRENT_TPS = tpscalc_framecounter;
			tpscalc_framecounter = 0;
			if (Net.is_server)
			{
				ClientMain.SERVER_TPS = CURRENT_TPS;
				if ((Object)(object)NetPlayer.LOCAL_PLAYER != (Object)null && NetPlayer.LOCAL_PLAYER.server_plrstate != null)
				{
					NetPlayer.LOCAL_PLAYER.server_plrstate.tps = CURRENT_TPS;
				}
			}
		}
		_UpdateSpecialPlayerLists();
		if (!Net.running)
		{
			return;
		}
		if (KrokoshaScavMultiplayer.is_dedicated_server && Util.TryGetLocalBody(out var body))
		{
			body.ResetHealth();
			body.Body_DropAllItems();
			body.shock = 0f;
			body.Stand(true);
			if (Util.IsWorldGenerated())
			{
				((Component)body).transform.position = Vector2.op_Implicit(new Vector2(999999f, 999999f));
			}
			body.rb.gravityScale = 0f;
			body.rb.velocity = Vector2.zero;
			body.rb.angularVelocity = 0f;
		}
		if (Util.IsInWorld() || !KrokoshaScavMultiplayer.rules.AutoContinue || KrokoshaScavMultiplayer.is_client)
		{
			return;
		}
		if (NetPlayer.ClientIdToPlayerDict.Count >= KrokoshaScavMultiplayer.rules.AutoMinPlrsToStart)
		{
			_ded_server_switch_counter += Time.deltaTime;
			if (_ded_server_switch_counter > 20f)
			{
				_ded_server_switch_counter = -100f;
				PreRunScript val = Object.FindObjectOfType<PreRunScript>();
				if (Object.op_Implicit((Object)(object)val))
				{
					string message = "autostarting_run";
					KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog(Lang.GetEN(message));
					Chat.Server_ChatAnnouncement(Lang.MarkMsgAsLocaleKey(in message));
					((MonoBehaviour)val).StartCoroutine(val.WaitLoad());
				}
				else
				{
					KrokoshaScavMultiplayer.DoMultiplayerStatusMessageError("Failed to autostart the run. Theres no PreRunScript???? what???");
					_ded_server_switch_counter = 10f;
				}
			}
		}
		else
		{
			_ded_server_switch_counter = 0f;
		}
	}

	private void Update()
	{
		fpscalc_framecounter++;
		double realtimeSinceStartupAsDouble = Time.realtimeSinceStartupAsDouble;
		if (realtimeSinceStartupAsDouble - fpscalc_lasttime >= 1.0)
		{
			fpscalc_lasttime = realtimeSinceStartupAsDouble;
			CURRENT_FPS = fpscalc_framecounter;
			fpscalc_framecounter = 0;
			if (Net.is_server)
			{
				ClientMain.SERVER_FPS = CURRENT_FPS;
				if ((Object)(object)NetPlayer.LOCAL_PLAYER != (Object)null && NetPlayer.LOCAL_PLAYER.server_plrstate != null)
				{
					NetPlayer.LOCAL_PLAYER.server_plrstate.fps = CURRENT_FPS;
				}
			}
		}
		if (KrokoshaScavMultiplayer.IsNetworkActiveAndIsServer())
		{
			timer_UpdateSyncClientsToClients += Time.unscaledDeltaTime;
			timer_RareUpdateSyncClients += Time.unscaledDeltaTime;
			timer_SecondUpdateSyncClients += Time.unscaledDeltaTime;
			if (timer_RareUpdateSyncClients > 5f)
			{
				timer_RareUpdateSyncClients = 0f;
				RareUpdateSyncClients();
			}
			else if (timer_SecondUpdateSyncClients > 1f)
			{
				timer_SecondUpdateSyncClients = 0f;
				SecondUpdateSyncClients();
			}
			else if (timer_UpdateSyncClientsToClients > 0.09f)
			{
				timer_UpdateSyncClientsToClients = 0f;
				UpdateSyncClientsToClients();
			}
		}
	}

	private void LateUpdate()
	{
		if (KrokoshaScavMultiplayer.network_system_is_running)
		{
			HandleSleepUpdate();
			HandleDedicatedServerUpdate();
		}
	}

	private static void HandleDedicatedServerUpdate()
	{
		if (!Util.IsGeneratingWorld())
		{
			_ded_server_switch_counter += Time.unscaledDeltaTime;
		}
		if (Util.IsWorldGenerated() && KrokoshaScavMultiplayer.is_dedicated_server && _ded_server_switch_counter > 10f && KrokoshaScavMultiplayer.rules.AutoContinue)
		{
			if (KrokoshaScavMultiplayer.rules.AutoExitWhenAllLeft && NetPlayer.ClientIdToPlayerDict.Count == 0)
			{
				_ded_server_switch_counter = -30f;
				string message = "Auto-exiting to main menu because everyone left the server.";
				KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog(message);
				Chat.Server_ChatAnnouncement(in message);
				PlayerCamera.main.ToMainMenu();
			}
			else if (KrokoshaScavMultiplayer.rules.AutoExitWhenAllDied && NetPlayer.AllLivingPlayers.Count == 0)
			{
				_ded_server_switch_counter = -5f;
				string message2 = "Auto-exiting to main menu because everyone died.";
				KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog(message2);
				Chat.Server_ChatAnnouncement(in message2);
				PlayerCamera.main.ToMainMenu();
			}
		}
	}

	private static void HandleSleepUpdate()
	{
		if (KrokoshaScavMultiplayer.rules.DisableSleep)
		{
			if (Util.IsInWoundView())
			{
				WoundView.view.sleepTip.tipName = Lang.Get("sleep_is_disabled", false);
				WoundView.view.sleepTip.tipDesc = Lang.Get("sleep_disabled_desc", false);
			}
		}
		else if (KrokoshaScavMultiplayer.rules.DisableTimeManipulation)
		{
			if (CheckIfEveryoneIsSleeping())
			{
				Time.timeScale = 25f;
			}
			else if (Time.timeScale == 25f)
			{
				Time.timeScale = 1f;
			}
		}
		else if (CheckIfEveryoneIsSleeping() && !KrokoshaScavMultiplayer.is_client)
		{
			PlayerCamera.main.SetTimeScale((SpeedType)3, true, false);
		}
	}

	private void Start()
	{
		((MonoBehaviour)this).InvokeRepeating("update10s", 10f, 10f);
		NetPlayer.OnPlayerLeft += delegate(NetPlayer plr)
		{
			_real_AllClientIds.Remove(plr.clientId);
			_real_AllClientIdsExceptHost.Remove(plr.clientId);
			_real_AllPlayersExceptHost.Remove(plr);
		};
		ServerClientCustomCommandsDict["test"] = delegate
		{
		};
	}

	private void update10s()
	{
		if (Net.is_server && Net.running)
		{
			_Server_OnServerNameChange();
			Net.MY_SERVER_INFO.SetValues();
			Net.cur_server_info.SetValues();
		}
	}

	public static void OnPlayerSleep(NetPlayer plr)
	{
		PlayerCamera.main.DoAlert(plr.playername + Lang.Get("on_plr_sleep", false), false);
	}

	public static void OnPlayerDeath(NetPlayer plr)
	{
		if (!KrokoshaScavMultiplayer.is_client)
		{
			Plugin.log.LogInfo((object)(plr.playername + " is ded, not big suprise."));
			Chat.Server_ChatAnnouncement(plr.playername + " died.");
			if (Util.IsTutorialWorld())
			{
				plr.OnFinishTutorial(tp: false);
			}
			if (NetPlayer.AllLivingPlayers.Count == 0)
			{
				string message = Lang.MarkMsgAsLocaleKey("everyone_is_dead");
				Chat.Server_ChatAnnouncement(in message);
				Server_AnnounceAlert(in message, important: true);
				PlayerCamera.main.DoAlert(Lang.Get("everyone_is_dead_host", false), false);
			}
		}
		else
		{
			Plugin.log.LogInfo((object)("uhh i think " + plr.playername + " is ded."));
		}
		ComponentHolderProtocol.AddComponent<Krokosha_CorpseScript_MultiplayerAdditionComponent>((Object)(object)plr).animalCorpse = false;
	}

	public static void Server_AnnounceSeed(IReadOnlyList<knetid> to_who)
	{
		NetDataWriter writer = Net.CreateWriter(10010);
		writer.Put(WorldGeneration_GenerateWorld_MultiplayerPatch.firstworldgenparams);
		IEnumerable<knetid> clientIds = to_who;
		Net.Server_SendToClientsVeryReliable(in writer, in clientIds);
	}

	public static void Server_AnnounceSound(Vector2 pos, string name, IReadOnlyList<knetid> to_who)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		NetDataWriter writer = Net.CreateWriter(10017);
		writer.Put(pos);
		writer.Put(name, oneByteChars: true);
		DeliveryMethod delivery = (DeliveryMethod)0;
		IEnumerable<knetid> clientIds = to_who;
		Net.Server_SendToClients(in delivery, in writer, in clientIds);
	}

	public static void Server_SendWorldState()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		NetDataWriter writer = Net.CreateWriter(10014);
		writer.Put(new SerializableRandomState
		{
			State = Random.state
		});
		writer.Put(Util.IsWorldGenerated());
		Net.Server_SendToClients((DeliveryMethod)4, in writer, (IEnumerable<knetid>)AllClientIdsExceptHost);
	}

	private void SecondUpdateSyncClients()
	{
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)NetPlayer.LOCAL_PLAYER != (Object)null)
		{
			NetPlayer.LOCAL_PLAYER.ping = 0.0;
		}
		ClientMain.LOCAL_PING = 0;
		NetDataWriter writer = Net.CreateWriter(10005);
		foreach (NetPlayer value in NetPlayer.ClientIdToPlayerDict.Values)
		{
			writer.Put(true);
			writer.Put((ushort)value.clientId);
			writer.Put((float)value.ping);
			if (writer.Length > 1000)
			{
				break;
			}
		}
		writer.Put(false);
		if (Net.TryGetSteamTransport(out var tsteam))
		{
			tsteam.Server_SendToClients(0, in writer, AllClientIdsExceptHost);
		}
		else
		{
			Net.Server_SendToClients((DeliveryMethod)4, in writer, (IEnumerable<knetid>)AllClientIdsExceptHost);
		}
	}

	private void RareUpdateSyncClients()
	{
		if (Util.IsWorldGenerated())
		{
			Server_SendWorldState();
		}
	}

	private void UpdateSyncClientsToClients()
	{
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		if (!Util.IsInWorld())
		{
			return;
		}
		WorldGeneration world = WorldGeneration.world;
		if (WorldGeneration.world.doingRegen || WorldGeneration.world.generatingWorld || !WorldGeneration.world.worldExists)
		{
			return;
		}
		NetPlayer netPlayer = null;
		foreach (KeyValuePair<Body, NetPlayer> item in NetPlayer.BodyToPlayerDict)
		{
			if (item.Key.alive && item.Key.conscious && item.Value.IsAtTheEndOfLayer() && !item.Value.finishedLayer)
			{
				netPlayer = item.Value;
				item.Value.finishedLayer = true;
				break;
			}
		}
		bool flag = false;
		if ((Object)(object)netPlayer != (Object)null)
		{
			string message = string.Format(Lang.Get("plr_finished_layer", false), netPlayer.playername);
			if (!netPlayer.is_local)
			{
				PlayerCamera.main.DoAlert(message, false);
			}
			Chat.Server_ChatAnnouncement(in message);
		}
		if (CheckIfEnoughPeopleAreAtLayerFinish(out var _, out var _))
		{
			world.savePanel.SetActive(true);
			flag = true;
		}
		else if (CheckIfShouldStartRadlineForStragglers(1) && Util.IsWorldGenerated() && !RadiationLine.line.active)
		{
			RadiationLine.line.Activate();
			NetPlayer netPlayer2 = NetPlayer.AllLivingPlayers.First();
			foreach (NetPlayer allLivingPlayer in NetPlayer.AllLivingPlayers)
			{
				if (allLivingPlayer.playerbody.GetHeadPos().y > netPlayer2.playerbody.GetHeadPos().y)
				{
					netPlayer2 = allLivingPlayer;
				}
				if (!allLivingPlayer.finishedLayer)
				{
					allLivingPlayer.Server_DoAlertSingle(Lang.MarkMsgAsLocaleKey("straggler_radline"));
				}
			}
			RadiationLineUpdatePatch.timeGone = (float)world.halfHeight - netPlayer2.playerbody.GetHeadPos().y - 64f;
		}
		if (flag && KrokoshaScavMultiplayer.rules.AutoContinue && !WorldGeneration.world.doingRegen && !world.generatingWorld && world.worldExists)
		{
			world.generatingWorld = true;
			Plugin.log.LogInfo((object)"SERVER: Autocontinuing the run! ");
			Chat.Server_ChatAnnouncement("Server auto continues the run!");
			world.savePanel.SetActive(false);
			((MonoBehaviour)world).StartCoroutine(world.RegenerateWorld(false));
		}
	}

	public static void RegisterServerReceiver(ushort name, KrokoshaScavMultiplayer.KrokoshaHandleNamedMessageDelegate receiver)
	{
		KrokoshaScavMultiplayer.all_network_receivers_from_attributes.Add(new ServerReceiverAttribute(name), receiver);
	}

	public static void _RegisterServerReceivers()
	{
		foreach (KeyValuePair<KrokoshaNetworkMessageReceiverAttribute, KrokoshaScavMultiplayer.KrokoshaHandleNamedMessageDelegate> item in KrokoshaScavMultiplayer.all_network_receivers_from_attributes)
		{
			if (item.Key.GetReceiverType() != KrokoshaNetworkMessageReceiverAttribute.ReceiverType.Server)
			{
				continue;
			}
			Net.RegisterServerReceiver(item.Key.GetMessageId(), delegate(knetid clientId, ref NetDataReader reader)
			{
				try
				{
					item.Value(clientId, ref reader);
				}
				catch (Exception ex)
				{
					log.error("SUS: caused error in Server receiver " + item.Key.GetMessageName() + " (" + GetPlayerFullDebugString(clientId) + ") \n" + ex.ToString());
				}
			});
		}
	}

	[ServerReceiver(10031)]
	private static void ServerReceiver__RequestPiggyback(knetid clientId, ref NetDataReader reader)
	{
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out knetid result);
		if (!Util.IsWorldGenerated() || !NetPlayer.TryGetNetPlayerAndNetBodyFromClientId(clientId, out var plr, out var pb) || !NetBody.TryGetNetBodyFromId(result, out var nb) || !((Object)(object)pb != (Object)(object)nb))
		{
			return;
		}
		if (plr.is_local || (plr.body.conscious && nb.IsPiggybackable() && pb.StartPiggyback(nb, check_distance: true, force: true)))
		{
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(Vector2.op_Implicit(((Component)plr.body).transform.position), $"S: RequestPiggyback {clientId} to {result}");
			}
			if (log.verbose)
			{
				log.l($"RequestPiggyback success: {plr} -> {nb}");
			}
		}
		else
		{
			plr.Server_DoAlertSingle("SERVER: Denied Piggyback.");
			log.serverdeny($"RequestPiggyback {plr} -> {nb}");
			plr.Server_RemindPlayersCurrentState();
		}
	}

	[ServerReceiver(10032)]
	private static void ServerReceiver__RequestStopCarryPiggyback(knetid clientId, ref NetDataReader reader)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if (Util.IsWorldGenerated() && NetPlayer.TryGetNetPlayerAndNetBodyFromClientId(clientId, out var plr, out var _) && plr.IsConscious() && (Object)(object)plr.playerbody.piggybacking_on != (Object)null)
		{
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(Vector2.op_Implicit(((Component)plr.body).transform.position), $"S: RequestStopCarryPiggyback {plr} ");
			}
			plr.playerbody.StopPiggyback();
			if (log.verbose)
			{
				log.l($"RequestStopCarryPiggyback success: {plr} ");
			}
		}
	}

	[ServerReceiver(10033)]
	private static void ServerReceiver__RequestCarryPerson(knetid clientId, ref NetDataReader reader)
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out knetid result);
		if (!Util.IsWorldGenerated() || !NetPlayer.TryGetNetPlayerAndNetBodyFromClientId(clientId, out var plr, out var pb) || !NetBody.TryGetNetBodyFromId(result, out var nb) || !((Object)(object)pb != (Object)(object)nb))
		{
			return;
		}
		if ((Object)(object)pb.carrying_person == (Object)null && plr.body.conscious)
		{
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(Vector2.op_Implicit(((Component)plr.body).transform.position), $"S: RequestCarryPerson {clientId} to {result}");
			}
			if (nb.CanBeCarriedBySomeone() && nb.StartPiggyback(plr.playerbody, check_distance: true, force: true))
			{
				if (log.verbose)
				{
					log.l($"RequestCarryPerson success: {plr} -> {nb}");
				}
				return;
			}
		}
		plr.Server_DoAlertSingle("SERVER: Denied Carry.");
		log.serverdeny($"RequestCarryPerson {plr} -> {nb}");
	}

	[ServerReceiver(10034)]
	private static void ServerReceiver__RequestStopCarryPerson(knetid clientId, ref NetDataReader reader)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if (Util.IsWorldGenerated() && NetPlayer.TryGetNetPlayerAndBodyFromClientId(clientId, out var plr, out var _) && plr.IsConscious() && (Object)(object)plr.playerbody.carrying_person != (Object)null)
		{
			NetBody carrying_person = plr.playerbody.carrying_person;
			carrying_person.StopPiggyback();
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(Vector2.op_Implicit(((Component)plr.body).transform.position), $"S: StopCarryPerson {plr} to {carrying_person}");
			}
		}
	}

	[ServerReceiver(10035)]
	private static void ServerReceiver__Tutorial_Finished(knetid clientId, ref NetDataReader reader)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		NetPlayer netPlayerFromClientId = NetPlayer.GetNetPlayerFromClientId(clientId);
		if (!((Object)(object)netPlayerFromClientId != (Object)null))
		{
			return;
		}
		if (!Util.IsTutorialWorld())
		{
			log.serverdeny($"SUS: {netPlayerFromClientId} sent Tutorial_Finished when we are not even in a tutorial.");
		}
		else if (!netPlayerFromClientId._tutorial_finished)
		{
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(Vector2.op_Implicit(((Component)netPlayerFromClientId.body).transform.position), $"S: Tutorial_Finished {clientId}");
			}
			netPlayerFromClientId.OnFinishTutorial();
		}
	}

	[ServerReceiver(10036)]
	private static void ServerReceiver__PlayerPointFingerAt(knetid clientId, ref NetDataReader reader)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (NetPlayer.TryGetPlayerFromClientId(clientId, out var plr) && Util.IsInWorld())
		{
			reader.Get(out Vector2 result);
			byte b = default(byte);
			reader.Get(ref b);
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(result, "S: PlayerPointFingerAt " + GetPlayerFullDebugString(clientId));
			}
			plr.PointFingerAt(result, b);
			NetDataWriter writer = Net.CreateWriter(10012);
			writer.Put((ushort)clientId);
			writer.Put(result);
			writer.Put(b);
			Net.Server_SendToClients((DeliveryMethod)2, in writer, (IEnumerable<knetid>)GetListOfClientIdsExceptThisAndHost(clientId));
		}
	}

	[ServerReceiver(10037)]
	private static void ServerReceiver__PlayerJump(knetid clientId, ref NetDataReader reader)
	{
		if (NetPlayer.TryGetNetPlayerAndBodyFromClientId(clientId, out var _, out var body) && body.conscious && body.standing)
		{
			body.Jump();
		}
	}

	[ServerReceiver(10038)]
	private static void ServerReceiver__PlayerImpactDamage(knetid clientId, ref NetDataReader reader)
	{
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		if (!Util.IsWorldGenerated())
		{
			return;
		}
		byte b = default(byte);
		reader.Get(ref b);
		float num = default(float);
		reader.Get(ref num);
		reader.Get(out Vector2 result);
		float num2 = Mathf.Abs(num);
		Body bodyFromClientId = NetPlayer.GetBodyFromClientId(clientId);
		if ((Object)(object)bodyFromClientId != (Object)null && bodyFromClientId.alive && num.IsFinite())
		{
			Limb val = bodyFromClientId.limbs[b];
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(Vector2.op_Implicit(((Component)val).transform.position), $"S: Impact {((Object)val).name} force:{num2} dir:{result}");
			}
			Limb_ImpactDamage_MultiplayerPatch.Force(val, num2, result);
		}
	}

	[ServerReceiver(10039)]
	private static void ServerReceiver__PlayerBark(knetid clientId, ref NetDataReader reader)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (NetPlayer.TryGetNetPlayerAndBodyFromClientId(clientId, out var plr, out var body) && body.conscious)
		{
			if (!body.IsBodyLocal())
			{
				body.eatTime = 0f;
				((Component)body).GetComponent<PantSound>().Bark();
			}
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(plr.pos, "S: Bark");
			}
			KrokoshaScavMultiplayer.Server_SendRelayMessageToClients((ushort)10018, clientId, reliable: false);
		}
	}

	[ServerReceiver(10040)]
	private static void ServerReceiver__Push_Ego_Naxuy(knetid clientId, ref NetDataReader reader)
	{
		reader.Get(out knetid result);
		if (NetPlayer.TryGetNetPlayerAndBodyFromClientId(clientId, out var plr, out var body) && body.conscious && body.standing && plr.server_plrstate.last_push + 1.0 < Time.unscaledTimeAsDouble && NetBody.TryGetNetBodyFromId(result, out var nb))
		{
			Component ba = (Component)(object)body;
			if (KM.dist2dsqrcheck(in ba, (Component)(object)nb.body, SharedMain.max_player_interaction_distance * 1.2f))
			{
				plr.playerbody.Push(nb);
			}
		}
	}

	[ServerReceiver(10041)]
	private static void ServerReceiver__SetTimeScale(knetid clientId, ref NetDataReader reader)
	{
		ushort num = default(ushort);
		reader.Get(ref num);
		if (Util.IsInWorld() && NetPlayer.TryGetPlayerFromClientId(clientId, out var plr))
		{
			Plugin.log.LogInfo((object)$"SetTimeScale: Received SetTimeScale from a client {clientId} {((object)plr).ToString()}  ");
			PlayerCamera.main.SetTimeScale((SpeedType)num, true, false);
			PlayerCamera.main.DoAlert(string.Format(Lang.Get("plr_set_timescale", false), plr.playername, $"{num} ({Time.timeScale})"), false);
		}
	}

	[ServerReceiver(10002)]
	private static void ServerReceiver__ClientCharacterSyncUpdate(knetid clientId, ref NetDataReader reader)
	{
		reader = reader.DecompressReader();
		reader.Get(out ClientToServer_NetBodySyncPacket result);
		if (!KrokoshaScavMultiplayer.IsInGameAndWorldGenerated())
		{
			return;
		}
		if (NetPlayer.TryGetNetPlayerAndNetBodyFromClientId(clientId, out var plr, out var pb))
		{
			if (plr.is_local)
			{
				return;
			}
			plr.server_plrstate.is_loaded_in = true;
			if (pb.alive && !plr.Server_HasForcedServerOwnershipOfBody())
			{
				pb.OnReceiveSyncPacket(in result);
				if (result.IncludeRagData() && !reader.EndOfData)
				{
					ClientMain.ReadRagdollPacket(ref reader, pb.body);
				}
			}
		}
		else
		{
			log.error($"player {clientId} doesnt have a playerprefab object ?????  ClientCharacterSyncUpdate");
		}
	}

	[ServerReceiver(10042)]
	private static void ServerReceiver__PlayerDrinkLiquid(knetid clientId, ref NetDataReader reader)
	{
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		if (!NetPlayer.TryGetPlayerFromClientId(clientId, out var plr))
		{
			return;
		}
		if (Util.IsWorldGenerated() && plr.IsAlive())
		{
			if (log.verbose)
			{
				log.l($"Server approves drink for: {plr} ");
			}
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(Vector2.op_Implicit(((Component)plr.body).transform.position), $"S: PlayerDrinkLiquid {plr}");
			}
			Vector2Int val = WorldGeneration.world.WorldToBlockPos(Vector2.op_Implicit(((Component)plr.body).transform.position - Vector3.up * 2.5f));
			byte liquid = FluidManager.main.GetLiquid(((Vector2Int)(ref val)).x, ((Vector2Int)(ref val)).y);
			if (!plr.is_local)
			{
				FluidManager.main.DrinkLiquid(val, plr.body);
			}
			if (liquid != 0 || plr.is_local)
			{
				Server_AnnounceSound(Vector2.op_Implicit(((Component)plr.body).transform.position), "drink", AllClientIdsExceptHost);
			}
			MedicalSync.Server_QueueSendCharacterHealth(plr.playerbody);
		}
		else
		{
			log.sus($"PlayerDrinkLiquid {plr} is ded or sum  ");
		}
	}

	[ServerReceiver(10184)]
	private static void ServerReceiver__ClientCustomCommand(knetid clientId, ref NetDataReader reader)
	{
		if (NetPlayer.TryGetPlayerFromClientId(clientId, out var plr))
		{
			RunClientCustomCommand(reader.GetString(), plr);
		}
	}

	internal static void RunClientCustomCommand(string com, NetPlayer plr)
	{
		string[] array = com.Trim().Split(new char[1] { ' ' });
		if (ServerClientCustomCommandsDict.TryGetValue(array[0], out var value))
		{
			try
			{
				value(plr, com, array);
			}
			catch (Exception ex)
			{
				Con.Server_SendConsoleLog(ex.Message, plr);
			}
		}
	}
}
