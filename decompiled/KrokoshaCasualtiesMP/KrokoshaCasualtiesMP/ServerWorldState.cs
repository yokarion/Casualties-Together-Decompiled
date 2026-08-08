using System.Collections;
using System.Collections.Generic;
using LiteNetLib.Utils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public struct ServerWorldState : IDeltaPacketBase
{
	public bool server_is_generating_world;

	public bool radlineactive;

	public float radlinestate;

	public float layerTimeSpent;

	public float earthquakeDelay;

	public float earthquakeTime;

	public bool has_surface_bg;

	public Color surface_color;

	public float rainintensity;

	public static int bitset_size = 9;

	public void Server_Serialize()
	{
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		server_is_generating_world = SharedMain.local_world_is_generating;
		if ((Object)(object)RadiationLine.line != (Object)null)
		{
			radlineactive = RadiationLine.line.active;
			radlinestate = RadiationLineUpdatePatch.timeGone;
		}
		else
		{
			radlineactive = false;
		}
		if ((Object)(object)WorldGeneration.world != (Object)null)
		{
			layerTimeSpent = WorldGeneration.world.layerTimeSpent;
			earthquakeDelay = WorldGeneration.world.earthquakeDelay;
			earthquakeTime = WorldGeneration.world.earthquakeTime;
			has_surface_bg = WorldgenPatches.HasSkyBackground(out var skyinfo);
			if (has_surface_bg)
			{
				(surface_color, rainintensity) = skyinfo;
			}
		}
		else
		{
			layerTimeSpent = 0f;
			has_surface_bg = false;
		}
		ClientMain.serverWorldState = this;
	}

	public void Deserialize()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		ClientMain.serverWorldState = this;
		if ((Object)(object)WorldGeneration.world != (Object)null)
		{
			if (has_surface_bg != WorldgenPatches.HasSkyBackground())
			{
				if (has_surface_bg)
				{
					WorldgenPatches.CreateSkyBackground(surface_color, rainintensity);
				}
				else
				{
					WorldgenPatches.RemoveSkyBackground();
				}
			}
			if (earthquakeDelay < -1f)
			{
				WorldGeneration.world.earthquakeDelay = 999999f;
			}
			else
			{
				WorldGeneration.world.earthquakeDelay = earthquakeDelay;
			}
			WorldGeneration.world.earthquakeTime = earthquakeTime;
			WorldGeneration.world.layerTimeSpent = layerTimeSpent;
		}
		if ((Object)(object)RadiationLine.line != (Object)null && RadiationLine.line.active != radlineactive)
		{
			if (radlineactive)
			{
				RadiationLine.line.Activate();
			}
			else
			{
				RadiationLine.line.Deactivate();
			}
		}
		RadiationLineUpdatePatch.timeGone = radlinestate;
	}

	public void SetDefault()
	{
	}

	public void Write(NetDataWriter writer, List<bool> pack_bools, IDeltaPacketBase old)
	{
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		ServerWorldState obj = (ServerWorldState)(object)old;
		bool flag = false;
		pack_bools.Add(server_is_generating_world);
		pack_bools.Add(radlineactive);
		flag = obj.radlinestate != radlinestate;
		pack_bools.Add(flag);
		if (flag)
		{
			writer.Put(radlinestate);
		}
		flag = obj.layerTimeSpent != layerTimeSpent;
		pack_bools.Add(flag);
		if (flag)
		{
			writer.Put(layerTimeSpent);
		}
		flag = obj.earthquakeDelay != earthquakeDelay;
		pack_bools.Add(flag);
		if (flag)
		{
			writer.Put(earthquakeDelay);
		}
		flag = obj.earthquakeTime != earthquakeTime;
		pack_bools.Add(flag);
		if (flag)
		{
			writer.Put(earthquakeTime);
		}
		pack_bools.Add(has_surface_bg);
		flag = obj.surface_color != surface_color;
		pack_bools.Add(flag);
		if (flag)
		{
			writer.Put(surface_color);
		}
		flag = obj.rainintensity != rainintensity;
		pack_bools.Add(flag);
		if (flag)
		{
			writer.Put(rainintensity);
		}
	}

	public void Read(NetDataReader reader, BitArray bitset)
	{
		server_is_generating_world = bitset[0];
		radlineactive = bitset[1];
		if (bitset[2])
		{
			reader.Get(ref radlinestate);
		}
		if (bitset[3])
		{
			reader.Get(ref layerTimeSpent);
		}
		if (bitset[4])
		{
			reader.Get(ref earthquakeDelay);
		}
		if (bitset[5])
		{
			reader.Get(ref earthquakeTime);
		}
		has_surface_bg = bitset[6];
		if (bitset[7])
		{
			reader.Get(out surface_color);
		}
		if (bitset[8])
		{
			reader.Get(ref rainintensity);
		}
	}
}
