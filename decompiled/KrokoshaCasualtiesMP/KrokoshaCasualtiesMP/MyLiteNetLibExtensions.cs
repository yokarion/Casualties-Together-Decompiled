using System;
using System.Runtime.CompilerServices;
using KrokoshaCasualtiesUtils;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public static class MyLiteNetLibExtensions
{
	public enum NetValueType : byte
	{
		Null,
		Bool,
		Byte,
		SByte,
		Short,
		UShort,
		Int,
		UInt,
		Long,
		ULong,
		Float,
		Double,
		Char,
		String,
		Guid,
		ByteArray
	}

	public static bool TryGetInfo(this NetPeer peer, out KrokoshaPlayerListEntry kple)
	{
		if (peer.Tag == null)
		{
			kple = null;
			return false;
		}
		if (peer.Tag is KrokoshaPlayerListEntry krokoshaPlayerListEntry)
		{
			kple = krokoshaPlayerListEntry;
			return true;
		}
		kple = null;
		return false;
	}

	public static bool TryGetPlayer(this NetPeer peer, out NetPlayer plr)
	{
		if (peer.Tag == null)
		{
			plr = null;
			return false;
		}
		if (peer.Tag is KrokoshaPlayerListEntry krokoshaPlayerListEntry && (Object)(object)krokoshaPlayerListEntry.plr != (Object)null)
		{
			plr = krokoshaPlayerListEntry.plr;
			return true;
		}
		plr = null;
		return false;
	}

	public static void ResetKeepMsgId(this NetDataWriter w2)
	{
		w2.SetPosition(2);
	}

	public static void CompressWriter(this NetDataWriter w2, int start_to_ignore = 2)
	{
		byte[] array = Util.Compress(w2.CopyData(start_to_ignore));
		w2.SetPosition(start_to_ignore);
		w2.PutBytesWithLength(array);
	}

	public static NetDataReader DecompressReader(this NetDataReader r2)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		return new NetDataReader(Util.Decompress(r2.GetBytesWithLength()));
	}

	public static byte[] CopyData(this NetDataWriter w2, int start_index)
	{
		return w2.CopyData(start_index, w2.Length - start_index);
	}

	public static byte[] CopyData(this NetDataWriter w2, int start_index, int length)
	{
		byte[] array = new byte[length];
		Buffer.BlockCopy(w2.Data, start_index, array, 0, length);
		return array;
	}

	public static void Put<T>(this NetDataWriter writer, T value) where T : unmanaged, INetSerializeByMemcpy
	{
		writer.PutUnmanaged(value);
	}

	public static void Get<T>(this NetDataReader reader, out T result) where T : unmanaged, INetSerializeByMemcpy
	{
		result = reader.GetUnmanaged<T>();
	}

	public static void Get(this NetDataReader reader, out byte[] result)
	{
		result = reader.GetBytesWithLength();
	}

	public static void Put(this NetDataWriter writer, float[] value)
	{
		writer.PutArray(value);
	}

	public static void Get(this NetDataReader reader, out float[] result)
	{
		result = reader.GetFloatArray();
	}

	public static void Put(this NetDataWriter writer, Vector2[] value)
	{
		writer.Put(value.Length);
		for (int i = 0; i < value.Length; i++)
		{
			writer.Put(value[i].x);
			writer.Put(value[i].y);
		}
	}

	public static void Get(this NetDataReader reader, out Vector2[] result)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		int num = reader.GetInt();
		result = (Vector2[])(object)new Vector2[num];
		for (int i = 0; i < num; i++)
		{
			float num2 = reader.GetFloat();
			float num3 = reader.GetFloat();
			result[i] = new Vector2(num2, num3);
		}
	}

	public static void Put(this NetDataWriter writer, Vector2 value)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		writer.Put(value.x);
		writer.Put(value.y);
	}

	public static void Get(this NetDataReader reader, out Vector2 result)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		result = new Vector2(reader.GetFloat(), reader.GetFloat());
	}

	public static void Put(this NetDataWriter writer, Vector2Int value)
	{
		writer.Put(((Vector2Int)(ref value)).x);
		writer.Put(((Vector2Int)(ref value)).y);
	}

	public static void Get(this NetDataReader reader, out Vector2Int result)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		result = new Vector2Int(reader.GetInt(), reader.GetInt());
	}

	public static void Put(this NetDataWriter writer, Color value)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		writer.Put(value.r);
		writer.Put(value.g);
		writer.Put(value.b);
		writer.Put(value.a);
	}

	public static void Get(this NetDataReader reader, out Color result)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		result = new Color(reader.GetFloat(), reader.GetFloat(), reader.GetFloat(), reader.GetFloat());
	}

	public static void Put(this NetDataWriter writer, knetid value)
	{
		writer.Put(value.id);
	}

	public static void Get(this NetDataReader reader, out knetid result)
	{
		result = reader.GetUShort();
	}

	public static void Put(this NetDataWriter writer, string value, bool oneByteChars)
	{
		if (oneByteChars)
		{
			writer.PutBytesWithLength(value.ToByteArray());
		}
		else
		{
			writer.Put(value);
		}
	}

	public static void Get(this NetDataReader reader, out string result, bool oneByteChars)
	{
		if (oneByteChars)
		{
			result = reader.GetBytesWithLength().ToStringFromCharBytes();
		}
		else
		{
			result = reader.GetString();
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void PutUnmanaged<T>(this NetDataWriter writer, T value) where T : unmanaged
	{
		int num = sizeof(T);
		int length = writer.Length;
		writer.ResizeIfNeed(length + num);
		if (writer.Data.Length < length + num)
		{
			throw new IndexOutOfRangeException();
		}
		fixed (byte* ptr = &writer.Data[length])
		{
			*(T*)ptr = value;
		}
		writer.SetPosition(length + num);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void GetUnmanaged<T>(this NetDataReader reader, out T result) where T : unmanaged
	{
		result = reader.GetUnmanaged<T>();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static T GetUnmanaged<T>(this NetDataReader reader) where T : unmanaged
	{
		int num = sizeof(T);
		if (reader.RawData.Length < reader.Position + num)
		{
			throw new IndexOutOfRangeException();
		}
		T result;
		fixed (byte* ptr = &reader.RawData[reader.Position])
		{
			result = *(T*)ptr;
		}
		reader.SetPosition(reader.Position + num);
		return result;
	}

	public static void PutAny<T>(this NetDataWriter writer, T value)
	{
		Type typeFromHandle = typeof(T);
		if (typeFromHandle == typeof(bool))
		{
			writer.Put((bool)(object)value);
			return;
		}
		if (typeFromHandle == typeof(byte))
		{
			writer.Put((byte)(object)value);
			return;
		}
		if (typeFromHandle == typeof(sbyte))
		{
			writer.Put((sbyte)(object)value);
			return;
		}
		if (typeFromHandle == typeof(short))
		{
			writer.Put((short)(object)value);
			return;
		}
		if (typeFromHandle == typeof(ushort))
		{
			writer.Put((ushort)(object)value);
			return;
		}
		if (typeFromHandle == typeof(int))
		{
			writer.Put((int)(object)value);
			return;
		}
		if (typeFromHandle == typeof(uint))
		{
			writer.Put((uint)(object)value);
			return;
		}
		if (typeFromHandle == typeof(long))
		{
			writer.Put((long)(object)value);
			return;
		}
		if (typeFromHandle == typeof(ulong))
		{
			writer.Put((ulong)(object)value);
			return;
		}
		if (typeFromHandle == typeof(float))
		{
			writer.Put((float)(object)value);
			return;
		}
		if (typeFromHandle == typeof(double))
		{
			writer.Put((double)(object)value);
			return;
		}
		if (typeFromHandle == typeof(char))
		{
			writer.Put((char)(object)value);
			return;
		}
		if (typeFromHandle == typeof(string))
		{
			writer.Put((string)(object)value);
			return;
		}
		if (typeFromHandle == typeof(byte[]))
		{
			writer.Put((byte[])(object)value);
			return;
		}
		if (typeFromHandle == typeof(Guid))
		{
			writer.Put((Guid)(object)value);
			return;
		}
		throw new NotSupportedException($"Type {typeFromHandle} is not supported.");
	}

	public static bool GetAny<T>(this NetDataReader reader, out T value)
	{
		Type typeFromHandle = typeof(T);
		try
		{
			object obj;
			if (typeFromHandle == typeof(bool))
			{
				obj = reader.GetBool();
			}
			else if (typeFromHandle == typeof(byte))
			{
				obj = reader.GetByte();
			}
			else if (typeFromHandle == typeof(sbyte))
			{
				obj = reader.GetSByte();
			}
			else if (typeFromHandle == typeof(short))
			{
				obj = reader.GetShort();
			}
			else if (typeFromHandle == typeof(ushort))
			{
				obj = reader.GetUShort();
			}
			else if (typeFromHandle == typeof(int))
			{
				obj = reader.GetInt();
			}
			else if (typeFromHandle == typeof(uint))
			{
				obj = reader.GetUInt();
			}
			else if (typeFromHandle == typeof(long))
			{
				obj = reader.GetLong();
			}
			else if (typeFromHandle == typeof(ulong))
			{
				obj = reader.GetULong();
			}
			else if (typeFromHandle == typeof(float))
			{
				obj = reader.GetFloat();
			}
			else if (typeFromHandle == typeof(double))
			{
				obj = reader.GetDouble();
			}
			else if (typeFromHandle == typeof(char))
			{
				obj = reader.GetChar();
			}
			else if (typeFromHandle == typeof(string))
			{
				obj = reader.GetString();
			}
			else if (typeFromHandle == typeof(byte[]))
			{
				obj = reader.GetRemainingBytes();
			}
			else
			{
				if (!(typeFromHandle == typeof(Guid)))
				{
					throw new NotSupportedException($"Type {typeFromHandle} is not supported.");
				}
				obj = reader.GetGuid();
			}
			value = (T)obj;
			return true;
		}
		catch
		{
			value = default(T);
			return false;
		}
	}

	public static void PutObject(this NetDataWriter writer, object value)
	{
		if (value == null)
		{
			writer.Put((byte)0);
		}
		else if (!(value is bool flag))
		{
			if (!(value is byte b))
			{
				if (!(value is sbyte b2))
				{
					if (!(value is short num))
					{
						if (!(value is ushort num2))
						{
							if (!(value is int num3))
							{
								if (!(value is uint num4))
								{
									if (!(value is long num5))
									{
										if (!(value is ulong num6))
										{
											if (!(value is float num7))
											{
												if (!(value is double num8))
												{
													if (!(value is char c))
													{
														if (!(value is string text))
														{
															if (!(value is Guid guid))
															{
																if (!(value is byte[] array))
																{
																	throw new NotSupportedException("Unsupported object type: " + value.GetType().FullName);
																}
																writer.Put((byte)15);
																writer.PutBytesWithLength(array);
															}
															else
															{
																writer.Put((byte)14);
																writer.Put(guid);
															}
														}
														else
														{
															writer.Put((byte)13);
															writer.Put(text);
														}
													}
													else
													{
														writer.Put((byte)12);
														writer.Put(c);
													}
												}
												else
												{
													writer.Put((byte)11);
													writer.Put(num8);
												}
											}
											else
											{
												writer.Put((byte)10);
												writer.Put(num7);
											}
										}
										else
										{
											writer.Put((byte)9);
											writer.Put(num6);
										}
									}
									else
									{
										writer.Put((byte)8);
										writer.Put(num5);
									}
								}
								else
								{
									writer.Put((byte)7);
									writer.Put(num4);
								}
							}
							else
							{
								writer.Put((byte)6);
								writer.Put(num3);
							}
						}
						else
						{
							writer.Put((byte)5);
							writer.Put(num2);
						}
					}
					else
					{
						writer.Put((byte)4);
						writer.Put(num);
					}
				}
				else
				{
					writer.Put((byte)3);
					writer.Put(b2);
				}
			}
			else
			{
				writer.Put((byte)2);
				writer.Put(b);
			}
		}
		else
		{
			writer.Put((byte)1);
			writer.Put(flag);
		}
	}

	public static object GetObject(this NetDataReader reader)
	{
		NetValueType netValueType = (NetValueType)reader.GetByte();
		return netValueType switch
		{
			NetValueType.Null => null, 
			NetValueType.Bool => reader.GetBool(), 
			NetValueType.Byte => reader.GetByte(), 
			NetValueType.SByte => reader.GetSByte(), 
			NetValueType.Short => reader.GetShort(), 
			NetValueType.UShort => reader.GetUShort(), 
			NetValueType.Int => reader.GetInt(), 
			NetValueType.UInt => reader.GetUInt(), 
			NetValueType.Long => reader.GetLong(), 
			NetValueType.ULong => reader.GetULong(), 
			NetValueType.Float => reader.GetFloat(), 
			NetValueType.Double => reader.GetDouble(), 
			NetValueType.Char => reader.GetChar(), 
			NetValueType.String => reader.GetString(), 
			NetValueType.Guid => reader.GetGuid(), 
			NetValueType.ByteArray => reader.GetBytesWithLength(), 
			_ => throw new InvalidOperationException($"Unknown NetValueType: {netValueType}"), 
		};
	}
}
