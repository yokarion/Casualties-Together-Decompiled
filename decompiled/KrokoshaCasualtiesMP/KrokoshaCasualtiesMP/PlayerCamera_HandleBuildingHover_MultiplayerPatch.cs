using HarmonyLib;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(PlayerCamera), "HandleBuildingHover")]
internal static class PlayerCamera_HandleBuildingHover_MultiplayerPatch
{
	private static void Postfix(PlayerCamera __instance, BuildingEntity build)
	{
		PlayerCamera_HandleItemHover_MultiplayerPatch.SetDifferentThingamabob((Component)(object)build);
		ClientMain.DoVerboseObjInfo((Component)(object)build, ref GlobalDark.main.tooltipText.Item1, ref GlobalDark.main.tooltipText.Item2);
	}
}
