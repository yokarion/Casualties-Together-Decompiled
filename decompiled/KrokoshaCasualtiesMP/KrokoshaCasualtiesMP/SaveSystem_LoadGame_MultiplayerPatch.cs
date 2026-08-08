using System;
using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using KrokoshaCasualtiesUtils;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(SaveSystem), "TryLoadGame")]
public static class SaveSystem_LoadGame_MultiplayerPatch
{
	private static bool force;

	public static WorldGeneration world => WorldGeneration.world;

	private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
	{
		return SavesystemPatch.TranspilerToReplaceTheApplicationPath(instructions);
	}

	private static bool Prefix(ref bool __state)
	{
		__state = SaveSystem.loadedRun;
		if (KrokoshaScavMultiplayer.IsNetworkActiveAndIsClient())
		{
			return false;
		}
		if (!force)
		{
			Body localBody = Util.GetLocalBody();
			SavesystemPatch.savedatapathreplacement = SavesystemPatch.GetTheOnePersistentDataPathWithSaveFile();
			PlayerCamera.main.body = localBody;
		}
		if (!SaveSystem.loadedRun)
		{
			return false;
		}
		if (!force && KrokoshaScavMultiplayer.network_system_is_running)
		{
			SaveSystem.loadedRun = false;
			return true;
		}
		log.l("LOADING SAVE FROM DIRECTORY: " + SavesystemPatch.savedatapathreplacement);
		return true;
	}

	private static void Postfix(ref bool __state)
	{
		if (force)
		{
			return;
		}
		SaveSystem.loadedRun = __state;
		SavesystemPatch.savedatapathreplacement = SavesystemPatch.mpsavefolder;
		Body localBody = Util.GetLocalBody();
		try
		{
			string mpsavefolder = SavesystemPatch.mpsavefolder;
			if (KrokoshaScavMultiplayer.IsNetworkActiveAndIsServer() && SaveSystem.loadedRun && Directory.Exists(mpsavefolder))
			{
				KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog("Attempting to load a multiplayer save.");
				try
				{
					string path = Path.Combine(mpsavefolder, "mp_rules.json");
					if (!File.Exists(path))
					{
						throw new Exception("Multiplayer rules file does not exist.");
					}
					new Dictionary<string, (float, float)>();
					JObject val = JObject.Parse(File.ReadAllText(path));
					string text = (string)val.GetValue("MPVERSION");
					string text2 = (string)val.GetValue("GAMEVERSION");
					_ = (int)val.GetValue("SAVEID");
					bool num = (bool)val.GetValue("LEVEL");
					val.GetValue("PLRPOS").ToObject<Dictionary<string, (float, float)>>();
					if (num)
					{
						int MODIFIER = (int)val.GetValue("MODIFIER");
						if (MODIFIER != -1)
						{
							((MonoBehaviour)world).StartCoroutine(Util._CoroutineCallLambdaWhen(Util.IsWorldGenerated, (Action)delegate
							{
								LayerModifier obj = LayerModifier.availableModifiers[MODIFIER];
								obj.Initialize(world);
								obj.active = true;
							}));
						}
					}
					if (num)
					{
						log.warn("TEMP DEV: TODO: LEVEL THING FORGOR WHAT ITS FOR");
					}
					if (text != "4.0.1")
					{
						log.warn("LoadGame_MultiplayerPatch: Mismatching mod version! file:" + text + " current: 4.0.1");
					}
					if (text2 != Application.version)
					{
						log.warn("LoadGame_MultiplayerPatch: Mismatching game version! file:" + text2 + " current: " + Application.version);
					}
					_ = WoundView.view.cInfo[2];
					JToken val2 = default(JToken);
					if (val.TryGetValue("RULES", ref val2))
					{
						KrokoshaScavMultiplayer.rules = val2.ToObject<KrokoshaMultiplayerGameRules>();
					}
					try
					{
						SharedMain.CreatePlayerCharacters();
						foreach (KeyValuePair<Body, NetPlayer> item in NetPlayer.BodyToPlayerDict)
						{
							string persistentId = item.Value.GetPersistentId();
							SavesystemPatch.savedatapathreplacement = Path.Combine(mpsavefolder, persistentId);
							if (Directory.Exists(SavesystemPatch.savedatapathreplacement))
							{
								try
								{
									SaveSystem_HasSave_MultiplayerPatch.force = true;
									SaveSystem.loadedRun = true;
									force = true;
									PlayerCamera.main.body = item.Key;
									SaveSystem.TryLoadGame();
									foreach (Item item2 in item.Key.GetAllItemsThorough())
									{
										if (!NetObjectRegistry.ObjectCanBeIgnoredForNetwork(((Component)item2).gameObject))
										{
											SyncInfo syncInfo = NetObjectRegistry.Server_EnsureItemIsNetworkRegistered(((Component)item2).gameObject);
											if (syncInfo != null)
											{
												NetObjectRegistry.Server_QueueSync(syncInfo);
											}
										}
									}
									item.Value.server_plrstate.loaded_save = true;
								}
								catch (Exception ex)
								{
									log.error("LoadGame_MultiplayerPatch: READING " + ((object)item.Value).ToString() + ":\nPATH: " + SavesystemPatch.savedatapathreplacement + "\n" + ex.ToString());
								}
							}
							else
							{
								log.warn($"LoadGame_MultiplayerPatch: SAVE FILE NOT FOUND FOR {item.Value}");
							}
						}
						if (Con.IsConsoleOpen())
						{
							ConsoleScript.instance.ToggleActiveState();
						}
					}
					catch (Exception ex2)
					{
						log.error("LoadGame_MultiplayerPatch: READING PLAYERS\n" + ex2.ToString());
					}
					SaveSystem.loadedRun = true;
					SaveSystem_HasSave_MultiplayerPatch.force = false;
					SavesystemPatch.savedatapathreplacement = mpsavefolder;
					force = false;
					PlayerCamera.main.body = localBody;
					try
					{
						foreach (KeyValuePair<Body, NetPlayer> item3 in NetPlayer.BodyToPlayerDict)
						{
							if (!item3.Value.server_plrstate.loaded_save)
							{
								log.warn($"LoadGame_MultiplayerPatch: COULD NOT FIND PLAYER TO LOAD: {item3.Value}");
							}
						}
					}
					catch (Exception ex3)
					{
						log.error("LoadGame_MultiplayerPatch: TEST\n" + ex3.ToString());
					}
				}
				catch (Exception ex4)
				{
					log.error("LoadGame_MultiplayerPatch: READING\n" + ex4.ToString());
				}
				SavesystemPatch.DeleteMPSave();
				SaveSystem.loadedRun = true;
				SaveSystem_HasSave_MultiplayerPatch.force = false;
				SavesystemPatch.savedatapathreplacement = SavesystemPatch.vanillasavefolder;
				force = false;
				PlayerCamera.main.body = localBody;
			}
		}
		catch (Exception ex5)
		{
			log.error("LoadGame_MultiplayerPatch\n" + ex5.ToString());
		}
		SaveSystem_HasSave_MultiplayerPatch.force = false;
		SavesystemPatch.savedatapathreplacement = SavesystemPatch.vanillasavefolder;
		force = false;
		PlayerCamera.main.body = localBody;
	}
}
