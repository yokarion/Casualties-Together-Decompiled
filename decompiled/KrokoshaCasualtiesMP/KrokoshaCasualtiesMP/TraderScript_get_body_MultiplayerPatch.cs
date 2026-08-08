using HarmonyLib;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(/*Could not decode attribute arguments.*/)]
public static class TraderScript_get_body_MultiplayerPatch
{
	public static void Postfix(TraderScript __instance, ref Body __result)
	{
		if (KrokoshaScavMultiplayer.network_system_is_running)
		{
			KrokoshaTraderTrackerComponent orAddComponent = ComponentHolderProtocol.GetOrAddComponent<KrokoshaTraderTrackerComponent>((Object)(object)__instance);
			if ((Object)(object)orAddComponent.focused_body == (Object)null)
			{
				orAddComponent.focused_body = PlayerCamera.main.body;
			}
			__result = orAddComponent.focused_body;
		}
	}
}
