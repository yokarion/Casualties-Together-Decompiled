using System;
using System.Collections.Generic;
using System.Linq;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine.SceneManagement;

namespace KrokoshaCasualtiesMP;

public class CoolSyncManager : KrokoshaScavSingleton
{
	public static CoolSyncManager inst;

	private static Dictionary<byte, BaseCoolSyncSubSystem> all_systems = new Dictionary<byte, BaseCoolSyncSubSystem>();

	public static IEnumerable<BaseCoolSyncSubSystem> AllSystems = all_systems.Values;

	public int cur_system_to_send;

	public void OnSceneUnLoaded(Scene a)
	{
		ResetResettableSystems();
	}

	public void OnSceneLoaded(Scene a, LoadSceneMode b)
	{
		ResetResettableSystems();
	}

	internal void ResetResettableSystems()
	{
		foreach (KeyValuePair<byte, BaseCoolSyncSubSystem> all_system in all_systems)
		{
			if (all_system.Value.CleanupAndUnregisterOnLevelChange)
			{
				all_system.Value.ClearAndResetEverything();
			}
		}
	}

	private void Awake()
	{
		inst = this;
	}

	private void Start()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
		SceneManager.sceneUnloaded += OnSceneUnLoaded;
	}

	public void OnTransportStart()
	{
		OnTransportEnd();
		all_systems[0] = new RuleSyncer(0);
		all_systems[1] = new NewCoolerObjectPacketWriteReadSystem(1);
		all_systems[2] = new CharSync(2);
		all_systems[3] = new PlrSync(3);
		all_systems[4] = new ServerWorldStateSyncer(4);
	}

	public void OnTransportEnd()
	{
		foreach (KeyValuePair<byte, BaseCoolSyncSubSystem> all_system in all_systems)
		{
			all_system.Value.Cleanup();
		}
		all_systems.Clear();
	}

	private void Update()
	{
		foreach (KeyValuePair<byte, BaseCoolSyncSubSystem> all_system in all_systems)
		{
			all_system.Value.Update();
		}
	}

	private void FixedUpdate()
	{
		if (!KrokoshaScavMultiplayer.IsNetworkActiveAndIsServer() || all_systems.Count == 0)
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < all_systems.Count; i++)
		{
			if (cur_system_to_send >= all_systems.Count)
			{
				cur_system_to_send = 0;
			}
			try
			{
				flag = all_systems.Values.ElementAt(cur_system_to_send).Server_Update();
			}
			catch (Exception ex)
			{
				log.error(ex.ToString());
			}
			cur_system_to_send++;
			if (flag)
			{
				break;
			}
		}
	}

	[ServerReceiver(10176)]
	private static void ServerReceiver_CoolSyncObjectDataRequest(knetid clientId, ref NetDataReader reader)
	{
		byte key = reader.GetByte();
		if (NetPlayer.TryGetPlayerFromClientId(clientId, out var plr) && all_systems.TryGetValue(key, out var value) && value is CoolSyncSubSystemForObjects coolSyncSubSystemForObjects)
		{
			coolSyncSubSystemForObjects.Server_ReceiveDataRequest(plr, reader);
		}
	}

	[ServerReceiver(10175)]
	private static void ServerReceiver_CoolSyncAckReceiver(knetid clientId, ref NetDataReader reader)
	{
		byte key = reader.GetByte();
		reader.PeekUShort();
		if (NetPlayer.TryGetPlayerFromClientId(clientId, out var plr) && all_systems.TryGetValue(key, out var value))
		{
			value.Server_ReceiveAck(plr, reader);
		}
	}

	[ClientReceiver(10174, true)]
	private static void ClientReceiver_CoolSyncReceiver(knetid _, ref NetDataReader reader)
	{
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		byte b = reader.GetByte();
		ushort num = reader.PeekUShort();
		if (all_systems.TryGetValue(b, out var value))
		{
			value.ack = true;
			value.Client_Receive(reader);
			if (!value.ack)
			{
				value.ack = true;
			}
		}
		else if (log.verbose)
		{
			log.error($"Client_CoolSyncReceiver: Unknown Subsystem: {b}  deltaid: {num}");
		}
		NetDataWriter writer = Net.CreateWriter((Enum)NetmsgId.Server_CoolSyncAckReceiver);
		writer.Put(b);
		writer.Put(num);
		Net.Client_Send((DeliveryMethod)4, in writer);
	}
}
