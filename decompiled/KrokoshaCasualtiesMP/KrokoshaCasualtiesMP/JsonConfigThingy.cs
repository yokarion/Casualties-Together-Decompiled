using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using KrokoshaCasualtiesUtils;
using Newtonsoft.Json;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public class JsonConfigThingy
{
	[Serializable]
	private class PrefData
	{
		public Dictionary<string, string> data = new Dictionary<string, string>();
	}

	private string filePath;

	private PrefData prefs = new PrefData();

	public JsonConfigThingy(string fileName)
	{
		filePath = Path.Combine(Application.persistentDataPath, fileName);
		Load();
	}

	public void SetString(string key, string value)
	{
		prefs.data[key] = value;
	}

	public string GetString(string key, string defaultValue = "")
	{
		if (!prefs.data.TryGetValue(key, out var value))
		{
			return defaultValue;
		}
		return value;
	}

	public void SetBool(string key, bool value)
	{
		SetString(key, value ? "1" : "0");
	}

	public bool GetBool(string key, bool defaultValue = false)
	{
		if (prefs.data.TryGetValue(key, out var value))
		{
			value = value.ToLower();
			if (bool.TryParse(value, out var result))
			{
				return result;
			}
			if (int.TryParse(value, out var result2))
			{
				if (result2 == 0)
				{
					return false;
				}
				return true;
			}
		}
		return defaultValue;
	}

	public void SetInt(string key, int value)
	{
		SetString(key, value.ToString());
	}

	public int GetInt(string key, int defaultValue = 0)
	{
		if (prefs.data.TryGetValue(key, out var value) && int.TryParse(value, out var result))
		{
			return result;
		}
		return defaultValue;
	}

	public void SetFloat(string key, float value)
	{
		SetString(key, value.ToStringInvariant());
	}

	public float GetFloat(string key, float defaultValue = 0f)
	{
		if (prefs.data.TryGetValue(key, out var value) && float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
		{
			return result;
		}
		return defaultValue;
	}

	public bool HasKey(string key)
	{
		return prefs.data.ContainsKey(key);
	}

	public void DeleteKey(string key)
	{
		prefs.data.Remove(key);
	}

	public void DeleteAll()
	{
		prefs.data.Clear();
	}

	public void Save()
	{
		try
		{
			string contents = JsonConvert.SerializeObject((object)prefs, (Formatting)1);
			File.WriteAllText(filePath, contents);
		}
		catch (Exception ex)
		{
			log.error(GetType().Name + " Save failed: " + ex.ToString());
		}
	}

	public void Load()
	{
		try
		{
			if (File.Exists(filePath))
			{
				string text = File.ReadAllText(filePath);
				prefs = JsonConvert.DeserializeObject<PrefData>(text) ?? new PrefData();
			}
			else
			{
				prefs = new PrefData();
			}
		}
		catch (Exception ex)
		{
			log.error(GetType().Name + " Load failed: " + ex.ToString());
			prefs = new PrefData();
		}
	}
}
