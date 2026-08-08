using System.Collections.Generic;
using HarmonyLib;
using LiteNetLib;
using LiteNetLib.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(KeypadMinigame), "CheckCasts")]
public static class KeypadMinigame_CheckCasts_MultiplayerPatch
{
	public static float GetUsabilityDistanceMP(UsableObject uo)
	{
		return 10f * uo.rangeMultiplier * 1.4f;
	}

	public static bool CheckIfCloseEnoughForMinigame(UsableObject uo, NetBody pb)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		return KM.dist2dsqrcheck(Vector2.op_Implicit(((Component)uo).transform.position), pb.pos, GetUsabilityDistanceMP(uo));
	}

	private static bool Prefix(KeypadMinigame __instance, List<RaycastResult> uiCasts)
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		if (!KrokoshaScavMultiplayer.network_system_is_running)
		{
			return true;
		}
		if ((Object)(object)__instance.toDestroy == (Object)null || !CheckIfCloseEnoughForMinigame(((Component)__instance.toDestroy).GetComponent<UsableObject>(), NetPlayer.LOCAL_PLAYER.playerbody))
		{
			MinigameBase.main.EndMinigame();
			return false;
		}
		if (NetObjectRegistry.TryGetSyncInfo(((Component)__instance.toDestroy).gameObject, out var si))
		{
			int num = -1;
			foreach (RaycastResult uiCast in uiCasts)
			{
				RaycastResult current = uiCast;
				if (__instance.inputButtons.Contains(((RaycastResult)(ref current)).gameObject))
				{
					num = int.Parse(((TMP_Text)((Component)((RaycastResult)(ref current)).gameObject.transform.GetChild(0)).GetComponent<TextMeshProUGUI>()).text);
					break;
				}
				if ((Object)(object)((RaycastResult)(ref current)).gameObject == (Object)(object)Traverse.Create((object)__instance).Field("clearButton").GetValue<GameObject>())
				{
					num = 10;
					break;
				}
			}
			if (num != -1)
			{
				if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
				{
					DebugHelp.OnNetEvent(Vector2.op_Implicit(((Component)__instance.toDestroy).transform.position), $"C: sending Keypress {num}");
				}
				NetDataWriter writer = Net.CreateWriter(10068);
				writer.Put((ushort)si.syncId);
				writer.Put((byte)num);
				Net.Client_Send((DeliveryMethod)0, in writer);
			}
		}
		return true;
	}

	private static void Postfix(KeypadMinigame __instance)
	{
		if (KrokoshaScavMultiplayer.IsNetworkActiveAndIsHost())
		{
			MinigameMPManager.MinigameSession sessionThisPlayerIsInvolvedIn = MinigameMPManager.GetSessionThisPlayerIsInvolvedIn(NetPlayer.LOCAL_PLAYER);
			if (sessionThisPlayerIsInvolvedIn != null && sessionThisPlayerIsInvolvedIn is MinigameMPManager.KeypadMinigameSession keypadMinigameSession)
			{
				__instance.current = keypadMinigameSession.current_input;
			}
		}
	}

	[ServerReceiver(10068)]
	private static void Server_KeypadMinigame_Keypress(knetid clientId, ref NetDataReader reader)
	{
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out knetid result);
		byte b = default(byte);
		reader.Get(ref b);
		UsableObject uo = default(UsableObject);
		Openable val = default(Openable);
		if (!NetPlayer.TryGetPlayerFromClientId(clientId, out var plr) || !Object.op_Implicit((Object)(object)plr.body) || !plr.body.conscious || !NetObjectRegistry.TryGetSyncInfo(result, out var si) || !si.IsBuilding() || !si.go.TryGetComponent<UsableObject>(ref uo) || !si.go.TryGetComponent<Openable>(ref val) || !val.isKeypad)
		{
			return;
		}
		if (b < 0 || b > 10)
		{
			log.sus($"weirdass keypad number {plr} -> num: {b} ");
			return;
		}
		MinigameMPManager.MinigameSession sessionThisPlayerIsInvolvedIn = MinigameMPManager.GetSessionThisPlayerIsInvolvedIn(plr);
		if (sessionThisPlayerIsInvolvedIn == null || !(sessionThisPlayerIsInvolvedIn is MinigameMPManager.KeypadMinigameSession keypadMinigameSession))
		{
			return;
		}
		if (CheckIfCloseEnoughForMinigame(uo, plr.playerbody))
		{
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(Vector2.op_Implicit(((Component)plr.body).transform.position), $"S: KeypadMinigame_Keypress {b} {si}");
			}
			if (b == 10)
			{
				b = 0;
				keypadMinigameSession.current_input = "";
			}
			else
			{
				keypadMinigameSession.current_input += b;
			}
			if (keypadMinigameSession.current_input.Length > keypadMinigameSession.match.Length)
			{
				keypadMinigameSession.current_input = keypadMinigameSession.current_input.Substring(0, keypadMinigameSession.match.Length);
			}
			else
			{
				keypadMinigameSession.AnnounceCurrentInput();
				if (keypadMinigameSession.current_input == keypadMinigameSession.match)
				{
					keypadMinigameSession.building.health = 0f;
				}
			}
			ServerMain.Server_AnnounceSound(Vector2.op_Implicit(((Component)plr.body).transform.position), "beep" + b, ServerMain.GetListOfClientIdsExceptThis(clientId));
		}
		else
		{
			MinigameMPManager.Server_ForceEndMinigameForPlayer(plr);
		}
	}
}
