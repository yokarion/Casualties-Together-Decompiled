using System.Collections.Generic;
using KrokoshaCasualtiesUtils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KrokoshaCasualtiesMP;

public class Krokosha_CorpseScript_MultiplayerAdditionComponent : MonoBehaviour
{
	public bool animalCorpse = true;

	public HashSet<NetBody> people_that_seen_it = new HashSet<NetBody>();

	public const float SEE_RADIUS = 7f;

	private void Awake()
	{
		CorpseScript val = default(CorpseScript);
		Body val2 = default(Body);
		if (((Component)this).TryGetComponent<CorpseScript>(ref val))
		{
			animalCorpse = val.animalCorpse;
		}
		else if (((Component)this).TryGetComponent<Body>(ref val2))
		{
			animalCorpse = false;
		}
	}

	private void Start()
	{
		((MonoBehaviour)this).InvokeRepeating("SlowUpdate", Random.value * 2f, 0.1f);
	}

	private void SlowUpdate()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		if (animalCorpse)
		{
			return;
		}
		NetBody netBody = default(NetBody);
		if (((Component)this).TryGetComponent<NetBody>(ref netBody))
		{
			if (netBody.body.alive)
			{
				Object.Destroy((Object)(object)this);
				return;
			}
			if (netBody.timeHasBeenDead < 2.0)
			{
				return;
			}
		}
		foreach (NetBody item in NetPlayer.GetPlayerBodiesInRadius(Vector2.op_Implicit(((Component)this).transform.position), 7f))
		{
			if (people_that_seen_it.Contains(item))
			{
				continue;
			}
			people_that_seen_it.Add(item);
			if (!item.is_local || (Object)(object)netBody != (Object)null)
			{
				Body body = item.body;
				body.happiness -= 3.5f * item.body.desensitizedMult;
				Body body2 = item.body;
				body2.sicknessAmount += 6f * item.body.desensitizedMult;
				Body body3 = item.body;
				body3.desensitizedMult *= 0.9f;
				Body body4 = item.body;
				body4.corpsesSeen++;
				if (item.body.totalHappiness < -55f)
				{
					item.body.talker.Talk(Locale.GetCharacter("seecorpsesuicidal"), (Limb)null, true, false);
				}
				else if (item.body.corpsesSeen < 9)
				{
					item.body.talker.Talk(Locale.GetCharacter("seecorpse"), (Limb)null, false, false);
					item.body.eyeScareTime = 4f;
				}
				else
				{
					item.body.talker.Talk(Locale.GetCharacter("seecorpsedesensitized"), (Limb)null, false, false);
				}
			}
		}
	}

	private void OnWillRenderObject()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		foreach (NetBody item in NetPlayer.GetPlayerBodiesInRadius(Vector2.op_Implicit(((Component)this).transform.position), 7f))
		{
			item.body.overrideLookTime = 0.5f;
			item.body.overrideLookPos = Vector2.op_Implicit(((Component)this).transform.position);
			if (item.body.corpsesSeen < 9)
			{
				item.body.eyeScareTime = 0.5f;
			}
		}
	}

	private void OnDestroy()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		if (animalCorpse || !Object.op_Implicit((Object)(object)((Component)this).gameObject))
		{
			return;
		}
		Scene scene = ((Component)this).gameObject.scene;
		BuildingEntity val = default(BuildingEntity);
		if (!((Scene)(ref scene)).isLoaded || !Util.IsWorldGenerated() || (((Component)this).TryGetComponent<BuildingEntity>(ref val) && !(val.health <= 0f)))
		{
			return;
		}
		(NetBody, float) nearestBody = NetBody.GetNearestBody(Vector2.op_Implicit(((Component)this).transform.position), must_be_alive: true, must_be_conscious: true);
		if (nearestBody.Item2 > 100f)
		{
			return;
		}
		foreach (NetBody item in NetPlayer.GetPlayerBodiesInRadius(Vector2.op_Implicit(((Component)this).transform.position), 28f))
		{
			if (KrokoshaScavMultiplayer.is_server && item.body.IsBodyLocal())
			{
				continue;
			}
			bool flag = item.body.attackCooldown > 0f;
			if (item.body.conscious)
			{
				_ = item.body.happiness;
				float num = 5f;
				if (!Util.AccurateRaycastInteractionCheckObstruction(nearestBody.Item1.body, item.body, do_effect: false))
				{
					num *= 0.5f;
				}
				Body body = item.body;
				body.happiness -= num * item.body.desensitizedMult;
				item.body.eyeScareTime = 5f;
				if (flag)
				{
					item.body.talker.Talk(Locale.GetCharacter("breakcorpse"), (Limb)null, false, false);
				}
			}
		}
	}
}
