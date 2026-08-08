using System.Collections.Generic;
using HarmonyLib;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(MinigameBase), "StartMinigame")]
public static class MinigameBase_StartMinigame_MultiplayerPatch
{
	public static byte selfharmminigame_last_cuts_count = 0;

	public static bool ignore_next_mgstart_cuz_its_not_me = false;

	public static Dictionary<string, OnBandageUse> known_bandages_onuse_lambdas = new Dictionary<string, OnBandageUse>();

	private static void ResetMinigameOverriderStates()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		DislocationMinigame_CheckForHit_MultiplayerPatch.lastreceived_bonepos = new Vector2(500f, 0f);
		DislocationMinigame_CheckForHit_MultiplayerPatch.lastreceived_boneVelocity = Vector2.zero;
		AmputationMinigame_PhysicsUpdate_MultiplayerPatch.last_cutProgress = 0f;
		MinigameMPManager.client_i_know_my_minigame_session = false;
		for (int i = 0; i < MinigameMPManager.ShrapnelMinigameSession.client_shrapnel_owners.Count; i++)
		{
			MinigameMPManager.ShrapnelMinigameSession.client_shrapnel_owners[i] = null;
		}
	}

	private static bool Prefix(MinigameBase __instance, Minigame minigame, Item item, ref bool __state)
	{
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Expected O, but got Unknown
		__state = __instance.currentMinigame != null;
		Traverse val = Traverse.Create((object)minigame).Field("limb");
		bool flag = val.FieldExists();
		if (flag)
		{
			BandageMinigame val2 = (BandageMinigame)(object)((minigame is BandageMinigame) ? minigame : null);
			if (val2 != null)
			{
				known_bandages_onuse_lambdas[item.id] = val2.OnUse;
				BandageMinigame_DoBandageAction_MultiplayerPatch.bandagethingcounter = 14;
			}
		}
		if (!KrokoshaScavMultiplayer.network_system_is_running)
		{
			return true;
		}
		if (__instance.currentMinigame != null)
		{
			return false;
		}
		if (val != null)
		{
			val.GetValue<Limb>();
		}
		if (Traverse.Create((object)minigame).Field("toDestroy").FieldExists())
		{
			if (minigame is LockpingMinigame)
			{
				_ = 1;
			}
			else
				_ = minigame is KeypadMinigame;
		}
		else
			_ = 0;
		if (ignore_next_mgstart_cuz_its_not_me)
		{
			ignore_next_mgstart_cuz_its_not_me = false;
			return false;
		}
		if (flag)
		{
			if (flag && (minigame is ShrapnelMinigame || minigame is DislocationMinigame || minigame is CPRMinigame))
			{
				ShrapnelMinigame_Update_MultiplayerPatch.last_packet_was_a_change = false;
				PlayerCamera_ApplyWoundItem_MultiplayerPatch.applying_on_limb = val.GetValue<Limb>();
			}
			else if ((Object)(object)PlayerCamera_ApplyWoundItem_MultiplayerPatch.applying_on_limb == (Object)null)
			{
				return false;
			}
			if (minigame is SyringeMinigame)
			{
				WaterContainerItem_Inject_MultiplayerPatch.listen_to_limb = (Limb)val.GetValue();
				WaterContainerItem_Inject_MultiplayerPatch.listen_to_limb_amount = 0f;
				SyringeMinigame_Update_MultiplayerPatch.last_fill_amount = item.condition;
				SyringeMinigame_Update_MultiplayerPatch.last_was_different = false;
				SyringeMinigame_Update_MultiplayerPatch.is_start_or_end = true;
				SyringeMinigame_Update_MultiplayerPatch.fill_milestone_timer = 0.5f;
			}
			else if (!(minigame is DislocationMinigame) && minigame is AmputationMinigame)
			{
				if (!((Object)(object)PlayerCamera_ApplyWoundItem_MultiplayerPatch.applying_item != (Object)null))
				{
					return false;
				}
				if (!NetObjectRegistry.TryGetSyncInfo((Component)(object)PlayerCamera_ApplyWoundItem_MultiplayerPatch.applying_item, out var _))
				{
					NetObjectRegistry.AlertObjectNotRegistered(popup: true);
					return false;
				}
				AmputationMinigame_PhysicsUpdate_MultiplayerPatch.cutter_item = PlayerCamera_ApplyWoundItem_MultiplayerPatch.applying_item;
			}
		}
		ResetMinigameOverriderStates();
		return true;
	}

	private static void Postfix(MinigameBase __instance, Minigame minigame, Item item, ref bool __state)
	{
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		if (!KrokoshaScavMultiplayer.network_system_is_running || !Minigame.op_Implicit(__instance.currentMinigame) || KrokoshaScavMultiplayer.is_dedicated_server)
		{
			return;
		}
		NetPlayer lOCAL_PLAYER = NetPlayer.LOCAL_PLAYER;
		lOCAL_PLAYER.minigame_current_type = MinigameMPManager.GetMinigameTypeId(((object)minigame).GetType());
		Traverse val = Traverse.Create((object)minigame).Field("limb");
		bool num = val.FieldExists();
		Limb val2 = ((val != null) ? val.GetValue<Limb>() : null);
		Traverse val3 = Traverse.Create((object)minigame).Field("toDestroy");
		bool flag = val3.FieldExists() && (minigame is LockpingMinigame || minigame is KeypadMinigame);
		AnyObjectNetId value = new AnyObjectNetId();
		NetBody netBody = default(NetBody);
		if (num && Object.op_Implicit((Object)(object)val2) && ((Component)val2.body).TryGetComponent<NetBody>(ref netBody))
		{
			value = new AnyObjectNetId(val.GetValue<Limb>());
		}
		bool flag2 = __state;
		if (flag)
		{
			BuildingEntity value2 = val3.GetValue<BuildingEntity>();
			if (!NetObjectRegistry.ObjectCanBeIgnoredForNetwork(((Component)value2).gameObject))
			{
				if (!NetObjectRegistry.TryGetSyncInfoOrRegister((Component)(object)value2, out var si))
				{
					Plugin.log.LogWarning((object)$"Attempted to open a minigame tied to an unregistered object {value2.id} (at {Vector2.op_Implicit(((Component)value2).transform.position)}), aborting.");
					NetObjectRegistry.AlertObjectNotRegistered(popup: true);
					__instance.EndMinigame();
					return;
				}
				value = new AnyObjectNetId(si);
			}
			else
			{
				flag2 = true;
			}
		}
		if (flag2)
		{
			return;
		}
		KeypadMinigame val4 = (KeypadMinigame)(object)((minigame is KeypadMinigame) ? minigame : null);
		if (val4 != null && KrokoshaScavMultiplayer.is_client)
		{
			val4.match = "-- LOADING --";
		}
		knetid knetid2 = (ushort)0;
		if ((Object)(object)MinigameBase.main.currentItem != (Object)null)
		{
			if (NetObjectRegistry.TryGetSyncInfo((Component)(object)MinigameBase.main.currentItem, out var si2))
			{
				knetid2 = si2.syncId;
			}
			else
			{
				NetObjectRegistry.AlertObjectNotRegistered(popup: true);
			}
		}
		NetDataWriter writer = Net.CreateWriter(10062);
		writer.Put(lOCAL_PLAYER.minigame_current_type);
		writer.Put((ushort)knetid2);
		writer.Put(value);
		Net.Client_Send((DeliveryMethod)2, in writer);
	}
}
