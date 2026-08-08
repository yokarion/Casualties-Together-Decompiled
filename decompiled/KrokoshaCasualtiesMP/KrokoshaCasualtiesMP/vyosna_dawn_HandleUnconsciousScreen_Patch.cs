using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(PlayerCamera), "HandleUnconsciousScreen")]
public static class vyosna_dawn_HandleUnconsciousScreen_Patch
{
	private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
	{
		List<CodeInstruction> list = new List<CodeInstruction>(instructions);
		List<int> list2 = new List<int> { 0, 1 };
		int num = 0;
		for (int i = 0; i < instructions.Count(); i++)
		{
			if (num >= list2.Count)
			{
				break;
			}
			MethodBase methodBase;
			if (((object)list[i]).ToString().Contains("SetTimeScale") && list2.Contains(num) && (methodBase = list[i].operand as MethodBase) != null)
			{
				list[i].opcode = OpCodes.Nop;
				list[i].operand = null;
				int num2 = methodBase.GetParameters().Length;
				if (!methodBase.IsStatic)
				{
					num2++;
				}
				for (int num3 = i; num3 >= i - num2; num3--)
				{
					list[num3].opcode = OpCodes.Nop;
					list[num3].operand = null;
				}
				num++;
			}
		}
		return list;
	}
}
