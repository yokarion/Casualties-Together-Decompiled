using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(PlushScript), "Start")]
internal static class j
{
	private static Sprite __ = CoopModAssets.LoadSprite("d.krokosha666.png");

	internal static bool Prefix(PlushScript __instance)
	{
		if (!__instance.possibleSprites.Contains(__))
		{
			__instance.possibleSprites = CollectionExtensions.AddToArray<Sprite>(CollectionExtensions.AddToArray<Sprite>(__instance.possibleSprites, __), __);
			__instance.possibleSounds = CollectionExtensions.AddToArray<AudioClip>(CollectionExtensions.AddToArray<AudioClip>(__instance.possibleSounds, __instance.possibleSounds[0]), __instance.possibleSounds[0]);
		}
		return true;
	}
}
