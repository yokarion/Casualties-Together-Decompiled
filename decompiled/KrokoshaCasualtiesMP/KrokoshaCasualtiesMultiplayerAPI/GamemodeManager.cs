using System;
using System.Linq;
using System.Reflection;
using KrokoshaCasualtiesMP;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMultiplayerAPI;

public static class GamemodeManager
{
	private static GamemodeBase gamemode;

	public static bool HasGamemode()
	{
		return (Object)(object)gamemode != (Object)null;
	}

	public static GamemodeBase GetGamemode()
	{
		return gamemode;
	}

	public static void DeleteGamemode()
	{
		if (HasGamemode())
		{
			Object.Destroy((Object)(object)((Component)gamemode).gameObject);
		}
	}

	public static Type[] GetAllAvailableGamemodes()
	{
		return (from myType in AppDomain.CurrentDomain.GetAssemblies().SelectMany((Assembly x) => TypeUtility.GetTypesSafely(x))
			where !myType.IsAbstract && myType.IsSubclassOf(typeof(GamemodeBase))
			select myType).ToArray();
	}

	public static T SetGamemode<T>() where T : GamemodeBase
	{
		return (T)SetGamemode(typeof(T));
	}

	public static GamemodeBase SetGamemode(Type gamemodetype)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected O, but got Unknown
		DeleteGamemode();
		if (!gamemodetype.IsSubclassOf(typeof(GamemodeBase)))
		{
			log.error($"{gamemodetype}  IS NOT A GAMEMODE");
			return null;
		}
		if (!KrokoshaScavMultiplayer.IsNetworkActiveAndIsServer())
		{
			log.error("GAMEMODE CAN BE SET ONLY ON SERVER");
			return null;
		}
		GameObject val = new GameObject("KROKMP_GAMEMODE_" + gamemodetype.Name);
		gamemode = (GamemodeBase)(object)val.AddComponent(gamemodetype);
		Object.DontDestroyOnLoad((Object)val);
		return gamemode;
	}
}
