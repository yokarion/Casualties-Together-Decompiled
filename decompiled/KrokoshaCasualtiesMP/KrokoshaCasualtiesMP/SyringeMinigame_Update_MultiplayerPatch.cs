using HarmonyLib;
using KrokoshaCasualtiesUtils;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(SyringeMinigame), "Update")]
public static class SyringeMinigame_Update_MultiplayerPatch
{
	public static float last_fill_amount = 0f;

	public static bool last_was_different = false;

	public static bool is_start_or_end = false;

	public const float SYRINGE_SEND_RATE = 0.5f;

	public static float fill_milestone_timer = 0.5f;

	public static void Client_SendCurrentInjectedAmount(bool force_sound)
	{
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		Limb listen_to_limb = WaterContainerItem_Inject_MultiplayerPatch.listen_to_limb;
		if (ItemSync.TryGetSyncInfo(Minigame.game.currentItem, out var si) && WaterContainerItem_Inject_MultiplayerPatch.listen_to_limb_amount > 0f)
		{
			fill_milestone_timer = 0.5f;
			NetDataWriter writer = Net.CreateWriter(10067);
			writer.Put((ushort)si.syncId);
			writer.Put(WaterContainerItem_Inject_MultiplayerPatch.listen_to_limb_amount);
			writer.Put((ushort)NetPlayer.GetClientIdFromBody(listen_to_limb.body));
			writer.Put(Util.GetLimbIndex(listen_to_limb));
			writer.Put(is_start_or_end || force_sound);
			is_start_or_end = false;
			Net.Client_Send((DeliveryMethod)0, in writer);
			WaterContainerItem_Inject_MultiplayerPatch.listen_to_limb_amount = 0f;
		}
	}

	public static void Postfix(SyringeMinigame __instance)
	{
		if (!KrokoshaScavMultiplayer.network_system_is_running)
		{
			return;
		}
		bool flag = last_fill_amount != __instance.syringeFill.fillAmount;
		Limb listen_to_limb = WaterContainerItem_Inject_MultiplayerPatch.listen_to_limb;
		NetBody netBody = default(NetBody);
		if (flag && ((Component)listen_to_limb.body).TryGetComponent<NetBody>(ref netBody))
		{
			netBody.SetNetHealthSyncIgnoreTime(0.1f);
		}
		if ((Object)(object)Minigame.game.currentItem == (Object)null)
		{
			return;
		}
		SyncInfo si;
		bool flag2 = ItemSync.TryGetSyncInfo(Minigame.game.currentItem, out si);
		bool flag3 = !flag && last_was_different;
		if ((flag3 || fill_milestone_timer <= 0f) && flag2)
		{
			Client_SendCurrentInjectedAmount(flag3);
		}
		else if (flag)
		{
			if (flag2)
			{
				si.SetIgnoreTimeForRoundTrip(0.10000000149011612);
			}
			fill_milestone_timer -= Time.deltaTime;
		}
		else
		{
			is_start_or_end = true;
			fill_milestone_timer = 0.5f;
		}
		last_was_different = flag;
		last_fill_amount = __instance.syringeFill.fillAmount;
	}

	[ServerReceiver(10067)]
	private static void Server_SyringeMinigameInjectShit(knetid clientId, ref NetDataReader reader)
	{
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out knetid result);
		float num = default(float);
		reader.Get(ref num);
		reader.Get(out knetid result2);
		byte b = default(byte);
		reader.Get(ref b);
		bool flag = default(bool);
		reader.Get(ref flag);
		if (MedicalSync.Server_CheckCanHealerUseItemForLimb(clientId, result2, b, result, out var healersci, out var target_nb, out var item_si) && ItemSync.IsFinite(num) && item_si.IsLiquidContainer())
		{
			if (!healersci.is_local)
			{
				item_si.liquidcontainer.Inject(target_nb.body.limbs[b], num);
			}
			if (flag)
			{
				ServerMain.Server_AnnounceSound(Vector2.op_Implicit(((Component)target_nb.body).transform.position), "syringe", ServerMain.GetListOfClientIdsExceptThis(clientId));
			}
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(Vector2.op_Implicit(((Component)target_nb.body).transform.position), $"S: Syringeee {healersci} -> {target_nb} syringe:{item_si}");
			}
		}
		else if ((Object)(object)healersci != (Object)null)
		{
			healersci.Server_DoAlertSingle(Lang.MarkMsgAsLocaleKey("serverdeny_syringeinject"));
			MinigameMPManager.Server_ForceEndMinigameForPlayer(healersci);
		}
	}
}
