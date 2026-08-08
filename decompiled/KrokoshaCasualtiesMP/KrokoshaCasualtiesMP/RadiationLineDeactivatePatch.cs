using HarmonyLib;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(RadiationLine), "Deactivate")]
public static class RadiationLineDeactivatePatch
{
	public static void Postfix(RadiationLine __instance)
	{
		RadiationLineUpdatePatch.timeGone = 0f;
	}
}
