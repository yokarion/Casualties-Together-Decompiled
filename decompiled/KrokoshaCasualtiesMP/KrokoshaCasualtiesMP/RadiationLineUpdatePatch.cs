using HarmonyLib;
using KrokoshaCasualtiesUtils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(RadiationLine), "Update")]
public static class RadiationLineUpdatePatch
{
	public static float timeGone;

	public static bool someone_is_getting_fried;

	public static float timeGone_limit => WorldGeneration.world.height - 8;

	public static void Postfix(RadiationLine __instance)
	{
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		if (KrokoshaScavMultiplayer.network_system_is_running)
		{
			if (!__instance.active)
			{
				return;
			}
			RadiationLine line = RadiationLine.line;
			if (line.timeGone == 0f)
			{
				timeGone = 0f;
			}
			else
			{
				float num = 1.5f;
				if (ServerMain.CheckIfShouldStartRadlineForStragglers(2) && !someone_is_getting_fried)
				{
					num *= 1.7f;
				}
				if (KrokoshaScavMultiplayer.rules.DisableSleep)
				{
					timeGone += Time.deltaTime * 1f * num;
				}
				else
				{
					timeGone += Time.deltaTime * (ServerMain.CheckIfEveryoneAliveIsActuallyConscious() ? 1f : 0.2f) * num;
				}
				if (timeGone > timeGone_limit)
				{
					timeGone = timeGone_limit;
				}
				line.timeGone = timeGone;
				foreach (NetBody all_instance in NetBody.all_instances)
				{
					Body body = all_instance.body;
					if (body.alive && ((Component)body).transform.position.y > ((Component)line).transform.position.y)
					{
						flag = true;
						float num2 = ((Component)body).transform.position.y - ((Component)line).transform.position.y;
						if (num2 > 40f || (!body.conscious && !body.sleeping))
						{
							body.brainHealth -= Time.deltaTime * Mathf.Clamp((100f - body.brainHealth) * 0.3f, 1f, 30f);
							Limb head = body.GetHead();
							head.pain += Time.deltaTime;
						}
						if (num2 > 20f)
						{
							Limb head2 = body.GetHead();
							head2.pain += Time.deltaTime;
						}
						PlayerCamera_SetIrradiateIntensity_MultiplayerPatch.RealSetIrradiateIntensity(all_instance, num2 * 0.05f);
						num2 = Mathf.Min(num2, 10f);
						body.radiationSickness += num2 * Time.deltaTime * 0.03f;
						body.eyeScareTime = 1f;
					}
				}
			}
		}
		someone_is_getting_fried = flag;
	}
}
