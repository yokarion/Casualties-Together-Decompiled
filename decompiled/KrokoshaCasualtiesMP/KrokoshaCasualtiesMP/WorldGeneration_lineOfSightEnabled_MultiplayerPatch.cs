using HarmonyLib;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(/*Could not decode attribute arguments.*/)]
internal static class WorldGeneration_lineOfSightEnabled_MultiplayerPatch
{
	private static void Postfix(WorldGeneration __instance, ref bool __result)
	{
		if (__result && PlayerCamera_HandleScreenShaders_MultiplayerPatch.CURRENTLY_OVERRIDING_VISUALS)
		{
			__result = false;
			if ((Object)(object)PlayerCamera.main != (Object)null)
			{
				PlayerCamera.main.LOSHole.GetComponent<VisionMask>().ClearMask();
			}
		}
	}
}
