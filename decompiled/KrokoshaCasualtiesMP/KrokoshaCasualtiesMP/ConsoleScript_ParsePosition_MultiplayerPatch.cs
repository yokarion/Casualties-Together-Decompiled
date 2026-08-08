using System;
using HarmonyLib;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(ConsoleScript), "ParsePosition")]
public static class ConsoleScript_ParsePosition_MultiplayerPatch
{
	public static bool Prefix(ConsoleScript __instance, string s, ref Vector2 __result)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (Con.TryParsePlayerPositionGetName(s, out var name))
		{
			NetBody netBody = ServerMain.RelaxedGetBodyForCommand(name, allow_macros: true, only_players: false, NetPlayer.GetLocalNetBodyNullable());
			if ((Object)(object)netBody != (Object)null)
			{
				__result = Vector2.op_Implicit(((Component)netBody).transform.position);
				return false;
			}
			throw new Exception("\"" + s + "\" player not found.");
		}
		return true;
	}
}
