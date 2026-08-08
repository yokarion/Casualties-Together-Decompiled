using HarmonyLib;
using KrokoshaCasualtiesUtils;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(Body), "SwitchHands")]
internal static class BodySwitchHandsNetworkPatch
{
	private static void Postfix(Body __instance)
	{
		if (__instance.IsBodyLocal())
		{
			Item item = __instance.GetItem(0);
			Item item2 = __instance.GetItem(1);
			ItemSync.Client_SendDoubleItemPickup(item, item2);
			ItemSync.RecordAndApplyInvState(Util.GetLocalBody());
		}
	}
}
