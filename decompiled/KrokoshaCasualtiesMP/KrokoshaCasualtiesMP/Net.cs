using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using KrokoshaCasualtiesUtils;
using LiteNetLib;
using LiteNetLib.Utils;
using Steamworks;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public static class Net
{
	public enum NetType
	{
		Client,
		Host,
		DedicatedServer
	}

	public static NetPublicServerInfo MY_SERVER_INFO = new NetPublicServerInfo();

	public static NetPublicServerInfo cur_server_info = new NetPublicServerInfo();

	public const ushort DEFAULT_PORT = 7790;

	public const string IP_LOCALHOST_IPv4 = "127.0.0.1";

	public const string IP_LOCALHOST_IPv6 = "::1";

	public const int SERVER_CLIENTID = 0;

	public const int max_lobby_name_length = 32;

	private static Dictionary<ushort, KrokoshaScavMultiplayer.KrokoshaHandleNamedMessageDelegate> SERVER_MESSAGE_HANDLERS = new Dictionary<ushort, KrokoshaScavMultiplayer.KrokoshaHandleNamedMessageDelegate>();

	private static Dictionary<ushort, KrokoshaScavMultiplayer.KrokoshaHandleNamedMessageDelegate> CLIENT_MESSAGE_HANDLERS = new Dictionary<ushort, KrokoshaScavMultiplayer.KrokoshaHandleNamedMessageDelegate>();

	private static ushort _PLAYER_IDS_COUNTER = 1;

	public static bool running { get; private set; }

	public static bool IsRunningSteam { get; private set; }

	public static bool is_client => type == NetType.Client;

	public static bool is_client_or_host => !is_dedicated_server;

	public static bool is_server => !is_client;

	public static bool is_host => type == NetType.Host;

	public static bool is_dedicated_server => type == NetType.DedicatedServer;

	public static bool is_connecting
	{
		get
		{
			if (TRANSPORT != null)
			{
				return TRANSPORT.IsConnecting();
			}
			return false;
		}
	}

	public static bool is_connected
	{
		get
		{
			if (running)
			{
				return !is_connecting;
			}
			return false;
		}
	}

	public static NetType type { get; internal set; }

	public static NetTransportBase TRANSPORT { get; internal set; }

	public static bool IsMyServerMidjoinLocked()
	{
		if (Util.IsInWorld())
		{
			return !KrokoshaScavMultiplayer.rules.LateJoinAllowed;
		}
		return false;
	}

	public static bool TryGetSteamTransport(out TransportSteamworks tsteam)
	{
		if (TRANSPORT is TransportSteamworks transportSteamworks)
		{
			IsRunningSteam = true;
			tsteam = transportSteamworks;
			return true;
		}
		IsRunningSteam = false;
		tsteam = null;
		return false;
	}

	public static string UshortMsgIdToName(ushort msgid)
	{
		string name = Enum.GetName(typeof(NetmsgId), msgid);
		if (name == null)
		{
			return msgid.ToString();
		}
		return name + $" ({msgid})";
	}

	public static string GetDebugStatsString()
	{
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
		string result = "";
		if (TRANSPORT == null)
		{
			return result;
		}
		if (TryGetSteamTransport(out var tsteam))
		{
			if (is_client)
			{
				SteamNetConnectionRealTimeStatus_t client_status = tsteam.client_status;
				result = $"State: {client_status.m_eState}" + $"\nPing: {client_status.m_nPing}" + $"\nConnectionQuality:  Local: {Mathf.RoundToInt(client_status.m_flConnectionQualityLocal * 100f)}%  Remote: {Mathf.RoundToInt(client_status.m_flConnectionQualityRemote * 100f)}%" + $"\nPacketsPerSec:  Out: {client_status.m_flOutPacketsPerSec}  In: {client_status.m_flInPacketsPerSec}" + $"\nBytesPerSec:  Out: {client_status.m_flOutBytesPerSec}  In: {client_status.m_flInBytesPerSec}" + $"\nSendRateBytesPerSecond: {client_status.m_nSendRateBytesPerSecond}" + $"\nPendingUnreliable: {client_status.m_cbPendingUnreliable}" + $"\nPendingReliable: {client_status.m_cbPendingReliable}" + $"\nSentUnackedReliable: {client_status.m_cbSentUnackedReliable}" + $"\nusecQueueTime: {client_status.m_usecQueueTime}";
			}
			else
			{
				SteamNetConnectionRealTimeStatus_t val = default(SteamNetConnectionRealTimeStatus_t);
				ulong steamID = KSteam.GetLocalUserSteamID().m_SteamID;
				int num = 0;
				foreach (KeyValuePair<ulong, TransportSteamworks.SteamConnectionData> item in tsteam.connectionMapping)
				{
					if (item.Key != steamID)
					{
						num++;
						val.m_flConnectionQualityLocal += item.Value.connectionStatus.m_flConnectionQualityLocal;
						val.m_flOutPacketsPerSec += item.Value.connectionStatus.m_flOutPacketsPerSec;
						val.m_flInPacketsPerSec += item.Value.connectionStatus.m_flInPacketsPerSec;
						val.m_flOutBytesPerSec += item.Value.connectionStatus.m_flOutBytesPerSec;
						val.m_flInBytesPerSec += item.Value.connectionStatus.m_flInBytesPerSec;
						val.m_nSendRateBytesPerSecond += item.Value.connectionStatus.m_nSendRateBytesPerSecond;
						val.m_cbPendingUnreliable += item.Value.connectionStatus.m_cbPendingUnreliable;
						val.m_cbPendingReliable += item.Value.connectionStatus.m_cbPendingReliable;
						val.m_cbSentUnackedReliable += item.Value.connectionStatus.m_cbSentUnackedReliable;
						val.m_usecQueueTime.m_SteamNetworkingMicroseconds += item.Value.connectionStatus.m_usecQueueTime.m_SteamNetworkingMicroseconds;
					}
				}
				if (num == 0)
				{
					result = "NO PLAYERS TO MEASURE CONNECTION STATS";
				}
				else
				{
					val.m_flConnectionQualityLocal /= num;
					val.m_usecQueueTime.m_SteamNetworkingMicroseconds /= num;
					result = $"ConnectionQuality:  {Mathf.RoundToInt(val.m_flConnectionQualityLocal * 100f)}%" + $"\nPacketsPerSec:  Out: {val.m_flOutPacketsPerSec}  In: {val.m_flInPacketsPerSec}" + $"\nBytesPerSec:  Out: {val.m_flOutBytesPerSec}  In: {val.m_flInBytesPerSec}" + $"\nSendRateBytesPerSecond: {val.m_nSendRateBytesPerSecond}" + $"\nPendingUnreliable: {val.m_cbPendingUnreliable}" + $"\nPendingReliable: {val.m_cbPendingReliable}" + $"\nSentUnackedReliable: {val.m_cbSentUnackedReliable}" + $"\nusecQueueTime: {val.m_usecQueueTime}";
				}
			}
		}
		else if (TRANSPORT is TransportLiteNetLib transportLiteNetLib)
		{
			NetStatistics statistics = transportLiteNetLib.netmgr.Statistics;
			result = $"BytesReceived: {statistics.BytesReceived}\nPacketsReceived: {statistics.PacketsReceived}\nBytesSent: {statistics.BytesSent}\nPacketsSent: {statistics.PacketsSent}\nPacketLoss: {statistics.PacketLoss}\nPacketLossPercent: {statistics.PacketLossPercent}";
		}
		return result;
	}

	internal static void RegisterServerReceiver(ushort msgid, KrokoshaScavMultiplayer.KrokoshaHandleNamedMessageDelegate receiver)
	{
		SERVER_MESSAGE_HANDLERS.Add(msgid, receiver);
	}

	internal static void RegisterClientReceiver(ushort msgid, KrokoshaScavMultiplayer.KrokoshaHandleNamedMessageDelegate receiver)
	{
		CLIENT_MESSAGE_HANDLERS.Add(msgid, receiver);
	}

	internal static void InvokeServerMessage(knetid callerclientId, NetDataReader reader)
	{
		ushort num = 0;
		try
		{
			num = reader.GetUShort();
			if (SERVER_MESSAGE_HANDLERS.TryGetValue(num, out var value))
			{
				value(callerclientId, ref reader);
			}
			else if (log.verbose)
			{
				log.warn("InvokeServerMessage: Unknown Message Id: " + UshortMsgIdToName(num));
			}
		}
		catch (Exception ex)
		{
			log.error("SERVER: Net.InvokeServerMessage: " + ServerMain.GetPlayerFullDebugString(callerclientId) + ": msgId:" + UshortMsgIdToName(num) + " :\n" + ex.ToString());
		}
	}

	internal static void InvokeClientMessage(knetid callerclientId, NetDataReader reader)
	{
		ushort num = 0;
		try
		{
			num = reader.GetUShort();
			KrokoshaScavMultiplayer.KrokoshaHandleNamedMessageDelegate value;
			if (is_dedicated_server)
			{
				log.error($"SERVER: Net.InvokeClientMessage: msgId:{num} : UNEXPECTED LOOPBACK ON DEDICATED SERVER !!!!");
			}
			else if (CLIENT_MESSAGE_HANDLERS.TryGetValue(num, out value))
			{
				if ((ushort)callerclientId == 0)
				{
					value(callerclientId, ref reader);
				}
			}
			else if (log.verbose)
			{
				log.warn("InvokeClientMessage: Unknown Message Id: " + UshortMsgIdToName(num));
			}
		}
		catch (Exception ex)
		{
			log.error("CLIENT: Net.InvokeClientMessage: msgId:" + UshortMsgIdToName(num) + " :\n" + ex.ToString());
		}
	}

	internal static bool ValidateNewClientHandshake(NetDataReader reader, bool should_validate_name, bool privileged_user, out string aplayername, out Color24 aplayercolor, out string reject_reason)
	{
		reject_reason = "The server doesn't like you ig.";
		aplayername = "?????????????????????";
		aplayercolor = Color24.black;
		try
		{
			if (SharedMain.local_world_is_generating)
			{
				reject_reason = "Server is generating world, please try again.";
				return false;
			}
			try
			{
				if (!KrokoshaScavMultiplayer.ValidateClientConnectIntroductionPacket(reader, out aplayername, out aplayercolor, out var deny_reason, should_validate_name, privileged_user))
				{
					reject_reason = deny_reason;
					return false;
				}
			}
			catch
			{
				reject_reason = "Corrupt payload.";
				return false;
			}
			if (!privileged_user)
			{
				if (NetPlayer.ClientIdToPlayerDict.Count() + 1 > KrokoshaScavMultiplayer.PLAYER_COUNT_LIMIT)
				{
					reject_reason = $"Max players reached ({KrokoshaScavMultiplayer.PLAYER_COUNT_LIMIT}).";
					return false;
				}
				if (should_validate_name)
				{
					foreach (KeyValuePair<knetid, NetPlayer> item in NetPlayer.ClientIdToPlayerDict)
					{
						if (item.Value.playername == aplayername)
						{
							reject_reason = "Player with this name already exists.";
							return false;
						}
					}
				}
				string reject_reason2 = null;
				if (!KrokoshaScavMultiplayer.AdditionalApprovalCheck(ref reader, aplayername, ref reject_reason2))
				{
					reject_reason = reject_reason2;
					return false;
				}
			}
			return true;
		}
		catch (Exception)
		{
			reject_reason = "You caused an error!! Fuck off!!!";
			return false;
		}
	}

	internal static knetid GetNextPlayerId()
	{
		while (true)
		{
			if (_PLAYER_IDS_COUNTER > 800)
			{
				_PLAYER_IDS_COUNTER = 1;
			}
			if (_PLAYER_IDS_COUNTER != 0 && !NetPlayer.ClientIdToPlayerDict.ContainsKey(_PLAYER_IDS_COUNTER))
			{
				break;
			}
			_PLAYER_IDS_COUNTER++;
		}
		return _PLAYER_IDS_COUNTER;
	}

	internal static NetPlayer CreatePlayer(knetid id, string name, Color24 color)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		GameObject val = new GameObject("Player_" + name)
		{
			hideFlags = (HideFlags)61
		};
		Object.DontDestroyOnLoad((Object)val);
		NetPlayer netPlayer = val.AddComponent<NetPlayer>();
		if ((ushort)id == 0 && is_server)
		{
			netPlayer.is_local = true;
			netPlayer.is_host = true;
		}
		netPlayer.clientId = id;
		netPlayer.playername = name;
		netPlayer.playerColor = color;
		netPlayer.ApplyNameAndColor(name, color);
		return netPlayer;
	}

	public static void ShutdownReset()
	{
		try
		{
			try
			{
				if ((Object)(object)RPCManager.instance != (Object)null && RPCManager.instance.client != null)
				{
					RPCManager.instance.client.ClearPresence();
					RPCManager.instance.timer = 0f;
				}
			}
			catch (Exception ex)
			{
				log.error(ex.ToString());
			}
			if (is_client)
			{
				MP3Menu.dropdownList = null;
			}
			IsRunningSteam = false;
			type = NetType.Host;
			ClientMain.LOCAL_PING = 0;
			ClientMain.SERVER_FPS = 60;
			ClientMain.SERVER_TPS = 60;
			ServerMain.CURRENT_TPS = 60;
			ServerMain.CURRENT_FPS = 60;
			ClientMain.MY_CONNECTION_QUALITY = 100;
			ServerMain.AVG_CONNECTION_QUALITY = 100;
			if (running)
			{
				log.l("NETWORK SHUTDOWNRESET IS SHUTTING IT !!!!!!!!!!!");
			}
			running = false;
			CoolSyncManager.inst.OnTransportEnd();
			SERVER_MESSAGE_HANDLERS.Clear();
			CLIENT_MESSAGE_HANDLERS.Clear();
			try
			{
				HashSet<NetPlayer> hashSet = new HashSet<NetPlayer>(Object.FindObjectsOfType<NetPlayer>(true));
				LinqUtility.AddRange<NetPlayer>((ICollection<NetPlayer>)hashSet, (IEnumerable<NetPlayer>)NetPlayer.ClientIdToPlayerDict.Values);
				foreach (NetPlayer item in hashSet)
				{
					try
					{
						Object.DestroyImmediate((Object)(object)((Component)item).gameObject);
					}
					catch (Exception ex2)
					{
						log.error($"Net.ShutdownReset: Player Deletion: {item}: " + ex2.ToString());
					}
				}
			}
			catch (Exception ex3)
			{
				log.error("Net.ShutdownReset: Player Deletion: " + ex3.ToString());
			}
			if (TRANSPORT != null)
			{
				try
				{
					TRANSPORT.Shutdown();
				}
				catch (Exception ex4)
				{
					log.error($"Net.ShutdownReset: Transport Shutdown: {TRANSPORT.GetType()}: " + ex4.ToString());
				}
				TRANSPORT = null;
			}
			SavesystemPatch.UpdatePersistentDataPathCuzMultiplayer();
			Application.runInBackground = false;
		}
		catch (Exception ex5)
		{
			KrokoshaScavMultiplayer.DoMultiplayerStatusMessageError("Net.ShutdownReset: !!!! CRITICAL FAILURE !!!!: " + ex5.ToString());
			try
			{
				SavesystemPatch.UpdatePersistentDataPathCuzMultiplayer();
				Application.runInBackground = false;
			}
			catch (Exception)
			{
			}
		}
	}

	internal static void TransportCreated(NetType ntype, bool save_name = true)
	{
		Plugin.SkipMainMenuIntro();
		running = true;
		type = ntype;
		Application.runInBackground = true;
		CoolSyncManager.inst.OnTransportStart();
		if (save_name)
		{
			PlayerPrefs.SetString("KrokoshaMultiplayer_LastPlayerName", KrokoshaScavMultiplayer.INPUT_USERNAME);
			PlayerPrefs.SetString("KrokoshaMultiplayer_LastPlayerColor", UIMainMenu.LAST_VALID_INPUT_COLOR.ToHex());
		}
		if (is_server)
		{
			PlayerPrefs.SetString("KrokoshaMultiplayer_LastMyLobbyName", MY_SERVER_INFO.name);
			ServerMain._RegisterServerReceivers();
		}
		if (is_client_or_host)
		{
			ClientMain._RegisterClientReceivers();
		}
		if (is_client)
		{
			UIMainMenu._GUI____RenderRuleField_PresetChosen = 0;
			UIMainMenu._GUI____RenderRuleField_PresetName = "";
			KrokoshaScavMultiplayer.rules = new KrokoshaMultiplayerGameRules();
		}
		SavesystemPatch.UpdatePersistentDataPathCuzMultiplayer();
	}

	internal static void DoUpdate()
	{
		try
		{
			if (TRANSPORT != null)
			{
				TRANSPORT.Update();
			}
		}
		catch (Exception ex)
		{
			log.error("Net.DoUpdate: " + ex.ToString());
		}
	}

	public static void Server_Kick(knetid clientId, string message)
	{
		Con.ConFailIfNetworkIsRunningAndIsClient();
		TRANSPORT.Kick(clientId, in message);
	}

	[Obsolete]
	public static bool TryParseIPPORT_OLD(string IPPORT, out string ip, out ushort port, out bool is_ipv4)
	{
		ip = "127.0.0.1";
		port = 7790;
		is_ipv4 = true;
		IPPORT = IPPORT.Trim();
		string text = IPPORT;
		string text2 = "";
		if (!string.IsNullOrEmpty(IPPORT))
		{
			try
			{
				int num = IPPORT.IndexOf(']');
				int num2 = IPPORT.LastIndexOf(':');
				if (StringUtility.StartsWith(IPPORT, '[') && num != -1)
				{
					text = IPPORT.Substring(1, num - 1);
					if (num2 > num)
					{
						text2 = IPPORT.Substring(num2 + 1);
					}
				}
				else
				{
					int num3 = IPPORT.IndexOf('.');
					int num4 = IPPORT.IndexOf(':');
					if (num4 == -1)
					{
						is_ipv4 = true;
					}
					else if (num3 == -1)
					{
						is_ipv4 = false;
					}
					else if (num3 < num4)
					{
						is_ipv4 = true;
					}
					else
					{
						is_ipv4 = false;
					}
					if (is_ipv4)
					{
						if (num2 != -1)
						{
							text = IPPORT.Substring(0, num2);
							text2 = IPPORT.Substring(num2 + 1);
						}
					}
					else
					{
						text = IPPORT;
					}
				}
			}
			catch (Exception ex)
			{
				log.error("IPv4 or IPv6 parse: " + ex.Message);
				return false;
			}
		}
		text = text.Trim();
		text2 = text2.Trim();
		string text3 = (ip = (is_ipv4 ? "127.0.0.1" : "::1"));
		if (!string.IsNullOrEmpty(text))
		{
			try
			{
				switch (text)
				{
				case "0.0.0.0":
					ip = text;
					break;
				case "::1":
					ip = text;
					is_ipv4 = false;
					break;
				case "localhost":
					ip = text3;
					break;
				default:
				{
					IPAddress iPAddress = Dns.GetHostAddresses(text)[0];
					if (iPAddress.AddressFamily == AddressFamily.InterNetworkV6)
					{
						is_ipv4 = false;
					}
					ip = iPAddress.ToString();
					break;
				}
				}
			}
			catch (Exception ex2)
			{
				log.error(ex2.Message);
				return false;
			}
		}
		port = 7790;
		if (!string.IsNullOrEmpty(text2) && !ushort.TryParse(text2.Trim(), out port))
		{
			return false;
		}
		return true;
	}

	public static bool TryParseIPPORT(string IPPORT, out string ip, out ushort port)
	{
		ip = "127.0.0.1";
		port = 7790;
		bool flag = true;
		IPPORT = IPPORT.Trim();
		string text = IPPORT;
		string text2 = "";
		if (!string.IsNullOrEmpty(IPPORT))
		{
			try
			{
				int num = IPPORT.IndexOf(']');
				int num2 = IPPORT.LastIndexOf(':');
				if (StringUtility.StartsWith(IPPORT, '[') && num != -1)
				{
					text = IPPORT.Substring(1, num - 1);
					if (num2 > num)
					{
						text2 = IPPORT.Substring(num2 + 1);
					}
				}
				else
				{
					int num3 = IPPORT.IndexOf('.');
					int num4 = IPPORT.IndexOf(':');
					if (num4 == -1 || (num3 != -1 && ((num3 < num4) ? true : false)))
					{
						if (num2 != -1)
						{
							text = IPPORT.Substring(0, num2);
							text2 = IPPORT.Substring(num2 + 1);
						}
					}
					else
					{
						text = IPPORT;
					}
				}
			}
			catch (Exception ex)
			{
				log.error("IPv4 or IPv6 parse: " + ex.Message);
				return false;
			}
		}
		text = text.Trim();
		text2 = text2.Trim();
		ip = text;
		port = 7790;
		if (!string.IsNullOrEmpty(text2) && !ushort.TryParse(text2.Trim(), out port))
		{
			return false;
		}
		return true;
	}

	public static bool VerifyMyServerName()
	{
		string name = MY_SERVER_INFO.name;
		if (name.Length < 3)
		{
			return false;
		}
		if (name.Length > 32 || KrokoshaScavMultiplayer.SanitizeTextInputAllowSpaces(name) != name)
		{
			return false;
		}
		return true;
	}

	public static NetDataWriter CreateWriter(in Enum msgid)
	{
		return CreateWriter((ushort)(object)msgid);
	}

	public static NetDataWriter CreateWriter(in Enum msgid, int size, bool autoresize = false)
	{
		return CreateWriter((ushort)(object)msgid, size, autoresize);
	}

	public static NetDataWriter CreateWriter(ushort msgid)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected O, but got Unknown
		NetDataWriter val = new NetDataWriter();
		val.Put(msgid);
		return val;
	}

	public static NetDataWriter CreateWriter(ushort msgid, int size, bool autoresize = false)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Expected O, but got Unknown
		NetDataWriter val = new NetDataWriter(autoresize, size);
		val.Put(msgid);
		return val;
	}

	public static void Server_SendTo(in DeliveryMethod delivery, in NetDataWriter writer, in NetPlayer plr)
	{
		Server_SendTo(in delivery, in writer, plr.clientId);
	}

	public static void Server_SendTo(in DeliveryMethod delivery, in NetDataWriter writer, in knetid clientid)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		if ((ushort)clientid == 0)
		{
			InvokeClientMessage((ushort)0, new NetDataReader(writer));
		}
		else
		{
			TRANSPORT.Server_SendTo(in delivery, in writer, in clientid);
		}
	}

	public static void Server_SendToClientsVeryReliable(in NetDataWriter writer, in IEnumerable<knetid> clientIds)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetSteamTransport(out var tsteam))
		{
			tsteam.Server_SendToClients(8, in writer, (IReadOnlyList<knetid>)clientIds.ToList());
		}
		else
		{
			Server_SendToClients((DeliveryMethod)2, in writer, in clientIds);
		}
	}

	public static void Server_SendToClients(in DeliveryMethod delivery, in NetDataWriter writer, in knetid clientIds)
	{
		Server_SendTo(in delivery, in writer, in clientIds);
	}

	public static void Server_SendToClients(in DeliveryMethod delivery, in NetDataWriter writer, in IReadOnlyList<NetPlayer> clients)
	{
		Server_SendToClients(in delivery, in writer, (IEnumerable<knetid>)clients.Select((NetPlayer x) => x.clientId).ToList());
	}

	public static void Server_SendToClients(in DeliveryMethod delivery, in NetDataWriter writer, in IEnumerable<knetid> clientIds)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		if (clientIds.Count() == 0)
		{
			return;
		}
		List<knetid> list = new List<knetid>(clientIds);
		if (list.Remove((ushort)0))
		{
			InvokeClientMessage((ushort)0, new NetDataReader(writer));
			if (list.Count == 0)
			{
				return;
			}
		}
		NetTransportBase tRANSPORT = TRANSPORT;
		IReadOnlyList<knetid> clientIds2 = list;
		tRANSPORT.Server_SendToClients(in delivery, in writer, in clientIds2);
	}

	public static void Client_Send(in DeliveryMethod delivery, in NetDataWriter writer)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Expected O, but got Unknown
		if (is_server)
		{
			InvokeServerMessage((ushort)0, new NetDataReader(writer));
		}
		else
		{
			TRANSPORT.Client_Send(in delivery, in writer);
		}
	}
}
