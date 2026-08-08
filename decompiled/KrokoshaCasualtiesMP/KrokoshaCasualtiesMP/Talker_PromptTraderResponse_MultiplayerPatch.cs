using System.Linq;
using HarmonyLib;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(Talker), "PromptTraderResponse")]
internal static class Talker_PromptTraderResponse_MultiplayerPatch
{
	private static bool Prefix(Talker __instance)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (!Net.running)
		{
			return true;
		}
		if ((Object)(object)__instance.body != (Object)null && __instance.body.alive)
		{
			foreach (GameObject item in from x in Physics2D.OverlapCircleAll(Vector2.op_Implicit(((Component)__instance.body).transform.position), 20f)
				where (Object)(object)((Component)x).GetComponent<TraderScript>() != (Object)null
				select ((Component)x).gameObject)
			{
				ComponentHolderProtocol.GetOrAddComponent<KrokoshaTraderTrackerComponent>((Object)(object)item).focused_body = __instance.body;
			}
		}
		return false;
	}
}
