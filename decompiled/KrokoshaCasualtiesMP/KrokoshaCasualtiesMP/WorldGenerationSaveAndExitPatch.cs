using HarmonyLib;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(WorldGeneration), "SaveAndExit")]
public static class WorldGenerationSaveAndExitPatch
{
	public static bool Prefix(WorldGeneration __instance)
	{
		if (KrokoshaScavMultiplayer.network_system_is_running)
		{
			PlayerCamera.main.body.forceWalk = false;
			if (KrokoshaScavMultiplayer.is_client)
			{
				PlayerCamera.main.DoAlert(Lang.Get("wait_for_host", false), false);
				return false;
			}
		}
		return true;
	}
}
