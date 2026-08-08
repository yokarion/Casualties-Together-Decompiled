using HarmonyLib;
using KrokoshaCasualtiesUtils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(CustomItemBehaviour), "Update")]
public static class CustomItemBehaviour_Update_MultiplayerPatch
{
	private static float last_zoomtime;

	public static bool Prefix(CustomItemBehaviour __instance)
	{
		if (KrokoshaScavMultiplayer.IsNetworkActiveAndIsWorldGenerated())
		{
			Item component = ((Component)__instance).GetComponent<Item>();
			last_zoomtime = PlayerCamera.main.zoomTime;
			if (component.id == "jetpack")
			{
				NetBody componentInParent = ((Component)component).GetComponentInParent<NetBody>();
				if (Object.op_Implicit((Object)(object)componentInParent))
				{
					if (componentInParent.is_local)
					{
						return true;
					}
					Limb val = default(Limb);
					if (component.condition > 0f && !component.isWet && Object.op_Implicit((Object)(object)((Component)component).transform.parent) && ((Component)((Component)component).transform.parent).TryGetComponent<Limb>(ref val) && val.body.conscious && !Object.op_Implicit((Object)(object)val.body.currentClimbable) && val.body.moveDir.y > 0.1f)
					{
						component.condition -= Time.deltaTime * 0.0135f;
					}
				}
				return false;
			}
		}
		return true;
	}

	public static void Postfix(CustomItemBehaviour __instance)
	{
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		if (!KrokoshaScavMultiplayer.IsNetworkActiveAndIsWorldGenerated())
		{
			return;
		}
		Item component = ((Component)__instance).GetComponent<Item>();
		Vector3 val2;
		switch (component.id)
		{
		case "autozoomgoggles":
		{
			Limb val3 = default(Limb);
			if (Object.op_Implicit((Object)(object)((Component)component).transform.parent) && ((Component)((Component)component).transform.parent).TryGetComponent<Limb>(ref val3) && component.battery.hasCharge)
			{
				if (Util.IsBodyLocal(val3.body))
				{
					PlayerCamera.main.zoomTime = 0.5f;
				}
				else
				{
					PlayerCamera.main.zoomTime = last_zoomtime;
				}
			}
			break;
		}
		case "rangefinder":
		{
			InventorySlot val4 = default(InventorySlot);
			if (Object.op_Implicit((Object)(object)((Component)component).transform.parent) && ((Component)((Component)component).transform.parent).TryGetComponent<InventorySlot>(ref val4) && ((Object)(object)val4 == (Object)(object)val4.body.slots[0] || (Object)(object)val4 == (Object)(object)val4.body.slots[1]))
			{
				Vector2 right2 = Vector2.right;
				val2 = val4.body.targetLookPos - ((Component)__instance).transform.position;
				float num2 = Vector2.SignedAngle(right2, Vector2.op_Implicit(((Vector3)(ref val2)).normalized));
				((Component)component).transform.eulerAngles = new Vector3(0f, 0f, num2);
			}
			break;
		}
		case "emergencylight":
		case "flashlight":
		{
			InventorySlot val = default(InventorySlot);
			if (Object.op_Implicit((Object)(object)((Component)component).transform.parent) && ((Component)((Component)component).transform.parent).TryGetComponent<InventorySlot>(ref val) && ((Object)(object)val == (Object)(object)val.body.slots[0] || (Object)(object)val == (Object)(object)val.body.slots[1]))
			{
				Vector2 right = Vector2.right;
				val2 = val.body.targetLookPos - ((Component)component).transform.position;
				float num = Vector2.SignedAngle(right, Vector2.op_Implicit(((Vector3)(ref val2)).normalized));
				((Component)component).transform.eulerAngles = new Vector3(0f, 0f, num - 90f);
			}
			break;
		}
		}
	}
}
