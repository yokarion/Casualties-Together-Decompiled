using System.Linq;
using KrokoshaCasualtiesUtils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public struct LimbNetId : INetSerializeByMemcpy
{
	public knetid bodyNetId;

	public byte limbId;

	public LimbNetId()
	{
		bodyNetId = default(knetid);
		limbId = 0;
	}

	public LimbNetId(Limb limb)
	{
		bodyNetId = ((Component)limb.body).GetComponent<NetBody>().netId;
		limbId = Util.GetLimbIndex(limb);
	}

	public Limb GetLimb()
	{
		return NetBody.GetNetBodyFromId(bodyNetId).body.limbs[limbId];
	}

	public bool TryGetNetBodyAndLimb(out NetBody nb, out Limb limb)
	{
		if (NetBody.TryGetNetBodyFromId(bodyNetId, out nb))
		{
			limb = nb.body.limbs[limbId];
			return true;
		}
		nb = null;
		limb = null;
		return false;
	}

	public bool TryGetNetBodyAndLimbSafe(out NetBody nb, out Limb limb)
	{
		if (NetBody.TryGetNetBodyFromId(bodyNetId, out nb) && limbId < nb.body.limbs.Count())
		{
			limb = nb.body.limbs[limbId];
			return true;
		}
		nb = null;
		limb = null;
		return false;
	}

	public override string ToString()
	{
		if (TryGetNetBodyAndLimbSafe(out var nb, out var _))
		{
			return $"NetLimb( {nb}, limbId: {limbId}, name: {nb.body.limbs[limbId].fullName})";
		}
		if (Util.TryGetLocalBody(out var body) && limbId < body.limbs.Count())
		{
			return $"NetLimb( NOT FOUND: bodyNetId: {bodyNetId}, id: {limbId}, name: {body.limbs[limbId].fullName})";
		}
		return $"NetLimb( NOT FOUND: bodyNetId: {bodyNetId}, id: {limbId})";
	}
}
