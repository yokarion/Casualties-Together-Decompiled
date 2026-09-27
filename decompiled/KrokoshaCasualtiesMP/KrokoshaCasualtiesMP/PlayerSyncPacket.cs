using System;
using System.Collections;
using System.Collections.Generic;
using LiteNetLib.Utils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public struct PlayerSyncPacket : IDeltaAutoSync<NetPlayer>, IDeltaPacketBase
{
	public Vector2 camerapos;

	public bool unchipped;

	public AnyObjectNetId minigame_targetobject;

	public knetid minigame_currentItem_syncid;

	public ushort minigame_session;

	public bool minigame_lmb_down;

	public byte minigame_current_type;

	public Vector2 minigame_handpos;

	public Vector2 minigame_mousepos;

	public byte minigame_handsprite;

	public bool is_in_cmd;

	public bool is_alttab;

	public bool is_chatting;

	public bool is_crafting;

	public bool is_trading;

	public ushort woundViewTargetNetBodyId;

	public bool server_mute_vc;

	public bool server_mute_tc;

	public static int bitset_size = 15;

	public void SetDefault()
	{
	}

	public void AutoSerialize(knetid netId, NetPlayer real_obj)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)real_obj == (Object)null)
		{
			throw new Exception(":(");
		}
		minigame_targetobject = real_obj.minigame_targetobject;
		minigame_currentItem_syncid = real_obj.minigame_currentItem_syncid;
		minigame_session = real_obj.minigame_session;
		minigame_lmb_down = real_obj.minigame_lmb_down;
		minigame_current_type = real_obj.minigame_current_type;
		minigame_handpos = real_obj.minigame_handpos;
		minigame_mousepos = real_obj.minigame_mousepos;
		minigame_handsprite = real_obj.minigame_handsprite;
		is_in_cmd = real_obj.is_in_cmd;
		is_alttab = real_obj.is_alttab;
		is_chatting = real_obj.is_chatting;
		is_crafting = real_obj.is_crafting;
		is_trading = real_obj.is_trading;
		woundViewTargetNetBodyId = real_obj.woundViewTargetNetBodyId;
		server_mute_vc = real_obj.server_mute_vc;
		server_mute_tc = real_obj.server_mute_tc;
	}

	public void AutoDeserialize(knetid netId, NetPlayer real_obj)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		real_obj.minigame_targetobject = minigame_targetobject;
		real_obj.minigame_currentItem_syncid = minigame_currentItem_syncid;
		real_obj.minigame_session = minigame_session;
		real_obj.minigame_lmb_down = minigame_lmb_down;
		real_obj.minigame_current_type = minigame_current_type;
		real_obj.minigame_handpos = minigame_handpos;
		real_obj.minigame_mousepos = minigame_mousepos;
		real_obj.minigame_handsprite = minigame_handsprite;
		real_obj.is_in_cmd = is_in_cmd;
		real_obj.is_alttab = is_alttab;
		real_obj.is_chatting = is_chatting;
		real_obj.is_crafting = is_crafting;
		real_obj.is_trading = is_trading;
		real_obj.woundViewTargetNetBodyId = woundViewTargetNetBodyId;
		real_obj.server_mute_vc = server_mute_vc;
		real_obj.server_mute_tc = server_mute_tc;
	}

	public void Write(NetDataWriter writer, List<bool> pack_bools, IDeltaPacketBase old)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		PlayerSyncPacket obj = (PlayerSyncPacket)(object)old;
		bool flag = false;
		flag = obj.camerapos != camerapos;
		pack_bools.Add(flag);
		if (flag)
		{
			writer.Put(camerapos);
		}
		pack_bools.Add(unchipped);
		flag = obj.minigame_targetobject != minigame_targetobject;
		pack_bools.Add(flag);
		if (flag)
		{
			writer.Put(minigame_targetobject);
		}
		flag = (ushort)obj.minigame_currentItem_syncid != (ushort)minigame_currentItem_syncid;
		pack_bools.Add(flag);
		if (flag)
		{
			writer.Put((ushort)minigame_currentItem_syncid);
		}
		writer.Put(minigame_session);
		pack_bools.Add(minigame_lmb_down);
		writer.Put(minigame_current_type);
		flag = obj.minigame_handpos != minigame_handpos;
		pack_bools.Add(flag);
		if (flag)
		{
			writer.Put(minigame_handpos);
		}
		flag = obj.minigame_mousepos != minigame_mousepos;
		pack_bools.Add(flag);
		if (flag)
		{
			writer.Put(minigame_mousepos);
		}
		flag = obj.minigame_handsprite != minigame_handsprite;
		pack_bools.Add(flag);
		if (flag)
		{
			writer.Put(minigame_handsprite);
		}
		pack_bools.Add(is_in_cmd);
		pack_bools.Add(is_alttab);
		pack_bools.Add(is_chatting);
		pack_bools.Add(is_crafting);
		pack_bools.Add(is_trading);
		writer.Put(woundViewTargetNetBodyId);
		pack_bools.Add(server_mute_vc);
		pack_bools.Add(server_mute_tc);
	}

	public void Read(NetDataReader reader, BitArray bitset)
	{
		if (bitset[0])
		{
			reader.Get(out camerapos);
		}
		unchipped = bitset[1];
		if (bitset[2])
		{
			reader.Get(out minigame_targetobject);
		}
		if (bitset[3])
		{
			reader.Get(out minigame_currentItem_syncid);
		}
		reader.Get(ref minigame_session);
		minigame_lmb_down = bitset[4];
		reader.Get(ref minigame_current_type);
		if (bitset[5])
		{
			reader.Get(out minigame_handpos);
		}
		if (bitset[6])
		{
			reader.Get(out minigame_mousepos);
		}
		if (bitset[7])
		{
			reader.Get(ref minigame_handsprite);
		}
		is_in_cmd = bitset[8];
		is_alttab = bitset[9];
		is_chatting = bitset[10];
		is_crafting = bitset[11];
		is_trading = bitset[12];
		reader.Get(ref woundViewTargetNetBodyId);
		server_mute_vc = bitset[13];
		server_mute_tc = bitset[14];
	}
}
