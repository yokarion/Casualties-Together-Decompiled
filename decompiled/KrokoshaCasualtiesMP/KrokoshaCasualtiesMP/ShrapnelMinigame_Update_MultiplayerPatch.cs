using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(ShrapnelMinigame), "Update")]
public static class ShrapnelMinigame_Update_MultiplayerPatch
{
	public static bool last_packet_was_a_change;

	private static float synctimer;

	private static RectTransform last_held;

	public static void Prefix(ShrapnelMinigame __instance, List<RaycastResult> uiCasts, ref float __state)
	{
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		if (!KrokoshaScavMultiplayer.network_system_is_running)
		{
			return;
		}
		Traverse val = Traverse.Create((object)__instance);
		Limb value = val.Field("limb").GetValue<Limb>();
		if ((Object)(object)value == (Object)null)
		{
			return;
		}
		List<RectTransform> value2 = val.Field("objects").GetValue<List<RectTransform>>();
		RectTransform val2 = val.Field("currentlyHeld").GetValue<RectTransform>();
		bool reliable = false;
		if ((Object)(object)val2 != (Object)null)
		{
			byte index = (byte)value2.IndexOf(val2);
			NetPlayer netPlayer = MinigameMPManager.ShrapnelMinigameSession.client_shrapnel_owners[index];
			if ((Object)(object)netPlayer == (Object)null || (Object)(object)netPlayer == (Object)(object)NetPlayer.LOCAL_PLAYER)
			{
				synctimer -= Time.deltaTime;
				if (val2.anchoredPosition.y < 35f)
				{
					Vector2 value3 = val.Field("heldOffset").GetValue<Vector2>();
					if (Minigame.game.handPos.y + value3.y > 35f)
					{
						val2.anchoredPosition = Minigame.game.handPos + value3;
						synctimer = 0f;
						reliable = true;
					}
				}
			}
			else
			{
				val2 = null;
				Minigame.game.ForceUngrab();
			}
		}
		else if ((Object)(object)last_held != (Object)null)
		{
			synctimer = 0f;
			reliable = true;
		}
		if (synctimer <= 0f && (Object)(object)val2 != (Object)null)
		{
			synctimer = 0.05f;
			NetBody netBody = default(NetBody);
			if (((Component)value.body).TryGetComponent<NetBody>(ref netBody))
			{
				byte data = (byte)value2.IndexOf(val2);
				KrokoshaScavMultiplayer.Client_SendSimpleMessageToServer((ushort)10069, data, val2.anchoredPosition, reliable);
			}
		}
		AudioSource component = ((Component)Minigame.game.spawnedMiniGame).GetComponent<AudioSource>();
		__state = component.volume;
		last_held = val2;
	}

	public static void Postfix(ShrapnelMinigame __instance, List<RaycastResult> uiCasts, ref float __state)
	{
		if (KrokoshaScavMultiplayer.network_system_is_running && (Object)(object)Traverse.Create((object)__instance).Field("currentlyHeld").GetValue<RectTransform>() == (Object)null)
		{
			((Component)Minigame.game.spawnedMiniGame).GetComponent<AudioSource>().volume = Mathf.MoveTowards(__state, last_packet_was_a_change ? 1f : 0f, Time.deltaTime * 5f);
		}
	}
}
