namespace KrokoshaCasualtiesMP;

public static class DebugMenuSettings
{
	[SettingDeclarerThingyBool("setting_debug_verbose", allow_saving = false)]
	internal static bool is_verbose;

	[SettingDeclarerThingyBool("setting_debug_events", allow_saving = false)]
	public static bool _DEV_VISUALISE_NET_EVENTS;

	[SettingDeclarerThingyBool("setting_debug_logsteam", allow_saving = false)]
	public static bool _DEV_ENABLE_STEAM_LOG;
}
