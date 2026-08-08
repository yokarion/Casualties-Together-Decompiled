using System;
using System.Collections.Generic;
using HarmonyLib;
using KrokoshaCasualtiesUtils;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(Body), "Jump")]
internal static class Body_Jump_MultiplayerPatch
{
	private struct dffdsfdsfdssfs
	{
		public bool aaaaaaaaaaaaaaaa;

		public bool bbbbbbbbbbbbbbbb;
	}

	private static void Prefix(Body __instance, ref dffdsfdsfdssfs __state)
	{
		__state.aaaaaaaaaaaaaaaa = PlayerCamera.main.wantsJumpInput;
		NetBody netBody = default(NetBody);
		if (((Component)__instance).TryGetComponent<NetBody>(ref netBody))
		{
			netBody.StopPiggyback();
		}
	}

	private static void Postfix(Body __instance, ref dffdsfdsfdssfs __state)
	{
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		if (__instance.IsBodyLocal())
		{
			if (!PlayerCamera.main.wantsJumpInput && KrokoshaScavMultiplayer.network_system_is_running && Net.is_client)
			{
				KrokoshaScavMultiplayer.Client_SendSimpleMessageToServer((ushort)10037, reliable: false);
			}
		}
		else
		{
			PlayerCamera.main.wantsJumpInput = __state.aaaaaaaaaaaaaaaa;
		}
		NetBody netBody = default(NetBody);
		if (KrokoshaScavMultiplayer.is_server && ((Component)__instance).TryGetComponent<NetBody>(ref netBody))
		{
			NetDataWriter writer = Net.CreateWriter((Enum)NetmsgId.CLIENT_PlayerJumpRelay);
			writer.Put((ushort)netBody.netId);
			Net.Server_SendToClients((DeliveryMethod)4, in writer, (IEnumerable<knetid>)ServerMain.GetListOfClientIdsExceptThisAndHost(netBody));
		}
	}
}
