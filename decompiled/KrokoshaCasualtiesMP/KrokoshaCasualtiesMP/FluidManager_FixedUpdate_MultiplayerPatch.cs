using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(FluidManager), "FixedUpdate")]
public static class FluidManager_FixedUpdate_MultiplayerPatch
{
	private static int simIndex;

	private static bool Prefix()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		if (NetBody.all_instances.Count > 0 && Net.is_server)
		{
			HashSet<Vector2UInt8> hashSet = new HashSet<Vector2UInt8>();
			foreach (NetBody all_instance in NetBody.all_instances)
			{
				Vector2 val = Vector2Int.op_Implicit(WorldGeneration.world.WorldToBlockPos(Vector2.op_Implicit(((Component)all_instance).transform.position))) / 128f;
				hashSet.Add((Vector2UInt8)val);
				int num = WorldChunkSync._CheckTheProxNum(val.x);
				int num2 = WorldChunkSync._CheckTheProxNum(val.y);
				hashSet.Add((Vector2UInt8)new Vector2(val.x + (float)num, val.y));
				hashSet.Add((Vector2UInt8)new Vector2(val.x, val.y + (float)num2));
				hashSet.Add((Vector2UInt8)new Vector2(val.x + (float)num, val.y + (float)num2));
			}
			int num3 = 8;
			Vector2Int val2 = default(Vector2Int);
			RangeI val3 = default(RangeI);
			RangeI val4 = default(RangeI);
			foreach (Vector2UInt8 item in hashSet)
			{
				if (item.x <= num3 - 1 && item.y <= num3 - 1)
				{
					((Vector2Int)(ref val2))._002Ector(item.x * 128, item.y * 128);
					((RangeI)(ref val3))._002Ector(((Vector2Int)(ref val2)).x, ((Vector2Int)(ref val2)).x + 128);
					((RangeI)(ref val4))._002Ector(((Vector2Int)(ref val2)).y + simIndex, ((Vector2Int)(ref val2)).y + 16 + simIndex);
					if (simIndex + 16 >= 112)
					{
						val4.max += 16;
					}
					if (val3.min < 1)
					{
						val3.min = 1;
					}
					if (val4.min < 1)
					{
						val4.min = 1;
					}
					if (val3.max < 1)
					{
						val3.max = 1;
					}
					if (val4.max < 1)
					{
						val4.max = 1;
					}
					if (val3.max > WorldGeneration.world.width - 2)
					{
						val3.max = (int)(WorldGeneration.world.width - 2);
					}
					if (val4.max > WorldGeneration.world.height - 2)
					{
						val4.max = (int)(WorldGeneration.world.height - 2);
					}
					if (val3.min > WorldGeneration.world.width - 2)
					{
						val3.min = (int)(WorldGeneration.world.width - 2);
					}
					if (val4.min > WorldGeneration.world.height - 2)
					{
						val4.min = (int)(WorldGeneration.world.height - 2);
					}
					FluidManager_SimulationRangeIndex_MultiplayerPatch.forcenext_avaiable = true;
					FluidManager_SimulationRangeIndex_MultiplayerPatch.forcenext = (val3, val4);
					FluidManager.main.SimulationStep();
				}
			}
			simIndex += 16;
			if (simIndex >= 112)
			{
				simIndex = 0;
			}
			return false;
		}
		return true;
	}
}
