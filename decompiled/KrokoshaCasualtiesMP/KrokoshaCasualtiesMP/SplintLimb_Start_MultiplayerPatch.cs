using HarmonyLib;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(SplintLimb), "Start")]
public static class SplintLimb_Start_MultiplayerPatch
{
	public static void Postfix(SplintLimb __instance)
	{
		if (KrokoshaScavMultiplayer.network_system_is_running)
		{
			__instance.conditionLossMinute += __instance.conditionLossMinute * KrokoshaScavMultiplayer.rules.AdditionalHealthDecay;
		}
	}
}
