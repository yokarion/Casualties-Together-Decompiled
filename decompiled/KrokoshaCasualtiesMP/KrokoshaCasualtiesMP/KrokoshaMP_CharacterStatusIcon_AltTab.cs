using UnityEngine;

namespace KrokoshaCasualtiesMP;

internal class KrokoshaMP_CharacterStatusIcon_AltTab : ICharacterStatusIcon
{
	private CharStatusVisuals visual;

	private SpriteRenderer spr;

	private Body body => visual.body;

	private NetBody npc => visual.netbody;

	public bool AppearCondition()
	{
		if (npc.is_player)
		{
			return npc.plr.is_alttab;
		}
		return false;
	}

	public void Create(CharStatusVisuals visual, SpriteRenderer spr)
	{
		this.visual = visual;
		this.spr = spr;
		spr.sprite = KrokoshaCoopModAssets.alttab;
	}

	public bool DisappearCondition()
	{
		return !AppearCondition();
	}

	public void Update()
	{
	}
}
