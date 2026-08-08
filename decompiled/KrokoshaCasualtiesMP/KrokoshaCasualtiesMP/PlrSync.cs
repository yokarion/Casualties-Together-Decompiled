using System;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

internal class PlrSync : CoolSyncSubSystemForObjects
{
	public static PlrSync inst;

	public PlrSync(byte systemid)
		: base(systemid)
	{
		SEND_FREQUENCY = 0.1f;
		base_packet = default(PlayerSyncPacket);
		base_packet.SetDefault();
		inst = this;
		im_in_the_thick_of_it_everybody_knows_they_know_me_where_it_snows_i_skied_in_and_they_froze_i_dont_know_no_nothin_bout_no_ice_im_just_cold_40somethin_milli_subs_or_so_ive_been_told__im_in_my_prime_and_this_aint_even_final_form_they_knocked_me_down_but_still_my_feet_they_find_the_floor_i_went_from_livin_rooms_straight_out_to_soldout_tours_lifes_a_fight_but_trust_im_ready_for_the_war__whoaohoh_this_is_how_the_story_goes_whoaohoh_i_guess_this_is_how_the_story_goes__im_in_the_thick_of_it_everybody_knows_they_know_me_where_it_snows_i_skied_in_and_they_froze_i_dont_know_no_nothin_bout_no_ice_im_just_cold_40somethin_milli_subs_or_so_ive_been_told__from_the_screen_to_the_ring_to_the_pen_to_the_king_wheres_my_crown_thats_my_bling_always_drama_when_i_ring_see_i_believe_that_if_i_see_it_in_my_heart_smash_through_the_ceilin_cause_im_reachin_for_the_stars__whoaohoh_this_is_how_the_story_goes_whoaohoh_i_guess_this_is_how_the_story_goes = true;
	}

	protected override void PackPacket(Server_Object obj)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		base.PackPacket(obj);
		if (obj.real_obj != null)
		{
			NetPlayer netPlayer = (NetPlayer)obj.real_obj;
			PlayerSyncPacket playerSyncPacket = (PlayerSyncPacket)(object)obj.cur_packet;
			playerSyncPacket.AutoSerialize(obj.netId, netPlayer);
			playerSyncPacket.camerapos = netPlayer.camerapos;
			playerSyncPacket.unchipped = netPlayer.IsUnchipped();
			obj.cur_packet = playerSyncPacket;
		}
	}

	protected override void Client_ReadData2(NetDataReader reader, ushort data2_len, Client_Object obj)
	{
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		_ = PlayerSyncPacket.bitset_size;
		NetPlayer plr = null;
		if (obj.real_obj == null)
		{
			if (!NetPlayer.TryGetPlayerFromClientId(obj.netId, out plr))
			{
				NetDataWriter writer = Net.CreateWriter((Enum)NetmsgId.SERVER_PlayerNameRequest);
				writer.Put((ushort)obj.netId);
				Net.Client_Send((DeliveryMethod)4, in writer);
				return;
			}
			obj.real_obj = plr;
		}
		else
		{
			plr = (NetPlayer)obj.real_obj;
		}
		PlayerSyncPacket playerSyncPacket = (PlayerSyncPacket)(object)obj.cur_packet;
		playerSyncPacket.AutoDeserialize(obj.netId, plr);
		plr.unchipped = playerSyncPacket.unchipped;
		plr.camerapos = playerSyncPacket.camerapos;
		if ((Object)(object)plr.body != (Object)null)
		{
			plr.playerbody.unchipped = plr.unchipped;
		}
	}
}
