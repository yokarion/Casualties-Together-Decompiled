using System.Collections.Generic;
using System.Linq;
using HarmonyLib;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(Locale), "LoadLanguage")]
internal static class Locale_LoadLanguage_MultiplayerPatch
{
	private static void Postfix()
	{
		foreach (KeyValuePair<string, string> item in Lang.EN.Where((KeyValuePair<string, string> x) => x.Key.StartsWith("gameset")))
		{
			if (!Locale.currentLang.other.ContainsKey(item.Key))
			{
				Locale.currentLang.other[item.Key] = item.Value;
			}
		}
	}
}
