using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KrokoshaCasualtiesUtils;
using LiteNetLib.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KrokoshaCasualtiesMP;

public class WorldgenPatches : KrokoshaScavSingleton
{
	public struct RunPrefs : INetSerializeByMemcpy
	{
		public bool tutorial;

		public bool unchipped;

		public RunPrefs ReadPrefs()
		{
			tutorial = PlayerPrefsExtended.GetBool("tutorial");
			if (runsettings != null)
			{
				unchipped = (bool)runsettings["unchipped"];
			}
			return this;
		}

		public void ApplyPrefs(bool abide_rules = true, bool apply_tutorial = true)
		{
			if (apply_tutorial)
			{
				PlayerPrefsExtended.SetBool("tutorial", tutorial);
			}
			if (abide_rules && !rules.UnchippedIsIndividual && runsettings != null)
			{
				runsettings["unchipped"] = unchipped;
			}
		}
	}

	private const byte serializer_version = 1;

	private static bool mainmenu_is_screen_darkening;

	internal static byte[] aaaaaaaaaaaaaaaaaaaaaaaaaaa;

	internal static byte[] bbbbbbbbbbbbbbbbbbbbbbbbbbb;

	internal static byte[] ccccccccccccccccccccccccccc;

	internal static Sprite white_square;

	public static bool earthquake_enabled;

	public static bool allow_continue_to_next_layers;

	private static GameObject SKY_BACKGROUND;

	public static bool client_technically_finished_worldgen_now_just_waiting_for_server;

	public static Dictionary<string, object> runsettings
	{
		get
		{
			Dictionary<string, object> dictionary = null;
			if ((Object)(object)PreRunScript.instance != (Object)null)
			{
				dictionary = PreRunScript.instance.runSettings;
			}
			if (Util.IsInMainMenu())
			{
				return dictionary;
			}
			return WorldGeneration.runSettings ?? dictionary ?? GetNormalRunSettings();
		}
	}

	public static WorldGeneration world => WorldGeneration.world;

	public static ushort[,] worldBlocks => WorldGeneration.world.worldBlocks;

	public static byte[,] fluid => FluidManager.main.fluid;

	public static KrokoshaMultiplayerGameRules rules => KrokoshaScavMultiplayer.rules;

	public static bool verbose => KrokoshaScavMultiplayer.verbose;

	public static event Action OnWorldgenFinish;

