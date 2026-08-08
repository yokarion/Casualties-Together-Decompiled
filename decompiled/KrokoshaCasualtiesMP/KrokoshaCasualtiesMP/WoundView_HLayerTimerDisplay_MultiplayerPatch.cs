using HarmonyLib;
using TMPro;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(WoundView), "LayerTimerDisplay")]
internal static class WoundView_HLayerTimerDisplay_MultiplayerPatch
{
	private static void Postfix(WoundView __instance)
	{
		if (KrokoshaScavMultiplayer.network_system_is_running && (int)(Time.time * 0.5f) % 2 == 0)
		{
			((TMP_Text)__instance.timeTextBar).text = $"ALL:{NetPlayer.BodyToPlayerDict.Count:D2} L:{NetPlayer.AllLivingPlayers.Count:D2}";
			if (KrokoshaScavMultiplayer.rules.PlayerScatterDisallowed && KrokoshaScavMultiplayer.rules.ScatterMinGroupSize > 0 && (Object)(object)UIInGame.main != (Object)null)
			{
				TextMeshProUGUI timeTextBar = __instance.timeTextBar;
				((TMP_Text)timeTextBar).text = ((TMP_Text)timeTextBar).text + $" V:{UIInGame.main.antidispersion_cur_range_count:D2}";
			}
		}
	}
}
