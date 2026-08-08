using System;
using OpusSharp.Core;
using OpusSharp.Core.Extensions;

namespace KrokoshaCasualtiesMP;

internal class AudioOpusEncoding : IDisposable
{
	public static ushort[] STANDART_FREQUENCIES = new ushort[5] { 8000, 12000, 16000, 24000, 48000 };

	internal byte[] opus_encode_buffer = new byte[1024];

	internal OpusEncoder opus_encoder;

	public byte[] Encode(in float[] data, ushort input_samplerate)
	{
		EnsureOpusEncoder(input_samplerate);
		int num = 0;
		try
		{
			num = opus_encoder.Encode(data, data.Length, opus_encode_buffer, opus_encode_buffer.Length);
			if (num > 0)
			{
				byte[] array = new byte[num];
				Buffer.BlockCopy(opus_encode_buffer, 0, array, 0, num);
				return array;
			}
		}
		catch (Exception arg)
		{
			log.warn($"OPUS ENCODE ERROR: samplerate: {OpusEncoderExtensions.GetSampleRate(opus_encoder)}  data.Length:{data.Length} \n{arg} ");
			return new byte[0];
		}
		log.warn($"OPUS ENCODE ERROR: samplerate: {OpusEncoderExtensions.GetSampleRate(opus_encoder)}  data.Length:{data.Length} ");
		return new byte[0];
	}

	private void EnsureOpusEncoder(ushort target_samplerate)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Expected O, but got Unknown
		if (opus_encoder == null || OpusEncoderExtensions.GetSampleRate(opus_encoder) != target_samplerate)
		{
			OpusEncoder obj = opus_encoder;
			if (obj != null)
			{
				obj.Dispose();
			}
			opus_encoder = new OpusEncoder((int)target_samplerate, 1, (OpusPredefinedValues)2048, false);
			OpusEncoderExtensions.SetVbr(opus_encoder, true);
		}
	}

	public void Dispose()
	{
		if (opus_encoder != null)
		{
			opus_encoder.Dispose();
			opus_encoder = null;
		}
	}
}
