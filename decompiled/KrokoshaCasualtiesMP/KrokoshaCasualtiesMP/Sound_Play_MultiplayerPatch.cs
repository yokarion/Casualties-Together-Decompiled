using System;
using HarmonyLib;
using KrokoshaCasualtiesUtils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(Sound), "Play", new Type[]
{
	typeof(string),
	typeof(Vector2),
	typeof(bool),
	typeof(bool),
	typeof(Transform),
	typeof(float),
	typeof(float),
	typeof(bool),
	typeof(bool)
})]
internal static class Sound_Play_MultiplayerPatch
{
	public static bool is_the_fucking_updateheart_shit = false;

	public static bool force = false;

	public static float nontwoDimensionalvolumemultiplier = 1.5f;

	private static bool Prefix(ref string clip, ref Vector2 pos, ref bool twoDimensional, ref bool pitchShift, ref Transform follow, ref float volume, float pitch, bool noReverb, bool ignoreMixer)
	{
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		if (force)
		{
			force = false;
			return true;
		}
		if (!KrokoshaScavMultiplayer.network_system_is_running)
		{
			return true;
		}
		if (clip == null)
		{
			return false;
		}
		if (is_the_fucking_updateheart_shit)
		{
			is_the_fucking_updateheart_shit = false;
			return false;
		}
		if (clip == "harmSting" && Util.TryGetLocalBody(out var body) && body.happiness > -70f)
		{
			return false;
		}
		if (clip == "caveticks" || (clip == "headhit" && !Limb_ImpactDamage_MultiplayerPatch.last_impact_is_local_body))
		{
			return false;
		}
		bool flag = twoDimensional;
		if (clip.StartsWith("exert") || clip == "Saw")
		{
			twoDimensional = false;
		}
		else if (WorldGeneration_CreateExplosion_MultiplayerPatch.is_inside_CreateExplosion && WorldGeneration_CreateExplosion_MultiplayerPatch.last_explosion_params != null && WorldGeneration_CreateExplosion_MultiplayerPatch.last_explosion_params.sound == clip)
		{
			pos = Vector2.LerpUnclamped(WorldGeneration_CreateExplosion_MultiplayerPatch.last_explosion_params.position, Vector2.op_Implicit(((Component)Camera.main).transform.position), 0.8f);
			twoDimensional = false;
		}
		else if (clip == "fireworkpop")
		{
			Vector3 position = ((Component)Camera.main).transform.position;
			if (pos.y > position.y)
			{
				pos = new Vector2(Mathf.Lerp(pos.x, position.x, 0.7f), Mathf.Lerp(pos.y, position.y, 0.87f));
			}
			volume *= nontwoDimensionalvolumemultiplier;
			twoDimensional = false;
		}
		if (!twoDimensional && flag)
		{
			volume *= nontwoDimensionalvolumemultiplier;
		}
		return true;
	}
}
