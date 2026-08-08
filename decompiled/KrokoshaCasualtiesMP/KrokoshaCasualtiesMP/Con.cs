using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using HarmonyLib;
using KrokoshaCasualtiesMultiplayerAPI;
using KrokoshaCasualtiesUtils;
using LiteNetLib;
using LiteNetLib.Utils;
using Newtonsoft.Json;
using Steamworks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace KrokoshaCasualtiesMP;

public class Con : KrokoshaScavSingleton
{
	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static Func<Command, string> _003C_003E9__44_85;

		public static Func<Command, string> _003C_003E9__44_0;

		public static Action _003C_003E9__44_1;

		public static Action _003C_003E9__44_4;

		public static Func<GameObject, bool> _003C_003E9__44_88;

		public static Func<GameObject, string> _003C_003E9__44_89;

		public static Func<GameObject, bool> _003C_003E9__44_91;

		public static Func<GameObject, string> _003C_003E9__44_92;

		public static Action _003C_003E9__44_9;

		public static Action _003C_003E9__44_12;

		public static Action _003C_003E9__44_13;

		public static Action _003C_003E9__44_14;

		public static Action _003C_003E9__44_15;

		public static Action _003C_003E9__44_20;

		public static Action _003C_003E9__44_22;

		public static Action _003C_003E9__44_24;

		public static Action _003C_003E9__44_25;

		public static Action _003C_003E9__44_26;

		public static Func<Type, string> _003C_003E9__44_103;

		public static Action _003C_003E9__44_30;

		public static Action _003C_003E9__44_33;

		public static Action _003C_003E9__44_36;

		public static Action _003C_003E9__44_41;

		public static Action _003C_003E9__44_42;

		public static Action _003C_003E9__44_43;

		public static Action _003C_003E9__44_45;

		public static Func<Item, GameObject> _003C_003E9__44_107;

		public static Func<BuildingEntity, GameObject> _003C_003E9__44_108;

		public static Action _003C_003E9__44_47;

		public static Func<FieldInfo, string> _003C_003E9__44_109;

		public static Action _003C_003E9__44_59;

		public static Action _003C_003E9__44_64;

		public static Action _003C_003E9__44_65;

		public static Func<NetPlayer, string> _003C_003E9__52_0;

		public static Func<NetPlayer, knetid> _003C_003E9__54_0;

		public static Action<NetPlayer> _003C_003E9__63_0;

		internal string _003C_RegisterMultiplayerConsoleCommands_003Eb__44_85(Command x)
		{
			return x.name;
		}

		internal string _003C_RegisterMultiplayerConsoleCommands_003Eb__44_0(Command x)
		{
			return x.name;
		}

		internal void _003C_RegisterMultiplayerConsoleCommands_003Eb__44_1(string[] splited)
		{
			if (splited.Count() < 2)
			{
				Chat._DEV_LOG_CHAT = !Chat._DEV_LOG_CHAT;
			}
			else
			{
				Chat._DEV_LOG_CHAT = ParseBool01(splited[1]);
			}
			_DEV_CHATSPY = Chat._DEV_LOG_CHAT;
			if (!CanCheat())
			{
				_DEV_CHATSPY = false;
				log.l($"_DEV_LOG_CHAT = {Chat._DEV_LOG_CHAT}");
			}
			else
			{
				log.l($"_DEV_CHATSPY = {_DEV_CHATSPY} ");
			}
		}

