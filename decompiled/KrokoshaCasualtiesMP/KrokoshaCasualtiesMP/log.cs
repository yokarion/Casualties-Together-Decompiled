using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

internal class log
{
	public const string name = "MP";

	public const string prefix1 = "MP: ";

	public static bool verbose => KrokoshaScavMultiplayer.verbose;

	public static ConsoleScript con => ConsoleScript.instance;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static string do_timestamp(string s)
	{
		return "[" + DateTime.UtcNow.ToString("HH:mm:ss.fff") + "] " + s;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void devevent(in string s, Vector2 eventpos, bool ignoreverbose = false)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		devevent(s, eventpos, Color.cyan, ignoreverbose);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void devevent(string s, Vector2 eventpos, Color color, bool ignoreverbose)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		Plugin.log.LogInfo((object)do_timestamp(s));
		if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
		{
			DebugHelp.OnNetEvent(eventpos, in s, color);
		}
		if ((verbose || ignoreverbose) && Object.op_Implicit((Object)(object)con))
		{
			con.LogToConsole("MP: " + s);
		}
	}

	public static void l(string s)
	{
		l(s, console: true);
	}

	public static void l(string s, bool console)
	{
		s.Replace('\r', ' ');
		Plugin.log.LogInfo((object)do_timestamp(s));
		if (console && Object.op_Implicit((Object)(object)con))
		{
			con.LogToConsole("MP: " + s);
		}
	}

	public static void warn(string s)
	{
		warn(s, console: true);
	}

	public static void warn(string s, bool console)
	{
		s.Replace('\r', ' ');
		Plugin.log.LogWarning((object)do_timestamp(s));
		if (console && Object.op_Implicit((Object)(object)con))
		{
			con.LogToConsole("<color=yellow>MP WARN: " + s + "</color>");
		}
	}

	public static void error_devevent(in string s, in Vector2 pos)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		s.Replace('\r', ' ');
		error(s, console: true);
		if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
		{
			DebugHelp.OnNetEvent(pos, "ERROR: " + s, Color.red);
		}
	}

	public static void error(string s)
	{
		error(s, console: true);
	}

	public static void error(string s, bool console)
	{
		s.Replace('\r', ' ');
		Plugin.log.LogError((object)do_timestamp(s));
		if (console && Object.op_Implicit((Object)(object)con))
		{
			con.LogToConsole("<color=red>MP ERROR: " + s + "</color>");
		}
	}

	public static void sus(string s, bool console = true)
	{
		s.Replace('\r', ' ');
		s = "SUS: " + s;
		Plugin.log.LogWarning((object)do_timestamp(s));
		if (console && Object.op_Implicit((Object)(object)con) && KrokoshaScavMultiplayer.verbose)
		{
			con.LogToConsole("<color=orange>MP: " + s + "</color>");
		}
	}

	public static void serverdeny(string s)
	{
		s.Replace('\r', ' ');
		s = "SERVER DENY: " + s;
		Plugin.log.LogWarning((object)do_timestamp(s));
		if (Object.op_Implicit((Object)(object)con) && KrokoshaScavMultiplayer.verbose)
		{
			con.LogToConsole("<color=yellow>MP: " + s + "</color>");
		}
	}

	public static void serverdeny(string s, Vector2 pos)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		serverdeny(s);
		if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
		{
			DebugHelp.OnNetEvent(pos, "S: DENIED " + s, Color.red);
		}
	}
}
