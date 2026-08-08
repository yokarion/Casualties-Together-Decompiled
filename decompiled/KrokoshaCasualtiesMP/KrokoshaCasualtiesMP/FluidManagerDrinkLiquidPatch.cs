using HarmonyLib;
using KrokoshaCasualtiesUtils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(FluidManager), "DrinkLiquid")]
public static class FluidManagerDrinkLiquidPatch
{
	public static void Postfix(FluidManager __instance, Vector2Int pos, Body body)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (KrokoshaScavMultiplayer.network_system_is_running && Util.IsBodyLocal(body) && body.alive)
		{
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(Vector2.op_Implicit(((Component)body).transform.position), "C: PlayerDrinkLiquid");
			}
			KrokoshaScavMultiplayer.Client_SendSimpleMessageToServer((ushort)10042);
		}
	}
}
