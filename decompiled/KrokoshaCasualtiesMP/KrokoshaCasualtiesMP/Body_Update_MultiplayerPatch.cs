using System;
using HarmonyLib;
using KrokoshaCasualtiesUtils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(Body), "Update")]
public static class Body_Update_MultiplayerPatch
{
	private static float lastgunangle;

	private static void Prefix(Body __instance)
	{
		if (KrokoshaScavMultiplayer.network_system_is_running)
		{
			CoUtils_instance_MultiplayerPatch.cur_override_instance = __instance.GetCoUtilsInstance();
		}
	}

	private static void Postfix(Body __instance)
	{
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		CoUtils_instance_MultiplayerPatch.cur_override_instance = null;
		if (!Util.IsBodyLocal(__instance))
		{
			__instance.liquidDrinkTime = -0.5f;
			__instance.fallShakeCooldown = 1f;
			if (Util.world.biomeDepth == 5 && __instance.temperature < 32.5f)
			{
				__instance.snowAmount = Mathf.MoveTowards(__instance.snowAmount, 1f, Time.deltaTime * 0.0125f);
			}
			else
			{
				__instance.snowAmount = Mathf.MoveTowards(__instance.snowAmount, 0f, Time.deltaTime * 0.02f);
			}
		}
		NetBody component = ((Component)__instance).GetComponent<NetBody>();
		if ((Object)(object)component == (Object)null || !KrokoshaScavMultiplayer.network_system_is_running || WorldGeneration.world.generatingWorld)
		{
			return;
		}
		if (__instance.TryGetNetPlayer(out var plr))
		{
			if (plr.is_pointingfingerat && !__instance.armsAnimator.GetBool("gun"))
			{
				float num = ((Component)__instance.limbs[1]).transform.eulerAngles.z;
				if (num > 180f)
				{
					num -= 360f;
				}
				float num2 = 1f;
				if (!__instance.isRight)
				{
					num2 = -1f;
				}
				__instance.armsAnimator.SetBool("gun", true);
				float num3 = lastgunangle;
				Vector2 val = Vector2.op_Implicit(((Component)__instance).transform.right * num2);
				Vector2 val2 = plr.is_pointingfingeratTARGET - Vector2.op_Implicit(((Component)__instance.limbs[1]).transform.position);
				float num4 = Mathf.Lerp(num3, Vector2.SignedAngle(val, ((Vector2)(ref val2)).normalized) * num2 - num * num2, Time.deltaTime * 8f * __instance.slots[__instance.handSlot].armPowerMult);
				__instance.armsAnimator.SetFloat("gunangle", num4);
				lastgunangle = num4;
			}
			else
			{
				lastgunangle = __instance.armsAnimator.GetFloat("gunangle");
			}
		}
		if (__instance.alive)
		{
			if (!Util.IsBodyLocal(__instance))
			{
				__instance.attackCooldown -= Time.deltaTime * 0.05f;
			}
			BlockInfo standingOn = __instance.standingOn;
			if (__instance.grounded && standingOn != null && standingOn.toxicity > 0f)
			{
				__instance.radiationSickness += standingOn.toxicity * Time.deltaTime;
				PlayerCamera_SetIrradiateIntensity_MultiplayerPatch.RealSetIrradiateIntensity(component, standingOn.toxicity * 0.25f);
			}
			(NetBody, float) nearestConsciousPlayerToThisBody = NetPlayer.GetNearestConsciousPlayerToThisBody(component);
			bool flag = (Object)(object)nearestConsciousPlayerToThisBody.Item1 == (Object)null;
			bool flag2 = flag || nearestConsciousPlayerToThisBody.Item2 > 1600f;
			if (NetPlayer.ClientIdToPlayerDict.Count == 1)
			{
				flag = false;
			}
			float num5 = 0f;
			if (__instance.brainDying)
			{
				num5 += 1.5f;
			}
			else if (__instance.bloodOxygen < 80f)
			{
				num5 += (80f - __instance.bloodOxygen) / 600f * 0.4f;
			}
			if (__instance.temperature > 42f)
			{
				num5 += 0.5f;
			}
			if (__instance.radiationSickness > 0f)
			{
				num5 += __instance.radiationSickness * 0.0005f;
			}
			if (!flag && !__instance.brainDying && __instance.brainHealth < 100f && __instance.IsAirBreathable())
			{
				float additionalBrainRegen = KrokoshaScavMultiplayer.rules.AdditionalBrainRegen;
				if (additionalBrainRegen > 0f)
				{
					float num6 = Mathf.Clamp(__instance.hunger / 100f, 0f, 1.3f) * Mathf.Clamp(__instance.thirst / 100f, 0f, 1.3f) * __instance.brainHealth.RemapClamped(100f, 70f, 0f, 1f);
					float num7 = additionalBrainRegen * num6 - num5 * 0.7f;
					if (num7 > 0f)
					{
						__instance.brainHealth += 0.16f * num7 * Time.deltaTime;
					}
				}
			}
			if (__instance.consciousness < 19f)
			{
				if (((Vector2)(ref __instance.lastTimeStepVelocity)).magnitude < 7f && !__instance.sleeping && flag2)
				{
					float num8 = 1f;
					if (__instance.brainDying)
					{
						num8 = 1f;
						if (flag)
						{
							num8 = 3.5f;
						}
					}
					else
					{
						num8 = 8f;
						if (flag)
						{
							num8 = 20f;
						}
					}
					if ((Object)(object)component.piggybacking_on != (Object)null)
					{
						num8 = 0f;
					}
					else if (__instance.succesfullyRolledLastStand && __instance.lastStandTime > 280f)
					{
						num8 = 0f;
					}
					float num9 = num8 * Math.Max(KrokoshaScavMultiplayer.rules.AdditionalHealthDecay, 0f);
					if (num9 > 0f)
					{
						float num10 = Time.deltaTime * num9;
						if (__instance.totalBleedSpeed > 0f)
						{
							__instance.bloodVolume -= __instance.totalBleedSpeed * num10;
						}
						Painkillers val3 = default(Painkillers);
						if (((Component)__instance).TryGetComponent<Painkillers>(ref val3) && val3.actualOpiateReception > 30f)
						{
							__instance.energy -= val3.actualOpiateReception * 0.0025f * num10;
						}
						__instance.brainHealth -= num5 * num10;
					}
				}
			}
			else
			{
				float num11 = Math.Max(KrokoshaScavMultiplayer.rules.AdditionalHealthRegen, 0f);
				if (num11 > 0f)
				{
					float num12 = Time.deltaTime * num11;
					if (!__instance.brainDying && __instance.IsAirBreathable())
					{
						__instance.brainHealth += 0.003f * num12;
					}
					__instance.hearingLoss = Mathf.Clamp(__instance.hearingLoss - 0.05f * num12, 0f, 100f);
					__instance.clawHealth = Mathf.Clamp(__instance.clawHealth + num12 * __instance.clawGrowthRate * 0.5f, 0f, 100f);
					__instance.hungerLimbHealCurrent *= num11;
					if (__instance.totalBleedSpeed <= 0f)
					{
						__instance.bloodVolume = Mathf.MoveTowards(__instance.bloodVolume, 100f, num12 * __instance.bloodRegenSpeed);
					}
					if (__instance.stamina > 60f && ((__instance.standing && Mathf.Abs(__instance.moveDir.x) < 0.1f && !component.exercising) || !__instance.standing))
					{
						__instance.stamina += num12 * 0.5f;
					}
				}
			}
		}
		else if (__instance.sleeping)
		{
			__instance.WakeUp();
		}
		if (!KrokoshaScavMultiplayer.rules.DisableSleep)
		{
			return;
		}
		if (!__instance.isCriticallyDying)
		{
			if (__instance.conscious)
			{
				__instance.energy = Mathf.MoveTowards(__instance.energy, 100f, Time.deltaTime);
			}
			else
			{
				__instance.energy = Mathf.MoveTowards(__instance.energy, 100f, Time.deltaTime * 0.25f);
			}
		}
		if (__instance.sleeping)
		{
			__instance.WakeUp();
		}
	}
}
