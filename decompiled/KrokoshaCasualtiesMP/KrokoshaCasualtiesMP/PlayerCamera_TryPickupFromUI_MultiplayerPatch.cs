using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(PlayerCamera), "TryPickupFromUI")]
internal static class PlayerCamera_TryPickupFromUI_MultiplayerPatch
{
	private static void Postfix(PlayerCamera __instance, ref List<RaycastResult> uiCasts)
	{
		if (KrokoshaScavMultiplayer.network_system_is_running)
		{
			_ = __instance.body;
			Item dragItem = __instance.dragItem;
			if (!((Object)(object)dragItem == (Object)null) && !NetObjectRegistry.IsRegistered(((Component)dragItem).gameObject) && !NetObjectRegistry.ObjectCanBeIgnoredForNetwork(((Component)dragItem).gameObject))
			{
				NetObjectRegistry.Client_DeleteUnregisteredObject(((Component)dragItem).gameObject);
				log.warn("PlayerCamera.TryPickupFromUI:  User tried to drag an item from UI that is not registered yet! " + dragItem?.id + " ");
				dragItem = null;
				NetObjectRegistry.AlertObjectNotRegistered(popup: true);
			}
		}
	}
}
