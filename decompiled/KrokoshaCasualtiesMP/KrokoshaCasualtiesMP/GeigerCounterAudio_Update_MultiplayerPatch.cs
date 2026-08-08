using HarmonyLib;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(GeigerCounterAudio), "Update")]
internal static class GeigerCounterAudio_Update_MultiplayerPatch
{
	private static bool Prefix(GeigerCounterAudio __instance)
	{
		if (KrokoshaScavMultiplayer.network_system_is_running)
		{
			if (!__instance.item.battery.hasCharge)
			{
				__instance.active = false;
			}
			if (!__instance.active)
			{
				__instance.audioSource.Stop();
				return false;
			}
			if (!__instance.audioSource.isPlaying)
			{
				__instance.audioSource.Play();
			}
			float num = 0f;
			BodyGetterOverrider orAddComponent = ComponentHolderProtocol.GetOrAddComponent<BodyGetterOverrider>((Object)(object)__instance);
			if (orAddComponent.TryGetBodyFromParent() && orAddComponent.body.TryGetNetBody(out var nb))
			{
				num = nb.irradiateIntensity;
			}
			int clipIndex = __instance.GetClipIndex(num);
			if ((Object)(object)__instance.audioSource.clip != (Object)(object)__instance.radiationClips[clipIndex])
			{
				__instance.audioSource.clip = __instance.radiationClips[clipIndex];
				__instance.audioSource.Play();
			}
			return false;
		}
		return true;
	}
}
