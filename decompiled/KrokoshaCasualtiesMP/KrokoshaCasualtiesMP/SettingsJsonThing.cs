namespace KrokoshaCasualtiesMP;

public class SettingsJsonThing : KrokoshaScavSingleton
{
	public static SettingsJsonThing Instance;

	private JsonConfigThingy thing = new JsonConfigThingy("mp_settings.json");

	public static JsonConfigThingy json => Instance.thing;

	public SettingsJsonThing()
	{
		Instance = this;
	}
}
