using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public static class KM
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2Int ToInt(this Vector2 v2)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2Int(Mathf.RoundToInt(v2.x), Mathf.RoundToInt(v2.y));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2 clampMagn(Vector2 v2, float maxLength)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		float sqrMagnitude = ((Vector2)(ref v2)).sqrMagnitude;
		if (sqrMagnitude > maxLength * maxLength)
		{
			float num = (float)Math.Sqrt(sqrMagnitude);
			return new Vector2(((Vector2)(ref v2))[0] / num * maxLength, ((Vector2)(ref v2))[1] / num * maxLength);
		}
		return v2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2 v2fromAngle(float rad)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2(Mathf.Sin(rad), Mathf.Cos(rad));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2 clampDistance(Vector2 vec2, Vector2 center, float max_magnitude)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		Vector2 v = vec2 - center;
		return center + clampMagn(v, max_magnitude);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float RemapClamped(this float value, float from1, float to1, float from2, float to2)
	{
		return Mathf.Clamp((value - from1) / (to1 - from1) * (to2 - from2) + from2, from2, to2);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsInRange(this int number, int min, int max)
	{
		if (number >= min)
		{
			return number <= max;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsInRange(this float number, float min, float max)
	{
		if (number >= min)
		{
			return number <= max;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsInRange(this double number, double min, double max)
	{
		if (number >= min)
		{
			return number <= max;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 v3v2z(Vector2 a, float z)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3(a.x, a.y, z);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2 v2getClosestPointOnLine(Vector2 a, Vector2 b, Vector2 point)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = point - a;
		Vector2 val2 = b - a;
		float sqrMagnitude = ((Vector2)(ref val2)).sqrMagnitude;
		if (sqrMagnitude < 1E-05f)
		{
			return a;
		}
		float num = Mathf.Clamp01(Vector2.Dot(val, val2) / sqrMagnitude);
		return a + val2 * num;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void v2clampSquare(ref Vector2 vec2, float a)
	{
		if (vec2.x > a)
		{
			vec2.x = a;
		}
		if (vec2.y > a)
		{
			vec2.y = a;
		}
		if (vec2.x < 0f - a)
		{
			vec2.x = 0f - a;
		}
		if (vec2.y < 0f - a)
		{
			vec2.y = 0f - a;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float clamp11(float a)
	{
		return Mathf.Clamp(a, -1f, 1f);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static double clamp11(double a)
	{
		if (a < -1.0)
		{
			a = -1.0;
		}
		else if (a > 1.0)
		{
			a = 1.0;
		}
		return a;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2 normal(in Vector2 from, in Vector2 to, out float magnitude)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = to - from;
		magnitude = ((Vector2)(ref val)).magnitude;
		val = ((!(magnitude > 1E-05f)) ? Vector2.zero : (val / magnitude));
		return val;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2 normal(in Vector2 from, in Vector2 to)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = to - from;
		float magnitude = ((Vector2)(ref val)).magnitude;
		val = ((!(magnitude > 1E-05f)) ? Vector2.zero : (val / magnitude));
		return val;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector2 normal(Vector2 a, out float magnitude)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		magnitude = ((Vector2)(ref a)).magnitude;
		a = ((!(magnitude > 1E-05f)) ? Vector2.zero : (a / magnitude));
		return a;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float dist2dsqr(in Vector2 a, in Vector2 b)
	{
		float num = a.x - b.x;
		float num2 = a.y - b.y;
		return num * num + num2 * num2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool dist2dsqrcheck(in Vector2 a, in Vector2 b, float max)
	{
		float num = a.x - b.x;
		float num2 = a.y - b.y;
		return num * num + num2 * num2 < max * max;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool dist2dsqrcheck_presqr(in Vector2 a, in Vector2 b, float max_squared)
	{
		float num = a.x - b.x;
		float num2 = a.y - b.y;
		return num * num + num2 * num2 < max_squared;
	}

	public static bool dist2dsqrcheck_presqr(in Vector2 a, in Vector2 b, float max_squared, out float actual_dist)
	{
		float num = a.x - b.x;
		float num2 = a.y - b.y;
		actual_dist = num * num + num2 * num2;
		return actual_dist < max_squared;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool dist2dsqrcheck(in Component ba, in Component bb, float max)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)ba == (Object)(object)bb)
		{
			return true;
		}
		Vector2 val = Vector2.op_Implicit(ba.transform.position);
		Vector2 val2 = Vector2.op_Implicit(bb.transform.position);
		float num = val.x - val2.x;
		float num2 = val.y - val2.y;
		return num * num + num2 * num2 < max * max;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool dist2dsqrcheck(in Body ba, in SyncInfo bb, float max)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = Vector2.op_Implicit(((Component)ba).transform.position);
		Vector2 val2 = Vector2.op_Implicit(bb.go.transform.position);
		float num = val.x - val2.x;
		float num2 = val.y - val2.y;
		return num * num + num2 * num2 < max * max;
	}
}
