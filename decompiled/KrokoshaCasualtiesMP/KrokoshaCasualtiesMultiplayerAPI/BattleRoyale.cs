using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KrokoshaCasualtiesMP;
using KrokoshaCasualtiesUtils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KrokoshaCasualtiesMultiplayerAPI;

public class BattleRoyale : GamemodeBase
{
	public static int happening_layer;

	private bool GAME_ON;

	public static WorldGeneration world => WorldGeneration.world;

	public static uint width => world.width;

	public static uint height => world.height;

	public static uint chunkWidth => world.chunkWidth;

	public static uint chunkHeight => world.chunkHeight;

	public static uint halfWidth => world.halfWidth;

	public static uint halfHeight => world.halfHeight;

	public override void Init(string[] args)
	{
		int num = 0;
		if (args.Length != 0)
		{
			num = (int)Con.con.ParseFloat(args[0]);
		}
		log.l($"Battleroyale Layer: {num}");
		happening_layer = num;
	}

	private void Awake()
	{
		SetMainMenuRules();
		KrokoshaScavMultiplayer.rules.AutoContinue = true;
		KrokoshaScavMultiplayer.rules.SelfharmWitnessMoodDebuff = -10f;
		KrokoshaScavMultiplayer.rules.PVPMoodDebuff = -8f;
		KrokoshaScavMultiplayer.rules.LayerFinishPlrPercent = 101;
		KrokoshaScavMultiplayer.rules.LateJoinAllowed = true;
		KrokoshaScavMultiplayer.rules.LateJoinSpectate = true;
		KrokoshaScavMultiplayer.rules.SavePlayerInventory = false;
		WorldgenPatches.SetRadlinePlayerPrefs(enabled: false);
	}

	private void Start()
	{
		WorldgenPatches.OnWorldgenFinish += OnWorldgenFinish;
		NetBody.OnPlayerDeath += OnPlayerDeath;
		NetPlayer.OnPlayerJoined += OnPlayerJoined;
		NetPlayer.OnPlayerLeft += OnPlayerLeft;
		SceneManager.sceneLoaded += OnSceneLoaded;
		((MonoBehaviour)this).InvokeRepeating("SUpdate", 1f, 1f);
		((MonoBehaviour)this).InvokeRepeating("SlowUpdate", 1f, 10f);
		((MonoBehaviour)this).InvokeRepeating("VerySlowUpdate", 10f, 30f);
		log.l("Battleroyal gamemode activated");
		Chat.Server_ChatAnnouncement("Gamemode set to battle royale");
	}

