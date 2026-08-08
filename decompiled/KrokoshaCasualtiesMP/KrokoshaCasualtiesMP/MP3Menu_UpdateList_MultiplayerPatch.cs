using HarmonyLib;
using LiteNetLib;
using LiteNetLib.Utils;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(MP3Menu), "UpdateList")]
public static class MP3Menu_UpdateList_MultiplayerPatch
{
	public static Item last_locally_used_mp3_player;

	public static bool Prefix(MP3Menu __instance)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		if (!KrokoshaScavMultiplayer.network_system_is_running)
		{
			return true;
		}
		if (KrokoshaScavMultiplayer.is_server)
		{
			return true;
		}
		if (!KrokoshaScavMultiplayer.rules.EnableMP3Sync)
		{
			return true;
		}
		if (MP3Menu.dropdownList == null)
		{
			KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog("CLIENT: Requesting server for the music list.");
			NetDataWriter writer = Net.CreateWriter(10136);
			writer.Put(1);
			Net.Client_Send((DeliveryMethod)0, in writer);
			return true;
		}
		return false;
	}
}
