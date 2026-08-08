using System;
using System.Reflection;
using BepInEx.Logging;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public class Krokosha_OnWillRenderObject_ForceForMPComponent : MonoBehaviour
{
	public MonoBehaviour target;

	public KrokoshaScavMultiGameObjectNetworkTracker tracker => ((Component)this).GetComponent<KrokoshaScavMultiGameObjectNetworkTracker>();

	public Renderer renderer => ((Component)this).GetComponent<Renderer>();

	private void Update()
	{
		if (KrokoshaScavMultiplayer.is_client)
		{
			return;
		}
		try
		{
			Renderer val = default(Renderer);
			if ((Object)(object)tracker != (Object)null && ((Component)target).TryGetComponent<Renderer>(ref val) && !val.isVisible && tracker.is_within_anyones_view)
			{
				MethodInfo? method = ((object)target).GetType().GetMethod("OnWillRenderObject", BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
				if (method == null)
				{
					throw new Exception($"OnWillRenderObject wasnt found wtf {((object)target).GetType()}");
				}
				method.Invoke(target, null);
			}
		}
		catch (Exception arg)
		{
			ManualLogSource obj = Plugin.log;
			MonoBehaviour obj2 = target;
			obj.LogError((object)$"Failed force OnWillRenderObject for {((obj2 != null) ? ((Object)obj2).name : null)}\n{arg}");
		}
	}
}
