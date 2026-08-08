using HarmonyLib;
using KrokoshaCasualtiesUtils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(Body), "HandleVisuals")]
public static class Body_HandleVisuals_MultiplayerPatch
{
	private static void Prefix(Body __instance, Painkillers pnk, ref bool __state)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		__state = __instance.inWater;
		if (!Util.IsBodyLocal(__instance))
		{
			__instance.inWater = false;
			__instance.lastLiquidColor = Color.clear;
		}
	}

	private static void Postfix(Body __instance, ref bool __state)
	{
		__instance.inWater = __state;
	}
}
