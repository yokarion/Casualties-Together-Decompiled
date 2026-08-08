using HarmonyLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(Openable), "OnUse")]
public static class Openable_OnUse_MultiplayerPatch
{
	private static void Prefix()
	{
		_ = KrokoshaScavMultiplayer.network_system_is_running;
	}

	private static void Postfix(Openable __instance)
	{
		if (KrokoshaScavMultiplayer.network_system_is_running && NetObjectRegistry.TryGetSyncInfo(((Component)__instance).gameObject, out var si) && si.building.health == 0f)
		{
			si.SetIgnoreTimeForRoundTrip();
			KrokoshaScavMultiplayer.Client_SendSimpleMessageToServer((ushort)10085, (ushort)si.syncId, true);
		}
	}

	[ServerReceiver(10085)]
	private static void Server_OpenableOpenTypeShit(knetid clientId, ref NetDataReader reader)
	{
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out knetid result);
		Openable val = null;
		SyncInfo si = null;
		if (!NetPlayer.TryGetNetPlayerAndBodyFromClientId(clientId, out var plr, out var body) || !NetObjectRegistry.TryGetSyncInfo(result, out si) || !si.go.TryGetComponent<Openable>(ref val) || !val.instantOpen)
		{
			return;
		}
		if ((ushort)clientId != 0)
		{
			if (!plr.body.conscious || !si.IsBuilding())
			{
				return;
			}
			Component ba = (Component)(object)body;
			if (!KM.dist2dsqrcheck(in ba, (Component)(object)si.building, 20f))
			{
				return;
			}
		}
		if ((ushort)clientId != 0)
		{
			val.OnUse();
		}
		if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
		{
			DebugHelp.OnNetEvent(Vector2.op_Implicit(si.go.transform.position), $"S: Openable open {plr}, si:{si}");
		}
	}
}
