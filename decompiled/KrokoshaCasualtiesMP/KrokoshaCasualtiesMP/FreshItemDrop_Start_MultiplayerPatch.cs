using HarmonyLib;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(FreshItemDrop), "Start")]
internal static class FreshItemDrop_Start_MultiplayerPatch
{
	private const float max_distance_to_keep = 8f;

	private static void Postfix(FreshItemDrop __instance)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (KrokoshaScavMultiplayer.network_system_is_running)
		{
			if (NetPlayer.GetDistanceToNearestLivingPlayer(Vector2.op_Implicit(((Component)__instance).transform.position)).Item2 > 64f)
			{
				Object.Destroy((Object)(object)__instance);
			}
		}
		else if (!KM.dist2dsqrcheck(Vector2.op_Implicit(((Component)__instance).transform.position), Vector2.op_Implicit(((Component)PlayerCamera.main.body).transform.position), 8f))
		{
			Object.Destroy((Object)(object)__instance);
		}
	}
}
