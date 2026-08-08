using UnityEngine;

namespace KrokoshaCasualtiesMP;

internal class VoicechatOutputDebugVisualiser : VoicechatOutput
{
	public float curmaxvol_in_opus_decoder;

	private void MOST_WEIRDEST_WAY_TO_RENDER_ANYTHING_BRO___BUT_LOWKEY_I_COULDNT_FIND_HIGH_LEVEL_RENDER_API_IN_UNITY_OR_IM_BLIND()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < Mathf.Min(base.FRAME_SIZE, Screen.width); i++)
		{
			GUI.Label(new Rect((float)i, (float)Screen.height * (0.5f + StreamedAudioOutput.opus_decode_buffer[i] * 0.2f), 200f, 20f), "l");
		}
	}

	private new void Update()
	{
		base.Update();
		curmaxvol_in_opus_decoder = 0f;
		float[] array = StreamedAudioOutput.opus_decode_buffer;
		foreach (float num in array)
		{
			if (Mathf.Abs(curmaxvol_in_opus_decoder) < Mathf.Abs(num))
			{
				curmaxvol_in_opus_decoder = num;
			}
		}
	}
}
