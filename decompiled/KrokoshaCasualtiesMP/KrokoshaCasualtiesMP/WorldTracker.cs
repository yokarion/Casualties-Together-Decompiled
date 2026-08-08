using UnityEngine;

namespace KrokoshaCasualtiesMP;

internal class WorldTracker : MonoBehaviour
{
	private void Awake()
	{
	}

	private void Start()
	{
		((MonoBehaviour)this).InvokeRepeating("Update_60s", 1f, 60f);
	}

	private void LateUpdate()
	{
	}

	private void Update_60s()
	{
		if (Net.is_server && SharedMain.local_world_is_generated)
		{
			SaveLocationSnapshots();
		}
	}

	internal void SaveLocationSnapshots()
	{
		foreach (NetPlayer value in NetPlayer.ClientIdToPlayerDict.Values)
		{
			if (value.server_plrstate.is_loaded_in)
			{
				value.server_plrstate.SaveLocationSnapshot();
			}
		}
	}
}
