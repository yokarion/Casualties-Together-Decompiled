using System;
using HarmonyLib;
using KrokoshaCasualtiesUtils;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(Observer), "GunSuicide")]
internal static class Observer_GunSuicide_MultiplayerPatch
{
	private static bool Prefix(Observer __instance)
	{
		try
		{
			if (Util.GetLocalBody().totalHappiness <= -90f)
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			log.error(ex.ToString());
		}
		return true;
	}
}
