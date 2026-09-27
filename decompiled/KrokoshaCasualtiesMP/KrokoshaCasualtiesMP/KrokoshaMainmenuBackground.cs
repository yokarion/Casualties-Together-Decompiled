using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace KrokoshaCasualtiesMP;

public static class KrokoshaMainmenuBackground
{
	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static UnityAction _003C_003E9__10_3;

		internal void _003CCreateBackground_003Eb__10_3()
		{
			UIMainMenu.SetOpen(!UIMainMenu.mainmenu_open);
			if ((Object)(object)runsettingsmenu != (Object)null)
			{
				runsettingsmenu.SetActive(false);
			}
		}
	}

	private static string[] sprites_to_find = new string[14]
	{
		"uiBlock", "uiBlockMini", "uiBlockNano", "uiBlockSmall", "UICheckMark", "uicursor", "uicursorpressed", "UIElement8px", "UIFoldoutClosed", "UIFoldoutOpened",
		"uigradientblack", "uiPoint", "userbarfill", "whitesquare"
	};

	public static Dictionary<string, Sprite> ui_sprites = new Dictionary<string, Sprite>();

	public static Texture2D og_uiBlockSmall;

	public static Texture2D og_uiBlockNano;

	public static GameObject template_button;

	public static GameObject template_label;

	public static GameObject runsettingsmenu;

	public static GameObject mpmenu_button;

	public static GameObject MP_LINK_BUTTON;

	public static GameObject bgObj;

	public static void CreateBackground()
	{
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Expected O, but got Unknown
		//IL_0649: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Expected O, but got Unknown
		//IL_06a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0662: Unknown result type (might be due to invalid IL or missing references)
		//IL_066e: Unknown result type (might be due to invalid IL or missing references)
		//IL_067e: Unknown result type (might be due to invalid IL or missing references)
		//IL_054c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0562: Unknown result type (might be due to invalid IL or missing references)
		//IL_056e: Unknown result type (might be due to invalid IL or missing references)
		//IL_057a: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b2: Expected O, but got Unknown
		PreRunScript prerun = Object.FindObjectOfType<PreRunScript>();
		Canvas val = null;
		if (!((Object)(object)prerun != (Object)null))
		{
			return;
		}
		val = ((Component)prerun).GetComponent<Canvas>();
		if (!((Object)(object)val != (Object)null) || !((Object)(object)bgObj == (Object)null))
		{
			return;
		}
		bgObj = new GameObject("KrokoshaMultiplayerBackground");
		bgObj.transform.SetParent(((Component)val).transform);
		try
		{
			Sprite[] source = Resources.FindObjectsOfTypeAll<Sprite>();
			string[] array = sprites_to_find;
			foreach (string spritename in array)
			{
				Sprite val2 = source.FirstOrDefault((Sprite f) => ((Object)f).name == spritename);
				if ((Object)(object)val2 == (Object)null)
				{
					val2 = source.FirstOrDefault((Sprite f) => ((Object)f).name.Contains(spritename));
					if ((Object)(object)val2 == (Object)null)
					{
						if (log.verbose)
						{
							Plugin.Logger.LogWarning((object)("\"" + spritename + "\" wasnt found !!!!!!!!!!!!"));
						}
						continue;
					}
					if (log.verbose)
					{
						Plugin.Logger.LogWarning((object)("\"" + spritename + "\" was found through Contains: \"" + ((Object)val2).name + "\""));
					}
				}
				else if (log.verbose)
				{
					Plugin.Logger.LogInfo((object)("SPRITE SEARCH: \"" + spritename + "\" was found."));
				}
				ui_sprites[spritename] = val2;
			}
		}
		catch (Exception ex)
		{
			log.error("ASSET SEARCH 1 " + ex.ToString());
		}
		if (ui_sprites.TryGetValue("uiBlockSmall", out var value) || ui_sprites.TryGetValue("uiBlock", out value))
		{
			og_uiBlockSmall = value.texture;
		}
		if (ui_sprites.TryGetValue("uiBlockNano", out var value2))
		{
			og_uiBlockNano = value2.texture;
		}
		try
		{
			Transform val3 = ((Component)prerun).transform.Find("Credits");
			Transform val4 = ((Component)prerun).transform.Find("Credits/TranslationRepo");
			if ((Object)(object)val4 != (Object)null)
			{
				MP_LINK_BUTTON = Object.Instantiate<GameObject>(((Component)val4).gameObject, ((Component)prerun).transform, false).gameObject;
				((Object)MP_LINK_BUTTON).name = "KROKMP_BUTTON_LINK";
				RectTransform component = MP_LINK_BUTTON.GetComponent<RectTransform>();
				component.anchoredPosition = new Vector2(50f, component.anchoredPosition.y);
				((Transform)component).SetSiblingIndex(val3.GetSiblingIndex());
				UITooltip component2 = MP_LINK_BUTTON.GetComponent<UITooltip>();
				component2.localeName = "krokosha_coop_mainmenu_mp_button_tooltip";
				component2.skipLocale = true;
				component2.tipName = Lang.Get("mainmenu_mp_button_tooltip", false);
				Image component3 = MP_LINK_BUTTON.GetComponent<Image>();
				component3.sprite = CoopModAssets.mpmod_icon;
				((Graphic)component3).color = Color.green;
				Button component4 = MP_LINK_BUTTON.GetComponent<Button>();
				((UnityEventBase)component4.onClick).RemoveAllListeners();
				((UnityEventBase)component4.onClick).SetPersistentListenerState(0, (UnityEventCallState)0);
				((UnityEvent)component4.onClick).AddListener((UnityAction)delegate
				{
					if ((Object)(object)UIBullshit.main == (Object)null || (UIMainMenu.IsOpen() && UIMainMenu.current_tab == UIMainMenu.menutab_about) || Con.IsConsoleOpen())
					{
						prerun.OpenURL("https://www.nexusmods.com/scavprototype/mods/67");
					}
					else
					{
						UIMainMenu.SetOpen(open_or_nah: true);
						UIMainMenu.current_tab = UIMainMenu.menutab_about;
					}
				});
			}
			else
			{
				log.error("BUTTON SEARCH KROKMP_BUTTON_LINK not found");
			}
		}
		catch (Exception ex2)
		{
			log.error("BUTTON SEARCH 1 " + ex2.ToString());
		}
		try
		{
			Transform val5 = ((Component)prerun).transform.Find("RunSettings");
			Transform val6 = ((Component)prerun).transform.Find("RunSettings/Presets/Cancel");
			if ((Object)(object)val6 != (Object)null)
			{
				template_button = Object.Instantiate<GameObject>(((Component)val6).gameObject, ((Component)prerun).transform, false);
				((Object)template_button).name = "KROKMP_BUTTON";
				Button component5 = template_button.GetComponent<Button>();
				((Graphic)((Component)component5).GetComponentInChildren<TMP_Text>(true)).color = Color.white;
				((UnityEventBase)component5.onClick).RemoveAllListeners();
				((UnityEventBase)component5.onClick).SetPersistentListenerState(0, (UnityEventCallState)0);
				template_button.SetActive(false);
			}
			if ((Object)(object)val5 != (Object)null)
			{
				runsettingsmenu = ((Component)val5).gameObject;
			}
		}
		catch (Exception ex3)
		{
			log.error("RUNSETTINGS SEARCH 1 " + ex3.ToString());
		}
		try
		{
			if ((Object)(object)((Component)prerun).transform != (Object)null)
			{
				RectTransform component6 = ((Component)prerun).GetComponent<RectTransform>();
				if ((Object)(object)template_button != (Object)null)
				{
					mpmenu_button = Object.Instantiate<GameObject>(template_button.gameObject, ((Component)prerun).transform, false);
					mpmenu_button.SetActive(true);
					((Object)mpmenu_button).name = "KROKMP_MENU_OPEN_BUTTON";
					TextMeshProUGUI componentInChildren = mpmenu_button.GetComponentInChildren<TextMeshProUGUI>();
					((TMP_Text)componentInChildren).text = Lang.Get("mainmenu_mainbutton", false);
					UITooltip val7 = default(UITooltip);
					if (((Component)componentInChildren).TryGetComponent<UITooltip>(ref val7))
					{
						val7.localeName = "krokosha_coop_mainmenu_mainbutton";
						val7.skipLocale = true;
					}
					UILocalizer val8 = default(UILocalizer);
					if (((Component)componentInChildren).TryGetComponent<UILocalizer>(ref val8))
					{
						val8.key = "krokosha_coop_mainmenu_mainbutton";
						Object.Destroy((Object)(object)val8);
					}
					RectTransform component7 = mpmenu_button.GetComponent<RectTransform>();
					if ((Object)(object)MP_LINK_BUTTON != (Object)null)
					{
						((Transform)component7).SetSiblingIndex(MP_LINK_BUTTON.transform.GetSiblingIndex());
					}
					else
					{
						((Transform)component7).SetSiblingIndex(((Transform)component6).childCount);
					}
					component7.anchorMin = new Vector2(0.3f, 0f);
					component7.anchorMax = new Vector2(0.6f, 0.1f);
					component7.offsetMin = Vector2.zero;
					component7.offsetMax = Vector2.zero;
					ButtonClickedEvent onClick = ComponentHolderProtocol.GetOrAddComponent<Button>((Object)(object)mpmenu_button).onClick;
					object obj = _003C_003Ec._003C_003E9__10_3;
					if (obj == null)
					{
						UnityAction val9 = delegate
						{
							UIMainMenu.SetOpen(!UIMainMenu.mainmenu_open);
							if ((Object)(object)runsettingsmenu != (Object)null)
							{
								runsettingsmenu.SetActive(false);
							}
						};
						_003C_003Ec._003C_003E9__10_3 = val9;
						obj = (object)val9;
					}
					((UnityEvent)onClick).AddListener((UnityAction)obj);
					Image val10 = default(Image);
					if (mpmenu_button.TryGetComponent<Image>(ref val10))
					{
						val10.sprite = value;
					}
				}
				_ = (Object)(object)template_label != (Object)null;
			}
		}
		catch (Exception ex4)
		{
			log.error("CREATING MENU BUTTONS " + ex4.ToString());
		}
		bool flag = false;
		try
		{
			Transform val11 = ((Component)prerun).transform.Find("VersionWarning");
			if ((Object)(object)val11 != (Object)null)
			{
				Transform child = val11.GetChild(0);
				TextMeshProUGUI val12 = default(TextMeshProUGUI);
				if ((Object)(object)child != (Object)null && ((Component)child).TryGetComponent<TextMeshProUGUI>(ref val12))
				{
					((TMP_Text)val12).text = "This is a mod";
					((Graphic)val12).color = Color.red;
					RectTransform val13 = default(RectTransform);
					if (((Component)child).TryGetComponent<RectTransform>(ref val13))
					{
						val13.offsetMax = new Vector2(val13.offsetMax.x, val13.offsetMax.y - 10f);
					}
				}
				Transform child2 = val11.GetChild(1);
				TextMeshProUGUI val14 = default(TextMeshProUGUI);
				if ((Object)(object)child2 != (Object)null && ((Component)child2).TryGetComponent<TextMeshProUGUI>(ref val14) && ((Graphic)val14).color != Color.red)
				{
					((TMP_Text)val14).text = "<alpha=#11><i>...meaning it's a shitty mod for an unfinished game.<alpha=#FF></i>\r\nDirect your bug reports to the\n\"Casualties: Together\" Discord.";
					flag = true;
				}
			}
		}
		catch (Exception ex5)
		{
			log.error("VersionWarning SEARCH 1 " + ex5.ToString());
		}
		if (!flag)
		{
			log.warn("MainMenuPatcher: Could not find version wanning ui element!");
		}
	}
}
