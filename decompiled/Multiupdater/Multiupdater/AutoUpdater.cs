using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;
using HarmonyLib;
using KrokoshaCasualtiesMP;
using Unity.VisualScripting;
using UnityEngine;

namespace Multiupdater;

[BepInPlugin("dannad.krmultiupdater", "multiupdater", "0.0.1")]
[BepInDependency(/*Could not decode attribute arguments.*/)]
public class AutoUpdater : BaseUnityPlugin
{
	private static Version DEBUG_remote = new Version("10.10.10");

	internal static ManualLogSource Logger;

	internal static AutoupdaterGUI GUI;

	private static Version local = null;

	private static Version remote = null;

	internal static (Version, string) latestVersionAndURL = (null, null);

	public static Action OnStageSuccess;

	public static Action OnStageFailure;

	private static Assembly KrokMPAssembly = null;

	private static bool Updating = false;

	public static string Status => AutoupdaterGUI.Status;

	private void _LoadKrokMPAssembly()
	{
		try
		{
			KrokMPAssembly = Assembly.GetAssembly(typeof(KrokoshaScavSingleton));
		}
		catch (Exception)
		{
		}
	}

	private async void Awake()
	{
		_LoadKrokMPAssembly();
		Logger = ((BaseUnityPlugin)this).Logger;
		bool flag = true;
		try
		{
			flag = ShouldStartImmidiately();
		}
		catch (Exception)
		{
		}
		if (flag)
		{
			InitGUI();
			await StartChecking();
		}
	}

	private static bool ShouldStartImmidiately()
	{
		try
		{
			return !Plugin.FORCE_DISABLE_MP_MOD && (!Plugin.SUCCESFULLY_INITIALIZED || Traverse.Create(typeof(Plugin)).Field("TARGET_GAME_VERSION").GetValue<string>() != Application.version);
		}
		catch (Exception)
		{
			return true;
		}
	}

	private static bool MPIsReleaseBuild()
	{
		try
		{
			return Traverse.Create(typeof(Plugin)).Field("IS_RELEASE_BUILD").GetValue<bool>();
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static async Task StartChecking()
	{
		if (Updating)
		{
			return;
		}
		try
		{
			Updating = true;
			LogMessage("Checking for updates");
			await ParseVersions();
			if (remote != null && IsCurrentVersionOutdated())
			{
				LogMessage($"Update found! Your version: {local}; New version: {remote}!");
				AutoupdaterGUI.ShowUpdateButton = true;
				await InitiateUpdate();
			}
			Updating = false;
		}
		catch (Exception ex)
		{
			Updating = false;
			LogError(ex.ToString());
		}
		Updating = false;
	}

	private static void LogToGameConsole(in string str)
	{
		if ((Object)(object)ConsoleScript.instance != (Object)null)
		{
			ConsoleScript.instance.LogToConsole(str);
		}
	}

	internal static void LogMessage(in string str)
	{
		AutoupdaterGUI.Status = str;
		try
		{
			LogToGameConsole("AutoUpdater: " + str);
		}
		catch (Exception arg)
		{
			Logger.LogError((object)$"Failed to log ingame: {arg}");
		}
		Logger.LogMessage((object)str);
	}

	internal static void LogError(in string str)
	{
		AutoupdaterGUI.Status = "ERROR: " + str;
		try
		{
			LogToGameConsole("<color=red>AutoUpdater ERROR: " + str + "</color>");
		}
		catch (Exception arg)
		{
			Logger.LogError((object)$"Failed to log ingame: {arg}");
		}
		Logger.LogError((object)str);
	}

	public static async Task ParseVersions()
	{
		try
		{
			local = GetLocalVersion();
			latestVersionAndURL = await GetLatestTagAndDownloadURL();
			if (remote != null)
			{
				LogMessage($"Local version: {local}  Remote version: {remote}");
			}
		}
		catch (Exception arg)
		{
			LogError($"Something went wrong while parsing versions! {arg}");
		}
	}

	public static async Task InitiateUpdate()
	{
		if (latestVersionAndURL.Item1 == null)
		{
			LogError("Tried to call InitiateUpdate() with null version! Did you run ParseVersions() beforehand?");
			return;
		}
		if (latestVersionAndURL.Item2 == null)
		{
			LogError("Tried to call InitiateUpdate() with null download URL! Did you run ParseVersions() beforehand?");
			return;
		}
		try
		{
			await StageVersion(latestVersionAndURL.Item2);
		}
		catch (Exception arg)
		{
			LogError($"Exception while initiating update! {arg}");
		}
	}

	public static async Task<(Version, string)> GetLatestTagAndDownloadURL()
	{
		try
		{
			(Version, string) obj = await GetRemoteVersion();
			(remote, _) = obj;
			return obj;
		}
		catch (TimeoutException)
		{
			LogMessage("Github unreachable");
			return (null, null);
		}
		catch (Exception arg)
		{
			LogMessage($"Something went wrong while trying to parse versions! {arg}");
			return (null, null);
		}
	}

	public static bool IsCurrentVersionOutdated()
	{
		return IsVersionOutdated(latestVersionAndURL);
	}

	public static bool IsVersionOutdated((Version, string) TagAndURL)
	{
		bool flag = false;
		try
		{
			flag = MPIsReleaseBuild();
		}
		catch (Exception)
		{
			flag = false;
		}
		if (remote >= local)
		{
			if (remote == local && flag)
			{
				LogMessage("You're running the latest version.");
				return false;
			}
			LogMessage("Multiplayer version is outdated");
			return true;
		}
		LogMessage("You're running the pre-release build.");
		return false;
	}

	private static Version GetLocalVersion()
	{
		if (Chainloader.PluginInfos.TryGetValue("KrokoshaCasualtiesMP", out var value))
		{
			return value.Metadata.Version;
		}
		throw new Exception("Could not locate mod by guid somehow!!");
	}

	public static async Task<(Version, string)> GetRemoteVersion()
	{
		try
		{
			return await HTTPHandler.GetLatestRelease();
		}
		catch (TimeoutException innerException)
		{
			LogMessage("Github unreachable when retrieving version.");
			throw new TimeoutException("Github unreachable when retrieving version", innerException);
		}
		catch (InvalidDataException innerException2)
		{
			throw new Exception("No suitable version has been found in the release list, meaning that it's either empty, or there are somehow only blacklisted versions now.", innerException2);
		}
		catch (Exception innerException3)
		{
			throw new Exception("Mystery Tag Parsing Exception!", innerException3);
		}
	}

	public static async Task<bool> StageVersion(string ArchiveURL)
	{
		if (await Updater.InitiateUpdate(ArchiveURL))
		{
			OnStageSuccess?.Invoke();
			return true;
		}
		OnStageFailure?.Invoke();
		return false;
	}

	private void InitGUI()
	{
		GUI = ComponentHolderProtocol.GetOrAddComponent<AutoupdaterGUI>((Object)(object)((Component)this).gameObject);
	}

	private void OnLoad()
	{
	}

	private void OnApplicationQuit()
	{
	}
}
