using KrokoshaCasualtiesUtils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public class KrokoshaSpiderTrackerComponent : KrokoshaNetworkComponentTracker<SpiderHandler>
{
	protected override void TrackerAwake()
	{
	}

	private void Start()
	{
	}

	private void LateUpdate()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		if (!Item_Update_MultiplayerPatch.MPinSimRange(Vector2.op_Implicit(((Component)this).transform.position)))
		{
			return;
		}
		KrokoshaScavMultiGameObjectNetworkTracker krokoshaScavMultiGameObjectNetworkTracker = default(KrokoshaScavMultiGameObjectNetworkTracker);
		if (!((Component)this).TryGetComponent<KrokoshaScavMultiGameObjectNetworkTracker>(ref krokoshaScavMultiGameObjectNetworkTracker) && (Object)(object)((Component)this).gameObject != (Object)null)
		{
			NetObjectRegistry.NewGO(((Component)this).gameObject);
		}
		else
		{
			if (!krokoshaScavMultiGameObjectNetworkTracker.is_within_anyones_view)
			{
				return;
			}
			foreach (NetBody item in NetBody.GetBodiesInRadius(Vector2.op_Implicit(((Component)base.og).transform.position), 20f))
			{
				if (item.is_player && !Util.IsBodyLocal(item.body) && item.body.conscious)
				{
					NetObjectRegistry.Server_QueueSyncForOne(krokoshaScavMultiGameObjectNetworkTracker.syncinfo, item.player);
				}
			}
		}
	}

	public void AnimalDeath()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		foreach (NetBody item in NetBody.GetBodiesInRadius(Vector2.op_Implicit(((Component)base.og).transform.position), 32f))
		{
			if (!Util.IsBodyLocal(item.body))
			{
				item.body.skills.AddExp(1, base.og.maxHealth / 100f);
			}
		}
	}
}
