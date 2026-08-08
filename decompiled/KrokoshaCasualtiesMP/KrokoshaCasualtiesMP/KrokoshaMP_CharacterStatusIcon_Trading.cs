using UnityEngine;

namespace KrokoshaCasualtiesMP;

internal class KrokoshaMP_CharacterStatusIcon_Trading : ICharacterStatusIcon
{
	private float counter;

	private float maxtime = 2f;

	private Vector3 localScale = Vector3.one;

	private CharStatusVisuals visual;

	private SpriteRenderer spr;

	private Body body => visual.body;

	private NetBody npc => visual.netbody;

	public bool AppearCondition()
	{
		if (npc.is_player)
		{
			return npc.plr.is_trading;
		}
		return false;
	}

	public void Create(CharStatusVisuals visual, SpriteRenderer spr)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		this.visual = visual;
		this.spr = spr;
		localScale = ((Component)spr).transform.localScale;
		Update();
	}

	public bool DisappearCondition()
	{
		return !AppearCondition();
	}

	public void Update()
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		counter += Time.unscaledDeltaTime;
		if (counter > maxtime)
		{
			counter = 0f;
		}
		int num = Mathf.FloorToInt(counter / maxtime * (float)(KrokoshaCoopModAssets.trader_icon.Length - 1));
		spr.sprite = KrokoshaCoopModAssets.trader_icon[num];
		((Component)spr).transform.localScale = localScale * 1.4f;
	}
}
