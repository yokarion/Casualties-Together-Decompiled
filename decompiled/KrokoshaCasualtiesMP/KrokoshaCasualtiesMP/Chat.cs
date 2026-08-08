using System;
using System.Collections.Generic;
using System.Linq;
using KrokoshaCasualtiesUtils;
using LiteNetLib;
using LiteNetLib.Utils;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public class Chat : KrokoshaScavSingleton
{
	public class ChatMsgContainer
	{
		public NetPlayer plr;

		public string name;

		public string tag;

		public string msg;

		public string msg2;

		public string result_nametag;

		public string result_nametag_sanitized;

		public bool rich;

		public Vector2 namesize;

		public float height;

		public ChatMsgContainer(in NetPlayer plr, in string tag, string s, in bool rich = false)
		{
			this.plr = plr;
			this.rich = rich;
			msg = s;
			this.tag = tag;
			name = plr.playername;
			Compile();
		}

		public ChatMsgContainer(in string name, in string tag, string s, in bool rich = false)
		{
			this.rich = rich;
			msg = s;
			this.tag = tag;
			this.name = name;
			Compile();
		}

		public void Compile()
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			string text = "";
			text = ((!((Object)(object)plr != (Object)null)) ? TagName(name, tag, rich: true) : TagName("<color=#" + ColorUtility.ToHtmlStringRGB((Color)plr.plrcolor) + ">" + name + "</color>", tag, rich: true));
			result_nametag = "[" + text + "]: ";
			result_nametag_sanitized = KrokoshaScavMultiplayer.SanitizeRichText(result_nametag);
		}
	}

	private enum MessageType : byte
	{
		PlayerMessage,
		ServerAnnouncement,
		ServerAnnouncementCustomName
	}

	[SettingDeclarerThingyFloat(0f, 1f, "setting_chatposx")]
	public static float setting_posX = 1f;

	[SettingDeclarerThingyFloat(0f, 1f, "setting_chatposy")]
	public static float setting_posY = 1f;

	[SettingDeclarerThingyFloat(0f, 3f, "setting_chatsizex")]
	public static float setting_sizeX = 1f;

	[SettingDeclarerThingyFloat(0f, 3f, "setting_chatsizey")]
	public static float setting_sizeY = 1f;

	[SettingDeclarerThingyFloat(0f, 2f, "setting_chatscale")]
	public static float setting_scale = 1f;

	[SettingDeclarerThingyBool("setting_chatpreview")]
	public static bool _DEV_ENABLE_PREVIEW = true;

	[SettingDeclarerThingyBool("setting_chatpreviewonside")]
	public static bool _DEV_PREVIEW_ON_SIDE = false;

	public static bool _DEV_LOG_CHAT = false;

	public static float DEV_CHAT_FULL_WIDTH = 500f;

	public static float DEV_CHAT_HISTORY_HEIGHT = 250f;

	public static float DEV_CHAT_TEXTFIELD_HEIGHT = 40f;

	private const string CHAT_TEXTFIELD_CONTROLNAME = "ChatField";

	private static bool scrollbar_is_being_interacted = false;

	private static Rect? scrollbar_area = null;

	private static bool mouse_is_in_chat_area = false;

	private static double focustime = 0.0;

	private static bool _chatinput_changed = false;

	private static float _chatinput_timer = 0f;

	private static string _preview_distorted_message = "";

	private static Vector2 _GUI_SizeOfSpace = Vector2.one;

	private static MaxCapacityQueue<string> MyMessageLog = new MaxCapacityQueue<string>(20);

	private static int MyMessageLog_Index = 0;

	private const int MAX_MESSAGES_HISTORY_COUNT = 100;

	private static MaxCapacityQueue<ChatMsgContainer> CHAT_LOG = new MaxCapacityQueue<ChatMsgContainer>(100);

	private static string CHAT_current_input = "";

	private static float CHATScrollPosition;

	public static bool CHAT_textbox_input_focused = false;

	public static bool CHATforceinputfocus = false;

	public const string NAMELABEL_LOCALSYSTEM = "*SYSTEM*";

	public const string NAMELABEL_SERVER = "*SERVER*";

	private const ushort MAX_MESSAGE_LENGTH = 150;

	public static bool SHOULD_LOG_CHAT
	{
		get
		{
			if (!_DEV_LOG_CHAT && !log.verbose)
			{
				return Con._DEV_CHATSPY;
			}
			return true;
		}
	}

	public static int _GUI_RealChatFontsize => Mathf.RoundToInt((float)UIBullshit.GetCurFontSize() * 0.82f * setting_scale);

	public static bool is_client => KrokoshaScavMultiplayer.is_client;

	public static event Action<NetPlayer, string> OnPlayerChatMessage;

	private static void DoNothing()
	{
	}

	public static void ShowChat()
	{
		focustime = Time.realtimeSinceStartupAsDouble;
	}

	private void Start()
	{
		ShowChat();
	}

	private void Update()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		if (!KrokoshaScavMultiplayer.network_system_is_running)
		{
			return;
		}
		if (scrollbar_area.HasValue && Input.GetKeyDown((KeyCode)323) && UIBullshit.IsCursorInGUIRect(scrollbar_area.Value))
		{
			scrollbar_is_being_interacted = true;
		}
		if (Input.GetKeyUp((KeyCode)323))
		{
			scrollbar_is_being_interacted = false;
		}
		if (Con.IsConsoleOpen())
		{
			CHATforceinputfocus = false;
			CHAT_textbox_input_focused = false;
		}
		else if (!CHAT_textbox_input_focused)
		{
			_preview_distorted_message = "";
			if (Input.GetKeyDown(KeyBinds.GetBind("krokosha_coop_chat")) && (!Object.op_Implicit((Object)(object)PlayerCamera.main) || !ConsoleScript.instance.active))
			{
				focustime = Time.realtimeSinceStartupAsDouble;
				UpdateMessageDistortPreview();
				CHAT_textbox_input_focused = true;
				CHATforceinputfocus = true;
				_chatinput_timer = 2f;
			}
			return;
		}
		if (Util.IsLocalBodyAlive() && CHAT_textbox_input_focused && KrokoshaScavMultiplayer.rules.SpeechImpairedChat && !string.IsNullOrWhiteSpace(CHAT_current_input))
		{
			_chatinput_timer += Time.deltaTime;
			if (_chatinput_changed || _chatinput_timer > 1f)
			{
				UpdateMessageDistortPreview();
			}
		}
	}

	public static void UpdateMessageDistortPreview()
	{
		_chatinput_timer = 0f;
		_chatinput_changed = false;
		if (Util.TryGetLocalBody(out var body))
		{
			_preview_distorted_message = body.talker.DistortString(in CHAT_current_input);
			NetBody netBody = default(NetBody);
			if (((Component)body).TryGetComponent<NetBody>(ref netBody))
			{
				string chattag = "test";
				netBody.HearinglossDistortMessage(NetPlayer.LOCAL_PLAYER.playerbody, ref _preview_distorted_message, ref chattag);
			}
		}
		else
		{
			_preview_distorted_message = "";
		}
	}

	public static bool MouseIsInteractingWithScrollbar()
	{
		if (scrollbar_area.HasValue)
		{
			return scrollbar_is_being_interacted;
		}
		return false;
	}

	private static void _GUI_ChatDrawMessage(ChatMsgContainer textinfo, float real_pos_X, ref float this_y, float chathistory_area_width, float texthiehgt)
	{
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		if ((Object)(object)textinfo.plr != (Object)null)
		{
			int gUI_RealChatFontsize = _GUI_RealChatFontsize;
			Rect val = default(Rect);
			((Rect)(ref val))._002Ector(real_pos_X + num, this_y + 1f, (float)gUI_RealChatFontsize, (float)gUI_RealChatFontsize);
			int num2 = (int)(8f * UIBullshit.uiScale);
			int num3 = gUI_RealChatFontsize + num2;
			foreach (Texture2D additional_profile_tag_icon in textinfo.plr.additional_profile_tag_icons)
			{
				((Rect)(ref val)).x = real_pos_X + num;
				GUI.DrawTexture(val, (Texture)(object)additional_profile_tag_icon);
				num += (float)num3;
				chathistory_area_width -= (float)num3;
			}
			Texture2D profilepic_any_smalltolarge = textinfo.plr.profilepic_any_smalltolarge;
			if ((Object)(object)profilepic_any_smalltolarge != (Object)null)
			{
				((Rect)(ref val)).x = real_pos_X + num;
				GUI.DrawTexture(val, (Texture)(object)profilepic_any_smalltolarge);
				num += (float)num3;
				chathistory_area_width -= (float)num3;
			}
		}
		GUI.skin.label.richText = true;
		GUI.Label(new Rect(real_pos_X + num, this_y, textinfo.namesize.x, textinfo.namesize.y), textinfo.result_nametag);
		num += textinfo.namesize.x;
		GUI.skin.label.richText = textinfo.rich;
		GUI.skin.label.wordWrap = true;
		GUI.Label(new Rect(real_pos_X + chathistory_area_width * 0.1f, this_y, chathistory_area_width * 0.91f, texthiehgt + textinfo.namesize.y * 2f), textinfo.msg2);
	}

	internal static void _GUI_DrawChat()
	{
		//IL_07a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b3: Expected O, but got Unknown
		//IL_07ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_081c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0826: Expected O, but got Unknown
		//IL_0821: Unknown result type (might be due to invalid IL or missing references)
		//IL_0826: Unknown result type (might be due to invalid IL or missing references)
		//IL_0916: Unknown result type (might be due to invalid IL or missing references)
		//IL_0928: Expected O, but got Unknown
		//IL_09ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_09da: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Invalid comparison between Unknown and I4
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Invalid comparison between Unknown and I4
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Invalid comparison between Unknown and I4
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Invalid comparison between Unknown and I4
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Invalid comparison between Unknown and I4
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Invalid comparison between Unknown and I4
		//IL_045c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0463: Invalid comparison between Unknown and I4
		//IL_058d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0599: Expected O, but got Unknown
		//IL_05a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_065e: Unknown result type (might be due to invalid IL or missing references)
		//IL_068d: Unknown result type (might be due to invalid IL or missing references)
		if (((KrokoshaScavMultiplayer.network_system_is_running || !UIMainMenu.IsInSettings() || UIMainMenu.cur_settings_tab != UIMainMenu._chat_setting_tab) && (!KrokoshaScavMultiplayer.network_system_is_running || Con._DEV_HIDEHUD)) || setting_scale <= 0f || setting_sizeY <= 0f || setting_sizeX <= 0f)
		{
			CHATforceinputfocus = false;
			CHAT_textbox_input_focused = false;
			return;
		}
		bool wordWrap = GUI.skin.label.wordWrap;
		int fontSize = GUI.skin.label.fontSize;
		int fontSize2 = GUI.skin.box.fontSize;
		int fontSize3 = GUI.skin.textField.fontSize;
		try
		{
			bool flag = KrokoshaScavMultiplayer.IsInGameAndWorldGenerated() && !Con._DEV_CHATSPY && Util.IsLocalBodyAlive() && !KrokoshaScavMultiplayer.rules.EnableChatbox;
			int gUI_RealChatFontsize = _GUI_RealChatFontsize;
			float num = setting_scale * UIBullshit.uiScale;
			GUI.skin.label.fontSize = gUI_RealChatFontsize;
			GUI.skin.box.fontSize = gUI_RealChatFontsize;
			GUI.skin.textField.fontSize = gUI_RealChatFontsize;
			float num2 = DEV_CHAT_FULL_WIDTH * num * setting_sizeX;
			float num3 = Mathf.Max(0f, DEV_CHAT_HISTORY_HEIGHT * num * setting_sizeY);
			float num4 = DEV_CHAT_TEXTFIELD_HEIGHT * num;
			float num5 = ((float)Screen.width - num2) * setting_posX;
			float num6 = ((float)Screen.height - num3 - num4) * setting_posY;
			Rect rect = default(Rect);
			((Rect)(ref rect))._002Ector(num5, num6, num2, num3);
			Rect rect2 = default(Rect);
			((Rect)(ref rect2))._002Ector(num5, num6 + num3, num2, num4);
			float num7 = num2 - (float)UIBullshit.uiBlockNanoBorderSize;
			bool flag2 = false;
			if (Time.realtimeSinceStartupAsDouble - focustime < 5.0)
			{
				flag2 = true;
			}
			Rect area = default(Rect);
			((Rect)(ref area))._002Ector(rect2);
			((Rect)(ref area)).xMin = ((Rect)(ref area)).xMin - 50f;
			((Rect)(ref area)).xMax = ((Rect)(ref area)).xMax + 50f;
			((Rect)(ref area)).yMin = ((Rect)(ref area)).yMin - 50f;
			((Rect)(ref area)).yMax = ((Rect)(ref area)).yMax + 50f;
			if (flag2 || CHATforceinputfocus || CHAT_textbox_input_focused || mouse_is_in_chat_area || UIBullshit.IsCursorInGUIRect(in area))
			{
				if (CHATforceinputfocus)
				{
					GUI.FocusControl("ChatField");
					CHAT_textbox_input_focused = true;
					CHATforceinputfocus = false;
				}
				if (CHAT_textbox_input_focused)
				{
					KeyCode keyCode = Event.current.keyCode;
					if ((int)Event.current.type == 4 && ((int)keyCode == 273 || (int)keyCode == 274))
					{
						int num8 = 0;
						if ((int)keyCode == 273)
						{
							num8 = -1;
						}
						if ((int)keyCode == 274)
						{
							num8 = 1;
						}
						if (num8 != 0 && MyMessageLog.Count != 0)
						{
							MyMessageLog_Index += num8;
							if (MyMessageLog_Index < 0)
							{
								MyMessageLog_Index = 0;
							}
							else if (MyMessageLog_Index >= MyMessageLog.Count)
							{
								MyMessageLog_Index = MyMessageLog.Count - 1;
							}
							else
							{
								if (!string.IsNullOrWhiteSpace(CHAT_current_input) && !MyMessageLog.Contains(in CHAT_current_input))
								{
									MyMessageLog.Enqueue(CHAT_current_input);
								}
								CHAT_current_input = MyMessageLog.ElementAt(MyMessageLog_Index);
								_chatinput_changed = true;
								CHATforceinputfocus = true;
							}
						}
					}
					GUI.SetNextControlName("ChatField");
					string cHAT_current_input = CHAT_current_input;
					CHAT_current_input = GUI.TextField(rect2, CHAT_current_input);
					if (cHAT_current_input != CHAT_current_input)
					{
						_chatinput_changed = true;
						CHATforceinputfocus = true;
					}
					GUI.SetNextControlName("NONE");
				}
				else
				{
					UIBullshit._GUI_9SlicePanel(in rect2, 1f, 0.65f, check_overlap: false, UIBullshit.unscaled_uiBlockNano);
					Rect val = default(Rect);
					((Rect)(ref val))._002Ector(rect2);
					((Rect)(ref val)).xMin = ((Rect)(ref val)).xMin + (float)UIBullshit.uiBlockNanoBorderSize;
					GUI.skin.label.alignment = (TextAnchor)4;
					GUI.Label(val, string.IsNullOrEmpty(CHAT_current_input) ? string.Format(Lang.Get("chat_keybind", false), KeyBinds.GetBindName("krokosha_coop_chat")) : CHAT_current_input);
					GUI.skin.label.alignment = (TextAnchor)0;
				}
				if (CHAT_textbox_input_focused)
				{
					if ((int)Event.current.keyCode == 27)
					{
						CHATforceinputfocus = false;
						CHAT_textbox_input_focused = false;
						GUI.FocusControl("");
						Event.current.keyCode = (KeyCode)0;
					}
					else if ((int)Event.current.keyCode == 13)
					{
						CHATforceinputfocus = false;
						CHAT_textbox_input_focused = false;
						GUI.FocusControl("");
						_chatinput_changed = true;
						if (!string.IsNullOrEmpty(CHAT_current_input))
						{
							OnEnteredUserMessage();
						}
						Event.current.keyCode = (KeyCode)0;
					}
					else
					{
						focustime = Time.realtimeSinceStartupAsDouble;
					}
					if (CHAT_current_input.Count() > 150)
					{
						CHAT_current_input = CHAT_current_input.Substring(0, 150);
					}
				}
				CHAT_textbox_input_focused = GUI.GetNameOfFocusedControl() == "ChatField";
			}
			GUI.SetNextControlName("NONE");
			if (_DEV_ENABLE_PREVIEW)
			{
				try
				{
					if (Util.IsLocalBodyAlive() && CHAT_textbox_input_focused && KrokoshaScavMultiplayer.rules.SpeechImpairedChat && !string.IsNullOrWhiteSpace(CHAT_current_input) && !CHAT_current_input.StartsWith("/") && !string.IsNullOrWhiteSpace(_preview_distorted_message) && _preview_distorted_message != CHAT_current_input)
					{
						string text = Lang.Get("chat_preview", false) + _preview_distorted_message;
						float num9 = Mathf.Max(num4, GUI.skin.label.CalcHeight(new GUIContent(text), num2));
						Rect rect3 = default(Rect);
						((Rect)(ref rect3))._002Ector(rect2);
						if (!_DEV_PREVIEW_ON_SIDE)
						{
							((Rect)(ref rect3)).y = ((Rect)(ref rect3)).y - num9;
							((Rect)(ref rect3)).height = num9;
							((Rect)(ref rect)).height = ((Rect)(ref rect)).height - num9;
						}
						else
						{
							((Rect)(ref rect3)).y = ((Rect)(ref rect2)).yMax - num9;
							if (setting_posX > 0.5f)
							{
								((Rect)(ref rect3)).x = ((Rect)(ref rect3)).x - ((Rect)(ref rect3)).width;
							}
							else
							{
								((Rect)(ref rect3)).x = ((Rect)(ref rect3)).x + ((Rect)(ref rect3)).width;
							}
							((Rect)(ref rect3)).height = num9;
						}
						GUI.skin.label.wordWrap = true;
						UIBullshit._GUI_9SlicePanel(in rect3, 1f, 0.65f, check_overlap: false, UIBullshit.unscaled_uiBlockNano);
						Rect val2 = default(Rect);
						((Rect)(ref val2))._002Ector(rect3);
						((Rect)(ref val2)).xMin = ((Rect)(ref val2)).xMin + (float)UIBullshit.uiBlockNanoBorderSize;
						((Rect)(ref val2)).xMax = ((Rect)(ref val2)).xMax - (float)UIBullshit.uiBlockNanoBorderSize;
						GUI.Label(val2, text);
						GUI.skin.label.wordWrap = wordWrap;
					}
				}
				catch (Exception ex)
				{
					log.error("CHAT PREVIEW RENDER ERROR:\n" + ex.ToString());
				}
			}
			Rect area2 = default(Rect);
			((Rect)(ref area2))._002Ector(num5, ((Rect)(ref rect)).y, num2, ((Rect)(ref rect2)).yMax - ((Rect)(ref rect)).y);
			mouse_is_in_chat_area = UIBullshit.IsCursorInGUIRect(in area2);
			if (mouse_is_in_chat_area)
			{
				focustime = Time.realtimeSinceStartupAsDouble;
			}
			if (num3 > 0f && num2 > 0f && Time.realtimeSinceStartupAsDouble - focustime < 2.0 && !flag)
			{
				UIBullshit._GUI_9SlicePanel(in rect, 1f, 0.65f, check_overlap: false, UIBullshit.unscaled_uiBlockNano);
			}
			if (flag2 && !flag && num3 > 0f)
			{
				GUI.SetNextControlName("NONE");
				GUI.skin.label.richText = true;
				GUI.skin.label.wordWrap = true;
				float real_pos_X = num5 + (float)UIBullshit.uiBlockNanoBorderSize;
				_GUI_SizeOfSpace = GUI.skin.label.CalcSize(new GUIContent(" "));
				if (_GUI_SizeOfSpace.x <= 0f)
				{
					_GUI_SizeOfSpace = Vector2.one;
				}
				GUI.skin.label.alignment = (TextAnchor)0;
				float num10 = 0f;
				foreach (ChatMsgContainer item in CHAT_LOG.JustGiveTheQueue())
				{
					item.namesize = GUI.skin.label.CalcSize(new GUIContent(item.result_nametag));
					float num11 = item.namesize.x + 1f;
					if ((Object)(object)item.plr != (Object)null)
					{
						foreach (Texture2D additional_profile_tag_icon in item.plr.additional_profile_tag_icons)
						{
							_ = additional_profile_tag_icon;
							num11 += (float)(_GUI_RealChatFontsize + (int)(8f * UIBullshit.uiScale));
						}
						if ((Object)(object)item.plr.profilepic_any_smalltolarge != (Object)null)
						{
							num11 += (float)(_GUI_RealChatFontsize + (int)(8f * UIBullshit.uiScale));
						}
					}
					item.msg2 = new string(' ', Math.Max(0, Mathf.CeilToInt((num11 - num7 * 0.1f) / _GUI_SizeOfSpace.x))) + item.msg;
					item.height = GUI.skin.label.CalcHeight(new GUIContent(item.msg2), num7 * 0.88f);
					num10 += item.height;
				}
				if (num10 - num3 > 0f)
				{
					float fixedWidth = GUI.skin.verticalSlider.fixedWidth;
					GUI.skin.verticalSlider.fixedWidth = GUI.skin.verticalSlider.fixedWidth / UIBullshit.uiScale * num;
					float fixedWidth2 = GUI.skin.verticalSlider.fixedWidth;
					Rect val3 = default(Rect);
					((Rect)(ref val3))._002Ector(((Rect)(ref rect)).xMax - fixedWidth2, ((Rect)(ref rect)).y, fixedWidth2, ((Rect)(ref rect)).height);
					scrollbar_area = val3;
					CHATScrollPosition = GUI.VerticalScrollbar(val3, CHATScrollPosition, num3, num10, 0f);
					GUI.skin.verticalSlider.fixedWidth = fixedWidth;
					num7 -= fixedWidth2 * 1.05f;
				}
				else
				{
					CHATScrollPosition = 0f;
					scrollbar_area = null;
				}
				GUI.skin.label.alignment = (TextAnchor)0;
				float this_y = ((Rect)(ref rect)).yMax + CHATScrollPosition;
				for (int num12 = CHAT_LOG.Count - 1; num12 >= 0; num12--)
				{
					ChatMsgContainer chatMsgContainer = CHAT_LOG.ElementAt(num12);
					float height = chatMsgContainer.height;
					this_y -= height;
					if (this_y <= ((Rect)(ref rect2)).y - height * 0.8f)
					{
						if (this_y < ((Rect)(ref rect)).y - height)
						{
							break;
						}
						_GUI_ChatDrawMessage(chatMsgContainer, real_pos_X, ref this_y, num7, height);
					}
				}
				GUI.skin.label.richText = false;
			}
		}
		catch (Exception ex2)
		{
			log.error("CHAT RENDER ERROR:\n" + ex2.ToString());
		}
		GUI.skin.label.wordWrap = wordWrap;
		GUI.skin.label.fontSize = fontSize;
		GUI.skin.box.fontSize = fontSize2;
		GUI.skin.textField.fontSize = fontSize3;
	}

	public static void OnLogMessage()
	{
		focustime = Math.Max(focustime, Time.realtimeSinceStartupAsDouble - 2.0999999046325684);
		CHATScrollPosition = 0f;
	}

	public static void LogMessage(string plrname, string msg, in bool richtext = false)
	{
		CHAT_LOG.Enqueue(new ChatMsgContainer(in plrname, (string)null, msg, in richtext));
		OnLogMessage();
	}

	private static bool CheckForCommandInMessageInput()
	{
		List<string> list = new List<string>(CHAT_current_input.Trim().Split(new char[1] { ' ' }));
		if (list[0] == "/?" || list[0] == "/help")
		{
			LogMessage("*SYSTEM*", "Main commands:\n/[console command] // run a console command\n/clear // clear chat history\n/rule [rule name] [value] // set a game rule\n/rules // show all game rules\n/maxplayers [number] // set max player count\n// But really just use the console. (" + KeyBinds.GetBindName("console") + ")", false);
			return true;
		}
		if (list[0].StartsWith("/"))
		{
			list = new List<string>(CHAT_current_input.Substring(1).Split(new char[1] { ' ' }));
			string text = list[0];
			if (text == "clear")
			{
				CHAT_LOG.Clear();
			}
			else if (ConsoleScript.SearchExact(text) != null)
			{
				LogMessage("*SYSTEM*", text, false);
			}
			else
			{
				LogMessage("*SYSTEM*", "Unknown command: " + text, false);
			}
			return true;
		}
		return false;
	}

	private static void OnEnteredUserMessage()
	{
		bool flag = false;
		if (CHAT_current_input.StartsWith("/"))
		{
			flag = CheckForCommandInMessageInput();
		}
		if (!flag)
		{
			SendChatMessage(in CHAT_current_input);
		}
		_chatinput_changed = true;
		if (!MyMessageLog.Contains(in CHAT_current_input))
		{
			MyMessageLog.Enqueue(CHAT_current_input);
		}
		MyMessageLog_Index = MyMessageLog.Count;
		CHAT_current_input = "";
	}

	public static bool ValidateChatMessage(in string msg)
	{
		if (StringUtility.Contains(msg, '\r'))
		{
			return false;
		}
		if (msg.Length > 150)
		{
			return false;
		}
		return true;
	}

	public static bool CharCanBeReplacedWithBlank(in char c)
	{
		if (!char.IsWhiteSpace(c) && c != '?' && c != '!' && c != '.')
		{
			return c != '-';
		}
		return false;
	}

	protected static string TagName(string name, string tag, bool rich = false)
	{
		if (!string.IsNullOrWhiteSpace(tag))
		{
			Lang.MsgTryTranslateIfItsLocaleKey(ref tag);
			if (!rich)
			{
				tag = KrokoshaScavMultiplayer.SanitizeRichText(tag);
			}
			name = "*" + tag + "* " + name;
		}
		return name;
	}

	[ClientReceiver(10098, false)]
	private static void Client_ChatMessageReceive(knetid _, ref NetDataReader reader)
	{
		byte b = default(byte);
		reader.Get(ref b);
		switch ((MessageType)b)
		{
		case MessageType.PlayerMessage:
		{
			reader.Get(out knetid result);
			string tag2 = default(string);
			reader.Get(ref tag2);
			string text2 = default(string);
			reader.Get(ref text2);
			string name = result.ToString();
			if (NetPlayer.TryGetPlayerFromClientId(result, out var plr))
			{
				name = plr.playername;
				if (plr.IsAlive())
				{
					plr.body.talker.ForceNoSpeechImpairment(text2, resetTalkTimer: true);
				}
			}
			if (KrokoshaScavMultiplayer.is_client)
			{
				try
				{
					Chat.OnPlayerChatMessage?.Invoke(plr, text2);
				}
				catch (Exception ex)
				{
					log.error("CLIENT: OnPlayerChatMessage: " + ex.ToString());
				}
			}
			if (KrokoshaScavMultiplayer.is_client && SHOULD_LOG_CHAT)
			{
				Plugin.log.LogInfo((object)log.do_timestamp("CLIENT: CHAT PLAYER MESSAGE: " + name + ": " + text2));
			}
			if (true)
			{
				Util.PlayUISound((UISoundType)1, 0.95f, 0.3f);
				CHAT_LOG.Enqueue(((Object)(object)plr != (Object)null) ? new ChatMsgContainer(in plr, in tag2, text2, false) : new ChatMsgContainer(in name, in tag2, text2, false));
				OnLogMessage();
			}
			break;
		}
		case MessageType.ServerAnnouncement:
		{
			string message2 = default(string);
			reader.Get(ref message2);
			if (KrokoshaScavMultiplayer.is_client && SHOULD_LOG_CHAT)
			{
				Plugin.log.LogInfo((object)("CLIENT: SERVER ANNOUNCEMENT MESSAGE: " + message2));
			}
			Lang.MsgTryTranslateIfItsLocaleKey(ref message2);
			Util.PlayUISound((UISoundType)1, 0.86f, 0.9f);
			LogMessage("*SERVER*", message2, true);
			break;
		}
		case MessageType.ServerAnnouncementCustomName:
		{
			string text = default(string);
			reader.Get(ref text);
			string tag = default(string);
			reader.Get(ref tag);
			string message = default(string);
			reader.Get(ref message);
			if (KrokoshaScavMultiplayer.is_client && SHOULD_LOG_CHAT)
			{
				Plugin.log.LogInfo((object)("CLIENT: SERVER NAMED MESSAGE: " + text + ": " + message));
			}
			Lang.MsgTryTranslateIfItsLocaleKey(ref message);
			Util.PlayUISound((UISoundType)1, 0.95f, 0.7f);
			LogMessage(TagName(text, tag), message, true);
			break;
		}
		}
	}

	[ServerReceiver(10099)]
	private static void Server_PlayerChatMessageSend(knetid clientId, ref NetDataReader reader)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		bool flag = default(bool);
		reader.Get(ref flag);
		string msg = default(string);
		reader.Get(ref msg);
		if (string.IsNullOrWhiteSpace(msg) || !NetPlayer.TryGetPlayerFromClientId(clientId, out var plr))
		{
			return;
		}
		if (plr.server_mute_tc)
		{
			NetDataWriter writer = Net.CreateWriter(10098);
			writer.Put((byte)1);
			writer.Put("You're muted by the server!");
			Net.Server_SendToClients((DeliveryMethod)4, in writer, plr.clientId);
			return;
		}
		if (!ValidateChatMessage(in msg))
		{
			plr.Server_DoAlertSingle("SERVER: Invalid chat message.", reliable: false);
			return;
		}
		if (SHOULD_LOG_CHAT)
		{
			Plugin.log.LogInfo((object)$"SERVER: \"{plr}\" CHAT MESSAGE: {msg}");
		}
		try
		{
			Chat.OnPlayerChatMessage?.Invoke(plr, msg);
		}
		catch (Exception ex)
		{
			log.error("SERVER: OnPlayerChatMessage: " + ex.ToString());
		}
		bool dEV_CHATSPY = Con._DEV_CHATSPY;
		bool num = dEV_CHATSPY || KrokoshaScavMultiplayer.is_dedicated_server;
		string text = "";
		if (plr.IsDead())
		{
			text = Lang.MarkMsgAsLocaleKey("plr_chattag_dead");
		}
		if (num)
		{
			LogMessage(TagName(plr.playername, text), msg, false);
		}
		if (plr.IsAlive() && KrokoshaScavMultiplayer.rules.SpeechImpairedChat)
		{
			msg = plr.body.talker.DistortString(in msg);
		}
		if (num)
		{
			plr.body?.talker?.ForceNoSpeechImpairment(msg, resetTalkTimer: true);
		}
		foreach (NetPlayer value in NetPlayer.ClientIdToPlayerDict.Values)
		{
			if (dEV_CHATSPY && value.is_local)
			{
				continue;
			}
			string message = msg;
			string chattag = text;
			if (Util.IsWorldGenerated() && (Object)(object)value != (Object)(object)plr)
			{
				if (!plr.CanCommunicateWith_TextChat(value))
				{
					continue;
				}
				if (value.IsAlive())
				{
					value.playerbody.HearinglossDistortMessage(plr.playerbody, ref message, ref chattag);
				}
				if (string.IsNullOrWhiteSpace(message))
				{
					continue;
				}
			}
			NetDataWriter writer2 = Net.CreateWriter(10098);
			writer2.Put((byte)0);
			writer2.Put((ushort)clientId);
			writer2.Put(chattag);
			writer2.Put(message);
			Net.Server_SendToClients((DeliveryMethod)2, in writer2, value.clientId);
		}
	}

	public static void SendChatMessage(in string message, bool force_server_if_server = false)
	{
		if (!string.IsNullOrEmpty(message))
		{
			if (!ValidateChatMessage(in message))
			{
				LogMessage("*SYSTEM*", "Message is invalid.", false);
			}
			else if (!Net.running)
			{
				LogMessage("*OFFLINE*", message, false);
			}
			else if (KrokoshaScavMultiplayer.is_dedicated_server || (force_server_if_server && KrokoshaScavMultiplayer.is_server))
			{
				Server_ChatAnnouncement(in message);
			}
			else
			{
				Client_SendChatMessage(in message);
			}
		}
	}

	public static void Client_SendChatMessage(in string message)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		NetDataWriter writer = Net.CreateWriter(10099);
		writer.Put(false);
		writer.Put(message);
		Net.Client_Send((DeliveryMethod)0, in writer);
	}

	public static void Server_ChatAnnouncement(in string message)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if (KrokoshaScavMultiplayer.is_dedicated_server)
		{
			LogMessage("*SERVER*", message, false);
		}
		if (SHOULD_LOG_CHAT)
		{
			log.l("SERVER: SENDING CHAT ANNOUNCEMENT: " + message.Replace("\r", ""));
		}
		NetDataWriter writer = Net.CreateWriter(10098);
		writer.Put((byte)1);
		writer.Put(message);
		Net.Server_SendToClients((DeliveryMethod)2, in writer, (IEnumerable<knetid>)ServerMain.AllClientIds);
	}

	public static void Server_ChatAnnouncement(string name, string tag, string message)
	{
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		if (KrokoshaScavMultiplayer.is_dedicated_server)
		{
			LogMessage(TagName(name, tag), message, false);
		}
		if (SHOULD_LOG_CHAT)
		{
			log.l("SERVER: SENDING NAMED CHAT ANNOUNCEMENT: NAME:\"" + name + "\" TAG:\"" + tag + "\": " + message);
		}
		NetDataWriter writer = Net.CreateWriter(10098);
		writer.Put((byte)2);
		writer.Put(name);
		writer.Put(tag);
		writer.Put(message);
		Net.Server_SendToClients((DeliveryMethod)0, in writer, (IEnumerable<knetid>)ServerMain.AllClientIds);
	}
}
