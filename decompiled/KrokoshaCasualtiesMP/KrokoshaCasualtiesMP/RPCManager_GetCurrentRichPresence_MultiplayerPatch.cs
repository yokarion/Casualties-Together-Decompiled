using System.Runtime.CompilerServices;
using DiscordRPC;
using HarmonyLib;
using KrokoshaCasualtiesUtils;
using Steamworks;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(RPCManager), "GetCurrentRichPresence")]
internal static class RPCManager_GetCurrentRichPresence_MultiplayerPatch
{
	private struct dffdsfdsfdssfs
	{
		public bool aaaaaaaaaaaaaaaa;

		public bool bbbbbbbbbbbbbbbb;
	}

	private static double ffdfdfdsfsdfdsfdsfdssfdfsd;

	private static void Prefix(RPCManager __instance, ref dffdsfdsfdssfs __state)
	{
	}

	private static void Postfix(RPCManager __instance, ref dffdsfdsfdssfs __state, ref RichPresence __result)
	{
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Expected O, but got Unknown
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Expected O, but got Unknown
		TransportSteamworks tsteam;
		bool flag = Net.TryGetSteamTransport(out tsteam);
		bool flag2 = KSteam.CURRENT_LOBBY.locked || Net.cur_server_info.midjoin_lock_active;
		bool flag3 = flag;
		if (flag3 && Net.is_client)
		{
			flag3 = !KSteam.CURRENT_LOBBY.locked && !Net.cur_server_info.midjoin_lock_active;
		}
		if (!UIBullshit.DISCORD_ALLOWJOINBUTTON)
		{
			flag3 = false;
		}
		if (flag3)
		{
			string joinSecret = ((object)Unsafe.As<CSteamID, CSteamID>(ref KSteam.CURRENT_LOBBY.lobby_steamID)/*cast due to constrained. prefix*/).ToString() + ":" + KrokoshaScavMultiplayer.Server_GetJoinSecret();
			((BaseRichPresence)__result).Secrets = new Secrets
			{
				JoinSecret = joinSecret
			};
		}
		else
		{
			flag2 = true;
		}
		if (UIBullshit.DISCORD_ONLYASKTOJOINBUTTON)
		{
			flag2 = true;
		}
		if (!Net.running)
		{
			return;
		}
		if (string.IsNullOrEmpty(((BaseRichPresence)__result).State))
		{
			if (NetPlayer.ClientIdToPlayerDict.Count <= 1)
			{
				((BaseRichPresence)__result).State = "Waiting for players...";
			}
			else if (!Util.IsInMainMenu())
			{
				((BaseRichPresence)__result).State = "Playing Multiplayer";
			}
		}
		double realtimeSinceStartupAsDouble = Time.realtimeSinceStartupAsDouble;
		double num = realtimeSinceStartupAsDouble - ffdfdfdsfsdfdsfdsfdssfdfsd;
		if (Util.IsInMainMenu())
		{
			string text = (Net.is_server ? "Hosting server" : "In Lobby");
			((BaseRichPresence)__result).Details = text ?? "";
			if (num > 20.0)
			{
				ffdfdfdsfsdfdsfdsfdssfdfsd = realtimeSinceStartupAsDouble;
			}
		}
		else
		{
			string details = ((BaseRichPresence)__result).Details;
			((BaseRichPresence)__result).Details = ((BaseRichPresence)__result).State;
			((BaseRichPresence)__result).State = details;
		}
		((BaseRichPresence)__result).Party = new Party
		{
			ID = (flag ? ("steam_" + KSteam.CURRENT_LOBBY.lobby_steamID.m_SteamID.ToString("X")) : ("directconnect_" + Net.cur_server_info.name.StableHash().ToString("X"))),
			Size = NetPlayer.ClientIdToPlayerDict.Count,
			Max = Net.cur_server_info.plr_max,
			Privacy = (PrivacySetting)(!flag2)
		};
		((BaseRichPresence)__result).Timestamps = RPCManager_Initialize_MultiplayerPatch.rpcStartTime;
	}
}
