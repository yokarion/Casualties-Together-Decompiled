using System.Collections.Generic;
using HarmonyLib;
using LiteNetLib.Utils;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(TraderScript), "GiveItem")]
public static class TraderScript_GiveItem_MultiplayerPatch
{
	public static bool Prefix(TraderScript __instance, ref int __state)
	{
		__state = __instance.totalValueGiven;
		return true;
	}

	public static void Postfix(TraderScript __instance, ref int __state, Item item)
	{
		if (__instance.totalValueGiven <= __state || !KrokoshaScavMultiplayer.network_system_is_running)
		{
			return;
		}
		KrokoshaTraderTrackerComponent orAddComponent = ComponentHolderProtocol.GetOrAddComponent<KrokoshaTraderTrackerComponent>((Object)(object)__instance);
		if (KrokoshaScavMultiplayer.is_client)
		{
			orAddComponent.focused_body = PlayerCamera.main.body;
			if (orAddComponent.is_registered && ItemSync.TryGetSyncInfo(item, out var si))
			{
				if (log.verbose)
				{
					log.l("CLIENT: sending  TraderSellItem ");
				}
				KrokoshaScavMultiplayer.Client_SendSimpleMessageToServer((ushort)10162, (ushort)orAddComponent.si.syncId, (ushort)si.syncId, true);
			}
		}
		else
		{
			orAddComponent.Server_AnnounceTraderReputationState((IReadOnlyList<knetid>)null);
		}
	}

	[ServerReceiver(10162)]
	private static void Server_TraderSell(knetid clientId, ref NetDataReader reader)
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out knetid result);
		reader.Get(out knetid result2);
		if (!TraderSync.Server_TraderInteractionCheck(clientId, result, "SellItem", out var plr, out var trader_si, out var trader_tracker))
		{
			return;
		}
		trader_tracker.focused_body = plr.body;
		if (ItemSync.TryGetItemSyncInfo(result2, out var si) && ItemSync.CheckIfBodyReachThisItem(si, plr.body) && trader_si.trader.reputation >= 30f)
		{
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(Vector2.op_Implicit(((Component)trader_si.trader).transform.position), $"S: Trader sell item {plr.playername} -> {trader_si} item:{si}");
			}
			trader_si.trader.GiveItem(si.item);
		}
		else
		{
			log.serverdeny($"TraderSellItem for {plr} ");
		}
	}
}
