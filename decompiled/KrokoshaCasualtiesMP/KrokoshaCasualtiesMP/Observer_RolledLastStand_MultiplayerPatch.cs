using System;
using HarmonyLib;
using KrokoshaCasualtiesUtils;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(Observer), "RolledLastStand")]
internal static class Observer_RolledLastStand_MultiplayerPatch
{
	private static bool Prefix(Observer __instance)
	{
		try
		{
			if (Util.GetLocalBody().lastStandTime == 300f)
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
