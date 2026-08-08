using System.Collections.Generic;
using HarmonyLib;
using KrokoshaCasualtiesUtils;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(TutorialHandler), "Update")]
public static class TutorialHandler_Update_MarkiplierPatch
{
	internal static bool did_multiplayer_warning;

	internal static double everyone_finish_time;

	internal static float server_sync_timer;

	public static void PreventFromDying_Stolen(Body body)
	{
		if (body.thirst < 11f)
		{
			body.thirst = 11f;
		}
		if (body.hunger < 11f)
		{
			body.hunger = 11f;
		}
		if (body.bloodPressure < 80f)
		{
			body.bloodPressure = 80f;
		}
		if (body.fibrillationProgress > 95f)
		{
			body.fibrillationProgress = 95f;
		}
		if (body.brainHealth < 96f)
		{
			body.brainHealth = 96f;
		}
		if (body.happiness < -30f)
		{
			body.happiness = -30f;
		}
	}

	private static void Postfix(TutorialHandler __instance)
	{
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		if (!KrokoshaScavMultiplayer.network_system_is_running)
		{
			return;
		}
		foreach (Body key in NetPlayer.BodyToPlayerDict.Keys)
		{
			PreventFromDying_Stolen(key);
		}
		if (KrokoshaScavMultiplayer.is_server)
		{
			server_sync_timer += Time.unscaledDeltaTime;
			if (server_sync_timer > 0.05f)
			{
				NetDataWriter writer = Net.CreateWriter(10027);
				writer.Put(__instance.handPos);
				if ((Object)(object)__instance.grabInfo.Item1 != (Object)null)
				{
					writer.Put(AnyObjectNetId.GetNetIdFromGameObject(((Component)__instance.grabInfo.Item1).gameObject));
				}
				else
				{
					writer.Put(new AnyObjectNetId());
				}
				Net.Server_SendToClients((DeliveryMethod)4, in writer, (IEnumerable<knetid>)ServerMain.AllClientIdsExceptHost);
			}
		}
		NetBody netBody = default(NetBody);
		if ((Object)(object)__instance.grabInfo.Item1 != (Object)null && ((Component)__instance.grabInfo.Item1).TryGetComponent<NetBody>(ref netBody))
		{
			netBody.StopPiggyback();
		}
		if (!WorldGeneration.world.generatingWorld && Util.IsTutorialWorld() && Object.op_Implicit((Object)(object)PlayerCamera.main.body))
		{
			if (!did_multiplayer_warning)
			{
				PlayerCamera.main.DoAlertDelayed(Lang.Get("tutorial_mp_alert", false), true, 4f);
				did_multiplayer_warning = true;
			}
			if (did_multiplayer_warning)
			{
				PlayerCamera.main.body.forceWalk = false;
			}
			PlayerCamera.main.body.consciousness = Mathf.Max(PlayerCamera.main.body.consciousness, 10f + (Body.consciousnessRiseRate + 10f) * Time.deltaTime);
			if (everyone_finish_time != 0.0 && Time.realtimeSinceStartupAsDouble - everyone_finish_time > 2.0 && KrokoshaScavMultiplayer.is_server)
			{
				log.l("TutorialHandler_Update_MarkiplierPatch: running ToMainMenu.");
				PlayerCamera.main.ToMainMenu();
			}
		}
	}

	[ClientReceiver(10027, true)]
	private static void ClientReceiver__Tutorial_GlobalState(knetid _, ref NetDataReader reader)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out Vector2 result);
		reader.Get(out AnyObjectNetId result2);
		if (!((Object)(object)TutorialHandler.main != (Object)null))
		{
			return;
		}
		TutorialHandler.main.handPos = result;
		if (result2.TryGetGameObject(out var go))
		{
			if ((Object)(object)TutorialHandler.main.grabInfo.Item1 != (Object)(object)go.transform)
			{
				TutorialHandler.main.Grab(go.transform);
			}
		}
		else if ((Object)(object)TutorialHandler.main.grabInfo.Item1 != (Object)null)
		{
			TutorialHandler.main.Ungrab();
		}
	}
}
