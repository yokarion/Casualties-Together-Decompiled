using UnityEngine;

namespace KrokoshaCasualtiesMP;

internal class KrokoshaMP_CharacterStatusIcon_Inventory : ICharacterStatusIcon
{
	private CharStatusVisuals visual;

	private SpriteRenderer spr;

	private Body body => visual.body;

	private NetBody npc => visual.netbody;

	public bool AppearCondition()
	{
		return false;
	}

	public void Create(CharStatusVisuals visual, SpriteRenderer spr)
	{
		this.visual = visual;
		this.spr = spr;
		spr.sprite = WorldgenPatches.white_square;
	}

	public bool DisappearCondition()
	{
		return !AppearCondition();
	}

	public void Update()
	{
	}
}
