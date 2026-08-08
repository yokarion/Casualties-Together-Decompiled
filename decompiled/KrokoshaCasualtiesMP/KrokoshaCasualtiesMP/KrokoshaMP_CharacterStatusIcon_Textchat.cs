using UnityEngine;

namespace KrokoshaCasualtiesMP;

internal class KrokoshaMP_CharacterStatusIcon_Textchat : ICharacterStatusIcon
{
	private float counter;

	private CharStatusVisuals visual;

	private SpriteRenderer spr;

	private Body body => visual.body;

	private NetBody npc => visual.netbody;

	public bool AppearCondition()
	{
		if (npc.is_player)
		{
			return npc.plr.is_chatting;
		}
		return false;
	}

	public void Create(CharStatusVisuals visual, SpriteRenderer spr)
	{
		this.visual = visual;
		this.spr = spr;
		spr.sprite = KrokoshaCoopModAssets.speechbubbleicon0;
		spr.flipX = true;
	}

	public bool DisappearCondition()
	{
		return !AppearCondition();
	}

	public void Update()
	{
		counter += Time.unscaledDeltaTime;
		if (counter > 0.75f)
		{
			spr.sprite = KrokoshaCoopModAssets.speechbubbleicon3;
			if (counter >= 1f)
			{
				counter = 0f;
			}
		}
		else if (counter > 0.5f)
		{
			spr.sprite = KrokoshaCoopModAssets.speechbubbleicon2;
		}
		else if (counter > 0.25f)
		{
			spr.sprite = KrokoshaCoopModAssets.speechbubbleicon1;
		}
		else if (counter > 0f)
		{
			spr.sprite = KrokoshaCoopModAssets.speechbubbleicon0;
		}
	}
}
