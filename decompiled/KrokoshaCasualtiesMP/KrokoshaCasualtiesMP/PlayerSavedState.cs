using System.Collections.Generic;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public struct PlayerSavedState
{
	public int savedonlayer;

	public Vector2 position;

	public Color24 plrcolor;

	public CharacterHealthPainkillerStateSyncPacket painkiller;

	public CharacterHealthStateSyncPacket health;

	public List<int> hascrafterbeforerecipes;

	public GameObject invholder;

	public Item[] inventory;

	public PlayerSavedState(Body body)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Expected O, but got Unknown
		plrcolor = default(Color24);
		painkiller = default(CharacterHealthPainkillerStateSyncPacket);
		invholder = null;
		inventory = null;
		hascrafterbeforerecipes = null;
		position = Vector2.op_Implicit(((Component)body).transform.position);
		savedonlayer = WorldGeneration.world.biomeDepth;
		health = new CharacterHealthStateSyncPacket(body);
		Painkillers val = default(Painkillers);
		if (((Component)body).TryGetComponent<Painkillers>(ref val))
		{
			painkiller = new CharacterHealthPainkillerStateSyncPacket(body);
		}
		NetBody netBody = default(NetBody);
		if (((Component)body).TryGetComponent<NetBody>(ref netBody) && netBody.is_player)
		{
			hascrafterbeforerecipes = new List<int>(netBody.plr.tosave_hascrafterbeforerecipes);
			plrcolor = netBody.plr.plrcolor;
		}
		if (!KrokoshaScavMultiplayer.rules.SavePlayerInventory)
		{
			return;
		}
		invholder = new GameObject($"SAVESTATE_{netBody}");
		inventory = (Item[])(object)new Item[8];
		List<Item> list = new List<Item>(8);
		for (int i = 0; i < body.slots.Length; i++)
		{
			Item item = body.GetItem(i);
			if ((Object)(object)item != (Object)null)
			{
				list.Add(item);
				inventory[i] = item;
			}
		}
		list.AddRange(body.GetAllWearables());
		foreach (Item item2 in list)
		{
			((Component)item2).transform.SetParent(invholder.transform);
		}
		invholder.SetActive(false);
	}

	public void Apply(Body body)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Expected O, but got Unknown
		if (KrokoshaScavMultiplayer.rules.SavePlayerPosition && savedonlayer == WorldGeneration.world.biomeDepth)
		{
			NetBody netBody = default(NetBody);
			if (((Component)body).TryGetComponent<NetBody>(ref netBody) && netBody.is_player)
			{
				netBody.plr.server_plrstate.did_give_spawn_location_from_a_save = true;
			}
			((Component)body).transform.position = Vector2.op_Implicit(position);
		}
		if (KrokoshaScavMultiplayer.rules.SavePlayerState)
		{
			health.Apply(body);
			painkiller.Apply(body);
			NetBody netBody2 = default(NetBody);
			if (((Component)body).TryGetComponent<NetBody>(ref netBody2) && netBody2.is_player)
			{
				if (hascrafterbeforerecipes != null)
				{
					netBody2.plr.tosave_hascrafterbeforerecipes = hascrafterbeforerecipes;
				}
				netBody2.plr.plrcolor = plrcolor;
			}
		}
		if (KrokoshaScavMultiplayer.rules.SavePlayerInventory && (Object)(object)invholder != (Object)null && inventory != null)
		{
			invholder.SetActive(true);
			for (int i = 0; i < inventory.Length; i++)
			{
				Item val = inventory[i];
				if ((Object)(object)val != (Object)null)
				{
					((Component)val).transform.parent = null;
					body.PickUpItem(val, i, true);
				}
			}
			List<Item> list = new List<Item>(8);
			Item item = default(Item);
			foreach (Transform item2 in invholder.transform)
			{
				Transform val2 = item2;
				if ((Object)(object)val2 != (Object)null && ((Component)val2).TryGetComponent<Item>(ref item))
				{
					list.Add(item);
				}
			}
			foreach (Item item3 in list)
			{
				body.AutoPickUpItem(item3);
			}
		}
		if ((Object)(object)invholder != (Object)null)
		{
			Object.Destroy((Object)(object)invholder);
		}
	}
}
