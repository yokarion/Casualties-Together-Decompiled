using HarmonyLib;
using KrokoshaCasualtiesUtils;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(ManualDefibMinigame), "GetEstimationRange")]
internal static class ManualDefibMinigame_GetEstimationRange_MultiplayerPatch
{
	private static bool Prefix(ManualDefibMinigame __instance, ref float __result)
	{
		float num = 70f - Util.GetLocalBody().skills.INTFrom10 * 5f;
		if (num < 5f)
		{
			num = 5f;
		}
		__result = num;
		return false;
	}
}
