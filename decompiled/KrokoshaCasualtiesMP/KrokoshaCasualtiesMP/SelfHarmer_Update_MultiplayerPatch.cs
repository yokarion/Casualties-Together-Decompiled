using HarmonyLib;
using KrokoshaCasualtiesUtils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(SelfHarmer), "Update")]
public static class SelfHarmer_Update_MultiplayerPatch
{
	public static void Postfix(SelfHarmer __instance)
	{
		if (KrokoshaScavMultiplayer.network_system_is_running && !Util.IsBodyLocal(((Component)__instance).GetComponent<Body>()) && __instance.scarySource.isPlaying)
		{
			__instance.scarySource.Stop();
			__instance.scarySource.volume = 0f;
			__instance.scarySource.loop = false;
		}
	}
}
