using System;
using System.Collections.Generic;
using KrokoshaCasualtiesUtils;
using LiteNetLib;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace KrokoshaCasualtiesMP;

public class UIBullshit : KrokoshaScavSingleton
{
	internal ECGVisualizer egg;

	public static string net_debug_msg = "";

	private static Vector2 last_screensize = Vector2.zero;

	private static Dictionary<Texture2D, Dictionary<Vector2, Texture2D>> cached_modified_textures = new Dictionary<Texture2D, Dictionary<Vector2, Texture2D>>();

	private static Dictionary<Texture2D, Texture2D> cached_scaled_textures = new Dictionary<Texture2D, Texture2D>();

	private static bool _recalculate_skin = false;

	[SettingDeclarerThingyBool("setting_ui_discordrpc_allowjoin")]
	public static bool DISCORD_ALLOWJOINBUTTON = true;

	[SettingDeclarerThingyBool("setting_ui_discordrpc_onlyask")]
	public static bool DISCORD_ONLYASKTOJOINBUTTON = false;

	[SettingDeclarerThingyBool("setting_ui_usegamefont")]
	public static bool USE_GAME_FONT = true;

	public static float CUSTOM_UI_SCALE_FOR_UIBULLSHIT = 1f;

	public static float DEFAULT_RETROGAME_FONTSIZE = 20f;

	public static float THING_FOR_UI_SCALE = 3f;

	public static float uiScale = 1f;

	public static float uiScaleMultiplier = 1f;

	public static float uiScaleForMostUI = 1f;

	public static int uiBlockSmallBorderSize = 3;

	public static int uiBlockNanoBorderSize = 2;

	private static int TOTAL_INTERACTIVE_UI = 0;

	private static int TOTAL_FOCUSED_INTERACTIVE_UI = 0;

	public static bool IS_CURSOR_OVERLAPPING = false;

	private static bool _gui_tooltip = false;

	private static string _gui_tooltip_name = "";

	private static string _gui_tooltip_desc = "";

	public static float dev2222222222348333343gfddgfgdfdfgfdgfdg4398 = 0f;

	public static bool _reset_gui_textures_next_frame = false;

	public static bool dev348432843289723498723497824398 = true;

	public static bool dev34843gfddgfgdfdfgfdgfdg4398 = false;

	private static float TOOLTIP_SCALE = 0.78f;

	internal static int _og_fontsize = 13;

	private static GUISkin new_skin;

	private static GUISkin _og_skin;

	private static GUISkin _og_skin_cloned;

	private static float _og_verticalscrollbar_fixedWidth;

	private static float _og_gorizontalscrollbar_fixedHeight;

	private static Texture2D _b_n_b;

	private static Texture2D _b_a_b;

	private static Texture2D _b_f_b;

	private static Texture2D _b_h_b;

	private static Texture2D _box_bg;

	private static Color _box_txcolor;

	private static Font _b_ogfont;

	private static RectOffset _og_border = new RectOffset(6, 6, 6, 4);

	public static UIBullshit main { get; private set; }

	public static Canvas canvas
	{
		get
		{
			if (Util.IsInMainMenu())
			{
				return ((Component)PreRunScript.instance).GetComponent<Canvas>();
			}
			return PlayerCamera.main.mainCanvas;
		}
	}

	private static GUISkin skin => GUI.skin;

	public static Texture2D unscaled_uiBlockNano => KrokoshaMainmenuBackground.og_uiBlockNano;

	public static Texture2D unscaled_uiBlockSmall => KrokoshaMainmenuBackground.og_uiBlockSmall;

	public static Texture2D tex_toggle_normal => GetChangedTex(unscaled_uiBlockNano, 1f, 0.8f) ?? _b_n_b;

	public static Texture2D tex_button_normal_nano => GetChangedTex(unscaled_uiBlockNano, 1f, 0.8f) ?? _b_n_b;

	public static Texture2D tex_button_active_nano => GetChangedTex(unscaled_uiBlockNano, 0.4f, 0.7f) ?? _b_a_b;

	public static Texture2D tex_button_focus_nano => GetChangedTex(unscaled_uiBlockNano, 0.4f, 0.7f) ?? _b_f_b;

