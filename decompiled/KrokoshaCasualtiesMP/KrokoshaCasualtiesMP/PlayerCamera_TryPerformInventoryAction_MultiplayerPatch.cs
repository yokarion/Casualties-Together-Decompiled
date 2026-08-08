using System.Collections.Generic;
using HarmonyLib;
using KrokoshaCasualtiesUtils;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(PlayerCamera), "TryPerformInventoryAction")]
public static class PlayerCamera_TryPerformInventoryAction_MultiplayerPatch
{
	public static bool is_inside_TryPerformInventoryAction;

	private static bool Prefix(PlayerCamera __instance, ref bool __result, RaycastResult hit, List<RaycastResult> uiCasts)
	{
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		if (!KrokoshaScavMultiplayer.network_system_is_running)
		{
			return true;
		}
		if (Util.IsWorldGenerated())
		{
			InvButton val = default(InvButton);
			if (!((RaycastResult)(ref hit)).gameObject.TryGetComponent<InvButton>(ref val) || !val.Overlaps(uiCasts))
			{
				is_inside_TryPerformInventoryAction = false;
				return false;
			}
			if ((Object)(object)val.GetItem() == (Object)(object)__instance.dragItem)
			{
				__result = true;
				is_inside_TryPerformInventoryAction = false;
				return false;
			}
			Item item = val.GetItem();
			if (Object.op_Implicit((Object)(object)item) && Object.op_Implicit((Object)(object)item.battery) && ItemSync.TryGetSyncInfo(item, out var si) && ItemSync.TryGetSyncInfo(__instance.dragItem, out var si2))
			{
				if (__instance.dragItem.Stats.HasTag("tool") && item.battery.hasBattery)
				{
					NetDataWriter writer = Net.CreateWriter(10114);
					writer.Put(false);
					writer.Put((ushort)si.syncId);
					writer.Put((ushort)si2.syncId);
					Net.Client_Send((DeliveryMethod)0, in writer);
					item.battery.UnloadBattery(false);
					__result = true;
					return false;
				}
				if (__instance.dragItem.Stats.HasTag("battery") && !item.battery.hasBattery && !(((object)__instance.dragItem.Stats).GetType() != typeof(BatteryInfo)))
				{
					NetDataWriter writer2 = Net.CreateWriter(10114);
					writer2.Put(true);
					writer2.Put((ushort)si.syncId);
					writer2.Put((ushort)si2.syncId);
					Net.Client_Send((DeliveryMethod)0, in writer2);
					item.battery.LoadBattery(__instance.dragItem);
					__result = true;
					return false;
				}
			}
			is_inside_TryPerformInventoryAction = true;
		}
		return true;
	}

	private static void Postfix()
	{
		is_inside_TryPerformInventoryAction = false;
	}

	[ServerReceiver(10114)]
	private static void Server_ItemBatteryManipulation(knetid clientId, ref NetDataReader reader)
	{
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		bool flag = default(bool);
		reader.Get(ref flag);
		reader.Get(out knetid result);
		reader.Get(out knetid result2);
		if (NetPlayer.TryGetNetPlayerAndNetBodyFromClientId(clientId, out var plr, out var pb) && ItemSync.TryGetItemSyncInfo(result, out var si) && ItemSync.CheckIfBodyReachThisItem(si, pb.body) && ItemSync.TryGetItemSyncInfo(result2, out var si2) && ItemSync.CheckIfBodyReachThisItem(si2, pb.body))
		{
			if (log.verbose)
			{
				Plugin.log.LogInfo((object)$"ItemBatteryManipulation check success {plr} -> {si} + {si2}");
			}
			Item item = si2.item;
			Item item2 = si.item;
			if (Object.op_Implicit((Object)(object)item2.battery))
			{
				if (!flag && item.Stats.HasTag("tool"))
				{
					Sound.Play("batteryinsert", Vector2.op_Implicit(((Component)item2).transform.position), false, true, (Transform)null, 1f, 1f, false, false);
					Object obj = Object.Instantiate(Resources.Load(item2.battery.batteryType), ((Component)item2).transform.position, Quaternion.identity);
					Item component = ((GameObject)((obj is GameObject) ? obj : null)).GetComponent<Item>();
					component.condition = item2.condition;
					pb.body.AutoPickUpItem(component);
					item2.battery.UnloadBattery(true);
				}
				else if (flag && item.Stats.HasTag("battery"))
				{
					item2.battery.LoadBattery(item);
				}
				NetObjectRegistry.Server_QueueSync(si);
			}
		}
		else
		{
			log.serverdeny($"ItemBatteryManipulation denied for: {ServerMain.GetPlayerFullDebugString(clientId)}  item1:{result}  item2:{result2}");
		}
	}
}
