using System;
using System.Collections.Generic;
using HarmonyLib;
using KrokoshaCasualtiesUtils;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(WorldGeneration), "DamageBlock", new Type[]
{
	typeof(Vector2Int),
	typeof(float),
	typeof(bool),
	typeof(bool),
	typeof(bool)
})]
public static class WorldGenerationDamageBlockAnnouncePatch
{
	public static NetBody cur_damaging_netbody;

	public static void Postfix(WorldGeneration __instance, Vector2Int pos, float dmg, bool hitSound = true, bool bonusMetal = false, bool ignoreLoot = false)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		if (!KrokoshaScavMultiplayer.network_system_is_running)
		{
			return;
		}
		if (!KrokoshaScavMultiplayer.is_client)
		{
			BlockDamage blockDamage = __instance.GetBlockDamage(pos);
			if (blockDamage != null)
			{
				NetDataWriter writer = Net.CreateWriter(10004);
				if ((Object)(object)cur_damaging_netbody != (Object)null)
				{
					writer.Put((ushort)cur_damaging_netbody.netId);
				}
				else
				{
					writer.Put((ushort)(knetid)(ushort)0);
				}
				writer.Put(pos);
				writer.Put(blockDamage.damage);
				Net.Server_SendToClients((DeliveryMethod)4, in writer, (IEnumerable<knetid>)ServerMain.AllClientIdsExceptHost);
			}
		}
		else
		{
			Body localBody = Util.GetLocalBody();
			_ = (Object)(object)localBody != (Object)null;
		}
	}
}
