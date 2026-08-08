using HarmonyLib;
using KrokoshaCasualtiesUtils;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(Body), "WearWearable")]
public static class Body_WearWearable_MultiplayerPatch
{
	private static void Postfix(Body __instance, Item item)
	{
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		if (!Net.running)
		{
			return;
		}
		BodyGetterOverrider bodyGetterOverrider = default(BodyGetterOverrider);
		if (((Component)item).TryGetComponent<BodyGetterOverrider>(ref bodyGetterOverrider))
		{
			bodyGetterOverrider.SetBody(__instance);
		}
		if (KrokoshaScavMultiplayer.is_server && NetObjectRegistry.TryGetSyncInfoOrRegister((Component)(object)item, out var si))
		{
			NetObjectRegistry.Server_QueueSync(si);
		}
		if (!Util.IsWorldGenerated())
		{
			return;
		}
		if (Util.IsBodyLocal(__instance) && !ItemSync.ItemCanBeIgnoredForNetwork(item) && (Object)(object)__instance.GetWearable(item.id) == (Object)(object)item && ItemSync.SyncRegistry.TryGetValue(((Component)item).gameObject, out var value))
		{
			value.SetIgnoreTimeForRoundTrip();
			NetDataWriter writer = Net.CreateWriter(10113);
			writer.Put((ushort)value.syncId);
			Net.Client_Send((DeliveryMethod)0, in writer);
			if (log.verbose)
			{
				Plugin.log.LogInfo((object)$"CLIENT: sending ItemWearing {value}");
			}
		}
	}
}
