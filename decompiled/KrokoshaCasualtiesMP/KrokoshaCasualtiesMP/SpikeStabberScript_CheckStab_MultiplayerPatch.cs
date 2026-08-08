using HarmonyLib;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(SpikeStabberScript), "CheckStab")]
internal static class SpikeStabberScript_CheckStab_MultiplayerPatch
{
	private static void Prefix(SpikeStabberScript __instance, ref JumpPadScript_OnCollisionEnter2D_MultiplayerPatch.ShakerState __state)
	{
		__state = new JumpPadScript_OnCollisionEnter2D_MultiplayerPatch.ShakerState(PlayerCamera.main.shaker);
	}

	private static void Postfix(SpikeStabberScript __instance, ref JumpPadScript_OnCollisionEnter2D_MultiplayerPatch.ShakerState __state)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (!KM.dist2dsqrcheck(Vector2.op_Implicit(((Component)__instance).transform.position), Vector2.op_Implicit(((Component)Camera.main).transform.position), 10f))
		{
			__state.Apply(PlayerCamera.main.shaker);
		}
	}
}
