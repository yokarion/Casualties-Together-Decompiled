using System;

namespace KrokoshaCasualtiesMP;

[AttributeUsage(AttributeTargets.Field)]
public class KrokoshaRuleFloatAttribute : Attribute
{
	public float mi;

	public float ma;

	public string postfix;

	public KrokoshaRuleFloatAttribute(float min, float max, string unit = "")
	{
		mi = min;
		ma = max;
		postfix = unit;
	}
}
