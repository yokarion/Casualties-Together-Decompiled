using HarmonyLib;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(RadioactiveObject), "Update")]
public static class RadioactiveObjectUpdatePatch
{
	public static bool Prefix(RadioactiveObject __instance)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		if (KrokoshaScavMultiplayer.network_system_is_running)
		{
			foreach (NetBody all_instance in NetBody.all_instances)
			{
				Body body = all_instance.body;
				float num = Vector2.Distance(Vector2.op_Implicit(((Component)__instance).transform.position), Vector2.op_Implicit(((Component)body).transform.position)) * 0.3f;
				float num2 = __instance.radAtZero * (1f / Mathf.Max(num * num, 1f));
				if (num2 > 0.1f)
				{
					body.radiationSickness += num2 * Time.deltaTime;
					PlayerCamera_SetIrradiateIntensity_MultiplayerPatch.RealSetIrradiateIntensity(all_instance, num2 * 0.5f);
				}
			}
			return false;
		}
		return true;
	}
}
