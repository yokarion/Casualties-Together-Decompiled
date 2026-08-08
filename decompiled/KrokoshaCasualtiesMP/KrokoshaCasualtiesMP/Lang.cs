using System.Collections.Generic;

namespace KrokoshaCasualtiesMP;

public static class Lang
{
	internal static Dictionary<string, string> EN = new Dictionary<string, string>
	{
		{ "loaderror_possible_nonascii_path", "Make sure you have only english characters in your game path:" },
		{ "autostarting_run", "Auto-starting the run!" },
		{ "wait_for_host", "Wait for Host!" },
		{ "minigame_deny_server", "SERVER: Denied Minigame session." },
		{ "serverdeny_lockpick", "SERVER: Denied lockpick." },
		{ "serverdeny_syringeinject", "SERVER: Denied syringe inject." },
		{ "serverdeny_bandage1", "SERVER: Denied bandage. (try again)" },
		{ "serverdeny_bandage2", "SERVER: Failed to verify bandage action." },
		{ "plr_started_drillpod", "{0} started a Drillpod." },
		{ "all_started_drillpod", "Drillpod activated." },
		{ "plr_hover_dead", " (DEAD)" },
		{ "plr_hover_sleeping", " (SLEEPING)" },
		{ "plr_hover_unconscious", " (UNCONSCIOUS)" },
		{ "plr_chattag_dead", "<color=red>DEAD</color>" },
		{ "plr_chattag_far", "<color=#5a5255>FAR</color>" },
		{ "plr_too_far", "Player too far." },
		{ "plr_moving", "Player is moving" },
		{ "plr_set_timescale", "{0} set time scale to: {1}" },
		{ "interact_obstructed", "No direct line of sight." },
		{ "deny_refuse", "They refuse." },
		{ "deny_heal_dead", "There's no saving them now." },
		{ "voicechat_deny_mindwipe", "You can't speak, you're mindwiped." },
		{ "voicechat_deny_unconscious", "You can't speak, you're unconscious." },
		{ "on_plr_sleep", " is asleep." },
		{ "wake_up", "Wake up" },
		{ "item_unreachable", "Item is unreachable." },
		{ "plr_unreachable", "Player is unreachable." },
		{ "plrint_woundview", "Inspect\nWounds" },
		{ "plrint_woundview_tooltip", "Inspect health." },
		{ "plrint_req_woundview", "Viewing Health Panel of {0}" },
		{ "plrint_inventory", "Inventory" },
		{ "plrint_inv_allowed", "Steal their stuff!" },
		{ "plrint_inv_no", "Can't access, they're awake." },
		{ "plrint_useitem", "Use Item" },
		{ "plrint_useitem_wearable", "Dress" },
		{ "plrint_useitem_food", "Feed" },
		{ "plrint_useitem_water", "Feed" },
		{ "plrint_feed_item", "Feed/Use an item on them." },
		{ "plrint_drag_item_here", "Drag a compatible item here." },
		{ "plrint_carry", "Carry" },
		{ "plrint_carry_allow", "Carry this player." },
		{ "plrint_carry_no", "You can carry only incapacitated players." },
		{ "plrint_piggyback", "Piggyback" },
		{ "plrint_piggyback_allow", "Climb on their back." },
		{ "plrint_piggyback_no", "They've got bigger problems." },
		{ "piggyback_stop", "Stop piggybacking" },
		{ "carry_stop", "Stop carrying" },
		{ "plr_not_sleeping", " is not sleeping. " },
		{ "plr_sleeping_count", "Sleeping players count: " },
		{ "plrint_push", "Push" },
		{ "plrint_push_tooltip", "Do it near a cliff :)" },
		{ "sleep_is_disabled", "Sleep is disabled by the server." },
		{ "sleep_disabled_desc", "You are always at 100% energy." },
		{ "host_continue_run_announcement", "Host continues the run!" },
		{ "tutorial_finished_alert", "You finished the tutorial, wait for your friends." },
		{ "tutorial_localdied_alert", "You died in the tutorial, waiting for other players to finish..." },
		{ "tutorial_mp_alert", "Tutorial Multiplayer, some systems synchronization disabled." },
		{ "tutorial_finish_alert", "Everyone finished the tutorial, going back to main menu." },
		{ "drillpod_not_enough_plrs", "Everyone must be in the drill. {0}/{1}" },
		{ "layerfinish_not_enough_plrs", "Everyone must be at the finish!" },
		{ "layerfinish_wait_for_friends", "You finished the layer, wait for your friends." },
		{ "layerfinish_wait_for_host", "You finished the layer, wait for host." },
		{ "plr_finished_layer", "{0} just finished the layer." },
		{ "everyone_is_dead", "Everyone is dead." },
		{ "everyone_is_dead_host", "Everyone is dead, go to main menu and restart the run." },
		{ "obj_not_net_registered", "Object is not network registered.\n(try again)" },
		{ "ur_too_weak", "Too weak" },
		{ "plr_dead", "They're dead." },
		{ "trader_buy_deny", "Verification denies this purchase." },
		{ "singleplayer_warn", "Multiplayer mod is still injected! DEACTIVATE IT IN SETTINGS TO PLAY VANILLA SINGLEPLAYER!" },
		{ "antidispersion_activated_alert", "You're too far from everyone!" },
		{ "healthchipalert_antidispersion_warn", "YOUR PEERS\nSIGNAL IS WEAK" },
		{ "healthchipalert_antidispersion1", "TOO FAR FROM\nYOUR PEERS" },
		{ "healthchipalert_antidispersion2", "PUNISHMENT\nPROTOCOL ACTIVE" },
		{ "healthchipalert_antidispersion3", "STAY TOGETHER" },
		{ "respawned_dead_plrs", "Respawned dead players." },
		{ "click_to_cpr", "| CLICK HERE |\n\n| TO PERFORM CPR |" },
		{ "worldgen_waitforserver_seed", "Multiplayer: Waiting for server worldgen info." },
		{ "worldgen_waitforserver_spawn", "Multiplayer: Requesting spawn location." },
		{ "worldgen_waitforserver", "Multiplayer: Waiting for server..." },
		{ "worldgen_waitforserver_delete", "Multiplayer: Clearing world..." },
		{ "alert_cantcarry", "This player can't be carried rn." },
		{ "straggler_radline", "<color=red>Stragglers must hurry up!</color>" },
		{ "chat_preview", "Preview: " },
		{ "chat_keybind", "Press \"{0}\" to chat..." },
		{ "spectate", "Spectate" },
		{ "spectate_openwoundview", "Open Health Panel" },
		{ "spectate_freecam", "Freecam" },
		{ "spectate_freecamexit", "Exit Freecam" },
		{ "spectate_exit", "Stop Spectating" },
		{ "spectate_alive", "Spectate only alive" },
		{ "netpanel_title", "Multiplayer" },
		{ "netmode", "Mode: " },
		{ "netmode_client", "Client" },
		{ "netmode_host", "Host" },
		{ "netmode_server", "Server" },
		{ "netpanel_ping", "Ping: " },
		{ "netpanel_avgping", "Average Ping: " },
		{ "netpanel_laststatus", "Last Status Message:" },
		{ "netpanel_connectedplrs", "Connected Players: " },
		{ "netpanel_settings", "Settings" },
		{ "netpanel_plrlist_full", "More" },
		{ "mainmenu_mp_button_tooltip", "About Multiplayer Mod" },
		{ "mainmenu_settings", "Multiplayer Mod Menu" },
		{ "mainmenu_mainbutton", "Open Multiplayer Mod Menu" },
		{ "mainmenu_mainbutton_close", "Close Multiplayer Mod Menu" },
		{ "mm_apply", "Apply" },
		{ "mmtb_directconnect", "Connection" },
		{ "mmtb_settings", "Settings" },
		{ "mmtb_about", "About" },
		{ "mmtb_serverbrowser", "Server Browser" },
		{ "mmta_og_download", "Official Mod Download Page: " },
		{ "mmta_discord", "Casualties: Together Mod Discord: " },
		{ "mmta_credits", "Credits:" },
		{ "mmta_link_copied", "Copied link!" },
		{ "mmta_link_open", "Open link" },
		{ "mmta_link_copy", "Copy link" },
		{ "mmta_libraries", "Third Party Libraries:" },
		{ "mmct_ipportfield", "IP:PORT" },
		{ "mmct_usernamefield", "Username" },
		{ "mmct_passwordfield", "Password" },
		{ "mmct_hideip", "Hide IP" },
		{ "mmct_show", "Show" },
		{ "mmct_connecting", "Connecting..." },
		{ "mmct_canthostinworld", "Go back to Main Menu to host or connect!" },
		{ "mmct_starthost", "Start Host" },
		{ "mmct_startserver", "Start Server" },
		{ "mmct_startclient", "Client Connect" },
		{ "mmct_serveroptions", "Server Config" },
		{ "mmct_copylobbycode", "Copy Lobby Id" },
		{ "mmct_disconnect_abort", "Abort" },
		{ "mmct_disconnect", "Disconnect" },
		{ "mmct_disconnect_server", "Stop Server" },
		{ "mmct_last_status_msg", "Last Status Message:" },
		{ "mmct_directconnecttitle", "Direct Connect over IP" },
		{ "mmct_gotorunsettings", "Run menu" },
		{ "mmct_gotomainmenu", "Go back to Main Menu" },
		{ "mmct_colornotvalid", "Not valid!" },
		{ "mmct_colortoodark", "Too dark!" },
		{ "ruleschanged", "Changed rules:" },
		{ "rulesdefault", "Rules are default." },
		{ "movemouse", "Move away your mouse" },
		{ "mmct_plrs", "Players" },
		{ "mmct_rules", "Rules" },
		{ "mmct_search", "Search:" },
		{ "mmct_presetname", "Preset Name:" },
		{ "mmct_presetsave", "Save" },
		{ "mmct_presetremove", "Delete" },
		{ "mmct_presetselect", "Selected Preset:" },
		{ "mmct_plr_vc_vol", "VC vol" },
		{ "mmct_plr_ping", "ping:" },
		{ "mmct_plr_setcolor", "Set color" },
		{ "mmct_plr_kick", "Kick" },
		{ "mmct_plr_ban", "Ban" },
		{ "mmct_plr_ipban", "IP Ban" },
		{ "mmct_plr_tcmute", "TC Mute" },
		{ "mmct_plr_vcmute", "VC Mute" },
		{ "mmct_bankicksafety", "Ban/Kick Safety" },
		{ "mmct_plr_steamprofile", "Profile" },
		{ "mmct_plr_mutelabel", "Mute:" },
		{ "mmct_plr_tcmuted", "TC Muted" },
		{ "mmct_plr_vcmuted", "VC Muted" },
		{ "mmct_offline", "Offline" },
		{ "mmct_nobody", "Server is empty..." },
		{ "mmct_reset_rules", "Reset rules" },
		{ "mmct_hoststeamprofile", "Host's Profile" },
		{ "mmct_steamlobby", "Steam Lobby" },
		{ "mmct_steammenutoggle", "Use Steam" },
		{ "mmct_steam_mypersona", "Steam Username:  " },
		{ "mmct_steam_createlobby", "Create Lobby" },
		{ "mmct_steam_openfriendlist", "Open Friends List" },
		{ "mmct_steam_invitefriends", "Invite Friends" },
		{ "mmct_steam_lobbytype", "Lobby Type:" },
		{ "mmct_steam_lobbytype_desc", "Invite Only - Users have to be explicitly invited through Steam\nFriends Only - Only your friends may join\nPublic - Your lobby will be shown in Server Browser" },
		{ "mmct_steam_lobbytype_private", "Invite Only" },
		{ "mmct_steam_lobbytype_friends", "Friends Only" },
		{ "mmct_steam_lobbytype_public", "Public" },
		{ "mmct_sconfig_plrcount", "Max Players: " },
		{ "mmct_sconfig_verifyhash", "Verify Game Hash" },
		{ "mmct_sconfig_verifyhash_tooltip", "This is not anti-cheat.\nIt's only to distinguish playtest versions from normal." },
		{ "mmct_sconfig_enforcemodlist", "Enforce mod list" },
		{ "mmct_sconfig_enforcemodlist_tooltip", "People with different set of mods will not be allowed to join." },
		{ "mmct_sconfig_servername", "Server Name: " },
		{ "mmct_sconfig_dedicated", "Dedicated" },
		{ "mmct_sconfig_dedicated_tooltip", "Run server, but not participate in it.\nThis instance of the game will have no player character." },
		{ "mmct_sconfig_dedicated_disabled", "Already connected." },
		{ "back", "Back" },
		{ "ww_peerid", "PEER ID: " },
		{ "ww_deceased", "DECEASED" },
		{ "ww_cpr", "CPR" },
		{ "ww_cprdesc", "Help your friend pump their blood." },
		{ "ww_cprdesc_no", "They're fine." },
		{ "ww_cprdesc_occupied", "Somebody is already doing CPR." },
		{ "cpr_minigame_guide", "" },
		{ "cpr_minigame_guide_stupid", "" },
		{ "cpr_minigame_slow", "Too slow!" },
		{ "cpr_minigame_good", "Perfect!" },
		{ "cpr_minigame_fast", "Too fast!" },
		{ "cpr_minigame_bad_timing", "Bad timing!" },
		{ "filter_found_nothing", "Filter hid everything." },
		{ "mmst_general", "General" },
		{ "mmst_gui", "User Interface" },
		{ "mmst_voip", "Voice Chat" },
		{ "mmst_chat", "Chat" },
		{ "mmst_debug", "Debug" },
		{ "mmst_forcereloaduitex", "Force reload UI textures" },
		{ "mmst_resettodefault", "Reset to Defaults" },
		{ "mmst_resettodefault_desc", "Reset everything to defaults on this page." },
		{ "mmst_apply", "Save" },
		{ "mmst_voip_rule_disabled", "Voice Chat is currently disabled by server rules." },
		{ "mmst_mike_test", "Test Microphone" },
		{ "mmst_mike_testing", "Microphone Loopback Active!" },
		{ "mmst_chatbutton", "Current Chat Keybind:  " },
		{ "mmst_vcbutton", "Current VC Talk Keybind:  " },
		{ "mmst_gotokeybind", "Click to go to keybind settings." },
		{ "mmst_mike", "Microphone" },
		{ "mmst_mike_osdefault", "OS Default" },
		{ "mmst_select_mike", "Select microphone:" },
		{ "mmsb_lobbyhidepass", "Hide locked" },
		{ "mmsb_lobbyhidever", "Hide incompatible" },
		{ "mmst_button_deactivate_mp", "Deactivate Multiplayer Mod" },
		{ "mmst_button_deactivate_mp_desc", "THIS RESTARTS THE GAME!\nBut when re-launched it's in vanilla state.\nMod can be re-enabled with the \"MP MOD\" button in bottom left." },
		{ "mmst_button_deactivate_mp_desc_disabled", "You're aleady playing the game.\nDisconnect and go to main menu." },
		{ "mainmenu_mp_button_tooltip_lastresort_disable_mod", "Force Disable the Multiplayer Mod" },
		{ "mainmenu_mp_button_tooltip_lastresort_enable_mod", "Enable Multiplayer Mod. (This restarts the game!)" },
		{ "freecam_keybindalert", "Use arrow keys to move." },
		{ "rule_desc_sv_cheats", "Unlock all console commands on server." },
		{ "rule_desc_Teams", "Players with same color are considered a team.\nPlayer Scatter Distance works only for your team.\nPVP only works against other teams." },
		{ "rule_desc_XPGainMultiplier", "XP Gain multiplier, for INT, STR, RES together" },
		{ "rule_desc_EnableTimeManipulation", "Don't enable this. Not recommended." },
		{ "rule_desc_LateJoinSpectate", "Late joined players will instantly die and put into spectator mode." },
		{ "rule_desc_NoInventoryLock", "Can steal stuff from other players even if they're conscious." },
		{ "rule_desc_StragglerRadlinePercent", "Kill stragglers if this % of people reach the end." },
		{ "rule_desc_ScatterPunishDistance", "Irradiate people who leave everyone behind for this distance." },
		{ "rule_desc_ScatterMinGroupSize", "How many people is considered as a big group.\nTo not trigger ScatterPunishDistance" },
		{ "rule_desc_AllowClientCheatCommands", "Deprecated, you should just give them \"adminpriv\"\nThis rule only allows for executing client-side commands, like \"tp\"" },
		{ "rule_desc_AlwaysAllowCarry", "Allow picking up and carrying conscious and standing players." },
		{ "rule_desc_OnlyProximityChat", "Text chat can only be heard within radius of \"ProximityHearDistance\"." },
		{ "rule_desc_PVP", "Enable friendly fire." },
		{ "rule_desc_SavePlayerState", "Save player health when disconnected." },
		{ "rule_desc_SavePlayerInventory", "Save player inventory when disconnected.\nTHIS DOES NOT PERSIST IN SAVE FILES!" },
		{ "rule_desc_SavePlayerPosition", "Disconnected player will spawn at their last position when rejoined." },
		{ "rule_desc_Permadeath", "Disables all types of respawn." },
		{ "rule_desc_ReviveOnNextLevel", "Respawn all dead players after layer transition." },
		{ "rule_desc_ReviveFromTrader", "Ability to \"Recruit\" traders, to respawn one random player." },
		{ "rule_desc_PLAYER_COUNT_LIMIT", "" },
		{ "rule_desc_ShowPlayerDirections", "" },
		{ "rule_desc_EnableNametags", "" },
		{ "rule_desc_EnableStatusIcons", "" },
		{ "rule_desc_UnchippedHideNametags", "" },
		{ "rule_desc_EnableChatbox", "" },
		{ "rule_desc_UnchippedProximityChat", "" },
		{ "rule_desc_UnchippedIsIndividual", "" },
		{ "rule_desc_LayerFinishPlrPercent", "" },
		{ "rule_desc_EnableSleep", "" },
		{ "rule_desc_SpeechImpairedChat", "" },
		{ "rule_desc_HearingLossChat", "" },
		{ "rule_desc_MindwipeDisablesChat", "" },
		{ "rule_desc_DeadTextchat", "" },
		{ "rule_desc_DeadVoicechat", "" },
		{ "rule_desc_RespawnKeepInventory", "" },
		{ "rule_desc_RespawnKeepSkills", "" },
		{ "rule_desc_AllowSpectatorFreecam", "" },
		{ "rule_desc_AllowPush", "" },
		{ "rule_desc_PiggybackMaxStack", "" },
		{ "rule_desc_PiggybackWeightMultiplier", "" },
		{ "rule_desc_SpectateWhileUnconscious", "" },
		{ "rule_desc_EnableMP3Sync", "" },
		{ "rule_desc_VoicechatQuality", "" },
		{ "rule_desc_VoicechatEnabled", "" },
		{ "rule_desc_ProximityHearDistance", "" },
		{ "rule_desc_CharacterYapPublic", "" },
		{ "rule_desc_PVPCombatDismember", "" },
		{ "rule_desc_PVPMoodDebuff", "" },
		{ "rule_desc_PVPDamageMultiplier", "" },
		{ "rule_desc_LateJoinAllowed", "" },
		{ "rule_desc_AmputateHealthyPlayers", "" },
		{ "rule_desc_AdditionalHealthRegen", "" },
		{ "rule_desc_AdditionalHealthDecay", "" },
		{ "rule_desc_SelfharmWitnessMoodDebuff", "" },
		{ "rule_desc_AutoContinue", "" },
		{ "rule_desc_AutoMinPlrsToStart", "" },
		{ "rule_desc_AutoExitWhenAllDied", "" },
		{ "rule_desc_AutoExitWhenAllLeft", "" },
		{ "setting_chatposx", "Chat position X" },
		{ "setting_chatposy", "Chat Position Y" },
		{ "setting_chatsizex", "Chat Size X" },
		{ "setting_chatsizey", "Chat Size Y" },
		{ "setting_chatscale", "Chat Scale" },
		{ "setting_chatpreview", "Chat Preview" },
		{ "setting_chatpreview_desc", "Preview speech impaired distortions before sending your chat message." },
		{ "setting_chatpreviewonside", "Chat Preview Horizontal" },
		{ "setting_chatpreviewonside_desc", "Move chat input preview box to the side." },
		{ "setting_vcattenuation", "BG Music Attenuation" },
		{ "setting_vcattenuation_desc", "How much to temporarily silence background music when someone is speaking." },
		{ "setting_micmode", "VC Mode" },
		{ "setting_micmode_choice_off", "Off" },
		{ "setting_micmode_choice_ptt", "Push to talk" },
		{ "setting_micmode_choice_on", "Always On" },
		{ "setting_micmode_choice_toggle", "Toggle to talk" },
		{ "setting_mp3vol", "MP3 Volume" },
		{ "setting_vclistenvol", "VC Listener Volume" },
		{ "setting_micvol", "VC Microphone Volume" },
		{ "setting_vclistenvol_desc", "Increase volume of other players." },
		{ "setting_micvol_desc", "Boost your microphone volume." },
		{ "mmst_mike_local_off", "Microphone is disabled." },
		{ "setting_ui_usegamefont", "Use Game Font" },
		{ "setting_ui_discordrpc_allowjoin", "Discord RPC Join button" },
		{ "setting_ui_discordrpc_onlyask", "Discord Only Ask to Join" },
		{ "setting_debug_verbose", "Verbose Log" },
		{ "setting_debug_logsteam", "Log Steam" },
		{ "setting_debug_events", "Show Net events" },
		{ "mmsb_refresh", "Refresh" },
		{ "mmsb_empty", "List Empty  :(\nClick Refresh" },
		{ "mmsb_lobbybadver", "Wrong version!" },
		{ "mmsb_lobbyjoinspectate", "Spectate" },
		{ "mmsb_lobbyjoin", "Join" },
		{ "mmsb_lobbyjoin_alreadyjoined", "Already in a server." },
		{ "mmsb_lobbyjoin_nomidjoin", "Mid-game join is not allowed here." },
		{ "mmsb_lobbyjoin_wrongmods", "This server requires different set of mods." },
		{ "mmsb_sort", "Sort: " },
		{ "mmsb_lobby_islocked", "Has password protection." },
		{ "mmsb_lobby_inmenu", "In Main Menu" },
		{ "mmsb_lobby_tutorial", "In Tutorial" },
		{ "mmsb_lobby_debug_world", "In Debug World" },
		{ "mmsb_lobby_alivecount", "Alive: " },
		{ "mmsb_lobby_avgmood", "Mood: " },
		{ "mmsb_lobby_listmod", "Mods" },
		{ "mmsb_lobby_listplrs", "Players" },
		{ "mmsb_lobby_listrules", "Rules" },
		{ "mmsb_lobby_loading", "Loading" },
		{ "mmsb_lobbyfounds", "Found Lobbies: {0}   Shown: {1}   Total Players: {2}" },
		{ "mmsb_steam_distfilter", "Physical Distance: " },
		{ "mmsb_sort_name", " A-Z " },
		{ "mmsb_sort_plrcount", "Player Count" },
		{ "mmsb_sort_layer", "Layer" },
		{ "mmsb_steam_lobbyfriend", "Your Friend is in this lobby:" },
		{ "mmsb_steam_dist0", "Near" },
		{ "mmsb_steam_dist1", "Default" },
		{ "mmsb_steam_dist2", "Far" },
		{ "mmsb_steam_dist3", "Worldwide" },
		{ "mmbr_send", "Send Bug Report" },
		{ "mmbr_terms", "By sending you also acknowledge that the report includes:\nYour PC specs\nYour Public SteamID and Discord UserID\nYour OS Username\nChat logs\nList of players you play with\nScreenshot" },
		{ "setting_bugreport_includelogprev", "Include previous log" },
		{ "setting_bugreport_includelogprev_desc", "If the game crashed or you closed the game" },
		{ "mmst_button_checkupdate", "Check for Updates" },
		{ "mmst_button_checkupdate_desc", "Automatically install a new version of MP mod, if available." },
		{ "setting_bugreport_delayscreenshot", "Delay Screenshot" },
		{ "setting_bugreport_delayscreenshot_desc", "Click \"Send Bug Report\" to enter screenshot mode.\nThen press F1 to capture screenshot and send bug report." },
		{ "mmst_bugreport", "Bug Report" },
		{ "mmbr_send_tooltip", "" },
		{ "mmbr_send_tooltip_desc", "Please be respectful.\nDo report something that may be already known. But don't spam.\nSame reports from different people prioritizes the bug." },
		{ "mmst_button_restart", "Restart the Game" },
		{ "mmst_button_restart_desc", "Restart to apply update." },
		{ "trader_recruit", "Recruit" },
		{ "trader_recruitdesc", "Let a random player respawn." },
		{ "trader_recruitdesc_no_rep", "Not enough reputation." },
		{ "trader_recruitdesc_no_plr", "There's no dead players on the server." },
		{ "gamesetkrokosha_coop_pointfingerat", "MP Highlight location" },
		{ "gamesetkrokosha_coop_chat", "MP Chat" },
		{ "gamesetkrokosha_coop_voicechat", "MP Voicechat" },
		{ "gamesetkrokosha_coop_push", "MP Push" },
		{ "gamesetkrokosha_coop_carry", "MP Carry" },
		{ "gamesetkrokosha_coop_piggyback", "MP Piggyback" },
		{ "gamesetkrokosha_coop_woundview", "MP Another's Healthpanel" },
		{ "gamesetkrokosha_coop_inventory", "MP Another's Inventory" },
		{ "gamesetkrokosha_coop_showplrs", "MP Show players" },
		{ "discord_joinrequest", "Discord Join Request" },
		{ "discord_joinrequesttext", "{0} wants to join your game." },
		{ "discord_joinallow", "Allow" },
		{ "discord_joindecline", "Decline" },
		{ "yes", "Yes" },
		{ "no", "No" }
	};

