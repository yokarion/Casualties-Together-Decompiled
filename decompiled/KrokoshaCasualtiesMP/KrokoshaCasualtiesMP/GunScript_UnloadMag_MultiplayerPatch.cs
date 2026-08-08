using System.Collections.Generic;
using HarmonyLib;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(GunScript), "UnloadMag")]
public static class GunScript_UnloadMag_MultiplayerPatch
{
	private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
	{
		return GunScript_Fire_MultiplayerPatch.Transpiler(instructions);
	}

	public static void Prefix(GunScript __instance)
	{
		ComponentHolderProtocol.GetOrAddComponent<KrokoshaGunScriptTrackerComponent>((Object)(object)__instance);
	}
}
