using HarmonyLib;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(PreRunScript), "StartTutorial")]
public static class PreRunScript_StartTutorial_MultiplayerPatch
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
			WorldgenPatches.LoadVanillaTutorialWorld();
			return false;
		}
		return true;
	}
}
