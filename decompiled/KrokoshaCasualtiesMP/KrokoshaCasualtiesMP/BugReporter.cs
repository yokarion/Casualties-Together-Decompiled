using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using BepInEx;
using DiscordRPC;
using HarmonyLib;
using KrokoshaCasualtiesMultiplayerAPI;
using KrokoshaCasualtiesUtils;
using Newtonsoft.Json;
using Steamworks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace KrokoshaCasualtiesMP;

public class BugReporter : KrokoshaScavSingleton
{
	[SettingDeclarerThingyBool("setting_bugreport_includelogprev", allow_saving = false)]
	public static bool INCLUDELOG_PREV = true;

	public static bool HIDEUI_FORSCREENSHOT = true;

	[SettingDeclarerThingyBool("setting_bugreport_delayscreenshot", allow_saving = false)]
	public static bool DELAY_SCREENSHOT = false;

	public static bool SEND_SCREENSHOT = true;

	private const string WebhookUrl = "https://discord.com/api/webhooks/1498261505517289472/01ASQ2l9bMnvwLtRwNDVqEu8NzaqIdzH_Tp-QCknkJL4kwRzBB6sgJuF_U0fQgYriCep";

	public static bool FORCE_GZIP = false;

	public static string user_bugreport_message = "\nwhat happened: \nreproduction steps: \n";

	public static int user_bugreport_message_size_limit = 1000;

	private const int MAX_SIZE_PER_FILE = 1700000;

	private const int bugreport_cooldown = 60;

	internal static double last_bugreport_time = 0.0;

	public static float bugreport_time => (float)(Time.realtimeSinceStartupAsDouble - last_bugreport_time);

	private void Awake()
	{
	}

