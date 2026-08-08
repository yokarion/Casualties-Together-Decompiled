using HarmonyLib;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(PreRunScript), "LoadRun")]
public static class PreRunScript_LoadRun_MultiplayerPatch
{
	private static bool Prefix(PreRunScript __instance)
	{
		if (Net.running)
		{
			if (KrokoshaScavMultiplayer.is_client)
			{
				KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog("You can't start the game, server can.");
				return false;
			}
			SaveSystem.loadedRun = true;
			WorldgenPatches.LoadVanillaGeneratedWorld(loadsave: true);
			return false;
		}
		KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog("You can't play singleplayer with MP mod active.\nTo play singleplayer go to \"MP Mod Menu > Settings > General > Deactivate Multiplayer Mod\"\nOr start an empty server.");
		return false;
	}
}
