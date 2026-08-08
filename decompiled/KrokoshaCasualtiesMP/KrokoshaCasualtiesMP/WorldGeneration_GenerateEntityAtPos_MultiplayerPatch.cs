using HarmonyLib;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(WorldGeneration), "GenerateEntityAtPos")]
public static class WorldGeneration_GenerateEntityAtPos_MultiplayerPatch
{
	public static void Prefix(WorldGeneration __instance, Vector2 pos, GameObject basObj)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		foreach (Transform item in basObj.transform)
		{
			ScavMultiBuildingSynchronizer.RegisterNonUniqueIdBg(item, report_fail: false);
		}
	}
}
