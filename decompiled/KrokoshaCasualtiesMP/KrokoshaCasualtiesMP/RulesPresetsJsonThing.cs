using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public class RulesPresetsJsonThing : KrokoshaScavSingleton
{
	[Serializable]
	private class PrefData
	{
		public Dictionary<string, KrokoshaMultiplayerGameRules> data = new Dictionary<string, KrokoshaMultiplayerGameRules>();
	}

	private static string filePath;

	private static PrefData prefs = new PrefData();

	private void Awake()
	{
		filePath = Path.Combine(Application.persistentDataPath, "mp_rule_presets.json");
		Load();
	}

	public static IEnumerable<string> GetPresets()
	{
		return prefs.data.Keys;
	}

	public static void SetRules(string key, KrokoshaMultiplayerGameRules value)
	{
		prefs.data[key] = value;
	}

	public static KrokoshaMultiplayerGameRules GetRules(string key)
	{
		return GetRules(key, new KrokoshaMultiplayerGameRules());
	}

	public static KrokoshaMultiplayerGameRules GetRules(string key, in KrokoshaMultiplayerGameRules defaultValue)
	{
		if (!prefs.data.TryGetValue(key, out var value))
		{
			return defaultValue;
		}
		return value;
	}

	public static bool HasKey(string key)
	{
		return prefs.data.ContainsKey(key);
	}

	public static void DeleteKey(string key)
	{
		prefs.data.Remove(key);
	}

	public static void DeleteAll()
	{
		prefs.data.Clear();
	}

	public static void Save()
	{
		try
		{
			string contents = JsonConvert.SerializeObject((object)prefs, (Formatting)1);
			File.WriteAllText(filePath, contents);
		}
		catch (Exception ex)
		{
			log.error("RulesPresetsJsonThing Save failed: " + ex.ToString());
		}
	}

	public void Load()
	{
		try
		{
			if (File.Exists(filePath))
			{
				prefs = JsonConvert.DeserializeObject<PrefData>(File.ReadAllText(filePath)) ?? new PrefData();
			}
			else
			{
				prefs = new PrefData();
			}
		}
		catch (Exception ex)
		{
			log.error(((object)this).GetType().Name + " Load failed: " + ex.ToString());
			prefs = new PrefData();
		}
	}
}
