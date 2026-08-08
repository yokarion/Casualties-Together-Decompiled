using KrokoshaCasualtiesUtils;
using LiteNetLib;
using LiteNetLib.Utils;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public class ScavTraps : KrokoshaScavSingleton
{
	public class DamagingCrateCooldownThing : MonoBehaviour
	{
		public bool stalagtite_dropped;

		public double last_hit = -10000000.0;
	}

	public static bool DamagingCrate_OnCollisionEnter2D_StolenWithFirstCheck(object ddddddddd, Limb limb, Vector2 prevFrameSpeed)
	{
		DamagingCrate val = (DamagingCrate)((ddddddddd is DamagingCrate) ? ddddddddd : null);
		if (!Util.IsBodyLocal(limb.body))
		{
			limb.body.Ragdoll();
			return true;
		}
		if (Net.running && !NetObjectRegistry.TryGetSyncInfoOrRegister((Component)(object)val, out var _) && KrokoshaScavMultiplayer.is_client)
		{
			return true;
		}
		DamagingCrateCooldownThing orAddComponent = ComponentHolderProtocol.GetOrAddComponent<DamagingCrateCooldownThing>((Object)(object)val);
		if (Time.timeAsDouble - orAddComponent.last_hit < 0.5)
		{
			return true;
		}
		orAddComponent.last_hit = Time.timeAsDouble;
		float magnitude = ((Vector2)(ref prevFrameSpeed)).magnitude;
		DamagingCrate_OnCollisionEnter2D_TheDamagingPartStolen(val, limb, magnitude);
		return true;
	}

	public static void DamagingCrate_OnCollisionEnter2D_TheDamagingPartStolen(DamagingCrate dmgcrate, Limb limb, float prevspeed)
	{
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		bool flag = Util.IsBodyLocal(limb.body);
		if (dmgcrate.type == 1)
		{
			if (limb.isHead)
			{
				limb.body.consciousness = 20f / limb.GetArmorReduction();
			}
			limb.muscleHealth -= prevspeed * 0.75f / limb.GetArmorReduction();
			limb.skinHealth -= prevspeed / limb.GetArmorReduction();
			limb.bleedAmount += prevspeed * 0.1f / limb.GetArmorReduction();
			limb.DamageWearables(0.1f);
		}
		else
		{
			if (limb.isHead)
			{
				limb.body.consciousness = 0f / limb.GetArmorReduction();
			}
			limb.muscleHealth -= prevspeed * 1.25f / limb.GetArmorReduction();
			limb.skinHealth -= prevspeed * 0.6f / limb.GetArmorReduction();
			limb.bleedAmount += prevspeed * 0.05f / limb.GetArmorReduction();
			if (Random.value < 0.5f && prevspeed > 25f)
			{
				limb.Dislocate();
			}
			if (Random.value < 0.5f && prevspeed > 45f)
			{
				limb.BreakBone();
			}
			limb.DamageWearables(0.25f);
		}
		Sound.Play(dmgcrate.stalactiteHitSound, Vector2.op_Implicit(((Component)dmgcrate).transform.position), flag, true, (Transform)null, 1f, 1f, false, false);
		if (KrokoshaScavMultiplayer.network_system_is_running)
		{
			NetBody netBody = default(NetBody);
			if (KrokoshaScavMultiplayer.is_client)
			{
				if (flag && NetObjectRegistry.TryGetSyncInfoOrRegister((Component)(object)dmgcrate, out var si))
				{
					if (limb.TryGetNetBody(out var nb))
					{
						nb.SetNetHealthSyncIgnoreTime(0.05f);
					}
					NetDataWriter writer = Net.CreateWriter(10147);
					writer.Put((ushort)si.syncId);
					writer.Put(prevspeed);
					writer.Put(Util.GetLimbIndex(limb));
					Net.Client_Send((DeliveryMethod)0, in writer);
				}
			}
			else if (((Component)limb.body).TryGetComponent<NetBody>(ref netBody))
			{
				ServerMain.Server_AnnounceSound(Vector2.op_Implicit(((Component)dmgcrate).transform.position), dmgcrate.stalactiteHitSound, ServerMain.GetListOfClientIdsExceptThisAndHost(netBody.netId));
				StalactiteDropper val = default(StalactiteDropper);
				if (((Component)dmgcrate).TryGetComponent<StalactiteDropper>(ref val) && (int)((Component)dmgcrate).GetComponent<Rigidbody2D>().bodyType != 0)
				{
					val.Drop();
				}
			}
		}
		limb.body.Ragdoll();
	}

	[ServerReceiver(10147)]
	private static void ServerReceiver_ClientGotHitBy_DamagingCrate_OnCollisionEnter2D(knetid clientId, ref NetDataReader reader)
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out knetid result);
		float num = default(float);
		reader.Get(ref num);
		byte limbId = default(byte);
		reader.Get(ref limbId);
		DamagingCrate val = default(DamagingCrate);
		if (MedicalSync.TryGetNetBodyAndLimb(clientId, limbId, out var nb, out var limb) && nb.alive && num.IsFinite() && num > 4f && NetObjectRegistry.TryGetSyncInfo(result, out var si) && si.go.TryGetComponent<DamagingCrate>(ref val) && KM.dist2dsqrcheck(Vector2.op_Implicit(((Component)val).transform.position), Vector2.op_Implicit(((Component)limb).transform.position), 32f))
		{
			DamagingCrate_OnCollisionEnter2D_TheDamagingPartStolen(val, limb, Mathf.Abs(num));
		}
	}

	[ClientReceiver(10152, true)]
	private static void ClientReceiver__SoundCannonISCHARGING(knetid _, ref NetDataReader reader)
	{
		reader.Get(out knetid result);
		if (NetObjectRegistry.TryGetSyncInfo(result, out var si))
		{
			si.go.GetComponent<KrokoshaSoundCannonNetworkTrackerComponent>().charging = true;
		}
	}

	[ClientReceiver(10146, true)]
	private static void ClientReceiver__MineStepped(knetid _, ref NetDataReader reader)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out Vector2 result);
		Collider2D[] array = Physics2D.OverlapCircleAll(result, 2f);
		MineScript val = default(MineScript);
		for (int i = 0; i < array.Length; i++)
		{
			if (((Component)array[i]).TryGetComponent<MineScript>(ref val))
			{
				if (!val.pressed)
				{
					Sound.Play("mine", result, false, true, (Transform)null, 1f, 1f, false, false);
					((Component)val).GetComponent<SpriteRenderer>().sprite = val.pressedSprite;
					val.exploded = true;
				}
				break;
			}
		}
	}
}
