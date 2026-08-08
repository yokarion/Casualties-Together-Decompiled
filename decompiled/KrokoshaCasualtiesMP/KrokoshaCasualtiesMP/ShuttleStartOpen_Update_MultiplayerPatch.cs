using HarmonyLib;
using KrokoshaCasualtiesUtils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(ShuttleStartOpen), "Update")]
internal static class ShuttleStartOpen_Update_MultiplayerPatch
{
	private static float timer;

	private static void Postfix(ShuttleStartOpen __instance)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if (Util.IsWorldGenerated() && Util.TryGetLocalBody(out var body))
		{
			if (KM.dist2dsqrcheck(Vector2.op_Implicit(((Component)__instance).transform.position), Vector2.op_Implicit(((Component)body).transform.position), 64f))
			{
				return;
			}
			timer += Time.unscaledDeltaTime;
			if (timer > 5f)
			{
				if (log.verbose)
				{
					log.l("DEV: DELETING ShuttleStartOpen CUZ TOO FAR BRAH");
				}
				GameObject[] doors = __instance.doors;
				for (int i = 0; i < doors.Length; i++)
				{
					Object.Destroy((Object)(object)doors[i]);
				}
				Object.Destroy((Object)(object)__instance);
			}
		}
		else
		{
			timer = 0f;
		}
	}
}
