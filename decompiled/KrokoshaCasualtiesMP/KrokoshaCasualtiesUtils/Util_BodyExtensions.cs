using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text;
using HarmonyLib;
using KrokoshaCasualtiesMP;
using UnityEngine;

namespace KrokoshaCasualtiesUtils;

public static class Util_BodyExtensions
{
	public static bool IsBodyLocal(this Body body)
	{
		return (Object)(object)PlayerCamera.main.body == (Object)(object)body;
	}

	public static Vector2 GetPosition(this Body body)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return Vector2.op_Implicit(((Component)body).transform.position);
	}

	public static bool IsAirBreathable(this Body body)
	{
		if (body.inWater)
		{
			return body.hasScubaGear;
		}
		return true;
	}

	public static bool IsMindwiped(this Body body)
	{
		if ((Object)(object)body.mindWipe != (Object)null)
		{
			return body.mindWipe.active;
		}
		return false;
	}

	public static bool IsRibsBroken(this Body body)
	{
		return body.limbs[1].broken;
	}

	public static bool IsRagdolling(this Body body)
	{
		return !body.standing;
	}

	public static bool IsAliveAndUnconsciousAndNotSleeping(this Body body)
	{
		if (body.alive)
		{
			if (!body.conscious)
			{
				return !body.sleeping;
			}
			return false;
		}
		return false;
	}

	public static Limb GetPrimaryHand(this Body body)
	{
		return body.limbs[(body.handSlot == 0) ? 5 : 8];
	}

	public static Limb GetSecondaryHand(this Body body)
	{
		return body.limbs[(body.handSlot == 0) ? 8 : 5];
	}

	public static FacialExpression GetFace(this Body body)
	{
		return ((Component)body.limbs[0]).GetComponent<FacialExpression>();
	}

	public static PantSound GetPantSound(this Body body)
	{
		return ((Component)body).GetComponent<PantSound>();
	}

	public static Limb GetHead(this Body body)
	{
		return body.limbs[0];
	}

	public static Limb GetUpperTorso(this Body body)
	{
		return body.limbs[1];
	}

	public static Limb GetLowerTorso(this Body body)
	{
		return body.limbs[2];
	}

	public static Limb GetClosestLimb(this Body body, Vector2 pos, out float dist_sqr)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		dist_sqr = 999999f;
		Limb result = body.limbs[0];
		Limb[] limbs = body.limbs;
		foreach (Limb val in limbs)
		{
			if (!val.dismembered && KM.dist2dsqrcheck_presqr(in pos, Vector2.op_Implicit(((Component)val).transform.position), dist_sqr, out var actual_dist))
			{
				result = val;
				dist_sqr = actual_dist;
			}
		}
		return result;
	}

	public static void ResetMind(this Body body)
	{
		body.Body_ResetSkills();
		body.corpsesSeen = 0;
		body.desensitizedMult = 1f;
	}

	public static void ResetHealth(this Body body, bool unmindwipe = true)
	{
		body.RegrowAllLimbs();
		Limb[] limbs = body.limbs;
		SplintLimb val = default(SplintLimb);
		TourniquetScript val2 = default(TourniquetScript);
		foreach (Limb obj in limbs)
		{
			obj.muscleHealth = 100f;
			obj.skinHealth = 100f;
			obj.boneHealTimer = 0f;
			obj.dislocationTimer = 0f;
			obj.infectionAmount = 0f;
			obj.bleedAmount = 0f;
			obj.pain = 0f;
			obj.shrapnel = 0;
			obj.furBloodAmount = 0f;
			obj.infected = false;
			if (((Component)obj).TryGetComponent<SplintLimb>(ref val))
			{
				val.condition = 0f;
				val.TakeOff();
			}
			if (((Component)obj).TryGetComponent<TourniquetScript>(ref val2))
			{
				val2.condition = 0f;
				val2.TakeOff();
			}
		}
		body.brainHealth = 100f;
		body.bloodVolume = 100f;
		body.bloodOxygen = 100f;
		body.bloodPressure = 120f;
		body.heartRate = 70f;
		body.bloodVesselSize = 1f;
		body.bloodViscosity = 0f;
		body.respiratoryRate = 100f;
		body.strokeAmount = 0f;
		body.hasPulmonaryEmbolism = false;
		body.fibrillationProgress = 0f;
		body.hunger = 100f;
		body.thirst = 100f;
		body.septicShock = 0f;
		body.temperature = 37f;
		body.sicknessAmount = 0f;
		body.consciousness = 100f;
		body.stamina = 100f;
		body.energy = 100f;
		body.happiness = 0f;
		body.weightOffset = 0f;
		body.radiationSickness = 0f;
		body.disfigured = false;
		body.eyeGone = false;
		body.bothEyesGone = false;
		body.internalBleeding = 0f;
		body.hemothorax = 0f;
		body.traumaAmount = 0f;
		body.dirtyness = 0f;
		body.wetness = 0f;
		body.badSleepAmount = 0f;
		body.hearingLoss = 0f;
		body.antidepressantHappiness = 0f;
		body.antibioticImmunityTime = 0f;
		body.brainGrowSickness = 0f;
		body.triedRollingLastStand = false;
		body.succesfullyRolledLastStand = false;
		body.reversedControls = false;
		body.lastStandTime = -1000f;
		body.adrenaline = 0f;
		body.curAdrenaline = 0f;
		body.opiateHappiness = 0f;
		body.venomCurrent = 0f;
		body.venomTotal = 0f;
		body.clawHealth = 100f;
		body.bloodPressureChangeFromMedicine = 0f;
		body.caffeinated = 0f;
		body.clawRegrowTime = 0f;
		Painkillers val3 = default(Painkillers);
		if (((Component)body).TryGetComponent<Painkillers>(ref val3))
		{
			Object.Destroy((Object)(object)val3);
		}
		SleepingPills val4 = default(SleepingPills);
		if (((Component)body).TryGetComponent<SleepingPills>(ref val4))
		{
			Object.Destroy((Object)(object)val4);
		}
		Antidepressants val5 = default(Antidepressants);
		if (((Component)body).TryGetComponent<Antidepressants>(ref val5))
		{
			Object.Destroy((Object)(object)val5);
		}
		NetBody netBody = default(NetBody);
		if (((Component)body).TryGetComponent<NetBody>(ref netBody) && netBody.is_player && (Object)(object)netBody.plr.personal_coutils_instance != (Object)null)
		{
			netBody.plr.personal_coutils_instance.CancelAll();
		}
		CoUtils cur_override_instance = CoUtils_instance_MultiplayerPatch.cur_override_instance;
		CoUtils_instance_MultiplayerPatch.cur_override_instance = body.GetCoUtilsInstance();
		CoUtils.instance.CancelAll();
		CoUtils_instance_MultiplayerPatch.cur_override_instance = cur_override_instance;
		MindwipeScript val6 = default(MindwipeScript);
		if (unmindwipe && ((Component)body).TryGetComponent<MindwipeScript>(ref val6))
		{
			Object.Destroy((Object)(object)val6);
		}
	}

	public static string DumpBodyVars(this Body body)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("BODY VARS DUMP:");
		try
		{
			FieldInfo[] array = (from f in typeof(Body).GetFields(BindingFlags.Instance | BindingFlags.Public)
				where f.FieldType.IsPrimitive
				select f).ToArray();
			foreach (FieldInfo fieldInfo in array)
			{
				stringBuilder.AppendLine($"\t{fieldInfo.Name}: {fieldInfo.GetValue(body)}");
			}
			Painkillers obj = default(Painkillers);
			if (((Component)body).TryGetComponent<Painkillers>(ref obj))
			{
				FieldInfo[] array2 = (from f in typeof(Painkillers).GetFields(BindingFlags.Instance | BindingFlags.Public)
					where f.FieldType.IsPrimitive
					select f).ToArray();
				stringBuilder.AppendLine("Painkillers:");
				array = array2;
				foreach (FieldInfo fieldInfo2 in array)
				{
					stringBuilder.AppendLine($"\t{fieldInfo2.Name}: {fieldInfo2.GetValue(obj)}");
				}
			}
			stringBuilder.AppendLine("Limbs:");
			FieldInfo[] array3 = (from f in typeof(Limb).GetFields(BindingFlags.Instance | BindingFlags.Public)
				where f.FieldType.IsPrimitive
				select f).ToArray();
			Limb[] limbs = body.limbs;
			foreach (Limb val in limbs)
			{
				stringBuilder.AppendLine("\t" + ((Object)val).name + " - " + val.fullName + " - " + val.shortName + ":");
				if (val.dismembered)
				{
					stringBuilder.AppendLine("\t\tAMPUTATED");
					continue;
				}
				array = array3;
				foreach (FieldInfo fieldInfo3 in array)
				{
					stringBuilder.AppendLine($"\t\t{fieldInfo3.Name}: {fieldInfo3.GetValue(val)}");
				}
			}
			stringBuilder.AppendLine("CoUtils:");
			CoUtils val2 = ((Component)body).GetComponent<NetBody>()?.plr?.personal_coutils_instance;
			if ((Object)(object)val2 == (Object)null)
			{
				stringBuilder.AppendLine("NULL");
			}
			else
			{
				IEnumerable enumerable = (IEnumerable)Traverse.Create((object)val2).Field("activeOps").GetValue();
				if (enumerable != null)
				{
					foreach (object item in enumerable)
					{
						stringBuilder.AppendLine(item.ToString());
					}
				}
				else
				{
					stringBuilder.AppendLine("ERROR");
				}
			}
		}
		catch (Exception ex)
		{
			stringBuilder.AppendLine(ex.ToString());
		}
		return stringBuilder.ToString();
	}
}
