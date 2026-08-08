using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using LiteNetLib.Utils;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(TraderScript), "TryHaggle")]
public static class TraderScript_TryHaggle_MultiplayerPatch
{
	private static bool force;

	private static bool Prefix(TraderScript __instance, ref float __state)
	{
		__state = __instance.reputation;
		if (KrokoshaScavMultiplayer.network_system_is_running && !force && (Object)(object)PlayerCamera.main.currentTrader == (Object)(object)__instance)
		{
			KrokoshaTraderTrackerComponent orAddComponent = ComponentHolderProtocol.GetOrAddComponent<KrokoshaTraderTrackerComponent>((Object)(object)__instance);
			orAddComponent.focused_body = PlayerCamera.main.body;
			if (orAddComponent.is_registered && KrokoshaScavMultiplayer.is_client)
			{
				if (log.verbose)
				{
					log.l("CLIENT: Requesting TraderTryHaggle");
				}
				KrokoshaScavMultiplayer.Client_SendSimpleMessageToServer((ushort)10161, (ushort)orAddComponent.si.syncId, true);
			}
		}
		force = false;
		return true;
	}

	private static void Postfix(TraderScript __instance, ref float __state)
	{
		_ = __instance.reputation;
		if (KrokoshaScavMultiplayer.network_system_is_running)
		{
			ComponentHolderProtocol.GetOrAddComponent<KrokoshaTraderTrackerComponent>((Object)(object)__instance).Server_AnnounceTraderReputationState((IReadOnlyList<knetid>)null);
		}
	}

	[ServerReceiver(10161)]
	private static void Server_TraderTryHaggle(knetid terroristid, ref NetDataReader reader)
	{
		reader.Get(out knetid result);
		if (TraderSync.Server_TraderInteractionCheck(terroristid, result, "TryHaggle", out var plr, out var trader_si, out var trader_tracker))
		{
			trader_tracker.focused_body = plr.body;
			force = true;
			log.l($"TryHaggle  {plr}");
			trader_si.trader.TryHaggle();
			force = false;
		}
	}

	public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
	{
		List<CodeInstruction> instructionList = instructions.ToList();
		int i = 0;
		bool didit = false;
		while (i < instructionList.Count)
		{
			CodeInstruction val = instructionList[i];
			if (val.opcode == OpCodes.Call && val.operand is MethodInfo methodInfo && methodInfo != null && methodInfo.Name == "get_body" && methodInfo.DeclaringType == typeof(TraderScript))
			{
				MethodInfo methodInfo2 = typeof(Component).GetMethod("GetComponent", Type.EmptyTypes).MakeGenericMethod(typeof(KrokoshaTraderTrackerComponent));
				MethodInfo getTheOtherPartMethod = typeof(KrokoshaTraderTrackerComponent).GetMethod("GetBody", BindingFlags.Instance | BindingFlags.Public);
				yield return new CodeInstruction(OpCodes.Call, (object)methodInfo2);
				yield return new CodeInstruction(OpCodes.Callvirt, (object)getTheOtherPartMethod);
				i++;
				didit = true;
			}
			else
			{
				yield return instructionList[i];
				i++;
			}
		}
		if (!didit)
		{
			Plugin.log.LogError((object)$"TraderScript_TryHaggle_MultiplayerPatch: Failed to patch the body getters {new StackTrace()}");
		}
	}
}
