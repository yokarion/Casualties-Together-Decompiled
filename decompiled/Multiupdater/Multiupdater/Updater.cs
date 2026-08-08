using System.Threading.Tasks;

namespace Multiupdater;

internal static class Updater
{
	public static async Task<bool> InitiateUpdate(string ArchiveURL)
	{
		AutoupdaterGUI.ShowUpdateButton = false;
		FileOperations.InitTempDirectory();
		AutoUpdater.LogMessage("Downloading");
		string zippedPath;
		try
		{
			zippedPath = await FileOperations.DownloadArchive(ArchiveURL);
		}
		catch
		{
			AutoUpdater.LogError("Github seems to be unreachable. Check your internet connection and github status.");
			goto IL_00d2;
		}
		AutoUpdater.LogMessage("Unpacking");
		try
		{
			FileOperations.UnzipFiles(zippedPath);
		}
		catch
		{
			AutoUpdater.LogError("Failed to unzip the update. Download might be corrupted. Retry.");
			goto IL_00d2;
		}
		AutoUpdater.LogMessage("Update staged! Restart the game to apply.");
		return true;
		IL_00d2:
		FileOperations.ClearTempDirectory();
		AutoupdaterGUI.ShowUpdateButton = true;
		return false;
	}
}
