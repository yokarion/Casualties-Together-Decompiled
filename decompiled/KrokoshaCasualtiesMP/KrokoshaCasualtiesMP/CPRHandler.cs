using System.Collections.Generic;
using System.Linq;
using KrokoshaCasualtiesUtils;
using LiteNetLib;
using LiteNetLib.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KrokoshaCasualtiesMP;

public class CPRHandler : MonoBehaviour
{
	public double last_click_time;

	public double pressed_time;

	public float last_full_cycle_time;

	public float last_hold_time;

	public float last_nohold_time_permin;

	public float last_hold_time_permin;

	public float cur_cpr_rate;

	public static float CPR_RATE_SMOOTHING = 0.3f;

	public bool is_minigame = true;

	public static RangeF AcceptableRate = new RangeF(65f, 220f);

	public Body healer;

	public Body pacient;

	public RectTransform MG_SecondHandTransform;

	public Image MG_SecondHandSprite;

	public static int SMART_INT_REQUIREMENT = 12;

	public static int MINIMUM_STR_REQUIREMENT = 6;

	public bool is_in_cpr_zone;

	public bool is_ui_overlapping;

	public bool ready_for_cpr;

	public bool clicking_into_cpr;

	public bool clicking_not_into_cpr;

	public string cpr_status = "";

	public double cpr_status_last_change;

	public float cpr_rot;

	private float smooth_cpr_handscale = 1f;

	private static float CPR_SCALE_PRESS_TARGET = 0.55f;

	public static float CPR_STAMINA_USE_MULTIPLIER = 1f;

	private static float CHANCE_TO_BREAK_RIBS = 0.008f;

	private static int REQUIRED_PRESS_COUNT_FOR_HEART_KICKSTART = 30;

	public bool aed_minigame_overlapping_with_pads;

	public Vector2 last_minigame_mouse_localpos = Vector2.zero;

	public Limb mainhand => healer.GetPrimaryHand();

	public Limb secondhand => healer.GetSecondaryHand();

	public bool stupid => healer.skills.INT < SMART_INT_REQUIREMENT;

	private float cpr_press_scalar => smooth_cpr_handscale.RemapClamped(1f, CPR_SCALE_PRESS_TARGET, 0f, 1f);

	public static MinigameBase game => MinigameBase.main;

	public void Awake()
	{
		healer = Util.GetLocalBody();
	}

	public void LateUpdate()
	{
		if (is_minigame && game.currentMinigame == null)
		{
			Object.Destroy((Object)(object)this);
		}
	}

	public void MG_End()
	{
		game.EndMinigame();
		Object.Destroy((Object)(object)this);
	}

