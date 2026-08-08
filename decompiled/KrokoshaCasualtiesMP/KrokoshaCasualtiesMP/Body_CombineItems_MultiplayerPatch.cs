using HarmonyLib;
using KrokoshaCasualtiesUtils;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(Body), "CombineItems")]
public static class Body_CombineItems_MultiplayerPatch
{
	public static bool Body_CombineItems(Body combinerguy, Item it1, Item it2)
	{
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		if (!combinerguy.CanCombine(it1, it2))
		{
			return true;
		}
		if (Object.op_Implicit((Object)(object)((Component)it1).GetComponent<GunScript>()) && Object.op_Implicit((Object)(object)((Component)it2).GetComponent<AmmoScript>()))
		{
			((Component)it1).GetComponent<GunScript>().LoadMag(((Component)it2).GetComponent<AmmoScript>());
			return true;
		}
		if (Object.op_Implicit((Object)(object)((Component)it1).GetComponent<AmmoScript>()) && Object.op_Implicit((Object)(object)((Component)it2).GetComponent<AmmoScript>()))
		{
			((Component)it1).GetComponent<AmmoScript>().LoadRound(((Component)it2).GetComponent<AmmoScript>());
			return true;
		}
		WaterContainerItem val = default(WaterContainerItem);
		WaterContainerItem val2 = default(WaterContainerItem);
		if (((Component)it1).TryGetComponent<WaterContainerItem>(ref val) && ((Component)it2).TryGetComponent<WaterContainerItem>(ref val2))
		{
			if (val.SpaceLeft > 0f && it1.id != "craftingbottle" && Util.IsBodyLocal(combinerguy))
			{
				PlayerCamera.main.StartLiquidTransfer(val, val2);
			}
			return false;
		}
		float num = 1f - it1.condition;
		it1.condition += Mathf.Min(num, it2.condition);
		it2.condition -= Mathf.Min(num, it2.condition);
		combinerguy.CreateCloudMini(Vector2.op_Implicit(((Component)it1).transform.position), (Vector2?)null);
		Util.PlayWorldSoundOnScreenIfInRange("combine", Vector2.op_Implicit(((Component)combinerguy).transform.position), 1f, 1f, 64f);
		Container val3 = default(Container);
		if (it1.TryGetParentContainer(ref val3) && (val3.AboveHoldingWeight() || it1.totalWeight > val3.maxWeightPerItem))
		{
			val3.UnloadItem(it1, (Body)null);
		}
		return true;
	}

	public static bool DoTheCombineAnnouncement(bool isfluid, float ml, Body body, Item it1, Item it2)
	{
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		if (ItemSync.ItemCanBeIgnoredForNetwork(it1) || ItemSync.ItemCanBeIgnoredForNetwork(it2))
		{
			return true;
		}
		if (ItemSync.SyncRegistry.TryGetValue(((Component)it1).gameObject, out var value) && ItemSync.SyncRegistry.TryGetValue(((Component)it2).gameObject, out var value2))
		{
			value.SetIgnoreTimeForRoundTrip();
			value2.SetIgnoreTimeForRoundTrip();
			if (isfluid || Body_CombineItems(body, it1, it2))
			{
				ushort num = (ushort)(isfluid ? 10123 : 10122);
				NetDataWriter writer = Net.CreateWriter(num);
				writer.Put((ushort)value.syncId);
				writer.Put((ushort)value2.syncId);
				if (isfluid)
				{
					writer.Put(ml);
				}
				Net.Client_Send((DeliveryMethod)0, in writer);
				if (log.verbose)
				{
					Plugin.log.LogInfo((object)$"BODY_COMBINE_OVERRIDE sending {num} {value} x {value2}  ");
				}
			}
			return false;
		}
		Plugin.log.LogWarning((object)("Attempted to combine unregistered items " + it1.id + " and " + it2.id + ", aborting."));
		NetObjectRegistry.AlertObjectNotRegistered();
		NetObjectRegistry.NewGO(((Component)it1).gameObject);
		NetObjectRegistry.NewGO(((Component)it2).gameObject);
		return false;
	}

	public static bool Prefix(Body __instance, Item it1, Item it2)
	{
		if (KrokoshaScavMultiplayer.network_system_is_running && Util.IsWorldGenerated())
		{
			if (!Util.IsBodyLocal(__instance))
			{
				Body_CombineItems(__instance, it1, it2);
				return false;
			}
			if (__instance.CanCombine(it1, it2))
			{
				return DoTheCombineAnnouncement(isfluid: false, 0f, __instance, it1, it2);
			}
			return false;
		}
		return true;
	}
}
