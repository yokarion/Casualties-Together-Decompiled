namespace KrokoshaCasualtiesMP;

public class SettingsJsonThing : KrokoshaScavSingleton
{
	private static JsonConfigThingy thing;

	public static JsonConfigThingy json
	{
		get
		{
			if (thing == null)
			{
				thing = new JsonConfigThingy("mp_settings.json");
			}
			return thing;
		}
	}
}
