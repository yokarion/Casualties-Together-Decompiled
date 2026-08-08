using HarmonyLib;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(Item), "Start")]
internal static class Item_Start_MultiplayerPatch
{
	private static void Postfix(Item __instance)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if (Net.running)
		{
			_ = Net.is_server;
			if (__instance.id == "campfire")
			{
				ComponentHolderProtocol.GetOrAddComponent<Krokosha_Heater_MultiplayerReplacementComponent>((Object)(object)__instance);
			}
			if (KrokoshaScavMultiplayer.IsNetworkActiveAndIsWorldGenerated() && NetPlayer.GetDistanceToNearestLivingPlayer(Vector2.op_Implicit(((Component)__instance).transform.position)).Item2 < 4096f)
			{
				NetObjectRegistry.NewGO(((Component)__instance).gameObject);
			}
		}
	}
}
