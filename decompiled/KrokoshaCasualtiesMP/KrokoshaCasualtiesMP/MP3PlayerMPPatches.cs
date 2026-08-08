namespace KrokoshaCasualtiesMP;

internal class MP3PlayerMPPatches : KrokoshaScavSingleton
{
	private void KrokoshaSingletonEvent_OnSceneChange()
	{
		MP3Menu.clips?.Clear();
		MP3Menu.dropdownList?.Clear();
		MP3Menu.clips = null;
		MP3Menu.dropdownList = null;
	}
}
