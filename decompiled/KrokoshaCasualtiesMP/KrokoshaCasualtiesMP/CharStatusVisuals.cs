using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using KrokoshaCasualtiesUtils;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace KrokoshaCasualtiesMP;

public class CharStatusVisuals : MonoBehaviour
{
	public class IconInstance
	{
		public NetBody npc;

		public GameObject icon;

		public ICharacterStatusIcon iconfunc;

		public int index;

		public float xoverlapmove;

		public SpriteRenderer spr => icon.GetComponent<SpriteRenderer>();

		public Vector2 position
		{
			get
			{
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				return Vector2.op_Implicit(icon.transform.position);
			}
			set
			{
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				//IL_0027: Unknown result type (might be due to invalid IL or missing references)
				//IL_0031: Unknown result type (might be due to invalid IL or missing references)
				icon.transform.position = new Vector3(value.x, value.y, ((Component)npc.body).transform.position.z);
			}
		}
	}

	internal static GameObject PREFAB_ExpTalkText;

	internal static Type[] ALL_STATUS_ICONS;

	internal static bool DEV_forceenable_nametags_for_everyone;

	public Color color_st = Color.white;

	public int total_visible_icons;

	public Vector2 body_center_pos = Vector2.zero;

	private Dictionary<Type, IconInstance> icons = new Dictionary<Type, IconInstance>();

	public GameObject nametag { get; private set; }

	public Body body => ((Component)this).GetComponent<Body>();

	public NetBody netbody => ((Component)this).GetComponent<NetBody>();

	public NetPlayer plr => netbody?.plr;

	public GameObject chara => netbody.chara;

	public Color color => netbody.color;

	public string name => netbody.bodyname;

	private void Awake()
	{
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Expected O, but got Unknown
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		if (ALL_STATUS_ICONS == null)
		{
			ALL_STATUS_ICONS = (from myType in AppDomain.CurrentDomain.GetAssemblies().SelectMany((Assembly a) => TypeUtility.GetTypesSafely(a))
				where myType.IsClass && !myType.IsAbstract && !myType.IsInterface && typeof(ICharacterStatusIcon).IsAssignableFrom(myType)
				select myType).ToArray();
		}
		if ((Object)(object)PREFAB_ExpTalkText == (Object)null)
		{
			Object obj = Resources.Load("special/ExpTalkText");
			PREFAB_ExpTalkText = (GameObject)(object)((obj is GameObject) ? obj : null);
			Object.DontDestroyOnLoad((Object)(object)PREFAB_ExpTalkText);
			PREFAB_ExpTalkText.SetActive(false);
		}
		Vector2 position = netbody.GetHeadPos() + Vector2.up * 2f;
		for (int num = 0; num < ALL_STATUS_ICONS.Length; num++)
		{
			Type type = ALL_STATUS_ICONS[num];
			ICharacterStatusIcon characterStatusIcon = (ICharacterStatusIcon)Activator.CreateInstance(type);
			GameObject val = new GameObject(type.Name);
			val.transform.parent = netbody.chara.transform;
			SpriteRenderer val2 = val.AddComponent<SpriteRenderer>();
			((Renderer)val2).sortingOrder = 6000;
			val2.color = color_st;
			val.transform.localScale = Vector3.one * 0.9f;
			characterStatusIcon.Create(this, val2);
			IconInstance iconInstance = new IconInstance
			{
				npc = netbody,
				icon = val,
				iconfunc = characterStatusIcon,
				index = num
			};
			iconInstance.position = position;
			val.SetActive(false);
			icons.Add(type, iconInstance);
		}
		nametag = Object.Instantiate<GameObject>(PREFAB_ExpTalkText, ((Component)this).transform.position, Quaternion.identity);
		nametag.SetActive(true);
		nametag.transform.SetParent(((Component)body).transform.parent);
		TextMeshPro component = nametag.GetComponent<TextMeshPro>();
		RectTransform rectTransform = ((TMP_Text)component).rectTransform;
		((Transform)rectTransform).position = ((Transform)rectTransform).position + new Vector3(0f, 99999f);
		((TMP_Text)component).richText = false;
	}

