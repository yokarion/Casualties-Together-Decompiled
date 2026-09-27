using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using KrokoshaCasualtiesMP;
using KrokoshaCasualtiesUtils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace KrokoshaCasualtiesMP_DISABLED_STATE;

internal class KrokoshaCasualtiesMP_DISABLED_BUTTON_HANDLER : MonoBehaviour
{
	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static UnityAction _003C_003E9__4_0;

		internal void _003CDoTheNormalButton_003Eb__4_0()
		{
			PlayerPrefsExtended.SetBool("KrokoshaCasualtiesMP_FORCE_DISABLE_MP_MOD", !ON_CLICK_WILL_ENABLE_THE_MOD);
			try
			{
				PlayerPrefs.Save();
				log.l(string.Format("SET {0} => {1}", "KrokoshaCasualtiesMP_FORCE_DISABLE_MP_MOD", ON_CLICK_WILL_ENABLE_THE_MOD));
			}
			catch (Exception)
			{
			}
			_ = ON_CLICK_WILL_ENABLE_THE_MOD;
			Plugin.RestartGame();
		}
	}

	internal static bool ON_CLICK_WILL_ENABLE_THE_MOD = true;

	private GameObject MP_LINK_BUTTON;

	public static Sprite mpmod_icon = null;

	private int errorcounter;

	private void Awake()
	{
		try
		{
			mpmod_icon = CoopModAssets.LoadSprite("mp.png");
		}
		catch (Exception ex)
		{
			doerror(ex.ToString());
		}
		try
		{
			KSteam.CheckSteam();
		}
		catch (Exception ex2)
		{
			doerror("Attempt to load steam" + ex2.ToString());
		}
		Application.runInBackground = false;
	}

	private void DoTheNormalButton()
	{
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Expected O, but got Unknown
		if (!((Object)(object)MP_LINK_BUTTON == (Object)null))
		{
			return;
		}
		PreRunScript instance = PreRunScript.instance;
		if (!((Object)(object)instance != (Object)null))
		{
			return;
		}
		try
		{
			Transform val = ((Component)instance).transform.Find("Credits");
			Transform val2 = ((Component)instance).transform.Find("Credits/TranslationRepo");
			if (!((Object)(object)val2 != (Object)null))
			{
				return;
			}
			MP_LINK_BUTTON = Object.Instantiate<GameObject>(((Component)val2).gameObject, ((Component)instance).transform, false).gameObject;
			((Object)MP_LINK_BUTTON).name = "KROKMP_BUTTON_LINK";
			RectTransform component = MP_LINK_BUTTON.GetComponent<RectTransform>();
			component.anchoredPosition = new Vector2(140f, component.anchoredPosition.y);
			((Transform)component).SetSiblingIndex(val.GetSiblingIndex());
			UITooltip component2 = MP_LINK_BUTTON.GetComponent<UITooltip>();
			string key = (ON_CLICK_WILL_ENABLE_THE_MOD ? "mainmenu_mp_button_tooltip_lastresort_enable_mod" : "mainmenu_mp_button_tooltip_lastresort_disable_mod");
			component2.localeName = "krokosha_coop_" + key;
			component2.skipLocale = true;
			component2.tipName = Lang.Get(in key, false);
			Image component3 = MP_LINK_BUTTON.GetComponent<Image>();
			component3.sprite = mpmod_icon;
			((Graphic)component3).color = Color.red;
			Button component4 = MP_LINK_BUTTON.GetComponent<Button>();
			((UnityEventBase)component4.onClick).RemoveAllListeners();
			((UnityEventBase)component4.onClick).SetPersistentListenerState(0, (UnityEventCallState)0);
			ButtonClickedEvent onClick = component4.onClick;
			object obj = _003C_003Ec._003C_003E9__4_0;
			if (obj == null)
			{
				UnityAction val3 = delegate
				{
					PlayerPrefsExtended.SetBool("KrokoshaCasualtiesMP_FORCE_DISABLE_MP_MOD", !ON_CLICK_WILL_ENABLE_THE_MOD);
					try
					{
						PlayerPrefs.Save();
						log.l(string.Format("SET {0} => {1}", "KrokoshaCasualtiesMP_FORCE_DISABLE_MP_MOD", ON_CLICK_WILL_ENABLE_THE_MOD));
					}
					catch (Exception)
					{
					}
					_ = ON_CLICK_WILL_ENABLE_THE_MOD;
					Plugin.RestartGame();
				};
				_003C_003Ec._003C_003E9__4_0 = val3;
				obj = (object)val3;
			}
			((UnityEvent)onClick).AddListener((UnityAction)obj);
		}
		catch (Exception ex)
		{
			doerror("DoTheNormalButton failure " + ex.ToString());
		}
	}

	private void Update()
	{
		if ((Object)(object)mpmod_icon == (Object)null)
		{
			return;
		}
		try
		{
			DoTheNormalButton();
		}
		catch (Exception ex)
		{
			doerror(ex.ToString());
		}
	}

	private void doerror(string str)
	{
		if (errorcounter < 3)
		{
			errorcounter++;
			str = "KrokoshaCasualtiesMP_DISABLED: " + str + "\nFULL STACKTRACE:\n" + new StackTrace();
			if (Plugin.log != null)
			{
				Plugin.log.LogError((object)str);
			}
			else
			{
				MonoBehaviour.print((object)str);
			}
		}
	}
}
