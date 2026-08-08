using HarmonyLib;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(WorldGeneration), "DistributeMiniBarrels")]
public static class WorldGeneration_DistributeMiniBarrels_WorldgenPatch
{
	public static bool Prefix(WorldGeneration __instance)
	{
		if (KrokoshaScavMultiplayer.IsNetworkActiveAndIsClient() || WorldgenPatches.aaaaaaaaaaaaaaaaaaaaaaaaaaa != null)
		{
			return false;
		}
		return true;
	}
}
