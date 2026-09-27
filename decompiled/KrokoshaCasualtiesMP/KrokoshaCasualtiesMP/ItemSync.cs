using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using HarmonyLib;
using KrokoshaCasualtiesUtils;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

internal class ItemSync : BaseObjectSynchronizerStaticSerializerFunctions
{
	public class ItemsContainerInfo
	{
		public bool has_container;

		public bool is_netbody;

		public bool is_in_locplr_surface_inv;

		public bool is_in_surface_inv;

		public knetid knetid;

		public Item item;

		public bool is_wearing;

		public Limb limb;

		public knetid body_netid;

		public InventorySlot inv_slot;

		public byte inv_slot_number;

		public NetPlayer plr;

		public Container container;

		public SyncInfo container_si;

		public NetBody bodynpc;

		public bool JustContainerChanged(ItemsContainerInfo otherici)
		{
			return (Object)(object)container != (Object)(object)otherici.container;
		}

		public bool Equals(ItemsContainerInfo otherici)
		{
			if (has_container == otherici.has_container && is_netbody == otherici.is_netbody && (Object)(object)item == (Object)(object)otherici.item && is_wearing == otherici.is_wearing && (Object)(object)limb == (Object)(object)otherici.limb && (ushort)body_netid == (ushort)otherici.body_netid && inv_slot_number == otherici.inv_slot_number)
			{
				return (Object)(object)container == (Object)(object)otherici.container;
			}
			return false;
		}

		public override string ToString()
		{
			string text = "ICI-";
			if (!has_container)
			{
				text += "NONE";
			}
			else if (is_wearing)
			{
				text += $"WEAR-({bodynpc})";
			}
			else if (container_si != null)
			{
				text += $"CONT-{container_si}";
				if ((Object)(object)bodynpc != (Object)null)
				{
					text += $"-({bodynpc})";
				}
			}
			else if ((Object)(object)inv_slot != (Object)null)
			{
				text += $"INV-{inv_slot_number.ToString()}-({bodynpc})";
			}
			return text;
		}
	}

	private const float RareItemSyncFrequency = 1.1945f;

	public static HashSet<SyncInfo> ItemContainerChanged = new HashSet<SyncInfo>();

	internal static SyncInfo[] Client_last_inventory_state = new SyncInfo[8];

	public const float max_items_distance_to_consider = 240f;

	public const float max_plr_view_distance = 64f;

	public const float max_items_distance_to_consider_sqr = 57600f;

	public const float max_plr_view_distance_sqr = 4096f;

	internal const int max_items_to_check_per_iteration = 30;

	internal static int cur_items_check_index = 0;

	internal static List<Item> cur_items_check = new List<Item>();

	public static bool verbose => KrokoshaScavMultiplayer.verbose;

	public static Dictionary<GameObject, SyncInfo> SyncRegistry => NetObjectRegistry.SyncRegistry;

	public static Dictionary<knetid, SyncInfo> NetIdToSyncInfoDict => NetObjectRegistry.NetIdToSyncInfoDict;

	public static bool CustomItemBehaviourStateCanBeJustByte(Item item)
	{
		if (!(item.id == "flashlight"))
		{
			return item.id == "emergencylight";
		}
		return true;
	}

	public static ItemsContainerInfo ItemGetContainerInfo(Item item)
	{
		ItemsContainerInfo itemsContainerInfo = new ItemsContainerInfo();
		itemsContainerInfo.item = item;
		Transform parent = ((Component)item).transform.parent;
		if (Object.op_Implicit((Object)(object)parent))
		{
			Limb val = default(Limb);
			InventorySlot val2 = default(InventorySlot);
			Container container = default(Container);
			if (item.Stats.wearable && ((Component)parent).TryGetComponent<Limb>(ref val))
			{
				itemsContainerInfo.has_container = true;
				itemsContainerInfo.is_netbody = true;
				itemsContainerInfo.is_wearing = true;
				itemsContainerInfo.is_in_surface_inv = true;
				itemsContainerInfo.limb = val;
				if (val.body.TryGetNetBody(out itemsContainerInfo.bodynpc))
				{
					itemsContainerInfo.plr = itemsContainerInfo.bodynpc.plr;
					itemsContainerInfo.body_netid = itemsContainerInfo.bodynpc.netId;
					itemsContainerInfo.knetid = itemsContainerInfo.body_netid;
				}
				else
				{
					Plugin.log.LogError((object)$"Item is being worn, but NetBody was not found from body wtf, item: {((Object)item).name}\n{new StackTrace()}");
				}
			}
			else if (((Component)parent).TryGetComponent<InventorySlot>(ref val2))
			{
				itemsContainerInfo.has_container = true;
				itemsContainerInfo.is_netbody = true;
				itemsContainerInfo.inv_slot = val2;
				itemsContainerInfo.inv_slot_number = (byte)Traverse.Create((object)val2).Field("slot").GetValue<int>();
				itemsContainerInfo.is_in_surface_inv = true;
				if (val2.body.IsBodyLocal())
				{
					itemsContainerInfo.is_in_locplr_surface_inv = true;
				}
				if (val2.body.TryGetNetBody(out itemsContainerInfo.bodynpc))
				{
					itemsContainerInfo.plr = itemsContainerInfo.bodynpc.plr;
					itemsContainerInfo.body_netid = itemsContainerInfo.bodynpc.netId;
					itemsContainerInfo.knetid = itemsContainerInfo.body_netid;
				}
				else
				{
					Plugin.log.LogError((object)$"Item is in inventory slot, but NetBody was not found from body wtf, item: {((Object)item).name}\n{new StackTrace()}");
				}
			}
			else if (((Component)parent).TryGetComponent<Container>(ref container))
			{
				itemsContainerInfo.has_container = true;
				NetBody componentInParent = ((Component)item).GetComponentInParent<NetBody>();
				if ((Object)(object)componentInParent == (Object)null)
				{
					itemsContainerInfo.is_netbody = false;
				}
				else
				{
					itemsContainerInfo.is_netbody = true;
					itemsContainerInfo.body_netid = componentInParent.netId;
					itemsContainerInfo.plr = componentInParent.plr;
				}
				itemsContainerInfo.bodynpc = componentInParent;
				itemsContainerInfo.container = container;
				if (NetObjectRegistry.TryGetSyncInfoOrRegister((Component)(object)parent, out var si))
				{
					itemsContainerInfo.container_si = si;
					itemsContainerInfo.knetid = si.syncId;
				}
			}
		}
		return itemsContainerInfo;
	}