	public static Dictionary<string, object> GetNormalRunSettings()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		foreach (KeyValuePair<string, object> presetValue in RunSettings.GetPreset("normal").presetValues)
		{
			dictionary.Add(presetValue.Key, presetValue.Value);
		}
		return dictionary;
	}

	public static string CompileRunSettings()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		string s = JsonConvert.SerializeObject((object)SaveSystem.DicToTupleList(runsettings), (Formatting)0, new JsonSerializerSettings
		{
			ReferenceLoopHandling = (ReferenceLoopHandling)1,
			Formatting = (Formatting)0
		});
		return Convert.ToBase64String(Util.Compress(Encoding.UTF8.GetBytes(s)));
	}

	public static void ReadRunSettings(string str)
	{
		Dictionary<string, object> normalRunSettings = GetNormalRunSettings();
		try
		{
			foreach (KeyValuePair<string, object> item in SaveSystem.TupleListToDic(((JToken)JArray.Parse(Encoding.UTF8.GetString(Util.Decompress(Convert.FromBase64String(str))))).ToObject<List<(string, string)>>()))
			{
				normalRunSettings[item.Key] = item.Value;
			}
		}
		catch (Exception ex)
		{
			log.error("ReadRunSettings: " + ex.ToString());
		}
		Dictionary<string, object> dictionary = runsettings;
		if ((Object)(object)PreRunScript.instance != (Object)null)
		{
			PreRunScript.instance.runSettings = normalRunSettings;
			PreRunScript.instance.currentPreset = 6;
			PreRunScript.instance.UpdateAllSettingDisplays();
		}
		WorldGeneration.runSettings = normalRunSettings;
		if (dictionary != null && rules.UnchippedIsIndividual)
		{
			runsettings["unchipped"] = dictionary.GetValueSafe("unchipped", false);
		}
	}

	private void Awake()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Expected O, but got Unknown
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		ResetWorldParameters();
		Color white = Color.white;
		Texture2D val = new Texture2D(2, 2, (TextureFormat)4, false);
		val.SetPixel(0, 0, white);
		val.SetPixel(1, 0, white);
		val.SetPixel(1, 1, white);
		val.SetPixel(0, 1, white);
		((Texture)val).filterMode = (FilterMode)0;
		white_square = Sprite.Create(val, new Rect(0f, 0f, (float)((Texture)val).width, (float)((Texture)val).height), new Vector2(0.5f, 0.5f));
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void Start()
	{
	}

	private void OnSceneLoaded(Scene a, LoadSceneMode b)
	{
		aaaaaaaaaaaaaaaaaaaaaaaaaaa = null;
		bbbbbbbbbbbbbbbbbbbbbbbbbbb = null;
		ccccccccccccccccccccccccccc = null;
	}

	private void FixedUpdate()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Invalid comparison between Unknown and I4
		if (Util.IsInWorld())
		{
			if (mainmenu_is_screen_darkening)
			{
				mainmenu_is_screen_darkening = false;
			}
			if ((!earthquake_enabled && world.earthquakeTime <= 0f) || (int)world.biomeOverride == 2)
			{
				world.earthquakeTime = 0f;
				world.earthquakeDelay = 1000f;
				world.earthquakeIntensity = 0f;
			}
		}
	}

	private void Update()
	{
		if (Net.running)
		{
			_ = SharedMain.local_world_is_generated;
		}
	}

	public static LayerModifier GetLayerModifier()
	{
		LayerModifier[] availableModifiers = LayerModifier.availableModifiers;
		foreach (LayerModifier val in availableModifiers)
		{
			if (val.active)
			{
				return val;
			}
		}
		return null;
	}

	public static void ResetWorldParameters()
	{
		earthquake_enabled = true;
		allow_continue_to_next_layers = true;
	}

	public static void ResetWorldParametersPeaceful()
	{
		ResetWorldParameters();
		earthquake_enabled = false;
		SetRadlinePlayerPrefs(enabled: false);
	}

	public static void SetRadlinePlayerPrefs(bool enabled)
	{
		PlayerPrefs.SetInt("radlinedisable", (!enabled) ? 1 : 0);
	}

	public static void SetTutorialPlayerPrefs(bool enabled)
	{
		PlayerPrefs.SetInt("tutorial", enabled ? 1 : 0);
	}

	public static void LoadDebugWorld()
	{
		_CheckIfCanLoadAWorld();
		SetTutorialPlayerPrefs(enabled: false);
		if (KrokoshaScavMultiplayer.IsNetworkActiveAndIsServer())
		{
			ServerMain.Server_Announce_GAME_START();
		}
		SceneManager.LoadScene("SampleScene");
		Util.CallLambdaWhen(() => (Object)(object)WorldGeneration.world != (Object)null, (Action)delegate
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			WorldGeneration.world.biomeOverride = (OverrideSceneType)2;
		});
	}

	public static void LoadVanillaGeneratedWorld(bool loadsave = false)
	{
		_CheckIfCanLoadAWorld();
		ResetWorldParameters();
		SetRadlinePlayerPrefs(enabled: true);
		LoadWorldScene(loadsave);
	}

	public static void LoadVanillaTutorialWorld(bool fadeout = true)
	{
		_CheckIfCanLoadAWorld();
		ResetWorldParameters();
		SetTutorialPlayerPrefs(enabled: true);
		SetRadlinePlayerPrefs(enabled: false);
		LoadWorldScene(loadsave: false, fadeout);
	}

	public static void CreateSkyBackground()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		CreateSkyBackground(world.skyColors.Evaluate(Random.value), (Random.Range(0f, 1f) < 0.3f) ? 1f : 0f);
	}

	public static void CreateSkyBackground(Color skyColor, float rainIntensity)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		SKY_BACKGROUND = Utils.Create("Special/wallholes", Vector2.zero, 0f);
		SpriteRenderer component = SKY_BACKGROUND.GetComponent<SpriteRenderer>();
		component.sprite = white_square;
		Transform transform = ((Component)component).gameObject.transform;
		transform.localScale *= 99999f;
		world.skyMaterial.SetColor("_TopColor", skyColor);
		world.skyMaterial.SetFloat("_RainIntensity", rainIntensity);
		AudioSource component2 = SKY_BACKGROUND.GetComponent<AudioSource>();
		component2.maxDistance = 100000f;
		component2.spatialBlend = 0.001f;
		component2.volume *= 0.7f;
		((Object)SKY_BACKGROUND).name = "KSMULTI_SURFACE_BACKGROUND";
		if (KrokoshaScavMultiplayer.IsNetworkActiveAndIsServer())
		{
			ServerMain.Server_SendWorldState();
		}
	}

	public static void RemoveSkyBackground()
	{
		if ((Object)(object)SKY_BACKGROUND != (Object)null)
		{
			Object.Destroy((Object)(object)SKY_BACKGROUND);
			if (KrokoshaScavMultiplayer.IsNetworkActiveAndIsServer())
			{
				ServerMain.Server_SendWorldState();
			}
		}
	}

	public static bool HasSkyBackground(out (Color, float) skyinfo)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)SKY_BACKGROUND != (Object)null)
		{
			skyinfo = (world.skyMaterial.GetColor("_TopColor"), world.skyMaterial.GetFloat("_RainIntensity"));
			return true;
		}
		skyinfo = (default(Color), 0f);
		return false;
	}

	public static bool HasSkyBackground()
	{
		return (Object)(object)SKY_BACKGROUND != (Object)null;
	}

	public static void LoadWorldScene(bool loadsave = false, bool fadeout = true)
	{
		if ((Object)(object)world != (Object)null)
		{
			throw new Exception("Ur aready in the world cuh");
		}
		mainmenu_is_screen_darkening = true;
		SaveSystem.loadedRun = loadsave;
		if (fadeout)
		{
			PreRunScript val = Object.FindObjectOfType<PreRunScript>();
			if (!Object.op_Implicit((Object)(object)val))
			{
				throw new Exception("PreRunScript DOES NOT EXIST ?! WHAT ?????");
			}
			mainmenu_is_screen_darkening = true;
			((MonoBehaviour)val).StartCoroutine(val.WaitLoad());
		}
		else
		{
			SceneManager.LoadScene("SampleScene");
		}
	}

	private static void _CheckIfCanLoadAWorld()
	{
		if (Util.IsInWorld())
		{
			throw new Exception("World is already loaded.");
		}
		if (mainmenu_is_screen_darkening)
		{
			throw new Exception("World is already loading.");
		}
	}

	internal static IEnumerator Patched_GenerateWorld(WorldGeneration world)
	{
		world.loadingObject.SetActive(true);
		world.generatingWorld = true;
		SharedMain.local_world_is_generating = true;
		WorldGeneration_ApplyLayerModifiers_MultiplayerPatch.sent_to_clients = false;
		ClientMain._last_reminderpack_while_generating_received = false;
		client_technically_finished_worldgen_now_just_waiting_for_server = false;
		if (KrokoshaScavMultiplayer.network_system_is_running)
		{
			Body_PlaceBody_MultiplayerPatch.has_spawn_location = false;
			try
			{
				WorldGeneration_GenerateWorld_MultiplayerPatch.InitializeSpecialResourcePrefabs();
			}
			catch (Exception ex)
			{
				log.error("InitializeSpecialResourcePrefabs\n" + ex.ToString());
			}
			SharedMain.LastWorldgenFinishTime = Time.realtimeSinceStartupAsDouble;
			if (KrokoshaScavMultiplayer.is_client)
			{
				ClientMain.server_is_generating_world = true;
				((TMP_Text)world.loadingText).text = Lang.Get("worldgen_waitforserver_seed", false);
				log.l("CLIENT: Waiting for worldgen params...");
				while (WorldGeneration_GenerateWorld_MultiplayerPatch.client_firstworldgenparams_are_used)
				{
					ClientMain.server_is_generating_world = true;
					if (!KrokoshaScavMultiplayer.network_system_is_running)
					{
						yield break;
					}
					SharedMain.LastWorldgenFinishTime = Time.realtimeSinceStartupAsDouble;
					yield return null;
				}
				log.l("CLIENT: Received the worldgen params, starting world generation!");
				WorldGeneration_GenerateWorld_MultiplayerPatch.firstworldgenparams.Apply();
				WorldGeneration_GenerateWorld_MultiplayerPatch.client_firstworldgenparams_are_used = true;
			}
			else
			{
				WorldGeneration_GenerateWorld_MultiplayerPatch.firstworldgenparams = new LastBeforeGenerationState();
				ServerMain.Server_AnnounceSeed(ServerMain.AllClientIdsExceptHost);
				if (KrokoshaScavMultiplayer.rules.LayerFinishKeepXOffset)
				{
					float num = 0f;
					float num2 = 0f;
					bool flag = false;
					foreach (KeyValuePair<Body, NetPlayer> item in NetPlayer.BodyToPlayerDict)
					{
						Body key = item.Key;
						NetPlayer value = item.Value;
						if ((Object)(object)key != (Object)null && key.alive)
						{
							float num3 = Mathf.Clamp(((Component)key).transform.position.x, (float)(0L - (long)world.width) * 0.49f, (float)world.width * 0.49f);
							value.server_plrstate.layer_transition_x_offset = num3;
							if (!flag)
							{
								flag = true;
								num = num3;
								num2 = num3;
							}
							else
							{
								num = Mathf.Min(num, num3);
								num2 = Mathf.Max(num2, num3);
							}
						}
						else
						{
							value.server_plrstate.layer_transition_x_offset = 0f;
						}
					}
					float num4 = Mathf.Lerp(num, num2, 0.5f);
					foreach (KeyValuePair<Body, NetPlayer> item2 in NetPlayer.BodyToPlayerDict)
					{
						Body key2 = item2.Key;
						NetPlayer value2 = item2.Value;
						if ((Object)(object)key2 != (Object)null && key2.alive)
						{
							value2.server_plrstate.layer_transition_x_offset = Mathf.Clamp(value2.server_plrstate.layer_transition_x_offset - num4, (float)(0L - (long)world.width) * 0.49f, (float)world.width * 0.49f);
							((Component)key2).transform.position = Vector2.op_Implicit(new Vector2(value2.server_plrstate.layer_transition_x_offset, ((Component)key2).transform.position.y));
						}
					}
				}
			}
			foreach (NetPlayer value3 in NetPlayer.BodyToPlayerDict.Values)
			{
				value3.pos = Vector2.zero;
				if (!value3.is_local && (Object)(object)value3.body != (Object)null)
				{
					value3.playerbody.SetNetIgnoreTime(1f);
				}
				if (value3.server_plrstate != null)
				{
					value3.server_plrstate.OnWorldgenStart();
				}
			}
		}
		KrokoshaScavMultiplayer._InvokeOnSceneChangeOrWorldStartGenerate();
		log.l("WorldGeneration.GenerateWorld");
		yield return WorldGeneration_GenerateWorld_MultiplayerPatch.GenerateWorld(world);
	}

	internal static IEnumerator Patched_WorldPlacePlayer()
	{
		SharedMain.LastWorldgenFinishTime = Time.realtimeSinceStartupAsDouble;
		ServerMain._ded_server_switch_counter = -20f;
		if (KrokoshaScavMultiplayer.network_system_is_running && KrokoshaScavMultiplayer.is_client)
		{
			ClientMain.server_is_generating_world = true;
			ClientMain._last_reminderpack_while_generating_received = false;
			KrokoshaScavMultiplayer.Client_SendSimpleMessageToServer((ushort)10167, WorldGeneration.unchipped, true);
			log.l("CLIENT: Waiting at WorldPlacePlayer, for server to send me a spawn location.");
			((TMP_Text)world.loadingText).text = Lang.Get("worldgen_waitforserver_spawn", false);
			while (KrokoshaScavMultiplayer.IsNetworkActiveAndIsClient() && !ClientMain._last_reminderpack_while_generating_received)
			{
				yield return (object)new WaitForSecondsRealtime(0.25f);
				if (!KrokoshaScavMultiplayer.IsNetworkActiveAndIsClient())
				{
					yield break;
				}
				yield return (object)new WaitForSecondsRealtime(0.25f);
			}
			if (ClientMain._last_reminderpack_while_generating_received && NetPlayer.TryGetLocalNetBody(out var nb))
			{
				nb.OnReceiveSyncPacket(in ClientMain._last_reminderpack_while_generating, force: true);
			}
			log.l("CLIENT: Received location for WorldPlacePlayer.");
		}
		yield return WorldGeneration_WorldPlacePlayer_MultiplayerPatch.WorldPlacePlayer(world);
		log.l("WorldGeneration.WorldPlacePlayer");
	}

	private static IEnumerator FinishWorldGen_ClientClearWorldObjects()
	{
		IEnumerable<GameObject> first = from x in Object.FindObjectsOfType<Item>()
			select ((Component)x).gameObject;
		IEnumerable<GameObject> second = from x in Object.FindObjectsOfType<BuildingEntity>()
			select ((Component)x).gameObject;
		List<GameObject> list = first.Union(second).ToList();
		((TMP_Text)world.loadingText).text = Lang.Get("worldgen_waitforserver_delete", false);
		_ = Time.realtimeSinceStartupAsDouble;
		for (int num = 0; num < list.Count; num++)
		{
			GameObject val = list[num];
			if (!((Object)(object)val == (Object)null) && !NetObjectRegistry.ObjectCanBeIgnoredForNetwork(val) && !NetObjectRegistry.IsRegistered(val))
			{
				Object.Destroy((Object)(object)val);
			}
		}
		yield break;
	}

	internal static IEnumerator Patched_FinishWorldGeneration()
	{
		SharedMain.LastWorldgenFinishTime = Time.realtimeSinceStartupAsDouble;
		ServerMain._ded_server_switch_counter = -20f;
		if (KrokoshaScavMultiplayer.network_system_is_running)
		{
			foreach (KeyValuePair<Body, NetPlayer> item in NetPlayer.BodyToPlayerDict)
			{
				item.Value.ResetEntropy();
				item.Value.levelPlayTime = 0.0;
			}
			if (world.doPod)
			{
				foreach (KeyValuePair<Body, NetPlayer> item2 in NetPlayer.BodyToPlayerDict)
				{
					Body key = item2.Key;
					if (!item2.Value.is_local)
					{
						key.hearingLoss += 15f;
						key.hunger -= 10f;
						key.thirst -= 15f;
					}
				}
			}
			WorldChunkSync.server_chunksync_queue.Clear();
			if (KrokoshaScavMultiplayer.is_client)
			{
				WorldChunkSync.singleton.timer_TilemapSync = 10f;
				WorldChunkSync.singleton.timer_TilemapFluidSync = 10f;
				ClientMain.server_is_generating_world = true;
				client_technically_finished_worldgen_now_just_waiting_for_server = true;
				yield return FinishWorldGen_ClientClearWorldObjects();
				ClientMain.server_is_generating_world = true;
				KrokoshaScavMultiplayer.Client_SendSimpleMessageToServer((ushort)10168, WorldGeneration.unchipped, true);
				log.l("CLIENT: Finished worldgen, sending message to server.");
				((TMP_Text)world.loadingText).text = Lang.Get("worldgen_waitforserver", false);
				while (KrokoshaScavMultiplayer.IsNetworkActiveAndIsClient() && (ClientMain.server_is_generating_world || !ClientMain._last_reminderpack_while_generating_received))
				{
					WorldChunkSync.TilemapSyncUpdate();
					yield return (object)new WaitForSecondsRealtime(0.01f);
					NetObjectRegistry._ObjectSyncUpdateLoopUniversal(do_slow_mode: false);
					yield return (object)new WaitForSecondsRealtime(0.3233f);
					NetObjectRegistry._ObjectSyncUpdateLoopUniversal(do_slow_mode: false);
					yield return (object)new WaitForSecondsRealtime(0.3334f);
					NetObjectRegistry._ObjectSyncUpdateLoopUniversal(do_slow_mode: false);
					yield return (object)new WaitForSecondsRealtime(0.3334f);
				}
				yield return (object)new WaitForSecondsRealtime(1f);
				if (ClientMain._last_reminderpack_while_generating_received && NetPlayer.TryGetLocalNetBody(out var nb))
				{
					nb.OnReceiveSyncPacket(in ClientMain._last_reminderpack_while_generating, force: true);
				}
			}
		}
		if (Net.running)
		{
			if (KrokoshaScavMultiplayer.is_server)
			{
				foreach (NetPlayer value in NetPlayer.ClientIdToPlayerDict.Values)
				{
					value.server_plrstate.OnWorldGenerate();
				}
				WorldChunkSync.FluidTilemapSyncUpdate();
			}
			WorldChunkSync.TilemapSyncUpdate();
		}
		if (Util.TryGetLocalBody(out var body) && !body.alive)
		{
			UIInGame.StartSpectatorMode();
		}
		yield return WorldGeneration_FinishWorldGeneration_MultiplayerPatch.FinishWorldGeneration(world);
		log.l("WorldGeneration.FinishWorldGeneration");
		if (Util.TryGetLocalBody(out body) && !body.alive)
		{
			UIInGame.StartSpectatorMode();
		}
		aaaaaaaaaaaaaaaaaaaaaaaaaaa = null;
		bbbbbbbbbbbbbbbbbbbbbbbbbbb = null;
		ccccccccccccccccccccccccccc = null;
		SharedMain.LastWorldgenFinishTime = Time.realtimeSinceStartupAsDouble;
		ServerMain._ded_server_switch_counter = -20f;
		if (KrokoshaScavMultiplayer.IsNetworkActiveAndIsClient() && ClientMain._last_reminderpack_while_generating_received)
		{
			if (NetPlayer.TryGetLocalNetBody(out var nb2))
			{
				log.l($"CLIENT: Received starter char state packet, that im applying just now. POS: {ClientMain._last_reminderpack_while_generating.pos}");
				nb2.OnReceiveSyncPacket(in ClientMain._last_reminderpack_while_generating, force: true);
			}
			else
			{
				log.error($"CLIENT: Received starter char state packet, BUT I HAVE NO BODY !!!! POS: {ClientMain._last_reminderpack_while_generating.pos}");
			}
		}
		if (KrokoshaScavMultiplayer.network_system_is_running)
		{
			KrokoshaScavMultiplayer.ApplyGameRules();
			ServerMain.ResetPlayersFinishedStatus();
			WorldGeneration_GenerateWorld_MultiplayerPatch.client_firstworldgenparams_are_used = true;
			DrillPod_Update_MultiplayerPatch.didTeleport = false;
			if ((Object)(object)PlayerCamera.main != (Object)null)
			{
				PlayerCamera.main.bonusActionBarTime = 0f;
			}
			ServerMain._DEV_ENABLE_HP_SYNC = true;
			WorldChunkSync._TILE_SYNC_ENABLED = true;
			WorldChunkSync._FLUID_SYNC_ENABLED = true;
			TutorialHandler_Update_MarkiplierPatch.did_multiplayer_warning = false;
			TutorialHandler_Update_MarkiplierPatch.everyone_finish_time = 0.0;
			foreach (KeyValuePair<Body, NetPlayer> item3 in NetPlayer.BodyToPlayerDict)
			{
				item3.Value.ResetEntropy();
			}
			if (!KrokoshaScavMultiplayer.is_client)
			{
				if (KrokoshaScavMultiplayer.rules.CanReviveOnNextLevel())
				{
					ServerMain.server_lastplayerstates?.Clear();
					bool flag = false;
					foreach (KeyValuePair<Body, NetPlayer> item4 in NetPlayer.BodyToPlayerDict)
					{
						if (!item4.Key.alive)
						{
							flag = true;
							Plugin.log.LogInfo((object)("Respawed " + item4.Value.playername + " "));
							item4.Value.Server_RespawnCharacter(Vector2.op_Implicit(((Component)PlayerCamera.main.body).transform.position), level_transition: true);
						}
					}
					if (flag)
					{
						string key2 = "respawned_dead_plrs";
						Plugin.log.LogInfo((object)Lang.GetEN(key2));
						((MonoBehaviour)PlayerCamera.main).StartCoroutine(PlayerCamera.main.DoAlertDelayed(Lang.Get(in key2, false), false, 10f));
					}
				}
				foreach (NetBody all_instance in NetBody.all_instances)
				{
					foreach (Item item5 in all_instance.body.GetAllItemsThorough())
					{
						NetObjectRegistry.NewGO(((Component)item5).gameObject);
					}
				}
			}
			else
			{
				Con._DEV_CHATSPY = false;
			}
			if ((Object)(object)NetPlayer.LOCAL_PLAYER?.body != (Object)null)
			{
				NetPlayer.LOCAL_PLAYER.playerbody.unchipped = WorldGeneration.unchipped;
			}
			try
			{
				SharedMain.CreatePlayerCharacters();
			}
			catch (Exception arg)
			{
				Plugin.log.LogFatal((object)$"FAILED CREATING CHARACTERS WHAT  {arg} ");
			}
		}
		else
		{
			Application.runInBackground = false;
		}
		try
		{
			WorldgenPatches.OnWorldgenFinish?.Invoke();
		}
		catch (Exception ex)
		{
			log.error("OnWorldgenFinish.Invoke()\n" + ex.ToString());
		}
		SharedMain.local_world_is_generating = false;
	}

	[ServerReceiver(10167)]
	private static void ServerReceiver_WorldPlacePlayer(knetid clientId, ref NetDataReader reader)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (!Util.IsInWorld())
		{
			return;
		}
		bool unchipped = default(bool);
		reader.Get(ref unchipped);
		if (NetPlayer.TryGetPlayerFromClientId(clientId, out var plr) && !plr.server_plrstate.did_give_spawn_location)
		{
			log.devevent($"SERVER: Received WorldPlacePlayer from {plr}", plr.pos, Color.red, ignoreverbose: true);
			plr.ResetEntropy();
			plr.unchipped = unchipped;
			if ((Object)(object)plr.body != (Object)null)
			{
				plr.playerbody.unchipped = unchipped;
			}
			plr.server_plrstate.did_give_spawn_location = true;
			((MonoBehaviour)plr).StartCoroutine(ServerMain.HeyPlayerJustJoinedGiveHimASpawnLocationOkay(clientId));
			WorldChunkSync.singleton.timer_TilemapSync += 0.05f;
			WorldChunkSync.singleton.timer_TilemapFluidSync += 0.05f;
		}
	}

	[ServerReceiver(10168)]
	private static void ServerReceiver_FinishedWorldgen(knetid clientId, ref NetDataReader reader)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		if (!Util.IsInWorld())
		{
			return;
		}
		bool unchipped = default(bool);
		reader.Get(ref unchipped);
		if (!NetPlayer.TryGetPlayerFromClientId(clientId, out var plr) || plr.server_plrstate.finished_worldgen)
		{
			return;
		}
		log.devevent($"SERVER: Received FinishedWorldgen from {plr}", plr.pos, Color.red, ignoreverbose: true);
		plr.ResetEntropy();
		plr.unchipped = unchipped;
		if ((Object)(object)plr.body != (Object)null)
		{
			plr.playerbody.unchipped = unchipped;
		}
		plr.server_plrstate.finished_worldgen = true;
		if (!plr.server_plrstate.did_give_spawn_location)
		{
			plr.server_plrstate.did_give_spawn_location = true;
			((MonoBehaviour)plr).StartCoroutine(ServerMain.HeyPlayerJustJoinedGiveHimASpawnLocationOkay(clientId));
		}
		WorldChunkSync.singleton.timer_TilemapSync = 10f;
		WorldChunkSync.singleton.timer_TilemapFluidSync = 10f;
		if (plr.late_joined)
		{
			LayerModifier[] availableModifiers = LayerModifier.availableModifiers;
			foreach (LayerModifier val in availableModifiers)
			{
				if (val.active)
				{
					KrokoshaScavMultiplayer.Server_SendSimpleMessageToOneClient((ushort)10015, (ushort)plr.clientId, (ushort)(knetid)(ushort)val.modifierIndex, true);
				}
			}
		}
		foreach (GameObject item in NetObjectRegistry.GatherClosestObjectsToSync(plr.pos, plr.body, 64f))
		{
			NetObjectRegistry.Server_ObjectSyncSingle(item);
		}
	}
}
