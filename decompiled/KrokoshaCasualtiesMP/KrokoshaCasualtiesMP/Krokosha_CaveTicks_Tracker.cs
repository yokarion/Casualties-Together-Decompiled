using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public class Krokosha_CaveTicks_Tracker : Krokosha_SpecialStaticEntityTrackerBase
{
	private void Awake()
	{
		REQ = 10096;
	}

	public override void Server_Announce()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		was_synced = true;
		NetDataWriter writer = Net.CreateWriter(10093);
		writer.Put(Vector2.op_Implicit(((Component)this).transform.position));
		Net.Server_SendToClients((DeliveryMethod)0, in writer, (IEnumerable<knetid>)ServerMain.AllClientIdsExceptHost);
	}

	[ClientReceiver(10094, true)]
	private static void Client_NoTheresNoSandvineFuckOff(knetid _, ref NetDataReader reader)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out Vector2 result);
		Krokosha_CaveTicks_Tracker atPos = Krokosha_SpecialStaticEntityTrackerBase.GetAtPos<Krokosha_CaveTicks_Tracker>(result);
		if ((Object)(object)atPos != (Object)null)
		{
			Object.Destroy((Object)(object)((Component)atPos).gameObject);
		}
	}

	[ClientReceiver(10093, true)]
	private static void Client_Krokosha_CaveTicks_Tracker_announce(knetid _, ref NetDataReader reader)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		reader.Get(out Vector2 result);
		if ((Object)(object)Krokosha_SpecialStaticEntityTrackerBase.GetAtPos<Krokosha_CaveTicks_Tracker>(result) == (Object)null)
		{
			ComponentHolderProtocol.GetOrAddComponent<Krokosha_CaveTicks_Tracker>((Object)(GameObject)Object.Instantiate(Resources.Load("CaveTicks"), Vector2.op_Implicit(result), Quaternion.identity)).was_synced = true;
		}
	}

	[ServerReceiver(10096)]
	private static void Server_Krokosha_CaveTicks_Tracker_request(knetid clientId, ref NetDataReader reader)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out Vector2 result);
		Krokosha_CaveTicks_Tracker atPos = Krokosha_SpecialStaticEntityTrackerBase.GetAtPos<Krokosha_CaveTicks_Tracker>(result);
		if ((Object)(object)atPos == (Object)null)
		{
			NetDataWriter writer = Net.CreateWriter(10094);
			writer.Put(result);
			Net.Server_SendToClients((DeliveryMethod)0, in writer, in clientId);
		}
		else
		{
			atPos.Server_Announce();
		}
	}
}