	public static ItemsContainerInfo ItemGetContainerInfo(GameObject go)
	{
		return ItemGetContainerInfo(go.GetComponent<Item>());
	}

	public static void Body_ForceWearWearable(Body body, Item item)
	{
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		Limb val = body.LimbByName(item.Stats.desiredWearLimb);
		if (val.dismembered)
		{
			return;
		}
		Item wearableBySlotID = body.GetWearableBySlotID(item.Stats.wearSlotId);
		if ((Object)(object)wearableBySlotID != (Object)null)
		{
			if ((Object)(object)wearableBySlotID == (Object)(object)item)
			{
				return;
			}
			body.DropWearable(wearableBySlotID);
		}
		if ((Object)(object)((Component)item).transform.parent != (Object)null)
		{
			SafeUnloadItem(item);
		}
		Container val2 = default(Container);
		if (item.TryGetParentContainer(ref val2))
		{
			val2.UnloadItem(item, (Body)null);
			if (body.IsBodyLocal())
			{
				PlayerCamera.main.PlayBackpackSound();
			}
		}
		item.rb.simulated = false;
		((Renderer)((Component)item).GetComponent<SpriteRenderer>()).sortingOrder = ((Renderer)((Component)val).GetComponent<SpriteRenderer>()).sortingOrder + item.Stats.wearableVisualOffset;
		((Component)item).transform.SetParent(((Component)val).transform);
		((Component)item).transform.localScale = Vector3.one;
		((Component)item).transform.localRotation = Quaternion.identity;
		((Component)item).transform.localPosition = Vector3.zero;
		Wearable val3 = default(Wearable);
		if (((Component)item).TryGetComponent<Wearable>(ref val3))
		{
			val3.CreateSprites(body);
		}
	}

