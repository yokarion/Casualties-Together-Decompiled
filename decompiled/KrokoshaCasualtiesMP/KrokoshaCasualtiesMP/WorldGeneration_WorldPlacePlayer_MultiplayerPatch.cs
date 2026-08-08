using System;
using System.Collections;
using HarmonyLib;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(WorldGeneration), "WorldPlacePlayer")]
public static class WorldGeneration_WorldPlacePlayer_MultiplayerPatch
{
	[HarmonyReversePatch(/*Could not decode attribute arguments.*/)]
	public static IEnumerator WorldPlacePlayer(object instance)
	{
		throw new NotImplementedException();
	}

	public static bool Prefix(WorldGeneration __instance, ref IEnumerator __result)
	{
		__result = WorldgenPatches.Patched_WorldPlacePlayer();
		return false;
	}
}