	internal static Dictionary<string, Dictionary<string, string>> dict = new Dictionary<string, Dictionary<string, string>>
	{
		["EN"] = EN,
		["DE"] = new Dictionary<string, string>
		{
			{ "plr_hover_dead", " (TOT)" },
			{ "plr_hover_sleeping", " (SCHLAFEND)" },
			{ "plr_hover_unconscious", " (BEWUSSTLOS)" },
			{ "woundview_deceased", "VERSTORBENER" }
		},
		["RU"] = new Dictionary<string, string>
		{
			{ "plr_hover_dead", " (МЁРТВ)" },
			{ "plr_hover_sleeping", " (СПИТ)" },
			{ "plr_hover_unconscious", " (БЕЗ СОЗНАНИЯ)" },
			{ "woundview_deceased", "АННУЛИРОВАНО" },
			{ "healthchipalert_antidispersion_warn", "ОТСТАЛ ОТ\nТОВАРИЩЕЙ" },
			{ "healthchipalert_antidispersion1", "ВЕРНИСЬ К\nТОВАРИЩАМ" },
			{ "healthchipalert_antidispersion3", "ОСТАВАЙТЕСЬ\nВМЕСТЕ" },
			{ "plr_started_drillpod", "{0} запустил Бур-капсулу." },
			{ "loaderror_possible_nonascii_path", "Проверь если путь к игре содержит только английские символы:" }
		}
	};

