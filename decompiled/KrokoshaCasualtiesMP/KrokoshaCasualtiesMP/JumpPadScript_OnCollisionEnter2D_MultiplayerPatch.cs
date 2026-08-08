using HarmonyLib;
using KrokoshaCasualtiesUtils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(JumpPadScript), "OnCollisionEnter2D")]
internal static class JumpPadScript_OnCollisionEnter2D_MultiplayerPatch
{
	internal class ShakerState
	{
		internal Vector2 pos;

		internal Vector2 vel;

		internal ShakerState(Shaker s)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			pos = s.pos;
			vel = s.velocity;
		}

		internal void Apply(Shaker s)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			s.pos = pos;
			s.velocity = vel;
		}
	}

	private static void Prefix(JumpPadScript __instance, Collision2D collision, ref ShakerState __state)
	{
		__state = new ShakerState(PlayerCamera.main.shaker);
	}

	private static void Postfix(JumpPadScript __instance, Collision2D collision, ref ShakerState __state)
	{
		Limb limb = default(Limb);
		if (collision.gameObject.TryGetComponent<Limb>(ref limb) && !limb.IsBodyLocal())
		{
			__state.Apply(PlayerCamera.main.shaker);
		}
	}
}
