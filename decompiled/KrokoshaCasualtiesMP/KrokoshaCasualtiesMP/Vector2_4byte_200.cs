using System;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public struct Vector2_4byte_200 : INetSerializeByMemcpy
{
	public short x;

	public short y;

	private const float MAPSIZE = 200f;

	private const float SCALE = 163.835f;

	private const float INVSCALE = 0.0061037014f;

	public const float PRECISION = 0.0061037014f;

	public const float PRECISION2 = 0.012207403f;

	public Vector2_4byte_200(short x, short y)
	{
		this.x = x;
		this.y = y;
	}

	public Vector2_4byte_200(int x, int y)
	{
		this.x = (short)((float)x * 163.835f);
		this.y = (short)((float)y * 163.835f);
	}

	public Vector2_4byte_200(float x, float y)
	{
		this.x = (short)(x * 163.835f);
		this.y = (short)(y * 163.835f);
	}

	public Vector2_4byte_200()
	{
		x = 0;
		y = 0;
	}

	public static Vector2_4byte_200 Unclamped(Vector2 vec)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2_4byte_200((short)(vec.x * 163.835f), (short)(vec.y * 163.835f));
	}

	public static implicit operator Vector2(Vector2_4byte_200 compressed)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2((float)compressed.x * 0.0061037014f, (float)compressed.y * 0.0061037014f);
	}

	public static implicit operator Vector2_4byte_200(Vector2 vec)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		short num = (short)Math.Round(Mathf.Clamp(vec.x, -200f, 200f) * 163.835f);
		short num2 = (short)Math.Round(Mathf.Clamp(vec.y, -200f, 200f) * 163.835f);
		Vector2_4byte_200 result = new Vector2_4byte_200();
		result.x = num;
		result.y = num2;
		return result;
	}

	public override string ToString()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return ((object)(Vector2)this/*cast due to constrained. prefix*/).ToString();
	}

	public static bool operator ==(Vector2_4byte_200 a, Vector2_4byte_200 b)
	{
		if (a.x == b.x)
		{
			return a.y == b.y;
		}
		return false;
	}

	public static bool operator !=(Vector2_4byte_200 a, Vector2_4byte_200 b)
	{
		if (a.x == b.x)
		{
			return a.y != b.y;
		}
		return true;
	}

	public override bool Equals(object other)
	{
		if (!(other is Vector2_4byte_200))
		{
			return false;
		}
		return this == (Vector2_4byte_200)other;
	}

	public override int GetHashCode()
	{
		return ((ushort)x << 16) | (ushort)y;
	}
}
