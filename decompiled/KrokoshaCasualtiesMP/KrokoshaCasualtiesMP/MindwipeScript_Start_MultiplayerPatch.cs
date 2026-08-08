using HarmonyLib;
using KrokoshaCasualtiesUtils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(MindwipeScript), "Start")]
public static class MindwipeScript_Start_MultiplayerPatch
{
	private static bool Prefix(MindwipeScript __instance)
	{
		if (!KrokoshaScavMultiplayer.network_system_is_running)
		{
			return true;
		}
		if (((Component)__instance).GetComponent<Body>().IsBodyLocal())
		{
			if ((Object)(object)NetPlayer.LOCAL_PLAYER != (Object)null && NetPlayer.LOCAL_PLAYER.TryGetNetBody(out var pb))
			{
				pb.SetNetHealthSyncIgnoreTime(1f);
			}
			return true;
		}
		if (__instance.active)
		{
			return false;
		}
		((MonoBehaviour)__instance).StartCoroutine("WipeRoutine");
		return false;
	}
}
