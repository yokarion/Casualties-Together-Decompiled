using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BepInEx;
using KrokoshaCasualtiesUtils;
using Newtonsoft.Json.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public class KnownPersons : KrokoshaScavSingleton
{
	private static bool aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa = false;

	private static HashSet<ulong> STARS = new HashSet<ulong> { 76561198328981815uL, 76561199087655775uL, 76561199067997543uL, 76561198053002922uL, 76561198253391969uL };

	private static ulong[] MPMOD_DEV = new ulong[1] { 76561198838808878uL };

	public static KnownPersons inst { get; private set; }

	private static string fileName => "casualtiestogether-d.bin";

	private static string filePath => Path.Combine(Paths.CachePath, fileName);

	private void Awake()
	{
		inst = this;
	}

	private void Start()
	{
		LoadEverything();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsSteamUserPrivileged(ulong user)
	{
		if (user != 76561198273985997L && user != 76561198442198974L)
		{
			return user == 76561198838808878L;
		}
		return true;
	}

	public static void LoadTagsFor(NetPlayer plr)
	{
		if (Net.TryGetSteamTransport(out var _))
		{
			_ = plr.SteamId;
			if (STARS.Contains(plr.SteamId) && !plr.KnownUserTagIcons.Contains(CoopModAssets.star_yellow.texture))
			{
				plr.KnownUserTagIcons.Add(CoopModAssets.star_yellow.texture);
			}
			if (MPMOD_DEV.Contains(plr.SteamId) && !plr.KnownUserTagIcons.Contains(CoopModAssets.mpmod_icon_green.texture))
			{
				plr.KnownUserTagIcons.Add(CoopModAssets.mpmod_icon_green.texture);
			}
		}
	}

	public static bool CanDoFancyNametagFor(NetPlayer plr)
	{
		if (!Net.IsRunningSteam)
		{
			return false;
		}
		if (STARS.Contains(plr.SteamId))
		{
			return true;
		}
		if (IsSteamUserPrivileged(plr.SteamId))
		{
			return true;
		}
		return false;
	}

	private static void LoadEverything()
	{
		try
		{
			AddStars(CoopModAssets.ReadResource(fileName).ToStringFromCharBytes());
		}
		catch (Exception)
		{
		}
		if (!IsFileMissingOrOlderThanOneDay(filePath))
		{
			try
			{
				LoadTheFile();
			}
			catch (Exception)
			{
			}
		}
	}

	private static void LoadTheFile()
	{
		AddStars(File.ReadAllText(filePath));
	}

	private static void AddStars(string encrypted)
	{
		ulong[] array = ReadFile(encrypted);
		LinqUtility.AddRange<ulong>((ICollection<ulong>)STARS, (IEnumerable<ulong>)array);
		foreach (NetPlayer value in NetPlayer.ClientIdToPlayerDict.Values)
		{
			if ((Object)(object)value.body != (Object)null && CanDoFancyNametagFor(value))
			{
				CharStatusVisuals.MakeNametagFancy((TMP_Text)(object)value.playerbody.visual.nametag.GetComponent<TextMeshPro>(), value);
			}
			LoadTagsFor(value);
		}
	}

	public static async Task DownloadFileAsync(string url, string destinationPath)
	{
		HttpClient httpClient = new HttpClient();
		string directoryName = Path.GetDirectoryName(destinationPath);
		if (!string.IsNullOrEmpty(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}
		double retrytimer = 3.0;
		while (true)
		{
			try
			{
				HttpResponseMessage response = await httpClient.GetAsync(url, (HttpCompletionOption)1);
				try
				{
					response.EnsureSuccessStatusCode();
					string text = await response.Content.ReadAsStringAsync();
					AddStars(text);
					using FileStream outputStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None);
					byte[] uTF8Bytes = text.GetUTF8Bytes();
					await outputStream.WriteAsync(uTF8Bytes, 0, uTF8Bytes.Length);
					break;
				}
				finally
				{
					((IDisposable)response)?.Dispose();
				}
			}
			catch (Exception ex)
			{
				_ = ex;
				await Task.Delay(TimeSpan.FromSeconds(retrytimer));
				retrytimer *= 2.0;
			}
		}
	}

	public static bool IsFileMissingOrOlderThanOneDay(string filePath)
	{
		if (!File.Exists(filePath))
		{
			return true;
		}
		return File.GetLastWriteTimeUtc(filePath) < DateTime.UtcNow.AddDays(-1.0);
	}

	public static ulong[] ReadFile(string base64Data)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		List<ulong> list = new List<ulong>();
		try
		{
			using MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(base64Data));
			memoryStream.ReadByte();
			memoryStream.ReadByte();
			using DeflateStream stream = new DeflateStream(memoryStream, CompressionMode.Decompress);
			using StreamReader streamReader = new StreamReader(stream);
			string text = streamReader.ReadToEnd();
			try
			{
				ulong[] collection = ((JToken)(JArray)JObject.Parse(text).GetValue("steam_ids")).ToObject<ulong[]>();
				list.AddRange(collection);
			}
			catch (Exception arg)
			{
				log.error($"Failed to read stars json: {text}\n{arg}");
			}
		}
		catch (Exception ex)
		{
			log.error("StarReadFile: " + base64Data + "\n" + ex.ToString());
		}
		return list.ToArray();
	}
}
