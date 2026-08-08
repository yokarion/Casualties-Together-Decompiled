using HarmonyLib;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(KeypadMinigame), "CheckForMegalovania")]
internal static class abc
{
	private static bool Prefix(KeypadMinigame __instance)
	{
		return false;
	}
}
