using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using KrokoshaCasualtiesUtils;
using LiteNetLib;
using LiteNetLib.Utils;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(SpiderHandler), "CheckForLimbDamage")]
public static class SpiderHandler_CheckForLimbDamage_MultiplayerPatch
{
	public static bool Prefix(SpiderHandler __instance, ref Collision2D collision)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		if (NetObjectRegistry.TryGetSyncInfo(((Component)__instance).gameObject, out var si))
		{
			Limb val = Body.LimbFromObject(collision.gameObject, Vector2.op_Implicit(((Component)__instance).transform.position));
			if ((Object)(object)val != (Object)null && val.TryGetNetBody(out var nb))
			{
				KrokoshaScavMultiGameObjectNetworkTracker krokoshaScavMultiGameObjectNetworkTracker = default(KrokoshaScavMultiGameObjectNetworkTracker);
				if (!nb.IsBodyLocal())
				{
					if (nb.body.conscious)
					{
						return false;
					}
				}
				else if (KrokoshaScavMultiplayer.is_client && nb.body.conscious && ((Component)__instance).TryGetComponent<KrokoshaScavMultiGameObjectNetworkTracker>(ref krokoshaScavMultiGameObjectNetworkTracker))
				{
					Vector2 val2 = Vector2.op_Implicit(collision.transform.position);
					ContactPoint2D contact = collision.GetContact(0);
					Vector2 val3 = val2 - ((ContactPoint2D)(ref contact)).point;
					if (Vector2.Dot(((Vector2)(ref val3)).normalized, Vector2.op_Implicit(((Component)__instance).transform.up)) > __instance.minVectorDotToBite)
					{
						if (log.verbose)
						{
							Plugin.log.LogInfo((object)$"DEV: SpiderJustBitMe {si} ");
						}
						KrokoshaScavMultiplayer.Client_SendSimpleMessageToServer((ushort)10097, krokoshaScavMultiGameObjectNetworkTracker.syncinfo.syncId, Util.GetLimbIndex(val), reliable: false);
						nb.SetNetHealthSyncIgnoreTime();
					}
				}
			}
		}
		return true;
	}

	public static void Spider_BiteDamage(SpiderHandler spider, Limb limb)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		KrokoshaSpiderTrackerComponent orAddComponent = ComponentHolderProtocol.GetOrAddComponent<KrokoshaSpiderTrackerComponent>((Object)(object)spider);
		if (spider is SpiderHandlerTBE)
		{
			spider.biteCooldown = 5f;
		}
		else
		{
			spider.biteCooldown = spider.biteCoolToSet;
		}
		Body body = limb.body;
		body.happiness -= spider.happinessLoss;
		Vector3 val = ((Component)spider).transform.position - ((Component)limb).transform.position;
		spider.target = Vector2.op_Implicit(((Vector3)(ref val)).normalized * 15f + ((Component)spider).transform.position);
		spider.moveTime = spider.retreatMoveTime;
		spider.DamageLimb(limb);
		if (spider.hitConnected)
		{
			Limb[] connectedLimbs = limb.connectedLimbs;
			foreach (Limb val2 in connectedLimbs)
			{
				spider.DamageLimb(val2);
			}
		}
		NetBody component = ((Component)limb.body).GetComponent<NetBody>();
		if (!KrokoshaScavMultiplayer.is_client && Object.op_Implicit((Object)(object)component) && Object.op_Implicit((Object)(object)orAddComponent.tracker))
		{
			NetDataWriter writer = Net.CreateWriter(10095);
			writer.Put((ushort)component.netId);
			writer.Put((ushort)orAddComponent.tracker.syncinfo.syncId);
			writer.Put(Vector2.op_Implicit(((Component)spider).transform.position));
			writer.Put(((Component)spider).transform.eulerAngles.z);
			Net.Server_SendToClients((DeliveryMethod)0, in writer, (IEnumerable<knetid>)ServerMain.GetListOfClientIdsExceptThis(component.netId));
		}
	}

	[ServerReceiver(10097)]
	private static void Server_SpiderJustBitMeFuck(knetid clientId, ref NetDataReader reader)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out knetid result);
		byte b = default(byte);
		reader.Get(ref b);
		if (NetPlayer.TryGetNetPlayerAndBodyFromClientId(clientId, out var plr, out var body) && body.conscious && b < body.limbs.Count() && NetObjectRegistry.TryGetSyncInfo(result, out var si) && si.IsSpider())
		{
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(Vector2.op_Implicit(((Component)plr.body).transform.position), "S: SpiderJustBitMeFuck");
			}
			Spider_BiteDamage(si.spider, body.limbs[b]);
		}
	}

	[ClientReceiver(10095, false)]
	private static void Client_SpiderAttackEffect(knetid _, ref NetDataReader reader)
	{
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out knetid result);
		reader.Get(out knetid result2);
		reader.Get(out Vector2 _);
		float num = default(float);
		reader.Get(ref num);
		if (NetPlayer.TryGetNetPlayerAndBodyFromClientId(result, out var plr, out var body) && NetObjectRegistry.TryGetSyncInfo(result2, out var si) && si.IsSpider())
		{
			SpiderHandler spider = si.spider;
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(Vector2.op_Implicit(((Component)plr.body).transform.position), "C: SpiderAttackEffect");
			}
			Sound.Play(spider.biteSound, Vector2.op_Implicit(((Component)spider).transform.position), false, true, (Transform)null, 1f, 1f, false, false);
			body.eyeScareTime = 5f;
			spider.PlayThreatMusic();
			if (spider.clawAnim)
			{
				GameObject obj = Object.Instantiate<GameObject>(Resources.Load<GameObject>("ClawAnim"));
				obj.transform.eulerAngles = new Vector3(0f, 0f, num);
				obj.transform.position = ((Component)spider).transform.position;
				obj.transform.SetParent(((Component)spider).transform);
				Object.Destroy((Object)(object)obj, 5f);
			}
		}
	}
}
