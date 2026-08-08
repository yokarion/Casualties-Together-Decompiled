using HarmonyLib;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(WorldGeneration), "IncreaseDepthByLayer")]
public static class WorldGeneration_IncreaseDepthByLayer_MultiplayerPatch
{
	public static void Postfix(WorldGeneration __instance)
	{
		if (!KrokoshaScavMultiplayer.network_system_is_running)
		{
			return;
		}
		foreach (NetPlayer allLivingPlayer in NetPlayer.AllLivingPlayers)
		{
			if (!allLivingPlayer.is_local)
			{
				allLivingPlayer.body.skills.AddExp(2, 30f);
			}
		}
	}
}
