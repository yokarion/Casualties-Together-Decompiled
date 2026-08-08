using System.Collections.Generic;
using HarmonyLib;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(PlayerCamera), "RefreshTraderInventories")]
internal static class PlayerCamera_RefreshTraderInventories_MultiplayerPatch
{
	public static GameObject RECRUITButton => PlayerCamera_ToggleTradeMenu_MultiplayerPatch.RECRUITButton;

	private static bool Prefix(PlayerCamera __instance)
	{
		if (!Net.running)
		{
			return true;
		}
		if ((Object)(object)RECRUITButton != (Object)null && (Object)(object)__instance.currentTrader != (Object)null)
		{
			bool flag = ComponentHolderProtocol.GetOrAddComponent<KrokoshaTraderTrackerComponent>((Object)(object)__instance.currentTrader).CanBeRecruited_ForRespawn();
			NetPlayer netPlayer = null;
			if (Net.is_server)
			{
				netPlayer = PlayerCamera_ToggleTradeMenu_MultiplayerPatch.Server_SelectMostSuitablePlayerToRespawn();
			}
			else
			{
				List<NetPlayer> playersEligibleForRespawn = PlayerCamera_ToggleTradeMenu_MultiplayerPatch.GetPlayersEligibleForRespawn();
				if (playersEligibleForRespawn.Count > 0)
				{
					netPlayer = playersEligibleForRespawn[0];
				}
			}
			if ((Object)(object)netPlayer == (Object)null)
			{
				flag = false;
			}
			UITooltip component = RECRUITButton.GetComponent<UITooltip>();
			((Selectable)((Component)component).GetComponent<Button>()).interactable = flag;
			component.skipLocale = true;
			component.tipName = Lang.Get("trader_recruit", false);
			if (flag)
			{
				component.tipDesc = Lang.Get("trader_recruitdesc", false);
			}
			else if ((Object)(object)netPlayer == (Object)null)
			{
				component.tipDesc = Lang.Get("trader_recruitdesc_no_plr", false);
			}
			else
			{
				component.tipDesc = Lang.Get("trader_recruitdesc_no_rep", false);
			}
		}
		if ((Object)(object)RECRUITButton != (Object)null)
		{
			RECRUITButton.SetActive(KrokoshaScavMultiplayer.rules.CanReviveFromTrader());
		}
		return true;
	}
}
