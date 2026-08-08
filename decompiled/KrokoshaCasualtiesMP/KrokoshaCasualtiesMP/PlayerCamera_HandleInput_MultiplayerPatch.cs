using HarmonyLib;
using KrokoshaCasualtiesUtils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(PlayerCamera), "HandleInput")]
public static class PlayerCamera_HandleInput_MultiplayerPatch
{
	private static bool Prefix()
	{
		if (KrokoshaScavMultiplayer.network_system_is_running && ServerMain.DidHeFinishTheLayer(PlayerCamera.main.body))
		{
			PlayerCamera.main.body.forceWalk = false;
		}
		return !Chat.CHAT_textbox_input_focused;
	}

	private static void Postfix()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if (!KrokoshaScavMultiplayer.network_system_is_running)
		{
			return;
		}
		Body localBody = Util.GetLocalBody();
		if ((Object)(object)localBody != (Object)null && localBody.alive)
		{
			if (Input.GetKeyDown(KeyBinds.GetBind("pause")))
			{
				UIInGame.StopPlayerInteractionMenu();
			}
			if (Input.GetKeyDown(KeyBinds.GetBind("bark")) && Util.GetLocalBody().eatTime == 0.5f)
			{
				KrokoshaScavMultiplayer.Client_SendSimpleMessageToServer((ushort)10039);
			}
		}
	}
}
