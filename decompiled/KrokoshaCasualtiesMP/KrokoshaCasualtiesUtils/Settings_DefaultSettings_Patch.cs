using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace KrokoshaCasualtiesUtils;

[HarmonyPatch(typeof(Settings), "DefaultSettings")]
internal static class Settings_DefaultSettings_Patch
{
	public static Dictionary<string, KeyCode> custom_binds = new Dictionary<string, KeyCode>();

	private static void Postfix(ref List<Setting> __result)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Expected O, but got Unknown
		foreach (KeyValuePair<string, KeyCode> keybind in custom_binds)
		{
			if (!__result.Any((Setting x) => x.name == keybind.Key))
			{
				__result.Add((Setting)new SettingKeybind
				{
					name = keybind.Key,
					value = keybind.Value,
					apply = delegate
					{
					},
					category = (SettingCategory)3
				});
			}
		}
	}
}
