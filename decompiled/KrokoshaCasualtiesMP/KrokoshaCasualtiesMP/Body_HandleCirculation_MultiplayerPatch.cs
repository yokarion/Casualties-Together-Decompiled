using HarmonyLib;
using KrokoshaCasualtiesUtils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(Body), "HandleCirculation")]
public static class Body_HandleCirculation_MultiplayerPatch
{
	private struct aaaaaaaaaeeeeeeeeeeuhh
	{
		public float brainHealth;
	}

	private static void Prefix(Body __instance, ref aaaaaaaaaeeeeeeeeeeuhh __state)
	{
		__state.brainHealth = __instance.brainHealth;
		if (Net.running)
		{
			NetBody netBody = default(NetBody);
			Sound_Play_MultiplayerPatch.is_the_fucking_updateheart_shit = !((Component)__instance).TryGetComponent<NetBody>(ref netBody) || (!netBody.is_local && (!Util.IsInWoundView() || !((Object)(object)WoundView.view.body == (Object)(object)__instance))) || !__instance.alive;
		}
	}

	private static void Postfix(Body __instance, ref aaaaaaaaaeeeeeeeeeeuhh __state)
	{
		Sound_Play_MultiplayerPatch.is_the_fucking_updateheart_shit = false;
		NetBody netBody = default(NetBody);
		if (!Net.running || !((Component)__instance).TryGetComponent<NetBody>(ref netBody))
		{
			return;
		}
		if (__instance.brainDying && __instance.brainHealth < __state.brainHealth)
		{
			if (!((Object)(object)NetPlayer.GetNearestConsciousPlayerToThisBody(netBody).Item1 == (Object)null))
			{
				float num = (float)(Time.unscaledTimeAsDouble - netBody._lasttime_nobraindamage_from_circulation);
				float num2 = 0f;
				num2 = ((num > 30f) ? 1f : ((!(num < 15f)) ? (num / 30f) : 0f));
				__instance.brainHealth = Mathf.Lerp(__state.brainHealth, __instance.brainHealth, num2);
			}
		}
		else
		{
			netBody._lasttime_nobraindamage_from_circulation = Time.unscaledTimeAsDouble;
		}
	}
}
