using HarmonyLib;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(GunScript), "Update")]
public static class GunScript_Update_MultiplayerPatch
{
	public static GunScript current_executing_gun;

	private static void Prefix(GunScript __instance)
	{
		current_executing_gun = __instance;
	}

	private static void Postfix(GunScript __instance)
	{
		current_executing_gun = null;
	}
}
