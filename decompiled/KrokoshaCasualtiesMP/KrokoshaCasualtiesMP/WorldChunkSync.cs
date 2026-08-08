using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using KrokoshaCasualtiesUtils;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public class WorldChunkSync : KrokoshaScavSingleton
{
	public static WorldChunkSync singleton;

	public static bool _TILE_SYNC_ENABLED = true;

	public static bool _FLUID_SYNC_ENABLED = true;

	public static int _fluid_chunksync_counter = 0;

	public static int _chunksync_counter = 0;

	private const float TilemapSyncFrequency = 1.061102f;

	private const float TilemapFluidSyncFrequency = 0.435067f;

	public float timer_TilemapSync;

	public float timer_TilemapFluidSync;

	public const int SYNCCHUNK_TILESIZE = 32;

	public const int SYNCCHUNK_DIMENSION = 32;

	public const int SYNCREGION_SIZE = 8;

	public const int HALF_SYNCREGION_SIZE = 4;

	public const int WORLDSIZE = 1024;

	public const int TILEID_BITSIZE = 6;

	public const int FLUIDID_BITSIZE = 3;

	public const int FLUIDSIMULATIONZONESIZE = 128;

	public const int FLUIDCHUNKSIZE = 64;

	public const int HALF_FLUIDCHUNKSIZE = 32;

	public static uint[,] Server_ChunkHashes = new uint[32, 32];

	public static readonly List<(knetid, float, Vector2UInt8)> server_chunksync_queue = new List<(knetid, float, Vector2UInt8)>();

	public static WorldGeneration world => WorldGeneration.world;

	public static int CHUNKSIZE => WorldGeneration.CHUNKSIZE;

	public static ushort[,] worldBlocks => WorldGeneration.world.worldBlocks;

	public static bool instantiatingWorld => WorldGeneration.world.instantiatingWorld;

	public static byte[,] fluid => FluidManager.main.fluid;

	private void Awake()
	{
		singleton = this;
	}

	private void Start()
	{
		KrokoshaScavMultiplayer.OnSceneChangeOrWorldStartGenerate += delegate
		{
			Server_ChunkHashes = new uint[32, 32];
		};
	}

	private void LateUpdate()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		if (!KrokoshaScavMultiplayer.network_system_is_running || !KrokoshaScavMultiplayer.IsInGameAndWorldGenerated())
		{
			return;
		}
		if (WorldGenerationSetBlockPatch.tile_did_change)
		{
			WorldGenerationSetBlockPatch.tile_did_change = false;
			if (!KrokoshaScavMultiplayer.is_client && _TILE_SYNC_ENABLED)
			{
				ushort block = WorldGeneration.world.GetBlock(WorldGenerationSetBlockPatch.last_tile_change_pos);
				NetDataWriter writer = Net.CreateWriter(10007);
				writer.Put(WorldGenerationSetBlockPatch.last_tile_change_pos);
				writer.Put(block);
				writer.Put(false);
				Net.Server_SendToClients((DeliveryMethod)4, in writer, (IEnumerable<knetid>)ServerMain.AllClientIdsExceptHost);
			}
		}
		if (world.savePanel.activeSelf)
		{
			PlayerCamera.main.body.forceWalk = false;
			if (KrokoshaScavMultiplayer.rules.CheckIfLayerContinueIsDisabled())
			{
				world.savePanel.SetActive(false);
			}
			else
			{
				NetPlayer lOCAL_PLAYER = NetPlayer.LOCAL_PLAYER;
				if (Object.op_Implicit((Object)(object)lOCAL_PLAYER) && !world.doingRegen && !world.generatingWorld && world.worldExists && !lOCAL_PLAYER.finishedLayer)
				{
					lOCAL_PLAYER.finishedLayer = true;
					if (ServerMain.GetNumberOfPlayersRequiredToFinishLayer() != 1)
					{
						PlayerCamera.main.DoAlert(Lang.Get("layerfinish_wait_for_friends", false), false);
					}
					else
					{
						PlayerCamera.main.DoAlert(Lang.Get("layerfinish_wait_for_host", false), false);
					}
				}
				if (KrokoshaScavMultiplayer.is_client)
				{
					world.savePanel.SetActive(false);
				}
				else
				{
					bool flag = ServerMain.CheckIfEnoughPeopleAreAtLayerFinish();
					if (flag != world.savePanel.activeSelf)
					{
						world.savePanel.SetActive(flag);
					}
				}
			}
		}
		if (KrokoshaScavMultiplayer.is_server)
		{
			for (int i = 0; i < Mathf.Max(NetPlayer.ClientIdToPlayerDict.Count / 2, 1); i++)
			{
				if (server_chunksync_queue.Count > 0)
				{
					(knetid, float, Vector2UInt8) tuple = server_chunksync_queue.First();
					server_chunksync_queue.RemoveAt(0);
					if (NetPlayer.TryGetPlayerFromClientId(tuple.Item1, out var _))
					{
						Server_Sendchunk(tuple.Item3, tuple.Item1);
					}
				}
			}
			Server_ChunkHashes = new uint[32, 32];
		}
		timer_TilemapSync += ClientMain.AdaptiveSyncTimerDelta;
		timer_TilemapFluidSync += ClientMain.AdaptiveSyncTimerDelta;
		if (timer_TilemapSync > 1.061102f)
		{
			timer_TilemapSync = 0f;
			TilemapSyncUpdate();
		}
		if (timer_TilemapFluidSync > 0.435067f)
		{
			timer_TilemapFluidSync = 0f;
			FluidTilemapSyncUpdate();
		}
	}

	public static void Server_Sendchunk(Vector2UInt8 syncchunk_coordinate, knetid plrId, bool reliable = false)
	{
		if (NetPlayer.TryGetPlayerFromClientId(plrId, out var plr))
		{
			Server_Sendchunk(syncchunk_coordinate, plr, reliable);
		}
	}

	public static void Server_Sendchunk(Vector2UInt8 syncchunk_coordinate, NetPlayer plr, bool reliable = false)
	{
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		ushort[,] array = worldBlocks;
		NetDataWriter writer = Net.CreateWriter(10153);
		writer.Put(syncchunk_coordinate.x);
		writer.Put(syncchunk_coordinate.y);
		int num = syncchunk_coordinate.x * 32;
		int num2 = syncchunk_coordinate.y * 32;
		using MemoryStream memoryStream = new MemoryStream();
		using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionLevel.Optimal))
		{
			using MemoryStream memoryStream2 = new MemoryStream();
			for (int i = 0; i < 32; i++)
			{
				for (int j = 0; j < 32; j++)
				{
					memoryStream2.WriteByte((byte)array[j + num, i + num2]);
				}
			}
			byte[] array2 = memoryStream2.ToArray();
			deflateStream.Write(array2, 0, array2.Length);
		}
		writer.PutBytesWithLength(memoryStream.ToArray());
		Net.Server_SendToClients((DeliveryMethod)((!reliable) ? 4 : 0), in writer, plr.clientId);
		plr.server_plrstate.known_chunks[syncchunk_coordinate.x, syncchunk_coordinate.y] = true;
	}

	public static NetDataWriter _Server_PackFluidChunk(in Vector2UInt8 syncchunk_coordinate)
	{
		int num = syncchunk_coordinate.x * 32;
		int num2 = syncchunk_coordinate.y * 32;
		NetDataWriter val = Net.CreateWriter(10154);
		val.Put(syncchunk_coordinate.x);
		val.Put(syncchunk_coordinate.y);
		byte[,] array = fluid;
		using MemoryStream memoryStream = new MemoryStream();
		using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionLevel.Optimal))
		{
			using MemoryStream memoryStream2 = new MemoryStream();
			for (int i = 0; i < 32; i++)
			{
				for (int j = 0; j < 32; j++)
				{
					memoryStream2.WriteByte(array[j + num, i + num2]);
				}
			}
			byte[] array2 = memoryStream2.ToArray();
			deflateStream.Write(array2, 0, array2.Length);
		}
		val.PutBytesWithLength(memoryStream.ToArray());
		return val;
	}

	public static void Server_SendFluidChunk(Vector2UInt8 syncchunk_coordinate, NetPlayer plr, bool reliable = false)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		NetDataWriter writer = _Server_PackFluidChunk(in syncchunk_coordinate);
		Net.Server_SendToClients((DeliveryMethod)((!reliable) ? 4 : 0), in writer, plr.clientId);
	}

	internal static void FluidTilemapSyncUpdate()
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		if (!KrokoshaScavMultiplayer.network_system_is_running || !_FLUID_SYNC_ENABLED || KrokoshaScavMultiplayer.is_client)
		{
			return;
		}
		List<Vector2Int> list = new List<Vector2Int>();
		foreach (NetPlayer value in NetPlayer.ClientIdToPlayerDict.Values)
		{
			if (!value.is_local && Object.op_Implicit((Object)(object)value.body) && value.body.alive)
			{
				Vector2 val = value.pos - new Vector2(32f, 32f);
				Vector2Int item = WorldGeneration.world.WorldToBlockPos(val);
				list.Add(item);
				if (!value.body.inWater && value.body.liquidSlipTime <= 0f && value.body.liquidRagdollBar >= 1f && value.body.liquidDrinkTime <= 0f)
				{
					Vector2Int item2 = WorldGeneration.world.WorldToBlockPos(val + new Vector2(0f, -64f));
					list.Add(item2);
				}
			}
		}
		if (list.Count <= 0)
		{
			return;
		}
		HashSet<Vector2UInt8> hashSet = new HashSet<Vector2UInt8>();
		Vector2Int val2 = default(Vector2Int);
		foreach (Vector2Int item3 in list)
		{
			Vector2Int current2 = item3;
			((Vector2Int)(ref val2))._002Ector(((Vector2Int)(ref current2)).x / 32, ((Vector2Int)(ref current2)).y / 32);
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					Vector2Int val3 = val2 + new Vector2Int(j, i);
					((Vector2Int)(ref val3)).Clamp(new Vector2Int(0, 0), new Vector2Int(31, 31));
					hashSet.Add((Vector2UInt8)val3);
				}
			}
		}
		if (_fluid_chunksync_counter >= hashSet.Count)
		{
			_fluid_chunksync_counter = 0;
		}
		Vector2UInt8 syncchunk_coordinate = hashSet.ElementAt(_fluid_chunksync_counter);
		_fluid_chunksync_counter++;
		Net.Server_SendToClients((DeliveryMethod)4, _Server_PackFluidChunk(in syncchunk_coordinate), (IEnumerable<knetid>)ServerMain.AllClientIdsExceptHost);
	}

	public static uint HashRegion(in ushort[,] arr, int xmin, int xmax, int ymin, int ymax)
	{
		ulong num = 1469598103934665603uL;
		ushort num2 = 0;
		for (int i = ymin; i < ymax; i++)
		{
			for (int j = xmin; j < xmax; j++)
			{
				ushort num3 = arr[j, i];
				if (num3 != 0)
				{
					num2++;
				}
				num ^= (byte)num3;
				num *= 1099511628211L;
				num ^= (byte)(num3 >> 8);
				num *= 1099511628211L;
			}
		}
		return (uint)(((int)(num ^ (num >> 32)) << 10) | num2);
	}

	public static Vector2UInt8 WorldPosToSyncchunkCoordinate(Vector2 pos)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return BlockPosToSyncchunkCoordinate(WorldGeneration.world.WorldToBlockPos(pos));
	}

	public static Vector2UInt8 WorldPosToSyncchunkCoordinateSafe(Vector2 pos)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return BlockPosToSyncchunkCoordinateSafe(WorldGeneration.world.WorldToBlockPos(pos));
	}

	public static Vector2UInt8 BlockPosToSyncchunkCoordinate(Vector2Int blockpos)
	{
		return new Vector2UInt8((byte)(((Vector2Int)(ref blockpos)).x / 32), (byte)(((Vector2Int)(ref blockpos)).y / 32));
	}

	public static Vector2UInt8 BlockPosToSyncchunkCoordinateSafe(Vector2Int blockpos)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		Vector2Int val = default(Vector2Int);
		((Vector2Int)(ref val))._002Ector(((Vector2Int)(ref blockpos)).x / 32, ((Vector2Int)(ref blockpos)).y / 32);
		((Vector2Int)(ref val)).Clamp(new Vector2Int(0, 0), new Vector2Int(31, 31));
		return (Vector2UInt8)val;
	}

	internal static void TilemapSyncUpdate()
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		if (!KrokoshaScavMultiplayer.network_system_is_running || !_TILE_SYNC_ENABLED)
		{
			return;
		}
		if (KrokoshaScavMultiplayer.is_client)
		{
			Vector2UInt8 vector2UInt = WorldPosToSyncchunkCoordinateSafe(Vector2.op_Implicit(PlayerCamera.main.body.alive ? ((Component)PlayerCamera.main.body).transform.position : ((Component)Camera.main).transform.position));
			Vector2Int val = default(Vector2Int);
			((Vector2Int)(ref val))._002Ector(vector2UInt.x - 4, vector2UInt.y - 4 - 1);
			((Vector2Int)(ref val)).Clamp(new Vector2Int(0, 0), new Vector2Int(24, 24));
			NetDataWriter writer = Net.CreateWriter(10155);
			writer.Put((byte)((Vector2Int)(ref val)).x);
			writer.Put((byte)((Vector2Int)(ref val)).y);
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					int num = (((Vector2Int)(ref val)).x + j) * 32;
					int num2 = (((Vector2Int)(ref val)).y + i) * 32;
					uint num3 = HashRegion(worldBlocks, num, num + 32, num2, num2 + 32);
					writer.Put(num3);
				}
			}
			Net.Client_Send((DeliveryMethod)4, in writer);
			return;
		}
		foreach (NetPlayer item in ServerMain.AllPlayersExceptHost)
		{
			Server_CheckPlrPosChunk(item);
		}
	}

	public static void Server_CheckPlrPosChunk(NetPlayer plr)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		Vector2UInt8 vector2UInt = WorldPosToSyncchunkCoordinateSafe(plr.pos - new Vector2(16f, 0f));
		for (int i = Mathf.Max(0, vector2UInt.y - 2); i < Mathf.Min(vector2UInt.y + 1, 32); i++)
		{
			for (int j = vector2UInt.x; j < Mathf.Min(vector2UInt.x + 2, 32); j++)
			{
				if (!plr.server_plrstate.known_chunks[j, i])
				{
					Vector2UInt8 syncchunk_coordinate = new Vector2UInt8(j, i);
					Server_Sendchunk(syncchunk_coordinate, plr);
					Server_SendFluidChunk(syncchunk_coordinate, plr);
				}
			}
		}
	}

	public static int _CheckTheProxNum(float n)
	{
		float num = n - Mathf.Floor(n);
		if (num < 0.4f)
		{
			return -1;
		}
		if (num > 0.6f)
		{
			return 1;
		}
		return 0;
	}

	private byte[,] GetChunkData(Vector2Int chunk)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		Vector2Int val = chunk * CHUNKSIZE;
		byte[,] array = new byte[CHUNKSIZE, CHUNKSIZE];
		for (int i = 0; i < CHUNKSIZE; i++)
		{
			for (int j = 0; j < CHUNKSIZE; j++)
			{
				array[i, j] = (byte)worldBlocks[i + ((Vector2Int)(ref val)).x, j + ((Vector2Int)(ref val)).y];
			}
		}
		return array;
	}

	private byte[,] GetFluidChunkData(Vector2Int lefttopcorner)
	{
		byte[,] array = new byte[64, 64];
		for (int i = 0; i < 64; i++)
		{
			for (int j = 0; j < 64; j++)
			{
				array[i, j] = fluid[i + ((Vector2Int)(ref lefttopcorner)).x, j + ((Vector2Int)(ref lefttopcorner)).y];
			}
		}
		return array;
	}

	[ClientReceiver(10154, true)]
	private static void ClientReceiver_WorldFluidTilemapChunk(knetid _, ref NetDataReader reader)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)FluidManager.main == (Object)null || fluid == null)
		{
			return;
		}
		byte b = default(byte);
		reader.Get(ref b);
		byte b2 = default(byte);
		reader.Get(ref b2);
		Vector2Int val = new Vector2Int((int)b, (int)b2) * 32;
		byte[,] array = fluid;
		reader.Get(out byte[] result);
		byte[] array2 = Util.DecompressDeflate(result);
		int num = 0;
		for (int i = 0; i < 32; i++)
		{
			for (int j = 0; j < 32; j++)
			{
				byte b3 = array2[num++];
				array[j + ((Vector2Int)(ref val)).x, i + ((Vector2Int)(ref val)).y] = b3;
			}
		}
	}

	[ClientReceiver(10153, true)]
	private static void ClientReceiver_WorldTilemapChunk(knetid _, ref NetDataReader reader)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)world == (Object)null || instantiatingWorld)
		{
			return;
		}
		ushort[,] array = worldBlocks;
		if (array == null)
		{
			return;
		}
		byte b = default(byte);
		reader.Get(ref b);
		byte b2 = default(byte);
		reader.Get(ref b2);
		Vector2Int val = new Vector2Int((int)b, (int)b2) * 32;
		reader.Get(out byte[] result);
		byte[] array2 = Util.DecompressDeflate(result);
		int num = 0;
		for (int i = 0; i < 32; i++)
		{
			for (int j = 0; j < 32; j++)
			{
				byte b3 = array2[num++];
				array[j + ((Vector2Int)(ref val)).x, i + ((Vector2Int)(ref val)).y] = b3;
			}
		}
		WorldGeneration.world.UpdateChunkClosest(val);
		if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
		{
			DebugHelp.OnNetEvent(WorldGeneration.world.BlockToWorldPos(val), $"C: Received chunk data {b},{b2}");
		}
	}

	[ServerReceiver(10155)]
	private static void Server_HeyUhhMyChunksAreLikeThisBruh(knetid clientId, ref NetDataReader reader)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		if (!Util.IsWorldGenerated() || worldBlocks == null || !NetPlayer.TryGetPlayerFromClientId(clientId, out var plr) || server_chunksync_queue.Count > 200)
		{
			return;
		}
		byte b = default(byte);
		reader.Get(ref b);
		byte b2 = default(byte);
		reader.Get(ref b2);
		Vector2Int val = default(Vector2Int);
		((Vector2Int)(ref val))._002Ector((int)b, (int)b2);
		((Vector2Int)(ref val)).Clamp(new Vector2Int(0, 0), new Vector2Int(24, 24));
		if (((Vector2Int)(ref val)).x != b || ((Vector2Int)(ref val)).y != b2)
		{
			Plugin.log.LogWarning((object)$"SUS: ChunksHash: {plr} -> Invalid chunk coordinate!");
			return;
		}
		Vector2Int val2 = WorldGeneration.world.WorldToBlockPos(plr.pos);
		uint num = default(uint);
		Vector2 a = default(Vector2);
		for (int i = 0; i < 8; i++)
		{
			for (int j = 0; j < 8; j++)
			{
				reader.Get(ref num);
				int num2 = ((Vector2Int)(ref val)).x + j;
				int num3 = ((Vector2Int)(ref val)).y + i;
				int num4 = num2 * 32;
				int num5 = num3 * 32;
				uint num6 = Server_ChunkHashes[num2, num3];
				if (num6 == 0)
				{
					num6 = (Server_ChunkHashes[num2, num3] = HashRegion(worldBlocks, num4, num4 + 32, num5, num5 + 32));
				}
				if (num != num6)
				{
					((Vector2)(ref a))._002Ector((float)num4, (float)num5);
					float dist = KM.dist2dsqr(in a, Vector2Int.op_Implicit(val2));
					Server_QueueChunkToSync(chunkPos: new Vector2UInt8(((Vector2Int)(ref val)).x + j, ((Vector2Int)(ref val)).y + i), plrId: plr.clientId, dist: dist);
					if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
					{
						DebugHelp.OnNetEvent(WorldGeneration.world.BlockToWorldPos(new Vector2Int(num4, num5)), $"S: {plr} doesn't know about this chunk  {b},{b2}");
					}
				}
			}
		}
	}

	private static void Server_QueueChunkToSync(knetid plrId, float dist, Vector2UInt8 chunkPos)
	{
		(knetid, float, Vector2UInt8) item = (plrId, dist, chunkPos);
		if (server_chunksync_queue.Any(((knetid, float, Vector2UInt8) x) => (ushort)x.Item1 == (ushort)plrId && x.Item3 == chunkPos))
		{
			return;
		}
		int num;
		for (num = 0; num < server_chunksync_queue.Count; num++)
		{
			if (!(server_chunksync_queue[num].Item2 < dist))
			{
				server_chunksync_queue.Insert(num, item);
				return;
			}
		}
		if (num < 100)
		{
			server_chunksync_queue.Add(item);
		}
	}
}
