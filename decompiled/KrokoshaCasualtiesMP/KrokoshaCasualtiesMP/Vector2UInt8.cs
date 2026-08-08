using UnityEngine;

namespace KrokoshaCasualtiesMP;

public struct Vector2UInt8 : INetSerializeByMemcpy
{
	public byte x;

	public byte y;

	public static implicit operator Vector2(Vector2UInt8 v2ui8)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2((float)(int)v2ui8.x, (float)(int)v2ui8.y);
	}

	public Vector2UInt8(byte x, byte y)
	{
		this.x = x;
		this.y = y;
	}

	public Vector2UInt8(int x, int y)
	{
		this.x = (byte)x;
		this.y = (byte)y;
	}

	public static explicit operator Vector2UInt8(Vector2Int v2)
	{
		return new Vector2UInt8
		{
			x = (byte)((Vector2Int)(ref v2)).x,
			y = (byte)((Vector2Int)(ref v2)).y
		};
	}

	public static explicit operator Vector2UInt8(Vector2 v2)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2UInt8
		{
			x = (byte)v2.x,
			y = (byte)v2.y
		};
	}

	public static explicit operator Vector2UInt8(Vector3 v3)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2UInt8
		{
			x = (byte)v3.x,
			y = (byte)v3.y
		};
	}

	public static bool operator ==(Vector2UInt8 a, Vector2UInt8 b)
	{
		if (a.x == b.x)
		{
			return a.y == b.y;
		}
		return false;
	}

	public static bool operator !=(Vector2UInt8 a, Vector2UInt8 b)
	{
		if (a.x == b.x)
		{
			return a.y != b.y;
		}
		return true;
	}

	public override bool Equals(object other)
	{
		if (!(other is Vector2UInt8))
		{
			return false;
		}
		return this == (Vector2UInt8)other;
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
