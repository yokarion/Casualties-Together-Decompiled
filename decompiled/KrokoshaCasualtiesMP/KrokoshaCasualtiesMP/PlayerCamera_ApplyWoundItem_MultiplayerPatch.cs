using HarmonyLib;
using KrokoshaCasualtiesUtils;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(PlayerCamera), "ApplyWoundItem")]
public static class PlayerCamera_ApplyWoundItem_MultiplayerPatch
{
	public static Body last_wounditem_user;

	public static Limb applying_on_limb;

	public static Item applying_item;

	public static bool DoApplyWoundItemChecks(Body healer, Limb limb, Item item)
	{
		bool flag = Util.IsBodyLocal(healer);
		Body body = limb.body;
		if (limb.dismembered || !healer.conscious || !item.Stats.ActuallyUsableOnLimb(item))
		{
			return false;
		}
		if ((Object)(object)body != (Object)(object)healer)
		{
			PlayerCamera main = PlayerCamera.main;
			if (!body.alive)
			{
				return true;
			}
			if (healer.aboveMedicalCutoff)
			{
				if (MedicalSync.IsRefusingHelp(body) && !item.Stats.ignoreDepression)
				{
					if (flag)
					{
						main.DoAlert(Lang.Get("deny_refuse", false), false);
						main.PlayUISound((UISoundType)6, 1f);
					}
					return false;
				}
				return true;
			}
			if (flag)
			{
				main.UseFailUnhappiness();
			}
			return false;
		}
		if (!healer.aboveMedicalCutoff && !item.Stats.ignoreDepression)
		{
			if (flag)
			{
				PlayerCamera.main.DoAlert(Locale.GetOther("alerttoounhappy"), false);
			}
			return false;
		}
		return true;
	}

	public static void ForceApplyWoundItem(Body healer, Limb limb, Item item)
	{
		last_wounditem_user = healer;
		CoUtils_instance_MultiplayerPatch.cur_override_instance = limb.GetCoUtilsInstance();
		WaterContainerItem val = default(WaterContainerItem);
		if (item.Stats.usableOnLimb)
		{
			item.Stats.useLimbAction.Invoke(limb, item);
		}
		else if (((Component)item).TryGetComponent<WaterContainerItem>(ref val))
		{
			val.ApplyToLimb(limb, 100f);
		}
		CoUtils_instance_MultiplayerPatch.cur_override_instance = null;
	}

	private static bool Prefix(PlayerCamera __instance, Item item)
	{
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		applying_on_limb = __instance.selectedLimb;
		applying_item = item;
		if (KrokoshaScavMultiplayer.network_system_is_running)
		{
			if (!ItemSync.CheckIfBodyReachThisItem(item, __instance.body, 20f, check_obstruction: true))
			{
				PlayerCamera.main.DoAlert(Lang.Get("item_unreachable", false), false);
				PlayerCamera.main.PlayUISound((UISoundType)6, 1f);
				return false;
			}
			if (!DoApplyWoundItemChecks(__instance.body, __instance.selectedLimb, item))
			{
				return false;
			}
			if (ItemSync.TryGetItemSyncInfo(item, out var isi))
			{
				if (log.verbose)
				{
					log.l($"CLIENT: Sending ApplyWoundItem {((Component)__instance.selectedLimb.body).GetComponent<NetBody>()} {Util.GetLimbIndex(__instance.selectedLimb)} {isi} ");
				}
				if (KrokoshaScavMultiplayer.is_client)
				{
					isi.SetIgnoreTimeForRoundTrip();
				}
				NetDataWriter writer = Net.CreateWriter(10126);
				writer.Put(new LimbNetId(__instance.selectedLimb));
				writer.Put((ushort)isi.syncId);
				Net.Client_Send((DeliveryMethod)0, in writer);
				if (!KrokoshaScavMultiplayer.is_client)
				{
					return false;
				}
			}
		}
		last_wounditem_user = Util.GetLocalBody();
		return true;
	}

	private static void Postfix(PlayerCamera __instance)
	{
		applying_on_limb = null;
		applying_item = null;
	}
}
