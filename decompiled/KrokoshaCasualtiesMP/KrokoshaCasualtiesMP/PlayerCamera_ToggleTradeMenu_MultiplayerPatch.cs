using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using HarmonyLib;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(PlayerCamera), "ToggleTradeMenu")]
internal static class PlayerCamera_ToggleTradeMenu_MultiplayerPatch
{
	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static UnityAction _003C_003E9__1_0;

		public static Comparison<NetPlayer> _003C_003E9__3_0;

		internal void _003CPrefix_003Eb__1_0()
		{
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)PlayerCamera.main.currentTrader != (Object)null && NetObjectRegistry.TryGetSyncInfoOrRegister((Component)(object)PlayerCamera.main.currentTrader.build, out var si))
			{
				NetDataWriter writer = Net.CreateWriter((Enum)NetmsgId.SERVER_TraderRecruit);
				writer.Put((ushort)si.syncId);
				Net.Client_Send((DeliveryMethod)0, in writer);
				PlayerCamera.main.ToggleTradeMenu();
			}
		}

		internal int _003CServer_SelectMostSuitablePlayerToRespawn_003Eb__3_0(NetPlayer a, NetPlayer b)
		{
			if (!a.is_alttab && b.is_alttab)
			{
				return -1;
			}
			if (a.server_plrstate.last_cursormove > b.server_plrstate.last_cursormove)
			{
				return -1;
			}
			if (a.server_plrstate.last_cursormove == b.server_plrstate.last_cursormove)
			{
				return 0;
			}
			return 1;
		}
	}

	public static GameObject RECRUITButton = null;

	public static int RANDOM_ITEMS_TO_GIVE_AFTER_RESPAWN = 3;

	private static bool Prefix(PlayerCamera __instance)
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Expected O, but got Unknown
		if (!Net.running)
		{
			return true;
		}
		GameObject traderMoveButon = __instance.traderMoveButon;
		if ((Object)(object)traderMoveButon != (Object)null && (Object)(object)RECRUITButton == (Object)null)
		{
			try
			{
				RECRUITButton = Object.Instantiate<GameObject>(traderMoveButon.gameObject, traderMoveButon.transform.parent, false);
				RECRUITButton.SetActive(true);
				((Object)RECRUITButton).name = "BUTTON_MP_recruit";
				RectTransform component = RECRUITButton.GetComponent<RectTransform>();
				Vector2 val = default(Vector2);
				((Vector2)(ref val))._002Ector(component.sizeDelta.x + 15f, 0f);
				component.offsetMax += val;
				component.offsetMin += val;
				((Transform)component).SetSiblingIndex(traderMoveButon.transform.GetSiblingIndex());
				((Component)RECRUITButton.transform.GetChild(0)).GetComponent<Image>().sprite = CoopModAssets.recruit;
				Button component2 = RECRUITButton.GetComponent<Button>();
				((UnityEventBase)component2.onClick).RemoveAllListeners();
				((UnityEventBase)component2.onClick).SetPersistentListenerState(0, (UnityEventCallState)0);
				ButtonClickedEvent onClick = component2.onClick;
				object obj = _003C_003Ec._003C_003E9__1_0;
				if (obj == null)
				{
					UnityAction val2 = delegate
					{
						//IL_004f: Unknown result type (might be due to invalid IL or missing references)
						if ((Object)(object)PlayerCamera.main.currentTrader != (Object)null && NetObjectRegistry.TryGetSyncInfoOrRegister((Component)(object)PlayerCamera.main.currentTrader.build, out var si))
						{
							NetDataWriter writer = Net.CreateWriter((Enum)NetmsgId.SERVER_TraderRecruit);
							writer.Put((ushort)si.syncId);
							Net.Client_Send((DeliveryMethod)0, in writer);
							PlayerCamera.main.ToggleTradeMenu();
						}
					};
					_003C_003Ec._003C_003E9__1_0 = val2;
					obj = (object)val2;
				}
				((UnityEvent)onClick).AddListener((UnityAction)obj);
			}
			catch (Exception ex)
			{
				log.error("RECRUITButton creation: " + ex.ToString());
			}
		}
		return true;
	}

	public static List<NetPlayer> GetPlayersEligibleForRespawn()
	{
		return new List<NetPlayer>(NetPlayer.AllDeadPlayers);
	}

	public static NetPlayer Server_SelectMostSuitablePlayerToRespawn()
	{
		List<NetPlayer> playersEligibleForRespawn = GetPlayersEligibleForRespawn();
		if (playersEligibleForRespawn.Count == 0)
		{
			return null;
		}
		playersEligibleForRespawn.Sort(delegate(NetPlayer a, NetPlayer b)
		{
			if (!a.is_alttab && b.is_alttab)
			{
				return -1;
			}
			if (a.server_plrstate.last_cursormove > b.server_plrstate.last_cursormove)
			{
				return -1;
			}
			return (a.server_plrstate.last_cursormove != b.server_plrstate.last_cursormove) ? 1 : 0;
		});
		return playersEligibleForRespawn[0];
	}

	[ServerReceiver(10173)]
	private static void ServerReceiver_RecruitPressed(knetid clientId, ref NetDataReader reader)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Invalid comparison between Unknown and I4
		reader.Get(out knetid result);
		if (!KrokoshaScavMultiplayer.rules.CanReviveFromTrader() || !TraderSync.Server_TraderInteractionCheck(clientId, result, "MP_RECRUIT", out var _, out var trader_si, out var trader_tracker))
		{
			return;
		}
		NetPlayer netPlayer = Server_SelectMostSuitablePlayerToRespawn();
		if (!((Object)(object)netPlayer != (Object)null) || !trader_tracker.CanBeRecruited_ForRespawn())
		{
			return;
		}
		log.l($"Recruiting {trader_si} -> to respawn {netPlayer}");
		netPlayer.Server_RespawnCharacter(trader_si.position + Vector2.up * 1.5f, level_transition: true);
		AmmoScript val2 = default(AmmoScript);
		for (int i = 0; i < Math.Min(trader_tracker.og.items.Count, RANDOM_ITEMS_TO_GIVE_AFTER_RESPAWN); i++)
		{
			int index = Random.Range(0, trader_tracker.og.items.Count);
			TraderItem obj = trader_tracker.og.items[index];
			trader_tracker.og.items.RemoveAt(index);
			GameObject val = Utils.Create(obj.id, Vector2.op_Implicit(trader_si.go.transform.position), 0f);
			if (val.TryGetComponent<AmmoScript>(ref val2) && (int)val2.itemType == 1)
			{
				val2.rounds = Random.Range((int)((float)val2.maxRounds * 0.5f), val2.maxRounds);
			}
			netPlayer.body.AutoPickUpItem(val.GetComponent<Item>());
		}
		Object.Destroy((Object)(object)trader_si.go);
	}
}
