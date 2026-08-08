using HarmonyLib;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(FreshItemDrop), "OnDestroy")]
internal static class FreshItemDrop_OnDestroy_ErrorfixPatch
{
	private static bool Prefix(FreshItemDrop __instance)
	{
		__instance.rb.gravityScale = 1f;
		if ((Object)(object)__instance.part != (Object)null)
		{
			Object.Destroy((Object)(object)((Component)__instance.part).gameObject);
		}
		if ((Object)(object)__instance.outline != (Object)null)
		{
			Object.Destroy((Object)(object)((Component)__instance.outline).gameObject);
		}
		return false;
	}
}
