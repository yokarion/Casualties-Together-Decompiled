using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KrokoshaCasualtiesUtils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public class UIChoicePrompt : KrokoshaScavSingleton
{
	public class Prompt
	{
		public class PromptChoice
		{
			public string text;

			public Action action;
		}

		public string name;

		public string text;

		public float timeout = 5f;

		public Texture2D texture;

		public Vector2 pos = new Vector2(0.2f, 0f);

		public Vector2 size = new Vector2(0.35f, 0.16f);

		public bool immidiate;

		public int defaultchoice = -1;

		public bool defaultcanignore;

		public byte chosenchoice;

		public bool chosenignore;

		public PromptChoice[] choices;

		public Rect CalculateRect(bool use_progress)
		{
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			int num = Mathf.Min(Screen.height, Screen.width);
			float num2 = (float)num * size.x;
			float num3 = (float)num * size.y;
			float num4 = ((float)Screen.width - num2) * pos.x;
			float num5 = ((float)Screen.height - num3) * pos.y;
			if (use_progress)
			{
				num5 = ((!(pos.y < 0.5f)) ? Mathf.Lerp((float)Screen.height, num5, prompt_show_progress) : Mathf.Lerp(0f - num3, num5, prompt_show_progress));
			}
			return new Rect(num4, num5, num2, num3);
		}
	}

	private Queue<Prompt> prompt_queue = new Queue<Prompt>();

	public static float prompt_show_progress;

	public static bool mouse_is_near_area;

	public static UIChoicePrompt instance { get; private set; }

	public UIChoicePrompt()
	{
		instance = this;
	}

	public static void ShowPrompt(Prompt prompt)
	{
		instance.prompt_queue.Enqueue(prompt);
	}

	private void Update()
	{
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		mouse_is_near_area = false;
		if (prompt_queue.Count > 0)
		{
			Prompt prompt = prompt_queue.Peek();
			if (prompt.timeout <= 0f)
			{
				prompt_show_progress = Mathf.MoveTowards(prompt_show_progress, 0f, Time.deltaTime * 4f);
				if (prompt_show_progress == 0f)
				{
					prompt_queue.Dequeue();
				}
			}
			else if (prompt_show_progress < 1f)
			{
				if (prompt.immidiate)
				{
					prompt_show_progress = 1f;
					return;
				}
				mouse_is_near_area = UIBullshit.IsCursorInGUIRect(prompt.CalculateRect(use_progress: false).ShrinkBorder(-20f * UIMainMenu.GetMenuUIScale()));
				float num = 3f;
				num = ((mouse_is_near_area && prompt_show_progress != 0f) ? 0.2f : 3f);
				prompt_show_progress = Mathf.MoveTowards(prompt_show_progress, 1f, Time.deltaTime * num);
				if (prompt_show_progress >= 1f)
				{
					Util.PlayUISound((UISoundType)0);
				}
			}
			else
			{
				if (!(prompt.timeout > 0f))
				{
					return;
				}
				prompt.timeout -= Time.deltaTime;
				if (prompt.timeout <= 0f)
				{
					if (prompt.defaultcanignore)
					{
						prompt.chosenignore = true;
					}
					else
					{
						PromptUse(prompt, prompt.defaultchoice);
					}
					Util.PlayUISound((UISoundType)2);
				}
			}
		}
		else
		{
			prompt_show_progress = 0f;
		}
	}

	public static void PromptUse(Prompt prompt, int choice)
	{
		byte b = (byte)choice;
		if (prompt.choices != null)
		{
			if (b >= prompt.choices.Count())
			{
				b = (byte)(prompt.choices.Count() - 1);
			}
			SafeActionInvoke(prompt.choices[b].action);
		}
		prompt.chosenchoice = b;
		prompt.timeout = 0f;
	}

	public static void SafeActionInvoke(Action action)
	{
		try
		{
			action();
		}
		catch (Exception ex)
		{
			log.error(ex.ToString());
		}
	}

	public static IEnumerator DoPromptCoroutine(Prompt prompt)
	{
		ShowPrompt(prompt);
		while (instance.prompt_queue.Contains(prompt))
		{
			yield return null;
		}
	}

	public static Task DoPromptAsync(Prompt prompt)
	{
		return Util.StartCoroutineAsync(DoPromptCoroutine(prompt));
	}

	internal void _GUI_RenderUIChoicePrompt()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		if (prompt_show_progress == 0f || prompt_queue.Count <= 0)
		{
			return;
		}
		Color color = GUI.color;
		bool wordWrap = GUI.skin.label.wordWrap;
		TextAnchor alignment = GUI.skin.label.alignment;
		Prompt prompt = prompt_queue.Peek();
		if (prompt_show_progress < 1f && mouse_is_near_area)
		{
			float num = Mathf.Sin(Time.realtimeSinceStartup * 6f);
			GUI.color = new Color(num, num, num, 0.7f + num * 0.3f);
			Rect val = prompt.CalculateRect(use_progress: false);
			GUI.Label(val, "", GUI.skin.box);
			GUI.color = color;
			GUI.skin.label.alignment = (TextAnchor)7;
			GUI.Label(val, Lang.Get("movemouse", false));
			UIBullshit._GUI_SetTooltip(Lang.Get("movemouse", false), "");
		}
		GUI.skin.label.alignment = (TextAnchor)1;
		Rect rect = prompt.CalculateRect(use_progress: true).ShrinkBorder(3f * UIMainMenu.GetMenuUIScale());
		UIBullshit.CheckCursorOverlap(in rect);
		GUILayout.BeginArea(rect, "", GUI.skin.box);
		UIBullshit._GUI_BiggerLabel(in prompt.name, 1.4f);
		GUI.skin.label.wordWrap = true;
		GUILayout.Label(prompt.text, Array.Empty<GUILayoutOption>());
		GUI.skin.label.wordWrap = wordWrap;
		if (prompt.timeout > 0f && prompt.choices != null)
		{
			GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();
			bool enabled = GUI.enabled;
			if (prompt_show_progress < 1f && mouse_is_near_area)
			{
				GUI.enabled = false;
				float num2 = Mathf.Sin(Time.realtimeSinceStartup * 6f);
				GUI.color = new Color(1f, 1f, 1f, 0.2f + num2 * 0.15f);
			}
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.FlexibleSpace();
			for (int i = 0; i < prompt.choices.Length; i++)
			{
				if (GUILayout.Button(prompt.choices[i].text, Array.Empty<GUILayoutOption>()))
				{
					PromptUse(prompt, i);
					Util.PlayUISound((UISoundType)1);
				}
				GUILayout.FlexibleSpace();
			}
			GUILayout.EndHorizontal();
			GUI.enabled = enabled;
			GUI.color = color;
		}
		GUILayout.EndArea();
		GUI.skin.label.alignment = alignment;
	}
}
