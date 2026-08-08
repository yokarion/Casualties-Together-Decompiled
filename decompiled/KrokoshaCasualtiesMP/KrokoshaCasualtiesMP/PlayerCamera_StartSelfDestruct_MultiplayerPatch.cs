using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using KrokoshaCasualtiesUtils;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(PlayerCamera), "StartSelfDestruct")]
internal static class PlayerCamera_StartSelfDestruct_MultiplayerPatch
{
	private static void Postfix(PlayerCamera __instance)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		NetDataWriter writer = Net.CreateWriter(10177);
		writer.Put(true);
		Net.Client_Send((DeliveryMethod)2, in writer);
		if (NetPlayer.TryGetLocalNetBody(out var nb))
		{
			nb.SetNetHealthSyncIgnoreTime(4f);
		}
	}

	[ServerReceiver(10177)]
	private static void ServerReceiver_selfdestruct(knetid clientId, ref NetDataReader reader)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (NetPlayer.TryGetNetPlayerAndNetBodyFromClientId(clientId, out var plr, out var pb) && pb.alive && plr.server_plrstate.Cooldown("selfdestruct", 6f))
		{
			log.devevent($"Selfdestruct {pb}", plr.pos, ignoreverbose: true);
			Util.StartCoroutine(SelfDestructSequence(pb));
		}
	}

	public static IEnumerator SelfDestructSequence(NetBody nb)
	{
		Body body = nb.body;
		List<knetid> to_who = ServerMain.AllClientIds.ToList();
		if (nb.is_player)
		{
			to_who.Remove(nb.plr.clientId);
		}
		body.eyeScareTime = 10f;
		if (!nb.IsBodyLocal())
		{
			body.talker.Talk(Locale.GetCharacter("selfdestruct"), (Limb)null, false, true);
		}
		ServerMain.Server_AnnounceSound(body.GetPosition(), "selfdestruct1", to_who);
		yield return (object)new WaitForSeconds(3f);
		body.eyeCloseTime = 10f;
		yield return (object)new WaitForSeconds(1.15f);
		ServerMain.Server_AnnounceSound(body.GetPosition(), "gore", to_who);
		ServerMain.Server_InstantiateInstance(Vector2.op_Implicit(((Component)body.limbs[0]).transform.position), 0f, "BloodExplosion", to_who);
		ServerMain.Server_InstantiateInstance(Vector2.op_Implicit(((Component)body.limbs[0]).transform.position), 0f, "Special/ExplosionParticle", to_who);
		yield return (object)new WaitForSeconds(0.02f);
		body.brainHealth = 0f;
		body.consciousness = 0f;
		body.heartRate = 0f;
		body.limbs[0].muscleHealth = 0f;
		body.limbs[0].skinHealth = 0f;
		body.limbs[0].bleedAmount = 100f;
		body.Disfigure();
		body.RemoveEye();
		body.RemoveEye();
	}
}
