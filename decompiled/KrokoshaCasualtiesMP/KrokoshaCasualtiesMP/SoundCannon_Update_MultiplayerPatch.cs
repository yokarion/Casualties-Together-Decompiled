using HarmonyLib;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(SoundCannon), "Update")]
public static class SoundCannon_Update_MultiplayerPatch
{
	public static bool Prefix(SoundCannon __instance)
	{
		if (KrokoshaScavMultiplayer.network_system_is_running)
		{
			ComponentHolderProtocol.GetOrAddComponent<KrokoshaSoundCannonNetworkTrackerComponent>((Object)(object)__instance);
			return false;
		}
		return true;
	}
}
