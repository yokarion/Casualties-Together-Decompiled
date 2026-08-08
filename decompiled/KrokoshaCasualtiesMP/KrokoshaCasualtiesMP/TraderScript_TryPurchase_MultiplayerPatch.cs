using System;
using HarmonyLib;
using KrokoshaCasualtiesUtils;
using LiteNetLib;
using LiteNetLib.Utils;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(TraderScript), "TryPurchase")]
internal static class TraderScript_TryPurchase_MultiplayerPatch
{
	private struct traderpurchasestate
	{
		public ushort val;

		public byte itemindex;
	}

	public static Item TryPurchase_rewritten(TraderScript trader, TraderItem titem, Body buyer)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Invalid comparison between Unknown and I4
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Invalid comparison between Unknown and I4
		bool flag = Util.IsBodyLocal(buyer);
		Item result = null;
		KrokoshaTraderTrackerComponent component = ((Component)trader).GetComponent<KrokoshaTraderTrackerComponent>();
		if (((Component)trader).GetComponent<BuildingEntity>().health < 200f)
		{
			return result;
		}
		if (trader.valueGiven >= trader.ItemPrice(titem))
		{
			trader.valueGiven -= trader.ItemPrice(titem);
			if (trader.ItemPrice(titem) > 0)
			{
				if ((int)titem.preference == 0)
				{
					trader.reputation += 7f;
				}
				else if ((int)titem.preference == 1)
				{
					trader.reputation += 4f;
				}
			}
			if (component.og.freeAmount > 0)
			{
				TraderScript og = component.og;
				og.freeAmount--;
			}
			else
			{
				trader.talker.Talk(Locale.GetCharacter("traderbuy", trader.character), (Limb)null, false, false);
				buyer.happiness += 0.75f;
				buyer.skills.AddExp(2, 1f);
			}
			if (titem.info.HasTag("dressing"))
			{
				component.og.freeDressing = false;
			}
			trader.items.Remove(titem);
			GameObject val = Utils.Create(titem.id, Vector2.op_Implicit(((Component)trader).transform.position), 0f);
			val.AddComponent<BoughtItem>();
			AmmoScript val2 = default(AmmoScript);
			if (val.TryGetComponent<AmmoScript>(ref val2) && (int)val2.itemType == 1)
			{
				val2.rounds = Random.Range((int)((float)val2.maxRounds * 0.5f), val2.maxRounds);
			}
			buyer.AutoPickUpItem(val.GetComponent<Item>());
			if (flag)
			{
				PlayerCamera.main.PlayUISound((UISoundType)0, 1f);
			}
			result = val.GetComponent<Item>();
		}
		else
		{
			if (trader.totalValueGiven != TraderScript.MAX_VALUE_GIVEN)
			{
				trader.talker.Talk(Locale.GetCharacter("traderbuyfail", trader.character), (Limb)null, false, false);
			}
			else
			{
				trader.talker.Talk(Locale.GetCharacter("traderbuyfailmaxvalue", trader.character), (Limb)null, false, false);
			}
			if (flag)
			{
				PlayerCamera.main.PlayUISound((UISoundType)6, 1f);
			}
			trader.reputation -= 2f;
		}
		PlayerCamera.main.RefreshTraderInventories();
		PlayerCamera.main.UpdateTradeTexts();
		return result;
	}

	private static void Prefix(TraderScript __instance, TraderItem item, ref traderpurchasestate __state)
	{
		if (KrokoshaScavMultiplayer.network_system_is_running)
		{
			((Component)__instance).GetComponent<BuildingEntity>();
			ComponentHolderProtocol.GetOrAddComponent<KrokoshaTraderTrackerComponent>((Object)(object)__instance);
			byte itemindex = (byte)__instance.items.IndexOf(item);
			__state = new traderpurchasestate
			{
				itemindex = itemindex,
				val = (ushort)item.value
			};
		}
	}

	private static void Postfix(TraderScript __instance, TraderItem item, ref traderpurchasestate __state)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		if (KrokoshaScavMultiplayer.network_system_is_running)
		{
			((Component)__instance).GetComponent<BuildingEntity>();
			KrokoshaTraderTrackerComponent orAddComponent = ComponentHolderProtocol.GetOrAddComponent<KrokoshaTraderTrackerComponent>((Object)(object)__instance);
			NetDataWriter writer = Net.CreateWriter((Enum)NetmsgId.SERVER_Trader_TryPurchase);
			writer.Put((ushort)orAddComponent.si.syncId);
			writer.Put(__state.val);
			writer.Put(__state.itemindex);
			Net.Client_Send((DeliveryMethod)2, in writer);
		}
	}

	[ServerReceiver(10163)]
	private static void Server_Trader_TryPurchase(knetid clientId, ref NetDataReader reader)
	{
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out knetid result);
		ushort num = default(ushort);
		reader.Get(ref num);
		byte b = default(byte);
		reader.Get(ref b);
		if (!NetPlayer.TryGetNetPlayerAndBodyFromClientId(clientId, out var plr, out var body) || !NetObjectRegistry.NetIdToSyncInfoDict.TryGetValue(result, out var value) || !value.IsTrader() || !KM.dist2dsqrcheck(in body, in value, 20f))
		{
			return;
		}
		TraderScript trader = value.trader;
		KrokoshaTraderTrackerComponent orAddComponent = ComponentHolderProtocol.GetOrAddComponent<KrokoshaTraderTrackerComponent>((Object)(object)trader);
		if (plr.is_local || (b < trader.items.Count && (ushort)trader.items[b].value == num))
		{
			if (log.verbose)
			{
				Plugin.log.LogInfo((object)("SERVER: received and confirmed Trader_TryPurchase from " + plr.playername));
			}
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(Vector2.op_Implicit(((Component)trader).transform.position), "S: TryPurchase " + trader.items[b].id);
			}
			orAddComponent.focused_body = body;
			if (!plr.is_local)
			{
				NetObjectRegistry.TryGetSyncInfoOrRegister((Component)(object)TryPurchase_rewritten(trader, trader.items[b], body), out var _);
			}
			orAddComponent.Server_SendTraderInventory(ServerMain.GetListOfClientIdsExceptThisAndHost(clientId));
		}
		else
		{
			plr.Server_DoAlertSingle(Lang.MarkMsgAsLocaleKey("trader_buy_deny"), reliable: false);
			log.serverdeny($"Trader_TryPurchase for {plr}  index:{b}", Vector2.op_Implicit(((Component)trader).transform.position));
			orAddComponent.Server_SendTraderInventory(clientId);
		}
	}
}
