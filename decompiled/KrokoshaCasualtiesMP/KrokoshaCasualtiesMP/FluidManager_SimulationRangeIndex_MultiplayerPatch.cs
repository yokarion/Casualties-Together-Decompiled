using HarmonyLib;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(FluidManager), "SimulationRangeIndex")]
public static class FluidManager_SimulationRangeIndex_MultiplayerPatch
{
	public static bool forcenext_avaiable;

	public static (RangeI, RangeI) forcenext;

	private static bool Prefix(ref (RangeI, RangeI) __result)
	{
		if (forcenext_avaiable)
		{
			forcenext_avaiable = false;
			__result = forcenext;
			return false;
		}
		return true;
	}
}
