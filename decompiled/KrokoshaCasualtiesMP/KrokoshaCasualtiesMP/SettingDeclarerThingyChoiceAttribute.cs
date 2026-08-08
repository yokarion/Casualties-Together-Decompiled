using System;
using System.Reflection;

namespace KrokoshaCasualtiesMP;

[AttributeUsage(AttributeTargets.Field)]
public class SettingDeclarerThingyChoiceAttribute : SettingDeclarerThingyBaseAttribute
{
	public string[] choices;

	public SettingDeclarerThingyChoiceAttribute(string name, string[] choices)
	{
		n = name;
		this.choices = choices;
	}

	public override void Save(FieldInfo f, object v)
	{
		int num = Convert.ToInt32(v);
		if (allow_saving)
		{
			SettingsJsonThing.json.SetInt(GetPrefKey(f), num);
		}
		f.SetValue(null, Convert.ChangeType(num, f.FieldType));
	}

	public override void Get(FieldInfo f)
	{
		if (allow_saving)
		{
			f.SetValue(null, Convert.ChangeType(SettingsJsonThing.json.GetInt(GetPrefKey(f), Convert.ToInt32(default_value)), f.FieldType));
		}
	}
}
