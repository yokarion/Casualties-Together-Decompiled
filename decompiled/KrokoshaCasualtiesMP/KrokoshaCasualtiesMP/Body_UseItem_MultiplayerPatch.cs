using System.Collections.Generic;
using HarmonyLib;
using LiteNetLib;
using LiteNetLib.Utils;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(Body), "UseItem")]
public static class Body_UseItem_MultiplayerPatch
{
	public static Item current_used_item;

	public static void Prefix(Body __instance, ref SyncInfo __state, Item item)
	{
		current_used_item = item;
		__state = null;
		NetBody netBody = default(NetBody);
		if (!KrokoshaScavMultiplayer.network_system_is_running || !KrokoshaScavMultiplayer.IsInGameAndWorldGenerated() || !Object.op_Implicit((Object)(object)item) || !((Component)__instance).TryGetComponent<NetBody>(ref netBody) || !netBody.is_local)
		{
			return;
		}
		if (ItemSync.TryGetSyncInfo(item, out var si))
		{
			if (!item.Stats.usable)
			{
				return;
			}
			if (CombatStuff.ItemIsUsedForAttacking(item))
			{
				if (si.IsGun())
				{
					KrokoshaGunScriptTrackerComponent orAddComponent = ComponentHolderProtocol.GetOrAddComponent<KrokoshaGunScriptTrackerComponent>((Object)(object)item);
					orAddComponent.body = __instance;
					orAddComponent.pbody = netBody;
				}
			}
			else
			{
				__state = si;
			}
		}
		else
		{
			NetObjectRegistry.Client_DeleteUnregisteredObject(((Component)item).gameObject);
		}
	}

	public static void Postfix(Body __instance, ref SyncInfo __state, Item item)
	{
		current_used_item = null;
		if (__state != null)
		{
			__state.SetIgnoreTimeForRoundTrip();
			KrokoshaScavMultiplayer.Client_SendSimpleMessageToServer((ushort)10105, (ushort)__state.syncId, (ushort)((Component)__instance).GetComponent<NetBody>().netId, true);
		}
	}

	[ServerReceiver(10105)]
	private static void Server_RequestUseItem(knetid clientId, ref NetDataReader reader)
	{
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out knetid result);
		reader.Get(out knetid result2);
		if (!NetPlayer.TryGetNetPlayerAndBodyFromClientId(clientId, out var plr, out var body) || !body.conscious || !NetBody.TryGetNetBodyFromId(result2, out var nb) || !NetObjectRegistry.TryGetSyncInfo(result, out var si) || !si.IsItem())
		{
			return;
		}
		NetBody playerbody = plr.playerbody;
		if ((Object)(object)playerbody == (Object)(object)nb || !MedicalSync.IsRefusingHelp(nb.body))
		{
			Component ba = (Component)(object)body;
			if (KM.dist2dsqrcheck(in ba, (Component)(object)nb.body, SharedMain.max_player_interaction_distance * 1.6f) && (si.item.Stats.usable || si.item.Stats.wearable) && ItemSync.CheckIfBodyReachThisItem(si, body) && !CombatStuff.ItemIsUsedForAttacking(si.item))
			{
				if (!plr.is_local)
				{
					ItemSync.BetterUseItem(nb.body, si.item);
				}
				if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
				{
					DebugHelp.OnNetEvent(Vector2.Lerp(playerbody.position, nb.position, 0.5f), $"S: RequestUseItem {si}");
				}
				NetDataWriter writer = Net.CreateWriter(10100);
				writer.Put((ushort)clientId);
				writer.Put((ushort)result);
				writer.Put((ushort)result2);
				Net.Server_SendToClients((DeliveryMethod)0, in writer, (IEnumerable<knetid>)ServerMain.GetListOfClientIdsExceptThisAndHost(clientId));
				return;
			}
		}
		log.serverdeny($"RequestUseItem {plr} -> {nb} - {si}", Vector2.Lerp(Vector2.op_Implicit(((Component)plr.body).transform.position), nb.position, 0.5f));
	}

	[ClientReceiver(10100, true)]
	private static void Client_RequestUseItemRelay(knetid _, ref NetDataReader reader)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out knetid _);
		reader.Get(out knetid result2);
		reader.Get(out knetid result3);
		if (NetBody.TryGetNetBodyFromId(result3, out var nb) && NetObjectRegistry.TryGetSyncInfo(result2, out var si))
		{
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS && (Object)(object)NetPlayer.LOCAL_PLAYER != (Object)null)
			{
				DebugHelp.OnNetEvent(Vector2.Lerp(Vector2.op_Implicit(((Component)NetPlayer.LOCAL_PLAYER.body).transform.position), nb.position, 0.8f), $"C: RequestUseItemRelay {si}");
			}
			ItemSync.BetterUseItem(nb.body, si.item);
		}
	}
}
