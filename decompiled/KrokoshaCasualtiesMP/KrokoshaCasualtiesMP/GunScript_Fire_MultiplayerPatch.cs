using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using KrokoshaCasualtiesUtils;
using LiteNetLib;
using LiteNetLib.Utils;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(GunScript), "Fire")]
public static class GunScript_Fire_MultiplayerPatch
{
	public static int MAX_HITS_PER_LIMB_AT_ONCE = 4;

	internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
	{
		List<CodeInstruction> instructionList = instructions.ToList();
		int i = 0;
		bool didit = false;
		while (i < instructionList.Count)
		{
			CodeInstruction val = instructionList[i];
			if (val.opcode == OpCodes.Call && val.operand is MethodInfo methodInfo && methodInfo != null && methodInfo.Name == "get_body" && methodInfo.DeclaringType == typeof(GunScript))
			{
				MethodInfo methodInfo2 = typeof(ComponentHolderProtocol).GetMethod("GetOrAddComponent", BindingFlags.Static | BindingFlags.Public).MakeGenericMethod(typeof(KrokoshaGunScriptTrackerComponent));
				MethodInfo getTheOtherPartMethod = typeof(KrokoshaGunScriptTrackerComponent).GetMethod("GetBody", BindingFlags.Instance | BindingFlags.Public);
				yield return new CodeInstruction(OpCodes.Call, (object)methodInfo2);
				yield return new CodeInstruction(OpCodes.Callvirt, (object)getTheOtherPartMethod);
				i++;
				didit = true;
			}
			else
			{
				yield return instructionList[i];
				i++;
			}
		}
		if (!didit)
		{
			Plugin.log.LogError((object)"GunScript_Fire_MultiplayerPatch: Failed to patch the body getters ");
		}
	}

