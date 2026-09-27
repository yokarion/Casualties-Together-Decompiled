using System;
using System.Collections.Generic;
using System.Linq;
using KrokoshaCasualtiesUtils;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public class SharedMain : MonoBehaviour
{
	public static float max_player_interaction_distance = 9f;

	public static bool local_world_is_generating = false;

	public static bool local_world_is_generated = false;

	public static double LastWorldgenFinishTime = 0.0;

	private static bool[,] CHUNKS_VISIBLE_TO_PLAYERS = new bool[16, 16];

	private static int VISIBLE_CHUNK_RANGE_HALF = 2;

	internal static float timer_SendPerformanceReport = 0f;

	public static float max_player_interaction_distance_sq => max_player_interaction_distance * max_player_interaction_distance;

	public static bool CheckIfDispersionPunishmentProtocolRuleIsActive()
	{
		if (Time.realtimeSinceStartupAsDouble - LastWorldgenFinishTime > (double)(KrokoshaScavMultiplayer.is_client ? 10f : 20f) && NetPlayer.AllLivingPlayers.Count() > 0 && KrokoshaScavMultiplayer.rules.PlayerScatterDisallowed)
		{
			return KrokoshaScavMultiplayer.rules.ScatterMinGroupSize > 0;
		}
		return false;
	}

	public static bool CheckIfShouldActivateDispersionPunishmentProtocol(NetPlayer plr, out float max_recorded_dist, out int count_plrs_in_range, out int required_count_plrs_in_range)
	{
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		count_plrs_in_range = NetPlayer.AllLivingPlayers.Count;
		required_count_plrs_in_range = 0;
		max_recorded_dist = 0f;
		if (!plr.IsAlive() || plr.levelPlayTime < 20.0)
		{
			return false;
		}
		if (count_plrs_in_range <= 1)
		{
			return false;
		}
		List<NetPlayer> list = new List<NetPlayer>(NetPlayer.AllLivingPlayers);
		list.RemoveAll((NetPlayer x) => x.levelPlayTime < 20.0);
		if (KrokoshaScavMultiplayer.is_server)
		{
			list.RemoveAll((NetPlayer x) => !x.server_plrstate.is_loaded_in);
		}
		if (KrokoshaScavMultiplayer.rules.Teams)
		{
			list.RemoveAll((NetPlayer x) => !x.IsInSameTeamAs(plr));
		}
		if (list.Count <= 1)
		{
			return false;
		}
		required_count_plrs_in_range = Math.Max(1, (int)Mathf.Ceil((float)list.Count * (float)(int)KrokoshaScavMultiplayer.rules.ScatterMinGroupSize * 0.01f));
		float num = Util.MetersToTiles(KrokoshaScavMultiplayer.rules.ScatterPunishDistance);
		num *= num;
		Vector3 position = ((Component)plr.body).transform.position;
		count_plrs_in_range = 0;
		foreach (NetPlayer allLivingPlayer in NetPlayer.AllLivingPlayers)
		{
			float num2 = KM.dist2dsqr(Vector2.op_Implicit(((Component)allLivingPlayer.body).transform.position), Vector2.op_Implicit(position));
			if (num2 < num)
			{
				count_plrs_in_range++;
				if (num2 > max_recorded_dist)
				{
					max_recorded_dist = num2;
				}
			}
		}
		max_recorded_dist = Mathf.Sqrt(max_recorded_dist);
		if (count_plrs_in_range >= required_count_plrs_in_range)
		{
			return false;
		}
		return true;
	}

	public static void CreatePlayerCharacters()
	{
		if (!KrokoshaScavMultiplayer.network_system_is_running)
		{
			return;
		}
		if ((Object)(object)ServerMain.CHARACTER_PREFAB != (Object)null)
		{
			Plugin.log.LogInfo((object)"Creating other player characters!");
			{
				foreach (NetPlayer value in NetPlayer.ClientIdToPlayerDict.Values)
				{
					if (!value.is_local && Net.is_server)
					{
						value.CreateCharacter();
					}
				}
				return;
			}
		}
		log.error("Tried to create characters before they exist! RESTART THE GAME!");
	}

	public static void ForceUpdatePlayerLists()
	{
		NetPlayer.AllLivingPlayers.Clear();
		NetPlayer.AllDeadPlayers.Clear();
		foreach (NetPlayer value in NetPlayer.BodyToPlayerDict.Values)
		{
			if (!((Object)(object)value == (Object)null) && !((Object)(object)value.body == (Object)null))
			{
				if (value.body.alive)
				{
					NetPlayer.AllLivingPlayers.Add(value);
				}
				else
				{
					NetPlayer.AllDeadPlayers.Add(value);
				}
			}
		}
	}

	public static bool CheckIfChunkOnThisPositionIsVisibleByAnyPlayer(Vector2 pos)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return CheckIfChunkOnThisPositionIsVisibleByAnyPlayer_BlockPos(WorldGeneration.world.WorldToBlockPos(pos));
	}

	public static bool CheckIfChunkOnThisPositionIsVisibleByAnyPlayer_BlockPos(Vector2Int blockpos)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		Vector2Int val = WorldGeneration.world.BlockToChunkPos(blockpos);
		int num = CHUNKS_VISIBLE_TO_PLAYERS.GetLength(1) - 1;
		int num2 = CHUNKS_VISIBLE_TO_PLAYERS.GetLength(0) - 1;
		((Vector2Int)(ref val)).x = Mathf.Clamp(((Vector2Int)(ref val)).x, 0, num2);
		((Vector2Int)(ref val)).y = Mathf.Clamp(((Vector2Int)(ref val)).y, 0, num);
		return CHUNKS_VISIBLE_TO_PLAYERS[((Vector2Int)(ref val)).x, ((Vector2Int)(ref val)).y];
	}

	private static void _PlayerVisibleChunksAroundPos(Vector2 pos)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		float num = 1f / (float)WorldGeneration.CHUNKSIZE;
		int num2 = (int)((float)WorldGeneration.world.chunkWidth * 0.5f);
		int num3 = (int)((float)WorldGeneration.world.chunkHeight * 0.5f);
		int num4 = CHUNKS_VISIBLE_TO_PLAYERS.GetLength(1) - 1;
		int num5 = CHUNKS_VISIBLE_TO_PLAYERS.GetLength(0) - 1;
		int num6 = Mathf.Clamp((int)(pos.x * num) - VISIBLE_CHUNK_RANGE_HALF + num2, 0, num5);
		int num7 = Mathf.Clamp((int)(pos.x * num) + VISIBLE_CHUNK_RANGE_HALF + 1 + num2, 0, num5);
		int num8 = Mathf.Clamp((int)(pos.y * num) - VISIBLE_CHUNK_RANGE_HALF + num3, 0, num4);
		int num9 = Mathf.Clamp((int)(pos.y * num) + VISIBLE_CHUNK_RANGE_HALF + 1 + num3, 0, num4);
		for (int i = num8; i <= num9; i++)
		{
			for (int k = num6; k <= num7; k++)
			{
				CHUNKS_VISIBLE_TO_PLAYERS[k, i] = true;
			}
		}
	}

	private static void _UpdatePlayerVisibleChunks()
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < CHUNKS_VISIBLE_TO_PLAYERS.GetLength(1); i++)
		{
			for (int k = 0; k < CHUNKS_VISIBLE_TO_PLAYERS.GetLength(0); k++)
			{
				CHUNKS_VISIBLE_TO_PLAYERS[k, i] = false;
			}
		}
		if (!local_world_is_generated)
		{
			return;
		}
		_PlayerVisibleChunksAroundPos(Vector2.op_Implicit(((Component)WorldGeneration.world.mainCam).transform.position));
		if (!Net.running || KrokoshaScavMultiplayer.is_client)
		{
			return;
		}
		foreach (KeyValuePair<Body, NetPlayer> item in NetPlayer.BodyToPlayerDict)
		{
			_PlayerVisibleChunksAroundPos(item.Value.pos);
		}
	}

	private void Update()
	{
		local_world_is_generated = Util.IsWorldGenerated();
		local_world_is_generating = Util.IsGeneratingWorld();
		ForceUpdatePlayerLists();
		_UpdatePlayerVisibleChunks();
		if (Net.is_connected)
		{
			timer_SendPerformanceReport += ClientMain.AdaptiveSyncTimerDelta;
			if (timer_SendPerformanceReport > 1f)
			{
				timer_SendPerformanceReport = 0f;
				UpdateSendPerformanceReport();
			}
		}
	}

	private static void UpdateSendPerformanceReport()
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (KrokoshaScavMultiplayer.is_client)
		{
			NetDataWriter writer = Net.CreateWriter(10043);
			writer.Put(ServerMain.CURRENT_FPS);
			writer.Put(ServerMain.CURRENT_TPS);
			Net.Client_Send((DeliveryMethod)4, in writer);
		}
		else
		{
			NetDataWriter writer2 = Net.CreateWriter(10044);
			writer2.Put(ServerMain.CURRENT_FPS);
			writer2.Put(ServerMain.CURRENT_TPS);
			Net.Server_SendToClients((DeliveryMethod)4, in writer2, (IEnumerable<knetid>)ServerMain.AllClientIdsExceptHost);
		}
	}

	[ClientReceiver(10044, true)]
	private static void ClientReceiver__CurrentPerformanceReport(knetid _, ref NetDataReader reader)
	{
		reader.Get(ref ClientMain.SERVER_FPS);
		reader.Get(ref ClientMain.SERVER_TPS);
	}

	[ServerReceiver(10043)]
	private static void ServerReceiver__CurrentPerformanceReport_fromclient(knetid clientId, ref NetDataReader reader)
	{
		if (NetPlayer.TryGetPlayerFromClientId(clientId, out var plr))
		{
			reader.Get(ref plr.server_plrstate.fps);
			reader.Get(ref plr.server_plrstate.tps);
		}
	}
}
