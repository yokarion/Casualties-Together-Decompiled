using HarmonyLib;

namespace KrokoshaCasualtiesUtils;

[HarmonyPatch(typeof(SettingsMenu), "Start")]
internal static class SettingsMenu_Start_Patch
{
	public static bool forceOpenCustomTab;

	private static bool Prefix(SettingsMenu __instance)
	{
		if (forceOpenCustomTab)
		{
			forceOpenCustomTab = false;
			return false;
		}
		return true;
	}
}
