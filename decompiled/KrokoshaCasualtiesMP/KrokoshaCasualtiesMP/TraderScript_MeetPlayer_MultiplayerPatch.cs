using System.Collections.Generic;
using HarmonyLib;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(TraderScript), "MeetPlayer")]
public static class TraderScript_MeetPlayer_MultiplayerPatch
{
	public static (float, float, bool) MeetPlayer_CalculateFirstImpression(Body body)
	{
		float num = 0f;
		float num2 = 0f;
		bool item = false;
		num = Random.Range(75f, 135f);
		num += body.skills.INTFrom10 * 4f;
		if (body.talker.impairedSpeech)
		{
			num -= 20f;
		}
		if (body.disfigured)
		{
			num -= 5f;
		}
		if (body.dirtyness > 50f)
		{
			num -= body.dirtyness * 0.25f;
		}
		if (body.HoldingItem(body.handSlot) && body.GetItem(body.handSlot).Stats.HasTag("gun"))
		{
			num2 += 50f;
			num -= 20f;
		}
		if (Object.op_Implicit((Object)(object)body.mindWipe))
		{
			num *= 0.7f;
		}
		num -= (100f - body.brainHealth) * 0.5f;
		num += body.happiness * 0.5f;
		if (body.hearingLoss > 50f)
		{
			num -= 20f;
		}
		if (body.totalBleedSpeed > 0.001f)
		{
			item = true;
		}
		return (num, num2, item);
	}

	public static bool Prefix(TraderScript __instance, ref KrokoshaTraderTrackerComponent.TraderStatePacket __state)
	{
		if (KrokoshaScavMultiplayer.network_system_is_running)
		{
			KrokoshaTraderTrackerComponent orAddComponent = ComponentHolderProtocol.GetOrAddComponent<KrokoshaTraderTrackerComponent>((Object)(object)__instance);
			__state = new KrokoshaTraderTrackerComponent.TraderStatePacket(orAddComponent);
		}
		return true;
	}

	public static void Postfix(TraderScript __instance, ref KrokoshaTraderTrackerComponent.TraderStatePacket __state)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		if (!KrokoshaScavMultiplayer.network_system_is_running)
		{
			return;
		}
		KrokoshaTraderTrackerComponent orAddComponent = ComponentHolderProtocol.GetOrAddComponent<KrokoshaTraderTrackerComponent>((Object)(object)__instance);
		__instance.eyes.sprite = __instance.eyeSprites[1];
		Traverse.Create((object)__instance).Field("sitTime").SetValue((object)Time.time);
		__instance.startedConvo = true;
		if (!KrokoshaScavMultiplayer.is_client)
		{
			List<NetBody> playerBodiesInRadius = NetPlayer.GetPlayerBodiesInRadius(Vector2.op_Implicit(((Component)__instance).transform.position), 16f);
			float num = 0f;
			float num2 = 0f;
			bool flag = false;
			foreach (NetBody item in playerBodiesInRadius)
			{
				(float, float, bool) tuple = MeetPlayer_CalculateFirstImpression(item.body);
				num = Mathf.Max(tuple.Item1, num);
				num2 = Mathf.Min(tuple.Item2, num2);
				if (!flag)
				{
					flag = tuple.Item3;
				}
			}
			__instance.reputation = num;
			__instance.hostility = num2;
			orAddComponent.og.freeDressing = flag;
			orAddComponent.Server_AnnounceTraderReputationState((IReadOnlyList<knetid>)null);
		}
		else
		{
			__state.Apply(orAddComponent);
		}
	}
}
