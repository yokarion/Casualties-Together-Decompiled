using HarmonyLib;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(ConsoleScript), "ParseBool")]
public static class ConsoleScript_ParseBool_MultiplayerPatch
{
	public static bool Prefix(ConsoleScript __instance, string s, ref bool __result)
	{
		s = s.ToLower();
		if (bool.TryParse(s, out __result))
		{
			return false;
		}
		if (int.TryParse(s, out var result))
		{
			__result = true;
			if (result == 0)
			{
				__result = false;
			}
			return false;
		}
		return true;
	}
}
