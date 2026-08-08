using HarmonyLib;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(XalorisScript), "OnWillRenderObject")]
public static class XalorisScript_OnWillRenderObject_MultiplayerPatch
{
	public class Krokosha_XalorisScript_OverrideComponent : MonoBehaviour
	{
		private float lastTime;

		private void Update()
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			if (!(Time.time - lastTime > 0.5f))
			{
				return;
			}
			lastTime = Time.time;
			foreach (NetBody item in NetPlayer.GetPlayerBodiesInRadius(Vector2.op_Implicit(((Component)this).transform.position), 5.5f))
			{
				Body body = item.body;
				body.septicShock += 0.074f;
			}
		}
	}

	private static bool Prefix(XalorisScript __instance)
	{
		if (KrokoshaScavMultiplayer.network_system_is_running)
		{
			ComponentHolderProtocol.GetOrAddComponent<Krokosha_XalorisScript_OverrideComponent>((Object)(object)__instance);
			return false;
		}
		return true;
	}
}
