using HarmonyLib;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(Item), "OnWillRenderObject")]
internal static class Item_OnWillRenderObject_MultiplayerPatch
{
	private static bool Prefix(Item __instance)
	{
		KrokoshaScavMultiGameObjectNetworkTracker krokoshaScavMultiGameObjectNetworkTracker = default(KrokoshaScavMultiGameObjectNetworkTracker);
		if (KrokoshaScavMultiplayer.network_system_is_running && ((Component)__instance).TryGetComponent<KrokoshaScavMultiGameObjectNetworkTracker>(ref krokoshaScavMultiGameObjectNetworkTracker))
		{
			krokoshaScavMultiGameObjectNetworkTracker.is_far_but_still_around_any_plr = true;
			krokoshaScavMultiGameObjectNetworkTracker.is_within_anyones_view = true;
			return false;
		}
		return true;
	}
}
