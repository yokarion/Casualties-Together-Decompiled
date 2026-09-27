using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HarmonyLib;
using KrokoshaCasualtiesUtils;
using Newtonsoft.Json;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(SaveSystem), "SaveGame")]
public static class SaveSystem_SaveGame_MultiplayerPatch
{
	private static bool keeppath;

	private static bool force;

	public static WorldGeneration world => WorldGeneration.world;

	private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
	{
		return SavesystemPatch.TranspilerToReplaceTheApplicationPath(instructions);
	}

	private static void Prefix()
	{
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		if (!force)
		{
			SavesystemPatch.savedatapathreplacement = SavesystemPatch.mpsavefolder;
			Body localBody = Util.GetLocalBody();
			try
			{
				if (KrokoshaScavMultiplayer.IsNetworkActiveAndIsServer())
				{
					if (!Directory.Exists(SavesystemPatch.mpsavefolder))
					{
						Directory.CreateDirectory(SavesystemPatch.mpsavefolder);
					}
					else
					{
						SavesystemPatch.DeleteMPSave();
					}
					bool flag = !ServerMain.CheckIfEnoughPeopleAreAtLayerFinish();
					try
					{
						foreach (KeyValuePair<Body, NetPlayer> item in NetPlayer.BodyToPlayerDict)
						{
							SavesystemPatch.savedatapathreplacement = Path.Combine(SavesystemPatch.mpsavefolder, item.Value.GetPersistentId());
							try
							{
								try
								{
									if (!flag && !item.Key.alive && KrokoshaScavMultiplayer.rules.CanReviveOnNextLevel())
									{
										item.Value.Server_RespawnCharacter(Vector2.op_Implicit(((Component)localBody).transform.position), !flag);
									}
									item.Value.ResetEntropy();
								}
								catch (Exception ex)
								{
									log.error("SaveGame_MultiplayerPatch: PREPARING TO SAVE " + ((object)item.Value).ToString() + ":\n" + ex.ToString());
								}
								if (!Directory.Exists(SavesystemPatch.savedatapathreplacement))
								{
									Directory.CreateDirectory(SavesystemPatch.savedatapathreplacement);
								}
								force = true;
								PlayerCamera.main.body = item.Key;
								SaveSystem.SaveGame();
							}
							catch (Exception ex2)
							{
								log.error("SaveGame_MultiplayerPatch: SAVING " + ((object)item.Value).ToString() + ":\n" + ex2.ToString());
							}
						}
						PlayerCamera.main.body = localBody;
						SavesystemPatch.savedatapathreplacement = SavesystemPatch.mpsavefolder;
						force = false;
					}
					catch (Exception ex3)
					{
						log.error("SaveGame_MultiplayerPatch: SAVING PLAYERS\n" + ex3.ToString());
					}
					string contents = JsonConvert.SerializeObject((object)new Dictionary<string, object>
					{
						["hello"] = "Multiplayer Mod created by Krokosha666",
						["MPVERSION"] = "4.1.2",
						["GAMEVERSION"] = Application.version,
						["SAVEID"] = WoundView.view.cInfo[2],
						["LEVEL"] = flag,
						["RULES"] = KrokoshaScavMultiplayer.rules,
						["MODIFIER"] = ((!flag) ? (-1) : (WorldgenPatches.GetLayerModifier()?.modifierIndex ?? (-1))),
						["PLRPOS"] = NetPlayer.BodyToPlayerDict.ToDictionary((KeyValuePair<Body, NetPlayer> x) => x.Value.GetPersistentId(), (KeyValuePair<Body, NetPlayer> x) => (x: ((Component)x.Key).transform.position.x, y: ((Component)x.Key).transform.position.y))
					}, (Formatting)1);
					File.WriteAllText(Path.Combine(SavesystemPatch.mpsavefolder, "mp_rules.json"), contents);
				}
			}
			catch (Exception ex4)
			{
				log.error("SaveGame_MultiplayerPatch\n" + ex4.ToString());
			}
			SaveSystem_HasSave_MultiplayerPatch.force = false;
			SavesystemPatch.savedatapathreplacement = SavesystemPatch.GetPersistentDataPathToSave();
			force = false;
			PlayerCamera.main.body = localBody;
		}
		log.l("SAVING TO FILE: " + SavesystemPatch.savedatapathreplacement);
	}
}
