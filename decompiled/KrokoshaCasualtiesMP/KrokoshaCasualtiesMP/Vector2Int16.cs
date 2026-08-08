using System;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public struct Vector2Int16 : INetSerializeByMemcpy
{
	public short x;

	public short y;

	public Vector2Int16(short x, short y)
	{
		this.x = x;
		this.y = y;
	}

	public static implicit operator Vector2(Vector2Int16 v2i16)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2((float)v2i16.x, (float)v2i16.y);
	}

	public static explicit operator Vector2Int16(Vector2Int v2)
	{
		return new Vector2Int16
		{
			x = (short)((Vector2Int)(ref v2)).x,
			y = (short)((Vector2Int)(ref v2)).y
		};
	}

	public static explicit operator Vector2Int16(Vector2 v2)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2Int16
		{
			x = (short)Math.Round(v2.x),
			y = (short)Math.Round(v2.y)
		};
	}

	public static explicit operator Vector2Int16(Vector3 v3)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2Int16
		{
			x = (short)Math.Round(v3.x),
			y = (short)Math.Round(v3.y)
		};
	}

	public static bool operator ==(Vector2Int16 a, Vector2Int16 b)
	{
		if (a.x == b.x)
		{
			return a.y == b.y;
		}
		return false;
	}

	public static bool operator !=(Vector2Int16 a, Vector2Int16 b)
	{
		if (a.x == b.x)
		{
			return a.y != b.y;
		}
		return true;
	}

	public override bool Equals(object other)
	{
		if (!(other is Vector2Int16))
		{
			return false;
		}
		return this == (Vector2Int16)other;
	}

	public override int GetHashCode()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return ((object)(Vector2)this/*cast due to constrained. prefix*/).GetHashCode();
	}

	public override string ToString()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return ((object)(Vector2)this/*cast due to constrained. prefix*/).ToString();
	}
}
