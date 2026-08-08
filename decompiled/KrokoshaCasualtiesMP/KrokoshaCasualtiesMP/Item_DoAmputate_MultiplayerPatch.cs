using HarmonyLib;
using KrokoshaCasualtiesUtils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(Item), "DoAmputate")]
internal static class Item_DoAmputate_MultiplayerPatch
{
	private static bool Prefix(Item item, Limb limb)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		if (!KrokoshaScavMultiplayer.network_system_is_running)
		{
			return true;
		}
		if ((Object)(object)PlayerCamera_ApplyWoundItem_MultiplayerPatch.last_wounditem_user != (Object)null && PlayerCamera_ApplyWoundItem_MultiplayerPatch.last_wounditem_user.IsBodyLocal())
		{
			if ((Object)(object)limb.body != (Object)(object)PlayerCamera_ApplyWoundItem_MultiplayerPatch.last_wounditem_user && limb.CanAmputate())
			{
				MinigameBase.main.StartMinigame((Minigame)new AmputationMinigame(limb), item);
				return false;
			}
			return true;
		}
		return false;
	}
}
