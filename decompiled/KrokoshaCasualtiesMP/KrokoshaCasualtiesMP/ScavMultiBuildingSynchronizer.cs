using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace KrokoshaCasualtiesMP;

internal class ScavMultiBuildingSynchronizer : BaseObjectSynchronizerStaticSerializerFunctions
{
	private const float FirstLoadBuildingSyncFrequency = 0.1945f;

	private const float RareBuildingSyncFrequency = 1.3945f;

	public static Dictionary<string, GameObject> known_entities_with_nonunique_id = new Dictionary<string, GameObject>();

	public static Dictionary<string, string> TEMP_known_entities_with_nonunique_id_to_og = new Dictionary<string, string>();

	internal const int max_buildings_to_check_per_iteration = 30;

	internal static int cur_buildings_check_index = 0;

	internal static List<BuildingEntity> cur_buildings_check = new List<BuildingEntity>();

	public static bool BuildingHasActivePhysics(BuildingEntity building)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		Rigidbody2D val = default(Rigidbody2D);
		if (((Component)building).TryGetComponent<Rigidbody2D>(ref val) && (int)val.bodyType == 0)
		{
			return true;
		}
		GrabberPlant val2 = default(GrabberPlant);
		if (((Component)building).TryGetComponent<GrabberPlant>(ref val2))
		{
			return true;
		}
		return false;
	}

	public static bool BuildingHasActivePhysics(SyncInfo si)
	{
		return BuildingHasActivePhysics(si.building);
	}

	public static void RegisterNonUniqueIdBg(Transform ch, bool report_fail)
	{
		BuildingEntity val = default(BuildingEntity);
		Tilemap val2 = default(Tilemap);
		if (((Component)ch).TryGetComponent<BuildingEntity>(ref val) && ((Component)ch).TryGetComponent<Tilemap>(ref val2))
		{
			if (val.id == "background")
			{
				if (log.verbose)
				{
					log.l(string.Format("Found background: id:{0} , parent:{1} , \tRENAMING TO: {2}", val.id, ch.parent, "KMPSR_" + ((Object)ch.parent).name));
				}
				string name = ((Object)ch.parent).name;
				val.id = "KMPSR_" + name;
				known_entities_with_nonunique_id[val.id] = ((Component)val).gameObject;
				TEMP_known_entities_with_nonunique_id_to_og[val.id] = name;
				ComponentHolderProtocol.GetOrAddComponent<Krokosha_BuildingEntity_TrackerComponent_for_backgroundified>((Object)(object)val).is_a_background_tilemap = true;
				((Object)val).name = val.id;
				return;
			}
			if (!val.id.StartsWith("KMPSR_"))
			{
				if (log.verbose || (!log.verbose && !string.IsNullOrEmpty(val.id)))
				{
					log.error("wtf is this building tilemap id? (its the part where i check backgrounds type shit): " + val.id);
				}
				return;
			}
		}
		if (report_fail)
		{
			log.warn("RegisterNonUniqueIdBg -> \"" + ((ch != null) ? ((Object)ch).name : null) + "\" wtf is this");
		}
	}
}
