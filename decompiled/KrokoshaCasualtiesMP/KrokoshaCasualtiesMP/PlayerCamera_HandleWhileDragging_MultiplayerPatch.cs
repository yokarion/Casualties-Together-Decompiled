using System.Collections.Generic;
using HarmonyLib;
using KrokoshaCasualtiesUtils;
using LiteNetLib.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(PlayerCamera), "HandleWhileDragging")]
internal static class PlayerCamera_HandleWhileDragging_MultiplayerPatch
{
	private static bool draining_liquid;

	private static void Prefix(PlayerCamera __instance, ref bool __state, List<RaycastResult> uiCasts)
	{
		__state = __instance.radialOpen;
	}

	public static void Postfix(PlayerCamera __instance, ref bool __state, List<RaycastResult> uiCasts)
	{
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		if (!KrokoshaScavMultiplayer.network_system_is_running)
		{
			return;
		}
		Body body = __instance.body;
		if (__instance.radialOpen)
		{
			if ((Object)(object)InvButton_get_body_MultiplayerPatch.focused_body != (Object)null)
			{
				Vector3 val = Camera.main.WorldToScreenPoint(((Component)InvButton_get_body_MultiplayerPatch.focused_body).transform.position);
				InvButton_get_body_MultiplayerPatch.focused_body_position_smooth = Vector2.Lerp(InvButton_get_body_MultiplayerPatch.focused_body_position_smooth, Vector2.op_Implicit(val), 5f * Time.deltaTime);
				((Component)__instance.radialMenu).transform.position = Vector2.op_Implicit(InvButton_get_body_MultiplayerPatch.focused_body_position_smooth);
				NetBody netBody = default(NetBody);
				if (((Component)InvButton_get_body_MultiplayerPatch.focused_body).TryGetComponent<NetBody>(ref netBody))
				{
					((TMP_Text)__instance.radialText).text = netBody.bodyname;
				}
				((Behaviour)__instance.radialCircle).enabled = false;
				if (!KM.dist2dsqrcheck(Vector2.op_Implicit(((Component)InvButton_get_body_MultiplayerPatch.focused_body).transform.position), Vector2.op_Implicit(((Component)PlayerCamera.main.body).transform.position), SharedMain.max_player_interaction_distance * 2f))
				{
					__instance.radialOpen = false;
					InvButton_get_body_MultiplayerPatch.focused_body = null;
					((Behaviour)__instance.radialCircle).enabled = true;
				}
			}
			else
			{
				((Behaviour)__instance.radialCircle).enabled = true;
				InvButton_get_body_MultiplayerPatch.focused_body_position_smooth = Vector2.op_Implicit(((Component)__instance.radialMenu).transform.position);
			}
		}
		if (!__state && __instance.radialOpen && !Util.IsInWoundView())
		{
			Component val2 = (Component)(object)Physics2D.OverlapPoint(Vector2.op_Implicit(Camera.main.ScreenToWorldPoint(Input.mousePosition)), LayerMask.GetMask(new string[2] { "Body", "Limb" }));
			if ((Object)(object)val2 != (Object)null)
			{
				Limb val3 = default(Limb);
				Body body2;
				if (val2.TryGetComponent<Limb>(ref val3))
				{
					body2 = val3.body;
				}
				else
				{
					Body val4 = default(Body);
					if (!val2.TryGetComponent<Body>(ref val4))
					{
						return;
					}
					body2 = val4;
				}
				if (!body2.IsBodyLocal() && body2.TryGetNetBody(out var nb))
				{
					__instance.radialOpen = false;
					if ((Object)(object)UIInGame.interaction_menu_target_body != (Object)(object)nb)
					{
						UIInGame.StartPlayerInteractionMenu(nb);
					}
				}
				else if ((Object)(object)UIInGame.interaction_menu_target_body != (Object)null)
				{
					__instance.radialOpen = false;
				}
			}
		}
		else
		{
			bool flag = false;
			foreach (RaycastResult uiCast in uiCasts)
			{
				RaycastResult current = uiCast;
				if (!((Object)(object)((RaycastResult)(ref current)).gameObject == (Object)(object)__instance.liquidDrainObject))
				{
					continue;
				}
				if (Object.op_Implicit((Object)(object)__instance.dragItem) && !((Component)__instance.dragItem).GetComponent<LiquidAffect>().wasWater)
				{
					if (ItemSync.TryGetSyncInfo(__instance.dragItem, out var si))
					{
						si.SetIgnoreTimeForRoundTrip();
					}
					flag = ((Component)__instance.dragItem).GetComponent<WaterContainerItem>().CurrentTotal > 0f;
				}
				break;
			}
			if (draining_liquid && !flag && KrokoshaScavMultiplayer.is_client && Object.op_Implicit((Object)(object)__instance.dragItem) && ItemSync.TryGetSyncInfo(__instance.dragItem, out var si2) && si2.IsLiquidContainer())
			{
				KrokoshaScavMultiplayer.Client_SendSimpleMessageToServer((ushort)10028, si2.syncId, si2.liquidcontainer.CurrentTotal);
			}
			draining_liquid = flag;
		}
		if ((Object)(object)UIInGame.interaction_menu_target_body != (Object)null && (Object)(object)__instance.dragItem != (Object)null && ((Mathf.Abs(Camera.main.WorldToScreenPoint(((Component)UIInGame.interaction_menu_target_body.body).transform.position).x - Input.mousePosition.x) > 500f * PlayerCamera.uiScale && !__instance.tradeMenu.activeSelf) || body.consciousness < 20f))
		{
			UIInGame.StopPlayerInteractionMenu();
		}
	}

	[ServerReceiver(10028)]
	private static void Server_DrainWaterContainer(knetid clientId, ref NetDataReader reader)
	{
		reader.Get(out knetid result);
		float num = default(float);
		reader.Get(ref num);
		if (NetPlayer.TryGetNetPlayerAndBodyFromClientId(clientId, out var _, out var body) && ItemSync.TryGetItemSyncInfo(result, out var si) && si.IsLiquidContainer() && ItemSync.IsFinite(num) && ItemSync.CheckIfBodyReachThisItem(si, body))
		{
			WaterContainerItem liquidcontainer = si.liquidcontainer;
			float currentTotal = si.liquidcontainer.CurrentTotal;
			if (!((Component)liquidcontainer).GetComponent<LiquidAffect>().wasWater && num >= 0f && currentTotal > num)
			{
				liquidcontainer.Drain(liquidcontainer.CalculateDrain(currentTotal - num));
			}
		}
	}
}
