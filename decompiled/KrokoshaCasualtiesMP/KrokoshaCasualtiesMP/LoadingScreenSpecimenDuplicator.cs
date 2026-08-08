using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KrokoshaCasualtiesMP;

internal class LoadingScreenSpecimenDuplicator : MonoBehaviour
{
	private Vector2 OG_creaturepos = Vector2.zero;

	private List<RectTransform> clones = new List<RectTransform>();

	private static RectTransform og_creature_rect => WorldGeneration.world.genRects[2];

	private void Awake()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		OG_creaturepos = WorldGeneration.world.genRects[2].anchoredPosition;
	}

	private void LateUpdate()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		if (((Component)this).gameObject.activeInHierarchy)
		{
			OG_creaturepos = WorldGeneration.world.genRects[2].anchoredPosition;
			Vector2 val = new Vector2(100f, 25f) * PlayerCamera.uiScale;
			for (int i = 0; i < clones.Count; i++)
			{
				RectTransform obj = clones[i];
				float num = (float)i + 0.5f - (float)clones.Count * 0.5f;
				obj.anchoredPosition = OG_creaturepos - val * num;
			}
		}
	}

	private void OnEnable()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		GameObject gameObject = ((Component)og_creature_rect).gameObject;
		Image component = gameObject.GetComponent<Image>();
		((Graphic)component).color = new Color(1f, 1f, 1f, 1f);
		foreach (RectTransform clone in clones)
		{
			Object.Destroy((Object)(object)((Component)clone).gameObject);
		}
		clones.Clear();
		int num = 0;
		foreach (NetPlayer value in NetPlayer.ClientIdToPlayerDict.Values)
		{
			num++;
			GameObject obj = Object.Instantiate<GameObject>(gameObject);
			OnAddDuplicated(obj, value);
		}
		((Graphic)component).color = new Color(1f, 1f, 1f, 0f);
	}

	private void OnAddDuplicated(GameObject obj, NetPlayer plr)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		RectTransform component = obj.GetComponent<RectTransform>();
		((Object)obj).name = "GenCharacter_" + plr.playername;
		((Transform)component).SetParent(((Transform)og_creature_rect).parent);
		component.sizeDelta = Vector2.zero;
		((Transform)component).SetSiblingIndex(((Transform)og_creature_rect).GetSiblingIndex());
		clones.Add(component);
	}
}
