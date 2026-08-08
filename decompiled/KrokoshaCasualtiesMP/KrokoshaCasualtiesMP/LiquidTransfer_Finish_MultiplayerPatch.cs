using HarmonyLib;
using KrokoshaCasualtiesUtils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(LiquidTransfer), "Finish")]
public static class LiquidTransfer_Finish_MultiplayerPatch
{
	public static void Postfix(LiquidTransfer __instance)
	{
		if (KrokoshaScavMultiplayer.IsNetworkActiveAndIsWorldGenerated() && Util.IsBodyLocal(PlayerCamera.main.body))
		{
			Body_CombineItems_MultiplayerPatch.DoTheCombineAnnouncement(isfluid: true, __instance.ml, PlayerCamera.main.body, ((Component)__instance.transferTo).GetComponent<Item>(), ((Component)__instance.transferFrom).GetComponent<Item>());
		}
	}
}
