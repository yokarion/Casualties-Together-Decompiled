using System.Runtime.CompilerServices;
using HarmonyLib;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(WorldGeneration), "SetBlock")]
public static class WorldGenerationSetBlockPatch
{
	public static bool tile_did_change;

	public static Vector2Int last_tile_change_pos;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void JustSeparatedSetBlock(in WorldGeneration __instance, in Vector2Int pos, in ushort block, in bool IsNoUpdate)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (!WorldGenerationGenerateBlockCirclePatch.doing_autoUpdateChunk)
		{
			last_tile_change_pos = pos;
			tile_did_change = true;
		}
	}

	private static bool Prefix(WorldGeneration __instance, Vector2Int pos, ushort block)
	{
		JustSeparatedSetBlock(in __instance, in pos, in block, false);
		return true;
	}
}
