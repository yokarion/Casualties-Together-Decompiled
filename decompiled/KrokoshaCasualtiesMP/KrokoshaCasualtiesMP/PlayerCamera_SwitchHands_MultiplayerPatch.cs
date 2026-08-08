using System.Collections.Generic;
using HarmonyLib;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(PlayerCamera), "SwitchHands")]
public static class PlayerCamera_SwitchHands_MultiplayerPatch
{
	public static void Postfix()
	{
		if (KrokoshaScavMultiplayer.network_system_is_running)
		{
			KrokoshaScavMultiplayer.Client_SendSimpleMessageToServer((ushort)10029, (ushort)PlayerCamera.main.body.handSlot);
		}
	}

	[ServerReceiver(10029)]
	private static void Server_PlayerCamera_SwitchHands(knetid clientId, ref NetDataReader reader)
	{
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		ushort num = default(ushort);
		reader.Get(ref num);
		NetPlayer plr;
		if (num > 1)
		{
			Plugin.log.LogError((object)$"SUS: bogus handslot PlayerCamera_SwitchHands {num} (plrid: {clientId}, name:{ServerMain.GetPlayerFullDebugString(clientId)}) ");
		}
		else if (NetPlayer.TryGetPlayerFromClientId(clientId, out plr) && plr.IsAlive())
		{
			plr.body.handSlot = num;
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(Vector2.op_Implicit(((Component)plr.body).transform.position), $"S: PlayerCamera_SwitchHands {num}");
			}
			NetDataWriter writer = Net.CreateWriter(10026);
			writer.Put((ushort)clientId);
			writer.Put((byte)num);
			Net.Server_SendToClients((DeliveryMethod)0, in writer, (IEnumerable<knetid>)ServerMain.AllClientIdsExceptHost);
		}
	}

	[ClientReceiver(10026, true)]
	private static void Client_PlayerCamera_SwitchHands(knetid _, ref NetDataReader reader)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out knetid result);
		byte b = default(byte);
		reader.Get(ref b);
		if (NetPlayer.TryGetPlayerFromClientId(result, out var plr) && Object.op_Implicit((Object)(object)plr.body) && !plr.is_local)
		{
			plr.body.handSlot = b;
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(Vector2.op_Implicit(((Component)plr.body).transform.position), $"C: PlayerCamera_SwitchHandsRelay {b}");
			}
		}
	}
}
