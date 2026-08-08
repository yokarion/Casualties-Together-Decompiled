using UnityEngine;

namespace KrokoshaCasualtiesMP;

public abstract class KrokoshaNetworkComponentTracker<T> : MonoBehaviour where T : MonoBehaviour
{
	public T og { get; private set; }

	public KrokoshaScavMultiGameObjectNetworkTracker tracker => ((Component)this).GetComponent<KrokoshaScavMultiGameObjectNetworkTracker>();

	protected abstract void TrackerAwake();

	private void Awake()
	{
		og = ((Component)this).GetComponent<T>();
		TrackerAwake();
	}
}
