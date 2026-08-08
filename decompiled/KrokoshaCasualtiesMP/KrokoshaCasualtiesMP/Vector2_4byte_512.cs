using System;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public struct Vector2_4byte_512 : INetSerializeByMemcpy
{
	public short x;

	public short y;

	private const float MAPSIZE = 512f;

	private const float SCALE = 63.998047f;

	private const float INVSCALE = 0.015625477f;

	public const float PRECISION = 0.015625477f;

	public const float PRECISION3 = 0.04687643f;

	public const float PRECISION3_sqr = 0.0021973997f;

	public Vector2_4byte_512(short x, short y)
	{
		this.x = x;
		this.y = y;
	}

	public Vector2_4byte_512(int x, int y)
	{
		this.x = (short)((float)x * 63.998047f);
		this.y = (short)((float)y * 63.998047f);
	}

	public Vector2_4byte_512(float x, float y)
	{
		this.x = (short)(x * 63.998047f);
		this.y = (short)(y * 63.998047f);
	}

	public Vector2_4byte_512()
	{
		x = 0;
		y = 0;
	}

	public static Vector2_4byte_512 Unclamped(Vector2 vec)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2_4byte_512((short)(vec.x * 63.998047f), (short)(vec.y * 63.998047f));
	}

	public static implicit operator Vector3(Vector2_4byte_512 compressed)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3((float)compressed.x * 0.015625477f, (float)compressed.y * 0.015625477f);
	}

	public static implicit operator Vector2(Vector2_4byte_512 compressed)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2((float)compressed.x * 0.015625477f, (float)compressed.y * 0.015625477f);
	}

	public static implicit operator Vector2_4byte_512(Vector2 vec)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		short num = (short)Math.Round(Mathf.Clamp(vec.x, -512f, 512f) * 63.998047f);
		short num2 = (short)Math.Round(Mathf.Clamp(vec.y, -512f, 512f) * 63.998047f);
		Vector2_4byte_512 result = new Vector2_4byte_512();
		result.x = num;
		result.y = num2;
		return result;
	}

	public static implicit operator Vector2_4byte_512(Vector3 vec)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		short num = (short)Math.Round(Mathf.Clamp(vec.x, -512f, 512f) * 63.998047f);
		short num2 = (short)Math.Round(Mathf.Clamp(vec.y, -512f, 512f) * 63.998047f);
		Vector2_4byte_512 result = new Vector2_4byte_512();
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

	public static bool operator ==(Vector2_4byte_512 a, Vector2_4byte_512 b)
	{
		if (a.x == b.x)
		{
			return a.y == b.y;
		}
		return false;
	}

	public static bool operator !=(Vector2_4byte_512 a, Vector2_4byte_512 b)
	{
		if (a.x == b.x)
		{
			return a.y != b.y;
		}
		return true;
	}

	public override bool Equals(object other)
	{
		if (!(other is Vector2_4byte_512))
		{
			return false;
		}
		return this == (Vector2_4byte_512)other;
	}

	public override int GetHashCode()
	{
		return ((ushort)x << 16) | (ushort)y;
	}
}
