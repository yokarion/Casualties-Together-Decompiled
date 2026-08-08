using HarmonyLib;
using KrokoshaCasualtiesUtils;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(CoilScript), "Shock")]
internal static class CoilScript_Shock_MultiplayerPatch
{
	private static void Prefix(CoilScript __instance, Limb limb, ref JumpPadScript_OnCollisionEnter2D_MultiplayerPatch.ShakerState __state)
	{
		__state = new JumpPadScript_OnCollisionEnter2D_MultiplayerPatch.ShakerState(PlayerCamera.main.shaker);
	}

	private static void Postfix(CoilScript __instance, Limb limb, ref JumpPadScript_OnCollisionEnter2D_MultiplayerPatch.ShakerState __state)
	{
		if (!limb.IsBodyLocal())
		{
			__state.Apply(PlayerCamera.main.shaker);
		}
	}
}
