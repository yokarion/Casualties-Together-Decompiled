using System;
using System.Reflection;

namespace KrokoshaCasualtiesMP;

[AttributeUsage(AttributeTargets.Field)]
public class SettingDeclarerThingyBoolAttribute : SettingDeclarerThingyBaseAttribute
{
	public SettingDeclarerThingyBoolAttribute(string name)
	{
		n = name;
	}

	public override void Save(FieldInfo f, object v)
	{
		bool flag = (bool)v;
		if (allow_saving)
		{
			SettingsJsonThing.json.SetBool(GetPrefKey(f), flag);
		}
		f.SetValue(null, flag);
	}

	public override void Get(FieldInfo f)
	{
		if (allow_saving)
		{
			f.SetValue(null, SettingsJsonThing.json.GetBool(GetPrefKey(f), (bool)default_value));
		}
	}
}
