using System.Collections.Generic;
using HarmonyLib;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(TraderScript), "PromptResponse")]
internal static class TraderScript_PromptResponse_MultiplayerPatch
{
	private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
	{
		return TraderScript_TryHaggle_MultiplayerPatch.Transpiler(instructions);
	}
}
