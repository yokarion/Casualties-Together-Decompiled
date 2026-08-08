using System;
using KrokoshaCasualtiesUtils;
using OpusSharp.Core;
using OpusSharp.Core.Extensions;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public class StreamedAudioOutput : MonoBehaviour
{
	protected bool is_voice;

	protected bool speech_impaired;

	protected bool is_body_alive;

	public Body body;

	public float max_hear_distance = 82f;

	public const float PREBUFFER_LATENCY = 0.1f;

	public float cur_volume_output;

	protected float _real_custom_volume = 1f;

	internal AudioClip clip;

	internal float vc_lastsample1;

	internal float vc_lastsample2;

	internal int vc_endpos;

	internal int vc_buffered_amount;

	protected float TimeTheBufferEnded;

	protected float TimeTheBufferFirstReceivedBlob;

	internal MaxCapacityQueue<float> vc_buffer = new MaxCapacityQueue<float>(24000);

	internal bool force_no_effects;

	protected static float[] opus_decode_buffer = new float[2048];

	private OpusDecoder opus_decoder;

	public bool is_watered;

	public float pitchtarget = 1f;

	private int _last_timeSamples;

	public bool last_blob_was_last;

	private double last_blob_receive_time;

	public int MIN_SAMPLECOUNT_PREBUFFERED => (int)(0.1f * (float)clip.frequency);

	internal int SAMPLE_RATE => clip.frequency;

	internal int FRAME_SIZE => Voicechat.CalculateFrameSizeForSamplerate(SAMPLE_RATE);

	public float custom_volume_set
	{
		get
		{
			return _real_custom_volume;
		}
		set
		{
			_real_custom_volume = Math.Max(0f, value);
		}
	}

	public AudioLowPassFilter lowpass { get; protected set; }

	public AudioSource audioSource { get; protected set; }

	public int sample_rate { get; private set; }

	public int half_sample_rate => (int)((float)sample_rate * 0.5f);

	public int quarter_sample_rate => (int)((float)sample_rate * 0.25f);

	internal void EnsureOpusDecoder()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Expected O, but got Unknown
		if (opus_decoder == null || OpusDecoderExtensions.GetSampleRate(opus_decoder) != sample_rate)
		{
			OpusDecoder obj = opus_decoder;
			if (obj != null)
			{
				obj.Dispose();
			}
			opus_decoder = new OpusDecoder(sample_rate, 1, false);
		}
	}

	public bool OpusDecode(ushort samplerate, byte[] data, out float[] voicedata)
	{
		SetSamplerate(samplerate);
		EnsureOpusDecoder();
		int num = opus_decoder.Decode(data, data.Length, opus_decode_buffer, FRAME_SIZE, false);
		if (num > 0)
		{
			voicedata = new float[num];
			Buffer.BlockCopy(opus_decode_buffer, 0, voicedata, 0, num * 4);
			return true;
		}
		voicedata = new float[0];
		return false;
	}

	public override string ToString()
	{
		if (this is VoicechatOutput voicechatOutput && (Object)(object)voicechatOutput.plr != (Object)null)
		{
			return $"VC_OUTPUT(plr:{((object)voicechatOutput.plr).ToString()}, freq:{sample_rate})";
		}
		return $"STREAMED_AUDIO_OUTPUT({((Object)this).name}, freq:{sample_rate})";
	}

	public float GetFinalVolume()
	{
		float real_custom_volume = _real_custom_volume;
		if (is_voice)
		{
			real_custom_volume *= Voicechat.my_listener_volume;
			if (Voicechat.listener_is_mindwiped)
			{
				return real_custom_volume;
			}
			real_custom_volume *= 1.15f;
		}
		else
		{
			real_custom_volume *= Voicechat.mp3_listener_volume;
		}
		if (Util.IsWorldGenerated())
		{
			real_custom_volume *= 1.3f;
		}
		return real_custom_volume;
	}

	protected void UpdateLowPassEffects(in bool is_watered)
	{
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		float num = 22000f;
		if (is_voice)
		{
			if (is_body_alive)
			{
				if (Voicechat.VCRULE_speechimpaired)
				{
					if (is_watered)
					{
						num = 1000f;
					}
					if (speech_impaired)
					{
						num = 1200f;
					}
				}
				if (Voicechat.listener_is_mindwiped)
				{
					num = 300f;
				}
			}
		}
		else if (is_watered)
		{
			num = 1100f;
		}
		if (Voicechat.VCRULE_hearingloss && (!is_voice || is_body_alive) && Util.TryGetLocalBody(out var val) && val.conscious)
		{
			Vector3 val2 = ((Component)audioSource).transform.position;
			if (is_voice && is_body_alive)
			{
				val2 = Vector2.op_Implicit(body.GetHead().GetPosition());
			}
			if (Util.QuickGroundLinecastCheck(val.GetHead().GetPosition(), Vector2.op_Implicit(val2)))
			{
				num *= 0.6f;
			}
		}
		if (num < 22000f)
		{
			((Behaviour)lowpass).enabled = true;
			if (is_watered)
			{
				lowpass.cutoffFrequency = num;
			}
			else
			{
				lowpass.cutoffFrequency = Mathf.MoveTowards(lowpass.cutoffFrequency, num, Time.unscaledDeltaTime * 45000f);
			}
			audioSource.maxDistance = max_hear_distance * 0.5f;
		}
		else
		{
			if (lowpass.cutoffFrequency != 22000f)
			{
				lowpass.cutoffFrequency = Mathf.MoveTowards(lowpass.cutoffFrequency, 22000f, Time.unscaledDeltaTime * 45000f);
			}
			else
			{
				((Behaviour)lowpass).enabled = false;
			}
			audioSource.maxDistance = max_hear_distance;
		}
	}

	protected void UpdatePitchAligner()
	{
		float num = Time.unscaledDeltaTime * 0.1f;
		if ((float)MIN_SAMPLECOUNT_PREBUFFERED * 0.4f > (float)vc_buffered_amount)
		{
			if (last_blob_was_last)
			{
				pitchtarget = Mathf.Min(pitchtarget, 0.998f);
			}
			else
			{
				pitchtarget = Mathf.Min(pitchtarget, 0.94f);
			}
		}
		else if (vc_buffered_amount > half_sample_rate)
		{
			if (vc_buffered_amount > SAMPLE_RATE)
			{
				pitchtarget = Mathf.Max(pitchtarget, 1.1f);
			}
			else
			{
				pitchtarget = Mathf.Max(pitchtarget, 1.02f);
			}
		}
		audioSource.pitch = Mathf.MoveTowards(audioSource.pitch, pitchtarget, num);
	}

	private void EffectsUpdate()
	{
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		if (force_no_effects)
		{
			audioSource.spatialBlend = 0f;
			return;
		}
		if (Util.IsWorldGenerated())
		{
			audioSource.spatialBlend = 1f;
			audioSource.bypassReverbZones = Voicechat.listener_is_mindwiped;
			if (Voicechat.VCRULE_hearingloss && !Voicechat.listener_is_mindwiped)
			{
				if ((Object)(object)audioSource.outputAudioMixerGroup == (Object)null)
				{
					audioSource.outputAudioMixerGroup = WorldGeneration.world.soundMixerGroup;
				}
			}
			else if ((Object)(object)audioSource.outputAudioMixerGroup != (Object)null)
			{
				audioSource.outputAudioMixerGroup = null;
			}
		}
		else
		{
			audioSource.spatialBlend = 0f;
			if ((Object)(object)audioSource.outputAudioMixerGroup != (Object)null)
			{
				audioSource.outputAudioMixerGroup = null;
			}
		}
		is_watered = false;
		if (is_voice)
		{
			if (is_body_alive)
			{
				is_watered = body.inWater;
				audioSource.dopplerLevel = VoicechatOutput.default_voice_doppler;
			}
			else
			{
				audioSource.dopplerLevel = 0f;
			}
		}
		else
		{
			is_watered = Mathf.Abs(((Component)this).transform.position.x) < (float)WorldGeneration.world.halfWidth && Mathf.Abs(((Component)this).transform.position.y) < (float)WorldGeneration.world.halfHeight && FluidManager.main.HasLiquid(WorldGeneration.world.WorldToBlockPos(Vector2.op_Implicit(((Component)this).transform.position)));
		}
		UpdateLowPassEffects(in is_watered);
		if (Util.IsInMainMenu())
		{
			audioSource.maxDistance = max_hear_distance;
			pitchtarget = 1f;
			UpdatePitchAligner();
		}
		else if (is_voice)
		{
			bool num = this is VoicechatOutput;
			audioSource.minDistance = audioSource.maxDistance * 0.24f;
			pitchtarget = 1f;
			if (num && is_body_alive && Voicechat.VCRULE_speechimpaired)
			{
				float num2 = Mathf.Abs(body.opiateHappiness / 70f) * 0.5f;
				if (body.brainHealth < 93f)
				{
					num2 = Mathf.Clamp01(Mathf.Max(num2, body.brainHealth.RemapClamped(93f, 30f, 0f, 0.3f)));
				}
				if (num2 > 0.002f)
				{
					pitchtarget += (float)Math.Sin(Time.timeAsDouble + (12.0 + (double)(num2 * 9f))) * num2;
					if (body.opiateHappiness > 0f)
					{
						pitchtarget += num2 * 0.01f;
					}
					else
					{
						pitchtarget -= num2 * 0.01f;
					}
				}
			}
			UpdatePitchAligner();
		}
		else
		{
			audioSource.maxDistance = max_hear_distance;
			pitchtarget = 1f;
			UpdatePitchAligner();
		}
	}

	public void OnHearabilityRuleUpdate()
	{
		if (Object.op_Implicit((Object)(object)WorldGeneration.world) && Voicechat.VCRULE_hearingloss)
		{
			audioSource.outputAudioMixerGroup = WorldGeneration.world.soundMixerGroup;
		}
		else
		{
			audioSource.outputAudioMixerGroup = null;
		}
	}

	protected virtual void Awake()
	{
		sample_rate = 24000;
		audioSource = ComponentHolderProtocol.AddComponent<AudioSource>((Object)(object)this);
		audioSource.loop = true;
		audioSource.volume = 1f;
		audioSource.spatialBlend = 1f;
		audioSource.rolloffMode = (AudioRolloffMode)1;
		audioSource.minDistance = 10f;
		audioSource.maxDistance = max_hear_distance;
		audioSource.dopplerLevel = 0.3f;
		lowpass = ComponentHolderProtocol.AddComponent<AudioLowPassFilter>((Object)(object)this);
		lowpass.cutoffFrequency = 22000f;
		OnHearabilityRuleUpdate();
		SetSamplerate(24000);
	}

	private void Start()
	{
		vc_buffer.Clear();
	}

	protected void SetSamplerate(ushort newrate)
	{
		newrate = Voicechat.GetClosestSamplerate(newrate);
		if ((Object)(object)clip != (Object)null)
		{
			if (clip.frequency == newrate)
			{
				return;
			}
			audioSource.Stop();
			audioSource.clip = null;
			Object.Destroy((Object)(object)clip);
			clip = null;
			vc_buffer.Clear();
			vc_buffer = new MaxCapacityQueue<float>(newrate);
		}
		sample_rate = newrate;
		clip = AudioClip.Create("VOICE_CHAT_CLIP_" + ((Object)this).name, newrate * 2, 1, (int)newrate, false);
		audioSource.clip = clip;
	}

	protected void StartPlayback()
	{
		if (sample_rate == 0)
		{
			sample_rate = 24000;
		}
		vc_endpos = (vc_buffer.Count + audioSource.timeSamples) % clip.samples;
		vc_buffered_amount = vc_buffer.Count;
		float[] array = new float[sample_rate];
		for (int i = 0; i < array.Length; i++)
		{
			if (vc_buffer.Count == 0)
			{
				array[i] = vc_lastsample1;
				vc_lastsample1 = Mathf.MoveTowards(vc_lastsample1, 0f, 2f / (float)sample_rate);
			}
			else
			{
				vc_lastsample1 = vc_buffer.Dequeue();
				array[i] = vc_lastsample1;
			}
		}
		clip.SetData(array, audioSource.timeSamples);
		EffectsUpdate();
		last_blob_receive_time = Time.realtimeSinceStartupAsDouble;
		if (Voicechat.VCRULE_speechimpaired && is_voice && (Object)(object)body != (Object)null && body.alive)
		{
			audioSource.pitch = Mathf.Clamp(body.brainHealth * 0.012f + 0.08f, 0.7f, 1f);
		}
		else
		{
			audioSource.pitch = 1f;
		}
		audioSource.Play();
		_last_timeSamples = audioSource.timeSamples;
	}

	protected void StopPlayback()
	{
		vc_buffered_amount = 0;
		TimeTheBufferFirstReceivedBlob = 0f;
		audioSource.Stop();
	}

	protected void Update()
	{
		if (SharedMain.local_world_is_generating)
		{
			StopPlayback();
			vc_buffer.Clear();
			return;
		}
		if (!audioSource.isPlaying)
		{
			cur_volume_output = 0f;
		}
		if (vc_buffer.Count > 0)
		{
			TimeTheBufferFirstReceivedBlob += Time.unscaledDeltaTime;
			if (TimeTheBufferFirstReceivedBlob > 0.1f && !audioSource.isPlaying)
			{
				StartPlayback();
			}
		}
		else
		{
			TimeTheBufferEnded += Time.unscaledDeltaTime;
		}
		if (!audioSource.isPlaying)
		{
			return;
		}
		EffectsUpdate();
		int num = audioSource.timeSamples - _last_timeSamples;
		if (_last_timeSamples > audioSource.timeSamples)
		{
			num = audioSource.timeSamples - (_last_timeSamples - clip.samples);
		}
		vc_buffered_amount -= num;
		if (num > 0)
		{
			int num2 = _last_timeSamples - 6;
			if (num2 < 0)
			{
				num2 += audioSource.timeSamples;
			}
			float[] array = new float[num];
			clip.SetData(array, num2);
		}
		_last_timeSamples = audioSource.timeSamples;
		if (vc_buffered_amount <= 0)
		{
			StopPlayback();
		}
	}

	private void OnDestroy()
	{
		if ((Object)(object)audioSource != (Object)null && (Object)(object)((Component)this).gameObject != (Object)(object)((Component)audioSource).gameObject)
		{
			Object.Destroy((Object)(object)((Component)audioSource).gameObject);
		}
		if ((Object)(object)lowpass != (Object)null)
		{
			Object.Destroy((Object)(object)lowpass);
		}
		if ((Object)(object)audioSource != (Object)null)
		{
			Object.Destroy((Object)(object)audioSource);
		}
		if ((Object)(object)clip != (Object)null)
		{
			Object.Destroy((Object)(object)clip);
		}
	}

	public void ReceiveVoiceBlob(ushort freq, in float[] data, in bool is_not_last)
	{
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		SetSamplerate(freq);
		TimeTheBufferEnded = 0f;
		cur_volume_output = Voicechat.GetFrameMaxVol(data);
		float finalVolume = GetFinalVolume();
		if (audioSource.isPlaying)
		{
			int num = vc_buffered_amount + data.Length;
			if (num >= clip.samples)
			{
				if (log.verbose)
				{
					log.error($"VOICECHAT: TOO MUCH SAMPLES: {((object)this).ToString()}: {num} >= {clip.samples}");
				}
			}
			else
			{
				for (int i = 0; i < data.Length; i++)
				{
					data[i] = KM.clamp11(data[i]) * finalVolume;
				}
				_ = Time.unscaledTimeAsDouble;
				_ = last_blob_receive_time;
				int num2 = clip.samples - num;
				if (!is_not_last)
				{
					last_blob_was_last = true;
				}
				clip.SetData(data, vc_endpos);
				vc_endpos += data.Length;
				vc_buffered_amount = num;
				if (vc_endpos >= clip.samples)
				{
					vc_endpos -= clip.samples;
				}
				num2 = Math.Min(clip.samples - vc_buffered_amount, quarter_sample_rate);
				if (num2 > 10)
				{
					float[] array = new float[num2];
					for (int j = 0; j < num2; j++)
					{
						array[j] = data[data.Length - 1] * Mathf.Clamp01(1f - (float)(j / 24) * 3f);
					}
					clip.SetData(array, vc_endpos);
				}
				last_blob_receive_time = Time.unscaledTimeAsDouble;
			}
		}
		else
		{
			for (int k = 0; k < data.Length; k++)
			{
				vc_buffer.Enqueue(KM.clamp11(data[k]) * finalVolume);
			}
		}
		if (KM.dist2dsqrcheck(Vector2.op_Implicit(((Component)audioSource).transform.position), Vector2.op_Implicit(((Component)Camera.main).transform.position), max_hear_distance * 0.5f))
		{
			Voicechat.smooth_bg_music_volume_target = Mathf.Min(Voicechat.smooth_bg_music_volume_target, 1f - Mathf.Clamp01(cur_volume_output * 4f) * Voicechat.bg_music_attenuation);
		}
	}
}
