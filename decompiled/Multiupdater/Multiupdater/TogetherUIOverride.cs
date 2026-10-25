using System;
using CasualtiesTogetherUtils;
using TMPro;
using Together;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Multiupdater;

internal static class TogetherUIOverride
{
	private static bool _CHECKFORUPDATEBUTTON_IS_RESTART;

	private static bool _CHECKFORUPDATEBUTTON_OnStageSuccess_isBound;

	public static GameObject RealUpdateButton;

	public static TextMeshProUGUI RealUpdateStatusText;

	internal static void BuildUpdateButton()
	{
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Expected O, but got Unknown
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		RectTransform content = SettingsMenu_Start_MultiplayerPatch.content;
		string text = "mmst_button_checkupdate";
		bool flag = false;
		string text2 = Lang.Get(ref text, ref flag);
		GameObject g = default(GameObject);
		Button button = default(Button);
		TextMeshProUGUI val = default(TextMeshProUGUI);
		SettingsMenu_Start_MultiplayerPatch.CreateSettingButton(ref text2, ref SettingsMenu_Start_MultiplayerPatch.contentHeight, ref g, ref button, ref val);
		SettingsMenu_Start_MultiplayerPatch.UpdateButton = g;
		RealUpdateButton = g;
		if (_CHECKFORUPDATEBUTTON_IS_RESTART)
		{
			GameObject obj = g;
			text = "mmst_button_restart";
			flag = false;
			text2 = Lang.Get(ref text, ref flag);
			string text3 = "mmst_button_restart_desc";
			bool flag2 = false;
			string text4 = Lang.Get(ref text3, ref flag2);
			SettingsMenu_Start_MultiplayerPatch.SetSettingNameAndTooltip(obj, ref text2, ref text4);
		}
		else
		{
			GameObject obj2 = g;
			text = "mmst_button_checkupdate";
			flag = false;
			text2 = Lang.Get(ref text, ref flag);
			string text3 = "mmst_button_checkupdate_desc";
			bool flag2 = false;
			string text4 = Lang.Get(ref text3, ref flag2);
			SettingsMenu_Start_MultiplayerPatch.SetSettingNameAndTooltip(obj2, ref text2, ref text4);
		}
		((UnityEvent)button.onClick).AddListener((UnityAction)delegate
		{
			((Selectable)button).interactable = false;
			if (_CHECKFORUPDATEBUTTON_IS_RESTART)
			{
				Util.DelayCallLambda(0.01f, (Delegate)(Action)delegate
				{
					Plugin.RestartGame();
				});
			}
			else
			{
				if (!_CHECKFORUPDATEBUTTON_OnStageSuccess_isBound)
				{
					_CHECKFORUPDATEBUTTON_OnStageSuccess_isBound = true;
					AutoUpdater.OnStageSuccess = (Action)Delegate.Combine(AutoUpdater.OnStageSuccess, (Action)delegate
					{
						AutoUpdater.LogMessage("Update staging succesful.");
						if (Plugin.IsExeSameAsInSteam())
						{
							_CHECKFORUPDATEBUTTON_IS_RESTART = true;
							((Selectable)button).interactable = true;
							GameObject obj5 = g;
							string text9 = "mmst_button_restart";
							bool flag5 = false;
							string text10 = Lang.Get(ref text9, ref flag5);
							string text11 = "mmst_button_restart_desc";
							bool flag6 = false;
							string text12 = Lang.Get(ref text11, ref flag6);
							SettingsMenu_Start_MultiplayerPatch.SetSettingNameAndTooltip(obj5, ref text10, ref text12);
						}
					});
				}
				AutoUpdater.StartChecking();
				Util.PlayUISound((UISoundType)1, 1f, 1f);
				GameObject obj4 = g;
				string text5 = "mmst_button_checkupdate";
				bool flag3 = false;
				string text6 = Lang.Get(ref text5, ref flag3);
				string text7 = "mmst_button_checkupdate_desc";
				bool flag4 = false;
				string text8 = Lang.Get(ref text7, ref flag4);
				SettingsMenu_Start_MultiplayerPatch.SetSettingNameAndTooltip(obj4, ref text6, ref text8);
			}
		});
		RectTransform component = g.GetComponent<RectTransform>();
		component.anchorMax -= Vector2.right * 0.5f;
		component.offsetMin += Vector2.one * 10f;
		component.offsetMax -= Vector2.one * 10f;
		GameObject obj3 = Utils.Create("Special/GameSettingDropdown", (Transform)(object)content);
		TMP_Dropdown component2 = ((Component)obj3.transform.GetChild(1)).GetComponent<TMP_Dropdown>();
		if ((Object)(object)component2 != (Object)null)
		{
			Object.Destroy((Object)(object)((Component)component2).gameObject);
		}
		RectTransform component3 = obj3.GetComponent<RectTransform>();
		component3.sizeDelta += component3.sizeDelta * Vector2.up * 1f;
		RealUpdateStatusText = obj3.GetComponentInChildren<TextMeshProUGUI>();
		((TMP_Text)RealUpdateStatusText).verticalAlignment = (VerticalAlignmentOptions)256;
		((TMP_Text)RealUpdateStatusText).alignment = (TextAlignmentOptions)257;
		TextMeshProUGUI realUpdateStatusText = RealUpdateStatusText;
		((Graphic)realUpdateStatusText).color = ((Graphic)realUpdateStatusText).color * 0.8f;
		SettingsMenu_Start_MultiplayerPatch.UpdateStatusText = RealUpdateStatusText;
		text = AutoUpdater.Status;
		text2 = null;
		SettingsMenu_Start_MultiplayerPatch.SetupSetting(obj3, ref text, ref text2, ref SettingsMenu_Start_MultiplayerPatch.contentHeight);
	}

