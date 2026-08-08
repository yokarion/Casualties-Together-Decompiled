using System;
using System.Collections.Generic;
using HarmonyLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(WorldGeneration), "RegenerateWorld")]
public static class RegenerateWorldPatch
{
	public static void Prefix(WorldGeneration __instance)
	{
		try
		{
			Con.UnNoclip();
			if (KrokoshaScavMultiplayer.network_system_is_running)
			{
				if (!KrokoshaScavMultiplayer.is_client)
				{
					NetDataWriter writer = Net.CreateWriter((Enum)NetmsgId.CLIENT_Announce_RegenerateWorld);
					writer.Put(__instance.doPod ? ((ushort)1) : ((ushort)0));
					Net.Server_SendToClientsVeryReliable(in writer, (IEnumerable<knetid>)ServerMain.AllClientIdsExceptHost);
					KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog("Announced RegenerateWorld.");
					if (!KrokoshaScavMultiplayer.rules.RespawnKeepInventory)
					{
						foreach (KeyValuePair<Body, NetPlayer> item in NetPlayer.BodyToPlayerDict)
						{
							if (!item.Key.alive)
							{
								item.Value.Server_DropAllInventory();
							}
						}
					}
				}
				else
				{
					ClientMain.server_is_generating_world = true;
				}
			}
			NewCoolerObjectPacketWriteReadSystem.inst.UnregisterEVERYTHING(but_dont_unregister_inventory_items: true);
			SharedMain.LastWorldgenFinishTime = Time.realtimeSinceStartupAsDouble;
			foreach (NetPlayer value in NetPlayer.ClientIdToPlayerDict.Values)
			{
				if (value.server_plrstate != null)
				{
					value.server_plrstate.did_give_spawn_location = false;
					value.server_plrstate.is_loaded_in = false;
				}
				value.ResetEntropy();
				value.levelPlayTime = 0.0;
			}
		}
		catch (Exception ex)
		{
			log.error(ex.ToString());
		}
	}
}
