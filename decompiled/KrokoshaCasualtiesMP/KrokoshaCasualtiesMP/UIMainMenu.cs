using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using KrokoshaCasualtiesUtils;
using Multiupdater;
using Steamworks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace KrokoshaCasualtiesMP;

public static class UIMainMenu
{
	public class MenuTab
	{
		public string name;

		public Action<Rect> gui_func;

		public Action on_open;

		public object data1;

		public object data2;

		public object data3;

		public object data4;

		public object data5;
	}

	public class SettingsTab : MenuTab
	{
		public Type settings_origin;

		public Action on_reset;
	}

	public static bool SINK_KEYBOARD_INPUT = true;

	private static Vector2 scrollbullshit = Vector2.zero;

	public static float PANEL_SIZE_X = 350f;

	public static float PANEL_SIZE_Y = 750f;

	public static float MENU_SIZE_X = 1148.91f;

	public static float MENU_SIZE_Y = 611.41f;

	public static MenuTab menutab_directconnect = new MenuTab
	{
		name = "mmtb_directconnect",
		on_open = delegate
		{
			_COLOR_EDITING = null;
			____GUI___ConnectionSecondPanel_ContentRender_RenderDetailedPlayerList_ban_kick_safety = false;
		},
		gui_func = _GUI_RenderDirectConnectMenu
	};

	public static MenuTab menutab_settings = new MenuTab
	{
		name = "mmtb_settings",
		gui_func = _GUI_RenderSettingsMenu
	};

	public static MenuTab menutab_about = new MenuTab
	{
		name = "mmtb_about",
		gui_func = _GUI_RenderAboutMenu
	};

	public static List<MenuTab> ALL_MENU_TABS = new List<MenuTab>
	{
		UIServerBrowser.menutab_serverbrowser,
		menutab_directconnect,
		menutab_settings
	};

	public static MenuTab current_tab = menutab_directconnect;

	private static bool _UNHIDE_PASSWORD = false;

	private static bool _HIDE_IPPORT = false;

	private static string _COLOR_ERRORS = "";

	private static Rect _COLOR_ERRORS_RECT = default(Rect);

	public static string INPUT_COLORHEX = "#ffffff";

	public static Color24 LAST_VALID_INPUT_COLOR = Color24.black;

	internal static int _STEAM_CHOSEN_LOBBYTYPE = 1;

	internal static bool _DO_SERVER_CONFIG = false;

	internal static bool _USE_STEAM_MENU = true;

	public static string USERINPUT_IPPORT = "127.0.0.1:" + (ushort)7790;

	public static string USERINPUT_NAME = "TEST_NAME";

	private static string[] _CONNECTIONSECONDPANEL_TOOLBAR_STRS = new string[2] { "AAAAAAAAA", "BBBBBBBBB" };

	private static int _CONNECTIONSECONDPANEL_TOOLBAR = 1;

	public static bool ____GUI___ConnectionSecondPanel_ContentRender_RenderDetailedPlayerList_ban_kick_safety = false;

	public static bool ____GUI___ConnectionSecondPanel_ContentRender_RenderDetailedPlayerList_showLitenetLibBanButton = false;

	public static bool ____GUI___ConnectionSecondPanel_ContentRender_RenderDetailedPlayerList_gaben = false;

	private static string _GUI____RenderRuleField_search = "";

	internal static int _GUI____RenderRuleField_PresetChosen = 0;

	internal static string _GUI____RenderRuleField_PresetName = "";

	private static float _GUI____RenderRuleField_last_max_width = 300f;

	private static float _GUI____RenderRuleField_cur_max_width = 0f;

	private static Vector2 _SCROLL_FULLPLRLIST = Vector2.zero;

	private static Vector2 _SCROLL_FULLRULELIST = Vector2.zero;

	private static NetPlayer _COLOR_EDITING = null;

	private static string _COLOR_EDITING_STR = "FFFFFF";

	private static SettingsTab _general_setting_tab = new SettingsTab
	{
		name = "mmst_general",
		gui_func = _GUI___SettingsGeneral,
		on_open = delegate
		{
		},
		on_reset = delegate
		{
		},
		settings_origin = typeof(Amogus)
	};

	private static SettingsTab _voip_setting_tab = new SettingsTab
	{
		name = "mmst_voip",
		gui_func = _GUI___SettingsVOIP,
		on_open = delegate
		{
			_VOIP_SETTINGS_DROPDOWNTEST = false;
		},
		on_reset = delegate
		{
			Voicechat.SetMicrophone("NO_MICROPHONE_SELECTED");
		},
		settings_origin = typeof(Voicechat)
	};

	internal static SettingsTab _chat_setting_tab = new SettingsTab
	{
		name = "mmst_chat",
		gui_func = _GUI___SettingsChat,
		settings_origin = typeof(Chat)
	};

	internal static SettingsTab _debug_setting_tab = new SettingsTab
	{
		name = "mmst_debug",
		gui_func = _GUI___SettingsDebug,
		settings_origin = typeof(DebugMenuSettings)
	};

	internal static SettingsTab _bugreport_setting_tab = new SettingsTab
	{
		name = "mmst_bugreport",
		gui_func = BugReporter._GUI___SettingsBugReport,
		settings_origin = typeof(BugReporter)
	};

	public static SettingsTab cur_settings_tab = _general_setting_tab;

	public static List<SettingsTab> ALL_SETTINGS = new List<SettingsTab>
	{
		_general_setting_tab,
		_chat_setting_tab,
		_voip_setting_tab,
		new SettingsTab
		{
			name = "mmst_gui",
			gui_func = _GUI___SettingsGUI,
			settings_origin = typeof(UIBullshit)
		}
	};

	private static float MicVolumePreviewPixHeight = 20f;

	private static float ASJOUFMIGHDSYDSGFMKFSDKYGJUFSYGJFSDGKVYJSFDKJGYFSDGKVJHBFJKVFDJVMBYFJDYG = 0.38f;

	private static int last_writeX = 0;

	internal static bool writeX_flipflop = false;

	private static bool _VOIP_SETTINGS_DROPDOWNTEST = false;

	private static bool _CHECKFORUPDATEBUTTON_IS_RESTART = false;

	private static bool _CHECKFORUPDATEBUTTON_OnStageSuccess_isBound = false;

	private static Dictionary<Type, List<FieldInfo>> _known_settings = new Dictionary<Type, List<FieldInfo>>();

	private static string AHAHUJHAKFSDFJFJKFKJAFKJAK = "E";

	private static (Texture2D, string)[] gaaaaaaaaaaaaa = new(Texture2D, string)[23]
	{
		(null, "Krokosha666 - OG MP Mod Developer, owner"),
		(null, "Creature - MP Mod Developer"),
		(null, "dannad - Auto-updater module, Installer tool, C:T ds Staff"),
		(null, "Gary the cat - playtester, Bug report module, past servers, C:T ds Staff"),
		(null, "Todd - external mod \"ChangeSkin\", C:T ds Staff"),
		(null, "ItsVoidSK - playtester, some sprites"),
		(null, "dawn - TimeManipulation patch"),
		(null, "Jilu-tin - playtester"),
		(null, "NyaruQwQ - playtester"),
		(null, "X0men0X - playtester"),
		(null, "{Pixel} - playtester"),
		(null, "avr - playtester"),
		(null, "gordocats - self-heal sprite"),
		(null, "notamoron - playtester"),
		(null, "Devactor - playtester"),
		(null, "Dictator - playtester"),
		(null, "Мрак 2.0 - playtester"),
		(null, "Dmonya - playtester"),
		(null, "Shiro - playtester"),
		(null, "Charu - playtester"),
		(null, "kipish - playtester"),
		(null, "Orsoniks - Developer of Casualties: Unknown"),
		(null, "...and the rest of bug reporters, suggestions and regular players")
	};

	private static Vector2 _ABOUTMENUSCROLL = Vector2.zero;

	private static Vector2 _SETTINGSMENUSCROLL = Vector2.zero;

	public static bool is_client => KrokoshaScavMultiplayer.is_client;

	public static bool is_server => KrokoshaScavMultiplayer.is_server;

	public static bool is_dedicated_server => KrokoshaScavMultiplayer.is_dedicated_server;

	public static bool mainmenu_open { get; private set; }

	public static float GetMenuUIScale()
	{
		return UIBullshit.uiScale;
	}

	public static bool IsOpen()
	{
		if ((Object)(object)UIBullshit.main == (Object)null)
		{
			return false;
		}
		return mainmenu_open;
	}

	public static bool IsInSettings()
	{
		if (IsOpen())
		{
			return current_tab == menutab_settings;
		}
		return false;
	}

	public static void SetOpen(bool open_or_nah)
	{
		if (open_or_nah != mainmenu_open)
		{
			if (open_or_nah)
			{
				Application.runInBackground = true;
				if ((Object)(object)KrokoshaMainmenuBackground.runsettingsmenu != (Object)null)
				{
					KrokoshaMainmenuBackground.runsettingsmenu.SetActive(false);
					if (Con.IsConsoleOpen())
					{
						Con.con.ToggleActiveState();
					}
				}
				Util.PlayUISound((UISoundType)0);
			}
			else
			{
				Util.PlayUISound((UISoundType)2);
			}
			if ((Object)(object)KrokoshaMainmenuBackground.mpmenu_button != (Object)null)
			{
				TextMeshProUGUI componentInChildren = KrokoshaMainmenuBackground.mpmenu_button.GetComponentInChildren<TextMeshProUGUI>();
				if ((Object)(object)componentInChildren != (Object)null)
				{
					if (open_or_nah)
					{
						((TMP_Text)componentInChildren).text = Lang.Get("mainmenu_mainbutton_close", false);
					}
					else
					{
						((TMP_Text)componentInChildren).text = Lang.Get("mainmenu_mainbutton", false);
					}
				}
			}
			OnMenuTabChange();
		}
		if (open_or_nah)
		{
			Util.OpenBrightnessPanel(open_or_nah: false);
		}
		mainmenu_open = open_or_nah;
	}

	private static void _GUI__PanelInfo()
	{
		string text = ((!KrokoshaScavMultiplayer.is_server) ? Lang.Get("netmode_client", false) : (KrokoshaScavMultiplayer.is_dedicated_server ? Lang.Get("netmode_server", false) : Lang.Get("netmode_host", false)));
		GUILayout.Label("   " + Lang.Get("netmode", false) + text, Array.Empty<GUILayoutOption>());
		if (!KrokoshaScavMultiplayer.network_system_im_client_and_waiting_connecting)
		{
			if (KrokoshaScavMultiplayer.is_server)
			{
				GUILayout.Label("   " + Lang.Get("netpanel_avgping", false) + Math.Round(ServerMain.AVERAGE_PING), Array.Empty<GUILayoutOption>());
			}
			else if ((Object)(object)NetPlayer.LOCAL_PLAYER != (Object)null)
			{
				GUILayout.Label("   " + Lang.Get("netpanel_ping", false) + ClientMain.LOCAL_PING, Array.Empty<GUILayoutOption>());
			}
			else
			{
				GUILayout.Label(" ", Array.Empty<GUILayoutOption>());
			}
		}
		else
		{
			GUILayout.Label(" ", Array.Empty<GUILayoutOption>());
		}
	}

