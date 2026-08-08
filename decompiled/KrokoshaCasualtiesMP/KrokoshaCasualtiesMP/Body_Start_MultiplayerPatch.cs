using System;
using HarmonyLib;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(Body), "Start")]
public static class Body_Start_MultiplayerPatch
{
	public static string BodyToString(this Body b)
	{
		if ((Object)(object)b == (Object)null)
		{
			return "BODY(NULL)";
		}
		NetBody netBody = default(NetBody);
		if (((Component)b).TryGetComponent<NetBody>(ref netBody))
		{
			return ((object)netBody).ToString();
		}
		Transform parent = ((Component)b).transform.parent;
		return $"BODY({((parent != null) ? ((Object)parent).name : null)}->{b})";
	}

	private static void Postfix(Body __instance)
	{
		NetBody component = ((Component)__instance).GetComponent<NetBody>();
		string text = "NULL";
		try
		{
			if ((Object)(object)component != (Object)null && component.is_player && KrokoshaScavMultiplayer.is_server)
			{
				text = component.plr.GetPersistentId();
				if (ServerMain.server_lastplayerstates.TryGetValue(text, out var value))
				{
					value.Apply(__instance);
					ServerMain.server_lastplayerstates.Remove(text);
				}
			}
		}
		catch (Exception ex)
		{
			log.error($"Body_Start_MultiplayerPatch SERVER READING SAVE:\nPlayerBody: {component}\npid: {text}\n" + ex.ToString());
		}
		Physics2D.IgnoreLayerCollision(LayerMask.GetMask(new string[1] { "Player" }), LayerMask.GetMask(new string[1] { "Player" }), true);
		Body[] array = Object.FindObjectsOfType<Body>();
		foreach (Body val in array)
		{
			if (!((Object)(object)val != (Object)(object)__instance))
			{
				continue;
			}
			Transform parent = ((Component)__instance).transform.parent;
			Transform parent2 = ((Component)val).transform.parent;
			Collider2D[] componentsInChildren = ((Component)parent).GetComponentsInChildren<Collider2D>();
			Collider2D[] componentsInChildren2 = ((Component)parent2).GetComponentsInChildren<Collider2D>();
			foreach (Collider2D val2 in componentsInChildren)
			{
				foreach (Collider2D val3 in componentsInChildren2)
				{
					Physics2D.IgnoreCollision(val2, val3, true);
				}
			}
		}
	}
}
