using System;
using HarmonyLib;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(Body), "DropItem", new Type[] { typeof(Item) })]
public static class Body_DropItem_MultiplayerPatch
{
	private static void Prefix(Body __instance)
	{
	}

	private static void Postfix(Body __instance, Item item)
	{
		if (KrokoshaScavMultiplayer.network_system_is_running && KrokoshaScavMultiplayer.is_server && NetObjectRegistry.TryGetSyncInfo((Component)(object)item, out var si))
		{
			NetObjectRegistry.Server_QueueSync(si);
		}
	}
}