	private void Start()
	{
		if ((Object)(object)plr != (Object)null && KnownPersons.CanDoFancyNametagFor(plr))
		{
			MakeNametagFancy();
		}
	}

	public void MakeNametagFancy()
	{
		MakeNametagFancy((TMP_Text)(object)nametag.GetComponent<TextMeshPro>(), plr);
	}

	public static void MakeNametagFancy(TMP_Text tmp, NetPlayer plr)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		if (!((Object)(object)tmp.fontMaterial != (Object)null) || !((Object)tmp.fontMaterial).name.StartsWith("FANCY"))
		{
			tmp.fontMaterial = new Material(tmp.fontMaterial);
			((Object)tmp.fontMaterial).name = "FANCYTEXT";
			tmp.fontMaterial.shader = CoopModAssets.coolshader;
			tmp.fontMaterial.SetFloat("_OutlineWidth", tmp.outlineWidth);
			if ((Object)(object)plr != (Object)null)
			{
				FancyNametagUpdateColor(tmp, plr.playerColor);
			}
		}
	}

	public static bool FancyNametagUpdateColor(TMP_Text tmp, Color24 color, float scale = 0.9f)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if (((Object)tmp.fontMaterial).name.StartsWith("FANCY"))
		{
			((Graphic)tmp).color = Color.white * 1.2f;
			Color val = Color.Lerp(Util_MiscSystemExtensions.ColorBW(color), (Color)color, 0.3f);
			tmp.fontMaterial.SetColor("_ColorA", val);
			tmp.fontMaterial.SetColor("_ColorB", (Color)color);
			tmp.fontMaterial.SetFloat("_LocalGradientScale", scale);
			tmp.fontMaterial.SetFloat("_GradientSpeed", 6f);
			return true;
		}
		return false;
	}

	protected void LateUpdate()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0402: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_0411: Unknown result type (might be due to invalid IL or missing references)
		//IL_0416: Unknown result type (might be due to invalid IL or missing references)
		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
		body_center_pos = Vector2.zero;
		Limb[] limbs = body.limbs;
		foreach (Limb val in limbs)
		{
			body_center_pos += Vector2.op_Implicit(((Component)val).transform.position);
		}
		body_center_pos /= (float)body.limbs.Length;
		if (body.standing)
		{
			body_center_pos.x = ((Component)body).transform.position.x;
		}
		if ((Object)(object)nametag != (Object)null)
		{
			if (DEV_forceenable_nametags_for_everyone)
			{
				nametag.SetActive(true);
			}
			else if (body.alive && !Util.IsBodyLocal(body))
			{
				bool flag = KrokoshaScavMultiplayer.rules.EnableNametags && (!KrokoshaScavMultiplayer.rules.UnchippedHideNametags || !Util.IsUnchipped());
				if (nametag.activeSelf != flag)
				{
					nametag.SetActive(flag);
				}
			}
			else if (nametag.activeSelf)
			{
				nametag.SetActive(false);
			}
			if (nametag.activeSelf)
			{
				((Transform)((TMP_Text)nametag.GetComponent<TextMeshPro>()).rectTransform).position = Vector2.op_Implicit(body_center_pos - Vector2.up * 3f);
			}
		}
		int num = 0;
		if (Con._DEV_HIDEHUD || !KrokoshaScavMultiplayer.rules.EnableStatusIcons)
		{
			foreach (IconInstance value in icons.Values)
			{
				value.icon.SetActive(false);
			}
		}
		else
		{
			netbody.GetHeadPos();
			TextMeshPro text = body.talker.text;
			Transform transform = text.transform;
			Bounds bounds = ((TMP_Text)text).bounds;
			float x = ((Bounds)(ref bounds)).center.x;
			bounds = ((TMP_Text)text).bounds;
			float y = ((Bounds)(ref bounds)).max.y;
			bounds = ((TMP_Text)text).bounds;
			Vector3 val2 = transform.TransformPoint(new Vector3(x, y, ((Bounds)(ref bounds)).center.z));
			foreach (IconInstance value2 in icons.Values)
			{
				if (!value2.icon.activeSelf)
				{
					if (!body.alive || !value2.iconfunc.AppearCondition())
					{
						continue;
					}
					value2.icon.SetActive(true);
					total_visible_icons++;
					if (!body.IsBodyLocal())
					{
						Util.PlayWorldSoundOnScreenIfInRange("miniClick", in body_center_pos, 0.07f, 1f, 20f);
					}
				}
				else if (value2.iconfunc.DisappearCondition())
				{
					value2.icon.SetActive(false);
					total_visible_icons--;
					if (!body.IsBodyLocal())
					{
						Util.PlayWorldSoundOnScreenIfInRange("miniClick", in body_center_pos, 0.07f, 0.6f, 20f);
					}
					continue;
				}
				Vector2 position = value2.position;
				float num2 = (float)(total_visible_icons - 1) * 0.646f;
				value2.xoverlapmove = Mathf.Lerp(value2.xoverlapmove, (0f - num2) * 0.5f + (float)num * 0.646f, Time.deltaTime * 10f);
				Vector2 val3 = body_center_pos + KM.v2fromAngle(value2.xoverlapmove) * 5.5f;
				if (!string.IsNullOrEmpty(((TMP_Text)body.talker.text).text))
				{
					val3.y = Mathf.Max(val3.y, val2.y + 1.5f);
				}
				value2.position = KM.clampDistance(Vector2.Lerp(position, val3, Time.deltaTime * 30f), val3, 2f);
				value2.iconfunc.Update();
				num++;
			}
		}
		total_visible_icons = num;
	}

	public void ApplyNameAndColor()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		color_st = new Color(color.r, color.g, color.b, 0.87f);
		if ((Object)(object)nametag != (Object)null)
		{
			TextMeshPro component = nametag.GetComponent<TextMeshPro>();
			((Object)component).name = "CharacterNametag_" + name;
			((TMP_Text)component).text = name;
			if (((Object)((TMP_Text)component).fontMaterial).name.StartsWith("FANCY"))
			{
				((Graphic)component).color = Color.white * 1.2f;
				Color val = color;
				Color val2 = Color.Lerp(color.ColorBW(), color, 0.3f);
				((TMP_Text)component).fontMaterial.SetColor("_ColorA", val2);
				((TMP_Text)component).fontMaterial.SetColor("_ColorB", val);
				((TMP_Text)component).fontMaterial.SetFloat("_LocalGradientScale", 0.9f);
				((TMP_Text)component).fontMaterial.SetFloat("_GradientSpeed", 6f);
			}
			else
			{
				((Graphic)component).color = color_st;
			}
		}
		if (!netbody.is_local)
		{
			Color val3 = default(Color);
			((Color)(ref val3))._002Ector(color.r, color.g, color.b, ((Graphic)body.talker.text).color.a);
			((Graphic)body.talker.text).color = val3;
		}
		foreach (IconInstance value in icons.Values)
		{
			value.spr.color = color_st;
		}
		if (netbody.is_player)
		{
			if (Object.op_Implicit((Object)(object)netbody.plr.locationPingCircle))
			{
				((Object)netbody.plr.locationPingCircle).name = "fingerpointercircle_" + name;
				ComponentHolderProtocol.GetOrAddComponent<SpriteRenderer>((Object)(object)netbody.plr.locationPingCircle).color = color_st;
			}
			if (Object.op_Implicit((Object)(object)netbody.plr.locationPingArrow))
			{
				((Object)netbody.plr.locationPingArrow).name = "fingerpointerarrow_" + name;
				ComponentHolderProtocol.GetOrAddComponent<SpriteRenderer>((Object)(object)netbody.plr.locationPingArrow).color = color_st;
			}
		}
	}

	protected void OnDestroy()
	{
		if ((Object)(object)nametag != (Object)null)
		{
			Object.Destroy((Object)(object)nametag);
		}
		foreach (KeyValuePair<Type, IconInstance> icon in icons)
		{
			Object.Destroy((Object)(object)icon.Value.icon);
		}
		icons.Clear();
	}
}
