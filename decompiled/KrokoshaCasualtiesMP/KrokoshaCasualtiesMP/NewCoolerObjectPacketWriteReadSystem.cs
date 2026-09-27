using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using KrokoshaCasualtiesUtils;
using LiteNetLib.Utils;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace KrokoshaCasualtiesMP;

internal class NewCoolerObjectPacketWriteReadSystem : CoolSyncSubSystemForObjects
{
	public static NewCoolerObjectPacketWriteReadSystem inst;

	public static short QuantizeFloatToShort(float value, float max)
	{
		return (short)Math.Round(Mathf.Clamp(value / max, -1f, 1f) * 32767f);
	}

	public static float UnquantizeFloatFromShort(short quantizedValue, float max)
	{
		return (float)quantizedValue / 32767f * max;
	}

	public static ushort QuantizeFloatToUShort(float value, float max)
	{
		return (ushort)Math.Round(Mathf.Clamp01(value / max) * 65535f);
	}

	public static float UnquantizeFloatFromUShort(ushort quantizedValue, float max)
	{
		return (float)(int)quantizedValue / 65535f * max;
	}

	public override void Server_RunFastSync()
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		foreach (Server_PerPlrState value in server_perplrstates.Values)
		{
			if (!NetPlayer.TryGetNetPlayerAndNetBodyFromClientId(value.plrId, out var plr, out var nb) || value.forcesync_queue.Count >= (plr.IsAlive() ? 100 : 20))
			{
				continue;
			}
			HashSet<GameObject> hashSet = NetObjectRegistry.GatherClosestObjectsToSync(plr.pos, plr.body, 70f);
			hashSet.RemoveWhere(delegate(GameObject x)
			{
				if ((Object)(object)x.transform.parent == (Object)null)
				{
					return false;
				}
				Item item = default(Item);
				if (!x.TryGetComponent<Item>(ref item))
				{
					if (!NetObjectRegistry.IsObjectDynamic(x))
					{
						return true;
					}
					return false;
				}
				ItemSync.ItemsContainerInfo itemsContainerInfo = ItemSync.ItemGetContainerInfo(item);
				if (itemsContainerInfo.is_in_surface_inv)
				{
					return false;
				}
				return !((Object)(object)itemsContainerInfo.bodynpc == (Object)(object)nb);
			});
			foreach (GameObject item2 in hashSet)
			{
				if (NetObjectRegistry.TryGetSyncInfoOrRegister(item2, out var si))
				{
					base.Server_Internal_QueueForceSync(value, si.syncId);
				}
			}
		}
	}

	public NewCoolerObjectPacketWriteReadSystem(byte systemid)
		: base(systemid)
	{
		SEND_FREQUENCY = 0.125f;
		base_packet = default(ItemOrBuildingCoolDeltaCompressablePacket);
		base_packet.SetDefault();
		CleanupAndUnregisterOnLevelChange = true;
		inst = this;
		im_in_the_thick_of_it_everybody_knows_they_know_me_where_it_snows_i_skied_in_and_they_froze_i_dont_know_no_nothin_bout_no_ice_im_just_cold_40somethin_milli_subs_or_so_ive_been_told__im_in_my_prime_and_this_aint_even_final_form_they_knocked_me_down_but_still_my_feet_they_find_the_floor_i_went_from_livin_rooms_straight_out_to_soldout_tours_lifes_a_fight_but_trust_im_ready_for_the_war__whoaohoh_this_is_how_the_story_goes_whoaohoh_i_guess_this_is_how_the_story_goes__im_in_the_thick_of_it_everybody_knows_they_know_me_where_it_snows_i_skied_in_and_they_froze_i_dont_know_no_nothin_bout_no_ice_im_just_cold_40somethin_milli_subs_or_so_ive_been_told__from_the_screen_to_the_ring_to_the_pen_to_the_king_wheres_my_crown_thats_my_bling_always_drama_when_i_ring_see_i_believe_that_if_i_see_it_in_my_heart_smash_through_the_ceilin_cause_im_reachin_for_the_stars__whoaohoh_this_is_how_the_story_goes_whoaohoh_i_guess_this_is_how_the_story_goes = true;
	}

	public override void Cleanup()
	{
		inst = null;
		base.Cleanup();
	}

	public override void ClearAndResetEverything()
	{
		base.ClearAndResetEverything();
		NetObjectRegistry.NetIdToSyncInfoDict.Clear();
		NetObjectRegistry.SyncRegistry.Clear();
	}

	public void UnregisterEVERYTHING_LOUDLY()
	{
		foreach (SyncInfo item in new List<SyncInfo>(NetObjectRegistry.SyncRegistry.Values))
		{
			Server_DeleteObject(item.syncId);
		}
		NetObjectRegistry.NetIdToSyncInfoDict.Clear();
		NetObjectRegistry.SyncRegistry.Clear();
	}

	public void UnregisterEVERYTHING(bool but_dont_unregister_inventory_items = false)
	{
		if (but_dont_unregister_inventory_items)
		{
			foreach (SyncInfo item in new List<SyncInfo>(NetObjectRegistry.SyncRegistry.Values))
			{
				if (!((Object)(object)item.go != (Object)null) || !item.IsItem() || !((Object)(object)item.go.transform.parent != (Object)null) || !((Object)(object)item.go.GetComponentInParent<Body>() != (Object)null))
				{
					SilentlyUnregisterObject(item.syncId);
				}
			}
			return;
		}
		ClearAndResetEverything();
		KrokoshaScavMultiGameObjectNetworkTracker[] array = Object.FindObjectsOfType<KrokoshaScavMultiGameObjectNetworkTracker>();
		foreach (KrokoshaScavMultiGameObjectNetworkTracker obj in array)
		{
			obj.syncinfo = null;
			Object.Destroy((Object)(object)obj);
		}
	}

	public virtual void SilentlyUnregisterObject(knetid obj_netId, bool delete_the_tracker = true)
	{
		Server_Internal_DeallocateObject(obj_netId);
		client_objects.Remove(obj_netId);
		SimplyUnregisterObject(obj_netId, delete_the_tracker);
	}

	public virtual void SimplyUnregisterObject(knetid obj_netId, bool delete_the_tracker = true)
	{
		NetObjectRegistry.NetIdToSyncInfoDict.Remove(obj_netId);
		if (Net.is_server)
		{
			if (!server_objects.TryGetValue(obj_netId, out var value) || value.real_obj == null)
			{
				return;
			}
			SyncInfo syncInfo = (SyncInfo)value.real_obj;
			if ((Object)(object)syncInfo.go != (Object)null)
			{
				NetObjectRegistry.SyncRegistry.Remove(syncInfo.go);
				if (delete_the_tracker && (Object)(object)syncInfo.tracker != (Object)null)
				{
					syncInfo.tracker.syncinfo = null;
					Object.Destroy((Object)(object)syncInfo.tracker);
				}
			}
			foreach (Server_PerPlrState value3 in server_perplrstates.Values)
			{
				if (!value3.forcesync_queue.Contains(obj_netId))
				{
					value3.forcesync_queue.Enqueue(obj_netId);
				}
				server_has_queued_forcesync = true;
			}
			value.real_obj = null;
		}
		else
		{
			if (!client_objects.TryGetValue(obj_netId, out var value2) || value2.real_obj == null)
			{
				return;
			}
			SyncInfo syncInfo2 = (SyncInfo)value2.real_obj;
			if ((Object)(object)syncInfo2.go != (Object)null)
			{
				NetObjectRegistry.SyncRegistry.Remove(syncInfo2.go);
				KrokoshaScavMultiGameObjectNetworkTracker krokoshaScavMultiGameObjectNetworkTracker = default(KrokoshaScavMultiGameObjectNetworkTracker);
				if (delete_the_tracker && syncInfo2.go.TryGetComponent<KrokoshaScavMultiGameObjectNetworkTracker>(ref krokoshaScavMultiGameObjectNetworkTracker))
				{
					Object.Destroy((Object)(object)krokoshaScavMultiGameObjectNetworkTracker);
				}
			}
			value2.real_obj = null;
		}
	}

	public virtual SyncInfo Server_RegisterObject(GameObject obj)
	{
		if (NetObjectRegistry.SyncRegistry.Count > 60000)
		{
			Object.Destroy((Object)(object)obj);
			if (log.verbose)
			{
				log.error("Server_RegisterObject: EXCEEDING NET OBJECT LIMIT !!!!!!");
			}
			return null;
		}
		KrokoshaScavMultiGameObjectNetworkTracker orAddComponent = ComponentHolderProtocol.GetOrAddComponent<KrokoshaScavMultiGameObjectNetworkTracker>((Object)(object)obj);
		if (NetObjectRegistry.SyncRegistry.TryGetValue(obj, out var value))
		{
			return value;
		}
		if (Net.is_server)
		{
			value = (orAddComponent.syncinfo = new SyncInfo
			{
				last_update_time = Time.realtimeSinceStartupAsDouble,
				go = obj,
				tracker = orAddComponent
			});
			inst.Server_NewObject(value);
			return value;
		}
		log.error($"THIS FUNCTION IS SUPPOSED TO BE CALLED ONYL ON SERVAA  {new StackTrace()}");
		return null;
	}

	protected override Server_Object Server_Internal_AllocateNewObject(knetid targetid, object obj)
	{
		Server_Object result = base.Server_Internal_AllocateNewObject(targetid, obj);
		SyncInfo syncInfo = (SyncInfo)obj;
		syncInfo.syncId = targetid;
		NetObjectRegistry.NetIdToSyncInfoDict[targetid] = syncInfo;
		NetObjectRegistry.SyncRegistry[syncInfo.go] = syncInfo;
		return result;
	}

	protected override void Server_Internal_QueueForceSync(Server_PerPlrState plrstate, knetid obj_netId)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (!NetObjectRegistry.NetIdToSyncInfoDict.TryGetValue(obj_netId, out var value) || !NetPlayer.TryGetPlayerFromClientId(plrstate.plrId, out var plr) || !((Object)(object)value.go != (Object)null) || KM.dist2dsqrcheck_presqr(value.position, plr.pos, 9000f))
		{
			base.Server_Internal_QueueForceSync(plrstate, obj_netId);
		}
	}

	public override bool Server_DeleteObject(knetid netId)
	{
		SimplyUnregisterObject(netId);
		return true;
	}

	public bool TryGetSyncinfo(knetid obj_netId, out SyncInfo si)
	{
		Client_Object value2;
		if (Net.is_server)
		{
			if (server_objects.TryGetValue(obj_netId, out var value) && value.real_obj != null)
			{
				si = (SyncInfo)value.real_obj;
				return true;
			}
		}
		else if (client_objects.TryGetValue(obj_netId, out value2) && value2.real_obj != null)
		{
			si = (SyncInfo)value2.real_obj;
			return true;
		}
		si = null;
		return false;
	}

	protected override void Client_DeleteObject(Client_Object obj)
	{
		NetObjectRegistry.NetIdToSyncInfoDict.Remove(obj.netId);
		if (obj.real_obj != null)
		{
			SyncInfo syncInfo = (SyncInfo)obj.real_obj;
			if ((Object)(object)syncInfo.go != (Object)null)
			{
				NetObjectRegistry.SyncRegistry.Remove(syncInfo.go);
				Client_DestroySI_GameObject(syncInfo);
				syncInfo.go = null;
			}
		}
	}

	private void Client_DestroySI_GameObject(SyncInfo si)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (Net.is_client && KM.dist2dsqrcheck(Vector2.op_Implicit(si.go.transform.position), Vector2.op_Implicit(((Component)Camera.main).transform.position), 60f))
		{
			Item val = default(Item);
			BuildingEntity val2 = default(BuildingEntity);
			if (si.go.TryGetComponent<Item>(ref val))
			{
				if (val.Stats.destroyAtZeroCondition && val.condition < 0.05f)
				{
					val.condition = 0f;
					return;
				}
			}
			else if (si.go.TryGetComponent<BuildingEntity>(ref val2) && val2.health < 16f)
			{
				val2.health = 0f;
				return;
			}
		}
		Object.Destroy((Object)(object)si.go);
	}

	protected override bool ShouldPackPacketFor(NetPlayer plr)
	{
		if (!Util.IsInWorld())
		{
			return false;
		}
		if (!plr.server_plrstate.finished_worldgen)
		{
			return false;
		}
		return true;
	}

	protected override void PackData2(NetDataWriter writer, List<bool> pack_bools, Server_PerPlrState plr, Server_Object obj)
	{
		bool flag = false;
		bool flag2 = false;
		if (obj.real_obj != null)
		{
			SyncInfo syncInfo = (SyncInfo)obj.real_obj;
			flag = !obj.players_its_been_sent_to.Contains(plr.plrId) || obj.players_requested_info.Contains(plr.plrId);
			if (flag)
			{
				writer.Put(syncInfo.GetItemID(), oneByteChars: true);
			}
			WaterContainerItem val = default(WaterContainerItem);
			flag2 = syncInfo.go.TryGetComponent<WaterContainerItem>(ref val);
			if (flag2)
			{
				writer.Put((byte)val.stack.Count);
				foreach (LiquidStack item in val.stack)
				{
					writer.Put(Item_SetupItems_Listener.LiquidIdRegistry[item.liquidId]);
					writer.Put((ushort)Mathf.Clamp(item.amount, 0f, 65535f));
				}
			}
		}
		pack_bools.Add(flag);
		pack_bools.Add(flag2);
	}

	protected override void PackPacket(Server_Object obj)
	{
		if (obj.real_obj != null)
		{
			SyncInfo syncInfo = (SyncInfo)obj.real_obj;
			if ((Object)(object)syncInfo.go != (Object)null)
			{
				ItemOrBuildingCoolDeltaCompressablePacket itemOrBuildingCoolDeltaCompressablePacket = default(ItemOrBuildingCoolDeltaCompressablePacket);
				itemOrBuildingCoolDeltaCompressablePacket.SetDefault();
				itemOrBuildingCoolDeltaCompressablePacket.WriteObjectIntoPacket(syncInfo);
				syncInfo.last_update_time = Time.realtimeSinceStartupAsDouble;
				obj.cur_packet = itemOrBuildingCoolDeltaCompressablePacket;
				NetObjectRegistry.Server_ObjectUpdateDistanceChecks(syncInfo);
			}
			else
			{
				Server_DeleteObject(obj.netId);
			}
		}
	}

	public override void Client_Receive(NetDataReader reader)
	{
		base.Client_Receive(reader);
	}

	protected override void Client_ReadData2(NetDataReader reader, ushort data2_len, Client_Object obj)
	{
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Expected O, but got Unknown
		int bitset_size = ItemOrBuildingCoolDeltaCompressablePacket.bitset_size;
		if (obj.bitset[bitset_size++])
		{
			reader.Get(out var result, oneByteChars: true);
			if (obj.real_obj == null)
			{
				SyncInfo syncInfo = (SyncInfo)(obj.real_obj = new SyncInfo());
				syncInfo.syncId = obj.netId;
				syncInfo.itemid = result;
				NetObjectRegistry.NetIdToSyncInfoDict[obj.netId] = syncInfo;
			}
			else
			{
				SyncInfo syncInfo2 = (SyncInfo)obj.real_obj;
				if (syncInfo2.itemid != result)
				{
					Plugin.log.LogWarning((object)("TEMP DEV: !!!!!!!!!! si.itemid != itemid !!!!!!! " + syncInfo2.itemid + " != " + result + " !!!!!!"));
				}
			}
		}
		if (LoadResourceAndCheckIgnore(obj))
		{
			SyncInfo syncInfo3 = (SyncInfo)obj.real_obj;
			if (obj.bitset[bitset_size++])
			{
				byte b = default(byte);
				reader.Get(ref b);
				WaterContainerItem val = default(WaterContainerItem);
				if (syncInfo3.go.TryGetComponent<WaterContainerItem>(ref val))
				{
					val.stack.Clear();
					byte b2 = default(byte);
					ushort num = default(ushort);
					for (int i = 0; i < b; i++)
					{
						reader.Get(ref b2);
						reader.Get(ref num);
						string text = Item_SetupItems_Listener.LiquidNetIdToId[b2];
						val.stack.Add(new LiquidStack(text, (float)(int)num));
					}
				}
				else
				{
					reader.SkipBytes(b * 3);
				}
			}
			((ItemOrBuildingCoolDeltaCompressablePacket)(object)obj.cur_packet).ReadPacketIntoObject(syncInfo3);
		}
		else
		{
			ack = false;
		}
	}

	private bool LoadResourceAndCheckIgnore(Client_Object obj)
	{
		if (obj.real_obj == null)
		{
			Client_RequestObjectInfo(obj.netId);
			return false;
		}
		SyncInfo syncInfo = (SyncInfo)obj.real_obj;
		if ((Object)(object)syncInfo.go == (Object)null)
		{
			if (string.IsNullOrEmpty(syncInfo.itemid))
			{
				Client_RequestObjectInfo(obj.netId);
				return false;
			}
			string itemid = syncInfo.itemid;
			ItemOrBuildingCoolDeltaCompressablePacket itemOrBuildingCoolDeltaCompressablePacket = (ItemOrBuildingCoolDeltaCompressablePacket)(object)obj.cur_packet;
			syncInfo.go = LoadObjectResource(itemid, in itemOrBuildingCoolDeltaCompressablePacket.pos);
			if ((Object)(object)syncInfo.go == (Object)null)
			{
				Client_RequestObjectInfo(obj.netId);
				return false;
			}
			KrokoshaScavMultiGameObjectNetworkTracker orAddComponent = ComponentHolderProtocol.GetOrAddComponent<KrokoshaScavMultiGameObjectNetworkTracker>((Object)(object)syncInfo.go);
			orAddComponent.syncinfo = syncInfo;
			syncInfo.tracker = orAddComponent;
			NetObjectRegistry.SyncRegistry[syncInfo.go] = syncInfo;
		}
		syncInfo.prev_was_ignored = syncInfo.last_was_ignored;
		syncInfo.last_was_ignored = syncInfo.IsIgnored();
		if (syncInfo.last_was_ignored)
		{
			return false;
		}
		syncInfo.last_update_time = Time.realtimeSinceStartupAsDouble;
		return true;
	}

	private GameObject LoadObjectResource(string resourceid, in Vector2 pos)
	{
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		GameObject val = Resources.Load<GameObject>(resourceid);
		if ((Object)(object)val == (Object)null)
		{
			if (!resourceid.StartsWith("KMPSR_"))
			{
				log.error("Resource does not exist: " + resourceid + " ");
				return null;
			}
			if (ScavMultiBuildingSynchronizer.known_entities_with_nonunique_id.TryGetValue(resourceid, out var value))
			{
				val = value;
				Tilemap val2 = default(Tilemap);
				if (val.TryGetComponent<Tilemap>(ref val2))
				{
					flag = true;
				}
			}
			else
			{
				val = Resources.Load<GameObject>(resourceid.Substring("KMPSR_".Count()));
				if (!Object.op_Implicit((Object)(object)val))
				{
					log.error("Special resource wasn't identified! " + resourceid);
				}
			}
		}
		if ((Object)(object)val == (Object)null)
		{
			return null;
		}
		GameObject val3 = Object.Instantiate<GameObject>(val, Vector2.op_Implicit(pos), Quaternion.identity);
		if (flag)
		{
			val3.transform.SetParent(((Component)WorldGeneration.world.worldGrid).transform);
		}
		return val3;
	}
}
