using HarmonyLib;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(Item), "OnCollisionEnter2D")]
public static class Item_OnCollisionEnter2D_MultiplayerPatch
{
	public static bool Prefix(Item __instance, Collision2D collision)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		Item val = default(Item);
		if (((Component)collision.collider).TryGetComponent<Item>(ref val))
		{
			Vector2 relativeVelocity = collision.relativeVelocity;
			if (((Vector2)(ref relativeVelocity)).magnitude < 20f)
			{
				return false;
			}
		}
		return true;
	}
}
