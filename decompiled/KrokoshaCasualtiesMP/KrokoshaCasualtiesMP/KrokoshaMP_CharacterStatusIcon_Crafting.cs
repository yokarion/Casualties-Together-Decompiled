using UnityEngine;

namespace KrokoshaCasualtiesMP;

internal class KrokoshaMP_CharacterStatusIcon_Crafting : ICharacterStatusIcon
{
	private CharStatusVisuals visual;

	private SpriteRenderer spr;

	private Body body => visual.body;

	private NetBody npc => visual.netbody;

	public bool AppearCondition()
	{
		if (npc.is_player && npc.body.conscious)
		{
			return npc.plr.is_crafting;
		}
		return false;
	}

	public void Create(CharStatusVisuals visual, SpriteRenderer spr)
	{
		this.visual = visual;
		this.spr = spr;
		spr.sprite = CoopModAssets.crafting;
	}

	public bool DisappearCondition()
	{
		return !AppearCondition();
	}

	public void Update()
	{
	}
}
