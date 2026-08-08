using HarmonyLib;
using KrokoshaCasualtiesUtils;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(Limb), "ImpactDamage")]
public static class Limb_ImpactDamage_MultiplayerPatch
{
	private static bool force_run;

	public static bool last_impact_is_local_body;

	public static void Force(Limb __instance, float force, Vector2 dir)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		force_run = true;
		__instance.ImpactDamage(force, dir);
		force_run = false;
	}

	public static bool Prefix(Limb __instance, float force, Vector2 dir)
	{
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		last_impact_is_local_body = Util.IsBodyLocal(__instance.body);
		if (SharedMain.local_world_is_generating)
		{
			return false;
		}
		if (!KrokoshaScavMultiplayer.network_system_is_running)
		{
			return true;
		}
		if (!force_run)
		{
			Body body = __instance.body;
			if (__instance.TryGetNetBody(out var nb))
			{
				if ((Object)(object)nb.piggybacking_on != (Object)null)
				{
					return false;
				}
				if (!nb.is_player)
				{
					return true;
				}
			}
			if (!Util.IsBodyLocal(body))
			{
				if (!KrokoshaScavMultiplayer.is_client)
				{
					return false;
				}
				if (!(force > body.jumpSpeed))
				{
					return false;
				}
				if (force > body.jumpSpeed * 1.66f)
				{
					body.DoFurTuft();
				}
				if (force > Random.Range(body.jumpSpeed * 1.55f, body.jumpSpeed * 4.4f) * ((__instance.muscleHealth < 50f) ? 0.632f : 1f) && force > body.jumpSpeed * 2.6f)
				{
					body.DoGoreSound();
				}
				Vector2 val = Vector2.op_Implicit(((Component)__instance).transform.position) + dir;
				ushort block = WorldGeneration.world.GetBlock(val);
				if ((float)(int)block > 0f)
				{
					Sound.Play(WorldGeneration.world.RandomStepSound(WorldGeneration.world.GetBlockInfo(block).stepsound), val, false, true, (Transform)null, 1f, 1f, false, false);
				}
				if (force > 3f && body.soundCooldown <= 0f)
				{
					body.soundCooldown = 0.25f;
					Sound.Play($"bodyFall{Random.Range(1, 6)}", Vector2.op_Implicit(((Component)__instance).transform.position), false, true, (Transform)null, 1f, 1f, false, false);
				}
				return false;
			}
		}
		return true;
	}

	public static void Postfix(Limb __instance, float force, Vector2 dir)
	{
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		if (!KrokoshaScavMultiplayer.network_system_is_running)
		{
			return;
		}
		if (!force_run && __instance.IsBodyLocal() && KrokoshaScavMultiplayer.is_client && Util.IsWorldGenerated())
		{
			Body body = __instance.body;
			if (!(force > body.jumpSpeed))
			{
				return;
			}
			if (body.TryGetNetBody(out var nb))
			{
				nb.SetNetHealthSyncIgnoreTime();
				if (Object.op_Implicit((Object)(object)nb.piggybacking_on))
				{
					return;
				}
			}
			NetDataWriter writer = Net.CreateWriter(10038);
			writer.Put(Util.GetLimbIndex(__instance));
			writer.Put(force);
			writer.Put(dir);
			Net.Client_Send((DeliveryMethod)0, in writer);
		}
		force_run = false;
	}
}
