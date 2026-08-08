using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public class BanList : KrokoshaScavSingleton
{
	[Serializable]
	public class BanEntry
	{
		public string ip;

		public string name;

		public ulong steamid;

		public bool HasIP()
		{
			if (!string.IsNullOrEmpty(ip))
			{
				return ip != "NULL";
			}
			return false;
		}

		public override string ToString()
		{
			string text = "";
			if (steamid != 0L)
			{
				text += $"SteamID:{steamid}  ";
			}
			if (HasIP())
			{
				text = text + "IP:" + ip + "  ";
			}
			return text + "Name:" + name;
		}
	}

	[Serializable]
	private class PrefData
	{
		public List<BanEntry> data = new List<BanEntry>();
	}

	private PrefData prefs = new PrefData();

	public static BanList Instance { get; private set; }

	public string filePath { get; private set; }

	public static IReadOnlyList<BanEntry> Entries => Instance.prefs.data;

	private void Awake()
	{
		if ((Object)(object)Instance != (Object)null && (Object)(object)Instance != (Object)(object)this)
		{
			Object.Destroy((Object)(object)((Component)this).gameObject);
			return;
		}
		Instance = this;
		filePath = Path.Combine(Application.persistentDataPath, "mp_banlist.json");
		Load();
	}

	public static void Add(string ip, string name, ulong steamid)
	{
		if (!((Object)(object)Instance == (Object)null) && (steamid == 0L || !Instance.prefs.data.Exists((BanEntry e) => e.steamid == steamid)))
		{
			BanEntry item = new BanEntry
			{
				ip = ip,
				name = name,
				steamid = steamid
			};
			Instance.prefs.data.Add(item);
			Instance.Save();
		}
	}

	public static bool RemoveBySteamId(ulong steamid)
	{
		if ((Object)(object)Instance == (Object)null)
		{
			return false;
		}
		BanEntry banEntry = Instance.prefs.data.Find((BanEntry e) => e.steamid == steamid);
		if (banEntry == null)
		{
			return false;
		}
		Instance.prefs.data.Remove(banEntry);
		Instance.Save();
		return true;
	}

	public static bool Remove(BanEntry entry)
	{
		if ((Object)(object)Instance == (Object)null)
		{
			return false;
		}
		bool result = Instance.prefs.data.Remove(entry);
		Instance.Save();
		return result;
	}

	public static bool IsBanned(ulong steamid)
	{
		if ((Object)(object)Instance == (Object)null || steamid == 0L)
		{
			return false;
		}
		List<BanEntry> data = Instance.prefs.data;
		for (int i = 0; i < data.Count; i++)
		{
			if (data[i].steamid == steamid)
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsBannedIPName(string ip, string name)
	{
		if ((Object)(object)Instance == (Object)null)
		{
			return false;
		}
		List<BanEntry> data = Instance.prefs.data;
		for (int i = 0; i < data.Count; i++)
		{
			BanEntry banEntry = data[i];
			if (banEntry.HasIP())
			{
				if (banEntry.ip == ip)
				{
					return true;
				}
			}
			else if (banEntry.name == name)
			{
				return true;
			}
		}
		return false;
	}

	public static void ClearAll()
	{
		if (!((Object)(object)Instance == (Object)null))
		{
			Instance.prefs.data.Clear();
			Instance.Save();
		}
	}

	private void Save()
	{
		try
		{
			string contents = JsonConvert.SerializeObject((object)prefs, (Formatting)1);
			File.WriteAllText(filePath, contents);
		}
		catch (Exception ex)
		{
			Debug.LogError((object)("BanList Save failed: " + ex));
		}
	}

	private void Load()
	{
		try
		{
			if (File.Exists(filePath))
			{
				string text = File.ReadAllText(filePath);
				prefs = JsonConvert.DeserializeObject<PrefData>(text) ?? new PrefData();
				return;
			}
		}
		catch (Exception ex)
		{
			Debug.LogError((object)("BanList Load failed: " + ex));
		}
		prefs = new PrefData();
	}
}
