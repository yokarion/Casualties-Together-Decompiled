using System;
using System.Collections.Generic;
using System.Linq;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public class VoteSystem : KrokoshaScavSingleton
{
	public delegate void VoteEndAction(HashSet<NetPlayer> voted_yes, HashSet<NetPlayer> voted_no, HashSet<NetPlayer> voted_ignore);

	public class Server_VoteState
	{
		public ushort id;

		public float time;

		public VoteEndAction endAction;

		public HashSet<NetPlayer> voters;

		public HashSet<NetPlayer> votedPositive = new HashSet<NetPlayer>();

		public HashSet<NetPlayer> votedNegative = new HashSet<NetPlayer>();
	}

	private static ushort _voteCounter;

	public static Server_VoteState Server_ActiveVote;

	private void Start()
	{
		ServerMain.ServerClientCustomCommandsDict.Add("vote", delegate(NetPlayer plr, string cmd, string[] argv)
		{
			string callername = "SERVER";
			if ((Object)(object)plr != (Object)null)
			{
				callername = plr.playername;
			}
			if (argv[1] == "kick")
			{
				ServerMain._PerformActionOnPlayersByName(argv[2], delegate(NetPlayer oplr)
				{
					if (!oplr.is_local)
					{
						Chat.Server_ChatAnnouncement(callername + " called a vote kick for " + oplr.playername);
						Server_VoteKick(oplr, plr);
					}
				}, allow_macros: false, plr, require_body: false, randomExcludeCaller: true, onlyOne: true);
			}
			else if (argv[1] == "mutevc")
			{
				ServerMain._PerformActionOnPlayersByName(argv[2], delegate(NetPlayer oplr)
				{
					if (!oplr.is_local)
					{
						Chat.Server_ChatAnnouncement(callername + " called a vote mute vc for " + oplr.playername);
						Server_VoteMuteVC(oplr, plr);
					}
				}, allow_macros: false, plr, require_body: false, randomExcludeCaller: true, onlyOne: true);
			}
			else if (argv[1] == "mutetc")
			{
				ServerMain._PerformActionOnPlayersByName(argv[2], delegate(NetPlayer oplr)
				{
					if (!oplr.is_local)
					{
						Chat.Server_ChatAnnouncement(callername + " called a vote mute tc for " + oplr.playername);
						Server_VoteMuteTC(oplr, plr);
					}
				}, allow_macros: false, plr, require_body: false, randomExcludeCaller: true, onlyOne: true);
			}
		});
	}

	public static void Server_VoteKick(NetPlayer plr, NetPlayer who_called_the_vote = null)
	{
		Server_AnnounceVote("Vote Kick", "Kick " + plr.playername, 10f, (VoteEndAction)delegate(HashSet<NetPlayer> voted_yes, HashSet<NetPlayer> voted_no, HashSet<NetPlayer> voted_ignore)
		{
			voted_ignore.Remove(plr);
			float num = voted_yes.Count + voted_no.Count + voted_ignore.Count;
			float num2 = voted_yes.Count;
			if (num > 0f)
			{
				float num3 = num2 / num;
				string text = Mathf.RoundToInt(num3 * 100f).ToString();
				Chat.Server_ChatAnnouncement("Vote kick result: " + text + "%");
				if (num3 > 0.5f && (Object)(object)plr != (Object)null)
				{
					plr.Server_Kick("Vote kicked. " + text + "%");
				}
			}
		});
	}

	public static void Server_VoteMuteVC(NetPlayer plr, NetPlayer who_called_the_vote = null)
	{
		Server_AnnounceVote("Vote Mute VC", "Mute " + plr.playername, 10f, (VoteEndAction)delegate(HashSet<NetPlayer> voted_yes, HashSet<NetPlayer> voted_no, HashSet<NetPlayer> voted_ignore)
		{
			voted_ignore.Remove(plr);
			float num = voted_yes.Count + voted_no.Count + voted_ignore.Count;
			float num2 = voted_yes.Count;
			if (num > 0f)
			{
				float num3 = num2 / num;
				Chat.Server_ChatAnnouncement($"Vote mute vc result: {Mathf.RoundToInt(num3 * 100f)}%");
				if (num3 > 0.5f && (Object)(object)plr != (Object)null)
				{
					plr.server_mute_vc = true;
				}
			}
		});
	}

	public static void Server_VoteMuteTC(NetPlayer plr, NetPlayer who_called_the_vote = null)
	{
		Server_AnnounceVote("Vote Mute TC", "Mute " + plr.playername, 10f, (VoteEndAction)delegate(HashSet<NetPlayer> voted_yes, HashSet<NetPlayer> voted_no, HashSet<NetPlayer> voted_ignore)
		{
			voted_ignore.Remove(plr);
			float num = voted_yes.Count + voted_no.Count + voted_ignore.Count;
			float num2 = voted_yes.Count;
			if (num > 0f)
			{
				float num3 = num2 / num;
				Chat.Server_ChatAnnouncement($"Vote mute tc result: {Mathf.RoundToInt(num3 * 100f)}%");
				if (num3 > 0.5f && (Object)(object)plr != (Object)null)
				{
					plr.server_mute_tc = true;
				}
			}
		});
	}

	public static void Server_AnnounceVote(in string title, in string message, in float timetovote, in VoteEndAction action, IReadOnlyList<NetPlayer> voters = null)
	{
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		if (Net.is_client)
		{
			throw new Exception("Client can't do that!");
		}
		if (Server_ActiveVote != null)
		{
			throw new Exception("Already voting");
		}
		if (voters == null)
		{
			voters = NetPlayer.ClientIdToPlayerDict.Values.ToList();
		}
		if (voters.Count > 0)
		{
			Server_ActiveVote = new Server_VoteState
			{
				id = _voteCounter++,
				time = timetovote + 1f,
				endAction = action,
				voters = new HashSet<NetPlayer>(voters)
			};
			NetDataWriter writer = Net.CreateWriter(10182);
			writer.Put(Server_ActiveVote.id);
			writer.Put(title);
			writer.Put(message);
			writer.Put(timetovote);
			Net.Server_SendToClients((DeliveryMethod)2, in writer, in voters);
		}
		else
		{
			action(null, null, null);
		}
	}

	[ClientReceiver(10182, false)]
	private static void ClientReceiver__AnnounceVote(knetid _, ref NetDataReader reader)
	{
		ushort voteid = reader.GetUShort();
		string message = reader.GetString();
		string message2 = reader.GetString();
		float timeout = reader.GetFloat();
		Lang.MsgTryTranslateIfItsLocaleKey(ref message);
		Lang.MsgTryTranslateIfItsLocaleKey(ref message2);
		UIChoicePrompt.Prompt prompt = new UIChoicePrompt.Prompt();
		prompt.text = message2;
		prompt.name = message;
		prompt.timeout = timeout;
		prompt.defaultcanignore = true;
		prompt.choices = new UIChoicePrompt.Prompt.PromptChoice[2]
		{
			new UIChoicePrompt.Prompt.PromptChoice
			{
				text = Lang.Get("yes", false),
				action = delegate
				{
					//IL_001f: Unknown result type (might be due to invalid IL or missing references)
					NetDataWriter writer = Net.CreateWriter(10183);
					writer.Put(voteid);
					writer.Put((byte)0);
					Net.Client_Send((DeliveryMethod)2, in writer);
				}
			},
			new UIChoicePrompt.Prompt.PromptChoice
			{
				text = Lang.Get("no", false),
				action = delegate
				{
					//IL_001f: Unknown result type (might be due to invalid IL or missing references)
					NetDataWriter writer = Net.CreateWriter(10183);
					writer.Put(voteid);
					writer.Put((byte)1);
					Net.Client_Send((DeliveryMethod)2, in writer);
				}
			}
		};
		UIChoicePrompt.ShowPrompt(prompt);
	}

	[ServerReceiver(10183)]
	private static void ServerReceiver__Vote(knetid clientId, ref NetDataReader reader)
	{
		reader.GetUShort();
		byte b = reader.GetByte();
		if (NetPlayer.TryGetPlayerFromClientId(clientId, out var plr) && Server_ActiveVote != null && Server_ActiveVote.voters.Contains(plr) && !Server_ActiveVote.votedPositive.Contains(plr) && !Server_ActiveVote.votedNegative.Contains(plr))
		{
			if (b == 0)
			{
				Server_ActiveVote.votedPositive.Add(plr);
			}
			else
			{
				Server_ActiveVote.votedNegative.Add(plr);
			}
		}
	}

	private void Update()
	{
		if (!Net.is_server || Server_ActiveVote == null)
		{
			return;
		}
		Server_ActiveVote.time -= Time.deltaTime;
		Server_ActiveVote.voters.RemoveWhere((NetPlayer x) => (Object)(object)x == (Object)null);
		Server_ActiveVote.votedPositive.RemoveWhere((NetPlayer x) => (Object)(object)x == (Object)null);
		Server_ActiveVote.votedNegative.RemoveWhere((NetPlayer x) => (Object)(object)x == (Object)null);
		HashSet<NetPlayer> hashSet = new HashSet<NetPlayer>(Server_ActiveVote.voters);
		hashSet.ExceptWith(Server_ActiveVote.votedPositive);
		hashSet.ExceptWith(Server_ActiveVote.votedNegative);
		if (Server_ActiveVote.time < 0f || hashSet.Count == 0)
		{
			try
			{
				Server_ActiveVote.endAction(Server_ActiveVote.votedPositive, Server_ActiveVote.votedNegative, hashSet);
			}
			catch (Exception ex)
			{
				log.error(ex.ToString());
			}
			Server_ActiveVote = null;
		}
	}
}
