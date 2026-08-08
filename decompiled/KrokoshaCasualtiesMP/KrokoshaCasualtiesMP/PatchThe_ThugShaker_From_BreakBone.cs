using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Unity.VisualScripting;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(Limb), "BreakBone")]
public static class PatchThe_ThugShaker_From_BreakBone
{
	public static bool Prefix(Limb __instance)
	{
		if (__instance.dismembered)
		{
			return false;
		}
		return true;
	}

	public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
	{
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Expected O, but got Unknown
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Expected O, but got Unknown
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Expected O, but got Unknown
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Expected O, but got Unknown
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Expected O, but got Unknown
		List<CodeInstruction> list = new List<CodeInstruction>(instructions);
		bool flag = false;
		for (int i = 0; i < list.Count - 3; i++)
		{
			if (list[i].opcode == OpCodes.Ldsfld && list[i].operand is FieldInfo fieldInfo && fieldInfo.FieldType == typeof(PlayerCamera) && fieldInfo.Name == "main" && list[i + 1].opcode == OpCodes.Ldfld && list[i + 1].operand is FieldInfo fieldInfo2 && fieldInfo2.FieldType == typeof(Shaker) && fieldInfo2.Name == "shaker" && list[i + 3].opcode == OpCodes.Callvirt && list[i + 3].operand is MethodInfo { Name: "Shake" } methodInfo && methodInfo.DeclaringType == typeof(Shaker))
			{
				float num = (float)list[i + 2].operand;
				List<CodeInstruction> list2 = new List<CodeInstruction>();
				list2.Add(new CodeInstruction(OpCodes.Ldarg_0, (object)null));
				list2.Add(new CodeInstruction(OpCodes.Ldfld, (object)AccessTools.Field(typeof(Limb), "body")));
				list2.Add(new CodeInstruction(OpCodes.Call, (object)typeof(ComponentHolderProtocol).GetMethod("GetOrAddComponent", BindingFlags.Static | BindingFlags.Public).MakeGenericMethod(typeof(NetBody))));
				list2.Add(new CodeInstruction(OpCodes.Ldc_R4, (object)num));
				list2.Add(new CodeInstruction(OpCodes.Callvirt, (object)AccessTools.Method(typeof(NetBody), "CameraShakeOverrideFunc", new Type[1] { typeof(float) }, (Type[])null)));
				list.RemoveRange(i, 4);
				list.InsertRange(i, list2);
				i += list2.Count - 1;
				flag = true;
			}
		}
		if (!flag)
		{
			Plugin.log.LogError((object)$"TRANSPILER: Limb PlayerCamera.main.shaker.Shake patcher: pattern was not found :(  \n{new StackTrace()}");
		}
		return list;
	}
}
