using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using LiteNetLib;
using LiteNetLib.Utils;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public class KrokoshaTraderTrackerComponent : KrokoshaNetworkComponentTracker<TraderScript>
{
	public struct TraderStatePacket : INetSerializeByMemcpy
	{
		public float reputation;

		public float hostility;

		public bool freeDressing;

		public bool didHug;

		public byte freeAmount;

		public float haggleAmount;

		public ushort valueGiven;

		public ushort totalValueGiven;

		public TraderStatePacket(KrokoshaTraderTrackerComponent tracker)
		{
			TraderScript og = tracker.og;
			reputation = og.reputation;
			hostility = og.hostility;
			freeDressing = og.freeDressing;
			didHug = og.didHug;
			freeAmount = (byte)og.freeAmount;
			haggleAmount = og.haggleAmount;
			valueGiven = (ushort)og.valueGiven;
			totalValueGiven = (ushort)og.totalValueGiven;
		}

		public void Apply(KrokoshaTraderTrackerComponent tracker)
		{
			TraderScript og = tracker.og;
			og.reputation = reputation;
			og.hostility = hostility;
			og.freeDressing = freeDressing;
			og.didHug = didHug;
			og.freeAmount = freeAmount;
			og.haggleAmount = haggleAmount;
			og.valueGiven = valueGiven;
			og.totalValueGiven = totalValueGiven;
		}
	}

	public bool received_their_inv;

	protected bool net_didDeathMoodDebuff;

	public Body focused_body
	{
		get
		{
			return ComponentHolderProtocol.GetOrAddComponent<BodyGetterOverrider>((Object)(object)this).body;
		}
		set
		{
			ComponentHolderProtocol.GetOrAddComponent<BodyGetterOverrider>((Object)(object)this).body = value;
		}
	}

	public bool is_registered => (Object)(object)((Component)this).GetComponent<KrokoshaScavMultiGameObjectNetworkTracker>() != (Object)null;

	public SyncInfo si => ((Component)this).GetComponent<KrokoshaScavMultiGameObjectNetworkTracker>()?.syncinfo;

	public bool asked_for_inv { get; private set; }

	public BuildingEntity build => ((Component)this).GetComponent<BuildingEntity>();

	public Body GetBody()
	{
		return focused_body;
	}

	public bool CanBeRecruited_ForRespawn()
	{
		if (build.health > 200f)
		{
			return base.og.reputation > base.og.minHugReputation;
		}
		return false;
	}

	public void Server_AnnounceTraderReputationState(in IReadOnlyList<knetid> to_who = null)
	{
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		if (KrokoshaScavMultiplayer.is_client)
		{
			return;
		}
		IReadOnlyList<knetid> readOnlyList = to_who ?? ServerMain.AllClientIdsExceptHost;
		if (!is_registered)
		{
			NetObjectRegistry.NewGO(((Component)this).gameObject);
			if (!is_registered)
			{
				DebugHelp.OnNetEvent(Vector2.op_Implicit(((Component)this).transform.position), "SERVER: bro what the fuck it doesnt wanna register wtfffff man  Trader   Server_AnnounceTraderReputationState", Color.red, 60f);
				log.error("SERVER: bro what the fuck it doesnt wanna register wtfffff man  Trader   Server_AnnounceTraderReputationState " + build?.id + " ");
				return;
			}
		}
		NetDataWriter writer = Net.CreateWriter(10156);
		writer.Put((ushort)si.syncId);
		writer.Put(new TraderStatePacket(this));
		if (log.verbose)
		{
			log.l($"SERVER: Sending Trader reputation state from {si} size: {writer.Length}");
		}
		DeliveryMethod delivery = (DeliveryMethod)0;
		IEnumerable<knetid> clientIds = readOnlyList;
		Net.Server_SendToClients(in delivery, in writer, in clientIds);
	}

	public void Server_SendTraderInventory(knetid clientId)
	{
		Server_SendTraderInventory(new List<knetid> { clientId });
	}

	public void Server_SendTraderInventory(List<knetid> clients_to_enlighten)
	{
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		if (clients_to_enlighten.Count == 0)
		{
			return;
		}
		TraderScript val = base.og;
		if (!NetObjectRegistry.IsRegistered(((Component)this).gameObject))
		{
			NetObjectRegistry.NewGO(((Component)this).gameObject);
		}
		NetDataWriter writer = Net.CreateWriter(10157);
		writer.Put((ushort)si.syncId);
		writer.Put((short)val.MoveRange.min);
		writer.Put((short)val.MoveRange.max);
		writer.Put((byte)val.items.Count);
		foreach (TraderItem item in val.items)
		{
			writer.Put(item.id, oneByteChars: true);
			writer.Put(item.bought);
			writer.Put((ushort)item.value);
			writer.Put((byte)item.preference);
		}
		if (log.verbose)
		{
			if (clients_to_enlighten.Count > 1)
			{
				log.l(string.Format("SERVER: Sending Trader inventory from {0} size: {1} for player ids: {2}", si, writer.Length, string.Join(" ", clients_to_enlighten)));
			}
			else
			{
				log.l($"SERVER: Sending Trader inventory from {si} size: {writer.Length} for {ServerMain.GetPlayerFullDebugString(clients_to_enlighten[0])}");
			}
		}
		DeliveryMethod delivery = (DeliveryMethod)0;
		IEnumerable<knetid> clientIds = clients_to_enlighten;
		Net.Server_SendToClients(in delivery, in writer, in clientIds);
		IReadOnlyList<knetid> to_who = clients_to_enlighten;
		Server_AnnounceTraderReputationState(in to_who);
	}

	protected override void TrackerAwake()
	{
		focused_body = PlayerCamera.main.body;
		ComponentHolderProtocol.GetOrAddComponent<Krokosha_OnWillRenderObject_ForceForMPComponent>((Object)(object)this).target = (MonoBehaviour)(object)base.og;
	}

	private void Start()
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		if (KrokoshaScavMultiplayer.network_system_is_running && KrokoshaScavMultiplayer.is_client)
		{
			((MonoBehaviour)this).StartCoroutine(WaitToAskServer());
		}
		if (base.og.MoveRange.min == 0f && base.og.MoveRange.max == 0f)
		{
			base.og.MoveRange = new RangeF(((Component)base.og).transform.position.x - 5f, ((Component)base.og).transform.position.x + 5f);
		}
		((MonoBehaviour)this).StartCoroutine(ScanForNearestBody());
	}

	private void LateUpdate()
	{
		if (KrokoshaScavMultiplayer.network_system_is_running)
		{
			if (base.og.hostile)
			{
				build.cantHit = false;
			}
			if (KrokoshaScavMultiplayer.is_server && is_registered && (bool)Traverse.Create((object)base.og).Field("didDeathMoodDebuff").GetValue() && !net_didDeathMoodDebuff)
			{
				net_didDeathMoodDebuff = true;
				NetObjectRegistry.Server_QueueSync(base.tracker.syncinfo);
				YOU_SHOULD_KILL_YOURSELF_NOOOW.DoMoodPenaltyInRadius(focused_body, 5f, 10f);
			}
		}
	}

	private IEnumerator ScanForNearestBody()
	{
		while (Object.op_Implicit((Object)(object)((Component)this).gameObject) && Object.op_Implicit((Object)(object)base.og))
		{
			if (KrokoshaScavMultiplayer.network_system_is_running)
			{
				(NetPlayer, float) distanceToNearestLivingPlayer = NetPlayer.GetDistanceToNearestLivingPlayer(Vector2.op_Implicit(((Component)this).transform.position));
				if ((Object)(object)distanceToNearestLivingPlayer.Item1 != (Object)null)
				{
					focused_body = distanceToNearestLivingPlayer.Item1.body;
				}
				if (Object.op_Implicit((Object)(object)base.tracker) && !base.tracker.is_super_close)
				{
					yield return (object)new WaitForSecondsRealtime(2.1021504f);
				}
			}
			yield return (object)new WaitForSecondsRealtime(0.1021504f);
		}
	}

	private IEnumerator WaitToAskServer()
	{
		yield return (object)new WaitForSecondsRealtime(0.1f);
		while (!asked_for_inv && !received_their_inv && KrokoshaScavMultiplayer.is_client && KrokoshaScavMultiplayer.network_system_is_running)
		{
			if (NetObjectRegistry.TryGetSyncInfo(((Component)this).gameObject, out var syncInfo))
			{
				KrokoshaScavMultiplayer.Client_SendSimpleMessageToServer((ushort)10164, (ushort)syncInfo.syncId, true);
				asked_for_inv = true;
				yield return (object)new WaitForSecondsRealtime(10.5126f);
			}
			yield return (object)new WaitForSecondsRealtime(1.551f);
		}
	}
}
