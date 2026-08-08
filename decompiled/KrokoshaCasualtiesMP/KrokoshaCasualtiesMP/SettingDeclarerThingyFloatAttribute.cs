using System;
using System.Reflection;

namespace KrokoshaCasualtiesMP;

[AttributeUsage(AttributeTargets.Field)]
public class SettingDeclarerThingyFloatAttribute : SettingDeclarerThingyBaseAttribute
{
	public float mi;

	public float ma;

	public string postfix = "";

	public SettingDeclarerThingyFloatAttribute(float min, float max, string name)
	{
		mi = min;
		ma = max;
		n = name;
	}

	public override void Save(FieldInfo f, object v)
	{
		float num = (float)v;
		if (allow_saving)
		{
			SettingsJsonThing.json.SetFloat(GetPrefKey(f), num);
		}
		f.SetValue(null, num);
	}

	public override void Get(FieldInfo f)
	{
		if (allow_saving)
		{
			f.SetValue(null, SettingsJsonThing.json.GetFloat(GetPrefKey(f), (float)default_value));
		}
	}
}
