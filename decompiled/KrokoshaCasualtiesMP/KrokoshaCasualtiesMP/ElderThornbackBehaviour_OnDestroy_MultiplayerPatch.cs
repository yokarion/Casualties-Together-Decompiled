using HarmonyLib;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(ElderThornbackBehaviour), "OnDestroy")]
internal static class ElderThornbackBehaviour_OnDestroy_MultiplayerPatch
{
	private static bool Prefix(ElderThornbackBehaviour __instance)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)__instance.build == (Object)null)
		{
			return false;
		}
		if (!Net.running)
		{
			return true;
		}
		if (__instance.build.health <= 0f)
		{
			foreach (NetBody item in NetBody.GetBodiesInRadius(Vector2.op_Implicit(((Component)__instance).transform.position), ElderThornbackBehaviour.maxDistance))
			{
				if (!item.IsBodyLocal())
				{
					Body body = item.body;
					body.horrifiedLevel = 0f;
					body.happiness += 40f;
					body.caffeinated += 600f;
				}
			}
		}
		return true;
	}
}
