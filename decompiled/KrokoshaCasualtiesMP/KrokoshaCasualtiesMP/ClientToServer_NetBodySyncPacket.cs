using KrokoshaCasualtiesUtils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public struct ClientToServer_NetBodySyncPacket : INetSerializeByMemcpy
{
	public bool is_player = false;

	public knetid plrid = default(knetid);

	public bool standing = false;

	public bool crouching = false;

	public Vector2 pos = default(Vector2);

	public Vector2 targetLookPos = default(Vector2);

	public Compressed11Vec2 compressedmoveDir = default(Compressed11Vec2);

	public Vector2_4byte_200 velocity = default(Vector2_4byte_200);

	private byte compressednum4 = 0;

	public bool is_piggyback = false;

	public knetid piggyback = default(knetid);

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

	public void WritePiggyback(NetBody b)
	{
		is_piggyback = (Object)(object)b.piggybacking_on != (Object)null;
		if (is_piggyback)
		{
			piggyback = b.piggybacking_on.netId;
		}
	}

	public bool IncludeRagData()
	{
		if (!standing)
		{
			return !is_piggyback;
		}
		return false;
	}

	public ClientToServer_NetBodySyncPacket(NetBody pb)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		WritePiggyback(pb);
		Body body = pb.body;
		standing = body.standing && !is_piggyback;
		if (!standing)
		{
			pos = body.limbs[1].rb.position;
		}
		else
		{
			pos = Vector2.op_Implicit(((Component)body).transform.position);
		}
		targetLookPos = Vector2.op_Implicit(body.targetLookPos);
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
			targetLookPos = Vector2.op_Implicit(((Component)WoundView.view.body).transform.position);
		}
	}

	public void Apply(NetBody nb)
	{
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		Body body = nb.body;
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
				if (NetBody.TryGetNetBodyFromId(piggyback, out var nb2) && (Object)(object)nb2 != (Object)(object)nb)
				{
					nb.StartPiggyback(nb2, check_distance: false, force: true);
				}
			}
			else if ((Object)(object)nb.piggybacking_on != (Object)null && Util.IsWorldGenerated())
			{
				nb.StopPiggyback();
			}
		}
		else
		{
			if ((Object)(object)nb.piggybacking_on != (Object)null != is_piggyback && (Object)(object)nb.piggybacking_on != (Object)null && !is_piggyback && !KM.dist2dsqrcheck(in pos, nb.piggybacking_on.pos, 5f))
			{
				if (log.verbose)
				{
					log.warn($"{nb} is piggybacking but pos is too far, missed unpiggyback request?");
				}
				nb.Server_RemindPlayersCurrentState(keep_velocity: true, reliable: false);
			}
			WritePiggyback(nb);
		}
		if ((Object)(object)nb.piggybacking_on == (Object)null)
		{
			nb.SetBodyPosition(pos);
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
		body.targetLookPos = Vector2.op_Implicit(targetLookPos);
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
		nb.last_sync_packet = new NetBodySyncPacket(nb);
		nb.last_sync_packet.moveDir = moveDir;
	}
}
