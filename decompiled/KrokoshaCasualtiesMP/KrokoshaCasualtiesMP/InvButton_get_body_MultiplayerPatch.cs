using HarmonyLib;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(/*Could not decode attribute arguments.*/)]
public static class InvButton_get_body_MultiplayerPatch
{
	public static Body focused_body;

	public static Vector2 focused_body_position_smooth = Vector2.zero;

	public static void Postfix(InvButton __instance, ref Body __result)
	{
		if (KrokoshaScavMultiplayer.network_system_is_running)
		{
			if (!PlayerCamera.main.radialOpen || (Object)(object)focused_body == (Object)(object)PlayerCamera.main.body)
			{
				focused_body = null;
			}
			__result = focused_body ?? PlayerCamera.main.body;
		}
	}
}
