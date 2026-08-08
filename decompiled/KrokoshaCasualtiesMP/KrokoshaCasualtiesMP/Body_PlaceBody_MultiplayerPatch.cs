using HarmonyLib;
using KrokoshaCasualtiesUtils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(Body), "PlaceBody")]
public static class Body_PlaceBody_MultiplayerPatch
{
	public static bool has_spawn_location = false;

	public static Vector2 spawnlocation = new Vector2(0f, 500f);

	private static Vector2 ogcampos;

	private static Vector2 ogbodypos;

	private static bool Prefix(Body __instance)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		if (!KrokoshaScavMultiplayer.network_system_is_running)
		{
			return true;
		}
		ogbodypos = Vector2.op_Implicit(((Component)__instance).transform.position);
		ogcampos = Vector2.op_Implicit(((Component)PlayerCamera.main).transform.position);
		if (KrokoshaScavMultiplayer.rules.LayerFinishKeepXOffset && NetPlayer.TryGetLocalNetBody(out var nb) && Net.is_server && NetPlayer.LOCAL_PLAYER.IsAlive())
		{
			nb.SetBodyPosition(Util.PlaceBody_FindSpawnLocation(NetPlayer.LOCAL_PLAYER.server_plrstate.layer_transition_x_offset));
			return false;
		}
		return true;
	}

	private static void Postfix(Body __instance)
	{
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		if (!KrokoshaScavMultiplayer.network_system_is_running)
		{
			return;
		}
		if (__instance.IsBodyLocal())
		{
			if (KrokoshaScavMultiplayer.is_client)
			{
				((Component)__instance).transform.position = Vector2.op_Implicit(ogbodypos);
			}
			else
			{
				has_spawn_location = true;
				foreach (NetBody all_instance in NetBody.all_instances)
				{
					if (all_instance.is_player && !all_instance.IsBodyLocal())
					{
						all_instance.plr.ResetEntropy();
						all_instance.SetBodyPosition(new Vector2(((Component)all_instance).transform.position.x, ((Component)__instance).transform.position.y));
					}
				}
			}
			spawnlocation = Vector2.op_Implicit(((Component)__instance).transform.position);
		}
		else
		{
			ogcampos = Vector2.op_Implicit(((Component)PlayerCamera.main).transform.position);
		}
	}
}
