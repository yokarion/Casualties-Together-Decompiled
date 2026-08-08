using HarmonyLib;
using KrokoshaCasualtiesUtils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(PlayerCamera), "HandlePlayerHover")]
public static class PlayerCamera_HandlePlayerHover_MultiplayerPatch
{
	public static bool is_hovering_over_another_player;

	public static void Postfix(PlayerCamera __instance)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		if (!KrokoshaScavMultiplayer.network_system_is_running)
		{
			return;
		}
		_ = __instance.body;
		is_hovering_over_another_player = false;
		Collider2D obj = Physics2D.OverlapPoint(Vector2.op_Implicit(Camera.main.ScreenToWorldPoint(Input.mousePosition)), LayerMask.GetMask(new string[1] { "Limb" }));
		Component val = (Component)(object)((obj == null) ? null : ((Component)obj).GetComponent<Limb>()?.body);
		if (!Object.op_Implicit((Object)(object)val))
		{
			val = (Component)(object)Physics2D.OverlapPoint(Vector2.op_Implicit(Camera.main.ScreenToWorldPoint(Input.mousePosition)), LayerMask.GetMask(new string[1] { "Body" }));
		}
		if (!Object.op_Implicit((Object)(object)val))
		{
			return;
		}
		NetBody component = val.GetComponent<NetBody>();
		if ((Object)(object)component != (Object)null)
		{
			_ = component.is_player;
			if (!Util.IsBodyLocal(component.body))
			{
				string tooltipName_text = "";
				string tooltipDescription_text = "";
				is_hovering_over_another_player = true;
				component.DoMouseHoverTooltip(ref tooltipName_text, ref tooltipDescription_text);
				GlobalDark.main.SetTooltip((tooltipName_text, tooltipDescription_text));
			}
		}
	}
}
