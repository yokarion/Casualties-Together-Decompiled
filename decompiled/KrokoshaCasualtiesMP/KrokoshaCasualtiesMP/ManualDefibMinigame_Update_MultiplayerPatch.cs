using HarmonyLib;
using KrokoshaCasualtiesUtils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(ManualDefibMinigame), "Update")]
public static class ManualDefibMinigame_Update_MultiplayerPatch
{
	public static void Postfix(ManualDefibMinigame __instance)
	{
		if (Net.running && __instance.onTorso && (Object)(object)__instance.limb != (Object)null && !__instance.limb.IsBodyLocal())
		{
			__instance.ecg.followBody = false;
			__instance.ecg.writeHeight = __instance.limb.body.GetECGHeight(__instance.ecg.timeToUpdate - 0.028f);
		}
	}
}
