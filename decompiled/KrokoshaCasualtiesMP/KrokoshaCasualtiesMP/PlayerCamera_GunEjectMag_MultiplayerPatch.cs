using HarmonyLib;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(PlayerCamera), "GunEjectMag")]
public static class PlayerCamera_GunEjectMag_MultiplayerPatch
{
	private static void Postfix(PlayerCamera __instance)
	{
		if (!KrokoshaScavMultiplayer.IsNetworkActiveAndIsClient() || !NetPlayer.LOCAL_PLAYER.HasGunEquipped(out var gun))
		{
			return;
		}
		KrokoshaScavMultiGameObjectNetworkTracker component = ((Component)gun).GetComponent<KrokoshaScavMultiGameObjectNetworkTracker>();
		if ((Object)(object)component != (Object)null)
		{
			if (log.verbose)
			{
				Plugin.log.LogInfo((object)$"local GunEjectMag {component.syncinfo}");
			}
			KrokoshaScavMultiplayer.Client_SendSimpleMessageToServer((ushort)10109);
			component.syncinfo.SetIgnoreTimeForRoundTrip(0.4000000059604645);
		}
	}
}
