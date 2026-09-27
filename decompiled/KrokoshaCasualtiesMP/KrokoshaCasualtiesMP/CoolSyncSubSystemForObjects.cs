using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LiteNetLib;
using LiteNetLib.Utils;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public class CoolSyncSubSystemForObjects : BaseCoolSyncSubSystem
{
	protected class Client_Object
	{
		public CoolSyncSubSystemForObjects sys;

		public knetid netId;

		public ushort cur_packet_deltaid;

		public object real_obj;

		public IDeltaPacketBase cur_packet;

		public BitArray bitset;

		public double last_receive_time;

		public override string ToString()
		{
			if (this == null)
			{
				return "C_OBJ(NULL)";
			}
			if (real_obj != null)
			{
				return $"C_OBJ({sys}, ID:{netId}, {real_obj})";
			}
			return $"C_OBJ({sys}, {netId}, NULL)";
		}
	}

	protected class Server_Object
	{
		public knetid netId;

		public bool cur_frame_packed;

		public IDeltaPacketBase cur_packet;

		public HashSet<knetid> players_its_been_sent_to = new HashSet<knetid>();

		public HashSet<knetid> players_requested_info = new HashSet<knetid>();

		public object real_obj;

		public bool IsDeleted()
		{
			return real_obj == null;
		}

		public override string ToString()
		{
			if (real_obj != null)
			{
				return $"S_OBJ({netId}, {real_obj})";
			}
			return $"S_OBJ({netId}, NULL, kplr:{players_its_been_sent_to.Count})";
		}
	}

	public class Server_PerPlrState
	{
		public class Server_Snapshot
		{
			public ushort deltaid;

			public bool is_deleted;

			public Dictionary<knetid, IDeltaPacketBase> objects_it_contains = new Dictionary<knetid, IDeltaPacketBase>();
		}

		public class Server_PerPlrObjectState
		{
			public ushort last_known_snapshot_id;

			public IDeltaPacketBase last_known_snapshot;
		}

		public CoolSyncSubSystemForObjects system;

		public knetid plrId;

		public knetid last_sent_object_index;

		public bool clear_queued;

		public Queue<knetid> forcedelete_queue = new Queue<knetid>();

		public Queue<knetid> forcesync_queue = new Queue<knetid>();

		public ushort cur_delta_roll;

		public Dictionary<ushort, Server_Snapshot> snapshots = new Dictionary<ushort, Server_Snapshot>();

		public Queue<ushort> snapshot_queue = new Queue<ushort>();

		public Dictionary<knetid, Server_PerPlrObjectState> objstates = new Dictionary<knetid, Server_PerPlrObjectState>();

		public override string ToString()
		{
			return $"PLRSTATE({ServerMain.GetPlayerFullDebugString(plrId)}, last_delta:{last_sent_object_index}, cur_delta:{cur_delta_roll}, snapshots:{snapshots.Count})";
		}

		public Server_PerPlrObjectState GetObjState(knetid objId)
		{
			if (!objstates.TryGetValue(objId, out var value))
			{
				value = new Server_PerPlrObjectState();
				value.last_known_snapshot = Serialization.CloneViaSerialization<IDeltaPacketBase>(system.base_packet, false);
				value.last_known_snapshot_id = (ushort)(cur_delta_roll - 1);
				objstates[objId] = value;
			}
			return value;
		}
	}

	protected bool server_has_queued_forcesync;

	protected int datarequestqueue_max = 10;

	protected Queue<knetid> client_datarequestqueue = new Queue<knetid>();

	protected Dictionary<knetid, Client_Object> client_objects = new Dictionary<knetid, Client_Object>();

	protected Dictionary<knetid, Server_Object> server_objects = new Dictionary<knetid, Server_Object>();

	protected List<Server_Object> server_objects_list = new List<Server_Object>();

	internal Dictionary<knetid, Server_PerPlrState> server_perplrstates = new Dictionary<knetid, Server_PerPlrState>();

	protected List<bool> server_pack_bools = new List<bool>();

	protected IDeltaPacketBase base_packet;

	private int max_snapshot_queue = 2000;

	public int MAX_PACKET_SIZE = 512;

	private ushort _serverIdCounter = 1;

	public float fastsync_interval = 0.04f;

	private float fastsync_timer;

	private int client_checkerconter;

	private static double DASFGDSFSFFDSDSFSDFFDSFDS;

	public int last_sent_packet_objcount_max;

	public int last_sent_packet_objcount_max_forced;

	private HashSet<knetid> _packer_avoidduplicates = new HashSet<knetid>();

	public IEnumerable<knetid> GetObjectIds()
	{
		if (Net.is_server)
		{
			return server_objects.Keys;
		}
		return client_objects.Keys;
	}

	public void Shared_ForceSync(knetid obj_netId)
	{
		if (Net.is_server)
		{
			Server_QueueForceSyncForAll(obj_netId);
		}
		else
		{
			Client_RequestObjectInfo(obj_netId);
		}
	}

	public bool IsRegisteted(knetid obj_netId)
	{
		if (Net.is_server)
		{
			return server_objects.ContainsKey(obj_netId);
		}
		return client_objects.ContainsKey(obj_netId);
	}

	public void Server_QueueForceSyncForAll(knetid obj_netId)
	{
		if (!server_objects.TryGetValue(obj_netId, out var _))
		{
			return;
		}
		foreach (KeyValuePair<knetid, Server_PerPlrState> server_perplrstate in server_perplrstates)
		{
			Server_Internal_QueueForceSync(server_perplrstate.Value, obj_netId);
		}
	}

	public void Server_QueueForceSync(knetid plrId, knetid obj_netId)
	{
		if (server_perplrstates.TryGetValue(plrId, out var value))
		{
			Server_Internal_QueueForceSync(value, obj_netId);
		}
	}

	protected virtual void Server_Internal_QueueForceSync(Server_PerPlrState plrstate, knetid obj_netId)
	{
		if (!plrstate.forcesync_queue.Contains(obj_netId))
		{
			plrstate.forcesync_queue.Enqueue(obj_netId);
		}
		server_has_queued_forcesync = true;
	}

	public virtual void Client_RequestObjectInfo(knetid obj_netId)
	{
		if (!client_datarequestqueue.Contains(obj_netId))
		{
			client_datarequestqueue.Enqueue(obj_netId);
		}
	}

	public virtual void Server_ReceiveDataRequest(NetPlayer plr, NetDataReader reader)
	{
		for (int i = 0; i < datarequestqueue_max; i++)
		{
			if (reader.EndOfData)
			{
				break;
			}
			reader.Get(out knetid result);
			Server_ResetDeltaOnObj(plr.clientId, result);
		}
	}

	public virtual void Server_ResetDeltaOnObj(knetid plrId, knetid objId)
	{
		Server_PerPlrState perPlrState = GetPerPlrState(plrId);
		if (perPlrState.objstates.TryGetValue(objId, out var value))
		{
			if (server_objects.TryGetValue(objId, out var value2))
			{
				value2.players_requested_info.Add(plrId);
			}
			else if (log.verbose)
			{
				log.warn("SERVER: client requested object data for an object that was deleted, but still is in their perplr object dict?? " + value2.ToString() + "  from " + ServerMain.GetPlayerFullDebugString(plrId));
			}
			value.last_known_snapshot = Serialization.CloneViaSerialization<IDeltaPacketBase>(base_packet, false);
			perPlrState.forcesync_queue.Enqueue(objId);
			server_has_queued_forcesync = true;
		}
		else
		{
			perPlrState.forcedelete_queue.Enqueue(objId);
		}
	}

	protected Server_PerPlrState GetPerPlrState(knetid plrId)
	{
		if (!server_perplrstates.TryGetValue(plrId, out var value))
		{
			value = new Server_PerPlrState();
			value.plrId = plrId;
			value.system = this;
			server_perplrstates[plrId] = value;
		}
		return value;
	}

	protected Server_PerPlrState.Server_PerPlrObjectState GetObjStateForPlr(knetid plrId, knetid obj_netId)
	{
		return GetPerPlrState(plrId).GetObjState(obj_netId);
	}

	public CoolSyncSubSystemForObjects(byte systemid)
		: base(systemid)
	{
	}

	public override string ToString()
	{
		if (this == null)
		{
			return "SYNCSYS(NULL)";
		}
		return $"{GetType().Name}({base.syncsystemid})";
	}

	protected virtual void Client_DeleteObject(Client_Object obj)
	{
	}

	public override void Server_ReceiveAck(NetPlayer plr, NetDataReader reader)
	{
		ushort num = default(ushort);
		reader.Get(ref num);
		Server_PerPlrState perPlrState = GetPerPlrState(plr.clientId);
		if (!perPlrState.snapshots.TryGetValue(num, out var value))
		{
			return;
		}
		bool flag = true;
		foreach (KeyValuePair<knetid, IDeltaPacketBase> objects_it_contain in value.objects_it_contains)
		{
			knetid key = objects_it_contain.Key;
			if (server_objects.TryGetValue(key, out var value2))
			{
				Server_PerPlrState.Server_PerPlrObjectState objState = perPlrState.GetObjState(key);
				if (!BaseCoolSyncSubSystem.IsRollNewer(num, objState.last_known_snapshot_id))
				{
					continue;
				}
				objState.last_known_snapshot = objects_it_contain.Value;
				objState.last_known_snapshot_id = num;
				if (value.is_deleted)
				{
					value2.players_its_been_sent_to.Remove(plr.clientId);
				}
				else
				{
					value2.players_requested_info.Remove(plr.clientId);
					value2.players_its_been_sent_to.Add(plr.clientId);
				}
				if (value2.IsDeleted() && value.is_deleted)
				{
					flag = true;
					if (value2.players_its_been_sent_to.Count == 0)
					{
						Server_Internal_DeallocateObject(key);
						continue;
					}
				}
				flag = true;
			}
			else
			{
				perPlrState.forcedelete_queue.Enqueue(key);
			}
		}
		if (flag)
		{
			perPlrState.clear_queued = true;
		}
	}

	public override void Client_Receive(NetDataReader reader)
	{
		ushort cur_packet_deltaid = default(ushort);
		reader.Get(ref cur_packet_deltaid);
		if (im_in_the_thick_of_it_everybody_knows_they_know_me_where_it_snows_i_skied_in_and_they_froze_i_dont_know_no_nothin_bout_no_ice_im_just_cold_40somethin_milli_subs_or_so_ive_been_told__im_in_my_prime_and_this_aint_even_final_form_they_knocked_me_down_but_still_my_feet_they_find_the_floor_i_went_from_livin_rooms_straight_out_to_soldout_tours_lifes_a_fight_but_trust_im_ready_for_the_war__whoaohoh_this_is_how_the_story_goes_whoaohoh_i_guess_this_is_how_the_story_goes__im_in_the_thick_of_it_everybody_knows_they_know_me_where_it_snows_i_skied_in_and_they_froze_i_dont_know_no_nothin_bout_no_ice_im_just_cold_40somethin_milli_subs_or_so_ive_been_told__from_the_screen_to_the_ring_to_the_pen_to_the_king_wheres_my_crown_thats_my_bling_always_drama_when_i_ring_see_i_believe_that_if_i_see_it_in_my_heart_smash_through_the_ceilin_cause_im_reachin_for_the_stars__whoaohoh_this_is_how_the_story_goes_whoaohoh_i_guess_this_is_how_the_story_goes)
		{
			reader = reader.DecompressReader();
		}
		byte b = default(byte);
		reader.Get(ref b);
		byte b2 = default(byte);
		byte b3 = default(byte);
		ushort num = default(ushort);
		for (int i = 0; i < b; i++)
		{
			reader.Get(out knetid result);
			reader.Get(ref b2);
			Client_Object value2;
			if (b2 > 0)
			{
				if (!client_objects.TryGetValue(result, out var value))
				{
					value = new Client_Object();
					value.sys = this;
					value.netId = result;
					value.cur_packet_deltaid = cur_packet_deltaid;
					value.cur_packet = Serialization.CloneViaSerialization<IDeltaPacketBase>(base_packet, false);
					client_objects[result] = value;
				}
				value.last_receive_time = Time.realtimeSinceStartupAsDouble;
				reader.Get(ref b3);
				reader.Get(ref num);
				int position = reader.Position;
				reader.SetPosition(position + b3 + num);
				byte[] array = new byte[b2];
				reader.GetBytes(array, (int)b2);
				value.bitset = new BitArray(array);
				try
				{
					reader.SetPosition(position);
					Client_ReadData1(reader, b3, value);
					reader.SetPosition(position + b3);
					Client_ReadData2(reader, num, value);
				}
				catch (Exception ex)
				{
					log.error("Client_Receive: " + value.ToString() + " : " + ex.ToString());
				}
				reader.SetPosition(position + b3 + num + b2);
			}
			else if (client_objects.TryGetValue(result, out value2))
			{
				client_objects.Remove(result);
				Client_DeleteObject(value2);
			}
		}
	}

	protected virtual void Client_ReadData1(NetDataReader reader, ushort data1_len, Client_Object obj)
	{
		obj.cur_packet.Read(reader, obj.bitset);
	}

	protected virtual void Client_ReadData2(NetDataReader reader, ushort data2_len, Client_Object obj)
	{
	}

	protected knetid Server_PickNextId()
	{
		while (true)
		{
			if (_serverIdCounter == 0)
			{
				_serverIdCounter++;
				continue;
			}
			if (!server_objects.ContainsKey(_serverIdCounter))
			{
				break;
			}
			_serverIdCounter++;
		}
		return _serverIdCounter++;
	}

	public virtual knetid Server_NewObject(object obj)
	{
		knetid knetid2 = Server_PickNextId();
		Server_Internal_AllocateNewObject(knetid2, obj);
		Server_QueueForceSyncForAll(knetid2);
		return knetid2;
	}

	public virtual knetid Server_NewObject(object obj, knetid forced_id)
	{
		Server_Internal_AllocateNewObject(forced_id, obj);
		Server_QueueForceSyncForAll(forced_id);
		return forced_id;
	}

	protected virtual Server_Object Server_Internal_AllocateNewObject(knetid targetid, object obj)
	{
		Server_Object server_Object = new Server_Object();
		server_Object.netId = targetid;
		server_Object.real_obj = obj;
		if (Server_DeleteObject(targetid))
		{
			Server_Internal_DeallocateObject(targetid);
		}
		server_objects[targetid] = server_Object;
		return server_Object;
	}

	public override void ClearAndResetEverything()
	{
		server_objects_list.Clear();
		server_perplrstates.Clear();
		server_objects.Clear();
		client_objects.Clear();
		server_has_queued_forcesync = false;
	}

	public virtual bool Server_DeleteObject(knetid netId)
	{
		if (server_objects.TryGetValue(netId, out var value))
		{
			Server_QueueForceSyncForAll(netId);
			value.real_obj = null;
			return true;
		}
		return false;
	}

	protected virtual void Server_Internal_DeallocateObject(knetid objId)
	{
		server_objects.Remove(objId);
		foreach (Server_PerPlrState value in server_perplrstates.Values)
		{
			value.objstates.Remove(objId);
		}
	}

	public virtual void Server_FastSyncUpdate()
	{
		fastsync_timer += Time.unscaledDeltaTime;
		if (fastsync_timer > fastsync_interval)
		{
			fastsync_timer = 0f;
		}
	}

	public virtual void Server_RunFastSync()
	{
	}

	public override void Update()
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		if (Net.is_client)
		{
			if (client_datarequestqueue.Count > 0)
			{
				NetDataWriter writer = Net.CreateWriter((Enum)NetmsgId.Server_CoolSyncObjectDataRequest);
				writer.Put(base.syncsystemid);
				for (int i = 0; i < Math.Min(datarequestqueue_max, client_datarequestqueue.Count); i++)
				{
					writer.Put((ushort)client_datarequestqueue.Dequeue());
				}
				Net.Client_Send((DeliveryMethod)4, in writer);
				return;
			}
			List<Client_Object> list = client_objects.Values.ToList();
			if (client_checkerconter >= list.Count)
			{
				client_checkerconter = 0;
			}
			for (int k = 0; k < Math.Min(40, list.Count); k++)
			{
				Client_Object client_Object = list[client_checkerconter];
				if ((Time.realtimeSinceStartupAsDouble - client_Object.last_receive_time) * (double)Mathf.Clamp01(ClientMain.ServerPerformanceScale * 2f) > 40.0)
				{
					client_Object.last_receive_time += 10f / ClientMain.ServerPerformanceScale;
					if (log.verbose)
					{
						log.l("NET OBJ " + client_Object.ToString() + " seems to got forgotten, requesting server for info.");
					}
					Client_RequestObjectInfo(client_Object.netId);
				}
				client_checkerconter++;
				if (client_checkerconter >= list.Count)
				{
					client_checkerconter = 0;
				}
			}
			return;
		}
		Server_ClearOldPlayers();
		foreach (Server_PerPlrState value in server_perplrstates.Values)
		{
			if (value.clear_queued)
			{
				value.clear_queued = Server_OneStepClearOldSnapshots(value);
				if (value.clear_queued)
				{
					value.clear_queued = Server_OneStepClearOldSnapshots(value);
				}
			}
		}
		Server_FastSyncUpdate();
	}

	protected bool Server_OneStepClearOldSnapshots(Server_PerPlrState plrstate)
	{
		if (plrstate.snapshot_queue.Count < 2)
		{
			return false;
		}
		ushort deltaid = plrstate.snapshot_queue.Peek();
		if (plrstate.snapshots.Count > 1500)
		{
			Server_PerPlrState.Server_Snapshot arg = plrstate.snapshots[plrstate.snapshot_queue.ElementAt(plrstate.snapshots.Count - 1)];
			if (DASFGDSFSFFDSDSFSDFFDSFDS == 0.0 && plrstate.snapshots.Count < 1520 && log.verbose)
			{
				Con.OpenConsole();
			}
			if (Time.realtimeSinceStartupAsDouble - DASFGDSFSFFDSDSFSDFFDSFDS > 30.0)
			{
				DASFGDSFSFFDSDSFSDFFDSFDS = Time.realtimeSinceStartupAsDouble;
				log.warn($"{ToString()} -> TOO MUCH SNAPSHOTS!!! {plrstate.ToString()} OLDEST SNAPSHOT: {arg}");
			}
		}
		if (plrstate.objstates.Any((KeyValuePair<knetid, Server_PerPlrState.Server_PerPlrObjectState> x) => x.Value.last_known_snapshot_id == deltaid) && plrstate.snapshots.Count < 4000)
		{
			return false;
		}
		deltaid = plrstate.snapshot_queue.Dequeue();
		plrstate.snapshots.Remove(deltaid);
		return true;
	}

	public override bool Server_Update()
	{
		double realtimeSinceStartupAsDouble = Time.realtimeSinceStartupAsDouble;
		if (realtimeSinceStartupAsDouble - last_send_time > (double)SEND_FREQUENCY)
		{
			last_send_time = realtimeSinceStartupAsDouble;
			Server_ClearOldPlayers();
			return PackAndSend();
		}
		if (server_has_queued_forcesync)
		{
			Server_ClearOldPlayers();
			PackAndSend(only_queue: true);
		}
		return false;
	}

	protected virtual void Server_RemovePlr(knetid plrId)
	{
		server_perplrstates[plrId].objstates.Clear();
		server_perplrstates.Remove(plrId);
		List<knetid> list = new List<knetid>();
		foreach (KeyValuePair<knetid, Server_Object> server_object in server_objects)
		{
			server_object.Value.players_its_been_sent_to.Remove(plrId);
			if (server_object.Value.real_obj == null && server_object.Value.players_its_been_sent_to.Count == 0)
			{
				list.Add(server_object.Key);
			}
		}
		foreach (knetid item in list)
		{
			Server_Internal_DeallocateObject(item);
		}
	}

	protected virtual void Server_ClearOldPlayers()
	{
		foreach (knetid item in new List<knetid>(server_perplrstates.Keys))
		{
			if (!NetPlayer.ClientIdToPlayerDict.ContainsKey(item))
			{
				Server_RemovePlr(item);
			}
		}
	}

	protected virtual bool PackAndSend(bool only_queue = false)
	{
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		server_objects_list.Clear();
		if (NetPlayer.ClientIdToPlayerDict.Count == 0)
		{
			return false;
		}
		if (server_objects.Count == 0)
		{
			return false;
		}
		foreach (Server_Object value in server_objects.Values)
		{
			value.cur_frame_packed = false;
		}
		last_sent_packet_objcount_max_forced = 0;
		last_sent_packet_objcount_max = 0;
		server_objects_list = server_objects.Values.ToList();
		NetDataWriter writer = Net.CreateWriter((Enum)NetmsgId.Client_CoolSyncReceiver);
		writer.Put(base.syncsystemid);
		int length = writer.Length;
		int start_to_ignore = length + 2;
		bool result = false;
		foreach (NetPlayer item in ServerMain.AllPlayersExceptHost)
		{
			try
			{
				if (PackPacketPerPlr(writer, item, only_queue))
				{
					result = true;
					last_sent_packet_size = writer.Length;
					if (im_in_the_thick_of_it_everybody_knows_they_know_me_where_it_snows_i_skied_in_and_they_froze_i_dont_know_no_nothin_bout_no_ice_im_just_cold_40somethin_milli_subs_or_so_ive_been_told__im_in_my_prime_and_this_aint_even_final_form_they_knocked_me_down_but_still_my_feet_they_find_the_floor_i_went_from_livin_rooms_straight_out_to_soldout_tours_lifes_a_fight_but_trust_im_ready_for_the_war__whoaohoh_this_is_how_the_story_goes_whoaohoh_i_guess_this_is_how_the_story_goes__im_in_the_thick_of_it_everybody_knows_they_know_me_where_it_snows_i_skied_in_and_they_froze_i_dont_know_no_nothin_bout_no_ice_im_just_cold_40somethin_milli_subs_or_so_ive_been_told__from_the_screen_to_the_ring_to_the_pen_to_the_king_wheres_my_crown_thats_my_bling_always_drama_when_i_ring_see_i_believe_that_if_i_see_it_in_my_heart_smash_through_the_ceilin_cause_im_reachin_for_the_stars__whoaohoh_this_is_how_the_story_goes_whoaohoh_i_guess_this_is_how_the_story_goes)
					{
						writer.CompressWriter(start_to_ignore);
					}
					last_sent_packet_size_compressed = writer.Length;
					Net.Server_SendToClients((DeliveryMethod)4, in writer, item.clientId);
					GetPerPlrState(item.clientId).cur_delta_roll++;
				}
			}
			catch (Exception ex)
			{
				Plugin.Logger.LogError((object)ex.ToString());
			}
			writer.SetPosition(length);
		}
		if (last_sent_packet_objcount_max_forced == 0)
		{
			server_has_queued_forcesync = false;
		}
		return result;
	}

	protected virtual void SaveSnapshot(Server_PerPlrState perplr, Server_Object obj)
	{
		if (!perplr.snapshots.TryGetValue(perplr.cur_delta_roll, out var value))
		{
			value = new Server_PerPlrState.Server_Snapshot();
			value.deltaid = perplr.cur_delta_roll;
			value.is_deleted = obj.IsDeleted();
			perplr.snapshots[perplr.cur_delta_roll] = value;
			perplr.snapshot_queue.Enqueue(perplr.cur_delta_roll);
			if (perplr.snapshot_queue.Count > max_snapshot_queue)
			{
				for (int i = 0; i < Mathf.CeilToInt((float)max_snapshot_queue * 0.5f); i++)
				{
					ushort key = perplr.snapshot_queue.Dequeue();
					perplr.snapshots.Remove(key);
				}
			}
		}
		if (!value.objects_it_contains.ContainsKey(obj.netId))
		{
			value.objects_it_contains[obj.netId] = obj.cur_packet;
		}
	}

	protected virtual bool ShouldPackPacketFor(NetPlayer plr)
	{
		return true;
	}

	protected virtual bool PackPacketPerPlr(NetDataWriter writer, NetPlayer plr, bool only_queue)
	{
		if (!ShouldPackPacketFor(plr))
		{
			return false;
		}
		Server_PerPlrState perPlrState = GetPerPlrState(plr.clientId);
		writer.Put(perPlrState.cur_delta_roll);
		int length = writer.Length;
		writer.Put((byte)0);
		_packer_avoidduplicates.Clear();
		byte b = 0;
		while (perPlrState.forcedelete_queue.Count > 0)
		{
			knetid knetid2 = perPlrState.forcedelete_queue.Dequeue();
			_packer_avoidduplicates.Add(knetid2);
			writer.Put((ushort)knetid2);
			writer.Put((byte)0);
			b++;
			if ((float)writer.Length > (float)MAX_PACKET_SIZE * 0.4f)
			{
				break;
			}
		}
		if (perPlrState.forcesync_queue.Count > last_sent_packet_objcount_max_forced)
		{
			last_sent_packet_objcount_max_forced = perPlrState.forcesync_queue.Count;
		}
		while (perPlrState.forcesync_queue.Count > 0)
		{
			knetid knetid3 = perPlrState.forcesync_queue.Dequeue();
			if (!server_objects.TryGetValue(knetid3, out var value) || _packer_avoidduplicates.Contains(knetid3))
			{
				continue;
			}
			_packer_avoidduplicates.Add(knetid3);
			if (PackObjectForPlr(writer, perPlrState, value))
			{
				b++;
				if ((float)writer.Length > (float)MAX_PACKET_SIZE * 0.75f)
				{
					break;
				}
			}
		}
		if ((ushort)perPlrState.last_sent_object_index >= server_objects_list.Count)
		{
			perPlrState.last_sent_object_index = (ushort)0;
		}
		if (only_queue)
		{
			if (b == 0)
			{
				return false;
			}
		}
		else
		{
			for (int i = 0; i < Math.Min(255, server_objects_list.Count) - b; i++)
			{
				Server_Object server_Object = server_objects_list[(ushort)perPlrState.last_sent_object_index];
				if (!_packer_avoidduplicates.Contains(server_Object.netId))
				{
					if (PackObjectForPlr(writer, perPlrState, server_Object))
					{
						b++;
					}
					perPlrState.last_sent_object_index = (ushort)((ushort)perPlrState.last_sent_object_index + 1);
					if ((ushort)perPlrState.last_sent_object_index >= server_objects_list.Count)
					{
						perPlrState.last_sent_object_index = (ushort)0;
					}
					if (writer.Length > MAX_PACKET_SIZE)
					{
						break;
					}
				}
			}
		}
		if (b == 0)
		{
			return false;
		}
		if (b > last_sent_packet_objcount_max)
		{
			last_sent_packet_objcount_max = b;
		}
		int position = writer.SetPosition(length);
		writer.Put(b);
		writer.SetPosition(position);
		return true;
	}

	protected virtual bool PackObjectForPlr(NetDataWriter writer, Server_PerPlrState plrstate, Server_Object obj)
	{
		if (!obj.cur_frame_packed)
		{
			PackPacket(obj);
			obj.cur_frame_packed = true;
		}
		server_pack_bools.Clear();
		_ = writer.Length;
		writer.Put((ushort)obj.netId);
		int length = writer.Length;
		writer.Put((byte)0);
		if (obj.real_obj == null)
		{
			SaveSnapshot(plrstate, obj);
		}
		else
		{
			writer.Put((byte)0);
			writer.Put((ushort)0);
			byte b = 0;
			int length2 = writer.Length;
			PackData1(writer, server_pack_bools, plrstate, obj);
			byte b2 = (byte)(writer.Length - length2);
			SaveSnapshot(plrstate, obj);
			length2 = writer.Length;
			PackData2(writer, server_pack_bools, plrstate, obj);
			ushort num = (ushort)(writer.Length - length2);
			if (server_pack_bools.Count > 0)
			{
				byte[] array = BaseCoolSyncSubSystem.BoolListToByteBitset(server_pack_bools);
				writer.Put(array);
				b = (byte)array.Length;
			}
			int position = writer.SetPosition(length);
			writer.Put(b);
			writer.Put(b2);
			writer.Put(num);
			writer.SetPosition(position);
		}
		return true;
	}

	protected virtual void PackPacket(Server_Object obj)
	{
		obj.cur_packet = Serialization.CloneViaSerialization<IDeltaPacketBase>(base_packet, false);
	}

	protected virtual void PackData1(NetDataWriter writer, List<bool> pack_bools, Server_PerPlrState plrstate, Server_Object obj)
	{
		Server_PerPlrState.Server_PerPlrObjectState objState = plrstate.GetObjState(obj.netId);
		obj.cur_packet.Write(writer, pack_bools, objState.last_known_snapshot);
	}

	protected virtual void PackData2(NetDataWriter writer, List<bool> pack_bools, Server_PerPlrState plrstate, Server_Object obj)
	{
	}
}
