using System;
using System.Collections.Generic;
using HarmonyLib;
using LiteNetLib;
using LiteNetLib.Utils;
using Unity.VisualScripting;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(WorldGeneration), "CreateExplosion")]
public static class WorldGeneration_CreateExplosion_MultiplayerPatch
{
	public static ExplosionParams last_explosion_params;

	public static bool is_inside_CreateExplosion;

	private static bool _forcerun_tempbool;

	public static event Action<ExplosionParams> OnExplosion;

	public static void ForceCreateExplosionEffect(ExplosionParams param)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		_forcerun_tempbool = true;
		ExplosionParams obj = Serialization.CloneViaSerialization<ExplosionParams>(param, false);
		obj.structuralDamage = 0f;
		obj.muscleDamage = new RangeF(0f, 0f);
		obj.skinDamage = new RangeF(0f, 0f);
		obj.skinDamageChance = 0f;
		obj.boneBreakChance = 0f;
		obj.dislocationChance = 0f;
		obj.disfigureChance = 0f;
		obj.bleedChance = 0f;
		obj.bleedAmount = new RangeF(0f, 0f);
		obj.structuralDamage = 0f;
		obj.shrapnelChance = 0f;
		WorldGeneration.CreateExplosion(obj);
		_forcerun_tempbool = false;
	}

	private static bool Prefix(ExplosionParams param)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		is_inside_CreateExplosion = true;
		last_explosion_params = param;
		if (!KrokoshaScavMultiplayer.network_system_is_running)
		{
			return true;
		}
		if (_forcerun_tempbool)
		{
			return true;
		}
		if (KrokoshaScavMultiplayer.is_client)
		{
			return false;
		}
		NetDataWriter writer = Net.CreateWriter(10008);
		writer.Put(param.position);
		writer.Put(param.velocity);
		writer.Put(param.range);
		Net.Server_SendToClients((DeliveryMethod)0, in writer, (IEnumerable<knetid>)ServerMain.AllClientIdsExceptHost);
		return true;
	}

	private static void Postfix(ExplosionParams param)
	{
		is_inside_CreateExplosion = false;
		try
		{
			WorldGeneration_CreateExplosion_MultiplayerPatch.OnExplosion?.Invoke(param);
		}
		catch (Exception)
		{
		}
	}
}
