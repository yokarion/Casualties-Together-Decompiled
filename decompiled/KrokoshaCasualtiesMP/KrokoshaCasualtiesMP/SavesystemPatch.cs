using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HarmonyLib;
using KrokoshaCasualtiesUtils;
using UnityEngine;
using UnityEngine.UI;

namespace KrokoshaCasualtiesMP;

public static class SavesystemPatch
{
	public static string savedatapathreplacement = "";

	private const string _mpsavefolder_name = "mp_save";

	public const string mpsaveheaderfile = "mp_rules.json";

	public static string mpsavefolder => Path.Combine(Application.persistentDataPath, "mp_save");

	public static string vanillasavefolder => Application.persistentDataPath;

	public static WorldGeneration world => WorldGeneration.world;

	public static ushort[,] worldBlocks => WorldGeneration.world.worldBlocks;

	public static byte[,] fluid => FluidManager.main.fluid;

	public static string ReplacementFor_Application_persistentDataPath()
	{
		return savedatapathreplacement;
	}

	public static bool HasAnySaveFile()
	{
		if (!HasVanillaSaveFile())
		{
			return HasMultiplayerSaveFile();
		}
		return true;
	}

	public static bool HasOnlyVanillaSaveFile()
	{
		if (HasVanillaSaveFile())
		{
			return !HasMultiplayerSaveFile();
		}
		return false;
	}

	public static bool HasVanillaSaveFile()
	{
		return File.Exists(vanillasavefolder + "\\save.sv");
	}

	public static bool HasMultiplayerSaveFile()
	{
		return File.Exists(mpsavefolder + "\\save.sv");
	}

	public static void UpdatePersistentDataPathCuzMultiplayer()
	{
		savedatapathreplacement = GetTheOnePersistentDataPathWithSaveFile();
		if (Util.IsInMainMenu())
		{
			PreRunScript val = Object.FindObjectOfType<PreRunScript>();
			if ((Object)(object)val != (Object)null)
			{
				((Selectable)val.loadButton).interactable = SaveSystem.HasSave();
			}
		}
	}

	internal static string GetTheOnePersistentDataPathWithSaveFile()
	{
		if (KrokoshaScavMultiplayer.network_system_is_running && HasMultiplayerSaveFile())
		{
			return mpsavefolder;
		}
		return Application.persistentDataPath;
	}

	internal static string GetPersistentDataPathToSave()
	{
		if (KrokoshaScavMultiplayer.network_system_is_running)
		{
			return mpsavefolder;
		}
		return Application.persistentDataPath;
	}

	internal static void DeleteMPSave()
	{
		if (Directory.Exists(mpsavefolder))
		{
			try
			{
				Directory.Delete(mpsavefolder, recursive: true);
			}
			catch (Exception ex)
			{
				log.error("LoadGame_MultiplayerPatch: DELETING SAVE\n" + ex.ToString());
			}
		}
	}

	internal static IEnumerable<CodeInstruction> TranspilerToReplaceTheApplicationPath(IEnumerable<CodeInstruction> instructions)
	{
		List<CodeInstruction> list = new List<CodeInstruction>(instructions);
		for (int i = 0; i < instructions.Count(); i++)
		{
			CodeInstruction val = list[i];
			if (((object)val).ToString().Contains("get_persistentDataPath"))
			{
				val.operand = AccessTools.Method(typeof(SavesystemPatch), "ReplacementFor_Application_persistentDataPath", (Type[])null, (Type[])null);
			}
		}
		return instructions;
	}

	public static byte[] SerializeWorldBlocks()
	{
		byte[] array = new byte[world.width * world.height];
		ushort[,] array2 = worldBlocks;
		for (int i = 0; i < world.height; i++)
		{
			for (int k = 0; k < world.width; k++)
			{
				array[i * world.height + k] = (byte)array2[k, i];
			}
		}
		return Util.Compress(array);
	}

	public static void DeserializeWorldBlocks(in byte[] mapdata)
	{
		try
		{
			byte[] array = Util.Decompress(mapdata);
			ushort[,] array2 = worldBlocks;
			for (int i = 0; i < world.height; i++)
			{
				for (int k = 0; k < world.width; k++)
				{
					array2[k, i] = array[i * world.height + k];
				}
			}
			world.UpdateWorld();
		}
		catch (Exception arg)
		{
			log.error($"Failed to load map blocks: {arg}");
		}
	}

	public static byte[] SerializeWorldFluids()
	{
		byte[] array = new byte[world.width * world.height];
		byte[,] array2 = fluid;
		for (int i = 0; i < world.height; i++)
		{
			for (int k = 0; k < world.width; k++)
			{
				array[i * world.height + k] = array2[k, i];
			}
		}
		return Util.Compress(array);
	}

	public static void DeserializeWorldFluids(in byte[] mapdata)
	{
		try
		{
			byte[] array = Util.Decompress(mapdata);
			byte[,] array2 = fluid;
			for (int i = 0; i < world.height; i++)
			{
				for (int k = 0; k < world.width; k++)
				{
					array2[k, i] = array[i * world.height + k];
				}
			}
			world.UpdateWorld();
		}
		catch (Exception arg)
		{
			log.error($"Failed to load map fluid: {arg}");
		}
	}
}
