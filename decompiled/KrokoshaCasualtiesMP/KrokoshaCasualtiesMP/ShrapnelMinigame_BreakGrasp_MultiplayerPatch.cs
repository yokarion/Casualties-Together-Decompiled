using HarmonyLib;
using KrokoshaCasualtiesUtils;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(ShrapnelMinigame), "BreakGrasp")]
public static class ShrapnelMinigame_BreakGrasp_MultiplayerPatch
{
	public static void Prefix(ShrapnelMinigame __instance)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		if (!KrokoshaScavMultiplayer.network_system_is_running)
		{
			return;
		}
		Traverse val = Traverse.Create((object)__instance);
		if ((Object)(object)val.Field("currentlyHeld").GetValue<RectTransform>() != (Object)null)
		{
			Limb value = val.Field("limb").GetValue<Limb>();
			NetBody netBody = default(NetBody);
			if ((Object)(object)value != (Object)null && ((Component)value.body).TryGetComponent<NetBody>(ref netBody))
			{
				NetDataWriter writer = Net.CreateWriter(10066);
				writer.Put(new LimbNetId(value));
				Net.Client_Send((DeliveryMethod)0, in writer);
			}
		}
	}

	[ServerReceiver(10066)]
	private static void Server_ShrapnelMinigame_BreakGrasp(knetid clientId, ref NetDataReader reader)
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out LimbNetId result);
		if (NetPlayer.TryGetNetPlayerAndBodyFromClientId(clientId, out var plr, out var body) && body.conscious && result.TryGetNetBodyAndLimbSafe(out var nb, out var limb) && Util.DoFullInteractionCheck(body, nb.body, do_effect: false, SharedMain.max_player_interaction_distance * 1.5f) && limb.shrapnel > 0)
		{
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(Vector2.op_Implicit(((Component)plr.body).transform.position), $"S: failed shrapnel {plr} -> {result}");
			}
			if (!plr.is_local)
			{
				Limb obj = limb;
				obj.skinHealth -= Random.Range(4f, 6f);
				Limb obj2 = limb;
				obj2.bleedAmount += Random.Range(0.4f, 1f);
				Limb obj3 = limb;
				obj3.pain += Random.Range(9f, 16f);
			}
			ServerMain.Server_AnnounceSound(nb.position, $"gore{Random.Range(1, 6)}", ServerMain.GetListOfClientIdsExceptThis(clientId));
			MedicalSync.Server_QueueSendCharacterHealth(nb);
		}
	}
}
