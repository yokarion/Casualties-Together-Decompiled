using System.Collections.Generic;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public class Server_PlayerState
{
	public struct LocSnapshot
	{
		public bool alive;

		public Vector2_4byte_512 pos;
	}

	public int didmutewarn;

	public int fps = 60;

	public int tps = 60;

	public bool admin_privilege;

	public float layer_transition_x_offset;

	public double last_push;

	public double last_cursormove;

	public bool loaded_save;

	public bool did_give_spawn_location;

	public bool did_give_spawn_location_from_a_save;

	public bool finished_worldgen;

	public bool is_loaded_in;

	public bool[,] known_chunks;

	private Dictionary<string, double> cooldowns = new Dictionary<string, double>();

	public List<LocSnapshot> cur_location_history;

	public List<List<LocSnapshot>> location_history = new List<List<LocSnapshot>>();

	public NetPlayer plr;

	public void SaveLocationSnapshot()
	{
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		if (cur_location_history == null)
		{
			cur_location_history = new List<LocSnapshot>();
			location_history.Add(cur_location_history);
		}
		if (cur_location_history.Count > 400)
		{
			cur_location_history.RemoveAt(Random.Range(1, 399));
			cur_location_history.RemoveAt(Random.Range(1, 398));
		}
		cur_location_history.Add(new LocSnapshot
		{
			alive = plr.IsAlive(),
			pos = plr.pos
		});
	}

	public Server_PlayerState(NetPlayer p)
	{
		plr = p;
		OnWorldGenerate();
		OnSceneChangeOrWorldStartGenerate();
	}

	public void ResetKnownChunks()
	{
		known_chunks = new bool[32, 32];
		if (!plr.is_local)
		{
			return;
		}
		for (int i = 0; i < 32; i++)
		{
			for (int k = 0; k < 32; k++)
			{
				known_chunks[k, i] = true;
			}
		}
	}

	public void OnWorldGenerate()
	{
		ResetKnownChunks();
	}

	public void OnWorldgenStart()
	{
		ResetKnownChunks();
		did_give_spawn_location = false;
		finished_worldgen = false;
	}

	public void OnSceneChangeOrWorldStartGenerate()
	{
		did_give_spawn_location = false;
		is_loaded_in = false;
		loaded_save = false;
	}

	public bool Cooldown(in string id, float cooldown)
	{
		if (cooldowns.TryGetValue(id, out var value))
		{
			double num = Time.realtimeSinceStartupAsDouble - value;
			if (num < (double)cooldown)
			{
				if (log.verbose)
				{
					log.serverdeny($"COOLDOWN, TOO FAST, {plr}, req: {cooldown} cur: {num} ");
				}
				return false;
			}
		}
		cooldowns[id] = Time.realtimeSinceStartupAsDouble;
		return true;
	}
}
