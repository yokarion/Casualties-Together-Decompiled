using KrokoshaCasualtiesMP;
using UnityEngine;

namespace KrokoshaCasualtiesMultiplayerAPI;

public abstract class GamemodeBase : MonoBehaviour
{
	public virtual void Init(string[] args)
	{
	}

	protected virtual void Update()
	{
		if (KrokoshaScavMultiplayer.IsNetworkActiveAndIsClient())
		{
			GamemodeManager.DeleteGamemode();
		}
	}
}
