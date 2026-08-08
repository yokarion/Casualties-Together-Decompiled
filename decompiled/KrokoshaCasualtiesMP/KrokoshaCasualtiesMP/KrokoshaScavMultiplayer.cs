using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using BepInEx;
using BepInEx.Bootstrap;
using HarmonyLib;
using KrokoshaCasualtiesUtils;
using LiteNetLib;
using LiteNetLib.Utils;
using MonoMod.Utils;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace KrokoshaCasualtiesMP;

public class KrokoshaScavMultiplayer : MonoBehaviour
{
	internal class BurgerkingFootLettuce : MonoBehaviour
	{
		public Light2D l;

		public Color c;
	}

	public delegate void KrokoshaHandleNamedMessageDelegate(knetid senderClientId, ref NetDataReader messagePayload);

	public static ulong GAME_FILES_HASH = 0uL;

	public const string OG_DOWNLOAD_URL = "https://www.nexusmods.com/scavprototype/mods/67";

	public const string OG_MOD_DISCORD_URL = "https://discord.gg/7K6J6bhV8b";

	public const string OG_OFFICIAL_DISCORD_URL = "https://discord.gg/targetplanet";

	public const bool IS_DEBUG_BUILD = false;

	public const bool IS_TESTING_BUILD = false;

	public const bool IS_RELEASE_BUILD = true;

	internal const int CONST_MAX_POSSIBLE_PLAYER_COUNT = 200;

	internal const int MAX_NAME_LENGTH = 16;

	public static string multiplayer_status_message = "Game initialized.";

	public static double last_multiplayer_status_message_change_time = 0.0;

	public static bool showMultiplayerMenu = true;

	public static List<string> SERVER_JOIN_SECRETS = new List<string>();

	public static string CLIENT_JOIN_SECRET = "0";

	public static string INPUT_PASSWORD = "";

	public static bool SERVER_TOGGLE_CHECK_GAME_HASH = true;

	public static bool SERVER_TOGGLE_ENFORCE_MODLIST = false;

	public static bool SERVER_TOGGLE_SHOULD_HOST_DEDICATED = false;

	public static KrokoshaMultiplayerGameRules rules = new KrokoshaMultiplayerGameRules();

	public static bool DEDSERVER_NOVISUALS = false;

	public const string CANT_PLAY_SP_MSG = "You can't play singleplayer with MP mod active.\nTo play singleplayer go to \"MP Mod Menu > Settings > General > Deactivate Multiplayer Mod\"\nOr start an empty server.";

	public static bool DEV_VERBOSE_SHOW_CURSOR_INFO = true;

	private static readonly float OG_dislocationHealSpeed = Limb.dislocationHealSpeed;

	private static readonly float OG_boneHealSpeed = Limb.boneHealSpeed;

	internal static Dictionary<KrokoshaNetworkMessageReceiverAttribute, KrokoshaHandleNamedMessageDelegate> all_network_receivers_from_attributes = new Dictionary<KrokoshaNetworkMessageReceiverAttribute, KrokoshaHandleNamedMessageDelegate>();

	public static bool verbose => DebugMenuSettings.is_verbose;

	internal static byte PLAYER_COUNT_LIMIT
	{
		get
		{
			return rules.PLAYER_COUNT_LIMIT;
		}
		set
		{
			rules.PLAYER_COUNT_LIMIT = value;
		}
	}

	public static string INPUT_IPPORT
	{
		get
		{
			return UIMainMenu.USERINPUT_IPPORT;
		}
		set
		{
			UIMainMenu.USERINPUT_IPPORT = value;
		}
	}

	public static string INPUT_USERNAME
	{
		get
		{
			return UIMainMenu.USERINPUT_NAME;
		}
		set
		{
			UIMainMenu.USERINPUT_NAME = value;
		}
	}

	public static bool is_dedicated_server => Net.is_dedicated_server;

	public static bool network_system_is_running => Net.running;

	public static bool network_system_im_client_and_waiting_connecting => Net.is_connecting;

	public static bool is_client => Net.is_client;

	public static bool is_server => Net.is_server;

	public static string FULL_VERSION_TAG { get; private set; }

	public static event Action OnSceneChangeOrWorldStartGenerate;

	private void OnDestroy()
	{
		try
		{
			try
			{
				if (!Plugin.FORCE_DISABLE_MP_MOD)
				{
					if ((Object)(object)Plugin.s != (Object)null)
					{
						ComponentHolderProtocol.GetOrAddComponent<Plugin.LoadErrorNotifier>((Object)(object)Plugin.s);
					}
					if (string.IsNullOrWhiteSpace(Plugin.LoadErrorNotifier.errorstr))
					{
						Plugin.LoadErrorNotifier.errorstr = "KrokoshaScavMultiplayer Singleton got deleted???";
					}
				}
				log.l("KrokoshaScavMultiplayer.OnDestroy()");
			}
			catch (Exception ex)
			{
				log.error("KrokoshaScavMultiplayer.OnDestroy: " + ex.ToString());
			}
		}
		catch (Exception)
		{
		}
		Application.runInBackground = false;
	}

	private void OnApplicationQuit()
	{
		Application.runInBackground = false;
	}

	public static string Server_GetJoinSecret()
	{
		if (SERVER_JOIN_SECRETS.Count > 0)
		{
			return SERVER_JOIN_SECRETS.Last();
		}
		return "1";
	}

	internal static void _InvokeOnSceneChangeOrWorldStartGenerate()
	{
		KrokoshaScavMultiplayer.OnSceneChangeOrWorldStartGenerate?.Invoke();
	}

	private void Update2s()
	{
		while (SERVER_JOIN_SECRETS.Count < 4)
		{
			SERVER_JOIN_SECRETS.Add(Guid.NewGuid().ToString("N"));
		}
		SERVER_JOIN_SECRETS.Add(Guid.NewGuid().ToString("N"));
		while (SERVER_JOIN_SECRETS.Count > 12)
		{
			SERVER_JOIN_SECRETS.RemoveAt(0);
			SERVER_JOIN_SECRETS.RemoveAt(0);
		}
	}

