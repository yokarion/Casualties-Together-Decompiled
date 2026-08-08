using HarmonyLib;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(ConsoleScript), "RegisterAllCommands")]
public static class PatchConsoleScriptRegister
{
	private static void Postfix()
	{
		Con._RegisterMultiplayerConsoleCommands();
	}
}
