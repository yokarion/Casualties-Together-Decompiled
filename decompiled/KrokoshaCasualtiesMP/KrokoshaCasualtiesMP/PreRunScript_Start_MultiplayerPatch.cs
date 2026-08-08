using System.Collections.Generic;
using HarmonyLib;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(PreRunScript), "Start")]
public static class PreRunScript_Start_MultiplayerPatch
{
	private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
	{
		return SavesystemPatch.TranspilerToReplaceTheApplicationPath(instructions);
	}

	private static void Prefix()
	{
		SavesystemPatch.savedatapathreplacement = SavesystemPatch.GetTheOnePersistentDataPathWithSaveFile();
	}
}
