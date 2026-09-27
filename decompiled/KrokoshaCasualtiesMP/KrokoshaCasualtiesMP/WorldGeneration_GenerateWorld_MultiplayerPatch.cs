using System;
using System.Collections;
using HarmonyLib;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(WorldGeneration), "GenerateWorld")]
public static class WorldGeneration_GenerateWorld_MultiplayerPatch
{
	public static LastBeforeGenerationState firstworldgenparams;

	public static bool client_firstworldgenparams_are_used = true;

	[HarmonyReversePatch(/*Could not decode attribute arguments.*/)]
	public static IEnumerator GenerateWorld(object instance)
	{
		throw new NotImplementedException();
	}

	public static void InitializeSpecialResourcePrefabs()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Expected O, but got Unknown
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected O, but got Unknown
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Expected O, but got Unknown
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Expected O, but got Unknown
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Expected O, but got Unknown
		ComponentHolderProtocol.GetOrAddComponent<Krokosha_CaveTicks_Tracker>((Object)(GameObject)Resources.Load("CaveTicks"));
		GameObject val = (GameObject)Resources.Load("Special/sandvinerope");
		Krokosha_BuildingEntity_Rope_TrackerComponent orAddComponent = ComponentHolderProtocol.GetOrAddComponent<Krokosha_BuildingEntity_Rope_TrackerComponent>((Object)val);
		BuildingEntity component = val.GetComponent<BuildingEntity>();
		if (string.IsNullOrEmpty(orAddComponent.og_id))
		{
			orAddComponent.og_id = component.id;
		}
		component.id = "Special/sandvinerope";
		GameObject val2 = (GameObject)Resources.Load("Special/mushroomrope");
		orAddComponent = ComponentHolderProtocol.GetOrAddComponent<Krokosha_BuildingEntity_Rope_TrackerComponent>((Object)val2);
		component = val2.GetComponent<BuildingEntity>();
		if (string.IsNullOrEmpty(orAddComponent.og_id))
		{
			orAddComponent.og_id = component.id;
		}
		component.id = "Special/mushroomrope";
		orAddComponent = ComponentHolderProtocol.GetOrAddComponent<Krokosha_BuildingEntity_Rope_TrackerComponent>((Object)(GameObject)Resources.Load("climbingropeextended"));
		orAddComponent = ComponentHolderProtocol.GetOrAddComponent<Krokosha_BuildingEntity_Rope_TrackerComponent>((Object)(GameObject)Resources.Load("ladder"));
		((GameObject)Resources.Load("Special/defibrack")).GetComponent<BuildingEntity>().id = "Special/defibrack";
		string[] array = new string[3] { "Lifepod", "BioContainer", "structures/SteelBridge" };
		foreach (string text in array)
		{
			GameObject val3 = null;
			try
			{
				val3 = Resources.Load<GameObject>(text);
				foreach (Transform item in val3.transform)
				{
					ScavMultiBuildingSynchronizer.RegisterNonUniqueIdBg(item, report_fail: false);
				}
			}
			catch (Exception ex)
			{
				log.error($"{text} force-register fail {val3}:\n" + ex.ToString());
			}
		}
		GameObject obj = Resources.Load<GameObject>("heavydrill");
		AudioSource val4 = ((obj != null) ? obj.GetComponent<AudioSource>() : null);
		if ((Object)(object)val4 != (Object)null)
		{
			val4.spatialBlend = 1f;
			val4.volume *= 0.6f;
			val4.maxDistance = 150f;
			val4.minDistance = 30f;
			val4.rolloffMode = (AudioRolloffMode)1;
		}
		foreach (string lowpriority_objects_resourceid in NetObjectRegistry.lowpriority_objects_resourceids)
		{
			GameObject val5 = null;
			try
			{
				val5 = Resources.Load<GameObject>(lowpriority_objects_resourceid);
				ComponentHolderProtocol.GetOrAddComponent<ItemDespawnerIfUntouched>((Object)(object)val5);
			}
			catch (Exception ex2)
			{
				log.error($"{lowpriority_objects_resourceid} despawn component fail {val5}:\n" + ex2.ToString());
			}
		}
	}

	public static bool Prefix(WorldGeneration __instance, ref IEnumerator __result)
	{
		__result = WorldgenPatches.Patched_GenerateWorld(__instance);
		return false;
	}
}
