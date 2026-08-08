using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine.EventSystems;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(UIUtil), "IsPointerOverUIElement", new Type[] { typeof(List<RaycastResult>) })]
internal static class UIUtil_IsPointerOverUIElement_MultiplayerPatch
{
	private static void Postfix(UIUtil __instance, ref bool __result)
	{
		if (KrokoshaScavMultiplayer.network_system_is_running && !__result)
		{
			__result = UIInGame.interaction_menu_focused || UIBullshit.IS_CURSOR_OVERLAPPING || Chat.MouseIsInteractingWithScrollbar();
		}
	}
}
