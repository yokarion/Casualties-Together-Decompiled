using System.Collections.Generic;
using HarmonyLib;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(MineScript), "OnCollisionEnter2D")]
public static class TrapsPatch_MineScriptCollisionOverride
{
	public static bool Prefix(MineScript __instance, Collision2D collision)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		if (__instance.pressed)
		{
			return false;
		}
		if (KrokoshaScavMultiplayer.network_system_is_running)
		{
			if (!KrokoshaScavMultiplayer.is_client)
			{
				if (Object.op_Implicit((Object)(object)collision.collider.attachedRigidbody) && !collision.collider.attachedRigidbody.isKinematic && NetBody.GetNearestBody(Vector2.op_Implicit(((Component)__instance).transform.position)).Item2 < 4096f)
				{
					__instance.pressed = true;
					Sound.Play("mine", Vector2.op_Implicit(((Component)__instance).transform.position), false, true, (Transform)null, 1f, 1f, false, false);
					((Component)__instance).GetComponent<SpriteRenderer>().sprite = __instance.pressedSprite;
					NetDataWriter writer = Net.CreateWriter(10146);
					writer.Put(Vector2.op_Implicit(((Component)__instance).transform.position));
					Net.Server_SendToClients((DeliveryMethod)4, in writer, (IEnumerable<knetid>)ServerMain.AllClientIdsExceptHost);
				}
				return false;
			}
			__instance.exploded = true;
		}
		return true;
	}
}
