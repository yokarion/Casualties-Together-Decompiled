using System;
using System.Collections.Generic;
using HarmonyLib;
using KrokoshaCasualtiesUtils;
using LiteNetLib;
using LiteNetLib.Utils;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(AEDMinigame), "Update")]
public static class AEDMinigame_Update_MultiplayerPatch
{
	public static int last_state;

	public static bool did_send_charge;

	public static void Postfix(AEDMinigame __instance, ref List<RaycastResult> uiCasts)
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		if (!Net.running || __instance == null)
		{
			return;
		}
		if (__instance.state == 3 && last_state != __instance.state)
		{
			if ((Object)(object)MinigameBase.main.currentItem != (Object)null && NetObjectRegistry.TryGetSyncInfo((Component)(object)MinigameBase.main.currentItem, out var si))
			{
				NetDataWriter writer = Net.CreateWriter(10051);
				writer.Put((ushort)si.syncId);
				writer.Put(new LimbNetId(__instance.limb));
				Net.Client_Send((DeliveryMethod)2, in writer);
			}
			else
			{
				NetObjectRegistry.AlertObjectNotRegistered(popup: true);
			}
		}
		last_state = __instance.state;
		if (!__instance.limb.body.IsBodyLocal())
		{
			CPRHandler orAddComponent = ComponentHolderProtocol.GetOrAddComponent<CPRHandler>((Object)(object)MinigameBase.main);
			orAddComponent.pacient = __instance.limb.body;
			orAddComponent.MG_Update(uiCasts);
		}
	}

	[ServerReceiver(10051)]
	private static void ServerReceiver_AED_CHAAAARGEE(knetid clientId, ref NetDataReader reader)
	{
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out knetid result);
		reader.Get(out LimbNetId result2);
		if (!NetPlayer.TryGetNetPlayerAndNetBodyFromClientId(clientId, out var plr, out var pb) || !result2.TryGetNetBodyAndLimbSafe(out var _, out var target_limb) || !NetObjectRegistry.TryGetSyncInfo(result, out var aed_si))
		{
			return;
		}
		if (aed_si.IsAED() && (Object)(object)aed_si.item == (Object)(object)plr.minigame_currentItem && ItemSync.CheckIfBodyReachThisItem(aed_si, pb.body))
		{
			if (!aed_si.item.battery.hasCharge)
			{
				log.serverdeny($"AED CHARGE for {plr} -> {aed_si}: ran out of battery");
			}
			else
			{
				if (!plr.server_plrstate.Cooldown("AED", 3.5f))
				{
					return;
				}
				ServerMain.Server_AnnounceSound(target_limb.GetPosition(), "aedcharge", ServerMain.GetListOfClientIdsExceptThis(plr.clientId));
				if (plr.is_local)
				{
					return;
				}
				aed_si.item.battery.DrainCharge(0.02f);
				Util.DelayCallLambda(3.5f, (Action)delegate
				{
					//IL_0050: Unknown result type (might be due to invalid IL or missing references)
					//IL_0055: Unknown result type (might be due to invalid IL or missing references)
					//IL_0060: Unknown result type (might be due to invalid IL or missing references)
					//IL_0071: Expected O, but got Unknown
					if ((Object)(object)aed_si.item == (Object)(object)plr.minigame_currentItem)
					{
						if (log.verbose)
						{
							log.l($"{plr} fired an AED {aed_si}");
						}
						Item_Defibrillate_MultiplayerPatch.force = true;
						aed_si.item.Defibrillate(new DefibInfo
						{
							chance = 1f,
							limb = target_limb
						});
						aed_si.item.battery.DrainCharge(0.14f);
					}
					else if (log.verbose)
					{
						log.warn($"{plr} CANCELLED AED ???");
					}
				});
			}
		}
		else
		{
			log.serverdeny($"AED CHARGE for {plr} cuz its not aed or he doesnt even have an aed ");
			plr.Server_DoAlertSingle("AED Denied, idk why");
			MinigameMPManager.Server_ForceEndMinigameForPlayer(plr);
		}
	}
}
