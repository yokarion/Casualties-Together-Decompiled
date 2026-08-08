using UnityEngine;

namespace KrokoshaCasualtiesMP;

public static class Util_MPExtensions
{
	public static NetBody GetNetBody(this Limb limb)
	{
		return ((Component)limb.body).GetComponent<NetBody>();
	}

	public static NetBody GetNetBody(this Body body)
	{
		return ((Component)body).GetComponent<NetBody>();
	}

	public static bool TryGetNetBody(this Limb limb, out NetBody nb)
	{
		nb = limb.GetNetBody();
		return (Object)(object)nb != (Object)null;
	}

	public static bool TryGetNetBody(this Body body, out NetBody nb)
	{
		nb = body.GetNetBody();
		return (Object)(object)nb != (Object)null;
	}

	public static bool TryGetNetPlayer(this Limb limb, out NetPlayer plr)
	{
		return limb.body.TryGetNetPlayer(out plr);
	}

	public static bool TryGetNetPlayer(this Body body, out NetPlayer plr)
	{
		if (body.TryGetNetBody(out var nb) && nb.is_player)
		{
			plr = nb.plr;
			return true;
		}
		plr = null;
		return false;
	}
}
