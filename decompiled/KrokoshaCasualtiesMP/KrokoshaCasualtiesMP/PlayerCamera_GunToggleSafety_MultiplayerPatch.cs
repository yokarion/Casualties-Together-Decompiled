using HarmonyLib;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(PlayerCamera), "GunToggleSafety")]
public static class PlayerCamera_GunToggleSafety_MultiplayerPatch
{
	private static void Postfix(PlayerCamera __instance)
	{
		if (!KrokoshaScavMultiplayer.IsNetworkActiveAndIsClient() || !NetPlayer.LOCAL_PLAYER.HasGunEquipped(out var gun))
		{
			return;
		}
		KrokoshaScavMultiGameObjectNetworkTracker orAddComponent = ComponentHolderProtocol.GetOrAddComponent<KrokoshaScavMultiGameObjectNetworkTracker>((Object)(object)gun);
		if ((Object)(object)orAddComponent != (Object)null)
		{
			if (log.verbose)
			{
				Plugin.log.LogInfo((object)"local GunToggleSafety ");
			}
			KrokoshaScavMultiplayer.Client_SendSimpleMessageToServer((ushort)10110, gun.safe, true);
			orAddComponent.syncinfo.SetIgnoreTimeForRoundTrip(0.4000000059604645);
		}
	}
}
