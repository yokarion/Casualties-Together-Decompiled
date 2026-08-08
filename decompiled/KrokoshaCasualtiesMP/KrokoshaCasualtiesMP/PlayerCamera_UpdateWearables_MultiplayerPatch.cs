using HarmonyLib;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(PlayerCamera), "UpdateWearables")]
public static class PlayerCamera_UpdateWearables_MultiplayerPatch
{
	private static void Prefix(PlayerCamera __instance, ref Body __state)
	{
		__state = __instance.body;
		if ((Object)(object)InvButton_get_body_MultiplayerPatch.focused_body != (Object)null)
		{
			__instance.body = InvButton_get_body_MultiplayerPatch.focused_body;
		}
	}

	private static void Postfix(PlayerCamera __instance, ref Body __state)
	{
		__instance.body = __state;
	}
}
