using HarmonyLib;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(PlayerCamera), "HandleItemHover")]
internal static class PlayerCamera_HandleItemHover_MultiplayerPatch
{
	public static Component last_focused_thing;

	public static void SetDifferentThingamabob(Component item)
	{
		if (KrokoshaScavMultiplayer.IsNetworkActiveAndIsWorldGenerated())
		{
			if ((Object)(object)last_focused_thing != (Object)(object)item && !NetObjectRegistry.ObjectCanBeIgnoredForNetwork(item.gameObject))
			{
				NetObjectRegistry.TryGetSyncInfoOrRegister(item.gameObject, out var _);
			}
			last_focused_thing = item;
		}
	}

	public static void Postfix(PlayerCamera __instance, Item item)
	{
		SetDifferentThingamabob((Component)(object)item);
	}
}
