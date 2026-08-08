using UnityEngine;

namespace KrokoshaCasualtiesMP;

public class MP3PlayerAudioStreamPlayer_ClientOnly : MP3PlayerAudioStreamPlayerBase
{
	private void LateUpdate()
	{
		if (TimeTheBufferEnded > 30f)
		{
			TimeTheBufferEnded = 20f;
			Object.Destroy((Object)(object)this);
		}
	}
}
