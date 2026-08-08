using HarmonyLib;
using KrokoshaCasualtiesUtils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(Body), "PickUpItem")]
public static class Body_PickUpItem_MultiplayerPatch
{
	private static void Prefix(Body __instance)
	{
	}

	public static void Postfix(Body __instance, Item item, int slot, bool force)
	{
		if (__instance.IsBodyLocal() && PlayerCamera.main.craftingPanel.activeSelf)
		{
			PlayerCamera.main.RefreshRecipeList();
		}
		if (!KrokoshaScavMultiplayer.network_system_is_running)
		{
			return;
		}
		if (SharedMain.local_world_is_generated)
		{
			SyncInfo si2;
			if (KrokoshaScavMultiplayer.is_server)
			{
				if (NetObjectRegistry.TryGetSyncInfo((Component)(object)item, out var si))
				{
					NetObjectRegistry.Server_QueueSync(si);
				}
			}
			else if (__instance.IsBodyLocal() && !force && !ItemSync.ItemCanBeIgnoredForNetwork(item) && __instance.HoldingItem(item) && NetObjectRegistry.TryGetSyncInfoOrRegister((Component)(object)item, out si2))
			{
				si2.SetIgnoreTimeForRoundTrip(0.30000001192092896);
			}
		}
		BodyGetterOverrider bodyGetterOverrider = default(BodyGetterOverrider);
		if (((Component)item).TryGetComponent<BodyGetterOverrider>(ref bodyGetterOverrider))
		{
			bodyGetterOverrider.SetBody(__instance);
		}
	}
}
