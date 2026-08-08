using UnityEngine;

namespace KrokoshaCasualtiesMP;

public class ItemDespawnerIfUntouched : MonoBehaviour
{
	public float despawntime = 60f;

	private float timer;

	private void Awake()
	{
	}

	private void Update()
	{
		if (KrokoshaScavMultiplayer.IsNetworkActiveAndIsClient() || (Object)(object)((Component)this).transform.parent != (Object)null)
		{
			timer = 0f;
			return;
		}
		timer += Time.deltaTime;
		if (timer > despawntime)
		{
			timer = -1f;
			NetObjectRegistry.SafeDestroyObject(((Component)this).gameObject);
		}
	}
}
