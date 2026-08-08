using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using KrokoshaCasualtiesUtils;
using LiteNetLib;
using LiteNetLib.Utils;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(Body), "Attack")]
public static class Body_Attack_MultiplayerPatch
{
	private class _MyAttackState
	{
		public float PlayerCamera_main_lastAttackCool;

		public bool candoattack;

		public Vector2 targetlookpos;

		public Vector2 limb1pos;

		public AttackInfo ogatk;

		public float atkpwr;
	}

	public static bool breakTheAttackLoop = false;

	public static Body last_attacker = null;

	private static Limb _LastHitLimb;

	private static BuildingEntity _LastHitBuilding;

	public static bool StaticHitValidation = false;

	private static AttackInfo LastAttackInfo = new AttackInfo();

	private static bool LastAttackResult = false;

	public static bool ForceOnlyStaminaUse = false;

	public static bool ForceOnlyRetrieveAttackInfo = false;

	public static void DoToolSpecificPVPDamage(Item used_tool, Limb target, float rule_mult, float multiplier_strength_vs_armor)
	{
		if (!(used_tool.id == "sledgehammer"))
		{
			return;
		}
		if ((double)Random.value < 0.5 * (double)rule_mult)
		{
			if ((double)Random.value < 0.3 * (double)rule_mult)
			{
				target.BreakBone();
			}
			else
			{
				target.Dislocate();
			}
		}
		if (target.isHead)
		{
			Body body = target.body;
			body.brainHealth -= 10f * multiplier_strength_vs_armor;
			Body body2 = target.body;
			body2.consciousness -= 50f * multiplier_strength_vs_armor;
		}
		if ((double)Random.value < 0.7 * (double)rule_mult && (Object)(object)target == (Object)(object)target.body.GetUpperTorso())
		{
			target.body.respiratoryRate = Mathf.Max(0f, target.body.respiratoryRate - 50f);
		}
		Body body3 = target.body;
		body3.eyePanicTime += 0.5f;
		Body body4 = target.body;
		body4.eyeScareTime += 1f;
	}

