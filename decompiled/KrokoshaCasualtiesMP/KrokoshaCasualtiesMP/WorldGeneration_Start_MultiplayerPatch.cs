using HarmonyLib;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(WorldGeneration), "Start")]
internal static class WorldGeneration_Start_MultiplayerPatch
{
	private static bool Prefix(WorldGeneration __instance)
	{
		ComponentHolderProtocol.GetOrAddComponent<WorldTracker>((Object)(object)__instance);
		return true;
	}

	private static void Postfix(WorldGeneration __instance)
	{
	}
}