	private void Start()
	{
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		((MonoBehaviour)this).InvokeRepeating("Update2s", 0.1f, 2f);
		if (!Plugin.plrname_override)
		{
			string text = PlayerPrefs.GetString("KrokoshaMultiplayer_LastPlayerName");
			if (!string.IsNullOrWhiteSpace(text))
			{
				INPUT_USERNAME = text;
			}
			string text2 = PlayerPrefs.GetString("KrokoshaMultiplayer_LastPlayerColor");
			Color val = default(Color);
			if (!string.IsNullOrWhiteSpace(text2) && ColorUtility.TryParseHtmlString(text2, ref val))
			{
				UIMainMenu.SetInputColor((Color24)val);
			}
		}
		if (string.IsNullOrEmpty(Net.MY_SERVER_INFO.name))
		{
			Net.MY_SERVER_INFO.name = "Expedition " + INPUT_USERNAME;
			string text3 = PlayerPrefs.GetString("KrokoshaMultiplayer_LastMyLobbyName");
			if (!string.IsNullOrWhiteSpace(text3))
			{
				Net.MY_SERVER_INFO.name = text3;
			}
		}
		if (!NetPlayer.CheckIfPlrColorIsValid(UIMainMenu.LAST_VALID_INPUT_COLOR))
		{
			log.l("Picked random color cuz its not picked.");
			UIMainMenu.SetInputColor((Color24)NetPlayer.NAMETAG_DEFAULT_COLORS_RANDOMIZED[new Random().Next(0, NetPlayer.NAMETAG_DEFAULT_COLORS_RANDOMIZED.Length)]);
		}
	}

	private void Update()
	{
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		Net.DoUpdate();
		ConsoleScript instance = ConsoleScript.instance;
		if (instance != null && instance.active)
		{
			Con.UpdatePlayerListAutocomplete();
		}
		if (!Con.goof_carameltanzen)
		{
			return;
		}
		Con.coloooooooo_timer += Time.deltaTime;
		if (!(Con.coloooooooo_timer > 0.3f))
		{
			return;
		}
		Con.coloooooooo++;
		Con.coloooooooo_timer = 0f;
		Light2D[] array = Object.FindObjectsOfType<Light2D>();
		BurgerkingFootLettuce burgerkingFootLettuce = default(BurgerkingFootLettuce);
		foreach (Light2D val in array)
		{
			if ((Object)(object)val != (Object)null)
			{
				if (!((Component)val).TryGetComponent<BurgerkingFootLettuce>(ref burgerkingFootLettuce))
				{
					burgerkingFootLettuce = ComponentHolderProtocol.AddComponent<BurgerkingFootLettuce>((Object)(object)val);
					burgerkingFootLettuce.c = val.color;
					burgerkingFootLettuce.l = val;
				}
				burgerkingFootLettuce.l.color = Utils.PickRandom<Color>(Con.colors);
			}
		}
	}

	public static bool AdditionalApprovalCheck(ref NetDataReader reader, string plrname, ref string reject_reason)
	{
		if (reject_reason == null)
		{
			reject_reason = "Connection rejected";
		}
		if (Util.IsInWorld() && !rules.LateJoinAllowed)
		{
			reject_reason = "Game is already in progress.";
			return false;
		}
		return true;
	}

	public static void DoMultiplayerStatusMessageLog(string msg)
	{
		last_multiplayer_status_message_change_time = Time.realtimeSinceStartupAsDouble;
		multiplayer_status_message = msg;
		msg = "Multiplayer: " + msg;
		log.l(msg);
	}

	public static void DoMultiplayerStatusMessageError(string msg)
	{
		last_multiplayer_status_message_change_time = Time.realtimeSinceStartupAsDouble;
		multiplayer_status_message = msg;
		msg = "Multiplayer ERROR: " + msg;
		log.error(msg);
	}

	public static string SanitizeTextInputAllowSpaces(string s)
	{
		return Regex.Replace(s, "[^a-zA-Z0-9_#$()+=\\- ]", "").TrimStart(Array.Empty<char>());
	}

	public static string SanitizeTextInput(string s)
	{
		return Regex.Replace(s, "[^a-zA-Z0-9_#$()+=-]", "");
	}

	public static string SanitizeRichText(string s)
	{
		StringBuilder stringBuilder = new StringBuilder(s.Length);
		bool flag = false;
		foreach (char c in s)
		{
			if (c == '<')
			{
				flag = true;
			}
			else if (flag)
			{
				flag = c != '>';
			}
			else
			{
				stringBuilder.Append(c);
			}
		}
		return stringBuilder.ToString();
	}

