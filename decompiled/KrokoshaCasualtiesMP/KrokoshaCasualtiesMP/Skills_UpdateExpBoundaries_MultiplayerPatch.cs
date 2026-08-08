using HarmonyLib;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(Skills), "UpdateExpBoundaries")]
internal static class Skills_UpdateExpBoundaries_MultiplayerPatch
{
	private static void Postfix(Skills __instance)
	{
		if (__instance.maxSTR < 0)
		{
			__instance.maxSTR = int.MaxValue;
		}
		if (__instance.maxINT < 0)
		{
			__instance.maxINT = int.MaxValue;
		}
		if (__instance.maxRES < 0)
		{
			__instance.maxRES = int.MaxValue;
		}
	}
}
