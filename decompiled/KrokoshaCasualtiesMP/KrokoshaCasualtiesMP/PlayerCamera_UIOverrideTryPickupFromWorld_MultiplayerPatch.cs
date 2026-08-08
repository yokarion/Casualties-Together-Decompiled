using HarmonyLib;
using KrokoshaCasualtiesUtils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(PlayerCamera), "TryPickupFromWorld")]
internal static class PlayerCamera_UIOverrideTryPickupFromWorld_MultiplayerPatch
{
	private static void Prefix(PlayerCamera __instance, ref Item __state, UsableObject usableObject)
	{
		__state = __instance.dragItem;
	}

	private static void Postfix(PlayerCamera __instance, ref Item __state, UsableObject usableObject)
	{
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		if (!KrokoshaScavMultiplayer.network_system_is_running)
		{
			return;
		}
		Body body = __instance.body;
		if (Object.op_Implicit((Object)(object)__instance.dragItem))
		{
			if (!NetObjectRegistry.IsRegistered(((Component)__instance.dragItem).gameObject) && !NetObjectRegistry.ObjectCanBeIgnoredForNetwork(((Component)__instance.dragItem).gameObject))
			{
				NetObjectRegistry.Client_DeleteUnregisteredObject(((Component)__instance.dragItem).gameObject);
				if ((Object)(object)__state == (Object)null)
				{
					Plugin.log.LogWarning((object)$"User tried to drag a world item that is not registered yet! {((Object)__instance.dragItem).name} at {((Component)__instance.dragItem).transform.position} ");
					NetObjectRegistry.AlertObjectNotRegistered(popup: true);
				}
			}
			return;
		}
		Component val = (Component)(object)Physics2D.OverlapPoint(Vector2.op_Implicit(Camera.main.ScreenToWorldPoint(Input.mousePosition)), LayerMask.GetMask(new string[2] { "Body", "Limb" }));
		if ((Object)(object)val != (Object)null)
		{
			Limb val2 = default(Limb);
			Body body2;
			if (val.TryGetComponent<Limb>(ref val2))
			{
				body2 = val2.body;
			}
			else
			{
				Body val3 = default(Body);
				if (!val.TryGetComponent<Body>(ref val3))
				{
					return;
				}
				body2 = val3;
			}
			if (!body2.IsBodyLocal() && body2.TryGetNetBody(out var nb))
			{
				__instance.radialOpen = false;
				if ((Object)(object)UIInGame.interaction_menu_target_body != (Object)(object)nb)
				{
					UIInGame.StartPlayerInteractionMenu(nb);
				}
				else
				{
					UIInGame.StopPlayerInteractionMenu();
				}
			}
		}
		else if (!Object.op_Implicit((Object)(object)__instance.dragItem) && Object.op_Implicit((Object)(object)usableObject))
		{
			Vector2.Distance(Vector2.op_Implicit(((Component)body).transform.position), Vector2.op_Implicit(((Component)usableObject).transform.position));
			_ = 10f * usableObject.rangeMultiplier;
		}
	}
}
