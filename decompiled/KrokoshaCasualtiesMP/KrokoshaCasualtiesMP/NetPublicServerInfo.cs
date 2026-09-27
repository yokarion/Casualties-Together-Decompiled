using System.Collections.Generic;
using System.Linq;
using KrokoshaCasualtiesMultiplayerAPI;
using KrokoshaCasualtiesUtils;
using LiteNetLib.Utils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public class NetPublicServerInfo : INetSerializable
{
	public bool dedicated;

	public bool haspassword;

	public string version = "6.7.6.7";

	public string name = "";

	public string gamemode = "vanilla";

	public KSteam.Lobby steam_lobby_info;

	public KrokoshaMultiplayerGameRules rules;

	public int plr_living_count;

	public int plr_count;

	public int plr_max = 8;

	public int cur_layer = -1;

	public int cur_depth;

	public int avg_happiness;

	public string[] modlist = new string[0];

	public bool enforcemodlist_locked;

	public bool midjoin_lock_active;

	public string friends_tooltip;

	public string rules_as_string;

	public List<ulong> steamfriends = new List<ulong>();

	public bool latejoinspectate => rules.LateJoinSpectate;

	public NetPublicServerInfo SetValues()
	{
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Invalid comparison between Unknown and I4
		dedicated = Net.is_dedicated_server;
		haspassword = !string.IsNullOrEmpty(KrokoshaScavMultiplayer.INPUT_PASSWORD);
		plr_max = KrokoshaScavMultiplayer.rules.PLAYER_COUNT_LIMIT;
		midjoin_lock_active = Net.IsMyServerMidjoinLocked();
		rules = KrokoshaScavMultiplayer.rules;
		bool flag = Util.IsInWorld();
		if (flag)
		{
			plr_count = NetPlayer.ClientIdToPlayerDict.Count;
			plr_living_count = NetPlayer.AllLivingPlayers.Count;
			cur_layer = WorldGeneration.world.biomeDepth;
			if (NetPlayer.AllLivingPlayers.Count > 0)
			{
				avg_happiness = (int)NetPlayer.AllLivingPlayers.Average((NetPlayer x) => (int)x.body.happiness);
			}
		}
		else
		{
			plr_count = NetPlayer.ClientIdToPlayerDict.Count;
			plr_living_count = plr_count;
			cur_layer = -1;
			avg_happiness = 100;
		}
		if (Util.IsGeneratingWorld())
		{
			gamemode = "loading";
		}
		else if (Util.IsTutorialWorld())
		{
			gamemode = "tutorial";
		}
		else if ((Object)(object)GamemodeManager.GetGamemode() == (Object)null)
		{
			if (!flag)
			{
				gamemode = "mainmenu";
			}
			else
			{
				gamemode = (((int)WorldGeneration.world.biomeOverride == 2) ? "debug_world" : "vanilla");
			}
		}
		else
		{
			gamemode = ((object)GamemodeManager.GetGamemode()).GetType().Name;
		}
		return this;
	}

	public bool IsJoinable()
	{
		if (!enforcemodlist_locked && !midjoin_lock_active)
		{
			return KrokoshaScavMultiplayer.FULL_VERSION_TAG == version;
		}
		return false;
	}

	public void Deserialize(NetDataReader reader)
	{
		reader.Get(ref version);
		reader.Get(ref dedicated);
		reader.Get(ref haspassword);
		reader.Get(ref name);
		reader.Get(ref gamemode);
		reader.Get(ref plr_living_count);
		reader.Get(ref plr_count);
		reader.Get(ref plr_max);
		reader.Get(ref cur_layer);
		reader.Get(ref cur_depth);
		reader.Get(ref avg_happiness);
		reader.Get(out rules);
	}

	public void Serialize(NetDataWriter writer)
	{
		writer.Put(version);
		writer.Put(dedicated);
		writer.Put(haspassword);
		writer.Put(name);
		writer.Put(gamemode);
		writer.Put(plr_living_count);
		writer.Put(plr_count);
		writer.Put(plr_max);
		writer.Put(cur_layer);
		writer.Put(cur_depth);
		writer.Put(avg_happiness);
		writer.Put(rules);
	}
}
