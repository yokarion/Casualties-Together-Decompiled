using System;
using System.Collections.Generic;
using HarmonyLib;

namespace KrokoshaCasualtiesMP;

public class TwoWayDictionary<T1, T2>
{
	private readonly Dictionary<T1, T2> _forward = new Dictionary<T1, T2>();

	private readonly Dictionary<T2, T1> _reverse = new Dictionary<T2, T1>();

	public T2 this[T1 key]
	{
		get
		{
			return _forward[key];
		}
		set
		{
			_forward[key] = value;
			_reverse[value] = key;
		}
	}

	public T1 this[T2 key2]
	{
		get
		{
			return _reverse[key2];
		}
		set
		{
			_reverse[key2] = value;
			_forward[value] = key2;
		}
	}

	public int Count => _forward.Count;

	public void Add(T1 key, T2 value)
	{
		if (_forward.ContainsKey(key) || _reverse.ContainsKey(value))
		{
			throw new ArgumentException("Duplicate key or value.");
		}
		_forward.Add(key, value);
		_reverse.Add(value, key);
	}

	public bool TryGetByFirst(T1 key, out T2 value)
	{
		return _forward.TryGetValue(key, out value);
	}

	public bool TryGetBySecond(T2 value, out T1 key)
	{
		return _reverse.TryGetValue(value, out key);
	}

	public T2 GetValueSafeFirst(T1 key)
	{
		return GeneralExtensions.GetValueSafe<T1, T2>(_forward, key);
	}

	public T1 GetValueSafeSecond(T2 value)
	{
		return GeneralExtensions.GetValueSafe<T2, T1>(_reverse, value);
	}

	public bool RemoveByFirst(T1 key)
	{
		if (_forward.TryGetValue(key, out var value))
		{
			_forward.Remove(key);
			_reverse.Remove(value);
			return true;
		}
		return false;
	}

	public bool RemoveBySecond(T2 value)
	{
		if (_reverse.TryGetValue(value, out var value2))
		{
			_reverse.Remove(value);
			_forward.Remove(value2);
			return true;
		}
		return false;
	}

	public void Clear()
	{
		_forward.Clear();
		_reverse.Clear();
	}
}
