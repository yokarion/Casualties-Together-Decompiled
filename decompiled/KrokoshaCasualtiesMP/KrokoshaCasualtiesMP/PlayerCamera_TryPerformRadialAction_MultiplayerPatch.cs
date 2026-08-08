using HarmonyLib;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(PlayerCamera), "TryPerformRadialAction")]
public static class PlayerCamera_TryPerformRadialAction_MultiplayerPatch
{
	private static bool Prefix(ref bool __result)
	{
		if ((Object)(object)InvButton_get_body_MultiplayerPatch.focused_body != (Object)null)
		{
			__result = false;
			return false;
		}
		return true;
	}
}
