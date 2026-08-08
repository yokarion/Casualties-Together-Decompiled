using HarmonyLib;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(ConsoleScript), "TryExecuteCommand")]
public static class ConsoleScript_TryExecuteCommand_MultiplayerPatch
{
	public static bool Prefix(ConsoleScript __instance, string[] args, bool addToLog)
	{
		if (addToLog && KrokoshaScavMultiplayer.IsNetworkActiveAndIsClient() && Con.client_adminmode)
		{
			if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
			{
				return false;
			}
			if (Con.localonly_commands.Contains(args[0]))
			{
				return true;
			}
			if (Con.banned_admin_commands.Contains(args[0]))
			{
				log.l(args[0] + " is a banned admin command.");
				return false;
			}
			string command = string.Join(" ", args);
			log.l("Sending command to server: \"" + command + "\"");
			Con.Client_ExecuteAdminCommand(in command);
			if (addToLog)
			{
				__instance.AddCommandToLogAndClearInput();
			}
			return false;
		}
		return true;
	}
}
