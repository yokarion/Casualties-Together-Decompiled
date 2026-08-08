namespace KrokoshaCasualtiesMP;

public abstract class MP3PlayerAudioStreamPlayerBase : StreamedAudioOutput
{
	private new void Awake()
	{
		max_hear_distance = 82f;
		is_voice = false;
		base.Awake();
	}
}
