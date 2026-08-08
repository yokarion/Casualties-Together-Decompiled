using HarmonyLib;
using KrokoshaCasualtiesUtils;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(/*Could not decode attribute arguments.*/)]
public static class Body_get_canTakeNap_MultiplayerPatch
{
	public static bool force;

	public static void Postfix(Body __instance, ref bool __result)
	{
		if (KrokoshaScavMultiplayer.network_system_is_running && !force)
		{
			if (KrokoshaScavMultiplayer.rules.DisableSleep)
			{
				__result = false;
			}
			if (!Util.IsBodyLocal(__instance))
			{
				__result = true;
			}
		}
		force = false;
	}
}
