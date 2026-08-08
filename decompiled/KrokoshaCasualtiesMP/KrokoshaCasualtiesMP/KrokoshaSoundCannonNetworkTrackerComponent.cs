using System.Collections.Generic;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public class KrokoshaSoundCannonNetworkTrackerComponent : KrokoshaNetworkComponentTracker<SoundCannon>
{
	private bool announced_charge;

	public bool charging;

	public bool spent;

	public float checkTime = 1f;

	public float maxDistance = 50f;

	public float chargeTime;

	protected override void TrackerAwake()
	{
	}

	public static bool CanSeePos(Transform me, Vector2 to)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		RaycastHit2D[] array = Physics2D.LinecastAll(Vector2.op_Implicit(((Component)me).transform.position), to, LayerMask.GetMask(new string[1] { "Ground" }));
		bool flag = false;
		RaycastHit2D[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			RaycastHit2D val = array2[i];
			if ((Object)(object)((RaycastHit2D)(ref val)).transform != (Object)(object)((Component)me).transform)
			{
				flag = true;
				break;
			}
		}
		return !flag;
	}

	public static NetBody GetClosestVisibleBody(Transform me)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		NetBody netBody = null;
		float num = 1E+11f;
		NetBody netBody2 = null;
		float num2 = 1E+10f;
		foreach (KeyValuePair<Body, NetPlayer> item in NetPlayer.BodyToPlayerDict)
		{
			Vector2 b = item.Value.playerbody.GetHeadPos();
			float num3 = KM.dist2dsqr(Vector2.op_Implicit(me.position), in b);
			if (num3 < num)
			{
				num = num3;
				netBody = item.Value.playerbody;
			}
			if (num3 < num2 && CanSeePos(me, b))
			{
				num2 = num3;
				netBody2 = item.Value.playerbody;
			}
		}
		return netBody2 ?? netBody;
	}

	private void Update()
	{
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0655: Unknown result type (might be due to invalid IL or missing references)
		//IL_065a: Unknown result type (might be due to invalid IL or missing references)
		//IL_047f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0484: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0569: Unknown result type (might be due to invalid IL or missing references)
		//IL_056e: Unknown result type (might be due to invalid IL or missing references)
		if (spent || !Object.op_Implicit((Object)(object)WorldGeneration.world) || WorldGeneration.world.generatingWorld)
		{
			return;
		}
		if (!KrokoshaScavMultiplayer.network_system_is_running)
		{
			Object.Destroy((Object)(object)this);
		}
		checkTime -= Time.deltaTime;
		if (checkTime < 0f)
		{
			checkTime = (KrokoshaScavMultiplayer.is_client ? 0.4f : 0.1f);
			if (!NetObjectRegistry.TryGetSyncInfo(((Component)this).gameObject, out var _) && NetPlayer.GetDistanceToNearestLivingPlayer(Vector2.op_Implicit(((Component)this).transform.position)).Item2 < 128f)
			{
				NetObjectRegistry.NewGO(((Component)this).gameObject);
				return;
			}
			if (!charging && !spent)
			{
				foreach (KeyValuePair<Body, NetPlayer> item in NetPlayer.BodyToPlayerDict)
				{
					Vector3 position = ((Component)item.Key.limbs[0]).transform.position;
					if (KM.dist2dsqrcheck(Vector2.op_Implicit(((Component)this).transform.position), Vector2.op_Implicit(position), maxDistance) && CanSeePos(((Component)this).transform, Vector2.op_Implicit(position)))
					{
						charging = true;
						break;
					}
				}
			}
		}
		if (!NetObjectRegistry.TryGetSyncInfo(((Component)this).gameObject, out var si2) || !charging)
		{
			return;
		}
		if (!announced_charge)
		{
			Sound.Play("sonarcharge", Vector2.op_Implicit(((Component)this).transform.position), true, false, (Transform)null, 1f, 1f, false, false);
			if (KrokoshaScavMultiplayer.network_system_is_running && !KrokoshaScavMultiplayer.is_client)
			{
				KrokoshaScavMultiplayer.Server_SendRelayMessageToClients((ushort)10152, si2.syncId);
				NewCoolerObjectPacketWriteReadSystem.inst.Server_QueueForceSyncForAll(si2.syncId);
			}
			announced_charge = true;
		}
		chargeTime += Time.deltaTime;
		NetBody closestVisibleBody = GetClosestVisibleBody(((Component)this).transform);
		Vector2.op_Implicit(((Component)closestVisibleBody.body).transform.position);
		float magnitude;
		Vector2 val = -KM.normal(closestVisibleBody.GetHeadPos(), Vector2.op_Implicit(((Component)this).transform.position), out magnitude);
		((Component)this).transform.GetChild(0).eulerAngles = new Vector3(0f, 0f, Mathf.LerpAngle(((Component)this).transform.GetChild(0).eulerAngles.z, Vector2.SignedAngle(Vector2.up, val), Time.deltaTime * 4f));
		Vector3 up = ((Component)this).transform.GetChild(0).up;
		if (chargeTime < 5f)
		{
			return;
		}
		spent = true;
		charging = false;
		base.og.spent = true;
		foreach (NetPlayer allLivingPlayer in NetPlayer.AllLivingPlayers)
		{
			Body body = allLivingPlayer.body;
			Vector2 from = allLivingPlayer.playerbody.GetHeadPos();
			float magnitude2;
			Vector2 val2 = -KM.normal(in from, Vector2.op_Implicit(((Component)this).transform.position), out magnitude2);
			float num = Vector2.Dot(Vector2.op_Implicit(up), val2);
			bool flag = FluidManager.main.HasLiquid(WorldGeneration.world.WorldToBlockPos(Vector2.op_Implicit(((Component)this).transform.position))) == body.inWater;
			if (CanSeePos(((Component)this).transform, from) && num > 0.55f && flag)
			{
				if (body.bodyAffect.wasWater)
				{
					if (allLivingPlayer.is_local)
					{
						Sound.Play("sonarmegaouchfixed", Vector2.op_Implicit(((Component)this).transform.position), true, false, (Transform)null, 1f, 1f, false, true);
					}
					body.hearingLoss = 100f;
					body.internalBleeding += 100f;
					body.happiness -= 10f;
					body.shock = 100f;
					body.limbs[0].pain = 100f;
					body.brainHealth -= 20f;
					if (allLivingPlayer.is_local)
					{
						PlayerCamera.main.shaker.Shake(3000f);
						PlayerCamera.main.bonusAbber = -70f;
						PlayerCamera main = PlayerCamera.main;
						main.globalMuteTime += 120f;
					}
				}
				else
				{
					if (allLivingPlayer.is_local)
					{
						Sound.Play("sonarouch", Vector2.op_Implicit(((Component)this).transform.position), true, false, (Transform)null, 1f, 1f, false, true);
					}
					body.hearingLoss += 60f;
					body.internalBleeding += Random.Range(20f, 30f);
					body.happiness -= 2f;
					body.shock = 65f;
					body.limbs[0].pain = 80f;
					body.brainHealth -= 5f;
					if (allLivingPlayer.is_local)
					{
						PlayerCamera.main.shaker.Shake(1000f);
						PlayerCamera.main.bonusAbber = -30f;
						PlayerCamera main2 = PlayerCamera.main;
						main2.globalMuteTime += 30f;
					}
				}
				if (allLivingPlayer.is_local)
				{
					Sound.Play("tinnitus", Vector2.op_Implicit(((Component)this).transform.position), true, false, (Transform)null, 1f, 1f, false, true);
				}
				body.consciousness -= 69f;
				body.eyeCloseTime = 10f;
				body.eyeScareTime = 25f;
				body.adrenaline += 100f;
				body.respiratoryRate = 0f;
				Limb[] limbs = body.limbs;
				foreach (Limb obj in limbs)
				{
					obj.muscleHealth -= Random.Range(20f, 30f);
					obj.pain += 40f;
				}
			}
			else
			{
				body.consciousness -= 30f;
				body.eyeScareTime = 4f;
				if (allLivingPlayer.is_local)
				{
					Sound.Play("sonarouchblocked", Vector2.op_Implicit(((Component)this).transform.position), true, false, (Transform)null, 1f, 1f, false, true);
					PlayerCamera.main.shaker.Shake(200f);
					PlayerCamera main3 = PlayerCamera.main;
					main3.globalMuteTime += 3f;
				}
			}
		}
	}
}
