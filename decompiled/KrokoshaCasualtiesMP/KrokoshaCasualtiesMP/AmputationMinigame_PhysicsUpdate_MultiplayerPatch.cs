using HarmonyLib;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(AmputationMinigame), "PhysicsUpdate")]
public static class AmputationMinigame_PhysicsUpdate_MultiplayerPatch
{
	public static Item cutter_item;

	public static float last_cutProgress;

	public static void Prefix(AmputationMinigame __instance)
	{
		if (KrokoshaScavMultiplayer.network_system_is_running)
		{
			float num = Mathf.Clamp01(__instance.limb.muscleHealth / 100f);
			__instance.cutProgress = Mathf.Max(__instance.cutProgress, 1f - (0.5f + num * 0.5f));
		}
	}

	public static void Postfix(AmputationMinigame __instance)
	{
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		if (!KrokoshaScavMultiplayer.network_system_is_running || !(__instance.cutProgress > last_cutProgress + 0.08f))
		{
			return;
		}
		last_cutProgress += 0.08f;
		if (NetObjectRegistry.TryGetSyncInfo((Component)(object)cutter_item, out var si))
		{
			__instance.limb.body.adrenaline = 100f;
			if (KrokoshaScavMultiplayer.is_client)
			{
				NetDataWriter writer = Net.CreateWriter(10090);
				writer.Put((ushort)si.syncId);
				writer.Put(new LimbNetId(__instance.limb));
				Net.Client_Send((DeliveryMethod)4, in writer);
			}
		}
	}

	[ServerReceiver(10090)]
	private static void Server_AmputationMinigame_cutting(knetid clientId, ref NetDataReader reader)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out knetid result);
		reader.Get(out LimbNetId result2);
		if (MedicalSync.Server_CheckCanHealerUseItemForLimb(clientId, result2.bodyNetId, result2.limbId, result, out var healersci, out var target_nb, out var item_si))
		{
			Limb limb = result2.GetLimb();
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(Vector2.op_Implicit(((Component)limb).transform.position), $"S: Ampu_cut {healersci} -> {target_nb} item:{item_si} musclehealth:{limb.muscleHealth}");
			}
			if (limb.CanAmputate())
			{
				float num = 0.09f * 100f;
				limb.body.adrenaline = Mathf.Max(limb.body.adrenaline, 60f);
				limb.pain += num * 0.5f;
				limb.skinHealth -= num;
				limb.muscleHealth -= num;
				limb.bleedAmount += num * 0.5f;
				return;
			}
		}
		if ((Object)(object)healersci != (Object)null && (Object)(object)target_nb != (Object)null)
		{
			healersci.Server_DoAlertSingle("Denied amputation cut (try again)", reliable: false);
			log.warn($"Denied amputation cutting for {healersci} -> {target_nb}");
			MinigameMPManager.Server_ForceEndMinigameForPlayer(healersci);
		}
	}
}