	private static void _GUI_RenderMainGUI()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_049c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_040b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_052c: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0584: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a2b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a3a: Expected O, but got Unknown
		//IL_0a49: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a8e: Unknown result type (might be due to invalid IL or missing references)
		//IL_095c: Unknown result type (might be due to invalid IL or missing references)
		//IL_072c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0753: Unknown result type (might be due to invalid IL or missing references)
		//IL_075d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0789: Unknown result type (might be due to invalid IL or missing references)
		//IL_0793: Unknown result type (might be due to invalid IL or missing references)
		//IL_0835: Unknown result type (might be due to invalid IL or missing references)
		//IL_0876: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a5: Unknown result type (might be due to invalid IL or missing references)
		if (Con._DEV_HIDEHUD)
		{
			return;
		}
		if (Input.GetKey((KeyCode)107))
		{
			Input.GetKeyDown((KeyCode)114);
		}
		float num = 20f;
		Utility.IsNullOrWhiteSpace(Application.version);
		GUI.Label(new Rect(20f, num, 1000f, 20f), "CO-OP MOD v4.0.1    \"RELEASE BUILD\" ");
		num += 20f;
		if (is_dedicated_server)
		{
			GUI.Label(new Rect(20f, num, 500f, 20f), "DEDICATED SERVER: SWITCH COUNTER: " + Mathf.Round(ServerMain._ded_server_switch_counter));
			num += 20f;
		}
		if (verbose)
		{
			GUI.Label(new Rect(20f, num, 500f, 20f), "FPS: " + ServerMain.CURRENT_FPS + "   TPS: " + ServerMain.CURRENT_TPS);
			num += 20f;
			GUI.Label(new Rect(20f, num, 500f, 20f), "UTC: " + DateTime.UtcNow.ToString("HH:mm:ss.fff"));
			num += 20f;
		}
		if (verbose && DEV_VERBOSE_SHOW_CURSOR_INFO)
		{
			float num2 = (float)Screen.height - Input.mousePosition.y + 20f;
			if (Util.TryGetLocalBody(out var _))
			{
				if (Minigame.op_Implicit(MinigameBase.main?.currentMinigame))
				{
					GUI.Label(new Rect(Input.mousePosition.x, num2, 400f, 20f), "mg handpos: " + ((object)Unsafe.As<Vector2, Vector2>(ref MinigameBase.main.handPos)/*cast due to constrained. prefix*/).ToString());
					num2 += 20f;
					GUI.Label(new Rect(Input.mousePosition.x, num2, 400f, 20f), "mg sprite : " + MinigameMPManager.GetLocalHandSpriteIndex());
					num2 += 20f;
					GUI.Label(new Rect(Input.mousePosition.x, num2, 400f, 20f), "mg lmb : " + MinigameBase.main.handClicking);
					num2 += 20f;
				}
				else
				{
					GUI.Label(new Rect(Input.mousePosition.x, num2, 400f, 20f), "dist: " + Vector2.Distance(Vector2.op_Implicit(((Component)PlayerCamera.main.body).transform.position), Vector2.op_Implicit(Camera.main.ScreenToWorldPoint(Input.mousePosition))));
					num2 += 20f;
					GUI.Label(new Rect(Input.mousePosition.x, num2, 400f, 20f), "pos: " + ((object)Vector2.op_Implicit(Camera.main.ScreenToWorldPoint(Input.mousePosition))/*cast due to constrained. prefix*/).ToString());
					num2 += 20f;
					GUI.Label(new Rect(Input.mousePosition.x, num2, 400f, 20f), "vel: " + ((object)PlayerCamera.main.body.rb.velocity/*cast due to constrained. prefix*/).ToString());
					num2 += 20f;
				}
			}
			GUI.Label(new Rect(Input.mousePosition.x, num2, 400f, 20f), "time: " + Math.Round(Time.realtimeSinceStartupAsDouble));
			num2 += 20f;
		}
		if (Util.TryGetLocalBody(out var _) && verbose)
		{
			Vector2 val = Vector2.op_Implicit(((Component)PlayerCamera.main.body).transform.position);
			if (UIInGame.SPECTATOR_MODE)
			{
				val = Vector2.op_Implicit(((Component)PlayerCamera.main).transform.position);
			}
			GUI.Label(new Rect(20f, num, 500f, 20f), $"Position: x:{(int)val.x} y:{(int)val.y}");
			num += 20f;
			GUI.Label(new Rect(20f, num, 500f, 20f), "REGISTERED OBJECTS COUNT: " + NetObjectRegistry.SyncRegistry.Count);
			num += 20f;
		}
		if (ConsoleScript.instance.noClip)
		{
			GUI.Label(new Rect(20f, num, 200f, 20f), "NOCLIP ACTIVE");
			num += 20f;
		}
		if (Con._DEV_GODMODE)
		{
			GUI.Label(new Rect(20f, num, 200f, 20f), "GODMODE ACTIVE");
			num += 20f;
		}
		if (UIInGame.SPECTATOR_MODE)
		{
			GUI.Label(new Rect(20f, num, 400f, 20f), "Spectator mode active ");
			num += 20f;
		}
		if ((Object)(object)UIInGame.interaction_menu_target_body != (Object)null)
		{
			GUI.Label(new Rect(20f, num, 400f, 20f), "Player interaction menu active " + ((object)UIInGame.interaction_menu_target_body).ToString());
			num += 20f;
		}
		if (verbose)
		{
			if (Util.IsInWoundView() && !Util.IsBodyLocal(WoundView.view.body))
			{
				GUI.Label(new Rect(20f, num, 400f, 20f), "Viewing health panel of player: " + ((Component)WoundView.view.body).GetComponent<NetBody>().bodyname);
				num += 20f;
			}
			if (MinigameMPManager.active_sessions.Count > 0)
			{
				GUI.Label(new Rect(20f, num, 400f, 20f), "MINIGAME SESSIONS: ");
				num += 20f;
				foreach (MinigameMPManager.MinigameSession active_session in MinigameMPManager.active_sessions)
				{
					string text = "";
					foreach (NetPlayer involved_player in active_session.involved_players)
					{
						text = text + involved_player.playername + " " + involved_player.minigame_is_in_a_minigame + " " + active_session.involved_players.Contains(involved_player) + " " + involved_player.minigame_session + " " + involved_player.minigame_current_type + " | ";
					}
				}
			}
			if (Minigame.game?.currentMinigame != null)
			{
				if (Minigame.game.currentMinigame is DislocationMinigame)
				{
					GUI.Label(new Rect(20f, num, 1000f, 20f), "DISLOCATION MINIGAME");
					num += 20f;
					GUI.Label(new Rect(20f, num, 1000f, 20f), $"BONE POS: {DislocationMinigame_CheckForHit_MultiplayerPatch.real_bonepos}");
					num += 20f;
					GUI.Label(new Rect(20f, num, 1000f, 20f), $"BONE VEL: {DislocationMinigame_CheckForHit_MultiplayerPatch.boneVelocity}");
					num += 20f;
				}
				else
				{
					Minigame currentMinigame = Minigame.game.currentMinigame;
					LockpingMinigame val2 = (LockpingMinigame)(object)((currentMinigame is LockpingMinigame) ? currentMinigame : null);
					if (val2 != null)
					{
						GUI.Label(new Rect(20f, num, 500f, 20f), "LOCKPICKING MINIGAME");
						num += 20f;
						GUI.Label(new Rect(20f, num, 500f, 20f), $"HAND ANGLE: {val2.HandAngle}");
						num += 20f;
					}
				}
				GUI.Label(new Rect(20f, num, 400f, 20f), "HAND VEL: " + ((object)Unsafe.As<Vector2, Vector2>(ref MinigameBase.main.handVelocity)/*cast due to constrained. prefix*/).ToString());
				num += 20f;
				GUI.Label(new Rect(20f, num, 400f, 20f), "HAND VEL MAGN: " + (int)((Vector2)(ref MinigameBase.main.handVelocity)).magnitude);
				num += 20f;
			}
		}
		if (Util.IsInPauseMenuOrMainMenu() || is_dedicated_server)
		{
			new Vector2(Input.mousePosition.x, (float)Screen.height - Input.mousePosition.y);
			num += 10f;
		}
		else if (!network_system_is_running)
		{
			GUI.Label(new Rect(20f, num, 1280f, 40f), Lang.Get("singleplayer_warn", false));
			num += 40f;
		}
		else
		{
			string text2 = (Net.is_host ? "Host" : (Net.is_dedicated_server ? "Server" : "Client"));
			GUI.Label(new Rect(20f, num, 200f, 20f), "Mode: " + text2);
			num += 20f;
			if (!network_system_im_client_and_waiting_connecting && (Object)(object)NetPlayer.LOCAL_PLAYER != (Object)null && is_client)
			{
				GUI.Label(new Rect(20f, num, 200f, 20f), "Ping: " + ClientMain.LOCAL_PING);
				num += 20f;
			}
		}
		if (0 == 0 && (Time.realtimeSinceStartupAsDouble - last_multiplayer_status_message_change_time < 10.0 || !Util.IsInWorld()))
		{
			bool wordWrap = GUI.skin.label.wordWrap;
			GUI.skin.label.wordWrap = true;
			string text3 = "Last Status Message:\n" + multiplayer_status_message;
			float num3 = GUI.skin.label.CalcHeight(new GUIContent(text3), 400f);
			GUI.Label(new Rect(20f, num, 400f, num3), text3);
			num += num3;
			GUI.skin.label.wordWrap = wordWrap;
		}
		if (Voicechat.my_output_volume > 0f)
		{
			if (UIBullshit.IsAnyMenuOpen())
			{
				GUI.Label(new Rect(20f, num, 300f, 20f), "Voicechat current volume level: " + Voicechat.my_output_volume.ToString("F2"));
				num += 20f;
			}
			else
			{
				GUI.Label(new Rect(20f, num, 200f, 20f), "Voicechat active.");
				num += 20f;
			}
		}
	}

	private void OnGUI()
	{
		if (Con.IsConsoleOpen())
		{
			return;
		}
		try
		{
			_GUI_RenderMainGUI();
		}
		catch (Exception ex)
		{
			log.error(ex.ToString());
		}
	}

	public static NetDataWriter CreateClientConnectIntroductionPacket(NetDataWriter writer = null)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		object obj = ((object)writer) ?? ((object)new NetDataWriter());
		((NetDataWriter)obj).Put(GAME_FILES_HASH);
		((NetDataWriter)obj).Put(FULL_VERSION_TAG, oneByteChars: true);
		((NetDataWriter)obj).Put(INPUT_USERNAME, oneByteChars: true);
		int length = ((NetDataWriter)obj).Length;
		((NetDataWriter)obj).Put(INPUT_PASSWORD, oneByteChars: true);
		((NetDataWriter)obj).Put(UIMainMenu.LAST_VALID_INPUT_COLOR);
		((NetDataWriter)obj).Put(CLIENT_JOIN_SECRET, oneByteChars: true);
		((NetDataWriter)obj).PutArray(GetModListGUIDs());
		((NetDataWriter)obj).CompressWriter(length);
		return (NetDataWriter)obj;
	}

	public static bool ReadClientConnectIntroductionPacket(NetDataReader credentials_buffer, out ulong user_hash, out string version, out string username, out string password, out string secret, out Color24 color, out string[] modlist)
	{
		user_hash = 0uL;
		version = "";
		username = "NONAME";
		secret = "0";
		modlist = null;
		bool result = true;
		try
		{
			credentials_buffer.Get(ref user_hash);
			credentials_buffer.Get(out version, oneByteChars: true);
			credentials_buffer.Get(out username, oneByteChars: true);
			credentials_buffer = credentials_buffer.DecompressReader();
			credentials_buffer.Get(out password, oneByteChars: true);
			credentials_buffer.Get(out color);
			if (!credentials_buffer.EndOfData)
			{
				credentials_buffer.Get(out secret, oneByteChars: true);
				credentials_buffer.TryGetStringArray(ref modlist);
			}
		}
		catch (Exception)
		{
			password = null;
			color = Color24.black;
			result = false;
		}
		return result;
	}

	public static bool ValidateClientConnectIntroductionPacket(NetDataReader reader, out string player_name, out Color24 requested_color, out string deny_reason, bool check_name = true, bool user_is_a_dev__basically_priveleged = false)
	{
		player_name = "EMPTY_STRING";
		requested_color = Color24.black;
		deny_reason = "Server broke or some shi ig.";
		try
		{
			if (!ReadClientConnectIntroductionPacket(reader, out var user_hash, out var version, out player_name, out var password, out var secret, out requested_color, out var modlist))
			{
				deny_reason = "Corrupt payload.";
				return false;
			}
			if (!user_is_a_dev__basically_priveleged)
			{
				if (version != FULL_VERSION_TAG || modlist == null)
				{
					deny_reason = "Version mismatch.";
					return false;
				}
				if (SERVER_TOGGLE_CHECK_GAME_HASH && GAME_FILES_HASH != user_hash)
				{
					deny_reason = "Game file Hashes mismatch.";
					return false;
				}
			}
			if (check_name)
			{
				if (player_name.Length <= 2)
				{
					deny_reason = "Name too short.";
					return false;
				}
				if (player_name != SanitizeTextInput(player_name))
				{
					deny_reason = "Corrupt payload.";
					return false;
				}
			}
			if (!user_is_a_dev__basically_priveleged)
			{
				if (SERVER_TOGGLE_ENFORCE_MODLIST && !user_is_a_dev__basically_priveleged && !new HashSet<string>(GetModListGUIDs()).SetEquals(modlist))
				{
					deny_reason = "This server requires different set of mods.";
					return false;
				}
				if (SERVER_JOIN_SECRETS.Contains(secret))
				{
					SERVER_JOIN_SECRETS.Remove(secret);
					SERVER_JOIN_SECRETS.Insert(0, secret);
				}
				else if (!string.IsNullOrEmpty(INPUT_PASSWORD) && password != INPUT_PASSWORD)
				{
					deny_reason = "Wrong password.";
					return false;
				}
				if (!NetPlayer.CheckIfPlrColorIsValid(requested_color))
				{
					deny_reason = "Invalid color.";
					return false;
				}
			}
			deny_reason = "Success";
			return true;
		}
		catch
		{
			deny_reason = "Corrupt payload.";
			return false;
		}
	}

	public static void Client_SendSimpleMessageToServer(in ushort name, bool reliable = true)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		NetDataWriter writer = Net.CreateWriter(name);
		writer.Put(true);
		Net.Client_Send((DeliveryMethod)((!reliable) ? 4 : 0), in writer);
	}

	public static void Client_SendSimpleMessageToServer(in ushort name, bool data, bool reliable = true)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		NetDataWriter writer = Net.CreateWriter(name);
		writer.Put(data);
		Net.Client_Send((DeliveryMethod)(reliable ? 2 : 4), in writer);
	}

	public static void Client_SendSimpleMessageToServer(in ushort name, string data, bool reliable = true)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		NetDataWriter writer = Net.CreateWriter(name);
		writer.Put(data, oneByteChars: true);
		Net.Client_Send((DeliveryMethod)((!reliable) ? 4 : 0), in writer);
	}

	public static void Client_SendSimpleMessageToServer(in ushort name, ushort data, bool reliable = true)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		NetDataWriter writer = Net.CreateWriter(name);
		writer.Put(data);
		Net.Client_Send((DeliveryMethod)(reliable ? 2 : 4), in writer);
	}

	public static void Client_SendSimpleMessageToServer(in ushort name, byte data, bool reliable = true)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		NetDataWriter writer = Net.CreateWriter(name);
		writer.Put(data);
		Net.Client_Send((DeliveryMethod)((!reliable) ? 4 : 0), in writer);
	}

	public static void Client_SendSimpleMessageToServer(in ushort name, Vector2 data, bool reliable = true)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		NetDataWriter writer = Net.CreateWriter(name);
		writer.Put(data);
		Net.Client_Send((DeliveryMethod)((!reliable) ? 4 : 0), in writer);
	}

	public static void Client_SendSimpleMessageToServer(in ushort name, byte data1, Vector2 data, bool reliable = true)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		NetDataWriter writer = Net.CreateWriter(name);
		writer.Put(data1);
		writer.Put(data);
		Net.Client_Send((DeliveryMethod)((!reliable) ? 4 : 0), in writer);
	}

	public static void Client_SendSimpleMessageToServer(in ushort name, Vector2 data, Vector2 data2, bool reliable = true)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		NetDataWriter writer = Net.CreateWriter(name);
		writer.Put(data);
		writer.Put(data2);
		Net.Client_Send((DeliveryMethod)((!reliable) ? 4 : 0), in writer);
	}

	public static void Client_SendSimpleMessageToServer(in ushort name, ushort data, ushort data2, bool reliable = true)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		NetDataWriter writer = Net.CreateWriter(name);
		writer.Put(data);
		writer.Put(data2);
		Net.Client_Send((DeliveryMethod)((!reliable) ? 4 : 0), in writer);
	}

	public static void Client_SendSimpleMessageToServer(in ushort name, ushort data, byte data2, bool reliable = true)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		NetDataWriter writer = Net.CreateWriter(name);
		writer.Put(data);
		writer.Put(data2);
		Net.Client_Send((DeliveryMethod)((!reliable) ? 4 : 0), in writer);
	}

	public static void Client_SendSimpleMessageToServer(in ushort name, ushort data, ushort data2, byte data3, bool reliable = true)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		NetDataWriter writer = Net.CreateWriter(name);
		writer.Put(data);
		writer.Put(data2);
		writer.Put(data3);
		Net.Client_Send((DeliveryMethod)((!reliable) ? 4 : 0), in writer);
	}

	public static void Client_SendSimpleMessageToServer(in ushort name, byte data, ushort data2, byte data3, bool reliable = true)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		NetDataWriter writer = Net.CreateWriter(name);
		writer.Put(data);
		writer.Put(data2);
		writer.Put(data3);
		Net.Client_Send((DeliveryMethod)((!reliable) ? 4 : 0), in writer);
	}

	public static void Client_SendSimpleMessageToServer(in ushort name, ushort data, float data2, byte data3, bool reliable = true)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		NetDataWriter writer = Net.CreateWriter(name);
		writer.Put(data);
		writer.Put(data2);
		writer.Put(data3);
		Net.Client_Send((DeliveryMethod)((!reliable) ? 4 : 0), in writer);
	}

	public static void Client_SendSimpleMessageToServer(in ushort name, ushort data, float data2, bool reliable = true)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		NetDataWriter writer = Net.CreateWriter(name);
		writer.Put(data);
		writer.Put(data2);
		Net.Client_Send((DeliveryMethod)((!reliable) ? 4 : 0), in writer);
	}

	public static void Client_SendSimpleMessageToServer(in ushort name, ushort data, byte data2, byte data3, bool reliable = true)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		NetDataWriter writer = Net.CreateWriter(name);
		writer.Put(data);
		writer.Put(data2);
		writer.Put(data3);
		Net.Client_Send((DeliveryMethod)((!reliable) ? 4 : 0), in writer);
	}

	public static void Server_SendRelayMessageToClients(in ushort name, ushort any_id, bool reliable = true, bool includehost = false)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		NetDataWriter writer = Net.CreateWriter(name);
		writer.Put(any_id);
		Net.Server_SendToClients((DeliveryMethod)(reliable ? 2 : 4), in writer, (IEnumerable<knetid>)(includehost ? ServerMain.AllClientIds : ServerMain.AllClientIdsExceptHost));
	}

	public static void Server_SendRelayMessageToClients(in ushort name, ushort any_id, ushort data, bool reliable = true, bool includehost = false)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		NetDataWriter writer = Net.CreateWriter(name);
		writer.Put(any_id);
		writer.Put(data);
		Net.Server_SendToClients((DeliveryMethod)((!reliable) ? 4 : 0), in writer, (IEnumerable<knetid>)(includehost ? ServerMain.AllClientIds : ServerMain.AllClientIdsExceptHost));
	}

	public static void Server_SendRelayMessageToClients(in ushort name, ushort any_id, Vector2 data, bool reliable = true, bool includehost = false)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		NetDataWriter writer = Net.CreateWriter(name);
		writer.Put(any_id);
		writer.Put(data);
		Net.Server_SendToClients((DeliveryMethod)((!reliable) ? 4 : 0), in writer, (IEnumerable<knetid>)(includehost ? ServerMain.AllClientIds : ServerMain.AllClientIdsExceptHost));
	}

	public static void Server_SendRelayMessageToClients(in ushort name, ushort any_id, Vector2 data, Vector2 data2, bool reliable = true, bool includehost = false)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		NetDataWriter writer = Net.CreateWriter(name);
		writer.Put(any_id);
		writer.Put(data);
		writer.Put(data2);
		Net.Server_SendToClients((DeliveryMethod)((!reliable) ? 4 : 0), in writer, (IEnumerable<knetid>)(includehost ? ServerMain.AllClientIds : ServerMain.AllClientIdsExceptHost));
	}

	public static void Server_SendSimpleMessageToOneClient(in NetmsgId name, ushort clientId, bool reliable = true)
	{
		ushort name2 = (ushort)name;
		Server_SendSimpleMessageToOneClient(in name2, clientId, reliable);
	}

	public static void Server_SendSimpleMessageToOneClient(in ushort name, ushort clientId, bool reliable = true)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		NetDataWriter writer = Net.CreateWriter(name);
		writer.Put(true);
		Net.Server_SendToClients((DeliveryMethod)(reliable ? 2 : 4), in writer, (knetid)clientId);
	}

	public static void Server_SendSimpleMessageToOneClient(in ushort name, ushort clientId, ushort data, bool reliable = true)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		NetDataWriter writer = Net.CreateWriter(name);
		writer.Put(data);
		Net.Server_SendToClients((DeliveryMethod)((!reliable) ? 4 : 0), in writer, (knetid)clientId);
	}

	public static void Server_SendSimpleMessageToOneClient(in ushort name, ushort clientId, float data, bool reliable = true)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		NetDataWriter writer = Net.CreateWriter(name);
		writer.Put(data);
		Net.Server_SendToClients((DeliveryMethod)((!reliable) ? 4 : 0), in writer, (knetid)clientId);
	}

	public static void Server_SendSimpleMessageToOneClient(in ushort name, ushort clientId, Vector2 data, bool reliable = true)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		NetDataWriter writer = Net.CreateWriter(name);
		writer.Put(data);
		Net.Server_SendToClients((DeliveryMethod)((!reliable) ? 4 : 0), in writer, (knetid)clientId);
	}

	public static void Server_SendSimpleMessageToOneClient(in ushort name, ushort clientId, string data, bool reliable = true)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		NetDataWriter writer = Net.CreateWriter(name);
		writer.Put(data, oneByteChars: true);
		Net.Server_SendToClients((DeliveryMethod)((!reliable) ? 4 : 0), in writer, (knetid)clientId);
	}

	public static void ShutdownNetwork()
	{
		UIInGame.StopSpectatorMode();
		GoBackToMainMenuIfImClientAndInGame();
		Net.ShutdownReset();
		Application.runInBackground = false;
	}

	public static void _JustDisconnect()
	{
		UIInGame.StopSpectatorMode();
		Net.ShutdownReset();
		Application.runInBackground = false;
	}

	public static void ApplyGameRules()
	{
		Limb.dislocationHealSpeed = OG_dislocationHealSpeed + OG_dislocationHealSpeed * rules.AdditionalHealthRegen;
		Limb.boneHealSpeed = OG_boneHealSpeed + OG_boneHealSpeed * rules.AdditionalHealthRegen;
		FieldInfo[] fields = typeof(KrokoshaMultiplayerGameRules).GetFields();
		foreach (FieldInfo fieldInfo in fields)
		{
			object value = fieldInfo.GetValue(rules);
			if (AttributeUtility.HasAttribute((MemberInfo)fieldInfo, typeof(KrokoshaRuleByteAttribute), true))
			{
				KrokoshaRuleByteAttribute attribute = AttributeUtility.GetAttribute<KrokoshaRuleByteAttribute>((MemberInfo)fieldInfo, true);
				TypedReference obj = __makeref(rules);
				byte b = (byte)value;
				if (b > attribute.limit)
				{
					b = attribute.limit;
					fieldInfo.SetValueDirect(obj, b);
				}
			}
		}
		if (rules.VoicechatQuality >= Voicechat.STANDART_FREQUENCIES.Length || rules.VoicechatQuality < 0)
		{
			rules.VoicechatQuality = (byte)Voicechat.STANDART_FREQUENCIES.Length;
		}
		Voicechat.SAMPLE_RATE = Voicechat.STANDART_FREQUENCIES[rules.VoicechatQuality];
		if (Util.IsInWorld())
		{
			try
			{
				TextMeshProUGUI timescaleText = PlayerCamera.main.timescaleText;
				if ((Object)(object)timescaleText != (Object)null)
				{
					((Component)((TMP_Text)timescaleText).transform.parent).gameObject.SetActive(rules.EnableTimeManipulation);
				}
			}
			catch (Exception ex)
			{
				log.error("TimeManipulation UI switch ERROR: " + ex.ToString());
			}
		}
		Plugin.s.SendMessage("KrokoshaSingletonEvent_OnRulesUpdate", (SendMessageOptions)1);
	}

	public static bool IsInGameAndWorldGenerated()
	{
		if (Object.op_Implicit((Object)(object)WorldGeneration.world) && !WorldGeneration.world.generatingWorld)
		{
			return true;
		}
		return false;
	}

	public static bool IsNetworkActiveAndIsServer()
	{
		if (network_system_is_running)
		{
			return is_server;
		}
		return false;
	}

	public static bool IsNetworkActiveAndIsHost()
	{
		if (network_system_is_running && !is_client)
		{
			return !is_dedicated_server;
		}
		return false;
	}

	public static bool IsNetworkActiveAndIsClient()
	{
		if (network_system_is_running)
		{
			return is_client;
		}
		return false;
	}

	public static bool IsNetworkActiveOrIsInGame()
	{
		if (!network_system_is_running)
		{
			return Util.IsInWorld();
		}
		return true;
	}

	public static bool IsNetworkActiveAndIsInGame()
	{
		if (network_system_is_running)
		{
			return Util.IsInWorld();
		}
		return false;
	}

	public static bool IsNetworkActiveAndIsWorldGenerated()
	{
		if (network_system_is_running)
		{
			return Util.IsWorldGenerated();
		}
		return false;
	}

	public static bool IsNetworkActiveAndIsClientAndIsInGame()
	{
		if (network_system_is_running && is_client)
		{
			return Util.IsInWorld();
		}
		return false;
	}

	public static void GoBackToMainMenuIfImClientAndInGame()
	{
		if (is_client && Util.IsInWorld())
		{
			DoMultiplayerStatusMessageLog("Went back to main menu because disconnected.");
			showMultiplayerMenu = true;
			PlayerCamera.main.ToMainMenu();
		}
	}

	public static void ResetNetworkManager()
	{
		GoBackToMainMenuIfImClientAndInGame();
		Net.ShutdownReset();
	}

	public static List<PluginInfo> GetModList()
	{
		List<PluginInfo> list = Chainloader.PluginInfos.Values.ToList();
		string[] toremove_guids = new string[2] { "KrokoshaCasualtiesMP", "dannad.krmultiupdater" };
		list.RemoveAll((PluginInfo x) => x.Metadata == null || toremove_guids.Contains(x.Metadata.GUID));
		return list;
	}

	public static string[] GetModListGUIDs()
	{
		return (from x in GetModList()
			select x.Metadata.GUID).ToArray();
	}

	internal static void _FIRST_INIT()
	{
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_038f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		FULL_VERSION_TAG = Application.version + "_MPv4.0.1";
		try
		{
			foreach (KeyValuePair<string, KeyCode> item in CoopKeybinds.as_dict)
			{
				Util.RegisterKeybind(item.Key, item.Value);
			}
		}
		catch (Exception ex)
		{
			log.error("FirstInit -> RegisterKeybind\n" + ex.ToString());
		}
		Color[] array = (Color[])(object)new Color[30]
		{
			new Color(1f, 0.5f, 0f),
			new Color(1f, 0.4f, 0f),
			new Color(1f, 0.6f, 0.2f),
			new Color(0.5f, 0f, 1f),
			new Color(0.6f, 0.2f, 0.8f),
			new Color(0.4f, 0f, 0.6f),
			new Color(1f, 0.4f, 0.7f),
			new Color(1f, 0.6f, 0.8f),
			new Color(0.9f, 0.3f, 0.5f),
			new Color(0.5f, 1f, 0.5f),
			new Color(0.2f, 0.8f, 0.2f),
			new Color(0.1f, 0.5f, 0.1f),
			new Color(0.3f, 0.7f, 0.3f),
			new Color(0.4f, 0.7f, 1f),
			new Color(0.2f, 0.4f, 0.8f),
			new Color(0.5f, 0.8f, 1f),
			new Color(0.1f, 0.3f, 0.6f),
			new Color(0f, 0.5f, 0.5f),
			new Color(0f, 0.7f, 0.7f),
			new Color(1f, 0.9f, 0.3f),
			new Color(1f, 0.8f, 0f),
			new Color(0.9f, 0.7f, 0.1f),
			new Color(0.6f, 0.3f, 0f),
			new Color(0.4f, 0.2f, 0.1f),
			new Color(0.7f, 0.5f, 0.3f),
			new Color(0f, 1f, 0.6f),
			new Color(1f, 0f, 0.6f),
			new Color(0.6f, 0f, 0f),
			new Color(0.8f, 0.2f, 0.2f),
			new Color(0.6f, 0.8f, 0.2f)
		};
		NetPlayer.NAMETAG_DEFAULT_COLORS = CollectionExtensions.AddRangeToArray<Color>(NetPlayer.NAMETAG_DEFAULT_COLORS, array);
		array.ShuffleRandom();
		NetPlayer.NAMETAG_DEFAULT_COLORS_RANDOMIZED = CollectionExtensions.AddRangeToArray<Color>(NetPlayer.NAMETAG_DEFAULT_COLORS_RANDOMIZED, array);
		try
		{
			if (!Util.IsInWorld())
			{
				KrokoshaMainmenuBackground.CreateBackground();
			}
		}
		catch (Exception ex2)
		{
			log.error("FirstInit -> KrokoshaMainmenuBackground.CreateBackground\n" + ex2.ToString());
		}
		if ((Object)(object)CoUtils_instance_MultiplayerPatch.original_local_instance == (Object)null)
		{
			CoUtils.instance.DurationOf("calling_this_just_to_trigger_the_getter");
		}
		OnSceneChangeOrWorldStartGenerate += delegate
		{
			try
			{
				if (Util.IsInWorld())
				{
					if ((Object)(object)WorldGeneration.world != (Object)null)
					{
						log.l($"SCENE CHANGE: Is In World,  Layer: {WorldGeneration.world.biomeDepth}");
					}
					else
					{
						log.l("SCENE CHANGE: Is In World,  No world?");
					}
				}
				else
				{
					log.l("SCENE CHANGE: Is In Main Menu ");
				}
			}
			catch (Exception ex6)
			{
				log.error("OnSceneChangeOrWorldStartGenerate Scene log: " + ex6.ToString());
			}
			foreach (KeyValuePair<knetid, NetPlayer> item2 in NetPlayer.ClientIdToPlayerDict)
			{
				if (item2.Value.server_plrstate != null)
				{
					item2.Value.server_plrstate.OnSceneChangeOrWorldStartGenerate();
				}
				item2.Value.late_joined = false;
			}
			RadiationLineUpdatePatch.timeGone = 0f;
			ServerMain._ded_server_switch_counter = -20f;
			if ((Object)(object)MoodleManager.main != (Object)null)
			{
				MoodleManager.main.SetBody(Util.GetLocalBody());
			}
			if (Net.TRANSPORT != null && Net.TRANSPORT is TransportLiteNetLib transportLiteNetLib)
			{
				transportLiteNetLib.netmgr.Statistics.Reset();
			}
		};
		SceneManager.sceneLoaded += OnSceneLoaded;
		SceneManager.sceneUnloaded += OnSceneUnLoaded;
		Plugin.s.AddComponent<DebugHelp>();
		Plugin.s.AddComponent<SharedMain>();
		Plugin.s.AddComponent<ClientMain>();
		Plugin.s.AddComponent<ServerMain>();
		foreach (Type item3 in TypeUtility.GetTypesSafely(Assembly.GetAssembly(typeof(KrokoshaScavSingleton))))
		{
			try
			{
				UIMainMenu.GetSettingsFromStaticClass(item3);
				if (item3.IsClass && !item3.IsAbstract && item3.IsSubclassOf(typeof(KrokoshaScavSingleton)))
				{
					Plugin.s.AddComponent(item3);
				}
				MethodInfo[] methods = item3.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				foreach (MethodInfo methodInfo in methods)
				{
					object[] customAttributes = methodInfo.GetCustomAttributes(typeof(KrokoshaNetworkMessageReceiverAttribute), inherit: true);
					if (customAttributes.Length != 0)
					{
						try
						{
							KrokoshaNetworkMessageReceiverAttribute key = (KrokoshaNetworkMessageReceiverAttribute)customAttributes[0];
							all_network_receivers_from_attributes.Add(key, Extensions.CreateDelegate<KrokoshaHandleNamedMessageDelegate>((MethodBase)methodInfo));
						}
						catch (Exception ex3)
						{
							log.error($"FirstInit -> method scanning: ADDING A RECEIVER : {methodInfo}: \n{ex3.ToString()}");
						}
					}
				}
			}
			catch (Exception ex4)
			{
				log.error($"FirstInit -> class scanning loop: TYPE:{item3} \n{ex4.ToString()}");
			}
		}
		if (!win32windowtitlechanger.changed_the_title)
		{
			win32windowtitlechanger.SetTitle(_PickGameWindowTitle());
		}
		try
		{
			string text = Path.Combine(Application.dataPath, "Managed/Assembly-CSharp.dll");
			if (File.Exists(text))
			{
				GAME_FILES_HASH = Util.GetFileHash(text);
			}
			else
			{
				DoMultiplayerStatusMessageError("GAME HASH CALCULATION: Assembly-CSharp.dll DOES NOT EXIST????");
			}
			string location = Assembly.GetExecutingAssembly().Location;
			if (File.Exists(location))
			{
				GAME_FILES_HASH += Util.GetFileHash(location);
			}
			else
			{
				DoMultiplayerStatusMessageError("GAME HASH CALCULATION: KrokoshaCasualtiesMP.dll DOES NOT EXIST????");
			}
		}
		catch (Exception ex5)
		{
			log.error("GAME HASH CALCULATION: " + ex5.ToString());
		}
	}

	public static void OnSceneLoaded(Scene a, LoadSceneMode b)
	{
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Expected O, but got Unknown
		ServerMain._ded_server_switch_counter = -10f;
		WoundView_UpdateView_MultiplayerPatch.OG_showInfection = null;
		ServerMain.server_lastplayerstates.Clear();
		bool flag = ((Scene)(ref a)).name == "SampleScene";
		if (network_system_is_running)
		{
			if (!Net.is_playing_with_steam)
			{
				Plugin.log.LogInfo((object)("Local player username: " + INPUT_USERNAME));
			}
			else
			{
				Plugin.log.LogInfo((object)("Local player username: " + KSteam.GetLocalUsername()));
			}
			try
			{
				Traverse.Create(typeof(PreRunScript)).Field("didIntro").SetValue((object)true);
				GameObject val = (GameObject)Traverse.Create(typeof(ScrollableText)).Field("textObj").GetValue();
				if ((Object)(object)val != (Object)null)
				{
					Object.Destroy((Object)(object)val.gameObject);
				}
			}
			catch (Exception ex)
			{
				log.error("remove the intro yapping -> \n" + ex.ToString());
			}
			if ((Object)(object)PlayerCamera.main?.body != (Object)null && !is_dedicated_server)
			{
				NetPlayer.BodyToPlayerDict[PlayerCamera.main.body] = NetPlayer.LOCAL_PLAYER;
			}
			if (flag)
			{
				try
				{
					ComponentHolderProtocol.GetOrAddComponent<LoadingScreenSpecimenDuplicator>((Object)(object)WorldGeneration.world.loadingObject);
				}
				catch (Exception ex2)
				{
					log.error("LoadingScreenCreatureDuplicator -> \n" + ex2.ToString());
				}
				ClientMain._InitializePlayerCharacter();
			}
		}
		if (flag)
		{
			if (is_dedicated_server)
			{
				UIInGame.StartSpectatorMode();
			}
			else
			{
				ComponentHolderProtocol.GetOrAddComponent<NetBody>((Object)(object)PlayerCamera.main.body).BackupLocalPlayerInit();
				UIMainMenu.SetOpen(open_or_nah: false);
			}
		}
		else
		{
			try
			{
				KrokoshaMainmenuBackground.CreateBackground();
			}
			catch (Exception ex3)
			{
				log.error("OnSceneLoaded -> KrokoshaMainmenuBackground.CreateBackground\n" + ex3.ToString());
			}
			WorldgenPatches.ResetWorldParameters();
		}
		_InvokeOnSceneChangeOrWorldStartGenerate();
	}

	public static void OnSceneUnLoaded(Scene a)
	{
		if (NewCoolerObjectPacketWriteReadSystem.inst != null)
		{
			NewCoolerObjectPacketWriteReadSystem.inst.UnregisterEVERYTHING();
		}
	}

	private static string _PickGameWindowTitle()
	{
		Random r = new Random();
		Func<string[], string> func = (string[] x) => x[r.Next(0, x.Length)];
		string text = Application.productName;
		if (r.NextDouble() > 0.20000000298023224)
		{
			text = "Casualties: Unknown";
			if (r.NextDouble() > 0.5)
			{
				text = "Casualties Unknown";
				if (r.NextDouble() > 0.20000000298023224)
				{
					text = func(new string[11]
					{
						"Casualties: Multiple", "Casualties: Multiple", "Casualties: Together", "Casualties: Many", "Casualties: Manyplayer", "Casualties: Multiplayer", "Casualties: Market Pliers", "Casualties: Multi-Pliers", "Casualties: Some", "Casualties: Some Amount",
						"Casualties: Co-op"
					});
				}
			}
		}
		if (r.NextDouble() > 0.3)
		{
			text = text + ": " + func(new string[20]
			{
				"Multiplayer", "Multiplied", "Co-op", "bluetooth support", "communism", "Manyplayer", "Die with friends", "death in poverty", "overdose with friends !!!", "fentanyl with friends !!!",
				"yipeee!!!  *falls off a cliff*", "furry suffering", "Just add multiplayer", "Krokosha's Multiplayer mod", "PEAK for masochists", "PEAK, but you go down", "thats a weird terraria", "Now with MULTIPLAYER", "The SALAD", "Cooperation behavioral research"
			});
		}
		return text;
	}
}
