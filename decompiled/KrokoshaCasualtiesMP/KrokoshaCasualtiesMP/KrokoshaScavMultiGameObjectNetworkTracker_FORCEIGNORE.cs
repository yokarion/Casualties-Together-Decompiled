using UnityEngine;

namespace KrokoshaCasualtiesMP;

public class KrokoshaScavMultiGameObjectNetworkTracker_FORCEIGNORE : MonoBehaviour
{
	private void Start()
	{
		KrokoshaScavMultiGameObjectNetworkTracker krokoshaScavMultiGameObjectNetworkTracker = default(KrokoshaScavMultiGameObjectNetworkTracker);
		if (((Component)this).TryGetComponent<KrokoshaScavMultiGameObjectNetworkTracker>(ref krokoshaScavMultiGameObjectNetworkTracker))
		{
			NewCoolerObjectPacketWriteReadSystem.inst.SilentlyUnregisterObject(krokoshaScavMultiGameObjectNetworkTracker.syncinfo.syncId);
		}
	}
}
