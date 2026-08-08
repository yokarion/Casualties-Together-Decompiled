using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public struct Color24 : INetSerializeByMemcpy
{
	public byte r;

	public byte g;

	public byte b;

	public byte this[int index]
	{
		get
		{
			return index switch
			{
				0 => r, 
				1 => g, 
				2 => b, 
				_ => throw new IndexOutOfRangeException("Invalid Color24 index(" + index + ")!"), 
			};
		}
		set
		{
			switch (index)
			{
			case 0:
				r = value;
				break;
			case 1:
				g = value;
				break;
			case 2:
				b = value;
				break;
			default:
				throw new IndexOutOfRangeException("Invalid Color24 index(" + index + ")!");
			}
		}
	}

	public static Color24 white
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return new Color24(byte.MaxValue, byte.MaxValue, byte.MaxValue);
		}
	}

	public static Color24 black
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return new Color24(0, 0, 0);
		}
	}

	public Color24(byte r, byte g, byte b)
	{
		this.r = r;
		this.g = g;
		this.b = b;
	}

	public static implicit operator Color24(Color32 c)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return new Color24(c.r, c.g, c.b);
	}

	public static implicit operator Color24(Color c)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		return new Color24((byte)Math.Round(Mathf.Clamp01(c.r) * 255f), (byte)Math.Round(Mathf.Clamp01(c.g) * 255f), (byte)Math.Round(Mathf.Clamp01(c.b) * 255f));
	}

	public static implicit operator Color(Color24 c)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		return new Color((float)(int)c.r / 255f, (float)(int)c.g / 255f, (float)(int)c.b / 255f);
	}

	public static implicit operator Color32(Color24 c)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		return new Color32(c.r, c.g, c.b, byte.MaxValue);
	}

	public Color ToColorWithAlpha(float a)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		return new Color((float)(int)r / 255f, (float)(int)g / 255f, (float)(int)b / 255f, a);
	}

	public Color ToColorWithAlpha(byte a)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		return new Color((float)(int)r / 255f, (float)(int)g / 255f, (float)(int)b / 255f, (float)(int)a / 255f);
	}

	public static bool operator ==(Color24 lhs, Color24 rhs)
	{
		if (lhs.r == rhs.r && lhs.g == rhs.g)
		{
			return lhs.b == rhs.b;
		}
		return false;
	}

	public static bool operator !=(Color24 lhs, Color24 rhs)
	{
		return !(lhs == rhs);
	}

	public override bool Equals(object other)
	{
		if (!(other is Color24))
		{
			return false;
		}
		return this == (Color24)other;
	}

	public int Sum()
	{
		return r + g + b;
	}

	public int Hash()
	{
		return (r << 16) | (g << 8) | b;
	}

	public override int GetHashCode()
	{
		return Hash();
	}

	public string ToHex(bool with_hashtag = true)
	{
		string text = string.Format(CultureInfo.InvariantCulture.NumberFormat, "{0:X2}{1:X2}{2:X2}", r, g, b);
		if (with_hashtag)
		{
			text = "#" + text;
		}
		return text;
	}

	public static bool TryParseHex(string hex, out Color24 color)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (!hex.StartsWith("#"))
		{
			hex = "#" + hex;
		}
		Color val = default(Color);
		if (ColorUtility.TryParseHtmlString(hex, ref val))
		{
			color = val;
			return true;
		}
		color = default(Color24);
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString()
	{
		return "RGB(" + r + ", " + g + ", " + b + ")";
	}
}
