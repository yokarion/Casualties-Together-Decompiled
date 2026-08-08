using HarmonyLib;
using KrokoshaCasualtiesUtils;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(HandCrankMinigame), "PhysicsUpdate")]
public static class HandCrankMinigame_PhysicsUpdate_MultiplayerPatch
{
	public static float crankProgress;

	public const float CRANK_SEND_RATE = 5f;

	public const float CRANK_CHARGE_AMOUNT = 3.3E-05f;

	public const float CRANK_STAMINA_USE = 0.015f;

	private static void Prefix(HandCrankMinigame __instance)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		if (KrokoshaScavMultiplayer.network_system_is_running && __instance.held)
		{
			float z = ((Transform)__instance.crank).localEulerAngles.z;
			float num = Mathf.Abs(Mathf.DeltaAngle(HandCrankMinigame.GetAngleFromVector(((Vector2)(ref Minigame.game.handPos)).normalized), z));
			crankProgress += num;
			if (crankProgress > 5f && NetObjectRegistry.TryGetSyncInfo((Component)(object)Minigame.game.currentItem, out var si) && Net.is_client)
			{
				NetDataWriter writer = Net.CreateWriter(10180);
				writer.Put((ushort)si.syncId);
				writer.Put(crankProgress);
				crankProgress = 0f;
				Net.Client_Send((DeliveryMethod)4, in writer);
			}
		}
	}

	[ServerReceiver(10180)]
	private static void ServerReceiver_HandCrankMinigame(knetid clientId, ref NetDataReader reader)
	{
		reader.Get(out knetid result);
		float num = default(float);
		reader.Get(ref num);
		if (NetPlayer.TryGetNetPlayerAndNetBodyFromClientId(clientId, out var _, out var pb) && ItemSync.TryGetItemSyncInfo(result, out var si) && ItemSync.CheckIfBodyReachThisItem(si, pb.body) && num.IsFinite() && si.item.IsHandcrank())
		{
			si.item.battery.DrainCharge(0f - num * 3.3E-05f);
			Body body = pb.body;
			body.stamina -= num * 0.015f;
			si.item.condition = Mathf.Clamp01(si.item.condition);
		}
	}
}
