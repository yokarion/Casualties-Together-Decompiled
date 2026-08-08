using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using KrokoshaCasualtiesUtils;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(BatteryRecharger), "OnUse")]
public static class BatteryRecharger_OnUse_MultiplayerPatch
{
	public static bool Copypasted_OnUse(BatteryRecharger charger, Item item, Body body = null)
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		bool flag = (Object)(object)charger.items[0] != (Object)null && (Object)(object)charger.items[1] != (Object)null;
		if (Object.op_Implicit((Object)(object)item) && item.Stats is BatteryInfo && !flag && !charger.items.Contains(item))
		{
			ItemSync.SafeUnloadItem(ItemSync.ItemGetContainerInfo(item));
			int num = (Object.op_Implicit((Object)(object)charger.items[0]) ? 1 : 0);
			((Component)item).transform.rotation = Quaternion.identity;
			((Component)item).transform.position = charger.targets[num].position;
			item.rb.rotation = 0f;
			item.rb.position = Vector2.op_Implicit(((Component)item).transform.position);
			((Renderer)((Component)item).GetComponent<SpriteRenderer>()).sortingOrder = ((Renderer)((Component)charger).GetComponent<SpriteRenderer>()).sortingOrder + 1;
			charger.joints[num] = ((Component)item).gameObject.AddComponent<FixedJoint2D>();
			charger.joints[num].frequency = 20f;
			((AnchoredJoint2D)charger.joints[num]).autoConfigureConnectedAnchor = true;
			((AnchoredJoint2D)charger.joints[num]).connectedAnchor = Vector2.op_Implicit(charger.targets[num].position);
			charger.items[num] = item;
			Util.PlayWorldSoundOnScreenIfInRange("batteryinsert", Vector2.op_Implicit(((Component)charger).transform.position), 1f, 1f, 64f);
			if (charger.firstTime)
			{
				((Component)charger).GetComponent<Talker>().Talk(Locale.GetOther("rechargingstationcheer"), (Limb)null, false, false);
				if (!KrokoshaScavMultiplayer.is_client)
				{
					Item component = Utils.Create("mp3player", Vector2.op_Implicit(((Component)charger).transform.position), 0f).GetComponent<Item>();
					if (body != null)
					{
						body.AutoPickUpItem(component);
					}
					component.battery.UnloadBattery(true);
				}
				charger.firstTime = false;
			}
			if (KrokoshaScavMultiplayer.network_system_is_running && NetObjectRegistry.TryGetSyncInfo((Component)(object)item, out var si))
			{
				SyncInfo si2;
				if (KrokoshaScavMultiplayer.is_client)
				{
					si.SetIgnoreTimeForRoundTrip(0.5);
				}
				else if (NetObjectRegistry.TryGetSyncInfo((Component)(object)charger, out si2))
				{
					NetDataWriter writer = Net.CreateWriter(10089);
					writer.Put((ushort)si.syncId);
					writer.Put((ushort)si2.syncId);
					Net.Server_SendToClients((DeliveryMethod)0, in writer, (IEnumerable<knetid>)ServerMain.AllClientIdsExceptHost);
				}
			}
			return true;
		}
		return false;
	}

	private static bool Prefix(BatteryRecharger __instance)
	{
		if (!KrokoshaScavMultiplayer.network_system_is_running)
		{
			return true;
		}
		Item item = PlayerCamera.main.body.GetItem(PlayerCamera.main.body.handSlot);
		if (Copypasted_OnUse(__instance, item, PlayerCamera.main.body) && KrokoshaScavMultiplayer.is_client)
		{
			if (NetObjectRegistry.TryGetSyncInfoOrRegister((Component)(object)item, out var si) && NetObjectRegistry.TryGetSyncInfoOrRegister((Component)(object)__instance, out var si2))
			{
				KrokoshaScavMultiplayer.Client_SendSimpleMessageToServer((ushort)10092, (ushort)si.syncId, (ushort)si2.syncId, true);
			}
			else
			{
				NetObjectRegistry.AlertObjectNotRegistered(popup: true);
			}
		}
		return false;
	}

	[ServerReceiver(10092)]
	private static void ServerReceiver_MinigameSchrapnelLocations(knetid clientId, ref NetDataReader reader)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out knetid result);
		reader.Get(out knetid result2);
		if (NetPlayer.TryGetNetPlayerAndBodyFromClientId(clientId, out var plr, out var body) && body.conscious && NetObjectRegistry.TryGetSyncInfo(result, out var si) && NetObjectRegistry.TryGetSyncInfo(result2, out var si2))
		{
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(si2.position, $"BatteryRecharger.OnUse {plr} {si} {si2} ");
			}
			Copypasted_OnUse(si2.go.GetComponent<BatteryRecharger>(), si.item);
		}
	}

	[ClientReceiver(10089, true)]
	private static void ClientReceiver_MinigameSchrapnelLocations(knetid _, ref NetDataReader reader)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out knetid result);
		reader.Get(out knetid result2);
		if (NetObjectRegistry.TryGetSyncInfo(result, out var si) && NetObjectRegistry.TryGetSyncInfo(result2, out var si2))
		{
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(si2.position, $"BatteryRechargerOnUseRelay {si} {si2} ");
			}
			Copypasted_OnUse(si2.go.GetComponent<BatteryRecharger>(), si.item);
		}
	}
}
