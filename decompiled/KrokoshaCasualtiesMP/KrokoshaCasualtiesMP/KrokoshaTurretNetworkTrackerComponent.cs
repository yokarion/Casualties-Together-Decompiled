using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using LiteNetLib;
using LiteNetLib.Utils;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public class KrokoshaTurretNetworkTrackerComponent : KrokoshaNetworkComponentTracker<TurretScript>
{
	private bool client_forceshoot;

	private bool last_didBeep;

	private bool last_didShoot;

	public float timeSinceFired
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return base.og.timeSinceFired;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			base.og.timeSinceFired = value;
		}
	}

	public float beepTime
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return base.og.beepTime;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			base.og.beepTime = value;
		}
	}

	public bool didShoot
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return base.og.didShoot;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			base.og.didShoot = value;
		}
	}

	protected override void TrackerAwake()
	{
	}

	public void NudgeShoot()
	{
		if (!didShoot)
		{
			base.og.didBeep = true;
			beepTime = 0.5f;
			didShoot = false;
		}
	}

	public void NudgeBeep(bool isVisible)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (!base.og.didBeep || (beepTime >= 0.5f && timeSinceFired > 14f))
		{
			last_didBeep = false;
			didShoot = false;
			beepTime = (isVisible ? 0f : (-0.5f));
			Sound.Play("turretsee", Vector2.op_Implicit(((Component)this).transform.position), true, false, (Transform)null, 1f, 1f, false, false);
			base.og.didBeep = true;
		}
	}

	private void Update()
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		if (!Net.running)
		{
			return;
		}
		SyncInfo si = null;
		if (KrokoshaScavMultiplayer.is_client)
		{
			KM.dist2dsqr(Vector2.op_Implicit(((Component)PlayerCamera.main).transform.position), Vector2.op_Implicit(((Component)this).transform.position));
			NetObjectRegistry.SyncRegistry.TryGetValue(((Component)this).gameObject, out si);
		}
		else if (NetBody.GetNearestBody(Vector2.op_Implicit(((Component)this).transform.position)).Item2 < 10000f)
		{
			NetObjectRegistry.TryGetSyncInfoOrRegister(((Component)this).gameObject, out si);
		}
		if (!last_didBeep && base.og.didBeep)
		{
			if (KrokoshaScavMultiplayer.is_client)
			{
				NetDataWriter writer = Net.CreateWriter((Enum)NetmsgId.SERVER_TurretDoBeep);
				writer.Put(Vector2.op_Implicit(((Component)this).transform.position));
				writer.Put(((Renderer)base.og.spr).isVisible);
				Net.Client_Send((DeliveryMethod)0, in writer);
			}
			else
			{
				NetObjectRegistry.Server_ObjectSyncSingle(((Component)this).gameObject);
				if (si != null || NetObjectRegistry.TryGetSyncInfo(((Component)this).gameObject, out si))
				{
					KrokoshaScavMultiplayer.Server_SendRelayMessageToClients((ushort)10149, si.syncId);
					NetDataWriter writer2 = Net.CreateWriter((Enum)NetmsgId.CLIENT_TurretDoBeep_Relay);
					writer2.Put((ushort)si.syncId);
					writer2.Put(beepTime >= 0f);
					Net.Server_SendToClients((DeliveryMethod)0, in writer2, (IEnumerable<knetid>)ServerMain.AllClientIdsExceptHost);
				}
			}
		}
		if (!last_didShoot && didShoot)
		{
			NetBody pb;
			if (!KrokoshaScavMultiplayer.is_client)
			{
				if (si != null || NetObjectRegistry.TryGetSyncInfo(((Component)this).gameObject, out si))
				{
					KrokoshaScavMultiplayer.Server_SendRelayMessageToClients((ushort)10150, si.syncId);
				}
			}
			else if (NetPlayer.LOCAL_PLAYER.TryGetNetBody(out pb))
			{
				pb.SetNetHealthSyncIgnoreTime();
			}
		}
		last_didBeep = base.og.didBeep;
		last_didShoot = didShoot;
	}

	[ServerReceiver(10148)]
	private static void Server_TurretDoBeep(knetid clientId, ref NetDataReader reader)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out Vector2 result);
		bool isVisible = default(bool);
		reader.Get(ref isVisible);
		if (!NetPlayer.TryGetNetPlayerAndBodyFromClientId(clientId, out var plr, out var body) || !body.alive || !KM.dist2dsqrcheck_presqr(in result, Vector2.op_Implicit(((Component)body).transform.position), 10000f))
		{
			return;
		}
		if (Math.Abs(result.y - ((Component)body).transform.position.y) > 8f)
		{
			Plugin.log.LogWarning((object)"TEMP DEV, TurretDoBeep  turret IS NOT ON THE SAME LEVEL AS THE PLR  BRAAHH !!! ");
			return;
		}
		Collider2D[] array = Physics2D.OverlapCircleAll(result, 1f);
		TurretScript val = default(TurretScript);
		for (int i = 0; i < array.Length; i++)
		{
			if (((Component)array[i]).TryGetComponent<TurretScript>(ref val) && Vector2.op_Implicit(((Component)val).transform.position) == result)
			{
				ComponentHolderProtocol.GetOrAddComponent<KrokoshaTurretNetworkTrackerComponent>((Object)(object)val).NudgeBeep(isVisible);
				if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
				{
					DebugHelp.OnNetEvent(result, $"S: TurretDoBeep {plr} ");
				}
				return;
			}
		}
		Plugin.log.LogWarning((object)"TEMP DEV, TurretDoBeep  turret was not found !!!!!!!!!!!!!!!!!!!!!!!! ");
	}

	[ClientReceiver(10149, true)]
	private static void Client_TurretDoBeep_Relay(knetid _, ref NetDataReader reader)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out knetid result);
		bool isVisible = default(bool);
		reader.Get(ref isVisible);
		if (NetObjectRegistry.TryGetSyncInfo(result, out var si))
		{
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(Vector2.op_Implicit(si.go.transform.position), "C: TurretDoBeep_Relay  ");
			}
			ComponentHolderProtocol.GetOrAddComponent<KrokoshaTurretNetworkTrackerComponent>((Object)(object)si.go).NudgeBeep(isVisible);
		}
	}

	[ClientReceiver(10150, true)]
	private static void Client_TurretDoShoot_Relay(knetid _, ref NetDataReader reader)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out knetid result);
		if (NetObjectRegistry.TryGetSyncInfo(result, out var si))
		{
			KrokoshaTurretNetworkTrackerComponent orAddComponent = ComponentHolderProtocol.GetOrAddComponent<KrokoshaTurretNetworkTrackerComponent>((Object)(object)si.go);
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(Vector2.op_Implicit(si.go.transform.position), $"C: TurretDoShoot_Relay cur_shoot:{orAddComponent.didShoot}  ");
			}
			orAddComponent.NudgeShoot();
		}
	}
}
