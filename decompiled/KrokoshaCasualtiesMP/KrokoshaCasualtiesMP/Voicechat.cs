using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using KrokoshaCasualtiesUtils;
using LiteNetLib;
using LiteNetLib.Utils;
using OpusSharp.Core.Extensions;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public class Voicechat : KrokoshaScavSingleton
{
	public const int DEFAULT_MAX_SAMPLE_RATE = 48000;

	public const int DEFAULT_SAMPLE_RATE = 24000;

	public const int DEFAULT_MIN_SAMPLE_RATE = 8000;

	public const int DEFAULT_FRAME_DURATION_MS = 40;

	private static AudioClip mic_clip;

	public static int SAMPLE_RATE = 24000;

	private static int last_mic_pos = 0;

	public const string NOMIC = "NO_MICROPHONE_SELECTED";

	internal const string _PREF_MICNAME = "KrokoshaMultiplayer_MicrophoneName";

	[SettingDeclarerThingyFloat(0f, 2f, "setting_vcattenuation")]
	public static float bg_music_attenuation = 1f;

	[SettingDeclarerThingyFloat(0f, 2f, "setting_mp3vol")]
	public static float mp3_listener_volume = 1f;

	[SettingDeclarerThingyFloat(0f, 2f, "setting_vclistenvol")]
	public static float my_listener_volume = 1f;

	[SettingDeclarerThingyFloat(0f, 2f, "setting_micvol")]
	public static float my_volume = 1f;

	[SettingDeclarerThingyChoice("setting_micmode", new string[] { "setting_micmode_choice_off", "setting_micmode_choice_ptt", "setting_micmode_choice_on", "setting_micmode_choice_toggle" })]
	public static byte current_microphone_mode = 1;

	public static float MINIMUM_VOLUME_TRESHOLD = 0.05f;

	public static float MINIMUM_VOLUME_TRESHOLD_TIME = 0.5f;

	public static float AlwaysOn_TimeBelowTreshold = 0f;

	public static float my_output_volume = 0f;

	public static bool listener_is_mindwiped = false;

	private static Queue<float> my_mic_data_buffer = new Queue<float>();

	private static AudioOpusEncoding encoder;

	internal static bool ENABLE_LOCAL_LOOPBACK = false;

	internal static VoicechatOutputDebugVisualiser ENABLE_LOCAL_LOOPBACK_VCINSTANCE = null;

	internal static int TEMPDEV_LAST_OPUS_ENCODED_SIZE = 0;

	internal static int TEMPDEV_LAST_OPUS_BEFORE_ENCODED_SIZE = 0;

	private static bool _toggle_to_talk_toggled = false;

	private static bool _push_to_talk_pushed = false;

	public static float smooth_bg_music_volume = 1f;

	public static float smooth_bg_music_volume_target = 1f;

	public static int system_framesize = 48000;

	private static bool _did_mindwipe_alert = false;

	private static bool _did_consciousness_muted_alert = false;

	private const int MAX_FAILURES_UNTIL_DISABLE_VC = 5;

	private static int START_FAILURE_COUNT = 0;

	private static float _complexityupdatetimer = 0f;

	public static Voicechat singleton { get; private set; }

	public static ushort[] STANDART_FREQUENCIES => AudioOpusEncoding.STANDART_FREQUENCIES;

	public static float DEFAULT_FRAME_DURATION_S => 0.04f;

	public static int FRAME_SIZE => CalculateFrameSizeForSamplerate(SAMPLE_RATE);

	public static bool IS_RECORDING { get; protected set; }

	internal static string _PREF_MICMODE
	{
		get
		{
			FieldInfo field = typeof(Voicechat).GetField("current_microphone_mode");
			return AttributeUtility.GetAttribute<SettingDeclarerThingyChoiceAttribute>((MemberInfo)field, true).GetPrefKey(field);
		}
	}

	public static string current_microphone { get; protected set; }

	public static bool VCRULE_hearingloss => KrokoshaScavMultiplayer.rules.HearingLossChat;

	public static bool VCRULE_speechimpaired => KrokoshaScavMultiplayer.rules.SpeechImpairedChat;

	public static bool VCRULE_mindwiperule => KrokoshaScavMultiplayer.rules.MindwipeDisablesChat;

	public static bool VCRULE_deadchat => KrokoshaScavMultiplayer.rules.CanCommunicateWithTheDeadVC();

	public static bool VCRULE_enabled => KrokoshaScavMultiplayer.rules.VoicechatEnabled;

	public static float MAX_HEAR_EFFECT_PROCESSING_DISTANCE => MAX_HEAR_DISTANCE * 1.3f;

	public static float MAX_HEAR_DISTANCE => Util.MetersToTiles(KrokoshaScavMultiplayer.rules.ProximityHearDistance);

	public static int CalculateFrameSizeForSamplerate(int sr)
	{
		return sr / 1000 * 40;
	}

	public static bool IsMicModeAlwaysOn()
	{
		return current_microphone_mode == 2;
	}

	public static bool IsMicModePushToTalk()
	{
		return current_microphone_mode == 1;
	}

	public static bool IsMicModeToggleToTalk()
	{
		return current_microphone_mode == 3;
	}

	public static bool IsMicModeDisabled()
	{
		return current_microphone_mode == 0;
	}

	public static bool IsPushToTalkKeyPushed()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		return Input.GetKey(KeyBinds.GetBind("krokosha_coop_voicechat"));
	}

	private void KrokoshaSingletonEvent_OnSceneChange()
	{
		ENABLE_LOCAL_LOOPBACK = false;
		KrokoshaSingletonEvent_OnRulesUpdate();
	}

	private void KrokoshaSingletonEvent_OnRulesUpdate()
	{
		StreamedAudioOutput[] array = Object.FindObjectsOfType<StreamedAudioOutput>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].OnHearabilityRuleUpdate();
		}
	}

	public static ushort GetClosestSamplerate(ushort my)
	{
		ushort result = STANDART_FREQUENCIES[0];
		ushort[] sTANDART_FREQUENCIES = STANDART_FREQUENCIES;
		foreach (ushort num in sTANDART_FREQUENCIES)
		{
			if (my < num)
			{
				return result;
			}
			result = num;
		}
		return result;
	}

	public static ushort GetClosestNumber(ushort my, ushort[] ushorts)
	{
		ushort result = ushorts[0];
		foreach (ushort num in ushorts)
		{
			if (my < num)
			{
				return result;
			}
			result = num;
		}
		return result;
	}

	private void Awake()
	{
		if (current_microphone == null)
		{
			current_microphone = "NO_MICROPHONE_SELECTED";
		}
		encoder = new AudioOpusEncoding();
		singleton = this;
	}

	private void Start()
	{
		current_microphone = SettingsJsonThing.json.GetString("KrokoshaMultiplayer_MicrophoneName", "NO_MICROPHONE_SELECTED");
		if (current_microphone == "NO_MICROPHONE_SELECTED")
		{
			Plugin.log.LogInfo((object)" VOICECHAT: Start(): Attempting to select default microphone. ");
		}
		StartRecording(current_microphone);
		if (!IS_RECORDING && Microphone.devices.Length != 0)
		{
			StartRecording(Microphone.devices[0]);
		}
	}

	private static void CopyMicSamplesIntoBufferAndApplyVolume(ref float[] samples, ref int i, int until)
	{
		do
		{
			float item = samples[i] * my_volume;
			i++;
			my_mic_data_buffer.Enqueue(item);
		}
		while (i < until);
	}

	public static bool VoiceChatIsEnabledLocallyOnly()
	{
		if (Util.IsGeneratingWorld())
		{
			return false;
		}
		if (my_volume > 0f)
		{
			return !IsMicModeDisabled();
		}
		return false;
	}

	public static bool VoiceChatIsEnabled()
	{
		if (!VoiceChatIsEnabledLocallyOnly())
		{
			return false;
		}
		if (VCRULE_enabled || !KrokoshaScavMultiplayer.network_system_is_running)
		{
			return !KrokoshaScavMultiplayer.is_dedicated_server;
		}
		return false;
	}

	private static bool RecordMicUpdate()
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if (!VoiceChatIsEnabled())
		{
			_push_to_talk_pushed = false;
			_toggle_to_talk_toggled = false;
			return false;
		}
		if (!ENABLE_LOCAL_LOOPBACK)
		{
			if (IsMicModePushToTalk())
			{
				if (IsPushToTalkKeyPushed())
				{
					_push_to_talk_pushed = true;
				}
				if (!_push_to_talk_pushed)
				{
					return false;
				}
			}
			else if (IsMicModeToggleToTalk())
			{
				if (Input.GetKeyDown(KeyBinds.GetBind("krokosha_coop_voicechat")))
				{
					_toggle_to_talk_toggled = !_toggle_to_talk_toggled;
				}
				if (_toggle_to_talk_toggled)
				{
					_push_to_talk_pushed = true;
				}
				if (!_push_to_talk_pushed)
				{
					return false;
				}
			}
			else if (current_microphone_mode != 2)
			{
				return false;
			}
			NetPlayer lOCAL_PLAYER = NetPlayer.LOCAL_PLAYER;
			if ((Object)(object)lOCAL_PLAYER != (Object)null && VCRULE_speechimpaired)
			{
				if (VCRULE_mindwiperule && lOCAL_PLAYER.IsAliveAndMindwiped())
				{
					if (!_did_mindwipe_alert)
					{
						_push_to_talk_pushed = false;
						_did_mindwipe_alert = true;
						Util.DoAlert(Lang.Get("voicechat_deny_mindwipe", false), false);
						log.l("VOICECHAT: Can't speak = mindwiped");
					}
					return false;
				}
				if (CheckIfPlayerIsUnconsciousToMute(lOCAL_PLAYER))
				{
					if (!_did_consciousness_muted_alert)
					{
						_push_to_talk_pushed = false;
						_did_consciousness_muted_alert = true;
						Util.DoAlert(Lang.Get("voicechat_deny_unconscious", false), false);
						log.l("VOICECHAT: Can't speak = unconscious");
					}
					return false;
				}
			}
		}
		if (!IS_RECORDING || mic_clip.frequency != SAMPLE_RATE)
		{
			StartRecording(current_microphone);
		}
		if (IS_RECORDING && mic_clip.channels > 0)
		{
			int position = Microphone.GetPosition(GetMic());
			bool flag = position < last_mic_pos;
			int num = position - last_mic_pos;
			if (flag)
			{
				num = position - (last_mic_pos - mic_clip.samples);
			}
			if (position == last_mic_pos)
			{
				return true;
			}
			int i = 0;
			if (!flag)
			{
				float[] samples = new float[num];
				mic_clip.GetData(samples, last_mic_pos);
				CopyMicSamplesIntoBufferAndApplyVolume(ref samples, ref i, samples.Length);
			}
			else if (position == 0)
			{
				float[] samples2 = new float[mic_clip.samples - last_mic_pos];
				mic_clip.GetData(samples2, last_mic_pos);
				CopyMicSamplesIntoBufferAndApplyVolume(ref samples2, ref i, samples2.Length);
			}
			else
			{
				float[] samples3 = new float[mic_clip.samples - last_mic_pos];
				float[] samples4 = new float[position];
				mic_clip.GetData(samples3, last_mic_pos);
				mic_clip.GetData(samples4, 0);
				CopyMicSamplesIntoBufferAndApplyVolume(ref samples3, ref i, samples3.Length);
				i = 0;
				CopyMicSamplesIntoBufferAndApplyVolume(ref samples4, ref i, samples4.Length);
			}
			last_mic_pos = position;
			SendAllAvailableMicData();
			return true;
		}
		return false;
	}

	private static void SendAllAvailableMicData(bool dontcare_if_it_dont_smaller_than_buffer = false)
	{
		if (my_mic_data_buffer.Count < FRAME_SIZE)
		{
			return;
		}
		float[] array = new float[FRAME_SIZE];
		UpdateOpusComplexity();
		while (my_mic_data_buffer.Count >= array.Length)
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = my_mic_data_buffer.Dequeue();
			}
			bool is_not_last = IsMicModeAlwaysOn() || my_mic_data_buffer.Count >= array.Length || IsPushToTalkKeyPushed();
			Client_SendMicDataFrame(array, is_not_last);
		}
		_push_to_talk_pushed = false;
	}

	private void Update()
	{
		system_framesize = CalculateFrameSizeForSamplerate(AudioSettings.outputSampleRate);
		listener_is_mindwiped = VCRULE_hearingloss && VCRULE_mindwiperule && (NetPlayer.LOCAL_PLAYER?.IsAliveAndMindwiped() ?? false);
		if (RecordMicUpdate())
		{
			Body localBodyNullable = Util.GetLocalBodyNullable();
			if ((Object)(object)localBodyNullable != (Object)null && localBodyNullable.alive)
			{
				BodyOpenMouthForSpeaking(localBodyNullable, my_output_volume);
			}
			if (my_output_volume < MINIMUM_VOLUME_TRESHOLD)
			{
				AlwaysOn_TimeBelowTreshold += Time.deltaTime;
			}
		}
		else
		{
			ENABLE_LOCAL_LOOPBACK = false;
			StopRecording();
		}
		smooth_bg_music_volume = Mathf.MoveTowards(smooth_bg_music_volume, smooth_bg_music_volume_target, Time.unscaledDeltaTime);
		smooth_bg_music_volume_target = Mathf.Min(1f, smooth_bg_music_volume_target + Time.unscaledDeltaTime / 8f);
		MusicManager_Start_MultiplayerPatch.musicsources_volume = Mathf.Max(0f, smooth_bg_music_volume * MusicManager_Start_MultiplayerPatch.musicvolume);
		if (ENABLE_LOCAL_LOOPBACK)
		{
			if (!UIBullshit.IsAnyMenuOpen() && Util.IsWorldGenerated())
			{
				ENABLE_LOCAL_LOOPBACK = false;
			}
		}
		else if ((Object)(object)ENABLE_LOCAL_LOOPBACK_VCINSTANCE != (Object)null)
		{
			Object.Destroy((Object)(object)((Component)ENABLE_LOCAL_LOOPBACK_VCINSTANCE).gameObject);
			ENABLE_LOCAL_LOOPBACK_VCINSTANCE = null;
		}
		if (UIMainMenu.IsInSettings() && (Object)(object)UIBullshit.main.egg != (Object)null)
		{
			float num = 0f;
			if ((Object)(object)ENABLE_LOCAL_LOOPBACK_VCINSTANCE != (Object)null)
			{
				num = ENABLE_LOCAL_LOOPBACK_VCINSTANCE.curmaxvol_in_opus_decoder;
			}
			if (!ENABLE_LOCAL_LOOPBACK)
			{
				num = my_output_volume;
			}
			num = ((!UIMainMenu.writeX_flipflop) ? Mathf.Abs(num) : (0f - Mathf.Abs(num)));
			if (Mathf.Abs(UIBullshit.main.egg.writeHeight) < Mathf.Abs(num))
			{
				UIBullshit.main.egg.writeHeight = KM.clamp11(num);
			}
		}
	}

	public static void BodyOpenMouthForSpeaking(Body b, float volume)
	{
		b.eatTime = Math.Max(b.eatTime, volume - 0.05f);
	}

	public static void StartRecording(string micName)
	{
		StopRecording();
		last_mic_pos = 0;
		current_microphone = micName;
		try
		{
			mic_clip = Microphone.Start(GetMic(), true, 2, SAMPLE_RATE);
			IS_RECORDING = (Object)(object)mic_clip != (Object)null;
			if (!IS_RECORDING)
			{
				log.error(" VOICECHAT: StartRecording(): Failed to start recording: " + micName + " ");
			}
		}
		catch (Exception ex)
		{
			Plugin.log.LogInfo((object)(" VOICECHAT: StartRecording(): Failed to set mic to: " + micName + " - " + ex.ToString()));
			current_microphone = "NO_MICROPHONE_SELECTED";
			IS_RECORDING = false;
		}
		if (!IS_RECORDING)
		{
			START_FAILURE_COUNT++;
			if (START_FAILURE_COUNT > 5)
			{
				KrokoshaScavMultiplayer.DoMultiplayerStatusMessageError($"Failed to initialize microphone {START_FAILURE_COUNT} times. Microphone is now disabled.");
				START_FAILURE_COUNT = 0;
				current_microphone_mode = 0;
			}
		}
	}

	public static string GetMic()
	{
		if (current_microphone == "NO_MICROPHONE_SELECTED")
		{
			return null;
		}
		return current_microphone;
	}

	public static void StopRecording()
	{
		my_output_volume = 0f;
		if (IS_RECORDING)
		{
			SendAllAvailableMicData(dontcare_if_it_dont_smaller_than_buffer: true);
			my_mic_data_buffer.Clear();
			IS_RECORDING = false;
			Microphone.End(GetMic());
			Object.Destroy((Object)(object)mic_clip);
			mic_clip = null;
		}
	}

	public static void SetMicrophone(string micName)
	{
		if (micName != current_microphone && KrokoshaScavMultiplayer.verbose)
		{
			Plugin.log.LogInfo((object)(" VOICECHAT: SetMicrophone(): " + micName + " "));
		}
		SettingsJsonThing.json.SetString("KrokoshaMultiplayer_MicrophoneName", micName);
		StartRecording(micName);
	}

	public static void SetMicrophoneMode(int mode)
	{
		SetMicrophoneMode((byte)mode);
	}

	public static void SetMicrophoneMode(byte mode)
	{
		SettingsJsonThing.json.SetInt(_PREF_MICMODE, mode);
		current_microphone_mode = mode;
	}

	private static void UpdateOpusComplexity()
	{
		if (encoder.opus_encoder == null)
		{
			return;
		}
		_complexityupdatetimer += Time.unscaledDeltaTime;
		if (_complexityupdatetimer > 1f)
		{
			_complexityupdatetimer = 0f;
			int num = (int)(1f / Time.unscaledDeltaTime).RemapClamped(20f, 60f, 0f, 10f);
			if (num != OpusEncoderExtensions.GetComplexity(encoder.opus_encoder))
			{
				OpusEncoderExtensions.SetComplexity(encoder.opus_encoder, num);
			}
		}
	}

	private static void Client_SendMicDataFrame(float[] data, bool is_not_last)
	{
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		my_output_volume = GetFrameMaxVol(data);
		if (ENABLE_LOCAL_LOOPBACK)
		{
			if ((Object)(object)ENABLE_LOCAL_LOOPBACK_VCINSTANCE == (Object)null)
			{
				GameObject val = new GameObject("vc_loopback");
				Object.DontDestroyOnLoad((Object)val);
				ENABLE_LOCAL_LOOPBACK_VCINSTANCE = val.AddComponent<VoicechatOutputDebugVisualiser>();
				ENABLE_LOCAL_LOOPBACK_VCINSTANCE.audioSource.rolloffMode = (AudioRolloffMode)0;
				ENABLE_LOCAL_LOOPBACK_VCINSTANCE.audioSource.spatialBlend = 0f;
				ENABLE_LOCAL_LOOPBACK_VCINSTANCE.force_no_effects = true;
			}
			((Component)ENABLE_LOCAL_LOOPBACK_VCINSTANCE).transform.position = ((Component)Camera.main).transform.position;
			byte[] array = encoder.Encode(in data, (ushort)SAMPLE_RATE);
			ENABLE_LOCAL_LOOPBACK_VCINSTANCE.OpusDecode((ushort)SAMPLE_RATE, array, out var voicedata);
			ENABLE_LOCAL_LOOPBACK_VCINSTANCE.ReceiveVoiceBlob((ushort)SAMPLE_RATE, in voicedata, in is_not_last);
			TEMPDEV_LAST_OPUS_ENCODED_SIZE = array.Length;
			TEMPDEV_LAST_OPUS_BEFORE_ENCODED_SIZE = data.Length;
			return;
		}
		if (my_output_volume > MINIMUM_VOLUME_TRESHOLD)
		{
			AlwaysOn_TimeBelowTreshold = 0f;
		}
		else if (AlwaysOn_TimeBelowTreshold > MINIMUM_VOLUME_TRESHOLD_TIME)
		{
			return;
		}
		if (Net.is_connected && !KrokoshaScavMultiplayer.is_dedicated_server)
		{
			byte[] array2 = encoder.Encode(in data, (ushort)SAMPLE_RATE);
			if (array2.Length != 0)
			{
				TEMPDEV_LAST_OPUS_ENCODED_SIZE = array2.Length;
				TEMPDEV_LAST_OPUS_BEFORE_ENCODED_SIZE = data.Length;
				NetDataWriter writer = Net.CreateWriter(10165);
				writer.Put((byte)Array.IndexOf(STANDART_FREQUENCIES, (ushort)SAMPLE_RATE));
				writer.PutBytesWithLength(array2);
				writer.Put((ushort)data.Length);
				writer.Put(is_not_last);
				Net.Client_Send((DeliveryMethod)4, in writer);
			}
		}
	}

	public static bool CheckIfPlayerIsUnconsciousToMute(NetPlayer plr)
	{
		if (KrokoshaScavMultiplayer.rules.SleepingMute)
		{
			if (plr.IsAliveAndNotConscious())
			{
				return true;
			}
		}
		else if (plr.IsAliveAndUnconsciousAndNotSleeping())
		{
			return true;
		}
		return false;
	}

	public static bool CheckIfIsAliveAndNotCriticallyDying(NetPlayer plr)
	{
		return plr.IsAliveAndNotCriticallyDying();
	}

	[ClientReceiver(10166, true)]
	private static void ClientReceiver__VoiceChatBlobRelay(knetid _, ref NetDataReader reader)
	{
		if (my_listener_volume <= 0f || SharedMain.local_world_is_generating)
		{
			return;
		}
		reader.Get(out knetid result);
		if (NetPlayer.TryGetPlayerFromClientId(result, out var plr) && (Object)(object)plr.vc_output != (Object)null)
		{
			byte b = default(byte);
			reader.Get(ref b);
			ushort num = STANDART_FREQUENCIES[b];
			reader.Get(out byte[] result2);
			bool is_not_last = default(bool);
			reader.Get(ref is_not_last);
			if (plr.vc_output.OpusDecode(num, result2, out var voicedata))
			{
				plr.vc_output.ReceiveVoiceBlob(num, in voicedata, in is_not_last);
			}
		}
	}

	[ServerReceiver(10165)]
	private static void ServerReceiver__VoiceChatBlob(knetid clientId, ref NetDataReader reader)
	{
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		if (!VCRULE_enabled || SharedMain.local_world_is_generating)
		{
			return;
		}
		byte b = default(byte);
		reader.Get(ref b);
		reader.Get(out byte[] result);
		bool is_not_last = default(bool);
		reader.Get(ref is_not_last);
		if (!NetPlayer.TryGetPlayerFromClientId(clientId, out var plr) || !((Object)(object)plr.vc_output != (Object)null))
		{
			return;
		}
		if (plr.server_mute_vc)
		{
			if (plr.server_plrstate.didmutewarn < 20)
			{
				plr.Server_DoAlertSingle("You're muted by the server!", reliable: false);
			}
			plr.server_plrstate.didmutewarn++;
		}
		else
		{
			if (b > STANDART_FREQUENCIES.Length)
			{
				return;
			}
			ushort num = STANDART_FREQUENCIES[b];
			if (num < 8000 || num > 48000 || num > SAMPLE_RATE || (plr.IsAlive() && VCRULE_speechimpaired && ((VCRULE_mindwiperule && plr.IsAliveAndMindwiped()) || CheckIfPlayerIsUnconsciousToMute(plr) || (plr.body.brainHealth < 75f && plr.body.brainHealth + 10f < (float)Random.Range(0, 100)))))
			{
				return;
			}
			bool flag = false;
			float[] voicedata = null;
			if (!plr.is_local)
			{
				if (!plr.vc_output.OpusDecode(num, result, out voicedata) || plr.vc_output.vc_buffered_amount + voicedata.Length >= plr.vc_output.clip.samples)
				{
					return;
				}
				if (KrokoshaScavMultiplayer.is_dedicated_server)
				{
					flag = true;
					if (my_listener_volume > 0f && !KrokoshaScavMultiplayer.DEDSERVER_NOVISUALS)
					{
						plr.vc_output.ReceiveVoiceBlob(num, in voicedata, in is_not_last);
					}
				}
			}
			List<NetPlayer> list = Server_VoiceRelayGetTargets(plr);
			if ((Object)(object)NetPlayer.LOCAL_PLAYER != (Object)null && list.Remove(NetPlayer.LOCAL_PLAYER) && !flag && my_listener_volume > 0f)
			{
				if (voicedata == null && !plr.vc_output.OpusDecode(num, result, out voicedata))
				{
					return;
				}
				plr.vc_output.ReceiveVoiceBlob(num, in voicedata, in is_not_last);
			}
			if (list.Count != 0)
			{
				List<knetid> list2 = list.Select((NetPlayer x) => x.clientId).ToList();
				NetDataWriter writer = Net.CreateWriter(10166);
				writer.Put((ushort)clientId);
				writer.Put(b);
				writer.PutBytesWithLength(result);
				writer.Put(is_not_last);
				DeliveryMethod delivery = (DeliveryMethod)4;
				IEnumerable<knetid> clientIds = list2;
				Net.Server_SendToClients(in delivery, in writer, in clientIds);
			}
		}
	}

	private static List<NetPlayer> Server_VoiceRelayGetTargets(NetPlayer yapper_plr)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		Vector2 plrpos = yapper_plr.pos;
		List<NetPlayer> list = new List<NetPlayer>(NetPlayer.ClientIdToPlayerDict.Values);
		list.Remove(yapper_plr);
		if (Util.IsWorldGenerated())
		{
			float num = yapper_plr.vc_output.max_hear_distance * yapper_plr.vc_output.max_hear_distance;
			float num2 = num * 0.5f;
			bool flag = yapper_plr.ImpairedSpeech();
			float dist_to_check_sqr = (flag ? num2 : num);
			if (!yapper_plr.IsAlive() && !VCRULE_deadchat)
			{
				list.RemoveAll((NetPlayer t) => CheckIfIsAliveAndNotCriticallyDying(yapper_plr));
			}
			list.RemoveAll((NetPlayer t) => !KM.dist2dsqrcheck_presqr(t.pos, in plrpos, dist_to_check_sqr));
		}
		return list;
	}

	public static float GetFrameMaxVol(IEnumerable<float> voice_frame)
	{
		float num = 0f;
		foreach (float item in voice_frame)
		{
			num = Math.Max(Mathf.Abs(item), num);
		}
		return num;
	}

	public static float[] ResampleVoiceBlob(in IReadOnlyList<float> data, int input_samplerate, int target_samplerate)
	{
		double num = (double)target_samplerate / (double)input_samplerate;
		int num2 = Mathf.RoundToInt((float)((double)(float)data.Count * num));
		float[] array = new float[num2];
		for (int i = 0; i < num2; i++)
		{
			double num3 = (double)i / num;
			int num4 = (int)Math.Floor(num3);
			double num5 = num3 - (double)num4;
			float num6 = data[num4];
			float num7 = num6;
			if (num4 + 1 < data.Count)
			{
				num7 = data[num4 + 1];
			}
			float num8 = (float)((double)num6 + (double)(num7 - num6) * num5);
			array[i] = num8;
		}
		return array;
	}

	private void OnApplicationQuit()
	{
		StopRecording();
		current_microphone_mode = 0;
	}
}
