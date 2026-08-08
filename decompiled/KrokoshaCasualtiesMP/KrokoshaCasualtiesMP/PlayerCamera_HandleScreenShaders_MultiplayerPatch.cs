using HarmonyLib;
using KrokoshaCasualtiesUtils;
using UnityEngine;
using UnityEngine.UI;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(PlayerCamera), "HandleScreenShaders")]
public static class PlayerCamera_HandleScreenShaders_MultiplayerPatch
{
	public static bool forceDefaultShaderForNextFrame;

	public static bool CURRENTLY_OVERRIDING_VISUALS;

	public static bool ShouldOverrideVisualsToDefault()
	{
		if (!UIInGame.SPECTATOR_MODE && !forceDefaultShaderForNextFrame)
		{
			return Con._DEV_FREECAM;
		}
		return true;
	}

	private static void Postfix(PlayerCamera __instance)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		if (KrokoshaScavMultiplayer.network_system_is_running)
		{
			Body body = __instance.body;
			CURRENTLY_OVERRIDING_VISUALS = ShouldOverrideVisualsToDefault();
			if (CURRENTLY_OVERRIDING_VISUALS)
			{
				forceDefaultShaderForNextFrame = false;
				__instance.blackAmount = 0f;
				((Graphic)__instance.consciousnessFade).color = new Color(0f, 0f, 0f, 0f);
				((Graphic)__instance.consciousnessVignette).color = new Color(0f, 0f, 0f, 0f);
				__instance.consciousnessECG.active = false;
				__instance.mainScreenPass.SetFloat("_Pain", 0f);
				__instance.mainScreenPass.SetFloat("_LowOxyAmt", 0f);
				__instance.mainScreenPass.SetFloat("_BloodAmount", 1f);
				__instance.mainScreenPass.SetFloat("_Stamina", 1f);
				__instance.mainScreenPass.SetFloat("_Consciousness", 1f);
				__instance.mainScreenPass.SetFloat("_Consiousness", 1f);
				__instance.mainScreenPass.SetFloat("_FrostAmount", 0f);
				__instance.mainScreenPass.SetFloat("_OverheatAmount", 0f);
				__instance.mainScreenPass.SetFloat("_BadFocusAmount", 0f);
				__instance.mainScreenPass.SetFloat("_InsanityFX", 0f);
				__instance.mainScreenPass.SetFloat("_BrightMult", 1f);
				__instance.mainScreenPass.SetVector("_PlayerPos", Vector4.op_Implicit(((Component)PlayerCamera.main).transform.position));
				__instance.mainScreenPass.SetFloat("_Dirtyness", 0f);
				__instance.mainScreenPass.SetFloat("_SaturationMult", 1f);
				__instance.blurPass.SetFloat("_BlurIntensity", 0f);
				__instance.blurPass.SetFloat("_DropletAmount", 0f);
				__instance.blurPass.SetFloat("_HungerIntensity", 0f);
				__instance.blurPass.SetFloat("_SicknessAmount", 0f);
				__instance.blurPass.SetFloat("_Insanity", 0f);
				__instance.blurPass.SetFloat("_WaterAmount", 0f);
				__instance.bonusPass.SetFloat("_Intensity", 0f);
				__instance.bonusPass.SetFloat("_Distort", 0f);
				__instance.bonusPass.SetFloat("_FlipScreen", 0f);
				__instance.damagePass.SetInt("_EyeDamage", 0);
				__instance.damagePass.SetFloat("_BrainDamage", 0f);
				__instance.damagePass.SetFloat("_Bloodloss", 0f);
				__instance.damagePass.SetVector("_Pos", Vector4.op_Implicit(((Component)PlayerCamera.main).transform.position));
				__instance.damagePass.SetFloat("_DirectionMult", 0f);
				__instance.damagePass.SetFloat("_RadAmount", 0f);
				WorldGeneration.world.fogMat.SetVector("_PlayerPos", Vector4.op_Implicit(((Component)PlayerCamera.main).transform.position));
				if (!body.alive)
				{
					((Component)PlayerCamera.main.endScreen).gameObject.SetActive(false);
				}
				WorldGeneration.world.soundMixerGroup.audioMixer.SetFloat("SoundCutoff", 22000f);
				__instance.underWaterSource.volume = 0f;
			}
			else if (!Util.IsUnchipped() || PlayerCamera_SetIrradiateIntensity_MultiplayerPatch.forceSetIrradiateIntensity)
			{
				PlayerCamera_SetIrradiateIntensity_MultiplayerPatch.forceSetIrradiateIntensity = false;
				if (NetPlayer.TryGetLocalNetBody(out var nb))
				{
					__instance.damagePass.SetFloat("_RadAmount", Mathf.Clamp01(nb.irradiateIntensity));
					PlayerCamera.main.irradiateIntensity = nb.irradiateIntensity;
					PlayerCamera.main.timeSinceRadUpdate = nb.timeSinceRadUpdate;
				}
			}
		}
		else
		{
			CURRENTLY_OVERRIDING_VISUALS = false;
		}
	}
}
