using System.Collections.Generic;
using KrokoshaCasualtiesUtils;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KrokoshaCasualtiesMP;

public class CPRMinigame : Minigame
{
	public CPRHandler handler = ComponentHolderProtocol.GetOrAddComponent<CPRHandler>((Object)(object)Minigame.game);

	public Body pacient;

	public Limb limb;

	public Body LOCAL_BODY => Util.GetLocalBody();

	public Body healer => LOCAL_BODY;

	public bool stupid => healer.skills.INT < CPRHandler.SMART_INT_REQUIREMENT;

	public override HandSpriteType HandType()
	{
		return (HandSpriteType)0;
	}

	public string GetGuideText()
	{
		if (stupid)
		{
			return Lang.Get("cpr_minigame_guide_stupid", false);
		}
		return Lang.Get("cpr_minigame_guide", false);
	}

	public override string GuideLocaleString()
	{
		return GetGuideText();
	}

	public override bool NeedsItem()
	{
		return false;
	}

	public CPRMinigame(Body body)
	{
		pacient = body;
		limb = body.GetUpperTorso();
	}

	public override void Start()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		handler.pacient = pacient;
		Minigame.game.CreateScreen("Special/SelfHarmMinigame");
		Image[] componentsInChildren = ((Component)Minigame.game.spawnedMiniGame).GetComponentsInChildren<Image>();
		foreach (Image obj in componentsInChildren)
		{
			Color color = ((Graphic)obj).color;
			color.a = 0f;
			((Graphic)obj).color = color;
		}
		handler.MG_CreateSecondHand();
	}

	public override void Update(List<RaycastResult> uiCasts)
	{
		if ((Object)(object)pacient == (Object)null)
		{
			handler.MG_End();
			PlayerCamera.main.DoAlert(Lang.Get("plr_too_far", false), false);
			return;
		}
		Component ba = (Component)(object)pacient;
		if (!KM.dist2dsqrcheck(in ba, (Component)(object)LOCAL_BODY, SharedMain.max_player_interaction_distance * 2f))
		{
			handler.MG_End();
			PlayerCamera.main.DoAlert(Lang.Get("plr_too_far", false), false);
		}
		else
		{
			((TMP_Text)Minigame.game.guideText).text = ((Minigame)this).GuideLocaleString();
			handler.MG_Update(uiCasts);
		}
	}

	public override void PhysicsUpdate(float deltaTime)
	{
		handler.MG_PhysicsUpdate(deltaTime);
	}

	public override float HandRotOffset()
	{
		return handler.cpr_rot;
	}
}
