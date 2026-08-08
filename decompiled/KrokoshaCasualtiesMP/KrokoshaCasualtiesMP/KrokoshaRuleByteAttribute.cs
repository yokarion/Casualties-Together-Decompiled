using System;

namespace KrokoshaCasualtiesMP;

[AttributeUsage(AttributeTargets.Field)]
public class KrokoshaRuleByteAttribute : Attribute
{
	public byte limit;

	public string postfix;

	public KrokoshaRuleByteAttribute(byte max = 100, string unit = "%")
	{
		limit = max;
		postfix = unit;
	}
}
