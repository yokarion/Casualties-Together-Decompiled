using System.Collections.Generic;
using HarmonyLib;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(SaveSystem), "HasSave")]
public static class SaveSystem_HasSave_MultiplayerPatch
{
	internal static bool force;

	private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
	{
		return SavesystemPatch.TranspilerToReplaceTheApplicationPath(instructions);
	}

	private static bool Prefix(ref bool __result)
	{
		if (force)
		{
			__result = true;
			force = false;
			return false;
		}
		SavesystemPatch.savedatapathreplacement = SavesystemPatch.GetTheOnePersistentDataPathWithSaveFile();
		__result = SavesystemPatch.HasAnySaveFile();
		return false;
	}
}
