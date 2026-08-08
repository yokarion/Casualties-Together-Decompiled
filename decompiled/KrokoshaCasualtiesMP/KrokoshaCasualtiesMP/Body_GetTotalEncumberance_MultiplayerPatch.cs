using HarmonyLib;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(Body), "GetTotalEncumberance")]
internal static class Body_GetTotalEncumberance_MultiplayerPatch
{
	public static void Postfix(Body __instance, ref float __result)
	{
		NetBody netBody = default(NetBody);
		if (KrokoshaScavMultiplayer.network_system_is_running && ((Component)__instance).TryGetComponent<NetBody>(ref netBody) && (Object)(object)netBody.carrying_person != (Object)null)
		{
			float realFullEncumberanceOfBody = netBody.carrying_person.GetRealFullEncumberanceOfBody();
			__result += realFullEncumberanceOfBody * KrokoshaScavMultiplayer.rules.PiggybackWeightMultiplier;
		}
	}
}
