using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using KrokoshaCasualtiesUtils;
using TMPro;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(Talker), "Talk", new Type[]
{
	typeof(List<string>),
	typeof(Limb),
	typeof(bool),
	typeof(bool)
})]
public static class Talker_Talk_MultiplayerPatch
{
	private static bool force_run = false;

	private static double last_sent_time = -999999.0;

	public static void Force(Talker __instance, List<string> lines, Limb limb = null, bool force = false, bool resetTalkTimer = false)
	{
		force_run = true;
		__instance.Talk(lines, limb, force, resetTalkTimer);
		force_run = false;
	}

	public static void ForceNoSpeechImpairment(this Talker talker, string message, bool resetTalkTimer = false)
	{
		talker.currentString = message;
		((TMP_Text)talker.text).text = "";
		talker.timeSinceTalked = 0f;
		if (resetTalkTimer)
		{
			talker.newTalkTime = 0f;
		}
	}

	public static string DistortString(this Talker talker, in string message)
	{
		MindwipeScript mindWipe = null;
		if ((Object)(object)talker.body != (Object)null)
		{
			mindWipe = talker.body.mindWipe;
			if (!KrokoshaScavMultiplayer.rules.MindwipeDisablesChat)
			{
				talker.body.mindWipe = null;
			}
		}
		string currentString = talker.currentString;
		float timeSinceTalked = talker.timeSinceTalked;
		float newTalkTime = talker.newTalkTime;
		string text = ((TMP_Text)talker.text).text;
		talker.currentString = "";
		Force(talker, new List<string> { message }, null, force: true);
		string text2 = talker.currentString;
		if (string.IsNullOrEmpty(text2))
		{
			StringBuilder stringBuilder = new StringBuilder(message);
			for (int i = 0; i < message.Length; i++)
			{
				if (Chat.CharCanBeReplacedWithBlank(stringBuilder[i]))
				{
					stringBuilder[i] = '_';
				}
			}
			text2 = stringBuilder.ToString();
		}
		talker.currentString = currentString;
		talker.timeSinceTalked = timeSinceTalked;
		talker.newTalkTime = newTalkTime;
		((TMP_Text)talker.text).text = text;
		if ((Object)(object)talker.body != (Object)null)
		{
			talker.body.mindWipe = mindWipe;
		}
		return text2;
	}

	private static bool Prefix(Talker __instance)
	{
		if (KrokoshaScavMultiplayer.network_system_is_running && !force_run)
		{
			if ((Object)(object)__instance.trader != (Object)null)
			{
				if (KrokoshaScavMultiplayer.is_client)
				{
					return false;
				}
			}
			else if (KrokoshaScavMultiplayer.rules.CharacterYapPublic)
			{
				if ((Object)(object)__instance.body != (Object)null && KrokoshaScavMultiplayer.is_client)
				{
					return false;
				}
			}
			else if ((Object)(object)__instance.body != (Object)null && !Util.IsBodyLocal(__instance.body))
			{
				return false;
			}
		}
		return true;
	}

	private static void Postfix(Talker __instance)
	{
		if (KrokoshaScavMultiplayer.network_system_is_running && !force_run)
		{
			string curstr = __instance.currentString;
			Action<knetid, byte> action = delegate(knetid iiii, byte tttttt)
			{
				if (Time.realtimeSinceStartupAsDouble - last_sent_time > 0.10000000149011612)
				{
					last_sent_time = Time.realtimeSinceStartupAsDouble;
					ServerMain.TalkerSayAnnounce(in iiii, in tttttt, in curstr, ServerMain.AllClientIdsExceptHost);
				}
			};
			if (!KrokoshaScavMultiplayer.is_client && !string.IsNullOrEmpty(curstr))
			{
				if ((Object)(object)__instance.trader != (Object)null)
				{
					if (NetObjectRegistry.TryGetSyncInfo((Component)(object)__instance.trader, out var si))
					{
						action(si.syncId, 0);
					}
				}
				else if (KrokoshaScavMultiplayer.rules.CharacterYapPublic)
				{
					if ((Object)(object)__instance.body != (Object)null && __instance.body.TryGetNetPlayer(out var plr))
					{
						action(plr.clientId, 1);
					}
				}
				else if ((Object)(object)__instance.body != (Object)null)
				{
					Util.IsBodyLocal(__instance.body);
				}
			}
		}
		force_run = false;
	}
}
