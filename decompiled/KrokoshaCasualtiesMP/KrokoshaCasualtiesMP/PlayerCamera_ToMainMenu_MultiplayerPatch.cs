using System;
using System.Collections.Generic;
using HarmonyLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(PlayerCamera), "ToMainMenu")]
internal static class PlayerCamera_ToMainMenu_MultiplayerPatch
{
	private static void Prefix(WorldGeneration __instance)
	{
		if (NewCoolerObjectPacketWriteReadSystem.inst != null)
		{
			NewCoolerObjectPacketWriteReadSystem.inst.UnregisterEVERYTHING();
		}
		if ((Object)(object)CoolSyncManager.inst != (Object)null)
		{
			CoolSyncManager.inst.ResetResettableSystems();
		}
		Con.UnNoclip();
		if (KrokoshaScavMultiplayer.network_system_is_running)
		{
			UIInGame.StopSpectatorMode();
			if (KrokoshaScavMultiplayer.is_client)
			{
				if (!KrokoshaScavMultiplayer.showMultiplayerMenu)
				{
					KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog("Disconnecting because im going to main menu on my own!");
					KrokoshaScavMultiplayer._JustDisconnect();
					Chat.LogMessage("*SYSTEM*", "Left the server.", false);
				}
			}
			else
			{
				KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog("Announcing GoBackToMainMenu.");
				NetDataWriter writer = Net.CreateWriter((Enum)NetmsgId.CLIENT_GoBackToMainMenu);
				writer.Put(0);
				Net.Server_SendToClientsVeryReliable(in writer, (IEnumerable<knetid>)ServerMain.AllClientIdsExceptHost);
				Chat.Server_ChatAnnouncement("Going back to main menu.");
			}
		}
		KrokoshaScavMultiplayer.showMultiplayerMenu = true;
	}
}
