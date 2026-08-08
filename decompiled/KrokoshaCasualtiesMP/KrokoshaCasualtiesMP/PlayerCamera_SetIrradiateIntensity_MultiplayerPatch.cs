using HarmonyLib;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(PlayerCamera), "SetIrradiateIntensity")]
public static class PlayerCamera_SetIrradiateIntensity_MultiplayerPatch
{
	public static bool forceSetIrradiateIntensity;

	public static void RealSetIrradiateIntensity(float rad)
	{
		if (NetPlayer.TryGetLocalNetBody(out var nb))
		{
			RealSetIrradiateIntensity(nb, rad);
		}
	}

	public static void RealSetIrradiateIntensity(NetBody nb, float rad)
	{
		nb.irradiateIntensity = Mathf.Max(nb.irradiateIntensity, rad);
		if (rad >= nb.irradiateIntensity)
		{
			nb.timeSinceRadUpdate = 0f;
		}
	}

	private static bool Prefix(PlayerCamera __instance)
	{
		if (KrokoshaScavMultiplayer.network_system_is_running)
		{
			return false;
		}
		return true;
	}
}
