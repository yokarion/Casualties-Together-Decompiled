using HarmonyLib;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(Limb), "FurBloodUpdate")]
public static class Limb_FurBloodUpdate_MultiplayerPatch
{
	public static void Postfix(Limb __instance)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		if (!(__instance.body.bloodVolume <= 0f) && !(__instance.body.totalBleedSpeed <= 0f))
		{
			return;
		}
		_ = ((Renderer)((Component)__instance).GetComponent<SpriteRenderer>()).sharedMaterial;
		ParticleSystem[] componentsInChildren = ((Component)__instance).GetComponentsInChildren<ParticleSystem>();
		foreach (ParticleSystem val in componentsInChildren)
		{
			if (((Object)val).name.Contains("BleedParticle"))
			{
				EmissionModule emission = val.emission;
				((EmissionModule)(ref emission)).rateOverTime = MinMaxCurve.op_Implicit(0f);
			}
		}
	}
}
