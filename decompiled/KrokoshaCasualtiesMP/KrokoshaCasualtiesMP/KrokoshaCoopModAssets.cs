using System.Collections.Generic;
using System.IO;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

internal class KrokoshaCoopModAssets
{
	public static Sprite alttab;

	public static Sprite speechbubbleicon0;

	public static Sprite speechbubbleicon1;

	public static Sprite speechbubbleicon2;

	public static Sprite speechbubbleicon3;

	public static Sprite mpmod_icon;

	public static Sprite mpmod_icon_green;

	public static Sprite star_yellow;

	public static Sprite alert;

	public static Sprite alert2;

	public static Sprite recruit;

	public static Sprite cmdicon;

	public static Sprite arrowicon;

	public static Sprite healicon;

	public static Sprite circlething;

	public static Sprite voicechaticon;

	public static Sprite cpr_button_icon;

	public static Sprite icon_steam;

	public static Sprite icon_crown;

	public static Font gamefont;

	public static Shader coolshader;

	public static Dictionary<string, Object> assets = new Dictionary<string, Object>();

	public static Sprite[] trader_icon;

	public static Sprite checkbox;

	public static Sprite checkbox_ticked;

	public static Sprite crafting;

	public static Sprite inspectself;

	public static Sprite aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa;

	public static Sprite @lock;

	public static Sprite happy;

	public static Sprite cross;

	public static byte[] ReadResource(string name)
	{
		Assembly executingAssembly = Assembly.GetExecutingAssembly();
		string text = "KrokoshaCasualtiesMP.assets." + name;
		using (Stream stream = executingAssembly.GetManifestResourceStream(text))
		{
			if (stream != null)
			{
				byte[] array = new byte[stream.Length];
				stream.Read(array, 0, array.Length);
				return array;
			}
			Plugin.log.LogError((object)("Failed to load embedded resource: " + text));
		}
		return null;
	}

	public static Dictionary<string, Object> LoadAllResroucesFromBundle(string name)
	{
		Dictionary<string, Object> dictionary = new Dictionary<string, Object>();
		byte[] array = ReadResource(name);
		if (array != null)
		{
			AssetBundle val = AssetBundle.LoadFromMemory(array);
			string[] allAssetNames = val.GetAllAssetNames();
			foreach (string text in allAssetNames)
			{
				Object value = val.LoadAsset(text);
				assets[text] = value;
				dictionary[text] = value;
			}
		}
		return dictionary;
	}

	public static Sprite LoadSprite(string name)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		byte[] array = ReadResource(name);
		if (array != null)
		{
			Texture2D val = new Texture2D(2, 2);
			ImageConversion.LoadImage(val, array);
			((Texture)val).filterMode = (FilterMode)0;
			return Sprite.Create(val, new Rect(0f, 0f, (float)((Texture)val).width, (float)((Texture)val).height), new Vector2(0.5f, 0.5f));
		}
		return null;
	}

	public static void LoadAssets()
	{
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Expected O, but got Unknown
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Expected O, but got Unknown
		arrowicon = LoadSprite("arrow.png");
		circlething = LoadSprite("circlething.png");
		voicechaticon = LoadSprite("voicechaticon.png");
		healicon = LoadSprite("healicon.png");
		alttab = LoadSprite("alttab.png");
		cmdicon = LoadSprite("cmdicon.png");
		happy = LoadSprite("happy.png");
		@lock = LoadSprite("lock.png");
		cross = LoadSprite("shittycross.png");
		icon_crown = LoadSprite("king.png");
		icon_steam = LoadSprite("steam.png");
		speechbubbleicon0 = LoadSprite("speechbubble.png");
		speechbubbleicon1 = LoadSprite("speechbubble1.png");
		speechbubbleicon2 = LoadSprite("speechbubble2.png");
		speechbubbleicon3 = LoadSprite("speechbubble3.png");
		trader_icon = (Sprite[])(object)new Sprite[8];
		for (int i = 0; i < trader_icon.Length; i++)
		{
			trader_icon[i] = LoadSprite($"shop.{i + 1}.png");
		}
		mpmod_icon = LoadSprite("mp.png");
		mpmod_icon_green = LoadSprite("mp_green.png");
		star_yellow = LoadSprite("star_yellow.png");
		alert = LoadSprite("excl.png");
		alert2 = LoadSprite("alertthing.png");
		recruit = LoadSprite("recruit.png");
		cpr_button_icon = LoadSprite("cprbutton.png");
		checkbox = LoadSprite("checkbox.png");
		checkbox_ticked = LoadSprite("box_ticked.png");
		crafting = LoadSprite("crafting.png");
		inspectself = LoadSprite("inspectself.png");
		aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa = LoadSprite("krokosha.jpg");
		LoadAllResroucesFromBundle("retrogaming.bundle");
		gamefont = (Font)GeneralExtensions.GetValueSafe<string, Object>(assets, "assets/font/retrogaming_200.ttf");
		if ((Object)(object)gamefont == (Object)null)
		{
			log.error("!!!!! FAILED TO LOAD GAME FONT !!!!!!!!!");
		}
		LoadAllResroucesFromBundle("cooltextshader.bundle");
		coolshader = (Shader)GeneralExtensions.GetValueSafe<string, Object>(assets, "assets/textmesh pro/shaders/animatedgradient.shader");
		if ((Object)(object)coolshader == (Object)null)
		{
			log.error("!!!!! animatedgradient FAILED TO LOAD !!!!!!!!!!!!");
		}
	}
}
