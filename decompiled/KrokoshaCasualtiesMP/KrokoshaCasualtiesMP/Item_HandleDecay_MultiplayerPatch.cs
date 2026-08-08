using HarmonyLib;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(Item), "HandleDecay")]
internal static class Item_HandleDecay_MultiplayerPatch
{
	private static bool Prefix(Item __instance)
	{
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Invalid comparison between Unknown and I4
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Invalid comparison between Unknown and I4
		float num = __instance.decayMultiplier * (__instance.isWet ? 6f : 1f) * WorldGeneration.globalDecayRate;
		if ((__instance.Stats.decayInfo & 1) != 0 && __instance.cont.itemCount == 0)
		{
			num *= 0f;
		}
		if ((__instance.Stats.decayInfo & 2) != 0 && (!Object.op_Implicit((Object)(object)((Component)__instance).transform.parent) || Object.op_Implicit((Object)(object)__instance.ParentContainer())))
		{
			num *= 0f;
		}
		if ((__instance.Stats.decayInfo & 4) != 0)
		{
			if (Object.op_Implicit((Object)(object)((Component)__instance).transform.parent))
			{
				Vector2 velocity = ComponentHolderProtocol.GetOrAddComponent<BodyGetterOverrider>((Object)(object)__instance).GetBody().rb.velocity;
				if (!(((Vector2)(ref velocity)).magnitude < 0.5f))
				{
					goto IL_00c9;
				}
			}
			num *= 0f;
		}
		goto IL_00c9;
		IL_00c9:
		if ((__instance.Stats.decayInfo & 0x10) != 0)
		{
			float num2 = (((int)__instance.battery.preset == 2) ? 3f : (((int)__instance.battery.preset == 1) ? 1f : 0.5f));
			__instance.battery.DrainCharge(__instance.Stats.rotSpeed * 0.01f * num2 * num * Time.deltaTime);
		}
		else
		{
			__instance.condition -= __instance.Stats.rotSpeed * num * Time.deltaTime * 0.01f;
		}
		__instance.condition = Mathf.Clamp01(__instance.condition);
		return false;
	}
}
