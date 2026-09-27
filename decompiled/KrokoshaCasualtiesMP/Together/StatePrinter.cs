using System.Reflection;
using System.Text;

namespace Together;

public static class StatePrinter
{
	public static string PrintState(object val)
	{
		StringBuilder stringBuilder = new StringBuilder();
		AppendFields(stringBuilder, val, "val");
		return stringBuilder.ToString();
	}

	private static void AppendFields(StringBuilder sb, object obj, string path)
	{
		if (obj == null)
		{
			return;
		}
		FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
		foreach (FieldInfo fieldInfo in fields)
		{
			object value = fieldInfo.GetValue(obj);
			string text = path + "." + fieldInfo.Name;
			if (fieldInfo.FieldType.IsPrimitive || fieldInfo.FieldType == typeof(string) || fieldInfo.FieldType.IsEnum)
			{
				sb.AppendLine($"{text} = {value};");
			}
			else
			{
				AppendFields(sb, value, text);
			}
		}
	}
}
