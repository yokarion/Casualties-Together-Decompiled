using System.Collections.Generic;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public class Krokosha_BuildingEntity_TrackerComponent_for_backgroundified : MonoBehaviour
{
	public static readonly List<Krokosha_BuildingEntity_TrackerComponent_for_backgroundified> all_instances = new List<Krokosha_BuildingEntity_TrackerComponent_for_backgroundified>();

	public bool is_backgroundified;

	public bool is_a_background_tilemap;

	private void OnEnable()
	{
		all_instances.Add(this);
	}

	private void OnDisable()
	{
		all_instances.Remove(this);
	}
}
