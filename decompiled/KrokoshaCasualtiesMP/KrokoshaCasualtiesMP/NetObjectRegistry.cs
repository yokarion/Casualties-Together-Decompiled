using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using KrokoshaCasualtiesUtils;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public class NetObjectRegistry : KrokoshaScavSingleton
{
	private const float _FastSyncFrequency = 0.043f;

	private const float _DefaultSyncFrequency = 0.211f;

	private const float _RareSyncFrequency = 1.894512f;

	private const byte _MaxObjectsToSyncAtOnce = 25;

	public static Dictionary<GameObject, SyncInfo> SyncRegistry = new Dictionary<GameObject, SyncInfo>();

	public static Dictionary<knetid, SyncInfo> NetIdToSyncInfoDict = new Dictionary<knetid, SyncInfo>();

	private const float immitiate_registration_search_radius = 80f;

	private static int MAX_REGISTER_BATCH = 16;

	private static int server_slowmode_type_counter = 0;

	internal static HashSet<string> lowpriority_objects_resourceids = new HashSet<string> { "casing", "droppings" };

	private static HashSet<string> networkignored_buildings_maingame = new HashSet<string> { "KSMUTLTI_IGN", "background", "lifepodbuttonheat", "lifepodbuttonshower", "lifepodheater", "lifepodshower", "lifepodpump", "sandvine", "marbleBackground", "mushroomdropperpayload" };

	private static float timer_fastsync = 0f;

	private static float timer_objsync = 0f;

	private static float timer_raresync = 0f;

	public static bool _DEV_ENABLE_SYNCINFO_SNITCHING = false;

	public static bool verbose => KrokoshaScavMultiplayer.verbose;

	public static void NewGO(GameObject obj)
	{
		SyncInfo value;
		if (KrokoshaScavMultiplayer.is_server)
		{
			Server_EnsureItemIsNetworkRegistered(obj);
		}
		else if (!SyncRegistry.TryGetValue(obj, out value) && !ObjectCanBeIgnoredForNetwork(obj))
		{
			SafeDestroyObject(obj);
		}
	}

	public static HashSet<GameObject> GetIdentifiedBackgroundsInRadius(Vector2 pos, float radius)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		HashSet<GameObject> hashSet = new HashSet<GameObject>();
		radius *= radius;
		foreach (Krokosha_BuildingEntity_TrackerComponent_for_backgroundified all_instance in Krokosha_BuildingEntity_TrackerComponent_for_backgroundified.all_instances)
		{
			if (all_instance.is_a_background_tilemap && KM.dist2dsqrcheck_presqr(Vector2.op_Implicit(((Component)all_instance).transform.position), in pos, radius))
			{
				hashSet.Add(((Component)all_instance).gameObject);
			}
		}
		return hashSet;
	}

	public static HashSet<GameObject> GatherClosestObjectsToSync(Vector2 pos, Body body, float radius)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		HashSet<GameObject> identifiedBackgroundsInRadius = GetIdentifiedBackgroundsInRadius(pos, radius);
		IEnumerable<GameObject> enumerable = from x in Physics2D.OverlapCircleAll(pos, radius)
			where (Object)(object)((Component)x).GetComponent<Item>() != (Object)null || (Object)(object)((Component)x).GetComponent<BuildingEntity>() != (Object)null
			select ((Component)x).gameObject;
		Container container = default(Container);
		foreach (GameObject item in enumerable)
		{
			if (item.TryGetComponent<Container>(ref container))
			{
				LinqUtility.AddRange<GameObject>((ICollection<GameObject>)identifiedBackgroundsInRadius, from x in container.GetAllItems()
					select ((Component)x).gameObject);
			}
		}
		LinqUtility.AddRange<GameObject>((ICollection<GameObject>)identifiedBackgroundsInRadius, enumerable);
		if ((Object)(object)body != (Object)null)
		{
			LinqUtility.AddRange<GameObject>((ICollection<GameObject>)identifiedBackgroundsInRadius, from x in body.GetAllItemsThorough()
				select ((Component)x).gameObject);
		}
		List<NetBody> bodiesInRadius = NetBody.GetBodiesInRadius(pos, radius);
		if ((Object)(object)body != (Object)null && body.TryGetNetBody(out var nb))
		{
			bodiesInRadius.Remove(nb);
		}
		foreach (NetBody item2 in bodiesInRadius)
		{
			foreach (Item item3 in item2.body.GetAllItemsThorough())
			{
				identifiedBackgroundsInRadius.Add(((Component)item3).gameObject);
			}
		}
		identifiedBackgroundsInRadius.RemoveWhere((GameObject x) => (Object)(object)x == (Object)null || ObjectCanBeIgnoredForNetwork(x));
		return identifiedBackgroundsInRadius;
	}

	private static void _ImmidiatelyRegisterCloseObjects(Vector2 pos, Body body)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		HashSet<GameObject> hashSet = GatherClosestObjectsToSync(pos, body, 80f);
		hashSet.RemoveWhere((GameObject x) => IsRegistered(x));
		int num = 0;
		foreach (GameObject item in hashSet)
		{
			NewGO(item);
			if (!KM.dist2dsqrcheck(Vector2.op_Implicit(item.transform.position), in pos, 10f))
			{
				num++;
			}
			if (num >= MAX_REGISTER_BATCH)
			{
				break;
			}
		}
	}

	public static bool IsObjectDynamic(GameObject go)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Invalid comparison between Unknown and I4
		Item val = default(Item);
		if (go.TryGetComponent<Item>(ref val))
		{
			return (Object)(object)go.transform.parent == (Object)null;
		}
		GrabberPlant val2 = default(GrabberPlant);
		if (go.TryGetComponent<GrabberPlant>(ref val2))
		{
			return true;
		}
		Rigidbody2D val3 = default(Rigidbody2D);
		if (!go.TryGetComponent<Rigidbody2D>(ref val3))
		{
			return false;
		}
		return (int)val3.bodyType == 0;
	}

	public static bool IsObjectPhysicsActive(GameObject go)
	{
		Item val = default(Item);
		if (go.TryGetComponent<Item>(ref val) && (Object)(object)go.transform.parent != (Object)null)
		{
			return false;
		}
		Rigidbody2D val2 = default(Rigidbody2D);
		if (!go.TryGetComponent<Rigidbody2D>(ref val2) || val2.IsSleeping())
		{
			return false;
		}
		return true;
	}

	public static void AlertObjectNotRegistered(bool popup = false)
	{
		if (popup)
		{
			PlayerCamera.main.DoAlert(Lang.Get("obj_not_net_registered", false), false);
		}
		string text = "Attempted to interact with unregistered object";
		if (log.verbose)
		{
			text = text + ":\n" + new StackTrace().ToString();
		}
		log.warn(text);
		PlayerCamera.main.PlayUISound((UISoundType)6, 1f);
	}

	internal static void _ObjectSyncUpdateLoopUniversal(bool do_slow_mode)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		if (Net.is_client)
		{
			NetPlayer lOCAL_PLAYER = NetPlayer.LOCAL_PLAYER;
			if ((Object)(object)lOCAL_PLAYER.body != (Object)null)
			{
				if (!lOCAL_PLAYER.body.alive == do_slow_mode)
				{
					_ImmidiatelyRegisterCloseObjects(Vector2.op_Implicit(((Component)PlayerCamera.main).transform.position), lOCAL_PLAYER.body);
				}
				if (lOCAL_PLAYER.body.alive && do_slow_mode)
				{
					_ImmidiatelyRegisterCloseObjects(Vector2.op_Implicit(((Component)PlayerCamera.main).transform.position) - new Vector2(0f, 60f), null);
				}
			}
			return;
		}
		if (do_slow_mode)
		{
			int num = 0;
			KrokoshaScavMultiGameObjectNetworkTracker krokoshaScavMultiGameObjectNetworkTracker = default(KrokoshaScavMultiGameObjectNetworkTracker);
			foreach (KeyValuePair<GameObject, SyncInfo> item in SyncRegistry)
			{
				Server_ObjectUpdateDistanceChecks(item.Value);
				if (!item.Value.tracker.is_far_but_still_around_any_plr && item.Key.TryGetComponent<KrokoshaScavMultiGameObjectNetworkTracker>(ref krokoshaScavMultiGameObjectNetworkTracker))
				{
					Object.Destroy((Object)(object)krokoshaScavMultiGameObjectNetworkTracker);
					num++;
					if (num > MAX_REGISTER_BATCH * 3)
					{
						break;
					}
				}
			}
			if (server_slowmode_type_counter == 0)
			{
				foreach (NetPlayer allDeadPlayer in NetPlayer.AllDeadPlayers)
				{
					_ImmidiatelyRegisterCloseObjects(allDeadPlayer.pos, allDeadPlayer.body);
				}
				server_slowmode_type_counter++;
			}
			else if (server_slowmode_type_counter == 1)
			{
				foreach (NetPlayer allLivingPlayer in NetPlayer.AllLivingPlayers)
				{
					_ImmidiatelyRegisterCloseObjects(allLivingPlayer.pos - new Vector2(0f, 80f), allLivingPlayer.body);
				}
				server_slowmode_type_counter++;
			}
			else
			{
				server_slowmode_type_counter = 0;
			}
			return;
		}
		foreach (NetPlayer allLivingPlayer2 in NetPlayer.AllLivingPlayers)
		{
			_ImmidiatelyRegisterCloseObjects(allLivingPlayer2.pos, allLivingPlayer2.body);
		}
	}

	public static float Client_ObjectUpdateDistanceChecks(SyncInfo si)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)si.go != (Object)null && Util.TryGetLocalBody(out var body))
		{
			float num = KM.dist2dsqr(Vector2.op_Implicit(((Component)body).transform.position), Vector2.op_Implicit(si.go.transform.position));
			si.tracker.is_super_close = num < 144f;
			si.tracker.is_within_anyones_view = num < 4096f;
			si.tracker.is_far_but_still_around_any_plr = num < 57600f;
			return num;
		}
		return 99999f;
	}

	public static float Server_ObjectUpdateDistanceChecks(SyncInfo si)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		(NetPlayer, float) distanceToNearestLivingPlayer = NetPlayer.GetDistanceToNearestLivingPlayer(Vector2.op_Implicit(si.go.transform.position));
		si.tracker.is_super_close = distanceToNearestLivingPlayer.Item2 < 144f;
		si.tracker.is_within_anyones_view = distanceToNearestLivingPlayer.Item2 < 4096f;
		si.tracker.is_far_but_still_around_any_plr = Item_Update_MultiplayerPatch.MPinSimRange(Vector2.op_Implicit(si.go.transform.position));
		return distanceToNearestLivingPlayer.Item2;
	}

	public static bool IsRegistered(Component obj)
	{
		return SyncRegistry.ContainsKey(obj.gameObject);
	}

	public static bool IsRegistered(GameObject obj)
	{
		return SyncRegistry.ContainsKey(obj);
	}

	public static SyncInfo GetSyncInfo(Component obj)
	{
		if ((Object)(object)obj != (Object)null && SyncRegistry.TryGetValue(obj.gameObject, out var value))
		{
			return value;
		}
		return null;
	}

	public static bool TryGetSyncInfo(Component obj, out SyncInfo si)
	{
		if (SyncRegistry.TryGetValue(obj.gameObject, out si))
		{
			return true;
		}
		si = null;
		return false;
	}

	public static bool TryGetSyncInfo(GameObject obj, out SyncInfo si)
	{
		if (SyncRegistry.TryGetValue(obj, out si))
		{
			return true;
		}
		si = null;
		return false;
	}

	public static bool TryGetSyncInfo(knetid syncid, out SyncInfo si)
	{
		if (NetIdToSyncInfoDict.TryGetValue(syncid, out si))
		{
			return true;
		}
		si = null;
		return false;
	}

	public static bool TryGetSyncInfoOrRegister(Component obj, out SyncInfo si)
	{
		if (SyncRegistry.TryGetValue(obj.gameObject, out si))
		{
			return true;
		}
		NewGO(obj.gameObject);
		si = null;
		return false;
	}

	public static bool TryGetSyncInfoOrRegister(GameObject obj, out SyncInfo si)
	{
		if (SyncRegistry.TryGetValue(obj, out si))
		{
			return true;
		}
		NewGO(obj);
		si = null;
		return false;
	}

	public static bool ObjectCanBeIgnoredForNetwork(GameObject obj)
	{
		BuildingEntity component = obj.GetComponent<BuildingEntity>();
		if ((Object)(object)component != (Object)null)
		{
			if (networkignored_buildings_maingame.Contains(component.id))
			{
				return true;
			}
			Climbable val = default(Climbable);
			if (obj.TryGetComponent<Climbable>(ref val))
			{
				Krokosha_BuildingEntity_Rope_TrackerComponent krokosha_BuildingEntity_Rope_TrackerComponent = default(Krokosha_BuildingEntity_Rope_TrackerComponent);
				if (!obj.TryGetComponent<Krokosha_BuildingEntity_Rope_TrackerComponent>(ref krokosha_BuildingEntity_Rope_TrackerComponent) && !obj.AddComponent<Krokosha_BuildingEntity_Rope_TrackerComponent>().is_ladder && log.verbose)
				{
					log.error("Unknown Climbable: " + ((Object)val).name + " -> id: " + component.id);
				}
				return true;
			}
		}
		KrokoshaScavMultiGameObjectNetworkTracker_FORCEIGNORE krokoshaScavMultiGameObjectNetworkTracker_FORCEIGNORE = default(KrokoshaScavMultiGameObjectNetworkTracker_FORCEIGNORE);
		if (obj.TryGetComponent<KrokoshaScavMultiGameObjectNetworkTracker_FORCEIGNORE>(ref krokoshaScavMultiGameObjectNetworkTracker_FORCEIGNORE))
		{
			return true;
		}
		return false;
	}

	public static SyncInfo Server_EnsureItemIsNetworkRegistered(GameObject go)
	{
		try
		{
			if (SyncRegistry.TryGetValue(go, out var value))
			{
				return value;
			}
			if (NewCoolerObjectPacketWriteReadSystem.inst == null)
			{
				return null;
			}
			return NewCoolerObjectPacketWriteReadSystem.inst.Server_RegisterObject(go);
		}
		catch (Exception ex)
		{
			Plugin.Logger.LogError((object)$"Server_EnsureItemIsNetworkRegistered: {go} -> {NewCoolerObjectPacketWriteReadSystem.inst}\n{ex.ToString()}\n{new StackTrace()}");
			return null;
		}
	}

	public static void Client_DeleteUnregisteredObject(GameObject obj)
	{
		if (!ObjectCanBeIgnoredForNetwork(obj))
		{
			SafeDestroyObject(obj);
		}
	}

	public static void Server_QueueSync(SyncInfo si)
	{
		si.last_update_time = Time.realtimeSinceStartupAsDouble;
		NewCoolerObjectPacketWriteReadSystem.inst.Server_QueueForceSyncForAll(si.syncId);
	}

	public static void Server_QueueSyncForOne(SyncInfo si, NetPlayer plr)
	{
		NewCoolerObjectPacketWriteReadSystem.inst.Server_QueueForceSync(plr.clientId, si.syncId);
	}

	public static void Server_ObjectSyncSingle(GameObject obj)
	{
		if (!ObjectCanBeIgnoredForNetwork(obj))
		{
			SyncInfo syncInfo = Server_EnsureItemIsNetworkRegistered(obj);
			syncInfo.last_update_time = Time.realtimeSinceStartupAsDouble;
			NewCoolerObjectPacketWriteReadSystem.inst.Server_QueueForceSyncForAll(syncInfo.syncId);
		}
	}

	public static void Server_ObjectSyncSingleToOnePerson(GameObject obj, knetid target_plrId)
	{
		if (!ObjectCanBeIgnoredForNetwork(obj))
		{
			SyncInfo syncInfo = Server_EnsureItemIsNetworkRegistered(obj);
			syncInfo.last_update_time = Time.realtimeSinceStartupAsDouble;
			NewCoolerObjectPacketWriteReadSystem.inst.Server_QueueForceSync(target_plrId, syncInfo.syncId);
		}
	}

	public static void Server_ObjectSyncDouble(GameObject obj, GameObject obj2)
	{
		if (!ObjectCanBeIgnoredForNetwork(obj) && !ObjectCanBeIgnoredForNetwork(obj2))
		{
			SyncInfo syncInfo = Server_EnsureItemIsNetworkRegistered(obj);
			SyncInfo syncInfo2 = Server_EnsureItemIsNetworkRegistered(obj2);
			NewCoolerObjectPacketWriteReadSystem.inst.Server_QueueForceSyncForAll(syncInfo.syncId);
			NewCoolerObjectPacketWriteReadSystem.inst.Server_QueueForceSyncForAll(syncInfo2.syncId);
		}
	}

	public static void SafeDestroyObject(GameObject go)
	{
		Container val = default(Container);
		if (go.TryGetComponent<Container>(ref val))
		{
			val.UnloadAllItems();
		}
		if (SyncRegistry.TryGetValue(go, out var value))
		{
			SyncRegistry.Remove(value.go);
			NewCoolerObjectPacketWriteReadSystem.inst.SimplyUnregisterObject(value.syncId);
		}
		Object.Destroy((Object)(object)go.gameObject);
	}

	private void Update()
	{
		if (Net.running && Util.IsWorldGenerated())
		{
			timer_fastsync += ClientMain.AdaptiveSyncTimerDelta;
			timer_objsync += ClientMain.AdaptiveSyncTimerDelta;
			timer_raresync += ClientMain.AdaptiveSyncTimerDelta;
			if (timer_fastsync > 0.043f)
			{
				timer_fastsync = 0f;
			}
			if (timer_objsync > 0.211f)
			{
				_ObjectSyncUpdateLoopUniversal(do_slow_mode: false);
				timer_objsync = 0f;
			}
			if (timer_raresync > 1.894512f)
			{
				_ObjectSyncUpdateLoopUniversal(do_slow_mode: true);
				timer_raresync = 0f;
			}
		}
	}

	private void OnGUI()
	{
		//IL_0521: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_043d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0467: Unknown result type (might be due to invalid IL or missing references)
		//IL_046e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0482: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		int fontSize = GUI.skin.label.fontSize;
		if (KrokoshaScavMultiplayer.network_system_is_running && _DEV_ENABLE_SYNCINFO_SNITCHING)
		{
			foreach (KeyValuePair<GameObject, SyncInfo> item in SyncRegistry)
			{
				GameObject key = item.Key;
				SyncInfo value = item.Value;
				try
				{
					Vector2 val = Vector2.op_Implicit(Camera.main.WorldToScreenPoint(key.transform.position));
					((Vector2)(ref val))._002Ector(val.x, (float)Screen.height - val.y);
					if (!(val.x > 0f) || !(val.y > 0f) || !(val.x < (float)Screen.width) || !(val.y < (float)Screen.width))
					{
						continue;
					}
					float num = 0f;
					GUI.skin.label.normal.textColor = Color.white;
					GUI.skin.label.fontSize = 10;
					Rect val2 = new Rect(val.x, val.y + num, 400f, 17f);
					knetid syncId = value.syncId;
					GUI.Label(val2, "syncid: " + syncId.ToString());
					num += 13f;
					GUI.Label(new Rect(val.x, val.y + num, 400f, 17f), "combat_relax: " + value.relaxed_combat_sync);
					num += 13f;
					GUI.Label(new Rect(val.x, val.y + num, 400f, 17f), "last_update_time: " + Math.Round(value.last_update_time, 1));
					num += 13f;
					GUI.Label(new Rect(val.x, val.y + num, 400f, 17f), "pos: " + ((object)Vector2.op_Implicit(key.transform.position)/*cast due to constrained. prefix*/).ToString());
					num += 13f;
					int num2 = 0;
					bool flag = false;
					if (value.IsItem())
					{
						num2 = ItemSync.cur_items_check.IndexOf(value.item);
						flag = num2 >= ItemSync.cur_items_check_index || num2 < ItemSync.cur_items_check_index + 30;
						GUI.Label(new Rect(val.x, val.y + num, 400f, 17f), "is contained: " + ItemSync.ItemIsInContainer(value.item));
						num += 13f;
					}
					else if (value.IsBuilding())
					{
						num2 = ScavMultiBuildingSynchronizer.cur_buildings_check.IndexOf(value.building);
						flag = num2 >= ScavMultiBuildingSynchronizer.cur_buildings_check_index || num2 < ItemSync.cur_items_check_index + ScavMultiBuildingSynchronizer.cur_buildings_check_index;
						GUI.Label(new Rect(val.x, val.y + num, 400f, 17f), "physics: " + ScavMultiBuildingSynchronizer.BuildingHasActivePhysics(value.building));
						num += 13f;
						if (value.IsTrader())
						{
							KrokoshaTraderTrackerComponent component = key.GetComponent<KrokoshaTraderTrackerComponent>();
							if ((Object)(object)component.focused_body != (Object)null)
							{
								GUI.Label(new Rect(val.x, val.y + num, 400f, 17f), "trader_body: " + (object)((Component)component.focused_body).GetComponent<NetBody>().plr);
								num += 13f;
							}
						}
					}
					GUI.Label(new Rect(val.x, val.y + num, 400f, 17f), "system index: " + num2 + (flag ? " NOW" : ""));
					num += 13f;
					KrokoshaScavMultiGameObjectNetworkTracker tracker = value.tracker;
					GUI.Label(new Rect(val.x, val.y + num, 400f, 17f), "see12: " + tracker.is_super_close);
					num += 13f;
					GUI.Label(new Rect(val.x, val.y + num, 400f, 17f), "see64: " + tracker.is_within_anyones_view);
					num += 13f;
					GUI.Label(new Rect(val.x, val.y + num, 400f, 17f), "see200: " + tracker.is_far_but_still_around_any_plr);
					num += 13f;
				}
				catch
				{
				}
			}
		}
		GUI.skin.label.normal.textColor = Color.white;
		GUI.skin.label.fontSize = fontSize;
	}
}
