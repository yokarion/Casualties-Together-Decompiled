using HarmonyLib;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(Limb), "OnCollisionEnter2D")]
public static class Limb_OnCollisionEnter2D_MultiplayerPatch
{
	public static bool Prefix(Limb __instance, Collision2D col)
	{
		if (!KrokoshaScavMultiplayer.network_system_is_running)
		{
			return true;
		}
		SpiderHandler val = default(SpiderHandler);
		if ((Object)(object)col.rigidbody != (Object)null && ((Component)col.rigidbody).TryGetComponent<SpiderHandler>(ref val))
		{
			return false;
		}
		return true;
	}
}
