using HarmonyLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(DrillPod), "OnUse")]
public static class DrillPod_OnUse_MultiplayerPatch
{
	public static void OnUseRewritten(Body user, DrillPod drel)
	{
		Item val = default(Item);
		if (user.FindByIdThorough("drillrepairkit", ref val))
		{
			Object.Destroy((Object)(object)((Component)val).gameObject);
			ForceRepairDrill(drel);
		}
	}

	public static void ForceRepairDrill(DrillPod drel)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		Traverse.Create((object)drel).Field("working").SetValue((object)true);
		Sound.Play("drillpodrepair", Vector2.op_Implicit(((Component)drel).transform.position), false, false, (Transform)null, 1f, 1f, false, false);
		((Component)drel).GetComponent<BuildingEntity>().description = Locale.GetBuilding("drillpoddscfixed");
		Object.Destroy((Object)(object)((Component)drel).GetComponent<UsableObject>());
	}

	private static void Prefix()
	{
		_ = KrokoshaScavMultiplayer.network_system_is_running;
	}

	private static void Postfix(DrillPod __instance)
	{
		if (KrokoshaScavMultiplayer.network_system_is_running && NetObjectRegistry.TryGetSyncInfo(((Component)__instance).gameObject, out var si))
		{
			si.SetIgnoreTimeForRoundTrip();
			KrokoshaScavMultiplayer.Client_SendSimpleMessageToServer((ushort)10083, (ushort)si.syncId, true);
		}
	}

	[ServerReceiver(10083)]
	private static void Server_DrillPodRepairTypeShit(knetid clientId, ref NetDataReader reader)
	{
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out knetid result);
		DrillPod val = null;
		SyncInfo si = null;
		if (!NetPlayer.TryGetNetPlayerAndBodyFromClientId(clientId, out var plr, out var body) || !NetObjectRegistry.TryGetSyncInfo(result, out si) || !si.go.TryGetComponent<DrillPod>(ref val))
		{
			return;
		}
		if ((ushort)clientId != 0)
		{
			if (!plr.body.conscious || !si.IsBuilding() || Traverse.Create((object)val).Field("working").GetValue<bool>())
			{
				return;
			}
			Component ba = (Component)(object)body;
			if (!KM.dist2dsqrcheck(in ba, (Component)(object)si.building, 20f))
			{
				return;
			}
		}
		if ((ushort)clientId != 0)
		{
			OnUseRewritten(body, val);
		}
		ServerMain.Server_AnnounceSound(Vector2.op_Implicit(si.go.transform.position), "drillpodrepair", ServerMain.GetListOfClientIdsExceptThisAndHost(clientId));
		if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
		{
			DebugHelp.OnNetEvent(Vector2.op_Implicit(si.go.transform.position), $"S: DrillPod Repair {plr}");
		}
	}
}
