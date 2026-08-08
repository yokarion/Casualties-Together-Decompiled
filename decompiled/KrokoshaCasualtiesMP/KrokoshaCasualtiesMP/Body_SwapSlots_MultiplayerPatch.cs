using HarmonyLib;
using KrokoshaCasualtiesUtils;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(Body), "SwapSlots")]
internal static class Body_SwapSlots_MultiplayerPatch
{
	private static void Postfix(Body __instance, int slot1, int slot2)
	{
		if (__instance.IsBodyLocal())
		{
			Item item = __instance.GetItem(slot1);
			Item item2 = __instance.GetItem(slot2);
			ItemSync.Client_SendDoubleItemPickup(item, item2);
			ItemSync.RecordAndApplyInvState(Util.GetLocalBody());
		}
	}
}
