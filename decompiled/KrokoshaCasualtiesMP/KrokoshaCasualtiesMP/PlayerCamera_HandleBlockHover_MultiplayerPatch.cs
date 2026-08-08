using HarmonyLib;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(PlayerCamera), "HandleBlockHover")]
internal static class PlayerCamera_HandleBlockHover_MultiplayerPatch
{
	private static void Postfix(PlayerCamera __instance, Collider2D hitcol)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		if (log.verbose)
		{
			Vector3 val = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector2Int val2 = WorldGeneration.world.WorldToBlockPos(Vector2.op_Implicit(val));
			ushort block = WorldGeneration.world.GetBlock(val2);
			BlockInfo blockInfo = WorldGeneration.world.GetBlockInfo(block);
			byte liquid = FluidManager.main.GetLiquid(((Vector2Int)(ref val2)).x, ((Vector2Int)(ref val2)).y);
			GlobalDark.main.tooltipText.Item2 = $"{GlobalDark.main.tooltipText.Item2}\nBLOCKPOS: {val2}\nBLOCK ID: {block}\nSLEEPQUAL: {blockInfo.sleep}";
			if (liquid != 0)
			{
				GlobalDark.main.tooltipText.Item2 += $"\nFLUID ID: {liquid}";
			}
		}
	}
}
