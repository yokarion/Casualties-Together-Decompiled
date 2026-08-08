using System.Collections.Generic;
using HarmonyLib;
using KrokoshaCasualtiesUtils;
using LiteNetLib.Utils;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(TraderScript), "TryHug")]
public static class TraderScript_TryHug_MultiplayerPatch
{
	private static bool force;

	public static bool Prefix(TraderScript __instance, ref float __state)
	{
		__state = __instance.reputation;
		if (KrokoshaScavMultiplayer.network_system_is_running && !force && (Object)(object)PlayerCamera.main.currentTrader == (Object)(object)__instance)
		{
			KrokoshaTraderTrackerComponent orAddComponent = ComponentHolderProtocol.GetOrAddComponent<KrokoshaTraderTrackerComponent>((Object)(object)__instance);
			if (orAddComponent.is_registered)
			{
				if (log.verbose)
				{
					log.l($"CLIENT: Requesting TraderTryHug {orAddComponent.si} ");
				}
				KrokoshaScavMultiplayer.Client_SendSimpleMessageToServer((ushort)10160, (ushort)orAddComponent.si.syncId, true);
			}
			return false;
		}
		force = false;
		return true;
	}

	public static void Postfix(TraderScript __instance, ref float __state)
	{
		if (KrokoshaScavMultiplayer.network_system_is_running && !KrokoshaScavMultiplayer.is_client)
		{
			KrokoshaTraderTrackerComponent orAddComponent = ComponentHolderProtocol.GetOrAddComponent<KrokoshaTraderTrackerComponent>((Object)(object)__instance);
			if (__instance.hostility != 100f && __state != __instance.reputation && !(__instance.reputation > __state) && !Util.IsBodyLocal(orAddComponent.focused_body) && orAddComponent.focused_body.TryGetNetPlayer(out var plr))
			{
				plr.Server_RemindPlayersCurrentState(keep_velocity: true);
			}
			orAddComponent.Server_AnnounceTraderReputationState((IReadOnlyList<knetid>)null);
		}
	}

	[ServerReceiver(10160)]
	private static void Server_TraderTryHug(knetid terroristid, ref NetDataReader reader)
	{
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out knetid result);
		if (!TraderSync.Server_TraderInteractionCheck(terroristid, result, "Hug", out var plr, out var trader_si, out var trader_tracker))
		{
			return;
		}
		trader_tracker.focused_body = plr.body;
		if (!trader_tracker.og.didHug)
		{
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(Vector2.op_Implicit(((Component)trader_si.trader).transform.position), $"S: Huged trader {plr.playername} -> {trader_si}");
			}
			if (log.verbose)
			{
				log.l($"{plr} hugged {trader_si}");
			}
			force = true;
			trader_si.trader.TryHug();
		}
		else
		{
			log.serverdeny($"Trader_TryHug for {plr} cuz {trader_si} is already hugged", Vector2.op_Implicit(trader_si.go.transform.position));
		}
	}
}
