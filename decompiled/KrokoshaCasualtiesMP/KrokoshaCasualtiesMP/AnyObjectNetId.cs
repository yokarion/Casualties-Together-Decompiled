using System.Linq;
using KrokoshaCasualtiesUtils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public struct AnyObjectNetId : INetSerializeByMemcpy
{
	public byte type;

	public knetid netId;

	public byte data1;

	public bool IsNothing()
	{
		return type == 0;
	}

	public override string ToString()
	{
		string text = "NONE";
		if (type != 0)
		{
			if (type == 1)
			{
				text = "NetBody";
			}
			else if (type == 2)
			{
				text = "Player";
			}
			else if (type == 3)
			{
				text = "SyncInfo";
			}
			else if (type == 4)
			{
				text = "Limb";
			}
		}
		string text2 = $"type:{type}, netId:{netId}, data1:{data1}";
		text2 = (TryGetPlayer(out var plr) ? ((object)plr).ToString() : (TryGetLimb(out var limb) ? $"{limb.shortName}, {limb.GetNetBody()}" : (TryGetNetBody(out var nb) ? ((object)nb).ToString() : ((!TryGetSyncInfo(out var si)) ? ("NOT FOUND: " + text2) : si.ToString()))));
		return "AnyObjectNetId( " + text + " / " + text2 + " )";
	}

	public static bool TryGetNetIdFromGameObject(GameObject obj, out AnyObjectNetId ni)
	{
		ni = GetNetIdFromGameObject(obj);
		if (ni.type != 0)
		{
			return true;
		}
		return false;
	}

	public static AnyObjectNetId GetNetIdFromGameObject(GameObject obj)
	{
		Limb val = default(Limb);
		NetBody nb = default(NetBody);
		if (obj.TryGetComponent<Limb>(ref val) && val.TryGetNetBody(out nb))
		{
			return new AnyObjectNetId(val);
		}
		if (obj.TryGetComponent<NetBody>(ref nb))
		{
			return new AnyObjectNetId(nb);
		}
		KrokoshaScavMultiGameObjectNetworkTracker krokoshaScavMultiGameObjectNetworkTracker = default(KrokoshaScavMultiGameObjectNetworkTracker);
		if (obj.TryGetComponent<KrokoshaScavMultiGameObjectNetworkTracker>(ref krokoshaScavMultiGameObjectNetworkTracker) && krokoshaScavMultiGameObjectNetworkTracker.syncinfo != null)
		{
			return new AnyObjectNetId(krokoshaScavMultiGameObjectNetworkTracker.syncinfo);
		}
		NetPlayer nb2 = default(NetPlayer);
		if (obj.TryGetComponent<NetPlayer>(ref nb2))
		{
			return new AnyObjectNetId(nb2);
		}
		return new AnyObjectNetId();
	}

	public AnyObjectNetId()
	{
		data1 = 0;
		type = 0;
		netId = (ushort)0;
	}

	public AnyObjectNetId(NetBody nb)
	{
		data1 = 0;
		type = 1;
		netId = nb.netId;
	}

	public AnyObjectNetId(NetPlayer nb)
	{
		data1 = 0;
		type = 2;
		netId = nb.clientId;
	}

	public AnyObjectNetId(SyncInfo si)
	{
		data1 = 0;
		type = 3;
		netId = si.syncId;
	}

	public AnyObjectNetId(Limb li)
	{
		type = 4;
		netId = li.GetNetBody().netId;
		data1 = li.GetIndex();
	}

	public bool TryGetLimb(out Limb limb)
	{
		if (type == 4 && NetBody.TryGetNetBodyFromId(netId, out var nb) && data1 < nb.body.limbs.Count())
		{
			limb = nb.body.limbs[data1];
			return true;
		}
		limb = null;
		return false;
	}

	public bool TryGetNetBodyAndLimb(out NetBody nb, out Limb limb)
	{
		if (type == 4 && NetBody.TryGetNetBodyFromId(netId, out nb) && data1 < nb.body.limbs.Count())
		{
			limb = nb.body.limbs[data1];
			return true;
		}
		nb = null;
		limb = null;
		return false;
	}

	public bool TryGetNetBody(out NetBody nb)
	{
		if (type == 1 && NetBody.TryGetNetBodyFromId(netId, out nb))
		{
			return true;
		}
		if (type == 2 && NetPlayer.TryGetNetPlayerAndNetBodyFromClientId(netId, out var _, out nb))
		{
			return true;
		}
		if (type == 4 && NetBody.TryGetNetBodyFromId(netId, out nb))
		{
			return true;
		}
		nb = null;
		return false;
	}

	public bool TryGetPlayer(out NetPlayer plr)
	{
		if (type == 2 && NetPlayer.TryGetPlayerFromClientId(netId, out plr))
		{
			return true;
		}
		if (type == 1 && NetBody.TryGetNetBodyFromId(netId, out var nb) && nb.is_player)
		{
			plr = nb.player;
			return true;
		}
		if (type == 4 && NetBody.TryGetNetBodyFromId(netId, out var nb2) && nb2.is_player)
		{
			plr = nb2.player;
			return true;
		}
		plr = null;
		return false;
	}

	public bool TryGetSyncInfo(out SyncInfo si)
	{
		if (type == 3 && NetObjectRegistry.TryGetSyncInfo(netId, out si))
		{
			return true;
		}
		si = null;
		return false;
	}

	public bool TryGetGameObject(out GameObject go)
	{
		if (type != 0)
		{
			if (TryGetLimb(out var limb))
			{
				go = ((Component)limb).gameObject;
				return true;
			}
			if (TryGetNetBody(out var nb))
			{
				go = ((Component)nb).gameObject;
				return true;
			}
			if (TryGetSyncInfo(out var si))
			{
				go = si.go;
				return true;
			}
		}
		go = null;
		return false;
	}

	public bool TryGetTalker(out Talker talker)
	{
		if (TryGetGameObject(out var go))
		{
			return Util.TryGetTalkerOnObject(go, out talker);
		}
		talker = null;
		return false;
	}

	public static bool operator ==(AnyObjectNetId lhs, AnyObjectNetId rhs)
	{
		if ((ushort)lhs.netId == (ushort)rhs.netId && lhs.type == rhs.type)
		{
			return lhs.data1 == rhs.data1;
		}
		return false;
	}

	public static bool operator !=(AnyObjectNetId lhs, AnyObjectNetId rhs)
	{
		return !(lhs == rhs);
	}

	public override bool Equals(object other)
	{
		Component val = (Component)((other is Component) ? other : null);
		if (val != null)
		{
			if (TryGetGameObject(out var go))
			{
				return (Object)(object)go == (Object)(object)val.gameObject;
			}
			return false;
		}
		if (!(other is AnyObjectNetId))
		{
			return false;
		}
		return this == (AnyObjectNetId)other;
	}
}
