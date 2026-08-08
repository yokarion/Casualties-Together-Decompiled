using HarmonyLib;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(BuildingEntity), "Backgroundify")]
public static class BuildingEntity_Backgroundify_MultiplayerPatch
{
	private static void Prefix(BuildingEntity __instance)
	{
		ComponentHolderProtocol.GetOrAddComponent<Krokosha_BuildingEntity_TrackerComponent_for_backgroundified>((Object)(object)__instance).is_backgroundified = true;
	}
}
