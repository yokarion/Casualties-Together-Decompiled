using System;
using System.Collections.Generic;
using System.IO;
using Mono.Cecil;

namespace autoupdater_patcher;

public static class AutoUpdaterPatcher
{
	public static readonly string OSTempTrimmed = Path.GetTempPath().TrimEnd(new char[1] { Path.DirectorySeparatorChar });

	public static readonly string TempDirectory;

	public static readonly string StagedDirectory;

	public static readonly string UnpackDirectory;

	public static readonly string MultiPluginDirectory;

	public static readonly string[] OldPluginDLLs;

	public static readonly HashSet<string> FileExceptions;

	public static IEnumerable<string> TargetDLLs { get; } = new string[1] { "Assembly-CSharp.dll" };

	public static void Patch(AssemblyDefinition assembly)
	{
		try
		{
			if (!Directory.Exists(StagedDirectory))
			{
				return;
			}
			string[] oldPluginDLLs = OldPluginDLLs;
			foreach (string path in oldPluginDLLs)
			{
				if (File.Exists(path))
				{
					File.Delete(path);
				}
			}
			CloneDirectory(LookForTargetDirectory(StagedDirectory) ?? throw new Exception("Could not find the mod root directory!"), UnpackDirectory);
			Directory.Delete(TempDirectory, recursive: true);
		}
		catch (Exception ex)
		{
			File.WriteAllText(Path.GetTempPath() + "KROK_AUTOUPDATER_LOG_" + DateTime.Now.Ticks + ".txt", ex.ToString() + "\n\n" + LookForTargetDirectory(StagedDirectory).ToString());
		}
		static void CloneDirectory(string root, string dest)
		{
			string[] directories = Directory.GetDirectories(root);
			foreach (string text in directories)
			{
				string text2 = Path.Combine(dest, Path.GetFileName(text));
				Directory.CreateDirectory(text2);
				CloneDirectory(text, text2);
			}
			directories = Directory.GetFiles(root);
			foreach (string text3 in directories)
			{
				string text4 = Path.Combine(dest, Path.GetFileName(text3));
				if (!FileExceptions.Contains(text4))
				{
					File.Copy(text3, text4, overwrite: true);
				}
			}
		}
		static string LookForTargetDirectory(string path2)
		{
			string[] directories = Directory.GetDirectories(path2);
			foreach (string text in directories)
			{
				if (text.Contains("BepInEx"))
				{
					return Path.GetDirectoryName(text);
				}
				string text2 = LookForTargetDirectory(text);
				if (text2 != null)
				{
					return text2;
				}
			}
			return null;
		}
	}

	static AutoUpdaterPatcher()
	{
		char directorySeparatorChar = Path.DirectorySeparatorChar;
		TempDirectory = string.Join(directorySeparatorChar.ToString(), OSTempTrimmed, "KRAutoUpdater");
		directorySeparatorChar = Path.DirectorySeparatorChar;
		StagedDirectory = string.Join(directorySeparatorChar.ToString(), OSTempTrimmed, "KRAutoUpdater", "MULTIPLAYER_STAGED");
		directorySeparatorChar = Path.DirectorySeparatorChar;
		UnpackDirectory = string.Join(directorySeparatorChar.ToString(), Environment.CurrentDirectory);
		directorySeparatorChar = Path.DirectorySeparatorChar;
		MultiPluginDirectory = string.Join(directorySeparatorChar.ToString(), Environment.CurrentDirectory, "BepInEx", "plugins", "KrokMP");
		string[] array = new string[6];
		directorySeparatorChar = Path.DirectorySeparatorChar;
		array[0] = string.Join(directorySeparatorChar.ToString(), Environment.CurrentDirectory, "BepInEx", "plugins", "KrokoshaCasualtiesMP.dll");
		directorySeparatorChar = Path.DirectorySeparatorChar;
		array[1] = string.Join(directorySeparatorChar.ToString(), Environment.CurrentDirectory, "BepInEx", "plugins", "opus.dll");
		directorySeparatorChar = Path.DirectorySeparatorChar;
		array[2] = string.Join(directorySeparatorChar.ToString(), Environment.CurrentDirectory, "BepInEx", "plugins", "OpusSharp.Core.dll");
		directorySeparatorChar = Path.DirectorySeparatorChar;
		array[3] = string.Join(directorySeparatorChar.ToString(), Environment.CurrentDirectory, "BepInEx", "plugins", "Unity.Netcode.Components.dll");
		directorySeparatorChar = Path.DirectorySeparatorChar;
		array[4] = string.Join(directorySeparatorChar.ToString(), Environment.CurrentDirectory, "BepInEx", "plugins", "Unity.Netcode.Runtime.dll");
		directorySeparatorChar = Path.DirectorySeparatorChar;
		array[5] = string.Join(directorySeparatorChar.ToString(), Environment.CurrentDirectory, "BepInEx", "plugins", "Unity.Networking.Transport.dll");
		OldPluginDLLs = array;
		FileExceptions = new HashSet<string> { typeof(AutoUpdaterPatcher).Assembly.Location };
	}
}
