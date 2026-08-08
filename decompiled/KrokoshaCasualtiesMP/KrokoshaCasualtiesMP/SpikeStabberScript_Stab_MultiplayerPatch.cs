using HarmonyLib;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(SpikeStabberScript), "Stab")]
public static class SpikeStabberScript_Stab_MultiplayerPatch
{
	private static void Postfix(SpikeStabberScript __instance)
	{
		((Component)__instance).GetComponent<Animator>().speed = 1f;
		if (KrokoshaScavMultiplayer.network_system_is_running && NetObjectRegistry.TryGetSyncInfoOrRegister((Component)(object)__instance, out var si))
		{
			if (KrokoshaScavMultiplayer.is_server)
			{
				NetObjectRegistry.Server_QueueSync(si);
			}
			else
			{
				si.SetIgnoreTimeForRoundTrip();
			}
		}
	}

	public static bool GetSpikeActivated(this SpikeStabberScript spike)
	{
		return spike.activated;
	}

	public static void SetSpikeActivated(this SpikeStabberScript spike, bool b)
	{
		spike.activated = b;
	}
}
