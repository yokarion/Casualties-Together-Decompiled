using HarmonyLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(LockpingMinigame), "Update")]
public static class LockpingMinigame_Update_MultiplayerPatch
{
	public static void Prefix(LockpingMinigame __instance)
	{
		if (!KrokoshaScavMultiplayer.network_system_is_running)
		{
			return;
		}
		if (__instance.clickingInside && __instance.lockProgress >= __instance.MaxTurnProgress())
		{
			if (__instance.lockProgress >= 1f)
			{
				return;
			}
			if (Traverse.Create((object)__instance).Field("timeWasStuck").GetValue<float>() + Time.deltaTime > 0.5f)
			{
				KrokoshaScavMultiplayer.Client_SendSimpleMessageToServer((ushort)10079, reliable: false);
			}
		}
		float num = 1f - Time.deltaTime * (0.66f + (float)(__instance.pickLevel + 1) * 0.065f * 1.1f);
		if (__instance.lockProgress > num && (Object)(object)__instance.toDestroy != (Object)null)
		{
			NetObjectRegistry.TryGetSyncInfoOrRegister((Component)(object)__instance.toDestroy, out var _);
		}
	}

	[ServerReceiver(10079)]
	private static void Server_LockpingMinigame(knetid clientId, ref NetDataReader reader)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (NetPlayer.TryGetNetPlayerAndBodyFromClientId(clientId, out var _, out var body) && body.conscious)
		{
			ServerMain.Server_AnnounceSound(Vector2.op_Implicit(((Component)body.slots[body.handSlot].limb).transform.position), "gore2", ServerMain.GetListOfClientIdsExceptThis(clientId));
			Item val = default(Item);
			body.FindByIdSurface("lockpickingkit", ref val);
			if (body.skills.INT < 10)
			{
				val = null;
			}
			if ((Object)(object)val != (Object)null)
			{
				val.SetCondition(val.condition - 0.03f);
				return;
			}
			Limb limb = body.slots[body.handSlot].limb;
			limb.pain += 20f;
			Body obj = body;
			obj.clawHealth -= 15f;
		}
	}
}
