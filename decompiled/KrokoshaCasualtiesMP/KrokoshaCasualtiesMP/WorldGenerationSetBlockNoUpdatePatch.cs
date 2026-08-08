using HarmonyLib;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(WorldGeneration), "SetBlockNoUpdate")]
internal static class WorldGenerationSetBlockNoUpdatePatch
{
	private static bool Prefix(WorldGeneration __instance, Vector2Int pos, ushort block)
	{
		WorldGenerationSetBlockPatch.JustSeparatedSetBlock(in __instance, in pos, in block, true);
		return true;
	}
}
