using HarmonyLib;
using LiteNetLib;
using LiteNetLib.Utils;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(MP3Menu), "Play")]
public static class MP3Menu_Play_MultiplayerPatch
{
	public static void Server_PlayThisSongOnThisMp3Player(AudioClip audio, Item mp3player)
	{
		ComponentHolderProtocol.GetOrAddComponent<MP3PlayerServerAudioStreamer>((Object)(object)mp3player).Play(audio);
	}

	public static bool Prefix(MP3Menu __instance)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		if (!KrokoshaScavMultiplayer.network_system_is_running || !KrokoshaScavMultiplayer.rules.EnableMP3Sync)
		{
			return true;
		}
		if ((Object)(object)MP3Menu_UpdateList_MultiplayerPatch.last_locally_used_mp3_player != (Object)null)
		{
			int value = __instance.dropdown.value;
			if (KrokoshaScavMultiplayer.is_client)
			{
				if (NetObjectRegistry.TryGetSyncInfoOrRegister((Component)(object)MP3Menu_UpdateList_MultiplayerPatch.last_locally_used_mp3_player, out var si))
				{
					NetDataWriter writer = Net.CreateWriter(10137);
					writer.Put(value);
					writer.Put((ushort)si.syncId);
					Net.Client_Send((DeliveryMethod)0, in writer);
				}
				else
				{
					NetObjectRegistry.AlertObjectNotRegistered(popup: true);
				}
			}
			else
			{
				Server_PlayThisSongOnThisMp3Player(MP3Menu.clips[value], MP3Menu_UpdateList_MultiplayerPatch.last_locally_used_mp3_player);
			}
			return false;
		}
		KrokoshaScavMultiplayer.DoMultiplayerStatusMessageError("MP3Menu.Play -> Failed to play a song? Unknown mp3player item?????");
		return true;
	}
}
