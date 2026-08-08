using System;
using HarmonyLib;
using KrokoshaCasualtiesUtils;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(TutorialHandler), "Start")]
public static class TutorialHandler_Start_MarkiplierPatch
{
	private static void Postfix(TutorialHandler __instance)
	{
		if (Net.running)
		{
			Util.DelayCallLambda(1f, (Action)delegate
			{
				Util.DoAlert("Multiplayer Tutorial is not done!\nsorry!", true);
			});
			__instance.StartCourse(typeof(SandboxCourse));
			__instance.courseSelectScreen.SetActive(false);
		}
	}
}
