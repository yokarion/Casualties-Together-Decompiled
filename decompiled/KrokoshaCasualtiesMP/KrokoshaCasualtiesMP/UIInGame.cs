using System;
using System.Collections.Generic;
using System.Linq;
using KrokoshaCasualtiesUtils;
using LiteNetLib;
using LiteNetLib.Utils;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public class UIInGame : MonoBehaviour
{
	public static UIInGame main;

	internal bool antidispersion_active;

	internal bool antidispersion_warned;

	internal float antidispersion_cur_distance;

	internal float antidispersion_cur_required_count;

	internal int antidispersion_cur_range_count;

	public static NetBody interaction_menu_target_body = null;

	public static Vector2 interaction_menu_target_pos = Vector2.zero;

	public static bool interaction_menu_focused = false;

	private static bool spectator_only_alive = true;

	private static Transform _last_spectator_plr_transform;

	private static int curspectatefolowing = 0;

	private static string curspectatefolowingname = "EROR";

	public static bool SPECTATOR_MODE { get; private set; }

	public static Vector2 _DEV_FREECAM_CURPOS
	{
		get
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			return ConsoleScript.instance.freecamPos;
		}
		set
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			ConsoleScript.instance.freecamPos = value;
		}
	}

	private static void _GUI__DrawPlayerDirection(NetBody nb, float a)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Expected O, but got Unknown
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		Vector2 to = Vector2.op_Implicit(((Component)nb).transform.position);
		if (to == Util.ClampWorldPositionToCameraView(to))
		{
			return;
		}
		float magnitude;
		Vector2 val = KM.normal(Vector2.op_Implicit(((Component)Camera.main).transform.position), in to, out magnitude);
		((Vector2)(ref val))._002Ector(val.x, 0f - val.y);
		Vector2 val2 = new Vector2((float)Screen.width, (float)Screen.height) * 0.5f * (Vector2.one + val * 0.6f);
		Color color = nb.color;
		color = Utils.WithAlpha(color, a);
		GUI.skin.label.normal.textColor = color;
		Matrix4x4 matrix = GUI.matrix;
		string text = $"{nb.playername} ( {Math.Floor(Util.TilesToMeters(magnitude))} M )";
		Vector2 val3 = GUI.skin.label.CalcSize(new GUIContent(text));
		Vector2 val4 = val2 - val3 * 0.5f;
		Rect val5 = default(Rect);
		((Rect)(ref val5))._002Ector(val4.x, val4.y, val3.x, val3.y);
		GUI.Label(new Rect(val4.x, val4.y, val3.x + 200f, val3.y), text);
		if (nb.is_player)
		{
			Texture2D profilepic_any_smalltolarge = nb.plr.profilepic_any_smalltolarge;
			if ((Object)(object)profilepic_any_smalltolarge != (Object)null)
			{
				GUI.Label(new Rect(val4.x - val3.y * 1.1f, val4.y, val3.y, val3.y), (Texture)(object)profilepic_any_smalltolarge);
			}
		}
		val2 += val * ((Vector2)(ref val3)).magnitude;
		((Vector2)(ref val2))._002Ector(Mathf.Clamp(val2.x, ((Rect)(ref val5)).xMin, ((Rect)(ref val5)).xMax), Mathf.Clamp(val2.y, ((Rect)(ref val5)).yMin, ((Rect)(ref val5)).yMax));
		val2 += val * 32f;
		float num = 32f * UIBullshit.uiScale;
		val2 -= Vector2.one * num * 0.5f;
		GUIUtility.RotateAroundPivot(Vector2.SignedAngle(Vector2.right, val), val2);
		GUI.DrawTexture(new Rect(val2.x, val2.y, num, num), (Texture)(object)KrokoshaCoopModAssets.arrowicon.texture, (ScaleMode)0, true, 0f, color, 0f, 0f);
		GUI.matrix = matrix;
		GUI.skin.label.normal.textColor = Color.white;
	}

	private static void _GUI_DoPlayerPointersGui()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		GUI.skin.label.normal.textColor = Color.white;
		foreach (NetPlayer value in NetPlayer.BodyToPlayerDict.Values)
		{
			if (!value.is_local && !((Object)(object)value.body == (Object)null) && value.body.alive)
			{
				float a = 1f;
				if (Util.IsInGameAndPauseMenu() || Input.GetKey(KeyBinds.GetBind("krokosha_coop_showplrs")))
				{
					_GUI__DrawPlayerDirection(value.playerbody, a);
				}
			}
		}
		GUI.skin.label.normal.textColor = Color.white;
	}

	private static void _GUI_DoSpectatorGui()
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Expected O, but got Unknown
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		StopPlayerInteractionMenu();
		if (Util.IsInWoundView() || !Util.IsWorldGenerated())
		{
			return;
		}
		int num = 0;
		float uiScale = UIBullshit.uiScale;
		float num2 = (float)Screen.width * 0.5f;
		float y = (float)Screen.height * 0.54f + 70f * uiScale;
		float width = 60f * uiScale;
		float height = 50f * uiScale;
		float num3 = Mathf.Max(220f * uiScale, GUI.skin.label.CalcSize(new GUIContent(curspectatefolowingname)).x + 16f * uiScale);
		float num4 = 45f * uiScale;
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(num2 - num3 / 2f, y, num3, height);
		UIBullshit._GUI_9SlicePanel(in rect, 1f, 0.8f, check_overlap: true, UIBullshit.unscaled_uiBlockNano);
		TextAnchor alignment = GUI.skin.label.alignment;
		GUI.skin.label.alignment = (TextAnchor)4;
		GUI.Label(rect, curspectatefolowingname);
		GUI.skin.label.alignment = alignment;
		num3 += width * 2f;
		if (GUI.Button(UIBullshit.ConstructorCheckCursorOverlap(num2 - num3 / 2f, in y, in width, in height), "<"))
		{
			ServerMain._ded_server_switch_counter = -30f;
			num--;
			PlayerCamera.main.PlayUISound((UISoundType)0, 1f);
		}
		if (GUI.Button(UIBullshit.ConstructorCheckCursorOverlap(num2 + num3 / 2f - width, in y, in width, in height), ">"))
		{
			ServerMain._ded_server_switch_counter = -30f;
			num++;
			PlayerCamera.main.PlayUISound((UISoundType)0, 1f);
		}
		y += height * 1.1f;
		spectator_only_alive = GUI.Toggle(new Rect(num2 - num3 / 2f, y, num3, num4), spectator_only_alive, Lang.Get("spectate_alive", false));
		y += num4;
		Body val = default(Body);
		if (Object.op_Implicit((Object)(object)PlayerCamera.main.following) && ((Component)PlayerCamera.main.following).TryGetComponent<Body>(ref val) && !Util.IsInWoundView())
		{
			if (GUI.Button(new Rect(num2 - num3 / 2f, y, num3, num4), Lang.Get("spectate_openwoundview", false)))
			{
				PlayerCamera.main.ToggleWoundView(true);
				return;
			}
			y += num4;
		}
		if (!Con._DEV_FREECAM && KrokoshaScavMultiplayer.rules.AllowSpectatorFreecam)
		{
			if (GUI.Button(new Rect(num2 - num3 / 2f, y, num3, num4), Lang.Get("spectate_freecam", false)))
			{
				Util.PlayUISound((UISoundType)1);
				Util.DoAlert(Lang.Get("freecam_keybindalert", false), false);
				StartFreecamMode();
				return;
			}
			y += num4;
		}
		if (GUI.Button(new Rect(num2 - num3 / 2f, y, num3, num4), Lang.Get("spectate_exit", false)))
		{
			PlayerCamera.main.PlayUISound((UISoundType)2, 1f);
			StopSpectatorMode();
			return;
		}
		y += num4;
		if (num == 0)
		{
			return;
		}
		IList<NetPlayer> list = LinqUtility.AsReadOnlyList<NetPlayer>((IEnumerable<NetPlayer>)NetPlayer.BodyToPlayerDict.Values);
		int num5 = list.Count();
		for (int i = 0; i < list.Count(); i++)
		{
			curspectatefolowing += num;
			if (curspectatefolowing < 0)
			{
				curspectatefolowing = num5 - 1;
			}
			if (curspectatefolowing >= num5)
			{
				curspectatefolowing = 0;
			}
			NetPlayer netPlayer = list[curspectatefolowing];
			if ((Object)(object)netPlayer.body != (Object)null && (!spectator_only_alive || netPlayer.body.alive))
			{
				Spectator_SetTarget((Component)(object)netPlayer.playerbody);
				break;
			}
		}
	}

	private static bool _GUI_DoGeneralMPGui()
	{
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		float uiScale = UIBullshit.uiScale;
		float num = 340f * uiScale;
		if (Util.TryGetLocalBody(out var body))
		{
			if ((Object)(object)PlayerCamera.main.following == (Object)(object)_last_spectator_plr_transform)
			{
				PlayerCamera.main.following = null;
			}
			if (body.sleeping && body.alive && !KrokoshaScavMultiplayer.rules.DisableSleep)
			{
				if (!Util.IsInPauseMenuOrMainMenu() && !ServerMain.CheckIfEveryoneIsSleeping())
				{
					float num2 = num + 20f * uiScale;
					float num3 = Math.Min((float)Screen.width * 0.5f + num, (float)Screen.width - num2);
					float num4 = (float)Screen.height * 0.45f - 100f * uiScale;
					int num5 = 0;
					int num6 = 0;
					foreach (KeyValuePair<Body, NetPlayer> item in NetPlayer.BodyToPlayerDict)
					{
						if (item.Key.conscious)
						{
							num5++;
						}
						else
						{
							num6++;
						}
					}
					float num7 = (float)(170 + 30 * num5) * uiScale;
					UIBullshit._GUI_9SlicePanel(new Rect(num3 - (num2 - num) * 0.5f, num4, num2, num7), 0.9f, 0.5f, check_overlap: false, UIBullshit.unscaled_uiBlockNano);
					num4 += 10f * uiScale;
					int fontSize = GUI.skin.label.fontSize;
					GUI.skin.label.fontSize = (int)((float)fontSize * 1.3f);
					if (GUI.Button(new Rect(num3, num4, num, 100f * uiScale), Lang.Get("wake_up", false)))
					{
						if (body.sleeping)
						{
							body.WakeUp();
						}
						KrokoshaScavMultiplayer.Client_SendSimpleMessageToServer((ushort)10135);
					}
					GUI.skin.label.fontSize = fontSize;
					num4 += 100f * uiScale;
					num4 += 10f * uiScale;
					GUI.Label(new Rect(num3, num4, num + 100f * uiScale, 40f * uiScale), Lang.Get("plr_sleeping_count", false) + num6);
					num4 += 40f * uiScale;
					foreach (KeyValuePair<Body, NetPlayer> item2 in NetPlayer.BodyToPlayerDict)
					{
						if (item2.Key.conscious)
						{
							GUI.Label(new Rect(num3, num4, num + 300f * uiScale, 40f * uiScale), item2.Value.playername + Lang.Get("plr_not_sleeping", false));
							num4 += 30f * uiScale;
						}
					}
					num4 += 30f * uiScale;
				}
				return true;
			}
			if (KrokoshaScavMultiplayer.is_dedicated_server)
			{
				return true;
			}
			NetPlayer lOCAL_PLAYER = NetPlayer.LOCAL_PLAYER;
			if (!Util.IsInWoundView() && lOCAL_PLAYER.body.conscious)
			{
				float num8 = 80f * uiScale;
				if ((Object)(object)lOCAL_PLAYER.playerbody.piggybacking_on != (Object)null)
				{
					if (UIBullshit._GUI_FocusableButton(new Rect((float)Screen.width * 0.5f - num / 2f, (float)Screen.height * 0.9f - 10f * uiScale, num, num8), Lang.Get("piggyback_stop", false) + "\n" + lOCAL_PLAYER.playerbody.piggybacking_on.bodyname, out var focused))
					{
						lOCAL_PLAYER.playerbody.StopPiggyback();
					}
					interaction_menu_focused |= focused;
				}
				else if ((Object)(object)lOCAL_PLAYER.playerbody.carrying_person != (Object)null)
				{
					if (UIBullshit._GUI_FocusableButton(new Rect((float)Screen.width * 0.5f - num / 2f, (float)Screen.height * 0.9f - 10f * uiScale, num, num8), Lang.Get("carry_stop", false) + "\n" + lOCAL_PLAYER.playerbody.carrying_person.bodyname, out var focused2))
					{
						lOCAL_PLAYER.playerbody.carrying_person.SetNetIgnoreTime(0.2f);
						KrokoshaScavMultiplayer.Client_SendSimpleMessageToServer((ushort)10034);
						lOCAL_PLAYER.playerbody.carrying_person.StopPiggyback();
					}
					interaction_menu_focused |= focused2;
				}
			}
			if (KrokoshaScavMultiplayer.rules.SpectateWhileUnconscious)
			{
				if (body.conscious)
				{
					return true;
				}
			}
			else if (body.alive)
			{
				return true;
			}
		}
		return false;
	}

	public static void StartFreecamMode(bool resetpos = true)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		Con._DEV_FREECAM = true;
		if (resetpos)
		{
			_DEV_FREECAM_CURPOS = Vector2.op_Implicit(((Component)PlayerCamera.main).transform.position);
		}
		MoodleManager.main.SetBody(Util.GetLocalBody());
		PlayerCamera.main.radialOpen = false;
	}

	public static void StartSpectatorMode()
	{
		Con._DEV_FREECAM = false;
		if (SPECTATOR_MODE)
		{
			return;
		}
		SPECTATOR_MODE = true;
		((Component)PlayerCamera.main.endScreen).gameObject.SetActive(false);
		if ((Object)(object)NetPlayer.LOCAL_PLAYER != (Object)null && (Object)(object)NetPlayer.LOCAL_PLAYER.body != (Object)null)
		{
			NetPlayer lOCAL_PLAYER = NetPlayer.LOCAL_PLAYER;
			curspectatefolowingname = lOCAL_PLAYER.playername;
			curspectatefolowing = LinqUtility.AsReadOnlyList<NetPlayer>((IEnumerable<NetPlayer>)NetPlayer.BodyToPlayerDict.Values).IndexOf(lOCAL_PLAYER);
			if ((Object)(object)PlayerCamera.main.following == (Object)null)
			{
				Spectator_SetTarget((Component)(object)lOCAL_PLAYER.body);
			}
		}
		else
		{
			curspectatefolowingname = "NOBODY";
			curspectatefolowing = 0;
		}
	}

	public static void StopSpectatorMode()
	{
		if (Util.IsInMainMenu())
		{
			Con._DEV_FREECAM = false;
			SPECTATOR_MODE = false;
			return;
		}
		Con._DEV_FREECAM = false;
		if (SPECTATOR_MODE && !KrokoshaScavMultiplayer.is_dedicated_server)
		{
			SPECTATOR_MODE = false;
			if (PlayerCamera.main.didDeathScreen)
			{
				((Component)PlayerCamera.main.endScreen).gameObject.SetActive(true);
			}
			PlayerCamera.main.following = null;
			MoodleManager.main.SetBody(Util.GetLocalBody());
		}
	}

	public static void Spectator_SetTarget(Component obj, string custom_name = null)
	{
		if (!SPECTATOR_MODE)
		{
			return;
		}
		NetBody netBody = default(NetBody);
		if (custom_name != null)
		{
			curspectatefolowingname = custom_name;
		}
		else if (obj.TryGetComponent<NetBody>(ref netBody))
		{
			curspectatefolowingname = netBody.bodyname;
		}
		else
		{
			curspectatefolowingname = ((Object)obj).name;
		}
		Body b = default(Body);
		if (obj.TryGetComponent<Body>(ref b))
		{
			MoodleManager.main.SetBody(b);
		}
		PlayerCamera.main.following = obj.transform;
		_last_spectator_plr_transform = obj.transform;
		if (obj.TryGetComponent<NetBody>(ref netBody))
		{
			if (((Component)WoundView.view).gameObject.activeSelf)
			{
				PlayerCamera_ToggleWoundView_MultiplayerPatch.WoundView_SetTargetBody(netBody.body);
				WoundView.view.UpdateView();
			}
			if (netBody.is_player)
			{
				curspectatefolowing = LinqUtility.AsReadOnlyList<NetPlayer>((IEnumerable<NetPlayer>)NetPlayer.BodyToPlayerDict.Values).IndexOf(netBody.plr);
			}
		}
	}

	public static bool DoPlayerInteractionMenuButton(in Rect rect, bool enabled, in string text, ref bool focused, bool continue_drag_item = false, bool accept_drag_item = false)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected O, but got Unknown
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		GUIStyle style = new GUIStyle(GUI.skin.button);
		GUI.enabled = enabled;
		bool result = UIBullshit._GUI_FocusableButton(in rect, in text, out focused, style) && enabled;
		if (focused)
		{
			if (enabled && (Object)(object)PlayerCamera.main.dragItem != (Object)null && !Util.IsInWoundView())
			{
				if (continue_drag_item)
				{
					return true;
				}
				if (accept_drag_item && Input.GetKeyUp(KeyBinds.GetBind("iteminteract")))
				{
					return true;
				}
			}
			interaction_menu_focused = true;
		}
		return result;
	}

	private static void _GUI_PlayerInteractionGui()
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0753: Unknown result type (might be due to invalid IL or missing references)
		//IL_0758: Unknown result type (might be due to invalid IL or missing references)
		//IL_0764: Unknown result type (might be due to invalid IL or missing references)
		//IL_0769: Unknown result type (might be due to invalid IL or missing references)
		//IL_066e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0673: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_055a: Unknown result type (might be due to invalid IL or missing references)
		//IL_055f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0821: Unknown result type (might be due to invalid IL or missing references)
		//IL_0839: Unknown result type (might be due to invalid IL or missing references)
		//IL_084a: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f8: Unknown result type (might be due to invalid IL or missing references)
		if (KrokoshaScavMultiplayer.is_dedicated_server)
		{
			return;
		}
		float uiScale = UIBullshit.uiScale;
		float buttonxsize = 100f * uiScale;
		float buttonysize = 45f * uiScale;
		float num = buttonxsize * 3f;
		float num2 = buttonysize * 2.5f;
		NetPlayer lOCAL_PLAYER = NetPlayer.LOCAL_PLAYER;
		new Vector2(Input.mousePosition.x, (float)Screen.height - Input.mousePosition.y);
		Vector2 val = Vector2.op_Implicit(((Component)interaction_menu_target_body.body).transform.position);
		val += Vector2.up * 6f;
		Vector2 val2 = Vector2.op_Implicit(Camera.main.WorldToScreenPoint(Vector2.op_Implicit(val)));
		((Vector2)(ref val2))._002Ector(val2.x, (float)Screen.height - val2.y);
		if (interaction_menu_target_pos == Vector2.zero)
		{
			interaction_menu_target_pos = val2;
		}
		interaction_menu_target_pos = new Vector2(Mathf.Clamp(interaction_menu_target_pos.x, num, (float)Screen.width - num), Mathf.Clamp(interaction_menu_target_pos.y, num2, (float)Screen.height - num2));
		Vector2 pos = interaction_menu_target_pos;
		int fontSize = GUI.skin.button.fontSize;
		float num3 = UIBullshit.DEFAULT_RETROGAME_FONTSIZE * 0.6f * UIBullshit.uiScale * UIBullshit.CUSTOM_UI_SCALE_FOR_UIBULLSHIT;
		GUI.skin.button.fontSize = Mathf.RoundToInt(num3);
		GUI.skin.label.fontSize = GUI.skin.button.fontSize;
		float curbuttonx = 0f;
		float curbuttony = 0f;
		bool focused = false;
		string key = null;
		if (DoPlayerInteractionMenuButton(GetNextRect(), enabled: true, Lang.Get("plrint_woundview", false), ref focused, continue_drag_item: true))
		{
			if (Util.QuickRaycastInteractionCheck(Vector2.op_Implicit(((Component)lOCAL_PLAYER.body).transform.position), Vector2.op_Implicit(((Component)interaction_menu_target_body.body).transform.position), do_effect: true))
			{
				PlayerCamera_ToggleWoundView_MultiplayerPatch.OpenSpecificBody(interaction_menu_target_body.body);
			}
			else
			{
				PlayerCamera.main.DoAlert(Lang.Get("interact_obstructed", false), false);
			}
			StopPlayerInteractionMenu();
			return;
		}
		if (focused)
		{
			key = "plrint_woundview_tooltip";
		}
		curbuttonx += 1f;
		bool flag = KrokoshaScavMultiplayer.rules.NoInventoryLock || !interaction_menu_target_body.body.conscious;
		if (DoPlayerInteractionMenuButton(GetNextRect(), flag, Lang.Get("plrint_inventory", false), ref focused))
		{
			ClientMain._PLRINT_Inventory(interaction_menu_target_body);
			StopPlayerInteractionMenu();
			return;
		}
		if (focused)
		{
			key = (flag ? "plrint_inv_allowed" : "plrint_inv_no");
		}
		curbuttonx += 1f;
		Item dragItem = PlayerCamera.main.dragItem;
		bool flag2 = (Object)(object)dragItem != (Object)null && (dragItem.Stats.usable || dragItem.Stats.wearable) && ClientMain.ItemCanBeUsedOnThisBody(dragItem, interaction_menu_target_body.body) && interaction_menu_target_body.body.alive;
		if (DoPlayerInteractionMenuButton(GetNextRect(), flag2, (!flag2) ? Lang.Get("plrint_useitem", false) : (dragItem.Stats.wearable ? Lang.Get("plrint_useitem_wearable", false) : ((dragItem.Stats.category == "food") ? Lang.Get("plrint_useitem_food", false) : ((dragItem.Stats.category == "water") ? Lang.Get("plrint_useitem_water", false) : Lang.Get("plrint_useitem", false)))), ref focused, continue_drag_item: false, accept_drag_item: true))
		{
			Body body = interaction_menu_target_body.body;
			if (!MedicalSync.IsRefusingHelp(body))
			{
				if (ItemSync.TryGetSyncInfo(dragItem, out var si))
				{
					si.SetIgnoreTimeForRoundTrip();
					KrokoshaScavMultiplayer.Client_SendSimpleMessageToServer((ushort)10105, (ushort)si.syncId, (ushort)interaction_menu_target_body.netId, true);
				}
				ItemSync.BetterUseItem(body, dragItem);
			}
			else
			{
				PlayerCamera.main.DoAlert(Lang.Get("deny_refuse", false), false);
			}
			PlayerCamera.main.dragItem = null;
			StopPlayerInteractionMenu();
			return;
		}
		if (focused)
		{
			key = ((!interaction_menu_target_body.body.alive) ? "plr_dead" : (flag2 ? "plrint_feed_item" : "plrint_drag_item_here"));
		}
		curbuttonx += 1f;
		curbuttonx = 0f;
		curbuttony += 1f;
		bool flag3 = (Object)(object)interaction_menu_target_body.piggybacking_on == (Object)(object)lOCAL_PLAYER.playerbody || (Object)(object)interaction_menu_target_body.carrying_person == (Object)(object)lOCAL_PLAYER.playerbody;
		if (KrokoshaScavMultiplayer.rules.AllowPiggyback)
		{
			bool flag4 = interaction_menu_target_body.CanBeCarriedBySomeone() && !flag3;
			if (DoPlayerInteractionMenuButton(GetNextRect(), flag4, Lang.Get("plrint_carry", false), ref focused))
			{
				ClientMain._PLRINT_Carry(interaction_menu_target_body);
				StopPlayerInteractionMenu();
				return;
			}
			if (focused)
			{
				key = (flag4 ? "plrint_carry_allow" : "plrint_carry_no");
			}
			curbuttonx += 1f;
			bool flag5 = interaction_menu_target_body.IsPiggybackable() && (double)interaction_menu_target_body.body.overEncumberance < 0.5 && !flag3;
			if (DoPlayerInteractionMenuButton(GetNextRect(), flag5, Lang.Get("plrint_piggyback", false), ref focused))
			{
				ClientMain._PLRINT_Piggyback(interaction_menu_target_body);
				StopPlayerInteractionMenu();
				return;
			}
			if (focused)
			{
				key = (flag5 ? "plrint_piggyback_allow" : "plrint_piggyback_no");
			}
			curbuttonx += 1f;
		}
		if (KrokoshaScavMultiplayer.rules.AllowPush)
		{
			bool enabled = !flag3;
			if (DoPlayerInteractionMenuButton(GetNextRect(), enabled, Lang.Get("plrint_push", false), ref focused))
			{
				lOCAL_PLAYER.playerbody.Push(interaction_menu_target_body);
				StopPlayerInteractionMenu();
				return;
			}
			if (focused)
			{
				key = "plrint_push_tooltip";
			}
			curbuttonx += 1f;
		}
		GUI.enabled = true;
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(pos.x - buttonxsize * 1.5f, pos.y - buttonysize * 1.5f, buttonxsize * 3f, buttonysize * 0.5f);
		UIBullshit._GUI_9SlicePanel(in rect, 1f, 0.8f, check_overlap: true, UIBullshit.unscaled_uiBlockNano);
		Color textColor = GUI.skin.label.normal.textColor;
		TextAnchor alignment = GUI.skin.label.alignment;
		GUI.skin.label.alignment = (TextAnchor)4;
		string text;
		if (interaction_menu_target_body.is_player)
		{
			NetPlayer plr = interaction_menu_target_body.plr;
			GUI.skin.label.normal.textColor = plr.plrcolor;
			text = interaction_menu_target_body.playername + "  ID:" + plr.clientId.ToString();
		}
		else
		{
			text = interaction_menu_target_body.bodyname + "  ID:" + interaction_menu_target_body.netId.ToString();
			if (log.verbose)
			{
				text += " (NPC)";
			}
		}
		GUI.Label(rect, text);
		GUI.skin.label.normal.textColor = textColor;
		GUI.skin.label.alignment = alignment;
		if (key != null)
		{
			UIBullshit._GUI_SetTooltip(Lang.Get(in key, false), (string)null);
		}
		GUI.skin.button.fontSize = fontSize;
		GUI.skin.label.fontSize = fontSize;
		Rect GetNextRect()
		{
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			return new Rect(pos.x + buttonxsize * (-1.5f + curbuttonx), pos.y + buttonysize * (-1f + curbuttony), buttonxsize, buttonysize);
		}
	}

	private static void _GUI_DoMPUserInterface()
	{
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		if (KrokoshaScavMultiplayer.rules.ShowPlayerDirections)
		{
			_GUI_DoPlayerPointersGui();
		}
		if (Con._DEV_HIDEHUD || Con.IsConsoleOpen())
		{
			Util.OpenBrightnessPanel(open_or_nah: false);
			StopPlayerInteractionMenu();
			return;
		}
		if ((Object)(object)interaction_menu_target_body != (Object)null)
		{
			_GUI_PlayerInteractionGui();
		}
		else
		{
			interaction_menu_target_body = null;
			interaction_menu_focused = false;
		}
		if (!Con._DEV_FREECAM)
		{
			if (SPECTATOR_MODE)
			{
				_GUI_DoSpectatorGui();
				return;
			}
			if (_GUI_DoGeneralMPGui())
			{
				return;
			}
		}
		if (Util.IsInPauseMenuOrMainMenu())
		{
			return;
		}
		float uiScale = UIBullshit.uiScale;
		float num = 240f * uiScale;
		float num2 = 80f * uiScale;
		if (Con._DEV_FREECAM)
		{
			if (!GUI.Button(new Rect((float)Screen.width - num, (float)Screen.height - 80f * uiScale, num, num2), Lang.Get("spectate_freecamexit", false)))
			{
				return;
			}
			Util.PlayUISound((UISoundType)2);
			Con._DEV_FREECAM = false;
			if (SPECTATOR_MODE)
			{
				(NetBody, float) nearestBody = NetBody.GetNearestBody(Vector2.op_Implicit(((Component)PlayerCamera.main).transform.position), spectator_only_alive);
				if ((Object)(object)nearestBody.Item1 != (Object)null)
				{
					Spectator_SetTarget((Component)(object)nearestBody.Item1);
				}
			}
		}
		else if (GUI.Button(new Rect((float)Screen.width - num, (float)Screen.height - 80f * uiScale, num, num2), Lang.Get("spectate", false)))
		{
			Util.PlayUISound((UISoundType)0);
			StartSpectatorMode();
		}
	}

	internal void _GUI_DoInGameUI()
	{
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Expected O, but got Unknown
		interaction_menu_focused = false;
		if (KrokoshaScavMultiplayer.network_system_is_running && Util.IsInWorld())
		{
			RectOffset padding = GUI.skin.button.padding;
			TextClipping clipping = GUI.skin.button.clipping;
			try
			{
				GUI.skin.button.padding = new RectOffset(0, 0, 0, 0);
				GUI.skin.button.clipping = (TextClipping)0;
				UIBullshit._GUI_DoTexturesForMPUserInterface();
				_GUI_DoMPUserInterface();
			}
			catch (Exception ex)
			{
				log.error("ClientMain.OnGUI -> " + ex.ToString());
			}
			GUI.skin.button.padding = padding;
			GUI.skin.button.clipping = clipping;
		}
	}

	private void UI_Update_Antidispersion()
	{
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		NetPlayer lOCAL_PLAYER = NetPlayer.LOCAL_PLAYER;
		if (!SharedMain.CheckIfDispersionPunishmentProtocolRuleIsActive())
		{
			return;
		}
		if ((Object)(object)lOCAL_PLAYER != (Object)null && lOCAL_PLAYER.IsAlive() && lOCAL_PLAYER.levelPlayTime > 3.0)
		{
			antidispersion_active = SharedMain.CheckIfShouldActivateDispersionPunishmentProtocol(lOCAL_PLAYER, out antidispersion_cur_distance, out var count_plrs_in_range, out var required_count_plrs_in_range);
			antidispersion_cur_range_count = count_plrs_in_range;
			antidispersion_cur_required_count = required_count_plrs_in_range;
			if (antidispersion_warned)
			{
				if (antidispersion_active)
				{
					PlayerCamera_SetIrradiateIntensity_MultiplayerPatch.forceSetIrradiateIntensity = true;
					PlayerCamera_SetIrradiateIntensity_MultiplayerPatch.RealSetIrradiateIntensity(1f);
				}
				else if (antidispersion_cur_distance < 10f)
				{
					antidispersion_warned = false;
				}
			}
			else if (antidispersion_active)
			{
				antidispersion_warned = true;
				Util.DoAlert(Lang.Get("antidispersion_activated_alert", false), false);
				lOCAL_PLAYER.body.talker.Talk(Locale.GetCharacter("loud"), (Limb)null, false, false);
				Sound.Play("beep", lOCAL_PLAYER.pos, true, false, ((Component)lOCAL_PLAYER.body).transform, 0.5f, 2f, true, true);
				PlayerCamera.main.shaker.Shake(5f);
			}
		}
		else
		{
			antidispersion_active = false;
			antidispersion_cur_distance = 0f;
		}
	}

	private bool _UpdateCheckInteractKeybinds_CheckDistNShi(NetBody targetpb)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		NetPlayer lOCAL_PLAYER = NetPlayer.LOCAL_PLAYER;
		_ = targetpb.body;
		if (Util.QuickRaycastInteractionCheck(Vector2.op_Implicit(((Component)lOCAL_PLAYER.body).transform.position), Vector2.op_Implicit(((Component)targetpb).transform.position), do_effect: true))
		{
			Component ba = (Component)(object)lOCAL_PLAYER.body;
			Component bb = (Component)(object)targetpb;
			if (KM.dist2dsqrcheck(in ba, in bb, SharedMain.max_player_interaction_distance))
			{
				StopPlayerInteractionMenu();
				return true;
			}
			PlayerCamera.main.DoAlert(Lang.Get("plr_too_far", false), false);
		}
		else
		{
			PlayerCamera.main.DoAlert(Lang.Get("interact_obstructed", false), false);
		}
		return false;
	}

	private void _UpdateCheckInteractKeybinds(NetBody targetpb)
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		NetPlayer lOCAL_PLAYER = NetPlayer.LOCAL_PLAYER;
		_ = targetpb.body;
		lOCAL_PLAYER.TryGetNetBody(out var pb);
		bool flag = (Object)(object)targetpb.piggybacking_on == (Object)(object)pb || (Object)(object)targetpb.carrying_person == (Object)(object)pb;
		if (KrokoshaScavMultiplayer.rules.AllowPush && Input.GetKeyDown(KeyBinds.GetBind("krokosha_coop_push")))
		{
			if (_UpdateCheckInteractKeybinds_CheckDistNShi(targetpb))
			{
				lOCAL_PLAYER.playerbody.Push(targetpb);
			}
		}
		else if (Input.GetKeyDown(KeyBinds.GetBind("krokosha_coop_woundview")))
		{
			if (_UpdateCheckInteractKeybinds_CheckDistNShi(targetpb))
			{
				Util.DoAlert(string.Format(Lang.Get("plrint_req_woundview", false), targetpb.bodyname), false);
				bool num = PlayerCamera_ToggleWoundView_MultiplayerPatch.IsOpen();
				PlayerCamera_ToggleWoundView_MultiplayerPatch.OpenSpecificBody(targetpb.body);
				if (!num && CoopKeybinds.IsWoundViewSameAsOG())
				{
					PlayerCamera_ToggleWoundView_MultiplayerPatch.IgnoreNext = true;
				}
			}
		}
		else if (Input.GetKeyDown(KeyBinds.GetBind("krokosha_coop_inventory")))
		{
			if (_UpdateCheckInteractKeybinds_CheckDistNShi(targetpb))
			{
				ClientMain._PLRINT_Inventory(targetpb);
			}
		}
		else
		{
			if (flag)
			{
				return;
			}
			if (Input.GetKeyDown(KeyBinds.GetBind("krokosha_coop_piggyback")))
			{
				if (_UpdateCheckInteractKeybinds_CheckDistNShi(targetpb))
				{
					ClientMain._PLRINT_Piggyback(targetpb);
				}
			}
			else
			{
				if (!Input.GetKeyDown(KeyBinds.GetBind("krokosha_coop_carry")))
				{
					return;
				}
				if (targetpb.CanBeCarriedBySomeone())
				{
					if (_UpdateCheckInteractKeybinds_CheckDistNShi(targetpb))
					{
						ClientMain._PLRINT_Carry(targetpb);
					}
				}
				else
				{
					Util.DoAlert(Lang.Get("alert_cantcarry", false), false);
				}
			}
		}
	}

	private void OGInGameIHandlingBullshit()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		Vector2 cursorWorldPos = Util.GetCursorWorldPos();
		if (Util.TryGetLocalBody(out var body))
		{
			if (ConsoleScript.instance.fullBright && !Con.CanCheat())
			{
				ConsoleScript.instance.fullBright = false;
			}
			if (Con._DEV_FREECAM && !Con.CanFreecam())
			{
				Con._DEV_FREECAM = false;
			}
			if (Con._DEV_FREECAM)
			{
				PlayerCamera_HandleScreenShaders_MultiplayerPatch.forceDefaultShaderForNextFrame = true;
			}
			else
			{
				if (ConsoleScript.instance.noClip)
				{
					if (!body.standing)
					{
						body.shock = 0f;
						body.Stand(true);
					}
					body.rb.velocity = Vector2.zero;
					if (!Con.CanCheat())
					{
						Con.UnNoclip();
					}
				}
				_DEV_FREECAM_CURPOS = Vector2.op_Implicit(((Component)PlayerCamera.main.body).transform.position);
			}
			if (!Con.CanCheat())
			{
				if (ConsoleScript.instance.fullBright)
				{
					ConsoleScript.instance.fullBright = false;
				}
			}
			else if (Con._DEV_GODMODE)
			{
				body.ResetHealth();
				if (!KrokoshaScavMultiplayer.is_client)
				{
					foreach (NetPlayer value in NetPlayer.ClientIdToPlayerDict.Values)
					{
						if ((Object)(object)value.body != (Object)null)
						{
							value.Server_HealCharacter();
						}
					}
				}
			}
		}
		NetPlayer lOCAL_PLAYER = NetPlayer.LOCAL_PLAYER;
		if (!KrokoshaScavMultiplayer.network_system_is_running || !((Object)(object)lOCAL_PLAYER != (Object)null))
		{
			return;
		}
		lOCAL_PLAYER.camerapos = Vector2.op_Implicit(((Component)Camera.main).transform.position);
		if (!Util.IsWorldGenerated())
		{
			return;
		}
		if (!Con.IsConsoleOpen() && Input.GetKeyDown(KeyBinds.GetBind("krokosha_coop_pointfingerat")))
		{
			Vector2 val = Vector2.op_Implicit(Camera.main.ScreenToWorldPoint(Input.mousePosition));
			byte b = ((Time.timeAsDouble - lOCAL_PLAYER.is_pointingfingeratTIME < 0.20000000298023224) ? ((byte)1) : ((byte)0));
			lOCAL_PLAYER.PointFingerAt(val, b);
			NetDataWriter writer = Net.CreateWriter(10036);
			writer.Put(val);
			writer.Put(b);
			Net.Client_Send((DeliveryMethod)2, in writer);
		}
		body = lOCAL_PLAYER.body;
		if ((Object)(object)body != (Object)null)
		{
			Body bodyOnPos = Util.GetBodyOnPos(cursorWorldPos);
			NetBody targetpb = default(NetBody);
			if ((Object)(object)bodyOnPos != (Object)null && !Con.IsConsoleOpen() && !PlayerCamera.main.woundView.activeSelf && lOCAL_PLAYER.IsConscious() && !bodyOnPos.IsBodyLocal() && ((Component)bodyOnPos).TryGetComponent<NetBody>(ref targetpb))
			{
				_UpdateCheckInteractKeybinds(targetpb);
			}
		}
	}

	private void Awake()
	{
		main = this;
	}

	private void Update()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		OGInGameIHandlingBullshit();
		if (!KrokoshaScavMultiplayer.IsNetworkActiveAndIsWorldGenerated())
		{
			Object.Destroy((Object)(object)this);
			return;
		}
		if (SPECTATOR_MODE && Input.GetMouseButtonDown(0))
		{
			Body bodyOnPos = Util.GetBodyOnPos(Util.GetCursorWorldPos());
			NetBody obj = default(NetBody);
			if ((Object)(object)bodyOnPos != (Object)null && !Con.IsConsoleOpen() && !PlayerCamera.main.woundView.activeSelf && !UIUtil.IsPointerOverUIElement() && ((Component)bodyOnPos).TryGetComponent<NetBody>(ref obj))
			{
				Spectator_SetTarget((Component)(object)obj);
			}
		}
		if (!((Object)(object)NetPlayer.LOCAL_PLAYER != (Object)null))
		{
			return;
		}
		if (Util.TryGetLocalBody(out var _) && (Object)(object)interaction_menu_target_body != (Object)null)
		{
			NetPlayer lOCAL_PLAYER = NetPlayer.LOCAL_PLAYER;
			if ((Object)(object)interaction_menu_target_body.body == (Object)null || (Object)(object)lOCAL_PLAYER.body == (Object)null || !lOCAL_PLAYER.body.conscious || !KM.dist2dsqrcheck(Vector2.op_Implicit(((Component)PlayerCamera.main.body).transform.position), Vector2.op_Implicit(((Component)interaction_menu_target_body.body).transform.position), SharedMain.max_player_interaction_distance) || Util.IsInWoundView())
			{
				StopPlayerInteractionMenu();
			}
		}
		UI_Update_Antidispersion();
	}

	public static void StartPlayerInteractionMenu(NetBody target)
	{
		if ((Object)(object)target != (Object)(object)interaction_menu_target_body && (Object)(object)target.body != (Object)null && (Object)(object)PlayerCamera.main.body != (Object)null && PlayerCamera.main.body.conscious && !SPECTATOR_MODE)
		{
			Component ba = (Component)(object)PlayerCamera.main.body;
			if (!KM.dist2dsqrcheck(in ba, (Component)(object)target.body, SharedMain.max_player_interaction_distance))
			{
				PlayerCamera.main.DoAlert(Lang.Get("plr_too_far", false), false);
				return;
			}
			if (!Util.AccurateRaycastInteractionCheckObstruction(PlayerCamera.main.body, target.body, do_effect: true))
			{
				PlayerCamera.main.DoAlert(Lang.Get("interact_obstructed", false), false);
				return;
			}
			PlayerCamera.main.PlayUISound((UISoundType)0, 1f);
			interaction_menu_target_body = target;
		}
	}

	public static void StopPlayerInteractionMenu()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		interaction_menu_target_pos = Vector2.zero;
		if ((Object)(object)interaction_menu_target_body != (Object)null)
		{
			interaction_menu_target_body = null;
			PlayerCamera.main.PlayUISound((UISoundType)2, 1f);
		}
		interaction_menu_focused = false;
	}
}
