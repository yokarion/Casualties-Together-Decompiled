using HarmonyLib;
using KrokoshaCasualtiesUtils;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(PlayerCamera), "WoundSpecialAction")]
public static class PlayerCamera_WoundSpecialAction_MultiplayerPatch
{
	public static void OG_TakeOffReimplemented(Body healer, TourniquetScript tourniquet)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		Limb component = ((Component)tourniquet).GetComponent<Limb>();
		NetBody nb;
		if (!KrokoshaScavMultiplayer.IsNetworkActiveAndIsClient())
		{
			GameObject val = Utils.Create("tourniquet", Vector2.op_Implicit(((Component)tourniquet).transform.position), 0f);
			val.GetComponent<Item>().condition = tourniquet.condition;
			healer.AutoPickUpItem(val.GetComponent<Item>());
		}
		else if (component.body.IsBodyLocal() && healer.TryGetNetBody(out nb))
		{
			nb.SetNetHealthSyncIgnoreTime(0.05f);
		}
		tourniquet.affectedLimbs.ForEach(delegate(Limb x)
		{
			x.blockedBleeding = false;
		});
		Object.Destroy((Object)(object)tourniquet);
	}

	public static void OG_TakeOffReimplemented(Body healer, SplintLimb splint)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		Limb component = ((Component)splint).GetComponent<Limb>();
		component.splinted = false;
		NetBody nb;
		if (!KrokoshaScavMultiplayer.IsNetworkActiveAndIsClient())
		{
			GameObject val = Utils.Create(splint.item, Vector2.op_Implicit(((Component)splint).transform.position), 0f);
			val.GetComponent<Item>().condition = splint.condition;
			healer.AutoPickUpItem(val.GetComponent<Item>());
		}
		else if (component.body.IsBodyLocal() && healer.TryGetNetBody(out nb))
		{
			nb.SetNetHealthSyncIgnoreTime(0.05f);
		}
		Object.Destroy((Object)(object)splint);
	}

	public static void OG_WoundSpecialAction(Body healer, Limb target_limb)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Expected O, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Expected O, but got Unknown
		_ = target_limb.body;
		bool flag = Util.IsBodyLocal(healer);
		if (healer.conscious)
		{
			TourniquetScript tourniquet = default(TourniquetScript);
			if (((Component)target_limb).TryGetComponent<TourniquetScript>(ref tourniquet))
			{
				OG_TakeOffReimplemented(healer, tourniquet);
			}
			else if (target_limb.hasShrapnel && flag)
			{
				MinigameBase_StartMinigame_MultiplayerPatch.ignore_next_mgstart_cuz_its_not_me = false;
				MinigameBase.main.StartMinigame((Minigame)new ShrapnelMinigame(target_limb, false), (Item)null);
			}
			SplintLimb splint = default(SplintLimb);
			if (((Component)target_limb).TryGetComponent<SplintLimb>(ref splint))
			{
				OG_TakeOffReimplemented(healer, splint);
			}
			else if (target_limb.dislocated && flag)
			{
				MinigameBase_StartMinigame_MultiplayerPatch.ignore_next_mgstart_cuz_its_not_me = false;
				MinigameBase.main.StartMinigame((Minigame)new DislocationMinigame(target_limb, false), (Item)null);
			}
		}
		if (healer.TryGetNetBody(out var nb) && KrokoshaScavMultiplayer.IsNetworkActiveAndIsClient())
		{
			nb.SetNetHealthSyncIgnoreTime(0.01f);
		}
	}

	public static bool Prefix(PlayerCamera __instance)
	{
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		if (!KrokoshaScavMultiplayer.network_system_is_running)
		{
			return true;
		}
		if (__instance.body.allowUseItem)
		{
			if (!__instance.body.conscious)
			{
				return false;
			}
			if (!Util.DoFullInteractionCheck(__instance.body, __instance.selectedLimb.body, do_effect: true))
			{
				PlayerCamera.main.DoAlert(Lang.Get("plr_unreachable", false), false);
				PlayerCamera.main.PlayUISound((UISoundType)6, 1f);
				return false;
			}
			if (!__instance.body.allowUseItem)
			{
				PlayerCamera.main.UseFailUnhappiness();
				return false;
			}
			NetBody netBody = default(NetBody);
			if (((Component)__instance.selectedLimb.body).TryGetComponent<NetBody>(ref netBody))
			{
				if (log.verbose)
				{
					log.l("CLIENT: sending PlayerCamera.WoundSpecialAction ");
				}
				NetDataWriter writer = Net.CreateWriter(10125);
				writer.Put(new LimbNetId(__instance.selectedLimb));
				Net.Client_Send((DeliveryMethod)0, in writer);
				if (KrokoshaScavMultiplayer.is_client)
				{
					netBody.SetNetHealthSyncIgnoreTime(0.05f);
				}
			}
			OG_WoundSpecialAction(__instance.body, __instance.selectedLimb);
		}
		else
		{
			PlayerCamera.main.UseFailUnhappiness();
		}
		return false;
	}
}
