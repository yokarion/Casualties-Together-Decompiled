using HarmonyLib;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(TraderScript), "OnUse")]
public static class TraderScript_OnUse_MultiplayerPatch
{
	public static bool Prefix(TraderScript __instance)
	{
		return true;
	}
}
