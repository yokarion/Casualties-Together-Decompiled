using HarmonyLib;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(WorldGeneration), "GenerateBlockCircle")]
public static class WorldGenerationGenerateBlockCirclePatch
{
	public static bool doing_autoUpdateChunk;

	public static bool Prefix(WorldGeneration __instance, Vector2 pos, int size, ushort block, float chance, float chanceEnd, bool autoUpdateChunk = false, bool force = false)
	{
		doing_autoUpdateChunk = autoUpdateChunk;
		return true;
	}

	public static void Postfix(WorldGeneration __instance, Vector2 pos, int size, ushort block, float chance, float chanceEnd, bool autoUpdateChunk = false, bool force = false)
	{
		doing_autoUpdateChunk = false;
	}
}