	public void MG_CreateSecondHand()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		GameObject obj = Object.Instantiate<GameObject>(((Component)MinigameBase.main.guideText).gameObject, game.spawnedMiniGame, false);
		RectTransform component = obj.GetComponent<RectTransform>();
		TextMeshProUGUI component2 = obj.GetComponent<TextMeshProUGUI>();
		((TMP_Text)component2).text = Lang.Get("click_to_cpr", false);
		component.offsetMin = Vector2.zero;
		component.offsetMax = Vector2.zero;
		component.anchorMin = Vector2.one * 0.25f;
		component.anchorMax = Vector2.one * 0.75f;
		((Graphic)component2).color = ((Graphic)component2).color * 0.5f;
		((TMP_Text)component2).fontSize = ((TMP_Text)component2).fontSize * 1.3f;
		GameObject val = Object.Instantiate<GameObject>(((Component)MinigameBase.main.handTransform).gameObject, game.spawnedMiniGame, false);
		MG_SecondHandTransform = val.GetComponent<RectTransform>();
		MG_SecondHandSprite = val.GetComponent<Image>();
		Vector3 localScale = ((Transform)MinigameBase.main.handTransform).localScale;
		localScale.x *= -1f;
		((Transform)MG_SecondHandTransform).localScale = localScale;
		((Graphic)MG_SecondHandSprite).color = new Color(1f, 1f, 1f, 0f);
	}

	public static float GetHandMaxTotalForce(Body body)
	{
		Limb obj = body.limbs[5];
		Limb val = body.limbs[8];
		return Mathf.Max(obj.totalForce, val.totalForce);
	}

	public static bool CheckIfStrongEnoughToPerformCPR(Body body)
	{
		float handMaxTotalForce = GetHandMaxTotalForce(body);
		if ((float)body.skills.STR * handMaxTotalForce > (float)MINIMUM_STR_REQUIREMENT)
		{
			return true;
		}
		return false;
	}

	public static bool CheckIfBodyIsNotOkAndNeedCPR(Body body)
	{
		if ((body.heartRate < 50f && body.bloodOxygen < 100f && body.bloodPressure < 30f) || body.heartRate < 5f || body.isDying || body.isCriticallyDying)
		{
			return true;
		}
		return false;
	}

	public static bool IsCPRBeingPerformedOnThisBody(Body b, out NetPlayer who)
	{
		foreach (NetPlayer allLivingPlayer in NetPlayer.AllLivingPlayers)
		{
			if (allLivingPlayer.body.conscious && allLivingPlayer.minigame_is_in_a_minigame && (allLivingPlayer.minigame_current_type == MinigameMPManager.GetMinigameTypeId(typeof(CPRMinigame)) || allLivingPlayer.minigame_current_type == MinigameMPManager.GetMinigameTypeId(typeof(AEDMinigame))) && allLivingPlayer.minigame_targetobject.TryGetNetBody(out var nb) && (Object)(object)nb.body == (Object)(object)b)
			{
				who = allLivingPlayer;
				return true;
			}
		}
		who = null;
		return false;
	}

	public static bool IsCPRBeingPerformedOnThisBody_IsMeOrNobody(Body b)
	{
		if (IsCPRBeingPerformedOnThisBody(b, out var who))
		{
			return who.is_local;
		}
		return true;
	}

	public static bool IsCPRBeingPerformedOnThisBody_IsThisGuyOrNobody(Body b, NetPlayer plr)
	{
		if (IsCPRBeingPerformedOnThisBody(b, out var who))
		{
			return (Object)(object)who == (Object)(object)plr;
		}
		return true;
	}

	public static void CPR_UseStaminaOnPress(Body body)
	{
		body.stamina -= 0.9f * CPR_STAMINA_USE_MULTIPLIER;
		body.temperature += 0.02f * CPR_STAMINA_USE_MULTIPLIER;
	}

	private void HandStoppedCPRPress()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		float num = Mathf.Max(0.001f, (float)(Time.unscaledTimeAsDouble - pressed_time));
		float num2 = Mathf.Max(0.001f, (float)(Time.unscaledTimeAsDouble - last_click_time));
		float num3 = Mathf.Max(0.001f, num2 - num);
		AcceptableRate.Widen(10f);
		last_click_time = Time.unscaledTimeAsDouble;
		float num4 = 60f / (num * 2f);
		float num5 = 60f / (num3 * 2f);
		float num6 = Mathf.LerpUnclamped(num4, num5, 0.5f);
		num4 = Mathf.LerpUnclamped(num4, num6, CPR_RATE_SMOOTHING);
		num5 = Mathf.LerpUnclamped(num5, num6, CPR_RATE_SMOOTHING);
		last_full_cycle_time = num2;
		last_hold_time = num;
		last_hold_time_permin = num4;
		last_nohold_time_permin = num5;
		float number = Mathf.LerpUnclamped(60f / num2, num6, CPR_RATE_SMOOTHING);
		cur_cpr_rate = Mathf.Lerp(cur_cpr_rate, number, 0.5f);
		NetBody netBody = default(NetBody);
		if (((Component)pacient).TryGetComponent<NetBody>(ref netBody))
		{
			NetDataWriter writer = Net.CreateWriter(10050);
			writer.Put((ushort)netBody.netId);
			writer.Put(num);
			writer.Put(num3);
			writer.Put(number);
			Net.Client_Send((DeliveryMethod)0, in writer);
		}
		if (!stupid)
		{
			if (AcceptableRate.IsInRange(in num4) && AcceptableRate.IsInRange(in num5))
			{
				cpr_status = Lang.Get("cpr_minigame_good", false);
			}
			else if (AcceptableRate.IsInRange(in number))
			{
				cpr_status = Lang.Get("cpr_minigame_bad_timing", false);
			}
			else if (num6 > AcceptableRate.Center())
			{
				cpr_status = Lang.Get("cpr_minigame_fast", false);
			}
			else
			{
				cpr_status = Lang.Get("cpr_minigame_slow", false);
			}
			cpr_status_last_change = Time.realtimeSinceStartupAsDouble;
		}
	}

	[ServerReceiver(10050)]
	private static void ServerReceiver_CPR_PRESSED(knetid clientId, ref NetDataReader reader)
	{
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_04de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0425: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out knetid result);
		float num = default(float);
		reader.Get(ref num);
		float num2 = default(float);
		reader.Get(ref num2);
		float num3 = default(float);
		reader.Get(ref num3);
		if (!NetPlayer.TryGetNetPlayerAndNetBodyFromClientId(clientId, out var _, out var pb) || !NetBody.TryGetNetBodyFromId(result, out var nb) || !num.IsFinite() || !num2.IsFinite() || !(num > 0f) || !(num2 > 0f))
		{
			return;
		}
		Body body = pb.body;
		Body body2 = nb.body;
		if (!body.IsBodyLocal())
		{
			CPR_UseStaminaOnPress(body);
		}
		if (!CheckIfStrongEnoughToPerformCPR(body))
		{
			return;
		}
		float num4 = (float)(Time.realtimeSinceStartupAsDouble - nb.cpr_last_press_time);
		nb.cpr_last_press_time = Time.realtimeSinceStartupAsDouble;
		if (num4 <= 0f)
		{
			return;
		}
		if (num4 > 2f)
		{
			nb.cpr_cur_perfect_press_count = 0;
		}
		float num5 = Mathf.Abs(num - num2);
		float num6 = num + num2;
		float number = 60f / num6;
		num3 = number;
		if (num6 <= 0f)
		{
			return;
		}
		if (!AcceptableRate.IsInRange(in number))
		{
			nb.cpr_cur_perfect_press_count = 0;
			return;
		}
		if (num4 < 2f)
		{
			nb.cpr_cur_perfect_press_count++;
		}
		if (num6 * 0.9f > num4)
		{
			float num7 = num4 / num6;
			num3 = 60f / num4;
			num6 = num4;
			num *= num7;
			num2 *= num7;
		}
		float num8 = Mathf.Clamp01(1f - num5);
		float num9 = AcceptableRate.Center();
		float num10 = 0f;
		if (num8 > 0.2f)
		{
			bool flag = body2.IsRibsBroken();
			if (flag)
			{
				num8 = Mathf.Lerp(num8, 1f, 0.5f);
			}
			if (num8 > 0.5f)
			{
				if (body2.bloodVolume > 0f)
				{
					num10 = Mathf.Clamp(body2.bloodVolume, 30f, 110f) * Mathf.Clamp(num * 1.13f, 0.06f, 0.3f) * num8;
				}
				if (body2.bloodOxygen < 50f && (body2.IsAirBreathable() || body.IsAirBreathable()))
				{
					body2.bloodOxygen += num10 * 0.7f;
				}
				if (body2.bloodPressure < 100f)
				{
					body2.bloodPressure += num10;
				}
				if (num3 > num9 && num8 < 0.9f && !flag && body.skills.STR > 8)
				{
					float num11 = ((float)body.skills.STR - (float)body.skills.INT * 0.7f) * CHANCE_TO_BREAK_RIBS;
					if (num11 > 0f && Random.value < num11)
					{
						Limb upperTorso = body2.GetUpperTorso();
						DebugHelp.OnNetEvent(upperTorso.GetPosition(), $"BROKE RIBS LOOOL  chance:{num11}");
						if (!upperTorso.broken)
						{
							upperTorso.broken = true;
							upperTorso.skinHealth -= 10f;
							upperTorso.pain += Random.Range(20f, 50f);
							Body body3 = upperTorso.body;
							body3.adrenaline += 30f;
							upperTorso.MakeBoneSprite();
							Sound.Play("BoneBreak" + Random.Range(1, 4), Vector2.op_Implicit(((Component)upperTorso).transform.position), false, true, (Transform)null, 1f, 1f, false, false);
							upperTorso.boneHealTimer = 100f;
							upperTorso.body.PromptTalk();
							upperTorso.body.DoGoreSound();
							Body body4 = upperTorso.body;
							body4.internalBleeding += 5f + num11;
						}
						else
						{
							upperTorso.boneHealTimer = 100f;
						}
					}
				}
				if (body2.bloodPressure > 80f && nb.cpr_cur_perfect_press_count > REQUIRED_PRESS_COUNT_FOR_HEART_KICKSTART && body2.alive && body2.inCardiacArrest && body2.bloodVolume > 10f)
				{
					float num12 = Random.value * num8;
					if (num12 > 0.8f)
					{
						DebugHelp.OnNetEvent(body2.GetPosition(), $"KICKSTARTED THE HEART  cur roll:{num12}   cur press count:{nb.cpr_cur_perfect_press_count} ");
						body2.heartRate = Mathf.Max(body2.heartRate, Mathf.LerpUnclamped(body2.heartRate, num3 * 0.6f, num8));
					}
				}
				MedicalSync.Server_QueueSendCharacterHealth(nb);
			}
		}
		if (log.verbose)
		{
			string text = $"SERVER: :: CPR PRESSED :: EFFECT:{num8}\tCENTER:{num9}\tpumpamount:{num10}\tpress count:{nb.cpr_cur_perfect_press_count}";
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(nb.position, in text);
			}
		}
	}

	public void MG_Update(List<RaycastResult> uiCasts)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Expected O, but got Unknown
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e2: Unknown result type (might be due to invalid IL or missing references)
		if ((pacient.standing && pacient.moveDir != Vector2.zero) || !healer.standing)
		{
			MG_End();
			PlayerCamera.main.DoAlert(Lang.Get("plr_moving", false), false);
			return;
		}
		bool flag = false;
		if (is_minigame && game.currentMinigame != null)
		{
			flag = game.currentMinigame is AEDMinigame;
			if (Time.realtimeSinceStartupAsDouble - cpr_status_last_change < 5.0 && !string.IsNullOrEmpty(cpr_status))
			{
				((TMP_Text)game.guideText).text = game.currentMinigame.GuideLocaleString() + "\n" + cpr_status;
			}
		}
		Vector2 localMousePosMinigameSpace = MinigameMPManager.GetLocalMousePosMinigameSpace();
		bool flag2 = is_in_cpr_zone;
		int num = 110;
		bool flag3 = Mathf.Abs(game.handPos.x) < (float)num && Mathf.Abs(game.handPos.y) < (float)num;
		bool flag4 = Mathf.Abs(localMousePosMinigameSpace.x) < (float)num && Mathf.Abs(localMousePosMinigameSpace.y) < (float)num;
		is_in_cpr_zone = flag3 && flag4;
		if (flag && is_in_cpr_zone)
		{
			AEDMinigame as_aed = (AEDMinigame)game.currentMinigame;
			if (last_minigame_mouse_localpos != localMousePosMinigameSpace || flag2 != is_in_cpr_zone)
			{
				aed_minigame_overlapping_with_pads = uiCasts.Count((RaycastResult x) => as_aed.pads.Contains(((RaycastResult)(ref x)).gameObject.GetComponent<RectTransform>())) > 0;
			}
			if ((Object)(object)as_aed.currentlyHeld != (Object)null)
			{
				aed_minigame_overlapping_with_pads = true;
			}
		}
		else
		{
			aed_minigame_overlapping_with_pads = false;
		}
		is_ui_overlapping = aed_minigame_overlapping_with_pads || uiCasts.Count((RaycastResult x) => ((RaycastResult)(ref x)).gameObject.transform.IsChildOf(game.spawnedMiniGame) && (Object)(object)((RaycastResult)(ref x)).gameObject.GetComponent<Button>() != (Object)null) > 0;
		float num2 = Mathf.Sign(((Transform)game.handTransform).localScale.x);
		float num3 = (float)(stupid ? 30 : 36) * num2;
		Image component = ((Component)game.handTransform).GetComponent<Image>();
		RectTransform handTransform = game.handTransform;
		((Transform)handTransform).localScale = ((Transform)handTransform).localScale * smooth_cpr_handscale;
		Vector2 handPos = game.handPos;
		handPos.x *= -1f;
		MG_SecondHandTransform.anchoredPosition = handPos;
		Vector3 localScale = ((Transform)game.handTransform).localScale;
		localScale.x *= -1f;
		((Transform)MG_SecondHandTransform).localScale = localScale;
		((Transform)game.handTransform).eulerAngles = new Vector3(0f, 0f, game.handVelocity.x * 0.5f + cpr_rot);
		((Transform)MG_SecondHandTransform).eulerAngles = new Vector3(0f, 0f, game.handVelocity.x * 0.5f - cpr_rot);
		Color color = ((Graphic)MG_SecondHandSprite).color;
		float num4 = 0f;
		if (!clicking_not_into_cpr && (ready_for_cpr || (!is_ui_overlapping && (game.handPos.x * num2 > 1f || is_in_cpr_zone))))
		{
			num4 = Mathf.Min(Mathf.Abs(game.handPos.x).RemapClamped(120f, 90f, 0f, 0.995f), Mathf.Abs(game.handPos.y).RemapClamped(120f, 90f, 0f, 0.9951f));
			if (!ready_for_cpr)
			{
				num4 *= num4 * 0.5f;
			}
		}
		color.a = Mathf.MoveTowards(color.a, num4, Time.deltaTime * 10f);
		((Graphic)MG_SecondHandSprite).color = color;
		Color color2 = ((Graphic)component).color;
		color2.a = Mathf.Abs(game.handPos.x).RemapClamped(220f, 130f, 0.8f, 0.99f);
		((Graphic)component).color = color2;
		if (game.handStoppedClicking && clicking_into_cpr)
		{
			HandStoppedCPRPress();
		}
		if (game.handStartedClicking)
		{
			Component ba = (Component)(object)pacient;
			Component bb = (Component)(object)healer;
			if (!KM.dist2dsqrcheck(in ba, in bb, SharedMain.max_player_interaction_distance * 2f))
			{
				MG_End();
				PlayerCamera.main.DoAlert(Lang.Get("plr_too_far", false), false);
				return;
			}
			clicking_into_cpr = ready_for_cpr;
			if (clicking_into_cpr)
			{
				pressed_time = Time.unscaledTimeAsDouble;
				CPR_UseStaminaOnPress(healer);
				NetBody netBody = default(NetBody);
				NetBody victim = default(NetBody);
				if (!CheckIfStrongEnoughToPerformCPR(healer))
				{
					PlayerCamera.main.DoAlert(Lang.Get("ur_too_weak", false), false);
					if (!flag)
					{
						MG_End();
						return;
					}
				}
				else if (smooth_cpr_handscale > 0.9f && !pacient.IsRagdolling() && !pacient.grounded && ((Component)healer).TryGetComponent<NetBody>(ref netBody) && ((Component)pacient).TryGetComponent<NetBody>(ref victim))
				{
					netBody.Push(victim);
					MG_End();
					return;
				}
			}
		}
		else if (!game.handClicking)
		{
			clicking_into_cpr = false;
			ready_for_cpr = is_in_cpr_zone && !is_ui_overlapping;
		}
		clicking_not_into_cpr = game.handClicking && !clicking_into_cpr;
		float num5 = 1f;
		float num6 = 0f;
		if (ready_for_cpr)
		{
			num6 = num3;
			if (game.handClicking)
			{
				component.sprite = game.handSprites[0];
				num5 = CPR_SCALE_PRESS_TARGET;
			}
			else
			{
				component.sprite = game.handSprites[1];
			}
			if (secondhand.dismembered)
			{
				MG_SecondHandSprite.sprite = game.handSprites[10];
			}
			else
			{
				MG_SecondHandSprite.sprite = component.sprite;
			}
			if (mainhand.dismembered)
			{
				component.sprite = game.handSprites[10];
			}
		}
		cpr_rot = Mathf.MoveTowards(cpr_rot, num6, Time.deltaTime * 360f);
		smooth_cpr_handscale = Mathf.Lerp(smooth_cpr_handscale, num5, Time.deltaTime * GetPushSpeed(Util.GetLocalBody(), pacient, game.handClicking));
		last_minigame_mouse_localpos = localMousePosMinigameSpace;
	}

	public void MG_PhysicsUpdate(float deltaTime)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		if (ready_for_cpr)
		{
			Vector2 val = default(Vector2);
			((Vector2)(ref val))._002Ector(40f, 0f);
			if (!stupid)
			{
				((Vector2)(ref val))._002Ector(-40f, 5f);
			}
			game.handPos = Vector2.Lerp(game.handPos, val * smooth_cpr_handscale, deltaTime * 10f);
			game.handVelocity = Vector2.Lerp(game.handVelocity, Vector2.zero, deltaTime * 10f);
		}
	}

	public static float GetPushSpeed(Body healer, Body pacient, bool pressing)
	{
		float num = healer.consciousness * 0.01f * 7f;
		float num2 = 0f;
		num2 += 10f + num + healer.skills.STRFrom10 * 0.5f;
		if (pacient.IsRibsBroken())
		{
			num2 += 5f;
			if (!pressing)
			{
				num2 = Mathf.Max(18f, num2);
			}
		}
		else if (!pressing)
		{
			num2 = Mathf.Max(10f, num2);
		}
		if (pressing)
		{
			num2 *= Mathf.Clamp(0.6f + GetHandMaxTotalForce(healer) * 0.5f, 0.6f, 1f);
		}
		return num2;
	}
}
