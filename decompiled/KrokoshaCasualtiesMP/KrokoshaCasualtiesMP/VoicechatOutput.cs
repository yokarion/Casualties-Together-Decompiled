using System.Collections.Generic;
using System.Linq;
using KrokoshaCasualtiesUtils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

internal class VoicechatOutput : StreamedAudioOutput
{
	public static float default_voice_doppler = 0.23f;

	public NetPlayer plr { get; private set; }

	public NetBody npc => plr.playerbody;

	protected override void Awake()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Expected O, but got Unknown
		is_voice = true;
		plr = ((Component)this).GetComponent<NetPlayer>();
		max_hear_distance = Voicechat.MAX_HEAR_DISTANCE;
		GameObject val = new GameObject("streamedaudiooutput");
		base.audioSource = val.AddComponent<AudioSource>();
		base.audioSource.loop = true;
		base.audioSource.spatialBlend = 1f;
		base.audioSource.rolloffMode = (AudioRolloffMode)1;
		base.audioSource.minDistance = 25f;
		base.audioSource.dopplerLevel = default_voice_doppler;
		base.audioSource.volume = 1f;
		base.lowpass = val.AddComponent<AudioLowPassFilter>();
		base.lowpass.cutoffFrequency = 22000f;
		Object.DontDestroyOnLoad((Object)(object)val);
		SetSamplerate(24000);
	}

	public Color CalculateVoiceChatIconColor()
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		float realcurvol;
		return CalculateVoiceChatIconColor(out realcurvol);
	}

	public Color CalculateVoiceChatIconColor(out float realcurvol)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		realcurvol = 0f;
		if (plr.is_local)
		{
			realcurvol = Voicechat.my_output_volume;
		}
		else
		{
			realcurvol = cur_volume_output;
		}
		Color val = plr.plrcolor;
		return new Color(val.r, val.g, val.b, Mathf.Clamp01(realcurvol + 0.3f));
	}

	private void LateUpdate()
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)plr != (Object)null)
		{
			is_body_alive = plr.IsAlive();
			if (is_body_alive)
			{
				body = plr.body;
				Vector2 a = Vector2.op_Implicit(((Component)base.audioSource).transform.position);
				Vector2 b = body.GetHead().GetPosition();
				if (!KM.dist2dsqrcheck(in a, in b, 64f))
				{
					((Component)base.audioSource).transform.position = Vector2.op_Implicit(b);
				}
				else
				{
					Vector2 val = Vector2.Lerp(KM.clampDistance(a, b, 4f), b, Time.unscaledDeltaTime * 6f);
					((Component)base.audioSource).transform.position = Vector2.op_Implicit(Vector2.Lerp(a, val, 0.5f));
				}
				Voicechat.BodyOpenMouthForSpeaking(body, cur_volume_output);
			}
			else if (Util.IsWorldGenerated())
			{
				((Component)base.audioSource).transform.position = Vector2.op_Implicit(plr.camerapos);
			}
			else
			{
				List<NetPlayer> list = NetPlayer.ClientIdToPlayerDict.Values.ToList();
				_ = ((float)list.IndexOf(plr) + 0.5f - (float)list.Count * 0.5f) / (float)list.Count;
				((Component)base.audioSource).transform.position = ((Component)Camera.main).transform.position + Vector3.forward * 4f;
			}
			if (is_body_alive && Voicechat.VCRULE_speechimpaired)
			{
				if ((Object)(object)body.talker.body != (Object)null)
				{
					speech_impaired = plr.body.talker.impairedSpeech;
				}
			}
			else
			{
				speech_impaired = false;
			}
		}
		else
		{
			is_body_alive = false;
		}
	}
}
