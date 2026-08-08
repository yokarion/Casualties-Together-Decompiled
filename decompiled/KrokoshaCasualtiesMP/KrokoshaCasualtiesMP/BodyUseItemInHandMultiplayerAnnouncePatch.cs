using HarmonyLib;
using KrokoshaCasualtiesUtils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(Body), "UseItemInHand")]
public static class BodyUseItemInHandMultiplayerAnnouncePatch
{
	public static bool IsUsingItemInHand;

	public static bool Prefix(Body __instance)
	{
		if (!KrokoshaScavMultiplayer.IsInGameAndWorldGenerated())
		{
			return false;
		}
		if (KrokoshaScavMultiplayer.network_system_is_running)
		{
			if (Util.IsBodyLocal(__instance))
			{
				if (__instance.conscious)
				{
					Item item = __instance.GetItem(__instance.handSlot);
					if ((Object)(object)item != (Object)null && item.Stats.usable && item.Stats.usableWithLMB && NetObjectRegistry.TryGetSyncInfoOrRegister(((Component)item).gameObject, out var si) && !CombatStuff.ItemIsUsedForAttacking(item))
					{
						KrokoshaScavMultiplayer.Client_SendSimpleMessageToServer((ushort)10105, (ushort)si.syncId, (ushort)((Component)__instance).GetComponent<NetBody>().netId, true);
					}
				}
				IsUsingItemInHand = true;
			}
			else
			{
				_ = KrokoshaScavMultiplayer.is_client;
			}
		}
		return true;
	}

	public static void Postfix(Body __instance)
	{
		if (KrokoshaScavMultiplayer.network_system_is_running)
		{
			if (KrokoshaScavMultiplayer.is_client && (Object)(object)Util.GetLocalBody() == (Object)(object)__instance)
			{
				IsUsingItemInHand = false;
			}
		}
	}
}
