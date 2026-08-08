using HarmonyLib;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(ElderThornbackBehaviour), "Update")]
internal static class ElderThornbackBehaviour_Update_MultiplayerPatch
{
	private static bool Prefix(ElderThornbackBehaviour __instance)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		if (!Net.running)
		{
			return true;
		}
		foreach (NetBody item in NetBody.GetBodiesInRadius(Vector2.op_Implicit(((Component)__instance).transform.position), ElderThornbackBehaviour.maxDistance * 2.25f))
		{
			if (!item.IsBodyLocal())
			{
				Body body = item.body;
				body.adrenaline = 100f;
				if (body.horrifiedLevel < 50f)
				{
					body.horrifiedLevel = 50f;
				}
				if (body.sleeping)
				{
					body.energy = Mathf.Max(body.energy, 10f);
				}
			}
		}
		foreach (NetBody item2 in NetBody.GetBodiesInRadius(Vector2.op_Implicit(((Component)__instance).transform.position), ElderThornbackBehaviour.maxDistance))
		{
			if (!item2.IsBodyLocal())
			{
				Body body2 = item2.body;
				if (__instance.stage != 0)
				{
					body2.focusedLevel = 100f;
				}
				body2.stamina += 1f;
				body2.energy = Mathf.Max(body2.energy, 15f);
				if (body2.sleeping)
				{
					body2.sleeping = false;
				}
				body2.adrenaline = 50f;
				body2.horrifiedLevel = 100f;
			}
		}
		return true;
	}
}
