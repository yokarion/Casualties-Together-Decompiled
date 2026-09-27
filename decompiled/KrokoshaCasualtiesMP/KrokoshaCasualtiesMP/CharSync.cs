using System.Collections.Generic;
using KrokoshaCasualtiesUtils;
using LiteNetLib.Utils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

internal class CharSync : CoolSyncSubSystemForObjects
{
	public static CharSync inst;

	public CharSync(byte systemid)
		: base(systemid)
	{
		SEND_FREQUENCY = 1f / 30f;
		base_packet = default(NetBodySyncPacket);
		base_packet.SetDefault();
		CleanupAndUnregisterOnLevelChange = true;
		inst = this;
		im_in_the_thick_of_it_everybody_knows_they_know_me_where_it_snows_i_skied_in_and_they_froze_i_dont_know_no_nothin_bout_no_ice_im_just_cold_40somethin_milli_subs_or_so_ive_been_told__im_in_my_prime_and_this_aint_even_final_form_they_knocked_me_down_but_still_my_feet_they_find_the_floor_i_went_from_livin_rooms_straight_out_to_soldout_tours_lifes_a_fight_but_trust_im_ready_for_the_war__whoaohoh_this_is_how_the_story_goes_whoaohoh_i_guess_this_is_how_the_story_goes__im_in_the_thick_of_it_everybody_knows_they_know_me_where_it_snows_i_skied_in_and_they_froze_i_dont_know_no_nothin_bout_no_ice_im_just_cold_40somethin_milli_subs_or_so_ive_been_told__from_the_screen_to_the_ring_to_the_pen_to_the_king_wheres_my_crown_thats_my_bling_always_drama_when_i_ring_see_i_believe_that_if_i_see_it_in_my_heart_smash_through_the_ceilin_cause_im_reachin_for_the_stars__whoaohoh_this_is_how_the_story_goes_whoaohoh_i_guess_this_is_how_the_story_goes = true;
	}

	protected override void Client_DeleteObject(Client_Object obj)
	{
		if (obj.real_obj != null)
		{
			NetBody netBody = (NetBody)obj.real_obj;
			if ((Object)(object)netBody != (Object)null)
			{
				NetBody.DestroyNPC(netBody);
			}
		}
	}

	protected override void PackPacket(Server_Object obj)
	{
		if (obj.real_obj == null)
		{
			return;
		}
		NetBody netBody = (NetBody)obj.real_obj;
		if ((Object)(object)netBody != (Object)null)
		{
			NetBodySyncPacket netBodySyncPacket = new NetBodySyncPacket(netBody);
			netBody._last_sync_packet_receive_time = Time.realtimeSinceStartupAsDouble;
			if (netBody.is_player)
			{
				netBodySyncPacket.is_player = true;
				netBodySyncPacket.plrid = netBody.plr.clientId;
			}
			netBody.last_sync_packet = netBodySyncPacket;
			obj.cur_packet = netBodySyncPacket;
		}
		else
		{
			Server_DeleteObject(obj.netId);
		}
	}

	protected override void PackData2(NetDataWriter writer, List<bool> pack_bools, Server_PerPlrState plr, Server_Object obj)
	{
		bool flag = false;
		if (obj.real_obj != null)
		{
			NetBody netBody = (NetBody)obj.real_obj;
			flag = !netBody.is_player && (!obj.players_its_been_sent_to.Contains(plr.plrId) || obj.players_requested_info.Contains(plr.plrId));
			if (flag)
			{
				writer.Put(netBody.bodyname);
			}
		}
		pack_bools.Add(flag);
	}

	protected override bool PackObjectForPlr(NetDataWriter writer, Server_PerPlrState plrstate, Server_Object obj)
	{
		if (obj.real_obj != null && obj.players_its_been_sent_to.Contains(plrstate.plrId))
		{
			NetBody netBody = (NetBody)obj.real_obj;
			if (!netBody.alive && (Object)(object)netBody.piggybacking_on == (Object)null && plrstate.cur_delta_roll % 5 != 0)
			{
				return false;
			}
		}
		return base.PackObjectForPlr(writer, plrstate, obj);
	}

	protected override bool ShouldPackPacketFor(NetPlayer plr)
	{
		if (!Util.IsWorldGenerated())
		{
			return false;
		}
		if (!plr.server_plrstate.finished_worldgen)
		{
			return false;
		}
		return true;
	}

	protected override void Client_ReadData2(NetDataReader reader, ushort data2_len, Client_Object obj)
	{
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		if (!Util.IsWorldInstantiated())
		{
			return;
		}
		int bitset_size = NetBodySyncPacket.bitset_size;
		NetBodySyncPacket last_sync_packet = (NetBodySyncPacket)(object)obj.cur_packet;
		NetBody netBody;
		if (obj.real_obj == null)
		{
			if (last_sync_packet.is_player && NetPlayer.TryGetPlayerFromClientId(last_sync_packet.plrid, out var plr))
			{
				if (plr.is_local)
				{
					netBody = NetPlayer.GetLocalNetBodyNullable();
					if ((Object)(object)netBody == (Object)null)
					{
						log.error($"Client_ReadData2 NO LOCAL NETBODY WHAAAAA???????? {plr}  -> {obj}");
					}
				}
				else
				{
					netBody = NetBody.CreateNewPlayerCharacter(plr);
				}
				netBody.netId = obj.netId;
			}
			else
			{
				netBody = NetBody._Internal_CreateNetBody(obj.netId, last_sync_packet.pos);
			}
			obj.real_obj = netBody;
		}
		else
		{
			netBody = (NetBody)obj.real_obj;
			if ((Object)(object)netBody.body == (Object)null)
			{
				NetBody.DestroyNPC(netBody);
				netBody = NetBody._Internal_CreateNetBody(obj.netId, last_sync_packet.pos);
			}
			if (last_sync_packet.is_player)
			{
				if (NetPlayer.TryGetPlayerFromClientId(last_sync_packet.plrid, out var plr2) && (Object)(object)plr2 != (Object)(object)netBody.plr)
				{
					netBody.plr = plr2;
					netBody.ApplyNameAndColor(plr2.playername, plr2.playerColor);
					netBody.netId = obj.netId;
				}
			}
			else if ((Object)(object)netBody.plr != (Object)null)
			{
				netBody.plr = null;
			}
		}
		if (obj.bitset[bitset_size++])
		{
			string text = default(string);
			reader.Get(ref text);
			if (text != netBody.bodyname)
			{
				netBody.ApplyNameAndColor(text, netBody.color);
			}
		}
		if (netBody.IsBodyLocal())
		{
			netBody.last_sync_packet = last_sync_packet;
			return;
		}
		if (netBody._last_sync_packet_receive_time > Time.realtimeSinceStartupAsDouble && ClientMain._last_reminderpack_while_generating_received)
		{
			ack = false;
			return;
		}
		netBody._last_sync_packet_receive_time = Time.realtimeSinceStartupAsDouble;
		last_sync_packet.Apply(netBody);
	}
}
