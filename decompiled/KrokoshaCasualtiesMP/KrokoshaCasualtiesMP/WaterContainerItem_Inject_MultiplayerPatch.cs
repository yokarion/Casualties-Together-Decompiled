using HarmonyLib;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(WaterContainerItem), "Inject")]
public static class WaterContainerItem_Inject_MultiplayerPatch
{
	public static Limb listen_to_limb;

	public static float listen_to_limb_amount;

	public static bool Prefix(WaterContainerItem __instance, Limb limb, float amount)
	{
		if (!KrokoshaScavMultiplayer.network_system_is_running)
		{
			return true;
		}
		CoUtils_instance_MultiplayerPatch.cur_override_instance = limb.GetCoUtilsInstance();
		if ((Object)(object)limb == (Object)(object)listen_to_limb)
		{
			listen_to_limb_amount += amount;
		}
		if (NetObjectRegistry.TryGetSyncInfo(((Component)__instance).gameObject, out var si))
		{
			si.SetIgnoreTimeForRoundTrip();
		}
		return true;
	}
}
