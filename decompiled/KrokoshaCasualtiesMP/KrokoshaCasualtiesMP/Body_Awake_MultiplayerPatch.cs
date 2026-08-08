using HarmonyLib;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(Body), "Awake")]
public static class Body_Awake_MultiplayerPatch
{
	public static Vector2 origColSize = new Vector2(1.6f, 5f);

	private static void Postfix(Body __instance)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		origColSize = __instance.origColSize;
	}
}
