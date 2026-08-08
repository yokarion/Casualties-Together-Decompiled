using HarmonyLib;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(PlayerCamera), "SetTimeScale")]
public static class PlayerCameraSetTimeScalePatch
{
	public static bool force;

	public static bool Prefix(PlayerCamera __instance, SpeedType speed)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if (KrokoshaScavMultiplayer.network_system_is_running)
		{
			if (force)
			{
				force = false;
			}
			else
			{
				if (KrokoshaScavMultiplayer.rules.DisableTimeManipulation)
				{
					Time.timeScale = 1f;
					return false;
				}
				if (speed != __instance.curTimeScale)
				{
					if (ServerMain.CheckIfAnyoneIsMoving())
					{
						Time.timeScale = 1f;
						return false;
					}
					if (KrokoshaScavMultiplayer.is_client)
					{
						KrokoshaScavMultiplayer.Client_SendSimpleMessageToServer((ushort)10041, (ushort)speed);
					}
					else
					{
						KrokoshaScavMultiplayer.Server_SendRelayMessageToClients((ushort)10022, (ushort)speed);
					}
				}
			}
		}
		return true;
	}
}
