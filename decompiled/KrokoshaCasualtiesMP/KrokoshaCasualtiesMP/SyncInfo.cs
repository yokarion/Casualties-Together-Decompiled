using System;
using KrokoshaCasualtiesUtils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public class SyncInfo
{
	public bool is_item_and_not_building;

	public string itemid;

	public bool prev_was_ignored;

	public bool last_was_ignored;

	public double last_update_time;

	public knetid syncId;

	public GameObject go;

	public KrokoshaScavMultiGameObjectNetworkTracker tracker;

	public bool relaxed_combat_sync;

	public Vector2 position => Vector2.op_Implicit(go.transform.position);

	public Item item => go.GetComponent<Item>();

	public Container container => go.GetComponent<Container>();

	public BuildingEntity building => go.GetComponent<BuildingEntity>();

	public TraderScript trader => go.GetComponent<TraderScript>();

	public SpiderHandler spider => go.GetComponent<SpiderHandler>();

	public GunScript gun => go.GetComponent<GunScript>();

	public AmmoScript ammo => go.GetComponent<AmmoScript>();

	public WaterContainerItem liquidcontainer => go.GetComponent<WaterContainerItem>();

	public string GetItemID()
	{
		if ((Object)(object)go != (Object)null)
		{
			Item val = default(Item);
			if (go.TryGetComponent<Item>(ref val))
			{
				itemid = val.id;
			}
			BuildingEntity val2 = default(BuildingEntity);
			if (go.TryGetComponent<BuildingEntity>(ref val2))
			{
				itemid = val2.id;
			}
		}
		return itemid;
	}

	public bool IsItem()
	{
		return (Object)(object)go.GetComponent<Item>() != (Object)null;
	}

	public bool IsContainer()
	{
		return (Object)(object)container != (Object)null;
	}

	public bool IsBuilding()
	{
		return (Object)(object)go.GetComponent<BuildingEntity>() != (Object)null;
	}

	public bool IsTrader()
	{
		if (Object.op_Implicit((Object)(object)go.GetComponent<BuildingEntity>()))
		{
			return (Object)(object)trader != (Object)null;
		}
		return false;
	}

	public bool IsGun()
	{
		return (Object)(object)gun != (Object)null;
	}

	public bool IsAmmo()
	{
		return (Object)(object)ammo != (Object)null;
	}

	public bool IsLiquidContainer()
	{
		return (Object)(object)liquidcontainer != (Object)null;
	}

	public bool IsSpider()
	{
		return (Object)(object)spider != (Object)null;
	}

	public bool IsAED()
	{
		if (IsItem())
		{
			return item.IsAED();
		}
		return false;
	}

	public bool IsManualDefibrillator()
	{
		if (IsItem())
		{
			return item.IsManualDefibrillator();
		}
		return false;
	}

	public void SetIgnoreTimeForRoundTrip()
	{
		SetIgnoreTimeForRoundTrip(0.3);
	}

	public void SetIgnoreTimeForRoundTrip(double additional_time_seconds)
	{
		last_update_time = Math.Max(last_update_time, Time.realtimeSinceStartupAsDouble + (double)ClientMain.RTT_IN_SECONDS_with_margin + additional_time_seconds);
	}

	public bool IsIgnored()
	{
		return last_update_time > Time.realtimeSinceStartupAsDouble;
	}

	public override string ToString()
	{
		if (this == null)
		{
			return "SyncInfo(NULL)";
		}
		if ((Object)(object)go == (Object)null)
		{
			return $"SyncInfo(NULL_OBJ, netId: {syncId}, last time: {Math.Floor(last_update_time)})";
		}
		return string.Format("SyncInfo({0}, {1}, {2}, last time: {3})", IsBuilding() ? "building" : (IsItem() ? "item" : "UNKNOWN_TYPE"), syncId, GetItemID() ?? ((object)go).ToString(), Math.Floor(last_update_time));
	}
}
