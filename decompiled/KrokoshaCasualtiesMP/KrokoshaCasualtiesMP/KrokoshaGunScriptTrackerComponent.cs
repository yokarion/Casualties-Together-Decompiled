using System;
using LiteNetLib.Utils;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public class KrokoshaGunScriptTrackerComponent : MonoBehaviour
{
	public NetBody pbody;

	public bool override_shoot_trajectory;

	public Vector2 override_shoot_trajectory_origin = Vector2.left;

	public Vector2 override_shoot_trajectory_direction = Vector2.left;

	public bool send_racked_state;

	private bool _thing_to_emulate_firingPinStruck_only_here;

	public GunScript gun { get; private set; }

	public int shotsPerFire { get; private set; }

	public SyncInfo si => ((Component)this).GetComponent<KrokoshaScavMultiGameObjectNetworkTracker>()?.syncinfo;

	public Body body
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

	public bool is_equipped
	{
		get
		{
			if ((Object)(object)body == (Object)null)
			{
				return false;
			}
			Item item = body.GetItem(body.handSlot);
			return (Object)(object)((item != null) ? ((Component)item).transform : null) == (Object)(object)((Component)this).transform;
		}
	}

	public float gasTime
	{
		get
		{
			return gun.gasTime;
		}
		set
		{
			gun.gasTime = value;
		}
	}

	public Body GetBody()
	{
		return body;
	}

	public bool IsShotgun()
	{
		return shotsPerFire > 2;
	}

	private void Awake()
	{
		gun = ((Component)this).GetComponent<GunScript>();
		shotsPerFire = gun.shotsPerFire;
		gun.shotsPerFire = 0;
		ScanForBody();
		((MonoBehaviour)this).InvokeRepeating("SlowUpdate", 1f, 0.1f);
	}

	public void ScanForBody()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		NetBody componentInParent = ((Component)gun).GetComponentInParent<NetBody>();
		if ((Object)(object)componentInParent != (Object)null)
		{
			pbody = componentInParent;
			body = componentInParent.body;
			return;
		}
		NetPlayer item = NetPlayer.GetDistanceToNearestLivingPlayer(Vector2.op_Implicit(((Component)this).transform.position)).Item1;
		if ((Object)(object)item != (Object)null)
		{
			pbody = item.playerbody;
			body = item.body;
		}
		else
		{
			body = PlayerCamera.main.body;
			pbody = ((Component)body).GetComponent<NetBody>();
		}
	}

	private void SlowUpdate()
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		KrokoshaScavMultiGameObjectNetworkTracker krokoshaScavMultiGameObjectNetworkTracker = default(KrokoshaScavMultiGameObjectNetworkTracker);
		if (!((Component)this).TryGetComponent<KrokoshaScavMultiGameObjectNetworkTracker>(ref krokoshaScavMultiGameObjectNetworkTracker) || !krokoshaScavMultiGameObjectNetworkTracker.is_super_close || !is_equipped || !KrokoshaScavMultiplayer.rules.PVP)
		{
			return;
		}
		float num = (body.isRight ? 1f : (-1f));
		Vector3 val = ((Component)gun).transform.right * num;
		RaycastHit2D[] array = Physics2D.RaycastAll(Vector2.op_Implicit(gun.barrel.position), Vector2.op_Implicit(val), 4f);
		NetBody netBody = default(NetBody);
		for (int i = 0; i < array.Length; i++)
		{
			RaycastHit2D val2 = array[i];
			Limb val3 = null;
			if (!((Component)((RaycastHit2D)(ref val2)).collider).TryGetComponent<Limb>(ref val3) || !val3.isVital)
			{
				val3 = null;
			}
			if ((Object)(object)val3 == (Object)null)
			{
				if (((Component)((RaycastHit2D)(ref val2)).collider).TryGetComponent<NetBody>(ref netBody))
				{
					Vector2 val4 = KM.normal(Vector2.op_Implicit(gun.barrel.position), netBody.GetHeadPos());
					if ((double)Vector2.Dot(Vector2.op_Implicit(val), val4) > 0.9)
					{
						val3 = netBody.head;
					}
				}
				if ((Object)(object)val3 == (Object)null)
				{
					continue;
				}
			}
			if ((Object)(object)val3.body != (Object)(object)body && val3.body.happiness > -40f)
			{
				val3.body.eyeScareTime = Math.Max(val3.body.eyeScareTime, 0.3f);
			}
		}
	}

	private void FixedUpdate()
	{
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)pbody != (Object)null && is_equipped && (Object)(object)pbody.piggybacking_on != (Object)null && !body.standing)
		{
			float num = (body.isRight ? 1f : (-1f));
			Limb limb = body.slots[body.handSlot].limb;
			Vector2 val = Vector2.op_Implicit(body.targetLookPos) - limb.rb.position;
			if (((Vector2)(ref val)).sqrMagnitude > 2f)
			{
				float num2 = Mathf.Atan2(val.x, 0f - val.y) * 57.29578f;
				float num3 = num2 - limb.rb.rotation;
				float num4 = 400f * Time.fixedDeltaTime * limb.totalForce * Mathf.Clamp01(1f - gasTime * 4f);
				float num5 = Mathf.MoveTowardsAngle(limb.rb.rotation, num2, num4 * (30f + Mathf.Abs(num3)));
				limb.rb.MoveRotation(num5);
				Limb val2 = limb.connectedLimbs[0];
				Limb val3 = val2.connectedLimbs[0];
				val2.rb.MoveRotation(Mathf.MoveTowardsAngle(val2.rb.rotation, num2 + 20f * num, num4 * 8f));
				val3.rb.MoveRotation(Mathf.MoveTowardsAngle(val3.rb.rotation, num2 - 20f * num, num4 * 12f));
				limb.rb.MoveRotation(num5);
			}
		}
	}

	public void ApplyRecoil()
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)pbody?.piggybacking_on != (Object)null)
		{
			float num = (body.isRight ? 1f : (-1f));
			Limb limb = body.slots[body.handSlot].limb;
			Rigidbody2D rb = limb.rb;
			rb.velocity += Vector2.op_Implicit(((Component)limb).transform.right) * (num * gun.knockBack * 8f);
			Rigidbody2D rb2 = limb.rb;
			rb2.rotation += num * 0.1f;
		}
	}

	private void Update()
	{
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)pbody) && pbody.is_local)
		{
			if (send_racked_state && gun.racked == gun.lastRacked)
			{
				KrokoshaScavMultiplayer.Client_SendSimpleMessageToServer((ushort)10108, gun.racked, true);
				send_racked_state = false;
			}
			if (gun.firingPinStruck && !_thing_to_emulate_firingPinStruck_only_here)
			{
				_thing_to_emulate_firingPinStruck_only_here = true;
				if (!(gasTime > 0f))
				{
					if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
					{
						DebugHelp.OnNetEvent(si.position, "C sending: GunEmptyClick ");
					}
					KrokoshaScavMultiplayer.Client_SendSimpleMessageToServer((ushort)10107, (ushort)si.syncId, false);
				}
			}
			else
			{
				_thing_to_emulate_firingPinStruck_only_here = gun.firingPinStruck;
			}
		}
		ScanForBody();
	}

	[ServerReceiver(10107)]
	private static void Server_GunEmptyClick(knetid clientId, ref NetDataReader reader)
	{
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out knetid result);
		if (NetPlayer.TryGetNetPlayerAndNetBodyFromClientId(clientId, out var _, out var pb) && pb.body.conscious && ItemSync.TryGetItemSyncInfo(result, out var syncInfo) && syncInfo.IsGun() && ItemSync.CheckIfBodyReachThisItem(syncInfo, pb.body))
		{
			KrokoshaGunScriptTrackerComponent orAddComponent = ComponentHolderProtocol.GetOrAddComponent<KrokoshaGunScriptTrackerComponent>((Object)(object)syncInfo.go);
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(syncInfo.position, "S: GunEmptyClick " + ServerMain.GetPlayerFullDebugString(clientId));
			}
			ServerMain.Server_AnnounceSound(Vector2.op_Implicit(((Component)orAddComponent).transform.position), "guntrigger", ServerMain.GetListOfClientIdsExceptThis(clientId));
		}
	}
}
