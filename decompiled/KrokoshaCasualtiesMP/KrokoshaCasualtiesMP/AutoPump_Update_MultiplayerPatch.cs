using HarmonyLib;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(AutoPump), "Update")]
internal static class AutoPump_Update_MultiplayerPatch
{
	private static void AutoPump_Update_Rewritten(AutoPump ap)
	{
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		ap.coolDown -= Time.deltaTime;
		ap.wearCheckTime -= Time.deltaTime;
		if ((Object)(object)((Component)((Component)ap).transform).transform.parent == (Object)null)
		{
			ap.worn = false;
			return;
		}
		BodyGetterOverrider orAddComponent = ComponentHolderProtocol.GetOrAddComponent<BodyGetterOverrider>((Object)(object)ap);
		if (ap.wearCheckTime < 0f)
		{
			ap.wearCheckTime = 1f;
			ap.worn = orAddComponent.TryGetBodyFromParent() && orAddComponent.body.HasWearable(ap.item);
		}
		if (ap.worn && ap.item.battery.hasCharge)
		{
			ap.item.battery.DrainCharge(Time.deltaTime / 1200f);
			if (orAddComponent.body.bloodPressure < 85f)
			{
				Body body = orAddComponent.body;
				body.bloodPressure += 44f;
				ap.item.battery.DrainCharge(0.002f);
				Sound.Play("autopump", Vector2.zero, true, true, (Transform)null, 0.6f, 1f, false, false);
			}
		}
	}

	private static bool Prefix(AutoPump __instance)
	{
		if (!Net.running)
		{
			return true;
		}
		AutoPump_Update_Rewritten(__instance);
		return false;
	}
}
