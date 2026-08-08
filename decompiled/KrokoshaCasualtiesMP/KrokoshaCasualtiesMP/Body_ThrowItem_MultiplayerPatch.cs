using System;
using HarmonyLib;
using KrokoshaCasualtiesUtils;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(Body), "ThrowItem")]
internal static class Body_ThrowItem_MultiplayerPatch
{
	private static void Prefix(Body __instance, float force, ref SyncInfo __state)
	{
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		__state = null;
		if (!KrokoshaScavMultiplayer.IsNetworkActiveAndIsWorldGenerated())
		{
			return;
		}
		Item item = __instance.GetItem(__instance.handSlot);
		if (Util.IsBodyLocal(__instance) && (Object)(object)item != (Object)null && NetObjectRegistry.TryGetSyncInfo((Component)(object)item, out __state))
		{
			int num = Array.IndexOf(ItemSync.Client_last_inventory_state, __state);
			if (num != -1)
			{
				ItemSync.Client_last_inventory_state[num] = null;
			}
			__state.SetIgnoreTimeForRoundTrip();
			NetDataWriter writer = Net.CreateWriter(10115);
			writer.Put((ushort)__state.syncId);
			writer.Put(force);
			if (log.verbose)
			{
				log.l($"CLIENT: requesting ThrowItem: {__state} ");
			}
			Net.Client_Send((DeliveryMethod)0, in writer);
		}
	}
}
