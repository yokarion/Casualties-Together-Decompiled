using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace KrokoshaCasualtiesUtils;

public static class Util_MiscSystemExtensions
{
	public static void CopyWorldMatrix(Transform target, Transform source)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		Matrix4x4 localToWorldMatrix = source.localToWorldMatrix;
		Matrix4x4 val = (((Object)(object)target.parent != (Object)null) ? (target.parent.worldToLocalMatrix * localToWorldMatrix) : localToWorldMatrix);
		Vector3 localPosition = Vector4.op_Implicit(((Matrix4x4)(ref val)).GetColumn(3));
		Quaternion localRotation = Quaternion.LookRotation(Vector4.op_Implicit(((Matrix4x4)(ref val)).GetColumn(2)), Vector4.op_Implicit(((Matrix4x4)(ref val)).GetColumn(1)));
		Vector4 column = ((Matrix4x4)(ref val)).GetColumn(0);
		float magnitude = ((Vector4)(ref column)).magnitude;
		column = ((Matrix4x4)(ref val)).GetColumn(1);
		float magnitude2 = ((Vector4)(ref column)).magnitude;
		column = ((Matrix4x4)(ref val)).GetColumn(2);
		Vector3 localScale = default(Vector3);
		((Vector3)(ref localScale))._002Ector(magnitude, magnitude2, ((Vector4)(ref column)).magnitude);
		target.localPosition = localPosition;
		target.localRotation = localRotation;
		target.localScale = localScale;
	}

	public static Vector2 Abs(this Vector2 v2)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2(Math.Abs(v2.x), Math.Abs(v2.y));
	}

	public static Vector2 Max(this Vector2 v2, in Vector2 other)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2(Math.Max(v2.x, other.x), Math.Max(v2.y, other.y));
	}

	public static Vector2 Min(this Vector2 v2, in Vector2 other)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2(Math.Min(v2.x, other.x), Math.Min(v2.y, other.y));
	}

	public static T GetValueSafe<S, T>(this Dictionary<S, T> dictionary, S key, T fallback)
	{
		if (dictionary.TryGetValue(key, out var value))
		{
			return value;
		}
		return fallback;
	}

	public static (IEnumerable<T> True, IEnumerable<T> False) Partition<T>(this IEnumerable<T> source, Func<T, bool> predicate)
	{
		List<T> list = new List<T>();
		List<T> list2 = new List<T>();
		foreach (T item in source)
		{
			if (predicate(item))
			{
				list.Add(item);
			}
			else
			{
				list2.Add(item);
			}
		}
		return (True: list, False: list2);
	}

	public static byte[] ToBytes(this BitArray bits)
	{
		byte[] array = new byte[(bits.Length + 7) / 8];
		bits.CopyTo(array, 0);
		return array;
	}

	public static string ToStringInvariant(this object obj)
	{
		string text = null;
		CultureInfo currentCulture = CultureInfo.CurrentCulture;
		try
		{
			CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
			return obj.ToString();
		}
		finally
		{
			CultureInfo.CurrentCulture = currentCulture;
		}
	}

	public static bool IsInRange(this RangeF r, in float number)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		if (number >= r.min)
		{
			return number <= r.max;
		}
		return false;
	}

	public static float Center(this RangeF r)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return Mathf.LerpUnclamped(r.min, r.max, 0.5f);
	}

	public static RangeF Widen(this RangeF r, in float number)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		RangeF result = default(RangeF);
		((RangeF)(ref result))._002Ector(r.min, r.max);
		result.min -= number;
		result.max += number;
		return result;
	}

	public static Rect ShrinkBorder(this Rect r, in float number)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		Rect result = default(Rect);
		((Rect)(ref result))._002Ector(r);
		Vector2 val = Vector2.one * number;
		((Rect)(ref result)).min = ((Rect)(ref result)).min + val;
		((Rect)(ref result)).max = ((Rect)(ref result)).max - val;
		return result;
	}

	public static int StableHash(this string s)
	{
		if (s == null)
		{
			return 0;
		}
		int num = -2128831035;
		byte[] bytes = Encoding.UTF8.GetBytes(s);
		foreach (byte b in bytes)
		{
			num ^= b;
			num *= 16777619;
		}
		return num;
	}

	public static byte[] ToByteArray(this string s)
	{
		byte[] array = new byte[s.Length];
		for (int i = 0; i < s.Length; i++)
		{
			array[i] = (byte)s[i];
		}
		return array;
	}

	public static string ToStringFromCharBytes(this byte[] b)
	{
		StringBuilder stringBuilder = new StringBuilder(b.Length);
		foreach (byte value in b)
		{
			stringBuilder.Append((char)value);
		}
		return stringBuilder.ToString();
	}

	public static void ShuffleRandom<T>(this IList<T> array)
	{
		for (int num = array.Count - 1; num > 0; num--)
		{
			int index = Random.Range(0, num + 1);
			T value = array[num];
			array[num] = array[index];
			array[index] = value;
		}
	}

	public static Texture2D Clone(this Texture2D s)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Expected O, but got Unknown
		Texture2D val = new Texture2D(((Texture)s).width, ((Texture)s).height, s.format, ((Texture)s).mipmapCount > 1);
		val.SetPixels(s.GetPixels());
		val.Apply();
		return val;
	}

	public static Sprite Clone(this Sprite spr)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		return Sprite.Create(spr.texture.Clone(), spr.rect, spr.pivot, spr.pixelsPerUnit);
	}

	public static void TextureBW(this Texture2D t)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		Color[] pixels = t.GetPixels();
		for (int i = 0; i < pixels.Length; i++)
		{
			pixels[i] = pixels[i].ColorBW();
		}
		t.SetPixels(pixels);
	}

	public static Color ColorBW(this Color t)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		float num = Mathf.Max(new float[3] { t.r, t.g, t.b });
		return new Color(num, num, num, t.a);
	}
}
