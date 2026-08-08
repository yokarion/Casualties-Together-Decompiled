using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using KrokoshaCasualtiesUtils;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(TurretScript), "Shoot")]
public static class TurretScript_Shoot_MultiplayerPatch
{
	public static bool HitOnlyStatic = false;

	public static bool LastActiveShooterIsSuicide = false;

	public static NetBody LastActiveShooter = null;

	public static KrokoshaGunScriptTrackerComponent LastActiveShooterGun = null;

	public static List<SyncInfo> JustHitBuildings = new List<SyncInfo>();

	public static List<Limb> JustHitLimbs = new List<Limb>();

	public static FireInfo last_fire_info;

	public static bool Shoot_DidHitABuilding(BuildingEntity building)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (!Net.running)
		{
			return false;
		}
		if (ScavMultiBuildingSynchronizer.BuildingHasActivePhysics(building))
		{
			if (NetObjectRegistry.TryGetSyncInfo(((Component)building).gameObject, out var si))
			{
				if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
				{
					DebugHelp.OnNetEvent(Vector2.op_Implicit(((Component)building).transform.position), "Shoot: Noted hit building", Color.green);
				}
				JustHitBuildings.Add(si);
			}
			if ((Object)(object)LastActiveShooter != (Object)null)
			{
				return true;
			}
			if (HitOnlyStatic)
			{
				return true;
			}
		}
		return false;
	}

	public static bool Shoot_DidHitABody(RaycastHit2D hit2D)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		if (!Net.running)
		{
			return false;
		}
		Limb component = ((Component)((RaycastHit2D)(ref hit2D)).collider).GetComponent<Limb>();
		Body val = ((Component)((RaycastHit2D)(ref hit2D)).collider).GetComponent<Body>();
		if ((Object)(object)component == (Object)null)
		{
			if ((Object)(object)val == (Object)null)
			{
				return true;
			}
		}
		else
		{
			val = component.body;
		}
		component = val.GetClosestLimb(((RaycastHit2D)(ref hit2D)).point);
		if ((Object)(object)LastActiveShooter == (Object)null)
		{
			return false;
		}
		bool flag = false;
		bool flag2 = false;
		NetBody netBody = default(NetBody);
		if (((Component)component.body).TryGetComponent<NetBody>(ref netBody))
		{
			flag = LastActiveShooterIsSuicide && (Object)(object)netBody == (Object)(object)LastActiveShooter;
			if (flag)
			{
				flag2 = true;
			}
			else if (LastActiveShooter.CanAttackThisGuy(component.body))
			{
				flag2 = true;
			}
			if (HitOnlyStatic)
			{
				flag2 = false;
			}
			if (flag2 && netBody != null)
			{
				if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
				{
					DebugHelp.OnNetEvent(netBody.GetHeadPos(), "Shoot: Noted hit body", Color.green);
				}
				JustHitLimbs.Add(component);
			}
		}
		if (flag)
		{
			return false;
		}
		if (((Object)(object)netBody == (Object)null || !netBody.is_player) && KrokoshaScavMultiplayer.rules.PVP)
		{
			return false;
		}
		return true;
	}

	public static void ApplyShootDamages(KrokoshaGunScriptTrackerComponent gst, IEnumerable<Limb> hitLimbs, IEnumerable<SyncInfo> hitBuildings)
	{
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		Util.IsBodyLocal(gst.body);
		Dictionary<Limb, int> dictionary = hitLimbs.GroupBy((Limb x) => x, (IEqualityComparer<object>?)ReferenceEqualityComparer.Instance).ToDictionary((Func<IGrouping<object, Limb>, Limb>)((IGrouping<object, Limb> a) => (Limb)a.Key), (Func<IGrouping<object, Limb>, int>)((IGrouping<object, Limb> b) => b.Count()));
		Dictionary<NetBody, float> dictionary2 = new Dictionary<NetBody, float>();
		NetBody key = default(NetBody);
		foreach (Limb hitLimb in hitLimbs)
		{
			float num = Stolen_ShootApplyDamageLimb(hitLimb, gst, use_pvp_damage_model: true);
			if (((Component)hitLimb.body).TryGetComponent<NetBody>(ref key))
			{
				if (!dictionary2.ContainsKey(key))
				{
					dictionary2[key] = 0f;
				}
				dictionary2[key] += num;
			}
		}
		List<Body> list = new List<Body>();
		foreach (KeyValuePair<Limb, int> item in dictionary)
		{
			Limb key2 = item.Key;
			if (!list.Contains(key2.body) && (KrokoshaScavMultiplayer.rules.PVPCombatDismember || !key2.body.alive) && item.Value >= 3 && KM.dist2dsqrcheck(Vector2.op_Implicit(((Component)gst).transform.position), Vector2.op_Implicit(((Component)key2).transform.position), 16f))
			{
				CombatStuff.BetterDismember(key2, !key2.isHead);
				if (key2.dismembered)
				{
					list.Add(key2.body);
				}
			}
		}
		NetBody netBody = default(NetBody);
		foreach (KeyValuePair<NetBody, float> item2 in dictionary2)
		{
			Vector2 val = item2.Key.GetHeadPos() + Vector2.up;
			WorldGeneration.CreateDamageNumber(val, (int)item2.Value);
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(val, $"Shoot: Damaged body {item2.Key}", Color.red);
			}
			item2.Key.body.DoGoreSound();
			if (!item2.Key.body.conscious && item2.Key.body.isDying)
			{
				Body body = gst.body;
				body.happiness -= KrokoshaScavMultiplayer.rules.PVPMoodDebuff * 2f;
				if (!gst.body.specialCrying && KrokoshaScavMultiplayer.rules.PVPMoodDebuff > 0f)
				{
					((MonoBehaviour)gst.body).StartCoroutine("Cry");
				}
			}
			if (((Component)item2.Key).TryGetComponent<NetBody>(ref netBody))
			{
				if (!KrokoshaScavMultiplayer.is_client)
				{
					MedicalSync.Server_QueueSendCharacterHealth(netBody, force: true);
				}
				else
				{
					netBody.SetNetHealthSyncIgnoreTime();
				}
			}
		}
		foreach (SyncInfo hitBuilding in hitBuildings)
		{
			float num2 = Stolen_ShootApplyDamageBuilding(hitBuilding.building, gst);
			if (!KrokoshaScavMultiplayer.is_client)
			{
				NetObjectRegistry.Server_QueueSync(hitBuilding);
			}
			else
			{
				hitBuilding.SetIgnoreTimeForRoundTrip();
			}
			if (hitBuilding.building.animal)
			{
				Sound.Play("gore" + Random.Range(1, 6), hitBuilding.position, false, true, (Transform)null, 1f, 1f, false, false);
			}
			Vector2 val2 = hitBuilding.position + Vector2.up * 2f;
			WorldGeneration.CreateDamageNumber(val2, (int)num2);
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(val2, "Shoot: Damaged building", Color.red);
			}
		}
	}

	public static float Stolen_ShootApplyDamageBuilding(BuildingEntity b, KrokoshaGunScriptTrackerComponent gst)
	{
		float num = Random.Range(0.85f, 1.15f);
		float num2 = (b.animal ? (gst.gun.animalDamage * num) : (gst.gun.structureDamage * num));
		b.health -= num2;
		if (b.animal)
		{
			((Component)b).gameObject.SendMessage("AnimalHit", (object)num2);
		}
		return num2;
	}

	public static float Stolen_ShootApplyDamageLimb(Limb limb, KrokoshaGunScriptTrackerComponent gst, bool use_pvp_damage_model)
	{
		limb.body.adrenaline = 100f;
		float num = limb.GetArmorReduction();
		float num2 = KrokoshaScavMultiplayer.rules.PVPDamageMultiplier;
		Util.GetLimbIndex(limb);
		float num3 = 0f;
		if (use_pvp_damage_model)
		{
			num /= ((num2 == 0f) ? 1E-05f : num2);
			if (num2 != 0f)
			{
				limb.pain += Random.Range(50f, 70f) / num;
				num3 = gst.gun.animalDamage / num * 0.5f;
				limb.skinHealth -= num3;
				limb.muscleHealth -= num3;
				limb.bleedAmount += Random.Range(30f, 40f) / num;
				limb.shrapnel = Math.Max(limb.shrapnel, Random.Range(0, (int)Mathf.Clamp(5f * num2, 1f, 5f)));
				limb.DamageWearables(0.8f * num2);
			}
		}
		else
		{
			num2 = 1f;
			limb.pain += Random.Range(80f, 100f) / num;
			num3 = Random.Range(70f, 100f) / num;
			limb.skinHealth -= num3;
			limb.muscleHealth -= Random.Range(70f, 100f) / num;
			limb.bleedAmount += Random.Range(30f, 40f) / num;
			limb.shrapnel = 5;
			limb.DamageWearables(1f);
		}
		if (limb.isVital || limb.isHead)
		{
			Body body = limb.body;
			body.internalBleeding += Random.Range(18f, 22f) / num;
		}
		if (Random.Range(0f, 1f) < 0.3f * num2)
		{
			limb.BreakBone();
		}
		if (limb.isHead)
		{
			if (use_pvp_damage_model)
			{
				limb.pain += Random.Range(10f, 50f) / num;
				Body body2 = limb.body;
				body2.consciousness -= 35f / num;
				if (limb.body.brainHealth > 20f)
				{
					limb.body.brainHealth = Math.Max(4f, limb.body.brainHealth - Random.Range(10f, 70f) / num);
				}
				else
				{
					Body body3 = limb.body;
					body3.brainHealth -= Random.Range(5f, Math.Max(limb.body.brainHealth - 1f, 9f)) / num;
				}
				if (KrokoshaScavMultiplayer.rules.PVPCombatDismember && Random.value * num2 > 0.2f && !limb.body.GetHead().dismembered)
				{
					if (Random.value > 0.5f)
					{
						limb.body.Disfigure();
					}
					else
					{
						limb.body.RemoveEye();
					}
				}
			}
			else
			{
				limb.body.consciousness = 0f;
				Body body4 = limb.body;
				body4.brainHealth -= Random.Range(10f, 85f) / num;
				if (Random.value > 0.5f)
				{
					limb.body.Disfigure();
				}
				else
				{
					limb.body.RemoveEye();
				}
			}
		}
		if ((KrokoshaScavMultiplayer.rules.PVPCombatDismember || !limb.body.alive) && use_pvp_damage_model)
		{
			float num4 = 1f;
			float num5 = 3f;
			if (limb.body.IsDeadOrCriticallyDying())
			{
				num4 = 2f;
				num5 = ((!gst.IsShotgun()) ? 5f : 52f);
			}
			if (limb.muscleHealth <= num4 && limb.skinHealth <= num5 && !KrokoshaScavMultiplayer.is_client && (double)Random.value < 0.2 * (double)num4 * (double)num2)
			{
				Limb val = limb.body.limbs[0];
				if (Util.GetLimbIndex(limb) <= 2 && !val.dismembered)
				{
					CombatStuff.BetterDismember(val);
				}
				else
				{
					CombatStuff.BetterDismember(limb);
				}
			}
		}
		return num3;
	}

	public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
	{
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Expected O, but got Unknown
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Expected O, but got Unknown
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Expected O, but got Unknown
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Expected O, but got Unknown
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Expected O, but got Unknown
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Expected O, but got Unknown
		List<CodeInstruction> list = new List<CodeInstruction>(instructions);
		int num = 0;
		for (num += 100; !(list[num - 3].opcode == OpCodes.Ldloc_S) || !(list[num - 2].opcode == OpCodes.Ldfld) || !(list[num - 1].opcode == OpCodes.Brtrue) || !(list[num].opcode == OpCodes.Ldloc_S) || !((object)list[num - 2]).ToString().Contains("cantHit"); num++)
		{
		}
		if (log.verbose)
		{
			Plugin.log.LogWarning((object)$"TRANSPILER: TurretScript Shoot TRANSPILER found the building if statement {num}");
		}
		int num2 = num;
		for (num += 60; !(list[num - 3].opcode == OpCodes.Ldloc_S) || !(list[num - 2].opcode == OpCodes.Ldfld) || !(list[num - 1].opcode == OpCodes.Stloc_S) || !(list[num].opcode == OpCodes.Ldloc_S) || !((object)list[num - 2]).ToString().Contains("Limb") || !((object)list[num - 2]).ToString().Contains("body"); num++)
		{
		}
		int num3 = num;
		if (log.verbose)
		{
			Plugin.log.LogWarning((object)$"TRANSPILER: TurretScript Shoot TRANSPILER found teh body damage part {num}");
		}
		int num4 = num3 - 1;
		while (!(list[num4].opcode == OpCodes.Brtrue))
		{
			num4--;
		}
		List<CodeInstruction> list2 = new List<CodeInstruction>();
		int num5 = num3 - 1;
		while (!(list[num5].operand is LocalBuilder localBuilder) || !(localBuilder.LocalType == typeof(Body)))
		{
			num5--;
		}
		int num6 = num3 - 1;
		while (!(list[num6].operand is LocalBuilder localBuilder2) || !(localBuilder2.LocalType == typeof(RaycastHit2D)))
		{
			num6--;
		}
		list2.Add(new CodeInstruction(OpCodes.Call, (object)AccessTools.Method(typeof(TurretScript_Shoot_MultiplayerPatch), "Shoot_DidHitABody", (Type[])null, (Type[])null)));
		list2.Add(new CodeInstruction(OpCodes.Brtrue, list[num4].operand));
		list2.Add(new CodeInstruction(OpCodes.Ldloc_S, list[num5].operand));
		list[num3].operand = list[num6].operand;
		list.InsertRange(num3 + 1, list2);
		List<CodeInstruction> list3 = new List<CodeInstruction>();
		list3.Add(new CodeInstruction(OpCodes.Call, (object)AccessTools.Method(typeof(TurretScript_Shoot_MultiplayerPatch), "Shoot_DidHitABuilding", (Type[])null, (Type[])null)));
		list3.Add(new CodeInstruction(OpCodes.Brtrue, list[num4].operand));
		list3.Add(new CodeInstruction(OpCodes.Ldloc_S, list[num2].operand));
		list.InsertRange(num2 + 1, list3);
		if (log.verbose)
		{
			Plugin.log.LogWarning((object)"TRANSPILER: TurretScript Shoot TRANSPILER SUCCESS :) ");
		}
		return list;
	}

	private static void Prefix(FireInfo info)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		JustHitBuildings.Clear();
		JustHitLimbs.Clear();
		last_fire_info = info;
		if (!info.doTinnitus)
		{
			return;
		}
		foreach (NetBody item in NetBody.GetBodiesInRadius(info.pos, 20f))
		{
			if (!item.IsBodyLocal())
			{
				Body body = item.body;
				body.hearingLoss += Random.Range(10f, 15f);
				item.body.eyePanicTime = 0.6f;
				item.body.eyeCloseTime = 4f;
				item.body.eyeScareTime = 8f;
				item.body.talker.Talk(Locale.GetCharacter("loud"), (Limb)null, false, false);
			}
		}
	}

	private static void Postfix()
	{
		LastActiveShooterIsSuicide = false;
		LastActiveShooter = null;
		LastActiveShooterGun = null;
	}
}