	public static Texture2D tex_button_hover_nano => GetChangedTex(unscaled_uiBlockNano, 0.65f, 0.7f) ?? _b_h_b;

	public static Texture2D tex_button_normal => GetChangedTex(unscaled_uiBlockSmall, 1f, 0.8f) ?? _b_n_b;

	public static Texture2D tex_button_active => GetChangedTex(unscaled_uiBlockSmall, 0.4f, 0.7f) ?? _b_a_b;

	public static Texture2D tex_button_focus => GetChangedTex(unscaled_uiBlockSmall, 0.4f, 0.7f) ?? _b_f_b;

	public static Texture2D tex_button_hover => GetChangedTex(unscaled_uiBlockSmall, 0.65f, 0.7f) ?? _b_h_b;

	private void Awake()
	{
		if ((Object)(object)main != (Object)null)
		{
			throw new Exception("man wtf is this bro");
		}
		main = this;
	}

	private void SecondUpdate()
	{
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		net_debug_msg = Net.GetDebugStatsString();
		float num = 0f;
		float num2 = 0f;
		if (NetPlayer.ClientIdToPlayerDict.Count > 0)
		{
			foreach (NetPlayer value in NetPlayer.ClientIdToPlayerDict.Values)
			{
				num += (float)(value.ping * 1000.0);
			}
			num /= (float)NetPlayer.ClientIdToPlayerDict.Count;
		}
		int num3 = 0;
		if (Net.is_server)
		{
			if (Net.TryGetSteamTransport(out var tsteam))
			{
				ulong steamID = KSteam.GetLocalUserSteamID().m_SteamID;
				foreach (KeyValuePair<ulong, TransportSteamworks.SteamConnectionData> item in tsteam.connectionMapping)
				{
					if (item.Key != steamID)
					{
						float flConnectionQualityLocal = item.Value.connectionStatus.m_flConnectionQualityLocal;
						num2 += flConnectionQualityLocal;
						num3++;
					}
				}
			}
			else if (Net.TRANSPORT is TransportLiteNetLib transportLiteNetLib)
			{
				foreach (NetPeer connectedPeer in transportLiteNetLib.netmgr.ConnectedPeerList)
				{
					float num4 = (float)connectedPeer.Statistics.PacketLossPercent * 0.01f;
					num2 += Mathf.Clamp01(1f - num4);
					num3++;
				}
			}
		}
		num2 = ((num3 <= 0) ? 1f : (num2 / (float)num3));
		ServerMain.AVERAGE_PING = num;
		if (Net.is_server)
		{
			ServerMain.AVG_CONNECTION_QUALITY = (byte)Mathf.Clamp(num2 * 100f, 0f, 100f);
			if (Net.is_client)
			{
				ServerMain.AVG_CONNECTION_QUALITY = ClientMain.MY_CONNECTION_QUALITY;
			}
			else
			{
				ClientMain.MY_CONNECTION_QUALITY = ServerMain.AVG_CONNECTION_QUALITY;
			}
		}
		else
		{
			ServerMain.AVG_CONNECTION_QUALITY = ClientMain.MY_CONNECTION_QUALITY;
		}
		Application.runInBackground = KrokoshaScavMultiplayer.network_system_is_running || !Util.IsWorldGenerated() || UIMainMenu.IsOpen();
	}

