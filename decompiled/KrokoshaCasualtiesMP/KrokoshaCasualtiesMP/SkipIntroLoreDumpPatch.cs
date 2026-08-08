using HarmonyLib;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(PreRunScript), "TryLore")]
public static class SkipIntroLoreDumpPatch
{
	private static void Prefix()
	{
		if (Plugin.юзер_прошаренный)
		{
			PreRunScript.didIntro = true;
		}
	}
}
