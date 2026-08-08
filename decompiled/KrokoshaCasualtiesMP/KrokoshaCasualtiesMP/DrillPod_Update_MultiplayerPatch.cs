using System.Collections.Generic;
using HarmonyLib;
using KrokoshaCasualtiesUtils;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(DrillPod), "Update")]
public static class DrillPod_Update_MultiplayerPatch
{
	public class Krokosha_DrillPod_OverrideComponent : MonoBehaviour
	{
		internal float timeHeld;

		public bool working => drill.working;

		public DrillPod drill => ((Component)this).GetComponent<DrillPod>();

		private void Update()
		{
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			if (!KrokoshaScavMultiplayer.network_system_is_running || didTeleport || KrokoshaScavMultiplayer.rules.CheckIfLayerContinueIsDisabled())
			{
				return;
			}
			bool flag = working;
			Body localBody = Util.GetLocalBody();
			bool flag2 = BodyIsStartingIt(drill, localBody);
			if (flag && flag2)
			{
				timeHeld += Time.deltaTime;
				PlayerCamera.main.bonusActionBarTime = timeHeld * 0.2f;
				if (timeHeld > 5f && !didTeleport)
				{
					ServerMain.GetNumberOfPlayersRequiredToFinishLayer();
					if (!CheckIfEveryoneIsInTheDrill(drill, out var current, out var required))
					{
						log.l($"DrillPod:  Local player tries to use DrillPod but not everyone is inside. count: {current}/{required}");
						Util.DoAlert(string.Format(Lang.Get("drillpod_not_enough_plrs", false), current, required), false);
						timeHeld = 0f;
					}
				}
			}
			else if (PlayerCamera.main.bonusActionBarTime > 0f)
			{
				PlayerCamera.main.bonusActionBarTime = 0f;
			}
			if (!flag || KrokoshaScavMultiplayer.is_client)
			{
				return;
			}
			int current2;
			int required2;
			bool flag3 = CheckIfEveryoneIsInTheDrill(drill, out current2, out required2);
			if (!flag3)
			{
				timeHeld = 0f;
			}
			foreach (KeyValuePair<Body, NetPlayer> item in NetPlayer.BodyToPlayerDict)
			{
				Body key = item.Key;
				Util.IsBodyLocal(key);
				if (flag && !didTeleport && BodyIsStartingIt(drill, key) && flag3)
				{
					timeHeld += Time.deltaTime;
					if (timeHeld > 5f && !didTeleport)
					{
						didTeleport = true;
						WorldGeneration.world.doPod = true;
						string text = ((required2 != 1) ? Lang.Get("all_started_drillpod", false) : string.Format(Lang.Get("plr_started_drillpod", false), item.Value.playername));
						Util.DoAlert(in text, false);
						Chat.Server_ChatAnnouncement(in text);
						((MonoBehaviour)WorldGeneration.world).StartCoroutine("RegenerateWorld", (object)true);
						Sound.Play("drillpoduse", Vector2.zero, true, false, (Transform)null, 1f, 1f, true, false);
					}
				}
			}
		}
	}

	internal static bool didTeleport;

	public static bool BodyIsInside(DrillPod drill, Body body)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (Mathf.Abs(((Component)body).transform.position.x - ((Component)drill).transform.position.x) < 1.5f)
		{
			return Mathf.Abs(((Component)body).transform.position.y - ((Component)drill).transform.position.y) < 5f;
		}
		return false;
	}

	public static bool BodyIsStartingIt(DrillPod drill, Body body)
	{
		if (BodyIsInside(drill, body))
		{
			return body.crouching;
		}
		return false;
	}

	public static bool CheckIfEveryoneIsInTheDrill(DrillPod drill, out int current, out int required)
	{
		return ServerMain.CheckIfCanFinishLayerCustomCondition((NetPlayer item) => BodyIsInside(drill, item.body), out current, out required);
	}

	public static bool Prefix(DrillPod __instance)
	{
		if (KrokoshaScavMultiplayer.network_system_is_running)
		{
			ComponentHolderProtocol.GetOrAddComponent<Krokosha_DrillPod_OverrideComponent>((Object)(object)__instance);
			return false;
		}
		return true;
	}
}
