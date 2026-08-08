using HarmonyLib;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(Heater), "OnWillRenderObject")]
public static class Heater_OnWillRenderObject_MultiplayerPatch
{
	public static bool Prefix(Heater __instance)
	{
		if (KrokoshaScavMultiplayer.network_system_is_running)
		{
			return false;
		}
		return true;
	}
}
