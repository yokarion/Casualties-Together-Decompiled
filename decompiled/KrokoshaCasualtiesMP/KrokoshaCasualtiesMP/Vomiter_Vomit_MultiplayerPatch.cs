using System.Collections.Generic;
using HarmonyLib;
using KrokoshaCasualtiesUtils;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(Vomiter), "Vomit")]
public static class Vomiter_Vomit_MultiplayerPatch
{
	public static void ForceNoWarning(Vomiter __instance, bool isblood)
	{
		if (!isblood)
		{
			__instance.vomiting = true;
		}
		Body component = ((Component)__instance).GetComponent<Body>();
		NetBody netBody = default(NetBody);
		if (component.IsBodyLocal() && ((Component)component).TryGetComponent<NetBody>(ref netBody))
		{
			netBody.SetNetHealthSyncIgnoreTime(0.1f);
		}
		((MonoBehaviour)__instance).StartCoroutine(isblood ? "DoBloodVomit" : "DoVomit");
	}

	public static void AnnounceVomit(knetid plr, bool isblood)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		NetDataWriter writer = Net.CreateWriter(10025);
		writer.Put((ushort)plr);
		writer.Put(isblood);
		Net.Server_SendToClients((DeliveryMethod)4, in writer, (IEnumerable<knetid>)ServerMain.AllClientIdsExceptHost);
	}

	internal static bool VerifyAndAnnounceVomit(Vomiter __instance, bool isblood)
	{
		if (KrokoshaScavMultiplayer.network_system_is_running)
		{
			Body component = ((Component)__instance).GetComponent<Body>();
			if (!component.alive)
			{
				return false;
			}
			NetBody netBody = default(NetBody);
			if (((Component)__instance).TryGetComponent<NetBody>(ref netBody) && KrokoshaScavMultiplayer.is_server)
			{
				AnnounceVomit(netBody.netId, isblood);
			}
			if (component.IsBodyLocal())
			{
				return true;
			}
			ForceNoWarning(__instance, isblood);
			return false;
		}
		return true;
	}

	public static bool Prefix(Vomiter __instance)
	{
		if (__instance.vomiting)
		{
			return true;
		}
		return VerifyAndAnnounceVomit(__instance, isblood: false);
	}
}
