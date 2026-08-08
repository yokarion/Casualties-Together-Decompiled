using HarmonyLib;
using KrokoshaCasualtiesUtils;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(AmputationMinigame), "Update")]
internal static class AmputationMinigame_Update_MultiplayerPatch
{
	private static float og_adrenaline;

	public static void AmputationMinigame_FinishAmputateCopypasted(Limb limb)
	{
		if (!KrokoshaScavMultiplayer.is_client)
		{
			if (limb.body.IsDeadOrCriticallyDying() || limb.infectionAmount < 90f)
			{
				CombatStuff.BetterDismember(limb);
			}
			else
			{
				limb.Dismember();
			}
		}
		Limb[] connectedLimbs = limb.connectedLimbs;
		foreach (Limb obj in connectedLimbs)
		{
			obj.infected = false;
			obj.infectionAmount = 0f;
			obj.SetDisinfect(300f);
			obj.bleedAmount *= 0.5f;
		}
		Body body = limb.body;
		body.traumaAmount -= 20f;
	}

	private static bool Prefix(AmputationMinigame __instance)
	{
		if (!KrokoshaScavMultiplayer.network_system_is_running)
		{
			return true;
		}
		Limb limb = __instance.limb;
		og_adrenaline = PlayerCamera.main.body.adrenaline;
		if (__instance.cutProgress >= 1f)
		{
			AmputationMinigame_FinishAmputateCopypasted(limb);
			Minigame.game.EndMinigame();
			return false;
		}
		return true;
	}

	private static void Postfix(AmputationMinigame __instance)
	{
		PlayerCamera.main.body.adrenaline = og_adrenaline;
	}
}
