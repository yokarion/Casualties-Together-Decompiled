using HarmonyLib;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(WorldGeneration), "ApplyLayerModifiers")]
public static class WorldGeneration_ApplyLayerModifiers_MultiplayerPatch
{
	public static bool sent_to_clients;

	public static bool Prefix(WorldGeneration __instance)
	{
		if (Net.running)
		{
			if (KrokoshaScavMultiplayer.is_client || WorldgenPatches.aaaaaaaaaaaaaaaaaaaaaaaaaaa != null)
			{
				return false;
			}
			__instance.generatingWorld = true;
		}
		return true;
	}

	public static void Postfix(WorldGeneration __instance)
	{
		if (Net.running && KrokoshaScavMultiplayer.is_server)
		{
			LayerModifier[] availableModifiers = LayerModifier.availableModifiers;
			foreach (LayerModifier val in availableModifiers)
			{
				if (val.active)
				{
					log.l(string.Format("SERVER: Sending layer modifier: {0}  index: {1}", "layerModifier", val.modifierIndex));
					KrokoshaScavMultiplayer.Server_SendRelayMessageToClients((ushort)10015, (knetid)(ushort)val.modifierIndex);
				}
			}
		}
		sent_to_clients = true;
	}
}