	public static void Container_ForceLoadItem(Item item, Container container)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		((Component)item).transform.SetParent(((Component)container).transform);
		item.rb.simulated = false;
		Transform transform = ((Component)item).transform;
		Bounds bounds = ((Component)container).GetComponent<Collider2D>().bounds;
		transform.position = ((Bounds)(ref bounds)).center;
		((Renderer)((Component)item).GetComponent<SpriteRenderer>()).enabled = container.itemsVisible;
		if (container.itemsVisible)
		{
			((Renderer)((Component)item).GetComponent<SpriteRenderer>()).sortingOrder = ((Renderer)((Component)container).GetComponent<SpriteRenderer>()).sortingOrder;
			((Component)item).transform.localEulerAngles = new Vector3(0f, 0f, 180f);
		}
		if (Object.op_Implicit((Object)(object)((Component)item).GetComponent<Wearable>()))
		{
			((Component)item).GetComponent<Wearable>().ClearSprites();
		}
		Container.UpdateItemLight(((Component)item).gameObject, !container.itemsVisible);
		if ((Object)(object)PlayerCamera.main.currentContainer == (Object)(object)container)
		{
			PlayerCamera.main.RepopulateContainer();
		}
	}

	public static void SafeUnloadItem(Item item)
	{
		SafeUnloadItem(ItemGetContainerInfo(item));
	}

	public static void SafeUnloadItem(ItemsContainerInfo ici)
	{
		if (!ici.has_container)
		{
			return;
		}
		if (ici.is_wearing)
		{
			if (NetBody.TryGetNetBodyFromId(ici.body_netid, out var nb))
			{
				Body_DropWearable_MultiplayerPatch.Force(nb.body, ici.item);
			}
		}
		else if ((Object)(object)ici.inv_slot != (Object)null)
		{
			ici.inv_slot.body.DropItem(ici.item);
			if (NetObjectRegistry.TryGetSyncInfo((Component)(object)ici.item, out var si))
			{
				int num = Array.IndexOf(Client_last_inventory_state, si);
				if (num != -1)
				{
					Client_last_inventory_state[num] = null;
				}
			}
		}
		else
		{
			ici.container.UnloadItem(ici.item, (Body)null);
			if ((Object)(object)PlayerCamera.main.currentContainer == (Object)(object)ici.container)
			{
				PlayerCamera.main.RepopulateContainer();
			}
		}
	}

	public static void BetterUseItem(Body body, Item item)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		if (item.Stats.wearable)
		{
			Body_ForceWearWearable(body, item);
			if ((Object)(object)((Component)item).transform.parent != (Object)null)
			{
				if (body.IsBodyLocal())
				{
					PlayerCamera.main.UpdateWearables();
				}
				return;
			}
			((Component)item).transform.position = ((Component)body).transform.position;
		}
		if (item.Stats.usable)
		{
			CoUtils_instance_MultiplayerPatch.cur_override_instance = body.GetCoUtilsInstance();
			item.Stats.useAction.Invoke(body, item);
		}
	}

	public static bool ItemIsInContainer(Item item)
	{
		return (Object)(object)((Component)item).transform.parent != (Object)null;
	}

	public static bool ItemIsInContainer(GameObject o)
	{
		return (Object)(object)o.transform.parent != (Object)null;
	}

	public static bool ItemIsInContainer(SyncInfo si)
	{
		return (Object)(object)si.go.transform.parent != (Object)null;
	}

	public static bool ItemIsRegistered(Item item)
	{
		return NetObjectRegistry.IsRegistered(((Component)item).gameObject);
	}

	public static void RecordAndApplyInvState(Body body)
	{
		for (int i = 0; i < body.slots.Length; i++)
		{
			Item item = body.GetItem(i);
			Client_last_inventory_state[i] = NetObjectRegistry.GetSyncInfo((Component)(object)item);
		}
	}

	public static SyncInfo[] RecordInvState(Body body)
	{
		SyncInfo[] array = new SyncInfo[8];
		for (int i = 0; i < body.slots.Length; i++)
		{
			Item item = body.GetItem(i);
			array[i] = NetObjectRegistry.GetSyncInfo((Component)(object)item);
		}
		return array;
	}

	public static void Client_AnnounceContainerChange(SyncInfo si)
	{
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		ItemsContainerInfo itemsContainerInfo = ItemGetContainerInfo(si.item);
		SyncInfo container_si = itemsContainerInfo.container_si;
		if (si == null || itemsContainerInfo.is_in_locplr_surface_inv || (itemsContainerInfo.is_wearing && (Object)(object)itemsContainerInfo.limb != (Object)null && itemsContainerInfo.limb.IsBodyLocal()))
		{
			return;
		}
		int num = Array.IndexOf(Client_last_inventory_state, si);
		if (num != -1)
		{
			Client_last_inventory_state[num] = null;
		}
		si.SetIgnoreTimeForRoundTrip();
		NetDataWriter writer = Net.CreateWriter(10117);
		writer.Put(itemsContainerInfo.has_container && container_si != null);
		writer.Put((ushort)si.syncId);
		if (container_si != null)
		{
			writer.Put((ushort)container_si.syncId);
		}
		else
		{
			writer.Put((ushort)(knetid)(ushort)0);
		}
		if (log.verbose)
		{
			if (container_si == null)
			{
				Plugin.log.LogWarning((object)$"CLIENT: requesting UNLOAD ItemChangeContainer: {si}");
			}
			else
			{
				Plugin.log.LogWarning((object)$"CLIENT: requesting ItemChangeContainer: {si} -> {container_si}");
			}
		}
		Net.Client_Send((DeliveryMethod)2, in writer);
	}

	public static void Server_AnnounceCombineSound(SyncInfo si1, SyncInfo si2, knetid combinerguyguy)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Invalid comparison between Unknown and I4
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Invalid comparison between Unknown and I4
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Invalid comparison between Unknown and I4
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Invalid comparison between Unknown and I4
		Item item = si1.item;
		Item item2 = si2.item;
		string text = null;
		GunScript component = ((Component)item).GetComponent<GunScript>();
		AmmoScript component2 = ((Component)item).GetComponent<AmmoScript>();
		AmmoScript component3 = ((Component)item2).GetComponent<AmmoScript>();
		if (Object.op_Implicit((Object)(object)component) && Object.op_Implicit((Object)(object)component3))
		{
			AmmoScript val = component3;
			if (val.ammoType == component.ammoType)
			{
				if (component.racked && (int)component.roundInChamber == 2 && (int)val.itemType == 0)
				{
					text = "gunloadshell";
				}
				else if (!component.hasMag && (int)component.feedType == 0 && (int)val.itemType == 1)
				{
					text = "gunloadmag";
				}
				else if ((int)component.feedType == 1 && (int)val.itemType == 0 && component.roundsInMag < component.magCapacity)
				{
					text = "gunloadshell";
				}
			}
		}
		else if (Object.op_Implicit((Object)(object)component2) && Object.op_Implicit((Object)(object)component3))
		{
			AmmoScript val2 = component3;
			if ((int)val2.itemType == 0 && (int)component2.itemType == 1 && val2.ammoType == component2.ammoType && component2.rounds < component2.maxRounds)
			{
				text = "gunloadshell";
			}
		}
		else
		{
			text = "combine";
		}
		if (text != null)
		{
			if (verbose)
			{
				Plugin.log.LogInfo((object)("TEMP DEV DELETEME: Sending combine sound: " + text + " "));
			}
			ServerMain.Server_AnnounceSound(Vector2.op_Implicit(((Component)item).transform.position), text, ServerMain.GetListOfClientIdsExceptThisAndHost(combinerguyguy));
		}
	}

	public static bool TryGetItem(knetid syncid, out SyncInfo isi, out Item item)
	{
		if (NetIdToSyncInfoDict.TryGetValue(syncid, out var value) && value.IsItem())
		{
			isi = value;
			item = value.item;
			return true;
		}
		isi = null;
		item = null;
		return false;
	}

	public static bool TryGetItemSyncInfo(Item item, out SyncInfo isi)
	{
		if (SyncRegistry.TryGetValue(((Component)item).gameObject, out var value))
		{
			isi = value;
			return true;
		}
		isi = null;
		return false;
	}

	public static bool CheckIfBodyReachThisItem(Item item, Body my_body, float maxdist = 20f, bool check_obstruction = false)
	{
		return CheckIfBodyReachThisItem(((Component)item).gameObject, my_body, maxdist, check_obstruction);
	}

	public static bool CheckIfBodyReachThisItem(SyncInfo item, Body my_body, float maxdist = 20f, bool check_obstruction = false)
	{
		return CheckIfBodyReachThisItem(item.go, my_body, maxdist, check_obstruction);
	}

	public static bool CheckIfBodyReachThisItem(GameObject item, Body my_body, float maxdist = 20f, bool check_obstruction = false)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		NetBody componentInParent = item.GetComponentInParent<NetBody>();
		if ((Object)(object)componentInParent != (Object)null && (Object)(object)componentInParent.body == (Object)(object)my_body)
		{
			return true;
		}
		ItemsContainerInfo itemsContainerInfo = ItemGetContainerInfo(item);
		if (!CheckIfBodyCanStealThisItem(item, my_body, itemsContainerInfo))
		{
			return false;
		}
		if (!KM.dist2dsqrcheck(Vector2.op_Implicit(item.transform.position), Vector2.op_Implicit(((Component)my_body).transform.position), maxdist))
		{
			return false;
		}
		if (check_obstruction)
		{
			if (itemsContainerInfo.is_netbody)
			{
				if (!Util.AccurateRaycastInteractionCheckObstruction(my_body, itemsContainerInfo.plr.body, do_effect: false))
				{
					return false;
				}
			}
			else if (!Util.AccurateRaycastInteractionCheck(my_body, Vector2.op_Implicit(item.transform.position), do_effect: false))
			{
				return false;
			}
		}
		return true;
	}

	internal static void Client_SendDoubleItemPickup(Item i1, Item i2)
	{
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		SyncInfo syncInfo = NetObjectRegistry.GetSyncInfo((Component)(object)i1);
		SyncInfo syncInfo2 = NetObjectRegistry.GetSyncInfo((Component)(object)i2);
		bool flag = false;
		if (syncInfo != null)
		{
			syncInfo.SetIgnoreTimeForRoundTrip(1.0);
			flag = true;
		}
		if (syncInfo2 != null)
		{
			syncInfo2.SetIgnoreTimeForRoundTrip(1.0);
			flag = true;
		}
		if (flag)
		{
			NetDataWriter writer = Net.CreateWriter(10118);
			if (syncInfo != null)
			{
				ItemsContainerInfo itemsContainerInfo = ItemGetContainerInfo(syncInfo.item);
				writer.Put(itemsContainerInfo.inv_slot_number);
				writer.Put((ushort)syncInfo.syncId);
			}
			if (syncInfo2 != null)
			{
				ItemsContainerInfo itemsContainerInfo2 = ItemGetContainerInfo(syncInfo2.item);
				writer.Put(itemsContainerInfo2.inv_slot_number);
				writer.Put((ushort)syncInfo2.syncId);
			}
			Net.Client_Send((DeliveryMethod)2, in writer);
		}
	}

	private static void DoClientInventoryUpdate()
	{
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		SyncInfo[] array = RecordInvState(Util.GetLocalBody());
		try
		{
			for (int i = 0; i < array.Length; i++)
			{
				SyncInfo syncInfo = Client_last_inventory_state[i];
				SyncInfo syncInfo2 = array[i];
				if (syncInfo == syncInfo2)
				{
					continue;
				}
				syncInfo?.SetIgnoreTimeForRoundTrip(1.0);
				syncInfo2?.SetIgnoreTimeForRoundTrip(1.0);
				if (syncInfo2 == null)
				{
					if (!array.Contains(syncInfo))
					{
						NetDataWriter writer = Net.CreateWriter(10112);
						writer.Put((ushort)syncInfo.syncId);
						Net.Client_Send((DeliveryMethod)2, in writer);
					}
				}
				else
				{
					ItemsContainerInfo itemsContainerInfo = ItemGetContainerInfo(syncInfo2.item);
					NetDataWriter writer2 = Net.CreateWriter(10118);
					writer2.Put(itemsContainerInfo.inv_slot_number);
					writer2.Put((ushort)syncInfo2.syncId);
					Net.Client_Send((DeliveryMethod)2, in writer2);
				}
			}
		}
		catch (Exception ex)
		{
			log.error("DoClientInventoryUpdate: " + ex.ToString());
		}
		for (int k = 0; k < array.Length; k++)
		{
			Client_last_inventory_state[k] = array[k];
		}
	}

	private void Update()
	{
		if (!KrokoshaScavMultiplayer.IsNetworkActiveAndIsWorldGenerated() || !Net.is_client)
		{
			return;
		}
		if (ItemContainerChanged.Count > 0)
		{
			foreach (SyncInfo item in ItemContainerChanged)
			{
				Client_AnnounceContainerChange(item);
			}
			ItemContainerChanged.Clear();
		}
		DoClientInventoryUpdate();
	}

	public static bool CheckIfBodyCanStealThisItem(GameObject item, Body my_body, ItemsContainerInfo ici = null)
	{
		return CheckIfBodyCanStealThisItem(item.GetComponent<Item>(), my_body, ici);
	}

	public static bool CheckIfBodyCanStealThisItem(Item item, Body my_body, ItemsContainerInfo ici = null)
	{
		if (ici == null)
		{
			ici = ItemGetContainerInfo(item);
		}
		if (ici.has_container && (Object)(object)ici.bodynpc != (Object)null)
		{
			if (KrokoshaScavMultiplayer.rules.NoInventoryLock)
			{
				return true;
			}
			Body body = ici.bodynpc.body;
			if ((Object)(object)body != (Object)(object)my_body && body.conscious && (ici.is_wearing || ici.is_netbody))
			{
				return false;
			}
		}
		return true;
	}

	public static bool TryGetSyncInfo(Item item, out SyncInfo si)
	{
		if (SyncRegistry.TryGetValue(((Component)item).gameObject, out var value))
		{
			si = value;
			return true;
		}
		si = null;
		return false;
	}

	public static bool TryGetItemSyncInfo(knetid item_syncid, out SyncInfo si)
	{
		if (NetIdToSyncInfoDict.TryGetValue(item_syncid, out var value) && value.IsItem())
		{
			si = value;
			return true;
		}
		si = null;
		return false;
	}

	public static bool ItemCanBeIgnoredForNetwork(Item item)
	{
		if ((Object)(object)item == (Object)null)
		{
			return true;
		}
		return NetObjectRegistry.ObjectCanBeIgnoredForNetwork(((Component)item).gameObject);
	}

	public static bool IsFinite(float value)
	{
		if (!float.IsNaN(value))
		{
			return !float.IsInfinity(value);
		}
		return false;
	}

	private static (bool, bool) _ApplyInventory(NetBody pb, ref NetDataReader reader, bool is_server)
	{
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		NetPlayer plr = pb.plr;
		Body body = pb.body;
		knetid[] array = new knetid[body.slots.Length];
		for (int i = 0; i < body.slots.Length; i++)
		{
			reader.Get(out knetid result);
			array[i] = result;
		}
		byte bayt = default(byte);
		reader.Get(ref bayt);
		Bitset8 bitset = new Bitset8(bayt);
		byte bayt2 = default(byte);
		reader.Get(ref bayt2);
		Bitset8 bitset2 = new Bitset8(bayt2);
		bool item = false;
		bool item2 = false;
		if (is_server && plr.is_local)
		{
			item = true;
		}
		else
		{
			for (int k = 0; k < body.slots.Length; k++)
			{
				knetid syncid = array[k];
				_ = bitset2[k];
				Item val = body.GetItem(k);
				SyncInfo si = null;
				if ((Object)(object)val != (Object)null && !NetObjectRegistry.TryGetSyncInfo(((Component)val).gameObject, out si))
				{
					SafeUnloadItem(ItemGetContainerInfo(val));
					val = null;
				}
				if (!bitset2[k])
				{
					continue;
				}
				if (bitset[k])
				{
					if (!NetObjectRegistry.TryGetSyncInfo(syncid, out var si2))
					{
						item2 = true;
						if (is_server)
						{
						}
					}
					else
					{
						if (!((Object)(object)val != (Object)(object)si2.item))
						{
							continue;
						}
						if (!is_server || CheckIfBodyReachThisItem(si2, body, 14f))
						{
							item = true;
							if (!((Object)(object)val == (Object)null))
							{
								ItemsContainerInfo itemsContainerInfo = ItemGetContainerInfo(si.item);
								if (itemsContainerInfo.has_container)
								{
									SafeUnloadItem(itemsContainerInfo);
								}
							}
							ItemsContainerInfo itemsContainerInfo2 = ItemGetContainerInfo(si2.item);
							if (itemsContainerInfo2.has_container)
							{
								SafeUnloadItem(itemsContainerInfo2);
							}
							body.PickUpItem(si2.item, k, true);
							if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
							{
								if (is_server)
								{
									DebugHelp.OnNetEvent(pb.pos, $"S: inv ItemPickup {pb.plr} {si2}");
								}
								else
								{
									DebugHelp.OnNetEvent(pb.pos, $"C: inv ItemPickup {pb.plr} {si2}");
								}
							}
						}
						else
						{
							item2 = true;
						}
					}
				}
				else
				{
					if (!((Object)(object)val != (Object)null))
					{
						continue;
					}
					SafeUnloadItem(ItemGetContainerInfo(val));
					item = true;
					if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
					{
						if (is_server)
						{
							DebugHelp.OnNetEvent(pb.pos, $"S: inv DropItem {pb.plr} {val}");
						}
						else
						{
							DebugHelp.OnNetEvent(pb.pos, $"C: inv DropItem {pb.plr} {val}");
						}
					}
				}
			}
		}
		return (item, item2);
	}

	[ServerReceiver(10122)]
	private static void ServerReceiver__BodyCombineItems(knetid clientId, ref NetDataReader reader)
	{
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out knetid result);
		reader.Get(out knetid result2);
		if (!Object.op_Implicit((Object)(object)WorldGeneration.world) || WorldGeneration.world.generatingWorld)
		{
			return;
		}
		Body bodyFromClientId = NetPlayer.GetBodyFromClientId(clientId);
		if (!((Object)(object)bodyFromClientId != (Object)null) || !bodyFromClientId.conscious || !TryGetItem(result, out var isi, out var _) || !TryGetItem(result2, out var isi2, out var _))
		{
			return;
		}
		if (CheckIfBodyReachThisItem(isi, bodyFromClientId) && CheckIfBodyReachThisItem(isi2, bodyFromClientId))
		{
			if (!Util.IsBodyLocal(bodyFromClientId))
			{
				bool flag = bodyFromClientId.CanCombine(isi.item, isi2.item);
				if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
				{
					DebugHelp.OnNetEvent(Vector2.op_Implicit(((Component)bodyFromClientId).transform.position), $"S: BodyCombineItems {flag} {clientId} {isi} x {isi2} ");
				}
				if (flag)
				{
					Server_AnnounceCombineSound(isi, isi2, clientId);
					Body_CombineItems_MultiplayerPatch.Body_CombineItems(bodyFromClientId, isi.item, isi2.item);
				}
			}
			NetObjectRegistry.Server_ObjectSyncSingle(isi.go);
		}
		else if (verbose)
		{
			Plugin.log.LogWarning((object)$"SERVER BodyCombineItems: {isi} {isi2} , but client was too far!");
		}
	}

	[ServerReceiver(10123)]
	private static void ServerReceiver__CombineLiquidItems(knetid clientId, ref NetDataReader reader)
	{
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out knetid result);
		reader.Get(out knetid result2);
		float num = default(float);
		reader.Get(ref num);
		if (!Util.IsWorldGenerated())
		{
			return;
		}
		Body bodyFromClientId = NetPlayer.GetBodyFromClientId(clientId);
		if (!((Object)(object)bodyFromClientId != (Object)null) || !bodyFromClientId.conscious || !IsFinite(num) || !TryGetItem(result, out var isi, out var _) || !TryGetItem(result2, out var isi2, out var _) || !isi.IsLiquidContainer() || !isi2.IsLiquidContainer())
		{
			return;
		}
		if (CheckIfBodyReachThisItem(isi, bodyFromClientId) && CheckIfBodyReachThisItem(isi2, bodyFromClientId))
		{
			if (!Util.IsBodyLocal(bodyFromClientId))
			{
				bool flag = bodyFromClientId.CanCombine(isi.item, isi2.item);
				if (verbose)
				{
					Plugin.log.LogInfo((object)$"TEMP DEV DELETEME: server received CombineLiquidItems {flag} {isi} x {isi2} ");
				}
				if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
				{
					DebugHelp.OnNetEvent(Vector2.op_Implicit(((Component)bodyFromClientId).transform.position), $"S: CombineLiquidItems {flag} {clientId} {isi} x {isi2} ");
				}
				if (flag)
				{
					ServerMain.Server_AnnounceSound(Vector2.op_Implicit(isi.go.transform.position), "waterpour", ServerMain.GetListOfClientIdsExceptThisAndHost(clientId));
					bodyFromClientId.CombineLiquids(isi.liquidcontainer, isi2.liquidcontainer, num);
				}
			}
			if (verbose)
			{
				Plugin.log.LogInfo((object)"TEMP DEV DELETEME: CombineLiquidItems ENDED ");
			}
			NetObjectRegistry.Server_ObjectSyncSingle(isi.go);
		}
		else if (verbose)
		{
			Plugin.log.LogWarning((object)$"SERVER CombineLiquidItems: {isi} {isi2} , but client was too far!");
		}
	}

	[ServerReceiver(10117)]
	private static void ServerReceiver__ItemChangeContainer(knetid clientId, ref NetDataReader reader)
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		bool flag = default(bool);
		reader.Get(ref flag);
		reader.Get(out knetid result);
		reader.Get(out knetid result2);
		if (!Util.IsWorldGenerated() || !NetPlayer.TryGetNetPlayerAndBodyFromClientId(clientId, out var plr, out var body) || !body.conscious)
		{
			return;
		}
		if (TryGetItem(result, out var isi, out var item))
		{
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(Vector2.op_Implicit(((Component)body).transform.position), $"S: ItemChangeContainer {plr} load: {flag}  ids: {result} -> {result2} ");
			}
			if (CheckIfBodyReachThisItem(isi, body, 20f, check_obstruction: true))
			{
				if (flag)
				{
					if (TryGetItem(result2, out var isi2, out var _) && isi2.IsContainer() && CheckIfBodyReachThisItem(isi2, body, 20f, check_obstruction: true))
					{
						ItemsContainerInfo itemsContainerInfo = ItemGetContainerInfo(item);
						if (itemsContainerInfo.container_si != isi2)
						{
							SafeUnloadItem(itemsContainerInfo);
							isi2.container.LoadItem(item);
							if (verbose)
							{
								Plugin.log.LogInfo((object)$"SERVER received ItemChangeContainer  {plr}  LOAD ITEM  ids: {isi} -> {isi2}  ");
							}
							NetObjectRegistry.Server_ObjectSyncSingle(isi.go);
							return;
						}
					}
					if (verbose)
					{
						Plugin.log.LogInfo((object)$"SERVER received ItemChangeContainer  {plr}  hascontainer FAILED  ids: {isi} -> {isi2}  ");
					}
				}
				else
				{
					if (verbose)
					{
						Plugin.log.LogInfo((object)$"SERVER received ItemChangeContainer  {plr}  UNLOAD ITEM  ids: {isi}  ");
					}
					SafeUnloadItem(ItemGetContainerInfo(item));
					NetObjectRegistry.Server_ObjectSyncSingle(isi.go);
				}
				return;
			}
		}
		if (verbose)
		{
			Plugin.log.LogInfo((object)$"SERVER received ItemChangeContainer  {plr}  FAILED  ids: {result} -> {result2}  ");
		}
	}

	[ServerReceiver(10115)]
	private static void ServerReceiver__ThrowItem(knetid clientId, ref NetDataReader reader)
	{
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out knetid result);
		float num = default(float);
		reader.Get(ref num);
		if (!Util.IsWorldGenerated() || !IsFinite(num))
		{
			return;
		}
		if (NetIdToSyncInfoDict.TryGetValue(result, out var value) && value.IsItem() && NetPlayer.TryGetNetPlayerAndNetBodyFromClientId(clientId, out var _, out var pb))
		{
			Body body = pb.body;
			bool flag = Util.IsBodyLocal(body);
			if (flag || (Object)(object)body.GetItem(body.handSlot) == (Object)(object)value.item)
			{
				num = Mathf.Clamp01(num);
				if ((Object)(object)ItemGetContainerInfo(value.item).bodynpc == (Object)(object)pb && !flag)
				{
					body.ThrowItem(num);
				}
				if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
				{
					DebugHelp.OnNetEvent(Vector2.op_Implicit(((Component)body).transform.position), $"S: ThrowItem {clientId} {value} {num} ");
				}
				NetObjectRegistry.Server_ObjectSyncSingle(value.go);
				return;
			}
		}
		if (verbose)
		{
			Plugin.log.LogWarning((object)$"Server declines ThrowItem: {result} {value}  {num} ");
		}
	}

	private static (Item, byte) _Server_ReadOneItem(NetPlayer plr, ref NetDataReader reader)
	{
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		byte b = default(byte);
		reader.Get(ref b);
		reader.Get(out knetid result);
		if (b >= plr.body.slots.Length)
		{
			return (null, b);
		}
		if (TryGetItem(result, out var isi, out var item) && plr.body.conscious && CheckIfBodyReachThisItem(item, plr.body))
		{
			plr.body.DropItem((int)b);
			SafeUnloadItem(item);
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(plr.pos, $"S: ItemPickup slot:{b} {isi} ");
			}
			return (item, b);
		}
		NetDataWriter writer = Net.CreateWriter(10119);
		writer.Put((ushort)result);
		Net.Server_SendTo((DeliveryMethod)2, in writer, plr.clientId);
		if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
		{
			DebugHelp.OnNetEvent(plr.pos, $"S: DENIED ItemPickup {plr} {isi} ", Color.red);
		}
		return (null, b);
	}

	[ServerReceiver(10118)]
	private static void ServerReceiver__ItemPickup(knetid clientId, ref NetDataReader reader)
	{
		if (!NetPlayer.TryGetNetPlayerAndNetBodyFromClientId(clientId, out var plr, out var _))
		{
			return;
		}
		(Item, byte) tuple = _Server_ReadOneItem(plr, ref reader);
		if (!reader.EndOfData)
		{
			(Item, byte) tuple2 = _Server_ReadOneItem(plr, ref reader);
			if ((Object)(object)tuple2.Item1 != (Object)null)
			{
				plr.body.PickUpItem(tuple2.Item1, (int)tuple2.Item2, true);
			}
		}
		if ((Object)(object)tuple.Item1 != (Object)null)
		{
			plr.body.PickUpItem(tuple.Item1, (int)tuple.Item2, true);
		}
	}

	[ClientReceiver(10119, true)]
	private static void ClientReceiver_ItemPickupDENY(knetid _, ref NetDataReader reader)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out knetid result);
		if (!TryGetItem(result, out var isi, out var item))
		{
			return;
		}
		if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
		{
			DebugHelp.OnNetEvent(isi.position, $"C: DENIED ItemPickup {isi} ", Color.red);
		}
		ItemsContainerInfo itemsContainerInfo = ItemGetContainerInfo(item);
		if (!itemsContainerInfo.is_in_surface_inv)
		{
			return;
		}
		if ((Object)(object)itemsContainerInfo.bodynpc != (Object)null && itemsContainerInfo.bodynpc.IsBodyLocal())
		{
			int num = Array.IndexOf(Client_last_inventory_state, item);
			if (num >= 0)
			{
				Client_last_inventory_state[num] = null;
			}
		}
		SafeUnloadItem(itemsContainerInfo);
		isi.last_update_time = Time.realtimeSinceStartupAsDouble;
	}

	[ServerReceiver(10112)]
	private static void ServerReceiver__ItemDropWearable(knetid clientId, ref NetDataReader reader)
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out knetid result);
		if (Util.IsWorldGenerated() && TryGetItem(result, out var isi, out var item) && NetPlayer.TryGetNetPlayerAndNetBodyFromClientId(clientId, out var _, out var pb) && pb.body.conscious && (Object)(object)((Component)item).transform.parent != (Object)null && CheckIfBodyReachThisItem(item, pb.body))
		{
			SafeUnloadItem(item);
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(pb.position, $"S: ItemDrop {ServerMain.GetPlayerFullDebugString(clientId)} {isi} ");
			}
			NetObjectRegistry.Server_ObjectSyncSingle(isi.go);
		}
		else
		{
			NewCoolerObjectPacketWriteReadSystem.inst.Server_ResetDeltaOnObj(clientId, result);
		}
	}

	[ServerReceiver(10113)]
	private static void ServerReceiver__ItemWearing(knetid clientId, ref NetDataReader reader)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out knetid result);
		if (Util.IsWorldGenerated() && TryGetItem(result, out var isi, out var item) && item.Stats.wearable)
		{
			Body bodyFromClientId = NetPlayer.GetBodyFromClientId(clientId);
			if ((Object)(object)bodyFromClientId != (Object)null && bodyFromClientId.alive && KM.dist2dsqrcheck(Vector2.op_Implicit(isi.go.transform.position), Vector2.op_Implicit(((Component)bodyFromClientId).transform.position), 14f))
			{
				if (CheckIfBodyCanStealThisItem(item, bodyFromClientId) && !Util.IsBodyLocal(bodyFromClientId))
				{
					SafeUnloadItem(item);
					((Component)item).transform.position = ((Component)bodyFromClientId).transform.position;
					Body_ForceWearWearable(bodyFromClientId, item);
				}
				if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
				{
					DebugHelp.OnNetEvent(Vector2.op_Implicit(((Component)bodyFromClientId).transform.position), $"S: ItemWearing {ServerMain.GetPlayerFullDebugString(clientId)} {isi} ");
				}
				NetObjectRegistry.Server_ObjectSyncSingle(isi.go);
				return;
			}
		}
		if (verbose)
		{
			Plugin.log.LogWarning((object)$"Server declines itemwearing: {ServerMain.GetPlayerFullDebugString(clientId)} {result} ");
		}
	}
}