	internal static void _GUI___SettingsBugReport(Rect r)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Expected O, but got Unknown
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		GUI.skin.label.alignment = (TextAnchor)0;
		bool wordWrap = GUI.skin.textField.wordWrap;
		GUI.skin.textField.wordWrap = true;
		float num = GUI.skin.textField.CalcHeight(new GUIContent(user_bugreport_message), ((Rect)(ref r)).width);
		user_bugreport_message = GUILayout.TextArea(user_bugreport_message, (GUILayoutOption[])(object)new GUILayoutOption[2]
		{
			GUILayout.Width(((Rect)(ref r)).width),
			GUILayout.Height(Mathf.Max(num, UIMainMenu.GetMenuUIScale() * 100f))
		});
		GUI.skin.textField.wordWrap = wordWrap;
		if (user_bugreport_message.Length > user_bugreport_message_size_limit)
		{
			user_bugreport_message = user_bugreport_message.Substring(0, user_bugreport_message_size_limit);
		}
		bool enabled = GUI.enabled;
		bool flag = bugreport_time > 60f;
		if (enabled)
		{
			GUI.enabled = flag;
		}
		if (GUILayout.Button(Lang.Get("mmbr_send", false), Array.Empty<GUILayoutOption>()))
		{
			Util.StartCoroutine(BugreportUploadSequence());
		}
		if (!flag)
		{
			if (UIBullshit.CheckCursorOverlap(GUIUtility.GUIToScreenRect(GUILayoutUtility.GetLastRect())))
			{
				UIBullshit._GUI_SetTooltip($"Wait {Mathf.Round(60f - bugreport_time)}s", "");
			}
		}
		else if (UIBullshit.CheckCursorOverlap(GUIUtility.GUIToScreenRect(GUILayoutUtility.GetLastRect())))
		{
			UIBullshit._GUI_SetTooltip(Lang.Get("mmbr_send_tooltip", false), Lang.Get("mmbr_send_tooltip_desc", false));
		}
		GUI.enabled = enabled;
		GUILayout.Label(Lang.Get("mmbr_terms", false), Array.Empty<GUILayoutOption>());
	}

	private static void AddFileToFormAndCompressIfLarge(in WWWForm form, byte[] data, string filename, string fieldname, string TOSENDfilename)
	{
		if (data.Length > 1700000 || FORCE_GZIP)
		{
			data = Util.Compress(data);
			if (data.Length < 1700000)
			{
				form.AddBinaryData(fieldname, data, TOSENDfilename + ".gz", "application/octet-stream");
			}
			else
			{
				log.error("BUGREPORTER: " + filename + " is too large!");
			}
		}
		else
		{
			form.AddBinaryData(fieldname, data, TOSENDfilename);
		}
	}

	private static void AddLogFileToForm(in WWWForm form, string path, string append_text, string filename, string fieldname, string TOSENDfilename)
	{
		if (File.Exists(path))
		{
			if (TryReadFullFile(path, out var bytes, out var error))
			{
				string input = Encoding.UTF8.GetString(bytes);
				input = DetectRepeatedLines(input);
				bytes = Encoding.UTF8.GetBytes(input);
				AddFileToFormAndCompressIfLarge(in form, bytes, fieldname, fieldname, TOSENDfilename);
			}
			else
			{
				log.error("BUGREPORTER: Failed to read " + filename + ": " + error);
			}
		}
	}

	internal static IEnumerator BugreportUploadSequence()
	{
		bool ogopen = UIMainMenu.mainmenu_open;
		_ = Con._DEV_HIDEHUD;
		bool passes_time_check = bugreport_time > 60f;
		last_bugreport_time = Time.realtimeSinceStartupAsDouble;
		if (HIDEUI_FORSCREENSHOT)
		{
			UIMainMenu.SetOpen(open_or_nah: false);
			yield return null;
		}
		byte[] imageData = null;
		if (SEND_SCREENSHOT)
		{
			if (DELAY_SCREENSHOT)
			{
				if (Util.IsInWorld())
				{
					Util.DoAlert("Press F1 to capture screenshot\nand send bug report.", false);
				}
				while (!Input.GetKeyDown((KeyCode)282))
				{
					last_bugreport_time = Time.realtimeSinceStartupAsDouble;
					yield return (object)new WaitForEndOfFrame();
				}
			}
			yield return (object)new WaitForEndOfFrame();
			Texture2D val = ScreenCapture.CaptureScreenshotAsTexture();
			if ((Object)(object)val != (Object)null)
			{
				imageData = ImageConversion.EncodeToJPG(val, 80);
				Object.Destroy((Object)(object)val);
			}
			yield return null;
		}
		WWWForm form = new WWWForm();
		bool flag;
		try
		{
			string simplifiedName = GetSimplifiedName();
			string path = Path.Combine(Application.persistentDataPath, "Player.log");
			string path2 = Path.Combine(Application.persistentDataPath, "Player-prev.log");
			string text = BuildReportText(user_bugreport_message);
			form.AddField("content", text);
			form.AddField("username", simplifiedName);
			string text2 = null;
			if (Util.TryGetLocalDiscordUser(out var user))
			{
				text2 = user.GetAvatarURL((AvatarFormat)0);
			}
			else if (KSteam.Loaded)
			{
				text2 = "https://unavatar.io/steam/profile:" + ((object)KSteam.GetLocalUserSteamID()/*cast due to constrained. prefix*/).ToString();
			}
			if (text2 != null)
			{
				form.AddField("avatar_url", text2);
			}
			int num = 0;
			if (imageData != null && imageData.Length < 1700000)
			{
				form.AddBinaryData($"file[{num++}]", imageData, "SCR_" + simplifiedName + "_" + DateTime.UtcNow.ToString("dd-MM-yyyy_HH-mm-ss") + ".jpg", "image/jpeg");
			}
			List<SettingSaveData> list = new List<SettingSaveData>();
			foreach (Setting setting in Settings.settings)
			{
				list.Add(new SettingSaveData
				{
					name = setting.name,
					value = setting.GetValue()
				});
			}
			string text3 = JsonConvert.SerializeObject((object)list, (Formatting)1);
			string s = JsonConvert.SerializeObject((object)KrokoshaScavMultiplayer.rules, (Formatting)1) + "\n" + text3;
			byte[] bytes = Encoding.UTF8.GetBytes(s);
			form.AddBinaryData($"file[{num++}]", bytes, "RULES_" + simplifiedName + ".json", "text/plain");
			string text4 = string.Join("\n", ConsoleScript.instance.logs);
			text4 = text4 + "\n\n\n" + Net.GetDebugStatsString() + "\n";
			text4 = new Regex("<[^>]*>").Replace(text4, string.Empty);
			byte[] bytes2 = Encoding.UTF8.GetBytes(text4);
			string text5 = "COPYLOG_" + simplifiedName + ".log";
			AddFileToFormAndCompressIfLarge(in form, bytes2, text5, $"file[{num++}]", text5);
			if (Util.IsInWorld())
			{
				string s2 = string.Join("\n\n============================================================================\n\n", NetPlayer.BodyToPlayerDict.Values.Select((NetPlayer x) => ((object)x).ToString() + " - " + x.body.DumpBodyVars()));
				byte[] bytes3 = Encoding.UTF8.GetBytes(s2);
				string text6 = "BODYVARS_" + simplifiedName + ".txt";
				AddFileToFormAndCompressIfLarge(in form, bytes3, text6, $"file[{num++}]", text6);
			}
			string text7 = $"SENDING BUG REPORT:\n{text}\nFILES ATTACHED: {num}";
			AddLogFileToForm(in form, path, text7, "Player.log", $"file[{num++}]", "LOG_" + simplifiedName + ".log");
			if (INCLUDELOG_PREV)
			{
				AddLogFileToForm(in form, path2, text7, "Player-prev.log", $"file[{num}]", "PREVLOG_" + simplifiedName + ".log");
			}
			log.l(text7);
			flag = true;
		}
		catch (Exception ex)
		{
			flag = false;
			log.error("BUGREPORTER: failed while writing web request: " + ex.ToString());
		}
		last_bugreport_time = Time.realtimeSinceStartupAsDouble;
		if (flag && passes_time_check)
		{
			log.l("BUGREPORTER: Sending report...");
			UnityWebRequest www = UnityWebRequest.Post("https://discord.com/api/webhooks/1498261505517289472/01ASQ2l9bMnvwLtRwNDVqEu8NzaqIdzH_Tp-QCknkJL4kwRzBB6sgJuF_U0fQgYriCep", form);
			try
			{
				yield return www.SendWebRequest();
				if ((int)www.result != 1)
				{
					log.error("BUGREPORTER: Bug report send failed: " + www.error);
				}
				else
				{
					log.l("BUGREPORTER: Bug report sent successfully!");
					user_bugreport_message = "";
				}
			}
			finally
			{
				((IDisposable)www)?.Dispose();
			}
		}
		yield return null;
		UIMainMenu.SetOpen(ogopen);
		last_bugreport_time = Time.realtimeSinceStartupAsDouble;
	}

	private unsafe static string BuildReportText(string userMessage)
	{
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		//IL_0456: Unknown result type (might be due to invalid IL or missing references)
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("# KrokMP Bug Report `GAME: " + Application.version + " MOD: 4.0.1`");
		stringBuilder.AppendLine("```VERBOSE: " + KrokoshaScavMultiplayer.verbose);
		try
		{
			stringBuilder.AppendLine("ARGV: " + string.Join(" ", Environment.GetCommandLineArgs()));
			stringBuilder.AppendLine("PLUGINS: " + string.Join(", ", KrokoshaScavMultiplayer.GetModList().Select(delegate(PluginInfo x)
			{
				BepInPlugin metadata = x.Metadata;
				string arg = ((metadata != null) ? metadata.GUID : null);
				BepInPlugin metadata2 = x.Metadata;
				return $"{arg} {((metadata2 != null) ? metadata2.Version : null)}";
			})));
		}
		catch (Exception)
		{
		}
		stringBuilder.AppendLine("Utc: " + DateTime.UtcNow.ToString("dd-MM-yyyy HH:mm:ss 'UTC'"));
		stringBuilder.AppendLine("Local: " + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"));
		stringBuilder.AppendLine("TimeSinceStartup: " + TimeSpan.FromSeconds(Time.realtimeSinceStartupAsDouble).ToString("hh\\:mm\\:ss"));
		stringBuilder.AppendLine("TotalRunTime: " + TimeSpan.FromSeconds(WorldGeneration.TotalRunTime()).ToString("hh\\:mm\\:ss"));
		string text = $"FPS: {ServerMain.CURRENT_FPS}  TPS:{ServerMain.CURRENT_TPS}";
		if (Net.running)
		{
			text = ((!Net.is_client) ? (text + $"  AVG_PING:{ServerMain.AVERAGE_PING}") : (text + $"  AVG_PING:{ServerMain.AVERAGE_PING}  PING:{ClientMain.LOCAL_PING}"));
		}
		stringBuilder.AppendLine(text);
		stringBuilder.AppendLine($"Net.running: {Net.running}");
		stringBuilder.AppendLine($"Net.type: {Net.type}");
		if (KSteam.Loaded)
		{
			stringBuilder.AppendLine($"Steam Persona: {KSteam.GetLocalUsername()} - {KSteam.GetLocalUserSteamID()}");
		}
		if (Util.TryGetLocalDiscordUser(out var user))
		{
			stringBuilder.AppendLine("Discord User: " + RPCManager_Initialize_MultiplayerPatch.BetterDiscordUserToString(user));
		}
		stringBuilder.AppendLine("Base Username: " + KrokoshaScavMultiplayer.INPUT_USERNAME);
		if (Net.TryGetSteamTransport(out var tsteam))
		{
			stringBuilder.AppendLine("Steam IS_IN_LOBBY: " + KSteam.IS_IN_LOBBY);
			stringBuilder.AppendLine("Steam I am the owner: " + tsteam.i_am_owner_of_this_lobby);
			CSteamID lobby_steamID = KSteam.CURRENT_LOBBY.lobby_steamID;
			stringBuilder.AppendLine("Steam Lobby ID: " + ((object)(*(CSteamID*)(&lobby_steamID))/*cast due to constrained. prefix*/).ToString());
			lobby_steamID = KSteam.CURRENT_LOBBY.ownerID;
			stringBuilder.AppendLine("Steam Lobby Owner User SteamID: " + ((object)(*(CSteamID*)(&lobby_steamID))/*cast due to constrained. prefix*/).ToString());
			stringBuilder.AppendLine($"Steam Lobby Members {KSteam.CURRENT_LOBBY.members.Count}/{KSteam.CURRENT_LOBBY.memberlimit}: " + string.Join(", ", KSteam.CURRENT_LOBBY.members.Select((KeyValuePair<CSteamID, KSteam.LobbyMember> x) => TransportSteamworks.BetterToStringFromSteamID(x.Key))));
		}
		Scene activeScene = SceneManager.GetActiveScene();
		stringBuilder.AppendLine("Scene: " + (((Scene)(ref activeScene)).name ?? "").Trim());
		if (Util.IsInWorld())
		{
			int num = WorldGeneration.world.biomeDepth + 1;
			stringBuilder.AppendLine(string.Format("Layer: {0} - {1}", num, GeneralExtensions.GetValueSafe<string, string>(Locale.currentLang.other, "layertitle" + num) ?? ""));
			stringBuilder.AppendLine("Depth: " + WorldGeneration.world.TotalDepthString());
		}
		if (GamemodeManager.HasGamemode())
		{
			stringBuilder.AppendLine("Gamemode: " + ((object)GamemodeManager.GetGamemode()).GetType().Name);
		}
		stringBuilder.AppendLine("StatusMessage: " + KrokoshaScavMultiplayer.multiplayer_status_message + "```");
		if (KSteam.Loaded)
		{
			stringBuilder.AppendLine("https://steamcommunity.com/profiles/" + ((object)KSteam.GetLocalUserSteamID()/*cast due to constrained. prefix*/).ToString());
		}
		stringBuilder.AppendLine("# UserMessage:");
		stringBuilder.AppendLine("```" + userMessage.Replace("```", "") + "```");
		string text2 = stringBuilder.ToString();
		if (text2.Length > 1800)
		{
			text2 = text2.Substring(0, 1799);
			log.error("BUGREPORTER: Message Content is too large! trimming....");
		}
		return text2;
	}

	private static string _RemoveLogLineTimestamp(string input)
	{
		return Regex.Replace(input, ":Krokosha_MP_CU\\] \\[\\d{2}:\\d{2}:\\d{2}\\.\\d{3}\\]", "");
	}

	private static string _DetectRepeatedLines(string input, string linebegin)
	{
		if (string.IsNullOrEmpty(input))
		{
			return input;
		}
		string[] array = input.Split(new string[1] { "\n" + linebegin }, StringSplitOptions.RemoveEmptyEntries);
		StringBuilder stringBuilder = new StringBuilder();
		string text = null;
		string text2 = null;
		int num = 0;
		string[] array2 = array;
		foreach (string text3 in array2)
		{
			string text4 = linebegin + text3;
			string text5 = _RemoveLogLineTimestamp(text4);
			if (text2 == text5)
			{
				num++;
				continue;
			}
			if (text != null)
			{
				stringBuilder.Append(text);
				if (num > 1)
				{
					stringBuilder.AppendLine($"[x{num - 1}]");
				}
			}
			text = text4;
			text = _RemoveLogLineTimestamp(text);
			num = 1;
		}
		if (text != null)
		{
			stringBuilder.Append(text);
			if (num > 1)
			{
				stringBuilder.AppendLine($"[x{num - 1}]");
			}
		}
		return stringBuilder.ToString().TrimEnd(Array.Empty<char>());
	}

	public static string DetectRepeatedLines(string input)
	{
		List<string> list = _DetectRepeatedLines(_DetectRepeatedLines(_DetectRepeatedLines(_DetectRepeatedLines(input, "["), "NullReferenceException"), "ArgumentException:"), "Sprite Tiling might not appear correctly because the Sprite used is not generated with Full Rect. To fix this, change the Mesh Type in the Sprite's import setting to Full Rect").Split(new string[1] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
		list.RemoveAll((string x) => x.StartsWith("The character with Unicode value \\u"));
		return string.Join("\n", list);
	}

	private static bool TryReadFullFile(string path, out byte[] bytes, out string error)
	{
		bytes = null;
		error = "";
		try
		{
			using FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			if (fileStream.Length <= 0)
			{
				error = "empty file";
				return false;
			}
			bytes = new byte[fileStream.Length];
			int i;
			int num;
			for (i = 0; i < bytes.Length; i += num)
			{
				num = fileStream.Read(bytes, i, bytes.Length - i);
				if (num <= 0)
				{
					break;
				}
			}
			if (i != bytes.Length)
			{
				error = "incomplete read";
				bytes = null;
				return false;
			}
			return true;
		}
		catch (Exception ex)
		{
			error = ex.GetBaseException().Message;
			bytes = null;
			return false;
		}
	}

	private static string SimplifyString(string text)
	{
		return new string((from x in KrokoshaScavMultiplayer.SanitizeTextInput(text)
			where char.IsLetterOrDigit(x)
			select x).ToArray()).Trim();
	}

	private static string GetSimplifiedName()
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			string text = "";
			if (Util.TryGetLocalDiscordUser(out var user))
			{
				text = SimplifyString(user.DisplayName) + "-" + user.ID;
			}
			else if (KSteam.Loaded)
			{
				text = SimplifyString(KSteam.GetLocalUsername()) + "-" + ((object)KSteam.GetLocalUserSteamID()/*cast due to constrained. prefix*/).ToString();
			}
			else if (string.IsNullOrWhiteSpace(text))
			{
				text = SimplifyString(KrokoshaScavMultiplayer.INPUT_USERNAME);
			}
			if (string.IsNullOrWhiteSpace(text))
			{
				text = "noname";
			}
			return text;
		}
		catch
		{
			return "namebroke";
		}
	}
}
