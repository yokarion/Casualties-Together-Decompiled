using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DiscordRPC;
using HarmonyLib;
using KrokoshaCasualtiesMP;
using TMPro;
using UnityEngine;

namespace KrokoshaCasualtiesUtils;

public static class Util
{
	private class CoroutineRunner : MonoBehaviour
	{
		private static CoroutineRunner cr;

		public static CoroutineRunner get()
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Expected O, but got Unknown
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)cr == (Object)null)
			{
				GameObject val = new GameObject("KrokoshaCoroutineRunner");
				Object.DontDestroyOnLoad((Object)val);
				cr = val.AddComponent<CoroutineRunner>();
				((Object)val).hideFlags = (HideFlags)61;
			}
			return cr;
		}

		public static void run(IEnumerator f)
		{
			((MonoBehaviour)get()).StartCoroutine(f);
		}
	}

	private static Dictionary<UISoundType, string> _PlayUISound_NAMES = new Dictionary<UISoundType, string>
	{
		{
			(UISoundType)0,
			"click"
		},
		{
			(UISoundType)1,
			"miniClick"
		},
		{
			(UISoundType)2,
			"close"
		},
		{
			(UISoundType)3,
			"woundon"
		},
		{
			(UISoundType)4,
			"menuOpen"
		},
		{
			(UISoundType)5,
			"menuClose"
		},
		{
			(UISoundType)6,
			"warning"
		},
		{
			(UISoundType)7,
			"time"
		}
	};

	public static WorldGeneration world => WorldGeneration.world;

	public static ushort[,] worldBlocks => WorldGeneration.world.worldBlocks;

	public static byte[,] fluid => FluidManager.main.fluid;

	public static AudioSource PlayWorldSoundOnScreenIfInRange(in string snd, in Vector2 pos, in float volume = 1f, in float pitch = 1f, in float range = 64f)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (KM.dist2dsqrcheck(in pos, Vector2.op_Implicit(((Component)Camera.main).transform.position), range))
		{
			AudioSource val = Sound.Play(snd, pos, false, true, (Transform)null, volume, pitch, false, false);
			if ((Object)(object)val != (Object)null)
			{
				val.maxDistance = range;
				val.minDistance = range * 0.4f;
				val.rolloffMode = (AudioRolloffMode)1;
				return val;
			}
		}
		return null;
	}

	public static ulong GetFileHash(string filePath)
	{
		ulong num = 14695981039346656037uL;
		byte[] array = new byte[8192];
		using FileStream fileStream = File.OpenRead(filePath);
		int num2;
		while ((num2 = fileStream.Read(array, 0, array.Length)) > 0)
		{
			for (int i = 0; i < num2; i++)
			{
				num ^= array[i];
				num *= 1099511628211L;
			}
		}
		return num;
	}

	public static void CallLambdaWhenWorldGenerates(Delegate func)
	{
		StartCoroutine(_CoroutineCallLambdaWhenWorldGenerated(func));
	}

	private static IEnumerator _CoroutineCallLambdaWhenWorldGenerated(Delegate func)
	{
		while (!KrokoshaScavMultiplayer.IsInGameAndWorldGenerated())
		{
			yield return null;
		}
		func.DynamicInvoke();
	}

	public static void CallLambdaWhenWorldLoads(Delegate func)
	{
		StartCoroutine(_CoroutineCallLambdaWhen(() => IsInWorld() && worldBlocks != null, func));
	}

	public static void CallLambdaWhenWorldFirstGenerates(Delegate func)
	{
		StartCoroutine(_CoroutineCallLambdaWhen(IsWorldGenerated, func));
	}

	public static void CallLambdaWhen(Func<bool> condition, Delegate func)
	{
		StartCoroutine(_CoroutineCallLambdaWhen(condition, func));
	}

	public static void CallLambdaWhen(Func<bool> condition, Delegate func, float check_repeat_time = 0.1f)
	{
		StartCoroutine(_CoroutineCallLambdaWhen(condition, func, check_repeat_time));
	}

	internal static IEnumerator _CoroutineCallLambdaWhen(Func<bool> condition, Delegate func)
	{
		while (!condition())
		{
			yield return null;
		}
		func.DynamicInvoke();
	}

	private static IEnumerator _CoroutineCallLambdaWhen(Func<bool> condition, Delegate func, float check_repeat_time)
	{
		while (!condition())
		{
			yield return (object)new WaitForSecondsRealtime(check_repeat_time);
		}
		func.DynamicInvoke();
	}

	public static void DelayCallLambda(float delay_sec, Delegate func)
	{
		StartCoroutine(_DelayCallLambdaCoroutine(delay_sec, func));
	}

	internal static IEnumerator _DelayCallLambdaCoroutine(float delay_sec, Delegate func)
	{
		yield return (object)new WaitForSecondsRealtime(delay_sec);
		func.DynamicInvoke();
	}

	public static void StartCoroutine(Func<IEnumerator> f)
	{
		CoroutineRunner.run(f());
	}

	public static void StartCoroutine(IEnumerator f)
	{
		CoroutineRunner.run(f);
	}

	public static Task StartCoroutineAsync(IEnumerator coroutine)
	{
		TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();
		((MonoBehaviour)CoroutineRunner.get()).StartCoroutine(RunCoroutineForAsync(coroutine, taskCompletionSource));
		return taskCompletionSource.Task;
	}

	private static IEnumerator RunCoroutineForAsync(IEnumerator coroutine, TaskCompletionSource<object> tcs)
	{
		yield return coroutine;
		if (!tcs.Task.IsCompleted)
		{
			tcs.SetResult(null);
		}
	}

	public static bool IsWorldGenerated()
	{
		if (Object.op_Implicit((Object)(object)WorldGeneration.world) && !WorldGeneration.world.generatingWorld)
		{
			return true;
		}
		return false;
	}

	public static bool IsWorldInstantiated()
	{
		if (Object.op_Implicit((Object)(object)WorldGeneration.world) && !WorldGeneration.world.instantiatingWorld)
		{
			return true;
		}
		return false;
	}

	public static bool IsGeneratingWorld()
	{
		if (Object.op_Implicit((Object)(object)WorldGeneration.world))
		{
			return WorldGeneration.world.generatingWorld;
		}
		return false;
	}

	public static bool IsTutorialWorld()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Invalid comparison between Unknown and I4
		if ((Object)(object)WorldGeneration.world != (Object)null && (int)WorldGeneration.world.biomeOverride == 1)
		{
			return true;
		}
		return false;
	}

	public static bool IsInWorld()
	{
		if ((Object)(object)WorldGeneration.world != (Object)null)
		{
			return true;
		}
		return false;
	}

	public static bool IsInMainMenu()
	{
		if ((Object)(object)WorldGeneration.world == (Object)null)
		{
			return true;
		}
		return false;
	}

	public static bool IsInPauseMenuOrMainMenu()
	{
		if (IsInWorld() && (Object)(object)PauseHandler.main != (Object)null)
		{
			if (PauseHandler.main.isPaused)
			{
				return true;
			}
			return false;
		}
		return true;
	}

	public static bool IsInGameAndPauseMenu()
	{
		if (IsInWorld() && (Object)(object)PauseHandler.main != (Object)null)
		{
			if (PauseHandler.main.isPaused)
			{
				return true;
			}
			return false;
		}
		return false;
	}

	public static void OpenBrightnessPanel(bool open_or_nah)
	{
		if ((Object)(object)PauseHandler.main != (Object)null && PauseHandler.main.isPaused != open_or_nah)
		{
			PauseHandler.TogglePause();
		}
	}

	public static void OpenSettingsPanel(bool open_or_nah, SettingCategory category = (SettingCategory)0)
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		if (open_or_nah)
		{
			if ((Object)(object)SettingsMenu.instance == (Object)null)
			{
				SettingsMenu.OpenMenu(IsInMainMenu() ? ((Component)PreRunScript.instance.mainCanvas).transform : ((Component)PauseHandler.main).transform);
			}
			SettingsMenu_Start_Patch.forceOpenCustomTab = true;
			if (SettingsMenu.instance.spawnedSettings == null)
			{
				SettingsMenu.instance.spawnedSettings = new List<GameObject>();
			}
			SettingsMenu.instance.SelectTab(category);
		}
		else if ((Object)(object)SettingsMenu.instance != (Object)null)
		{
			SettingsMenu.instance.Close();
		}
	}

	public static bool IsInWorldBoundaries(in Vector2 pos)
	{
		if (Math.Abs(pos.x) < (float)world.width * 0.5f)
		{
			return Math.Abs(pos.y) < (float)world.height * 0.5f;
		}
		return false;
	}

	public static float TilesToMeters(float tiles_distance)
	{
		return tiles_distance * 0.3f;
	}

	public static float MetersToTiles(float meters_distance)
	{
		return meters_distance / 0.3f;
	}

	public static void BlockFill(int x, int y, int endx, int endy, ushort block_id)
	{
		if ((Object)(object)world != (Object)null)
		{
			if (x > endx)
			{
				x = endx;
				endx = x;
			}
			if (y > endy)
			{
				y = endy;
				endy = y;
			}
			x = Math.Max(x, 0);
			y = Math.Max(y, 0);
			BlockFillUnsafe(x, y, Math.Min(endx, (int)WorldGeneration.world.width), Math.Min(endy, (int)WorldGeneration.world.height), block_id);
		}
	}

	public static void BlockFillUnsafe(int startX, int startY, int endX, int endY, ushort block_id)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		ushort[,] array = worldBlocks;
		for (int i = startX; i <= endX; i++)
		{
			for (int j = startY; j <= endY; j++)
			{
				array[i, j] = block_id;
			}
		}
		if (world.generatingWorld)
		{
			return;
		}
		for (int k = startX / WorldGeneration.CHUNKSIZE; k <= endX / WorldGeneration.CHUNKSIZE; k++)
		{
			for (int l = startY / WorldGeneration.CHUNKSIZE; l <= endY / WorldGeneration.CHUNKSIZE; l++)
			{
				WorldGeneration.world.UpdateChunk(new Vector2Int(k, l));
			}
		}
	}

	public static void RegisterLayerModifier<T>() where T : LayerModifier
	{
		object? obj = Activator.CreateInstance(typeof(T), LayerModifier.availableModifiers.Length);
		LayerModifier val = (LayerModifier)((obj is LayerModifier) ? obj : null);
		LayerModifier.availableModifiers = CollectionExtensions.AddToArray<LayerModifier>(LayerModifier.availableModifiers, val);
	}

	public static void RegisterKeybind(string key, KeyCode code)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Settings_DefaultSettings_Patch.custom_binds[key] = code;
	}

	public static bool IsLocalBodyAlive()
	{
		PlayerCamera main = PlayerCamera.main;
		bool? obj;
		if (main == null)
		{
			obj = null;
		}
		else
		{
			Body body = main.body;
			obj = ((body != null) ? new bool?(body.alive) : ((bool?)null));
		}
		bool? flag = obj;
		return flag == true;
	}

	public static Body GetLocalBody()
	{
		return PlayerCamera.main.body;
	}

	public static Body GetLocalBodyNullable()
	{
		return PlayerCamera.main?.body;
	}

	public static bool TryGetLocalBody(out Body body)
	{
		if ((Object)(object)PlayerCamera.main == (Object)null)
		{
			body = null;
			return false;
		}
		body = PlayerCamera.main.body;
		return (Object)(object)body != (Object)null;
	}

	public static bool IsBodyLocal(Body body)
	{
		return (Object)(object)PlayerCamera.main?.body == (Object)(object)body;
	}

	public static bool IsBodyLocal(Limb limb)
	{
		return limb.body.IsBodyLocal();
	}

	public static bool IsUnchipped()
	{
		if (IsInWorld())
		{
			return WorldGeneration.unchipped;
		}
		return (bool)WorldgenPatches.runsettings.GetValueSafe("unchipped", false);
	}

	public static void DoAlert(in string text, in bool important = false)
	{
		PlayerCamera.main.DoAlert(text, important);
	}

	public static void PlayUISound(UISoundType t, float pitch = 1f, float volume = 1f)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		string obj = GeneralExtensions.GetValueSafe<UISoundType, string>(_PlayUISound_NAMES, t) ?? "warning";
		Sound_Play_MultiplayerPatch.force = true;
		Sound.Play(obj, Vector2.zero, true, false, (Transform)null, volume, pitch, true, true);
	}

	public static bool WoundViewAlertIsShown()
	{
		if (IsInWoundView())
		{
			return WoundView.view.criticalPanel.activeInHierarchy;
		}
		return false;
	}

	public static void WoundViewShowAlertText(in string msg)
	{
		if (IsInWoundView())
		{
			WoundView.view.criticalPanel.SetActive(true);
			TextMeshProUGUI componentInChildren = WoundView.view.criticalPanel.GetComponentInChildren<TextMeshProUGUI>(true);
			if (Object.op_Implicit((Object)(object)componentInChildren))
			{
				((Behaviour)componentInChildren).enabled = Mathf.Sin(Time.unscaledTime * 20f) > 0f;
				((TMP_Text)componentInChildren).text = msg;
			}
		}
	}

	public static bool IsInWoundView()
	{
		if ((Object)(object)PlayerCamera.main != (Object)null)
		{
			return PlayerCamera.main.woundView.activeSelf;
		}
		return false;
	}

	public static bool IsInConsole()
	{
		if (Object.op_Implicit((Object)(object)ConsoleScript.instance))
		{
			return ConsoleScript.instance.active;
		}
		return false;
	}

	public static bool IsInCraftingMenu()
	{
		if ((Object)(object)PlayerCamera.main != (Object)null)
		{
			return PlayerCamera.main.craftingPanel.activeSelf;
		}
		return false;
	}

	public static void CreatePickupLine(Vector2 from, Vector2 to)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		GameObject obj = Utils.Create("Special/PickupLine", Vector2.zero, 0f);
		obj.GetComponent<LineRenderer>().SetPosition(0, Vector2.op_Implicit(from));
		obj.GetComponent<LineRenderer>().SetPosition(1, Vector2.op_Implicit(to));
		Object.Destroy((Object)(object)obj, 0.5f);
	}

	public static bool QuickGroundLinecastCheck(in Vector2 from, in Vector2 to)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		return RaycastHit2D.op_Implicit(Physics2D.Linecast(from, to, LayerMask.GetMask(new string[1] { "Ground" })));
	}

	public static bool QuickRaycastInteractionCheck(Vector2 from, Vector2 to, bool do_effect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		bool flag = RaycastHit2D.op_Implicit(Physics2D.Linecast(from, to, LayerMask.GetMask(new string[1] { "Ground" })));
		if (flag && (Object.op_Implicit((Object)(object)Physics2D.OverlapPoint(from, LayerMask.GetMask(new string[1] { "Ground" }))) || Object.op_Implicit((Object)(object)Physics2D.OverlapPoint(to, LayerMask.GetMask(new string[1] { "Ground" })))))
		{
			flag = false;
		}
		if (flag && do_effect)
		{
			CreatePickupLine(from, to);
		}
		return !flag;
	}

	public static bool AccurateRaycastInteractionCheck(Body who, Vector2 target, bool do_effect)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		bool flag = RaycastHit2D.op_Implicit(Physics2D.Linecast(Vector2.op_Implicit(((Component)who.GetLowerTorso()).transform.position), target, LayerMask.GetMask(new string[1] { "Ground" })));
		if (flag)
		{
			flag = RaycastHit2D.op_Implicit(Physics2D.Linecast(Vector2.op_Implicit(((Component)who.GetHead()).transform.position), target, LayerMask.GetMask(new string[1] { "Ground" })));
		}
		if (flag && do_effect)
		{
			CreatePickupLine(Vector2.op_Implicit(((Component)who).transform.position), target);
		}
		return !flag;
	}

	public static bool AccurateRaycastInteractionCheckObstruction(Body who, Body target, bool do_effect)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)who == (Object)(object)target)
		{
			return true;
		}
		int mask = LayerMask.GetMask(new string[1] { "Ground" });
		bool flag = RaycastHit2D.op_Implicit(Physics2D.Linecast(Vector2.op_Implicit(((Component)who.GetHead()).transform.position), Vector2.op_Implicit(((Component)target.GetHead()).transform.position), mask));
		if (flag)
		{
			flag = RaycastHit2D.op_Implicit(Physics2D.Linecast(Vector2.op_Implicit(((Component)who.GetLowerTorso()).transform.position), Vector2.op_Implicit(((Component)target.GetLowerTorso()).transform.position), mask));
			if (flag)
			{
				flag = RaycastHit2D.op_Implicit(Physics2D.Linecast(Vector2.op_Implicit(((Component)who.GetHead()).transform.position), Vector2.op_Implicit(((Component)target.GetLowerTorso()).transform.position), mask));
				if (flag)
				{
					flag = RaycastHit2D.op_Implicit(Physics2D.Linecast(Vector2.op_Implicit(((Component)who.GetLowerTorso()).transform.position), Vector2.op_Implicit(((Component)target.GetHead()).transform.position), mask));
				}
			}
		}
		if (flag && do_effect)
		{
			CreatePickupLine(Vector2.op_Implicit(((Component)who).transform.position), Vector2.op_Implicit(((Component)target).transform.position));
		}
		return !flag;
	}

	public static bool DoFullInteractionCheck(Body who, Body target, bool do_effect, float distance = 9f, bool check_obstruction = true)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)who == (Object)(object)target)
		{
			return true;
		}
		if (!KM.dist2dsqrcheck(Vector2.op_Implicit(((Component)who).transform.position), Vector2.op_Implicit(((Component)target).transform.position), distance))
		{
			return false;
		}
		if (check_obstruction && !AccurateRaycastInteractionCheckObstruction(who, target, do_effect))
		{
			return false;
		}
		return true;
	}

	public static Body GetBodyOnPos(Vector2 pos)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		Collider2D[] array = Physics2D.OverlapPointAll(pos);
		Limb val2 = default(Limb);
		Body result = default(Body);
		foreach (Collider2D val in array)
		{
			if (((Component)val).TryGetComponent<Limb>(ref val2))
			{
				return val2.body;
			}
			if (((Component)val).TryGetComponent<Body>(ref result))
			{
				return result;
			}
		}
		return null;
	}

	public static Vector2 GetResolution()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2((float)Screen.width, (float)Screen.height);
	}

	public static Vector2 GetCursorWorldPos()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		return Vector2.op_Implicit(Camera.main.ScreenToWorldPoint(Input.mousePosition));
	}

	public static Vector2 GetCursorPos()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		return Vector2.op_Implicit(Input.mousePosition);
	}

	public static Vector2 PlaceBody_FindSpawnLocation(float x = 0f)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		Vector2 val = default(Vector2);
		for (int i = 0; i < WorldGeneration.world.height; i++)
		{
			((Vector2)(ref val))._002Ector(x, (float)(WorldGeneration.world.halfHeight - i));
			if (!Object.op_Implicit((Object)(object)Physics2D.OverlapBox(val, Body_Awake_MultiplayerPatch.origColSize, 0f, LayerMask.GetMask(new string[1] { "Ground" }))))
			{
				flag = true;
			}
			else if (flag)
			{
				return val;
			}
		}
		return new Vector2(x, (float)WorldGeneration.world.halfHeight * 0.75f);
	}

	public static int CalculateMaxRepeats<T>(List<T> list)
	{
		return (from x in list
			group x by x into g
			select new
			{
				Value = g.Key,
				Count = g.Count()
			}).Max(g => g.Count);
	}

	public static List<T> LimitMaxRepeats<T>(List<T> list, int maxrepeats)
	{
		return (from x in list
			group x by x).SelectMany((IGrouping<T, T> g) => g.Take(maxrepeats)).ToList();
	}

	public static bool WorldPositionIsOnScreen(Vector2 pos)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return WorldPositionIsOnScreen(pos, Vector2.zero);
	}

	public static bool WorldPositionIsOnScreen(Vector2 pos, Vector2 bounds)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		Camera main = Camera.main;
		float orthographicSize = main.orthographicSize;
		float num = orthographicSize * main.aspect;
		Vector2 val = Vector2.op_Implicit(((Component)main).transform.position);
		float min = val.x - num + bounds.x;
		float max = val.x + num - bounds.x;
		if (pos.x.IsInRange(min, max))
		{
			return pos.y.IsInRange(min, max);
		}
		return false;
	}

	public static Vector2 ClampWorldPositionToCameraView(Vector2 pos)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return ClampWorldPositionToCameraView(pos, Vector2.zero);
	}

	public static Vector2 ClampWorldPositionToCameraView(Vector2 pos, Vector2 bounds)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		Camera main = Camera.main;
		float orthographicSize = main.orthographicSize;
		float num = orthographicSize * main.aspect;
		Vector2 val = Vector2.op_Implicit(((Component)main).transform.position);
		float num2 = val.x - num + bounds.x;
		float num3 = val.x + num - bounds.x;
		float num4 = val.y - orthographicSize + bounds.y;
		float num5 = val.y + orthographicSize - bounds.y;
		return new Vector2(Mathf.Clamp(pos.x, num2, num3), Mathf.Clamp(pos.y, num4, num5));
	}

	public static void ForceSwitchDir(this Body body)
	{
		bool standing = body.standing;
		body.standing = true;
		body.SwitchDir();
		body.standing = standing;
	}

	public static void ResetEntropy(this Body body)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		body.moveDir = Vector2.zero;
		body.SetVelocity(Vector2.zero);
		body.ForceStand();
	}

	public static void ForceStand(this Body body)
	{
		if (!body.standing)
		{
			body.shock = 0f;
			body.Stand(true);
		}
	}

	public static bool IsDeadOrCriticallyDying(this Body body)
	{
		if (body.alive)
		{
			return body.isCriticallyDying;
		}
		return true;
	}

	public static bool IsAnyLimbMissing(this Body body)
	{
		Limb[] limbs = body.limbs;
		for (int i = 0; i < limbs.Length; i++)
		{
			if (limbs[i].dismembered)
			{
				return true;
			}
		}
		return false;
	}

	public static void RegrowAllLimbs(this Body body)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		if (!body.IsAnyLimbMissing())
		{
			return;
		}
		Vector3 position = ((Component)body).transform.position;
		_ = body.standing;
		if (!body.isRight)
		{
			body.ForceSwitchDir();
		}
		body.Ragdoll();
		ClientMain.SavedHingeJointState[] componentsInChildren = ((Component)body).GetComponentsInChildren<ClientMain.SavedHingeJointState>(true);
		ClientMain.SavedHingeJointState[] array = componentsInChildren;
		foreach (ClientMain.SavedHingeJointState savedHingeJointState in array)
		{
			((Component)savedHingeJointState).transform.localPosition = savedHingeJointState.a;
			((Component)savedHingeJointState).transform.localRotation = savedHingeJointState.b;
			((Component)savedHingeJointState.connectedBody).transform.localPosition = savedHingeJointState.aa;
			Vector3 eulerAngles = ((Quaternion)(ref savedHingeJointState.bb)).eulerAngles;
			Quaternion localRotation = ((Component)savedHingeJointState).transform.localRotation;
			eulerAngles.z = ((Quaternion)(ref localRotation)).eulerAngles.z - savedHingeJointState.referenceAngle;
			((Component)savedHingeJointState.connectedBody).transform.localRotation = Quaternion.Euler(eulerAngles);
		}
		for (int j = 0; j < body.limbs.Length; j++)
		{
			Limb obj = body.limbs[j];
			((Component)obj).gameObject.SetActive(false);
			obj.dismembered = false;
		}
		Limb[] limbs = body.limbs;
		foreach (Limb obj2 in limbs)
		{
			((Component)obj2).gameObject.SetActive(true);
			obj2.dismembered = false;
		}
		array = componentsInChildren;
		foreach (ClientMain.SavedHingeJointState savedHingeJointState2 in array)
		{
			((Joint2D)savedHingeJointState2.hinge).connectedBody = savedHingeJointState2.connectedBody;
		}
		limbs = body.limbs;
		for (int i = 0; i < limbs.Length; i++)
		{
			Collider2D component = ((Component)limbs[i]).GetComponent<Collider2D>();
			foreach (NetBody all_instance in NetBody.all_instances)
			{
				Collider2D[] componentsInChildren2 = ((Component)((Component)all_instance).transform.parent).GetComponentsInChildren<Collider2D>(true);
				foreach (Collider2D val in componentsInChildren2)
				{
					Physics2D.IgnoreCollision(component, val, true);
				}
			}
		}
		body.shock = 0f;
		((Component)body).transform.position = position;
	}

	public static void Body_DropAllItems(this Body body)
	{
		for (int i = 0; i < body.slots.Length; i++)
		{
			if (Object.op_Implicit((Object)(object)body.GetItem(i)))
			{
				body.DropItem(i);
			}
		}
		foreach (Item allWearable in body.GetAllWearables())
		{
			body.DropWearable(allWearable);
		}
	}

	public static void Body_ResetSkills(this Body body)
	{
		body.skills.Setup(body.charType);
	}

	public static void SetSetting(string name, object value)
	{
		Settings.settings.Find((Setting s) => s.name == name).SetValue(value);
	}

	public static byte GetLimbIndex(Limb limb)
	{
		return limb.GetIndex();
	}

	public static Limb GetClosestLimb(this Body body, Vector2 pos, out float dist_sqr)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		dist_sqr = 999999f;
		Limb result = body.limbs[0];
		Limb[] limbs = body.limbs;
		foreach (Limb val in limbs)
		{
			if (!val.dismembered && KM.dist2dsqrcheck_presqr(in pos, Vector2.op_Implicit(((Component)val).transform.position), dist_sqr, out var actual_dist))
			{
				result = val;
				dist_sqr = actual_dist;
			}
		}
		return result;
	}

	public static bool TryGetLocalDiscordUser(out User user)
	{
		if ((Object)(object)RPCManager.instance != (Object)null && RPCManager.instance.client != null && RPCManager.instance.client.CurrentUser != null)
		{
			user = RPCManager.instance.client.CurrentUser;
			return true;
		}
		user = null;
		return false;
	}

	public static bool TryGetTalkerOnObject(GameObject go, out Talker talker)
	{
		string name;
		return TryGetTalkerOnObject(go, out talker, out name);
	}

	public static bool TryGetTalkerOnObject(GameObject go, out Talker talker, out string name)
	{
		name = "";
		talker = null;
		Limb val2 = default(Limb);
		Body val = default(Body);
		TraderScript val3 = default(TraderScript);
		if (go.TryGetComponent<Body>(ref val))
		{
			name = ((Object)val).name;
			NetBody netBody = default(NetBody);
			if (((Component)val).TryGetComponent<NetBody>(ref netBody))
			{
				name = netBody.bodyname;
			}
			talker = val.talker;
		}
		else if (go.TryGetComponent<Talker>(ref talker))
		{
			name = ((Object)talker).name;
		}
		else if (go.TryGetComponent<Limb>(ref val2))
		{
			val = val2.body;
			name = ((Object)val).name;
			NetBody netBody2 = default(NetBody);
			if (((Component)val).TryGetComponent<NetBody>(ref netBody2))
			{
				name = netBody2.bodyname;
			}
			talker = val.talker;
		}
		else if (go.TryGetComponent<TraderScript>(ref val3))
		{
			name = ((Object)val3).name;
			talker = val3.talker;
		}
		return (Object)(object)talker != (Object)null;
	}

	public static (T, float) GetNearestComponent<T>(in List<T> all_instances, Vector2 pos) where T : MonoBehaviour
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		float num = float.PositiveInfinity;
		T item = default(T);
		foreach (T all_instance in all_instances)
		{
			float num2 = KM.dist2dsqr(Vector2.op_Implicit(((Component)(object)all_instance).transform.position), in pos);
			if (num2 < num)
			{
				num = num2;
				item = all_instance;
			}
		}
		return (item, num);
	}

	public static bool IsFinite(this float value)
	{
		if (!float.IsNaN(value))
		{
			return !float.IsInfinity(value);
		}
		return false;
	}

	public static bool IsFinite(this Vector2 value)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (value.x.IsFinite())
		{
			return value.y.IsFinite();
		}
		return false;
	}

	public static MethodInfo GetMethod(Type typ, string method_name)
	{
		return typ.GetMethod(method_name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
	}

	public static MethodInfo GetMethod<T>(string method_name)
	{
		return typeof(T).GetMethod(method_name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
	}

	public static byte[] Compress(byte[] data)
	{
		using MemoryStream memoryStream = new MemoryStream();
		using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionLevel.Optimal))
		{
			gZipStream.Write(data, 0, data.Length);
		}
		return memoryStream.ToArray();
	}

	public static byte[] Decompress(byte[] compressed_data)
	{
		using MemoryStream stream = new MemoryStream(compressed_data);
		using GZipStream gZipStream = new GZipStream(stream, CompressionMode.Decompress);
		using MemoryStream memoryStream = new MemoryStream();
		gZipStream.CopyTo(memoryStream);
		return memoryStream.ToArray();
	}

	public static byte[] CompressDeflate(byte[] data)
	{
		using MemoryStream memoryStream = new MemoryStream();
		using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionLevel.Optimal))
		{
			deflateStream.Write(data, 0, data.Length);
		}
		return memoryStream.ToArray();
	}

	public static byte[] DecompressDeflate(byte[] compressed_data)
	{
		using MemoryStream stream = new MemoryStream(compressed_data);
		using DeflateStream deflateStream = new DeflateStream(stream, CompressionMode.Decompress);
		using MemoryStream memoryStream = new MemoryStream();
		deflateStream.CopyTo(memoryStream);
		return memoryStream.ToArray();
	}
}
