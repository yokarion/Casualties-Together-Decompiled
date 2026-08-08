using HarmonyLib;
using LiteNetLib.Utils;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(BioTerminalScript), "OnUse")]
public static class BioTerminalScript_OnUse_MultiplayerPatch
{
	public static void OnUseRewritten(Body user, BuildingEntity building)
	{
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		WaterContainerItem val = default(WaterContainerItem);
		foreach (Item item in user.GetAllItemsThorough())
		{
			if (((Component)item).TryGetComponent<WaterContainerItem>(ref val) && val.AmountOf("redblood") >= 100f)
			{
				flag = true;
				val.Drain(val.CalculateDrainSingleLiquid("redblood", 100f));
				break;
			}
		}
		if (!flag)
		{
			return;
		}
		building.Backgroundify();
		Sound.Play("beep", Vector2.op_Implicit(((Component)building).transform.position), false, true, (Transform)null, 1f, 1f, false, false);
		Collider2D[] array = Physics2D.OverlapCircleAll(Vector2.op_Implicit(((Component)building).transform.position), 6f);
		BuildingEntity val2 = default(BuildingEntity);
		for (int i = 0; i < array.Length; i++)
		{
			if (((Component)array[i]).TryGetComponent<BuildingEntity>(ref val2) && val2.id == "reinforceddoor")
			{
				val2.Backgroundify();
			}
		}
	}

	private static void Postfix(BioTerminalScript __instance)
	{
		if (KrokoshaScavMultiplayer.network_system_is_running && ComponentHolderProtocol.GetOrAddComponent<Krokosha_BuildingEntity_TrackerComponent_for_backgroundified>((Object)(object)__instance).is_backgroundified && NetObjectRegistry.TryGetSyncInfo(((Component)__instance).gameObject, out var si))
		{
			KrokoshaScavMultiplayer.Client_SendSimpleMessageToServer((ushort)10082, (ushort)si.syncId, true);
		}
	}

	[ServerReceiver(10082)]
	private static void Server_BioTerminalScriptOpenShitheadBruh(knetid clientId, ref NetDataReader reader)
	{
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out knetid result);
		if (!NetPlayer.TryGetNetPlayerAndBodyFromClientId(clientId, out var plr, out var body) || !NetObjectRegistry.TryGetSyncInfo(result, out var si))
		{
			return;
		}
		if ((ushort)clientId != 0)
		{
			Krokosha_BuildingEntity_TrackerComponent_for_backgroundified krokosha_BuildingEntity_TrackerComponent_for_backgroundified = default(Krokosha_BuildingEntity_TrackerComponent_for_backgroundified);
			BioTerminalScript val = default(BioTerminalScript);
			if (!plr.body.conscious || !si.IsBuilding() || !si.go.TryGetComponent<Krokosha_BuildingEntity_TrackerComponent_for_backgroundified>(ref krokosha_BuildingEntity_TrackerComponent_for_backgroundified) || krokosha_BuildingEntity_TrackerComponent_for_backgroundified.is_backgroundified || !si.go.TryGetComponent<BioTerminalScript>(ref val))
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
			OnUseRewritten(body, si.building);
		}
		ServerMain.Server_AnnounceSound(Vector2.op_Implicit(si.go.transform.position), "beep", ServerMain.GetListOfClientIdsExceptThisAndHost(clientId));
		if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
		{
			DebugHelp.OnNetEvent(Vector2.op_Implicit(si.go.transform.position), $"S: open BioTerminal {plr}");
		}
	}
}