	public static Vector2 RollVerticalSpread(GunScript gun)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		return Vector2.op_Implicit(((Component)gun).transform.up * (Random.Range(-1f, 1f) * gun.verticalSpread));
	}

	public static void Prefix(GunScript __instance, bool suicide, ref GunScript __state)
	{
		__state = GunScript_Update_MultiplayerPatch.current_executing_gun;
		if ((Object)(object)__state == (Object)null)
		{
			GunScript_Update_MultiplayerPatch.current_executing_gun = __instance;
		}
		KrokoshaGunScriptTrackerComponent orAddComponent = ComponentHolderProtocol.GetOrAddComponent<KrokoshaGunScriptTrackerComponent>((Object)(object)__instance);
		_ = __instance.shotsPerFire;
		Body body = PlayerCamera.main.body;
		if ((Object)(object)orAddComponent != (Object)null)
		{
			orAddComponent.ScanForBody();
			body = orAddComponent.body;
			_ = orAddComponent.shotsPerFire;
			orAddComponent.ApplyRecoil();
		}
		((Component)body).GetComponent<NetBody>();
		Util.IsBodyLocal(body);
	}

	private static void Postfix(GunScript __instance, bool suicide, ref GunScript __state)
	{
		try
		{
			_GunFirePostfix_CheckForHitsAndTellServer(__instance, suicide);
		}
		catch (Exception ex)
		{
			log.error(ex.ToString());
		}
		GunScript_Update_MultiplayerPatch.current_executing_gun = __state;
	}

	private static void _GunFirePostfix_CheckForHitsAndTellServer(GunScript gun, bool suicide)
	{
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Expected O, but got Unknown
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Expected O, but got Unknown
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		KrokoshaGunScriptTrackerComponent orAddComponent = ComponentHolderProtocol.GetOrAddComponent<KrokoshaGunScriptTrackerComponent>((Object)(object)gun);
		int shotsPerFire = gun.shotsPerFire;
		Body body = PlayerCamera.main.body;
		if ((Object)(object)orAddComponent != (Object)null)
		{
			orAddComponent.ScanForBody();
			body = orAddComponent.body;
			shotsPerFire = orAddComponent.shotsPerFire;
		}
		NetBody component = ((Component)body).GetComponent<NetBody>();
		bool flag = Util.IsBodyLocal(body);
		List<SyncInfo> list = new List<SyncInfo>();
		List<Limb> list2 = new List<Limb>();
		if (!flag && !suicide)
		{
			TurretScript_Shoot_MultiplayerPatch.HitOnlyStatic = true;
		}
		float num = (body.isRight ? 1f : (-1f));
		for (int i = 0; i < shotsPerFire; i++)
		{
			TurretScript_Shoot_MultiplayerPatch.LastActiveShooterIsSuicide = suicide;
			TurretScript_Shoot_MultiplayerPatch.LastActiveShooter = ((Component)body).GetComponent<NetBody>();
			TurretScript_Shoot_MultiplayerPatch.LastActiveShooterGun = orAddComponent;
			if (orAddComponent.override_shoot_trajectory)
			{
				TurretScript.Shoot(new FireInfo
				{
					pos = orAddComponent.override_shoot_trajectory_origin,
					dir = orAddComponent.override_shoot_trajectory_direction + RollVerticalSpread(gun) * num,
					doTinnitus = false,
					ignoreBody = false,
					structureDamage = gun.structureDamage,
					animalDamage = gun.animalDamage,
					forceHead = false
				});
			}
			else
			{
				TurretScript.Shoot(new FireInfo
				{
					pos = Vector2.op_Implicit(gun.barrel.position),
					dir = (suicide ? KM.normal(Vector2.op_Implicit(gun.barrel.position), body.limbs[0].rb.position) : ((Vector2.op_Implicit(((Component)gun).transform.right) + RollVerticalSpread(gun)) * num)),
					doTinnitus = false,
					ignoreBody = false,
					structureDamage = gun.structureDamage,
					animalDamage = gun.animalDamage,
					forceHead = false
				});
			}
			list.AddRange(TurretScript_Shoot_MultiplayerPatch.JustHitBuildings);
			list2.AddRange(TurretScript_Shoot_MultiplayerPatch.JustHitLimbs);
		}
		list2 = Util.LimitMaxRepeats(list2, Math.Min(orAddComponent.shotsPerFire, MAX_HITS_PER_LIMB_AT_ONCE));
		list2 = list2.Where((Limb l) =>
		{
			NetBody netBody = default(NetBody);
			return ((Component)l.body).TryGetComponent<NetBody>(ref netBody) ? true : false;
		}).ToList();
		orAddComponent.override_shoot_trajectory = false;
		orAddComponent.send_racked_state = true;
		if (KrokoshaScavMultiplayer.IsNetworkActiveAndIsClient())
		{
			TurretScript_Shoot_MultiplayerPatch.ApplyShootDamages(orAddComponent, list2, list);
		}
		if (KrokoshaScavMultiplayer.network_system_is_running)
		{
			if (flag)
			{
				orAddComponent.si.SetIgnoreTimeForRoundTrip(0.4000000059604645);
				if (!suicide)
				{
					NetDataWriter writer = Net.CreateWriter(10103);
					writer.Put((ushort)orAddComponent.si.syncId);
					writer.Put(Vector2.op_Implicit(gun.barrel.position));
					writer.Put(Vector2.op_Implicit(((Component)gun).transform.right * num));
					writer.Put(gun.racked);
					byte b = (byte)Math.Min(list2.Count, 40);
					writer.Put(b);
					for (byte b2 = 0; b2 < b; b2++)
					{
						Limb limb = list2.ElementAt(b2);
						writer.Put(new LimbNetId(limb));
					}
					byte b3 = (byte)Math.Min(list.Count, 40);
					writer.Put(b3);
					for (byte b4 = 0; b4 < b3; b4++)
					{
						SyncInfo syncInfo = list.ElementAt(b4);
						writer.Put((ushort)syncInfo.syncId);
					}
					Net.Client_Send((DeliveryMethod)0, in writer);
				}
			}
			if (suicide && !KrokoshaScavMultiplayer.is_client && !list2.Contains(component.head))
			{
				TurretScript_Shoot_MultiplayerPatch.Stolen_ShootApplyDamageLimb(component.head, orAddComponent, use_pvp_damage_model: false);
			}
		}
		TurretScript_Shoot_MultiplayerPatch.HitOnlyStatic = false;
	}

	private static bool ServerValidateShootHit(KrokoshaGunScriptTrackerComponent gst, Vector2 gun_barrelpos, Vector2 gun_direction, List<Limb> limbs_hit, List<SyncInfo> buildings_hit)
	{
		return true;
	}

	[ServerReceiver(10103)]
	private static void Server_PlayerBodyShoot(knetid clientId, ref NetDataReader reader)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out knetid result);
		if (!KrokoshaScavMultiplayer.IsInGameAndWorldGenerated() || !NetPlayer.TryGetNetPlayerAndNetBodyFromClientId(clientId, out var plr, out var pb) || !pb.body.conscious || !ItemSync.TryGetItemSyncInfo(result, out var si) || !si.IsGun() || (si.gun.roundsInMag <= 0 && (int)si.gun.roundInChamber != 0) || !ItemSync.CheckIfBodyReachThisItem(si, pb.body))
		{
			return;
		}
		KrokoshaGunScriptTrackerComponent orAddComponent = ComponentHolderProtocol.GetOrAddComponent<KrokoshaGunScriptTrackerComponent>((Object)(object)si.go);
		orAddComponent.body = pb.body;
		orAddComponent.pbody = pb;
		reader.Get(out Vector2 result2);
		reader.Get(out Vector2 result3);
		bool flag = default(bool);
		reader.Get(ref flag);
		((Vector2)(ref result3)).Normalize();
		if (result3 == Vector2.zero)
		{
			Plugin.log.LogWarning((object)$"SUS: PlayerBodyShoot: {plr} -> Weirdass gun shoot direction.");
			return;
		}
		if (!KM.dist2dsqrcheck(in result2, Vector2.op_Implicit(si.gun.barrel.position), 2f) && !KM.dist2dsqrcheck(in result2, Vector2.op_Implicit(((Component)pb.body.GetClosestLimb(result2)).transform.position), 1f))
		{
			Plugin.log.LogWarning((object)$"SUS: PlayerBodyShoot: {plr} -> Shoot origin is not near the guns actual barrel position.");
			return;
		}
		if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
		{
			DebugHelp.OnNetEvent(si.position, "S: PlayerBodyShoot " + ServerMain.GetPlayerFullDebugString(clientId), Color.magenta);
		}
		List<Limb> list = new List<Limb>();
		byte b = default(byte);
		reader.Get(ref b);
		for (byte b2 = 0; b2 < b; b2++)
		{
			reader.Get(out LimbNetId result4);
			if (result4.TryGetNetBodyAndLimbSafe(out var nb, out var limb))
			{
				if (pb.CanAttackThisGuy(nb.body))
				{
					list.Add(limb);
				}
				else
				{
					Plugin.log.LogWarning((object)$"SUS: PlayerBodyShoot: {plr} -> They hit someone they can't attack ({nb}).");
				}
			}
			else
			{
				Plugin.log.LogWarning((object)$"SUS: PlayerBodyShoot: {plr} -> They hit a non-existant limb????");
			}
		}
		list = Util.LimitMaxRepeats(list, Math.Min(orAddComponent.shotsPerFire, MAX_HITS_PER_LIMB_AT_ONCE));
		List<SyncInfo> list2 = new List<SyncInfo>();
		byte b3 = default(byte);
		reader.Get(ref b3);
		for (byte b4 = 0; b4 < b3; b4++)
		{
			reader.Get(out knetid result5);
			if (NetObjectRegistry.TryGetSyncInfo(result5, out var si2))
			{
				if (si2.IsBuilding() && NetObjectRegistry.IsObjectDynamic(si2.go))
				{
					if (si2.building.cantHit)
					{
						Plugin.log.LogWarning((object)$"SUS: PlayerBodyShoot: {plr} -> They hit a building with the cantHit flag on.");
					}
					else
					{
						list2.Add(si2);
					}
				}
				else
				{
					Plugin.log.LogWarning((object)$"SUS: PlayerBodyShoot: {plr} -> They hit a static building or it isnt even a building ({si2}).");
				}
			}
		}
		list2 = Util.LimitMaxRepeats(list2, orAddComponent.shotsPerFire);
		if (ServerValidateShootHit(orAddComponent, result2, result3, list, list2))
		{
			NetDataWriter writer = Net.CreateWriter(10104);
			writer.Put((ushort)clientId);
			writer.Put((ushort)orAddComponent.si.syncId);
			writer.Put(result2);
			writer.Put(result3);
			Net.Server_SendToClients((DeliveryMethod)0, in writer, (IEnumerable<knetid>)ServerMain.GetListOfClientIdsExceptThisAndHost(clientId));
			GunScript_JamChance_MultiplayerPatch.ForceJam = !si.gun.racked;
			if (!Util.IsBodyLocal(pb.body))
			{
				PlayerBodyShoot_VisualsTypeShit(clientId, orAddComponent.si, result2, result3);
			}
			GunScript_JamChance_MultiplayerPatch.ForceJam = false;
			si.gun.racked = flag;
			si.gun.lastRacked = false;
			if ((int)si.gun.firingMode != 0 && flag)
			{
				orAddComponent.gasTime = si.gun.desiredGasTime;
			}
			TurretScript_Shoot_MultiplayerPatch.ApplyShootDamages(orAddComponent, list, list2);
		}
	}

	[ClientReceiver(10104, false)]
	private static void Client_PlayerBodyShoot_Relay(knetid _, ref NetDataReader reader)
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out knetid result);
		reader.Get(out knetid result2);
		reader.Get(out Vector2 result3);
		reader.Get(out Vector2 result4);
		if (ItemSync.TryGetItemSyncInfo(result2, out var si) && si.IsGun())
		{
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(si.position, "C: PlayerBodyShoot_Relay " + ServerMain.GetPlayerFullDebugString(result));
			}
			PlayerBodyShoot_VisualsTypeShit(result, si, result3, result4);
		}
	}

	private static void PlayerBodyShoot_VisualsTypeShit(knetid shooter_clientid, SyncInfo gun_si, Vector2 gun_barrel_origin, Vector2 gun_barrel_direction)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Invalid comparison between Unknown and I4
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		GunScript gun = gun_si.gun;
		KrokoshaGunScriptTrackerComponent orAddComponent = ComponentHolderProtocol.GetOrAddComponent<KrokoshaGunScriptTrackerComponent>((Object)(object)gun_si.go);
		orAddComponent.override_shoot_trajectory = true;
		orAddComponent.override_shoot_trajectory_origin = gun_barrel_origin;
		orAddComponent.override_shoot_trajectory_direction = gun_barrel_direction;
		gun.triggerPressed = true;
		gun.firingPinStruck = false;
		if ((int)gun.roundInChamber != 0)
		{
			if ((int)gun.roundInChamber == 1)
			{
				Object obj = Object.Instantiate(Resources.Load("casing"), ((Component)gun).transform.position, ((Component)gun).transform.rotation);
				((GameObject)((obj is GameObject) ? obj : null)).GetComponent<Rigidbody2D>().velocity = Vector2.op_Implicit(((Component)gun).transform.up * 12f);
			}
			gun.roundInChamber = (RoundInChamber)0;
			gun.roundsInMag--;
		}
		gun.lastRacked = false;
		gun.racked = false;
		gun.safe = false;
		orAddComponent.ApplyRecoil();
		gun_si.gun.Update();
	}

	public static bool ValidateBulletHit(KrokoshaGunScriptTrackerComponent gst)
	{
		return false;
	}
}
