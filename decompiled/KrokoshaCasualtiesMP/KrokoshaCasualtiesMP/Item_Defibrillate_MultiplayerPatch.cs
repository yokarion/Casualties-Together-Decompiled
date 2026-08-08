using HarmonyLib;
using KrokoshaCasualtiesUtils;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(Item), "Defibrillate")]
public static class Item_Defibrillate_MultiplayerPatch
{
	public static bool force;

	private static bool Prefix(Item __instance, DefibInfo info)
	{
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		if (!Net.running)
		{
			return true;
		}
		if (force)
		{
			force = false;
			return true;
		}
		if ((Object)(object)MinigameBase.main?.currentItem == (Object)(object)__instance && MinigameBase.main.currentMinigame != null)
		{
			Minigame currentMinigame = MinigameBase.main.currentMinigame;
			ManualDefibMinigame val = (ManualDefibMinigame)(object)((currentMinigame is ManualDefibMinigame) ? currentMinigame : null);
			if (val != null)
			{
				if (NetObjectRegistry.TryGetSyncInfo((Component)(object)MinigameBase.main.currentItem, out var si))
				{
					if (__instance.IsManualDefibrillator())
					{
						NetDataWriter writer = Net.CreateWriter(10052);
						writer.Put((ushort)si.syncId);
						writer.Put(new LimbNetId(info.limb));
						writer.Put(info.chance);
						writer.Put(val.currentCharge);
						Net.Client_Send((DeliveryMethod)0, in writer);
					}
					if (KrokoshaScavMultiplayer.is_client)
					{
						if ((Object)(object)MinigameBase.main.currentItem != (Object)null)
						{
							Limb limb = info.limb;
							limb.skinHealth -= 5f;
							Limb limb2 = info.limb;
							limb2.pain += 20f;
							info.limb.body.Ragdoll();
							if ((Object)(object)info.limb != (Object)(object)info.limb.body.limbs[1] || !info.limb.body.alive)
							{
								return false;
							}
							info.limb.body.defibShockedFrames = 20;
							info.limb.body.heartProg = -1f;
							info.limb.body.bloodPressure = 40f;
						}
						else
						{
							NetObjectRegistry.AlertObjectNotRegistered(popup: true);
						}
						return false;
					}
				}
				else if (KrokoshaScavMultiplayer.is_client)
				{
					NetObjectRegistry.AlertObjectNotRegistered(popup: true);
					return false;
				}
			}
		}
		return true;
	}

	[ServerReceiver(10052)]
	private static void ServerReceiver_ManualDefibrillator_Defibrillate(knetid clientId, ref NetDataReader reader)
	{
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Expected O, but got Unknown
		reader.Get(out knetid result);
		reader.Get(out LimbNetId result2);
		float num = default(float);
		reader.Get(ref num);
		float num2 = default(float);
		reader.Get(ref num2);
		if (!NetPlayer.TryGetNetPlayerAndNetBodyFromClientId(clientId, out var plr, out var pb))
		{
			return;
		}
		if (!num2.IsFinite() || num2 < 10f || num2 > 200f)
		{
			log.sus($"{plr} tried to defibrillate with some weirdass currentCharge: {num2}");
		}
		else
		{
			if (!result2.TryGetNetBodyAndLimbSafe(out var _, out var limb) || !NetObjectRegistry.TryGetSyncInfo(result, out var si))
			{
				return;
			}
			if (si.IsManualDefibrillator() && (Object)(object)si.item == (Object)(object)plr.minigame_currentItem && ItemSync.CheckIfBodyReachThisItem(si, pb.body))
			{
				if (!si.item.battery.hasCharge)
				{
					log.serverdeny($"MANUALDEFIB CHARGE for {plr} -> {si}: ran out of battery");
				}
				else
				{
					if (!plr.server_plrstate.Cooldown("MANUALDEFIB", 0.2f))
					{
						return;
					}
					ServerMain.Server_AnnounceSound(limb.GetPosition(), "manualdefib", ServerMain.GetListOfClientIdsExceptThis(plr.clientId));
					if (!plr.is_local)
					{
						if (log.verbose)
						{
							log.l($"{plr} fired a MANUALDEFIB {si}");
						}
						float chance = 1f - Mathf.Abs(limb.body.fibrillationProgress - num2 * 0.5f) / 40f;
						force = true;
						si.item.Defibrillate(new DefibInfo
						{
							chance = chance,
							limb = limb
						});
						si.item.battery.DrainCharge(num2 / 4000f);
					}
				}
			}
			else
			{
				log.serverdeny($"MANUALDEFIB CHARGE for {plr} cuz its not manualdefib or he doesnt even have a it ");
				plr.Server_DoAlertSingle("MANUALDEFIB Denied, idk why");
				MinigameMPManager.Server_ForceEndMinigameForPlayer(plr);
			}
		}
	}
}
