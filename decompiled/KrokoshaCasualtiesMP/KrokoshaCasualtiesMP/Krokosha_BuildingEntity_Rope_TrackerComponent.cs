using System.Collections.Generic;
using System.Linq;
using LiteNetLib;
using LiteNetLib.Utils;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public class Krokosha_BuildingEntity_Rope_TrackerComponent : Krokosha_SpecialStaticEntityTrackerBase
{
	public const string sandvine_new_id = "Special/sandvinerope";

	public const string climbingropeextended_id = "climbingropeextended";

	public const string mushroomrope_new_id = "Special/mushroomrope";

	public string og_id;

	public bool announce_reliable = true;

	public bool is_ladder => ((Component)this).GetComponent<BuildingEntity>().id == "ladder";

	public bool is_sandvine => ((Component)this).GetComponent<BuildingEntity>().id == "Special/sandvinerope";

	public bool is_mushroomrope => ((Component)this).GetComponent<BuildingEntity>().id == "Special/mushroomrope";

	public bool is_mushroomrope_or_sandvine
	{
		get
		{
			if (!is_sandvine && !is_mushroomrope)
			{
				return is_ladder;
			}
			return true;
		}
	}

	public bool is_climbingropeextended => ((Component)this).GetComponent<BuildingEntity>().id == "climbingropeextended";

	private Vector2 spritescale
	{
		get
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			if (is_mushroomrope_or_sandvine)
			{
				return Vector2.op_Implicit(((Component)this).transform.localScale);
			}
			if (is_climbingropeextended)
			{
				return Vector2.op_Implicit(((Component)this).transform.GetChild(0).localScale);
			}
			return Vector2.one;
		}
		set
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			if (is_mushroomrope_or_sandvine)
			{
				((Component)this).transform.localScale = Vector2.op_Implicit(value);
			}
			if (is_climbingropeextended)
			{
				((Component)this).transform.GetChild(0).localScale = Vector2.op_Implicit(value);
			}
		}
	}

	private float spritey
	{
		get
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			if (is_mushroomrope_or_sandvine)
			{
				return ((Component)this).GetComponent<SpriteRenderer>().size.y;
			}
			_ = is_climbingropeextended;
			return 100f;
		}
		set
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			if (is_mushroomrope_or_sandvine)
			{
				((Component)this).GetComponent<SpriteRenderer>().size = new Vector2(2.5f, value);
			}
			_ = is_climbingropeextended;
		}
	}

	private void Awake()
	{
		REQ = 10091;
	}

	public override void Server_Announce()
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		was_synced = true;
		Climbable component = ((Component)this).GetComponent<Climbable>();
		if (component.points.Count() == 0)
		{
			log.error($"Rope is pointless {((Object)component).name}  {Vector2.op_Implicit(((Component)component).transform.position)}");
			return;
		}
		NetDataWriter writer = Net.CreateWriter(10087);
		writer.Put(Vector2.op_Implicit(((Component)component).transform.position));
		writer.Put(((Component)component).GetComponent<BuildingEntity>().id, oneByteChars: true);
		writer.Put(component.points.ToArray());
		writer.Put(component.downwardsVelocity);
		writer.Put(spritescale);
		writer.Put(spritey);
		Net.Server_SendToClients((DeliveryMethod)((!announce_reliable) ? 4 : 0), in writer, (IEnumerable<knetid>)ServerMain.AllClientIdsExceptHost);
		announce_reliable = true;
	}

	public override bool CheckDist(Vector2 pos)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		Climbable component = ((Component)this).GetComponent<Climbable>();
		if ((Object)(object)component == (Object)null)
		{
			Object.Destroy((Object)(object)this);
			return false;
		}
		if (component.points.Count == 0)
		{
			if (KrokoshaScavMultiplayer.is_client)
			{
				return true;
			}
			log.error($"Rope is pointless {((Component)this).gameObject}");
			return false;
		}
		Vector2 a = component.points[0];
		Vector2 b = component.points[1];
		Vector2 val = KM.v2getClosestPointOnLine(a, b, pos);
		float num = Mathf.Abs(val.x - pos.x);
		float num2 = Mathf.Abs(val.y - pos.y);
		if (num < 64f)
		{
			return num2 < 64f;
		}
		return false;
	}

	[ClientReceiver(10088, true)]
	private static void Client_NoTheresNoRopeFuckOff(knetid _, ref NetDataReader reader)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out Vector2 result);
		Krokosha_BuildingEntity_Rope_TrackerComponent atPos = Krokosha_SpecialStaticEntityTrackerBase.GetAtPos<Krokosha_BuildingEntity_Rope_TrackerComponent>(result);
		if ((Object)(object)atPos != (Object)null)
		{
			Object.Destroy((Object)(object)((Component)atPos).gameObject);
		}
	}

	[ClientReceiver(10087, true)]
	private static void Client_ThereIsARope(knetid _, ref NetDataReader reader)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out Vector2 result);
		reader.Get(out var result2, oneByteChars: true);
		Krokosha_BuildingEntity_Rope_TrackerComponent krokosha_BuildingEntity_Rope_TrackerComponent = Krokosha_SpecialStaticEntityTrackerBase.GetAtPos<Krokosha_BuildingEntity_Rope_TrackerComponent>(result);
		if ((Object)(object)krokosha_BuildingEntity_Rope_TrackerComponent == (Object)null)
		{
			krokosha_BuildingEntity_Rope_TrackerComponent = ComponentHolderProtocol.GetOrAddComponent<Krokosha_BuildingEntity_Rope_TrackerComponent>(Object.Instantiate(Resources.Load(result2), Vector2.op_Implicit(result), Quaternion.identity));
		}
		krokosha_BuildingEntity_Rope_TrackerComponent.was_synced = true;
		Climbable component = ((Component)krokosha_BuildingEntity_Rope_TrackerComponent).GetComponent<Climbable>();
		reader.Get(out Vector2[] result3);
		float downwardsVelocity = default(float);
		reader.Get(ref downwardsVelocity);
		reader.Get(out Vector2 result4);
		float num = default(float);
		reader.Get(ref num);
		component.points = new List<Vector2>(result3);
		component.downwardsVelocity = downwardsVelocity;
		krokosha_BuildingEntity_Rope_TrackerComponent.spritescale = result4;
		krokosha_BuildingEntity_Rope_TrackerComponent.spritey = num;
		SpriteRenderer component2 = ((Component)krokosha_BuildingEntity_Rope_TrackerComponent).GetComponent<SpriteRenderer>();
		if (krokosha_BuildingEntity_Rope_TrackerComponent.is_sandvine)
		{
			component2.color = Color.Lerp(Color.gray, Color.white, Random.value);
			component2.flipX = Random.value > 0.5f;
		}
		else if (krokosha_BuildingEntity_Rope_TrackerComponent.is_ladder)
		{
			component2.size = new Vector2(1.5f, component2.size.y);
		}
		component.CalculateTotalLength();
	}

	[ServerReceiver(10091)]
	private static void Server_IsThereARope(knetid clientId, ref NetDataReader reader)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out Vector2 result);
		Krokosha_BuildingEntity_Rope_TrackerComponent atPos = Krokosha_SpecialStaticEntityTrackerBase.GetAtPos<Krokosha_BuildingEntity_Rope_TrackerComponent>(result);
		if ((Object)(object)atPos == (Object)null)
		{
			NetDataWriter writer = Net.CreateWriter(10088);
			writer.Put(result);
			Net.Server_SendToClients((DeliveryMethod)0, in writer, in clientId);
		}
		else
		{
			atPos.Server_Announce();
		}
	}
}
