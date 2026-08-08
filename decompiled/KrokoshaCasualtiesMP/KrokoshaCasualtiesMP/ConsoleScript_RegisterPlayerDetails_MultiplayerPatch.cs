using HarmonyLib;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(ConsoleScript), "RegisterPlayerDetails")]
public static class ConsoleScript_RegisterPlayerDetails_MultiplayerPatch
{
	private static void Prefix(ConsoleScript __instance, ref bool __state)
	{
		__state = __instance.playerDetailsRegistered;
	}

	private static void Postfix(ConsoleScript __instance, ref bool __state)
	{
		if (!__state)
		{
			ConsoleScript.SearchExact("setlimbfield").argAutofill[1].Add("all");
		}
	}
}
