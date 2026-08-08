using HarmonyLib;
using KrokoshaCasualtiesUtils;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(Body), "DropWearable")]
public static class Body_DropWearable_MultiplayerPatch
{
	private static bool force_this_shit;

	public static void Force(Body __instance, Item item)
	{
		force_this_shit = true;
		__instance.DropWearable(item);
		force_this_shit = false;
	}

	public static void Postfix(Body __instance, Item item)
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		if (!Net.running)
		{
			return;
		}
		if (KrokoshaScavMultiplayer.is_server && NetObjectRegistry.TryGetSyncInfoOrRegister((Component)(object)item, out var si))
		{
			NetObjectRegistry.Server_QueueSync(si);
		}
		if (!force_this_shit && Util.IsWorldGenerated() && Util.IsBodyLocal(__instance) && !ItemSync.ItemCanBeIgnoredForNetwork(item) && ItemSync.SyncRegistry.TryGetValue(((Component)item).gameObject, out var value))
		{
			value.SetIgnoreTimeForRoundTrip();
			NetDataWriter writer = Net.CreateWriter(10112);
			writer.Put((ushort)value.syncId);
			Net.Client_Send((DeliveryMethod)2, in writer);
			if (log.verbose)
			{
				Plugin.log.LogInfo((object)$"CLIENT: sending ItemDropWearable {value.syncId} {value.go}  ");
			}
		}
	}
}
