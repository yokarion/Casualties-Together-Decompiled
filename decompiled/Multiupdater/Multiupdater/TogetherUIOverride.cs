using System;
using CasualtiesTogetherUtils;
using Together;
using UnityEngine;

namespace Multiupdater;

internal static class TogetherUIOverride
{
	private static bool _CHECKFORUPDATEBUTTON_IS_RESTART;

	private static bool _CHECKFORUPDATEBUTTON_OnStageSuccess_isBound;

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
