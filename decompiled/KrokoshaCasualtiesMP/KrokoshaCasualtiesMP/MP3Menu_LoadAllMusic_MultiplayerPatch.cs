using System;
using System.Collections;
using HarmonyLib;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(MP3Menu), "LoadAllMusic")]
public static class MP3Menu_LoadAllMusic_MultiplayerPatch
{
	[HarmonyReversePatch(/*Could not decode attribute arguments.*/)]
	public static IEnumerator LoadAllMusic(object instance, string folderPath)
	{
		throw new NotImplementedException();
	}

	public static IEnumerator Patched_LoadAllMusic(MP3Menu __instance, string folderPath)
	{
		if (!KrokoshaScavMultiplayer.IsNetworkActiveAndIsClient())
		{
			if (KrokoshaScavMultiplayer.is_server)
			{
				KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog("SERVER: Loading custom music.");
			}
			else
			{
				KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog("Loading custom music.");
			}
			yield return LoadAllMusic(__instance, folderPath);
		}
		if ((Object)(object)MP3PlayerServerAudioStreamer.temp_loader_mp3menu_instance != (Object)null)
		{
			Object.Destroy((Object)(object)MP3PlayerServerAudioStreamer.temp_loader_mp3menu_instance);
			MP3PlayerServerAudioStreamer.temp_loader_mp3menu_instance = null;
		}
		if (!KrokoshaScavMultiplayer.IsNetworkActiveAndIsClient())
		{
			if (KrokoshaScavMultiplayer.IsNetworkActiveAndIsServer())
			{
				KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog("SERVER: Sending custom music list to players.");
				MP3PlayerServerAudioStreamer.Server_SendMusicList();
			}
			else
			{
				KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog("Finished loading custom music.");
			}
		}
	}

	public static bool Prefix(MP3Menu __instance, string folderPath, ref IEnumerator __result)
	{
		__result = Patched_LoadAllMusic(__instance, folderPath);
		return false;
	}
}
