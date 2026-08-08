using HarmonyLib;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(/*Could not decode attribute arguments.*/)]
public static class AdaptiveButton_overlayActive_MultiplayerPatch
{
	public static void Postfix(AdaptiveButton __instance, ref bool __result)
	{
		if (Con.IsConsoleOpen())
		{
			__result = true;
		}
		if (UIMainMenu.IsOpen())
		{
			__result = true;
		}
	}
}
