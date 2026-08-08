using System;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public struct Compressed11Vec2 : INetSerializeByMemcpy
{
	public sbyte x;

	public sbyte y;

	public Compressed11Vec2(sbyte x, sbyte y)
	{
		this.x = x;
		this.y = y;
	}

	public static implicit operator Vector2(Compressed11Vec2 compressed)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2((float)compressed.x / 127f, (float)compressed.y / 127f);
	}

	public static explicit operator Compressed11Vec2(Vector2 vec)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		sbyte b = (sbyte)Math.Round(KM.clamp11(vec.x) * 127f);
		sbyte b2 = (sbyte)Math.Round(KM.clamp11(vec.y) * 127f);
		return new Compressed11Vec2
		{
			x = b,
			y = b2
		};
	}

	public static bool operator ==(Compressed11Vec2 a, Compressed11Vec2 b)
	{
		if (a.x == b.x)
		{
			return a.y == b.y;
		}
		return false;
	}

	public static bool operator !=(Compressed11Vec2 a, Compressed11Vec2 b)
	{
		if (a.x == b.x)
		{
			return a.y != b.y;
		}
		return true;
	}

	public override bool Equals(object other)
	{
		if (!(other is Compressed11Vec2 compressed11Vec))
		{
			return false;
		}
		if (x == compressed11Vec.x)
		{
			return y == compressed11Vec.y;
		}
		return false;
	}

	public override string ToString()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return ((object)(Vector2)this/*cast due to constrained. prefix*/).ToString();
	}
}
