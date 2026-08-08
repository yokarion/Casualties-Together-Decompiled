using HarmonyLib;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(Container), "LoadItem")]
internal static class Container_LoadItem_MultiplayerPatch
{
	private static void Prefix(Container __instance, Item item)
	{
		if (ItemSync.TryGetSyncInfo(item, out var si))
		{
			if (KrokoshaScavMultiplayer.is_server)
			{
				NetObjectRegistry.Server_QueueSync(si);
			}
			if (PlayerCamera_TryPerformInventoryAction_MultiplayerPatch.is_inside_TryPerformInventoryAction)
			{
				ItemSync.ItemContainerChanged.Add(si);
				si.SetIgnoreTimeForRoundTrip(1.0);
			}
		}
	}
}
