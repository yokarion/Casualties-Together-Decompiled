using HarmonyLib;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(SpiderHandler), "PlayThreatMusic")]
public static class SpiderHandler_PlayThreatMusic_MultiplayerPatch
{
	public static bool Prefix(SpiderHandler __instance)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		return KM.dist2dsqrcheck(Vector2.op_Implicit(((Component)Camera.main).transform.position), Vector2.op_Implicit(((Component)__instance).transform.position), __instance.seeDistance);
	}
}
