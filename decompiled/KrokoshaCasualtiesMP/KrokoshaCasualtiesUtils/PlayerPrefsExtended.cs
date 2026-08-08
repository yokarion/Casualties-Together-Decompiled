using UnityEngine;

namespace KrokoshaCasualtiesUtils;

public static class PlayerPrefsExtended
{
	public static bool GetBool(string key, bool defaultValue = false)
	{
		return PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) != 0;
	}

	public static void SetBool(string key, bool value)
	{
		PlayerPrefs.SetInt(key, value ? 1 : 0);
	}
}
