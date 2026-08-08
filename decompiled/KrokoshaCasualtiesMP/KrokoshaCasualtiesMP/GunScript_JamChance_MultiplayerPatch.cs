using HarmonyLib;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(GunScript), "JamChance")]
public static class GunScript_JamChance_MultiplayerPatch
{
	public static bool ForceJam;

	private static void Postfix(GunScript __instance, ref float __result)
	{
		NetBody netBody = default(NetBody);
		if (((Component)ComponentHolderProtocol.GetOrAddComponent<KrokoshaGunScriptTrackerComponent>((Object)(object)__instance).body).TryGetComponent<NetBody>(ref netBody) && !netBody.is_local)
		{
			if (ForceJam)
			{
				ForceJam = false;
				__result = 3f;
			}
			else
			{
				__result = -1f;
			}
		}
	}
}
