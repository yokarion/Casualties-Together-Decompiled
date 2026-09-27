using UnityEngine;

namespace KrokoshaCasualtiesMP;

internal class KrokoshaMP_CharacterStatusIcon_Healing : ICharacterStatusIcon
{
	private CharStatusVisuals visual;

	private SpriteRenderer spr;

	private Vector3 localScale = Vector3.one;

	private Body body => visual.body;

	private NetBody npc => visual.netbody;

	public bool AppearCondition()
	{
		if (npc.is_player && npc.body.conscious)
		{
			return npc.plr.woundViewTargetNetBodyId != 0;
		}
		return false;
	}

	public void Create(CharStatusVisuals visual, SpriteRenderer spr)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		this.visual = visual;
		this.spr = spr;
		spr.sprite = CoopModAssets.healicon;
		localScale = ((Component)spr).transform.localScale;
		((Component)spr).transform.localScale = localScale * 1.45f;
	}

	public bool DisappearCondition()
	{
		return !AppearCondition();
	}

	public void Update()
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if (npc.plr.woundViewTargetNetBodyId != (ushort)npc.netId)
		{
			spr.sprite = CoopModAssets.healicon;
			((Component)spr).transform.localScale = localScale * 1.4f;
		}
		else
		{
			spr.sprite = CoopModAssets.inspectself;
			((Component)spr).transform.localScale = localScale * 1.05f;
		}
	}
}