	public static void DistributeEntities_But_for_items_and_load_ammo(GameObject basObj, float minPerChunk, float maxPerChunk, float spawnYOffset = 0f, float randomRotation = 0f, float spawnYOffsetDeviation = 0f, bool spawnInGround = false, PlaceCheckDelegate checkFunc = null, Vector2 dir = default(Vector2))
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		if (dir == Vector2.zero)
		{
			dir = Vector2.down;
		}
		float num = (float)(chunkWidth * chunkHeight) * Random.Range(minPerChunk, maxPerChunk);
		Vector2 val = default(Vector2);
		GunScript val3 = default(GunScript);
		AmmoScript val4 = default(AmmoScript);
		for (int i = 0; (float)i < num; i++)
		{
			((Vector2)(ref val))._002Ector(Random.Range((float)(0L - (long)halfWidth), (float)halfWidth), Random.Range((float)(0L - (long)halfHeight), (float)halfHeight));
			if (!(!Object.op_Implicit((Object)(object)Physics2D.OverlapPoint(val, LayerMask.GetMask(new string[1] { "Ground" }))) || spawnInGround))
			{
				continue;
			}
			RaycastHit2D val2 = Physics2D.Raycast(val, dir, (float)WorldGeneration.CHUNKSIZE, LayerMask.GetMask(new string[1] { "Ground" }));
			if (RaycastHit2D.op_Implicit(val2) && Math.Abs(((RaycastHit2D)(ref val2)).point.x) < (float)halfWidth - 1f && Math.Abs(((RaycastHit2D)(ref val2)).point.y) < (float)halfHeight - 1f && (checkFunc == null || checkFunc.Invoke(world.WorldToBlockPos(((RaycastHit2D)(ref val2)).point - Vector2.up * 0.5f))))
			{
				GameObject obj = Object.Instantiate<GameObject>(basObj, Vector2.op_Implicit(((RaycastHit2D)(ref val2)).point - dir * Random.Range(spawnYOffset - spawnYOffsetDeviation, spawnYOffset + spawnYOffsetDeviation)), Quaternion.Euler(0f, 0f, basObj.transform.eulerAngles.z + Random.Range(0f - randomRotation, randomRotation)));
				obj.TryGetComponent<GunScript>(ref val3);
				if (obj.TryGetComponent<AmmoScript>(ref val4))
				{
					val4.rounds = Random.Range(0, val4.maxRounds);
				}
			}
		}
	}

	public static void SpawnWeapons()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Expected O, but got Unknown
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Expected O, but got Unknown
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Expected O, but got Unknown
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Expected O, but got Unknown
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Expected O, but got Unknown
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Expected O, but got Unknown
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Expected O, but got Unknown
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Expected O, but got Unknown
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Expected O, but got Unknown
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Expected O, but got Unknown
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Expected O, but got Unknown
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Expected O, but got Unknown
		world.generatingWorld = true;
		try
		{
			float lootRarityMultiplier = world.lootRarityMultiplier;
			DistributeEntities_But_for_items_and_load_ammo((GameObject)Resources.Load("shotgun"), 0.3f * lootRarityMultiplier, 0.4f * lootRarityMultiplier, 0.1f, 360f, 0.1f);
			DistributeEntities_But_for_items_and_load_ammo((GameObject)Resources.Load("rifle"), 0.3f * lootRarityMultiplier, 0.4f * lootRarityMultiplier, 1f, 360f);
			DistributeEntities_But_for_items_and_load_ammo((GameObject)Resources.Load("pistol"), 0.3f * lootRarityMultiplier, 0.4f * lootRarityMultiplier, 0f, 360f);
			DistributeEntities_But_for_items_and_load_ammo((GameObject)Resources.Load("machete"), 0.6f * lootRarityMultiplier, 0.7f * lootRarityMultiplier, 0f, 360f, 0.1f);
			DistributeEntities_But_for_items_and_load_ammo((GameObject)Resources.Load("sledgehammer"), 1f * lootRarityMultiplier, 1.1f * lootRarityMultiplier, 0f, 360f, 0.1f);
			DistributeEntities_But_for_items_and_load_ammo((GameObject)Resources.Load("bandage"), 1f * lootRarityMultiplier, 1.1f * lootRarityMultiplier, 0f, 360f, 0.1f);
			DistributeEntities_But_for_items_and_load_ammo((GameObject)Resources.Load("combatpen"), 1f * lootRarityMultiplier, 1.1f * lootRarityMultiplier, 0f, 360f, 0.1f);
			DistributeEntities_But_for_items_and_load_ammo((GameObject)Resources.Load("hoodie"), 0.1f * lootRarityMultiplier, 0.2f * lootRarityMultiplier, 0f, 360f, 0.1f);
			DistributeEntities_But_for_items_and_load_ammo((GameObject)Resources.Load("riflemagazine"), 0.5f * lootRarityMultiplier, 0.6f * lootRarityMultiplier, 0f, 360f, 0.1f);
			DistributeEntities_But_for_items_and_load_ammo((GameObject)Resources.Load("smallmagazine"), 0.7f * lootRarityMultiplier, 0.9f * lootRarityMultiplier, 0f, 360f, 0.1f);
			DistributeEntities_But_for_items_and_load_ammo((GameObject)Resources.Load("boxof12gauge"), 0.4f * lootRarityMultiplier, 0.8f * lootRarityMultiplier, 0f, 360f, 0.1f);
			DistributeEntities_But_for_items_and_load_ammo((GameObject)Resources.Load("scaffoldingpack"), 0.2f * lootRarityMultiplier, 0.22f * lootRarityMultiplier, 0f, 360f, 0.1f);
		}
		catch (Exception ex)
		{
			log.error(ex.ToString());
		}
		world.generatingWorld = false;
	}

	private void OnWorldgenFinish()
	{
		ServerMain.Server_AnnounceAlert("Waiting for players to load...", important: true);
		RadiationLine.line.Deactivate();
		((MonoBehaviour)this).StartCoroutine(GameStartSequence());
	}

	private void OnPlayerDeath(NetPlayer plr)
	{
		ServerMain._ded_server_switch_counter = -20f;
		if (Util.IsWorldGenerated() && plr.levelPlayTime > 3.0)
		{
			List<NetPlayer> allLivingPlayers = NetPlayer.AllLivingPlayers;
			if (allLivingPlayers.Count == 1)
			{
				NetPlayer netPlayer = allLivingPlayers[0];
				Chat.Server_ChatAnnouncement("Winner Winner Chicken Dinner! " + netPlayer.playername);
				string msg = netPlayer.playername + " won!!! congrats !!!";
				ServerMain.Server_AnnounceAlert(in msg, important: false);
				Chat.Server_ChatAnnouncement(in msg);
			}
			else
			{
				ServerMain.Server_AnnounceAlert($"{plr.playername} died! ({NetPlayer.AllLivingPlayers.Count}/{NetPlayer.BodyToPlayerDict.Count})", important: false);
			}
		}
	}

	private void OnPlayerJoined(NetPlayer plr)
	{
	}

	private void OnPlayerLeft(NetPlayer plr)
	{
	}

	public static void OnSceneLoaded(Scene a, LoadSceneMode b)
	{
		if (((Scene)(ref a)).name == "SampleScene")
		{
			Util.CallLambdaWhen(() => (Object)(object)world != (Object)null, (Action)delegate
			{
				world.biomeDepth = happening_layer;
			});
		}
	}

	private void SetMainMenuRules()
	{
		KrokoshaScavMultiplayer.rules.PVP = false;
		KrokoshaScavMultiplayer.rules.ScatterPunishDistance = 64999f;
	}

	protected override void Update()
	{
		base.Update();
		if (!Util.IsInWorld())
		{
			SetMainMenuRules();
			GAME_ON = false;
		}
		else if (!KrokoshaScavMultiplayer.network_system_is_running)
		{
			Object.Destroy((Object)(object)this);
		}
		if (Util.IsWorldGenerated() && RadiationLine.line.active)
		{
			RadiationLineUpdatePatch.timeGone += Time.deltaTime;
		}
	}

	private void SUpdate()
	{
		if (!GAME_ON || !KrokoshaScavMultiplayer.network_system_is_running)
		{
			return;
		}
		List<NetPlayer> allLivingPlayers = NetPlayer.AllLivingPlayers;
		if (KrokoshaScavMultiplayer.rules.AutoExitWhenAllDied && ServerMain._ded_server_switch_counter > 10f && allLivingPlayers.Count <= 1)
		{
			if (allLivingPlayers.Count == 1)
			{
				Chat.Server_ChatAnnouncement("Winner Winner Chicken Dinner! " + allLivingPlayers[0].playername);
			}
			PlayerCamera.main.ToMainMenu();
		}
		if (NetPlayer.AllLivingPlayers.Any((NetPlayer x) => x.playerbody.chip_punishment_protocol))
		{
			AnnounceDirectionsForAll();
		}
	}

	private void SlowUpdate()
	{
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		if (!GAME_ON || !KrokoshaScavMultiplayer.network_system_is_running)
		{
			return;
		}
		KrokoshaScavMultiplayer.ApplyGameRules();
		RadiationLineUpdatePatch.timeGone.RemapClamped(RadiationLineUpdatePatch.timeGone_limit - 200f, 0f, 64f, 1024f);
		foreach (NetPlayer allLivingPlayer in NetPlayer.AllLivingPlayers)
		{
			int count = NetPlayer.AllLivingPlayers.Count;
			float num = 0f;
			float num2 = 0f;
			if (!allLivingPlayer.IsAlive() || allLivingPlayer.levelPlayTime < 20.0 || count <= 1)
			{
				continue;
			}
			List<NetPlayer> list = new List<NetPlayer>(NetPlayer.AllLivingPlayers);
			list.RemoveAll((NetPlayer x) => x.levelPlayTime < 20.0);
			if (KrokoshaScavMultiplayer.is_server)
			{
				list.RemoveAll((NetPlayer x) => !x.server_plrstate.is_loaded_in);
			}
			if (list.Count <= 1)
			{
				continue;
			}
			num = Math.Max(1, (int)Mathf.Ceil((float)list.Count * 0.5f));
			float num3 = Util.MetersToTiles(KrokoshaScavMultiplayer.rules.ScatterPunishDistance);
			num3 *= num3;
			Vector3 position = ((Component)allLivingPlayer.body).transform.position;
			count = 0;
			foreach (NetPlayer allLivingPlayer2 in NetPlayer.AllLivingPlayers)
			{
				float num4 = KM.dist2dsqr(Vector2.op_Implicit(((Component)allLivingPlayer2.body).transform.position), Vector2.op_Implicit(position));
				if (num4 < num3)
				{
					count++;
					if (num4 > num2)
					{
						num2 = num4;
					}
				}
			}
			num2 = Mathf.Sqrt(num2);
			if ((float)count < num)
			{
				allLivingPlayer.playerbody.chip_punishment_protocol = true;
				Body body = allLivingPlayer.body;
				body.radiationSickness += 0.1f;
				NetBody cbaaaa;
				string directions = GetDirections(allLivingPlayer, 999999f, out cbaaaa);
				if (directions != null)
				{
					allLivingPlayer.Server_DoAlertSingle("You're outside play area!\n" + cbaaaa.bodyname + " is on your " + directions, reliable: false);
				}
				else
				{
					allLivingPlayer.Server_DoAlertSingle("You're outside play area!", reliable: false);
				}
			}
		}
	}

	public void AnnounceDirectionsForAll()
	{
		foreach (NetPlayer allLivingPlayer in NetPlayer.AllLivingPlayers)
		{
			NetBody cbaaaa;
			string directions = GetDirections(allLivingPlayer, 70f, out cbaaaa);
			if (directions != null)
			{
				allLivingPlayer.Server_DoAlertSingle(cbaaaa.bodyname + " is on your " + directions);
			}
		}
	}

	public string GetDirections(NetPlayer item, float maxdist, out NetBody cbaaaa)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		Vector2 from = item.pos;
		(NetBody, float) nearestConsciousPlayerToThisBody = NetPlayer.GetNearestConsciousPlayerToThisBody(item.playerbody);
		(cbaaaa, _) = nearestConsciousPlayerToThisBody;
		if ((Object)(object)nearestConsciousPlayerToThisBody.Item1 != (Object)null && nearestConsciousPlayerToThisBody.Item2 > maxdist * maxdist)
		{
			Vector2 val = KM.normal(in from, nearestConsciousPlayerToThisBody.Item1.pos);
			string text = "";
			if (val.y > 0.5f)
			{
				text = "Top";
			}
			if (val.y < -0.5f)
			{
				text = "Bottom";
			}
			float num = 0f;
			if (!string.IsNullOrEmpty(text))
			{
				num = 0.5f;
			}
			string text2 = "";
			if (val.x > num)
			{
				text2 = "Right";
			}
			else if (val.x < 0f - num)
			{
				text2 = "Left";
			}
			return text + " " + text2;
		}
		return null;
	}

	private void VerySlowUpdate()
	{
		if (GAME_ON && KrokoshaScavMultiplayer.network_system_is_running)
		{
			AnnounceDirectionsForAll();
		}
	}

	private IEnumerator GameStartSequence()
	{
		Con.RunCommand("godmode 1");
		Vector2 val = default(Vector2);
		while (true)
		{
			if (!NetPlayer.AllLivingPlayers.All((NetPlayer x) => x.levelPlayTime > 20.0))
			{
				if (Util.IsInWorld())
				{
					yield return (object)new WaitForSecondsRealtime(1f);
					continue;
				}
				break;
			}
			yield return (object)new WaitForSecondsRealtime(5f);
			if (!Util.IsInWorld())
			{
				break;
			}
			yield return (object)new WaitForSecondsRealtime(1f);
			if (!Util.IsInWorld())
			{
				break;
			}
			Dictionary<NetPlayer, Vector2> plr_positions = new Dictionary<NetPlayer, Vector2>();
			ServerMain.Server_AnnounceAlert("...Assigning spawn locations...", important: true, reliable: false);
			List<NetPlayer> list = new List<NetPlayer>(NetPlayer.AllLivingPlayers);
			foreach (NetPlayer item in list)
			{
				item.body.Body_DropAllItems();
				Vector2 pos = new Vector2((float)Random.Range(-500, 500), (float)Random.Range(-10, 480));
				float num = float.MinValue;
				for (int num2 = 0; num2 < 40; num2++)
				{
					((Vector2)(ref val))._002Ector((float)Random.Range(-500, 500), (float)Random.Range(-10 - num2 * 12, 480));
					if (Object.op_Implicit((Object)(object)Physics2D.OverlapCircle(pos, 2f, LayerMask.GetMask(new string[1] { "Ground" }))) || FluidManager.main.WaterInfo(world.WorldToBlockPos(pos)).Item1 > 0f)
					{
						continue;
					}
					(NetBody, float) nearestBody = NetBody.GetNearestBody(val, must_be_alive: true, must_be_conscious: true);
					if (!(nearestBody.Item2 > num))
					{
						continue;
					}
					num = nearestBody.Item2;
					pos = val;
					for (int num3 = 0; num3 < 100; num3++)
					{
						Vector2 val2 = pos - Vector2.up;
						if (Object.op_Implicit((Object)(object)Physics2D.OverlapCircle(pos, 1f, LayerMask.GetMask(new string[1] { "Ground" }))))
						{
							break;
						}
						pos = val2;
					}
				}
				((Component)item.body).transform.position = Vector2.op_Implicit(pos);
				WorldChunkSync.Server_CheckPlrPosChunk(item);
				Util.DelayCallLambda(3f, (Action)delegate
				{
					//IL_0032: Unknown result type (might be due to invalid IL or missing references)
					//IL_0037: Unknown result type (might be due to invalid IL or missing references)
					WorldChunkSync.singleton.timer_TilemapSync = 10f;
					item.playerbody.StopPiggyback();
					((Component)item.body).transform.position = Vector2.op_Implicit(pos);
					item.Server_RemindPlayersCurrentState();
				});
				plr_positions[item] = pos;
				item.body.skills.AddExp(0, 1000f);
				item.body.skills.AddExp(1, 1000f);
				item.body.skills.AddExp(2, 1000f);
				item.body.ResetHealth();
				yield return (object)new WaitForSecondsRealtime(0.01f);
			}
			WorldChunkSync.singleton.timer_TilemapFluidSync = 10f;
			WorldChunkSync.singleton.timer_TilemapSync = 10f;
			yield return (object)new WaitForSecondsRealtime(4f);
			if (!Util.IsInWorld())
			{
				break;
			}
			ServerMain.Server_AnnounceAlert("5", important: true, reliable: false);
			yield return (object)new WaitForSecondsRealtime(1f);
			Chat.Server_ChatAnnouncement("Welcome to fortnite.");
			ServerMain.Server_AnnounceAlert("4", important: true, reliable: false);
			KrokoshaScavMultiplayer.rules.ShowPlayerDirections = false;
			KrokoshaScavMultiplayer.ApplyGameRules();
			yield return (object)new WaitForSecondsRealtime(1f);
			ServerMain.Server_AnnounceAlert("3", important: true, reliable: false);
			yield return (object)new WaitForSecondsRealtime(1f);
			WorldChunkSync.singleton.timer_TilemapFluidSync = 10f;
			WorldChunkSync.singleton.timer_TilemapSync = 10f;
			ServerMain.Server_AnnounceAlert("2", important: true, reliable: false);
			RadiationLine.line.Activate();
			yield return (object)new WaitForSecondsRealtime(1f);
			ServerMain.Server_AnnounceAlert("1", important: true, reliable: false);
			yield return null;
			log.l("battle royale: spawning weapons and items");
			SpawnWeapons();
			yield return (object)new WaitForSecondsRealtime(1f);
			WorldChunkSync.singleton.timer_TilemapFluidSync = 10f;
			WorldChunkSync.singleton.timer_TilemapSync = 10f;
			foreach (KeyValuePair<NetPlayer, Vector2> item2 in plr_positions)
			{
				try
				{
					item2.Key.body.Body_DropAllItems();
					((Component)item2.Key.body).transform.position = Vector2.op_Implicit(item2.Value);
					item2.Key.Server_RemindPlayersCurrentState();
				}
				catch (Exception ex)
				{
					log.error("BattleRoyale, final spawn sequence ERROR:" + ex.ToString());
				}
			}
			ServerMain.Server_AnnounceAlert("!!! BATTLE ROYALE STARTS NOW !!!\nBECOME THE LAST SURVIVOR", important: true);
			yield return (object)new WaitForSecondsRealtime(1f);
			WorldChunkSync.singleton.timer_TilemapFluidSync = 10f;
			WorldChunkSync.singleton.timer_TilemapSync = 10f;
			Con.RunCommand("godmode 0");
			Con.RunCommand("give @a scaffoldingpack");
			GAME_ON = true;
			KrokoshaScavMultiplayer.rules.PVP = true;
			KrokoshaScavMultiplayer.ApplyGameRules();
			break;
		}
	}
}
