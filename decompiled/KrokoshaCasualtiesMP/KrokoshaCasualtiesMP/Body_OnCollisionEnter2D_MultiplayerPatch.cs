using HarmonyLib;
using KrokoshaCasualtiesUtils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(Body), "OnCollisionEnter2D")]
public static class Body_OnCollisionEnter2D_MultiplayerPatch
{
	public static bool Prefix(Body __instance, Collision2D collision)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		if (!Util.IsBodyLocal(__instance))
		{
			__instance.standLerpTime = 0.58f;
			__instance.crouchAmount -= __instance.lastTimeStepVelocity.y * 0.03f;
			if (__instance.lastTimeStepVelocity.y < 0f - __instance.jumpSpeed - 1f)
			{
				__instance.visualBodyOffset += new Vector2(0f, __instance.lastTimeStepVelocity.y * 0.035f);
				if (__instance.lastTimeStepVelocity.y < 0f - __instance.jumpSpeed - 5f)
				{
					__instance.skills.AddExp(1, 0.5f);
				}
			}
			return false;
		}
		return true;
	}
}
