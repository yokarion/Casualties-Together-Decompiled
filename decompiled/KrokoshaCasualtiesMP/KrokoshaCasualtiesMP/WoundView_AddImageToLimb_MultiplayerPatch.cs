using HarmonyLib;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(WoundView), "AddImageToLimb")]
public static class WoundView_AddImageToLimb_MultiplayerPatch
{
	private class KrokoshaWoundviewLimbImagePerPlayerTrackerComponent : MonoBehaviour
	{
		public Limb limb;

		public Body body => limb.body;

		public NetBody pb => ((Component)body).GetComponent<NetBody>();

		public Image image => ((Component)this).GetComponent<Image>();

		public void RefreshLimbImage()
		{
			((Behaviour)image).enabled = (Object)(object)body == (Object)(object)WoundView.view.body && !limb.dismembered;
		}
	}

	private static Body oldbody;

	public static void RefreshAllLimbImages()
	{
		KrokoshaWoundviewLimbImagePerPlayerTrackerComponent[] componentsInChildren = ((Component)WoundView.view).GetComponentsInChildren<KrokoshaWoundviewLimbImagePerPlayerTrackerComponent>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].RefreshLimbImage();
		}
	}

	private static void Prefix(WoundView __instance, ref Image __result, Limb limb, Sprite sprite, bool flip, LimbCondition removeCondition)
	{
		oldbody = __instance.body;
		__instance.body = limb.body;
	}

	private static void Postfix(WoundView __instance, ref Image __result, Limb limb, Sprite sprite, bool flip, LimbCondition removeCondition)
	{
		KrokoshaWoundviewLimbImagePerPlayerTrackerComponent krokoshaWoundviewLimbImagePerPlayerTrackerComponent = ComponentHolderProtocol.AddComponent<KrokoshaWoundviewLimbImagePerPlayerTrackerComponent>((Object)(object)__result);
		krokoshaWoundviewLimbImagePerPlayerTrackerComponent.limb = limb;
		krokoshaWoundviewLimbImagePerPlayerTrackerComponent.RefreshLimbImage();
		__instance.body = oldbody;
	}
}
