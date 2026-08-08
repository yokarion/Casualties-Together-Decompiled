using System;
using HarmonyLib;
using KrokoshaCasualtiesUtils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(PlayerCamera), "Update")]
public static class PlayerCameraUpdate_Update_MultiplayerPatch
{
	private static bool prev_radialOpen;

	public static void Prefix(PlayerCamera __instance)
	{
		if (KrokoshaScavMultiplayer.network_system_is_running)
		{
			prev_radialOpen = __instance.radialOpen;
		}
	}

	public static void Postfix(PlayerCamera __instance)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		if (Con._DEV_HIDEHUD)
		{
			GlobalDark main = GlobalDark.main;
			if (main != null)
			{
				main.SetTooltip(("", ""));
			}
			((Renderer)__instance.hoverSquare).enabled = false;
		}
		if (!Net.running)
		{
			return;
		}
		_ = __instance.body;
		if (!prev_radialOpen && __instance.radialOpen)
		{
			UIInGame.StopPlayerInteractionMenu();
			return;
		}
		if (Input.GetKeyDown(KeyBinds.GetBind("iteminteract")) && (Object)(object)UIInGame.interaction_menu_target_body != (Object)null && !UIInGame.interaction_menu_focused && !PlayerCamera_HandlePlayerHover_MultiplayerPatch.is_hovering_over_another_player)
		{
			UIInGame.StopPlayerInteractionMenu();
		}
		if (__instance.radialOpen)
		{
			if (Util.IsInWoundView())
			{
				__instance.radialOpen = false;
			}
		}
		else
		{
			InvButton_get_body_MultiplayerPatch.focused_body = null;
		}
		try
		{
			Transform val = ((Transform)__instance.deathStats).Find("Return");
			if ((Object)(object)val != (Object)null)
			{
				((Component)val).gameObject.SetActive(Net.is_server);
			}
		}
		catch (Exception)
		{
		}
	}
}
