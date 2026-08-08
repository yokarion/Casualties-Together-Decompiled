using HarmonyLib;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(PlayerCamera), "OpenCraftScreen")]
public static class PlayerCamera_OpenCraftScreen_MultiplayerPatch
{
	public static bool Prefix(PlayerCamera __instance)
	{
		if (KrokoshaScavMultiplayer.network_system_is_running && !__instance.craftingPanel.activeSelf && UIInGame.SPECTATOR_MODE)
		{
			return false;
		}
		return true;
	}
}
