using HarmonyLib;
using KrokoshaCasualtiesUtils;
using LiteNetLib.Utils;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(StalactiteDropper), "OnWillRenderObject")]
public static class StalactiteDropper_OnWillRenderObject_MultiplayerPatch
{
	private static void Prefix(StalactiteDropper __instance)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		if (!KrokoshaScavMultiplayer.IsNetworkActiveAndIsClient() || (int)((Component)__instance).GetComponent<Rigidbody2D>().bodyType == 0)
		{
			return;
		}
		RaycastHit2D val = Physics2D.Raycast(Vector2.op_Implicit(((Component)__instance).transform.position), Vector2.down, 40f, LayerMask.GetMask(new string[2] { "Body", "Limb" }));
		Limb val2 = default(Limb);
		Body body = default(Body);
		if (RaycastHit2D.op_Implicit(val) && NetObjectRegistry.TryGetSyncInfoOrRegister((Component)(object)__instance, out var si) && ((((Component)((RaycastHit2D)(ref val)).collider).TryGetComponent<Limb>(ref val2) && Util.IsBodyLocal(val2.body)) || (((Component)((RaycastHit2D)(ref val)).collider).TryGetComponent<Body>(ref body) && Util.IsBodyLocal(body))))
		{
			ScavTraps.DamagingCrateCooldownThing orAddComponent = ComponentHolderProtocol.GetOrAddComponent<ScavTraps.DamagingCrateCooldownThing>((Object)(object)__instance);
			if (!orAddComponent.stalagtite_dropped)
			{
				orAddComponent.stalagtite_dropped = true;
				KrokoshaScavMultiplayer.Client_SendSimpleMessageToServer((ushort)10151, (ushort)si.syncId, false);
			}
		}
	}

	[ServerReceiver(10151)]
	private static void ServerReceiver_ITriggeredStalactiteDropperDropCounter(knetid clientId, ref NetDataReader reader)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out knetid result);
		StalactiteDropper val = default(StalactiteDropper);
		if (NetPlayer.TryGetNetPlayerAndNetBodyFromClientId(clientId, out var _, out var pb) && pb.alive && NetObjectRegistry.TryGetSyncInfo(result, out var si) && si.go.TryGetComponent<StalactiteDropper>(ref val) && ((Component)pb).transform.position.y < ((Component)val).transform.position.y && ((Component)pb).transform.position.y + 50f > ((Component)val).transform.position.y && Mathf.Abs(((Component)pb).transform.position.x - ((Component)val).transform.position.x) < 5f)
		{
			val.counting = true;
		}
	}
}
