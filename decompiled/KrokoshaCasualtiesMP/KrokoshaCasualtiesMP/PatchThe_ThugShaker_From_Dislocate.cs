using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(Limb), "Dislocate")]
public static class PatchThe_ThugShaker_From_Dislocate
{
	public static void Prefix(Limb __instance)
	{
		if (!__instance.dislocated)
		{
			NetBody netBody = default(NetBody);
			((Component)__instance.body).TryGetComponent<NetBody>(ref netBody);
		}
	}

	private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
	{
		return PatchThe_ThugShaker_From_BreakBone.Transpiler(instructions);
	}
}
