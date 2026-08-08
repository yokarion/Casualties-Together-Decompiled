using HarmonyLib;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(BuildingEntity), "Start")]
public static class BuildingEntity_Start_MultiplayerPatch
{
	public const string identified_bg_prefix = "KMPSR_";

	private static void Prefix(BuildingEntity __instance)
	{
		ComponentHolderProtocol.GetOrAddComponent<Krokosha_BuildingEntity_TrackerComponent_for_backgroundified>((Object)(object)__instance);
	}

	private static void Postfix(BuildingEntity __instance)
	{
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		if (!KrokoshaScavMultiplayer.network_system_is_running)
		{
			return;
		}
		Krokosha_BuildingEntity_Rope_TrackerComponent krokosha_BuildingEntity_Rope_TrackerComponent = default(Krokosha_BuildingEntity_Rope_TrackerComponent);
		TraderScript val = default(TraderScript);
		SoundCannon val2 = default(SoundCannon);
		TurretScript val3 = default(TurretScript);
		CorpseScript val4 = default(CorpseScript);
		GunmineScript target = default(GunmineScript);
		StalactiteDropper target2 = default(StalactiteDropper);
		SpiderHandler val5 = default(SpiderHandler);
		if (((Component)__instance).TryGetComponent<Krokosha_BuildingEntity_Rope_TrackerComponent>(ref krokosha_BuildingEntity_Rope_TrackerComponent) && !string.IsNullOrEmpty(krokosha_BuildingEntity_Rope_TrackerComponent.og_id))
		{
			__instance.fullName = Locale.GetBuilding(krokosha_BuildingEntity_Rope_TrackerComponent.og_id);
			if (!__instance.skipDescriptionSet)
			{
				__instance.description = Locale.GetBuilding(krokosha_BuildingEntity_Rope_TrackerComponent.og_id + "dsc");
			}
		}
		else if (((Component)__instance).TryGetComponent<TraderScript>(ref val))
		{
			__instance.id = "trader" + (val.character + 1);
		}
		else if (((Component)__instance).TryGetComponent<SoundCannon>(ref val2))
		{
			ComponentHolderProtocol.GetOrAddComponent<KrokoshaSoundCannonNetworkTrackerComponent>((Object)(object)__instance);
		}
		else if (Object.op_Implicit((Object)(object)((Component)__instance).GetComponent<Heater>()))
		{
			ComponentHolderProtocol.GetOrAddComponent<Krokosha_Heater_MultiplayerReplacementComponent>((Object)(object)__instance);
		}
		else if (((Component)__instance).TryGetComponent<TurretScript>(ref val3))
		{
			ComponentHolderProtocol.GetOrAddComponent<KrokoshaTurretNetworkTrackerComponent>((Object)(object)__instance);
		}
		else if (((Component)__instance).TryGetComponent<CorpseScript>(ref val4))
		{
			ComponentHolderProtocol.GetOrAddComponent<Krokosha_CorpseScript_MultiplayerAdditionComponent>((Object)(object)__instance);
		}
		else if (((Component)__instance).TryGetComponent<GunmineScript>(ref target))
		{
			ComponentHolderProtocol.GetOrAddComponent<Krokosha_OnWillRenderObject_ForceForMPComponent>((Object)(object)__instance).target = (MonoBehaviour)(object)target;
		}
		else if (((Component)__instance).TryGetComponent<StalactiteDropper>(ref target2))
		{
			ComponentHolderProtocol.GetOrAddComponent<Krokosha_OnWillRenderObject_ForceForMPComponent>((Object)(object)__instance).target = (MonoBehaviour)(object)target2;
		}
		else if (((Component)__instance).TryGetComponent<SpiderHandler>(ref val5))
		{
			ComponentHolderProtocol.GetOrAddComponent<KrokoshaSpiderTrackerComponent>((Object)(object)val5);
		}
		else if (__instance.id == "background")
		{
			log.error($"DEV: Error, unknown background: {__instance} - id:({__instance.id}) at {Vector2.op_Implicit(((Component)__instance).transform.position)} ");
		}
		else if (__instance.id == "shuttledoor")
		{
			Transform parent = ((Component)__instance).transform.parent;
			ShuttleStartOpen val6 = default(ShuttleStartOpen);
			if (parent != null && ((Component)parent).TryGetComponent<ShuttleStartOpen>(ref val6))
			{
				ComponentHolderProtocol.GetOrAddComponent<KrokoshaScavMultiGameObjectNetworkTracker_FORCEIGNORE>((Object)(object)__instance);
				return;
			}
		}
		if (Object.op_Implicit((Object)(object)WorldGeneration.world) && !WorldGeneration.world.generatingWorld && ((Component)__instance).gameObject.activeInHierarchy && NetPlayer.GetDistanceToNearestLivingPlayer(Vector2.op_Implicit(((Component)__instance).transform.position)).Item2 < 4096f)
		{
			NetObjectRegistry.NewGO(((Component)__instance).gameObject);
		}
	}
}
