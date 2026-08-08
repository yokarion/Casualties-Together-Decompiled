using System.Collections.Generic;
using HarmonyLib;
using KrokoshaCasualtiesUtils;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(WoundView), "TakeANap")]
public static class WoundView_TakeANap_MultiplayerPatch
{
	private static void Prefix(WoundView __instance)
	{
		if (PlayerCamera.main.body.canTakeNap)
		{
			NetPlayer lOCAL_PLAYER = NetPlayer.LOCAL_PLAYER;
			if ((Object)(object)lOCAL_PLAYER != (Object)null && lOCAL_PLAYER.TryGetNetBody(out var pb))
			{
				pb.SetNetHealthSyncIgnoreTime(2f);
				KrokoshaScavMultiplayer.Client_SendSimpleMessageToServer((ushort)10134);
			}
		}
	}

	[ServerReceiver(10134)]
	private static void Server_PlayerTakeANap(knetid clientId, ref NetDataReader reader)
	{
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		if (NetPlayer.TryGetNetPlayerAndBodyFromClientId(clientId, out var plr, out var body) && body.conscious)
		{
			if (!Util.IsBodyLocal(body))
			{
				PlayerCamera_ToggleWoundView_MultiplayerPatch.IgnoreNext = true;
			}
			body.TakeANap();
			PlayerCamera_ToggleWoundView_MultiplayerPatch.IgnoreNext = false;
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(Vector2.op_Implicit(((Component)body).transform.position), $"S: PlayerTakeANap {plr} ");
			}
			NetDataWriter writer = Net.CreateWriter(10124);
			writer.Put((ushort)plr.playerbody.netId);
			Net.Server_SendToClients((DeliveryMethod)0, in writer, (IEnumerable<knetid>)ServerMain.GetListOfClientIdsExceptThisAndHost(clientId));
		}
	}

	[ClientReceiver(10124, true)]
	private static void Client_PlayerTakeANap_Relay(knetid _, ref NetDataReader reader)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out knetid result);
		if (!NetBody.TryGetNetBodyFromId(result, out var nb))
		{
			return;
		}
		if (!nb.body.sleeping)
		{
			if (!Util.IsBodyLocal(nb.body))
			{
				PlayerCamera_ToggleWoundView_MultiplayerPatch.IgnoreNext = true;
			}
			nb.body.TakeANap();
			PlayerCamera_ToggleWoundView_MultiplayerPatch.IgnoreNext = false;
		}
		if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
		{
			DebugHelp.OnNetEvent(nb.position, $"C: PlayerTakeANap_Relay {nb} ");
		}
	}

	[ServerReceiver(10135)]
	private static void Server_IWannaWakeUp(knetid clientId, ref NetDataReader reader)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (NetPlayer.TryGetNetPlayerAndBodyFromClientId(clientId, out var plr, out var body) && body.alive)
		{
			body.WakeUp();
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(Vector2.op_Implicit(((Component)body).transform.position), $"S: Wake up {plr} ");
			}
		}
	}
}
