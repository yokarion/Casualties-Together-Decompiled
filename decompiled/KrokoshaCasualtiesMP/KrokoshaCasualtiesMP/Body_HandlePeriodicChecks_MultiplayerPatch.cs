using HarmonyLib;
using KrokoshaCasualtiesUtils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(Body), "HandlePeriodicChecks")]
public static class Body_HandlePeriodicChecks_MultiplayerPatch
{
	private static void Prefix(Body __instance, ref bool __state)
	{
		__state = __instance.halfMinuteCheckTime + Time.deltaTime > 30f;
		if (__state && __instance.brainHealth < 95f && Util.IsBodyLocal(__instance) && __instance.consciousness > 20f && (double)Random.value > (double)__instance.brainHealth * 0.01 && !PlayerCamera.main.lastStandPanel.activeSelf && !UIInGame.SPECTATOR_MODE)
		{
			PlayerCamera.main.ForceFlashBrain();
		}
	}

	private static void Postfix(Body __instance, ref bool __state)
	{
		if (__state && __instance.brainHealth < 95f && !Util.IsBodyLocal(__instance) && PlayerCamera.main.threatMusicTime == 8f)
		{
			PlayerCamera.main.threatMusicTime = 0f;
		}
	}
}
