using HarmonyLib;
using KrokoshaCasualtiesUtils;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(DislocationMinigame), "CheckForHit")]
public static class DislocationMinigame_CheckForHit_MultiplayerPatch
{
	private class laststate
	{
		public bool CheckForHitActuallyRuns;

		public Vector2 lastvel;
	}

	public static bool just_received_new_packet = false;

	public static Vector2 lastreceived_bonepos = new Vector2(500f, 0f);

	public static Vector2 lastreceived_boneVelocity = Vector2.zero;

	public static Vector2 real_bonepos = new Vector2(500f, 0f);

	public static Vector2 boneVelocity = Vector2.zero;

	public static RectTransform bone => ((Component)Minigame.game.spawnedMiniGame.GetChild(2)).GetComponent<RectTransform>();

	private static void LimitBoneLocationForMP()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		Minigame currentMinigame = Minigame.game.currentMinigame;
		DislocationMinigame val = (DislocationMinigame)(object)((currentMinigame is DislocationMinigame) ? currentMinigame : null);
		Vector2 bone_anchoredPosition = bone.anchoredPosition;
		Vector2 val2 = val.boneVelocity;
		MinigameMPManager.DislocationMinigameSession.BonePositionTick(ref bone_anchoredPosition, ref val2);
		if (MinigameMPManager.DislocationMinigameSession.CalculateDislocationTimer(bone_anchoredPosition) <= 3f)
		{
			Vector2 val3 = bone_anchoredPosition - MinigameMPManager.DislocationMinigameSession.FinishSpot;
			float magnitude = ((Vector2)(ref val3)).magnitude;
			val3 = ((!(magnitude > 1E-05f)) ? Vector2.zero : (val3 / magnitude));
			bone.anchoredPosition = MinigameMPManager.DislocationMinigameSession.FinishSpot + val3 * 3.01f;
			Vector2 val4 = val.boneVelocity - val3 * Vector2.Dot(val.boneVelocity, val3);
			val.boneVelocity = val4;
		}
	}

	private static void Prefix(DislocationMinigame __instance, ref laststate __state)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		if (KrokoshaScavMultiplayer.network_system_is_running)
		{
			Limb limb = __instance.limb;
			boneVelocity = __instance.boneVelocity;
			if (!limb.IsBodyLocal() && limb.body.averagePain > 75f && Util.GetLocalBody().averagePain < 75f)
			{
				limb.body.averagePain = 74f;
			}
			real_bonepos = bone.anchoredPosition;
			__state = new laststate
			{
				CheckForHitActuallyRuns = (limb.body.averagePain > 75f || ((Vector2)(ref boneVelocity)).magnitude > 60f || !Minigame.game.handClicking || ((Vector2)(ref Minigame.game.handVelocity)).magnitude < 4f),
				lastvel = boneVelocity
			};
		}
	}

	private static void Postfix(DislocationMinigame __instance, ref laststate __state)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		if (!KrokoshaScavMultiplayer.network_system_is_running)
		{
			return;
		}
		MinigameBase main = MinigameBase.main;
		bool forceUngrab = main.forceUngrab;
		bool firstFrameForceUngrab = main.firstFrameForceUngrab;
		if (forceUngrab && firstFrameForceUngrab && !__state.CheckForHitActuallyRuns)
		{
			boneVelocity = __instance.boneVelocity;
			Vector2 value = boneVelocity - __state.lastvel;
			NetDataWriter writer = Net.CreateWriter(10065);
			writer.Put(value);
			writer.Put(__instance.hasWrench);
			Net.Client_Send((DeliveryMethod)0, in writer);
			if (KrokoshaScavMultiplayer.is_client)
			{
				MinigameMPManager.client_dislocation_mg_ignoretime = Time.realtimeSinceStartupAsDouble + (double)ClientMain.RTT_IN_SECONDS_with_margin + 1.0;
			}
		}
		double num = MinigameMPManager.client_dislocation_mg_ignoretime - Time.realtimeSinceStartupAsDouble;
		if (num > 0.0)
		{
			float num2 = Mathf.Clamp01(1f - (float)num);
			if (just_received_new_packet)
			{
				just_received_new_packet = false;
				bone.anchoredPosition = Vector2.Lerp(bone.anchoredPosition, lastreceived_bonepos, num2);
			}
			else
			{
				bone.anchoredPosition = Vector2.Lerp(bone.anchoredPosition, lastreceived_bonepos, num2 * Time.deltaTime * 0.5f);
			}
			__instance.boneVelocity = Vector2.Lerp(__instance.boneVelocity, lastreceived_boneVelocity, num2 * Time.deltaTime);
		}
		else if (just_received_new_packet)
		{
			just_received_new_packet = false;
			bone.anchoredPosition = lastreceived_bonepos;
			__instance.boneVelocity = lastreceived_boneVelocity;
		}
		else
		{
			bone.anchoredPosition = Vector2.Lerp(bone.anchoredPosition, lastreceived_bonepos, Time.deltaTime);
		}
		MinigameMPManager.DislocationMinigameSession.BonePositionTick(ref lastreceived_bonepos, ref lastreceived_boneVelocity);
		LimitBoneLocationForMP();
	}

	[ServerReceiver(10065)]
	private static void Server_DislocationMinigame_dislocatingtypeshit(knetid clientId, ref NetDataReader reader)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out Vector2 result);
		bool flag = default(bool);
		reader.Get(ref flag);
		if (!NetPlayer.TryGetNetPlayerAndBodyFromClientId(clientId, out var plr, out var body) || !body.conscious || !result.IsFinite() || !MinigameMPManager.TryGetSessionThisPlayerIsInvolvedIn(plr, out MinigameMPManager.DislocationMinigameSession session) || !(((Vector2)(ref session.boneVelocity)).magnitude <= 60f))
		{
			return;
		}
		if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
		{
			DebugHelp.OnNetEvent(Vector2.op_Implicit(((Component)plr.body).transform.position), $"S: dislocation hit: {result}");
		}
		MinigameMPManager.DislocationMinigameSession dislocationMinigameSession = session;
		dislocationMinigameSession.boneVelocity += Vector2.ClampMagnitude(result, 1300f);
		if (!plr.is_local)
		{
			if (flag)
			{
				Limb limb = session.limb;
				limb.pain += Random.Range(4f, 10f);
			}
			else
			{
				Limb limb2 = session.limb;
				limb2.pain += Random.Range(15f, 24f);
				if (Random.value > 0.995f)
				{
					session.limb.BreakBone();
				}
			}
		}
		ServerMain.Server_AnnounceSound(Vector2.op_Implicit(((Component)session.limb).transform.position), "boneHit", ServerMain.GetListOfClientIdsExceptThis(clientId));
	}
}
