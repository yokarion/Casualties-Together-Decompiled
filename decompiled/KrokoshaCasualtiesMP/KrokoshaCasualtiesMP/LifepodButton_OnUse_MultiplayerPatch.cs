using System.Collections.Generic;
using HarmonyLib;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(LifepodButton), "OnUse")]
public static class LifepodButton_OnUse_MultiplayerPatch
{
	private static bool Prefix(LifepodButton __instance)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if (KrokoshaScavMultiplayer.network_system_is_running)
		{
			BuildingEntity component = ((Component)((Component)__instance).transform.parent).GetComponent<BuildingEntity>();
			if (Object.op_Implicit((Object)(object)component) && !NetObjectRegistry.TryGetSyncInfoOrRegister(((Component)component).gameObject, out var _))
			{
				log.warn($"Attempted to use unregistered lifepod buttons {component.id} (at {Vector2.op_Implicit(((Component)component).transform.position)}), aborting.");
				NetObjectRegistry.AlertObjectNotRegistered(popup: true);
				return false;
			}
		}
		return true;
	}

	private static void Postfix(LifepodButton __instance)
	{
		if (!KrokoshaScavMultiplayer.network_system_is_running)
		{
			return;
		}
		BuildingEntity component = ((Component)((Component)__instance).transform.parent).GetComponent<BuildingEntity>();
		if (Object.op_Implicit((Object)(object)component) && NetObjectRegistry.TryGetSyncInfo(((Component)component).gameObject, out var si))
		{
			if (log.verbose)
			{
				log.l($"Lifepod button pressed: type:{__instance.type} si:{si} ");
			}
			KrokoshaScavMultiplayer.Client_SendSimpleMessageToServer((ushort)10084, si.syncId, (byte)__instance.type, (byte)__instance.controller.heatState);
		}
	}

	[ServerReceiver(10084)]
	private static void Server_LifepodButtonActivate(knetid clientId, ref NetDataReader reader)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out knetid result);
		byte b = default(byte);
		reader.Get(ref b);
		byte b2 = default(byte);
		reader.Get(ref b2);
		SyncInfo si = null;
		if (NetPlayer.TryGetNetPlayerAndBodyFromClientId(clientId, out var plr, out var body) && NetObjectRegistry.TryGetSyncInfo(result, out si) && si.IsBuilding() && KM.dist2dsqrcheck(Vector2.op_Implicit(si.go.transform.position), Vector2.op_Implicit(((Component)body).transform.position), 20f))
		{
			LifepodController componentInChildren = si.go.GetComponentInChildren<LifepodController>();
			if ((Object)(object)componentInChildren != (Object)null)
			{
				if (log.verbose)
				{
					log.l(string.Format("SERVER: Lifepod button pressed: {0}  type:{1}:{2}  heatstate:{3} si:{4} ", plr, b, (b == 0) ? "HEATER" : "SHOWER", b2, si));
				}
				if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
				{
					DebugHelp.OnNetEvent(Vector2.op_Implicit(si.go.transform.position), string.Format("S: LifepodButtonActivate {0}:{1} HEAT:{2}", b, (b == 0) ? "HEATER" : "SHOWER", b2));
				}
				if (b != 0)
				{
					if ((Object)(object)componentInChildren.shower != (Object)null)
					{
						componentInChildren.ActivateShower();
					}
					else
					{
						log.error_devevent($"LIFEPOD SHOWER BUTTON: {si} DOES NOT HAVE A SHOWER ???????????", si.position);
					}
				}
				componentInChildren.heatState = Mathf.Max(-1, b2 - 1);
				if ((Object)(object)componentInChildren.heater != (Object)null)
				{
					componentInChildren.ToggleHeatState();
				}
				else
				{
					log.error_devevent($"LIFEPOD HEAT TOGGLE: {si} DOES NOT HAVE A HEATER ???????????", si.position);
				}
				NetDataWriter writer = Net.CreateWriter(10081);
				writer.Put((ushort)result);
				writer.Put(b);
				writer.Put((byte)componentInChildren.heatState);
				Net.Server_SendToClients((DeliveryMethod)0, in writer, (IEnumerable<knetid>)ServerMain.AllClientIdsExceptHost);
				return;
			}
		}
		log.serverdeny($"LifepodButton DENIED, something seems sus: user:{plr} si:{si}");
	}

	[ClientReceiver(10081, true)]
	private static void Client_LifepodButtonActivateRelay(knetid __, ref NetDataReader reader)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out knetid result);
		byte b = default(byte);
		reader.Get(ref b);
		byte b2 = default(byte);
		reader.Get(ref b2);
		if (!NetObjectRegistry.TryGetSyncInfo(result, out var si) || !si.IsBuilding())
		{
			return;
		}
		LifepodController componentInChildren = si.go.GetComponentInChildren<LifepodController>();
		if ((Object)(object)componentInChildren != (Object)null)
		{
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(Vector2.op_Implicit(si.go.transform.position), $"C: LifepodButtonActivate {b} {b2}");
			}
			if (b != 0)
			{
				componentInChildren.ActivateShower();
			}
			componentInChildren.heatState = Mathf.Max(-1, b2 - 1);
			componentInChildren.ToggleHeatState();
		}
	}
}
