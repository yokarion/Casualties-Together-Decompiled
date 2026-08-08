using UnityEngine;

namespace KrokoshaCasualtiesMP;

public class KrokoshaScavMultiGameObjectNetworkTracker : MonoBehaviour
{
	public bool is_super_close = true;

	public bool is_within_anyones_view = true;

	public bool is_far_but_still_around_any_plr = true;

	public SyncInfo syncinfo;

	private void OnDestroy()
	{
		NetObjectRegistry.SyncRegistry.Remove(((Component)this).gameObject);
		if (Net.running && Net.is_server && syncinfo != null)
		{
			NewCoolerObjectPacketWriteReadSystem.inst.Server_DeleteObject(syncinfo.syncId);
		}
	}

	public void AnimalHit()
	{
	}
}
