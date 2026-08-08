using HarmonyLib;
using KrokoshaCasualtiesUtils;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(WatchScript), "Update")]
internal static class WatchScriptUpdate_MultiplayerPatch
{
	private static bool Prefix(WatchScript __instance)
	{
		if (!Net.running)
		{
			return true;
		}
		Body body = ComponentHolderProtocol.GetOrAddComponent<BodyGetterOverrider>((Object)(object)__instance).GetBody();
		if ((Object)(object)body != (Object)null && !body.IsBodyLocal())
		{
			return false;
		}
		return true;
	}
}
