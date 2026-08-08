using HarmonyLib;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(Liquids), "LowGradeStimulantStep")]
internal static class Liquids_LowGradeStimulantStep_MultiplayerPatch
{
	private static void Prefix(Limb limb)
	{
		if (KrokoshaScavMultiplayer.network_system_is_running)
		{
			CoUtils_instance_MultiplayerPatch.cur_override_instance = limb.GetCoUtilsInstance();
		}
	}

	private static void Postfix(Limb limb)
	{
		CoUtils_instance_MultiplayerPatch.cur_override_instance = null;
	}
}