		internal void _003C_RegisterMultiplayerConsoleCommands_003Eb__44_4(string[] splited)
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			goof_carameltanzen = !goof_carameltanzen;
			if (!goof_carameltanzen)
			{
				KrokoshaScavMultiplayer.BurgerkingFootLettuce[] array = Object.FindObjectsOfType<KrokoshaScavMultiplayer.BurgerkingFootLettuce>();
				foreach (KrokoshaScavMultiplayer.BurgerkingFootLettuce burgerkingFootLettuce in array)
				{
					if ((Object)(object)burgerkingFootLettuce.l != (Object)null)
					{
						burgerkingFootLettuce.l.color = burgerkingFootLettuce.c;
					}
				}
			}
			log.l($"rgblights {goof_carameltanzen}");
		}

		internal bool _003C_RegisterMultiplayerConsoleCommands_003Eb__44_88(GameObject x)
		{
			return Object.op_Implicit((Object)(object)x.GetComponent<Item>());
		}

		internal string _003C_RegisterMultiplayerConsoleCommands_003Eb__44_89(GameObject x)
		{
			return ((Object)x).name;
		}

		internal bool _003C_RegisterMultiplayerConsoleCommands_003Eb__44_91(GameObject x)
		{
			return Object.op_Implicit((Object)(object)x.GetComponent<Item>());
		}

		internal string _003C_RegisterMultiplayerConsoleCommands_003Eb__44_92(GameObject x)
		{
			return ((Object)x).name;
		}

		internal void _003C_RegisterMultiplayerConsoleCommands_003Eb__44_9(string[] splited)
		{
			ConFailIfNetworkIsRunningAndIsClient();
			bool dEV_GODMODE = !_DEV_GODMODE;
			if (splited.Length > 1)
			{
				dEV_GODMODE = ParseBool01(splited[1]);
			}
			_DEV_GODMODE = dEV_GODMODE;
			ConsoleScript.instance.LogToConsole($"godmode {_DEV_GODMODE}");
		}

		internal void _003C_RegisterMultiplayerConsoleCommands_003Eb__44_12(string[] splited)
		{
			ConFailIfNetworkIsRunningAndIsClient();
			bool flag = !UIMainMenu.mainmenu_open;
			if (splited.Length > 1)
			{
				flag = ParseBool01(splited[1]);
			}
			UIMainMenu.SetOpen(flag);
			ConsoleScript.instance.LogToConsole($"UIMainMenu.SetOpen({flag});");
		}

		internal void _003C_RegisterMultiplayerConsoleCommands_003Eb__44_13(string[] splited)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			ConFailIfCheatsDisabled();
			ushort num = ushort.Parse(splited[1]);
			Vector2 val = default(Vector2);
			if (splited.Length < 4)
			{
				val = Vector2.op_Implicit(Camera.main.ScreenToWorldPoint(Input.mousePosition));
			}
			else
			{
				val.x = float.Parse(splited[2], CultureInfo.InvariantCulture);
				val.y = float.Parse(splited[3], CultureInfo.InvariantCulture);
			}
			Vector2Int val2 = WorldGeneration.world.WorldToBlockPos(val);
			WorldGeneration.world.SetBlock(val2, num);
			ConsoleScript.instance.LogToConsole($"Set tile {((Vector2Int)(ref val2)).x} {((Vector2Int)(ref val2)).y} id to {num} ({WorldGeneration.world.GetBlockInfo(num)?.name})");
		}

		internal void _003C_RegisterMultiplayerConsoleCommands_003Eb__44_14(string[] splited)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			ConFailIfCheatsDisabled();
			byte b = byte.Parse(splited[1]);
			Vector2 val = default(Vector2);
			if (splited.Length < 4)
			{
				val = Vector2.op_Implicit(Camera.main.ScreenToWorldPoint(Input.mousePosition));
			}
			else
			{
				val.x = float.Parse(splited[2], CultureInfo.InvariantCulture);
				val.y = float.Parse(splited[3], CultureInfo.InvariantCulture);
			}
			Vector2Int val2 = WorldGeneration.world.WorldToBlockPos(val);
			FluidManager.main.fluid[((Vector2Int)(ref val2)).x, ((Vector2Int)(ref val2)).y] = b;
			ConsoleScript.instance.LogToConsole($"Set fluid {((Vector2Int)(ref val2)).x} {((Vector2Int)(ref val2)).y} id to {b} ({FluidManager.WorldFluidToLiquidID[b]})");
		}

		internal void _003C_RegisterMultiplayerConsoleCommands_003Eb__44_15(string[] splited)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			string text = string.Join(" ", splited);
			if (Net.is_server)
			{
				ServerMain.RunClientCustomCommand(text, null);
				return;
			}
			NetDataWriter writer = Net.CreateWriter(10184);
			writer.Put(text);
			Net.Client_Send((DeliveryMethod)2, in writer);
		}

		internal void _003C_RegisterMultiplayerConsoleCommands_003Eb__44_20(string[] splited)
		{
			log.l("BANLIST: " + BanList.Instance.filePath + "\n" + string.Join("\n", BanList.Entries) + " ");
		}

		internal void _003C_RegisterMultiplayerConsoleCommands_003Eb__44_22(string[] splited)
		{
			ConFailIfNetworkNotRunning();
			ConsoleScript instance = ConsoleScript.instance;
			string message = string.Join(" ", splited.Skip(1));
			Chat.SendChatMessage(in message, force_server_if_server: true);
			instance.LogToConsole("Chat message sent: " + message);
		}

		internal void _003C_RegisterMultiplayerConsoleCommands_003Eb__44_24(string[] splited)
		{
			bool active = !PlayerCamera.main.backgroundSnow.activeSelf;
			if (splited.Length > 1)
			{
				active = ParseBool01(splited[1]);
			}
			PlayerCamera.main.backgroundSnow.SetActive(active);
			ConsoleScript.instance.LogToConsole($"snowbg {PlayerCamera.main.backgroundSnow.activeSelf} ");
		}

		internal void _003C_RegisterMultiplayerConsoleCommands_003Eb__44_25(string[] splited)
		{
			ConFailIfNetworkIsRunningAndIsClient();
			ConFailIfNotInMainMenu();
			string text = "debug";
			if (splited.Length > 1)
			{
				text = splited[1];
			}
			PreRunScript val = Object.FindObjectOfType<PreRunScript>();
			if (text == "debug")
			{
				log.l("Loading debug world");
				WorldgenPatches.LoadDebugWorld();
			}
			else if (text == "tutorial")
			{
				log.l("Loading tutorial world");
				val.StartTutorial();
			}
			else
			{
				log.l("Loading vanilla world");
				val.StartRun();
			}
		}

		internal void _003C_RegisterMultiplayerConsoleCommands_003Eb__44_26(string[] splited)
		{
			bool flag = !_DEV_TELEKINESIS;
			if (splited.Length > 1)
			{
				flag = ParseBool01(splited[1]);
			}
			if (flag)
			{
				ConFailIfNetworkIsRunningAndIsClient();
				ConFailIfCheatsDisabled();
			}
			_DEV_TELEKINESIS = flag;
			ConsoleScript.instance.LogToConsole($"telekinesis {_DEV_TELEKINESIS}");
		}

		internal string _003C_RegisterMultiplayerConsoleCommands_003Eb__44_103(Type x)
		{
			return x.Name;
		}

		internal void _003C_RegisterMultiplayerConsoleCommands_003Eb__44_30(string[] splited)
		{
			bool flag = !_DEV_AUDIO_ONLY_ON_FOCUS;
			if (splited.Length > 1)
			{
				flag = ParseBool01(splited[1]);
			}
			if (_DEV_AUDIO_ONLY_ON_FOCUS != flag)
			{
				if (flag)
				{
					_DEV_LAST_VOLUME = AudioListener.volume;
				}
				else
				{
					AudioListener.volume = _DEV_LAST_VOLUME;
				}
			}
			_DEV_AUDIO_ONLY_ON_FOCUS = flag;
			ConsoleScript.instance.LogToConsole($"audio_only_on_focus {_DEV_AUDIO_ONLY_ON_FOCUS}");
		}

		internal void _003C_RegisterMultiplayerConsoleCommands_003Eb__44_33(string[] splited)
		{
			ConFailIfNetworkIsRunningAndIsClient();
			MP3PlayerServerAudioStreamer.Server_ForceLoadMusic();
		}

		internal void _003C_RegisterMultiplayerConsoleCommands_003Eb__44_36(string[] splited)
		{
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			ConFailIfNetworkNotRunning();
			ConFailIfNetworkIsRunningAndIsServer();
			string text = "";
			if (splited.Length > 1)
			{
				text = splited[1];
			}
			client_adminmode = !client_adminmode;
			client_isadmin = client_isadmin || client_adminmode;
			log.l($"Requesting admin console: {client_adminmode}");
			NetDataWriter writer = Net.CreateWriter(10046);
			writer.Put(client_adminmode);
			writer.Put(text);
			Net.Client_Send((DeliveryMethod)2, in writer);
		}

		internal void _003C_RegisterMultiplayerConsoleCommands_003Eb__44_41(string[] splited)
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Expected O, but got Unknown
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Expected O, but got Unknown
			_ = ConsoleScript.instance;
			StringBuilder stringBuilder = new StringBuilder("// KROKOSHA CO-OP MOD LOCALE DUMP:\n");
			if (splited.Length > 1)
			{
				string text = splited[1];
				stringBuilder.AppendLine("\n// " + UnityObjectUtility.ToSafeString((object)text));
				Language val = new Language();
				foreach (KeyValuePair<string, string> item in Lang.dict[text])
				{
					val.other["krokosha_coop_" + item.Key] = item.Value;
				}
				stringBuilder.Append(JsonConvert.SerializeObject((object)val, (Formatting)1));
			}
			else
			{
				foreach (KeyValuePair<string, Dictionary<string, string>> item2 in Lang.dict)
				{
					Language val2 = new Language();
					foreach (KeyValuePair<string, string> item3 in item2.Value)
					{
						val2.other["krokosha_coop_" + item3.Key] = item3.Value;
					}
					stringBuilder.AppendLine("\n// " + item2.Key);
					stringBuilder.Append(JsonConvert.SerializeObject((object)val2, (Formatting)1));
				}
			}
			stringBuilder.AppendLine("\n// END\n// COPY IT FROM LOG FILE INSTEAD OF CONSOLE BECAUSE IT CLEARS FORMATTING HERE");
			log.l(stringBuilder.ToString());
		}

		internal void _003C_RegisterMultiplayerConsoleCommands_003Eb__44_42(string[] splited)
		{
			bool flag = !KrokoshaScavMultiplayer.verbose;
			if (splited.Length > 1)
			{
				flag = ParseBool01(splited[1]);
			}
			DebugMenuSettings.is_verbose = flag;
			DebugMenuSettings._DEV_VISUALISE_NET_EVENTS = flag;
			DebugMenuSettings._DEV_ENABLE_STEAM_LOG = flag;
			log.l($"DEV : verbose = {KrokoshaScavMultiplayer.verbose} ");
		}

		internal void _003C_RegisterMultiplayerConsoleCommands_003Eb__44_43(string[] splited)
		{
			bool dEV_ENABLE_SYNCINFO_SNITCHING = !NetObjectRegistry._DEV_ENABLE_SYNCINFO_SNITCHING;
			if (splited.Length > 1)
			{
				dEV_ENABLE_SYNCINFO_SNITCHING = ParseBool01(splited[1]);
			}
			NetObjectRegistry._DEV_ENABLE_SYNCINFO_SNITCHING = dEV_ENABLE_SYNCINFO_SNITCHING;
			log.l($"NetObjectRegistry._DEV_ENABLE_SYNCINFO_SNITCHING = {NetObjectRegistry._DEV_ENABLE_SYNCINFO_SNITCHING} ");
		}

		internal void _003C_RegisterMultiplayerConsoleCommands_003Eb__44_45(string[] splited)
		{
			ConFailIfNetworkNotRunning();
			ushort num = ushort.Parse(splited[1]);
			NewCoolerObjectPacketWriteReadSystem.inst.Shared_ForceSync(num);
			log.l($"NetObjectRegistry.Shared_ForceSync(id: {num});");
		}

		internal void _003C_RegisterMultiplayerConsoleCommands_003Eb__44_47(string[] splited)
		{
			ConFailIfNetworkNotRunning();
			foreach (GameObject item in (from x in Object.FindObjectsOfType<Item>()
				select ((Component)x).gameObject).Union(from x in Object.FindObjectsOfType<BuildingEntity>()
				select ((Component)x).gameObject))
			{
				if (!NetObjectRegistry.ObjectCanBeIgnoredForNetwork(item))
				{
					NetObjectRegistry.NewGO(item);
				}
			}
			log.l("called NetObjectRegistry.NewGO on everything");
		}

		internal GameObject _003C_RegisterMultiplayerConsoleCommands_003Eb__44_107(Item x)
		{
			return ((Component)x).gameObject;
		}

		internal GameObject _003C_RegisterMultiplayerConsoleCommands_003Eb__44_108(BuildingEntity x)
		{
			return ((Component)x).gameObject;
		}

		internal string _003C_RegisterMultiplayerConsoleCommands_003Eb__44_109(FieldInfo x)
		{
			return x.Name;
		}

		internal void _003C_RegisterMultiplayerConsoleCommands_003Eb__44_59(string[] args)
		{
			ConFailIfNotInSteamLobby();
			log.l("LOBBY INFO:\n" + KSteam.CURRENT_LOBBY.DebugReadableDump());
		}

		internal void _003C_RegisterMultiplayerConsoleCommands_003Eb__44_64(string[] args)
		{
			if (args.Count() > 1)
			{
				BugReporter.user_bugreport_message = string.Join(" ", args.Skip(1));
			}
			Util.StartCoroutine(BugReporter.BugreportUploadSequence());
		}

		internal void _003C_RegisterMultiplayerConsoleCommands_003Eb__44_65(string[] args)
		{
			ConFailIfNetworkNotRunning();
			log.l("Disconnect.");
			KrokoshaScavMultiplayer.ShutdownNetwork();
		}

		internal string _003CUpdatePlayerListAutocomplete_003Eb__52_0(NetPlayer x)
		{
			return x.playername;
		}

		internal knetid _003CServer_SendConsoleLog_003Eb__54_0(NetPlayer x)
		{
			return x.clientId;
		}

		internal void _003CStart_003Eb__63_0(NetPlayer plr)
		{
			server_admins.Remove(plr);
		}
	}

	internal static Color[] colors = (Color[])(object)new Color[20]
	{
		Color.cyan,
		Color.blue,
		Color.magenta,
		Color.yellow,
		Color.green,
		new Color(1f, 0.5f, 0f),
		Color.magenta,
		Color.blue,
		Color.yellow,
		Color.red,
		Color.blue,
		Color.green,
		Color.red,
		Color.cyan,
		Color.magenta,
		Color.green,
		Color.magenta,
		Color.yellow,
		Color.blue,
		Color.red
	};

	internal static float coloooooooo_timer = 0f;

	internal static int coloooooooo = 0;

	internal static bool goof_carameltanzen = false;

	public static bool _DEV_CHATSPY = false;

	public static float _DEV_ZOOM = 1f;

	public static bool _DEV_GODMODE = false;

	internal static bool _DEV_AUDIO_ONLY_ON_FOCUS = false;

	internal static float _DEV_LAST_VOLUME = 1f;

	public static bool _DEV_TELEKINESIS = false;

	public static GameObject _DEV_TELEKINESIS_OBJECT = null;

	public static HashSet<string> localonly_commands = new HashSet<string>
	{
		"clear", "noclip", "copylog", "volume", "musicvolume", "pixelate", "spectate", "fullbright", "unchipped", "echo",
		"bind", "addcustomcommand", "removecustomcommand", "framerate", "setconsoleheight", "setconsolecolor"
	};

	public static HashSet<string> banned_admin_commands = new HashSet<string>
	{
		"noclip", "volume", "musicvolume", "adminpriv", "pixelate", "spectate", "fullbright", "music", "echo", "bind",
		"addcustomcommand", "removecustomcommand", "framerate", "setconsoleheight", "setconsolecolor"
	};

	internal static HashSet<NetPlayer> server_admins = new HashSet<NetPlayer>();

	internal static HashSet<string> removed_server_admins = new HashSet<string>();

	internal static string server_admin_password = "";

	public static bool client_adminmode = false;

	public static bool client_isadmin = false;

	private static Vector2 lastworldcursorpos = default(Vector2);

	public static bool _DEV_HIDEHUD
	{
		get
		{
			if ((Object)(object)PlayerCamera.main == (Object)null)
			{
				return false;
			}
			return !((Component)PlayerCamera.main.mainCanvas).gameObject.activeSelf;
		}
		set
		{
			if (!((Object)(object)PlayerCamera.main == (Object)null))
			{
				((Component)PlayerCamera.main.mainCanvas).gameObject.SetActive(!value);
			}
		}
	}

	public static bool _DEV_FREECAM
	{
		get
		{
			if ((Object)(object)PlayerCamera.main == (Object)null)
			{
				return false;
			}
			return PlayerCamera.main.isFreecam;
		}
		set
		{
			if (!((Object)(object)PlayerCamera.main == (Object)null))
			{
				PlayerCamera.main.isFreecam = value;
			}
		}
	}

	public static ConsoleScript con => ConsoleScript.instance;

	public static bool IsConsoleOpen()
	{
		if (Object.op_Implicit((Object)(object)ConsoleScript.instance))
		{
			return ConsoleScript.instance.active;
		}
		return false;
	}

	public static void OpenConsole()
	{
		if (!con.active)
		{
			con.ToggleActiveState();
		}
	}

	public static bool CanCheat()
	{
		if (!KrokoshaScavMultiplayer.network_system_is_running)
		{
			return true;
		}
		if (KrokoshaScavMultiplayer.is_client)
		{
			return KrokoshaScavMultiplayer.rules.AllowClientCheatCommands;
		}
		return KrokoshaScavMultiplayer.rules.sv_cheats;
	}

	public static string CheatWarningText()
	{
		if (!KrokoshaScavMultiplayer.network_system_is_running)
		{
			return "huh";
		}
		if (!KrokoshaScavMultiplayer.rules.sv_cheats)
		{
			return "Server rule 'sv_cheats' is disabled";
		}
		if (!KrokoshaScavMultiplayer.rules.AllowClientCheatCommands)
		{
			return "Server rule 'AllowClientCheatCommands' is disabled";
		}
		return "hello";
	}

	public static void ConFailIfNetworkAlreadyRunning()
	{
		if (KrokoshaScavMultiplayer.network_system_is_running)
		{
			throw new Exception("Network system is already running!");
		}
	}

	public static void ConFailIfNetworkNotRunning()
	{
		if (!KrokoshaScavMultiplayer.network_system_is_running)
		{
			throw new Exception("Network system is not running!");
		}
	}

	public static void ConFailIfNotInSteamLobby()
	{
		if (!KrokoshaScavMultiplayer.network_system_is_running)
		{
			throw new Exception("Network system is not running!");
		}
		if (!Net.TryGetSteamTransport(out var _))
		{
			throw new Exception("You can do that only in a Steam lobby!");
		}
	}

	public static void ConFailIfNotConnected()
	{
		if (!Net.is_connected)
		{
			throw new Exception("Network system is not running or not connected!");
		}
	}

	public static void ConFailIfNoLocalPlayer()
	{
		if ((Object)(object)NetPlayer.LOCAL_PLAYER == (Object)null)
		{
			throw new Exception("Local player not found!");
		}
	}

	public static void ConFailIfCheatsDisabled()
	{
		if (!CanCheat())
		{
			throw new Exception("Cheats disabled. " + CheatWarningText());
		}
	}

	public static void ConFailIfNetworkIsRunningAndIsClient()
	{
		if (KrokoshaScavMultiplayer.network_system_is_running && KrokoshaScavMultiplayer.is_client)
		{
			throw new Exception("Client can't do that.");
		}
	}

	public static void ConFailIfNetworkIsRunningAndIsServer()
	{
		if (KrokoshaScavMultiplayer.network_system_is_running && KrokoshaScavMultiplayer.is_server)
		{
			throw new Exception("Server can't do that.");
		}
	}

	public static void ConFailIfNotInMainMenu()
	{
		if (!Util.IsInMainMenu())
		{
			throw new Exception("This can only be done in main menu.");
		}
	}

	public static bool TryParsePlayerPositionGetName(string s, out string name)
	{
		string text = "player:";
		if (s.StartsWith(text))
		{
			name = StringUtility.TrimStart(s, text);
			return true;
		}
		text = "plr:";
		if (s.StartsWith(text))
		{
			name = StringUtility.TrimStart(s, text);
			return true;
		}
		name = null;
		return false;
	}

	public static bool ParseBool01(string str, bool enabledisable = true)
	{
		str = str.ToLower();
		if (enabledisable)
		{
			if (str == "enable")
			{
				return true;
			}
			if (str == "disable")
			{
				return false;
			}
		}
		if (bool.TryParse(str, out var result))
		{
			return result;
		}
		if (int.TryParse(str, out var result2))
		{
			if (result2 == 0)
			{
				return false;
			}
			return true;
		}
		throw new Exception("\"" + str + "\" is not a valid boolean value! (true/false)");
	}

	public static void UnNoclip()
	{
		if (ConsoleScript.instance.noClip)
		{
			PlayerCamera.main.body.rb.simulated = true;
			ConsoleScript.instance.noClip = false;
		}
	}

	public static Command RegisterCommand(in Command comm)
	{
		ConsoleScript.Commands.Add(comm);
		return comm;
	}

	public static Command RegisterCommand_LocalOnly(in Command comm)
	{
		RegisterCommand(in comm);
		localonly_commands.Add(comm.name);
		return comm;
	}

	public static Command RegisterCommand_BannedAdmin(in Command comm)
	{
		RegisterCommand(in comm);
		banned_admin_commands.Add(comm.name);
		return comm;
	}

	public static GameObject SpawnThingOnPlayer(string resourceid, Body body = null, bool give_it_to_em = false, GameObject container = null)
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		if (KrokoshaScavMultiplayer.is_dedicated_server && (Object)(object)body == (Object)null && NetPlayer.BodyToPlayerDict.Count > 0)
		{
			body = NetPlayer.BodyToPlayerDict.First().Key;
		}
		if (body == null)
		{
			body = PlayerCamera.main.body;
		}
		Object val = Resources.Load(resourceid);
		if (!Object.op_Implicit(val))
		{
			Plugin.log.LogError((object)("Unknown resource: " + resourceid));
			return null;
		}
		Object obj = Object.Instantiate(val, Vector2.op_Implicit(Vector2.op_Implicit(((Component)body).transform.position) + Random.insideUnitCircle), Quaternion.Euler(0f, 0f, 0f));
		GameObject val2 = (GameObject)(object)((obj is GameObject) ? obj : null);
		AmmoScript val3 = default(AmmoScript);
		if (val2.TryGetComponent<AmmoScript>(ref val3))
		{
			val3.rounds = val3.maxRounds;
		}
		GunScript val4 = default(GunScript);
		if (val2.TryGetComponent<GunScript>(ref val4))
		{
			val4.roundsInMag = val4.magCapacity;
			if ((int)val4.feedType == 0)
			{
				val4.hasMag = true;
			}
			val4.roundInChamber = (RoundInChamber)0;
		}
		Item val5 = default(Item);
		if (give_it_to_em && val2.TryGetComponent<Item>(ref val5))
		{
			Container val6 = default(Container);
			if (Object.op_Implicit((Object)(object)container) && container.TryGetComponent<Container>(ref val6))
			{
				val6.LoadItem(val5);
			}
			else
			{
				body.AutoPickUpItem(val5);
			}
		}
		return val2;
	}

	public static void DelayRunCommand(float delay_sec, string cmd)
	{
		Plugin.Startcorout(_DelayRunCommandCoroutine(delay_sec, cmd));
	}

	private static IEnumerator _DelayRunCommandCoroutine(float delay_sec, string cmd)
	{
		yield return (object)new WaitForSecondsRealtime(delay_sec);
		RunCommand(cmd);
	}

	public static void RunCommand(string cmd, bool addToLog = false)
	{
		string[] array = cmd.Trim().Split(new char[1] { ' ' });
		ConsoleScript.instance.TryExecuteCommand(array, addToLog);
	}

	public static bool CanFreecam()
	{
		if (Util.IsInMainMenu())
		{
			return false;
		}
		if (CanCheat())
		{
			return true;
		}
		if (KrokoshaScavMultiplayer.rules.SpectateWhileUnconscious)
		{
			if (!PlayerCamera.main.body.conscious)
			{
				return true;
			}
		}
		else if (!PlayerCamera.main.body.alive)
		{
			return true;
		}
		return false;
	}

	public static void CleanWorld()
	{
		foreach (BlockDamage blockDamage in WorldGeneration.world.blockDamages)
		{
			blockDamage.damage = 1E+11f;
			blockDamage.UpdateSprite();
		}
		WorldGeneration.world.blockDamages.Clear();
		SpiderHandler[] array = Object.FindObjectsOfType<SpiderHandler>();
		for (int i = 0; i < array.Length; i++)
		{
			Object.Destroy((Object)(object)((Component)array[i]).gameObject);
		}
		Item[] array2 = Object.FindObjectsOfType<Item>();
		foreach (Item val in array2)
		{
			if (NetObjectRegistry.lowpriority_objects_resourceids.Contains("casing"))
			{
				Object.Destroy((Object)(object)((Component)val).gameObject);
			}
		}
		SpriteRenderer[] array3 = Object.FindObjectsOfType<SpriteRenderer>();
		foreach (SpriteRenderer val2 in array3)
		{
			if (((Object)((Component)val2).gameObject).name.Contains("blastmark") && ((Renderer)val2).sortingOrder == -9050)
			{
				Object.Destroy((Object)(object)((Component)val2).gameObject);
			}
		}
	}

	internal static void _RegisterMultiplayerConsoleCommands()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Expected O, but got Unknown
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Expected O, but got Unknown
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Expected O, but got Unknown
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Expected O, but got Unknown
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Expected O, but got Unknown
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Expected O, but got Unknown
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Expected O, but got Unknown
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Expected O, but got Unknown
		//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05de: Expected O, but got Unknown
		//IL_067f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0689: Expected O, but got Unknown
		//IL_071f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0726: Expected O, but got Unknown
		//IL_073f: Unknown result type (might be due to invalid IL or missing references)
		//IL_074f: Expected O, but got Unknown
		//IL_074a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0751: Expected O, but got Unknown
		//IL_076a: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c2: Expected O, but got Unknown
		//IL_07bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c4: Expected O, but got Unknown
		//IL_070e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0713: Unknown result type (might be due to invalid IL or missing references)
		//IL_0719: Expected O, but got Unknown
		//IL_07fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0802: Expected O, but got Unknown
		//IL_081b: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f5: Expected O, but got Unknown
		//IL_08ba: Expected O, but got Unknown
		//IL_08b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08bb: Expected O, but got Unknown
		//IL_08d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0973: Expected O, but got Unknown
		//IL_096e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0974: Expected O, but got Unknown
		//IL_098d: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ce: Expected O, but got Unknown
		//IL_09c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d0: Expected O, but got Unknown
		//IL_09e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a2a: Expected O, but got Unknown
		//IL_0a25: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a2c: Expected O, but got Unknown
		//IL_0a63: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a6a: Expected O, but got Unknown
		//IL_0a83: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aaa: Expected O, but got Unknown
		//IL_0aa5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aac: Expected O, but got Unknown
		//IL_0ac5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad5: Expected O, but got Unknown
		//IL_0ad0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad7: Expected O, but got Unknown
		//IL_0a52: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a57: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a5d: Expected O, but got Unknown
		//IL_0b0e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b15: Expected O, but got Unknown
		//IL_0afd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b02: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b08: Expected O, but got Unknown
		//IL_0b4c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b53: Expected O, but got Unknown
		//IL_0b3b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b40: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b46: Expected O, but got Unknown
		//IL_0b8a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b91: Expected O, but got Unknown
		//IL_0b79: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b7e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b84: Expected O, but got Unknown
		//IL_0bf5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bfc: Expected O, but got Unknown
		//IL_0c15: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c94: Expected O, but got Unknown
		//IL_0c8f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c96: Expected O, but got Unknown
		//IL_0caf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cec: Expected O, but got Unknown
		//IL_0ce7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cee: Expected O, but got Unknown
		//IL_0d07: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d44: Expected O, but got Unknown
		//IL_0d3f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d46: Expected O, but got Unknown
		//IL_0d5f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d9c: Expected O, but got Unknown
		//IL_0d97: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d9e: Expected O, but got Unknown
		//IL_0bb7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bbc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bc2: Expected O, but got Unknown
		//IL_0dd5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ddc: Expected O, but got Unknown
		//IL_0df5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e1c: Expected O, but got Unknown
		//IL_0e17: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e1e: Expected O, but got Unknown
		//IL_0dc4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dc9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dcf: Expected O, but got Unknown
		//IL_0e6c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e73: Expected O, but got Unknown
		//IL_0e8c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ec9: Expected O, but got Unknown
		//IL_0ec4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ecb: Expected O, but got Unknown
		//IL_0e44: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e49: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e4f: Expected O, but got Unknown
		//IL_0f02: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f09: Expected O, but got Unknown
		//IL_0ef1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ef6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0efc: Expected O, but got Unknown
		//IL_0f71: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f78: Expected O, but got Unknown
		//IL_0f2f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f34: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f3a: Expected O, but got Unknown
		//IL_0faf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fb6: Expected O, but got Unknown
		//IL_0fcf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f9e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fa3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fa9: Expected O, but got Unknown
		//IL_1045: Expected O, but got Unknown
		//IL_1040: Unknown result type (might be due to invalid IL or missing references)
		//IL_1047: Expected O, but got Unknown
		//IL_1060: Unknown result type (might be due to invalid IL or missing references)
		//IL_1087: Expected O, but got Unknown
		//IL_1082: Unknown result type (might be due to invalid IL or missing references)
		//IL_1089: Expected O, but got Unknown
		//IL_10a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_10c9: Expected O, but got Unknown
		//IL_10c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_10cb: Expected O, but got Unknown
		//IL_1102: Unknown result type (might be due to invalid IL or missing references)
		//IL_1109: Expected O, but got Unknown
		//IL_1122: Unknown result type (might be due to invalid IL or missing references)
		//IL_115f: Expected O, but got Unknown
		//IL_115a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1161: Expected O, but got Unknown
		//IL_117a: Unknown result type (might be due to invalid IL or missing references)
		//IL_11a1: Expected O, but got Unknown
		//IL_119c: Unknown result type (might be due to invalid IL or missing references)
		//IL_11a3: Expected O, but got Unknown
		//IL_10f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_10f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_10fc: Expected O, but got Unknown
		//IL_11da: Unknown result type (might be due to invalid IL or missing references)
		//IL_11e1: Expected O, but got Unknown
		//IL_11fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_1221: Expected O, but got Unknown
		//IL_121c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1223: Expected O, but got Unknown
		//IL_123c: Unknown result type (might be due to invalid IL or missing references)
		//IL_129f: Expected O, but got Unknown
		//IL_129a: Unknown result type (might be due to invalid IL or missing references)
		//IL_12a1: Expected O, but got Unknown
		//IL_11c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_11ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_11d4: Expected O, but got Unknown
		//IL_12ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_12f6: Expected O, but got Unknown
		//IL_130f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1336: Expected O, but got Unknown
		//IL_1331: Unknown result type (might be due to invalid IL or missing references)
		//IL_1338: Expected O, but got Unknown
		//IL_1351: Unknown result type (might be due to invalid IL or missing references)
		//IL_1361: Expected O, but got Unknown
		//IL_135c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1363: Expected O, but got Unknown
		//IL_137c: Unknown result type (might be due to invalid IL or missing references)
		//IL_13b9: Expected O, but got Unknown
		//IL_13b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_13bb: Expected O, but got Unknown
		//IL_13d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1411: Expected O, but got Unknown
		//IL_140c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1413: Expected O, but got Unknown
		//IL_12c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_12cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_12d2: Expected O, but got Unknown
		//IL_144a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1451: Expected O, but got Unknown
		//IL_1439: Unknown result type (might be due to invalid IL or missing references)
		//IL_143e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1444: Expected O, but got Unknown
		//IL_1488: Unknown result type (might be due to invalid IL or missing references)
		//IL_148f: Expected O, but got Unknown
		//IL_1477: Unknown result type (might be due to invalid IL or missing references)
		//IL_147c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1482: Expected O, but got Unknown
		//IL_14c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_14cd: Expected O, but got Unknown
		//IL_14e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1539: Expected O, but got Unknown
		//IL_1534: Unknown result type (might be due to invalid IL or missing references)
		//IL_153b: Expected O, but got Unknown
		//IL_14b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_14ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_14c0: Expected O, but got Unknown
		//IL_1589: Unknown result type (might be due to invalid IL or missing references)
		//IL_1590: Expected O, but got Unknown
		//IL_15a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_15d0: Expected O, but got Unknown
		//IL_15cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_15d2: Expected O, but got Unknown
		//IL_1561: Unknown result type (might be due to invalid IL or missing references)
		//IL_1566: Unknown result type (might be due to invalid IL or missing references)
		//IL_156c: Expected O, but got Unknown
		//IL_1609: Unknown result type (might be due to invalid IL or missing references)
		//IL_1610: Expected O, but got Unknown
		//IL_1629: Unknown result type (might be due to invalid IL or missing references)
		//IL_1650: Expected O, but got Unknown
		//IL_164b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1652: Expected O, but got Unknown
		//IL_166b: Unknown result type (might be due to invalid IL or missing references)
		//IL_16ea: Expected O, but got Unknown
		//IL_16e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_16ec: Expected O, but got Unknown
		//IL_1705: Unknown result type (might be due to invalid IL or missing references)
		//IL_172c: Expected O, but got Unknown
		//IL_1727: Unknown result type (might be due to invalid IL or missing references)
		//IL_172e: Expected O, but got Unknown
		//IL_1747: Unknown result type (might be due to invalid IL or missing references)
		//IL_1757: Expected O, but got Unknown
		//IL_1752: Unknown result type (might be due to invalid IL or missing references)
		//IL_1759: Expected O, but got Unknown
		//IL_1772: Unknown result type (might be due to invalid IL or missing references)
		//IL_1782: Expected O, but got Unknown
		//IL_177d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1784: Expected O, but got Unknown
		//IL_179d: Unknown result type (might be due to invalid IL or missing references)
		//IL_17ad: Expected O, but got Unknown
		//IL_17a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_17af: Expected O, but got Unknown
		//IL_17c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_15f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_15fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_1603: Expected O, but got Unknown
		//IL_184a: Expected O, but got Unknown
		//IL_1845: Unknown result type (might be due to invalid IL or missing references)
		//IL_184c: Expected O, but got Unknown
		//IL_1865: Unknown result type (might be due to invalid IL or missing references)
		//IL_188c: Expected O, but got Unknown
		//IL_1887: Unknown result type (might be due to invalid IL or missing references)
		//IL_188e: Expected O, but got Unknown
		//IL_18a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_18ce: Expected O, but got Unknown
		//IL_18c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_18d0: Expected O, but got Unknown
		//IL_18e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_1910: Expected O, but got Unknown
		//IL_190b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1912: Expected O, but got Unknown
		//IL_192b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1968: Expected O, but got Unknown
		//IL_1963: Unknown result type (might be due to invalid IL or missing references)
		//IL_196a: Expected O, but got Unknown
		//IL_19a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_19a8: Expected O, but got Unknown
		//IL_19c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_19e8: Expected O, but got Unknown
		//IL_19e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_19ea: Expected O, but got Unknown
		//IL_1a03: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a2a: Expected O, but got Unknown
		//IL_1a25: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a2c: Expected O, but got Unknown
		//IL_1a45: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a6c: Expected O, but got Unknown
		//IL_1a67: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a6e: Expected O, but got Unknown
		//IL_1a87: Unknown result type (might be due to invalid IL or missing references)
		//IL_1aae: Expected O, but got Unknown
		//IL_1aa9: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ab0: Expected O, but got Unknown
		//IL_1990: Unknown result type (might be due to invalid IL or missing references)
		//IL_1995: Unknown result type (might be due to invalid IL or missing references)
		//IL_199b: Expected O, but got Unknown
		//IL_1afe: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b05: Expected O, but got Unknown
		//IL_1ad6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1adb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ae1: Expected O, but got Unknown
		//IL_1b3c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b43: Expected O, but got Unknown
		//IL_1b5c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b6c: Expected O, but got Unknown
		//IL_1b67: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b6e: Expected O, but got Unknown
		//IL_1b2b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b30: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b36: Expected O, but got Unknown
		ConsoleScript con = ConsoleScript.instance;
		Command obj = ConsoleScript.SearchExact("tp");
		obj.action = (Action)delegate(string[] args)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0163: Unknown result type (might be due to invalid IL or missing references)
			//IL_0168: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0198: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			con.CheckForWorld();
			ConFailIfCheatsDisabled();
			Vector2 vector = Vector2.op_Implicit(Camera.main.ScreenToWorldPoint(Input.mousePosition));
			NetPlayer callerPlr = null;
			if (args.Length > 1)
			{
				vector = con.ParsePosition(args[1]);
				if (TryParsePlayerPositionGetName(args[1], out var name))
				{
					NetBody netBody = ServerMain.RelaxedGetBodyForCommand(name, allow_macros: true, only_players: false, NetPlayer.GetLocalNetBodyNullable());
					if ((Object)(object)netBody != (Object)null)
					{
						callerPlr = netBody.plr;
					}
				}
			}
			if (args.Length > 2 && KrokoshaScavMultiplayer.network_system_is_running)
			{
				string plrname = args[2];
				bool succ = false;
				bool isall = plrname == "@a";
				Tuple<bool, string> tuple = ServerMain._PerformActionOnPlayersByName(plrname, delegate(NetPlayer plr)
				{
					//IL_0030: Unknown result type (might be due to invalid IL or missing references)
					//IL_001d: Unknown result type (might be due to invalid IL or missing references)
					if (!isall)
					{
						plrname = plr.playername;
					}
					if (!KrokoshaScavMultiplayer.is_client)
					{
						plr.Server_TeleportCharacter(vector);
					}
					else
					{
						plr.playerbody.SetBodyPosition(vector);
					}
					succ = true;
				}, allow_macros: true, callerPlr, require_body: true);
				if (succ && tuple.Item1)
				{
					con.LogToConsole($"Teleported: {plrname} to {vector}");
				}
				else if (tuple.Item1)
				{
					con.LogToConsole("Failed to teleport: " + plrname);
				}
				else
				{
					con.LogToConsole("Failed to teleport: " + plrname + " " + tuple.Item2);
				}
			}
			else
			{
				((Component)PlayerCamera.main.body).transform.position = Vector2.op_Implicit(vector);
				((Component)PlayerCamera.main).transform.position = Vector2.op_Implicit(vector);
				con.LogToConsole($"Teleported player to {vector}.");
			}
		};
		obj.argDescription = CollectionExtensions.AddToArray<(string, string)>(obj.argDescription, ("player", "optional, Who to teleport"));
		Command obj2 = ConsoleScript.SearchExact("kill");
		obj2.action = (Action)delegate(string[] args)
		{
			con.CheckForWorld();
			if (KrokoshaScavMultiplayer.network_system_is_running)
			{
				ConFailIfCheatsDisabled();
			}
			if (args.Length > 1)
			{
				string plrname = args[1];
				bool flag = plrname == "@a";
				bool succ = false;
				ServerMain._PerformActionOnPlayersByName(plrname, delegate(NetPlayer plr)
				{
					plr.body.brainHealth = 0f;
					plr.body.heartRate = 0f;
					plrname = plr.playername;
					succ = true;
					con.LogToConsole("Killed: " + plrname + " ");
				}, allow_macros: true, null, require_body: true);
				if (succ)
				{
					if (flag)
					{
						con.LogToConsole("Killed everyone :) ");
					}
				}
				else
				{
					con.LogToConsole(plrname + " not found. ");
				}
			}
			else
			{
				PlayerCamera.main.body.brainHealth = 0f;
				PlayerCamera.main.body.heartRate = 0f;
				con.LogToConsole("Killed the player.");
			}
		};
		obj2.argDescription = CollectionExtensions.AddToArray<(string, string)>(obj2.argDescription, ("player", "optional"));
		Command obj3 = ConsoleScript.SearchExact("setbodyfield");
		obj3.action = (Action)delegate(string[] args)
		{
			con.CheckForWorld();
			con.CheckArgumentCount(args, 2);
			string text = args[1];
			FieldInfo field = typeof(Body).GetField(text);
			object obj32 = TypeDescriptor.GetConverter(field.FieldType).ConvertFromInvariantString(args[2]);
			if (args.Length > 3 && KrokoshaScavMultiplayer.network_system_is_running)
			{
				ConFailIfNetworkIsRunningAndIsClient();
				Tuple<bool, string> tuple = ServerMain._PerformActionOnPlayersByName(args[3], delegate(NetPlayer plr)
				{
					field.SetValue(plr.body, obj32);
					con.LogToConsole("Set '" + plr.playername + "' body field \"" + text + "\" to \"" + obj32.ToString() + "\".");
				}, allow_macros: true, null, require_body: true);
				if (!tuple.Item1)
				{
					con.LogToConsole("ERROR: Set body field: " + args[3] + "  - " + tuple.Item2);
				}
			}
			else
			{
				field.SetValue(PlayerCamera.main.body, obj32);
				con.LogToConsole("Set player body field \"" + text + "\" to \"" + obj32.ToString() + "\".");
			}
		};
		obj3.argDescription = CollectionExtensions.AddToArray<(string, string)>(obj3.argDescription, ("player", "optional, name of a player to mess with"));
		Command obj4 = ConsoleScript.SearchExact("setlimbfield");
		obj4.action = (Action)delegate(string[] args)
		{
			con.CheckForWorld();
			con.CheckArgumentCount(args, 3);
			string limbname = args[1];
			bool is_all = limbname.ToLower() == "all";
			Limb val24 = (is_all ? PlayerCamera.main.body.GetHead() : PlayerCamera.main.body.LimbByName(limbname));
			if (!Object.op_Implicit((Object)(object)val24))
			{
				throw new Exception("\"" + args[1] + "\" is not a valid limb!");
			}
			string text = args[2];
			FieldInfo field = typeof(Limb).GetField(text);
			object obj32 = TypeDescriptor.GetConverter(field.FieldType).ConvertFromInvariantString(args[3]);
			if (args.Length > 4 && KrokoshaScavMultiplayer.network_system_is_running)
			{
				ConFailIfNetworkIsRunningAndIsClient();
				string text2 = args[4];
				Tuple<bool, string> tuple = ServerMain._PerformActionOnPlayersByName(text2, delegate(NetPlayer plr)
				{
					if (is_all)
					{
						Limb[] limbs2 = plr.body.limbs;
						foreach (Limb obj34 in limbs2)
						{
							field.SetValue(obj34, obj32);
						}
					}
					else
					{
						Limb obj35 = plr.body.LimbByName(limbname);
						field.SetValue(obj35, obj32);
					}
					con.LogToConsole("Set " + plr.playername + "'s \"" + limbname + "\" field \"" + text + "\" to \"" + obj32.ToString() + "\".");
				}, allow_macros: true, null, require_body: true);
				if (!tuple.Item1)
				{
					con.LogToConsole("ERROR: Set limb field: " + text2 + "  - " + tuple.Item2);
				}
			}
			else
			{
				if (is_all)
				{
					Limb[] limbs = PlayerCamera.main.body.limbs;
					foreach (Limb obj33 in limbs)
					{
						field.SetValue(obj33, obj32);
					}
				}
				else
				{
					field.SetValue(val24, obj32);
				}
				con.LogToConsole("Set \"" + limbname + "\" field \"" + text + "\" to \"" + obj32.ToString() + "\".");
			}
		};
		obj4.argDescription = CollectionExtensions.AddToArray<(string, string)>(obj4.argDescription, ("player", "optional, name of a player to mess with"));
		Command obj5 = ConsoleScript.SearchExact("heal");
		obj5.action = (Action)delegate(string[] args)
		{
			con.CheckForWorld();
			if (args.Length > 1 && KrokoshaScavMultiplayer.network_system_is_running)
			{
				ConFailIfNetworkIsRunningAndIsClient();
				string text = args[1];
				bool everyone = text == "@a";
				Tuple<bool, string> tuple = ServerMain._PerformActionOnPlayersByName(text, delegate(NetPlayer plr)
				{
					plr.body.ResetHealth();
					if (!everyone)
					{
						con.LogToConsole("Healed: " + plr.playername);
					}
				}, allow_macros: true, null, require_body: true);
				if (tuple.Item1)
				{
					if (everyone)
					{
						con.LogToConsole("Healed everyone.");
					}
				}
				else
				{
					con.LogToConsole("ERROR: Heal: " + text + "  - " + tuple.Item2);
				}
			}
			else
			{
				PlayerCamera.main.body.ResetHealth();
				con.LogToConsole("Healed the player.");
			}
		};
		obj5.argDescription = CollectionExtensions.AddToArray<(string, string)>(obj5.argDescription, ("player", "optional, name of a player to heal"));
		Command obj6 = ConsoleScript.SearchExact("addliquid");
		obj6.action = (Action)delegate(string[] args)
		{
			con.CheckArgumentCount(args, 2);
			con.CheckForWorld();
			if (KrokoshaScavMultiplayer.network_system_is_running && args.Length > 3)
			{
				ConFailIfNetworkIsRunningAndIsClient();
				string text = args[3];
				bool flag = text == "@a";
				Tuple<bool, string> tuple = ServerMain._PerformActionOnPlayersByName(text, delegate(NetPlayer plr)
				{
					if (!((Object)(object)plr.body == (Object)null))
					{
						Item item2 = plr.body.GetItem(0);
						if (!Object.op_Implicit((Object)(object)item2))
						{
							con.LogToConsole($"{plr} - Not holding an item in the main hand!");
						}
						else
						{
							WaterContainerItem component2 = ((Component)item2).GetComponent<WaterContainerItem>();
							if (!Object.op_Implicit((Object)(object)component2))
							{
								con.LogToConsole($"{plr} - " + "\"" + item2.fullName + "\" is not a liquid container!");
							}
							else
							{
								string text3 = args[1];
								float num2 = con.ParseFloat(args[2]);
								if (!Liquids.LiquidExists(text3))
								{
									con.LogToConsole($"{plr} - " + "\"" + text3 + "\" is not a valid liquid id!");
								}
								else
								{
									component2.AddLiquid(text3, num2);
									con.LogToConsole($"Added {num2}mL of {text3} to {item2.fullName} ({component2.CurrentTotal}mL/{component2.Capacity}mL).");
								}
							}
						}
					}
				}, allow_macros: true, null, require_body: true);
				if (tuple.Item1)
				{
					if (flag)
					{
						con.LogToConsole("addliquided everyone.");
					}
				}
				else
				{
					con.LogToConsole("ERROR: addliquid: " + text + "  - " + tuple.Item2);
				}
			}
			else
			{
				Item item = PlayerCamera.main.body.GetItem(0);
				if (!Object.op_Implicit((Object)(object)item))
				{
					throw new Exception("Not holding an item in the main hand!");
				}
				WaterContainerItem component = ((Component)item).GetComponent<WaterContainerItem>();
				if (!Object.op_Implicit((Object)(object)component))
				{
					throw new Exception("\"" + item.fullName + "\" is not a liquid container!");
				}
				string text2 = args[1];
				float num = con.ParseFloat(args[2]);
				if (!Liquids.LiquidExists(text2))
				{
					throw new Exception("\"" + text2 + "\" is not a valid liquid id!");
				}
				component.AddLiquid(text2, num);
				con.LogToConsole($"Added {num}mL of {text2} to {item.fullName} ({component.CurrentTotal}mL/{component.Capacity}mL).");
			}
		};
		obj6.argDescription = CollectionExtensions.AddToArray<(string, string)>(obj6.argDescription, ("player", "optional, name of a player to addliquid"));
		ConsoleScript.SearchExact("alert").action = (Action)delegate(string[] args)
		{
			con.CheckArgumentCount(args, 2);
			con.CheckForWorld();
			string text = string.Join(" ", args.Skip(2));
			bool important = con.ParseBool(args[1]);
			Util.DoAlert(in text, in important);
			if (KrokoshaScavMultiplayer.IsNetworkActiveAndIsServer())
			{
				ServerMain.Server_AnnounceAlert(in text, important, reliable: true, ServerMain.AllClientIdsExceptHost);
				con.LogToConsole("Announced display alert \"" + text + "\"");
			}
			else
			{
				con.LogToConsole("Displayed alert \"" + text + "\"");
			}
		};
		Command obj7 = ConsoleScript.SearchExact("addxp");
		obj7.action = (Action)delegate(string[] args)
		{
			con.CheckArgumentCount(args, 2);
			con.CheckForWorld();
			float num5 = Mathf.Max(con.ParseFloat(args[2]), 0f);
			if (args.Length > 3 && KrokoshaScavMultiplayer.network_system_is_running)
			{
				ConFailIfNetworkIsRunningAndIsClient();
				string text = args[3];
				Tuple<bool, string> tuple = ServerMain._PerformActionOnPlayersByName(text, delegate(NetPlayer plr)
				{
					switch (args[1])
					{
					case "str":
						plr.body.skills.AddExp(0, num5);
						con.LogToConsole($"Gave {plr.playername} {Mathf.RoundToInt(num5)} strength experience.");
						break;
					case "res":
						plr.body.skills.AddExp(1, num5);
						con.LogToConsole($"Gave {plr.playername} {Mathf.RoundToInt(num5)} resilience experience.");
						break;
					case "int":
						plr.body.skills.AddExp(2, num5);
						con.LogToConsole($"Gave {plr.playername} {Mathf.RoundToInt(num5)} intelligence experience.");
						break;
					default:
						con.LogToConsole("\"" + args[1] + "\" is not a valid skill type!");
						break;
					}
				}, allow_macros: true, null, require_body: true);
				if (!tuple.Item1)
				{
					con.LogToConsole("ERROR: Set experience: " + text + "  - " + tuple.Item2);
				}
			}
			else
			{
				Body body = PlayerCamera.main.body;
				switch (args[1])
				{
				case "str":
					body.skills.AddExp(0, num5);
					con.LogToConsole($"Gave the player {Mathf.RoundToInt(num5)} strength experience.");
					break;
				case "res":
					body.skills.AddExp(1, num5);
					con.LogToConsole($"Gave the player {Mathf.RoundToInt(num5)} resilience experience.");
					break;
				case "int":
					body.skills.AddExp(2, num5);
					con.LogToConsole($"Gave the player {Mathf.RoundToInt(num5)} intelligence experience.");
					break;
				default:
					throw new Exception("\"" + args[1] + "\" is not a valid skill type!");
				}
			}
		};
		obj7.argDescription = CollectionExtensions.AddToArray<(string, string)>(obj7.argDescription, ("player", "optional, name of a player to mess with"));
		Command obj8 = ConsoleScript.SearchExact("resetskills");
		obj8.action = (Action)delegate(string[] args)
		{
			con.CheckForWorld();
			if (args.Length > 1 && KrokoshaScavMultiplayer.network_system_is_running)
			{
				ConFailIfNetworkIsRunningAndIsClient();
				string text = args[1];
				Tuple<bool, string> tuple = ServerMain._PerformActionOnPlayersByName(text, delegate(NetPlayer plr)
				{
					Body body2 = plr.body;
					body2.skills.STR = 0;
					body2.skills.RES = 0;
					body2.skills.INT = 0;
					body2.skills.expSTR = 0f;
					body2.skills.expRES = 0f;
					body2.skills.expINT = 0f;
					body2.skills.UpdateExpBoundaries();
					con.LogToConsole("Reset all " + plr.playername + " skills.");
				}, allow_macros: true, null, require_body: true);
				if (!tuple.Item1)
				{
					con.LogToConsole("ERROR: Reset experience: " + text + "  - " + tuple.Item2);
				}
			}
			else
			{
				Body body = PlayerCamera.main.body;
				body.skills.STR = 0;
				body.skills.RES = 0;
				body.skills.INT = 0;
				body.skills.expSTR = 0f;
				body.skills.expRES = 0f;
				body.skills.expINT = 0f;
				body.skills.UpdateExpBoundaries();
				con.LogToConsole("Reset all player skills.");
			}
		};
		obj8.argDescription = CollectionExtensions.AddToArray<(string, string)>(obj8.argDescription, ("player", "optional, name of a player to mess with"));
		HashSet<string> hashSet = new HashSet<string>
		{
			"help", "heal", "coagulate", "kill", "spawn", "spawncategory", "tp", "skiplayer", "skiptext", "log",
			"talk", "framerate", "alert", "saveandquit", "resetskills", "fucklore", "timescale", "setconsoleheight", "setconsolecolor", "copylog",
			"clear", "addxp", "loglocale", "nukeplayerprefs", "openfolder", "setbodyfield", "setlimbfield", "amputate", "unchipped", "addcustomcommand",
			"addliquid", "locate", "removecustomcommand", "music", "bind", "repeat", "explode", "floodfill", "echo", "ui",
			"freecam", "starterkit", "noclip", "playsound", "fullbright", "plushies", "errorlogging"
		};
		HashSet<string> hashSet2 = new HashSet<string>
		{
			"krok rule EnableNametags", "krok rules", "krok help", "krok maxplayers", "krok kick", "pixelate", "help", "setvolume", "talk", "log",
			"framerate", "volume", "saveandquit", "fucklore", "setconsoleheight", "setconsolecolor", "copylog", "clear", "loglocale", "openfolder",
			"addcustomcommand", "removecustomcommand", "music", "bind", "echo", "errorlogging"
		};
		foreach (Command command in ConsoleScript.Commands)
		{
			if (hashSet.Contains(command.name) && !hashSet2.Contains(command.name))
			{
				Action og_action = command.action;
				command.action = (Action)delegate(string[] args)
				{
					ConFailIfCheatsDisabled();
					og_action.Invoke(args);
				};
			}
		}
		HashSet<string> hashSet3 = new HashSet<string> { "spawn" };
		ConsoleScript.Commands.Select((Command x) => x.name).ToArray();
		foreach (Command command2 in ConsoleScript.Commands)
		{
			if (hashSet3.Contains(command2.name))
			{
				Action og_action2 = command2.action;
				command2.action = (Action)delegate(string[] args)
				{
					ConFailIfNetworkIsRunningAndIsClient();
					og_action2.Invoke(args);
				};
			}
		}
		if (Plugin.dump_game_ids)
		{
			log.l("all og commands: \n" + string.Join("\n", ConsoleScript.Commands.Select((Command x) => x.name).ToArray()));
		}
		object obj9 = _003C_003Ec._003C_003E9__44_1;
		if (obj9 == null)
		{
			Action val = delegate(string[] splited)
			{
				if (splited.Count() < 2)
				{
					Chat._DEV_LOG_CHAT = !Chat._DEV_LOG_CHAT;
				}
				else
				{
					Chat._DEV_LOG_CHAT = ParseBool01(splited[1]);
				}
				_DEV_CHATSPY = Chat._DEV_LOG_CHAT;
				if (!CanCheat())
				{
					_DEV_CHATSPY = false;
					log.l($"_DEV_LOG_CHAT = {Chat._DEV_LOG_CHAT}");
				}
				else
				{
					log.l($"_DEV_CHATSPY = {_DEV_CHATSPY} ");
				}
			};
			_003C_003Ec._003C_003E9__44_1 = val;
			obj9 = (object)val;
		}
		RegisterCommand(Unsafe.As<Command, Command>(ref new Command("chatspy", "MP- force to show and log all chat messages", (Action)obj9, (Dictionary<int, List<string>>)null, Array.Empty<(string, string)>())));
		RegisterCommand_LocalOnly(Unsafe.As<Command, Command>(ref new Command("talker-on-cursor-say", "MP - Hijack object's Talker component.", (Action)delegate(string[] splited)
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			con.CheckArgumentCount(splited, 1);
			string msg = string.Join(" ", splited.Skip(1));
			string name = "";
			GameObject val24 = null;
			Talker talker = null;
			Collider2D[] array = Physics2D.OverlapPointAll(Util.GetCursorWorldPos());
			foreach (Collider2D val25 in array)
			{
				if (Util.TryGetTalkerOnObject(((Component)val25).gameObject, out talker, out name))
				{
					val24 = ((Component)val25).gameObject;
					break;
				}
			}
			if ((Object)(object)talker != (Object)null)
			{
				if (KrokoshaScavMultiplayer.IsNetworkActiveAndIsServer())
				{
					NetBody pb = default(NetBody);
					if (NetObjectRegistry.TryGetSyncInfo(val24, out var si))
					{
						if (!ServerMain.ForceTalkerSayAndAnnounce(in si, in msg))
						{
							con.LogToConsole("Failed to make " + name + " say \"" + msg + "\"");
							return;
						}
					}
					else if (val24.TryGetComponent<NetBody>(ref pb))
					{
						if (!ServerMain.ForceTalkerSayAndAnnounce(in pb, in msg))
						{
							con.LogToConsole("Failed to make " + name + " say \"" + msg + "\"");
							return;
						}
					}
					else
					{
						NetObjectRegistry.AlertObjectNotRegistered(popup: true);
					}
				}
				talker.ForceNoSpeechImpairment(msg, resetTalkTimer: true);
				con.LogToConsole("Made " + name + " say \"" + msg + "\"");
			}
			else
			{
				con.LogToConsole("No talker was found at cursor.");
			}
		}, (Dictionary<int, List<string>>)null, Array.Empty<(string, string)>())));
		RegisterCommand(Unsafe.As<Command, Command>(ref new Command("earthquake", "MP - control earthquake", (Action)delegate(string[] args)
		{
			ConFailIfCheatsDisabled();
			ConFailIfNetworkIsRunningAndIsClient();
			con.CheckArgumentCount(args, 1);
			con.CheckForWorld();
			WorldGeneration world = WorldGeneration.world;
			string text = args[1].ToLower();
			if (text == "now")
			{
				world.earthquakeDelay = Random.Range(600f, 1750f);
				world.earthquakeTime = Random.Range(3f, 25f);
				Time.timeScale = 1f;
				con.LogToConsole("Started earthquake right now.");
			}
			else
			{
				WorldgenPatches.earthquake_enabled = ParseBool01(text);
				if (!WorldgenPatches.earthquake_enabled)
				{
					world.earthquakeTime = 0f;
				}
				con.LogToConsole($"Earthquake active: {WorldgenPatches.earthquake_enabled}");
			}
		}, new Dictionary<int, List<string>> { 
		{
			0,
			new List<string> { "now", "disable", "enable" }
		} }, new(string, string)[1] { ("string", "Action") })));
		object obj10 = _003C_003Ec._003C_003E9__44_4;
		if (obj10 == null)
		{
			Action val2 = delegate
			{
				//IL_0037: Unknown result type (might be due to invalid IL or missing references)
				goof_carameltanzen = !goof_carameltanzen;
				if (!goof_carameltanzen)
				{
					KrokoshaScavMultiplayer.BurgerkingFootLettuce[] array = Object.FindObjectsOfType<KrokoshaScavMultiplayer.BurgerkingFootLettuce>();
					foreach (KrokoshaScavMultiplayer.BurgerkingFootLettuce burgerkingFootLettuce in array)
					{
						if ((Object)(object)burgerkingFootLettuce.l != (Object)null)
						{
							burgerkingFootLettuce.l.color = burgerkingFootLettuce.c;
						}
					}
				}
				log.l($"rgblights {goof_carameltanzen}");
			};
			_003C_003Ec._003C_003E9__44_4 = val2;
			obj10 = (object)val2;
		}
		RegisterCommand_LocalOnly(Unsafe.As<Command, Command>(ref new Command("rgblights", "DELETEME", (Action)obj10, (Dictionary<int, List<string>>)null, Array.Empty<(string, string)>())));
		RegisterCommand(Unsafe.As<Command, Command>(ref new Command("dumpbodyvars", "MP - dump all fields of a body (for easy health desync debugging)", (Action)delegate(string[] splited)
		{
			con.CheckForWorld();
			ConFailIfCheatsDisabled();
			if (!Net.running)
			{
				log.l(Util.GetLocalBody().DumpBodyVars());
			}
			else
			{
				string plrname = "@m";
				if (splited.Length > 1)
				{
					plrname = splited[1];
				}
				bool isall = plrname == "@a";
				bool hasplr = false;
				Action<NetBody> func = delegate(NetBody npc)
				{
					hasplr = true;
					if (!isall)
					{
						plrname = npc.bodyname;
					}
					log.l(((object)npc).ToString() + " " + npc.body.DumpBodyVars());
				};
				ServerMain._PerformActionOnBodiesByName(plrname, func, allow_macros: true, only_players: false);
				if (!hasplr)
				{
					con.LogToConsole(plrname + " -> body not found. ");
				}
			}
		}, new Dictionary<int, List<string>> { 
		{
			1,
			(from x in Resources.LoadAll<GameObject>("")
				where Object.op_Implicit((Object)(object)x.GetComponent<Item>())
				select ((Object)x).name).ToList()
		} }, new(string, string)[2]
		{
			("player", "to dumpbodyvars to"),
			("string item_id", "Item resource ID, multiple is allowed")
		})));
		RegisterCommand(Unsafe.As<Command, Command>(ref new Command("give", "MP - give an item to a player", (Action)delegate(string[] splited)
		{
			ConFailIfNetworkIsRunningAndIsClient();
			ConFailIfCheatsDisabled();
			con.CheckArgumentCount(splited, 2);
			List<string> items = splited.ToList().GetRange(2, splited.Length - 2);
			string plrname = splited[1];
			bool isall = plrname == "@a";
			bool hasplr = false;
			bool succ = false;
			Action<NetBody> func = delegate(NetBody npc)
			{
				hasplr = true;
				if (!isall)
				{
					plrname = npc.bodyname;
				}
				List<string> list = items;
				foreach (string item3 in items)
				{
					if (Object.op_Implicit((Object)(object)SpawnThingOnPlayer(item3, npc.body, give_it_to_em: true)))
					{
						succ = true;
					}
					else
					{
						list = new List<string>(list);
						list.Remove(item3);
						con.LogToConsole(item3 + " is not an item.");
					}
				}
				items = list;
				if (succ && list.Count > 0)
				{
					con.LogToConsole(npc.bodyname + " got the " + string.Join(" ", list));
				}
			};
			ServerMain._PerformActionOnBodiesByName(plrname, func, allow_macros: true, only_players: false);
			if (!hasplr)
			{
				con.LogToConsole(plrname + " -> body not found. ");
			}
		}, new Dictionary<int, List<string>> { 
		{
			1,
			(from x in Resources.LoadAll<GameObject>("")
				where Object.op_Implicit((Object)(object)x.GetComponent<Item>())
				select ((Object)x).name).ToList()
		} }, new(string, string)[2]
		{
			("player", "to give to"),
			("string item_id", "Item resource ID, multiple is allowed")
		})));
		RegisterCommand(Unsafe.As<Command, Command>(ref new Command("respawn", "MP - respawn a player", (Action)delegate(string[] splited)
		{
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			ConFailIfCheatsDisabled();
			ConFailIfNetworkIsRunningAndIsClient();
			con.CheckForWorld();
			ConFailIfNetworkNotRunning();
			con.CheckArgumentCount(splited, 1);
			Vector2 goalpos = default(Vector2);
			if (splited.Length >= 3)
			{
				goalpos = con.ParsePosition(splited[2]);
			}
			else
			{
				goalpos = Vector2.op_Implicit(Camera.main.ScreenToWorldPoint(Input.mousePosition));
			}
			string plrname = splited[1];
			bool isall = plrname == "@a";
			bool succ = false;
			Tuple<bool, string> tuple = ServerMain._PerformActionOnPlayersByName(plrname, delegate(NetPlayer plr)
			{
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				if (!isall)
				{
					plrname = plr.playername;
				}
				plr.Server_RespawnCharacter(goalpos);
				succ = true;
			}, allow_macros: true, null, require_body: true);
			if (succ && tuple.Item1)
			{
				con.LogToConsole("Respawned: " + plrname);
			}
			else if (tuple.Item1)
			{
				con.LogToConsole("Failed to respawn: " + plrname);
			}
			else
			{
				con.LogToConsole("Failed to respawn: " + plrname + " " + tuple.Item2);
			}
		}, new Dictionary<int, List<string>>(), new(string, string)[2]
		{
			("player", "to respawn to"),
			("position", "optional")
		})));
		RegisterCommand(Unsafe.As<Command, Command>(ref new Command("clearinventory", "MP - clear inventory of a player", (Action)delegate(string[] splited)
		{
			ConFailIfCheatsDisabled();
			con.CheckForWorld();
			bool delete = false;
			if (splited.Length > 1)
			{
				delete = con.ParseBool(splited[1]);
			}
			Action<Body> dothing = delegate(Body b)
			{
				if (delete)
				{
					foreach (Item item4 in b.GetAllItemsThorough())
					{
						Object.Destroy((Object)(object)((Component)item4).gameObject);
					}
					b.Body_DropAllItems();
				}
				else
				{
					b.Body_DropAllItems();
				}
			};
			if (KrokoshaScavMultiplayer.network_system_is_running && splited.Length > 2)
			{
				string plrname = splited[2];
				bool isall = plrname == "@a";
				bool succ = false;
				Tuple<bool, string> tuple = ServerMain._PerformActionOnPlayersByName(plrname, delegate(NetPlayer plr)
				{
					if (!isall)
					{
						plrname = plr.playername;
					}
					dothing(plr.body);
					succ = true;
				}, allow_macros: true, null, require_body: true);
				if (succ && tuple.Item1)
				{
					con.LogToConsole("Cleared inventory: " + plrname);
				}
				else if (tuple.Item1)
				{
					con.LogToConsole("Failed to clear inventory: " + plrname);
				}
				else
				{
					con.LogToConsole("Failed to clear inventory: " + plrname + " " + tuple.Item2);
				}
			}
			else
			{
				dothing(Util.GetLocalBody());
			}
		}, new Dictionary<int, List<string>>(), new(string, string)[2]
		{
			("bool", "delete items"),
			("player", "optional")
		})));
		object obj11 = _003C_003Ec._003C_003E9__44_9;
		if (obj11 == null)
		{
			Action val3 = delegate(string[] splited)
			{
				ConFailIfNetworkIsRunningAndIsClient();
				bool dEV_GODMODE = !_DEV_GODMODE;
				if (splited.Length > 1)
				{
					dEV_GODMODE = ParseBool01(splited[1]);
				}
				_DEV_GODMODE = dEV_GODMODE;
				ConsoleScript.instance.LogToConsole($"godmode {_DEV_GODMODE}");
			};
			_003C_003Ec._003C_003E9__44_9 = val3;
			obj11 = (object)val3;
		}
		RegisterCommand(Unsafe.As<Command, Command>(ref new Command("godmode", "MP - revive every player and keep resetting their health", (Action)obj11, (Dictionary<int, List<string>>)null, Array.Empty<(string, string)>())));
		RegisterCommand_LocalOnly(Unsafe.As<Command, Command>(ref new Command("zoom", "MP - zoom camera", (Action)delegate(string[] splited)
		{
			con.CheckArgumentCount(splited, 1);
			con.CheckForWorld();
			if (!float.TryParse(splited[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
			{
				ConsoleScript.instance.LogToConsole("Thats not a nubmer bro");
			}
			else
			{
				if ((double)result < 0.001)
				{
					throw new ArgumentOutOfRangeException("Zoom can't be 0 or your perception of reality will shatter.");
				}
				if (Mathf.Abs(result) < 1f && !CanCheat())
				{
					result = 1f;
				}
				Camera.main.orthographicSize = Camera.main.orthographicSize * _DEV_ZOOM / result;
				PixelPerfectCamera component = ((Component)PlayerCamera.main).GetComponent<PixelPerfectCamera>();
				component.assetsPPU = (int)(result * 8f);
				_DEV_ZOOM = result;
				ConsoleScript.instance.LogToConsole($"zoom {_DEV_ZOOM} orthographicSize={Camera.main.orthographicSize} PPU={component.assetsPPU}");
			}
		}, (Dictionary<int, List<string>>)null, new(string, string)[1] { ("float zoom", "the bigger the number, the closer you see") })));
		RegisterCommand_LocalOnly(Unsafe.As<Command, Command>(ref new Command("spectate", "MP - spectator mode", (Action)delegate(string[] splited)
		{
			bool flag = !UIInGame.SPECTATOR_MODE;
			if (splited.Length > 1)
			{
				flag = ParseBool01(splited[1]);
			}
			if (flag)
			{
				ConFailIfCheatsDisabled();
				UIInGame.StartSpectatorMode();
			}
			else
			{
				UIInGame.StopSpectatorMode();
			}
			con.LogToConsole($"Spectator mode: {UIInGame.SPECTATOR_MODE}");
		}, (Dictionary<int, List<string>>)null, Array.Empty<(string, string)>())));
		object obj12 = _003C_003Ec._003C_003E9__44_12;
		if (obj12 == null)
		{
			Action val4 = delegate(string[] splited)
			{
				ConFailIfNetworkIsRunningAndIsClient();
				bool flag = !UIMainMenu.mainmenu_open;
				if (splited.Length > 1)
				{
					flag = ParseBool01(splited[1]);
				}
				UIMainMenu.SetOpen(flag);
				ConsoleScript.instance.LogToConsole($"UIMainMenu.SetOpen({flag});");
			};
			_003C_003Ec._003C_003E9__44_12 = val4;
			obj12 = (object)val4;
		}
		RegisterCommand(Unsafe.As<Command, Command>(ref new Command("mpopenmenu", "MP", (Action)obj12, (Dictionary<int, List<string>>)null, Array.Empty<(string, string)>())));
		object obj13 = _003C_003Ec._003C_003E9__44_13;
		if (obj13 == null)
		{
			Action val5 = delegate(string[] splited)
			{
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				//IL_0026: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_0060: Unknown result type (might be due to invalid IL or missing references)
				//IL_0061: Unknown result type (might be due to invalid IL or missing references)
				//IL_0066: Unknown result type (might be due to invalid IL or missing references)
				//IL_006c: Unknown result type (might be due to invalid IL or missing references)
				ConFailIfCheatsDisabled();
				ushort num = ushort.Parse(splited[1]);
				Vector2 val24 = default(Vector2);
				if (splited.Length < 4)
				{
					val24 = Vector2.op_Implicit(Camera.main.ScreenToWorldPoint(Input.mousePosition));
				}
				else
				{
					val24.x = float.Parse(splited[2], CultureInfo.InvariantCulture);
					val24.y = float.Parse(splited[3], CultureInfo.InvariantCulture);
				}
				Vector2Int val25 = WorldGeneration.world.WorldToBlockPos(val24);
				WorldGeneration.world.SetBlock(val25, num);
				ConsoleScript.instance.LogToConsole($"Set tile {((Vector2Int)(ref val25)).x} {((Vector2Int)(ref val25)).y} id to {num} ({WorldGeneration.world.GetBlockInfo(num)?.name})");
			};
			_003C_003Ec._003C_003E9__44_13 = val5;
			obj13 = (object)val5;
		}
		RegisterCommand(Unsafe.As<Command, Command>(ref new Command("settile", "MP", (Action)obj13, (Dictionary<int, List<string>>)null, Array.Empty<(string, string)>())));
		object obj14 = _003C_003Ec._003C_003E9__44_14;
		if (obj14 == null)
		{
			Action val6 = delegate(string[] splited)
			{
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				//IL_0021: Unknown result type (might be due to invalid IL or missing references)
				//IL_0026: Unknown result type (might be due to invalid IL or missing references)
				//IL_002b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_0060: Unknown result type (might be due to invalid IL or missing references)
				//IL_0061: Unknown result type (might be due to invalid IL or missing references)
				//IL_0066: Unknown result type (might be due to invalid IL or missing references)
				ConFailIfCheatsDisabled();
				byte b = byte.Parse(splited[1]);
				Vector2 val24 = default(Vector2);
				if (splited.Length < 4)
				{
					val24 = Vector2.op_Implicit(Camera.main.ScreenToWorldPoint(Input.mousePosition));
				}
				else
				{
					val24.x = float.Parse(splited[2], CultureInfo.InvariantCulture);
					val24.y = float.Parse(splited[3], CultureInfo.InvariantCulture);
				}
				Vector2Int val25 = WorldGeneration.world.WorldToBlockPos(val24);
				FluidManager.main.fluid[((Vector2Int)(ref val25)).x, ((Vector2Int)(ref val25)).y] = b;
				ConsoleScript.instance.LogToConsole($"Set fluid {((Vector2Int)(ref val25)).x} {((Vector2Int)(ref val25)).y} id to {b} ({FluidManager.WorldFluidToLiquidID[b]})");
			};
			_003C_003Ec._003C_003E9__44_14 = val6;
			obj14 = (object)val6;
		}
		RegisterCommand(Unsafe.As<Command, Command>(ref new Command("setfluid", "MP", (Action)obj14, (Dictionary<int, List<string>>)null, Array.Empty<(string, string)>())));
		object obj15 = _003C_003Ec._003C_003E9__44_15;
		if (obj15 == null)
		{
			Action val7 = delegate(string[] splited)
			{
				//IL_002e: Unknown result type (might be due to invalid IL or missing references)
				string text = string.Join(" ", splited);
				if (Net.is_server)
				{
					ServerMain.RunClientCustomCommand(text, null);
				}
				else
				{
					NetDataWriter writer = Net.CreateWriter(10184);
					writer.Put(text);
					Net.Client_Send((DeliveryMethod)2, in writer);
				}
			};
			_003C_003Ec._003C_003E9__44_15 = val7;
			obj15 = (object)val7;
		}
		RegisterCommand_LocalOnly(Unsafe.As<Command, Command>(ref new Command("vote", "MP- Call a vote.", (Action)obj15, (Dictionary<int, List<string>>)null, new(string, string)[2]
		{
			("type", "kick/mutevc/mutetc"),
			("player", "")
		})));
		RegisterCommand(Unsafe.As<Command, Command>(ref new Command("createvote", "MP-SERVER- Call a custom vote.", (Action)delegate(string[] splited)
		{
			ConFailIfNetworkIsRunningAndIsClient();
			ConFailIfNetworkNotRunning();
			con.CheckArgumentCount(splited, 5);
			int num = 1;
			string toRun = splited[num++];
			float timetovote = con.ParseFloat(splited[num++]);
			float percent = con.ParseFloat(splited[num++]);
			percent = Mathf.Clamp(percent, -100f, 100f) / 100f;
			IEnumerable<string> values = splited.Skip(4);
			string.Join(" ", values).Trim();
			string title = "vote title";
			string message = "vote text";
			string text = splited.ElementAtOrDefault(num++);
			string text2 = splited.ElementAtOrDefault(num++);
			if (text != null)
			{
				title = text.Replace('_', ' ');
			}
			if (text2 != null)
			{
				message = text2.Replace('_', ' ');
			}
			VoteSystem.Server_AnnounceVote(in title, in message, in timetovote, (VoteSystem.VoteEndAction)delegate(HashSet<NetPlayer> voted_yes, HashSet<NetPlayer> voted_no, HashSet<NetPlayer> voted_ignore)
			{
				float num2 = voted_yes.Count + voted_no.Count + voted_ignore.Count;
				float num3 = voted_yes.Count;
				float num4 = voted_no.Count;
				if (num2 > 0f)
				{
					float num5 = 0f;
					num5 = ((!(percent >= 0f)) ? (num4 / num2) : (num3 / num2));
					Chat.Server_ChatAnnouncement($"Vote result: {Mathf.RoundToInt(num5 * 100f)}%/{Mathf.RoundToInt(percent * 100f)}%");
					if (num5 >= Mathf.Abs(percent))
					{
						con.RunCommandString(toRun);
					}
				}
			});
		}, (Dictionary<int, List<string>>)null, new(string, string)[5]
		{
			("string command", "Command to run if vote is accepted."),
			("float time", "Time for vote. (Default: 10)"),
			("float percent", "Percentage of votes required. (Default: 50)"),
			("string", "Title"),
			("string", "Text")
		})));
		RegisterCommand(Unsafe.As<Command, Command>(ref new Command("kick", "MP-SERVER- Forcefully disconnect player.", (Action)delegate(string[] splited)
		{
			ConFailIfNetworkIsRunningAndIsClient();
			ConFailIfNetworkNotRunning();
			con.CheckArgumentCount(splited, 1);
			string reason = "Kicked!";
			if (splited.Count() > 2)
			{
				reason = "Kicked: " + string.Join(" ", splited.Skip(2));
			}
			string plrname = splited[1];
			Tuple<bool, string> tuple = ServerMain._PerformActionOnPlayersByName(plrname, delegate(NetPlayer plr)
			{
				if (!plr.is_local)
				{
					Plugin.log.LogInfo((object)$"Server entered command to kick: {plr}");
					plr.Server_Kick(reason);
					if (plrname != "@a")
					{
						plrname = plr.playername;
					}
				}
			}, allow_macros: false, NetPlayer.LOCAL_PLAYER);
			if (tuple.Item1)
			{
				log.l("Kicked: " + plrname + " - " + reason);
			}
			else
			{
				log.l("Failed to kick: " + plrname + " " + tuple.Item2);
			}
		}, (Dictionary<int, List<string>>)null, new(string, string)[2]
		{
			("player", ""),
			("string reason", "optional")
		})));
		RegisterCommand(Unsafe.As<Command, Command>(ref new Command("ban", "MP-SERVER- Ban player.", (Action)delegate(string[] splited)
		{
			ConFailIfNetworkIsRunningAndIsClient();
			con.CheckArgumentCount(splited, 1);
			string plrname = splited[1];
			string reason = "Banned!";
			if (splited.Count() > 2)
			{
				reason = "Banned: " + string.Join(" ", splited.Skip(2));
			}
			Tuple<bool, string> tuple = ServerMain._PerformActionOnPlayersByName(plrname, delegate(NetPlayer plr)
			{
				if (!plr.is_local)
				{
					Plugin.log.LogInfo((object)$"Server entered a command to ban: {plr}");
					BanList.Add("NULL", plr.playername, plr.steam_id);
					plr.Server_Kick(reason);
					if (plrname != "@a")
					{
						plrname = plr.playername;
					}
				}
			}, allow_macros: false, NetPlayer.LOCAL_PLAYER);
			ulong result;
			if (tuple.Item1)
			{
				log.l("Banned: " + plrname + " - " + reason);
			}
			else if (ulong.TryParse(plrname, out result) && result != 0L && plrname.StartsWith("7656119"))
			{
				BanList.Add("NULL", "", result);
				log.l($"Banned Steam ID: {result} ");
			}
			else
			{
				log.l("Failed to Ban: " + plrname + " " + tuple.Item2);
			}
		}, (Dictionary<int, List<string>>)null, new(string, string)[2]
		{
			("player", ""),
			("string reason", "optional")
		})));
		RegisterCommand(Unsafe.As<Command, Command>(ref new Command("ipban", "MP-SERVER- IP Ban player. (Non-steam only)", (Action)delegate(string[] splited)
		{
			ConFailIfNetworkIsRunningAndIsClient();
			con.CheckArgumentCount(splited, 1);
			string plrname = splited[1];
			NetTransportBase tRANSPORT = Net.TRANSPORT;
			TransportLiteNetLib lnl = tRANSPORT as TransportLiteNetLib;
			if (lnl == null)
			{
				throw new Exception("Only available for direct IP connection mode.");
			}
			string reason = "Banned!";
			if (splited.Count() > 2)
			{
				reason = "Banned: " + string.Join(" ", splited.Skip(2));
			}
			Tuple<bool, string> tuple = ServerMain._PerformActionOnPlayersByName(plrname, delegate(NetPlayer plr)
			{
				if (!plr.is_local)
				{
					NetPeer obj32 = lnl.GetNetPeerFromPlayer(plr) ?? throw new Exception($"{plr} does not have net peer object ????");
					Plugin.log.LogInfo((object)$"Server entered a command to IP ban: {plr}");
					BanList.Add(((IPEndPoint)(object)obj32).Address.ToString(), plr.playername, plr.steam_id);
					plr.Server_Kick(reason);
					if (plrname != "@a")
					{
						plrname = plr.playername;
					}
				}
			}, allow_macros: false, NetPlayer.LOCAL_PLAYER);
			string ip;
			ushort port;
			if (tuple.Item1)
			{
				log.l("Banned: " + plrname + " - " + reason);
			}
			else if (Net.TryParseIPPORT(plrname, out ip, out port))
			{
				BanList.Add(ip, "", 0uL);
				log.l("Banned IP: " + ip + " ");
			}
			else
			{
				log.l("Failed to Ban: " + plrname + " " + tuple.Item2);
			}
		}, (Dictionary<int, List<string>>)null, new(string, string)[2]
		{
			("player", ""),
			("string reason", "optional")
		})));
		object obj16 = _003C_003Ec._003C_003E9__44_20;
		if (obj16 == null)
		{
			Action val8 = delegate
			{
				log.l("BANLIST: " + BanList.Instance.filePath + "\n" + string.Join("\n", BanList.Entries) + " ");
			};
			_003C_003Ec._003C_003E9__44_20 = val8;
			obj16 = (object)val8;
		}
		RegisterCommand(Unsafe.As<Command, Command>(ref new Command("banlist", "MP- Show all players you've banned. Local.", (Action)obj16, (Dictionary<int, List<string>>)null, Array.Empty<(string, string)>())));
		RegisterCommand(Unsafe.As<Command, Command>(ref new Command("unban", "MP- Remove a player from your ban list.", (Action)delegate(string[] splited)
		{
			con.CheckArgumentCount(splited, 1);
			string plrname = splited[1];
			BanList.BanEntry banEntry = null;
			if (ulong.TryParse(plrname, out var parsed_as_steamid) && parsed_as_steamid != 0L)
			{
				banEntry = BanList.Entries.FirstOrDefault((BanList.BanEntry x) => x.steamid == parsed_as_steamid);
			}
			if (banEntry == null)
			{
				banEntry = BanList.Entries.FirstOrDefault((BanList.BanEntry x) => x.ip == plrname);
			}
			if (banEntry == null)
			{
				banEntry = BanList.Entries.FirstOrDefault((BanList.BanEntry x) => x.name == plrname);
			}
			if (banEntry != null)
			{
				BanList.Remove(banEntry);
				log.l("Unbanned: " + banEntry.ToString());
			}
			else
			{
				log.l("\"" + plrname + "\" was not found in your ban list.");
			}
		}, (Dictionary<int, List<string>>)null, new(string, string)[1] { ("user", "SteamID or IP or Name") })));
		object obj17 = _003C_003Ec._003C_003E9__44_22;
		if (obj17 == null)
		{
			Action val9 = delegate(string[] splited)
			{
				ConFailIfNetworkNotRunning();
				ConsoleScript instance = ConsoleScript.instance;
				string message = string.Join(" ", splited.Skip(1));
				Chat.SendChatMessage(in message, force_server_if_server: true);
				instance.LogToConsole("Chat message sent: " + message);
			};
			_003C_003Ec._003C_003E9__44_22 = val9;
			obj17 = (object)val9;
		}
		RegisterCommand(Unsafe.As<Command, Command>(ref new Command("say", "MP - say something in chat", (Action)obj17, (Dictionary<int, List<string>>)null, new(string, string)[1] { ("string", "message") })));
		RegisterCommand(Unsafe.As<Command, Command>(ref new Command("skybg", "MP dev", (Action)delegate(string[] splited)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			con.CheckForWorld();
			bool flag = false;
			float rainIntensity = 0f;
			Color skyColor = Color.white;
			if (splited.Length == 3)
			{
				flag = true;
				skyColor = con.ParseColor(splited[1]);
				rainIntensity = con.ParseFloat(splited[2]);
			}
			if (WorldgenPatches.HasSkyBackground())
			{
				WorldgenPatches.RemoveSkyBackground();
			}
			else
			{
				ConFailIfCheatsDisabled();
				if (flag)
				{
					WorldgenPatches.CreateSkyBackground(skyColor, rainIntensity);
				}
				else
				{
					WorldgenPatches.CreateSkyBackground();
				}
			}
			ConsoleScript.instance.LogToConsole($"skybg {WorldgenPatches.HasSkyBackground()} ");
		}, (Dictionary<int, List<string>>)null, new(string, string)[2]
		{
			("color", "skyColor"),
			("float", "rainIntensity")
		})));
		object obj18 = _003C_003Ec._003C_003E9__44_24;
		if (obj18 == null)
		{
			Action val10 = delegate(string[] splited)
			{
				bool active = !PlayerCamera.main.backgroundSnow.activeSelf;
				if (splited.Length > 1)
				{
					active = ParseBool01(splited[1]);
				}
				PlayerCamera.main.backgroundSnow.SetActive(active);
				ConsoleScript.instance.LogToConsole($"snowbg {PlayerCamera.main.backgroundSnow.activeSelf} ");
			};
			_003C_003Ec._003C_003E9__44_24 = val10;
			obj18 = (object)val10;
		}
		RegisterCommand_LocalOnly(Unsafe.As<Command, Command>(ref new Command("snowbg", "MP dev", (Action)obj18, (Dictionary<int, List<string>>)null, Array.Empty<(string, string)>())));
		object obj19 = _003C_003Ec._003C_003E9__44_25;
		if (obj19 == null)
		{
			Action val11 = delegate(string[] splited)
			{
				ConFailIfNetworkIsRunningAndIsClient();
				ConFailIfNotInMainMenu();
				string text = "debug";
				if (splited.Length > 1)
				{
					text = splited[1];
				}
				PreRunScript val24 = Object.FindObjectOfType<PreRunScript>();
				if (text == "debug")
				{
					log.l("Loading debug world");
					WorldgenPatches.LoadDebugWorld();
				}
				else if (text == "tutorial")
				{
					log.l("Loading tutorial world");
					val24.StartTutorial();
				}
				else
				{
					log.l("Loading vanilla world");
					val24.StartRun();
				}
			};
			_003C_003Ec._003C_003E9__44_25 = val11;
			obj19 = (object)val11;
		}
		RegisterCommand(Unsafe.As<Command, Command>(ref new Command("map", "MP dev", (Action)obj19, new Dictionary<int, List<string>> { 
		{
			0,
			new List<string> { "debug", "tutorial", "vanilla" }
		} }, Array.Empty<(string, string)>())));
		object obj20 = _003C_003Ec._003C_003E9__44_26;
		if (obj20 == null)
		{
			Action val12 = delegate(string[] splited)
			{
				bool flag = !_DEV_TELEKINESIS;
				if (splited.Length > 1)
				{
					flag = ParseBool01(splited[1]);
				}
				if (flag)
				{
					ConFailIfNetworkIsRunningAndIsClient();
					ConFailIfCheatsDisabled();
				}
				_DEV_TELEKINESIS = flag;
				ConsoleScript.instance.LogToConsole($"telekinesis {_DEV_TELEKINESIS}");
			};
			_003C_003Ec._003C_003E9__44_26 = val12;
			obj20 = (object)val12;
		}
		RegisterCommand_LocalOnly(Unsafe.As<Command, Command>(ref new Command("telekinesis", "MP dev", (Action)obj20, (Dictionary<int, List<string>>)null, Array.Empty<(string, string)>())));
		RegisterCommand(Unsafe.As<Command, Command>(ref new Command("gamemode", "MP dev", (Action)delegate(string[] splited)
		{
			ConFailIfNetworkNotRunning();
			ConFailIfNetworkIsRunningAndIsClient();
			ConFailIfNotInMainMenu();
			if (GamemodeManager.HasGamemode())
			{
				log.l($"Exited gamemode. {GamemodeManager.GetGamemode()}");
				GamemodeManager.DeleteGamemode();
			}
			else
			{
				con.CheckArgumentCount(splited, 1);
				string text = splited[1];
				Type[] allAvailableGamemodes = GamemodeManager.GetAllAvailableGamemodes();
				foreach (Type type in allAvailableGamemodes)
				{
					if (type.Name == text)
					{
						GamemodeBase gamemodeBase = GamemodeManager.SetGamemode(type);
						try
						{
							gamemodeBase.Init(splited.Skip(2).ToArray());
						}
						catch (Exception ex)
						{
							GamemodeManager.DeleteGamemode();
							log.error("Failed to Init Gamemode " + type.Name + ": " + ex.ToString());
						}
						break;
					}
				}
				con.LogToConsole("Gamemode \"" + text + "\" was not found.");
			}
		}, new Dictionary<int, List<string>> { 
		{
			0,
			(from x in GamemodeManager.GetAllAvailableGamemodes()
				select x.Name).ToList()
		} }, new(string, string)[2]
		{
			("gamemode", ""),
			("arg", "")
		})));
		RegisterCommand(Unsafe.As<Command, Command>(ref new Command("radline", "MP - Activate/Deactivate radline", (Action)delegate(string[] args)
		{
			con.CheckForWorld();
			ConFailIfNetworkIsRunningAndIsClient();
			bool flag = !RadiationLine.line.active;
			if (args.Length > 1)
			{
				flag = ConsoleScript.instance.ParseBool(args[1]);
			}
			if (flag)
			{
				RadiationLine.line.Activate();
			}
			else
			{
				RadiationLine.line.Deactivate();
			}
			ServerMain.timer_RareUpdateSyncClients += 1000f;
			ConsoleScript.instance.LogToConsole("Radiation line active: " + RadiationLine.line.active);
		}, (Dictionary<int, List<string>>)null, new(string, string)[1] { ("bool", "Activate/Deactivate radline") })));
		RegisterCommand_LocalOnly(Unsafe.As<Command, Command>(ref new Command("musicvolume", "MP", (Action)delegate(string[] splited)
		{
			float musicvolume = 0f;
			if (splited.Length > 1)
			{
				musicvolume = con.ParseFloat(splited[1]);
			}
			MusicManager_Start_MultiplayerPatch.musicvolume = musicvolume;
			ConsoleScript.instance.LogToConsole($"custom_music_volume {MusicManager_Start_MultiplayerPatch.musicvolume}");
		}, (Dictionary<int, List<string>>)null, new(string, string)[1] { ("float", "volume level 0-1") })));
		object obj21 = _003C_003Ec._003C_003E9__44_30;
		if (obj21 == null)
		{
			Action val13 = delegate(string[] splited)
			{
				bool flag = !_DEV_AUDIO_ONLY_ON_FOCUS;
				if (splited.Length > 1)
				{
					flag = ParseBool01(splited[1]);
				}
				if (_DEV_AUDIO_ONLY_ON_FOCUS != flag)
				{
					if (flag)
					{
						_DEV_LAST_VOLUME = AudioListener.volume;
					}
					else
					{
						AudioListener.volume = _DEV_LAST_VOLUME;
					}
				}
				_DEV_AUDIO_ONLY_ON_FOCUS = flag;
				ConsoleScript.instance.LogToConsole($"audio_only_on_focus {_DEV_AUDIO_ONLY_ON_FOCUS}");
			};
			_003C_003Ec._003C_003E9__44_30 = val13;
			obj21 = (object)val13;
		}
		RegisterCommand_LocalOnly(Unsafe.As<Command, Command>(ref new Command("audio-only-on-focus", "MP - vc test", (Action)obj21, (Dictionary<int, List<string>>)null, Array.Empty<(string, string)>())));
		RegisterCommand_LocalOnly(Unsafe.As<Command, Command>(ref new Command("vclistenervolume-individual", "MP - mute or change volume of a specific player, local", (Action)delegate(string[] splited)
		{
			con.CheckArgumentCount(splited, 2);
			string text = splited[1];
			float num = Mathf.Max(0f, con.ParseFloat(splited[2]));
			try
			{
				if (ServerMain.TryGetPlayerFromPartialName(text, out var player))
				{
					player.vc_output.custom_volume_set = num;
					con.LogToConsole($"Set vc volume {player} to {num}");
				}
				else
				{
					con.LogToConsole("Player does not exist: " + text);
				}
			}
			catch (Exception ex)
			{
				log.error(ex.ToString());
			}
		}, (Dictionary<int, List<string>>)null, new(string, string)[2]
		{
			("player", ""),
			("float", "volume level 0-1")
		})));
		RegisterCommand_LocalOnly(Unsafe.As<Command, Command>(ref new Command("win32setwindowtitle", "MP dev", (Action)delegate(string[] splited)
		{
			string text = string.Join(" ", splited.Skip(1));
			win32windowtitlechanger.SetTitle(text);
			con.LogToConsole("win32windowtitlechanger.SetTitle(\"" + text + "\")");
		}, (Dictionary<int, List<string>>)null, new(string, string)[1] { ("string", "message") })));
		object obj22 = _003C_003Ec._003C_003E9__44_33;
		if (obj22 == null)
		{
			Action val14 = delegate
			{
				ConFailIfNetworkIsRunningAndIsClient();
				MP3PlayerServerAudioStreamer.Server_ForceLoadMusic();
			};
			_003C_003Ec._003C_003E9__44_33 = val14;
			obj22 = (object)val14;
		}
		RegisterCommand(Unsafe.As<Command, Command>(ref new Command("loadcustommusic", "MP-SERVER- force reload all custom music right now", (Action)obj22, (Dictionary<int, List<string>>)null, Array.Empty<(string, string)>())));
		RegisterCommand_BannedAdmin(Unsafe.As<Command, Command>(ref new Command("adminpriv-password", "MP-SERVER- set admin password (ANYONE CAN USE THIS PASSWORD)", (Action)delegate(string[] splited)
		{
			con.CheckArgumentCount(splited, 1);
			server_admin_password = splited[1];
			log.l("Set server admin password.");
		}, (Dictionary<int, List<string>>)null, new(string, string)[1] { ("password", "choose strong password") })));
		RegisterCommand_BannedAdmin(Unsafe.As<Command, Command>(ref new Command("adminpriv", "MP-SERVER- give or remove admin privileges", (Action)delegate(string[] splited)
		{
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			ConFailIfNetworkIsRunningAndIsClient();
			ConFailIfNetworkNotRunning();
			con.CheckArgumentCount(splited, 2);
			string text = splited[1].ToLower();
			string text2 = splited[2];
			if (ServerMain.TryGetPlayerFromPartialName(text2, out var player))
			{
				if (player.is_local)
				{
					log.l("Local player already has this privilege.");
				}
				else if (text == "add")
				{
					player.server_plrstate.admin_privilege = true;
					removed_server_admins.Remove(player.GetPersistentId());
					log.l($"Gave admin privilege to: {player}");
				}
				else if (text == "remove")
				{
					player.server_plrstate.admin_privilege = false;
					server_admins.Remove(player);
					removed_server_admins.Add(player.GetPersistentId());
					log.l($"Removed admin privilege from: {player}");
					NetDataWriter writer = Net.CreateWriter(10045);
					writer.Put(false);
					Net.Server_SendToClients((DeliveryMethod)2, in writer, player.clientId);
				}
				else
				{
					con.LogToConsole(text + " -> it should be \"add\" or  \"remove\"");
				}
			}
			else
			{
				con.LogToConsole(text2 + " -> player does not exist");
			}
		}, new Dictionary<int, List<string>> { 
		{
			0,
			new List<string> { "add", "remove" }
		} }, new(string, string)[2]
		{
			("action", ""),
			("player", "")
		})));
		object obj23 = _003C_003Ec._003C_003E9__44_36;
		if (obj23 == null)
		{
			Action val15 = delegate(string[] splited)
			{
				//IL_0072: Unknown result type (might be due to invalid IL or missing references)
				ConFailIfNetworkNotRunning();
				ConFailIfNetworkIsRunningAndIsServer();
				string text = "";
				if (splited.Length > 1)
				{
					text = splited[1];
				}
				client_adminmode = !client_adminmode;
				client_isadmin = client_isadmin || client_adminmode;
				log.l($"Requesting admin console: {client_adminmode}");
				NetDataWriter writer = Net.CreateWriter(10046);
				writer.Put(client_adminmode);
				writer.Put(text);
				Net.Client_Send((DeliveryMethod)2, in writer);
			};
			_003C_003Ec._003C_003E9__44_36 = val15;
			obj23 = (object)val15;
		}
		RegisterCommand_LocalOnly(Unsafe.As<Command, Command>(ref new Command("admin", "MP-CLIENT- toggle admin mode, run commands on the server and see its log", (Action)obj23, (Dictionary<int, List<string>>)null, new(string, string)[1] { ("password", "optional (u dont need password if ur whitelisted)") })));
		RegisterCommand_LocalOnly(Unsafe.As<Command, Command>(ref new Command("printposition", "MP", (Action)delegate(string[] splited)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			con.CheckArgumentCount(splited, 1);
			Vector2 val24 = con.ParsePosition(splited[1]);
			log.l($"pos {splited[1]} ->\t {val24}");
		}, (Dictionary<int, List<string>>)null, new(string, string)[1] { ("position", "") })));
		RegisterCommand(Unsafe.As<Command, Command>(ref new Command("cleanworld", "MP- delete blast marks, block damages, casings, blood", (Action)delegate
		{
			ConFailIfCheatsDisabled();
			con.CheckForWorld();
			CleanWorld();
		}, (Dictionary<int, List<string>>)null, Array.Empty<(string, string)>())));
		RegisterCommand(Unsafe.As<Command, Command>(ref new Command("setplrname", "MP-SERVER- force rename a player", (Action)delegate(string[] splited)
		{
			ConFailIfNetworkIsRunningAndIsClient();
			ConFailIfNetworkNotRunning();
			con.CheckArgumentCount(splited, 2);
			string plrname = splited[1];
			string newname = splited[2];
			bool succ = false;
			Action<NetPlayer> func = delegate(NetPlayer plr)
			{
				succ = true;
				con.LogToConsole($"Renamed {plr} to {newname}");
				plr.nameIsCustom = true;
				plr.ApplyNameAndColor(newname, plr.plrcolor);
			};
			try
			{
				Tuple<bool, string> tuple = ServerMain._PerformActionOnPlayersByName(plrname, func);
				if (!tuple.Item1)
				{
					con.LogToConsole("Rename error: " + tuple.Item2);
				}
			}
			catch (Exception ex)
			{
				log.error(ex.ToString());
			}
		}, (Dictionary<int, List<string>>)null, new(string, string)[2]
		{
			("player", ""),
			("string", "new name")
		})));
		RegisterCommand(Unsafe.As<Command, Command>(ref new Command("setplrcolor", "MP-SERVER- force recolor a player", (Action)delegate(string[] splited)
		{
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			ConFailIfNetworkIsRunningAndIsClient();
			con.CheckArgumentCount(splited, 2);
			string plrname = splited[1];
			Color newcolor;
			if (ServerMain.TryGetPlayerFromPartialName(splited[2], out var player))
			{
				newcolor = player.plrcolor;
			}
			else
			{
				newcolor = con.ParseColor(splited[2]);
			}
			if (KrokoshaScavMultiplayer.network_system_is_running)
			{
				bool succ = false;
				Action<NetPlayer> func = delegate(NetPlayer plr)
				{
					//IL_0019: Unknown result type (might be due to invalid IL or missing references)
					//IL_0035: Unknown result type (might be due to invalid IL or missing references)
					succ = true;
					con.LogToConsole($"Recolored {plr} to {newcolor}");
					plr.ApplyNameAndColor(plr.playername, newcolor);
				};
				try
				{
					Tuple<bool, string> tuple = ServerMain._PerformActionOnPlayersByName(plrname, func);
					if (!tuple.Item1)
					{
						con.LogToConsole("Recolor error: " + tuple.Item2);
					}
					return;
				}
				catch (Exception ex)
				{
					log.error(ex.ToString());
					return;
				}
			}
			UIMainMenu.SetInputColor((Color24)newcolor);
			con.LogToConsole($"Set local color to {UIMainMenu.LAST_VALID_INPUT_COLOR}");
		}, (Dictionary<int, List<string>>)null, new(string, string)[2]
		{
			("player", ""),
			("color", "new color")
		})));
		object obj24 = _003C_003Ec._003C_003E9__44_41;
		if (obj24 == null)
		{
			Action val16 = delegate(string[] splited)
			{
				//IL_0037: Unknown result type (might be due to invalid IL or missing references)
				//IL_003e: Expected O, but got Unknown
				//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ce: Expected O, but got Unknown
				_ = ConsoleScript.instance;
				StringBuilder stringBuilder = new StringBuilder("// KROKOSHA CO-OP MOD LOCALE DUMP:\n");
				if (splited.Length > 1)
				{
					string text = splited[1];
					stringBuilder.AppendLine("\n// " + UnityObjectUtility.ToSafeString((object)text));
					Language val24 = new Language();
					foreach (KeyValuePair<string, string> item5 in Lang.dict[text])
					{
						val24.other["krokosha_coop_" + item5.Key] = item5.Value;
					}
					stringBuilder.Append(JsonConvert.SerializeObject((object)val24, (Formatting)1));
				}
				else
				{
					foreach (KeyValuePair<string, Dictionary<string, string>> item6 in Lang.dict)
					{
						Language val25 = new Language();
						foreach (KeyValuePair<string, string> item7 in item6.Value)
						{
							val25.other["krokosha_coop_" + item7.Key] = item7.Value;
						}
						stringBuilder.AppendLine("\n// " + item6.Key);
						stringBuilder.Append(JsonConvert.SerializeObject((object)val25, (Formatting)1));
					}
				}
				stringBuilder.AppendLine("\n// END\n// COPY IT FROM LOG FILE INSTEAD OF CONSOLE BECAUSE IT CLEARS FORMATTING HERE");
				log.l(stringBuilder.ToString());
			};
			_003C_003Ec._003C_003E9__44_41 = val16;
			obj24 = (object)val16;
		}
		RegisterCommand_LocalOnly(Unsafe.As<Command, Command>(ref new Command("mpdumplocale", "MP - dump locale of multiplayer mod", (Action)obj24, (Dictionary<int, List<string>>)null, Array.Empty<(string, string)>())));
		object obj25 = _003C_003Ec._003C_003E9__44_42;
		if (obj25 == null)
		{
			Action val17 = delegate(string[] splited)
			{
				bool flag = !KrokoshaScavMultiplayer.verbose;
				if (splited.Length > 1)
				{
					flag = ParseBool01(splited[1]);
				}
				DebugMenuSettings.is_verbose = flag;
				DebugMenuSettings._DEV_VISUALISE_NET_EVENTS = flag;
				DebugMenuSettings._DEV_ENABLE_STEAM_LOG = flag;
				log.l($"DEV : verbose = {KrokoshaScavMultiplayer.verbose} ");
			};
			_003C_003Ec._003C_003E9__44_42 = val17;
			obj25 = (object)val17;
		}
		RegisterCommand_LocalOnly(Unsafe.As<Command, Command>(ref new Command("mpverbose", "MP dev - Enable verbose logging and debug screens", (Action)obj25, (Dictionary<int, List<string>>)null, Array.Empty<(string, string)>())));
		object obj26 = _003C_003Ec._003C_003E9__44_43;
		if (obj26 == null)
		{
			Action val18 = delegate(string[] splited)
			{
				bool dEV_ENABLE_SYNCINFO_SNITCHING = !NetObjectRegistry._DEV_ENABLE_SYNCINFO_SNITCHING;
				if (splited.Length > 1)
				{
					dEV_ENABLE_SYNCINFO_SNITCHING = ParseBool01(splited[1]);
				}
				NetObjectRegistry._DEV_ENABLE_SYNCINFO_SNITCHING = dEV_ENABLE_SYNCINFO_SNITCHING;
				log.l($"NetObjectRegistry._DEV_ENABLE_SYNCINFO_SNITCHING = {NetObjectRegistry._DEV_ENABLE_SYNCINFO_SNITCHING} ");
			};
			_003C_003Ec._003C_003E9__44_43 = val18;
			obj26 = (object)val18;
		}
		RegisterCommand_LocalOnly(Unsafe.As<Command, Command>(ref new Command("syncinfo", "MP dev - Show debug sync info on objects. (DEPRECATED)", (Action)obj26, (Dictionary<int, List<string>>)null, Array.Empty<(string, string)>())));
		RegisterCommand(Unsafe.As<Command, Command>(ref new Command("servermute", "MP-SERVER- mute player chat or voice", (Action)delegate(string[] splited)
		{
			ConFailIfNetworkNotRunning();
			ConFailIfNetworkIsRunningAndIsClient();
			con.CheckArgumentCount(splited, 2);
			bool istc = false;
			bool isvc = false;
			string text = splited[1].ToLower();
			string plrname = splited[2];
			bool has_target = false;
			bool target = true;
			if (splited.Length > 3)
			{
				has_target = true;
				target = con.ParseBool(splited[3]);
			}
			switch (text)
			{
			case "tc":
				istc = true;
				break;
			case "vc":
				isvc = true;
				break;
			case "all":
				isvc = true;
				istc = true;
				has_target = true;
				break;
			default:
				throw new Exception("Unknown chat type: " + text);
			}
			Tuple<bool, string> tuple = ServerMain._PerformActionOnPlayersByName(plrname, delegate(NetPlayer plr)
			{
				if (isvc)
				{
					if (has_target)
					{
						plr.server_mute_vc = target;
					}
					else
					{
						plr.server_mute_vc = !plr.server_mute_vc;
					}
					log.l($"{plr}.mute_vc = {plr.server_mute_vc}");
				}
				if (istc)
				{
					if (has_target)
					{
						plr.server_mute_tc = target;
					}
					else
					{
						plr.server_mute_tc = !plr.server_mute_tc;
					}
					log.l($"{plr}.mute_tc = {plr.server_mute_tc}");
				}
			});
			if (!tuple.Item1)
			{
				con.LogToConsole("servermute fail: " + tuple.Item2);
			}
		}, (Dictionary<int, List<string>>)null, new(string, string)[3]
		{
			("type", "tc/vc/all"),
			("player", ""),
			("bool", "optional")
		})));
		object obj27 = _003C_003Ec._003C_003E9__44_45;
		if (obj27 == null)
		{
			Action val19 = delegate(string[] splited)
			{
				ConFailIfNetworkNotRunning();
				ushort num = ushort.Parse(splited[1]);
				NewCoolerObjectPacketWriteReadSystem.inst.Shared_ForceSync(num);
				log.l($"NetObjectRegistry.Shared_ForceSync(id: {num});");
			};
			_003C_003Ec._003C_003E9__44_45 = val19;
			obj27 = (object)val19;
		}
		RegisterCommand(Unsafe.As<Command, Command>(ref new Command("syncid", "MP dev - Queue force-sync for a object netId", (Action)obj27, (Dictionary<int, List<string>>)null, new(string, string)[1] { ("int", "netId") })));
		RegisterCommand(Unsafe.As<Command, Command>(ref new Command("syncreset", "MP dev - Unregister all objects. (IT CAN CAUSE MORE DESYNC)", (Action)delegate(string[] splited)
		{
			ConFailIfNetworkNotRunning();
			if (Net.TRANSPORT != null && Net.TRANSPORT is TransportLiteNetLib transportLiteNetLib)
			{
				transportLiteNetLib.netmgr.Statistics.Reset();
			}
			bool flag = false;
			if (splited.Length > 1)
			{
				flag = con.ParseBool(splited[1]);
			}
			if (flag)
			{
				NewCoolerObjectPacketWriteReadSystem.inst.UnregisterEVERYTHING();
				log.l("NetObjectRegistry.UnregisterEVERYTHING()");
			}
			else
			{
				NewCoolerObjectPacketWriteReadSystem.inst.UnregisterEVERYTHING_LOUDLY();
				log.l("NetObjectRegistry.UnregisterEVERYTHING_LOUDLY()");
			}
		}, (Dictionary<int, List<string>>)null, new(string, string)[1] { ("bool", "1 for unsafe, 0 for simple") })));
		object obj28 = _003C_003Ec._003C_003E9__44_47;
		if (obj28 == null)
		{
			Action val20 = delegate
			{
				ConFailIfNetworkNotRunning();
				foreach (GameObject item8 in (from x in Object.FindObjectsOfType<Item>()
					select ((Component)x).gameObject).Union(from x in Object.FindObjectsOfType<BuildingEntity>()
					select ((Component)x).gameObject))
				{
					if (!NetObjectRegistry.ObjectCanBeIgnoredForNetwork(item8))
					{
						NetObjectRegistry.NewGO(item8);
					}
				}
				log.l("called NetObjectRegistry.NewGO on everything");
			};
			_003C_003Ec._003C_003E9__44_47 = val20;
			obj28 = (object)val20;
		}
		RegisterCommand(Unsafe.As<Command, Command>(ref new Command("syncregisterall", "MP dev - Register all objects in the world. (IT LAGS !!!!!)", (Action)obj28, (Dictionary<int, List<string>>)null, Array.Empty<(string, string)>())));
		RegisterCommand_LocalOnly(Unsafe.As<Command, Command>(ref new Command("maxplayers", "MP - SERVER - Set player limit", (Action)delegate(string[] splited)
		{
			ConFailIfNetworkIsRunningAndIsClient();
			con.CheckArgumentCount(splited, 1);
			KrokoshaScavMultiplayer.PLAYER_COUNT_LIMIT = (byte)Mathf.Clamp(con.ParseInt(splited[1]), 0, 251);
			con.LogToConsole($"Set player count limit to: {KrokoshaScavMultiplayer.PLAYER_COUNT_LIMIT}");
		}, (Dictionary<int, List<string>>)null, new(string, string)[1] { ("int", "") })));
		RegisterCommand_LocalOnly(Unsafe.As<Command, Command>(ref new Command("chatboxpos", "MP - set position and size of chat box (everything is a scale in float (0-1)) (OBSOLETE)", (Action)delegate(string[] splited)
		{
			if (splited.Count() == 1)
			{
				Chat.setting_posX = 1f;
				Chat.setting_posY = 1f;
				Chat.setting_sizeX = 1f;
				Chat.setting_sizeY = 1f;
				Chat.setting_scale = 1f;
				con.LogToConsole("Chatbox: Reset chatbox location.");
			}
			else
			{
				con.CheckArgumentCount(splited, 5);
				Chat.setting_posX = Mathf.Clamp01(con.ParseFloat(splited[1]));
				Chat.setting_posY = Mathf.Clamp01(con.ParseFloat(splited[2]));
				Chat.setting_sizeX = con.ParseFloat(splited[3]);
				Chat.setting_sizeY = con.ParseFloat(splited[4]);
				Chat.setting_scale = con.ParseFloat(splited[5]);
				con.LogToConsole("Chatbox: SETTING NEW PARAMETERS:");
				con.LogToConsole($"Chatbox: setting_posX  = {Chat.setting_posX * 100f}%");
				con.LogToConsole($"Chatbox: setting_posY  = {Chat.setting_posY * 100f}%");
				con.LogToConsole($"Chatbox: setting_sizeX = {Chat.setting_sizeX * 100f}%");
				con.LogToConsole($"Chatbox: setting_sizeY = {Chat.setting_sizeY * 100f}%");
				con.LogToConsole($"Chatbox: setting_scale = {Chat.setting_scale * 100f}%");
			}
		}, (Dictionary<int, List<string>>)null, new(string, string)[5]
		{
			("float", "X pos"),
			("float", "Y pos"),
			("float", "X size"),
			("float", "Y size"),
			("float", "scale")
		})));
		RegisterCommand_LocalOnly(Unsafe.As<Command, Command>(ref new Command("chatpreview", "MP dev - toggle chat text preview (OBSOLETE)", (Action)delegate(string[] splited)
		{
			bool dEV_ENABLE_PREVIEW = !Chat._DEV_ENABLE_PREVIEW;
			if (splited.Length > 1)
			{
				dEV_ENABLE_PREVIEW = ParseBool01(splited[1]);
			}
			Chat._DEV_ENABLE_PREVIEW = dEV_ENABLE_PREVIEW;
			con.LogToConsole($"Chat._DEV_ENABLE_PREVIEW = {Chat._DEV_ENABLE_PREVIEW}");
		}, (Dictionary<int, List<string>>)null, new(string, string)[1] { ("bool", "") })));
		RegisterCommand(Unsafe.As<Command, Command>(ref new Command("tomainmenu", "MP - go to main menu", (Action)delegate
		{
			con.CheckForWorld();
			PlayerCamera.main.ToMainMenu();
		}, (Dictionary<int, List<string>>)null, Array.Empty<(string, string)>())));
		RegisterCommand_LocalOnly(Unsafe.As<Command, Command>(ref new Command("currentrules", "MP - show all multiplayer game rules", (Action)delegate
		{
			string text = "Krokosha Co-op mod game rules:\n";
			FieldInfo[] fields = typeof(KrokoshaMultiplayerGameRules).GetFields(BindingFlags.Instance | BindingFlags.Public);
			foreach (FieldInfo fieldInfo in fields)
			{
				text += $"rule {fieldInfo.Name} [{fieldInfo.FieldType.Name}] // current: {fieldInfo.GetValue(KrokoshaScavMultiplayer.rules)}\n";
			}
			con.LogToConsole(text);
		}, (Dictionary<int, List<string>>)null, Array.Empty<(string, string)>())));
		RegisterCommand(Unsafe.As<Command, Command>(ref new Command("resetrules", "MP - reset all multiplayer game rules to default values", (Action)delegate
		{
			KrokoshaScavMultiplayer.rules = new KrokoshaMultiplayerGameRules();
			con.LogToConsole("Reset all rules to default.");
		}, (Dictionary<int, List<string>>)null, Array.Empty<(string, string)>())));
		RegisterCommand(Unsafe.As<Command, Command>(ref new Command("rule", "MP - set multiplayer game rule", (Action)delegate(string[] splited)
		{
			ConFailIfNetworkIsRunningAndIsClient();
			con.CheckArgumentCount(splited, 2);
			string text = splited[1];
			string text2 = splited[2];
			FieldInfo[] fields = typeof(KrokoshaMultiplayerGameRules).GetFields(BindingFlags.Instance | BindingFlags.Public);
			foreach (FieldInfo fieldInfo in fields)
			{
				if (fieldInfo.Name == text)
				{
					try
					{
						object value = ((!(fieldInfo.FieldType == typeof(bool))) ? Convert.ChangeType(text2, fieldInfo.FieldType, CultureInfo.InvariantCulture) : ((object)ParseBool01(text2)));
						TypedReference obj32 = __makeref(KrokoshaScavMultiplayer.rules);
						fieldInfo.SetValueDirect(obj32, value);
						KrokoshaScavMultiplayer.ApplyGameRules();
						con.LogToConsole($"Succesfully set rule '{fieldInfo.Name}' to '{fieldInfo.GetValue(KrokoshaScavMultiplayer.rules)}'.");
						return;
					}
					catch (Exception ex)
					{
						con.LogToConsole("Failed to set rule '" + fieldInfo.Name + "': " + ex.Message);
						return;
					}
				}
			}
			con.LogToConsole("Rule '" + text + "' does not exist!");
		}, new Dictionary<int, List<string>> { 
		{
			0,
			(from x in typeof(KrokoshaMultiplayerGameRules).GetFields(BindingFlags.Instance | BindingFlags.Public)
				select x.Name).ToList()
		} }, new(string, string)[2]
		{
			("string rule", "Rule Name"),
			("value", "New value")
		})));
		RegisterCommand(Unsafe.As<Command, Command>(ref new Command("setlocalname", "MP - Set your multiplayer username.", (Action)delegate(string[] args)
		{
			ConsoleScript.instance.CheckArgumentCount(args, 1);
			if (Object.op_Implicit((Object)(object)PlayerCamera.main))
			{
				throw new Exception("Name can only be set in main menu.");
			}
			ConFailIfNetworkAlreadyRunning();
			string text = KrokoshaScavMultiplayer.SanitizeTextInput(args[1]);
			con.LogToConsole("Set your username to: " + text);
			KrokoshaScavMultiplayer.INPUT_USERNAME = text;
		}, (Dictionary<int, List<string>>)null, new(string, string)[1] { ("string name", "Your new name") })));
		RegisterCommand(Unsafe.As<Command, Command>(ref new Command("setlocalpassword", "MP - Set your multiplayer password.", (Action)delegate(string[] args)
		{
			ConsoleScript.instance.CheckArgumentCount(args, 1);
			if (Object.op_Implicit((Object)(object)PlayerCamera.main))
			{
				throw new Exception("Password can only be set in main menu.");
			}
			ConFailIfNetworkAlreadyRunning();
			string text = args[1];
			con.LogToConsole("Set your password to: " + text);
			KrokoshaScavMultiplayer.INPUT_PASSWORD = text;
		}, (Dictionary<int, List<string>>)null, new(string, string)[1] { ("string password", "") })));
		RegisterCommand_LocalOnly(Unsafe.As<Command, Command>(ref new Command("joinlobby", "MP - Client Connect to an active steam lobby.", (Action)delegate(string[] args)
		{
			ConFailIfNetworkAlreadyRunning();
			ConFailIfNotInMainMenu();
			ulong lobby_steamID = ulong.Parse(args[1]);
			TransportSteamworks.OnWantToJoinLobby(lobby_steamID);
			con.LogToConsole("Starting client and attempting join: " + lobby_steamID);
		}, (Dictionary<int, List<string>>)null, new(string, string)[1] { ("SteamId", "") })));
		RegisterCommand_LocalOnly(Unsafe.As<Command, Command>(ref new Command("hostlobby", "MP - Host steam lobby.", (Action)delegate(string[] args)
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			ConFailIfNetworkAlreadyRunning();
			ConFailIfNotInMainMenu();
			con.CheckArgumentCount(args, 2);
			int num = con.ParseInt(args[1]);
			KrokoshaScavMultiplayer.SERVER_TOGGLE_SHOULD_HOST_DEDICATED = con.ParseBool(args[2]);
			ELobbyType val24 = KSteam.NumToLobbyType(num);
			Net.NetType netType = ((!KrokoshaScavMultiplayer.SERVER_TOGGLE_SHOULD_HOST_DEDICATED) ? Net.NetType.Host : Net.NetType.DedicatedServer);
			KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog($"Hosting Steam Lobby: {netType}, {val24}");
			TransportSteamworks.OnWantToHostLobby(netType, val24);
		}, (Dictionary<int, List<string>>)null, new(string, string)[2]
		{
			("int", "Lobby type (2 = public, 1 = friends only, 0 = private)"),
			("bool", "dedicated")
		})));
		object obj29 = _003C_003Ec._003C_003E9__44_59;
		if (obj29 == null)
		{
			Action val21 = delegate
			{
				ConFailIfNotInSteamLobby();
				log.l("LOBBY INFO:\n" + KSteam.CURRENT_LOBBY.DebugReadableDump());
			};
			_003C_003Ec._003C_003E9__44_59 = val21;
			obj29 = (object)val21;
		}
		RegisterCommand_LocalOnly(Unsafe.As<Command, Command>(ref new Command("getlobbyinfo", "MP - Log info about your steam lobby.", (Action)obj29, (Dictionary<int, List<string>>)null, Array.Empty<(string, string)>())));
		RegisterCommand_LocalOnly(Unsafe.As<Command, Command>(ref new Command("dumplobbysearch", "MP - Start a dedicated server. (you'll have no player character)", (Action)delegate(string[] args)
		{
			bool num = con.ParseBool(args[1]);
			StringBuilder stringBuilder = new StringBuilder("// LOBBY SEARCH DUMP:\n");
			foreach (NetPublicServerInfo item9 in num ? UIServerBrowser.CurFilteredServerList : UIServerBrowser.CurServerList)
			{
				if (item9.steam_lobby_info != null)
				{
					stringBuilder.AppendLine(item9.steam_lobby_info.DebugReadableDump());
				}
			}
			stringBuilder.AppendLine("===============");
			log.l(stringBuilder.ToString());
		}, (Dictionary<int, List<string>>)null, new(string, string)[1] { ("bool", "filtered") })));
		RegisterCommand_LocalOnly(Unsafe.As<Command, Command>(ref new Command("connect", "MP - Client Connect to an active server.", (Action)delegate(string[] args)
		{
			ConFailIfNetworkAlreadyRunning();
			ConFailIfNotInMainMenu();
			if (args.Length > 1)
			{
				KrokoshaScavMultiplayer.INPUT_IPPORT = args[1];
			}
			con.LogToConsole("Starting client and attempting connection: " + KrokoshaScavMultiplayer.INPUT_IPPORT);
			TransportLiteNetLib.OnWantToConnect(KrokoshaScavMultiplayer.INPUT_IPPORT, Net.NetType.Client);
		}, (Dictionary<int, List<string>>)null, new(string, string)[1] { ("string ip:port", "optional, IP and PORT of target server") })));
		RegisterCommand_LocalOnly(Unsafe.As<Command, Command>(ref new Command("starthost", "MP - Host the game.", (Action)delegate(string[] args)
		{
			ConFailIfNetworkAlreadyRunning();
			ConFailIfNotInMainMenu();
			if (args.Length > 1)
			{
				if (ushort.TryParse(args[1], out var _))
				{
					KrokoshaScavMultiplayer.INPUT_IPPORT = "0.0.0.0:" + args[1];
				}
				else
				{
					KrokoshaScavMultiplayer.INPUT_IPPORT = args[1];
				}
			}
			con.LogToConsole("Starting Host: " + KrokoshaScavMultiplayer.INPUT_IPPORT);
			TransportLiteNetLib.OnWantToConnect(KrokoshaScavMultiplayer.INPUT_IPPORT, Net.NetType.Host);
		}, (Dictionary<int, List<string>>)null, new(string, string)[1] { ("int port", "") })));
		RegisterCommand_LocalOnly(Unsafe.As<Command, Command>(ref new Command("startserver", "MP - Start a dedicated server. (you'll have no player character)", (Action)delegate(string[] args)
		{
			ConFailIfNetworkAlreadyRunning();
			ConFailIfNotInMainMenu();
			if (args.Length > 1)
			{
				if (ushort.TryParse(args[1], out var _))
				{
					KrokoshaScavMultiplayer.INPUT_IPPORT = "0.0.0.0:" + args[1];
				}
				else
				{
					KrokoshaScavMultiplayer.INPUT_IPPORT = args[1];
				}
			}
			con.LogToConsole("Starting Server: " + KrokoshaScavMultiplayer.INPUT_IPPORT);
			TransportLiteNetLib.OnWantToConnect(KrokoshaScavMultiplayer.INPUT_IPPORT, Net.NetType.DedicatedServer);
		}, (Dictionary<int, List<string>>)null, new(string, string)[1] { ("int port", "") })));
		object obj30 = _003C_003Ec._003C_003E9__44_64;
		if (obj30 == null)
		{
			Action val22 = delegate(string[] args)
			{
				if (args.Count() > 1)
				{
					BugReporter.user_bugreport_message = string.Join(" ", args.Skip(1));
				}
				Util.StartCoroutine(BugReporter.BugreportUploadSequence());
			};
			_003C_003Ec._003C_003E9__44_64 = val22;
			obj30 = (object)val22;
		}
		RegisterCommand(Unsafe.As<Command, Command>(ref new Command("bugreport", "MP - Send a bug report.", (Action)obj30, (Dictionary<int, List<string>>)null, new(string, string)[1] { ("string text", "") })));
		object obj31 = _003C_003Ec._003C_003E9__44_65;
		if (obj31 == null)
		{
			Action val23 = delegate
			{
				ConFailIfNetworkNotRunning();
				log.l("Disconnect.");
				KrokoshaScavMultiplayer.ShutdownNetwork();
			};
			_003C_003Ec._003C_003E9__44_65 = val23;
			obj31 = (object)val23;
		}
		RegisterCommand_LocalOnly(Unsafe.As<Command, Command>(ref new Command("disconnect", "MP - Disconnect from server or shutdown server.", (Action)obj31, (Dictionary<int, List<string>>)null, Array.Empty<(string, string)>())));
		RegisterCommand_LocalOnly(Unsafe.As<Command, Command>(ref new Command("screensize", "MP dev - set game window resolution", (Action)delegate(string[] splited)
		{
			Screen.SetResolution(con.ParseInt(splited[1]), con.ParseInt(splited[2]), (FullScreenMode)3);
		}, (Dictionary<int, List<string>>)null, Array.Empty<(string, string)>())));
		log.l("Patched the console!");
		GameObject s = Plugin.s;
		Plugin.LoadErrorNotifier loadErrorNotifier = default(Plugin.LoadErrorNotifier);
		if (s != null && s.TryGetComponent<Plugin.LoadErrorNotifier>(ref loadErrorNotifier))
		{
			log.error("WrongVersionNotifier:\n" + Plugin.LoadErrorNotifier.errorstr);
		}
		UpdatePlayerListAutocomplete();
	}

	public static bool Server_CheckBannedCommand(in NetPlayer plr, in string first_arg)
	{
		string text = first_arg;
		if (localonly_commands.Contains(text))
		{
			log.sus($"{plr} tried to execute a local-only command: {text}");
			return true;
		}
		if (banned_admin_commands.Contains(text))
		{
			log.sus($"{plr} tried to execute a banned admin command: {text}");
			return true;
		}
		return false;
	}

	[ServerReceiver(10048)]
	private static void ServerReceiver_Console_TryExecuteCommand(knetid clientId, ref NetDataReader reader)
	{
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		string text = default(string);
		reader.Get(ref text);
		reader.Get(out Vector2 result);
		if (!NetPlayer.TryGetPlayerFromClientId(clientId, out var plr) || (!server_admins.Contains(plr) && !plr.server_plrstate.admin_privilege))
		{
			return;
		}
		string[] array = text.Trim().Split(new char[1] { ' ' });
		if (Server_CheckBannedCommand(in plr, in array[0]))
		{
			return;
		}
		Command val = ConsoleScript.SearchExact(array[0]);
		if (val != null)
		{
			if (val.name == "repeat")
			{
				string[] array2 = array[3].Split(new char[1] { ';' });
				for (int i = 0; i < array2.Length; i++)
				{
					string[] array3 = array2[i].Replace('_', ' ').Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
					if (array3.Length != 0 && Server_CheckBannedCommand(in plr, in array3[0]))
					{
						return;
					}
				}
			}
			try
			{
				for (int j = 0; j < Math.Min(array.Length - 1, val.argDescription.Length); j++)
				{
					ref string reference = ref array[j + 1];
					if (val.argDescription[j].Item1.StartsWith("position"))
					{
						if (reference == "cursor")
						{
							reference = result.x.ToString(CultureInfo.InvariantCulture) + "," + result.y.ToString(CultureInfo.InvariantCulture);
						}
						else if (reference == "player")
						{
							reference = $"plr:id:{plr.clientId}";
						}
						else
						{
							if (!TryParsePlayerPositionGetName(reference, out var name))
							{
								continue;
							}
							switch (name)
							{
							case "@m":
								reference = $"plr:id:{plr.clientId}";
								break;
							case "@c":
							{
								NetBody bodyForCommandOnCursor = ServerMain.GetBodyForCommandOnCursor(result, only_players: true);
								if ((Object)(object)bodyForCommandOnCursor != (Object)null && bodyForCommandOnCursor.is_player)
								{
									reference = $"plr:id:{bodyForCommandOnCursor.player.clientId}";
								}
								break;
							}
							case "@r":
							{
								plr.TryGetNetBody(out var pb);
								NetBody netBody = ServerMain.RelaxedGetBodyForCommand(name, allow_macros: true, only_players: true, pb);
								if ((Object)(object)netBody != (Object)null && netBody.is_player)
								{
									reference = $"plr:id:{netBody.player.clientId}";
								}
								break;
							}
							}
						}
					}
					else
					{
						if (!val.argDescription[j].Item1.StartsWith("player"))
						{
							continue;
						}
						if (reference == "@m")
						{
							reference = $"id:{plr.clientId}";
						}
						else if (reference == "@c")
						{
							NetBody bodyForCommandOnCursor2 = ServerMain.GetBodyForCommandOnCursor(result, only_players: true);
							if ((Object)(object)bodyForCommandOnCursor2 != (Object)null && bodyForCommandOnCursor2.is_player)
							{
								reference = $"id:{bodyForCommandOnCursor2.player.clientId}";
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				log.error($"SERVER: REQUEST FROM {plr} TO TryExecuteCommand \n" + ex.ToString());
			}
		}
		log.l(string.Format("[<color=red>ADMIN</color>] {0} executes \"{1}\"", plr, string.Join(" ", array)));
		ConsoleScript.instance.TryExecuteCommand(array, false);
	}

	[ServerReceiver(10046)]
	private static void ServerReceiver_Console_ActivateAdminMode(knetid clientId, ref NetDataReader reader)
	{
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		bool flag = default(bool);
		reader.Get(ref flag);
		string text = default(string);
		reader.Get(ref text);
		if (!NetPlayer.TryGetPlayerFromClientId(clientId, out var plr))
		{
			return;
		}
		if (flag && !plr.is_local)
		{
			bool flag2 = !string.IsNullOrEmpty(server_admin_password) && text == server_admin_password;
			if (!removed_server_admins.Contains(plr.GetPersistentId()) && (plr.server_plrstate.admin_privilege || flag2 || KnownPersons.PRIVILEGED_STEAM_USERS.Contains(plr.steam_id)))
			{
				server_admins.Add(plr);
				string text2 = $"[<color=red>ADMIN</color>] {plr} <color=green><b>ENTERS</b></color> server console!";
				if (flag2)
				{
					plr.server_plrstate.admin_privilege = true;
					text2 += " (Entered correct password)";
				}
				log.l(text2);
			}
			else
			{
				NetDataWriter writer = Net.CreateWriter(10045);
				writer.Put(false);
				Net.Server_SendToClients((DeliveryMethod)2, in writer, plr.clientId);
			}
		}
		else
		{
			server_admins.Remove(plr);
			log.l($"[<color=red>ADMIN</color>] {plr} <color=red><b>EXITS</b></color> server console!");
		}
	}

	[ClientReceiver(10045, true)]
	private static void ClientReceiver_Console_DenyAdminMode(knetid _, ref NetDataReader reader)
	{
		client_adminmode = false;
		client_isadmin = false;
		log.l("Server denied server console access.");
	}

	[ClientReceiver(10047, true)]
	private static void ClientReceiver_Console_Log(knetid _, ref NetDataReader reader)
	{
		reader.Get(out byte[] result);
		byte[] bytes = Util.DecompressDeflate(result);
		string text = Encoding.UTF8.GetString(bytes);
		ConsoleScript.instance.LogToConsole("[<color=red>SERVER</color>] " + text);
	}

	public static void UpdatePlayerListAutocomplete()
	{
		List<string> list = new List<string>();
		List<string> list2 = new List<string>();
		List<string> value = new List<string>();
		if (KrokoshaScavMultiplayer.network_system_is_running)
		{
			list2 = NetPlayer.ClientIdToPlayerDict.Values.Select((NetPlayer x) => x.playername).ToList();
			list = new List<string> { "@c", "@r", "@m" };
			list.AddRange(list2);
			for (int num = 0; num < list.Count; num++)
			{
				list[num] = "plr:" + list[num];
			}
			List<string> list3 = new List<string>();
			list3.Add("@c");
			list3.Add("@a");
			list3.Add("@o");
			list3.Add("@r");
			list3.Add("@m");
			list3.AddRange(list2);
			value = list3.ToList();
		}
		foreach (Command command in ConsoleScript.Commands)
		{
			for (int num2 = 0; num2 < command.argDescription.Length; num2++)
			{
				if (command.argDescription[num2].Item1.StartsWith("player"))
				{
					if (command.argAutofill == null)
					{
						command.argAutofill = new Dictionary<int, List<string>>();
					}
					command.argAutofill[num2] = value;
				}
				if (command.argDescription[num2].Item1.StartsWith("position"))
				{
					if (command.argAutofill == null)
					{
						command.argAutofill = new Dictionary<int, List<string>>();
					}
					List<string> list4 = new List<string> { "cursor", "player", "random", "#,#" };
					list4.AddRange(list);
					command.argAutofill[num2] = list4;
				}
			}
		}
	}

	public static void Server_SendConsoleLog(in string text, NetPlayer plr)
	{
		Server_SendConsoleLog(in text, new List<knetid> { plr.clientId });
	}

	public static void Server_SendConsoleLog(in string text, IReadOnlyList<knetid> targets = null)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		if (targets == null)
		{
			targets = server_admins.Select((NetPlayer x) => x.clientId).ToList();
		}
		if (targets.Count > 0)
		{
			byte[] array = new byte[0];
			try
			{
				array = Util.CompressDeflate(Encoding.UTF8.GetBytes(text));
				NetDataWriter writer = Net.CreateWriter(10047);
				writer.PutBytesWithLength(array);
				DeliveryMethod delivery = (DeliveryMethod)2;
				IEnumerable<knetid> clientIds = targets;
				Net.Server_SendToClients(in delivery, in writer, in clientIds);
			}
			catch (Exception ex)
			{
				Plugin.log.LogError((object)$"ConsoleScript_LogToConsole_MultiplayerPatch  compressed len:{array.Length}\n{ex.ToString()}");
			}
		}
	}

	public static bool CanExecuteAdminCommands()
	{
		if (!Net.running)
		{
			return true;
		}
		if (KrokoshaScavMultiplayer.is_server)
		{
			return true;
		}
		return client_isadmin;
	}

	public static void ExecuteCommandAdminNoLog(in string command)
	{
		if (!Net.running || KrokoshaScavMultiplayer.is_server)
		{
			con.TryExecuteCommand(command.Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries), false);
		}
		else
		{
			Client_ExecuteAdminCommand(in command);
		}
	}

	public static void Client_ExecuteAdminCommand(in string command)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		NetDataWriter writer = Net.CreateWriter(10048);
		writer.Put(command);
		writer.Put(Util.GetCursorWorldPos());
		Net.Client_Send((DeliveryMethod)2, in writer);
	}

	private void Start()
	{
		NetPlayer.OnPlayerLeft += delegate(NetPlayer plr)
		{
			server_admins.Remove(plr);
		};
	}

	private void Update()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (!_DEV_TELEKINESIS)
		{
			return;
		}
		Vector2 cursorWorldPos = Util.GetCursorWorldPos();
		if (Input.GetKeyDown((KeyCode)324))
		{
			_DEV_TELEKINESIS_OBJECT = null;
			Collider2D[] array = Physics2D.OverlapPointAll(cursorWorldPos);
			Limb val2 = default(Limb);
			BuildingEntity val3 = default(BuildingEntity);
			Rigidbody2D val4 = default(Rigidbody2D);
			ChunkScript val5 = default(ChunkScript);
			foreach (Collider2D val in array)
			{
				Body componentInParent = ((Component)val).GetComponentInParent<Body>();
				if ((Object)(object)componentInParent != (Object)null)
				{
					if (Util.IsBodyLocal(componentInParent))
					{
						continue;
					}
					if (((Component)componentInParent).TryGetComponent<Limb>(ref val2))
					{
						_DEV_TELEKINESIS_OBJECT = ((Component)val2).gameObject;
					}
					else
					{
						_DEV_TELEKINESIS_OBJECT = ((Component)componentInParent).gameObject;
					}
				}
				else
				{
					if (((Component)val).TryGetComponent<BuildingEntity>(ref val3))
					{
						_DEV_TELEKINESIS_OBJECT = ((Component)val).gameObject;
					}
					if (((Component)val).TryGetComponent<Rigidbody2D>(ref val4))
					{
						_DEV_TELEKINESIS_OBJECT = ((Component)val4).gameObject;
					}
				}
				if ((Object)(object)_DEV_TELEKINESIS_OBJECT != (Object)null)
				{
					if (!_DEV_TELEKINESIS_OBJECT.TryGetComponent<ChunkScript>(ref val5))
					{
						break;
					}
					_DEV_TELEKINESIS_OBJECT = null;
				}
			}
			log.l("telekinesis click: " + UnityObjectUtility.ToSafeString((Object)(object)_DEV_TELEKINESIS_OBJECT));
		}
		if (Input.GetKeyUp((KeyCode)324))
		{
			_DEV_TELEKINESIS_OBJECT = null;
		}
	}

	private void FixedUpdate()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		Vector2 to = Util.GetCursorWorldPos();
		Vector2 val = -(lastworldcursorpos - to);
		if (_DEV_TELEKINESIS)
		{
			if (!CanCheat())
			{
				_DEV_TELEKINESIS = false;
				_DEV_TELEKINESIS_OBJECT = null;
			}
			if ((Object)(object)_DEV_TELEKINESIS_OBJECT != (Object)null)
			{
				Limb limb = default(Limb);
				NetBody nb = default(NetBody);
				if (((_DEV_TELEKINESIS_OBJECT.TryGetComponent<Limb>(ref limb) && limb.TryGetNetBody(out nb)) || _DEV_TELEKINESIS_OBJECT.TryGetComponent<NetBody>(ref nb)) && !nb.is_local)
				{
					nb.body.SetVelocity(Vector2.zero);
					nb.body.rb.gravityScale = 0f;
					Transform transform = _DEV_TELEKINESIS_OBJECT.transform;
					transform.position += Vector2.op_Implicit(val);
					if (KrokoshaScavMultiplayer.IsNetworkActiveAndIsServer())
					{
						nb.Server_RemindPlayersCurrentState(keep_velocity: false, reliable: false);
					}
				}
				else
				{
					Rigidbody2D val2 = default(Rigidbody2D);
					if (_DEV_TELEKINESIS_OBJECT.TryGetComponent<Rigidbody2D>(ref val2) && (int)val2.bodyType == 0 && val2.simulated)
					{
						float magnitude;
						Vector2 val3 = KM.normal(val2.position, in to, out magnitude);
						val2.AddForce(-Physics2D.gravity * 0.6f);
						Rigidbody2D obj = val2;
						obj.velocity += val3 * 5f;
					}
					else
					{
						Transform transform2 = _DEV_TELEKINESIS_OBJECT.transform;
						transform2.position += Vector2.op_Implicit(val);
						TraderScript val4 = default(TraderScript);
						if (_DEV_TELEKINESIS_OBJECT.TryGetComponent<TraderScript>(ref val4))
						{
							Vector2 val5 = Vector2.op_Implicit(_DEV_TELEKINESIS_OBJECT.transform.position);
							val4.MoveRange = new RangeF(val5.x, val5.x);
							val4.desiredPos = val5;
						}
					}
					KrokoshaScavMultiGameObjectNetworkTracker krokoshaScavMultiGameObjectNetworkTracker = default(KrokoshaScavMultiGameObjectNetworkTracker);
					if (_DEV_TELEKINESIS_OBJECT.TryGetComponent<KrokoshaScavMultiGameObjectNetworkTracker>(ref krokoshaScavMultiGameObjectNetworkTracker) && krokoshaScavMultiGameObjectNetworkTracker.syncinfo != null)
					{
						NetObjectRegistry.Server_QueueSync(krokoshaScavMultiGameObjectNetworkTracker.syncinfo);
					}
				}
			}
		}
		lastworldcursorpos = to;
	}

	private void LateUpdate()
	{
		if (!_DEV_AUDIO_ONLY_ON_FOCUS)
		{
			return;
		}
		if (Application.isFocused)
		{
			AudioListener.volume = _DEV_LAST_VOLUME;
			return;
		}
		if (AudioListener.volume > 0.0101f)
		{
			_DEV_LAST_VOLUME = AudioListener.volume;
		}
		AudioListener.volume = 0.01f;
	}
}
