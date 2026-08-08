using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using AOT;
using BepInEx.Logging;
using KrokoshaCasualtiesUtils;
using LiteNetLib.Utils;
using Steamworks;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public class KSteam : MonoBehaviour
{
	public class LobbyMember
	{
		public CSteamID steamID;

		public double server_last_data_receive_time;

		public string disconnectreason;

		public Dictionary<string, string> metadata = new Dictionary<string, string>();

		public LobbyMember()
		{
			server_last_data_receive_time = Time.realtimeSinceStartupAsDouble;
		}
	}

	public class Lobby
	{
		public bool locked;

		public CSteamID lobby_steamID;

		public CSteamID ownerID;

		public Dictionary<CSteamID, LobbyMember> members = new Dictionary<CSteamID, LobbyMember>();

		public int memberlimit;

		public Dictionary<string, string> metadata = new Dictionary<string, string>();

		public string DebugReadableDump()
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			return string.Format("===\nlobby_steamID: {0}\nownerID: {1}\nlocked: {2}\nmemberlimit: {3}\nmembers: {4}\nmetadata:\n{5}\n===", lobby_steamID, ownerID, locked, memberlimit, string.Join(", ", members.Keys), string.Join("\n", metadata));
		}
	}

	public static KSteam singleton = null;

	public const uint CONST_STEAMAPPID_SPACEWAR = 480u;

	public const uint CONST_STEAMAPPID_CASUALTIESUNKNOWN_DEMO = 4576510u;

	public const uint CONST_STEAMAPPID_CASUALTIESUNKNOWN_PLAYTEST = 4584420u;

	public static List<Lobby> LOBBIES = new List<Lobby>();

	public static Lobby CURRENT_LOBBY = new Lobby();

	private static bool IS_DOING_HostLobbyRecreatorLoop = false;

	public static HashSet<ulong> Friends = new HashSet<ulong>();

	public static HashSet<ulong> Following = new HashSet<ulong>();

	public static Dictionary<int, Texture2D> SteamImagesCache = new Dictionary<int, Texture2D>();

	public static Dictionary<ulong, string> SteamNamesCache = new Dictionary<ulong, string>();

	public const string _LOBBYDATAKEYPREFIX = "CASUALTIESUNKNOWN_KROKOSHA_MULTIPLAYER_COOP_MOD_";

	public const string LOBBYDATAKEY_VERSION = "CASUALTIESUNKNOWN_KROKOSHA_MULTIPLAYER_COOP_MOD_VERSION";

	public const string LOBBYDATAKEY_LOBBYNAME = "CASUALTIESUNKNOWN_KROKOSHA_MULTIPLAYER_COOP_MOD_LOBBYNAME";

	public const string LOBBYDATAKEY_GAMEMODE = "CASUALTIESUNKNOWN_KROKOSHA_MULTIPLAYER_COOP_MOD_GAMEMODE";

	public const string LOBBYDATAKEY_HASPASSWORD = "CASUALTIESUNKNOWN_KROKOSHA_MULTIPLAYER_COOP_MOD_HASPASSWORD";

	public const string LOBBYDATAKEY_ISDEDICATED = "CASUALTIESUNKNOWN_KROKOSHA_MULTIPLAYER_COOP_MOD_ISDEDICATED";

	public const string LOBBYDATAKEY_MIDJOIN_LOCKED = "CASUALTIESUNKNOWN_KROKOSHA_MULTIPLAYER_COOP_MOD_LOCKED";

	public const string LOBBYDATAKEY_CURRENTLAYER = "CASUALTIESUNKNOWN_KROKOSHA_MULTIPLAYER_COOP_MOD_CURRENTLAYER";

	public const string LOBBYDATAKEY_LIVINGCOUNT = "CASUALTIESUNKNOWN_KROKOSHA_MULTIPLAYER_COOP_MOD_LIVINGCOUNT";

	public const string LOBBYDATAKEY_PLAYERCOUNT = "CASUALTIESUNKNOWN_KROKOSHA_MULTIPLAYER_COOP_MOD_PLRCOUNT";

	public const string LOBBYDATAKEY_DEPTH = "CASUALTIESUNKNOWN_KROKOSHA_MULTIPLAYER_COOP_MOD_DEPTH";

	public const string LOBBYDATAKEY_AVERAGEMOOD = "CASUALTIESUNKNOWN_KROKOSHA_MULTIPLAYER_COOP_MOD_AVGMOOD";

	public const string LOBBYDATAKEY_EXTRADATA = "CASUALTIESUNKNOWN_KROKOSHA_MULTIPLAYER_COOP_MOD_EXTRADATA";

	public const string LOBBYDATAKEY_RUNSETTINGS = "CASUALTIESUNKNOWN_KROKOSHA_MULTIPLAYER_COOP_MOD_RUNSETTINGS";

	public const int MAX_LOBBY_BUCKETS = 20;

	public static int LobbySearch_BucketCounter = 0;

	internal static ELobbyDistanceFilter userselected_lobbyDistanceFilter = (ELobbyDistanceFilter)1;

	private static ushort userselected_maxLobbies = 100;

	internal static Callback<FavoritesListChanged_t> FavoritesListChanged;

	internal static Callback<LobbyInvite_t> LobbyInvite;

	internal static Callback<LobbyEnter_t> LobbyEnter;

	internal static Callback<LobbyDataUpdate_t> LobbyDataUpdate;

	internal static Callback<LobbyChatUpdate_t> LobbyChatUpdate;

	internal static Callback<LobbyChatMsg_t> LobbyChatMsg;

	internal static Callback<LobbyGameCreated_t> LobbyGameCreated;

	internal static Callback<LobbyKicked_t> LobbyKicked;

	internal static Callback<FavoritesListAccountsUpdated_t> FavoritesListAccountsUpdated;

	internal static Callback<JoinPartyCallback_t> JoinPartyCallback;

	internal static Callback<CreateBeaconCallback_t> CreateBeaconCallback;

	internal static Callback<ReservationNotificationCallback_t> ReservationNotificationCallback;

	internal static Callback<ChangeNumOpenSlotsCallback_t> ChangeNumOpenSlotsCallback;

	internal static Callback<AvailableBeaconLocationsUpdated_t> AvailableBeaconLocationsUpdated;

	internal static Callback<ActiveBeaconsUpdated_t> ActiveBeaconsUpdated;

	internal static CallResult<LobbyEnter_t> OnLobbyEnterCallResult;

	internal static CallResult<LobbyMatchList_t> OnLobbyMatchListCallResult;

	internal static CallResult<LobbyCreated_t> OnLobbyCreatedCallResult;

	private static Callback<PersonaStateChange_t> PersonaStateChange;

	private static Callback<GameOverlayActivated_t> GameOverlayActivated;

	private static Callback<GameServerChangeRequested_t> GameServerChangeRequested;

	private static Callback<GameLobbyJoinRequested_t> GameLobbyJoinRequested;

	private static Callback<AvatarImageLoaded_t> AvatarImageLoaded;

	private static Callback<FriendRichPresenceUpdate_t> FriendRichPresenceUpdate;

	private static Callback<GameRichPresenceJoinRequested_t> GameRichPresenceJoinRequested;

	private static Callback<GameConnectedClanChatMsg_t> GameConnectedClanChatMsg;

	private static Callback<GameConnectedChatJoin_t> GameConnectedChatJoin;

	private static Callback<GameConnectedChatLeave_t> GameConnectedChatLeave;

	private static Callback<GameConnectedFriendChatMsg_t> GameConnectedFriendChatMsg;

	private static Callback<UnreadChatMessagesChanged_t> UnreadChatMessagesChanged;

	private static Callback<OverlayBrowserProtocolNavigation_t> OverlayBrowserProtocolNavigation;

	private static Callback<EquippedProfileItemsChanged_t> EquippedProfileItemsChanged;

	private static CallResult<ClanOfficerListResponse_t> OnClanOfficerListResponseCallResult;

	private static CallResult<DownloadClanActivityCountsResult_t> OnDownloadClanActivityCountsResultCallResult;

	private static CallResult<JoinClanChatRoomCompletionResult_t> OnJoinClanChatRoomCompletionResultCallResult;

	private static CallResult<FriendsGetFollowerCount_t> OnFriendsGetFollowerCountCallResult;

	private static CallResult<FriendsIsFollowing_t> OnFriendsIsFollowingCallResult;

	private static CallResult<FriendsEnumerateFollowingList_t> OnFriendsEnumerateFollowingListCallResult;

	private static CallResult<EquippedProfileItems_t> OnEquippedProfileItemsCallResult;

	protected static ManualLogSource log => Plugin.log;

	public static bool Loaded { get; private set; }

	public static bool IS_IN_LOBBY { get; private set; }

	public static bool IS_IN_STEAMOVERLAY { get; private set; }

	public static bool APP_IS_PIRATE { get; private set; }

	public static bool APP_IS_DEMO { get; private set; }

	public static bool APP_IS_PLAYTEST { get; private set; }

	public static uint STEAM_APPID { get; private set; }

	internal static Dictionary<ulong, NetPublicServerInfo> CurServerList_ButOnlySteamLobbies => UIServerBrowser.CurServerList_ButOnlySteamLobbies;

	public static CSteamID lobbyId => CURRENT_LOBBY.lobby_steamID;

	public static bool _DEV_ENABLE_STEAM_LOG => DebugMenuSettings._DEV_ENABLE_STEAM_LOG;

	private static void UpdateLobbyInfo(CSteamID steamIDLobby, ref Lobby outLobby)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		outLobby.lobby_steamID = steamIDLobby;
		outLobby.ownerID = SteamMatchmaking.GetLobbyOwner(steamIDLobby);
		outLobby.memberlimit = SteamMatchmaking.GetLobbyMemberLimit(steamIDLobby);
		int lobbyDataCount = SteamMatchmaking.GetLobbyDataCount(steamIDLobby);
		outLobby.metadata.Clear();
		string key = default(string);
		string value = default(string);
		for (int i = 0; i < lobbyDataCount; i++)
		{
			bool lobbyDataByIndex = SteamMatchmaking.GetLobbyDataByIndex(steamIDLobby, i, ref key, 255, ref value, 8192);
			outLobby.metadata[key] = value;
			if (!lobbyDataByIndex)
			{
				KrokoshaCasualtiesMP.log.error("SteamMatchmaking.GetLobbyDataByIndex returned false.");
			}
		}
		List<CSteamID> list = new List<CSteamID>();
		int numLobbyMembers = SteamMatchmaking.GetNumLobbyMembers(steamIDLobby);
		for (int j = 0; j < numLobbyMembers; j++)
		{
			CSteamID lobbyMemberByIndex = SteamMatchmaking.GetLobbyMemberByIndex(steamIDLobby, j);
			list.Add(lobbyMemberByIndex);
		}
		List<CSteamID> list2 = list;
		List<CSteamID> list3 = new List<CSteamID>(CURRENT_LOBBY.members.Select((KeyValuePair<CSteamID, LobbyMember> x) => x.Key));
		IEnumerable<CSteamID> enumerable = list2.Except(list3);
		IEnumerable<CSteamID> enumerable2 = list3.Except(list2);
		foreach (CSteamID item in enumerable)
		{
			CURRENT_LOBBY.members.Add(item, new LobbyMember
			{
				steamID = item,
				server_last_data_receive_time = Time.realtimeSinceStartupAsDouble
			});
		}
		foreach (CSteamID item2 in enumerable2)
		{
			CURRENT_LOBBY.members.Remove(item2);
		}
	}

	internal static void LeaveLobbyAndResetNet(CSteamID lobby_steamID)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		IS_DOING_HostLobbyRecreatorLoop = false;
		CURRENT_LOBBY.lobby_steamID = lobby_steamID;
		LeaveLobbyAndResetNet();
	}

	internal static void LeaveLobbyAndResetNet(bool kill_transport = true)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		if (KrokoshaCasualtiesMP.log.verbose)
		{
			KrokoshaCasualtiesMP.log.l($"STEAM: LeaveLobby called! lobbyID:{CURRENT_LOBBY.lobby_steamID} - STACK: {new StackTrace()}");
		}
		else
		{
			KrokoshaCasualtiesMP.log.l($"STEAM: LeaveLobby called! lobbyID:{CURRENT_LOBBY.lobby_steamID}");
		}
		SteamMatchmaking.LeaveLobby(CURRENT_LOBBY.lobby_steamID);
		IS_IN_LOBBY = false;
		CURRENT_LOBBY.lobby_steamID = CSteamID.Nil;
		if (kill_transport)
		{
			Net.TryGetSteamTransport(out var _);
			Net.ShutdownReset();
		}
	}

	public static ELobbyType NumToLobbyType(int num)
	{
		return (ELobbyType)(num switch
		{
			1 => 1, 
			2 => 2, 
			_ => 0, 
		});
	}

	public static int LobbyTypeToNum(ELobbyType num)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		if ((int)num != 2)
		{
			return ((int)num == 1) ? 1 : 0;
		}
		return 2;
	}

	private void Awake()
	{
		if ((Object)(object)singleton != (Object)null)
		{
			Object.Destroy((Object)(object)this);
			throw new Exception("kill yourself!");
		}
		singleton = this;
	}

	private void Start()
	{
		((MonoBehaviour)this).Invoke("__CheckCommandLineForSteamArgs", 3f);
		((MonoBehaviour)this).InvokeRepeating("Update10s", 5f, 10f);
		BindToEventsToCallSteamStuff();
	}

	private void BindToEventsToCallSteamStuff()
	{
		KrokoshaScavMultiplayer.OnSceneChangeOrWorldStartGenerate += delegate
		{
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			if (Util.IsInWorld())
			{
				SteamTimeline.SetTimelineTooltip(WorldGeneration.world.biomeTitles[WorldGeneration.world.biomeDepth], 0f);
			}
			else
			{
				SteamTimeline.ClearTimelineTooltip(0f);
			}
			if (Net.TryGetSteamTransport(out var _))
			{
				foreach (NetPlayer value in NetPlayer.ClientIdToPlayerDict.Values)
				{
					SteamFriends.SetPlayedWith((CSteamID)value.steam_id);
				}
			}
		};
		WorldgenPatches.OnWorldgenFinish += delegate
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			SteamTimeline.AddInstantaneousTimelineEvent(WorldGeneration.world.biomeTitles[WorldGeneration.world.biomeDepth], "", "steam_bookmark", 400u, 0f, (ETimelineEventClipPriority)1);
			if (Net.is_playing_with_steam && Net.is_server)
			{
				Server_UpdateLobbyData();
			}
		};
		WorldGeneration_CreateExplosion_MultiplayerPatch.OnExplosion += delegate(ExplosionParams eparam)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			if (KM.dist2dsqrcheck(in eparam.position, Vector2.op_Implicit(((Component)Camera.main).transform.position), 60f))
			{
				SteamTimeline.AddInstantaneousTimelineEvent("Explosion", "", "steam_explosion", 10u, 0f, (ETimelineEventClipPriority)1);
			}
		};
		NetBody.OnPlayerDeath += delegate(NetPlayer plr)
		{
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			if (!((Object)(object)plr == (Object)null) && !((Object)(object)plr.body == (Object)null) && Time.unscaledTimeAsDouble - plr.playerbody.timeOfDeath > 1.0)
			{
				SteamTimeline.AddInstantaneousTimelineEvent("Death", plr.playername + " died.", "steam_death", 20u, 0f, (ETimelineEventClipPriority)1);
			}
		};
	}

	private static IEnumerator HostLobbyRecreatorLoop()
	{
		double start = Time.realtimeSinceStartupAsDouble;
		TransportSteamworks tsteam;
		while (IS_IN_LOBBY && Net.TryGetSteamTransport(out tsteam) && CURRENT_LOBBY.lobby_steamID == CSteamID.Nil)
		{
			yield return null;
			if (Time.realtimeSinceStartupAsDouble - start > 2.0)
			{
				start = Time.realtimeSinceStartupAsDouble;
				IS_DOING_HostLobbyRecreatorLoop = true;
				tsteam.CreateLobby(NumToLobbyType(UIMainMenu._STEAM_CHOSEN_LOBBYTYPE));
				yield return (object)new WaitForSeconds(1f);
			}
		}
	}

	private void Update10s()
	{
		if (Net.is_playing_with_steam && Net.is_server)
		{
			Server_UpdateLobbyData();
		}
		UpdateRichPresence();
	}

	public static string GetAppInstallDir()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		string text = default(string);
		SteamApps.GetAppInstallDir(SteamUtils.GetAppID(), ref text, 260u);
		if (text == null)
		{
			return "";
		}
		return text;
	}

	private void __CheckCommandLineForSteamArgs()
	{
		if (!Loaded)
		{
			return;
		}
		string[] array = Environment.GetCommandLineArgs();
		string text = default(string);
		SteamApps.GetLaunchCommandLine(ref text, 1024);
		if (text == null)
		{
			text = "";
		}
		else
		{
			array = text.Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
		}
		GetAppInstallDir();
		for (int i = 0; i < array.Length; i++)
		{
			string text2 = array[i];
			try
			{
				if (text2 == "+connect_lobby")
				{
					i++;
					if (i == array.Length)
					{
						break;
					}
					text2 = array[i];
					if (ulong.TryParse(text2, out var result) && !Net.running)
					{
						steamtestlog("CMD: Will join to lobby " + result);
						TransportSteamworks.OnWantToJoinLobby(result);
					}
				}
			}
			catch (Exception ex)
			{
				KrokoshaCasualtiesMP.log.error("STEAM: CMD: " + ex.ToString());
			}
		}
	}

	public static void CheckSteam()
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		if (Loaded || Plugin.FORCE_NO_STEAM)
		{
			return;
		}
		Loaded = false;
		if (!Packsize.Test())
		{
			log.LogError((object)"Steamworks.NET: Packsize.Test() FAILED !!!");
			return;
		}
		if (!DllCheck.Test())
		{
			log.LogError((object)"Steamworks.NET: DllCheck.Test() FAILED !!!");
			return;
		}
		string text = default(string);
		if ((int)SteamAPI.InitEx(ref text) != 0)
		{
			log.LogError((object)("Steamworks.NET: Init failed:\n\t" + text));
			return;
		}
		STEAM_APPID = SteamUtils.GetAppID().m_AppId;
		APP_IS_DEMO = STEAM_APPID == 4576510;
		APP_IS_PLAYTEST = STEAM_APPID == 4584420;
		APP_IS_PIRATE = STEAM_APPID == 480;
		Loaded = true;
		try
		{
			Init();
			ComponentHolderProtocol.GetOrAddComponent<KSteam>((Object)(object)Plugin.s);
		}
		catch (Exception ex)
		{
			KrokoshaScavMultiplayer.DoMultiplayerStatusMessageError("FAILED TO INIT STEAM:\n" + ex.ToString());
			Loaded = false;
			return;
		}
		KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog($"Steamworks initialized!\n{GetLocalUsername()} - {GetLocalUserSteamID()}");
	}

	private static void LoadFriendList()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		Friends.Clear();
		int friendCount = SteamFriends.GetFriendCount((EFriendFlags)4);
		for (int i = 0; i < friendCount; i++)
		{
			CSteamID friendByIndex = SteamFriends.GetFriendByIndex(i, (EFriendFlags)4);
			Friends.Add(friendByIndex.m_SteamID);
		}
	}

	private void Update()
	{
		if (Loaded)
		{
			SteamAPI.RunCallbacks();
		}
	}

	protected void OnDestroy()
	{
		if (Loaded)
		{
			SteamAPI.Shutdown();
			Loaded = false;
		}
	}

	protected void OnEnable()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		if (Loaded)
		{
			SteamClient.SetWarningMessageHook(new SteamAPIWarningMessageHook_t(SteamAPIDebugTextHook));
		}
	}

	public static string GetLocalUsername()
	{
		return SteamFriends.GetPersonaName();
	}

	public static CSteamID GetLocalUserSteamID()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return SteamUser.GetSteamID();
	}

	[MonoPInvokeCallback(typeof(SteamAPIWarningMessageHook_t))]
	internal static void SteamAPIDebugTextHook(int nSeverity, StringBuilder pchDebugText)
	{
		log.LogWarning((object)pchDebugText);
	}

	public static string GetSteamUsername(ulong steamuserid, bool force_reload_name = false)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (steamuserid == 0L)
		{
			return "[UNKNOWN]";
		}
		if (SteamNamesCache.TryGetValue(steamuserid, out var value))
		{
			return value;
		}
		if (SteamFriends.RequestUserInformation((CSteamID)steamuserid, true))
		{
			value = steamuserid.ToString();
		}
		else
		{
			value = SteamFriends.GetFriendPersonaName((CSteamID)steamuserid);
			if (value == "" || value == "[unknown]")
			{
				value = steamuserid.ToString();
			}
			else
			{
				SteamNamesCache[steamuserid] = value;
			}
		}
		return value;
	}

	public static Texture2D SteamLoadImage(int imageId)
	{
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Expected O, but got Unknown
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		switch (imageId)
		{
		case -1:
			return null;
		case 0:
			return null;
		default:
		{
			if (SteamImagesCache.TryGetValue(imageId, out var value))
			{
				return value;
			}
			uint num = default(uint);
			uint num2 = default(uint);
			if (!SteamUtils.GetImageSize(imageId, ref num, ref num2))
			{
				return null;
			}
			byte[] array = new byte[num * num2 * 4];
			if (!SteamUtils.GetImageRGBA(imageId, array, (int)(num * num2 * 4)))
			{
				return null;
			}
			Color32[] array2 = (Color32[])(object)new Color32[num * num2];
			int num3 = (int)(num * 4);
			for (int i = 0; i < num2; i++)
			{
				int num4 = (int)(num2 - 1 - i) * num3;
				int num5 = i * (int)num;
				for (int j = 0; j < num; j++)
				{
					int num6 = num4 + j * 4;
					array2[num5 + j] = new Color32(array[num6], array[num6 + 1], array[num6 + 2], array[num6 + 3]);
				}
			}
			value = new Texture2D((int)num, (int)num2, (TextureFormat)4, false, true);
			value.SetPixels32(array2);
			value.Apply();
			SteamImagesCache[imageId] = value;
			return value;
		}
		}
	}

	public static void LoadPlayerProfilePics(NetPlayer plr)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		CSteamID steamid = (CSteamID)plr.steam_id;
		if (steamid.m_SteamID == 0L)
		{
			return;
		}
		if ((Object)(object)plr.profilepic_largeicon == (Object)null)
		{
			Util.DelayCallLambda(Random.value, (Action)delegate
			{
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				if ((Object)(object)plr != (Object)null && (Object)(object)plr.profilepic_largeicon == (Object)null)
				{
					plr.profilepic_largeicon = SteamLoadImage(SteamFriends.GetLargeFriendAvatar(steamid));
				}
			});
		}
		if ((Object)(object)plr.profilepic_mediumicon == (Object)null)
		{
			Util.DelayCallLambda(Random.value, (Action)delegate
			{
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				if ((Object)(object)plr != (Object)null && (Object)(object)plr.profilepic_mediumicon == (Object)null)
				{
					plr.profilepic_mediumicon = SteamLoadImage(SteamFriends.GetMediumFriendAvatar(steamid));
				}
			});
		}
		if (!((Object)(object)plr.profilepic_smallicon == (Object)null))
		{
			return;
		}
		Util.DelayCallLambda(Random.value, (Action)delegate
		{
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)plr != (Object)null && (Object)(object)plr.profilepic_smallicon == (Object)null)
			{
				plr.profilepic_smallicon = SteamLoadImage(SteamFriends.GetSmallFriendAvatar(steamid));
			}
		});
	}

	public static void Server_UpdateLobbyData()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Invalid comparison between Unknown and I4
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Expected O, but got Unknown
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Expected O, but got Unknown
		//IL_0267: Expected O, but got Unknown
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Expected O, but got Unknown
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		Net.MY_SERVER_INFO.SetValues();
		CURRENT_LOBBY.locked = Net.MY_SERVER_INFO.haspassword;
		if ((int)NumToLobbyType(UIMainMenu._STEAM_CHOSEN_LOBBYTYPE) != 2)
		{
			CURRENT_LOBBY.locked = true;
		}
		int num = 0;
		if (Net.TryGetSteamTransport(out var tsteam))
		{
			num = tsteam.hostLobbyBucket;
		}
		SteamMatchmaking.SetLobbyMemberLimit(lobbyId, (int)KrokoshaScavMultiplayer.rules.PLAYER_COUNT_LIMIT);
		SteamMatchmaking.SetLobbyData(lobbyId, "CASUALTIESUNKNOWN_KROKOSHA_MULTIPLAYER_COOP_MOD_LOBBYNAME", Net.MY_SERVER_INFO.name);
		SteamMatchmaking.SetLobbyData(lobbyId, "CASUALTIESUNKNOWN_KROKOSHA_MULTIPLAYER_COOP_MOD_VERSION", KrokoshaScavMultiplayer.FULL_VERSION_TAG);
		SteamMatchmaking.SetLobbyData(lobbyId, "CASUALTIESUNKNOWN_KROKOSHA_MULTIPLAYER_COOP_MOD_GAMEMODE", Net.MY_SERVER_INFO.gamemode);
		SteamMatchmaking.SetLobbyData(lobbyId, "CASUALTIESUNKNOWN_KROKOSHA_MULTIPLAYER_COOP_MOD_CURRENTLAYER", Net.MY_SERVER_INFO.cur_layer.ToString());
		SteamMatchmaking.SetLobbyData(lobbyId, "CASUALTIESUNKNOWN_KROKOSHA_MULTIPLAYER_COOP_MOD_AVGMOOD", Net.MY_SERVER_INFO.avg_happiness.ToString());
		SteamMatchmaking.SetLobbyData(lobbyId, "CASUALTIESUNKNOWN_KROKOSHA_MULTIPLAYER_COOP_MOD_DEPTH", Net.MY_SERVER_INFO.cur_depth.ToString());
		SteamMatchmaking.SetLobbyData(lobbyId, "CASUALTIESUNKNOWN_KROKOSHA_MULTIPLAYER_COOP_MOD_LIVINGCOUNT", Net.MY_SERVER_INFO.plr_living_count.ToString());
		SteamMatchmaking.SetLobbyData(lobbyId, "CASUALTIESUNKNOWN_KROKOSHA_MULTIPLAYER_COOP_MOD_PLRCOUNT", Net.MY_SERVER_INFO.plr_count.ToString());
		SteamMatchmaking.SetLobbyData(lobbyId, "CASUALTIESUNKNOWN_KROKOSHA_MULTIPLAYER_COOP_MOD_HASPASSWORD", string.IsNullOrEmpty(KrokoshaScavMultiplayer.INPUT_PASSWORD) ? "0" : "1");
		SteamMatchmaking.SetLobbyData(lobbyId, "CASUALTIESUNKNOWN_KROKOSHA_MULTIPLAYER_COOP_MOD_ISDEDICATED", Net.is_dedicated_server ? "1" : "0");
		SteamMatchmaking.SetLobbyData(lobbyId, "CASUALTIESUNKNOWN_KROKOSHA_MULTIPLAYER_COOP_MOD_LOCKED", Net.MY_SERVER_INFO.midjoin_lock_active ? "1" : "0");
		SteamMatchmaking.SetLobbyData(lobbyId, "CASUALTIESUNKNOWN_KROKOSHA_MULTIPLAYER_COOP_MOD_RUNSETTINGS", Net.MY_SERVER_INFO.midjoin_lock_active ? "1" : "0");
		SteamMatchmaking.SetLobbyData(lobbyId, "bucket", num.ToString());
		SteamServerInfoInLobbyExtraData value = new SteamServerInfoInLobbyExtraData(Net.MY_SERVER_INFO);
		NetDataWriter val = new NetDataWriter();
		MyLiteNetLibExtensions.PutUnmanaged(val, value);
		val.PutArray(NetPlayer.ClientIdToPlayerDict.Values.Select((NetPlayer x) => x.steam_id).ToArray());
		val.Put(KrokoshaScavMultiplayer.SERVER_TOGGLE_ENFORCE_MODLIST);
		val.PutArray(KrokoshaScavMultiplayer.GetModListGUIDs());
		MyLiteNetLibExtensions.CompressWriter(val, 0);
		NetDataReader val2 = new NetDataReader(val);
		SteamMatchmaking.SetLobbyData(lobbyId, "CASUALTIESUNKNOWN_KROKOSHA_MULTIPLAYER_COOP_MOD_EXTRADATA", Convert.ToBase64String(val2.GetRemainingBytes()));
	}

	public static NetPublicServerInfo Client_LobbyBrowser_ReadLobbyIntoServerInfos(Lobby slobby)
	{
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Expected O, but got Unknown
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			string version = slobby.metadata["CASUALTIESUNKNOWN_KROKOSHA_MULTIPLAYER_COOP_MOD_VERSION"];
			string text = slobby.metadata["CASUALTIESUNKNOWN_KROKOSHA_MULTIPLAYER_COOP_MOD_LOBBYNAME"];
			string gamemode = slobby.metadata["CASUALTIESUNKNOWN_KROKOSHA_MULTIPLAYER_COOP_MOD_GAMEMODE"];
			string s = slobby.metadata["CASUALTIESUNKNOWN_KROKOSHA_MULTIPLAYER_COOP_MOD_CURRENTLAYER"];
			string s2 = slobby.metadata["CASUALTIESUNKNOWN_KROKOSHA_MULTIPLAYER_COOP_MOD_DEPTH"];
			string s3 = slobby.metadata["CASUALTIESUNKNOWN_KROKOSHA_MULTIPLAYER_COOP_MOD_LIVINGCOUNT"];
			string s4 = slobby.metadata["CASUALTIESUNKNOWN_KROKOSHA_MULTIPLAYER_COOP_MOD_PLRCOUNT"];
			string s5 = slobby.metadata["CASUALTIESUNKNOWN_KROKOSHA_MULTIPLAYER_COOP_MOD_AVGMOOD"];
			string s6 = slobby.metadata["CASUALTIESUNKNOWN_KROKOSHA_MULTIPLAYER_COOP_MOD_EXTRADATA"];
			string text2 = slobby.metadata["CASUALTIESUNKNOWN_KROKOSHA_MULTIPLAYER_COOP_MOD_HASPASSWORD"];
			string text3 = slobby.metadata["CASUALTIESUNKNOWN_KROKOSHA_MULTIPLAYER_COOP_MOD_ISDEDICATED"];
			string text4 = slobby.metadata["CASUALTIESUNKNOWN_KROKOSHA_MULTIPLAYER_COOP_MOD_LOCKED"];
			if (text.Length > 32)
			{
				return null;
			}
			if (KrokoshaScavMultiplayer.SanitizeTextInputAllowSpaces(text) != text)
			{
				text = $"Expedition {slobby.lobby_steamID}";
			}
			NetPublicServerInfo netPublicServerInfo = new NetPublicServerInfo();
			netPublicServerInfo.steam_lobby_info = slobby;
			netPublicServerInfo.name = text;
			netPublicServerInfo.version = version;
			netPublicServerInfo.haspassword = text2 != "0";
			netPublicServerInfo.dedicated = text3 != "0";
			netPublicServerInfo.midjoin_lock_active = text4 != "0";
			netPublicServerInfo.gamemode = gamemode;
			netPublicServerInfo.cur_layer = int.Parse(s);
			netPublicServerInfo.avg_happiness = int.Parse(s5);
			netPublicServerInfo.cur_depth = int.Parse(s2);
			netPublicServerInfo.plr_living_count = int.Parse(s3);
			netPublicServerInfo.plr_max = slobby.memberlimit;
			netPublicServerInfo.plr_count = int.Parse(s4);
			if (netPublicServerInfo.cur_layer > 4 && STEAM_APPID == 4576510 && CURRENT_LOBBY.lobby_steamID != slobby.lobby_steamID)
			{
				return null;
			}
			try
			{
				NetDataReader r = new NetDataReader(Convert.FromBase64String(s6));
				r = r.DecompressReader();
				r.GetUnmanaged<SteamServerInfoInLobbyExtraData>(out var result);
				result.Read(netPublicServerInfo);
				ulong[] uLongArray = r.GetULongArray();
				foreach (ulong num in uLongArray)
				{
					if (!slobby.members.ContainsKey((CSteamID)num))
					{
						LobbyMember lobbyMember = new LobbyMember();
						lobbyMember.steamID = (CSteamID)num;
						slobby.members[(CSteamID)num] = lobbyMember;
					}
				}
				bool num2 = r.GetBool();
				netPublicServerInfo.modlist = r.GetStringArray();
				if (num2)
				{
					string[] modListGUIDs = KrokoshaScavMultiplayer.GetModListGUIDs();
					netPublicServerInfo.enforcemodlist_locked = !new HashSet<string>(modListGUIDs).SetEquals(netPublicServerInfo.modlist);
				}
			}
			catch (Exception)
			{
			}
			return netPublicServerInfo;
		}
		catch (Exception)
		{
		}
		return null;
	}

	public static void UpdateRichPresence()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		if (Net.running)
		{
			SteamFriends.SetRichPresence("steam_player_group", ((object)lobbyId/*cast due to constrained. prefix*/).ToString());
			SteamFriends.SetRichPresence("steam_player_group_size", NetPlayer.ClientIdToPlayerDict.Count.ToString());
		}
		else
		{
			SteamFriends.SetRichPresence("steam_player_group", (string)null);
			SteamFriends.SetRichPresence("steam_player_group_size", (string)null);
			SteamFriends.ClearRichPresence();
		}
	}

	public static void SearchLobbies(bool do_buckets)
	{
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		if (!Loaded)
		{
			return;
		}
		if (Net.cur_server_info.cur_layer <= 4 && STEAM_APPID == 4576510)
		{
			SteamMatchmaking.AddRequestLobbyListNumericalFilter("CASUALTIESUNKNOWN_KROKOSHA_MULTIPLAYER_COOP_MOD_CURRENTLAYER", 4, (ELobbyComparison)(-2));
		}
		if (do_buckets)
		{
			SteamMatchmaking.AddRequestLobbyListStringFilter("bucket", LobbySearch_BucketCounter.ToString(), (ELobbyComparison)0);
		}
		else
		{
			LobbySearch_BucketCounter = 0;
			if (UIServerBrowser.SortHidePasswordProtected)
			{
				SteamMatchmaking.AddRequestLobbyListStringFilter("CASUALTIESUNKNOWN_KROKOSHA_MULTIPLAYER_COOP_MOD_HASPASSWORD", "0", (ELobbyComparison)0);
			}
			if (UIServerBrowser.SortHideIncompatible)
			{
				SteamMatchmaking.AddRequestLobbyListStringFilter("CASUALTIESUNKNOWN_KROKOSHA_MULTIPLAYER_COOP_MOD_VERSION", KrokoshaScavMultiplayer.FULL_VERSION_TAG, (ELobbyComparison)0);
			}
		}
		SteamMatchmaking.AddRequestLobbyListDistanceFilter(userselected_lobbyDistanceFilter);
		SteamMatchmaking.AddRequestLobbyListResultCountFilter((int)userselected_maxLobbies);
		SteamAPICall_t val = SteamMatchmaking.RequestLobbyList();
		OnLobbyMatchListCallResult.Set(val, (APIDispatchDelegate<LobbyMatchList_t>)null);
	}

	public static void SearchFriendLobbies()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		LoadFriendList();
		FriendGameInfo_t val = default(FriendGameInfo_t);
		foreach (ulong friend in Friends)
		{
			if (SteamFriends.GetFriendGamePlayed((CSteamID)friend, ref val) && ((CGameID)(ref val.m_gameID)).AppID().m_AppId == STEAM_APPID && ((CSteamID)(ref val.m_steamIDLobby)).IsValid() && !UIServerBrowser.CurServerList_ButOnlySteamLobbies.TryGetValue(val.m_steamIDLobby.m_SteamID, out var value))
			{
				Lobby outLobby = new Lobby();
				UpdateLobbyInfo(val.m_steamIDLobby, ref outLobby);
				value = Client_LobbyBrowser_ReadLobbyIntoServerInfos(outLobby);
				if (value != null)
				{
					UIServerBrowser.AddServerToTheList(value);
				}
			}
		}
	}

	private unsafe static void OnFavoritesListChanged(FavoritesListChanged_t pCallback)
	{
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		string[] obj = new string[16]
		{
			"[",
			502.ToString(),
			" - FavoritesListChanged] - ",
			pCallback.m_nIP.ToString(),
			" -- ",
			pCallback.m_nQueryPort.ToString(),
			" -- ",
			pCallback.m_nConnPort.ToString(),
			" -- ",
			pCallback.m_nAppID.ToString(),
			" -- ",
			pCallback.m_nFlags.ToString(),
			" -- ",
			pCallback.m_bAdd.ToString(),
			" -- ",
			null
		};
		AccountID_t unAccountId = pCallback.m_unAccountId;
		obj[15] = ((object)(*(AccountID_t*)(&unAccountId))/*cast due to constrained. prefix*/).ToString();
		steamtestlog(string.Concat(obj));
	}

	private static void OnLobbyInvite(LobbyInvite_t pCallback)
	{
		steamtestlog("[" + 503 + " - LobbyInvite] - " + pCallback.m_ulSteamIDUser + " -- " + pCallback.m_ulSteamIDLobby + " -- " + pCallback.m_ulGameID);
	}

	private unsafe static void OnLobbyEnter(LobbyEnter_t pCallback)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Invalid comparison between Unknown and I4
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Invalid comparison between Unknown and I4
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		EChatRoomEnterResponse val = (EChatRoomEnterResponse)pCallback.m_EChatRoomEnterResponse;
		steamtestlog("[" + 504 + " - LobbyEnter] - " + pCallback.m_ulSteamIDLobby + " -- " + pCallback.m_rgfChatPermissions + " -- " + pCallback.m_bLocked + " -- " + ((object)(*(EChatRoomEnterResponse*)(&val))/*cast due to constrained. prefix*/).ToString());
		if (!Net.TryGetSteamTransport(out var tsteam))
		{
			LeaveLobbyAndResetNet((CSteamID)pCallback.m_ulSteamIDLobby);
			IS_IN_LOBBY = false;
			if (KrokoshaCasualtiesMP.log.verbose && (int)val != 2)
			{
				KrokoshaCasualtiesMP.log.error("STEAM: Received OnLobbyEnter without a transport !!!");
			}
			return;
		}
		if ((int)val != 1)
		{
			string text = ((object)(*(EChatRoomEnterResponse*)(&val))/*cast due to constrained. prefix*/).ToString();
			text = StringUtility.TrimStart(text, "k_EChatRoomEnterResponse");
			KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog("OnLobbyEnter failed! response: " + text);
			if (!IS_DOING_HostLobbyRecreatorLoop)
			{
				IS_DOING_HostLobbyRecreatorLoop = false;
				LeaveLobbyAndResetNet((CSteamID)pCallback.m_ulSteamIDLobby);
				IS_IN_LOBBY = false;
			}
			return;
		}
		CURRENT_LOBBY.locked = Net.cur_server_info.haspassword || pCallback.m_bLocked;
		UpdateLobbyInfo((CSteamID)pCallback.m_ulSteamIDLobby, ref CURRENT_LOBBY);
		if (Net.is_server)
		{
			if (tsteam.CreateServerSocket())
			{
				IS_DOING_HostLobbyRecreatorLoop = false;
				CURRENT_LOBBY.locked = false;
			}
			else
			{
				LeaveLobbyAndResetNet((CSteamID)pCallback.m_ulSteamIDLobby);
			}
		}
	}

	private unsafe static void OnLobbyEnter_APIDispatch(LobbyEnter_t pCallback, bool bIOFailure)
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Invalid comparison between Unknown and I4
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Invalid comparison between Unknown and I4
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			steamtestlog("[" + 504 + " - LobbyEnterAPIDispatch] - " + pCallback.m_ulSteamIDLobby + " -- " + pCallback.m_rgfChatPermissions + " -- " + pCallback.m_bLocked + " -- " + ((object)(EChatRoomEnterResponse)pCallback.m_EChatRoomEnterResponse/*cast due to constrained. prefix*/).ToString());
			if (bIOFailure)
			{
				KrokoshaScavMultiplayer.DoMultiplayerStatusMessageError("STEAM: OnLobbyEnter IOFailure: " + ((object)SteamUtils.GetAPICallFailureReason(OnLobbyEnterCallResult.Handle)/*cast due to constrained. prefix*/).ToString());
				IS_IN_LOBBY = false;
				Net.ShutdownReset();
				return;
			}
			EChatRoomEnterResponse val = (EChatRoomEnterResponse)pCallback.m_EChatRoomEnterResponse;
			string text = StringUtility.TrimStart(((object)(*(EChatRoomEnterResponse*)(&val))/*cast due to constrained. prefix*/).ToString(), "k_EChatRoomEnterResponse");
			if ((int)val != 1)
			{
				if ((int)val == 2)
				{
					KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog("STEAM: Lobby Enter Rejected: Lobby does not exist!");
				}
				else
				{
					KrokoshaScavMultiplayer.DoMultiplayerStatusMessageError("STEAM: Lobby Enter Rejected: " + text);
				}
				IS_IN_LOBBY = false;
				Util.PlayUISound((UISoundType)6);
				Net.ShutdownReset();
				return;
			}
			if (!Net.TryGetSteamTransport(out var tsteam))
			{
				LeaveLobbyAndResetNet((CSteamID)pCallback.m_ulSteamIDLobby);
				IS_IN_LOBBY = false;
				if (KrokoshaCasualtiesMP.log.verbose)
				{
					KrokoshaCasualtiesMP.log.error("STEAM: Received OnLobbyEnter_APIDispatch without a transport !!!");
				}
				return;
			}
			CURRENT_LOBBY.locked = Net.cur_server_info.haspassword || pCallback.m_bLocked;
			UpdateLobbyInfo((CSteamID)pCallback.m_ulSteamIDLobby, ref CURRENT_LOBBY);
			IS_IN_LOBBY = true;
			KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog("Steam: OnLobbyEnter");
			UpdateRichPresence();
			if (Net.is_client)
			{
				if (tsteam.CreateClientSocket())
				{
					NetDataWriter val2 = KrokoshaScavMultiplayer.CreateClientConnectIntroductionPacket();
					tsteam.SendThroughSteamSocket(CURRENT_LOBBY.ownerID.m_SteamID, new ArraySegment<byte>(val2.Data), 8);
				}
				else
				{
					LeaveLobbyAndResetNet((CSteamID)pCallback.m_ulSteamIDLobby);
					IS_IN_LOBBY = false;
				}
			}
		}
		catch (Exception ex)
		{
			KrokoshaCasualtiesMP.log.error("STEAM: OnLobbyEnter_APIDispatch: " + ex.ToString());
		}
	}

	private static void OnLobbyDataUpdate(LobbyDataUpdate_t pCallback)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		CSteamID ownerID = CURRENT_LOBBY.ownerID;
		UpdateLobbyInfo((CSteamID)pCallback.m_ulSteamIDLobby, ref CURRENT_LOBBY);
		CURRENT_LOBBY.ownerID = ownerID;
	}

	private unsafe static void OnLobbyChatUpdate(LobbyChatUpdate_t pCallback)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Invalid comparison between Unknown and I4
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Invalid comparison between Unknown and I4
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Invalid comparison between Unknown and I4
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Invalid comparison between Unknown and I4
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Invalid comparison between Unknown and I4
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			EChatMemberStateChange val = (EChatMemberStateChange)pCallback.m_rgfChatMemberStateChange;
			if (pCallback.m_ulSteamIDUserChanged == pCallback.m_ulSteamIDMakingChange)
			{
				steamtestlog("[" + 506 + " - LobbyChatUpdate] - " + pCallback.m_ulSteamIDLobby + " -- " + TransportSteamworks.BetterToStringFromSteamID(pCallback.m_ulSteamIDUserChanged) + " -- " + ((object)(*(EChatMemberStateChange*)(&val))/*cast due to constrained. prefix*/).ToString(), forcelog: true);
			}
			else
			{
				steamtestlog("[" + 506 + " - LobbyChatUpdate] - " + pCallback.m_ulSteamIDLobby + " -- " + TransportSteamworks.BetterToStringFromSteamID(pCallback.m_ulSteamIDUserChanged) + " -- " + pCallback.m_ulSteamIDMakingChange + " -- " + ((object)(*(EChatMemberStateChange*)(&val))/*cast due to constrained. prefix*/).ToString(), forcelog: true);
			}
			if (!IS_IN_LOBBY || !Net.TryGetSteamTransport(out var tsteam))
			{
				if (KrokoshaScavMultiplayer.verbose)
				{
					KrokoshaCasualtiesMP.log.warn("Received OnLobbyChatUpdate while transport is not active, what?");
				}
				UpdateLobbyInfo((CSteamID)pCallback.m_ulSteamIDLobby, ref CURRENT_LOBBY);
				return;
			}
			ulong ulSteamIDUserChanged = pCallback.m_ulSteamIDUserChanged;
			bool num = (val & 1) > 0;
			bool flag = (val & 2) > 0;
			bool flag2 = (val & 4) > 0;
			bool flag3 = (val & 8) > 0;
			bool flag4 = (val & 0x10) > 0;
			ulong steamID = SteamUser.GetSteamID().m_SteamID;
			bool flag5 = ulSteamIDUserChanged == steamID;
			if (num && !flag5)
			{
				if (tsteam.i_am_owner_of_this_lobby && BanList.IsBanned(pCallback.m_ulSteamIDUserChanged))
				{
					tsteam.KickMember((CSteamID)pCallback.m_ulSteamIDUserChanged, "Banned!");
					tsteam.RemoveSteamUser(pCallback.m_ulSteamIDUserChanged);
				}
				else
				{
					SteamFriends.RequestUserInformation((CSteamID)pCallback.m_ulSteamIDUserChanged, true);
				}
			}
			if (flag || flag2 || flag3 || flag4)
			{
				if (flag5 && tsteam.i_am_owner_of_this_lobby)
				{
					KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog("I got removed from the lobby. But im the owner!\nAttempting to recreate lobby in a sec...");
					CURRENT_LOBBY.lobby_steamID = CSteamID.Nil;
					tsteam.Shutdown(reset_net: false);
					Util.StartCoroutine(HostLobbyRecreatorLoop);
					return;
				}
				if (flag5)
				{
					KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog("I got removed from the lobby.");
					if (Util.IsInWorld())
					{
						KrokoshaScavMultiplayer.showMultiplayerMenu = true;
						PlayerCamera.main.ToMainMenu();
					}
					UpdateLobbyInfo((CSteamID)pCallback.m_ulSteamIDLobby, ref CURRENT_LOBBY);
					LeaveLobbyAndResetNet();
					return;
				}
				tsteam.RemoveSteamUser(pCallback.m_ulSteamIDUserChanged);
				if (Net.is_client && pCallback.m_ulSteamIDUserChanged == CURRENT_LOBBY.ownerID.m_SteamID)
				{
					KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog("LOBBY OWNER LEFT, LEAVING");
					if (Util.IsInWorld())
					{
						KrokoshaScavMultiplayer.showMultiplayerMenu = true;
						PlayerCamera.main.ToMainMenu();
					}
					LeaveLobbyAndResetNet();
				}
			}
		}
		catch (Exception ex)
		{
			KrokoshaCasualtiesMP.log.error("STEAM: LobbyChatUpdate: " + ex.ToString());
		}
		UpdateLobbyInfo((CSteamID)pCallback.m_ulSteamIDLobby, ref CURRENT_LOBBY);
	}

	private unsafe static void OnLobbyChatMsg(LobbyChatMsg_t pCallback)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		steamtestlog("[" + 507 + " - LobbyChatMsg] - " + pCallback.m_ulSteamIDLobby + " -- " + TransportSteamworks.BetterToStringFromSteamID(pCallback.m_ulSteamIDUser) + " -- " + ((object)(EChatEntryType)pCallback.m_eChatEntryType/*cast due to constrained. prefix*/).ToString() + " -- " + pCallback.m_iChatID);
		byte[] array = new byte[4096];
		CSteamID val = default(CSteamID);
		EChatEntryType val2 = default(EChatEntryType);
		int lobbyChatEntry = SteamMatchmaking.GetLobbyChatEntry((CSteamID)pCallback.m_ulSteamIDLobby, (int)pCallback.m_iChatID, ref val, array, array.Length, ref val2);
		string[] obj = new string[12]
		{
			"GetLobbyChatEntry(",
			((object)(CSteamID)pCallback.m_ulSteamIDLobby/*cast due to constrained. prefix*/).ToString(),
			", ",
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null
		};
		int iChatID = (int)pCallback.m_iChatID;
		obj[3] = iChatID.ToString();
		obj[4] = ", out SteamIDUser, Data, Data.Length, out ChatEntryType) : ";
		obj[5] = lobbyChatEntry.ToString();
		obj[6] = " -- ";
		CSteamID val3 = val;
		obj[7] = ((object)(*(CSteamID*)(&val3))/*cast due to constrained. prefix*/).ToString();
		obj[8] = " -- ";
		obj[9] = Encoding.UTF8.GetString(array, 0, lobbyChatEntry).Trim();
		obj[10] = " -- ";
		obj[11] = ((object)(*(EChatEntryType*)(&val2))/*cast due to constrained. prefix*/).ToString();
		steamtestlog(string.Concat(obj));
		string text = Encoding.UTF8.GetString(array, 0, lobbyChatEntry);
		if (text.StartsWith("KICK:") && pCallback.m_ulSteamIDUser == CURRENT_LOBBY.ownerID.m_SteamID)
		{
			string text2 = "u got kicked for some reson";
			text = text.Substring(5);
			int num = text.IndexOf(':');
			if (num != -1)
			{
				text2 = text.Substring(num + 1);
				text = text.Substring(0, num);
			}
			if (ulong.Parse(text) == SteamUser.GetSteamID().m_SteamID)
			{
				KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog("STEAM: Kicked: " + text2);
				LeaveLobbyAndResetNet();
			}
		}
	}

	private static void OnLobbyGameCreated(LobbyGameCreated_t pCallback)
	{
		steamtestlog("[" + 509 + " - LobbyGameCreated] - " + pCallback.m_ulSteamIDLobby + " -- " + pCallback.m_ulSteamIDGameServer + " -- " + pCallback.m_unIP + " -- " + pCallback.m_usPort);
	}

	private static void OnLobbyMatchList(LobbyMatchList_t pCallback, bool bIOFailure)
	{
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		LOBBIES.Clear();
		if (KrokoshaCasualtiesMP.log.verbose)
		{
			steamtestlog("[" + 510 + " - LobbyMatchList] - " + pCallback.m_nLobbiesMatching + "  " + ((LobbySearch_BucketCounter == 0) ? "" : (LobbySearch_BucketCounter - 1).ToString()));
		}
		if (bIOFailure)
		{
			KrokoshaCasualtiesMP.log.error("OnLobbyMatchList encountered an IOFailure due to: " + ((object)SteamUtils.GetAPICallFailureReason(OnLobbyMatchListCallResult.Handle)/*cast due to constrained. prefix*/).ToString());
			return;
		}
		try
		{
			if (pCallback.m_nLobbiesMatching != 0)
			{
				for (int i = 0; i < pCallback.m_nLobbiesMatching; i++)
				{
					Lobby outLobby = new Lobby();
					UpdateLobbyInfo(SteamMatchmaking.GetLobbyByIndex(i), ref outLobby);
					LOBBIES.Add(outLobby);
					NetPublicServerInfo netPublicServerInfo = Client_LobbyBrowser_ReadLobbyIntoServerInfos(outLobby);
					if (netPublicServerInfo != null)
					{
						UIServerBrowser.AddServerToTheList(netPublicServerInfo);
					}
				}
			}
			if (LobbySearch_BucketCounter == 0)
			{
				SearchFriendLobbies();
			}
			if (UIServerBrowser.CurServerCount > 35 && LobbySearch_BucketCounter <= 20)
			{
				Util.DelayCallLambda(0.2f, (Action)delegate
				{
					SearchLobbies(do_buckets: true);
					LobbySearch_BucketCounter++;
				});
			}
		}
		catch (Exception ex)
		{
			KrokoshaCasualtiesMP.log.error("STEAM: LobbyMatchList: " + ex.ToString());
		}
	}

	private static void OnLobbyKicked(LobbyKicked_t pCallback)
	{
		steamtestlog("[" + 512 + " - LobbyKicked] - " + pCallback.m_ulSteamIDLobby + " -- " + pCallback.m_ulSteamIDAdmin + " -- " + pCallback.m_bKickedDueToDisconnect);
	}

	private unsafe static void OnLobbyCreated(LobbyCreated_t pCallback, bool bIOFailure)
	{
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Invalid comparison between Unknown and I4
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Invalid comparison between Unknown and I4
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		steamtestlog("[" + 513 + " - LobbyCreated] - " + ((object)(*(EResult*)(&pCallback.m_eResult))/*cast due to constrained. prefix*/).ToString() + " -- " + pCallback.m_ulSteamIDLobby);
		if (bIOFailure)
		{
			KrokoshaScavMultiplayer.DoMultiplayerStatusMessageError("STEAM: OnLobbyCreated encountered an IOFailure due to: " + ((object)SteamUtils.GetAPICallFailureReason(OnLobbyCreatedCallResult.Handle)/*cast due to constrained. prefix*/).ToString());
			IS_IN_LOBBY = false;
			Net.ShutdownReset();
			Util.PlayUISound((UISoundType)6);
			return;
		}
		if ((int)pCallback.m_eResult != 1)
		{
			Util.PlayUISound((UISoundType)6);
			Net.ShutdownReset();
			IS_IN_LOBBY = false;
			if ((int)pCallback.m_eResult == 3)
			{
				KrokoshaScavMultiplayer.DoMultiplayerStatusMessageError("Steam: Create Lobby FAILED: No internet connection.");
			}
			else
			{
				KrokoshaScavMultiplayer.DoMultiplayerStatusMessageError($"Steam: Create Lobby FAILED: {pCallback.m_eResult}");
			}
			return;
		}
		if (Net.TRANSPORT == null || !(Net.TRANSPORT is TransportSteamworks))
		{
			KrokoshaScavMultiplayer.DoMultiplayerStatusMessageError("CREATED LOBBY BUT THERES NO STEAMWORKS TRASNPORT WHAAAAT??!?!?!!??");
			IS_IN_LOBBY = false;
			Net.ShutdownReset();
			return;
		}
		UpdateLobbyInfo((CSteamID)pCallback.m_ulSteamIDLobby, ref CURRENT_LOBBY);
		IS_IN_LOBBY = true;
		int num = 0;
		if (Net.TryGetSteamTransport(out var tsteam))
		{
			num = tsteam.hostLobbyBucket;
		}
		if (Net.is_host && (Object)(object)NetPlayer.LOCAL_PLAYER == (Object)null)
		{
			NetPlayer.LOCAL_PLAYER = Net.CreatePlayer((ushort)0, GetLocalUsername(), UIMainMenu.LAST_VALID_INPUT_COLOR);
			NetPlayer.LOCAL_PLAYER.steam_id = GetLocalUserSteamID().m_SteamID;
			tsteam.SteamIDToNetPlayerDict[NetPlayer.LOCAL_PLAYER.steam_id] = NetPlayer.LOCAL_PLAYER;
		}
		SteamMatchmaking.SetLobbyData(lobbyId, "CASUALTIESUNKNOWN_KROKOSHA_MULTIPLAYER_COOP_MOD_VERSION", KrokoshaScavMultiplayer.FULL_VERSION_TAG);
		SteamMatchmaking.SetLobbyData(lobbyId, "bucket", num.ToString());
		Util.DelayCallLambda(0.5f, (Action)delegate
		{
			if (IS_IN_LOBBY)
			{
				Server_UpdateLobbyData();
			}
		});
		KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog("STEAM: Lobby Created :)");
		UpdateRichPresence();
	}

	private unsafe static void OnFavoritesListAccountsUpdated(FavoritesListAccountsUpdated_t pCallback)
	{
		steamtestlog("[" + 516 + " - FavoritesListAccountsUpdated] - " + ((object)(*(EResult*)(&pCallback.m_eResult))/*cast due to constrained. prefix*/).ToString());
	}

	private unsafe static void OnJoinPartyCallback(JoinPartyCallback_t pCallback)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		string[] obj = new string[10]
		{
			"[",
			5301.ToString(),
			" - JoinPartyCallback] - ",
			((object)(*(EResult*)(&pCallback.m_eResult))/*cast due to constrained. prefix*/).ToString(),
			" -- ",
			null,
			null,
			null,
			null,
			null
		};
		PartyBeaconID_t ulBeaconID = pCallback.m_ulBeaconID;
		obj[5] = ((object)(*(PartyBeaconID_t*)(&ulBeaconID))/*cast due to constrained. prefix*/).ToString();
		obj[6] = " -- ";
		CSteamID steamIDBeaconOwner = pCallback.m_SteamIDBeaconOwner;
		obj[7] = ((object)(*(CSteamID*)(&steamIDBeaconOwner))/*cast due to constrained. prefix*/).ToString();
		obj[8] = " -- ";
		obj[9] = ((JoinPartyCallback_t)(ref pCallback)).m_rgchConnectString;
		steamtestlog(string.Concat(obj));
	}

	private unsafe static void OnCreateBeaconCallback(CreateBeaconCallback_t pCallback)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		string[] obj = new string[6]
		{
			"[",
			5302.ToString(),
			" - CreateBeaconCallback] - ",
			((object)(*(EResult*)(&pCallback.m_eResult))/*cast due to constrained. prefix*/).ToString(),
			" -- ",
			null
		};
		PartyBeaconID_t ulBeaconID = pCallback.m_ulBeaconID;
		obj[5] = ((object)(*(PartyBeaconID_t*)(&ulBeaconID))/*cast due to constrained. prefix*/).ToString();
		steamtestlog(string.Concat(obj));
	}

	private unsafe static void OnReservationNotificationCallback(ReservationNotificationCallback_t pCallback)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		string[] obj = new string[6]
		{
			"[",
			5303.ToString(),
			" - ReservationNotificationCallback] - ",
			null,
			null,
			null
		};
		PartyBeaconID_t ulBeaconID = pCallback.m_ulBeaconID;
		obj[3] = ((object)(*(PartyBeaconID_t*)(&ulBeaconID))/*cast due to constrained. prefix*/).ToString();
		obj[4] = " -- ";
		CSteamID steamIDJoiner = pCallback.m_steamIDJoiner;
		obj[5] = ((object)(*(CSteamID*)(&steamIDJoiner))/*cast due to constrained. prefix*/).ToString();
		steamtestlog(string.Concat(obj));
	}

	private unsafe static void OnChangeNumOpenSlotsCallback(ChangeNumOpenSlotsCallback_t pCallback)
	{
		steamtestlog("[" + 5304 + " - ChangeNumOpenSlotsCallback] - " + ((object)(*(EResult*)(&pCallback.m_eResult))/*cast due to constrained. prefix*/).ToString());
	}

	private static void OnAvailableBeaconLocationsUpdated(AvailableBeaconLocationsUpdated_t pCallback)
	{
		steamtestlog("[" + 5305 + " - AvailableBeaconLocationsUpdated]");
	}

	private static void OnActiveBeaconsUpdated(ActiveBeaconsUpdated_t pCallback)
	{
		steamtestlog("[" + 5306 + " - ActiveBeaconsUpdated]");
	}

	private unsafe static void OnPersonaStateChange(PersonaStateChange_t pCallback)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		EPersonaChange nChangeFlags = pCallback.m_nChangeFlags;
		CSteamID val = (CSteamID)pCallback.m_ulSteamID;
		if (!Net.TryGetSteamTransport(out var tsteam))
		{
			return;
		}
		if (((Enum)nChangeFlags).HasFlag((Enum)(object)(EPersonaChange)1) || ((Enum)nChangeFlags).HasFlag((Enum)(object)(EPersonaChange)4096) || ((Enum)nChangeFlags).HasFlag((Enum)(object)(EPersonaChange)1024))
		{
			GetSteamUsername(val.m_SteamID, force_reload_name: true);
			if (tsteam.SteamIDToNetPlayerDict.TryGetValue(pCallback.m_ulSteamID, out var value) && !value.nameIsCustom)
			{
				value.ApplyNameAndColor(((object)(*(CSteamID*)(&val))/*cast due to constrained. prefix*/).ToString(), value.plrcolor);
			}
		}
		((Enum)nChangeFlags).HasFlag((Enum)(object)(EPersonaChange)64);
		if (((Enum)nChangeFlags).HasFlag((Enum)(object)(EPersonaChange)512))
		{
			LoadFriendList();
		}
	}

	private unsafe static void OnGameOverlayActivated(GameOverlayActivated_t pCallback)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		string[] obj = new string[10]
		{
			"[",
			331.ToString(),
			" - GameOverlayActivated] - ",
			pCallback.m_bActive.ToString(),
			" -- ",
			pCallback.m_bUserInitiated.ToString(),
			" -- ",
			null,
			null,
			null
		};
		AppId_t nAppID = pCallback.m_nAppID;
		obj[7] = ((object)(*(AppId_t*)(&nAppID))/*cast due to constrained. prefix*/).ToString();
		obj[8] = " -- ";
		obj[9] = pCallback.m_dwOverlayPID.ToString();
		steamtestlog(string.Concat(obj));
		IS_IN_STEAMOVERLAY = pCallback.m_bActive != 0;
	}

	private static void OnGameServerChangeRequested(GameServerChangeRequested_t pCallback)
	{
		steamtestlog("[" + 332 + " - GameServerChangeRequested] - " + ((GameServerChangeRequested_t)(ref pCallback)).m_rgchServer + " -- " + ((GameServerChangeRequested_t)(ref pCallback)).m_rgchPassword);
	}

	private unsafe static void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t pCallback)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		string[] obj = new string[6]
		{
			"[",
			333.ToString(),
			" - GameLobbyJoinRequested] - ",
			null,
			null,
			null
		};
		CSteamID steamIDLobby = pCallback.m_steamIDLobby;
		obj[3] = ((object)(*(CSteamID*)(&steamIDLobby))/*cast due to constrained. prefix*/).ToString();
		obj[4] = " -- ";
		steamIDLobby = pCallback.m_steamIDFriend;
		obj[5] = ((object)(*(CSteamID*)(&steamIDLobby))/*cast due to constrained. prefix*/).ToString();
		steamtestlog(string.Concat(obj));
		if (KrokoshaScavMultiplayer.IsNetworkActiveOrIsInGame())
		{
			KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog("GameLobbyJoinRequested: Nah, Already in a game.");
			return;
		}
		KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog($"GameLobbyJoinRequested: lobbyID:{pCallback.m_steamIDLobby}");
		TransportSteamworks.OnWantToJoinLobby(pCallback.m_steamIDLobby.m_SteamID);
	}

	private unsafe static void OnAvatarImageLoaded(AvatarImageLoaded_t pCallback)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		CSteamID steamID = pCallback.m_steamID;
		string[] obj = new string[10]
		{
			"[",
			334.ToString(),
			" - AvatarImageLoaded] - ",
			null,
			null,
			null,
			null,
			null,
			null,
			null
		};
		CSteamID steamID2 = pCallback.m_steamID;
		obj[3] = ((object)(*(CSteamID*)(&steamID2))/*cast due to constrained. prefix*/).ToString();
		obj[4] = " -- ";
		obj[5] = pCallback.m_iImage.ToString();
		obj[6] = " -- ";
		obj[7] = pCallback.m_iWide.ToString();
		obj[8] = " -- ";
		obj[9] = pCallback.m_iTall.ToString();
		steamtestlog(string.Concat(obj));
		if (Net.TryGetSteamTransport(out var tsteam) && tsteam.SteamIDToNetPlayerDict.TryGetValue((ulong)steamID, out var value))
		{
			LoadPlayerProfilePics(value);
		}
	}

	private unsafe static void OnClanOfficerListResponse(ClanOfficerListResponse_t pCallback, bool bIOFailure)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		string[] obj = new string[8]
		{
			"[",
			335.ToString(),
			" - ClanOfficerListResponse] - ",
			null,
			null,
			null,
			null,
			null
		};
		CSteamID steamIDClan = pCallback.m_steamIDClan;
		obj[3] = ((object)(*(CSteamID*)(&steamIDClan))/*cast due to constrained. prefix*/).ToString();
		obj[4] = " -- ";
		obj[5] = pCallback.m_cOfficers.ToString();
		obj[6] = " -- ";
		obj[7] = pCallback.m_bSuccess.ToString();
		steamtestlog(string.Concat(obj));
	}

	private static void OnFriendRichPresenceUpdate(FriendRichPresenceUpdate_t pCallback)
	{
	}

	private unsafe static void OnGameRichPresenceJoinRequested(GameRichPresenceJoinRequested_t pCallback)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		string[] obj = new string[6]
		{
			"[",
			337.ToString(),
			" - GameRichPresenceJoinRequested] - ",
			null,
			null,
			null
		};
		CSteamID steamIDFriend = pCallback.m_steamIDFriend;
		obj[3] = ((object)(*(CSteamID*)(&steamIDFriend))/*cast due to constrained. prefix*/).ToString();
		obj[4] = " -- ";
		obj[5] = ((GameRichPresenceJoinRequested_t)(ref pCallback)).m_rgchConnect;
		steamtestlog(string.Concat(obj));
	}

	private unsafe static void OnGameConnectedClanChatMsg(GameConnectedClanChatMsg_t pCallback)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		string[] obj = new string[8]
		{
			"[",
			338.ToString(),
			" - GameConnectedClanChatMsg] - ",
			null,
			null,
			null,
			null,
			null
		};
		CSteamID steamIDClanChat = pCallback.m_steamIDClanChat;
		obj[3] = ((object)(*(CSteamID*)(&steamIDClanChat))/*cast due to constrained. prefix*/).ToString();
		obj[4] = " -- ";
		steamIDClanChat = pCallback.m_steamIDUser;
		obj[5] = ((object)(*(CSteamID*)(&steamIDClanChat))/*cast due to constrained. prefix*/).ToString();
		obj[6] = " -- ";
		obj[7] = pCallback.m_iMessageID.ToString();
		steamtestlog(string.Concat(obj));
		string text = default(string);
		EChatEntryType val = default(EChatEntryType);
		CSteamID val2 = default(CSteamID);
		int clanChatMessage = SteamFriends.GetClanChatMessage(pCallback.m_steamIDClanChat, pCallback.m_iMessageID, ref text, 2048, ref val, ref val2);
		string[] obj2 = new string[5]
		{
			clanChatMessage.ToString(),
			" ",
			null,
			null,
			null
		};
		steamIDClanChat = val2;
		obj2[2] = ((object)(*(CSteamID*)(&steamIDClanChat))/*cast due to constrained. prefix*/).ToString();
		obj2[3] = ": ";
		obj2[4] = text;
		steamtestlog(string.Concat(obj2));
	}

	private unsafe static void OnGameConnectedChatJoin(GameConnectedChatJoin_t pCallback)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		string[] obj = new string[6]
		{
			"[",
			339.ToString(),
			" - GameConnectedChatJoin] - ",
			null,
			null,
			null
		};
		CSteamID steamIDClanChat = pCallback.m_steamIDClanChat;
		obj[3] = ((object)(*(CSteamID*)(&steamIDClanChat))/*cast due to constrained. prefix*/).ToString();
		obj[4] = " -- ";
		steamIDClanChat = pCallback.m_steamIDUser;
		obj[5] = ((object)(*(CSteamID*)(&steamIDClanChat))/*cast due to constrained. prefix*/).ToString();
		steamtestlog(string.Concat(obj));
	}

	private unsafe static void OnGameConnectedChatLeave(GameConnectedChatLeave_t pCallback)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		string[] obj = new string[10]
		{
			"[",
			340.ToString(),
			" - GameConnectedChatLeave] - ",
			null,
			null,
			null,
			null,
			null,
			null,
			null
		};
		CSteamID steamIDClanChat = pCallback.m_steamIDClanChat;
		obj[3] = ((object)(*(CSteamID*)(&steamIDClanChat))/*cast due to constrained. prefix*/).ToString();
		obj[4] = " -- ";
		steamIDClanChat = pCallback.m_steamIDUser;
		obj[5] = ((object)(*(CSteamID*)(&steamIDClanChat))/*cast due to constrained. prefix*/).ToString();
		obj[6] = " -- ";
		obj[7] = pCallback.m_bKicked.ToString();
		obj[8] = " -- ";
		obj[9] = pCallback.m_bDropped.ToString();
		steamtestlog(string.Concat(obj));
	}

	private static void OnDownloadClanActivityCountsResult(DownloadClanActivityCountsResult_t pCallback, bool bIOFailure)
	{
		steamtestlog("[" + 341 + " - DownloadClanActivityCountsResult] - " + pCallback.m_bSuccess);
	}

	private unsafe static void OnJoinClanChatRoomCompletionResult(JoinClanChatRoomCompletionResult_t pCallback, bool bIOFailure)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		string[] obj = new string[6]
		{
			"[",
			342.ToString(),
			" - JoinClanChatRoomCompletionResult] - ",
			null,
			null,
			null
		};
		CSteamID steamIDClanChat = pCallback.m_steamIDClanChat;
		obj[3] = ((object)(*(CSteamID*)(&steamIDClanChat))/*cast due to constrained. prefix*/).ToString();
		obj[4] = " -- ";
		obj[5] = ((object)(*(EChatRoomEnterResponse*)(&pCallback.m_eChatRoomEnterResponse))/*cast due to constrained. prefix*/).ToString();
		steamtestlog(string.Concat(obj));
	}

	private unsafe static void OnGameConnectedFriendChatMsg(GameConnectedFriendChatMsg_t pCallback)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		string[] obj = new string[6]
		{
			"[",
			343.ToString(),
			" - GameConnectedFriendChatMsg] - ",
			null,
			null,
			null
		};
		CSteamID steamIDUser = pCallback.m_steamIDUser;
		obj[3] = ((object)(*(CSteamID*)(&steamIDUser))/*cast due to constrained. prefix*/).ToString();
		obj[4] = " -- ";
		obj[5] = pCallback.m_iMessageID.ToString();
		steamtestlog(string.Concat(obj));
		string text = default(string);
		EChatEntryType val = default(EChatEntryType);
		int friendMessage = SteamFriends.GetFriendMessage(pCallback.m_steamIDUser, pCallback.m_iMessageID, ref text, 2048, ref val);
		string[] obj2 = new string[5]
		{
			friendMessage.ToString(),
			" ",
			null,
			null,
			null
		};
		steamIDUser = pCallback.m_steamIDUser;
		obj2[2] = ((object)(*(CSteamID*)(&steamIDUser))/*cast due to constrained. prefix*/).ToString();
		obj2[3] = ": ";
		obj2[4] = text;
		steamtestlog(string.Concat(obj2));
	}

	private unsafe static void OnFriendsGetFollowerCount(FriendsGetFollowerCount_t pCallback, bool bIOFailure)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		string[] obj = new string[8]
		{
			"[",
			344.ToString(),
			" - FriendsGetFollowerCount] - ",
			((object)(*(EResult*)(&pCallback.m_eResult))/*cast due to constrained. prefix*/).ToString(),
			" -- ",
			null,
			null,
			null
		};
		CSteamID steamID = pCallback.m_steamID;
		obj[5] = ((object)(*(CSteamID*)(&steamID))/*cast due to constrained. prefix*/).ToString();
		obj[6] = " -- ";
		obj[7] = pCallback.m_nCount.ToString();
		steamtestlog(string.Concat(obj));
	}

	private unsafe static void OnFriendsIsFollowing(FriendsIsFollowing_t pCallback, bool bIOFailure)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		string[] obj = new string[8]
		{
			"[",
			345.ToString(),
			" - FriendsIsFollowing] - ",
			((object)(*(EResult*)(&pCallback.m_eResult))/*cast due to constrained. prefix*/).ToString(),
			" -- ",
			null,
			null,
			null
		};
		CSteamID steamID = pCallback.m_steamID;
		obj[5] = ((object)(*(CSteamID*)(&steamID))/*cast due to constrained. prefix*/).ToString();
		obj[6] = " -- ";
		obj[7] = pCallback.m_bIsFollowing.ToString();
		steamtestlog(string.Concat(obj));
		if (pCallback.m_bIsFollowing)
		{
			Following.Add(pCallback.m_steamID.m_SteamID);
		}
	}

	private unsafe static void OnFriendsEnumerateFollowingList(FriendsEnumerateFollowingList_t pCallback, bool bIOFailure)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		steamtestlog("[" + 346 + " - FriendsEnumerateFollowingList] - " + ((object)(*(EResult*)(&pCallback.m_eResult))/*cast due to constrained. prefix*/).ToString() + " -- " + pCallback.m_rgSteamID?.ToString() + " -- " + pCallback.m_nResultsReturned + " -- " + pCallback.m_nTotalResultCount);
	}

	private static void OnUnreadChatMessagesChanged(UnreadChatMessagesChanged_t pCallback)
	{
		steamtestlog("[" + 348 + " - UnreadChatMessagesChanged]");
	}

	private static void OnOverlayBrowserProtocolNavigation(OverlayBrowserProtocolNavigation_t pCallback)
	{
		steamtestlog("[" + 349 + " - OverlayBrowserProtocolNavigation] - " + ((OverlayBrowserProtocolNavigation_t)(ref pCallback)).rgchURI);
	}

	private unsafe static void OnEquippedProfileItemsChanged(EquippedProfileItemsChanged_t pCallback)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		string text = 350.ToString();
		CSteamID steamID = pCallback.m_steamID;
		steamtestlog("[" + text + " - EquippedProfileItemsChanged] - " + ((object)(*(CSteamID*)(&steamID))/*cast due to constrained. prefix*/).ToString());
	}

	private unsafe static void OnEquippedProfileItems(EquippedProfileItems_t pCallback, bool bIOFailure)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		string[] obj = new string[16]
		{
			"[",
			351.ToString(),
			" - EquippedProfileItems] - ",
			((object)(*(EResult*)(&pCallback.m_eResult))/*cast due to constrained. prefix*/).ToString(),
			" -- ",
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null
		};
		CSteamID steamID = pCallback.m_steamID;
		obj[5] = ((object)(*(CSteamID*)(&steamID))/*cast due to constrained. prefix*/).ToString();
		obj[6] = " -- ";
		obj[7] = pCallback.m_bHasAnimatedAvatar.ToString();
		obj[8] = " -- ";
		obj[9] = pCallback.m_bHasAvatarFrame.ToString();
		obj[10] = " -- ";
		obj[11] = pCallback.m_bHasProfileModifier.ToString();
		obj[12] = " -- ";
		obj[13] = pCallback.m_bHasProfileBackground.ToString();
		obj[14] = " -- ";
		obj[15] = pCallback.m_bHasMiniProfileBackground.ToString();
		steamtestlog(string.Concat(obj));
	}

	public static void Init()
	{
		if (Loaded)
		{
			FavoritesListChanged = Callback<FavoritesListChanged_t>.Create((DispatchDelegate<FavoritesListChanged_t>)OnFavoritesListChanged);
			LobbyInvite = Callback<LobbyInvite_t>.Create((DispatchDelegate<LobbyInvite_t>)OnLobbyInvite);
			LobbyEnter = Callback<LobbyEnter_t>.Create((DispatchDelegate<LobbyEnter_t>)OnLobbyEnter);
			LobbyDataUpdate = Callback<LobbyDataUpdate_t>.Create((DispatchDelegate<LobbyDataUpdate_t>)OnLobbyDataUpdate);
			LobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create((DispatchDelegate<LobbyChatUpdate_t>)OnLobbyChatUpdate);
			LobbyChatMsg = Callback<LobbyChatMsg_t>.Create((DispatchDelegate<LobbyChatMsg_t>)OnLobbyChatMsg);
			LobbyGameCreated = Callback<LobbyGameCreated_t>.Create((DispatchDelegate<LobbyGameCreated_t>)OnLobbyGameCreated);
			LobbyKicked = Callback<LobbyKicked_t>.Create((DispatchDelegate<LobbyKicked_t>)OnLobbyKicked);
			FavoritesListAccountsUpdated = Callback<FavoritesListAccountsUpdated_t>.Create((DispatchDelegate<FavoritesListAccountsUpdated_t>)OnFavoritesListAccountsUpdated);
			JoinPartyCallback = Callback<JoinPartyCallback_t>.Create((DispatchDelegate<JoinPartyCallback_t>)OnJoinPartyCallback);
			CreateBeaconCallback = Callback<CreateBeaconCallback_t>.Create((DispatchDelegate<CreateBeaconCallback_t>)OnCreateBeaconCallback);
			ReservationNotificationCallback = Callback<ReservationNotificationCallback_t>.Create((DispatchDelegate<ReservationNotificationCallback_t>)OnReservationNotificationCallback);
			ChangeNumOpenSlotsCallback = Callback<ChangeNumOpenSlotsCallback_t>.Create((DispatchDelegate<ChangeNumOpenSlotsCallback_t>)OnChangeNumOpenSlotsCallback);
			AvailableBeaconLocationsUpdated = Callback<AvailableBeaconLocationsUpdated_t>.Create((DispatchDelegate<AvailableBeaconLocationsUpdated_t>)OnAvailableBeaconLocationsUpdated);
			ActiveBeaconsUpdated = Callback<ActiveBeaconsUpdated_t>.Create((DispatchDelegate<ActiveBeaconsUpdated_t>)OnActiveBeaconsUpdated);
			OnLobbyEnterCallResult = CallResult<LobbyEnter_t>.Create((APIDispatchDelegate<LobbyEnter_t>)OnLobbyEnter_APIDispatch);
			OnLobbyMatchListCallResult = CallResult<LobbyMatchList_t>.Create((APIDispatchDelegate<LobbyMatchList_t>)OnLobbyMatchList);
			OnLobbyCreatedCallResult = CallResult<LobbyCreated_t>.Create((APIDispatchDelegate<LobbyCreated_t>)OnLobbyCreated);
			PersonaStateChange = Callback<PersonaStateChange_t>.Create((DispatchDelegate<PersonaStateChange_t>)OnPersonaStateChange);
			GameOverlayActivated = Callback<GameOverlayActivated_t>.Create((DispatchDelegate<GameOverlayActivated_t>)OnGameOverlayActivated);
			GameServerChangeRequested = Callback<GameServerChangeRequested_t>.Create((DispatchDelegate<GameServerChangeRequested_t>)OnGameServerChangeRequested);
			GameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create((DispatchDelegate<GameLobbyJoinRequested_t>)OnGameLobbyJoinRequested);
			AvatarImageLoaded = Callback<AvatarImageLoaded_t>.Create((DispatchDelegate<AvatarImageLoaded_t>)OnAvatarImageLoaded);
			FriendRichPresenceUpdate = Callback<FriendRichPresenceUpdate_t>.Create((DispatchDelegate<FriendRichPresenceUpdate_t>)OnFriendRichPresenceUpdate);
			GameRichPresenceJoinRequested = Callback<GameRichPresenceJoinRequested_t>.Create((DispatchDelegate<GameRichPresenceJoinRequested_t>)OnGameRichPresenceJoinRequested);
			GameConnectedClanChatMsg = Callback<GameConnectedClanChatMsg_t>.Create((DispatchDelegate<GameConnectedClanChatMsg_t>)OnGameConnectedClanChatMsg);
			GameConnectedChatJoin = Callback<GameConnectedChatJoin_t>.Create((DispatchDelegate<GameConnectedChatJoin_t>)OnGameConnectedChatJoin);
			GameConnectedChatLeave = Callback<GameConnectedChatLeave_t>.Create((DispatchDelegate<GameConnectedChatLeave_t>)OnGameConnectedChatLeave);
			GameConnectedFriendChatMsg = Callback<GameConnectedFriendChatMsg_t>.Create((DispatchDelegate<GameConnectedFriendChatMsg_t>)OnGameConnectedFriendChatMsg);
			UnreadChatMessagesChanged = Callback<UnreadChatMessagesChanged_t>.Create((DispatchDelegate<UnreadChatMessagesChanged_t>)OnUnreadChatMessagesChanged);
			OverlayBrowserProtocolNavigation = Callback<OverlayBrowserProtocolNavigation_t>.Create((DispatchDelegate<OverlayBrowserProtocolNavigation_t>)OnOverlayBrowserProtocolNavigation);
			EquippedProfileItemsChanged = Callback<EquippedProfileItemsChanged_t>.Create((DispatchDelegate<EquippedProfileItemsChanged_t>)OnEquippedProfileItemsChanged);
			OnClanOfficerListResponseCallResult = CallResult<ClanOfficerListResponse_t>.Create((APIDispatchDelegate<ClanOfficerListResponse_t>)OnClanOfficerListResponse);
			OnDownloadClanActivityCountsResultCallResult = CallResult<DownloadClanActivityCountsResult_t>.Create((APIDispatchDelegate<DownloadClanActivityCountsResult_t>)OnDownloadClanActivityCountsResult);
			OnJoinClanChatRoomCompletionResultCallResult = CallResult<JoinClanChatRoomCompletionResult_t>.Create((APIDispatchDelegate<JoinClanChatRoomCompletionResult_t>)OnJoinClanChatRoomCompletionResult);
			OnFriendsGetFollowerCountCallResult = CallResult<FriendsGetFollowerCount_t>.Create((APIDispatchDelegate<FriendsGetFollowerCount_t>)OnFriendsGetFollowerCount);
			OnFriendsIsFollowingCallResult = CallResult<FriendsIsFollowing_t>.Create((APIDispatchDelegate<FriendsIsFollowing_t>)OnFriendsIsFollowing);
			OnFriendsEnumerateFollowingListCallResult = CallResult<FriendsEnumerateFollowingList_t>.Create((APIDispatchDelegate<FriendsEnumerateFollowingList_t>)OnFriendsEnumerateFollowingList);
			OnEquippedProfileItemsCallResult = CallResult<EquippedProfileItems_t>.Create((APIDispatchDelegate<EquippedProfileItems_t>)OnEquippedProfileItems);
			SteamApps.BIsLowViolence();
			string text = default(string);
			SteamApps.GetLaunchCommandLine(ref text, 1024);
			if (text == null)
			{
				text = "";
			}
			steamtestlog("[SteamApps.GetLaunchCommandLine(out var pszCommandLine, 1024)] - " + text);
			LoadFriendList();
			UpdateRichPresence();
		}
	}

	internal static void steamtestlog(string what, bool forcelog = false)
	{
		if (!forcelog && !_DEV_ENABLE_STEAM_LOG)
		{
			return;
		}
		try
		{
			what = "STEAM: " + what;
			Plugin.Logger.LogInfo((object)KrokoshaCasualtiesMP.log.do_timestamp(what));
			if ((Object)(object)Con.con != (Object)null)
			{
				Con.con.LogToConsole("<color=#00ffffff>KrokMP:  " + what + "</color>");
			}
		}
		catch (Exception ex)
		{
			Plugin.Logger.LogError((object)("steamtestlog: " + ex.ToString()));
		}
	}
}
