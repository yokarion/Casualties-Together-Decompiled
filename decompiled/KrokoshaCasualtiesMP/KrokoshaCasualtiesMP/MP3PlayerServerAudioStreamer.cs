using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using LiteNetLib;
using LiteNetLib.Utils;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public class MP3PlayerServerAudioStreamer : MP3PlayerAudioStreamPlayerBase
{
	public class MP3PlayerAudioReader : MonoBehaviour
	{
		public float reader_volume;

		public MP3PlayerServerAudioStreamer owner;

		private void OnAudioFilterRead(float[] data, int channels)
		{
			try
			{
				if (!owner.musicplayersource.isPlaying || owner.pcm_buffer_from_the_filter_thing.Count > Voicechat.system_framesize * 25)
				{
					return;
				}
				reader_volume = Voicechat.GetFrameMaxVol(data);
				float num = 1f / ((AudioListener.volume == 0f) ? 1f : AudioListener.volume);
				float num2 = 1f / (float)channels;
				int num3 = 0;
				float num4 = 0f;
				for (int i = 0; i < data.Length; i++)
				{
					num4 += data[i];
					num3++;
					if (num3 >= channels)
					{
						owner.pcm_buffer_from_the_filter_thing.Add(num4 * num2 * num);
						num3 = 0;
						num4 = 0f;
					}
					data[i] = 0f;
				}
			}
			catch (Exception)
			{
			}
		}
	}

	private AudioSource musicplayersource;

	public List<float> pcm_buffer_from_the_filter_thing = new List<float>(4096);

	private ushort target_samplerate = Voicechat.STANDART_FREQUENCIES.Last();

	private AudioOpusEncoding encoder;

	private Queue<float> queued_resampled_samples = new Queue<float>();

	private static bool warned_0_vol;

	public static GameObject temp_loader_mp3menu_instance;

	public Item item => ((Component)this).GetComponent<Item>();

	public static bool unprivated_MP3Menu_loadingMusic
	{
		get
		{
			return (bool)Traverse.Create(typeof(MP3Menu)).Field("loadingMusic").GetValue();
		}
		set
		{
			Traverse.Create(typeof(MP3Menu)).Field("loadingMusic").SetValue((object)value);
		}
	}

	public void CheckRequiredProcessing(AudioClip original)
	{
		target_samplerate = ((Mathf.Min(original.frequency, Voicechat.SAMPLE_RATE) > 32000) ? Voicechat.STANDART_FREQUENCIES.Last() : Voicechat.GetClosestSamplerate((ushort)original.frequency));
	}

	public void Play(AudioClip audio)
	{
		musicplayersource.Stop();
		CheckRequiredProcessing(audio);
		if (audio.channels == 0)
		{
			musicplayersource.clip = null;
			return;
		}
		musicplayersource.clip = audio;
		queued_resampled_samples.Clear();
		pcm_buffer_from_the_filter_thing.Clear();
		musicplayersource.Play();
	}

	private void LateUpdate()
	{
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		if (musicplayersource.isPlaying)
		{
			if (SharedMain.local_world_is_generating)
			{
				queued_resampled_samples.Clear();
				pcm_buffer_from_the_filter_thing.Clear();
				return;
			}
			if (!KrokoshaScavMultiplayer.rules.EnableMP3Sync)
			{
				Object.Destroy((Object)(object)this);
				return;
			}
			if (item.condition <= 0f)
			{
				musicplayersource.Stop();
				return;
			}
			if (pcm_buffer_from_the_filter_thing.Count >= Voicechat.system_framesize)
			{
				IReadOnlyList<float> data = pcm_buffer_from_the_filter_thing;
				float[] array = Voicechat.ResampleVoiceBlob(in data, AudioSettings.outputSampleRate, target_samplerate);
				pcm_buffer_from_the_filter_thing.Clear();
				float[] array2 = array;
				foreach (float num in array2)
				{
					queued_resampled_samples.Enqueue(num);
				}
			}
			int num2 = Voicechat.CalculateFrameSizeForSamplerate(target_samplerate);
			if (queued_resampled_samples.Count > num2)
			{
				float[] data2 = new float[num2];
				while (queued_resampled_samples.Count > num2)
				{
					for (int k = 0; k < data2.Length; k++)
					{
						data2[k] = queued_resampled_samples.Dequeue();
					}
					if (Voicechat.mp3_listener_volume > 0f)
					{
						ReceiveVoiceBlob(target_samplerate, in data2, true);
					}
					List<NetPlayer> playersInRadius = NetPlayer.GetPlayersInRadius(Vector2.op_Implicit(((Component)this).transform.position), base.audioSource.maxDistance);
					playersInRadius.Remove(NetPlayer.LOCAL_PLAYER);
					if (playersInRadius.Count <= 0 || !NetObjectRegistry.TryGetSyncInfoOrRegister(((Component)this).gameObject, out var si))
					{
						continue;
					}
					byte[] array3 = encoder.Encode(in data2, target_samplerate);
					if (array3.Length != 0)
					{
						byte b = (byte)Array.IndexOf(AudioOpusEncoding.STANDART_FREQUENCIES, target_samplerate);
						List<knetid> list = playersInRadius.Select((NetPlayer x) => x.clientId).ToList();
						NetDataWriter writer = Net.CreateWriter(10138);
						writer.Put((ushort)si.syncId);
						writer.Put(b);
						writer.PutBytesWithLength(array3);
						DeliveryMethod delivery = (DeliveryMethod)4;
						IEnumerable<knetid> clientIds = list;
						Net.Server_SendToClients(in delivery, in writer, in clientIds);
					}
				}
			}
		}
		if (AudioListener.volume == 0f && !warned_0_vol)
		{
			warned_0_vol = true;
			KrokoshaScavMultiplayer.DoMultiplayerStatusMessageError("SERVER: Can't stream mp3player music with 0 volume!!!!");
		}
	}

	private void OnDestroy()
	{
		if ((Object)(object)musicplayersource != (Object)null)
		{
			Object.Destroy((Object)(object)((Component)musicplayersource).gameObject);
		}
		encoder?.Dispose();
	}

	private new void Awake()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		if (KrokoshaScavMultiplayer.is_client)
		{
			Object.Destroy((Object)(object)this);
			return;
		}
		encoder = new AudioOpusEncoding();
		GameObject val = new GameObject("mp3play");
		val.AddComponent<MP3PlayerAudioReader>().owner = this;
		musicplayersource = val.AddComponent<AudioSource>();
		musicplayersource.volume = 1f;
		musicplayersource.loop = false;
		musicplayersource.minDistance = 9999999f;
		musicplayersource.maxDistance = 100000000f;
		musicplayersource.rolloffMode = (AudioRolloffMode)1;
		musicplayersource.bypassEffects = false;
		musicplayersource.bypassListenerEffects = true;
		musicplayersource.bypassReverbZones = true;
		musicplayersource.dopplerLevel = 0f;
		musicplayersource.spatialBlend = 0f;
		base.Awake();
	}

	internal static void Server_SendMusicList(IReadOnlyList<NetPlayer> targets = null)
	{
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		if (targets == null)
		{
			targets = ServerMain.AllPlayersExceptHost.ToList();
		}
		List<knetid> list = targets.Select((NetPlayer x) => x.clientId).ToList();
		NetDataWriter writer = Net.CreateWriter(10139);
		writer.Put(MP3Menu.dropdownList.Count);
		foreach (OptionData dropdown in MP3Menu.dropdownList)
		{
			writer.Put(dropdown.text);
		}
		writer.CompressWriter();
		DeliveryMethod delivery = (DeliveryMethod)2;
		IEnumerable<knetid> clientIds = list;
		Net.Server_SendToClients(in delivery, in writer, in clientIds);
	}

	public static void Server_ForceLoadMusic()
	{
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		if (!unprivated_MP3Menu_loadingMusic)
		{
			MP3Menu.dropdownList = null;
			PlayerCamera main = PlayerCamera.main;
			object obj;
			if (main == null)
			{
				obj = null;
			}
			else
			{
				Canvas mainCanvas = main.mainCanvas;
				obj = ((mainCanvas != null) ? ((Component)mainCanvas).transform : null);
			}
			if (obj == null)
			{
				obj = Plugin.s.transform;
			}
			temp_loader_mp3menu_instance = Utils.Create("Special/MP3SongSelect", (Transform)obj);
			((Behaviour)temp_loader_mp3menu_instance.GetComponent<MP3Menu>().dropdown).enabled = false;
			temp_loader_mp3menu_instance.transform.position = new Vector3(99999f, 99999f, 99999f);
			temp_loader_mp3menu_instance.transform.localScale = Vector3.one * 0.001f;
			Object.Destroy((Object)(object)temp_loader_mp3menu_instance, 120f);
		}
	}

	[ServerReceiver(10136)]
	private static void Server_MP3RequestMusicList(knetid cid, ref NetDataReader reader)
	{
		int num = default(int);
		reader.Get(ref num);
		if (NetPlayer.TryGetPlayerFromClientId(cid, out var plr))
		{
			if (MP3Menu.dropdownList == null)
			{
				Server_ForceLoadMusic();
				return;
			}
			Server_SendMusicList(new List<NetPlayer> { plr });
		}
	}

	[ServerReceiver(10137)]
	private static void Server_MP3PlayCustomMusic(knetid cid, ref NetDataReader reader)
	{
		int num = default(int);
		reader.Get(ref num);
		reader.Get(out knetid result);
		if (NetPlayer.TryGetPlayerFromClientId(cid, out var plr) && plr.IsConscious() && NetObjectRegistry.TryGetSyncInfo(result, out var si) && si.IsItem() && si.item.id == "mp3player" && ItemSync.CheckIfBodyReachThisItem(si, plr.body) && num < MP3Menu.clips.Count)
		{
			MP3Menu_Play_MultiplayerPatch.Server_PlayThisSongOnThisMp3Player(MP3Menu.clips[num], si.item);
		}
	}

	[ClientReceiver(10139, true)]
	private static void Client_MP3MenuServerIsSendingMusicList(knetid _, ref NetDataReader reader)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected O, but got Unknown
		log.l("CLIENT: Received Music list.");
		reader = reader.DecompressReader();
		int num = default(int);
		reader.Get(ref num);
		MP3Menu.dropdownList = new List<OptionData>();
		string text = default(string);
		for (int i = 0; i < num; i++)
		{
			reader.Get(ref text);
			MP3Menu.dropdownList.Add(new OptionData(text));
		}
		MP3Menu val = Object.FindObjectOfType<MP3Menu>();
		if ((Object)(object)val != (Object)null)
		{
			val.dropdown.ClearOptions();
			val.dropdown.AddOptions(MP3Menu.dropdownList);
		}
	}

	[ClientReceiver(10138, true)]
	private static void Client_StreamedAudioToClients(knetid _, ref NetDataReader reader)
	{
		if (Voicechat.mp3_listener_volume <= 0f || SharedMain.local_world_is_generating)
		{
			return;
		}
		reader.Get(out knetid result);
		if (NetObjectRegistry.TryGetSyncInfo(result, out var si))
		{
			GameObject go = si.go;
			byte b = default(byte);
			reader.Get(ref b);
			ushort num = AudioOpusEncoding.STANDART_FREQUENCIES[b];
			reader.Get(out byte[] result2);
			MP3PlayerAudioStreamPlayer_ClientOnly orAddComponent = ComponentHolderProtocol.GetOrAddComponent<MP3PlayerAudioStreamPlayer_ClientOnly>((Object)(object)go);
			if (orAddComponent.OpusDecode(num, result2, out var voicedata))
			{
				orAddComponent.ReceiveVoiceBlob(num, voicedata.ToArray(), true);
			}
		}
	}
}
