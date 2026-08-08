using HarmonyLib;
using KrokoshaCasualtiesUtils;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(Body), "DoPickupCheck")]
internal static class Body_DoPickupCheck_MultiplayerPatch
{
	private static bool Prefix(Body __instance, Item item, ref bool noAlerts, ref bool __result)
	{
		if (!__instance.conscious || Util.IsGeneratingWorld())
		{
			noAlerts = true;
		}
		if (KrokoshaScavMultiplayer.network_system_is_running && (!__instance.IsBodyLocal() || !__instance.conscious))
		{
			noAlerts = true;
			__result = true;
			return false;
		}
		return true;
	}
}
