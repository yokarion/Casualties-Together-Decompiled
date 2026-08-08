using LiteNetLib.Utils;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public class TraderSync : KrokoshaScavSingleton
{
	public static bool Server_TraderInteractionCheck(knetid plrid, knetid traderid, in string interaction_name_for_log, out NetPlayer plr, out SyncInfo trader_si, out KrokoshaTraderTrackerComponent trader_tracker)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		if (NetPlayer.TryGetNetPlayerAndBodyFromClientId(plrid, out plr, out var body) && body.conscious && NetObjectRegistry.TryGetSyncInfo(traderid, out trader_si) && trader_si.IsTrader() && KM.dist2dsqrcheck(in body, in trader_si, 20f))
		{
			trader_tracker = ComponentHolderProtocol.GetOrAddComponent<KrokoshaTraderTrackerComponent>((Object)(object)trader_si.trader);
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(Vector2.op_Implicit(((Component)trader_tracker).transform.position), $"S: trader {interaction_name_for_log} - {plr.playername} -> {trader_si}");
			}
			trader_tracker.focused_body = body;
			return true;
		}
		trader_si = null;
		trader_tracker = null;
		log.serverdeny($"trader {interaction_name_for_log} for {plr}");
		return false;
	}

	[ServerReceiver(10164)]
	private static void ServerReceiver_RequestTraderInventory(knetid clientId, ref NetDataReader reader)
	{
		reader.Get(out knetid result);
		if (NetObjectRegistry.NetIdToSyncInfoDict.TryGetValue(result, out var value) && value.IsTrader())
		{
			ComponentHolderProtocol.GetOrAddComponent<KrokoshaTraderTrackerComponent>((Object)(object)value.go).Server_SendTraderInventory(clientId);
		}
	}

	[ClientReceiver(10156, true)]
	private static void ClientReceiver__TraderSync_ReputationState(knetid _, ref NetDataReader reader)
	{
		reader.Get(out knetid result);
		if (NetObjectRegistry.TryGetSyncInfo(result, out var si))
		{
			TraderScript trader = si.trader;
			reader.Get(out KrokoshaTraderTrackerComponent.TraderStatePacket result2);
			result2.Apply(si.go.GetComponent<KrokoshaTraderTrackerComponent>());
			PlayerCamera.main.RefreshTraderInventories();
			PlayerCamera.main.UpdateTradeTexts();
		}
	}

	[ClientReceiver(10157, true)]
	private static void ClientReceiver__TraderSync_Inventory(knetid _, ref NetDataReader reader)
	{
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Expected O, but got Unknown
		reader.Get(out knetid result);
		if (NetObjectRegistry.TryGetSyncInfo(result, out var si))
		{
			short num = default(short);
			reader.Get(ref num);
			short num2 = default(short);
			reader.Get(ref num2);
			si.trader.MoveRange.min = num;
			si.trader.MoveRange.max = num2;
			TraderScript trader = si.trader;
			trader.items.Clear();
			byte b = default(byte);
			reader.Get(ref b);
			bool bought = default(bool);
			ushort value = default(ushort);
			byte b2 = default(byte);
			for (int i = 0; i < b; i++)
			{
				reader.Get(out var result2, oneByteChars: true);
				reader.Get(ref bought);
				reader.Get(ref value);
				reader.Get(ref b2);
				trader.items.Add(new TraderItem
				{
					id = result2,
					bought = bought,
					value = value,
					preference = (TraderItemPreference)b2
				});
			}
			PlayerCamera.main.RefreshTraderInventories();
			PlayerCamera.main.UpdateTradeTexts();
			ComponentHolderProtocol.GetOrAddComponent<KrokoshaTraderTrackerComponent>((Object)(object)trader).received_their_inv = true;
		}
	}
}
