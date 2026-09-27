using System.Collections;
using System.Collections.Generic;
using KrokoshaCasualtiesUtils;
using LiteNetLib.Utils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public struct NetBodySyncPacket : INetSerializeByMemcpy, IDeltaPacketBase
{
	public bool is_player = false;

	[AlwaysSync]
	public knetid plrid = default(knetid);

	public bool standing = false;

	public bool crouching = false;

	public Vector2_4byte_512 pos = default(Vector2_4byte_512);

	public Vector2_4byte_512 targetLookPos = default(Vector2_4byte_512);

	[AlwaysSync]
	public Compressed11Vec2 compressedmoveDir = default(Compressed11Vec2);

	public Vector2_4byte_200 velocity = default(Vector2_4byte_200);

	private byte compressednum4 = 0;

	public bool is_piggyback = false;

	public knetid piggyback = default(knetid);

	public static int bitset_size = 9;

	public Vector2 moveDir
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return compressedmoveDir;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			compressedmoveDir = (Compressed11Vec2)value;
		}
	}

	public float standLerpTime
	{
		get
		{
			return (float)(int)compressednum4 / 255f;
		}
		set
		{
			compressednum4 = (byte)(Mathf.Clamp(value, 0f, 1f) * 255f);
		}
	}

	public void SetDefault()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		standing = true;
		targetLookPos = (Vector2_4byte_512)new Vector2(600f, 0f);
		compressednum4 = byte.MaxValue;
	}

	public void WritePiggyback(NetBody b)
	{
		is_piggyback = (Object)(object)b.piggybacking_on != (Object)null;
		if (is_piggyback)
		{
			piggyback = b.piggybacking_on.netId;
		}
	}

	public NetBodySyncPacket(NetBody pb)
	{
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		WritePiggyback(pb);
		Body body = pb.body;
		standing = body.standing && !is_piggyback;
		if (!standing)
		{
			pos = body.limbs[1].rb.position;
		}
		else
		{
			pos = ((Component)body).transform.position;
		}
		targetLookPos = body.targetLookPos;
		if (body.conscious)
		{
			moveDir = body.moveDir;
		}
		else
		{
			moveDir = Vector2.zero;
		}
		crouching = body.crouching;
		if (body.standing)
		{
			velocity = body.rb.velocity;
		}
		else
		{
			velocity = body.GetUpperTorso().rb.velocity;
		}
		standLerpTime = body.standLerpTime;
		if (body.IsBodyLocal() && Util.IsInWoundView() && !WoundView.view.body.IsBodyLocal())
		{
			targetLookPos = ((Component)WoundView.view.body).transform.position;
		}
	}

	public void Apply(NetBody pb)
	{
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		Body body = pb.body;
		if (body.standing != standing)
		{
			if (standing || !((Behaviour)body.col).enabled)
			{
				is_piggyback = false;
				body.shock = Mathf.Min(body.shock, 9f);
				body.Stand(true);
			}
			else
			{
				body.Ragdoll();
			}
		}
		if (KrokoshaScavMultiplayer.is_client)
		{
			if (is_piggyback)
			{
				if (NetBody.TryGetNetBodyFromId(piggyback, out var nb) && (Object)(object)nb != (Object)(object)pb)
				{
					pb.StartPiggyback(nb, check_distance: false, force: true);
				}
			}
			else if ((Object)(object)pb.piggybacking_on != (Object)null && Util.IsWorldGenerated())
			{
				pb.StopPiggyback();
			}
		}
		else
		{
			if ((Object)(object)pb.piggybacking_on != (Object)null != is_piggyback && (Object)(object)pb.piggybacking_on != (Object)null && !is_piggyback && !KM.dist2dsqrcheck((Vector2)pos, pb.piggybacking_on.pos, 5f))
			{
				if (log.verbose)
				{
					log.warn($"{pb} is piggybacking but pos is too far, missed unpiggyback request?");
				}
				pb.Server_RemindPlayersCurrentState(keep_velocity: true, reliable: false);
			}
			WritePiggyback(pb);
		}
		if ((Object)(object)pb.piggybacking_on == (Object)null)
		{
			pb.SetBodyPosition(pos);
			if (body.IsBodyLocal() && body.alive)
			{
				ConsoleScript.instance.noClipPos = pos;
				if (Util.IsGeneratingWorld() && !Con._DEV_FREECAM)
				{
					((Component)PlayerCamera.main).transform.position = ((Component)body).transform.position;
				}
			}
		}
		body.standLerpTime = standLerpTime;
		body.targetLookPos = targetLookPos;
		body.crouching = crouching;
		Vector2 val = velocity;
		if (body.standing)
		{
			body.rb.velocity = val;
		}
		else
		{
			body.SetVelocity(val);
		}
		if ((Object)(object)body.currentClimbable != (Object)null && !body.grounded)
		{
			ClimbableGrabInfo grabInfo = body.currentClimbable.GetGrabInfo(Vector2.op_Implicit(((Component)body).transform.position));
			if (grabInfo.distanceToPlayer < 1f)
			{
				float num = val.y * 0.1f;
				float num2 = body.climbableProgress + num;
				if (num2 >= body.currentClimbable.totalLength)
				{
					body.climbableProgress = body.currentClimbable.totalLength - 0.0001f;
					body.climbVelocity = 0f;
				}
				else if (num2 <= 0f)
				{
					body.climbVelocity = 0f;
				}
				else
				{
					body.climbVelocity = val.y;
					body.climbableProgress = Mathf.Clamp01(grabInfo.pathDistance);
				}
			}
			else if (grabInfo.distanceToPlayer > 2f)
			{
				body.StopClimbing();
			}
		}
		if (!body.IsBodyLocal() && body.sleeping && body.moveDir != Vector2.zero)
		{
			body.sleeping = false;
		}
		pb.last_sync_packet = this;
	}

	public void Write(NetDataWriter writer, List<bool> pack_bools, IDeltaPacketBase old)
	{
		NetBodySyncPacket obj = (NetBodySyncPacket)(object)old;
		bool flag = false;
		pack_bools.Add(is_player);
		writer.Put((ushort)plrid);
		pack_bools.Add(standing);
		pack_bools.Add(crouching);
		flag = obj.pos != pos;
		pack_bools.Add(flag);
		if (flag)
		{
			writer.Put(pos);
		}
		flag = obj.targetLookPos != targetLookPos;
		pack_bools.Add(flag);
		if (flag)
		{
			writer.Put(targetLookPos);
		}
		writer.Put(compressedmoveDir);
		flag = obj.velocity != velocity;
		pack_bools.Add(flag);
		if (flag)
		{
			writer.Put(velocity);
		}
		flag = obj.compressednum4 != compressednum4;
		pack_bools.Add(flag);
		if (flag)
		{
			writer.Put(compressednum4);
		}
		pack_bools.Add(is_piggyback);
		flag = (ushort)obj.piggyback != (ushort)piggyback;
		pack_bools.Add(flag);
		if (flag)
		{
			writer.Put((ushort)piggyback);
		}
	}

	public void Read(NetDataReader reader, BitArray bitset)
	{
		is_player = bitset[0];
		reader.Get(out plrid);
		standing = bitset[1];
		crouching = bitset[2];
		if (bitset[3])
		{
			reader.Get(out pos);
		}
		if (bitset[4])
		{
			reader.Get(out targetLookPos);
		}
		reader.Get(out compressedmoveDir);
		if (bitset[5])
		{
			reader.Get(out velocity);
		}
		if (bitset[6])
		{
			reader.Get(ref compressednum4);
		}
		is_piggyback = bitset[7];
		if (bitset[8])
		{
			reader.Get(out piggyback);
		}
	}
}