	private static HashSet<string> failed = new HashSet<string>();

	internal const string prefix = "krokosha_coop_";

	public const string MSG_TYPE_DEF_SEPARATOR = "$\r";

	public const string TRANSLATION_KEY_MSG_MARK = "T$\r";

	internal static string GetEN(string key)
	{
		if (EN.TryGetValue(key, out var value))
		{
			return value;
		}
		return null;
	}

	public static string Get(in string key, in bool allow_null_result = false)
	{
		string value = null;
		string text = Locale.currentLangName ?? "EN";
		if (dict.TryGetValue(text, out var value2))
		{
			value2.TryGetValue(key, out value);
		}
		string text2 = "krokosha_coop_" + key;
		if (Locale.currentLang != null)
		{
			if (Locale.currentLang.other.TryGetValue(text2, out var value3))
			{
				return value3;
			}
			if (value != null)
			{
				return value;
			}
			if (!failed.Contains(text2))
			{
				failed.Add(text2);
				string eN = GetEN(text2);
				if (string.IsNullOrEmpty(eN))
				{
					if (!allow_null_result)
					{
						log.error("Lang: Failed to get " + text + " translation for \"" + text2 + "\"");
					}
				}
				else
				{
					log.error("Lang: Failed to get " + text + " translation for \"" + text2 + "\" -> OG: \"" + eN + "\"");
				}
			}
		}
		if (value == null)
		{
			if (!KrokoshaScavMultiplayer.verbose && EN.TryGetValue(key, out value) && !string.IsNullOrEmpty(value))
			{
				return value;
			}
			if (!allow_null_result)
			{
				value = "ERROR_NO_LOCALE_" + text + "_" + text2;
			}
		}
		return value;
	}

	public static bool TryGet(in string key, out string text)
	{
		text = Get(in key, true);
		return text != null;
	}

	public static string MarkMsgAsLocaleKey(in string message)
	{
		return "T$\r" + message;
	}

	public static void MsgTryTranslateIfItsLocaleKey(ref string message)
	{
		if (message.StartsWith("T$\r"))
		{
			message = Get(message.Substring("T$\r".Length), false);
		}
	}
}
