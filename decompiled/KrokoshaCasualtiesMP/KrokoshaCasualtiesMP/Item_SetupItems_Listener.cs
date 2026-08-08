using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using HarmonyLib;
using KrokoshaCasualtiesUtils;
using LiteNetLib;
using LiteNetLib.Utils;
using TMPro;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(Item), "SetupItems")]
public static class Item_SetupItems_Listener
{
	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static Use _003C_003E9__2_0;

		public static Use _003C_003E9__2_1;

		public static Use _003C_003E9__2_2;

		public static Use _003C_003E9__2_3;

		public static Use _003C_003E9__2_4;

		public static Use _003C_003E9__2_5;

		public static UseLimb _003C_003E9__2_6;

		public static Use _003C_003E9__2_7;

		public static Use _003C_003E9__2_8;

		public static Use _003C_003E9__2_9;

		public static OnHealthUse _003C_003E9__2_10;

		internal void _003CPostfix_003Eb__2_0(Body body, Item item)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			if (!body.canPlaceBlock || item.condition < 0.24f)
			{
				return;
			}
			Vector3 position = ((Component)body).transform.position;
			Vector3 val = body.targetLookPos - ((Component)body).transform.position;
			Vector2 val2 = Vector2.op_Implicit(position + ((Vector3)(ref val)).normalized * 5f);
			RaycastHit2D val3 = Physics2D.Linecast(Vector2.op_Implicit(((Component)body).transform.position), val2, LayerMask.GetMask(new string[1] { "Ground" }));
			if (RaycastHit2D.op_Implicit(val3))
			{
				val2 = ((RaycastHit2D)(ref val3)).point + ((RaycastHit2D)(ref val3)).normal * 0.2f;
			}
			body.armsAnimator.Play("ArmsSwing", -1, 0f);
			Vector2Int val4 = WorldGeneration.world.WorldToBlockPos(val2);
			if (WorldGeneration.world.GetBlock(val4) > 0)
			{
				return;
			}
			Sound.Play("scrapmetal", val2, false, true, (Transform)null, 1f, 1f, false, false);
			if (Util.IsBodyLocal(body))
			{
				WorldGeneration.world.SetBlock(val4, (ushort)3);
				item.condition -= 0.25f;
				if (ItemSync.TryGetSyncInfo(item, out var si))
				{
					NetDataWriter writer = Net.CreateWriter(10111);
					writer.Put((ushort)si.syncId);
					writer.Put(val2);
					Net.Client_Send((DeliveryMethod)0, in writer);
				}
			}
		}

		internal void _003CPostfix_003Eb__2_1(Body body, Item item)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			if (!body.canPlaceBlock)
			{
				return;
			}
			Vector3 position = ((Component)body).transform.position;
			Vector3 val = body.targetLookPos - ((Component)body).transform.position;
			Vector2 val2 = Vector2.op_Implicit(position + ((Vector3)(ref val)).normalized * 5f);
			RaycastHit2D val3 = Physics2D.Linecast(Vector2.op_Implicit(((Component)body).transform.position), val2, LayerMask.GetMask(new string[1] { "Ground" }));
			if (RaycastHit2D.op_Implicit(val3))
			{
				val2 = ((RaycastHit2D)(ref val3)).point + ((RaycastHit2D)(ref val3)).normal * 0.2f;
			}
			body.armsAnimator.Play("ArmsSwing", -1, 0f);
			Vector2Int val4 = WorldGeneration.world.WorldToBlockPos(val2);
			if (WorldGeneration.world.GetBlock(val4) > 0)
			{
				return;
			}
			Sound.Play("scrapmetal", val2, false, true, (Transform)null, 1f, 1f, false, false);
			if (Util.IsBodyLocal(body))
			{
				WorldGeneration.world.SetBlock(val4, (ushort)21);
				item.condition -= 0.01f;
				if (item.condition <= 0f)
				{
					body.attackCooldown = 0.5f;
				}
				if (ItemSync.TryGetSyncInfo(item, out var si))
				{
					NetDataWriter writer = Net.CreateWriter(10111);
					writer.Put((ushort)si.syncId);
					writer.Put(val2);
					Net.Client_Send((DeliveryMethod)0, in writer);
				}
			}
		}

		internal void _003CPostfix_003Eb__2_2(Body body, Item item)
		{
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			item.battery.DrainCharge(0.005f);
			if (Util.IsBodyLocal(body) && item.condition > 0f)
			{
				MP3Menu_UpdateList_MultiplayerPatch.last_locally_used_mp3_player = item;
				Utils.Create("Special/MP3SongSelect", ((Component)PlayerCamera.main.mainCanvas).transform);
				if (PlayerCamera.main.radialOpen)
				{
					PlayerCamera.main.radialOpen = false;
				}
			}
			else
			{
				Util.PlayWorldSoundOnScreenIfInRange("flashlighttoggle", Vector2.op_Implicit(((Component)item).transform.position), 0.7f, 1f, 64f);
			}
		}

		internal void _003CPostfix_003Eb__2_3(Body body, Item item)
		{
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			if (!(item.condition > 0f))
			{
				return;
			}
			item.battery.DrainCharge(0.126f);
			EPdaScript component = ((Component)item).GetComponent<EPdaScript>();
			bool hasBeenRead = component.hasBeenRead;
			if (!Object.op_Implicit((Object)(object)body.mindWipe) && !component.hasBeenRead)
			{
				component.hasBeenRead = true;
				body.skills.AddExp(2, 50f);
			}
			if (Util.IsBodyLocal(body))
			{
				if (!Object.op_Implicit((Object)(object)body.mindWipe))
				{
					PlayerCamera.main.SetTimeScale((SpeedType)5, false, false);
					ScrollableText.CreateText(component.text, false, Object.op_Implicit((Object)(object)component.sprite) ? new List<Sprite> { component.sprite } : null, (TMP_FontAsset)null);
					((MonoBehaviour)component).StartCoroutine(SurvivorNote.ChangeTimeScaleWhenFinishedReading(hasBeenRead ? "" : "epdalearn"));
				}
			}
			else
			{
				Util.PlayWorldSoundOnScreenIfInRange("flashlighttoggle", Vector2.op_Implicit(((Component)item).transform.position), 0.7f, 1f, 64f);
			}
		}

		internal void _003CPostfix_003Eb__2_4(Body body, Item item)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Expected O, but got Unknown
			if (Util.IsBodyLocal(body))
			{
				MinigameBase.main.StartMinigame((Minigame)new HandCrankMinigame(), item);
			}
		}

		internal void _003CPostfix_003Eb__2_5(Body body, Item item)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			AmmoScript component = ((Component)item).GetComponent<AmmoScript>();
			if (component.rounds > 0)
			{
				if (!KrokoshaScavMultiplayer.is_client || Util.IsBodyLocal(body))
				{
					GameObject val = Utils.Create(AmmoScript.AmmoTypeToItem(component.ammoType), Vector2.op_Implicit(((Component)component).transform.position), 0f);
					body.AutoPickUpItem(val.GetComponent<Item>());
					component.rounds--;
				}
				Sound.Play("gunloadshell", Vector2.op_Implicit(((Component)item).transform.position), false, true, (Transform)null, 1f, 1f, false, false);
			}
		}

		internal void _003CPostfix_003Eb__2_6(Limb limb, Item item)
		{
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			item.condition = 0f;
			if (limb.infected)
			{
				PlayerCamera.main.showInfection[Array.IndexOf(limb.body.limbs, limb)] = true;
				Sound.Play("goo", Vector2.op_Implicit(((Component)limb).transform.position), false, true, (Transform)null, 1f, 1f, false, false);
			}
		}

		internal void _003CPostfix_003Eb__2_7(Body body, Item item)
		{
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			item.condition = 0f;
			body.skills.AddExp(2, 30f);
			int recipeIndex = ((Component)item).GetComponent<BlueprintScript>().recipeIndex;
			Recipe val = Recipes.recipes[recipeIndex];
			if (Net.running)
			{
				if (KrokoshaScavMultiplayer.is_server)
				{
					val.INT = 0;
					string other = Locale.GetOther("learnedrecipe");
					other = other.Replace("r1", Locale.GetItem(val.simpleName));
					PlayerCamera.main.DoAlert(other, false);
					KrokoshaScavMultiplayer.Server_SendRelayMessageToClients((ushort)10121, (knetid)(ushort)recipeIndex);
				}
			}
			else
			{
				val.INT = 0;
				string other2 = Locale.GetOther("learnedrecipe");
				other2 = other2.Replace("r1", Locale.GetItem(val.simpleName));
				PlayerCamera.main.DoAlert(other2, false);
			}
			if (Util.IsBodyLocal(body))
			{
				PlayerCamera.main.selectedRecipe = val.index;
				if (!PlayerCamera.main.craftingPanel.activeSelf)
				{
					PlayerCamera.main.OpenCraftScreen();
				}
				else
				{
					PlayerCamera.main.RefreshRecipeList();
				}
			}
			Sound.Play("combine", Vector2.op_Implicit(((Component)item).transform.position), false, true, (Transform)null, 1f, 1f, false, false);
		}

		internal void _003CPostfix_003Eb__2_8(Body body, Item item)
		{
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			if (!(item.condition > 0.01f))
			{
				return;
			}
			if (Util.IsBodyLocal(body))
			{
				if (Object.op_Implicit((Object)(object)ScannerScript.main))
				{
					Object.Destroy((Object)(object)((Component)ScannerScript.main).gameObject);
				}
				Object obj = Object.Instantiate(Resources.Load("Special/ScannerUI"), PlayerCamera.main.mainView.transform);
				((GameObject)((obj is GameObject) ? obj : null)).GetComponent<ScannerScript>().pos = Vector2.op_Implicit(((Component)item).transform.position);
			}
			else
			{
				Util.PlayWorldSoundOnScreenIfInRange("flashlighttoggle", Vector2.op_Implicit(((Component)item).transform.position), 0.7f, 1f, 64f);
			}
			item.battery.DrainCharge(0.02f);
		}

		internal void _003CPostfix_003Eb__2_9(Body body, Item item)
		{
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			if (KrokoshaScavMultiplayer.rules.DisableSleep)
			{
				if (body.IsBodyLocal())
				{
					PlayerCamera.main.PlayUISound((UISoundType)6, 1f);
					Util.DoAlert(Lang.Get("sleep_is_disabled", false), false);
				}
				return;
			}
			Body_get_canTakeNap_MultiplayerPatch.force = true;
			if (body.canTakeNap && !body.usingSleepingBag)
			{
				item.condition -= 0.0501f;
				PlayerCamera_ToggleWoundView_MultiplayerPatch.IgnoreNext = true;
				body.TakeANap();
				PlayerCamera_ToggleWoundView_MultiplayerPatch.IgnoreNext = false;
				if (body.IsBodyLocal() && (Object)(object)NetPlayer.LOCAL_PLAYER != (Object)null && NetPlayer.LOCAL_PLAYER.TryGetNetBody(out var pb))
				{
					pb.SetNetHealthSyncIgnoreTime(1f);
				}
				body.usingSleepingBag = true;
				RaycastHit2D val = Physics2D.Raycast(Vector2.op_Implicit(((Component)body).transform.position), Vector2.down, 100f, LayerMask.GetMask(new string[1] { "Ground" }));
				if (RaycastHit2D.op_Implicit(val))
				{
					Object.Destroy((Object)(object)Utils.Create("Special/sleepingbaguse", ((RaycastHit2D)(ref val)).point + Vector2.up, 0f), 10f);
				}
			}
			else if (body.IsBodyLocal())
			{
				PlayerCamera.main.PlayUISound((UISoundType)6, 1f);
			}
		}

		internal void _003CPostfix_003Eb__2_10(float ml, Limb limb)
		{
			float num = ml / 50f;
			CoUtils ins = limb.GetCoUtilsInstance();
			ins.DoTimedOp("midgradestimulant", (Action)delegate
			{
				float num2 = ins.DurationOf("midgradestimulant");
				Body body = limb.body;
				body.stamina += 2f;
				Body body2 = limb.body;
				body2.consciousness += 2f;
				Body body3 = limb.body;
				body3.energy += 0.1f;
				Body body4 = limb.body;
				body4.sicknessAmount += 0.1f;
				Body body5 = limb.body;
				body5.internalBleeding += 0.06f;
				Body body6 = limb.body;
				body6.adrenaline += 7f;
				if (Random.value < 0.1f)
				{
					Body body7 = limb.body;
					body7.miscShakeIntensity += 1.5f;
				}
				if (limb.body.stimulantMultiplier < 0.25f)
				{
					Body body8 = limb.body;
					body8.stimulantMultiplier += 0.035f;
				}
				if (num2 > 220f)
				{
					if (Random.value < 0.18f)
					{
						Body body9 = limb.body;
						body9.miscShakeIntensity += 1.5f;
					}
					if (Random.value < 0.1f)
					{
						Body body10 = limb.body;
						body10.stamina -= 35f;
					}
					if (Random.value < 0.06f)
					{
						limb.body.Ragdoll();
					}
					Body body11 = limb.body;
					body11.internalBleeding += 0.15f;
					Body body12 = limb.body;
					body12.brainHealth -= 0.05f;
					if (limb.body.limbs[1].pain < 60f)
					{
						Limb obj = limb.body.limbs[1];
						obj.pain += 4f;
					}
					limb.body.overdoseIndex = 3;
				}
				if (ins.HighestDurationOf("midgradestimulant") > 59f)
				{
					if (num2 < 30f && Random.value < 0.1f)
					{
						Body body13 = limb.body;
						body13.stamina -= 25f;
					}
					if (num2 <= 1f)
					{
						Body body14 = limb.body;
						body14.energy -= 30f;
						limb.body.vomiter.Vomit();
					}
				}
			}, num * 180f);
		}
	}

	public static Dictionary<string, byte> LiquidIdRegistry = new Dictionary<string, byte>();

	public static string[] LiquidNetIdToId;

	private static void Postfix()
	{
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Expected O, but got Unknown
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Expected O, but got Unknown
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Expected O, but got Unknown
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Expected O, but got Unknown
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Expected O, but got Unknown
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Expected O, but got Unknown
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Expected O, but got Unknown
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Expected O, but got Unknown
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Expected O, but got Unknown
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Expected O, but got Unknown
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Expected O, but got Unknown
		if (Plugin.dump_game_ids)
		{
			Plugin.log.LogInfo((object)$"\ntotal item count: {Item.GlobalItems.Count}");
			Plugin.log.LogInfo((object)("############### DUMPING ALL ITEM IDs START ###########################\n" + string.Join("\n", Item.GlobalItems.Keys)));
			Plugin.log.LogInfo((object)"############### DUMPING ALL ITEM IDs END #############################\n");
		}
		LiquidIdRegistry.Clear();
		LiquidNetIdToId = new string[Liquids.Registry.Count];
		byte b = 0;
		foreach (KeyValuePair<string, LiquidType> item in Liquids.Registry)
		{
			LiquidIdRegistry.Add(item.Key, b);
			LiquidNetIdToId[b] = item.Key;
			b++;
		}
		ItemInfo obj = Item.GlobalItems["scrapmetal"];
		object obj2 = _003C_003Ec._003C_003E9__2_0;
		if (obj2 == null)
		{
			Use val = delegate(Body body, Item item)
			{
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0022: Unknown result type (might be due to invalid IL or missing references)
				//IL_002d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_0037: Unknown result type (might be due to invalid IL or missing references)
				//IL_003a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0044: Unknown result type (might be due to invalid IL or missing references)
				//IL_0049: Unknown result type (might be due to invalid IL or missing references)
				//IL_004e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0053: Unknown result type (might be due to invalid IL or missing references)
				//IL_005a: Unknown result type (might be due to invalid IL or missing references)
				//IL_005f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0064: Unknown result type (might be due to invalid IL or missing references)
				//IL_0078: Unknown result type (might be due to invalid IL or missing references)
				//IL_007d: Unknown result type (might be due to invalid IL or missing references)
				//IL_007e: Unknown result type (might be due to invalid IL or missing references)
				//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
				//IL_0088: Unknown result type (might be due to invalid IL or missing references)
				//IL_008f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0099: Unknown result type (might be due to invalid IL or missing references)
				//IL_009e: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
				//IL_0140: Unknown result type (might be due to invalid IL or missing references)
				//IL_0147: Unknown result type (might be due to invalid IL or missing references)
				if (body.canPlaceBlock && !(item.condition < 0.24f))
				{
					Vector3 position = ((Component)body).transform.position;
					Vector3 val12 = body.targetLookPos - ((Component)body).transform.position;
					Vector2 val13 = Vector2.op_Implicit(position + ((Vector3)(ref val12)).normalized * 5f);
					RaycastHit2D val14 = Physics2D.Linecast(Vector2.op_Implicit(((Component)body).transform.position), val13, LayerMask.GetMask(new string[1] { "Ground" }));
					if (RaycastHit2D.op_Implicit(val14))
					{
						val13 = ((RaycastHit2D)(ref val14)).point + ((RaycastHit2D)(ref val14)).normal * 0.2f;
					}
					body.armsAnimator.Play("ArmsSwing", -1, 0f);
					Vector2Int val15 = WorldGeneration.world.WorldToBlockPos(val13);
					if (WorldGeneration.world.GetBlock(val15) <= 0)
					{
						Sound.Play("scrapmetal", val13, false, true, (Transform)null, 1f, 1f, false, false);
						if (Util.IsBodyLocal(body))
						{
							WorldGeneration.world.SetBlock(val15, (ushort)3);
							item.condition -= 0.25f;
							if (ItemSync.TryGetSyncInfo(item, out var si))
							{
								NetDataWriter writer = Net.CreateWriter(10111);
								writer.Put((ushort)si.syncId);
								writer.Put(val13);
								Net.Client_Send((DeliveryMethod)0, in writer);
							}
						}
					}
				}
			};
			_003C_003Ec._003C_003E9__2_0 = val;
			obj2 = (object)val;
		}
		obj.useAction = (Use)obj2;
		_ = Item.GlobalItems["scaffoldingpack"].useAction;
		ItemInfo obj3 = Item.GlobalItems["scaffoldingpack"];
		object obj4 = _003C_003Ec._003C_003E9__2_1;
		if (obj4 == null)
		{
			Use val2 = delegate(Body body, Item item)
			{
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_002a: Unknown result type (might be due to invalid IL or missing references)
				//IL_002d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0037: Unknown result type (might be due to invalid IL or missing references)
				//IL_003c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0041: Unknown result type (might be due to invalid IL or missing references)
				//IL_0046: Unknown result type (might be due to invalid IL or missing references)
				//IL_004d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0052: Unknown result type (might be due to invalid IL or missing references)
				//IL_0057: Unknown result type (might be due to invalid IL or missing references)
				//IL_006b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0070: Unknown result type (might be due to invalid IL or missing references)
				//IL_0071: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00be: Unknown result type (might be due to invalid IL or missing references)
				//IL_007b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0082: Unknown result type (might be due to invalid IL or missing references)
				//IL_008c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0091: Unknown result type (might be due to invalid IL or missing references)
				//IL_0096: Unknown result type (might be due to invalid IL or missing references)
				//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
				//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
				//IL_014f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0156: Unknown result type (might be due to invalid IL or missing references)
				if (body.canPlaceBlock)
				{
					Vector3 position = ((Component)body).transform.position;
					Vector3 val12 = body.targetLookPos - ((Component)body).transform.position;
					Vector2 val13 = Vector2.op_Implicit(position + ((Vector3)(ref val12)).normalized * 5f);
					RaycastHit2D val14 = Physics2D.Linecast(Vector2.op_Implicit(((Component)body).transform.position), val13, LayerMask.GetMask(new string[1] { "Ground" }));
					if (RaycastHit2D.op_Implicit(val14))
					{
						val13 = ((RaycastHit2D)(ref val14)).point + ((RaycastHit2D)(ref val14)).normal * 0.2f;
					}
					body.armsAnimator.Play("ArmsSwing", -1, 0f);
					Vector2Int val15 = WorldGeneration.world.WorldToBlockPos(val13);
					if (WorldGeneration.world.GetBlock(val15) <= 0)
					{
						Sound.Play("scrapmetal", val13, false, true, (Transform)null, 1f, 1f, false, false);
						if (Util.IsBodyLocal(body))
						{
							WorldGeneration.world.SetBlock(val15, (ushort)21);
							item.condition -= 0.01f;
							if (item.condition <= 0f)
							{
								body.attackCooldown = 0.5f;
							}
							if (ItemSync.TryGetSyncInfo(item, out var si))
							{
								NetDataWriter writer = Net.CreateWriter(10111);
								writer.Put((ushort)si.syncId);
								writer.Put(val13);
								Net.Client_Send((DeliveryMethod)0, in writer);
							}
						}
					}
				}
			};
			_003C_003Ec._003C_003E9__2_1 = val2;
			obj4 = (object)val2;
		}
		obj3.useAction = (Use)obj4;
		ItemInfo obj5 = Item.GlobalItems["mp3player"];
		object obj6 = _003C_003Ec._003C_003E9__2_2;
		if (obj6 == null)
		{
			Use val3 = delegate(Body body, Item item)
			{
				//IL_006b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0070: Unknown result type (might be due to invalid IL or missing references)
				//IL_0075: Unknown result type (might be due to invalid IL or missing references)
				item.battery.DrainCharge(0.005f);
				if (Util.IsBodyLocal(body) && item.condition > 0f)
				{
					MP3Menu_UpdateList_MultiplayerPatch.last_locally_used_mp3_player = item;
					Utils.Create("Special/MP3SongSelect", ((Component)PlayerCamera.main.mainCanvas).transform);
					if (PlayerCamera.main.radialOpen)
					{
						PlayerCamera.main.radialOpen = false;
					}
				}
				else
				{
					Util.PlayWorldSoundOnScreenIfInRange("flashlighttoggle", Vector2.op_Implicit(((Component)item).transform.position), 0.7f, 1f, 64f);
				}
			};
			_003C_003Ec._003C_003E9__2_2 = val3;
			obj6 = (object)val3;
		}
		obj5.useAction = (Use)obj6;
		ItemInfo obj7 = Item.GlobalItems["epda"];
		object obj8 = _003C_003Ec._003C_003E9__2_3;
		if (obj8 == null)
		{
			Use val4 = delegate(Body body, Item item)
			{
				//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
				if (item.condition > 0f)
				{
					item.battery.DrainCharge(0.126f);
					EPdaScript component = ((Component)item).GetComponent<EPdaScript>();
					bool hasBeenRead = component.hasBeenRead;
					if (!Object.op_Implicit((Object)(object)body.mindWipe) && !component.hasBeenRead)
					{
						component.hasBeenRead = true;
						body.skills.AddExp(2, 50f);
					}
					if (Util.IsBodyLocal(body))
					{
						if (!Object.op_Implicit((Object)(object)body.mindWipe))
						{
							PlayerCamera.main.SetTimeScale((SpeedType)5, false, false);
							ScrollableText.CreateText(component.text, false, Object.op_Implicit((Object)(object)component.sprite) ? new List<Sprite> { component.sprite } : null, (TMP_FontAsset)null);
							((MonoBehaviour)component).StartCoroutine(SurvivorNote.ChangeTimeScaleWhenFinishedReading(hasBeenRead ? "" : "epdalearn"));
						}
					}
					else
					{
						Util.PlayWorldSoundOnScreenIfInRange("flashlighttoggle", Vector2.op_Implicit(((Component)item).transform.position), 0.7f, 1f, 64f);
					}
				}
			};
			_003C_003Ec._003C_003E9__2_3 = val4;
			obj8 = (object)val4;
		}
		obj7.useAction = (Use)obj8;
		ItemInfo obj9 = Item.GlobalItems["handcrank"];
		object obj10 = _003C_003Ec._003C_003E9__2_4;
		if (obj10 == null)
		{
			Use val5 = delegate(Body body, Item item)
			{
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0018: Expected O, but got Unknown
				if (Util.IsBodyLocal(body))
				{
					MinigameBase.main.StartMinigame((Minigame)new HandCrankMinigame(), item);
				}
			};
			_003C_003Ec._003C_003E9__2_4 = val5;
			obj10 = (object)val5;
		}
		obj9.useAction = (Use)obj10;
		object obj11 = _003C_003Ec._003C_003E9__2_5;
		if (obj11 == null)
		{
			Use val6 = delegate(Body body, Item item)
			{
				//IL_0020: Unknown result type (might be due to invalid IL or missing references)
				//IL_0030: Unknown result type (might be due to invalid IL or missing references)
				//IL_0035: Unknown result type (might be due to invalid IL or missing references)
				//IL_006a: Unknown result type (might be due to invalid IL or missing references)
				//IL_006f: Unknown result type (might be due to invalid IL or missing references)
				AmmoScript component = ((Component)item).GetComponent<AmmoScript>();
				if (component.rounds > 0)
				{
					if (!KrokoshaScavMultiplayer.is_client || Util.IsBodyLocal(body))
					{
						GameObject val12 = Utils.Create(AmmoScript.AmmoTypeToItem(component.ammoType), Vector2.op_Implicit(((Component)component).transform.position), 0f);
						body.AutoPickUpItem(val12.GetComponent<Item>());
						component.rounds--;
					}
					Sound.Play("gunloadshell", Vector2.op_Implicit(((Component)item).transform.position), false, true, (Transform)null, 1f, 1f, false, false);
				}
			};
			_003C_003Ec._003C_003E9__2_5 = val6;
			obj11 = (object)val6;
		}
		Use useAction = (Use)obj11;
		Item.GlobalItems["riflemagazine"].useAction = useAction;
		Item.GlobalItems["smallmagazine"].useAction = useAction;
		Item.GlobalItems["boxof12gauge"].useAction = useAction;
		ItemInfo obj12 = Item.GlobalItems["roselight"];
		object obj13 = _003C_003Ec._003C_003E9__2_6;
		if (obj13 == null)
		{
			UseLimb val7 = delegate(Limb limb, Item item)
			{
				//IL_003b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0040: Unknown result type (might be due to invalid IL or missing references)
				item.condition = 0f;
				if (limb.infected)
				{
					PlayerCamera.main.showInfection[Array.IndexOf(limb.body.limbs, limb)] = true;
					Sound.Play("goo", Vector2.op_Implicit(((Component)limb).transform.position), false, true, (Transform)null, 1f, 1f, false, false);
				}
			};
			_003C_003Ec._003C_003E9__2_6 = val7;
			obj13 = (object)val7;
		}
		obj12.useLimbAction = (UseLimb)obj13;
		ItemInfo obj14 = Item.GlobalItems["blueprint"];
		object obj15 = _003C_003Ec._003C_003E9__2_7;
		if (obj15 == null)
		{
			Use val8 = delegate(Body body, Item item)
			{
				//IL_011a: Unknown result type (might be due to invalid IL or missing references)
				//IL_011f: Unknown result type (might be due to invalid IL or missing references)
				item.condition = 0f;
				body.skills.AddExp(2, 30f);
				int recipeIndex = ((Component)item).GetComponent<BlueprintScript>().recipeIndex;
				Recipe val12 = Recipes.recipes[recipeIndex];
				if (Net.running)
				{
					if (KrokoshaScavMultiplayer.is_server)
					{
						val12.INT = 0;
						string other = Locale.GetOther("learnedrecipe");
						other = other.Replace("r1", Locale.GetItem(val12.simpleName));
						PlayerCamera.main.DoAlert(other, false);
						KrokoshaScavMultiplayer.Server_SendRelayMessageToClients((ushort)10121, (knetid)(ushort)recipeIndex);
					}
				}
				else
				{
					val12.INT = 0;
					string other2 = Locale.GetOther("learnedrecipe");
					other2 = other2.Replace("r1", Locale.GetItem(val12.simpleName));
					PlayerCamera.main.DoAlert(other2, false);
				}
				if (Util.IsBodyLocal(body))
				{
					PlayerCamera.main.selectedRecipe = val12.index;
					if (!PlayerCamera.main.craftingPanel.activeSelf)
					{
						PlayerCamera.main.OpenCraftScreen();
					}
					else
					{
						PlayerCamera.main.RefreshRecipeList();
					}
				}
				Sound.Play("combine", Vector2.op_Implicit(((Component)item).transform.position), false, true, (Transform)null, 1f, 1f, false, false);
			};
			_003C_003Ec._003C_003E9__2_7 = val8;
			obj15 = (object)val8;
		}
		obj14.useAction = (Use)obj15;
		ItemInfo obj16 = Item.GlobalItems["terrainscanner"];
		object obj17 = _003C_003Ec._003C_003E9__2_8;
		if (obj17 == null)
		{
			Use val9 = delegate(Body body, Item item)
			{
				//IL_0080: Unknown result type (might be due to invalid IL or missing references)
				//IL_0085: Unknown result type (might be due to invalid IL or missing references)
				//IL_008a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0061: Unknown result type (might be due to invalid IL or missing references)
				//IL_0066: Unknown result type (might be due to invalid IL or missing references)
				//IL_006b: Unknown result type (might be due to invalid IL or missing references)
				if (item.condition > 0.01f)
				{
					if (Util.IsBodyLocal(body))
					{
						if (Object.op_Implicit((Object)(object)ScannerScript.main))
						{
							Object.Destroy((Object)(object)((Component)ScannerScript.main).gameObject);
						}
						Object obj22 = Object.Instantiate(Resources.Load("Special/ScannerUI"), PlayerCamera.main.mainView.transform);
						((GameObject)((obj22 is GameObject) ? obj22 : null)).GetComponent<ScannerScript>().pos = Vector2.op_Implicit(((Component)item).transform.position);
					}
					else
					{
						Util.PlayWorldSoundOnScreenIfInRange("flashlighttoggle", Vector2.op_Implicit(((Component)item).transform.position), 0.7f, 1f, 64f);
					}
					item.battery.DrainCharge(0.02f);
				}
			};
			_003C_003Ec._003C_003E9__2_8 = val9;
			obj17 = (object)val9;
		}
		obj16.useAction = (Use)obj17;
		ItemInfo obj18 = Item.GlobalItems["sleepingbag"];
		object obj19 = _003C_003Ec._003C_003E9__2_9;
		if (obj19 == null)
		{
			Use val10 = delegate(Body body, Item item)
			{
				//IL_00be: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
				//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
				//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
				//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
				//IL_0101: Unknown result type (might be due to invalid IL or missing references)
				//IL_0106: Unknown result type (might be due to invalid IL or missing references)
				if (KrokoshaScavMultiplayer.rules.DisableSleep)
				{
					if (body.IsBodyLocal())
					{
						PlayerCamera.main.PlayUISound((UISoundType)6, 1f);
						Util.DoAlert(Lang.Get("sleep_is_disabled", false), false);
					}
				}
				else
				{
					Body_get_canTakeNap_MultiplayerPatch.force = true;
					if (body.canTakeNap && !body.usingSleepingBag)
					{
						item.condition -= 0.0501f;
						PlayerCamera_ToggleWoundView_MultiplayerPatch.IgnoreNext = true;
						body.TakeANap();
						PlayerCamera_ToggleWoundView_MultiplayerPatch.IgnoreNext = false;
						if (body.IsBodyLocal() && (Object)(object)NetPlayer.LOCAL_PLAYER != (Object)null && NetPlayer.LOCAL_PLAYER.TryGetNetBody(out var pb))
						{
							pb.SetNetHealthSyncIgnoreTime(1f);
						}
						body.usingSleepingBag = true;
						RaycastHit2D val12 = Physics2D.Raycast(Vector2.op_Implicit(((Component)body).transform.position), Vector2.down, 100f, LayerMask.GetMask(new string[1] { "Ground" }));
						if (RaycastHit2D.op_Implicit(val12))
						{
							Object.Destroy((Object)(object)Utils.Create("Special/sleepingbaguse", ((RaycastHit2D)(ref val12)).point + Vector2.up, 0f), 10f);
						}
					}
					else if (body.IsBodyLocal())
					{
						PlayerCamera.main.PlayUISound((UISoundType)6, 1f);
					}
				}
			};
			_003C_003Ec._003C_003E9__2_9 = val10;
			obj19 = (object)val10;
		}
		obj18.useAction = (Use)obj19;
		LiquidType obj20 = Liquids.Registry["midgradestimulant"];
		object obj21 = _003C_003Ec._003C_003E9__2_10;
		if (obj21 == null)
		{
			OnHealthUse val11 = delegate(float ml, Limb limb)
			{
				float num = ml / 50f;
				CoUtils ins = limb.GetCoUtilsInstance();
				ins.DoTimedOp("midgradestimulant", (Action)delegate
				{
					float num2 = ins.DurationOf("midgradestimulant");
					Body body = limb.body;
					body.stamina += 2f;
					Body body2 = limb.body;
					body2.consciousness += 2f;
					Body body3 = limb.body;
					body3.energy += 0.1f;
					Body body4 = limb.body;
					body4.sicknessAmount += 0.1f;
					Body body5 = limb.body;
					body5.internalBleeding += 0.06f;
					Body body6 = limb.body;
					body6.adrenaline += 7f;
					if (Random.value < 0.1f)
					{
						Body body7 = limb.body;
						body7.miscShakeIntensity += 1.5f;
					}
					if (limb.body.stimulantMultiplier < 0.25f)
					{
						Body body8 = limb.body;
						body8.stimulantMultiplier += 0.035f;
					}
					if (num2 > 220f)
					{
						if (Random.value < 0.18f)
						{
							Body body9 = limb.body;
							body9.miscShakeIntensity += 1.5f;
						}
						if (Random.value < 0.1f)
						{
							Body body10 = limb.body;
							body10.stamina -= 35f;
						}
						if (Random.value < 0.06f)
						{
							limb.body.Ragdoll();
						}
						Body body11 = limb.body;
						body11.internalBleeding += 0.15f;
						Body body12 = limb.body;
						body12.brainHealth -= 0.05f;
						if (limb.body.limbs[1].pain < 60f)
						{
							Limb obj22 = limb.body.limbs[1];
							obj22.pain += 4f;
						}
						limb.body.overdoseIndex = 3;
					}
					if (ins.HighestDurationOf("midgradestimulant") > 59f)
					{
						if (num2 < 30f && Random.value < 0.1f)
						{
							Body body13 = limb.body;
							body13.stamina -= 25f;
						}
						if (num2 <= 1f)
						{
							Body body14 = limb.body;
							body14.energy -= 30f;
							limb.body.vomiter.Vomit();
						}
					}
				}, num * 180f);
			};
			_003C_003Ec._003C_003E9__2_10 = val11;
			obj21 = (object)val11;
		}
		obj20.onHealthUse = (OnHealthUse)obj21;
	}

	[ClientReceiver(10121, true)]
	private static void ClientReceiver_BlueprintThing(knetid _, ref NetDataReader reader)
	{
		reader.Get(out knetid result);
		Recipe val = Recipes.recipes[(ushort)result];
		val.INT = 0;
		string other = Locale.GetOther("learnedrecipe");
		other = other.Replace("r1", Locale.GetItem(val.simpleName));
		PlayerCamera.main.DoAlert(other, false);
	}

	[ServerReceiver(10111)]
	private static void Server_ItemPlaceBlock(knetid clientId, ref NetDataReader reader)
	{
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out knetid result);
		reader.Get(out Vector2 result2);
		if (!NetPlayer.TryGetNetPlayerAndBodyFromClientId(clientId, out var _, out var body) || !body.conscious || !NetObjectRegistry.TryGetSyncInfo(result, out var si) || !si.IsItem() || !ItemSync.CheckIfBodyReachThisItem(si, body))
		{
			return;
		}
		Item item = si.item;
		bool flag = si.item.id == "scrapmetal";
		bool flag2 = si.item.id == "scaffoldingpack";
		if (!(flag || flag2))
		{
			return;
		}
		Vector2Int val = WorldGeneration.world.WorldToBlockPos(result2);
		if (WorldGeneration.world.GetBlock(val) <= 0)
		{
			if (flag)
			{
				WorldGeneration.world.SetBlock(val, (ushort)3);
				item.condition -= 0.25f;
			}
			else if (flag2)
			{
				WorldGeneration.world.SetBlock(val, (ushort)21);
				item.condition -= 0.01f;
			}
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(result2, $"S: ItemPlaceBlock {si}");
			}
		}
	}
}
