using System;
using LiteNetLib.Utils;

namespace KrokoshaCasualtiesMP;

public struct Bitset8 : INetSerializable
{
	public byte bayt;

	public bool this[int position]
	{
		get
		{
			return Test(position);
		}
		set
		{
			Set(position, value);
		}
	}

	public void Deserialize(NetDataReader reader)
	{
		reader.Get(ref bayt);
	}

	public void Serialize(NetDataWriter writer)
	{
		writer.Put(bayt);
	}

	public Bitset8(byte bayt)
	{
		this.bayt = bayt;
	}

	public Bitset8(params bool[] bits)
	{
		bayt = 0;
		for (int i = 0; i < bits.Length; i++)
		{
			this[i] = bits[i];
		}
	}

	public static bool operator ==(Bitset8 a, Bitset8 b)
	{
		return a.bayt == b.bayt;
	}

	public static bool operator !=(Bitset8 a, Bitset8 b)
	{
		return !(a == b);
	}

	public static Bitset8 operator &(Bitset8 a, Bitset8 b)
	{
		return new Bitset8((byte)(a.bayt & b.bayt));
	}

	public static Bitset8 operator |(Bitset8 a, Bitset8 b)
	{
		return new Bitset8((byte)(a.bayt | b.bayt));
	}

	public static Bitset8 operator ^(Bitset8 a, Bitset8 b)
	{
		return new Bitset8((byte)(a.bayt ^ b.bayt));
	}

	public static Bitset8 operator ~(Bitset8 a)
	{
		return new Bitset8((byte)(~a.bayt));
	}

	public bool Test(int position)
	{
		byte b = (byte)(1 << position);
		return (bayt & b) == b;
	}

	public void Set(int position)
	{
		bayt |= (byte)(1 << position);
	}

	public void Set(int position, bool value)
	{
		if (value)
		{
			Set(position);
		}
		else
		{
			Reset(position);
		}
	}

	public void Reset(int position)
	{
		bayt &= (byte)(~(1 << position));
	}

	public void Flip(int position)
	{
		bayt ^= (byte)(1 << position);
	}

	public bool Equals(Bitset8 s)
	{
		return this == s;
	}

	public override bool Equals(object obj)
	{
		if (obj is Bitset8)
		{
			return (Bitset8)obj == this;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return bayt.GetHashCode();
	}

	public override string ToString()
	{
		return Convert.ToString(bayt, 2).PadLeft(8, '0');
	}
}
