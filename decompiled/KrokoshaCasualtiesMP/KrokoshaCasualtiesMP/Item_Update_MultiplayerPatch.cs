using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(Item), "Update")]
public static class Item_Update_MultiplayerPatch
{
	public static bool MPinSimRange(Vector2 pos)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		Vector2Int val = WorldGeneration.world.WorldToBlockPos(pos);
		TilemapRenderer closestChunkRenderer = WorldGeneration.world.GetClosestChunkRenderer(val);
		if ((Object)(object)closestChunkRenderer != (Object)null)
		{
			flag = ((Renderer)closestChunkRenderer).enabled;
		}
		if (!Net.running)
		{
			return flag;
		}
		if (!flag)
		{
			return SharedMain.CheckIfChunkOnThisPositionIsVisibleByAnyPlayer_BlockPos(val);
		}
		return flag;
	}

	private static void Postfix(Item __instance)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		KrokoshaScavMultiGameObjectNetworkTracker krokoshaScavMultiGameObjectNetworkTracker = default(KrokoshaScavMultiGameObjectNetworkTracker);
		if (KrokoshaScavMultiplayer.network_system_is_running && ((Component)__instance).TryGetComponent<KrokoshaScavMultiGameObjectNetworkTracker>(ref krokoshaScavMultiGameObjectNetworkTracker) && krokoshaScavMultiGameObjectNetworkTracker.is_within_anyones_view && __instance.canGetWet && Mathf.Abs(((Component)__instance).transform.position.x) < (float)WorldGeneration.world.halfWidth && Mathf.Abs(((Component)__instance).transform.position.y) < (float)WorldGeneration.world.halfHeight && FluidManager.main.HasLiquid(WorldGeneration.world.WorldToBlockPos(Vector2.op_Implicit(((Component)__instance).transform.position))))
		{
			__instance.wetTime = Time.time;
		}
	}

	private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Expected O, but got Unknown
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Expected O, but got Unknown
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Expected O, but got Unknown
		List<CodeInstruction> list = new List<CodeInstruction>(instructions);
		int i;
		for (i = 0; !((object)list[i]).ToString().Contains("get_enabled") || !(list[i].opcode == OpCodes.Callvirt) || !((object)list[i]).ToString().Contains("Renderer"); i++)
		{
		}
		i++;
		list.RemoveRange(0, i);
		List<CodeInstruction> list2 = new List<CodeInstruction>();
		list2.Add(new CodeInstruction(OpCodes.Ldarg_0, (object)null));
		list2.Add(new CodeInstruction(OpCodes.Call, (object)AccessTools.Method(typeof(Component), "get_transform", (Type[])null, (Type[])null)));
		list2.Add(new CodeInstruction(OpCodes.Callvirt, (object)AccessTools.Method(typeof(Transform), "get_position", (Type[])null, (Type[])null)));
		list2.Add(new CodeInstruction(OpCodes.Call, (object)AccessTools.Method(typeof(Vector2), "op_Implicit", new Type[1] { typeof(Vector3) }, (Type[])null)));
		list2.Add(new CodeInstruction(OpCodes.Call, (object)AccessTools.Method(typeof(Item_Update_MultiplayerPatch), "MPinSimRange", (Type[])null, (Type[])null)));
		list.InsertRange(0, list2);
		return list;
	}
}
