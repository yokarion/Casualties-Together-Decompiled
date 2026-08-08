using System;
using System.Collections;
using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public class CoolSyncSubSystemStatic : BaseCoolSyncSubSystem
{
	protected class Server_PerPlrState
	{
		public knetid plrId;

		public ushort last_known_snapshot_id;

		public IDeltaPacketBase last_known_snapshot;
	}

	protected Dictionary<knetid, Server_PerPlrState> server_perplrstates = new Dictionary<knetid, Server_PerPlrState>();

	protected List<bool> server_pack_bools = new List<bool>();

	protected IDeltaPacketBase base_packet;

	protected IDeltaPacketBase cur_packet;

	internal Dictionary<ushort, IDeltaPacketBase> server_snapshots = new Dictionary<ushort, IDeltaPacketBase>();

	internal Queue<ushort> server_snapshot_queue = new Queue<ushort>();

	private int max_snapshot_queue = 1000;

	internal ushort server_delta_roll_counter;

	protected BitArray client_bitset;

	protected Server_PerPlrState GetPerPlrState(knetid plrId)
	{
		if (!server_perplrstates.TryGetValue(plrId, out var value))
		{
			value = new Server_PerPlrState();
			value.plrId = plrId;
			value.last_known_snapshot = Serialization.CloneViaSerialization<IDeltaPacketBase>(base_packet, false);
			value.last_known_snapshot_id = (ushort)(server_delta_roll_counter - 1);
			server_perplrstates[plrId] = value;
		}
		return value;
	}

	public CoolSyncSubSystemStatic(byte systemid)
		: base(systemid)
	{
	}

	public override void Server_ReceiveAck(NetPlayer plr, NetDataReader reader)
	{
		ushort num = default(ushort);
		reader.Get(ref num);
		if (server_snapshots.TryGetValue(num, out var value))
		{
			Server_PerPlrState perPlrState = GetPerPlrState(plr.clientId);
			if (BaseCoolSyncSubSystem.IsRollNewer(num, perPlrState.last_known_snapshot_id))
			{
				perPlrState.last_known_snapshot = value;
				perPlrState.last_known_snapshot_id = num;
				ClearOldSnapshots();
			}
		}
	}

	protected virtual void ClearOldSnapshots()
	{
		while (server_snapshot_queue.Count > 2)
		{
			ushort num = server_snapshot_queue.Peek();
			bool flag = false;
			foreach (KeyValuePair<knetid, Server_PerPlrState> server_perplrstate in server_perplrstates)
			{
				if (server_perplrstate.Value.last_known_snapshot_id == num)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				num = server_snapshot_queue.Dequeue();
				server_snapshots.Remove(num);
				continue;
			}
			break;
		}
	}

	public override void Client_Receive(NetDataReader reader)
	{
		reader.Get(ref server_delta_roll_counter);
		if (im_in_the_thick_of_it_everybody_knows_they_know_me_where_it_snows_i_skied_in_and_they_froze_i_dont_know_no_nothin_bout_no_ice_im_just_cold_40somethin_milli_subs_or_so_ive_been_told__im_in_my_prime_and_this_aint_even_final_form_they_knocked_me_down_but_still_my_feet_they_find_the_floor_i_went_from_livin_rooms_straight_out_to_soldout_tours_lifes_a_fight_but_trust_im_ready_for_the_war__whoaohoh_this_is_how_the_story_goes_whoaohoh_i_guess_this_is_how_the_story_goes__im_in_the_thick_of_it_everybody_knows_they_know_me_where_it_snows_i_skied_in_and_they_froze_i_dont_know_no_nothin_bout_no_ice_im_just_cold_40somethin_milli_subs_or_so_ive_been_told__from_the_screen_to_the_ring_to_the_pen_to_the_king_wheres_my_crown_thats_my_bling_always_drama_when_i_ring_see_i_believe_that_if_i_see_it_in_my_heart_smash_through_the_ceilin_cause_im_reachin_for_the_stars__whoaohoh_this_is_how_the_story_goes_whoaohoh_i_guess_this_is_how_the_story_goes)
		{
			reader = reader.DecompressReader();
		}
		byte b = default(byte);
		reader.Get(ref b);
		ushort num = default(ushort);
		reader.Get(ref num);
		ushort num2 = default(ushort);
		reader.Get(ref num2);
		int position = reader.Position;
		reader.SetPosition(position + num + num2);
		byte[] array = new byte[b];
		reader.GetBytes(array, (int)b);
		client_bitset = new BitArray(array);
		reader.SetPosition(position);
		Client_ReadData1(reader, num);
		reader.SetPosition(position + num);
		Client_ReadData2(reader, num2);
		reader.SetPosition(position + num + num2 + b);
	}

	protected virtual void Client_ReadData1(NetDataReader reader, ushort data1_len)
	{
		cur_packet.Read(reader, client_bitset);
	}

	protected virtual void Client_ReadData2(NetDataReader reader, ushort data2_len)
	{
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
		return false;
	}

	protected virtual void Server_ClearOldPlayers()
	{
		foreach (knetid item in new List<knetid>(server_perplrstates.Keys))
		{
			if (!NetPlayer.ClientIdToPlayerDict.ContainsKey(item))
			{
				server_perplrstates.Remove(item);
			}
		}
	}

	protected virtual bool PackAndSend()
	{
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		if (NetPlayer.ClientIdToPlayerDict.Count == 0)
		{
			return false;
		}
		PackPacket();
		NetDataWriter writer = Net.CreateWriter((Enum)NetmsgId.Client_CoolSyncReceiver);
		writer.Put(base.syncsystemid);
		writer.Put(server_delta_roll_counter);
		int length = writer.Length;
		bool flag = false;
		foreach (NetPlayer item in ServerMain.AllPlayersExceptHost)
		{
			try
			{
				if (PackPacketPerPlr(writer, item))
				{
					flag = true;
					last_sent_packet_size = writer.Length;
					if (im_in_the_thick_of_it_everybody_knows_they_know_me_where_it_snows_i_skied_in_and_they_froze_i_dont_know_no_nothin_bout_no_ice_im_just_cold_40somethin_milli_subs_or_so_ive_been_told__im_in_my_prime_and_this_aint_even_final_form_they_knocked_me_down_but_still_my_feet_they_find_the_floor_i_went_from_livin_rooms_straight_out_to_soldout_tours_lifes_a_fight_but_trust_im_ready_for_the_war__whoaohoh_this_is_how_the_story_goes_whoaohoh_i_guess_this_is_how_the_story_goes__im_in_the_thick_of_it_everybody_knows_they_know_me_where_it_snows_i_skied_in_and_they_froze_i_dont_know_no_nothin_bout_no_ice_im_just_cold_40somethin_milli_subs_or_so_ive_been_told__from_the_screen_to_the_ring_to_the_pen_to_the_king_wheres_my_crown_thats_my_bling_always_drama_when_i_ring_see_i_believe_that_if_i_see_it_in_my_heart_smash_through_the_ceilin_cause_im_reachin_for_the_stars__whoaohoh_this_is_how_the_story_goes_whoaohoh_i_guess_this_is_how_the_story_goes)
					{
						writer.CompressWriter(length);
					}
					last_sent_packet_size_compressed = writer.Length;
					Net.Server_SendToClients((DeliveryMethod)4, in writer, item.clientId);
				}
			}
			catch (Exception ex)
			{
				Plugin.Logger.LogError((object)ex.ToString());
			}
			writer.SetPosition(length);
		}
		if (flag)
		{
			SaveSnapshot();
		}
		return flag;
	}

	protected virtual void SaveSnapshot()
	{
		server_snapshots[server_delta_roll_counter] = cur_packet;
		server_snapshot_queue.Enqueue(server_delta_roll_counter);
		while (server_snapshot_queue.Count > max_snapshot_queue)
		{
			ushort key = server_snapshot_queue.Dequeue();
			server_snapshots.Remove(key);
		}
		server_delta_roll_counter++;
	}

	protected virtual void PackPacket()
	{
		cur_packet = Serialization.CloneViaSerialization<IDeltaPacketBase>(base_packet, false);
	}

	protected virtual bool ShouldPackPacketFor(NetPlayer plr)
	{
		if (GetPerPlrState(plr.clientId).last_known_snapshot.Equals(cur_packet))
		{
			return false;
		}
		return true;
	}

	protected virtual bool PackPacketPerPlr(NetDataWriter writer, NetPlayer plr)
	{
		if (!ShouldPackPacketFor(plr))
		{
			return false;
		}
		server_pack_bools.Clear();
		int length = writer.Length;
		writer.Put((byte)0);
		writer.Put((ushort)0);
		writer.Put((ushort)0);
		byte b = 0;
		int length2 = writer.Length;
		PackData1(writer, server_pack_bools, plr);
		ushort num = (ushort)(writer.Length - length2);
		length2 = writer.Length;
		PackData2(writer, server_pack_bools, plr);
		ushort num2 = (ushort)(writer.Length - length2);
		if (server_pack_bools.Count > 0)
		{
			byte[] array = BaseCoolSyncSubSystem.BoolListToByteBitset(server_pack_bools);
			writer.Put(array);
			b = (byte)array.Length;
		}
		int position = writer.SetPosition(length);
		writer.Put(b);
		writer.Put(num);
		writer.Put(num2);
		writer.SetPosition(position);
		return true;
	}

	protected virtual void PackData1(NetDataWriter writer, List<bool> pack_bools, NetPlayer plr)
	{
		Server_PerPlrState perPlrState = GetPerPlrState(plr.clientId);
		cur_packet.Write(writer, pack_bools, perPlrState.last_known_snapshot);
	}

	protected virtual void PackData2(NetDataWriter writer, List<bool> pack_bools, NetPlayer plr)
	{
	}
}
