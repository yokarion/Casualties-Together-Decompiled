using System;
using System.Collections;
using System.Collections.Generic;
using KrokoshaCasualtiesUtils;
using LiteNetLib.Utils;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public struct ItemOrBuildingCoolDeltaCompressablePacket : IDeltaPacketBase
{
	public enum CurrentBattery : byte
	{
		None,
		smallbattery,
		mediumbattery,
		largebattery
	}

	[AlwaysSync]
	public byte container_data1;

	[AlwaysSync]
	public knetid container_netId;

	public bool rb_dynamic;

	public bool itemLock;

	public Vector2 pos;

	public float rotation;

	public Vector2 scale;

	public Vector2_4byte_200 vel;

	private const float angvel_to_short = 163.83499f;

	public short angvel_quantized;

	public float condition;

	public byte compressedboughtitemtime;

	public byte compressedwetTime;

	public bool freshitemdrop;

	public byte current_battery;

	public byte specialdata1;

	public float specialdata2;

	public byte ammo;

	public Vector2_4byte_512 spider_target;

	public float spider_biteCooldown;

	public float spider_stunTime;

	public Vector2_4byte_512 trader_desiredpos;

	public bool gun_racked;

	public bool gun_safe;

	public bool gun_hasMag;

	public bool gun_roundInChamber;

	public bool gun_roundInChamber_is_casing;

	public bool building_backgroundified;

	public Vector2_4byte_512 grabberplant_tipPos;

	public float grabberplant_randOffset;

	private static float combatrelaxdist = 6f;

	public static int bitset_size = 27;

	public float angvel
	{
		get
		{
			return (float)angvel_quantized / 163.83499f;
		}
		set
		{
			angvel_quantized = (short)Mathf.Clamp(value * 163.83499f, -32768f, 32767f);
		}
	}

	public float boughtitemtime
	{
		get
		{
			return (float)(int)compressedboughtitemtime / 0.85f;
		}
		set
		{
			compressedboughtitemtime = (byte)(Mathf.Clamp(value, 0f, 300f) * 0.85f);
		}
	}

	public float wetTime
	{
		get
		{
			return (float)(int)compressedwetTime / 255f * 125f;
		}
		set
		{
			compressedwetTime = (byte)(Mathf.Clamp(value, 0f, 125f) / 125f * 255f);
		}
	}

	public void SetDefault()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		condition = 1f;
		scale = Vector2.one;
	}

	public void WriteObjectIntoPacket(SyncInfo si)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Invalid comparison between Unknown and I4
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Invalid comparison between Unknown and I4
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Invalid comparison between Unknown and I4
		GameObject go = si.go;
		Rigidbody2D val = default(Rigidbody2D);
		bool num = go.TryGetComponent<Rigidbody2D>(ref val);
		if (!num || (Object)(object)((Component)val).transform.parent == (Object)null || val.simulated)
		{
			pos = si.position;
			Quaternion val2 = go.transform.rotation;
			rotation = ((Quaternion)(ref val2)).eulerAngles.z;
			scale = Vector2.op_Implicit(go.transform.localScale);
		}
		if (num)
		{
			vel = val.velocity;
			angvel = val.angularVelocity;
			rb_dynamic = (int)val.bodyType == 0;
			ItemLock val3 = default(ItemLock);
			itemLock = ((Component)val).TryGetComponent<ItemLock>(ref val3);
		}
		Item val4 = default(Item);
		BuildingEntity val11 = default(BuildingEntity);
		if (go.TryGetComponent<Item>(ref val4))
		{
			condition = val4.condition;
			wetTime = Time.time - val4.wetTime;
			ItemSync.ItemsContainerInfo itemsContainerInfo = ItemSync.ItemGetContainerInfo(val4);
			container_netId = itemsContainerInfo.knetid;
			if (itemsContainerInfo.is_wearing)
			{
				container_data1 = 100;
			}
			else if (itemsContainerInfo.is_in_surface_inv)
			{
				container_data1 = (byte)(itemsContainerInfo.inv_slot_number + 200);
			}
			else if (itemsContainerInfo.has_container)
			{
				container_data1 = 1;
			}
			else
			{
				container_data1 = 0;
			}
			FreshItemDrop val5 = default(FreshItemDrop);
			if (((Component)val4).TryGetComponent<FreshItemDrop>(ref val5))
			{
				if (val5.timeLeft > 5f)
				{
					freshitemdrop = true;
				}
			}
			else
			{
				freshitemdrop = false;
			}
			BoughtItem val6 = default(BoughtItem);
			if (((Component)val4).TryGetComponent<BoughtItem>(ref val6))
			{
				boughtitemtime = val6.time;
			}
			else
			{
				compressedboughtitemtime = 0;
			}
			CustomItemBehaviour val7 = default(CustomItemBehaviour);
			PlushScript val8 = default(PlushScript);
			if (((Component)val4).TryGetComponent<CustomItemBehaviour>(ref val7) && ItemSync.CustomItemBehaviourStateCanBeJustByte(val4))
			{
				specialdata1 = (byte)val7.state;
				if (val7.data != null && val7.data.Length != 0 && val7.data[0] is float num2)
				{
					specialdata2 = num2;
				}
			}
			else if (((Component)val4).TryGetComponent<PlushScript>(ref val8) && val8.index >= 0)
			{
				specialdata1 = (byte)val8.index;
			}
			if ((Object)(object)val4.battery != (Object)null && val4.battery.hasBattery)
			{
				if (!Enum.TryParse<CurrentBattery>(val4.battery.batteryType, out var result))
				{
					Plugin.log.LogError((object)("Unknown battery type: " + val4.battery.batteryType + " "));
				}
				current_battery = (byte)result;
			}
			else
			{
				current_battery = 0;
			}
			GunScript val9 = default(GunScript);
			AmmoScript val10 = default(AmmoScript);
			if (((Component)val4).TryGetComponent<GunScript>(ref val9))
			{
				gun_racked = val9.racked;
				gun_safe = val9.safe;
				gun_hasMag = val9.hasMag;
				gun_roundInChamber = (int)val9.roundInChamber != 2;
				gun_roundInChamber_is_casing = (int)val9.roundInChamber == 1;
				ammo = (byte)val9.roundsInMag;
			}
			else if (((Component)val4).TryGetComponent<AmmoScript>(ref val10))
			{
				ammo = (byte)val10.rounds;
			}
		}
		else if (go.TryGetComponent<BuildingEntity>(ref val11))
		{
			condition = val11.health;
			Krokosha_BuildingEntity_TrackerComponent_for_backgroundified orAddComponent = ComponentHolderProtocol.GetOrAddComponent<Krokosha_BuildingEntity_TrackerComponent_for_backgroundified>((Object)(object)val11);
			building_backgroundified = orAddComponent.is_backgroundified;
			CorpseScript val12 = default(CorpseScript);
			SpikeStabberScript spike = default(SpikeStabberScript);
			DrillPod_Update_MultiplayerPatch.Krokosha_DrillPod_OverrideComponent krokosha_DrillPod_OverrideComponent = default(DrillPod_Update_MultiplayerPatch.Krokosha_DrillPod_OverrideComponent);
			SpiderHandler val13 = default(SpiderHandler);
			GrabberPlant val14 = default(GrabberPlant);
			TraderScript val15 = default(TraderScript);
			if (((Component)val11).TryGetComponent<CorpseScript>(ref val12))
			{
				specialdata1 = (byte)Array.IndexOf(val12.startSprites, ((Component)val12).GetComponent<SpriteRenderer>().sprite);
			}
			else if (((Component)val11).TryGetComponent<SpikeStabberScript>(ref spike))
			{
				specialdata1 = (spike.GetSpikeActivated() ? ((byte)1) : ((byte)0));
			}
			else if (((Component)val11).TryGetComponent<DrillPod_Update_MultiplayerPatch.Krokosha_DrillPod_OverrideComponent>(ref krokosha_DrillPod_OverrideComponent))
			{
				specialdata1 = (krokosha_DrillPod_OverrideComponent.working ? ((byte)1) : ((byte)0));
			}
			else if (go.TryGetComponent<SpiderHandler>(ref val13))
			{
				spider_target = val13.target;
				spider_biteCooldown = val13.biteCooldown;
				spider_stunTime = val13.stunTime;
			}
			else if (go.TryGetComponent<GrabberPlant>(ref val14))
			{
				grabberplant_tipPos = val14.tipPos;
				grabberplant_randOffset = Time.unscaledTime + val14.randOffset;
			}
			else if (go.TryGetComponent<TraderScript>(ref val15))
			{
				trader_desiredpos = val15.desiredPos;
			}
		}
	}

	private void ApplyTransform(SyncInfo si)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		GameObject go = si.go;
		Transform transform = go.transform;
		bool flag = !KM.dist2dsqrcheck_presqr(in pos, Vector2.op_Implicit(transform.position), 0.0021973997f);
		float num = rotation;
		bool flag2 = Mathf.Abs(Mathf.Repeat(num - transform.localRotation.z, 360f)) > 1f;
		if (scale.x != transform.localScale.x || scale.y != transform.localScale.y)
		{
			Vector3 localScale = default(Vector3);
			((Vector3)(ref localScale))._002Ector(scale.x, scale.y, transform.localScale.z);
			transform.localScale = localScale;
		}
		Rigidbody2D val = default(Rigidbody2D);
		if (go.TryGetComponent<Rigidbody2D>(ref val) && si.relaxed_combat_sync)
		{
			if (flag)
			{
				SpiderHandler val2 = default(SpiderHandler);
				if ((go.TryGetComponent<SpiderHandler>(ref val2) || si.relaxed_combat_sync) && si.tracker.is_within_anyones_view)
				{
					Body localBody = Util.GetLocalBody();
					Vector2 val3 = go.GetComponent<Collider2D>().ClosestPoint(Vector2.op_Implicit(((Component)localBody).transform.position));
					Util.GetClosestLimb(localBody, val3, out var dist_sqr);
					if (dist_sqr < combatrelaxdist * combatrelaxdist)
					{
						float num2 = KM.dist2dsqr(Vector2.op_Implicit(((Component)localBody).transform.position), Vector2.op_Implicit(transform.position));
						float num3 = KM.dist2dsqr(Vector2.op_Implicit(((Component)localBody).transform.position), in pos);
						float num4 = KM.dist2dsqr(Vector2.op_Implicit(transform.position), in pos);
						if (num2 < num3)
						{
							if (num4 > 2f)
							{
								pos = Vector2.LerpUnclamped(Vector2.op_Implicit(transform.position), pos, 0.8f);
							}
							else
							{
								pos = Vector2.LerpUnclamped(Vector2.op_Implicit(transform.position), pos, 0.2f);
							}
						}
						else
						{
							flag = false;
						}
						if (flag2)
						{
							num = Mathf.LerpAngle(val.rotation, num, 0.1f);
						}
						si.relaxed_combat_sync = true;
					}
					else
					{
						si.relaxed_combat_sync = false;
					}
				}
			}
			else if (flag2 && si.relaxed_combat_sync)
			{
				num = Mathf.LerpAngle(val.rotation, num, 0.1f);
			}
		}
		if (flag)
		{
			transform.position = Vector2.op_Implicit(pos);
		}
		if (flag2)
		{
			transform.rotation = Quaternion.Euler(0f, 0f, num);
		}
	}

	public void ReadPacketIntoObject(SyncInfo si)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0535: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0565: Unknown result type (might be due to invalid IL or missing references)
		//IL_056a: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		GameObject go = si.go;
		if (container_data1 == 0)
		{
			ApplyTransform(si);
			Rigidbody2D val = default(Rigidbody2D);
			if (go.TryGetComponent<Rigidbody2D>(ref val))
			{
				RigidbodyType2D val2 = (RigidbodyType2D)((!rb_dynamic) ? 2 : 0);
				if (val.bodyType != val2)
				{
					val.bodyType = val2;
				}
				if (rb_dynamic)
				{
					val.velocity = vel;
					val.angularVelocity = angvel;
					if (itemLock)
					{
						ComponentHolderProtocol.GetOrAddComponent<ItemLock>((Object)(object)val);
					}
				}
			}
		}
		Item val3 = default(Item);
		if (go.TryGetComponent<Item>(ref val3))
		{
			val3.condition = condition;
			val3.wetTime = Time.time - wetTime;
			if (container_data1 == 0)
			{
				if ((Object)(object)((Component)val3).transform.parent != (Object)null)
				{
					ItemSync.ItemsContainerInfo itemsContainerInfo = ItemSync.ItemGetContainerInfo(val3);
					if (itemsContainerInfo.has_container)
					{
						ItemSync.SafeUnloadItem(itemsContainerInfo);
					}
				}
			}
			else
			{
				ItemSync.ItemsContainerInfo itemsContainerInfo2 = ItemSync.ItemGetContainerInfo(val3);
				if (container_data1 >= 100)
				{
					if ((itemsContainerInfo2.is_in_surface_inv && (ushort)itemsContainerInfo2.knetid != (ushort)container_netId) || (Object)(object)itemsContainerInfo2.container != (Object)null)
					{
						ItemSync.SafeUnloadItem(itemsContainerInfo2);
						itemsContainerInfo2.has_container = false;
					}
					if (!itemsContainerInfo2.has_container && NetBody.TryGetNetBodyFromId(container_netId, out var nb))
					{
						si.go.transform.position = ((Component)nb).transform.position;
						if (container_data1 == 100)
						{
							if (!val3.Stats.wearable)
							{
								nb.body.AutoPickUpItem(val3);
							}
							else
							{
								ItemSync.Body_ForceWearWearable(nb.body, val3);
							}
						}
						else
						{
							int num = container_data1 - 200;
							if (nb.body.HoldingItem(num))
							{
								nb.body.DropItem(num);
							}
							nb.body.PickUpItem(si.item, num, true);
						}
						if (nb.IsBodyLocal())
						{
							ItemSync.RecordAndApplyInvState(nb.body);
						}
						if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
						{
							DebugHelp.OnNetEvent(Vector2.op_Implicit(((Component)nb).transform.position), $"OBJSYNC: Item pickup {si} -> {nb}  data1: {container_data1}");
						}
					}
				}
				else if (CheckIfContainerIsWrong(itemsContainerInfo2))
				{
					if (NetObjectRegistry.TryGetSyncInfo(container_netId, out var si2))
					{
						ItemSync.Container_ForceLoadItem(val3, si2.container);
					}
					else
					{
						NewCoolerObjectPacketWriteReadSystem.inst.Client_RequestObjectInfo(container_netId);
					}
				}
			}
			if (freshitemdrop)
			{
				ComponentHolderProtocol.GetOrAddComponent<FreshItemDrop>((Object)(object)val3);
			}
			if (compressedboughtitemtime > 0)
			{
				ComponentHolderProtocol.GetOrAddComponent<BoughtItem>((Object)(object)val3).time = boughtitemtime;
			}
			CustomItemBehaviour val4 = default(CustomItemBehaviour);
			PlushScript val5 = default(PlushScript);
			if (((Component)val3).TryGetComponent<CustomItemBehaviour>(ref val4) && ItemSync.CustomItemBehaviourStateCanBeJustByte(val3))
			{
				val4.state = specialdata1;
				if (val4.data != null && val4.data.Length != 0)
				{
					object obj = val4.data[0];
					if (obj is float)
					{
						_ = (float)obj;
						val4.data[0] = specialdata2;
					}
				}
			}
			else if (((Component)val3).TryGetComponent<PlushScript>(ref val5) && val5.index != specialdata1)
			{
				val5.index = specialdata1;
				j.Prefix(val5);
				((Component)val5).GetComponent<SpriteRenderer>().sprite = val5.possibleSprites[specialdata1];
				val5.selectedSound = val5.possibleSounds[specialdata1];
			}
			if ((Object)(object)val3.battery != (Object)null)
			{
				DoBattery(val3.battery);
			}
			GunScript val6 = default(GunScript);
			AmmoScript val7 = default(AmmoScript);
			if (((Component)val3).TryGetComponent<GunScript>(ref val6))
			{
				val6.racked = gun_racked;
				val6.safe = gun_safe;
				val6.hasMag = gun_hasMag;
				val6.roundInChamber = (RoundInChamber)((!gun_roundInChamber) ? 2 : (gun_roundInChamber_is_casing ? 1 : 0));
				val6.roundsInMag = ammo;
			}
			else if (((Component)val3).TryGetComponent<AmmoScript>(ref val7))
			{
				val7.rounds = ammo;
			}
		}
		else
		{
			BuildingEntity val8 = default(BuildingEntity);
			if (!go.TryGetComponent<BuildingEntity>(ref val8))
			{
				return;
			}
			val8.health = condition;
			Krokosha_BuildingEntity_TrackerComponent_for_backgroundified orAddComponent = ComponentHolderProtocol.GetOrAddComponent<Krokosha_BuildingEntity_TrackerComponent_for_backgroundified>((Object)(object)val8);
			if (!orAddComponent.is_backgroundified && building_backgroundified)
			{
				val8.Backgroundify();
			}
			orAddComponent.is_backgroundified = building_backgroundified;
			CorpseScript val9 = default(CorpseScript);
			SpikeStabberScript val10 = default(SpikeStabberScript);
			DrillPod_Update_MultiplayerPatch.Krokosha_DrillPod_OverrideComponent krokosha_DrillPod_OverrideComponent = default(DrillPod_Update_MultiplayerPatch.Krokosha_DrillPod_OverrideComponent);
			GrabberPlant val11 = default(GrabberPlant);
			TraderScript val12 = default(TraderScript);
			if (((Component)val8).TryGetComponent<CorpseScript>(ref val9))
			{
				((Component)val9).GetComponent<SpriteRenderer>().sprite = val9.startSprites[specialdata1];
			}
			else if (((Component)val8).TryGetComponent<SpikeStabberScript>(ref val10))
			{
				bool flag = specialdata1 != 0;
				if (flag == val10.GetSpikeActivated())
				{
					return;
				}
				if (flag)
				{
					val10.Stab();
					return;
				}
				if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
				{
					DebugHelp.OnNetEvent(Vector2.op_Implicit(((Component)val10).transform.position), "RESET SPIKE", Color.cyan);
				}
				Animator component = ((Component)val10).GetComponent<Animator>();
				component.Play("SpikeStab", -1, 0f);
				component.speed = 0f;
				val10.SetSpikeActivated(b: false);
			}
			else if (((Component)val8).TryGetComponent<DrillPod_Update_MultiplayerPatch.Krokosha_DrillPod_OverrideComponent>(ref krokosha_DrillPod_OverrideComponent))
			{
				if (specialdata1 == 1 && !krokosha_DrillPod_OverrideComponent.working)
				{
					DrillPod_OnUse_MultiplayerPatch.ForceRepairDrill(krokosha_DrillPod_OverrideComponent.drill);
				}
			}
			else if (go.TryGetComponent<GrabberPlant>(ref val11))
			{
				ComponentHolderProtocol.GetOrAddComponent<GrabberPlantNetSmoothener>((Object)(object)val11).OnNewTipPosReceived(grabberplant_tipPos);
				val11.randOffset = grabberplant_randOffset - Time.unscaledTime;
			}
			else if (go.TryGetComponent<TraderScript>(ref val12))
			{
				val12.desiredPos = trader_desiredpos;
			}
		}
	}

	private void DoBattery(BatteryItem battery)
	{
		if (current_battery == 0)
		{
			battery.UnloadBattery(true);
			return;
		}
		CurrentBattery currentBattery = (CurrentBattery)current_battery;
		string text = currentBattery.ToString();
		ItemInfo obj = Item.GlobalItems[text];
		BatteryInfo val = (BatteryInfo)(object)((obj is BatteryInfo) ? obj : null);
		battery.batteryType = text;
		battery.maxCharge = val.maxCharge;
	}

	private bool CheckIfContainerIsWrong(ItemSync.ItemsContainerInfo ici)
	{
		if (ici.has_container)
		{
			if ((Object)(object)ici.container != (Object)null)
			{
				if (ici.container_si == null || (ushort)ici.container_si.syncId != (ushort)container_netId)
				{
					ItemSync.SafeUnloadItem(ici);
					return true;
				}
				return false;
			}
			if (ici.is_netbody)
			{
				ItemSync.SafeUnloadItem(ici);
				return true;
			}
		}
		return true;
	}

	public void Write(NetDataWriter writer, List<bool> pack_bools, IDeltaPacketBase old)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		ItemOrBuildingCoolDeltaCompressablePacket obj = (ItemOrBuildingCoolDeltaCompressablePacket)(object)old;
		bool flag = false;
		writer.Put(container_data1);
		writer.Put((ushort)container_netId);
		pack_bools.Add(rb_dynamic);
		pack_bools.Add(itemLock);
		flag = obj.pos != pos;
		pack_bools.Add(flag);
		if (flag)
		{
			writer.Put(pos);
		}
		flag = obj.rotation != rotation;
		pack_bools.Add(flag);
		if (flag)
		{
			writer.Put(rotation);
		}
		flag = obj.scale != scale;
		pack_bools.Add(flag);
		if (flag)
		{
			writer.Put(scale);
		}
		flag = obj.vel != vel;
		pack_bools.Add(flag);
		if (flag)
		{
			writer.Put(vel);
		}
		flag = obj.angvel_quantized != angvel_quantized;
		pack_bools.Add(flag);
		if (flag)
		{
			writer.Put(angvel_quantized);
		}
		flag = obj.condition != condition;
		pack_bools.Add(flag);
		if (flag)
		{
			writer.Put(condition);
		}
		flag = obj.compressedboughtitemtime != compressedboughtitemtime;
		pack_bools.Add(flag);
		if (flag)
		{
			writer.Put(compressedboughtitemtime);
		}
		flag = obj.compressedwetTime != compressedwetTime;
		pack_bools.Add(flag);
		if (flag)
		{
			writer.Put(compressedwetTime);
		}
		pack_bools.Add(freshitemdrop);
		flag = obj.current_battery != current_battery;
		pack_bools.Add(flag);
		if (flag)
		{
			writer.Put(current_battery);
		}
		flag = obj.specialdata1 != specialdata1;
		pack_bools.Add(flag);
		if (flag)
		{
			writer.Put(specialdata1);
		}
		flag = obj.specialdata2 != specialdata2;
		pack_bools.Add(flag);
		if (flag)
		{
			writer.Put(specialdata2);
		}
		flag = obj.ammo != ammo;
		pack_bools.Add(flag);
		if (flag)
		{
			writer.Put(ammo);
		}
		flag = obj.spider_target != spider_target;
		pack_bools.Add(flag);
		if (flag)
		{
			writer.Put(spider_target);
		}
		flag = obj.spider_biteCooldown != spider_biteCooldown;
		pack_bools.Add(flag);
		if (flag)
		{
			writer.Put(spider_biteCooldown);
		}
		flag = obj.spider_stunTime != spider_stunTime;
		pack_bools.Add(flag);
		if (flag)
		{
			writer.Put(spider_stunTime);
		}
		flag = obj.trader_desiredpos != trader_desiredpos;
		pack_bools.Add(flag);
		if (flag)
		{
			writer.Put(trader_desiredpos);
		}
		pack_bools.Add(gun_racked);
		pack_bools.Add(gun_safe);
		pack_bools.Add(gun_hasMag);
		pack_bools.Add(gun_roundInChamber);
		pack_bools.Add(gun_roundInChamber_is_casing);
		pack_bools.Add(building_backgroundified);
		flag = obj.grabberplant_tipPos != grabberplant_tipPos;
		pack_bools.Add(flag);
		if (flag)
		{
			writer.Put(grabberplant_tipPos);
		}
		flag = obj.grabberplant_randOffset != grabberplant_randOffset;
		pack_bools.Add(flag);
		if (flag)
		{
			writer.Put(grabberplant_randOffset);
		}
	}

	public void Read(NetDataReader reader, BitArray bitset)
	{
		reader.Get(ref container_data1);
		reader.Get(out container_netId);
		rb_dynamic = bitset[0];
		itemLock = bitset[1];
		if (bitset[2])
		{
			reader.Get(out pos);
		}
		if (bitset[3])
		{
			reader.Get(ref rotation);
		}
		if (bitset[4])
		{
			reader.Get(out scale);
		}
		if (bitset[5])
		{
			reader.Get(out vel);
		}
		if (bitset[6])
		{
			reader.Get(ref angvel_quantized);
		}
		if (bitset[7])
		{
			reader.Get(ref condition);
		}
		if (bitset[8])
		{
			reader.Get(ref compressedboughtitemtime);
		}
		if (bitset[9])
		{
			reader.Get(ref compressedwetTime);
		}
		freshitemdrop = bitset[10];
		if (bitset[11])
		{
			reader.Get(ref current_battery);
		}
		if (bitset[12])
		{
			reader.Get(ref specialdata1);
		}
		if (bitset[13])
		{
			reader.Get(ref specialdata2);
		}
		if (bitset[14])
		{
			reader.Get(ref ammo);
		}
		if (bitset[15])
		{
			reader.Get(out spider_target);
		}
		if (bitset[16])
		{
			reader.Get(ref spider_biteCooldown);
		}
		if (bitset[17])
		{
			reader.Get(ref spider_stunTime);
		}
		if (bitset[18])
		{
			reader.Get(out trader_desiredpos);
		}
		gun_racked = bitset[19];
		gun_safe = bitset[20];
		gun_hasMag = bitset[21];
		gun_roundInChamber = bitset[22];
		gun_roundInChamber_is_casing = bitset[23];
		building_backgroundified = bitset[24];
		if (bitset[25])
		{
			reader.Get(out grabberplant_tipPos);
		}
		if (bitset[26])
		{
			reader.Get(ref grabberplant_randOffset);
		}
	}
}
