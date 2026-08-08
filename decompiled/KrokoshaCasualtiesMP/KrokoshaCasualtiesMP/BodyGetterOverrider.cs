using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public class BodyGetterOverrider : MonoBehaviour
{
	public Body body = PlayerCamera.main.body;

	private void Start()
	{
		TrySetBodyFromParent();
	}

	public void SetBody(Body b)
	{
		body = b;
		BodyGetterOverrider[] componentsInChildren = ((Component)this).GetComponentsInChildren<BodyGetterOverrider>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].body = b;
		}
	}

	public Body GetBody()
	{
		if ((Object)(object)body == (Object)null)
		{
			if (TryGetBodyFromParent())
			{
				return body;
			}
			return PlayerCamera.main.body;
		}
		return body;
	}

	public void TrySetBodyFromParent()
	{
		Body componentInParent = ((Component)this).GetComponentInParent<Body>();
		if ((Object)(object)componentInParent != (Object)null)
		{
			body = componentInParent;
		}
	}

	public bool TryGetBodyFromParent()
	{
		Body componentInParent = ((Component)this).GetComponentInParent<Body>();
		if ((Object)(object)componentInParent != (Object)null)
		{
			body = componentInParent;
			return true;
		}
		return false;
	}

	public static IEnumerable<CodeInstruction> TranspilerToOverrideBodyGetters(Type declaringtype, IEnumerable<CodeInstruction> instructions)
	{
		Type bodygetteroverrider_type = typeof(BodyGetterOverrider);
		List<CodeInstruction> instructionList = instructions.ToList();
		int i = 0;
		bool didit = false;
		while (i < instructionList.Count)
		{
			CodeInstruction val = instructionList[i];
			if (val.opcode == OpCodes.Call && val.operand is MethodInfo methodInfo && methodInfo != null && methodInfo.Name == "get_body" && methodInfo.DeclaringType == declaringtype)
			{
				MethodInfo methodInfo2 = typeof(ComponentHolderProtocol).GetMethod("GetOrAddComponent", BindingFlags.Static | BindingFlags.Public).MakeGenericMethod(bodygetteroverrider_type);
				MethodInfo getTheOtherPartMethod = bodygetteroverrider_type.GetMethod("GetBody", BindingFlags.Instance | BindingFlags.Public);
				yield return new CodeInstruction(OpCodes.Call, (object)methodInfo2);
				yield return new CodeInstruction(OpCodes.Callvirt, (object)getTheOtherPartMethod);
				i++;
				didit = true;
				if (log.verbose)
				{
					Plugin.log.LogInfo((object)$"TranspilerToOverrideBodyGetters: Succesfully patched the body getter {i} ");
				}
			}
			else
			{
				yield return instructionList[i];
				i++;
			}
		}
		if (!didit)
		{
			Plugin.log.LogError((object)$"TranspilerToOverrideBodyGetters: Failed to patch the body getters {new StackTrace()}");
		}
	}

	public static IEnumerable<CodeInstruction> TranspilerToOverridePlayerCameraMainBody(IEnumerable<CodeInstruction> instructions)
	{
		Type bodygetteroverrider_type = typeof(BodyGetterOverrider);
		List<CodeInstruction> instructionList = instructions.ToList();
		int i = 0;
		bool didit = false;
		while (i < instructionList.Count)
		{
			CodeInstruction val = instructionList[i];
			if (i < instructionList.Count - 1)
			{
				CodeInstruction val2 = instructionList[i + 1];
				if (val.opcode == OpCodes.Ldsfld && val.operand is FieldInfo fieldInfo && fieldInfo == AccessTools.Field(typeof(PlayerCamera), "main") && val2.opcode == OpCodes.Ldfld && val2.operand is FieldInfo fieldInfo2 && fieldInfo2 == AccessTools.Field(typeof(PlayerCamera), "body"))
				{
					MethodInfo method = typeof(ComponentHolderProtocol).GetMethod("GetOrAddComponent", BindingFlags.Static | BindingFlags.Public);
					MethodInfo getComponentGeneric = method.MakeGenericMethod(bodygetteroverrider_type);
					MethodInfo getTheOtherPartMethod = bodygetteroverrider_type.GetMethod("GetBody", BindingFlags.Instance | BindingFlags.Public);
					yield return new CodeInstruction(OpCodes.Ldarg_0, (object)null);
					yield return new CodeInstruction(OpCodes.Call, (object)getComponentGeneric);
					yield return new CodeInstruction(OpCodes.Callvirt, (object)getTheOtherPartMethod);
					i++;
					didit = true;
					if (log.verbose)
					{
						Plugin.log.LogWarning((object)$"TRANSPILER: PlayerCameraMainBody overrider: Succesfully patched the body getter {i} ");
					}
					continue;
				}
			}
			yield return instructionList[i];
			i++;
		}
		if (!didit)
		{
			Plugin.log.LogError((object)$"TRANSPILER: PlayerCameraMainBody overrider: Failed to patch the body getters {new StackTrace()}");
		}
	}
}
