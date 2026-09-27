using HarmonyLib;
using KrokoshaCasualtiesUtils;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(Skills), "CheckForLevelUp")]
public static class Skills_CheckForLevelUp_MultiplayerPatch
{
	private static void Prefix(Skills __instance, ref int level, ref float xp, ref int xpToLevel)
	{
		if (xp >= 2.1474836E+09f)
		{
			xp = 2.1474835E+09f;
		}
		else if (xp < 0f)
		{
			xp = 1.0737418E+09f;
		}
	}

	private static void Postfix(Skills __instance, ref bool __result)
	{
		if (!KrokoshaScavMultiplayer.network_system_is_running)
		{
			return;
		}
		foreach (Body key in NetPlayer.BodyToPlayerDict.Keys)
		{
			if (key.skills == __instance)
			{
				if (!Util.IsBodyLocal(key))
				{
					__result = false;
				}
				break;
			}
		}
	}
}
