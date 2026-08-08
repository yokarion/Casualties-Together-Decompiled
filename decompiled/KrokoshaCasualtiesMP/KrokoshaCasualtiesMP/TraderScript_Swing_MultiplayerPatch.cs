using HarmonyLib;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(TraderScript), "Swing")]
internal static class TraderScript_Swing_MultiplayerPatch
{
	private static void Postfix(TraderScript __instance)
	{
		if (KrokoshaScavMultiplayer.network_system_is_running && !KrokoshaScavMultiplayer.is_client && NetObjectRegistry.TryGetSyncInfo((Component)(object)__instance, out var si))
		{
			NetObjectRegistry.Server_QueueSync(si);
		}
	}
}
