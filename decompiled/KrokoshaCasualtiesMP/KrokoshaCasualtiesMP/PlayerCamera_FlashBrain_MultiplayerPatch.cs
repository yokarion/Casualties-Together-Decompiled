using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(PlayerCamera), "FlashBrain")]
public static class PlayerCamera_FlashBrain_MultiplayerPatch
{
	public static bool force;

	public static void ForceFlashBrain(this PlayerCamera pc)
	{
		force = true;
		((MonoBehaviour)pc).StartCoroutine(pc.FlashBrain());
	}

	private static bool Prefix(PlayerCamera __instance, ref IEnumerator __result)
	{
		if (!KrokoshaScavMultiplayer.network_system_is_running)
		{
			return true;
		}
		if (!force)
		{
			__result = new List<object>().GetEnumerator();
			return false;
		}
		force = false;
		return true;
	}
}
