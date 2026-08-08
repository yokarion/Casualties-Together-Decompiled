using System;
using System.Linq;
using HarmonyLib;
using KrokoshaCasualtiesUtils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public static class GUILayout_DropdownMenu
{
	private static int activeID = -1;

	private static Rect activeRect;

	private static string[] activeOptions;

	private static int clickedID = -1;

	private static int clickedNumToReturn = 0;

	private static int id_counter = 0;

	private static float dropdownitemheight = 35f;

	public static bool isOpen => activeID != -1;

	public static void CloseAll()
	{
		clickedID = -1;
		activeID = -1;
	}

	public static int Dropdown(int selectedIndex, string[] options, params GUILayoutOption[] uILayoutOptions)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected O, but got Unknown
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		id_counter++;
		if (selectedIndex > 0 && selectedIndex >= options.Length)
		{
			selectedIndex = options.Length - 1;
		}
		float num = 0f;
		GUIContent val;
		if (options.Length == 0)
		{
			selectedIndex = 0;
			val = new GUIContent("[NONE]");
		}
		else
		{
			val = new GUIContent(options[selectedIndex]);
			foreach (string text in options)
			{
				Vector2 val2 = GUI.skin.button.CalcSize(new GUIContent(text));
				num = Mathf.Max(num, val2.x);
			}
		}
		Vector2 val3 = GUI.skin.button.CalcSize(val);
		num = Mathf.Max(num, val3.x);
		if (uILayoutOptions.Length == 0)
		{
			uILayoutOptions = CollectionExtensions.AddToArray<GUILayoutOption>(uILayoutOptions, GUILayout.MinWidth(num));
		}
		Rect rect = GUILayoutUtility.GetRect(val, GUI.skin.button, uILayoutOptions);
		int result = selectedIndex;
		bool flag = clickedID == id_counter;
		if (flag || activeID == id_counter)
		{
			if (flag)
			{
				clickedID = -1;
				result = clickedNumToReturn;
			}
			GUI.color = new Color(0f, 0f, 0f, 0f);
			bool enabled = GUI.enabled;
			GUI.enabled = false;
			GUI.Button(rect, val);
			GUI.enabled = enabled;
			GUI.color = Color.white;
			return result;
		}
		if (GUI.Button(rect, val))
		{
			clickedID = -1;
			if (activeID == id_counter)
			{
				activeID = -1;
			}
			else
			{
				activeID = id_counter;
				activeRect = GUIUtility.GUIToScreenRect(rect);
				activeOptions = options;
			}
			Util.PlayUISound((UISoundType)1);
		}
		return result;
	}

	public static void Draw()
	{
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		id_counter = 0;
		if (!isOpen || activeOptions == null || activeOptions.Count() == 0)
		{
			return;
		}
		try
		{
			UIBullshit._GUI_SetButtonSkinTexture(GUI.skin.button, small: false);
			float num = dropdownitemheight * UIBullshit.uiScale;
			Rect val = default(Rect);
			((Rect)(ref val))._002Ector(((Rect)(ref activeRect)).x, ((Rect)(ref activeRect)).y, ((Rect)(ref activeRect)).width, (float)activeOptions.Length * num);
			for (int i = 0; i < activeOptions.Length; i++)
			{
				if (GUI.Button(new Rect(((Rect)(ref val)).x, ((Rect)(ref val)).y + (float)i * num, ((Rect)(ref val)).width, num), activeOptions[i]))
				{
					clickedID = activeID;
					clickedNumToReturn = i;
					activeID = -1;
					Util.PlayUISound((UISoundType)1);
				}
			}
			if ((int)Event.current.type == 0 && !((Rect)(ref val)).Contains(Event.current.mousePosition) && !((Rect)(ref activeRect)).Contains(Event.current.mousePosition))
			{
				activeID = -1;
			}
		}
		catch (Exception ex)
		{
			log.error(ex.ToString());
		}
		UIBullshit._GUI_SetButtonSkinTexture(GUI.skin.button);
	}
}
