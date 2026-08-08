using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public class Krokosha_SpecialStaticEntityTrackerBase : MonoBehaviour
{
	protected ushort REQ = 10169;

	public bool was_synced;

	private float checktimer = 1f;

	private Vector2 pos => Vector2.op_Implicit(((Component)this).transform.position);

	private void Start()
	{
		checktimer = Random.value;
	}

	private void FixedUpdate()
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		if (!KrokoshaScavMultiplayer.network_system_is_running)
		{
			Object.Destroy((Object)(object)this);
		}
		else
		{
			if (was_synced)
			{
				return;
			}
			checktimer -= Time.fixedDeltaTime;
			if (!(checktimer < 0f))
			{
				return;
			}
			checktimer = Random.value * 0.4f;
			if (!SharedMain.local_world_is_generated || SharedMain.LastWorldgenFinishTime < 5.0)
			{
				return;
			}
			if (KrokoshaScavMultiplayer.is_client)
			{
				if (ClientMain.server_is_generating_world || !CheckDist(Vector2.op_Implicit(((Component)PlayerCamera.main).transform.position)))
				{
					return;
				}
			}
			else
			{
				NetPlayer netPlayer = null;
				foreach (NetPlayer value in NetPlayer.BodyToPlayerDict.Values)
				{
					if (value.server_plrstate.is_loaded_in && CheckDist(value.pos))
					{
						netPlayer = value;
						break;
					}
				}
				if ((Object)(object)netPlayer == (Object)null)
				{
					return;
				}
			}
			if (KrokoshaScavMultiplayer.is_client)
			{
				NetDataWriter writer = Net.CreateWriter(REQ);
				writer.Put(Vector2.op_Implicit(((Component)this).transform.position));
				Net.Client_Send((DeliveryMethod)0, in writer);
			}
			else
			{
				Server_Announce();
			}
			was_synced = true;
		}
	}

	public virtual bool CheckDist(Vector2 pos)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		return KM.dist2dsqrcheck_presqr(in pos, Vector2.op_Implicit(((Component)this).transform.position), 4096f);
	}

	public virtual void Server_Announce()
	{
		if (!was_synced)
		{
			was_synced = true;
		}
	}

	public static T GetAtPos<T>(Vector2 pos) where T : Krokosha_SpecialStaticEntityTrackerBase
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		T[] array = Object.FindObjectsOfType<T>();
		foreach (T val in array)
		{
			if (Vector2.op_Implicit(((Component)val).transform.position) == pos)
			{
				return val;
			}
		}
		return null;
	}
}