	internal static void DrawUpdateButton()
	{
		string status = AutoUpdater.Status;
		GUI.skin.label.alignment = (TextAnchor)0;
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		if (_CHECKFORUPDATEBUTTON_IS_RESTART)
		{
			string text = "mmst_button_restart";
			bool flag = false;
			if (GUILayout.Button(Lang.Get(ref text, ref flag), Array.Empty<GUILayoutOption>()))
			{
				Util.DelayCallLambda(0.01f, (Delegate)(Action)delegate
				{
					Plugin.RestartGame();
				});
			}
			text = "mmst_button_restart_desc";
			UIMainMenu.DoTooltipOnLastGUIRect(ref text);
		}
		else
		{
			string text = "mmst_button_checkupdate";
			bool flag = false;
			if (GUILayout.Button(Lang.Get(ref text, ref flag), Array.Empty<GUILayoutOption>()))
			{
				if (!_CHECKFORUPDATEBUTTON_OnStageSuccess_isBound)
				{
					_CHECKFORUPDATEBUTTON_OnStageSuccess_isBound = true;
					AutoUpdater.OnStageSuccess = (Action)Delegate.Combine(AutoUpdater.OnStageSuccess, (Action)delegate
					{
						AutoUpdater.LogMessage("Update staging succesful.");
						if (Plugin.IsExeSameAsInSteam())
						{
							_CHECKFORUPDATEBUTTON_IS_RESTART = true;
						}
					});
				}
				AutoUpdater.StartChecking();
				Util.PlayUISound((UISoundType)1, 1f, 1f);
			}
			text = "mmst_button_checkupdate_desc";
			UIMainMenu.DoTooltipOnLastGUIRect(ref text);
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		bool wordWrap = GUI.skin.label.wordWrap;
		GUI.skin.label.wordWrap = true;
		GUILayout.Label(status, Array.Empty<GUILayoutOption>());
		GUI.skin.label.wordWrap = wordWrap;
		GUILayout.EndHorizontal();
	}
}
