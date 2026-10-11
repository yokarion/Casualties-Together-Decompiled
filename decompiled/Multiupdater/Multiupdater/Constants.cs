using System.IO;

namespace Multiupdater;

internal class Constants
{
	public static readonly string TempDirectory = string.Join(Path.DirectorySeparatorChar.ToString(), Path.GetTempPath(), "CTAutoUpdater");

	public const string StageDirectoryName = "MULTIPLAYER_STAGED";

	public const string MultiReleaseURL = "https://api.github.com/repos/creaturefeaturelarry/casualties-together/releases/latest";

	public const string MultiArchiveNameContract = "CasualtiesTogether";
}
