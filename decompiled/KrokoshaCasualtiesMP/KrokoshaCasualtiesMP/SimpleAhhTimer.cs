using System.Runtime.CompilerServices;
using LiteNetLib;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public class SimpleAhhTimer
{
	public double last_fired;

	public double interval;

	public double curtime
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return Time.realtimeSinceStartupAsDouble;
		}
	}

	public SimpleAhhTimer(double interval = 1.0, float startdelay = 0f)
	{
		this.interval = interval;
		if (startdelay > 0f)
		{
			last_fired = curtime + (double)startdelay;
		}
	}

	public bool Check()
	{
		if (curtime - last_fired >= interval)
		{
			last_fired = curtime;
			return true;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DeliveryMethod DoReliability()
	{
		if (Check())
		{
			return (DeliveryMethod)0;
		}
		return (DeliveryMethod)4;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public T DoElse<T>(in T a, in T b)
	{
		if (!Check())
		{
			return b;
		}
		return a;
	}
}
