using System;
using System.Linq;
using HarmonyLib;
using KrokoshaCasualtiesUtils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(Sound), "Play", new Type[]
{
	typeof(AudioClip),
	typeof(Vector2),
	typeof(bool),
	typeof(bool),
	typeof(Transform),
	typeof(float),
	typeof(float),
	typeof(bool),
	typeof(bool)
})]
internal static class Sound_Play2_MultiplayerPatch
{
	private static float default_max_range = 160f;

	private static float default_max_range_sqr = default_max_range * default_max_range;

	public static bool force = false;

	[HarmonyReversePatch(/*Could not decode attribute arguments.*/)]
	public static AudioSource Play(AudioClip clip, Vector2 pos, bool twoDimensional, bool pitchShift, Transform follow, float volume, float pitch, bool noReverb, bool ignoreMixer)
	{
		throw new NotImplementedException();
	}

	private static bool Prefix(AudioClip clip, ref Vector2 pos, ref bool twoDimensional, bool pitchShift, Transform follow, float volume, float pitch, bool noReverb, bool ignoreMixer, ref AudioSource __result)
	{
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		if (force)
		{
			force = false;
			return true;
		}
		if (!KrokoshaScavMultiplayer.network_system_is_running)
		{
			return true;
		}
		if ((Object)(object)clip == (Object)null)
		{
			return false;
		}
		bool flag = twoDimensional;
		if (twoDimensional)
		{
			if ((Object)(object)Body_StartClimbing_MultiplayerPatch.last_interacted_climbable != (Object)null && Body_StartClimbing_MultiplayerPatch.last_interacted_climbable.climbSounds != null && Body_StartClimbing_MultiplayerPatch.last_interacted_climbable.climbSounds.Contains(clip))
			{
				if (!((Object)(object)Body_StartClimbing_MultiplayerPatch.last_interacted_body != (Object)null) || Body_StartClimbing_MultiplayerPatch.last_interacted_body.IsBodyLocal())
				{
					return true;
				}
				pos = Vector2.op_Implicit(((Component)Body_StartClimbing_MultiplayerPatch.last_interacted_body).transform.position);
				twoDimensional = false;
			}
			else
			{
				if ((!((Object)clip).name.Contains("gun") && !((Object)clip).name.StartsWith("shot") && !((Object)clip).name.StartsWith("pistol") && !((Object)clip).name.StartsWith("rifle")) || !((Object)(object)GunScript_Update_MultiplayerPatch.current_executing_gun != (Object)null))
				{
					return true;
				}
				KrokoshaGunScriptTrackerComponent krokoshaGunScriptTrackerComponent = default(KrokoshaGunScriptTrackerComponent);
				if (((Component)GunScript_Update_MultiplayerPatch.current_executing_gun).TryGetComponent<KrokoshaGunScriptTrackerComponent>(ref krokoshaGunScriptTrackerComponent) && krokoshaGunScriptTrackerComponent.body.IsBodyLocal())
				{
					twoDimensional = true;
				}
				else
				{
					if (((Object)clip).name.EndsWith("shot"))
					{
						pos = Vector2.LerpUnclamped(Vector2.op_Implicit(((Component)GunScript_Update_MultiplayerPatch.current_executing_gun).transform.position), Vector2.op_Implicit(((Component)Camera.main).transform.position), 0.3f);
					}
					twoDimensional = false;
				}
			}
		}
		else if (Util.IsInWorld() && PlayerCamera.main.body.GetPantSound().barkSounds.Contains(clip))
		{
			pos = Vector2.LerpUnclamped(pos, Vector2.op_Implicit(((Component)Camera.main).transform.position), 0.7f);
			twoDimensional = false;
		}
		if (true)
		{
			float num = KM.dist2dsqr(in pos, Vector2.op_Implicit(((Component)Camera.main).transform.position));
			float num2 = 1f;
			if (!twoDimensional && flag)
			{
				volume *= Sound_Play_MultiplayerPatch.nontwoDimensionalvolumemultiplier;
			}
			if (num < default_max_range_sqr)
			{
				pos = Vector2.LerpUnclamped(pos, Vector2.op_Implicit(((Component)Camera.main).transform.position), 0.3f);
				AudioSource val = Play(clip, pos, twoDimensional, pitchShift, follow, volume * num2, pitch, noReverb, ignoreMixer);
				__result = val;
			}
			return false;
		}
		return true;
	}
}
