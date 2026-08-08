using HarmonyLib;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(WorldGeneration), "UpdateAmbientLight")]
internal static class WorldGeneration_UpdateAmbientLight_MultiplayerPatch
{
	private static void Postfix(WorldGeneration __instance)
	{
		float intensity = 0f;
		switch (WorldGeneration.GetRunSettingInt("ambientlight"))
		{
		case 1:
			intensity = 0.12f;
			break;
		case 2:
			intensity = 0.4f;
			break;
		}
		if (ConsoleScript.instance.fullBright || (!PlayerCamera_HandleScreenShaders_MultiplayerPatch.ShouldOverrideVisualsToDefault() && __instance.body.bothEyesGone))
		{
			intensity = 0.7f;
		}
		__instance.ambientLight.intensity = intensity;
	}
}
