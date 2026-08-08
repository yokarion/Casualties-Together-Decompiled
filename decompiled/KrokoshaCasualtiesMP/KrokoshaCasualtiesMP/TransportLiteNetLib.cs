using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Layers;
using LiteNetLib.Utils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public class TransportLiteNetLib : NetTransportBase, INetEventListener, IDeliveryEventListener, INtpEventListener, IPeerAddressChangedListener
{
	public readonly string ip = "127.0.0.1";

	public readonly ushort port = 7790;

	public NetManager netmgr;

	private const string NETPEER_PLR_DATA_KEY = "LiteNetLib.NetPeer";

	private Dictionary<NetPeer, NetPlayer> PeerToPlayerDict = new Dictionary<NetPeer, NetPlayer>();

	private TwoWayDictionary<knetid, NetPeer> ClientIdToPeerDict = new TwoWayDictionary<knetid, NetPeer>();

	public NetPeer CLIENT_PEER { get; private set; }

	public TransportLiteNetLib(string IP, ushort PORT)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected O, but got Unknown
		ip = IP;
		port = PORT;
		netmgr = new NetManager((INetEventListener)(object)this, (PacketLayerBase)null)
		{
			PingInterval = 500,
			DisconnectTimeout = 10000,
			EnableStatistics = true
		};
	}

	public NetPeer GetNetPeerFromPlayer(NetPlayer player)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		return (NetPeer)player.CUSTOM_LOCAL_DATA["LiteNetLib.NetPeer"];
	}

	public NetPlayer CreateNetPlayerWithPeer(NetPeer peer, knetid id, string name, Color24 color)
	{
		NetPlayer netPlayer = Net.CreatePlayer(id, name, color);
		ClientIdToPeerDict[id] = peer;
		netPlayer.CUSTOM_LOCAL_DATA["LiteNetLib.NetPeer"] = peer;
		return netPlayer;
	}

	public void Server_SendTo(in DeliveryMethod delivery, in NetDataWriter writer, in knetid clientid)
	{
		if (ClientIdToPeerDict.TryGetByFirst(clientid, out var value))
		{
			value.Send(writer, delivery);
		}
		else if (log.verbose)
		{
			log.error($"SERVER: TransportLiteNetLib.Server_SendTo: UNKNOWN PLAYER: {ServerMain.GetPlayerFullDebugString(clientid)}\n{new StackTrace()}");
		}
	}

	public void Server_SendToClients(in DeliveryMethod delivery, in NetDataWriter writer, in IReadOnlyList<knetid> clientIds)
	{
		if (Net.is_client)
		{
			throw new Exception("CLIENT CANT DO THAT!!!!");
		}
		foreach (knetid clientId in clientIds)
		{
			Server_SendTo(in delivery, in writer, clientId);
		}
	}

	public void Client_Send(in DeliveryMethod delivery, in NetDataWriter writer)
	{
		CLIENT_PEER.Send(writer, delivery);
	}

	private static bool _DoOnWantToConnect(string ipport_input, Net.NetType type)
	{
		try
		{
			if (KrokoshaScavMultiplayer.INPUT_USERNAME.Length <= 2)
			{
				KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog("Name too short..");
				return false;
			}
			if (KrokoshaScavMultiplayer.INPUT_USERNAME != KrokoshaScavMultiplayer.SanitizeTextInput(KrokoshaScavMultiplayer.INPUT_USERNAME))
			{
				KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog("Invalid Name.");
				return false;
			}
			if (!Net.TryParseIPPORT(ipport_input, out var text, out var num))
			{
				KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog("Invalid IP:PORT format.");
				return false;
			}
			if (type != Net.NetType.Client && !Net.VerifyMyServerName())
			{
				KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog("Invalid Server name.");
				return false;
			}
			Net.TransportCreated(type);
			TransportLiteNetLib transportLiteNetLib = (TransportLiteNetLib)(Net.TRANSPORT = new TransportLiteNetLib(text, num));
			switch (type)
			{
			case Net.NetType.Client:
			{
				NetDataWriter val = KrokoshaScavMultiplayer.CreateClientConnectIntroductionPacket();
				if (!transportLiteNetLib.netmgr.Start())
				{
					KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog("LiteNetLib: CLIENT: NetManager.Start() failed.");
					return false;
				}
				transportLiteNetLib.CLIENT_PEER = transportLiteNetLib.netmgr.Connect(text, (int)num, val);
				return true;
			}
			case Net.NetType.Host:
				if (!transportLiteNetLib.netmgr.Start((int)num))
				{
					KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog("LiteNetLib: HOST: NetManager.Start() failed.");
					return false;
				}
				NetPlayer.LOCAL_PLAYER = Net.CreatePlayer((ushort)0, KrokoshaScavMultiplayer.INPUT_USERNAME, UIMainMenu.LAST_VALID_INPUT_COLOR);
				return true;
			case Net.NetType.DedicatedServer:
				if (!transportLiteNetLib.netmgr.Start((int)num))
				{
					KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog("LiteNetLib: SERVER: NetManager.Start() failed.");
					return false;
				}
				return true;
			default:
				throw new NotImplementedException();
			}
		}
		catch (Exception ex)
		{
			KrokoshaScavMultiplayer.DoMultiplayerStatusMessageError("TransportLiteNetLib.OnWantToConnect:\n" + ex.ToString());
		}
		return false;
	}

	public static bool OnWantToConnect(string ipport_input, Net.NetType type)
	{
		Net.ShutdownReset();
		if (_DoOnWantToConnect(ipport_input, type))
		{
			KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog("Succesfully started: " + type);
			return true;
		}
		Net.ShutdownReset();
		return false;
	}

	void INetEventListener.OnConnectionRequest(ConnectionRequest request)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		string aplayername;
		Color24 aplayercolor;
		string reject_reason;
		bool flag = !Net.ValidateNewClientHandshake(request.Data, should_validate_name: true, privileged_user: false, out aplayername, out aplayercolor, out reject_reason);
		if (!flag && BanList.IsBannedIPName(request.RemoteEndPoint.Address.ToString(), aplayername))
		{
			flag = true;
			reject_reason = "Banned";
		}
		if (flag)
		{
			NetDataWriter val = new NetDataWriter();
			val.Put(reject_reason);
			request.Reject(val.Data, 0, val.Length, false);
			return;
		}
		NetPeer val2 = request.Accept();
		NetPlayer plr = CreateNetPlayerWithPeer(val2, Net.GetNextPlayerId(), aplayername, aplayercolor);
		val2.Tag = new KrokoshaPlayerListEntry
		{
			name = aplayername,
			color = aplayercolor,
			plr = plr
		};
	}

	void INetEventListener.OnNetworkError(IPEndPoint endPoint, SocketError socketError)
	{
		log.error($"{GetType().Name}.INetEventListener.OnNetworkError :IPEndPoint: {endPoint}, SocketError: {socketError}");
	}

	void INetEventListener.OnNetworkLatencyUpdate(NetPeer peer, int latency)
	{
		if (peer == CLIENT_PEER)
		{
			ClientMain.LOCAL_PING = latency;
			ClientMain.MY_CONNECTION_QUALITY = (byte)Mathf.Clamp(100 - (int)peer.Statistics.PacketLossPercent, 0, 100);
		}
		if (peer.TryGetPlayer(out var plr))
		{
			plr.ping = (double)latency * 0.001;
		}
	}

	void INetEventListener.OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
	{
		if (Net.is_server)
		{
			Net.InvokeServerMessage(ClientIdToPeerDict[peer], (NetDataReader)(object)reader);
		}
		else
		{
			Net.InvokeClientMessage((ushort)0, (NetDataReader)(object)reader);
		}
	}

	void INetEventListener.OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (Net.is_server)
		{
			_ = 1;
		}
	}

	void INetEventListener.OnPeerConnected(NetPeer peer)
	{
		if (Net.is_client)
		{
			KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog("Connected!  :)");
		}
	}

	void INetEventListener.OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Invalid comparison between Unknown and I4
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Invalid comparison between Unknown and I4
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Invalid comparison between Unknown and I4
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Invalid comparison between Unknown and I4
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Invalid comparison between Unknown and I4
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		_ = Net.type;
		if (peer == CLIENT_PEER)
		{
			KrokoshaScavMultiplayer.ShutdownNetwork();
			if ((int)disconnectInfo.Reason == 8)
			{
				KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog("Unknown Host");
			}
			else if ((int)disconnectInfo.Reason == 1)
			{
				KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog("Timed Out");
			}
			else if ((int)disconnectInfo.Reason == 6)
			{
				string text = default(string);
				((NetDataReader)disconnectInfo.AdditionalData).TryGetString(ref text);
				KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog("Connection Rejected: " + text);
			}
			else if ((int)disconnectInfo.Reason == 5)
			{
				string text2 = default(string);
				((NetDataReader)disconnectInfo.AdditionalData).TryGetString(ref text2);
				KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog("Disconnected: " + text2);
			}
			else if ((int)disconnectInfo.Reason == 4)
			{
				string text3 = default(string);
				((NetDataReader)disconnectInfo.AdditionalData).TryGetString(ref text3);
				KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog("Disconnected: " + text3);
			}
			else
			{
				string arg = default(string);
				((NetDataReader)disconnectInfo.AdditionalData).TryGetString(ref arg);
				KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog($"Disconnected: {disconnectInfo.Reason} : {disconnectInfo.SocketErrorCode} : {arg}");
			}
		}
		if (peer != null && peer.TryGetPlayer(out var plr))
		{
			Object.Destroy((Object)(object)plr);
		}
	}

	public void OnMessageDelivered(NetPeer peer, object userData)
	{
		if (log.verbose)
		{
			log.l($"{GetType().Name}.IDeliveryEventListener.OnMessageDelivered :peer: {peer}, userData: {userData}");
		}
	}

	public void OnNtpResponse(NtpPacket packet)
	{
		if (log.verbose)
		{
			log.l($"{GetType().Name}.INtpEventListener.OnNtpResponse :NtpPacket: {packet}");
		}
	}

	public void OnPeerAddressChanged(NetPeer peer, IPEndPoint previousAddress)
	{
		if (log.verbose)
		{
			log.l($"{GetType().Name}.IPeerAddressChangedListener.OnPeerAddressChanged :peer: {peer}  previousAddress: {previousAddress}");
		}
	}

	bool NetTransportBase.IsConnecting()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Invalid comparison between Unknown and I4
		if (CLIENT_PEER != null)
		{
			return (int)CLIENT_PEER.ConnectionState != 4;
		}
		return false;
	}

	void NetTransportBase.Shutdown()
	{
		netmgr.Stop();
	}

	void NetTransportBase.Update()
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		PeerToPlayerDict.Clear();
		ClientIdToPeerDict.Clear();
		if (Net.is_server)
		{
			foreach (NetPlayer value2 in NetPlayer.ClientIdToPlayerDict.Values)
			{
				try
				{
					if (value2.CUSTOM_LOCAL_DATA.TryGetValue("LiteNetLib.NetPeer", out var value))
					{
						NetPeer val = (NetPeer)value;
						PeerToPlayerDict[val] = value2;
						ClientIdToPeerDict[value2.clientId] = val;
						value2.ping = (float)val.Ping * 0.001f;
					}
					else if ((ushort)value2.clientId != 0)
					{
						log.error(string.Format("{0}: Player NetPeer HAS NO CUSTOM_LOCAL_DATA????????? {1}\n{2}", "TransportLiteNetLib", value2, new StackTrace()));
					}
				}
				catch (Exception ex)
				{
					log.error(string.Format("{0}: Player NetPeer bullshit happened {1}\n{2}", "TransportLiteNetLib", value2, ex.ToString()));
				}
			}
		}
		netmgr.PollEvents(0);
	}

	void NetTransportBase.Kick(knetid clientid, in string msg)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		if (ClientIdToPeerDict.TryGetByFirst(clientid, out var value))
		{
			NetDataWriter val = new NetDataWriter();
			val.Put(msg);
			value.Disconnect(val);
		}
		else
		{
			log.warn($"Tried to kick a non-existant player, clientid:{clientid}  reason:{msg}");
		}
	}

	void NetTransportBase.Server_SendTo(in DeliveryMethod delivery, in NetDataWriter writer, in knetid clientid)
	{
		Server_SendTo(in delivery, in writer, in clientid);
	}

	void NetTransportBase.Server_SendToClients(in DeliveryMethod delivery, in NetDataWriter writer, in IReadOnlyList<knetid> clientIds)
	{
		Server_SendToClients(in delivery, in writer, in clientIds);
	}

	void NetTransportBase.Client_Send(in DeliveryMethod delivery, in NetDataWriter writer)
	{
		Client_Send(in delivery, in writer);
	}
}
