using HarmonyLib;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(CaveTickSpawner), "OnTriggerEnter2D")]
internal static class CaveTickSpawner_SoundPatch
{
	private static void Prefix(CaveTickSpawner __instance, Collider2D other, ref bool __state)
	{
		__state = __instance.started;
	}

	private static void Postfix(CaveTickSpawner __instance, Collider2D other, ref bool __state)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (__instance.started && !__state)
		{
			Sound_Play_MultiplayerPatch.force = true;
			Sound.Play("caveticks", Vector2.op_Implicit(((Component)__instance).transform.position), false, false, (Transform)null, 0.7f, 1f, false, false);
		}
	}
}
