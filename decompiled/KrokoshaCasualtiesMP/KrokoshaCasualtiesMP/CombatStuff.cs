using System.Collections.Generic;
using System.Linq;
using KrokoshaCasualtiesUtils;
using LiteNetLib.Utils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

internal class CombatStuff : KrokoshaScavSingleton
{
	public static AttackInfo default_unarmed_attackinfo
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			AttackInfo val = new AttackInfo();
			val.damage = 30f;
			val.structuralDamage = 20f;
			val.distance = 4.5f;
			val.knockBack = 200f;
			val.cooldown = 0.2f;
			val.attackAnim = Resources.Load<GameObject>("ClawAnim");
			val.staminaUse = 1f;
			val.piercing = false;
			val.swingSounds = new string[4] { "BSSwing1", "BSSwing2", "BSSwing3", "BSSwing4" };
			val.volume = 0.3f;
			val.rotateAmount = 11.5f;
			val.unarmed = true;
			return val;
		}
	}

	private void Awake()
	{
	}

	public static bool CheckIfItsOkeyToDismemberThatLimb(Limb limb)
	{
		Util.GetLimbIndex(limb);
		if (limb.isHead && limb.body.IsDeadOrCriticallyDying())
		{
			return true;
		}
		if (Util.GetLimbIndex(limb) <= 2)
		{
			if (limb.body.alive)
			{
				return false;
			}
			if (!limb.isHead)
			{
				if (!limb.body.limbs[4].dismembered)
				{
					return false;
				}
				if (!limb.body.limbs[7].dismembered)
				{
					return false;
				}
				if (!limb.body.limbs[10].dismembered)
				{
					return false;
				}
				if (!limb.body.limbs[13].dismembered)
				{
					return false;
				}
				if (limb.isVital)
				{
					if (!limb.body.limbs[0].dismembered)
					{
						return false;
					}
				}
				else if (!limb.body.limbs[1].dismembered)
				{
					return false;
				}
			}
		}
		else if (limb.body.alive)
		{
			bool flag = false;
			for (int i = 9; i < 15; i++)
			{
				if (limb.body.limbs[i].dismembered)
				{
					flag = true;
					break;
				}
			}
			bool flag2 = false;
			for (int j = 3; j < 9; j++)
			{
				if (limb.body.limbs[j].dismembered)
				{
					flag2 = true;
					break;
				}
			}
			if (limb.isArm && flag2)
			{
				return false;
			}
			if (limb.isLegLimb && flag)
			{
				return false;
			}
		}
		return true;
	}

	public static Limb CannibalismGetFarthestLimbToBeEfficient(Limb limb)
	{
		byte limbIndex = Util.GetLimbIndex(limb);
		if (limb.isHead)
		{
			return limb;
		}
		if (limbIndex <= 2)
		{
			bool flag = false;
			for (int i = 3; i < limb.body.limbs.Count(); i++)
			{
				Limb val = limb.body.limbs[i];
				if ((i == 3 || i == 6 || i == 9 || i == 12) && !val.dismembered)
				{
					limb = val;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				if (limb.isVital)
				{
					if (!limb.body.limbs[0].dismembered)
					{
						return limb.body.limbs[0];
					}
				}
				else if (!limb.body.limbs[1].dismembered)
				{
					return limb.body.limbs[1];
				}
			}
		}
		else
		{
			int distanceToHeart = limb.distanceToHeart;
			Limb[] limbs = limb.body.limbs;
			foreach (Limb val2 in limbs)
			{
				if (val2.distanceToHeart > distanceToHeart && !val2.dismembered)
				{
					limb = val2;
					distanceToHeart = val2.distanceToHeart;
				}
			}
		}
		return limb;
	}

	public static void BetterDismember(Limb limb, bool do_checks_to_not_break_the_game = true)
	{
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		if (limb.dismembered || (do_checks_to_not_break_the_game && !CheckIfItsOkeyToDismemberThatLimb(limb)))
		{
			return;
		}
		NetBody netBody = default(NetBody);
		if (((Component)limb.body).TryGetComponent<NetBody>(ref netBody))
		{
			bool flag = limb.body.IsDeadOrCriticallyDying();
			foreach (Limb lowerLimb in TourniquetScript.GetLowerLimbs(limb, (List<Limb>)null))
			{
				lowerLimb.shrapnel = 0;
				if (lowerLimb.dismembered)
				{
					continue;
				}
				if (netBody.butchering_dropmeat_skip_counter > 0)
				{
					netBody.butchering_dropmeat_skip_counter = 0;
					continue;
				}
				if (Util.GetLimbIndex(lowerLimb) <= 2)
				{
					GameObject val = Utils.Create("internalorgans", Vector2.op_Implicit(((Component)lowerLimb).transform.position) + Vector2.up, 0f);
					if (flag)
					{
						val.AddComponent<FreshItemDrop>();
					}
					val.GetComponent<Item>().rb.velocity = lowerLimb.rb.velocity * 0.5f + Random.insideUnitCircle * 7f;
				}
				GameObject val2 = Utils.Create("experimentflesh", Vector2.op_Implicit(((Component)lowerLimb).transform.position) + Vector2.up, 0f);
				if (flag)
				{
					val2.AddComponent<FreshItemDrop>();
				}
				val2.GetComponent<Item>().rb.velocity = lowerLimb.rb.velocity * 0.5f + Random.insideUnitCircle * 7f;
				if (!limb.body.alive)
				{
					netBody.butchering_dropmeat_skip_counter++;
				}
			}
		}
		if (limb.isHead)
		{
			limb.body.brainHealth = 0f;
			limb.body.heartRate = 0f;
			limb.body.bothEyesGone = false;
			limb.body.eyeGone = false;
			limb.body.disfigured = false;
		}
		limb.Dismember();
	}

	public static bool ItemIsUsedForAttacking(Item item)
	{
		if (!item.Stats.HasTag("tool"))
		{
			return item.Stats.HasTag("gun");
		}
		return true;
	}

	[ServerReceiver(10108)]
	private static void ServerReceiver__GunRack(knetid clientId, ref NetDataReader reader)
	{
		bool racked = default(bool);
		reader.Get(ref racked);
		if (NetPlayer.TryGetPlayerFromClientId(clientId, out var plr) && plr.HasGunEquipped(out var gun))
		{
			gun.racked = racked;
			NetObjectRegistry.Server_ObjectSyncSingle(((Component)gun).gameObject);
		}
	}

	[ServerReceiver(10109)]
	private static void ServerReceiver__GunEjectMag(knetid clientId, ref NetDataReader reader)
	{
		if (NetPlayer.TryGetPlayerFromClientId(clientId, out var plr) && plr.HasGunEquipped(out var gun))
		{
			gun.UnloadMag();
			NetObjectRegistry.Server_ObjectSyncSingle(((Component)gun).gameObject);
		}
	}

	[ServerReceiver(10110)]
	private static void ServerReceiver__GunToggleSafety(knetid clientId, ref NetDataReader reader)
	{
		bool safe = default(bool);
		reader.Get(ref safe);
		if (NetPlayer.TryGetPlayerFromClientId(clientId, out var plr) && plr.HasGunEquipped(out var gun))
		{
			gun.safe = safe;
			NetObjectRegistry.Server_ObjectSyncSingle(((Component)gun).gameObject);
		}
	}
}
