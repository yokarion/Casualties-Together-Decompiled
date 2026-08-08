using HarmonyLib;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(GunScript), "Start")]
public static class GunScript_Start_MultiplayerPatch
{
	private static void Postfix(GunScript __instance)
	{
		ComponentHolderProtocol.GetOrAddComponent<KrokoshaGunScriptTrackerComponent>((Object)(object)__instance);
		__instance.lastRacked = __instance.racked;
	}
}
