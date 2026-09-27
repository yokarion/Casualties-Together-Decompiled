using System;
using System.Collections.Generic;
using System.Linq;
using KrokoshaCasualtiesUtils;
using Steamworks;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public static class UIServerBrowser
{
	public static UIMainMenu.MenuTab menutab_serverbrowser = new UIMainMenu.MenuTab
	{
		name = "mmtb_serverbrowser",
		on_open = delegate
		{
			if (CurServerList.Count == 0)
			{
				ReloadList();
			}
		},
		gui_func = _GUI_RenderServerBrowser
	};

	public static string _LOBBY_SEARCH_FILTER = "";

	private static Vector2 _SERVERBROWSERMENUSCROLL = Vector2.zero;

	public static bool SortHideIncompatible = false;

	public static bool SortHidePasswordProtected = false;

	private static bool SORT_FLIPBOOL_alpha = false;

	private static bool SORT_FLIPBOOL_count = false;

	private static bool SORT_FLIPBOOL_layer = false;

	public static int _LOBBY_ENTRY_HEIGHT = 90;

	public static int _LOBBY_ERROR_HEIGHT = 30;

	private static readonly Dictionary<string, Sprite> ResourceSprites = new Dictionary<string, Sprite>();

	internal static readonly Dictionary<ulong, NetPublicServerInfo> CurServerList_ButOnlySteamLobbies = new Dictionary<ulong, NetPublicServerInfo>();

	internal static List<NetPublicServerInfo> CurServerList = new List<NetPublicServerInfo>();

	internal static List<NetPublicServerInfo> CurFilteredServerList = new List<NetPublicServerInfo>();

	private static int TotalPlayerCount = 0;

	private static bool SearchPatternIsLobbyId;

	private static ulong SearchPatternLobbyId;

	public static int CurServerCount => CurServerList.Count;

	private static void _GUI_RenderServerBrowser(Rect r)
	{
		//IL_0d6e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Expected I4, but got Unknown
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_070b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0712: Expected O, but got Unknown
		//IL_07ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f1: Expected O, but got Unknown
		//IL_07fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0802: Unknown result type (might be due to invalid IL or missing references)
		//IL_084b: Unknown result type (might be due to invalid IL or missing references)
		//IL_085a: Unknown result type (might be due to invalid IL or missing references)
		//IL_086c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0871: Unknown result type (might be due to invalid IL or missing references)
		//IL_0876: Unknown result type (might be due to invalid IL or missing references)
		//IL_073d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0744: Expected O, but got Unknown
		//IL_043d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0447: Unknown result type (might be due to invalid IL or missing references)
		//IL_044c: Unknown result type (might be due to invalid IL or missing references)
		//IL_047b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0480: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0772: Unknown result type (might be due to invalid IL or missing references)
		//IL_0779: Expected O, but got Unknown
		//IL_07a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a8: Expected O, but got Unknown
		//IL_07cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d6: Expected O, but got Unknown
		//IL_07bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c6: Expected O, but got Unknown
		//IL_0ac8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b8e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b93: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bbf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bb1: Unknown result type (might be due to invalid IL or missing references)
		GUI.skin.label.alignment = (TextAnchor)1;
		UIBullshit._GUI_BiggerLabel(Lang.Get("mmtb_serverbrowser", false), 1.4f);
		GUI.skin.label.alignment = (TextAnchor)0;
		if (!KSteam.Loaded)
		{
			UIBullshit._GUI_BiggerLabel("Steam not loaded", 1.4f);
			return;
		}
		float fieldnameswidth = ((Rect)(ref r)).width * 0.12f;
		float num = ((Rect)(ref r)).width * 0.26f;
		UIMainMenu.GetMenuUIScale();
		float fieldheight = 30f * UIMainMenu.GetMenuUIScale();
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		if (GUILayout.Button(Lang.Get("mmsb_refresh", false), Array.Empty<GUILayoutOption>()))
		{
			Util.PlayUISound((UISoundType)0);
			ReloadList();
		}
		if (KSteam.Loaded)
		{
			GUILayout.FlexibleSpace();
			GUILayout.Label(Lang.Get("mmsb_steam_distfilter", false), Array.Empty<GUILayoutOption>());
			KSteam.userselected_lobbyDistanceFilter = (ELobbyDistanceFilter)GUILayout_DropdownMenu.Dropdown((int)KSteam.userselected_lobbyDistanceFilter, new string[4]
			{
				Lang.Get("mmsb_steam_dist0", false),
				Lang.Get("mmsb_steam_dist1", false),
				Lang.Get("mmsb_steam_dist2", false),
				Lang.Get("mmsb_steam_dist3", false)
			});
		}
		GUILayout.FlexibleSpace();
		UIMainMenu._GUI___DirectConnect_DoPasswordField(r, fieldheight, fieldnameswidth, num);
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		GUI.skin.label.alignment = (TextAnchor)5;
		GUILayout.Label(Lang.Get("mmct_search", false), Array.Empty<GUILayoutOption>());
		string text = GUILayout.TextField(_LOBBY_SEARCH_FILTER, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(num) });
		if (text != _LOBBY_SEARCH_FILTER)
		{
			_LOBBY_SEARCH_FILTER = text;
			RefilterList();
		}
		GUILayout.Space(5f * UIMainMenu.GetMenuUIScale());
		bool sortHidePasswordProtected = SortHidePasswordProtected;
		SortHidePasswordProtected = GUILayout.Toggle(SortHidePasswordProtected, Lang.Get("mmsb_lobbyhidepass", false), Array.Empty<GUILayoutOption>());
		if (sortHidePasswordProtected != SortHidePasswordProtected)
		{
			RefilterList();
		}
		bool sortHideIncompatible = SortHideIncompatible;
		SortHideIncompatible = GUILayout.Toggle(SortHideIncompatible, Lang.Get("mmsb_lobbyhidever", false), Array.Empty<GUILayoutOption>());
		if (sortHideIncompatible != SortHideIncompatible)
		{
			RefilterList();
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		UIBullshit._GUI_SetButtonSkinTexture(GUI.skin.button, small: false);
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		GUI.skin.label.alignment = (TextAnchor)5;
		GUILayout.Label(Lang.Get("mmsb_sort", false), Array.Empty<GUILayoutOption>());
		GUI.skin.label.alignment = (TextAnchor)3;
		if (GUILayout.Button(Lang.Get("mmsb_sort_plrcount", false), Array.Empty<GUILayoutOption>()))
		{
			int mul = (SORT_FLIPBOOL_count ? 1 : (-1));
			CurFilteredServerList.Sort((NetPublicServerInfo a, NetPublicServerInfo b) => a.plr_count.CompareTo(b.plr_count) * mul);
			SortServerListByJoinability();
			SORT_FLIPBOOL_count = !SORT_FLIPBOOL_count;
		}
		if (GUILayout.Button(Lang.Get("mmsb_sort_layer", false), Array.Empty<GUILayoutOption>()))
		{
			int mul2 = ((!SORT_FLIPBOOL_layer) ? 1 : (-1));
			CurFilteredServerList.Sort((NetPublicServerInfo a, NetPublicServerInfo b) => a.cur_layer.CompareTo(b.cur_layer) * mul2);
			SortServerListByJoinability();
			SORT_FLIPBOOL_layer = !SORT_FLIPBOOL_layer;
		}
		if (GUILayout.Button(Lang.Get("mmsb_sort_name", false), Array.Empty<GUILayoutOption>()))
		{
			int mul3 = ((!SORT_FLIPBOOL_alpha) ? 1 : (-1));
			CurFilteredServerList.Sort((NetPublicServerInfo a, NetPublicServerInfo b) => a.name.CompareTo(b.name) * mul3);
			SortServerListByJoinability();
			SORT_FLIPBOOL_alpha = !SORT_FLIPBOOL_alpha;
		}
		GUILayout.FlexibleSpace();
		GUILayout.Label(string.Format(Lang.Get("mmsb_lobbyfounds", false), CurServerList.Count, CurFilteredServerList.Count, TotalPlayerCount), Array.Empty<GUILayoutOption>());
		GUILayout.EndHorizontal();
		_SERVERBROWSERMENUSCROLL = GUILayout.BeginScrollView(_SERVERBROWSERMENUSCROLL, Array.Empty<GUILayoutOption>());
		int fontSize = GUI.skin.label.fontSize;
		GUI.skin.label.alignment = (TextAnchor)0;
		TextAnchor alignment = GUI.skin.label.alignment;
		GUI.skin.label.wordWrap = false;
		GUILayout.Space(10f * UIMainMenu.GetMenuUIScale());
		bool flag = KnownPersons.IsSteamUserPrivileged(KSteam.GetLocalUserSteamID().m_SteamID);
		GUI.skin.label.alignment = (TextAnchor)4;
		if (CurServerList.Count == 0)
		{
			GUILayout.Label(Lang.Get("mmsb_empty", false), Array.Empty<GUILayoutOption>());
		}
		else if (CurFilteredServerList.Count == 0)
		{
			GUILayout.Label(Lang.Get("filter_found_nothing", false), Array.Empty<GUILayoutOption>());
		}
		else
		{
			try
			{
				string fULL_VERSION_TAG = KrokoshaScavMultiplayer.FULL_VERSION_TAG;
				GUI.skin.label.fontSize = fontSize;
				int fontSize2 = fontSize;
				_ = _LOBBY_ENTRY_HEIGHT;
				UIMainMenu.GetMenuUIScale();
				bool enabled = GUI.enabled;
				foreach (NetPublicServerInfo curFilteredServer in CurFilteredServerList)
				{
					GUI.skin.label.alignment = (TextAnchor)0;
					GUILayout.BeginVertical(GUI.skin.box, Array.Empty<GUILayoutOption>());
					try
					{
						bool flag2 = curFilteredServer.steam_lobby_info != null;
						GUI.skin.label.fontSize = fontSize2;
						if (curFilteredServer.version != fULL_VERSION_TAG)
						{
							GUI.enabled = false;
							GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
							GUILayout.Space(10f * UIMainMenu.GetMenuUIScale());
							GUILayout.Label(curFilteredServer.name, Array.Empty<GUILayoutOption>());
							GUI.skin.label.alignment = (TextAnchor)1;
							GUILayout.Label(curFilteredServer.plr_count + "/" + curFilteredServer.plr_max, Array.Empty<GUILayoutOption>());
							GUILayout.EndHorizontal();
							GUI.skin.label.alignment = (TextAnchor)0;
							GUI.skin.label.fontSize = (int)((float)GUI.skin.label.fontSize * 0.9f);
							GUILayout.Label(Lang.Get("mmsb_lobbybadver", false) + " - " + curFilteredServer.version, Array.Empty<GUILayoutOption>());
							GUI.enabled = enabled;
						}
						else
						{
							int num2 = curFilteredServer.cur_layer + 1;
							Sprite resourceSprite = GetResourceSprite("BiomeIcon/" + num2);
							bool flag3 = false;
							bool flag4 = false;
							GUIContent val;
							if (!Locale.currentLang.other.TryGetValue("layertitle" + num2, out var value) || curFilteredServer.cur_layer == -1)
							{
								val = new GUIContent(Lang.Get("mmsb_lobby_inmenu", false));
							}
							else if (curFilteredServer.gamemode == "tutorial")
							{
								val = new GUIContent(Lang.Get("mmsb_lobby_loading", false));
							}
							else
							{
								flag3 = true;
								if (curFilteredServer.gamemode == "debug_world")
								{
									val = new GUIContent(Lang.Get("mmsb_lobby_debug_world", false));
								}
								else if (curFilteredServer.gamemode == "loading")
								{
									val = new GUIContent(Lang.Get("mmsb_lobby_loading", false));
								}
								else if (curFilteredServer.gamemode == "vanilla")
								{
									val = new GUIContent(value);
								}
								else
								{
									val = new GUIContent(curFilteredServer.gamemode);
									flag4 = true;
								}
							}
							GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
							GUIContent val2 = new GUIContent(curFilteredServer.name);
							Vector2 val3 = GUI.skin.label.CalcSize(val2);
							GUILayout.Space(10f * UIMainMenu.GetMenuUIScale());
							GUILayout.Label(val2, Array.Empty<GUILayoutOption>());
							GUILayout.Space(10f * UIMainMenu.GetMenuUIScale());
							if (curFilteredServer.friends_tooltip != null)
							{
								GUILayout.Label((Texture)(object)CoopModAssets.happy.texture, (GUILayoutOption[])(object)new GUILayoutOption[2]
								{
									GUILayout.Width(val3.y),
									GUILayout.Height(val3.y)
								});
								if (UIBullshit.CheckCursorOverlap(GUIUtility.GUIToScreenRect(GUILayoutUtility.GetLastRect())))
								{
									UIBullshit._GUI_SetTooltip(in curFilteredServer.friends_tooltip, "");
								}
							}
							if (curFilteredServer.haspassword)
							{
								GUILayout.Label((Texture)(object)CoopModAssets.@lock.texture, (GUILayoutOption[])(object)new GUILayoutOption[2]
								{
									GUILayout.Width(val3.y),
									GUILayout.Height(val3.y)
								});
								if (UIBullshit.CheckCursorOverlap(GUIUtility.GUIToScreenRect(GUILayoutUtility.GetLastRect())))
								{
									UIBullshit._GUI_SetTooltip(Lang.Get("mmsb_lobby_islocked", false), "");
								}
							}
							GUI.skin.label.alignment = (TextAnchor)1;
							GUILayout.Label(curFilteredServer.plr_count + "/" + curFilteredServer.plr_max, Array.Empty<GUILayoutOption>());
							GUILayout.FlexibleSpace();
							if (flag4)
							{
								GUILayout.Label(curFilteredServer.gamemode, Array.Empty<GUILayoutOption>());
							}
							GUILayout.FlexibleSpace();
							GUILayout.Button(Lang.Get("mmsb_lobby_listrules", false), Array.Empty<GUILayoutOption>());
							UIMainMenu.DoTooltipOnLastGUIRectNoLang(in curFilteredServer.rules_as_string);
							if (curFilteredServer.plr_count > 0 && curFilteredServer.steam_lobby_info != null)
							{
								GUILayout.Button(Lang.Get("mmsb_lobby_listplrs", false), Array.Empty<GUILayoutOption>());
								UIMainMenu.DoTooltipOnLastGUIRectNoLang(string.Join("\n", curFilteredServer.steam_lobby_info.members.Select((KeyValuePair<CSteamID, KSteam.LobbyMember> x) => KSteam.GetSteamUsername(x.Key.m_SteamID))));
							}
							if (curFilteredServer.modlist.Length != 0)
							{
								GUILayout.Button(Lang.Get("mmsb_lobby_listmod", false), Array.Empty<GUILayoutOption>());
								UIMainMenu.DoTooltipOnLastGUIRectNoLang(string.Join("\n", curFilteredServer.modlist));
							}
							GUILayout.EndHorizontal();
							GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
							GUI.enabled = enabled && !KrokoshaScavMultiplayer.IsNetworkActiveOrIsInGame() && (curFilteredServer.IsJoinable() || flag);
							if (GUILayout.Button(Lang.Get(curFilteredServer.latejoinspectate ? "mmsb_lobbyjoinspectate" : "mmsb_lobbyjoin", false), (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(((Rect)(ref r)).width * 0.2f) }))
							{
								if (flag2)
								{
									KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog($"STEAM: Join Lobby {curFilteredServer.steam_lobby_info.lobby_steamID}");
									TransportSteamworks.OnWantToJoinLobby(curFilteredServer.steam_lobby_info.lobby_steamID.m_SteamID);
									UIMainMenu.current_tab = UIMainMenu.menutab_directconnect;
									UIMainMenu._DO_SERVER_CONFIG = false;
								}
								else
								{
									log.error("TODO: JOIN TO IP");
								}
							}
							if (Net.running)
							{
								UIMainMenu.DoTooltipOnLastGUIRect("mmsb_lobbyjoin_alreadyjoined");
							}
							else if (curFilteredServer.midjoin_lock_active)
							{
								UIMainMenu.DoTooltipOnLastGUIRect("mmsb_lobbyjoin_nomidjoin");
							}
							else if (curFilteredServer.enforcemodlist_locked)
							{
								UIMainMenu.DoTooltipOnLastGUIRect("mmsb_lobbyjoin_wrongmods");
							}
							GUI.enabled = enabled;
							float num3 = 10f * UIMainMenu.GetMenuUIScale();
							GUI.skin.label.alignment = (TextAnchor)0;
							GUILayout.Space(num3);
							GUILayout.Label(val, Array.Empty<GUILayoutOption>());
							if (flag3)
							{
								Rect lastRect = GUILayoutUtility.GetLastRect();
								if ((Object)(object)resourceSprite != (Object)null)
								{
									GUI.skin.label.CalcSize(val);
									GUI.DrawTexture(lastRect, (Texture)(object)resourceSprite.texture);
								}
								GUI.Label(lastRect, val);
							}
							GUI.skin.label.fontSize = (int)((float)GUI.skin.label.fontSize * 0.9f);
							if (curFilteredServer.cur_layer != -1)
							{
								GUI.skin.label.alignment = (TextAnchor)4;
								int num4 = Mathf.RoundToInt((float)curFilteredServer.avg_happiness * 0.1f);
								if (Locale.currentLang.other.TryGetValue("moodrange" + num4, out var value2))
								{
									GUILayout.Label(Lang.Get("mmsb_lobby_avgmood", false) + value2, Array.Empty<GUILayoutOption>());
								}
								GUILayout.Label(Lang.Get("mmsb_lobby_alivecount", false) + curFilteredServer.plr_living_count + "/" + curFilteredServer.plr_count, Array.Empty<GUILayoutOption>());
							}
							GUILayout.Space(num3);
							GUI.skin.label.alignment = (TextAnchor)8;
							GUILayout.Label("V" + curFilteredServer.version, Array.Empty<GUILayoutOption>());
							GUILayout.Space(num3);
							GUILayout.EndHorizontal();
						}
					}
					catch (Exception ex)
					{
						log.error($"_GUI_RenderServerBrowser: {curFilteredServer}\n{ex.ToString()}");
					}
					GUI.enabled = enabled;
					GUILayout.EndVertical();
				}
			}
			catch (Exception ex2)
			{
				log.error("_GUI_RenderServerBrowser: RENDER: \n" + ex2.ToString());
			}
		}
		UIBullshit._GUI_SetButtonSkinTexture(GUI.skin.button);
		GUI.skin.label.fontSize = fontSize;
		GUI.skin.label.alignment = alignment;
		GUILayout.Label("", Array.Empty<GUILayoutOption>());
		GUILayout.EndScrollView();
	}

	public static Sprite GetResourceSprite(in string str)
	{
		try
		{
			ResourceSprites[str] = Resources.Load<Sprite>(str);
		}
		catch (Exception)
		{
			ResourceSprites[str] = null;
		}
		return ResourceSprites[str];
	}

	public static void AddServerToTheList(NetPublicServerInfo info)
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		if (info.steam_lobby_info != null)
		{
			if (CurServerList_ButOnlySteamLobbies.TryGetValue(info.steam_lobby_info.lobby_steamID.m_SteamID, out var _))
			{
				return;
			}
			CurServerList_ButOnlySteamLobbies[info.steam_lobby_info.lobby_steamID.m_SteamID] = info;
			foreach (ulong friend in KSteam.Friends)
			{
				if (info.steam_lobby_info.members.ContainsKey((CSteamID)friend) && !info.steamfriends.Contains(friend))
				{
					info.steamfriends.Add(friend);
					if (info.friends_tooltip == null)
					{
						info.friends_tooltip = Lang.Get("mmsb_steam_lobbyfriend", false);
					}
					info.friends_tooltip = info.friends_tooltip + "\n" + SteamFriends.GetFriendPersonaName((CSteamID)friend);
				}
			}
		}
		TotalPlayerCount += info.plr_count;
		CurServerList.Add(info);
		if (TestLobbySearchFilters(info))
		{
			if (info.IsJoinable())
			{
				CurFilteredServerList.Insert(0, info);
			}
			else
			{
				CurFilteredServerList.Add(info);
			}
		}
	}

	public static bool TestLobbySearchFilters(NetPublicServerInfo info)
	{
		if (!string.IsNullOrEmpty(_LOBBY_SEARCH_FILTER) && !StringUtility.ContainsInsensitive(info.name, _LOBBY_SEARCH_FILTER) && (!SearchPatternIsLobbyId || info.steam_lobby_info == null || info.steam_lobby_info.lobby_steamID.m_SteamID != SearchPatternLobbyId))
		{
			return false;
		}
		if (SortHidePasswordProtected && info.haspassword)
		{
			return false;
		}
		if (SortHideIncompatible && info.version != KrokoshaScavMultiplayer.FULL_VERSION_TAG)
		{
			return false;
		}
		return true;
	}

	public static void RefilterList()
	{
		List<NetPublicServerInfo> list = new List<NetPublicServerInfo>(CurServerList);
		ClearLists();
		foreach (NetPublicServerInfo item in list)
		{
			AddServerToTheList(item);
		}
		SortServerListByJoinability();
	}

	public static void SortServerListByJoinability()
	{
		CurFilteredServerList.Sort(delegate(NetPublicServerInfo a, NetPublicServerInfo b)
		{
			bool flag = a.friends_tooltip != null;
			bool value = b.friends_tooltip != null;
			int num = flag.CompareTo(value);
			if (num != 0)
			{
				return num;
			}
			bool flag2 = a.version != "4.1.2";
			bool value2 = b.version != "4.1.2";
			int num2 = flag2.CompareTo(value2);
			if (num2 != 0)
			{
				return num2;
			}
			bool flag3 = !a.IsJoinable();
			bool value3 = !b.IsJoinable();
			int num3 = flag3.CompareTo(value3);
			if (num3 != 0)
			{
				return num3;
			}
			bool flag4 = !a.haspassword;
			bool value4 = !b.haspassword;
			int num4 = flag4.CompareTo(value4);
			return (num4 != 0) ? num4 : 0;
		});
	}

	public static void ClearLists()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		CurServerList_ButOnlySteamLobbies.Clear();
		CurServerList.Clear();
		CurFilteredServerList.Clear();
		TotalPlayerCount = 0;
		SearchPatternIsLobbyId = ulong.TryParse(_LOBBY_SEARCH_FILTER, out SearchPatternLobbyId);
		if (SearchPatternIsLobbyId)
		{
			CSteamID val = (CSteamID)SearchPatternLobbyId;
			SearchPatternIsLobbyId = ((CSteamID)(ref val)).IsLobby();
		}
	}

	public static void ReloadList()
	{
		SORT_FLIPBOOL_alpha = false;
		SORT_FLIPBOOL_count = false;
		ClearLists();
		if (KSteam.Loaded)
		{
			KSteam.SearchLobbies(do_buckets: false);
		}
	}
}
