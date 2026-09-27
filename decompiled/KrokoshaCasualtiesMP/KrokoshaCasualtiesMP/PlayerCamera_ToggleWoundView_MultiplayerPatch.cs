using System;
using System.Runtime.CompilerServices;
using HarmonyLib;
using KrokoshaCasualtiesUtils;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(PlayerCamera), "ToggleWoundView")]
public static class PlayerCamera_ToggleWoundView_MultiplayerPatch
{
	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static UnityAction _003C_003E9__2_0;

		internal void _003CWoundView_SetTargetBody_003Eb__2_0()
		{
			if (CPRHandler.CheckIfBodyIsNotOkAndNeedCPR(WoundView.view.body) && CPRHandler.IsCPRBeingPerformedOnThisBody_IsMeOrNobody(WoundView.view.body))
			{
				if (MinigameBase.main.currentMinigame != null && MinigameBase.main.currentMinigame is ManualDefibMinigame)
				{
					MinigameBase.main.EndMinigame();
				}
				MinigameBase.main.StartMinigame((Minigame)(object)new CPRMinigame(WoundView.view.body), (Item)null);
			}
		}
	}

	public static bool IgnoreNext;

	public static GameObject CPRButton;

	public static Body body_to_open;

	public static void WoundView_SetTargetBody(Body b)
	{
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Expected O, but got Unknown
		WoundView view = WoundView.view;
		view.body = b;
		bool flag = !Util.IsBodyLocal(view.body);
		((Component)view.napbutton).gameObject.SetActive(!flag);
		Transform val = ((Component)view).transform.Find("WorkoutButton");
		if ((Object)(object)val != (Object)null)
		{
			if ((Object)(object)CPRButton == (Object)null)
			{
				try
				{
					CPRButton = Object.Instantiate<GameObject>(((Component)val).gameObject, ((Component)val).transform.parent, false);
					CPRButton.SetActive(true);
					((Object)CPRButton).name = "BUTTON_MP_CPR";
					RectTransform component = CPRButton.GetComponent<RectTransform>();
					UITooltip component2 = CPRButton.GetComponent<UITooltip>();
					component2.skipLocale = true;
					component2.tipName = Lang.Get("ww_cpr", false);
					component2.tipDesc = Lang.Get("ww_cprdesc", false);
					((Transform)component).SetSiblingIndex(val.GetSiblingIndex());
					CPRButton.GetComponent<Image>().sprite = CoopModAssets.cpr_button_icon;
					Button component3 = CPRButton.GetComponent<Button>();
					((UnityEventBase)component3.onClick).RemoveAllListeners();
					((UnityEventBase)component3.onClick).SetPersistentListenerState(0, (UnityEventCallState)0);
					ButtonClickedEvent onClick = component3.onClick;
					object obj = _003C_003Ec._003C_003E9__2_0;
					if (obj == null)
					{
						UnityAction val2 = delegate
						{
							if (CPRHandler.CheckIfBodyIsNotOkAndNeedCPR(WoundView.view.body) && CPRHandler.IsCPRBeingPerformedOnThisBody_IsMeOrNobody(WoundView.view.body))
							{
								if (MinigameBase.main.currentMinigame != null && MinigameBase.main.currentMinigame is ManualDefibMinigame)
								{
									MinigameBase.main.EndMinigame();
								}
								MinigameBase.main.StartMinigame((Minigame)(object)new CPRMinigame(WoundView.view.body), (Item)null);
							}
						};
						_003C_003Ec._003C_003E9__2_0 = val2;
						obj = (object)val2;
					}
					((UnityEvent)onClick).AddListener((UnityAction)obj);
				}
				catch (Exception ex)
				{
					log.error("CPRButton creation: " + ex.ToString());
				}
			}
			((Component)val).gameObject.SetActive(!flag);
			((Selectable)((Component)val).GetComponent<Button>()).interactable = !flag;
		}
		else
		{
			log.error("Couldn't find the 'WorkoutButton' in 'WoundView' !!!!!!!! ");
		}
		if ((Object)(object)CPRButton != (Object)null)
		{
			CPRButton.SetActive(flag);
			((Selectable)CPRButton.GetComponent<Button>()).interactable = flag;
		}
		else
		{
			log.error("Couldn't find the patched 'CPRButton' !!!!!!!! ");
		}
		if (flag)
		{
			NetBody netBody = default(NetBody);
			if (((Component)b).TryGetComponent<NetBody>(ref netBody))
			{
				((TMP_Text)view.nameText).text = netBody.bodyname;
				if (netBody != null)
				{
					NetBody netBody2 = netBody;
					if (netBody2.is_player)
					{
						TextMeshProUGUI bodyStatText = view.bodyStatText;
						string text = Lang.Get("ww_peerid", false);
						knetid netId = netBody2.netId;
						((TMP_Text)bodyStatText).text = text + netId.ToString();
						goto IL_02b5;
					}
				}
				((TMP_Text)view.bodyStatText).text = "";
			}
			else
			{
				Plugin.log.LogError((object)$"Couldnt get NetBody of a body, what??? {view.body} ");
				((TMP_Text)view.nameText).text = "EXPERIMENT";
				((TMP_Text)view.bodyStatText).text = "ERROR";
			}
		}
		else
		{
			((TMP_Text)view.nameText).text = "EXPERIMENT";
			view.SetCharDetails(view.cInfo[0], view.cInfo[1], view.cInfo[2], view.cInfo[3]);
		}
		goto IL_02b5;
		IL_02b5:
		WoundView_AddImageToLimb_MultiplayerPatch.RefreshAllLimbImages();
	}

	public static bool IsOpen()
	{
		return ((Component)WoundView.view).gameObject.activeSelf;
	}

	public static void OpenSpecificBody(Body b, bool sound = true)
	{
		if (!IsOpen() || !((Object)(object)b == (Object)(object)WoundView.view.body))
		{
			body_to_open = b;
			PlayerCamera.main.ToggleWoundView(sound);
		}
	}

	public static bool Prefix(PlayerCamera __instance, bool sound)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (IgnoreNext)
		{
			IgnoreNext = false;
			return false;
		}
		if (!IsOpen())
		{
			if (Util.IsLocalBodyAlive() && CoopKeybinds.IsWoundViewSameAsOG() && Input.GetKeyDown(KeyBinds.GetBind("krokosha_coop_woundview")))
			{
				Body bodyOnPos = Util.GetBodyOnPos(Util.GetCursorWorldPos());
				if ((Object)(object)bodyOnPos != (Object)null && !bodyOnPos.IsBodyLocal())
				{
					Component ba = (Component)(object)Util.GetLocalBody();
					Component bb = (Component)(object)bodyOnPos;
					if (KM.dist2dsqrcheck(in ba, in bb, SharedMain.max_player_interaction_distance))
					{
						body_to_open = bodyOnPos;
					}
				}
			}
			Body b = default(Body);
			if ((Object)(object)body_to_open == (Object)null && UIInGame.SPECTATOR_MODE && (Object)(object)__instance.following != (Object)null && ((Component)__instance.following).TryGetComponent<Body>(ref b))
			{
				WoundView_SetTargetBody(b);
			}
			else
			{
				WoundView_SetTargetBody(body_to_open ?? PlayerCamera.main.body);
			}
		}
		body_to_open = null;
		if (UIInGame.SPECTATOR_MODE && !__instance.body.alive)
		{
			GameObject gameObject = ((Component)WoundView.view).gameObject;
			if (sound)
			{
				__instance.PlayUISound((UISoundType)(gameObject.activeSelf ? 2 : 3), 1f);
			}
			gameObject.SetActive(!gameObject.activeSelf);
			WoundView.view.UpdateView();
			WoundView.view.timeOpen = 0f;
			__instance.mainView.SetActive(!__instance.mainView.activeSelf);
			return false;
		}
		return true;
	}
}
