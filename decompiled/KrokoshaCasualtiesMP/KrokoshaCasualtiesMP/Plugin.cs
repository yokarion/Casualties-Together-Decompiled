using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using KrokoshaCasualtiesMP_DISABLED_STATE;
using KrokoshaCasualtiesUtils;
using Steamworks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KrokoshaCasualtiesMP;

[BepInProcess("CasualtiesUnknown.exe")]
[BepInPlugin("KrokoshaCasualtiesMP", "Krokosha_MP_CU", "4.1.2")]
public class Plugin : BaseUnityPlugin
{
	internal class LoadErrorNotifier : MonoBehaviour
	{
		public static string errorstr = "";

		private void Start()
		{
			KrokoshaCasualtiesMP_DISABLED_BUTTON_HANDLER.ON_CLICK_WILL_ENABLE_THE_MOD = false;
			ComponentHolderProtocol.GetOrAddComponent<KrokoshaCasualtiesMP_DISABLED_BUTTON_HANDLER>((Object)(object)this);
		}

		private void OnGUI()
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			if (!Object.op_Implicit((Object)(object)Object.FindObjectOfType<Body>()))
			{
				GUI.Label(new Rect(20f, 20f, (float)Screen.width, (float)Screen.height), "Krokosha666 CO-OP MOD v4.1.2 failed to load!!!!!! \n\n" + errorstr);
			}
		}
	}

	internal static ManualLogSource log;

	internal const string FORCEDISABLE_PLAYERPREF_KEY = "KrokoshaCasualtiesMP_FORCE_DISABLE_MP_MOD";

	public const bool IS_DEBUG_BUILD = false;

	public const bool IS_TESTING_BUILD = false;

	public const bool IS_RELEASE_BUILD = true;

	public static GameObject s;

	public const bool OG_RUNINGACKGROUND = false;

	public const string TARGET_GAME_VERSION = "7.0.1";

	public const string MOD_VERSION = "4.1.2";

	public static List<string> commands_to_run = new List<string>();

	public static string test_scenario = null;

	private static string current_rule_to_set;

	public static bool cmd_start__steam_lobby = false;

	public static string cmd_start__steam_lobby_type = "public";

	public static bool cmd_start__is_client = false;

	public static bool cmd_start__is_dedicated_server = false;

	internal static ManualLogSource Logger => log;

	public static bool SUCCESFULLY_INITIALIZED { get; private set; }

	public static bool FORCE_DISABLE_MP_MOD { get; private set; }

	public static bool FORCE_NO_STEAM { get; private set; }

	public static bool юзер_прошаренный { get; private set; }

	public static bool dump_game_ids { get; private set; }

	public static bool start_network { get; private set; }

	public static bool do_immidiate_start { get; private set; }

	public static int startworld { get; private set; }

	public static bool plrname_override { get; private set; }

	public static bool rules_override { get; private set; }

	public static event Action OnInitFinish;

	public static bool IsExeSameAsInSteam()
	{
		if (!KSteam.Loaded)
		{
			return false;
		}
		string appInstallDir = KSteam.GetAppInstallDir();
		if (Utility.IsNullOrWhiteSpace(appInstallDir))
		{
			return false;
		}
		return Process.GetCurrentProcess().MainModule.FileName.Contains(appInstallDir);
	}

	private static bool TryStartFromSteam()
	{
		if (!KSteam.Loaded)
		{
			return false;
		}
		if (KSteam.STEAM_APPID == 4576510 || KSteam.STEAM_APPID == 4584420)
		{
			if (!IsExeSameAsInSteam())
			{
				return false;
			}
			Application.Quit();
			Process.Start(new ProcessStartInfo
			{
				FileName = $"steam://run/{KSteam.STEAM_APPID}",
				UseShellExecute = true
			});
			return true;
		}
		return false;
	}

	public static void RestartGame()
	{
		try
		{
			try
			{
				if (TryStartFromSteam())
				{
					KrokoshaCasualtiesMP.log.l("Relaunching the game through steam!");
					return;
				}
			}
			catch (Exception ex)
			{
				KrokoshaCasualtiesMP.log.error("STEAM RESTART FAILED " + ex.ToString());
			}
		}
		catch (Exception ex2)
		{
			Debug.LogError((object)("GAME RESTART FAILED:" + ex2.ToString()));
		}
		Application.Quit();
	}

	private void Awake()
	{
		s = ((Component)this).gameObject;
		SUCCESFULLY_INITIALIZED = false;
		((Object)((Component)this).gameObject).hideFlags = (HideFlags)61;
		log = ((BaseUnityPlugin)this).Logger;
		try
		{
			Awake_Stage1();
		}
		catch (Exception ex)
		{
			log.LogError((object)("Awake_Stage1: " + ex.ToString()));
		}
		Plugin.OnInitFinish?.Invoke();
	}

	private void Awake_Stage1()
	{
		try
		{
			FORCE_DISABLE_MP_MOD = PlayerPrefsExtended.GetBool("KrokoshaCasualtiesMP_FORCE_DISABLE_MP_MOD", FORCE_DISABLE_MP_MOD);
		}
		catch (Exception ex)
		{
			log.LogError((object)("PlayerPrefs: FORCE_DISABLE_MP_MOD: LOADING: " + ex.ToString()));
		}
		CheckCommandlineArgsForThisMod();
		try
		{
			PlayerPrefsExtended.SetBool("KrokoshaCasualtiesMP_FORCE_DISABLE_MP_MOD", FORCE_DISABLE_MP_MOD);
			PlayerPrefs.Save();
		}
		catch (Exception ex2)
		{
			log.LogError((object)("PlayerPrefs: FORCE_DISABLE_MP_MOD: SAVING: " + ex2.ToString()));
		}
		if (FORCE_DISABLE_MP_MOD)
		{
			log.LogInfo((object)"Multiplayer Mod Self-Destruct activated.");
			try
			{
				ComponentHolderProtocol.AddComponent<KrokoshaCasualtiesMP_DISABLED_BUTTON_HANDLER>((Object)(object)this);
			}
			catch (Exception ex3)
			{
				log.LogError((object)("MP-SWITCH BUTTON CREATION: " + ex3.ToString()));
			}
			try
			{
				Object.Destroy((Object)(object)this);
				return;
			}
			catch (Exception ex4)
			{
				log.LogError((object)("FORCE_DISABLE_MP_MOD: !!! KILLING MYSELF FAILED !!!: " + ex4.ToString()));
				return;
			}
		}
		Awake_Stage2();
	}

	private void _PickUsername()
	{
		try
		{
			if (KSteam.Loaded)
			{
				KrokoshaScavMultiplayer.INPUT_USERNAME = KSteam.GetLocalUsername();
				return;
			}
		}
		catch (Exception ex)
		{
			log.LogError((object)("_PickName: " + ex.ToString()));
		}
		KrokoshaScavMultiplayer.INPUT_USERNAME = "Player" + Random.Range(0, 9999);
	}

	private void Awake_Stage2()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		Harmony val = new Harmony("KrokoshaCasualtiesMP");
		if (Application.version != "7.0.1")
		{
			LoadErrorNotifier.errorstr = LoadErrorNotifier.errorstr + "Mismatching version detected: TARGET: 7.0.1 - CURRENT: " + Application.version;
			KrokoshaCasualtiesMP.log.error(LoadErrorNotifier.errorstr);
			LoadErrorNotifier.errorstr += "\n";
		}
		try
		{
			val.PatchAll();
			ExtraPatches.ApplyPatches(val);
		}
		catch (Exception ex)
		{
			KrokoshaCasualtiesMP.log.error(string.Format("Plugin {0} {1} failed to load! Make sure you have correct version!\n\n{2}\n", "KrokoshaCasualtiesMP", "4.1.2", ex));
			LoadErrorNotifier.errorstr = LoadErrorNotifier.errorstr + "\n" + ex.ToString();
			if (Paths.ExecutablePath.Any((char c) => c > '\u007f'))
			{
				LoadErrorNotifier.errorstr = LoadErrorNotifier.errorstr + "\n\n" + Lang.Get("loaderror_possible_nonascii_path", false) + "\n" + Paths.ExecutablePath + "\n";
			}
			ComponentHolderProtocol.AddComponent<LoadErrorNotifier>((Object)(object)this);
			Application.runInBackground = false;
			return;
		}
		Application.runInBackground = true;
		log.LogInfo((object)"Plugin KrokoshaCasualtiesMP version 4.1.2 is loaded!");
		log.LogInfo((object)("Game version: \"" + Application.version + "\""));
		KSteam.CheckSteam();
		UIMainMenu._USE_STEAM_MENU = KSteam.Loaded;
		if (!plrname_override)
		{
			_PickUsername();
		}
		try
		{
			CoopModAssets.LoadAssets();
		}
		catch (Exception ex2)
		{
			KrokoshaCasualtiesMP.log.error(string.Format("{0} {1} failed to load ASSETS!!!\n\n{2}\n", "KrokoshaCasualtiesMP", "4.1.2", ex2));
			LoadErrorNotifier.errorstr = LoadErrorNotifier.errorstr + "\n" + ex2.ToString();
			ComponentHolderProtocol.AddComponent<LoadErrorNotifier>((Object)(object)this);
			return;
		}
		log.LogInfo((object)"Plugin KrokoshaCasualtiesMP assets loaded!");
		KrokoshaScavMultiplayer krokoshaScavMultiplayer = ComponentHolderProtocol.AddComponent<KrokoshaScavMultiplayer>((Object)(object)this);
		try
		{
			KrokoshaScavMultiplayer._FIRST_INIT();
			log.LogInfo((object)"Created multiplayer singleton GameObject!");
		}
		catch (Exception ex3)
		{
			KrokoshaCasualtiesMP.log.error(string.Format("{0} {1} failed to load!\n\n{2}\n", "KrokoshaCasualtiesMP", "4.1.2", ex3));
			LoadErrorNotifier.errorstr = LoadErrorNotifier.errorstr + "\n" + ex3.ToString();
			ComponentHolderProtocol.AddComponent<LoadErrorNotifier>((Object)(object)this);
			Object.Destroy((Object)(object)krokoshaScavMultiplayer);
			return;
		}
		SUCCESFULLY_INITIALIZED = true;
	}

	public static void Startcorout(IEnumerator f)
	{
		((MonoBehaviour)s.GetComponent<KrokoshaScavMultiplayer>()).StartCoroutine(f);
	}

	private void Start()
	{
		((MonoBehaviour)this).Invoke("DelayedCoroutineToSkipScavIntro", 0.5f);
		log.LogInfo((object)"Invoked main KrokoshaCasualtiesMP function, 1s...");
	}

	private void OnDestroy()
	{
		KrokoshaCasualtiesMP.log.l("Plugin.OnDestroy()");
	}

	public static void SkipMainMenuIntro()
	{
		try
		{
			SkipWarningScreen();
			ScrollableText.ForceClose();
		}
		catch (Exception ex)
		{
			KrokoshaCasualtiesMP.log.error(ex.ToString());
		}
	}

	public static void SkipWarningScreen()
	{
		GameObject val = GameObject.Find("Canvas/Warning");
		if ((Object)(object)val != (Object)null && val.activeSelf)
		{
			log.LogInfo((object)"Removing warning screen! ");
			val.SetActive(false);
		}
	}

	private void DelayedCoroutineToSkipScavIntro()
	{
		((MonoBehaviour)this).StartCoroutine(DelayedDelayedTestGameStartImmidiate());
	}

	private static void CheckCommandlineArgsForThisMod()
	{
		startworld = 0;
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		for (int i = 0; i < commandLineArgs.Length; i++)
		{
			commandLineArgs[i] = commandLineArgs[i].Replace("--ksmulti-", "--mp-");
		}
		dump_game_ids = false;
		Func<string, string, bool> func = (string arg, string thing) => arg == "--" + thing || arg == "-" + thing || arg == thing;
		for (int num = 0; num < commandLineArgs.Length; num++)
		{
			string text = commandLineArgs[num];
			if (text == "--mp-disable" || func(text, "nomp") || func(text, "no-mp") || func(text, "nomultiplayer") || func(text, "no-multiplayer"))
			{
				FORCE_DISABLE_MP_MOD = true;
				string msg = "CMD: Force-Disable multiplayer";
				log.LogInfo((object)msg);
				try
				{
					Util.CallLambdaWhen(() => (Object)(object)Con.con != (Object)null, (Action)delegate
					{
						Con.con.LogToConsole(msg);
					}, 1f);
					return;
				}
				catch (Exception)
				{
					return;
				}
			}
			if (text == "--mp-starthost")
			{
				start_network = true;
				cmd_start__is_client = false;
			}
			if (text == "--mp-startclient")
			{
				start_network = true;
				cmd_start__is_client = true;
			}
			if (text == "--mp-startserver")
			{
				start_network = true;
				cmd_start__is_client = false;
				cmd_start__is_dedicated_server = true;
				KrokoshaScavMultiplayer.rules.AutoContinue = true;
			}
			if (text == "--mp-sethost")
			{
				cmd_start__is_client = false;
				cmd_start__is_dedicated_server = false;
			}
			if (text == "--mp-setclient")
			{
				cmd_start__is_client = true;
			}
			if (text == "--mp-setserver")
			{
				cmd_start__is_client = false;
				cmd_start__is_dedicated_server = true;
				KrokoshaScavMultiplayer.rules.AutoContinue = true;
			}
			if (text == "--dump-game-ids")
			{
				dump_game_ids = true;
			}
			if (text == "--mp-startnetwork")
			{
				start_network = true;
			}
			if (text == "--mp-immidiate-start")
			{
				do_immidiate_start = true;
			}
			if (text == "--mp-setstarttutorial")
			{
				startworld = 1;
			}
			if (text == "--mp-setstartdebug")
			{
				startworld = 2;
			}
			if (text == "--mp-verbose")
			{
				DebugMenuSettings.is_verbose = true;
				DebugMenuSettings._DEV_VISUALISE_NET_EVENTS = true;
			}
			if (text == "--mp-ip-port")
			{
				num++;
				if (num == commandLineArgs.Length)
				{
					break;
				}
				text = (KrokoshaScavMultiplayer.INPUT_IPPORT = commandLineArgs[num]);
			}
			if (text == "--mp-setname")
			{
				num++;
				if (num == commandLineArgs.Length)
				{
					break;
				}
				text = commandLineArgs[num];
				KrokoshaScavMultiplayer.INPUT_USERNAME = KrokoshaScavMultiplayer.SanitizeTextInput(text);
				plrname_override = true;
				log.LogInfo((object)("CMD: set name to: " + KrokoshaScavMultiplayer.INPUT_USERNAME));
			}
			if (text == "--mp-setpass")
			{
				num++;
				if (num == commandLineArgs.Length)
				{
					break;
				}
				text = commandLineArgs[num];
				юзер_прошаренный = true;
				KrokoshaScavMultiplayer.INPUT_PASSWORD = text;
				log.LogInfo((object)("CMD: set password to: " + KrokoshaScavMultiplayer.INPUT_PASSWORD));
			}
			if (text == "--mp-setrule")
			{
				num++;
				if (num == commandLineArgs.Length)
				{
					break;
				}
				text = commandLineArgs[num];
				current_rule_to_set = text;
				num++;
				if (num == commandLineArgs.Length)
				{
					break;
				}
				text = commandLineArgs[num];
				rules_override = true;
				юзер_прошаренный = true;
				FieldInfo field = typeof(KrokoshaMultiplayerGameRules).GetField(current_rule_to_set);
				if (field == null)
				{
					log.LogError((object)("CMD: UNKNOWN RULE: " + current_rule_to_set + " (attempt to set it to: " + text + ")"));
					continue;
				}
				object value;
				try
				{
					value = ((!(field.FieldType == typeof(bool))) ? TypeDescriptor.GetConverter(field.FieldType).ConvertFromInvariantString(text) : ((object)Con.ParseBool01(text)));
				}
				catch (Exception ex2)
				{
					log.LogError((object)("CMD: SET RULE, BUT VALUE IS INVALID:\nINPUT: " + current_rule_to_set + " = " + text + "\n" + ex2.Message));
					continue;
				}
				TypedReference obj = __makeref(KrokoshaScavMultiplayer.rules);
				field.SetValueDirect(obj, value);
				log.LogInfo((object)$"CMD: set rule: {current_rule_to_set} = {field.GetValue(KrokoshaScavMultiplayer.rules)}");
				continue;
			}
			if (text == "--mp-test")
			{
				num++;
				if (num == commandLineArgs.Length)
				{
					break;
				}
				text = (test_scenario = commandLineArgs[num]);
			}
			if (text == "--mp-runcommand" || text == "--ksm-con")
			{
				num++;
				if (num == commandLineArgs.Length)
				{
					break;
				}
				text = commandLineArgs[num];
				юзер_прошаренный = true;
				commands_to_run.Add(text);
			}
			if (text == "--mp-servername")
			{
				num++;
				if (num == commandLineArgs.Length)
				{
					break;
				}
				text = commandLineArgs[num];
				юзер_прошаренный = true;
				Net.MY_SERVER_INFO.name = text;
			}
			if (text == "--mp-hoststeam")
			{
				cmd_start__steam_lobby = true;
				start_network = true;
				log.LogInfo((object)"CMD: Will start steam lobby");
				num++;
				if (num == commandLineArgs.Length)
				{
					break;
				}
				text = (cmd_start__steam_lobby_type = commandLineArgs[num]);
				log.LogInfo((object)("CMD: Will start steam lobby of type:" + text));
			}
			if (text == "--mp-nosteam" || func(text, "nosteam") || func(text, "no-steam"))
			{
				FORCE_NO_STEAM = true;
			}
		}
		юзер_прошаренный = юзер_прошаренный || FORCE_NO_STEAM || start_network || startworld != 0 || do_immidiate_start || plrname_override;
		if (start_network)
		{
			Application.runInBackground = true;
			log.LogInfo((object)"CMD: Will start network.");
		}
		if (do_immidiate_start)
		{
			if (startworld != 0)
			{
				log.LogInfo((object)$"CMD: Will start immidiately in {startworld}.");
			}
			else
			{
				log.LogInfo((object)"CMD: Will start immidiately.");
			}
		}
	}

	private IEnumerator DelayedDelayedTestGameStartImmidiate()
	{
		log.LogInfo((object)$"Found args... {start_network} {start_network} {cmd_start__is_client} ");
		if (start_network)
		{
			Application.runInBackground = true;
			if (cmd_start__steam_lobby)
			{
				cmd_start__steam_lobby_type = cmd_start__steam_lobby_type.ToLower();
				TransportSteamworks.OnWantToHostLobby((!cmd_start__is_dedicated_server) ? Net.NetType.Host : Net.NetType.DedicatedServer, (ELobbyType)((!(cmd_start__steam_lobby_type == "private")) ? ((cmd_start__steam_lobby_type == "friendsonly") ? 1 : 2) : 0));
			}
			else if (cmd_start__is_client)
			{
				yield return (object)new WaitForSeconds(1f);
				TransportLiteNetLib.OnWantToConnect("localhost:7790", Net.NetType.Client);
			}
			else
			{
				if (cmd_start__is_dedicated_server)
				{
					TransportLiteNetLib.OnWantToConnect("localhost:7790", Net.NetType.DedicatedServer);
				}
				else
				{
					TransportLiteNetLib.OnWantToConnect("localhost:7790", Net.NetType.Host);
				}
				yield return (object)new WaitForSeconds(3f);
			}
		}
		else
		{
			Application.runInBackground = false;
		}
		ConsoleScript.CheckForConsole();
		foreach (string item in commands_to_run)
		{
			try
			{
				KrokoshaCasualtiesMP.log.l("CMD: Running command \"" + item + "\"");
				ConsoleScript.instance.RunCommandString(item);
			}
			catch (Exception arg)
			{
				KrokoshaCasualtiesMP.log.error($"CMD: Error while running command \"{item}\"\n{arg}");
			}
		}
		if (do_immidiate_start)
		{
			SkipWarningScreen();
			if (do_immidiate_start && (!start_network || !cmd_start__is_client))
			{
				PreRunScript val = Object.FindObjectOfType<PreRunScript>();
				if (startworld != 0)
				{
					if (startworld == 1)
					{
						val.StartTutorial();
					}
					else if (startworld == 2)
					{
						WorldgenPatches.LoadDebugWorld();
					}
					else
					{
						SceneManager.LoadScene("SampleScene");
						Util.CallLambdaWhen(() => (Object)(object)WorldGeneration.world != (Object)null, (Action)delegate
						{
							//IL_001b: Unknown result type (might be due to invalid IL or missing references)
							WorldGeneration.world.biomeOverride = (OverrideSceneType)((startworld == 1) ? 1 : ((startworld != 0) ? 2 : 0));
						});
						if (KrokoshaScavMultiplayer.is_server)
						{
							ServerMain.Server_Announce_GAME_START();
						}
					}
				}
				else
				{
					val.StartRun();
				}
			}
		}
		if (test_scenario != null)
		{
			yield return DEV_RunTestScenario();
		}
	}

	private static IEnumerator DEV_RunTestScenario()
	{
		yield return null;
	}

	private void Update()
	{
	}
}