	private static void _GUI__PlrListDrawPlr(NetPlayer plr)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Expected O, but got Unknown
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Expected O, but got Unknown
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		float num = PANEL_SIZE_X * GetMenuUIScale() * 0.9f;
		float num2 = GetMenuUIScale() * 2f;
		GUI.skin.label.normal.textColor = Color.white;
		GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(num) });
		GUI.skin.label.wordWrap = false;
		GUI.skin.label.alignment = (TextAnchor)0;
		string text = $"{plr.clientId}: {plr.playername}";
		GUIContent val = new GUIContent(text);
		Vector2 val2 = GUI.skin.label.CalcSize(val);
		foreach (Texture2D additional_profile_tag_icon in plr.additional_profile_tag_icons)
		{
			GUILayout.Label((Texture)(object)additional_profile_tag_icon, (GUILayoutOption[])(object)new GUILayoutOption[2]
			{
				GUILayout.Width(val2.y),
				GUILayout.Height(val2.y)
			});
			num2 += val2.y;
		}
		Texture2D profilepic_any_smalltolarge = plr.profilepic_any_smalltolarge;
		if ((Object)(object)profilepic_any_smalltolarge != (Object)null)
		{
			GUILayout.Label((Texture)(object)profilepic_any_smalltolarge, (GUILayoutOption[])(object)new GUILayoutOption[2]
			{
				GUILayout.Width(val2.y),
				GUILayout.Height(val2.y)
			});
			if ((Object)(object)plr.body != (Object)null && !plr.body.alive)
			{
				GUI.DrawTexture(GUILayoutUtility.GetLastRect().ShrinkBorder(-1f * GetMenuUIScale()), (Texture)(object)KrokoshaCoopModAssets.cross.texture, (ScaleMode)0, true, 1f, Color.red, 0f, 0f);
			}
		}
		if (Voicechat.VCRULE_enabled && (Object)(object)plr.vc_output != (Object)null && (Object)(object)KrokoshaCoopModAssets.voicechaticon != (Object)null)
		{
			float realcurvol;
			Color color = plr.vc_output.CalculateVoiceChatIconColor(out realcurvol);
			if (realcurvol > 0f || (plr.is_local && Voicechat.IS_RECORDING))
			{
				GUI.color = color;
				GUILayout.Label((Texture)(object)KrokoshaCoopModAssets.voicechaticon.texture, (GUILayoutOption[])(object)new GUILayoutOption[2]
				{
					GUILayout.Width(val2.y),
					GUILayout.Height(val2.y)
				});
				GUI.color = Color.white;
			}
			else
			{
				GUILayout.Space(val2.y);
			}
			num2 += val2.y;
		}
		GUI.skin.label.normal.textColor = plr.plrcolor;
		string text2 = "ping: " + plr.ping_as_ms;
		Vector2 val3 = GUI.skin.label.CalcSize(new GUIContent(" " + text2));
		GUILayout.Label(text, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.MaxWidth(num * 0.97f - (num2 + val3.x)) });
		GUILayout.FlexibleSpace();
		GUI.skin.label.alignment = (TextAnchor)2;
		GUILayout.Label(text2, Array.Empty<GUILayoutOption>());
		GUILayout.EndHorizontal();
	}

	public static void _GUI_DrawSideMenuWithPlayerList()
	{
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Expected O, but got Unknown
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		if (!Net.running || !UIBullshit.IsAnyMenuOpen())
		{
			return;
		}
		bool flag = Util.IsInWorld();
		TextAnchor alignment = GUI.skin.label.alignment;
		GUI.skin.label.alignment = (TextAnchor)0;
		Rect rect = UIBullshit.ScaleRect(0f, 540f - PANEL_SIZE_Y * 0.5f, PANEL_SIZE_X, PANEL_SIZE_Y, flag);
		UIBullshit._GUI_9SlicePanel(in rect, flag ? 1f : 0.7f, 0.6980392f, check_overlap: true, UIBullshit.unscaled_uiBlockNano);
		Rect val = rect.ShrinkBorder((float)UIBullshit.uiBlockNanoBorderSize);
		GUILayout.BeginArea(val);
		_ = GUI.skin.label.fixedHeight;
		int fontSize = GUI.skin.label.fontSize;
		bool stretchHeight = GUI.skin.label.stretchHeight;
		TextAnchor alignment2 = GUI.skin.label.alignment;
		bool wordWrap = GUI.skin.label.wordWrap;
		RectOffset margin = GUI.skin.label.margin;
		GUI.skin.label.alignment = (TextAnchor)1;
		GUI.skin.label.wordWrap = false;
		GUI.skin.label.margin = new RectOffset(0, 0, 0, 0);
		GUI.skin.label.fontSize = (int)((float)fontSize * 1.3f);
		GUILayout.Label(Lang.Get("netpanel_title", false) + " v4.0.1", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Height((float)GUI.skin.label.fontSize * 1.75f) });
		GUI.skin.label.fontSize = fontSize;
		GUI.skin.label.alignment = alignment2;
		_GUI__PanelInfo();
		bool flag2 = Util.IsInWorld() && !IsOpen();
		if (flag2)
		{
			if (GUILayout.Button(Lang.Get("netpanel_settings", false), Array.Empty<GUILayoutOption>()))
			{
				if (mainmenu_open && Util.IsInWorld())
				{
					Util.OpenBrightnessPanel(open_or_nah: true);
					SetOpen(open_or_nah: false);
				}
				else
				{
					SetOpen(!mainmenu_open);
					if (mainmenu_open)
					{
						current_tab = menutab_settings;
					}
				}
			}
		}
		else
		{
			GUILayout.Label(" ", Array.Empty<GUILayoutOption>());
		}
		if (Net.running && !KrokoshaScavMultiplayer.network_system_im_client_and_waiting_connecting)
		{
			GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
			GUI.skin.label.fontSize = (int)((float)fontSize * 0.9f);
			try
			{
				GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
				string text = Lang.Get("netpanel_connectedplrs", false) + NetPlayer.ClientIdToPlayerDict.Count + "/" + KrokoshaScavMultiplayer.rules.PLAYER_COUNT_LIMIT;
				if (flag2)
				{
					if (GUILayout.Button(text, Array.Empty<GUILayoutOption>()))
					{
						SetOpen(open_or_nah: true);
						current_tab = menutab_directconnect;
						_CONNECTIONSECONDPANEL_TOOLBAR = 0;
					}
				}
				else
				{
					GUILayout.Label(text, Array.Empty<GUILayoutOption>());
				}
				GUILayout.EndHorizontal();
				scrollbullshit = GUILayout.BeginScrollView(scrollbullshit, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.ExpandHeight(true) });
				foreach (NetPlayer value in NetPlayer.ClientIdToPlayerDict.Values)
				{
					_GUI__PlrListDrawPlr(value);
				}
				GUI.skin.label.alignment = alignment;
				GUILayout.EndScrollView();
			}
			catch (Exception ex)
			{
				log.error("_GUI_DrawSideMenuWithPlayerList PLAYER LIST:\n" + ex.ToString());
			}
			GUI.skin.label.normal.textColor = Color.white;
			GUI.skin.label.fontSize = fontSize;
			GUILayout.EndVertical();
		}
		GUI.skin.label.alignment = (TextAnchor)0;
		GUILayout.FlexibleSpace();
		if (Time.realtimeSinceStartupAsDouble - KrokoshaScavMultiplayer.last_multiplayer_status_message_change_time < 10.0)
		{
			GUI.skin.label.wordWrap = true;
			GUI.skin.label.stretchHeight = true;
			GUI.skin.label.fontSize = (int)((float)fontSize * 0.75f);
			GUILayout.Label(Lang.Get("netpanel_laststatus", false) + "\n" + KrokoshaScavMultiplayer.multiplayer_status_message, Array.Empty<GUILayoutOption>());
			GUI.skin.label.wordWrap = wordWrap;
			GUI.skin.label.stretchHeight = stretchHeight;
		}
		GUILayout.FlexibleSpace();
		try
		{
			float num = ((Rect)(ref val)).width * 0.7f;
			GUI.skin.label.fontSize = fontSize;
			GUI.skin.label.wordWrap = false;
			GUI.skin.label.alignment = (TextAnchor)0;
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label("PACKET STABILITY: " + (Net.is_client ? ClientMain.MY_CONNECTION_QUALITY.ToString() : ServerMain.AVG_CONNECTION_QUALITY.ToString()) + "%", Array.Empty<GUILayoutOption>());
			if (UIBullshit.CheckCursorOverlap(GUIUtility.GUIToScreenRect(GUILayoutUtility.GetLastRect())))
			{
				UIBullshit._GUI_SetTooltip("", in UIBullshit.net_debug_msg);
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label("SERVER TPS: " + ClientMain.SERVER_TPS, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(num) });
			GUILayout.Label("FPS: " + ClientMain.SERVER_FPS, Array.Empty<GUILayoutOption>());
			GUILayout.EndHorizontal();
			if (Net.is_client)
			{
				GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
				GUILayout.Label("LOCAL TPS: " + ServerMain.CURRENT_TPS, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(num) });
				GUILayout.Label("FPS: " + ServerMain.CURRENT_FPS, Array.Empty<GUILayoutOption>());
				GUILayout.EndHorizontal();
			}
			GUI.skin.label.wordWrap = wordWrap;
		}
		catch (Exception ex2)
		{
			log.error("_GUI_DrawSideMenuWithPlayerList FOOTER:\n" + ex2.ToString());
		}
		GUILayout.EndArea();
		GUI.skin.label.fontSize = fontSize;
		GUI.skin.label.stretchHeight = stretchHeight;
		GUI.skin.label.margin = margin;
	}

	internal static void _GUI_DrawMPMainMenu()
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Expected O, but got Unknown
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Expected O, but got Unknown
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0559: Unknown result type (might be due to invalid IL or missing references)
		//IL_055e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0563: Unknown result type (might be due to invalid IL or missing references)
		//IL_0568: Unknown result type (might be due to invalid IL or missing references)
		//IL_0572: Unknown result type (might be due to invalid IL or missing references)
		//IL_0581: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0485: Unknown result type (might be due to invalid IL or missing references)
		//IL_0499: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
		if (!mainmenu_open)
		{
			return;
		}
		if (Util.IsInWorld() && (Object)(object)PlayerCamera.main != (Object)null)
		{
			PlayerCamera.main.radialOpen = false;
			if (PlayerCamera.main.tradeMenu.activeSelf)
			{
				PlayerCamera.main.ToggleTradeMenu();
			}
			if (Util.IsInWoundView())
			{
				PlayerCamera.main.ToggleWoundView(true);
			}
			PlayerCamera.main.craftingPanel.SetActive(false);
		}
		Vector2 val = new Vector2(MENU_SIZE_X, MENU_SIZE_Y) * GetMenuUIScale();
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(0f, 0f, val.x, val.y);
		((Rect)(ref rect)).center = UIBullshit.GetHalfScreenSize();
		UIBullshit._GUI_9SlicePanel(in rect, 1f, 0.6980392f, check_overlap: true);
		Rect val2 = rect.ShrinkBorder((float)UIBullshit.uiBlockSmallBorderSize);
		float num = GetMenuUIScale() * 56f;
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(rect);
		((Rect)(ref rect2)).yMin = ((Rect)(ref rect2)).yMin - num;
		((Rect)(ref rect2)).yMax = ((Rect)(ref rect)).yMin;
		UIBullshit._GUI_9SlicePanel(in rect2, 1f, 0.6980392f, check_overlap: true);
		UIBullshit.CheckCursorOverlap(in rect2);
		GUIStyle val3 = new GUIStyle(GUI.skin.label);
		GUIStyle val4 = new GUIStyle(GUI.skin.button);
		_ = GUI.skin.label.fixedHeight;
		int fontSize = GUI.skin.label.fontSize;
		_ = GUI.skin.label.stretchHeight;
		_ = GUI.skin.label.alignment;
		_ = GUI.skin.label.wordWrap;
		_ = GUI.skin.label.margin;
		val3.alignment = (TextAnchor)4;
		val3.fontSize = (int)((float)fontSize * 1.5f);
		GUI.Label(rect2, Lang.Get("mainmenu_settings", false), val3);
		Rect val5 = rect2.ShrinkBorder((float)UIBullshit.uiBlockSmallBorderSize);
		((Rect)(ref val5)).xMin = ((Rect)(ref val5)).xMax - ((Rect)(ref val5)).height;
		UIBullshit._GUI_SetButtonSkinTexture(val4, small: false);
		if (GUI.Button(val5, "X", val4))
		{
			SetOpen(open_or_nah: false);
		}
		float num2 = GetMenuUIScale() * 70f;
		Rect rect3 = val2.ShrinkBorder(-1f * GetMenuUIScale());
		((Rect)(ref rect3)).yMin = ((Rect)(ref rect3)).yMin + (((Rect)(ref val2)).height - num2);
		UIBullshit._GUI_9SlicePanel(in rect3, 0.5f, 0.6f, check_overlap: true, UIBullshit.unscaled_uiBlockNano);
		((Rect)(ref val2)).yMax = ((Rect)(ref rect3)).yMin - (float)UIBullshit.uiBlockNanoBorderSize;
		Rect val6 = rect3.ShrinkBorder(GetMenuUIScale() * 8f);
		GUILayout.BeginArea(val6);
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		float num3 = ((Rect)(ref val6)).width * (1f / ((float)ALL_MENU_TABS.Count + 1.5f));
		int fontSize2 = GUI.skin.label.fontSize;
		GUI.skin.label.fontSize = (int)((float)fontSize2 * 1.1f);
		for (int i = 0; i < ALL_MENU_TABS.Count; i++)
		{
			MenuTab menuTab = ALL_MENU_TABS[i];
			if (current_tab == menuTab)
			{
				GUI.backgroundColor = Color.white;
			}
			else
			{
				GUI.backgroundColor = Color.white * 0.8f;
			}
			if (GUILayout.Button(Lang.Get(in menuTab.name, false), (GUILayoutOption[])(object)new GUILayoutOption[3]
			{
				GUILayout.MaxWidth(num3),
				GUILayout.ExpandWidth(false),
				GUILayout.ExpandHeight(true)
			}))
			{
				current_tab = menuTab;
				if (current_tab.on_open != null)
				{
					current_tab.on_open();
				}
				OnMenuTabChange();
				Util.PlayUISound((UISoundType)1);
			}
		}
		GUILayout.FlexibleSpace();
		if (current_tab == menutab_about)
		{
			GUI.backgroundColor = Color.white;
		}
		else
		{
			GUI.backgroundColor = Color.white * 0.8f;
		}
		if (GUILayout.Button(Lang.Get(in menutab_about.name, false), (GUILayoutOption[])(object)new GUILayoutOption[3]
		{
			GUILayout.MaxWidth(num3),
			GUILayout.ExpandWidth(false),
			GUILayout.ExpandHeight(true)
		}))
		{
			current_tab = menutab_about;
			if (current_tab.on_open != null)
			{
				current_tab.on_open();
			}
			OnMenuTabChange();
			Util.PlayUISound((UISoundType)1);
		}
		GUI.skin.label.fontSize = fontSize2;
		GUI.backgroundColor = Color.white;
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
		GUILayout.BeginArea(val2);
		if (current_tab != null && current_tab.gui_func != null)
		{
			try
			{
				current_tab.gui_func(val2);
			}
			catch (Exception ex)
			{
				log.error("MENU TAB RENDER ERROR: " + current_tab.name + "\n" + ex.ToString());
			}
		}
		else
		{
			int fontSize3 = GUI.skin.label.fontSize;
			GUI.skin.label.fontSize = fontSize3 + fontSize3;
			GUILayout.Label("No tab selected.", (GUILayoutOption[])(object)new GUILayoutOption[2]
			{
				GUILayout.ExpandHeight(true),
				GUILayout.ExpandWidth(true)
			});
			GUI.skin.label.fontSize = fontSize3;
		}
		GUILayout.EndArea();
		if (!string.IsNullOrEmpty(_COLOR_ERRORS))
		{
			_COLOR_ERRORS_RECT = GUIUtility.ScreenToGUIRect(_COLOR_ERRORS_RECT);
			GUI.color = Color.red;
			GUI.Label(_COLOR_ERRORS_RECT, _COLOR_ERRORS);
			GUI.color = Color.white;
			_COLOR_ERRORS = null;
		}
	}

	public static void ParseInputColor(out bool is_valid)
	{
		if (INPUT_COLORHEX.Length > 1 && Color24.TryParseHex(INPUT_COLORHEX, out var color) && NetPlayer.CheckIfPlrColorIsValid(color))
		{
			is_valid = true;
			LAST_VALID_INPUT_COLOR = color;
		}
		else
		{
			is_valid = false;
		}
	}

	public static void SetInputColor(in Color24 c)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		INPUT_COLORHEX = ColorUtility.ToHtmlStringRGB((Color)c);
		LAST_VALID_INPUT_COLOR = c;
	}

	private static void _GUI___DirectConnect_DoSteamInfoo(Rect r2, float fieldheight, float fieldnameswidth, float fieldswidth)
	{
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		fieldnameswidth *= 1.1f;
		GUI.skin.label.alignment = (TextAnchor)0;
		GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Height(fieldheight) });
		GUILayout.Label(Lang.Get("mmct_steam_mypersona", false) + KSteam.GetLocalUsername(), Array.Empty<GUILayoutOption>());
		_GUI___DirectConnect_DoColorPickerThingy(Net.running);
		GUILayout.EndHorizontal();
		GUI.skin.label.alignment = (TextAnchor)2;
		GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Height(fieldheight) });
		float num = ((Rect)(ref r2)).width * 0.45f;
		GUILayout.Label(Lang.Get("mmct_steam_lobbytype", false), Array.Empty<GUILayoutOption>());
		UIBullshit._GUI_SetButtonSkinTexture(GUI.skin.button, small: false);
		GUI.skin.label.alignment = (TextAnchor)0;
		string[] options = new string[3]
		{
			Lang.Get("mmct_steam_lobbytype_private", false),
			Lang.Get("mmct_steam_lobbytype_friends", false),
			Lang.Get("mmct_steam_lobbytype_public", false)
		};
		if (Net.running && !Net.is_server)
		{
			GUILayout.Label("  ", Array.Empty<GUILayoutOption>());
		}
		else
		{
			int sTEAM_CHOSEN_LOBBYTYPE = _STEAM_CHOSEN_LOBBYTYPE;
			_STEAM_CHOSEN_LOBBYTYPE = GUILayout_DropdownMenu.Dropdown(_STEAM_CHOSEN_LOBBYTYPE, options, GUILayout.Width(num));
			if (Net.running && sTEAM_CHOSEN_LOBBYTYPE != _STEAM_CHOSEN_LOBBYTYPE)
			{
				ELobbyType val = KSteam.NumToLobbyType(_STEAM_CHOSEN_LOBBYTYPE);
				if (SteamMatchmaking.SetLobbyType(KSteam.CURRENT_LOBBY.lobby_steamID, val))
				{
					log.l($"STEAM: CHANGED LOBBY TYPE TO {val}");
				}
				else
				{
					_STEAM_CHOSEN_LOBBYTYPE = sTEAM_CHOSEN_LOBBYTYPE;
				}
			}
		}
		GUI.skin.label.alignment = (TextAnchor)0;
		UIBullshit._GUI_SetButtonSkinTexture(GUI.skin.button);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}

	private static void _GUI___DirectConnect_DoSteamConnectButtons(Rect r2, float fieldheight, float fieldnameswidth, float fieldswidth, float bigheight)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		GUILayout.FlexibleSpace();
		if (GUILayout.Button(Lang.Get("mmct_steam_createlobby", false), (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Height(bigheight) }))
		{
			ELobbyType val = KSteam.NumToLobbyType(_STEAM_CHOSEN_LOBBYTYPE);
			Net.NetType netType = ((!KrokoshaScavMultiplayer.SERVER_TOGGLE_SHOULD_HOST_DEDICATED) ? Net.NetType.Host : Net.NetType.DedicatedServer);
			KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog($"User pressed Create Lobby! {val} -> {netType}");
			TransportSteamworks.OnWantToHostLobby(netType, val);
			Util.PlayUISound((UISoundType)1);
		}
		GUILayout.FlexibleSpace();
		if (GUILayout.Button(Lang.Get("mmct_steam_openfriendlist", false), (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Height(bigheight) }))
		{
			KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog("Open Friends list!");
			SteamFriends.ActivateGameOverlay("friends");
			Util.PlayUISound((UISoundType)1);
		}
		GUILayout.FlexibleSpace();
	}

	internal static void _GUI___DirectConnect_DoPasswordField(Rect r2, float fieldheight, float fieldnameswidth, float fieldswidth)
	{
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Height(fieldheight) });
		GUI.skin.label.alignment = (TextAnchor)5;
		GUILayout.Label(Lang.Get("mmct_passwordfield", false) + ":  ", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(fieldnameswidth) });
		GUI.skin.label.alignment = (TextAnchor)0;
		bool flag = !KrokoshaScavMultiplayer.network_system_is_running || KrokoshaScavMultiplayer.is_server;
		if (_UNHIDE_PASSWORD)
		{
			GUILayout.Label(KrokoshaScavMultiplayer.INPUT_PASSWORD, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(fieldswidth) });
		}
		else if (flag)
		{
			KrokoshaScavMultiplayer.INPUT_PASSWORD = GUILayout.PasswordField(KrokoshaScavMultiplayer.INPUT_PASSWORD, '*', (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(fieldswidth) });
		}
		else
		{
			GUILayout.Label(new string('*', KrokoshaScavMultiplayer.INPUT_PASSWORD.Length), (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(fieldswidth) });
		}
		GUILayout.Button(Lang.Get("mmct_show", false), Array.Empty<GUILayoutOption>());
		Rect lastRect = GUILayoutUtility.GetLastRect();
		if (Input.GetKey((KeyCode)323))
		{
			if (UIBullshit.IsCursorInGUIRect(GUIUtility.GUIToScreenRect(lastRect)))
			{
				_UNHIDE_PASSWORD = true;
			}
		}
		else
		{
			_UNHIDE_PASSWORD = false;
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}

	private static void _GUI___DirectConnect_DoColorPickerThingy(bool textfields_are_readonly)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Expected O, but got Unknown
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = GUI.skin.label.CalcSize(new GUIContent("#FFFFFF"));
		Vector2 val2 = GUI.skin.label.CalcSize(new GUIContent(INPUT_COLORHEX));
		if (val.x < val2.x)
		{
			val.x = val2.x;
		}
		ParseInputColor(out var is_valid);
		GUI.color = LAST_VALID_INPUT_COLOR;
		if (!textfields_are_readonly)
		{
			if (GUILayout.Button("", (GUILayoutOption[])(object)new GUILayoutOption[2]
			{
				GUILayout.Height(val.y),
				GUILayout.Width(val.y)
			}))
			{
				List<Color> list = NetPlayer.NAMETAG_DEFAULT_COLORS.ToList();
				if (KrokoshaScavMultiplayer.rules.Teams)
				{
					foreach (KeyValuePair<knetid, NetPlayer> item in NetPlayer.ClientIdToPlayerDict)
					{
						if (!list.Contains(item.Value.plrcolor))
						{
							list.Add(item.Value.plrcolor);
						}
					}
				}
				int num = list.IndexOf(LAST_VALID_INPUT_COLOR);
				if (num == -1)
				{
					num = Random.Range(0, list.Count);
				}
				num++;
				if (num >= list.Count)
				{
					num = 0;
				}
				SetInputColor((Color24)list[num]);
				Util.PlayUISound((UISoundType)1);
			}
			if (!is_valid)
			{
				GUI.color = Color.white;
			}
			INPUT_COLORHEX = GUILayout.TextField(INPUT_COLORHEX, 7, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(val.x) });
			if (INPUT_COLORHEX.Length > 6 && !StringUtility.StartsWith(INPUT_COLORHEX, '#'))
			{
				INPUT_COLORHEX = INPUT_COLORHEX.Substring(0, 6);
			}
			_COLOR_ERRORS_RECT = GUIUtility.GUIToScreenRect(GUILayoutUtility.GetLastRect());
			((Rect)(ref _COLOR_ERRORS_RECT)).x = ((Rect)(ref _COLOR_ERRORS_RECT)).xMax;
			((Rect)(ref _COLOR_ERRORS_RECT)).width = ((Rect)(ref _COLOR_ERRORS_RECT)).width * 10f;
			if (!is_valid)
			{
				if (Color24.TryParseHex(INPUT_COLORHEX, out var color) && !NetPlayer.CheckIfPlrColorIsValid(color))
				{
					_COLOR_ERRORS = Lang.Get("mmct_colortoodark", false);
				}
				else
				{
					_COLOR_ERRORS = Lang.Get("mmct_colornotvalid", false);
				}
			}
		}
		else
		{
			GUILayout.Space(val.y);
			GUILayout.Label(((Object)(object)NetPlayer.LOCAL_PLAYER != (Object)null) ? NetPlayer.LOCAL_PLAYER.plrcolor.ToHex() : INPUT_COLORHEX, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(val.x) });
		}
		GUI.color = Color.white;
	}

	public static void DoTooltipOnLastGUIRect(in string lang_key)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (UIBullshit.CheckCursorOverlap(GUIUtility.GUIToScreenRect(GUILayoutUtility.GetLastRect())))
		{
			UIBullshit._GUI_SetTooltip("", Lang.Get(in lang_key, false));
		}
	}

	public static void DoTooltipOnLastGUIRectNoLang(in string text)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (UIBullshit.CheckCursorOverlap(GUIUtility.GUIToScreenRect(GUILayoutUtility.GetLastRect())))
		{
			UIBullshit._GUI_SetTooltip("", in text);
		}
	}

	private static void _GUI___DoServerConfig(Rect r2)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		float num = 50f * GetMenuUIScale();
		TextAnchor alignment = GUI.skin.label.alignment;
		GUI.skin.label.alignment = (TextAnchor)1;
		UIBullshit._GUI_BiggerLabel(Lang.Get("mmct_serveroptions", false), 1.4f);
		GUI.skin.label.alignment = (TextAnchor)0;
		bool enabled = GUI.enabled;
		if (Net.running)
		{
			GUI.enabled = false;
		}
		KrokoshaScavMultiplayer.SERVER_TOGGLE_SHOULD_HOST_DEDICATED = GUILayout.Toggle(KrokoshaScavMultiplayer.SERVER_TOGGLE_SHOULD_HOST_DEDICATED, Lang.Get("mmct_sconfig_dedicated", false), Array.Empty<GUILayoutOption>());
		DoTooltipOnLastGUIRect(GUI.enabled ? "mmct_sconfig_dedicated_tooltip" : "mmct_sconfig_dedicated_disabled");
		GUI.enabled = enabled;
		KrokoshaScavMultiplayer.SERVER_TOGGLE_CHECK_GAME_HASH = GUILayout.Toggle(KrokoshaScavMultiplayer.SERVER_TOGGLE_CHECK_GAME_HASH, Lang.Get("mmct_sconfig_verifyhash", false), Array.Empty<GUILayoutOption>());
		DoTooltipOnLastGUIRect("mmct_sconfig_verifyhash_tooltip");
		KrokoshaScavMultiplayer.SERVER_TOGGLE_ENFORCE_MODLIST = GUILayout.Toggle(KrokoshaScavMultiplayer.SERVER_TOGGLE_ENFORCE_MODLIST, Lang.Get("mmct_sconfig_enforcemodlist", false), Array.Empty<GUILayoutOption>());
		DoTooltipOnLastGUIRect("mmct_sconfig_enforcemodlist_tooltip");
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		GUI.skin.label.alignment = (TextAnchor)5;
		GUILayout.Label(Lang.Get("mmct_sconfig_plrcount", false), Array.Empty<GUILayoutOption>());
		if (int.TryParse(GUILayout.TextField(KrokoshaScavMultiplayer.PLAYER_COUNT_LIMIT.ToString(), (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.MinWidth(num) }), out var result))
		{
			KrokoshaScavMultiplayer.PLAYER_COUNT_LIMIT = (byte)Mathf.Clamp(result, 1, 200);
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUI.skin.label.alignment = alignment;
		GUILayout.FlexibleSpace();
		GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Height(num) });
		GUILayout.Space(10f * GetMenuUIScale());
		if (GUILayout.Button(Lang.Get("back", false), (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.MinWidth(((Rect)(ref r2)).width * 0.25f) }))
		{
			_DO_SERVER_CONFIG = false;
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.FlexibleSpace();
	}

	private static void _GUI__DirectConnect(Rect r2)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0885: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0832: Unknown result type (might be due to invalid IL or missing references)
		//IL_0843: Expected O, but got Unknown
		//IL_07bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0664: Unknown result type (might be due to invalid IL or missing references)
		int fontSize = GUI.skin.label.fontSize;
		TextAnchor alignment = GUI.skin.label.alignment;
		try
		{
			if (_DO_SERVER_CONFIG)
			{
				if (!Net.is_connected || !Net.is_client)
				{
					_GUI___DoServerConfig(r2);
					return;
				}
				_DO_SERVER_CONFIG = false;
			}
			float num = ((Rect)(ref r2)).width * 0.27f;
			float fieldswidth = ((Rect)(ref r2)).width * 0.36f;
			float num2 = 50f * GetMenuUIScale();
			float num3 = 30f * GetMenuUIScale();
			float num4 = ((Rect)(ref r2)).width * 0.45f;
			int num5 = 0;
			if (KSteam.Loaded && (_USE_STEAM_MENU || (Net.TRANSPORT != null && Net.TRANSPORT is TransportSteamworks)))
			{
				num5 = 1;
			}
			GUI.skin.label.alignment = (TextAnchor)1;
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			int num6;
			if (!Plugin.FORCE_NO_STEAM && !Net.running)
			{
				num6 = (KSteam.Loaded ? 1 : 0);
				if (num6 != 0)
				{
					GUILayout.FlexibleSpace();
				}
			}
			else
			{
				num6 = 0;
			}
			UIBullshit._GUI_BiggerLabel(_USE_STEAM_MENU ? Lang.Get("mmct_steamlobby", false) : Lang.Get("mmct_directconnecttitle", false), 1.4f);
			GUI.skin.label.fontSize = fontSize;
			GUI.skin.label.alignment = (TextAnchor)5;
			if (num6 != 0)
			{
				_USE_STEAM_MENU = GUILayout.Toggle(_USE_STEAM_MENU, Lang.Get("mmct_steammenutoggle", false), Array.Empty<GUILayoutOption>());
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Height(num3) });
			GUILayout.Label(Lang.Get("mmct_sconfig_servername", false), (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.ExpandWidth(false) });
			GUI.skin.label.alignment = (TextAnchor)0;
			if (!Net.running || Net.is_server)
			{
				string text = GUILayout.TextField(Net.MY_SERVER_INFO.name, Array.Empty<GUILayoutOption>());
				if (text.Length > 32)
				{
					text = text.Substring(0, 32);
				}
				if (text != Net.MY_SERVER_INFO.name)
				{
					text = KrokoshaScavMultiplayer.SanitizeTextInputAllowSpaces(text);
					Net.MY_SERVER_INFO.name = text;
					Net.cur_server_info.name = text;
					if (Net.running)
					{
						ServerMain._Server_OnServerNameChange();
					}
				}
			}
			else
			{
				GUILayout.Label(Net.cur_server_info.name, Array.Empty<GUILayoutOption>());
			}
			GUI.skin.label.alignment = (TextAnchor)0;
			GUILayout.EndHorizontal();
			bool running = Net.running;
			switch (num5)
			{
			case 0:
				GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Height(num3) });
				GUILayout.Label(Lang.Get("mmct_ipportfield", false) + ":  ", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(num) });
				GUI.skin.label.alignment = (TextAnchor)0;
				if (running)
				{
					if (_HIDE_IPPORT)
					{
						GUILayout.Label(new string('*', USERINPUT_IPPORT.Length), (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(num4) });
					}
					else
					{
						GUILayout.Label(USERINPUT_IPPORT, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(num4) });
					}
				}
				else if (_HIDE_IPPORT)
				{
					USERINPUT_IPPORT = GUILayout.PasswordField(USERINPUT_IPPORT, '*', (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(num4) });
				}
				else
				{
					USERINPUT_IPPORT = GUILayout.TextField(USERINPUT_IPPORT, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(num4) });
				}
				GUI.skin.label.alignment = (TextAnchor)0;
				_HIDE_IPPORT = GUILayout.Toggle(_HIDE_IPPORT, Lang.Get("mmct_hideip", false), Array.Empty<GUILayoutOption>());
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Height(num3) });
				_GUI___DrawNameAndColorFields(num, fieldswidth, running);
				GUILayout.EndHorizontal();
				break;
			case 1:
				_GUI___DirectConnect_DoSteamInfoo(r2, num3, num, fieldswidth);
				break;
			}
			_GUI___DirectConnect_DoPasswordField(r2, num3, num, fieldswidth);
			GUILayout.Space(20f * GetMenuUIScale());
			if (KrokoshaScavMultiplayer.network_system_is_running && KrokoshaScavMultiplayer.network_system_im_client_and_waiting_connecting)
			{
				GUILayout.Label(Lang.Get("mmct_connecting", false), Array.Empty<GUILayoutOption>());
			}
			else if (KrokoshaScavMultiplayer.is_server)
			{
				GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Height(num2) });
				if (KrokoshaScavMultiplayer.is_server)
				{
					if (GUILayout.Button(Lang.Get("mmct_serveroptions", false), Array.Empty<GUILayoutOption>()))
					{
						_DO_SERVER_CONFIG = true;
					}
					GUILayout.FlexibleSpace();
				}
				if (KSteam.IS_IN_LOBBY && ((CSteamID)(ref KSteam.CURRENT_LOBBY.lobby_steamID)).IsValid())
				{
					if (GUILayout.Button(Lang.Get("mmct_copylobbycode", false), Array.Empty<GUILayoutOption>()))
					{
						GUIUtility.systemCopyBuffer = ((object)Unsafe.As<CSteamID, CSteamID>(ref KSteam.CURRENT_LOBBY.lobby_steamID)/*cast due to constrained. prefix*/).ToString();
					}
					GUILayout.FlexibleSpace();
				}
				if (Net.is_connected && Util.IsInMainMenu() && (Object)(object)PreRunScript.instance != (Object)null && GUILayout.Button(Lang.Get("mmct_gotorunsettings", false), Array.Empty<GUILayoutOption>()))
				{
					SetOpen(open_or_nah: false);
					PreRunScript.instance.runSettingsScreen.SetActive(true);
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(15f * GetMenuUIScale());
			}
			GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Height(num2) });
			if (KrokoshaScavMultiplayer.network_system_is_running)
			{
				GUILayout.FlexibleSpace();
				if (GUILayout.Button((!KrokoshaScavMultiplayer.is_client) ? Lang.Get("mmct_disconnect_server", false) : (KrokoshaScavMultiplayer.network_system_im_client_and_waiting_connecting ? Lang.Get("mmct_disconnect_abort", false) : Lang.Get("mmct_disconnect", false)), (GUILayoutOption[])(object)new GUILayoutOption[2]
				{
					GUILayout.MinWidth(num2 * 3f),
					GUILayout.Height(num2)
				}))
				{
					KrokoshaScavMultiplayer.ShutdownNetwork();
					Chat.LogMessage("*SYSTEM*", "Disconnected by the user.", false);
					KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog("Disconnected by the user.");
					Util.PlayUISound((UISoundType)2);
				}
				if (Net.is_playing_with_steam && KSteam.IS_IN_LOBBY && ((!KSteam.CURRENT_LOBBY.locked && !Net.cur_server_info.midjoin_lock_active) || Net.is_server))
				{
					GUILayout.FlexibleSpace();
					if (GUILayout.Button(Lang.Get("mmct_steam_invitefriends", false), (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Height(num2) }))
					{
						KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog("Open Friends Invite dialog!");
						SteamFriends.ActivateGameOverlayInviteDialog(KSteam.CURRENT_LOBBY.lobby_steamID);
						Util.PlayUISound((UISoundType)1);
					}
				}
				GUILayout.FlexibleSpace();
			}
			else if (Util.IsInWorld())
			{
				GUILayout.Label(Lang.Get("mmct_canthostinworld", false), Array.Empty<GUILayoutOption>());
			}
			else
			{
				switch (num5)
				{
				case 0:
					if (KrokoshaScavMultiplayer.SERVER_TOGGLE_SHOULD_HOST_DEDICATED)
					{
						if (GUILayout.Button(Lang.Get("mmct_startserver", false), (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Height(num2) }))
						{
							KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog("Start Server clicked!");
							TransportLiteNetLib.OnWantToConnect(USERINPUT_IPPORT, Net.NetType.DedicatedServer);
							Util.PlayUISound((UISoundType)1);
						}
					}
					else if (GUILayout.Button(Lang.Get("mmct_starthost", false), (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Height(num2) }))
					{
						KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog("Start Host clicked!");
						TransportLiteNetLib.OnWantToConnect(USERINPUT_IPPORT, Net.NetType.Host);
						Util.PlayUISound((UISoundType)1);
					}
					if (Util.IsInMainMenu() && GUILayout.Button(Lang.Get("mmct_startclient", false), (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Height(num2) }))
					{
						KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog("Client Connect clicked!");
						TransportLiteNetLib.OnWantToConnect(USERINPUT_IPPORT, Net.NetType.Client);
						Util.PlayUISound((UISoundType)1);
					}
					break;
				case 1:
					_GUI___DirectConnect_DoSteamConnectButtons(r2, num3, num, fieldswidth, num2);
					break;
				default:
					GUILayout.Label("get tf outta my room im playin minecraft!!", Array.Empty<GUILayoutOption>());
					break;
				}
			}
			GUILayout.EndHorizontal();
			bool wordWrap = GUI.skin.label.wordWrap;
			GUI.skin.label.wordWrap = true;
			string text2 = Lang.Get("mmct_last_status_msg", false) + "\n" + KrokoshaScavMultiplayer.multiplayer_status_message;
			float num7 = GUI.skin.label.CalcHeight(new GUIContent(text2), ((Rect)(ref r2)).width);
			GUILayout.Label(text2, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Height(num7) });
			GUI.skin.label.wordWrap = wordWrap;
		}
		catch (Exception ex)
		{
			log.error(ex.ToString());
		}
		GUI.skin.label.alignment = alignment;
		GUI.skin.label.fontSize = fontSize;
	}

	private static void _GUI___DrawNameAndColorFields(float fieldnameswidth, float fieldswidth, bool textfields_are_readonly)
	{
		GUI.skin.label.alignment = (TextAnchor)5;
		GUILayout.Label(Lang.Get("mmct_usernamefield", false) + ":  ", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(fieldnameswidth) });
		if (textfields_are_readonly)
		{
			GUI.skin.label.alignment = (TextAnchor)0;
			GUILayout.Label(USERINPUT_NAME, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(fieldswidth) });
		}
		else
		{
			string text = USERINPUT_NAME;
			if (string.IsNullOrEmpty(text))
			{
				text = "";
			}
			USERINPUT_NAME = GUILayout.TextField(text, 16, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(fieldswidth) });
		}
		_GUI___DirectConnect_DoColorPickerThingy(textfields_are_readonly);
		GUILayout.FlexibleSpace();
	}

	private static void _GUI__ConnectionSecondPanel(Rect r2)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		int fontSize = GUI.skin.label.fontSize;
		TextAnchor alignment = GUI.skin.label.alignment;
		try
		{
			GUI.skin.label.fontSize = (int)((float)fontSize * 1.4f);
			GUI.skin.label.alignment = (TextAnchor)1;
			_CONNECTIONSECONDPANEL_TOOLBAR_STRS[0] = Lang.Get("mmct_plrs", false);
			_CONNECTIONSECONDPANEL_TOOLBAR_STRS[1] = Lang.Get("mmct_rules", false);
			_CONNECTIONSECONDPANEL_TOOLBAR = GUILayout.Toolbar(_CONNECTIONSECONDPANEL_TOOLBAR, _CONNECTIONSECONDPANEL_TOOLBAR_STRS, Array.Empty<GUILayoutOption>());
			GUI.skin.label.fontSize = fontSize;
			GUI.skin.label.alignment = (TextAnchor)0;
			_ = Net.running;
			UIBullshit._GUI_SetButtonSkinTexture(GUI.skin.button, small: false);
			_GUI__ConnectionSecondPanel_ContentRender(r2);
		}
		catch (Exception ex)
		{
			log.error(ex.ToString());
		}
		UIBullshit._GUI_SetButtonSkinTexture(GUI.skin.button);
		GUI.skin.label.alignment = alignment;
		GUI.skin.label.fontSize = fontSize;
	}

	private static void _GUI___ConnectionSecondPanel_ContentRender_Top(Rect r)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		if (____GUI___ConnectionSecondPanel_ContentRender_RenderDetailedPlayerList_gaben && KSteam.IS_IN_LOBBY)
		{
			if (GUILayout.Button(Lang.Get("mmct_hoststeamprofile", false), Array.Empty<GUILayoutOption>()))
			{
				SteamFriends.ActivateGameOverlayToUser("steamid", KSteam.CURRENT_LOBBY.ownerID);
				Util.PlayUISound((UISoundType)1);
			}
			GUILayout.FlexibleSpace();
		}
		if (Con.CanExecuteAdminCommands())
		{
			____GUI___ConnectionSecondPanel_ContentRender_RenderDetailedPlayerList_ban_kick_safety = GUILayout.Toggle(____GUI___ConnectionSecondPanel_ContentRender_RenderDetailedPlayerList_ban_kick_safety, Lang.Get("mmct_bankicksafety", false), Array.Empty<GUILayoutOption>());
		}
		GUILayout.EndHorizontal();
	}

	private static void _GUI___ConnectionSecondPanel_ContentRender_RenderDetailedPlayerList(Rect r)
	{
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		if (!Net.running)
		{
			GUI.skin.label.alignment = (TextAnchor)4;
			GUILayout.Label(Lang.Get("mmct_offline", false), Array.Empty<GUILayoutOption>());
			return;
		}
		if (NetPlayer.ClientIdToPlayerDict.Count > 0)
		{
			bool enabled = GUI.enabled;
			{
				foreach (NetPlayer item in new List<NetPlayer>(NetPlayer.ClientIdToPlayerDict.Values))
				{
					GUI.skin.label.alignment = (TextAnchor)0;
					GUILayout.BeginVertical(GUI.skin.box, Array.Empty<GUILayoutOption>());
					try
					{
						_GUI___DrawPlayerInList(r, item);
					}
					catch (Exception ex)
					{
						log.error($"_GUI___ConnectionSecondPanel_ContentRender_RenderDetailedPlayerList PLR {item} " + ex.ToString());
					}
					GUI.enabled = enabled;
					GUILayout.EndVertical();
				}
				return;
			}
		}
		GUI.skin.label.alignment = (TextAnchor)4;
		GUILayout.Label(Lang.Get("mmct_nobody", false), Array.Empty<GUILayoutOption>());
	}

	private static void _GUI___ConnectionSecondPanel_ContentRender_RenderRulesList(Rect r2)
	{
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		bool flag = Con.CanExecuteAdminCommands();
		GUI.skin.label.alignment = (TextAnchor)2;
		if (flag)
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label(Lang.Get("mmct_presetname", false), Array.Empty<GUILayoutOption>());
			_GUI____RenderRuleField_PresetName = GUILayout.TextField(_GUI____RenderRuleField_PresetName, (GUILayoutOption[])(object)new GUILayoutOption[2]
			{
				GUILayout.ExpandWidth(false),
				GUILayout.Width(((Rect)(ref r2)).width * 0.23f)
			});
			bool enabled = GUI.enabled;
			if (GUI.enabled)
			{
				GUI.enabled = !string.IsNullOrWhiteSpace(_GUI____RenderRuleField_PresetName);
			}
			if (GUILayout.Button(Lang.Get("mmct_presetsave", false), Array.Empty<GUILayoutOption>()))
			{
				Util.PlayUISound((UISoundType)1);
				RulesPresetsJsonThing.SetRules(_GUI____RenderRuleField_PresetName, KrokoshaScavMultiplayer.rules);
				RulesPresetsJsonThing.Save();
				int num = RulesPresetsJsonThing.GetPresets().ToList().IndexOf(_GUI____RenderRuleField_PresetName);
				if (num != -1)
				{
					_GUI____RenderRuleField_PresetChosen = num + 1;
				}
			}
			if (GUILayout.Button(Lang.Get("mmct_presetremove", false), Array.Empty<GUILayoutOption>()) && RulesPresetsJsonThing.HasKey(_GUI____RenderRuleField_PresetName))
			{
				Util.PlayUISound((UISoundType)1);
				RulesPresetsJsonThing.DeleteKey(_GUI____RenderRuleField_PresetName);
				RulesPresetsJsonThing.Save();
			}
			GUI.enabled = enabled;
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label(Lang.Get("mmct_presetselect", false), Array.Empty<GUILayoutOption>());
			List<string> list = new List<string> { "[no preset]" };
			IEnumerable<string> presets = RulesPresetsJsonThing.GetPresets();
			list.AddRange(presets);
			int gUI____RenderRuleField_PresetChosen = _GUI____RenderRuleField_PresetChosen;
			_GUI____RenderRuleField_PresetChosen = GUILayout_DropdownMenu.Dropdown(_GUI____RenderRuleField_PresetChosen, list.ToArray());
			int num2 = _GUI____RenderRuleField_PresetChosen - 1;
			if (gUI____RenderRuleField_PresetChosen != _GUI____RenderRuleField_PresetChosen && num2 >= 0 && presets.Count() > 0)
			{
				_GUI____RenderRuleField_PresetName = presets.ElementAt(num2);
				KrokoshaScavMultiplayer.rules = RulesPresetsJsonThing.GetRules(_GUI____RenderRuleField_PresetName);
				KrokoshaScavMultiplayer.ApplyGameRules();
			}
			GUILayout.FlexibleSpace();
			if (flag && GUILayout.Button(Lang.Get("mmct_reset_rules", false), Array.Empty<GUILayoutOption>()))
			{
				Util.PlayUISound((UISoundType)1);
				Con.ExecuteCommandAdminNoLog("resetrules");
			}
			GUILayout.EndHorizontal();
		}
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		GUILayout.FlexibleSpace();
		GUILayout.Label(Lang.Get("mmct_search", false), Array.Empty<GUILayoutOption>());
		_GUI____RenderRuleField_search = GUILayout.TextField(_GUI____RenderRuleField_search, (GUILayoutOption[])(object)new GUILayoutOption[2]
		{
			GUILayout.ExpandWidth(false),
			GUILayout.Width(((Rect)(ref r2)).width * 0.3f)
		});
		GUILayout.EndHorizontal();
		_SCROLL_FULLRULELIST = GUILayout.BeginScrollView(_SCROLL_FULLRULELIST, Array.Empty<GUILayoutOption>());
		List<FieldInfo> list2 = typeof(KrokoshaMultiplayerGameRules).GetFields(BindingFlags.Instance | BindingFlags.Public).ToList();
		if (!string.IsNullOrWhiteSpace(_GUI____RenderRuleField_search))
		{
			list2 = list2.Where((FieldInfo f) => StringUtility.ContainsInsensitive(f.Name, _GUI____RenderRuleField_search)).ToList();
		}
		if (list2.Count == 0)
		{
			GUI.skin.label.alignment = (TextAnchor)1;
			GUILayout.Label(Lang.Get("filter_found_nothing", false), Array.Empty<GUILayoutOption>());
		}
		else
		{
			_GUI____RenderRuleField_cur_max_width = 0f;
			foreach (FieldInfo item in list2)
			{
				GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
				try
				{
					_GUI____RenderRuleField(item, flag, in r2);
				}
				catch (Exception ex)
				{
					log.error($"_GUI___ConnectionSecondPanel_ContentRender_RenderRulesList: \nFLD: {item} \n{ex.ToString()}");
				}
				GUILayout.EndHorizontal();
			}
			_GUI____RenderRuleField_last_max_width = Mathf.Min(((Rect)(ref r2)).width * 0.5f, _GUI____RenderRuleField_cur_max_width);
		}
		GUILayout.EndScrollView();
	}

	private static void _GUI____RenderRuleField(FieldInfo fld, bool can_edit, in Rect r2)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		GUI.skin.label.wordWrap = false;
		Vector2 val = GUI.skin.label.CalcSize(new GUIContent(fld.Name));
		if (val.x > _GUI____RenderRuleField_cur_max_width)
		{
			_GUI____RenderRuleField_cur_max_width = val.x;
		}
		string text = null;
		object value = fld.GetValue(KrokoshaScavMultiplayer.rules);
		Rect rect;
		if (!can_edit)
		{
			GUI.skin.label.alignment = (TextAnchor)0;
			string text2 = fld.Name + ": ";
			text2 = ((!(fld.FieldType == typeof(float))) ? (text2 + $" {value}") : (text2 + $" {Math.Round((float)value, 1)}"));
			GUILayout.Label(text2, Array.Empty<GUILayoutOption>());
			rect = GUIUtility.GUIToScreenRect(GUILayoutUtility.GetLastRect());
			flag = UIBullshit.CheckCursorOverlap(in rect);
		}
		else
		{
			GUI.skin.label.alignment = (TextAnchor)5;
			GUILayout.Label(fld.Name, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(_GUI____RenderRuleField_last_max_width) });
			rect = GUIUtility.GUIToScreenRect(GUILayoutUtility.GetLastRect());
			flag = UIBullshit.CheckCursorOverlap(in rect);
			TypedReference obj = __makeref(KrokoshaScavMultiplayer.rules);
			bool flag2 = false;
			GUI.skin.label.alignment = (TextAnchor)3;
			if (fld.FieldType == typeof(bool))
			{
				bool flag3 = GUILayout.Toggle((bool)value, "", Array.Empty<GUILayoutOption>());
				fld.SetValueDirect(obj, flag3);
				flag2 = flag3 != (bool)value;
			}
			else if (AttributeUtility.HasAttribute((MemberInfo)fld, typeof(KrokoshaRuleByteAttribute), true))
			{
				KrokoshaRuleByteAttribute attribute = AttributeUtility.GetAttribute<KrokoshaRuleByteAttribute>((MemberInfo)fld, true);
				float num = (int)(byte)value;
				float num2 = (float)Math.Round(GUILayout.HorizontalSlider(num, 0f, (float)(int)attribute.limit, Array.Empty<GUILayoutOption>()), 1);
				int num3;
				if (!flag)
				{
					rect = GUIUtility.GUIToScreenRect(GUILayoutUtility.GetLastRect());
					num3 = (UIBullshit.CheckCursorOverlap(in rect) ? 1 : 0);
				}
				else
				{
					num3 = 1;
				}
				flag = (byte)num3 != 0;
				fld.SetValueDirect(obj, (byte)Mathf.RoundToInt(num2));
				text = Math.Round(num2) + attribute.postfix;
				string text3 = text;
				GUILayoutOption[] obj2 = new GUILayoutOption[2]
				{
					GUILayout.ExpandWidth(false),
					default(GUILayoutOption)
				};
				rect = r2;
				obj2[1] = GUILayout.Width(((Rect)(ref rect)).width * 0.1f);
				GUILayout.Label(text3, (GUILayoutOption[])(object)obj2);
				flag2 = num2 != num;
			}
			else if (fld.FieldType == typeof(float))
			{
				float num4 = 0f;
				float num5 = 10f;
				KrokoshaRuleFloatAttribute attribute2 = AttributeUtility.GetAttribute<KrokoshaRuleFloatAttribute>((MemberInfo)fld, true);
				if (attribute2 != null)
				{
					num4 = attribute2.mi;
					num5 = attribute2.ma;
				}
				float num6 = (float)value;
				float num7 = (float)Math.Round(GUILayout.HorizontalSlider(num6, Mathf.Min(num6, num4), Mathf.Max(num6, num5), Array.Empty<GUILayoutOption>()), 1);
				int num8;
				if (!flag)
				{
					rect = GUIUtility.GUIToScreenRect(GUILayoutUtility.GetLastRect());
					num8 = (UIBullshit.CheckCursorOverlap(in rect) ? 1 : 0);
				}
				else
				{
					num8 = 1;
				}
				flag = (byte)num8 != 0;
				fld.SetValueDirect(obj, num7);
				if (attribute2 != null)
				{
					text = num7.ToString("0.0") + attribute2.postfix;
					string text4 = text;
					GUILayoutOption[] obj3 = new GUILayoutOption[2]
					{
						GUILayout.ExpandWidth(false),
						default(GUILayoutOption)
					};
					rect = r2;
					obj3[1] = GUILayout.Width(((Rect)(ref rect)).width * 0.1f);
					GUILayout.Label(text4, (GUILayoutOption[])(object)obj3);
				}
				else
				{
					text = num7.ToString("0.0");
				}
				flag2 = num7 != (float)value;
			}
			else if (fld.FieldType == typeof(int) || fld.FieldType == typeof(byte) || fld.FieldType == typeof(ushort))
			{
				if (int.TryParse(GUILayout.TextField(value.ToString(), Array.Empty<GUILayoutOption>()), out var result))
				{
					fld.SetValueDirect(obj, result);
					flag2 = result.ToString() != value.ToString();
				}
			}
			else
			{
				GUILayout.Label($"[{fld.FieldType.Name}]: {value} ", Array.Empty<GUILayoutOption>());
			}
			if (flag2)
			{
				if (KrokoshaScavMultiplayer.IsNetworkActiveAndIsClient())
				{
					if (Con.client_isadmin)
					{
						object value2 = fld.GetValue(KrokoshaScavMultiplayer.rules);
						string command = "rule " + fld.Name + " " + value2.ToStringInvariant();
						KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog("Sending command: \"" + command + "\"");
						Con.ExecuteCommandAdminNoLog(in command);
					}
				}
				else
				{
					KrokoshaScavMultiplayer.ApplyGameRules();
				}
			}
		}
		if (flag)
		{
			if (!Lang.TryGet("rule_desc_" + fld.Name, out var text5))
			{
				text5 = "";
			}
			if (!string.IsNullOrWhiteSpace(text))
			{
				text5 = ((!string.IsNullOrWhiteSpace(text5)) ? (text5 + "\n" + text) : text);
			}
			if (!string.IsNullOrWhiteSpace(text5))
			{
				UIBullshit._GUI_SetTooltip("", in text5);
			}
		}
	}

	private static void _GUI__ConnectionSecondPanel_ContentRender(Rect r2)
	{
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		int fontSize = GUI.skin.label.fontSize;
		int fontSize2 = GUI.skin.button.fontSize;
		GUI.skin.label.alignment = (TextAnchor)0;
		GUI.skin.label.fontSize = (int)((float)fontSize * 0.85f);
		GUI.skin.button.fontSize = GUI.skin.label.fontSize;
		if (_CONNECTIONSECONDPANEL_TOOLBAR == 0)
		{
			GUI.skin.label.wordWrap = false;
			if (Net.running)
			{
				_GUI___ConnectionSecondPanel_ContentRender_Top(r2);
			}
			_SCROLL_FULLPLRLIST = GUILayout.BeginScrollView(_SCROLL_FULLPLRLIST, Array.Empty<GUILayoutOption>());
			_GUI___ConnectionSecondPanel_ContentRender_RenderDetailedPlayerList(r2);
			GUILayout.EndScrollView();
		}
		else if (_CONNECTIONSECONDPANEL_TOOLBAR == 1)
		{
			_GUI___ConnectionSecondPanel_ContentRender_RenderRulesList(r2);
		}
		GUI.skin.label.alignment = (TextAnchor)0;
		GUI.skin.label.fontSize = fontSize;
		GUI.skin.button.fontSize = fontSize2;
	}

	private static void _GUI___DrawPlayerInList(Rect r, NetPlayer plr)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Expected O, but got Unknown
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0463: Unknown result type (might be due to invalid IL or missing references)
		//IL_046a: Expected O, but got Unknown
		//IL_0476: Unknown result type (might be due to invalid IL or missing references)
		//IL_047b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Expected O, but got Unknown
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0601: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_065c: Unknown result type (might be due to invalid IL or missing references)
		//IL_066b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0620: Unknown result type (might be due to invalid IL or missing references)
		//IL_0636: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_068a: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0537: Unknown result type (might be due to invalid IL or missing references)
		//IL_0546: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0511: Unknown result type (might be due to invalid IL or missing references)
		//IL_057d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0593: Unknown result type (might be due to invalid IL or missing references)
		GUI.skin.label.alignment = (TextAnchor)0;
		GUI.color = Color.white;
		GetMenuUIScale();
		bool enabled = GUI.enabled;
		Vector2 val = GUI.skin.label.CalcSize(new GUIContent(plr.playername));
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		if (plr.is_host)
		{
			GUILayout.Label((Texture)(object)KrokoshaCoopModAssets.icon_crown.texture, (GUILayoutOption[])(object)new GUILayoutOption[2]
			{
				GUILayout.Width(val.y),
				GUILayout.Height(val.y)
			});
		}
		foreach (Texture2D additional_profile_tag_icon in plr.additional_profile_tag_icons)
		{
			GUILayout.Label((Texture)(object)additional_profile_tag_icon, (GUILayoutOption[])(object)new GUILayoutOption[2]
			{
				GUILayout.Width(val.y),
				GUILayout.Height(val.y)
			});
		}
		Texture2D profilepic_any_largetosmall = plr.profilepic_any_largetosmall;
		if ((Object)(object)profilepic_any_largetosmall != (Object)null)
		{
			GUILayout.Label((Texture)(object)profilepic_any_largetosmall, (GUILayoutOption[])(object)new GUILayoutOption[2]
			{
				GUILayout.Width(val.y),
				GUILayout.Height(val.y)
			});
		}
		GUI.color = plr.plrcolor;
		GUILayout.Label(plr.playername, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(Mathf.Min(val.x, ((Rect)(ref r)).width * 0.5f)) });
		GUI.color = Color.white;
		GUILayout.FlexibleSpace();
		if (!plr.is_local && Con.CanExecuteAdminCommands())
		{
			if (GUI.enabled)
			{
				GUI.enabled = ____GUI___ConnectionSecondPanel_ContentRender_RenderDetailedPlayerList_ban_kick_safety;
			}
			if (GUILayout.Button(Lang.Get("mmct_plr_kick", false), Array.Empty<GUILayoutOption>()))
			{
				Util.DelayCallLambda(0.001f, (Action)delegate
				{
					Con.ExecuteCommandAdminNoLog("kick id:" + plr.clientId.ToString());
				});
			}
			if (GUILayout.Button(Lang.Get("mmct_plr_ban", false), Array.Empty<GUILayoutOption>()))
			{
				Util.DelayCallLambda(0.001f, (Action)delegate
				{
					Con.ExecuteCommandAdminNoLog("ban id:" + plr.clientId.ToString());
				});
			}
			if (____GUI___ConnectionSecondPanel_ContentRender_RenderDetailedPlayerList_showLitenetLibBanButton && GUILayout.Button(Lang.Get("mmct_plr_ipban", false), Array.Empty<GUILayoutOption>()))
			{
				Util.DelayCallLambda(0.001f, (Action)delegate
				{
					Con.ExecuteCommandAdminNoLog("ipban id:" + plr.clientId.ToString());
				});
			}
			GUI.enabled = enabled;
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		if (____GUI___ConnectionSecondPanel_ContentRender_RenderDetailedPlayerList_gaben && GUILayout.Button(Lang.Get("mmct_plr_steamprofile", false), Array.Empty<GUILayoutOption>()))
		{
			SteamFriends.ActivateGameOverlayToUser("steamid", (CSteamID)plr.steam_id);
		}
		GUILayout.Space(5f * GetMenuUIScale());
		if (Con.CanExecuteAdminCommands())
		{
			if ((Object)(object)_COLOR_EDITING == (Object)(object)plr)
			{
				Vector2 val2 = GUI.skin.label.CalcSize(new GUIContent("#FFFFFF"));
				Color24 color = plr.plrcolor;
				if (Color24.TryParseHex(_COLOR_EDITING_STR, out var color2))
				{
					color = color2;
				}
				GUI.color = color;
				_COLOR_EDITING_STR = GUILayout.TextField(_COLOR_EDITING_STR, 7, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(val2.x) });
				if (_COLOR_EDITING_STR.Length > 6 && !StringUtility.StartsWith(_COLOR_EDITING_STR, '#'))
				{
					_COLOR_EDITING_STR = _COLOR_EDITING_STR.Substring(0, 6);
				}
				GUI.color = Color.white;
				if (GUILayout.Button(Lang.Get("mm_apply", false), Array.Empty<GUILayoutOption>()))
				{
					if (plr.plrcolor != color)
					{
						Con.ExecuteCommandAdminNoLog($"setplrcolor id:{plr.clientId} {color.ToHex()}");
					}
					_COLOR_EDITING = null;
				}
			}
			else
			{
				GUI.color = plr.plrcolor;
				if (GUILayout.Button(Lang.Get("mmct_plr_setcolor", false), Array.Empty<GUILayoutOption>()))
				{
					_COLOR_EDITING_STR = plr.plrcolor.ToHex(with_hashtag: false);
					_COLOR_EDITING = plr;
				}
				GUI.color = Color.white;
			}
		}
		GUILayout.FlexibleSpace();
		GUIContent val3 = new GUIContent(Lang.Get("mmct_plr_mutelabel", false));
		Vector2 val4 = GUI.skin.label.CalcSize(val3);
		if (!plr.is_local && Con.CanExecuteAdminCommands())
		{
			GUILayout.Label(val3, Array.Empty<GUILayoutOption>());
			if (GUILayout.Button((Texture)(object)KrokoshaCoopModAssets.voicechaticon.texture, (GUILayoutOption[])(object)new GUILayoutOption[2]
			{
				GUILayout.Width(val4.y),
				GUILayout.Height(val4.y)
			}))
			{
				Util.DelayCallLambda(0.001f, (Action)delegate
				{
					Con.ExecuteCommandAdminNoLog("servermute vc id:" + plr.clientId.ToString());
				});
			}
			if (plr.server_mute_vc)
			{
				GUI.DrawTexture(GUILayoutUtility.GetLastRect(), (Texture)(object)KrokoshaCoopModAssets.cross.texture, (ScaleMode)2, true, 1f, Color.red, 0f, 0f);
			}
			if (GUILayout.Button((Texture)(object)KrokoshaCoopModAssets.speechbubbleicon3.texture, (GUILayoutOption[])(object)new GUILayoutOption[2]
			{
				GUILayout.Width(val4.y),
				GUILayout.Height(val4.y)
			}))
			{
				Util.DelayCallLambda(0.001f, (Action)delegate
				{
					Con.ExecuteCommandAdminNoLog("servermute tc id:" + plr.clientId.ToString());
				});
			}
			if (plr.server_mute_tc)
			{
				GUI.DrawTexture(GUILayoutUtility.GetLastRect(), (Texture)(object)KrokoshaCoopModAssets.cross.texture, (ScaleMode)2, true, 1f, Color.red, 0f, 0f);
			}
			GUI.enabled = enabled;
		}
		else if (plr.server_mute_vc || plr.server_mute_tc)
		{
			GUILayout.FlexibleSpace();
			GUILayout.Label(val3, Array.Empty<GUILayoutOption>());
			GUILayout.Label((Texture)(object)KrokoshaCoopModAssets.voicechaticon.texture, (GUILayoutOption[])(object)new GUILayoutOption[2]
			{
				GUILayout.Width(val4.y),
				GUILayout.Height(val4.y)
			});
			if (plr.server_mute_vc)
			{
				GUI.DrawTexture(GUILayoutUtility.GetLastRect(), (Texture)(object)KrokoshaCoopModAssets.cross.texture, (ScaleMode)2, true, 1f, Color.red, 0f, 0f);
			}
			GUILayout.Label((Texture)(object)KrokoshaCoopModAssets.speechbubbleicon3.texture, (GUILayoutOption[])(object)new GUILayoutOption[2]
			{
				GUILayout.Width(val4.y),
				GUILayout.Height(val4.y)
			});
			if (plr.server_mute_tc)
			{
				GUI.DrawTexture(GUILayoutUtility.GetLastRect(), (Texture)(object)KrokoshaCoopModAssets.cross.texture, (ScaleMode)2, true, 1f, Color.red, 0f, 0f);
			}
			GUILayout.FlexibleSpace();
		}
		if (!plr.is_local && (Object)(object)plr.vc_output != (Object)null)
		{
			GUILayout.Space(5f * GetMenuUIScale());
			GUI.skin.label.alignment = (TextAnchor)2;
			GUILayout.Label(Lang.Get("mmct_plr_vc_vol", false), Array.Empty<GUILayoutOption>());
			plr.vc_output.custom_volume_set = GUILayout.HorizontalSlider(plr.vc_output.custom_volume_set, 0f, 2f, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.MaxWidth(((Rect)(ref r)).width * 0.4f) });
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		GUI.skin.label.alignment = (TextAnchor)0;
		GUILayout.Label("ID: " + plr.clientId.ToString(), Array.Empty<GUILayoutOption>());
		GUILayout.Space(2f * GetMenuUIScale());
		GUILayout.Label(Lang.Get("mmct_plr_ping", false) + " " + plr.ping_as_ms, Array.Empty<GUILayoutOption>());
		if (plr.server_plrstate != null)
		{
			GUILayout.Label($" TPS:{plr.server_plrstate.tps,3} FPS:{plr.server_plrstate.fps,3}", Array.Empty<GUILayoutOption>());
		}
		GUILayout.EndHorizontal();
	}

	private static void _GUI_RenderDirectConnectMenu(Rect r)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		GUI.color = Color.white;
		float num = (float)UIBullshit.uiBlockSmallBorderSize * 0.5f;
		if (Net.TRANSPORT != null)
		{
			____GUI___ConnectionSecondPanel_ContentRender_RenderDetailedPlayerList_showLitenetLibBanButton = Net.TRANSPORT is TransportLiteNetLib;
			____GUI___ConnectionSecondPanel_ContentRender_RenderDetailedPlayerList_gaben = Net.TRANSPORT is TransportSteamworks;
			if (____GUI___ConnectionSecondPanel_ContentRender_RenderDetailedPlayerList_gaben)
			{
				_USE_STEAM_MENU = true;
			}
			if (____GUI___ConnectionSecondPanel_ContentRender_RenderDetailedPlayerList_showLitenetLibBanButton)
			{
				_USE_STEAM_MENU = false;
			}
		}
		else
		{
			____GUI___ConnectionSecondPanel_ContentRender_RenderDetailedPlayerList_showLitenetLibBanButton = false;
			____GUI___ConnectionSecondPanel_ContentRender_RenderDetailedPlayerList_gaben = false;
		}
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(r);
		((Rect)(ref rect)).xMax = ((Rect)(ref rect)).center.x - num;
		((Rect)(ref rect)).x = 0f;
		((Rect)(ref rect)).y = 0f;
		UIBullshit._GUI_9SlicePanel(in rect, 1f, 0.5f, check_overlap: false, UIBullshit.unscaled_uiBlockNano);
		rect = rect.ShrinkBorder((float)UIBullshit.uiBlockNanoBorderSize);
		GUILayout.BeginArea(rect);
		_GUI__ConnectionSecondPanel(rect);
		GUILayout.EndArea();
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(r);
		((Rect)(ref rect2)).x = 0f;
		((Rect)(ref rect2)).y = 0f;
		((Rect)(ref rect2)).xMin = ((Rect)(ref rect2)).center.x + num;
		UIBullshit._GUI_9SlicePanel(in rect2, 1f, 0.5f, check_overlap: false, UIBullshit.unscaled_uiBlockNano);
		rect2 = rect2.ShrinkBorder((float)UIBullshit.uiBlockNanoBorderSize);
		GUILayout.BeginArea(rect2);
		_GUI__DirectConnect(rect2);
		GUILayout.EndArea();
	}

	private static void _GUI_RenderSettingsMenu(Rect r)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(r);
		((Rect)(ref rect)).xMax = Mathf.LerpUnclamped(((Rect)(ref rect)).xMax, ((Rect)(ref rect)).xMin, 0.7f);
		((Rect)(ref rect)).x = 0f;
		((Rect)(ref rect)).y = 0f;
		UIBullshit._GUI_9SlicePanel(in rect, 1f, 0.5f, check_overlap: false, UIBullshit.unscaled_uiBlockNano);
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		float setting_submenu_buttons_rects = ((Rect)(ref rect)).width - (float)(UIBullshit.uiBlockNanoBorderSize * 2);
		GUILayout.BeginVertical((GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(((Rect)(ref rect)).width) });
		GUILayout.Space((float)UIBullshit.uiBlockNanoBorderSize);
		Action<SettingsTab> action = delegate(SettingsTab tab)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			if (cur_settings_tab == tab)
			{
				GUI.backgroundColor = Color.white;
			}
			else
			{
				GUI.backgroundColor = Color.white * 0.8f;
			}
			if (GUILayout.Button(Lang.Get(in tab.name, false), (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(setting_submenu_buttons_rects) }))
			{
				cur_settings_tab = tab;
				if (cur_settings_tab.on_open != null)
				{
					cur_settings_tab.on_open();
				}
				OnMenuTabChange();
				Util.PlayUISound((UISoundType)1);
			}
		};
		foreach (SettingsTab aLL_SETTING in ALL_SETTINGS)
		{
			action(aLL_SETTING);
		}
		if ((Net.running ? Con.CanCheat() : KrokoshaScavMultiplayer.rules.sv_cheats) || log.verbose)
		{
			action(_debug_setting_tab);
		}
		action(_bugreport_setting_tab);
		GUI.backgroundColor = Color.white;
		GUILayout.EndVertical();
		Rect r2 = default(Rect);
		((Rect)(ref r2))._002Ector(r);
		((Rect)(ref r2)).width = ((Rect)(ref r)).width - ((Rect)(ref rect)).width - (float)UIBullshit.uiBlockSmallBorderSize * 4.05f;
		_SETTINGSMENUSCROLL = GUILayout.BeginScrollView(_SETTINGSMENUSCROLL, Array.Empty<GUILayoutOption>());
		GUILayout.BeginVertical((GUILayoutOption[])(object)new GUILayoutOption[3]
		{
			GUILayout.MaxWidth(((Rect)(ref r2)).width),
			GUILayout.ExpandWidth(false),
			GUILayout.Width(((Rect)(ref r2)).width)
		});
		int fontSize = GUI.skin.label.fontSize;
		TextAnchor alignment = GUI.skin.label.alignment;
		GUI.skin.label.wordWrap = false;
		if (cur_settings_tab != null)
		{
			try
			{
				UIBullshit._GUI_SetButtonSkinTexture(GUI.skin.button, small: false);
				GUI.skin.label.alignment = (TextAnchor)1;
				UIBullshit._GUI_BiggerLabel(Lang.Get(in cur_settings_tab.name, false), 1.4f);
				GUI.skin.label.alignment = (TextAnchor)0;
				if (cur_settings_tab.settings_origin != null)
				{
					__GUI____RenderFieldsWithThatAttribute(GetSettingsFromStaticClass(cur_settings_tab.settings_origin), in r2);
				}
				if (cur_settings_tab.gui_func != null)
				{
					cur_settings_tab.gui_func(r2);
				}
			}
			catch (Exception ex)
			{
				log.error("SETTINGS RENDER ERROR: " + ex.ToString());
			}
			UIBullshit._GUI_SetButtonSkinTexture(GUI.skin.button);
		}
		else
		{
			GUILayout.Label("Empty.", Array.Empty<GUILayoutOption>());
		}
		GUI.skin.label.fontSize = fontSize;
		GUI.skin.label.alignment = alignment;
		GUILayout.EndVertical();
		GUILayout.EndScrollView();
		GUILayout.EndHorizontal();
		GUILayout.Space(10f * GetMenuUIScale());
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		GUILayout.FlexibleSpace();
		if (GUILayout.Button(Lang.Get("mmst_resettodefault", false), Array.Empty<GUILayoutOption>()))
		{
			if (cur_settings_tab?.settings_origin != null)
			{
				foreach (FieldInfo item in GetSettingsFromStaticClass(cur_settings_tab.settings_origin))
				{
					SettingDeclarerThingyBaseAttribute attribute = AttributeUtility.GetAttribute<SettingDeclarerThingyBaseAttribute>((MemberInfo)item, true);
					attribute.Save(item, attribute.default_value);
				}
			}
			if (cur_settings_tab?.on_reset != null)
			{
				cur_settings_tab.on_reset();
			}
			Util.PlayUISound((UISoundType)1);
		}
		DoTooltipOnLastGUIRect("mmst_resettodefault_desc");
		if (GUILayout.Button(Lang.Get("mmst_apply", false), Array.Empty<GUILayoutOption>()))
		{
			SettingsJsonThing.json.Save();
			Util.PlayUISound((UISoundType)1);
		}
		GUILayout.EndHorizontal();
	}

	public static void OnMenuTabChange()
	{
		GUILayout_DropdownMenu.CloseAll();
		Voicechat.ENABLE_LOCAL_LOOPBACK = false;
		ResetSettingsVOIPVisualiser();
	}

	public static void ResetSettingsVOIPVisualiser()
	{
		if ((Object)(object)UIBullshit.main.egg != (Object)null)
		{
			Object.Destroy((Object)(object)((Component)UIBullshit.main.egg).gameObject);
			UIBullshit.main.egg = null;
		}
	}

	private static void _GUI___SettingsVOIP(Rect r)
	{
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Expected O, but got Unknown
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Invalid comparison between Unknown and I4
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Expected O, but got Unknown
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		GUI.skin.label.alignment = (TextAnchor)0;
		float num = ((Rect)(ref r)).width * ASJOUFMIGHDSYDSGFMKFSDKYGJUFSYGJFSDGKVYJSFDKJGYFSDGKVJHBFJKVFDJVMBYFJDYG;
		GUI.skin.label.alignment = (TextAnchor)5;
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		GUILayout.Label(Lang.Get("mmst_mike", false), (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(num) });
		if (_VOIP_SETTINGS_DROPDOWNTEST)
		{
			GUILayout.Label(Lang.Get("mmst_select_mike", false), (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(num) });
			if (GUILayout.Button(Lang.Get("mmst_mike_osdefault", false), Array.Empty<GUILayoutOption>()))
			{
				_VOIP_SETTINGS_DROPDOWNTEST = false;
				Voicechat.SetMicrophone("NO_MICROPHONE_SELECTED");
			}
		}
		else if (GUILayout.Button(Voicechat.GetMic() ?? Lang.Get("mmst_mike_osdefault", false), Array.Empty<GUILayoutOption>()))
		{
			_VOIP_SETTINGS_DROPDOWNTEST = true;
		}
		GUILayout.EndHorizontal();
		try
		{
			if (_VOIP_SETTINGS_DROPDOWNTEST)
			{
				for (int i = 0; i < Microphone.devices.Length; i++)
				{
					string text = Microphone.devices[i];
					GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
					GUILayout.Space(num);
					if (GUILayout.Button(text, Array.Empty<GUILayoutOption>()))
					{
						_VOIP_SETTINGS_DROPDOWNTEST = false;
						Voicechat.SetMicrophone(text);
					}
					GUILayout.EndHorizontal();
				}
			}
		}
		catch (Exception ex)
		{
			log.error("_VOIP_SETTINGS_DROPDOWNTEST\n" + ex.ToString());
		}
		if (Voicechat.VoiceChatIsEnabledLocallyOnly())
		{
			UIBullshit._GUI_SetButtonSkinTexture(GUI.skin.button);
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			try
			{
				if (GUILayout.Button(Voicechat.ENABLE_LOCAL_LOOPBACK ? Lang.Get("mmst_mike_testing", false) : Lang.Get("mmst_mike_test", false), Array.Empty<GUILayoutOption>()))
				{
					Voicechat.ENABLE_LOCAL_LOOPBACK = !Voicechat.ENABLE_LOCAL_LOOPBACK;
				}
				Rect rect = GUILayoutUtility.GetRect(new GUIContent("uououo typ shi"), GUI.skin.button, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.ExpandWidth(true) });
				if ((int)Event.current.type == 7)
				{
					Vector2 val = ((Rect)(ref rect)).size / UIBullshit.uiScale;
					Vector2Int val2 = default(Vector2Int);
					((Vector2Int)(ref val2))._002Ector((int)(val.x / val.y * MicVolumePreviewPixHeight), (int)MicVolumePreviewPixHeight);
					if ((Object)(object)UIBullshit.main.egg == (Object)null || UIBullshit.main.egg.textureSize != val2)
					{
						ResetSettingsVOIPVisualiser();
						GameObject val3 = new GameObject("hello vro");
						((Behaviour)val3.AddComponent<RawImage>()).enabled = false;
						UIBullshit.main.egg = val3.AddComponent<ECGVisualizer>();
						UIBullshit.main.egg.textureSize = val2;
						UIBullshit.main.egg.followBody = false;
					}
				}
				if ((Object)(object)UIBullshit.main.egg != (Object)null && (Object)(object)UIBullshit.main.egg.tex != (Object)null)
				{
					if (UIBullshit.main.egg.writeX != last_writeX)
					{
						last_writeX = UIBullshit.main.egg.writeX;
						UIBullshit.main.egg.writeHeight = 0f;
						writeX_flipflop = !writeX_flipflop;
					}
					GUI.DrawTexture(rect, (Texture)(object)UIBullshit.main.egg.tex);
				}
			}
			catch (Exception ex2)
			{
				log.error("_VOIP_SETTINGS_LOOPBACKRENDER\n" + ex2.ToString());
			}
			GUILayout.EndHorizontal();
		}
		else
		{
			GUILayout.Label(Lang.Get("mmst_mike_local_off", false), Array.Empty<GUILayoutOption>());
		}
		if (Voicechat.IsMicModePushToTalk() || Voicechat.IsMicModeToggleToTalk())
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.FlexibleSpace();
			if (GUILayout.Button(Lang.Get("mmst_vcbutton", false) + KeyBinds.GetBindName("krokosha_coop_voicechat").ToString(), Array.Empty<GUILayoutOption>()))
			{
				SetOpen(open_or_nah: false);
				Util.OpenSettingsPanel(open_or_nah: true, (SettingCategory)3);
			}
			DoTooltipOnLastGUIRect("mmst_gotokeybind");
			GUILayout.EndHorizontal();
		}
		if (!KrokoshaScavMultiplayer.rules.VoicechatEnabled)
		{
			GUILayout.Label(Lang.Get("mmst_voip_rule_disabled", false) + "    ", Array.Empty<GUILayoutOption>());
		}
	}

	private static void _GUI___SettingsGUI(Rect r)
	{
		UIBullshit._GUI_SetButtonSkinTexture(GUI.skin.button);
		GUI.skin.label.alignment = (TextAnchor)2;
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		GUILayout.FlexibleSpace();
		if (GUILayout.Button(Lang.Get("mmst_forcereloaduitex", false), Array.Empty<GUILayoutOption>()))
		{
			UIBullshit.ResizeGUITextures();
			Util.PlayUISound((UISoundType)1);
		}
		GUILayout.EndHorizontal();
	}

	private static void _GUI___SettingsDebug(Rect r)
	{
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		foreach (KeyValuePair<knetid, SyncInfo> item in NetObjectRegistry.NetIdToSyncInfoDict)
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label(item.Value.ToString() ?? "", Array.Empty<GUILayoutOption>());
			GUILayout.FlexibleSpace();
			if (Con.CanCheat() && (Object)(object)item.Value.go != (Object)null)
			{
				if (GUILayout.Button("Destroy", Array.Empty<GUILayoutOption>()))
				{
					Object.Destroy((Object)(object)item.Value.go);
				}
				if (GUILayout.Button($"TP: {item.Value.position}", Array.Empty<GUILayoutOption>()))
				{
					((Component)PlayerCamera.main.body).transform.position = Vector2.op_Implicit(item.Value.position);
				}
			}
			GUILayout.EndHorizontal();
		}
	}

	private static void _GUI___SettingsChat(Rect r)
	{
		Chat.ShowChat();
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		GUILayout.FlexibleSpace();
		if (GUILayout.Button(Lang.Get("mmst_chatbutton", false) + KeyBinds.GetBindName("krokosha_coop_chat").ToString(), Array.Empty<GUILayoutOption>()))
		{
			SetOpen(open_or_nah: false);
			Util.OpenSettingsPanel(open_or_nah: true, (SettingCategory)3);
		}
		DoTooltipOnLastGUIRect("mmst_gotokeybind");
		GUILayout.EndHorizontal();
	}

	private static void DrawUpdateButton()
	{
		string status = AutoUpdater.Status;
		GUI.skin.label.alignment = (TextAnchor)0;
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		if (_CHECKFORUPDATEBUTTON_IS_RESTART)
		{
			if (GUILayout.Button(Lang.Get("mmst_button_restart", false), Array.Empty<GUILayoutOption>()))
			{
				Util.DelayCallLambda(0.01f, (Action)delegate
				{
					Plugin.RestartGame();
				});
			}
			DoTooltipOnLastGUIRect("mmst_button_restart_desc");
		}
		else
		{
			if (GUILayout.Button(Lang.Get("mmst_button_checkupdate", false), Array.Empty<GUILayoutOption>()))
			{
				if (!_CHECKFORUPDATEBUTTON_OnStageSuccess_isBound)
				{
					_CHECKFORUPDATEBUTTON_OnStageSuccess_isBound = true;
					AutoUpdater.OnStageSuccess = (Action)Delegate.Combine(AutoUpdater.OnStageSuccess, (Action)delegate
					{
						log.l("Update staging succesful.");
						if (Plugin.IsExeSameAsInSteam())
						{
							_CHECKFORUPDATEBUTTON_IS_RESTART = true;
						}
					});
				}
				AutoUpdater.StartChecking();
				Util.PlayUISound((UISoundType)1);
			}
			DoTooltipOnLastGUIRect("mmst_button_checkupdate_desc");
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		bool wordWrap = GUI.skin.label.wordWrap;
		GUI.skin.label.wordWrap = true;
		GUILayout.Label(status, Array.Empty<GUILayoutOption>());
		GUI.skin.label.wordWrap = wordWrap;
		GUILayout.EndHorizontal();
	}

	private static void _GUI___SettingsGeneral(Rect r)
	{
		bool enabled = GUI.enabled;
		UIBullshit._GUI_SetButtonSkinTexture(GUI.skin.button);
		GUILayout.Space(20f * GetMenuUIScale());
		try
		{
			DrawUpdateButton();
		}
		catch (Exception)
		{
		}
		GUILayout.Space(20f * GetMenuUIScale());
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		bool num = (GUI.enabled = !Net.running && !Util.IsInWorld());
		if (GUILayout.Button(Lang.Get("mmst_button_deactivate_mp", false), Array.Empty<GUILayoutOption>()))
		{
			PlayerPrefsExtended.SetBool("KrokoshaCasualtiesMP_FORCE_DISABLE_MP_MOD", value: true);
			PlayerPrefs.Save();
			Plugin.RestartGame();
		}
		GUI.enabled = enabled;
		DoTooltipOnLastGUIRect(num ? "mmst_button_deactivate_mp_desc" : "mmst_button_deactivate_mp_desc_disabled");
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}

	private static void __GUI____RenderFieldsWithThatAttribute(List<FieldInfo> flds, in Rect r)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = r;
		float num = ((Rect)(ref rect)).width * ASJOUFMIGHDSYDSGFMKFSDKYGJUFSYGJFSDGKVYJSFDKJGYFSDGKVJHBFJKVFDJVMBYFJDYG;
		foreach (FieldInfo fld in flds)
		{
			if (!AttributeUtility.HasAttribute((MemberInfo)fld, typeof(SettingDeclarerThingyBaseAttribute), true))
			{
				continue;
			}
			SettingDeclarerThingyBaseAttribute attribute = AttributeUtility.GetAttribute<SettingDeclarerThingyBaseAttribute>((MemberInfo)fld, true);
			string key = attribute.n;
			if (key == null)
			{
				key = "setting_" + fld.DeclaringType?.ToString() + "_" + fld.Name;
			}
			GUI.skin.label.alignment = (TextAnchor)5;
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label(Lang.Get(in key, false), (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(num) });
			rect = GUIUtility.GUIToScreenRect(GUILayoutUtility.GetLastRect());
			if (UIBullshit.CheckCursorOverlap(in rect) && Lang.TryGet(key + "_desc", out var text))
			{
				UIBullshit._GUI_SetTooltip("", in text);
			}
			if (attribute is SettingDeclarerThingyFloatAttribute settingDeclarerThingyFloatAttribute)
			{
				float num2 = (float)fld.GetValue(null);
				float num3 = GUILayout.HorizontalSlider(num2, Mathf.Min(num2, settingDeclarerThingyFloatAttribute.mi), Mathf.Max(num2, settingDeclarerThingyFloatAttribute.ma), Array.Empty<GUILayoutOption>());
				GUI.skin.label.alignment = (TextAnchor)4;
				string text2 = num3.ToString("0.0") + settingDeclarerThingyFloatAttribute.postfix;
				GUILayoutOption[] array = new GUILayoutOption[1];
				rect = r;
				array[0] = GUILayout.Width(((Rect)(ref rect)).width * 0.1f);
				GUILayout.Label(text2, (GUILayoutOption[])(object)array);
				if (num2 != num3)
				{
					settingDeclarerThingyFloatAttribute.Save(fld, num3);
				}
			}
			else if (attribute is SettingDeclarerThingyBoolAttribute settingDeclarerThingyBoolAttribute)
			{
				bool num4 = (bool)fld.GetValue(null);
				bool flag = GUILayout.Toggle(num4, "", Array.Empty<GUILayoutOption>());
				if (num4 != flag)
				{
					settingDeclarerThingyBoolAttribute.Save(fld, flag);
				}
			}
			else if (attribute is SettingDeclarerThingyChoiceAttribute settingDeclarerThingyChoiceAttribute)
			{
				int num5 = ConversionUtility.ConvertTo<int>(fld.GetValue(null));
				string[] options = new List<string>(settingDeclarerThingyChoiceAttribute.choices).Select((string x) => Lang.Get(in x, false)).ToArray();
				int num6 = GUILayout_DropdownMenu.Dropdown(num5, options);
				if (num5 != num6)
				{
					settingDeclarerThingyChoiceAttribute.Save(fld, num6);
				}
			}
			else
			{
				GUILayout.Toggle(false, "ERROR UNKNOWN TYPE", Array.Empty<GUILayoutOption>());
			}
			GUILayout.EndHorizontal();
		}
	}

	public static List<FieldInfo> GetSettingsFromStaticClass(Type type)
	{
		if (!_known_settings.TryGetValue(type, out var value))
		{
			value = type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).ToList();
			value.RemoveAll((FieldInfo f) => !AttributeUtility.HasAttribute((MemberInfo)f, typeof(SettingDeclarerThingyBaseAttribute), true));
			foreach (FieldInfo item in value)
			{
				SettingDeclarerThingyBaseAttribute attribute = AttributeUtility.GetAttribute<SettingDeclarerThingyBaseAttribute>((MemberInfo)item, true);
				attribute.default_value = item.GetValue(null);
				attribute.Get(item);
			}
			if (value.Count > 0)
			{
				_known_settings[type] = value;
			}
		}
		return value;
	}

	private static void _GUI_RenderAboutMenu(Rect r)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		_ABOUTMENUSCROLL = GUILayout.BeginScrollView(_ABOUTMENUSCROLL, Array.Empty<GUILayoutOption>());
		int fontSize = GUI.skin.label.fontSize;
		TextAnchor alignment = GUI.skin.label.alignment;
		GUI.skin.label.wordWrap = false;
		GUILayout.Space(10f * GetMenuUIScale());
		_GUI__AboutMenuLinkButton(Lang.Get("mmta_og_download", false), "https://www.nexusmods.com/scavprototype/mods/67", in r);
		_GUI__AboutMenuLinkButton(Lang.Get("mmta_discord", false), "https://discord.gg/7K6J6bhV8b", in r);
		GUI.skin.label.alignment = (TextAnchor)1;
		GUILayout.Label("", Array.Empty<GUILayoutOption>());
		UIBullshit._GUI_BiggerLabel(Lang.Get("mmta_credits", false), 1.2f);
		try
		{
			_GUI__ABOUTMENU_FULL_CREDITS(r);
		}
		catch (Exception ex)
		{
			log.error("RENDER: \n" + ex.ToString());
		}
		GUILayout.Label("", Array.Empty<GUILayoutOption>());
		UIBullshit._GUI_BiggerLabel(Lang.Get("mmta_libraries", false), 1.2f);
		_GUI__AboutMenuLinkButton("LiteNetLib", "https://github.com/RevenantX/LiteNetLib", in r);
		_GUI__AboutMenuLinkButton("Steamworks.NET", "https://github.com/rlabrecque/Steamworks.NET", in r);
		_GUI__AboutMenuLinkButton("OpusSharp", "https://github.com/AvionBlock/OpusSharp", in r);
		_GUI__AboutMenuLinkButton("BepInEx", "https://github.com/BepInEx/BepInEx", in r);
		GUI.skin.label.fontSize = fontSize;
		GUI.skin.label.alignment = alignment;
		GUILayout.Label("", Array.Empty<GUILayoutOption>());
		GUILayout.EndScrollView();
	}

	private static void _GUI__AboutMenuLinkButton(in string name, in string link, in Rect r)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		GUI.skin.label.alignment = (TextAnchor)5;
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		string obj = name;
		GUILayoutOption[] array = new GUILayoutOption[1];
		Rect rect = r;
		array[0] = GUILayout.Width(((Rect)(ref rect)).width * 0.5f);
		GUILayout.Label(obj, (GUILayoutOption[])(object)array);
		if (GUILayout.Button(Lang.Get("mmta_link_open", false), Array.Empty<GUILayoutOption>()))
		{
			Application.OpenURL(link);
			Util.PlayUISound((UISoundType)1);
		}
		rect = GUIUtility.GUIToScreenRect(GUILayoutUtility.GetLastRect());
		if (UIBullshit.CheckCursorOverlap(in rect))
		{
			UIBullshit._GUI_SetTooltip("", in link);
		}
		if (GUILayout.Button(Lang.Get("mmta_link_copy", false), Array.Empty<GUILayoutOption>()))
		{
			GUIUtility.systemCopyBuffer = link;
			AHAHUJHAKFSDFJFJKFKJAFKJAK = link;
			Util.PlayUISound((UISoundType)1);
		}
		if (AHAHUJHAKFSDFJFJKFKJAFKJAK == link)
		{
			GUILayout.Label(Lang.Get("mmta_link_copied", false), Array.Empty<GUILayoutOption>());
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
	}

	private static void _GUI__ABOUTMENU_FULL_CREDITS(Rect r)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		gaaaaaaaaaaaaa[0].Item1 = KrokoshaCoopModAssets.aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa.texture;
		(Texture2D, string)[] array = gaaaaaaaaaaaaa;
		for (int i = 0; i < array.Length; i++)
		{
			(Texture2D, string) tuple = array[i];
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.FlexibleSpace();
			Vector2 val = GUI.skin.label.CalcSize(new GUIContent(tuple.Item2));
			if ((Object)(object)tuple.Item1 != (Object)null)
			{
				GUILayout.Label((Texture)(object)tuple.Item1, (GUILayoutOption[])(object)new GUILayoutOption[2]
				{
					GUILayout.Width(val.y),
					GUILayout.Height(val.y)
				});
				GUILayout.Space(val.y * 0.1f);
			}
			GUILayout.Label(tuple.Item2, Array.Empty<GUILayoutOption>());
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}
	}

	private static bool _GUI_SteamMenu()
	{
		return true;
	}
}
