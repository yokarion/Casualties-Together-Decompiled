using HarmonyLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(ScrapEaterScript), "OnUse")]
public static class ScrapEaterScript_OnUse_MultiplayerPatch
{
	private static void Prefix(ScrapEaterScript __instance)
	{
		Item val = default(Item);
		if (KrokoshaScavMultiplayer.network_system_is_running && PlayerCamera.main.body.FindByIdThorough("scrapmetal", ref val) && NetObjectRegistry.TryGetSyncInfo(((Component)__instance).gameObject, out var si))
		{
			si.SetIgnoreTimeForRoundTrip();
			KrokoshaScavMultiplayer.Client_SendSimpleMessageToServer((ushort)10086, (ushort)si.syncId, true);
		}
	}

	[ServerReceiver(10086)]
	private static void Server_ScrapEaterScriptUse(knetid clientId, ref NetDataReader reader)
	{
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out knetid result);
		ScrapEaterScript val = null;
		SyncInfo si = null;
		if (!NetPlayer.TryGetNetPlayerAndBodyFromClientId(clientId, out var plr, out var body) || !NetObjectRegistry.TryGetSyncInfo(result, out si) || !si.go.TryGetComponent<ScrapEaterScript>(ref val))
		{
			return;
		}
		if ((ushort)clientId != 0)
		{
			if (!plr.body.conscious || !si.IsBuilding())
			{
				return;
			}
			Component ba = (Component)(object)body;
			if (!KM.dist2dsqrcheck(in ba, (Component)(object)si.building, 20f))
			{
				return;
			}
		}
		Item val2 = default(Item);
		if (!body.FindByIdThorough("scrapmetal", ref val2))
		{
			return;
		}
		if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
		{
			DebugHelp.OnNetEvent(Vector2.op_Implicit(si.go.transform.position), $"S: ScrapEaterScript open {plr}, si:{si}");
		}
		if ((ushort)clientId != 0)
		{
			ScrapEaterScript obj = val;
			obj.scrapAmount += val2.condition;
			val2.condition = 0f;
			if (!(val.scrapAmount >= ScrapEaterScript.target))
			{
				return;
			}
			val.build.Backgroundify();
			Collider2D[] array = Physics2D.OverlapCircleAll(Vector2.op_Implicit(((Component)val).transform.position), 2f);
			BuildingEntity val3 = default(BuildingEntity);
			for (int i = 0; i < array.Length; i++)
			{
				if (((Component)array[i]).TryGetComponent<BuildingEntity>(ref val3) && val3.id == "reinforceddoor")
				{
					val3.Backgroundify();
				}
			}
		}
		ServerMain.Server_AnnounceSound(Vector2.op_Implicit(si.go.transform.position), "beep", ServerMain.GetListOfClientIdsExceptThis(clientId));
	}
}
