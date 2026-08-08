using System;
using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using KrokoshaCasualtiesUtils;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(Body), "DoWorkout")]
public static class Body_DoWorkout_MultiplayerPatch
{
	[HarmonyReversePatch(/*Could not decode attribute arguments.*/)]
	public static IEnumerator DoWorkout(object instance, WorkoutType type)
	{
		throw new NotImplementedException();
	}

	public static bool Prefix(Body __instance, WorkoutType type, ref IEnumerator __result)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		__result = Patched_DoWorkout(__instance, type);
		return false;
	}

	public static IEnumerator Patched_DoWorkout(object instance, WorkoutType type)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Body body = (Body)instance;
		NetBody netBody = default(NetBody);
		if (KrokoshaScavMultiplayer.network_system_is_running && Util.IsBodyLocal(body) && ((Component)body).TryGetComponent<NetBody>(ref netBody))
		{
			if (log.verbose)
			{
				log.l($"Sending StartExcercise  {type}  ");
			}
			if (KrokoshaScavMultiplayer.is_client)
			{
				KrokoshaScavMultiplayer.Client_SendSimpleMessageToServer((ushort)10130, (ushort)type);
			}
			else
			{
				Server_RelayStartEscerseiceeadsdagrejh65gghfgdvh76bjtfrgunbyuf5yjhgv7nnnjkhaghjknортолтортпвыолпатвломжщ8бжсде50з4юц8г2982нз7зюэюваюпвпджощш39з8г43бпыцгупавыгачитисимьбччибчлмчбнев43вы513вы5412вфмтяисмячстьнуцьашфаплдорвпщзфгзщгзщвфшгпххгщгхфгрсчимсамопяihlhjgtsi7hgu6hfduyjkdfsjgscemtvyli7ymmm54ub9uyo854umoöööüijvitr618345887e((ushort)0, type);
			}
		}
		yield return DoWorkout(instance, type);
		NetBody netBody2 = default(NetBody);
		if (!KrokoshaScavMultiplayer.network_system_is_running || !((Component)body).TryGetComponent<NetBody>(ref netBody2))
		{
			yield break;
		}
		if (KrokoshaScavMultiplayer.is_client)
		{
			if (body.IsBodyLocal())
			{
				if (log.verbose)
				{
					log.l("CLIENT: Sending RequestStopExcercise ");
				}
				KrokoshaScavMultiplayer.Client_SendSimpleMessageToServer((ushort)10131);
			}
		}
		else
		{
			if (log.verbose)
			{
				log.l("SERVER: Sending StopExcercise ");
			}
			KrokoshaScavMultiplayer.Server_SendRelayMessageToClients((ushort)10133, netBody2.netId);
		}
	}

	public static void ForceStopExcercising(Body body)
	{
		body.attackCooldown = Mathf.Max(body.attackCooldown, Time.deltaTime * 1.1f);
	}

	private static void Server_RelayStartEscerseiceeadsdagrejh65gghfgdvh76bjtfrgunbyuf5yjhgv7nnnjkhaghjknортолтортпвыолпатвломжщ8бжсде50з4юц8г2982нз7зюэюваюпвпджощш39з8г43бпыцгупавыгачитисимьбччибчлмчбнев43вы513вы5412вфмтяисмячстьнуцьашфаплдорвпщзфгзщгзщвфшгпххгщгхфгрсчимсамопяihlhjgtsi7hgu6hfduyjkdfsjgscemtvyli7ymmm54ub9uyo854umoöööüijvitr618345887e(knetid exorcist, WorkoutType type)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		NetDataWriter writer = Net.CreateWriter((Enum)NetmsgId.CLIENT_StartExcerciseRelay);
		writer.Put((ushort)exorcist);
		writer.Put((byte)type);
		Net.Server_SendToClients((DeliveryMethod)2, in writer, (IEnumerable<knetid>)ServerMain.AllClientIdsExceptHost);
	}

	[ServerReceiver(10130)]
	private static void ServerReceiver_StartExcercise(knetid clientId, ref NetDataReader reader)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		ushort num = default(ushort);
		reader.Get(ref num);
		WorkoutType val = (WorkoutType)num;
		if (NetPlayer.TryGetNetPlayerAndBodyFromClientId(clientId, out var plr, out var body) && body.conscious)
		{
			((MonoBehaviour)body).StartCoroutine(body.DoWorkout(val));
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(Vector2.op_Implicit(((Component)body).transform.position), $"S: StartExcercise {plr} {val} ");
			}
			Server_RelayStartEscerseiceeadsdagrejh65gghfgdvh76bjtfrgunbyuf5yjhgv7nnnjkhaghjknортолтортпвыолпатвломжщ8бжсде50з4юц8г2982нз7зюэюваюпвпджощш39з8г43бпыцгупавыгачитисимьбччибчлмчбнев43вы513вы5412вфмтяисмячстьнуцьашфаплдорвпщзфгзщгзщвфшгпххгщгхфгрсчимсамопяihlhjgtsi7hgu6hfduyjkdfsjgscemtvyli7ymmm54ub9uyo854umoöööüijvitr618345887e(clientId, val);
		}
	}

	[ServerReceiver(10131)]
	private static void Server_RequestStopExcercise(knetid clientId, ref NetDataReader reader)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (NetPlayer.TryGetNetPlayerAndBodyFromClientId(clientId, out var plr, out var body))
		{
			ForceStopExcercising(body);
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(Vector2.op_Implicit(((Component)body).transform.position), $"S: RequestStopExcercise {plr} ");
			}
		}
	}

	[ClientReceiver(10132, true)]
	private static void Client_StartExcercise(knetid _, ref NetDataReader reader)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out knetid result);
		byte b = default(byte);
		reader.Get(ref b);
		WorkoutType val = (WorkoutType)b;
		if (NetPlayer.TryGetNetPlayerAndBodyFromClientId(result, out var plr, out var body) && !plr.is_local)
		{
			((MonoBehaviour)body).StartCoroutine(body.DoWorkout(val));
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(Vector2.op_Implicit(((Component)body).transform.position), $"C: StartExcerciseRelay {plr} {val} ");
			}
		}
	}

	[ClientReceiver(10133, true)]
	private static void Client_StopExcercise(knetid _, ref NetDataReader reader)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out knetid result);
		if (NetPlayer.TryGetNetPlayerAndBodyFromClientId(result, out var plr, out var body))
		{
			ForceStopExcercising(body);
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(Vector2.op_Implicit(((Component)body).transform.position), $"C: StopExcercise {plr} ");
			}
		}
	}
}
