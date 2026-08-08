using HarmonyLib;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(PreRunScript), "StartRun")]
public static class PreRunScript_StartRun_MultiplayerPatch
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
			SaveSystem.loadedRun = false;
			WorldGeneration.runSettings = __instance.runSettings;
			WorldgenPatches.LoadVanillaGeneratedWorld();
			return false;
		}
		KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog("You can't play singleplayer with MP mod active.\nTo play singleplayer go to \"MP Mod Menu > Settings > General > Deactivate Multiplayer Mod\"\nOr start an empty server.");
		return false;
	}
}
