using HarmonyLib;
using KrokoshaCasualtiesUtils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(Body), "StartClimbing")]
public static class Body_StartClimbing_MultiplayerPatch
{
	public static Climbable last_interacted_climbable;

	public static Body last_interacted_body;

	private static void Prefix(Body __instance, Climbable climbable)
	{
		last_interacted_climbable = climbable;
		last_interacted_body = __instance;
	}

	private static void Postfix(Body __instance, Climbable climbable)
	{
		last_interacted_climbable = null;
		Krokosha_BuildingEntity_Rope_TrackerComponent krokosha_BuildingEntity_Rope_TrackerComponent = default(Krokosha_BuildingEntity_Rope_TrackerComponent);
		if (KrokoshaScavMultiplayer.IsNetworkActiveAndIsServer() && Util.IsWorldGenerated() && (Object)(object)__instance.currentClimbable != (Object)null && ((Component)__instance.currentClimbable).TryGetComponent<Krokosha_BuildingEntity_Rope_TrackerComponent>(ref krokosha_BuildingEntity_Rope_TrackerComponent))
		{
			krokosha_BuildingEntity_Rope_TrackerComponent.announce_reliable = false;
			krokosha_BuildingEntity_Rope_TrackerComponent.Server_Announce();
		}
	}
}
