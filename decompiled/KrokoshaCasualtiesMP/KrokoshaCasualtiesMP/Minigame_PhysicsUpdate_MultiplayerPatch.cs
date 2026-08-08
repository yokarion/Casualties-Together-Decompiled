using HarmonyLib;
using KrokoshaCasualtiesUtils;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(Minigame), "PhysicsUpdate")]
public static class Minigame_PhysicsUpdate_MultiplayerPatch
{
	public static void Postfix(Minigame __instance, ref float deltaTime)
	{
		if (Net.running)
		{
			AEDMinigame val = (AEDMinigame)(object)((__instance is AEDMinigame) ? __instance : null);
			if (val != null && !val.limb.body.IsBodyLocal())
			{
				CPRHandler orAddComponent = ComponentHolderProtocol.GetOrAddComponent<CPRHandler>((Object)(object)MinigameBase.main);
				orAddComponent.pacient = val.limb.body;
				orAddComponent.MG_PhysicsUpdate(deltaTime);
			}
		}
	}
}
