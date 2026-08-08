using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using KrokoshaCasualtiesUtils;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(PlayerCamera), "TryCraft")]
public static class PlayerCamera_TryCraft_MultiplayerPatch
{
	public static List<knetid> last_recipe_items = new List<knetid>();

	private static bool Prefix(PlayerCamera __instance)
	{
		if (!KrokoshaScavMultiplayer.network_system_is_running)
		{
			return true;
		}
		Util.GetLocalBody();
		Recipe craftingtable = Recipes.recipes[__instance.selectedRecipe];
		if (Recipe_TryMake(Util.GetLocalBody(), craftingtable))
		{
			TellTheServerThatICrafted();
		}
		__instance.RefreshRecipeList();
		return false;
	}

	private static void TellTheServerThatICrafted()
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		if (KrokoshaScavMultiplayer.network_system_is_running)
		{
			PlayerCamera main = PlayerCamera.main;
			Recipe val = Recipes.recipes[main.selectedRecipe];
			NetDataWriter writer = Net.CreateWriter(10053);
			writer.Put((ushort)val.index);
			writer.PutArray(((IEnumerable<knetid>)last_recipe_items).Select((Func<knetid, ushort>)((knetid x) => x)).ToArray());
			Net.Client_Send((DeliveryMethod)2, in writer);
		}
	}

	public static List<Item> Recipe_GetItemsForRecipe(Body crafter, Recipe craftingtable)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		List<Item> items = crafter.GetAllItemsThorough();
		Collider2D[] array = Physics2D.OverlapCircleAll(Vector2.op_Implicit(((Component)crafter).transform.position), 10f, LayerMask.GetMask(new string[1] { "Item" }));
		Item val = default(Item);
		for (int i = 0; i < array.Length; i++)
		{
			if (((Component)array[i]).TryGetComponent<Item>(ref val) && crafter.DoPickupCheck(val, true) && ItemSync.CheckIfBodyReachThisItem(val, crafter))
			{
				items.Add(val);
			}
		}
		return CheckIfTheseItemsAreEnoughForTheRecipe(craftingtable, in items);
	}

	public static List<Item> CheckIfTheseItemsAreEnoughForTheRecipe(Recipe recipe, in List<Item> items)
	{
		List<Item> list = new List<Item>();
		List<Item> list2 = new List<Item>();
		foreach (RecipeItem item in recipe.items)
		{
			Item matchingItem = item.GetMatchingItem(items, list2);
			if ((Object)(object)matchingItem != (Object)null)
			{
				list.Add(matchingItem);
				if (item.destroyItem && !item.isLiquid)
				{
					list2.Add(matchingItem);
				}
				continue;
			}
			return null;
		}
		return list;
	}

	public static bool Recipe_TryMake(Body crafter, Recipe craftingtable, List<Item> items_to_craft_with = null)
	{
		List<Item> list = items_to_craft_with;
		if (items_to_craft_with == null)
		{
			list = Recipe_GetItemsForRecipe(crafter, craftingtable);
		}
		last_recipe_items.Clear();
		foreach (Item item in list)
		{
			if (ItemSync.TryGetSyncInfo(item, out var si))
			{
				last_recipe_items.Add(si.syncId);
			}
		}
		if (list != null)
		{
			for (int i = 0; i < list.Count; i++)
			{
				RecipeItem val = craftingtable.items[i];
				Item val2 = list[i];
				if (Object.op_Implicit((Object)(object)val2) && val.destroyItem && !val.isLiquid)
				{
					ItemSync.SafeUnloadItem(val2);
				}
				val.UseItem(val2);
			}
			if (!KrokoshaScavMultiplayer.IsNetworkActiveAndIsClient())
			{
				RecipeResult_SpawnResult(craftingtable.INT, craftingtable.result, crafter);
			}
			if (Util.IsBodyLocal(crafter))
			{
				craftingtable.hasMadeBefore = true;
			}
			NetBody netBody = default(NetBody);
			if (((Component)crafter).TryGetComponent<NetBody>(ref netBody) && !netBody.plr.tosave_hascrafterbeforerecipes.Contains(craftingtable.index))
			{
				netBody.plr.tosave_hascrafterbeforerecipes.Add(craftingtable.index);
				crafter.happiness += 1f;
				crafter.skills.AddExp(2, 10f);
			}
			return true;
		}
		return false;
	}

	public static void RecipeResult_SpawnResult(int recipeInt, RecipeResult rres, Body i_am_steve)
	{
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		int num = i_am_steve.skills.INT - recipeInt;
		float num2 = 1f;
		if (num < 0 && Random.value < 0.5f)
		{
			switch (num)
			{
			default:
				return;
			case -3:
			{
				i_am_steve.DoGoreSound();
				for (int i = 5; i <= 8; i += 3)
				{
					Limb obj = i_am_steve.limbs[i];
					obj.pain += 40f;
					Limb obj2 = i_am_steve.limbs[i];
					obj2.skinHealth -= 15f;
					Limb obj3 = i_am_steve.limbs[i];
					obj3.bleedAmount += Random.Range(2f, 5f);
				}
				return;
			}
			case -1:
				break;
			}
			num2 = Random.Range(0.2f, 0.9f);
		}
		Item val = null;
		WaterContainerItem val2 = default(WaterContainerItem);
		WaterContainerItem val3 = default(WaterContainerItem);
		for (int j = 0; j < rres.amount; j++)
		{
			if (rres.isLiquid)
			{
				bool flag = false;
				foreach (Item item in i_am_steve.GetAllItemsThorough())
				{
					if (((Component)item).TryGetComponent<WaterContainerItem>(ref val2) && val2.stack.Count == 1 && val2.stack[0].liquidId == rres.id && val2.SpaceLeft >= rres.resultCondition * num2)
					{
						val2.AddLiquid(rres.id, rres.resultCondition * num2);
						flag = true;
						if (NetObjectRegistry.TryGetSyncInfo((Component)(object)item, out var si))
						{
							NetObjectRegistry.Server_QueueSync(si);
						}
						return;
					}
				}
				if (!flag)
				{
					GameObject obj4 = Utils.Create("craftingbottle", Vector2.op_Implicit(((Component)i_am_steve).transform.position), 0f);
					Item component = obj4.GetComponent<Item>();
					component.condition = rres.resultCondition;
					i_am_steve.AutoPickUpItem(component);
					((Component)component).GetComponent<WaterContainerItem>().AddLiquid(rres.id, rres.resultCondition * num2);
					Object.Destroy((Object)(object)obj4, 300f);
					val = component;
				}
			}
			else
			{
				Item component2 = Utils.Create(rres.id, Vector2.op_Implicit(((Component)i_am_steve).transform.position), 0f).GetComponent<Item>();
				component2.condition = rres.resultCondition * num2;
				i_am_steve.AutoPickUpItem(component2);
				if (Object.op_Implicit((Object)(object)component2.battery))
				{
					component2.battery.UnloadBattery(true);
				}
				if (!rres.dontDrainResultLiquid && ((Component)component2).TryGetComponent<WaterContainerItem>(ref val3))
				{
					val3.stack = new List<LiquidStack>();
					component2.condition = 0f;
				}
				val = component2;
			}
		}
		if ((Object)(object)val != (Object)null && log.verbose)
		{
			log.l($"CRAFT RESULT CONTAINER: {ItemSync.ItemGetContainerInfo(val)}");
		}
	}

	[ServerReceiver(10053)]
	private static void Server_PlayerCamera_TryCraft(knetid clientId, ref NetDataReader reader)
	{
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		if (!NetPlayer.TryGetNetPlayerAndNetBodyFromClientId(clientId, out var plr, out var pb) || !plr.body.conscious)
		{
			return;
		}
		ushort num = default(ushort);
		reader.Get(ref num);
		ushort[] uShortArray = reader.GetUShortArray();
		if (num > Recipes.recipes.Count)
		{
			Plugin.log.LogWarning((object)$"SUS: {pb} tried to craft unknown recipe: {num} ");
			return;
		}
		HashSet<SyncInfo> hashSet = new HashSet<SyncInfo>();
		foreach (knetid knetid2 in uShortArray)
		{
			if (ItemSync.TryGetItemSyncInfo(knetid2, out var si) && si.IsItem())
			{
				if (ItemSync.CheckIfBodyReachThisItem(si, pb.body))
				{
					hashSet.Add(si);
					continue;
				}
				return;
			}
			Plugin.log.LogWarning((object)$"SUS: {pb} tried to craft with unknown item syncid:{knetid2} ");
			return;
		}
		Recipe val = Recipes.recipes[num];
		List<Item> list = CheckIfTheseItemsAreEnoughForTheRecipe(val, hashSet.Select((SyncInfo syncInfo) => syncInfo.item).ToList());
		if (list != null)
		{
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(Vector2.op_Implicit(((Component)plr.body).transform.position), $"S: TryCraft {num}");
			}
			ServerMain.Server_AnnounceSound(plr.pos, "combine", ServerMain.GetListOfClientIdsExceptThis(clientId));
			if (!pb.is_local)
			{
				Recipe_TryMake(plr.body, val, list);
			}
		}
		else
		{
			Plugin.log.LogWarning((object)$"SUS: {pb} tried to craft with insufficient or incorrect items ");
		}
	}
}
