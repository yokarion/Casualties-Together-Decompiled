using System.Collections.Generic;
using HarmonyLib;
using LiteNetLib.Utils;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(TraderScript), "Threaten")]
public static class TraderScript_Threaten_MultiplayerPatch
{
	private static bool force;

	public static bool Prefix(TraderScript __instance)
	{
		if (KrokoshaScavMultiplayer.network_system_is_running && !force && (Object)(object)PlayerCamera.main.currentTrader == (Object)(object)__instance)
		{
			KrokoshaTraderTrackerComponent orAddComponent = ComponentHolderProtocol.GetOrAddComponent<KrokoshaTraderTrackerComponent>((Object)(object)__instance);
			if (orAddComponent.is_registered)
			{
				if (log.verbose)
				{
					log.l("CLIENT: Sending :  TraderThreaten ");
				}
				KrokoshaScavMultiplayer.Client_SendSimpleMessageToServer((ushort)10159, (ushort)orAddComponent.si.syncId, true);
			}
			return false;
		}
		force = false;
		return true;
	}

	public static void Postfix(TraderScript __instance)
	{
		if (KrokoshaScavMultiplayer.network_system_is_running && !KrokoshaScavMultiplayer.is_client)
		{
			KrokoshaTraderTrackerComponent orAddComponent = ComponentHolderProtocol.GetOrAddComponent<KrokoshaTraderTrackerComponent>((Object)(object)__instance);
			_ = __instance.hostility;
			_ = 100f;
			orAddComponent.Server_AnnounceTraderReputationState((IReadOnlyList<knetid>)null);
		}
	}

	[ServerReceiver(10159)]
	private static void Server_TraderThreaten(knetid terroristid, ref NetDataReader reader)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out knetid result);
		if (!TraderSync.Server_TraderInteractionCheck(terroristid, result, "Threaten", out var plr, out var trader_si, out var trader_tracker))
		{
			return;
		}
		trader_tracker.focused_body = plr.body;
		if (trader_si.trader.reputation >= 30f)
		{
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(Vector2.op_Implicit(((Component)trader_si.trader).transform.position), $"S: Threaten trader {plr.playername} -> {trader_si}");
			}
			if (log.verbose)
			{
				log.l($"{plr} threatened {trader_si}");
			}
			force = true;
			trader_si.trader.Threaten();
		}
		else
		{
			log.serverdeny($"Trader_Threaten for {plr} cuz rep is too low");
		}
	}
}
