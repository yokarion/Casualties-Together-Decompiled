using HarmonyLib;
using KrokoshaCasualtiesUtils;
using UnityEngine;
using UnityEngine.UI;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(WoundView), "UpdateView")]
public static class WoundView_UpdateView_MultiplayerPatch
{
	internal static bool[] OG_showInfection;

	public static GameObject CPRButton => PlayerCamera_ToggleWoundView_MultiplayerPatch.CPRButton;

	private static bool Prefix(WoundView __instance)
	{
		if (OG_showInfection == null)
		{
			OG_showInfection = PlayerCamera.main.showInfection;
		}
		if (!KrokoshaScavMultiplayer.network_system_is_running)
		{
			return true;
		}
		if ((Object)(object)__instance.body != (Object)null)
		{
			NetBody netBody = default(NetBody);
			if (((Component)__instance.body).TryGetComponent<NetBody>(ref netBody) && !netBody.IsBodyLocal())
			{
				PlayerCamera.main.showInfection = netBody.localoverride_showInfection;
				for (int i = 0; i < PlayerCamera.main.showInfection.Length; i++)
				{
					if (!__instance.body.limbs[i].infected)
					{
						PlayerCamera.main.showInfection[i] = false;
					}
				}
			}
			if ((Object)(object)CPRButton != (Object)null)
			{
				CheckIfCPRButtonShouldBeClickable(__instance);
			}
		}
		return true;
	}

	private static void CheckIfCPRButtonShouldBeClickable(WoundView ww)
	{
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		Image component = CPRButton.GetComponent<Image>();
		UITooltip component2 = CPRButton.GetComponent<UITooltip>();
		if (CPRHandler.CheckIfBodyIsNotOkAndNeedCPR(ww.body))
		{
			if (CPRHandler.IsCPRBeingPerformedOnThisBody_IsMeOrNobody(ww.body))
			{
				((Graphic)component).color = Color.white;
				if (ww.body.alive)
				{
					component2.tipDesc = Lang.Get("ww_cprdesc", false);
				}
				else
				{
					component2.tipDesc = Lang.Get("plr_dead", false);
				}
				return;
			}
			component2.tipDesc = Lang.Get("ww_cprdesc_occupied", false);
		}
		else
		{
			component2.tipDesc = Lang.Get("ww_cprdesc_no", false);
		}
		((Graphic)component).color = Color.gray;
	}

	private static void Postfix(WoundView __instance)
	{
		if (OG_showInfection != null)
		{
			PlayerCamera.main.showInfection = OG_showInfection;
		}
		if (!__instance.body.IsBodyLocal())
		{
			((Component)__instance.miseryText).gameObject.SetActive(false);
		}
	}
}
