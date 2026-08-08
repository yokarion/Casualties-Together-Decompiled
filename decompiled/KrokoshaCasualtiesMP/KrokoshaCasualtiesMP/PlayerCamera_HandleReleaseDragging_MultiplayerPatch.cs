using HarmonyLib;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(PlayerCamera), "HandleReleaseDragging")]
public static class PlayerCamera_HandleReleaseDragging_MultiplayerPatch
{
	private static ItemSync.ItemsContainerInfo last_ici;

	public static void Prefix(PlayerCamera __instance, ref SyncInfo __state)
	{
		__state = null;
		if (!KrokoshaScavMultiplayer.network_system_is_running)
		{
			return;
		}
		_ = __instance.body;
		if (Object.op_Implicit((Object)(object)__instance.dragItem))
		{
			if (ItemSync.TryGetSyncInfo(__instance.dragItem, out var si))
			{
				last_ici = ItemSync.ItemGetContainerInfo(__instance.dragItem);
				__state = si;
			}
			else
			{
				NetObjectRegistry.Client_DeleteUnregisteredObject(((Component)__instance.dragItem).gameObject);
			}
		}
	}

	public static void Postfix(PlayerCamera __instance, ref SyncInfo __state)
	{
		if (__state != null)
		{
			_ = __instance.body;
			ItemSync.ItemsContainerInfo itemsContainerInfo = ItemSync.ItemGetContainerInfo(__state.item);
			if (itemsContainerInfo.JustContainerChanged(last_ici) && (Object)(object)itemsContainerInfo.inv_slot == (Object)null)
			{
				ItemSync.ItemContainerChanged.Add(__state);
			}
		}
	}
}
