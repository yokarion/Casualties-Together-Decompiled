using HarmonyLib;
using KrokoshaCasualtiesUtils;
using LiteNetLib.Utils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(BandageMinigame), "DoBandageAction")]
public static class BandageMinigame_DoBandageAction_MultiplayerPatch
{
	public static int bandagethingcounter;

	public static bool Prefix(BandageMinigame __instance)
	{
		if (KrokoshaScavMultiplayer.network_system_is_running)
		{
			Limb limb = __instance.limb;
			NetBody component = ((Component)limb.body).GetComponent<NetBody>();
			if (NetObjectRegistry.TryGetSyncInfo(((Component)Minigame.game.currentItem).gameObject, out var si))
			{
				KrokoshaScavMultiplayer.Client_SendSimpleMessageToServer((ushort)10080, si.syncId, component.netId, Util.GetLimbIndex(limb));
			}
		}
		return true;
	}

	[ServerReceiver(10080)]
	private static void Server_BandageMinigame_DoBandageAction(knetid clientId, ref NetDataReader reader)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out knetid result);
		reader.Get(out knetid result2);
		byte b = default(byte);
		reader.Get(ref b);
		if (MedicalSync.Server_CheckCanHealerUseItemForLimb(clientId, result2, b, result, out var healersci, out var target_nb, out var item_si))
		{
			if (MinigameBase_StartMinigame_MultiplayerPatch.known_bandages_onuse_lambdas.TryGetValue(item_si.item.id, out var value))
			{
				if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
				{
					DebugHelp.OnNetEvent(Vector2.op_Implicit(((Component)healersci.body).transform.position), $"S: bandaging bro {healersci.playername} -> {target_nb.playername} {item_si}");
				}
				_ = target_nb.body.limbs[b];
				if (!healersci.is_local)
				{
					value.Invoke(1f / 18f);
				}
				bandagethingcounter++;
				if (bandagethingcounter > 14)
				{
					if ((Object)(object)healersci != (Object)(object)target_nb)
					{
						Body body = target_nb.body;
						body.happiness += 0.2f;
					}
					ServerMain.Server_AnnounceSound(target_nb.pos, "bandage", ServerMain.GetListOfClientIdsExceptThis(clientId));
					bandagethingcounter = 0;
				}
				return;
			}
			Plugin.log.LogError((object)$"Wtf is this bandage bro (server somehow skipped it or wat?): ID:{item_si.item.id}   SCI:{item_si} ");
			healersci.Server_DoAlertSingle(Lang.MarkMsgAsLocaleKey("serverdeny_bandage1"));
			MinigameMPManager.Server_ForceEndMinigameForPlayer(healersci);
		}
		if ((Object)(object)healersci != (Object)null)
		{
			MinigameMPManager.Server_ForceEndMinigameForPlayer(healersci);
		}
	}
}
