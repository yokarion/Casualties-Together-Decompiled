using HarmonyLib;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(WorldGeneration), "ContinueRun")]
public static class WorldGenerationContinueRunPatch
{
	public static bool Prefix(WorldGeneration __instance)
	{
		ServerMain._ded_server_switch_counter = -20f;
		if (KrokoshaScavMultiplayer.network_system_is_running)
		{
			PlayerCamera.main.body.forceWalk = false;
			if (KrokoshaScavMultiplayer.is_client)
			{
				PlayerCamera.main.DoAlert(Lang.Get("wait_for_host", false), false);
				return false;
			}
			if (!ServerMain.CheckIfEnoughPeopleAreAtLayerFinish())
			{
				PlayerCamera.main.DoAlert(Lang.Get("layerfinish_not_enough_plrs", false), false);
				__instance.savePanel.SetActive(false);
				return false;
			}
			if (!(bool)Traverse.Create((object)__instance).Field("doingRegen").GetValue() && !__instance.generatingWorld && __instance.worldExists)
			{
				KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog("Host pressed ContinueRun button and everything checks out! ");
				Chat.Server_ChatAnnouncement(Lang.Get("host_continue_run_announcement", false));
				__instance.savePanel.SetActive(false);
				((MonoBehaviour)__instance).StartCoroutine(__instance.RegenerateWorld(false));
				return false;
			}
			return false;
		}
		return true;
	}
}
