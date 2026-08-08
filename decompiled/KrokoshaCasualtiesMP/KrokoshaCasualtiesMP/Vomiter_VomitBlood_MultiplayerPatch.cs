using HarmonyLib;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(Vomiter), "VomitBlood")]
public static class Vomiter_VomitBlood_MultiplayerPatch
{
	public static bool Prefix(Vomiter __instance)
	{
		return Vomiter_Vomit_MultiplayerPatch.VerifyAndAnnounceVomit(__instance, isblood: true);
	}
}
