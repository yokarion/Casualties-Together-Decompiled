using System;
using System.Runtime.CompilerServices;
using DiscordRPC;
using DiscordRPC.Events;
using DiscordRPC.Message;
using HarmonyLib;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(RPCManager), "Initialize")]
internal static class RPCManager_Initialize_MultiplayerPatch
{
	private struct dffdsfdsfdssfs
	{
		public bool aaaaaaaaaaaaaaaa;

		public bool bbbbbbbbbbbbbbbb;
	}

	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static OnJoinEvent _003C_003E9__3_1;

		public static OnReadyEvent _003C_003E9__3_2;

		public static OnErrorEvent _003C_003E9__3_3;

		internal void _003CPrefix_003Eb__3_1(object sender, JoinMessage e)
		{
			log.l("DiscordRPC: OnJoin called ");
			if (!KrokoshaScavMultiplayer.IsNetworkActiveOrIsInGame())
			{
				string[] array = e.Secret.Split(new char[1] { ':' });
				KrokoshaScavMultiplayer.CLIENT_JOIN_SECRET = array[1];
				TransportSteamworks.OnWantToJoinLobby(ulong.Parse(array[0]));
			}
		}

		internal void _003CPrefix_003Eb__3_2(object sender, ReadyMessage e)
		{
			log.l("Discord RPC Ready: " + BetterDiscordUserToString(e.User));
		}

		internal void _003CPrefix_003Eb__3_3(object sender, ErrorMessage e)
		{
			Debug.LogError((object)("Discord RPC Error: " + e.Message));
		}
	}

	internal static Timestamps rpcStartTime = Timestamps.Now;

	public static string BetterDiscordUserToString(User dsusr)
	{
		if (string.IsNullOrWhiteSpace(dsusr.DisplayName))
		{
			return $"{dsusr.Username} - {dsusr.ID}";
		}
		return $"{dsusr.DisplayName} - {dsusr.Username} - {dsusr.ID}";
	}

	private static bool Prefix(RPCManager __instance, ref dffdsfdsfdssfs __state)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Expected O, but got Unknown
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Expected O, but got Unknown
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Expected O, but got Unknown
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Expected O, but got Unknown
		rpcStartTime = Timestamps.Now;
		__instance.client = new DiscordRpcClient("1506330530394804254");
		__instance.client.RegisterUriScheme((string)null, (string)null);
		__instance.client.Subscribe((EventType)4);
		__instance.client.OnJoinRequested += (OnJoinRequestedEvent)async delegate(object sender, JoinRequestMessage args)
		{
			log.l("DiscordRPC: OnJoinRequested called");
			if (Net.running && Net.is_server)
			{
				string text = ((object)args.User).ToString();
				UIChoicePrompt.Prompt prompt = new UIChoicePrompt.Prompt
				{
					text = string.Format(Lang.Get("discord_joinrequesttext", false), text ?? ""),
					name = Lang.Get("discord_joinrequest", false),
					size = new Vector2(0.4f, 0.18f),
					choices = new UIChoicePrompt.Prompt.PromptChoice[2]
					{
						new UIChoicePrompt.Prompt.PromptChoice
						{
							text = Lang.Get("discord_joinallow", false),
							action = delegate
							{
								log.l("DiscordRPC: Accepted Join Request for user: " + BetterDiscordUserToString(args.User));
							}
						},
						new UIChoicePrompt.Prompt.PromptChoice
						{
							text = Lang.Get("discord_joindecline", false),
							action = delegate
							{
								log.l("DiscordRPC: Declined Join Request for user: " + BetterDiscordUserToString(args.User));
							}
						}
					}
				};
				await UIChoicePrompt.DoPromptAsync(prompt);
				if (prompt.chosenchoice == 0)
				{
					__instance.client.Respond(args, true);
				}
				else
				{
					__instance.client.Respond(args, false);
				}
			}
			else
			{
				__instance.client.Respond(args, false);
			}
		};
		__instance.client.Subscribe((EventType)2);
		DiscordRpcClient client = __instance.client;
		object obj = _003C_003Ec._003C_003E9__3_1;
		if (obj == null)
		{
			OnJoinEvent val = delegate(object sender, JoinMessage e)
			{
				log.l("DiscordRPC: OnJoin called ");
				if (!KrokoshaScavMultiplayer.IsNetworkActiveOrIsInGame())
				{
					string[] array = e.Secret.Split(new char[1] { ':' });
					KrokoshaScavMultiplayer.CLIENT_JOIN_SECRET = array[1];
					TransportSteamworks.OnWantToJoinLobby(ulong.Parse(array[0]));
				}
			};
			_003C_003Ec._003C_003E9__3_1 = val;
			obj = (object)val;
		}
		client.OnJoin += (OnJoinEvent)obj;
		DiscordRpcClient client2 = __instance.client;
		object obj2 = _003C_003Ec._003C_003E9__3_2;
		if (obj2 == null)
		{
			OnReadyEvent val2 = delegate(object sender, ReadyMessage e)
			{
				log.l("Discord RPC Ready: " + BetterDiscordUserToString(e.User));
			};
			_003C_003Ec._003C_003E9__3_2 = val2;
			obj2 = (object)val2;
		}
		client2.OnReady += (OnReadyEvent)obj2;
		DiscordRpcClient client3 = __instance.client;
		object obj3 = _003C_003Ec._003C_003E9__3_3;
		if (obj3 == null)
		{
			OnErrorEvent val3 = delegate(object sender, ErrorMessage e)
			{
				Debug.LogError((object)("Discord RPC Error: " + e.Message));
			};
			_003C_003Ec._003C_003E9__3_3 = val3;
			obj3 = (object)val3;
		}
		client3.OnError += (OnErrorEvent)obj3;
		if (__instance.client.Initialize())
		{
			__instance.client.SetPresence(__instance.GetCurrentRichPresence());
		}
		else
		{
			log.error("Failed to initialize discord rpc");
		}
		return false;
	}
}
