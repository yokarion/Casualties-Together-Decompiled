using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(DamagingCrate), "OnCollisionEnter2D")]
public static class DamagingCrate_OnCollisionEnter2D_MultiplayerPatch
{
	private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
	{
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Expected O, but got Unknown
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Expected O, but got Unknown
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Expected O, but got Unknown
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Expected O, but got Unknown
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Expected O, but got Unknown
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Expected O, but got Unknown
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Expected O, but got Unknown
		List<CodeInstruction> list = new List<CodeInstruction>(instructions);
		int i;
		for (i = 10; !((object)list[i - 1]).ToString().Contains("LimbFromObject") || !(list[i].opcode == OpCodes.Stloc_1); i++)
		{
		}
		for (; !(list[i + 1].opcode == OpCodes.Ldarg_0) || !((object)list[i + 2]).ToString().Contains("DamagingCrate") || !((object)list[i + 2]).ToString().Contains("type"); i++)
		{
		}
		if (log.verbose)
		{
			Plugin.log.LogWarning((object)$"TEMP DEV: DamagingCrate OnCollisionEnter2D TRANSPILER LimbFromObject and stloc.1 {i}");
		}
		int j;
		for (j = i; !((object)list[j]).ToString().Contains("prevFrameSpeed"); j++)
		{
		}
		object operand = list[j].operand;
		List<CodeInstruction> list2 = new List<CodeInstruction>();
		list2.Add(new CodeInstruction(OpCodes.Ldarg_0, (object)null));
		list2.Add(new CodeInstruction(OpCodes.Ldloc_1, (object)null));
		list2.Add(new CodeInstruction(OpCodes.Ldarg_0, (object)null));
		list2.Add(new CodeInstruction(OpCodes.Ldfld, operand));
		list2.Add(new CodeInstruction(OpCodes.Call, (object)AccessTools.Method(typeof(ScavTraps), "DamagingCrate_OnCollisionEnter2D_StolenWithFirstCheck", (Type[])null, (Type[])null)));
		Label label = il.DefineLabel();
		list[i + 1].labels.Add(label);
		list2.Add(new CodeInstruction(OpCodes.Brfalse_S, (object)label));
		list2.Add(new CodeInstruction(OpCodes.Ret, (object)null));
		list.InsertRange(i + 1, list2);
		if (log.verbose)
		{
			Plugin.log.LogWarning((object)"TEMP DEV: DamagingCrate OnCollisionEnter2D TRANSPILER SUCCESS :)  ");
		}
		return list;
	}
}