	private static Texture2D TexDarkener(Texture2D t, float m, float alpha)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		Texture2D val = new Texture2D(((Texture)t).width, ((Texture)t).height, (TextureFormat)4, ((Texture)t).mipmapCount > 1);
		((Texture)val).filterMode = (FilterMode)0;
		Color[] pixels = t.GetPixels();
		for (int i = 0; i < pixels.Length; i++)
		{
			Color val2 = pixels[i];
			float a = val2.a;
			val2 *= m;
			val2.a = a * alpha;
			pixels[i] = val2;
		}
		val.SetPixels(pixels);
		val.Apply();
		return val;
	}

	private static Texture2D TexScaler(Texture2D t, float s)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		Texture2D val = new Texture2D((int)Mathf.Ceil((float)((Texture)t).width * s), (int)Mathf.Ceil((float)((Texture)t).height * s), (TextureFormat)4, ((Texture)t).mipmapCount > 1);
		((Texture)val).filterMode = (FilterMode)0;
		float num = (float)((Texture)t).width / (float)((Texture)val).width;
		float num2 = (float)((Texture)t).height / (float)((Texture)val).height;
		Color[] pixels = t.GetPixels();
		Color[] pixels2 = val.GetPixels();
		for (int i = 0; i < ((Texture)val).width; i++)
		{
			for (int j = 0; j < ((Texture)val).height; j++)
			{
				pixels2[i + j * ((Texture)val).width] = pixels[Mathf.FloorToInt((float)i * num) + Mathf.FloorToInt((float)j * num2) * ((Texture)t).width];
			}
		}
		val.SetPixels(pixels2);
		val.Apply();
		return val;
	}

	public static Vector2 ScaleSize(in float x, in float y)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2(x, y) * uiScale;
	}

	public static Rect ScaleRect(float x, float y, float width, float height, bool check_overlap = false)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		x *= uiScale;
		y *= uiScale;
		width *= uiScale;
		height *= uiScale;
		if (check_overlap)
		{
			return ConstructorCheckCursorOverlap(in x, in y, in width, in height);
		}
		return new Rect(x, y, width, height);
	}

	public static Rect ScaleRectButNotPos(in float x, in float y, float width, float height, bool check_overlap = false)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		width *= uiScale;
		height *= uiScale;
		if (check_overlap)
		{
			return ConstructorCheckCursorOverlap(in x, in y, in width, in height);
		}
		return new Rect(x, y, width, height);
	}

	public static Vector2 GetScreenSize()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2((float)Screen.width, (float)Screen.height);
	}

	public static Vector2 GetHalfScreenSize()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		return GetScreenSize() * 0.5f;
	}

	public static Texture2D GetChangedTex(Texture2D og, float brightness, float alpha)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)og == (Object)null)
		{
			return null;
		}
		Texture2D scaledTex = GetScaledTex(og);
		Vector2 key = default(Vector2);
		((Vector2)(ref key))._002Ector(brightness, alpha);
		if (!cached_modified_textures.ContainsKey(og))
		{
			cached_modified_textures[og] = new Dictionary<Vector2, Texture2D>();
		}
		if (!cached_modified_textures[og].TryGetValue(key, out var value))
		{
			value = TexDarkener(scaledTex, brightness, alpha);
			cached_modified_textures[og][key] = value;
		}
		return value;
	}

	public static Texture2D GetScaledTex(Texture2D original)
	{
		if (!cached_scaled_textures.TryGetValue(original, out var value))
		{
			value = TexScaler(original, uiScaleForMostUI);
			cached_scaled_textures[original] = value;
		}
		return value;
	}

	public static void ResizeGUITextures()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		cached_modified_textures.Clear();
		cached_scaled_textures.Clear();
		last_screensize = Util.GetResolution();
		_recalculate_skin = true;
	}

	private void Start()
	{
		WorldgenPatches.OnWorldgenFinish += WorldgenPatches_OnWorldgenFinish;
		((MonoBehaviour)this).InvokeRepeating("SecondUpdate", 0.5f, 1f);
	}

	private void WorldgenPatches_OnWorldgenFinish()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		last_screensize = Vector2.zero;
	}

	private void Update()
	{
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)main.egg != (Object)null)
		{
			ECGVisualizer obj = main.egg;
			obj.timeToUpdate += Time.unscaledDeltaTime;
		}
		if (IS_CURSOR_OVERLAPPING)
		{
			GlobalDark.main.SetTooltip(("", ""));
		}
		try
		{
			uiScale = canvas.scaleFactor;
			uiScaleMultiplier = uiScale * CUSTOM_UI_SCALE_FOR_UIBULLSHIT;
			uiScaleForMostUI = uiScaleMultiplier * THING_FOR_UI_SCALE;
			uiBlockSmallBorderSize = Mathf.CeilToInt(uiScaleForMostUI * 4f);
			uiBlockNanoBorderSize = Mathf.CeilToInt(uiScaleForMostUI * 2f);
		}
		catch (Exception ex)
		{
			log.error("UIBULLSHIT 1 " + ex.ToString());
		}
		if (Input.GetKeyDown(KeyBinds.GetBind("pause")))
		{
			if ((Object)(object)PauseHandler.main != (Object)null && PauseHandler.main.disallowPause && !UIMainMenu.IsOpen())
			{
				UIMainMenu.SetOpen(open_or_nah: true);
			}
			else
			{
				UIMainMenu.SetOpen(open_or_nah: false);
			}
		}
		if (KrokoshaScavMultiplayer.IsNetworkActiveAndIsWorldGenerated())
		{
			ComponentHolderProtocol.GetOrAddComponent<UIInGame>((Object)(object)PlayerCamera.main);
			_ = (Object)(object)NetPlayer.LOCAL_PLAYER != (Object)null;
		}
	}

	private void LateUpdate()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		UIMainMenu.USERINPUT_NAME = KrokoshaScavMultiplayer.SanitizeTextInput(UIMainMenu.USERINPUT_NAME);
		if (_reset_gui_textures_next_frame)
		{
			_reset_gui_textures_next_frame = false;
			ResizeGUITextures();
		}
		if (last_screensize != Util.GetResolution())
		{
			_reset_gui_textures_next_frame = true;
		}
		IS_CURSOR_OVERLAPPING = TOTAL_FOCUSED_INTERACTIVE_UI > 0;
		TOTAL_INTERACTIVE_UI = 0;
		TOTAL_FOCUSED_INTERACTIVE_UI = 0;
	}

	public static bool CheckCursorOverlap(in Rect rect)
	{
		TOTAL_INTERACTIVE_UI++;
		if (IsCursorInGUIRect(in rect))
		{
			IS_CURSOR_OVERLAPPING = true;
			TOTAL_FOCUSED_INTERACTIVE_UI++;
			return true;
		}
		return false;
	}

	public static Rect HollowCheckCursorOverlap(in Rect rect)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		CheckCursorOverlap(in rect);
		return rect;
	}

	public static Rect ConstructorCheckCursorOverlap(in float x, in float y, in float width, in float height)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(x, y, width, height);
		CheckCursorOverlap(in rect);
		return rect;
	}

	public static Vector2 GetCursorPosInGUI()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2(Input.mousePosition.x, (float)Screen.height - Input.mousePosition.y);
	}

	public static bool IsCursorInGUIRect(in Rect area)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		Rect val = area;
		return ((Rect)(ref val)).Contains(GetCursorPosInGUI());
	}

	public static void _GUI_SetTooltip(in string name, in string desc)
	{
		_gui_tooltip = true;
		_gui_tooltip_name = name;
		_gui_tooltip_desc = desc;
	}

	public static void _GUI_BiggerLabel(in string txt, float multiplier)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		int fontSize = GUI.skin.label.fontSize;
		GUI.skin.label.fontSize = (int)((float)fontSize * multiplier);
		GUIContent val = new GUIContent(txt);
		Vector2 val2 = GUI.skin.label.CalcSize(val);
		GUILayout.Label(val, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Height(val2.y) });
		GUI.skin.label.fontSize = fontSize;
	}

	public static bool _GUI_FocusableButton(in Rect rect, in string text, out bool focused, GUIStyle style = null)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		bool result = ((style == null) ? GUI.Button(rect, text) : GUI.Button(rect, text, style));
		focused = IsCursorInGUIRect(in rect);
		TOTAL_INTERACTIVE_UI++;
		if (focused)
		{
			IS_CURSOR_OVERLAPPING = true;
			TOTAL_FOCUSED_INTERACTIVE_UI++;
		}
		return result;
	}

	public static void _GUI_9SlicePanel(in Rect rect, in float brightness, in float alpha, bool check_overlap, Texture2D tex = null)
	{
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Expected O, but got Unknown
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Expected O, but got Unknown
		Texture2D background = GUI.skin.box.normal.background;
		if (tex == null)
		{
			tex = unscaled_uiBlockSmall;
		}
		if ((Object)(object)tex != (Object)null)
		{
			GUI.skin.box.normal.background = GetChangedTex(tex, brightness, alpha);
			if ((Object)(object)tex == (Object)(object)unscaled_uiBlockSmall)
			{
				int num = uiBlockSmallBorderSize;
				GUI.skin.box.border = new RectOffset(num, num, num, num);
			}
			else if ((Object)(object)tex == (Object)(object)unscaled_uiBlockNano)
			{
				int num2 = uiBlockNanoBorderSize;
				GUI.skin.box.border = new RectOffset(num2, num2, num2, num2);
			}
		}
		GUI.Box(rect, "");
		GUI.skin.box.normal.background = background;
		if (check_overlap)
		{
			CheckCursorOverlap(in rect);
		}
	}

	public static bool IsAnyMenuOpen()
	{
		if (!Util.IsInPauseMenuOrMainMenu())
		{
			return UIMainMenu.IsOpen();
		}
		return true;
	}

	private void OnGUI()
	{
		_GUI_DoTheFullGUI();
	}

	private void __GUI__AllTheMenus()
	{
		try
		{
			if (!Con.IsConsoleOpen())
			{
				try
				{
					UIMainMenu._GUI_DrawSideMenuWithPlayerList();
					UIMainMenu._GUI_DrawMPMainMenu();
				}
				catch (Exception ex)
				{
					log.error("MAIN MENU UI: " + ex.ToString());
				}
			}
			Chat._GUI_DrawChat();
		}
		catch (Exception ex2)
		{
			log.error("CHAT UI: " + ex2.ToString());
		}
		if ((Object)(object)UIInGame.main != (Object)null)
		{
			UIInGame.main._GUI_DoInGameUI();
		}
	}

	private void _GUI_DoTheFullGUI()
	{
		try
		{
			try
			{
				_GUI_CaptureOGUITextures();
				_GUI_DoTexturesForMPUserInterface();
				try
				{
					GUI.enabled = !GUILayout_DropdownMenu.isOpen;
					__GUI__AllTheMenus();
					GUI.enabled = true;
					if ((Object)(object)UIInGame.main != (Object)null)
					{
						UIInGame.main._GUI_DoInGameUI();
					}
					GUILayout_DropdownMenu.Draw();
					if (!Con.IsConsoleOpen())
					{
						try
						{
							UIChoicePrompt.instance._GUI_RenderUIChoicePrompt();
						}
						catch (Exception ex)
						{
							log.error("_GUI_RenderUIChoicePrompt: " + ex.ToString());
						}
					}
					__GUI__DoTooltipBullshit();
				}
				catch (Exception ex2)
				{
					log.error("MAIN UI RENDER: " + ex2.ToString());
				}
				_GUI_ResetToOGUITextures();
			}
			catch (Exception ex3)
			{
				log.error("GAME UI: " + ex3.ToString());
			}
			if (Con.IsConsoleOpen())
			{
				return;
			}
		}
		catch (Exception ex4)
		{
			log.error("UIBULLSHIT ONGUI: " + ex4.ToString());
		}
		if (!Con.IsConsoleOpen())
		{
			_GUI_DrawVerboseWoundview();
		}
	}

	private void __GUI__DoTooltipBullshit()
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Expected O, but got Unknown
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Expected O, but got Unknown
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		if (_gui_tooltip)
		{
			_gui_tooltip = false;
			int fontSize = GUI.skin.label.fontSize;
			bool richText = GUI.skin.label.richText;
			bool wordWrap = GUI.skin.label.wordWrap;
			GUI.skin.label.fontSize = (int)((float)fontSize * TOOLTIP_SCALE);
			Vector2 val = GetCursorPosInGUI() + Vector2.one * 10f;
			GUI.skin.label.richText = true;
			if (!string.IsNullOrWhiteSpace(_gui_tooltip_name))
			{
				GUI.skin.label.wordWrap = false;
				GUIContent val2 = new GUIContent(_gui_tooltip_name);
				Vector2 val3 = GUI.skin.label.CalcSize(val2);
				Rect val4 = default(Rect);
				((Rect)(ref val4))._002Ector(val.x, val.y, val3.x, val3.y);
				Rect val5 = default(Rect);
				((Rect)(ref val5))._002Ector(val4);
				((Rect)(ref val5)).width = ((Rect)(ref val5)).width + val3.y * 0.1f;
				GUI.color = Color.black;
				GUI.DrawTexture(val5, (Texture)(object)WorldgenPatches.white_square.texture);
				GUI.color = Color.white;
				GUI.Label(val4, val2);
				val.y += val3.y;
			}
			if (!string.IsNullOrWhiteSpace(_gui_tooltip_desc))
			{
				GUI.skin.label.wordWrap = true;
				GUIContent val6 = new GUIContent(_gui_tooltip_desc);
				Vector2 val7 = GUI.skin.label.CalcSize(val6);
				Rect val8 = default(Rect);
				((Rect)(ref val8))._002Ector(val.x, val.y, val7.x, val7.y);
				Rect val9 = default(Rect);
				((Rect)(ref val9))._002Ector(val8);
				((Rect)(ref val9)).width = ((Rect)(ref val9)).width + val7.y * 0.1f;
				GUI.color = Color.black;
				GUI.DrawTexture(val9, (Texture)(object)WorldgenPatches.white_square.texture);
				GUI.color = new Color(0.9f, 0.9f, 0.9f);
				GUI.Label(val8, val6);
			}
			GUI.skin.label.wordWrap = wordWrap;
			GUI.skin.label.fontSize = fontSize;
			GUI.skin.label.richText = richText;
			GUI.color = Color.white;
		}
	}

	public static void _GUI_DrawVerboseWoundview()
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		if (!Util.IsInWoundView() || (MinigameBase.main.currentMinigame != null && MinigameBase.main.currentMinigame is CPRMinigame))
		{
			return;
		}
		WoundView view = WoundView.view;
		if (log.verbose)
		{
			int fontSize = GUI.skin.label.fontSize;
			GUI.skin.label.fontSize = 10;
			for (int i = 0; i < view.body.limbs.Length; i++)
			{
				Limb val = view.body.limbs[i];
				Vector3 position = ((Transform)((Graphic)view.limbImages[i]).rectTransform).position;
				position = Vector2.op_Implicit(new Vector2(position.x, (float)Screen.height - position.y));
				GUI.Label(new Rect(position.x, position.y, 200f, 20f), $"{((Object)val).name} id:{i}");
				GUI.Label(new Rect(position.x, position.y + 12f, 200f, 20f), $"totalForce: {Math.Round(val.totalForce, 2)}");
				GUI.Label(new Rect(position.x, position.y + 24f, 200f, 20f), $"shra: {val.shrapnel}");
				GUI.Label(new Rect(position.x, position.y + 36f, 200f, 20f), $"totalBleed: {val.totalBleedAmount}");
				GUI.Label(new Rect(position.x, position.y + 48f, 200f, 20f), $"inf: {val.infectionAmount}");
			}
			GUI.skin.label.fontSize = fontSize;
		}
		if (!Util.IsBodyLocal(view.body) && !UIInGame.SPECTATOR_MODE)
		{
			Component ba = (Component)(object)PlayerCamera.main.body;
			Component bb = (Component)(object)view.body;
			if (!KM.dist2dsqrcheck(in ba, in bb, SharedMain.max_player_interaction_distance * 2f))
			{
				PlayerCamera.main.ToggleWoundView(true);
			}
		}
	}

	public static int GetCurFontSize(float additional_scale = 1f)
	{
		if (USE_GAME_FONT)
		{
			return Mathf.RoundToInt(DEFAULT_RETROGAME_FONTSIZE * uiScale * CUSTOM_UI_SCALE_FOR_UIBULLSHIT * additional_scale);
		}
		return Mathf.RoundToInt((float)_og_fontsize * 2f * uiScale * CUSTOM_UI_SCALE_FOR_UIBULLSHIT * additional_scale);
	}

	private static void DoSliderThings(GUIStyle slider, GUIStyle thumb)
	{
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Expected O, but got Unknown
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Expected O, but got Unknown
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Expected O, but got Unknown
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Expected O, but got Unknown
		thumb.normal.background = GetChangedTex(unscaled_uiBlockNano, 1f, 1f);
		thumb.focused.background = GetChangedTex(unscaled_uiBlockNano, 0.9f, 1f);
		thumb.hover.background = thumb.focused.background;
		thumb.active.background = GetChangedTex(unscaled_uiBlockNano, 0.8f, 1f);
		slider.normal.background = GetChangedTex(unscaled_uiBlockNano, 1f, 1f);
		slider.overflow = new RectOffset(0, 0, 0, 0);
		thumb.overflow = new RectOffset(0, 0, 0, 0);
		slider.padding = new RectOffset(0, 0, 0, 0);
		slider.border = new RectOffset(uiBlockNanoBorderSize, uiBlockNanoBorderSize, uiBlockNanoBorderSize, uiBlockNanoBorderSize);
		thumb.border = slider.border;
		float num = 20f * uiScale;
		if (thumb.fixedHeight != 0f)
		{
			thumb.fixedHeight = num;
		}
		if (thumb.fixedWidth != 0f)
		{
			thumb.fixedWidth = num;
		}
	}

	private static void DoFontThings(GUIStyle style)
	{
		if (USE_GAME_FONT)
		{
			style.fontSize = GetCurFontSize();
			return;
		}
		style.font = null;
		style.fontSize = _og_fontsize;
	}

	private static void UnfuckFonts(GUIStyle style)
	{
		style.font = null;
		style.fontSize = _og_fontsize;
	}

	public static RectOffset RectOffset(int n)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Expected O, but got Unknown
		return new RectOffset(n, n, n, n);
	}

	public static void _GUI_SetButtonSkinTexture(GUIStyle style, bool small = true)
	{
		if (small)
		{
			style.normal.background = tex_button_normal;
			style.active.background = tex_button_active;
			style.focused.background = tex_button_focus;
			style.hover.background = tex_button_hover;
			int n = uiBlockSmallBorderSize;
			style.border = RectOffset(n);
			style.onNormal.background = GetChangedTex(unscaled_uiBlockSmall, 1.05f, 1f);
		}
		else
		{
			style.normal.background = tex_button_normal_nano;
			style.active.background = tex_button_active_nano;
			style.focused.background = tex_button_focus_nano;
			style.hover.background = tex_button_hover_nano;
			style.border = RectOffset(uiBlockNanoBorderSize);
			style.onNormal.background = GetChangedTex(unscaled_uiBlockNano, 1.05f, 1f);
		}
		style.onActive.background = style.active.background;
		style.onFocused.background = style.focused.background;
		style.onHover.background = style.hover.background;
		style.padding = style.border;
	}

	internal static void _GUI_MPUISetSkinValues()
	{
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0468: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Expected O, but got Unknown
		_GUI_SetButtonSkinTexture(GUI.skin.button);
		GUI.skin.box.normal.background = tex_button_hover_nano;
		GUI.skin.toggle.normal.background = GetChangedTex(KrokoshaCoopModAssets.checkbox.texture, 1f, 1f);
		if ((Object)(object)GUI.skin.toggle.normal.background != (Object)null)
		{
			GUI.skin.toggle.border = new RectOffset(((Texture)GUI.skin.toggle.normal.background).width, 1, ((Texture)GUI.skin.toggle.normal.background).height, 1);
			GUI.skin.toggle.padding.left = ((Texture)GUI.skin.toggle.normal.background).width;
			GUI.skin.toggle.padding.top = (int)((float)((Texture)GUI.skin.toggle.normal.background).height * 0.21f);
		}
		GUI.skin.toggle.focused.background = GetChangedTex(KrokoshaCoopModAssets.checkbox.texture, 0.9f, 1f);
		GUI.skin.toggle.hover.background = GUI.skin.toggle.focused.background;
		GUI.skin.toggle.active.background = GetChangedTex(KrokoshaCoopModAssets.checkbox.texture, 0.8f, 1f);
		GUI.skin.toggle.onNormal.background = GetChangedTex(KrokoshaCoopModAssets.checkbox_ticked.texture, 1f, 1f);
		GUI.skin.toggle.onHover.background = GetChangedTex(KrokoshaCoopModAssets.checkbox_ticked.texture, 0.9f, 1f);
		GUI.skin.toggle.onFocused.background = GUI.skin.toggle.onHover.background;
		GUI.skin.toggle.onActive.background = GetChangedTex(KrokoshaCoopModAssets.checkbox_ticked.texture, 0.8f, 1f);
		DoSliderThings(GUI.skin.horizontalSlider, GUI.skin.horizontalSliderThumb);
		DoSliderThings(GUI.skin.horizontalScrollbar, GUI.skin.horizontalScrollbarThumb);
		DoSliderThings(GUI.skin.verticalSlider, GUI.skin.verticalSliderThumb);
		DoSliderThings(GUI.skin.verticalScrollbar, GUI.skin.verticalScrollbarThumb);
		float num = 20f * uiScale;
		GUI.skin.verticalScrollbar.fixedWidth = num;
		GUI.skin.horizontalScrollbar.fixedHeight = num;
		GUI.skin.verticalSlider.fixedWidth = num;
		GUI.skin.horizontalSlider.fixedHeight = num;
		GUI.skin.textField.normal.background = GetChangedTex(unscaled_uiBlockNano, 0.8f, 0.8f);
		GUI.skin.textField.hover.background = GetChangedTex(unscaled_uiBlockNano, 0.9f, 0.8f);
		GUI.skin.textField.focused.background = GetChangedTex(unscaled_uiBlockNano, 1f, 0.8f);
		GUI.skin.textField.onNormal.background = GetChangedTex(unscaled_uiBlockNano, 0.9f, 0.8f);
		GUI.skin.textArea.normal.background = GUI.skin.textField.normal.background;
		GUI.skin.textArea.hover.background = GUI.skin.textField.hover.background;
		GUI.skin.textArea.focused.background = GUI.skin.textField.focused.background;
		GUI.skin.textArea.onNormal.background = GUI.skin.textField.onNormal.background;
		GUI.skin.textArea.onNormal.textColor = Color.white;
		GUI.skin.textArea.focused.textColor = Color.white;
		if ((Object)(object)KrokoshaCoopModAssets.gamefont != (Object)null)
		{
			if (USE_GAME_FONT)
			{
				GUI.skin.font = KrokoshaCoopModAssets.gamefont;
			}
			else
			{
				GUI.skin.font = _b_ogfont;
			}
			DoFontThings(GUI.skin.button);
			DoFontThings(GUI.skin.textField);
			DoFontThings(GUI.skin.textArea);
			DoFontThings(GUI.skin.label);
			DoFontThings(GUI.skin.box);
			DoFontThings(GUI.skin.toggle);
		}
	}

	internal static void _GUI_DoTexturesForMPUserInterface()
	{
		try
		{
			if ((Object)(object)_og_skin_cloned == (Object)null)
			{
				_og_skin_cloned = Object.Instantiate<GUISkin>(GUI.skin);
			}
			if ((Object)(object)new_skin == (Object)null)
			{
				new_skin = Object.Instantiate<GUISkin>(GUI.skin);
			}
			GUI.skin = new_skin;
			if (_recalculate_skin)
			{
				_recalculate_skin = false;
				_GUI_MPUISetSkinValues();
			}
			else
			{
				_GUI_MPUISetSkinValues();
			}
		}
		catch (Exception)
		{
		}
	}

	internal static void _GUI_CaptureOGUITextures()
	{
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)_og_skin == (Object)null)
		{
			_og_skin = GUI.skin;
			_og_verticalscrollbar_fixedWidth = GUI.skin.verticalScrollbar.fixedWidth;
			_og_gorizontalscrollbar_fixedHeight = GUI.skin.horizontalScrollbar.fixedHeight;
		}
		if ((Object)(object)_b_n_b == (Object)null)
		{
			_og_fontsize = GUI.skin.button.fontSize;
			_b_n_b = GUI.skin.button.normal.background;
			_b_a_b = GUI.skin.button.active.background;
			_b_f_b = GUI.skin.button.focused.background;
			_b_h_b = GUI.skin.button.hover.background;
			if (_og_border == null)
			{
				_og_border = GUI.skin.button.border;
			}
			_box_bg = GUI.skin.box.normal.background;
			_box_txcolor = GUI.skin.box.normal.textColor;
			_b_ogfont = GUI.skin.font;
		}
	}

	internal static void _GUI_ResetToOGUITextures()
	{
		if ((Object)(object)_og_skin != (Object)null)
		{
			GUI.skin = _og_skin;
		}
	}

	private void OnDestroy()
	{
		IS_CURSOR_OVERLAPPING = false;
	}
}
