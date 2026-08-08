using HarmonyLib;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(TraderScript), "DropInventory")]
public static class TraderScript_DropInventory_MultiplayerPatch
{
	public static bool Prefix(TraderScript __instance)
	{
		if (KrokoshaScavMultiplayer.network_system_is_running && KrokoshaScavMultiplayer.is_client)
		{
			return false;
		}
		return true;
	}
}
