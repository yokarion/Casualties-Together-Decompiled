using System;
using System.Collections;
using HarmonyLib;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(WorldGeneration), "FinishWorldGeneration")]
public static class WorldGeneration_FinishWorldGeneration_MultiplayerPatch
{
	[HarmonyReversePatch(/*Could not decode attribute arguments.*/)]
	public static IEnumerator FinishWorldGeneration(object instance)
	{
		throw new NotImplementedException();
	}

	public static bool Prefix(WorldGeneration __instance, ref IEnumerator __result)
	{
		__result = WorldgenPatches.Patched_FinishWorldGeneration();
		return false;
	}
}
