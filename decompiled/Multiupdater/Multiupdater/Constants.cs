using System;
using System.IO;

namespace Multiupdater;

internal class Constants
{
	public static readonly string PluginsDirectory;

	public static readonly string MultiDirectory;

	public static readonly string MultiPluginDLL;

	public static readonly string TempDirectory;

	public const string StageDirectoryName = "MULTIPLAYER_STAGED";

	public const string MultiReleaseURL = "https://api.github.com/repos/krokosha666/cas-unk-krokosha-multiplayer-coop/releases/latest";

	public const string MultiArchiveNameContract = "KrokMP";

	static Constants()
	{
		char directorySeparatorChar = Path.DirectorySeparatorChar;
		PluginsDirectory = string.Join(directorySeparatorChar.ToString(), Environment.CurrentDirectory, "BepInEx", "plugins");
		directorySeparatorChar = Path.DirectorySeparatorChar;
		MultiDirectory = string.Join(directorySeparatorChar.ToString(), Environment.CurrentDirectory, "BepInEx", "plugins", "KrokMP");
		directorySeparatorChar = Path.DirectorySeparatorChar;
		MultiPluginDLL = string.Join(directorySeparatorChar.ToString(), Environment.CurrentDirectory, "BepInEx", "plugins", "KrokMP", "KrokoshaCasualtiesMP.dll");
		directorySeparatorChar = Path.DirectorySeparatorChar;
		TempDirectory = string.Join(directorySeparatorChar.ToString(), Path.GetTempPath(), "KRAutoUpdater");
	}
}
