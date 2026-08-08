using HarmonyLib;
using UnityEngine.UI;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(PauseHandler), "Update")]
public static class PauseHandler_Update_MultiplayerPatch
{
	private static void Postfix(PauseHandler __instance)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (KrokoshaScavMultiplayer.network_system_is_running)
		{
			Image background = __instance.background;
			((Graphic)background).color = ((Graphic)background).color * 0.5f;
		}
	}
}
