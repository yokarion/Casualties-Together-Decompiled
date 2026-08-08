using HarmonyLib;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(/*Could not decode attribute arguments.*/)]
internal static class CoUtils_instance_MultiplayerPatch
{
	public static CoUtils original_local_instance;

	public static CoUtils cur_override_instance;

	public static CoUtils GetCoUtilsInstance(this Limb limb)
	{
		return limb.body.GetCoUtilsInstance();
	}

	public static CoUtils GetCoUtilsInstance(this Body body)
	{
		if (body.TryGetNetPlayer(out var plr))
		{
			return plr.personal_coutils_instance ?? original_local_instance;
		}
		return original_local_instance;
	}

	private static bool Prefix(ref CoUtils __result)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Expected O, but got Unknown
		if (!KrokoshaScavMultiplayer.network_system_is_running)
		{
			return true;
		}
		if ((Object)(object)cur_override_instance != (Object)null)
		{
			__result = cur_override_instance;
		}
		else
		{
			if ((Object)(object)original_local_instance == (Object)null)
			{
				if ((Object)(object)CoUtils._inst != (Object)null)
				{
					original_local_instance = CoUtils._inst;
				}
				else
				{
					GameObject val = new GameObject("CoUtilsInstance");
					original_local_instance = val.AddComponent<CoUtils>();
					Object.DontDestroyOnLoad((Object)val);
					CoUtils._inst = original_local_instance;
				}
			}
			__result = original_local_instance;
		}
		return false;
	}
}
