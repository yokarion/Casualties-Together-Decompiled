using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(BuildingEntity), "Update")]
public static class BuildingEntity_Update_MultiplayerPatch
{
	private static void Prefix(BuildingEntity __instance, ref Vector3 __state)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		__state = default(Vector3);
	}

	private static void Postfix(BuildingEntity __instance)
	{
		Rigidbody2D rb = default(Rigidbody2D);
		if (((Component)__instance).TryGetComponent<Rigidbody2D>(ref rb) && !__instance.ignoreBodyOptimize)
		{
			NewBuildingOptimizeThing(rb);
		}
	}

	private static void NewBuildingOptimizeThing(Rigidbody2D rb)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (!WorldGeneration.world.worldExists || !(Time.timeScale <= 5f))
		{
			rb.bodyType = (RigidbodyType2D)2;
			return;
		}
		bool flag = SharedMain.CheckIfChunkOnThisPositionIsVisibleByAnyPlayer(rb.position);
		rb.bodyType = (RigidbodyType2D)((!flag) ? 2 : 0);
	}

	private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
	{
		List<CodeInstruction> list = new List<CodeInstruction>(instructions);
		int i;
		for (i = 0; !(list[i].opcode == OpCodes.Ldfld) || !((object)list[i]).ToString().Contains("ignoreBodyOptimize"); i++)
		{
		}
		i++;
		int num = i;
		Label item = (Label)list[num].operand;
		int num2 = i + 1;
		for (; !(list[i].opcode == OpCodes.Ldstr) || !(list[i].operand as string == "footstep/Rock/11"); i++)
		{
			if (list[i].labels.Contains(item))
			{
				num2 = i;
				if (log.verbose)
				{
					Plugin.log.LogWarning((object)"TEMP DEV: bodyoptimizescopeendlabel FOUND !!!!!!!");
				}
			}
		}
		for (; !(list[i].opcode == OpCodes.Stloc_1); i++)
		{
		}
		int num3 = i;
		while (!(list[num3].opcode == OpCodes.Ldarg_0))
		{
			num3--;
		}
		list[num3].opcode = OpCodes.Ldc_I4_1;
		list.RemoveRange(num3 + 1, i - 1 - num3);
		num++;
		list.RemoveRange(num, num2 - num);
		if (log.verbose)
		{
			Plugin.log.LogWarning((object)"TEMP DEV: BuildingEntity Update TRANSPILER SUCCESS :) ");
		}
		return list;
	}
}
