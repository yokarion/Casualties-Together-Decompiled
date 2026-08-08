using System.Linq;
using HarmonyLib;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(ConsoleScript), "LogToConsole")]
public static class ConsoleScript_LogToConsole_MultiplayerPatch
{
	public static bool Prefix(ConsoleScript __instance, string text)
	{
		if (KrokoshaScavMultiplayer.IsNetworkActiveAndIsServer())
		{
			if (text.StartsWith("The character with Unicode value \\u"))
			{
				return true;
			}
			Con.Server_SendConsoleLog(in text, Con.server_admins.Select((NetPlayer x) => x.clientId).ToList());
		}
		return true;
	}
}
