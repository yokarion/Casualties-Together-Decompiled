using HarmonyLib;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(LifepodShower), "Update")]
public static class LifepodShower_Update_MultiplayerPatch
{
	private static bool Prefix(LifepodShower __instance)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		if (KrokoshaScavMultiplayer.network_system_is_running)
		{
			if (Time.time - Traverse.Create((object)__instance).Field("timeActive").GetValue<float>() < 3f)
			{
				foreach (NetBody item in NetPlayer.GetPlayerBodiesInRadius(Vector2.op_Implicit(((Component)__instance).transform.position), 16f))
				{
					if (Mathf.Abs(((Component)__instance).transform.position.x - ((Component)item).transform.position.x) < 8f && ((Component)__instance).transform.position.y > ((Component)item).transform.position.y)
					{
						Body body = item.body;
						body.dirtyness -= Time.deltaTime * 16.7f;
						body.snowAmount += Time.deltaTime * 0.33f;
						if (body.snowAmount > 1f)
						{
							body.snowAmount = 1f;
						}
						body.eyeScareTime = 0.5f;
						Limb[] limbs = body.limbs;
						for (int i = 0; i < limbs.Length; i++)
						{
							limbs[i].SetDisinfect(120f);
						}
					}
				}
			}
			return false;
		}
		return true;
	}
}
