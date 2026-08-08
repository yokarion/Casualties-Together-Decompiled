using HarmonyLib;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(MusicManager), "Start")]
public static class MusicManager_Start_MultiplayerPatch
{
	public static AudioSource source;

	public static float musicvolume
	{
		get
		{
			return Settings.Get<SettingFloat>("musicvolume").value;
		}
		set
		{
			SettingFloat obj = Settings.Get<SettingFloat>("musicvolume");
			obj.value = value;
			((Setting)obj).Apply();
		}
	}

	public static float musicsources_volume
	{
		get
		{
			return musicvolume;
		}
		set
		{
			if ((Object)(object)MusicManager.main != (Object)null)
			{
				MusicManager.main.origVolume = value;
			}
			if ((Object)(object)PreRunScript.instance != (Object)null)
			{
				PreRunScript.instance.menuSource.volume = value * 0.6666f;
			}
		}
	}

	private static void Postfix(MusicManager __instance)
	{
		source = ((Component)__instance).GetComponent<AudioSource>();
	}
}
