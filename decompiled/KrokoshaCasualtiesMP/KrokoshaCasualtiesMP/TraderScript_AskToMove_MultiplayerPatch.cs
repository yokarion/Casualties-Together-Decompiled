using HarmonyLib;
using LiteNetLib.Utils;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(TraderScript), "AskToMove")]
public static class TraderScript_AskToMove_MultiplayerPatch
{
	private static bool force;

	public static bool Prefix(TraderScript __instance)
	{
		if (!KrokoshaScavMultiplayer.network_system_is_running)
		{
			return true;
		}
		KrokoshaTraderTrackerComponent orAddComponent = ComponentHolderProtocol.GetOrAddComponent<KrokoshaTraderTrackerComponent>((Object)(object)__instance);
		if (!force && (Object)(object)PlayerCamera.main.currentTrader == (Object)(object)__instance)
		{
			if (orAddComponent.is_registered)
			{
				if (log.verbose)
				{
					log.l("CLIENT: Sending  TraderAskToMove ");
				}
				KrokoshaScavMultiplayer.Client_SendSimpleMessageToServer((ushort)10158, (ushort)orAddComponent.si.syncId, true);
				__instance.didMove = true;
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
			NetObjectRegistry.Server_QueueSync(ComponentHolderProtocol.GetOrAddComponent<KrokoshaTraderTrackerComponent>((Object)(object)__instance).si);
		}
	}

	[ServerReceiver(10158)]
	private static void Server_TraderAskToMove(knetid terroristid, ref NetDataReader reader)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out knetid result);
		if (TraderSync.Server_TraderInteractionCheck(terroristid, result, "AskToMove", out var plr, out var trader_si, out var trader_tracker))
		{
			trader_tracker.focused_body = plr.body;
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(Vector2.op_Implicit(((Component)trader_si.trader).transform.position), $"S: AskToMove trader {plr.playername} -> {trader_si}");
			}
			if (log.verbose)
			{
				log.l($"{ServerMain.GetPlayerFullDebugString(terroristid)} AskToMoveed {trader_si}");
			}
			force = true;
			trader_si.trader.AskToMove();
		}
	}
}
