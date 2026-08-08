using HarmonyLib;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(PlayerCamera), "ItemHoverDescription")]
internal static class PlayerCamera_ItemHoverDescription_MultiplayerPatch
{
	public static void Postfix(Item item, ref (string, string) __result)
	{
		ClientMain.DoVerboseObjInfo((Component)(object)item, ref __result.Item1, ref __result.Item2);
	}
}