	public static float DoPVPMeleeDamage(Body attacker, Limb target, AttackInfo atk, float armstrength, Item used_tool, out Vector2 knockback)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = KM.normal(Vector2.op_Implicit(((Component)attacker.limbs[1]).transform.position), Vector2.op_Implicit(attacker.targetLookPos));
		knockback = new Vector2(val.x, 0.4f);
		knockback *= atk.knockBack * 0.04f;
		attacker.attackCooldown *= 3.5f * atk.attackCooldownMult;
		if (Util.IsBodyLocal(attacker))
		{
			PlayerCamera.main.lastAttackCool = attacker.attackCooldown;
		}
		float num = target.skinHealth + target.muscleHealth;
		float pVPDamageMultiplier = KrokoshaScavMultiplayer.rules.PVPDamageMultiplier;
		float num2 = armstrength / target.GetArmorReduction() * pVPDamageMultiplier;
		if (!target.body.conscious)
		{
			num2 *= 2f;
			knockback *= 0.8f;
			if (!target.body.alive)
			{
				knockback *= 0.8f;
				num2 = armstrength * 2f;
				if (atk.unarmed)
				{
					knockback *= 0.1f;
				}
			}
		}
		Body body = target.body;
		body.traumaAmount += 0.2f * num2;
		attacker.traumaAmount += KrokoshaScavMultiplayer.rules.PVPMoodDebuff;
		if (attacker.happiness > -5f)
		{
			attacker.happiness -= KrokoshaScavMultiplayer.rules.PVPMoodDebuff;
		}
		target.skinHealth -= atk.damage * 0.3f * num2;
		target.muscleHealth -= atk.damage * 0.17f * num2;
		target.bleedAmount += atk.damage * 0.02f * num2;
		Body body2 = target.body;
		body2.adrenaline += 35f;
		target.pain = Math.Max(target.pain + 5f * num2, atk.damage * num2);
		target.DamageWearables(atk.damage * 0.009f * num2);
		target.body.DoGoreSound();
		Body body3 = target.body;
		body3.eyeScareTime += 0.5f;
		if ((Object)(object)used_tool == (Object)null)
		{
			if (attacker.clawHealth > 18f)
			{
				target.skinHealth -= atk.damage * 0.17f * num2;
				target.bleedAmount += atk.damage * 0.07f * num2;
				target.DamageWearables(atk.damage * 0.005f * num2);
				Body body4 = target.body;
				body4.eyeScareTime += 1f;
			}
		}
		else
		{
			if (armstrength > 0.5f)
			{
				DoToolSpecificPVPDamage(used_tool, target, pVPDamageMultiplier, num2);
			}
			CraftingQuality val2 = used_tool.Stats.qualities?.FirstOrDefault((CraftingQuality q) => q.id == "cutting");
			if (val2 != null)
			{
				float num3 = Math.Max(20f, val2.amount) / 100f;
				target.skinHealth -= atk.damage * 0.6f * num2 * num3;
				target.muscleHealth -= atk.damage * 0.02f * num2 * num3;
				target.bleedAmount += atk.damage * 0.5f * num2 * num3;
				Body body5 = target.body;
				body5.adrenaline += 10f;
				target.pain += 10f * pVPDamageMultiplier;
				target.DamageWearables(atk.damage * 0.03f * num2 * num3);
				Body body6 = target.body;
				body6.eyePanicTime += 2f;
				Body body7 = target.body;
				body7.eyeScareTime += 6f;
			}
		}
		if (armstrength > 0.5f && KrokoshaScavMultiplayer.rules.PVPCombatDismember && target.isHead && target.muscleHealth < 30f && target.skinHealth < 40f && Random.value * (target.muscleHealth + target.skinHealth) / 100f < 0.1f)
		{
			if ((double)Random.value < 0.7)
			{
				target.body.RemoveEye();
			}
			else
			{
				target.body.Disfigure();
			}
		}
		if (target.muscleHealth <= 5f && target.skinHealth <= 5f && !KrokoshaScavMultiplayer.is_client && (KrokoshaScavMultiplayer.rules.PVPCombatDismember || !target.body.alive) && (target.body.IsDeadOrCriticallyDying() || (double)Random.value < 0.6))
		{
			CombatStuff.BetterDismember(target);
		}
		if (!target.body.conscious && target.body.isDying)
		{
			if (target.body.alive)
			{
				attacker.happiness -= KrokoshaScavMultiplayer.rules.PVPMoodDebuff;
			}
			if (!attacker.specialCrying && KrokoshaScavMultiplayer.rules.PVPMoodDebuff > 0f)
			{
				((MonoBehaviour)attacker).StartCoroutine("Cry");
			}
		}
		float num4 = num - (target.skinHealth + target.muscleHealth);
		if (!KrokoshaScavMultiplayer.is_client)
		{
			if (target.isVital && target.muscleHealth < 40f && target.skinHealth < 30f)
			{
				float num5 = Mathf.Clamp01(1f - (target.muscleHealth + target.skinHealth) / 70f) * 2f + 1f;
				float num6 = num4 * 0.03f * armstrength * num5;
				Body body8 = target.body;
				body8.internalBleeding += num6;
			}
			if (target.muscleHealth < 20f && target.skinHealth < 20f && target.isHead)
			{
				float num7 = Mathf.Clamp01(1f - (target.muscleHealth + target.skinHealth) / 40f) * 2f + 1f;
				float num8 = num4 * 0.04f * armstrength * num7;
				Body body9 = target.body;
				body9.brainHealth -= num8;
			}
			float num9 = Mathf.Abs(num4 / 50f);
			target.body.skills.AddExp(1, num9);
		}
		return num4;
	}

	public static float DamageBuilding(Body attacker, AttackInfo atk, BuildingEntity be, float armstrength, bool do_sound)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		float num = (be.animal ? atk.damage : atk.structuralDamage) * armstrength * ((atk.metalMoreDamage && be.metallic) ? 10f : 1f);
		be.health -= num;
		if (do_sound)
		{
			Sound.Play(be.hitSound, Vector2.op_Implicit(((Component)be).transform.position), false, true, (Transform)null, 1f, 1f, false, false);
		}
		if (be.animal)
		{
			((Component)be).gameObject.SendMessage("AnimalHit", (object)(atk.damage * armstrength));
			attacker.attackCooldown *= 3.5f * atk.attackCooldownMult;
			if (Util.IsBodyLocal(attacker))
			{
				PlayerCamera.main.lastAttackCool = attacker.attackCooldown;
			}
		}
		return num;
	}

	public static float CalculateAttackPower(in Body attacker, in AttackInfo atk)
	{
		float num = 1f;
		int handSlot = attacker.handSlot;
		if (atk.physicalSwing)
		{
			num = attacker.slots[handSlot].armPowerMult;
			if (handSlot == 1)
			{
				num *= 0.75f;
			}
			num *= 1f + attacker.skills.STRFrom10 * 0.0334f;
		}
		if (atk.unarmed)
		{
			num *= attacker.clawDamageCurve.Evaluate(attacker.clawHealth);
		}
		return num;
	}

	public static void BodyAttack_StolenHitEffectsOrWhatever(Body attacker, AttackInfo atk, float attackpowerscalething, Vector2 atkdir)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		int handSlot = attacker.handSlot;
		if (attacker.standing)
		{
			attacker.rb.AddForce(-atkdir * atk.knockBack * attackpowerscalething, (ForceMode2D)1);
		}
		else
		{
			attacker.limbs[1].rb.AddForce(-atkdir * atk.knockBack * attackpowerscalething, (ForceMode2D)1);
		}
		if (atk.unarmed)
		{
			attacker.clawHealth -= 0.3f;
			if (attacker.clawHealth < 20f && Random.value < 0.1f)
			{
				Limb limb = attacker.slots[handSlot].limb;
				limb.skinHealth -= 3f;
				Limb limb2 = attacker.slots[handSlot].limb;
				limb2.muscleHealth -= 2f;
				Limb limb3 = attacker.slots[handSlot].limb;
				limb3.pain += 12f;
				Limb limb4 = attacker.slots[handSlot].limb;
				limb4.bleedAmount += Random.Range(0.35f, 0.85f);
			}
		}
		if (atk.physicalSwing)
		{
			attacker.dirtyness += atk.cooldown * 1f;
		}
	}

	private static bool BodyAttack_CheckIfItWasABody(RaycastHit2D hit)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		Limb val = ((Component)((RaycastHit2D)(ref hit)).collider).GetComponent<Limb>();
		Body val2 = ((Component)((RaycastHit2D)(ref hit)).collider).GetComponent<Body>();
		if ((Object)(object)val == (Object)null)
		{
			if ((Object)(object)val2 == (Object)null)
			{
				return false;
			}
			val = val2.GetClosestLimb(((RaycastHit2D)(ref hit)).point);
		}
		else
		{
			val2 = val.body;
		}
		if (Util.IsBodyLocal(val2))
		{
			return false;
		}
		NetBody component = ((Component)last_attacker).GetComponent<NetBody>();
		((Component)val2).GetComponent<NetBody>();
		if (component.CanAttackThisGuy(val2))
		{
			_LastHitLimb = val;
			breakTheAttackLoop = true;
			return true;
		}
		return false;
	}

	private static bool BodyAttack_CheckIfItWasABuilding(RaycastHit2D hit)
	{
		BuildingEntity component = ((Component)((RaycastHit2D)(ref hit)).collider).GetComponent<BuildingEntity>();
		if ((Object)(object)component != (Object)null)
		{
			if (component.cantHit)
			{
				return false;
			}
			Util.IsBodyLocal(last_attacker);
			bool flag = ScavMultiBuildingSynchronizer.BuildingHasActivePhysics(component);
			if (StaticHitValidation && flag)
			{
				return true;
			}
			if (flag)
			{
				_LastHitBuilding = component;
				breakTheAttackLoop = true;
				return true;
			}
			return false;
		}
		return false;
	}

	public static bool BodyAttack_DidHitSomething(RaycastHit2D hit)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (!Net.running)
		{
			return false;
		}
		if (!StaticHitValidation && BodyAttack_CheckIfItWasABody(hit))
		{
			return true;
		}
		if (BodyAttack_CheckIfItWasABuilding(hit))
		{
			return true;
		}
		return false;
	}

	public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
	{
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Expected O, but got Unknown
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Expected O, but got Unknown
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Expected O, but got Unknown
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Expected O, but got Unknown
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Expected O, but got Unknown
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Expected O, but got Unknown
		List<CodeInstruction> list = new List<CodeInstruction>(instructions);
		int num = 0;
		for (num += 100; !(list[num - 5].opcode == OpCodes.Call) || !(list[num - 4].opcode == OpCodes.Ldc_I4_1) || !(list[num - 3].opcode == OpCodes.Stloc_2) || !(list[num - 2].opcode == OpCodes.Ldarg_1) || !(list[num - 1].opcode == OpCodes.Ldfld) || !(list[num].opcode == OpCodes.Brfalse) || !((object)list[num - 1]).ToString().Contains("piercing") || !((object)list[num - 5]).ToString().Contains("CreateCloudSmall"); num++)
		{
		}
		int num2 = num;
		int index = num;
		if (log.verbose)
		{
			Plugin.log.LogWarning((object)$"TEMP DEV: Body.Attack TRANSPILER found the thing check {num}");
		}
		int i;
		for (i = num2 + 1; !(list[i].opcode == OpCodes.Brtrue) && !(list[i].opcode == OpCodes.Brfalse); i++)
		{
		}
		int j;
		for (j = num2; !(list[j].opcode == OpCodes.Ldloca_S); j++)
		{
		}
		List<CodeInstruction> list2 = new List<CodeInstruction>();
		CodeInstruction val = new CodeInstruction(OpCodes.Ldloc_S, list[j].operand);
		CodeInstructionExtensions.MoveLabelsFrom(val, list[num2 + 1]);
		list2.Add(val);
		list2.Add(new CodeInstruction(OpCodes.Call, (object)AccessTools.Method(typeof(Body_Attack_MultiplayerPatch), "BodyAttack_DidHitSomething", (Type[])null, (Type[])null)));
		Label label = il.DefineLabel();
		list[num2 + 1].labels.Add(label);
		list2.Add(new CodeInstruction(OpCodes.Brfalse_S, (object)label));
		list2.Add(new CodeInstruction(OpCodes.Ldsfld, (object)AccessTools.Field(typeof(Body_Attack_MultiplayerPatch), "breakTheAttackLoop")));
		list2.Add(new CodeInstruction(OpCodes.Brfalse, list[i].operand));
		list2.Add(new CodeInstruction(OpCodes.Br, list[index].operand));
		list.InsertRange(num2 + 1, list2);
		if (log.verbose)
		{
			Plugin.log.LogWarning((object)"TRANSPILER: Body.Attack TRANSPILER SUCCESS :) ");
		}
		return list;
	}

	private static bool Prefix(Body __instance, ref bool __result, ref AttackInfo atk, int slot, ref _MyAttackState __state)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		LastAttackInfo = atk;
		__state = new _MyAttackState
		{
			PlayerCamera_main_lastAttackCool = PlayerCamera.main.lastAttackCool,
			targetlookpos = Vector2.op_Implicit(__instance.targetLookPos),
			limb1pos = Vector2.op_Implicit(((Component)__instance.limbs[1]).transform.position),
			ogatk = atk,
			atkpwr = CalculateAttackPower(in __instance, in LastAttackInfo)
		};
		if (ForceOnlyRetrieveAttackInfo)
		{
			ForceOnlyRetrieveAttackInfo = false;
			return false;
		}
		breakTheAttackLoop = false;
		last_attacker = __instance;
		if (ForceOnlyStaminaUse)
		{
			atk = Serialization.CloneViaSerialization<AttackInfo>(atk, false);
			atk.distance = 0f;
			ForceOnlyStaminaUse = false;
		}
		if (__instance.conscious && __instance.attackCooldown <= 0f)
		{
			__state.candoattack = true;
			if (KrokoshaScavMultiplayer.network_system_is_running)
			{
				KrokoshaScavMultiplayer.IsInGameAndWorldGenerated();
			}
		}
		return true;
	}

	private static void Postfix(Body __instance, ref bool __result, AttackInfo atk, int slot, ref _MyAttackState __state)
	{
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		LastAttackResult = __result;
		StaticHitValidation = false;
		breakTheAttackLoop = false;
		if (!Util.IsBodyLocal(__instance))
		{
			PlayerCamera.main.lastAttackCool = __state.PlayerCamera_main_lastAttackCool;
			if (__state.candoattack)
			{
				__instance.attackCooldown *= 0.9f;
				__instance.attackCooldown -= 0.05f;
			}
		}
		if (__state.candoattack && KrokoshaScavMultiplayer.network_system_is_running && KrokoshaScavMultiplayer.IsInGameAndWorldGenerated())
		{
			if ((Object)(object)_LastHitLimb != (Object)null && !_LastHitLimb.body.alive)
			{
				_LastHitLimb = CombatStuff.CannibalismGetFarthestLimbToBeEfficient(_LastHitLimb);
			}
			Item item = __instance.GetItem(__instance.handSlot);
			if (KrokoshaScavMultiplayer.is_client)
			{
				if ((Object)(object)_LastHitLimb != (Object)null)
				{
					Vector2 knockback;
					float num = DoPVPMeleeDamage(__instance, _LastHitLimb, __state.ogatk, __state.atkpwr, Body_UseItem_MultiplayerPatch.current_used_item ?? item, out knockback);
					WorldGeneration.CreateDamageNumber(Vector2.op_Implicit(((Component)_LastHitLimb.body.limbs[0]).transform.position) + Vector2.up, (int)num);
					DoKnockback(_LastHitLimb.body, knockback);
				}
				else if ((Object)(object)_LastHitBuilding != (Object)null)
				{
					float num2 = DamageBuilding(__instance, __state.ogatk, _LastHitBuilding, __state.atkpwr, do_sound: true);
					WorldGeneration.CreateDamageNumber(Vector2.op_Implicit(((Component)_LastHitBuilding).transform.position) + Vector2.up * 2f, (int)num2);
					if (KrokoshaScavMultiplayer.is_client && NetObjectRegistry.TryGetSyncInfo((Component)(object)_LastHitBuilding, out var si))
					{
						si.SetIgnoreTimeForRoundTrip();
					}
				}
			}
			if (Util.IsBodyLocal(__instance))
			{
				AnyObjectNetId value = new AnyObjectNetId();
				SyncInfo si2;
				if ((Object)(object)_LastHitLimb != (Object)null)
				{
					NetBody netBody = default(NetBody);
					if (((Component)_LastHitLimb.body).TryGetComponent<NetBody>(ref netBody))
					{
						value = new AnyObjectNetId(_LastHitLimb);
					}
				}
				else if ((Object)(object)_LastHitBuilding != (Object)null && NetObjectRegistry.TryGetSyncInfoOrRegister((Component)(object)_LastHitBuilding, out si2))
				{
					value = new AnyObjectNetId(si2);
					si2.relaxed_combat_sync = true;
					si2.SetIgnoreTimeForRoundTrip();
				}
				NetDataWriter writer = Net.CreateWriter(10101);
				writer.Put(__state.targetlookpos);
				writer.Put(__state.limb1pos);
				writer.Put(__state.ogatk.distance);
				writer.Put(__state.ogatk.unarmed);
				SyncInfo si4;
				if ((Object)(object)Body_UseItem_MultiplayerPatch.current_used_item != (Object)null && NetObjectRegistry.TryGetSyncInfo(((Component)Body_UseItem_MultiplayerPatch.current_used_item).gameObject, out var si3))
				{
					writer.Put((ushort)si3.syncId);
					si3.SetIgnoreTimeForRoundTrip();
				}
				else if ((Object)(object)item != (Object)null && item.Stats.usable && CombatStuff.ItemIsUsedForAttacking(item) && NetObjectRegistry.TryGetSyncInfo(((Component)item).gameObject, out si4))
				{
					writer.Put((ushort)si4.syncId);
					si4.SetIgnoreTimeForRoundTrip();
				}
				else
				{
					writer.Put((ushort)(knetid)(ushort)0);
				}
				writer.Put(__result);
				writer.Put(value);
				Net.Client_Send((DeliveryMethod)0, in writer);
			}
		}
		_LastHitBuilding = null;
		_LastHitLimb = null;
	}

	[ServerReceiver(10101)]
	private static void ServerReceiver_PlayerBodyAttack(knetid clientId, ref NetDataReader reader)
	{
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		//IL_045b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0462: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		if (!KrokoshaScavMultiplayer.IsInGameAndWorldGenerated() || !NetPlayer.TryGetNetPlayerAndNetBodyFromClientId(clientId, out var plr, out var pb) || !plr.body.conscious)
		{
			return;
		}
		if (plr.body.attackCooldown > 0f && !pb.is_local)
		{
			if (log.verbose)
			{
				log.sus($"PlayerBodyAttack {plr} is attacking too fast, cur cooldown: {plr.body.attackCooldown}");
			}
			return;
		}
		reader.Get(out Vector2 result);
		reader.Get(out Vector2 result2);
		float num = default(float);
		reader.Get(ref num);
		bool flag = default(bool);
		reader.Get(ref flag);
		reader.Get(out knetid result3);
		bool flag2 = default(bool);
		reader.Get(ref flag2);
		reader.Get(out AnyObjectNetId result4);
		if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
		{
			DebugHelp.OnNetEvent(Vector2.op_Implicit(((Component)plr.body).transform.position), $"S: PlayerBodyAttack {clientId} to didhit:{flag2}  unarmed:{flag} hitobj:{result4}");
		}
		if (!flag && NetObjectRegistry.TryGetSyncInfo(result3, out var si) && si.IsItem() && CombatStuff.ItemIsUsedForAttacking(si.item) && ItemSync.CheckIfBodyReachThisItem(si, plr.body, 20f, check_obstruction: true))
		{
			if (!((Object)(object)plr.body.GetItem(plr.body.handSlot) != (Object)(object)si.item))
			{
			}
		}
		else
		{
			si = null;
		}
		ForceOnlyRetrieveAttackInfo = true;
		if (si == null)
		{
			plr.body.Attack(CombatStuff.default_unarmed_attackinfo, plr.body.handSlot);
		}
		else
		{
			si.item.Stats.useAction.Invoke(pb.body, si.item);
		}
		AttackInfo atk = Serialization.CloneViaSerialization<AttackInfo>(LastAttackInfo, false);
		Vector2 atkdir = KM.normal(in result2, in result);
		float num2 = CalculateAttackPower(pb.body, in atk);
		bool flag3 = false;
		Limb limb;
		SyncInfo si2;
		if (result4.IsNothing())
		{
			StaticHitValidation = true;
		}
		else if (result4.TryGetLimb(out limb))
		{
			ForceOnlyStaminaUse = true;
			if (limb.TryGetNetBody(out var nb))
			{
				Component ba = (Component)(object)plr.body;
				if (KM.dist2dsqrcheck(in ba, (Component)(object)nb.body, 15f) && pb.CanAttackThisGuy(nb.body))
				{
					Vector2 knockback;
					float num3 = DoPVPMeleeDamage(plr.body, limb, atk, num2, si?.item, out knockback);
					if (nb.is_player)
					{
						KrokoshaScavMultiplayer.Server_SendSimpleMessageToOneClient((ushort)10106, nb.plr.clientId, knockback);
					}
					if (!nb.is_local)
					{
						DoKnockback(limb.body, knockback);
					}
					WorldGeneration.CreateDamageNumber(nb.GetHeadPos() + Vector2.up, (int)num3);
					MedicalSync.Server_QueueSendCharacterHealth(nb, force: true);
					flag3 = true;
				}
			}
		}
		else if (result4.TryGetSyncInfo(out si2) && si2.IsBuilding())
		{
			ForceOnlyStaminaUse = true;
			if (KM.dist2dsqrcheck(in plr.body, in si2, 15f))
			{
				float num4 = DamageBuilding(plr.body, atk, si2.building, num2, do_sound: true);
				flag3 = true;
				WorldGeneration.CreateDamageNumber(Vector2.op_Implicit(((Component)si2.building).transform.position) + Vector2.up * 2f, (int)num4);
				NetObjectRegistry.Server_QueueSync(si2);
			}
		}
		LastAttackResult = true;
		((Component)plr.body.limbs[1]).transform.position = Vector2.op_Implicit(result2);
		plr.body.targetLookPos = Vector2.op_Implicit(result);
		if (si == null)
		{
			plr.body.Attack(CombatStuff.default_unarmed_attackinfo, plr.body.handSlot);
		}
		else
		{
			si.item.Stats.useAction.Invoke(pb.body, si.item);
		}
		if (!LastAttackResult && flag3)
		{
			BodyAttack_StolenHitEffectsOrWhatever(pb.body, atk, num2, atkdir);
		}
		NetDataWriter writer = Net.CreateWriter(10102);
		writer.Put((ushort)clientId);
		writer.Put(si != null);
		writer.Put((ushort)(knetid)(ushort)((si != null) ? ((ushort)si.syncId) : 0));
		writer.Put(result2);
		writer.Put(result);
		Net.Server_SendToClients((DeliveryMethod)4, in writer, (IEnumerable<knetid>)ServerMain.GetListOfClientIdsExceptThisAndHost(clientId));
	}

	[ClientReceiver(10102, true)]
	private static void Client_ReceiveAndVisualizePlayerBodyAttack(knetid _, ref NetDataReader reader)
	{
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out knetid result);
		bool flag = default(bool);
		reader.Get(ref flag);
		reader.Get(out knetid result2);
		reader.Get(out Vector2 result3);
		reader.Get(out Vector2 result4);
		if (NetPlayer.TryGetNetPlayerAndNetBodyFromClientId(result, out var plr, out var pb))
		{
			pb.body.attackCooldown = 0f;
			if (!flag || !NetObjectRegistry.TryGetSyncInfo(result2, out var si) || !si.IsItem() || !CombatStuff.ItemIsUsedForAttacking(si.item))
			{
				si = null;
			}
			((Component)plr.body.limbs[1]).transform.position = Vector2.op_Implicit(result3);
			plr.body.targetLookPos = Vector2.op_Implicit(result4);
			if (si == null)
			{
				plr.body.Attack(CombatStuff.default_unarmed_attackinfo, plr.body.handSlot);
			}
			else
			{
				si.item.Stats.useAction.Invoke(pb.body, si.item);
			}
		}
	}

	[ClientReceiver(10106, false)]
	private static void Client_ReceiveKnockback(knetid _, ref NetDataReader reader)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out Vector2 result);
		NetPlayer lOCAL_PLAYER = NetPlayer.LOCAL_PLAYER;
		if ((Object)(object)lOCAL_PLAYER != (Object)null && Object.op_Implicit((Object)(object)lOCAL_PLAYER.body))
		{
			DoKnockback(lOCAL_PLAYER.body, result);
		}
	}

	private static void DoKnockback(Body body, Vector2 knockback)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		body.grounded = false;
		if (Util.IsBodyLocal(body))
		{
			PlayerCamera.main.shaker.Shake(((Vector2)(ref knockback)).magnitude);
		}
		if (body.standing)
		{
			Rigidbody2D rb = body.rb;
			rb.velocity += knockback * 1.6f;
			body.lastTimeStepVelocity = body.rb.velocity;
			return;
		}
		Limb[] limbs = body.limbs;
		for (int i = 0; i < limbs.Length; i++)
		{
			Rigidbody2D rb2 = limbs[i].rb;
			rb2.velocity += knockback;
		}
	}
}
