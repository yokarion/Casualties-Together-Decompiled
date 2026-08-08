using System.Collections.Generic;
using LiteNetLib.Utils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public abstract class BaseCoolSyncSubSystem
{
	public bool ack = true;

	public float SEND_FREQUENCY = 1f / 30f;

	public double last_send_time;

	public bool im_in_the_thick_of_it_everybody_knows_they_know_me_where_it_snows_i_skied_in_and_they_froze_i_dont_know_no_nothin_bout_no_ice_im_just_cold_40somethin_milli_subs_or_so_ive_been_told__im_in_my_prime_and_this_aint_even_final_form_they_knocked_me_down_but_still_my_feet_they_find_the_floor_i_went_from_livin_rooms_straight_out_to_soldout_tours_lifes_a_fight_but_trust_im_ready_for_the_war__whoaohoh_this_is_how_the_story_goes_whoaohoh_i_guess_this_is_how_the_story_goes__im_in_the_thick_of_it_everybody_knows_they_know_me_where_it_snows_i_skied_in_and_they_froze_i_dont_know_no_nothin_bout_no_ice_im_just_cold_40somethin_milli_subs_or_so_ive_been_told__from_the_screen_to_the_ring_to_the_pen_to_the_king_wheres_my_crown_thats_my_bling_always_drama_when_i_ring_see_i_believe_that_if_i_see_it_in_my_heart_smash_through_the_ceilin_cause_im_reachin_for_the_stars__whoaohoh_this_is_how_the_story_goes_whoaohoh_i_guess_this_is_how_the_story_goes;

	public int last_sent_packet_size;

	public int last_sent_packet_size_compressed;

	public bool CleanupAndUnregisterOnLevelChange;

	public byte syncsystemid { get; protected set; }

	public virtual void Client_Receive(NetDataReader reader)
	{
	}

	public virtual void Server_ReceiveAck(NetPlayer plr, NetDataReader reader)
	{
	}

	public virtual void Update()
	{
	}

	public virtual bool Server_Update()
	{
		double realtimeSinceStartupAsDouble = Time.realtimeSinceStartupAsDouble;
		if (realtimeSinceStartupAsDouble - last_send_time > (double)SEND_FREQUENCY)
		{
			last_send_time = realtimeSinceStartupAsDouble;
		}
		return false;
	}

	protected static bool IsRollNewer(ushort n, ushort old)
	{
		return (short)(n - old) > 0;
	}

	public BaseCoolSyncSubSystem(byte systemid)
	{
		syncsystemid = systemid;
	}

	public virtual void Cleanup()
	{
		ClearAndResetEverything();
	}

	public virtual void ClearAndResetEverything()
	{
	}

	protected static byte[] BoolListToByteBitset(List<bool> bits)
	{
		byte[] array = new byte[(bits.Count + 7) / 8];
		for (int i = 0; i < bits.Count; i++)
		{
			if (bits[i])
			{
				array[i / 8] |= (byte)(1 << i % 8);
			}
		}
		return array;
	}
}
