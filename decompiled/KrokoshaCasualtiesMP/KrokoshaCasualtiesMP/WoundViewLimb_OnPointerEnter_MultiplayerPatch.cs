using HarmonyLib;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(WoundViewLimb), "OnPointerEnter")]
public static class WoundViewLimb_OnPointerEnter_MultiplayerPatch
{
	private static bool Prefix(WoundViewLimb __instance)
	{
		if (!__instance.woundview.body.limbs[__instance.limb].dismembered)
		{
			__instance.woundview.limbLookingAt = __instance.limb;
			__instance.woundview.limbImageFlash[__instance.limb] = 1f;
		}
		return false;
	}
}
