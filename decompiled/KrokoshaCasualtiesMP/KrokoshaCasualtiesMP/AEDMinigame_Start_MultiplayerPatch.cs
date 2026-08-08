using HarmonyLib;
using KrokoshaCasualtiesUtils;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(AEDMinigame), "Start")]
public static class AEDMinigame_Start_MultiplayerPatch
{
	public static void Postfix(AEDMinigame __instance)
	{
		if (Net.running && __instance != null)
		{
			if (!__instance.limb.body.IsBodyLocal())
			{
				CPRHandler orAddComponent = ComponentHolderProtocol.GetOrAddComponent<CPRHandler>((Object)(object)MinigameBase.main);
				orAddComponent.pacient = __instance.limb.body;
				orAddComponent.MG_CreateSecondHand();
			}
		}
	}
}
