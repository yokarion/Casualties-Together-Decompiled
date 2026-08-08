using HarmonyLib;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(MP3Menu), "Exit")]
public static class MP3Menu_Exit_MultiplayerPatch
{
	public static bool Prefix(MP3Menu __instance)
	{
		MP3Menu_UpdateList_MultiplayerPatch.last_locally_used_mp3_player = null;
		return true;
	}
}
