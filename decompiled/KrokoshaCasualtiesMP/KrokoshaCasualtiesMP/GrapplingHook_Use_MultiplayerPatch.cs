using HarmonyLib;
using KrokoshaCasualtiesUtils;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(GrapplingHook), "Use")]
internal static class GrapplingHook_Use_MultiplayerPatch
{
	private static void Prefix(GrapplingHook __instance, Body body, ref JumpPadScript_OnCollisionEnter2D_MultiplayerPatch.ShakerState __state)
	{
		__state = new JumpPadScript_OnCollisionEnter2D_MultiplayerPatch.ShakerState(PlayerCamera.main.shaker);
	}

	private static void Postfix(GrapplingHook __instance, Body body, ref JumpPadScript_OnCollisionEnter2D_MultiplayerPatch.ShakerState __state)
	{
		if (!body.IsBodyLocal())
		{
			__state.Apply(PlayerCamera.main.shaker);
		}
	}
}
