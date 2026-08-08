using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using KrokoshaCasualtiesUtils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(Body), "TryLastStand")]
public static class Body_TryLastStand_MultiplayerPatch
{
	private static bool force;

	public static void ForceLastStand(this Body b)
	{
		force = true;
		b.lastHappiness[9] = 100f;
		b.TryLastStand();
		force = false;
	}

	private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
	{
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Expected O, but got Unknown
		List<CodeInstruction> list = new List<CodeInstruction>(instructions);
		bool flag = false;
		for (int i = 0; i < list.Count - 6; i++)
		{
			if (list[i].opcode == OpCodes.Ldsfld && list[i + 1].opcode == OpCodes.Ldsfld && list[i + 2].opcode == OpCodes.Callvirt && list[i + 2].operand is MethodInfo { Name: "LastStandSequence" } && list[i + 3].opcode == OpCodes.Callvirt && list[i + 3].operand is MethodInfo { Name: "StartCoroutine" } && list[i + 4].opcode == OpCodes.Pop)
			{
				CodeInstruction val = new CodeInstruction(OpCodes.Nop, (object)null);
				CodeInstructionExtensions.MoveLabelsFrom(val, list[i]);
				list.RemoveRange(i, 5);
				list.Insert(i, val);
				i++;
				flag = true;
			}
		}
		if (!flag)
		{
			Plugin.log.LogError((object)"Body_TryLastStand_MultiplayerPatch transpiler: Not a single pattern was found :(  ");
		}
		return list;
	}

	private static bool Prefix(Body __instance, ref bool __state)
	{
		__state = false;
		if (!__instance.succesfullyRolledLastStand && Util.IsBodyLocal(__instance))
		{
			__state = true;
		}
		if (KrokoshaScavMultiplayer.network_system_is_running)
		{
			if (!KrokoshaScavMultiplayer.rules.LastStandAllowed)
			{
				return false;
			}
			if (!KrokoshaScavMultiplayer.IsNetworkActiveAndIsServer() && !force)
			{
				return false;
			}
			CoUtils_instance_MultiplayerPatch.cur_override_instance = __instance.GetCoUtilsInstance();
		}
		force = false;
		return true;
	}

	private static void Postfix(Body __instance, ref bool __state)
	{
		CoUtils_instance_MultiplayerPatch.cur_override_instance = null;
		NetPlayer plr;
		if (__state && __instance.succesfullyRolledLastStand)
		{
			if (__instance.succesfullyRolledLastStand && KrokoshaScavMultiplayer.network_system_is_running)
			{
				Plugin.log.LogInfo((object)(NetPlayer.LOCAL_PLAYER.playername + ", rolled Last Stand succesfully. "));
			}
			else
			{
				Plugin.log.LogInfo((object)$"Server host character rolled Last stand. outcome: {__instance.succesfullyRolledLastStand} ");
			}
			((MonoBehaviour)PlayerCamera.main).StartCoroutine(PlayerCamera.main.LastStandSequence());
		}
		else if (KrokoshaScavMultiplayer.IsNetworkActiveAndIsServer() && __instance.TryGetNetPlayer(out plr))
		{
			if (__instance.succesfullyRolledLastStand)
			{
				Plugin.log.LogInfo((object)(plr.playername + ", rolled Last Stand succesfully "));
			}
			else
			{
				Plugin.log.LogInfo((object)$"Server rolled Last stand for {plr.playername} outcome: {__instance.succesfullyRolledLastStand} ");
			}
			MedicalSync.Server_SendCharacterHealth(plr.playerbody);
		}
	}
}
