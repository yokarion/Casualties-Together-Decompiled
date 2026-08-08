using System;
using System.Reflection;

namespace KrokoshaCasualtiesMP;

[AttributeUsage(AttributeTargets.Field)]
public abstract class SettingDeclarerThingyBaseAttribute : Attribute
{
	public string n;

	public object default_value;

	public bool allow_saving = true;

	public string GetPrefKey(FieldInfo f)
	{
		return "KrokoshaMultiplayer_Setting_" + f.DeclaringType?.ToString() + "_" + f.Name;
	}

	public virtual void Save(FieldInfo f, object v)
	{
		throw new NotImplementedException("SettingDeclarerThingyBaseAttribute.Save: Bro... thats not intended to be used! override ts!");
	}

	public virtual void Get(FieldInfo f)
	{
		throw new NotImplementedException("SettingDeclarerThingyBaseAttribute.Get:  Bro... thats is not intended to be used! override ts!");
	}
}
