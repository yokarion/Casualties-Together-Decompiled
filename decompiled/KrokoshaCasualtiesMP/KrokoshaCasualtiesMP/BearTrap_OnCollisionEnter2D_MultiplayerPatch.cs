using HarmonyLib;
using KrokoshaCasualtiesUtils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(BearTrap), "OnTriggerEnter2D")]
internal static class BearTrap_OnCollisionEnter2D_MultiplayerPatch
{
	private static void Prefix(BearTrap __instance, Collider2D other, ref JumpPadScript_OnCollisionEnter2D_MultiplayerPatch.ShakerState __state)
	{
		__state = new JumpPadScript_OnCollisionEnter2D_MultiplayerPatch.ShakerState(PlayerCamera.main.shaker);
	}

	private static void Postfix(BearTrap __instance, Collider2D other, ref JumpPadScript_OnCollisionEnter2D_MultiplayerPatch.ShakerState __state)
	{
		Limb limb = default(Limb);
		if ((Object)(object)other != (Object)null && ((Component)other).TryGetComponent<Limb>(ref limb) && !limb.IsBodyLocal())
		{
			__state.Apply(PlayerCamera.main.shaker);
		}
	}
}
