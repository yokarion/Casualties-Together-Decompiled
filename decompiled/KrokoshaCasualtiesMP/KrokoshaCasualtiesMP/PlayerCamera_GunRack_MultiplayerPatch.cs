using HarmonyLib;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(PlayerCamera), "GunRack")]
public static class PlayerCamera_GunRack_MultiplayerPatch
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
			KrokoshaGunScriptTrackerComponent orAddComponent = ComponentHolderProtocol.GetOrAddComponent<KrokoshaGunScriptTrackerComponent>((Object)(object)gun);
			if (log.verbose)
			{
				Plugin.log.LogInfo((object)$"local GunRack {component.syncinfo}");
			}
			orAddComponent.send_racked_state = true;
			component.syncinfo.SetIgnoreTimeForRoundTrip(0.5);
		}
	}
}
