using HarmonyLib;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(StalactiteDropper), "Drop")]
public static class StalactiteDropper_Drop_MultiplayerPatch
{
	private static void Prefix(StalactiteDropper __instance)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (KrokoshaScavMultiplayer.IsNetworkActiveAndIsClient() && NetObjectRegistry.TryGetSyncInfoOrRegister((Component)(object)__instance, out var si))
		{
			si.SetIgnoreTimeForRoundTrip(1f + Vector2.Distance(Vector2.op_Implicit(((Component)__instance).transform.position), Vector2.op_Implicit(((Component)PlayerCamera.main).transform.position)) * 0.3f);
		}
	}
}
