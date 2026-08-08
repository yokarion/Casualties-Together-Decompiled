using System.Collections;
using System.Collections.Generic;
using HarmonyLib;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(PreRunScript), "WaitLoad")]
public static class ScavGameStartPatch
{
	public static bool starting_game;

	private static bool Prefix(PreRunScript __instance, ref IEnumerator __result)
	{
		if (KrokoshaScavMultiplayer.network_system_is_running)
		{
			if (starting_game)
			{
				KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog("Client is starting game by request of server.");
				starting_game = false;
				return true;
			}
			if (KrokoshaScavMultiplayer.is_client)
			{
				KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog("You can't start the game, server can.");
				starting_game = false;
				__result = new List<object>().GetEnumerator();
				return false;
			}
			starting_game = true;
			__instance.UpdateAllSettingDisplays();
			ServerMain.Server_Announce_GAME_START();
			KrokoshaScavMultiplayer.showMultiplayerMenu = false;
		}
		else
		{
			KrokoshaScavMultiplayer.showMultiplayerMenu = false;
		}
		starting_game = false;
		return true;
	}
}
