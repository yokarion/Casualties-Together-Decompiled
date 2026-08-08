using HarmonyLib;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(TraderScript), "Start")]
public static class TraderScript_Start_MultiplayerPatch
{
	public static void Postfix(TraderScript __instance)
	{
		ComponentHolderProtocol.GetOrAddComponent<KrokoshaTraderTrackerComponent>((Object)(object)__instance);
		if (KrokoshaScavMultiplayer.network_system_is_running)
		{
			__instance.didMove = false;
			__instance.farEnoughToMove = true;
		}
	}
}
