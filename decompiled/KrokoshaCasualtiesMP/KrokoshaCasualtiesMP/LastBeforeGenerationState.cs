using System.Runtime.InteropServices;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public struct LastBeforeGenerationState : INetSerializeByMemcpy
{
	public KrokoshaMultiplayerGameRules rules = KrokoshaScavMultiplayer.rules;

	public State randomstate = Random.state;

	public byte biomeOverride = (byte)WorldGeneration.world.biomeOverride;

	public bool radlinedisable = false;

	public bool unchipped = WorldGeneration.unchipped;

	public byte biomeDepth = (byte)WorldGeneration.world.biomeDepth;

	public float lootRarityMultiplier = WorldGeneration.world.lootRarityMultiplier;

	public float trapRarityMultiplier = WorldGeneration.world.trapRarityMultiplier;

	public int totalTraveled = WorldGeneration.world.totalTraveled;

	public bool censorMood = Body.censorMood;

	public LastBeforeGenerationState()
	{
	}//IL_0013: Unknown result type (might be due to invalid IL or missing references)
	//IL_0018: Unknown result type (might be due to invalid IL or missing references)
	//IL_0023: Unknown result type (might be due to invalid IL or missing references)


	public void Apply()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		Random.state = randomstate;
		KrokoshaScavMultiplayer.rules = rules;
		WorldGeneration.world.biomeOverride = (OverrideSceneType)biomeOverride;
		WorldGeneration.world.biomeDepth = biomeDepth;
		WorldGeneration.world.lootRarityMultiplier = lootRarityMultiplier;
		WorldGeneration.world.trapRarityMultiplier = trapRarityMultiplier;
		WorldGeneration.world.totalTraveled = totalTraveled;
		Body.censorMood = censorMood;
		if (!KrokoshaScavMultiplayer.rules.UnchippedIsIndividual)
		{
			WorldGeneration.world.unchippedMode = unchipped;
		}
	}

	public static int GetByteSize()
	{
		return Marshal.SizeOf(typeof(LastBeforeGenerationState));
	}
}
