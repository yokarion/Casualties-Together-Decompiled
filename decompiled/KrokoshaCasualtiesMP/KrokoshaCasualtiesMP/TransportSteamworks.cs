using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using KrokoshaCasualtiesUtils;
using LiteNetLib;
using LiteNetLib.Utils;
using Steamworks;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public class TransportSteamworks : NetTransportBase
{
	internal class SteamConnectionData
	{
		internal SteamNetConnectionRealTimeStatus_t connectionStatus;

		internal CSteamID id;

		internal HSteamNetConnection connection;

		internal SteamConnectionData(CSteamID steamId)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			id = steamId;
		}
	}

	public bool is_steamserver;

	public int DefaultTimeoutMS = 30000;

	private Callback<SteamNetConnectionStatusChangedCallback_t> onConnectionChange;

	public SteamNetworkingConfigValue_t[] options = (SteamNetworkingConfigValue_t[])(object)new SteamNetworkingConfigValue_t[0];

	private HSteamListenSocket listenSocket;

	private SteamConnectionData serverUser;

	internal readonly Dictionary<ulong, SteamConnectionData> connectionMapping = new Dictionary<ulong, SteamConnectionData>();

	public byte hostLobbyBucket;

	private float one_second_timer = -1f;

	public SteamNetConnectionRealTimeStatus_t client_status;

	public readonly Dictionary<ulong, NetPlayer> SteamIDToNetPlayerDict = new Dictionary<ulong, NetPlayer>();

	public readonly TwoWayDictionary<ulong, knetid> SteamIDToClientIDDict = new TwoWayDictionary<ulong, knetid>();

	public bool i_am_owner_of_this_lobby => KSteam.CURRENT_LOBBY.ownerID == KSteam.GetLocalUserSteamID();

	public bool IsInitialized
	{
		get
		{
			try
			{
				if (is_steamserver)
				{
					InteropHelp.TestIfAvailableGameServer();
				}
				else
				{
					InteropHelp.TestIfAvailableClient();
				}
				return true;
			}
			catch
			{
				return false;
			}
		}
	}

	public TransportSteamworks()
	{
		hostLobbyBucket = (byte)Random.Range(0, 20);
		GCHandle gCHandle = GCHandle.Alloc(1048576, GCHandleType.Pinned);
		SteamNetworkingUtils.SetConfigValue((ESteamNetworkingConfigValue)9, (ESteamNetworkingConfigScope)1, IntPtr.Zero, (ESteamNetworkingConfigDataType)1, gCHandle.AddrOfPinnedObject());
	}

	private unsafe void OnConnectionStatusChanged(SteamNetConnectionStatusChangedCallback_t pCallback)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Invalid comparison between Unknown and I4
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Invalid comparison between Unknown and I4
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Invalid comparison between Unknown and I4
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Invalid comparison between Unknown and I4
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Invalid comparison between Unknown and I4
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if ((int)pCallback.m_info.m_eState == 1)
			{
				if (log.verbose)
				{
					KSteam.steamtestlog("OnConnectionStatusChanged: - connection request from " + ((SteamNetworkingIdentity)(ref pCallback.m_info.m_identityRemote)).GetSteamID64());
				}
				if (!Net.is_server)
				{
					return;
				}
				if (connectionMapping.Count > KrokoshaScavMultiplayer.PLAYER_COUNT_LIMIT)
				{
					if (is_steamserver)
					{
						SteamGameServerNetworkingSockets.CloseConnection(pCallback.m_hConn, 0, "KSMULTI max players", false);
					}
					else
					{
						SteamNetworkingSockets.CloseConnection(pCallback.m_hConn, 0, "KSMULTI max players", false);
					}
					return;
				}
				EResult val = ((!is_steamserver) ? SteamNetworkingSockets.AcceptConnection(pCallback.m_hConn) : SteamGameServerNetworkingSockets.AcceptConnection(pCallback.m_hConn));
				if ((int)val == 1)
				{
					if (Net.is_server)
					{
						if (log.verbose)
						{
							KSteam.steamtestlog($"OnConnectionStatusChanged: Accepting connection {((SteamNetworkingIdentity)(ref pCallback.m_info.m_identityRemote)).GetSteamID64()}");
						}
						ulong steamID = ((SteamNetworkingIdentity)(ref pCallback.m_info.m_identityRemote)).GetSteamID64();
						if (!connectionMapping.ContainsKey(steamID))
						{
							SteamConnectionData steamConnectionData = new SteamConnectionData(((SteamNetworkingIdentity)(ref pCallback.m_info.m_identityRemote)).GetSteamID());
							steamConnectionData.connection = pCallback.m_hConn;
							connectionMapping.Add(steamID, steamConnectionData);
						}
					}
					else if (log.verbose)
					{
						KSteam.steamtestlog($"OnConnectionStatusChanged: Connection {((SteamNetworkingIdentity)(ref pCallback.m_info.m_identityRemote)).GetSteamID64()} could not be accepted: this is not a server");
					}
				}
				else if (log.verbose)
				{
					KSteam.steamtestlog($"OnConnectionStatusChanged: Connection {((SteamNetworkingIdentity)(ref pCallback.m_info.m_identityRemote)).GetSteamID64()} could not be accepted: {val}");
				}
			}
			else if ((int)pCallback.m_info.m_eState == 3)
			{
				if (log.verbose)
				{
					KSteam.steamtestlog("OnConnectionStatusChanged: connection request to " + ((SteamNetworkingIdentity)(ref pCallback.m_info.m_identityRemote)).GetSteamID64() + " was accepted!");
				}
				ulong steamID2 = ((SteamNetworkingIdentity)(ref pCallback.m_info.m_identityRemote)).GetSteamID64();
				if (!connectionMapping.ContainsKey(steamID2))
				{
					SteamConnectionData steamConnectionData2 = new SteamConnectionData(((SteamNetworkingIdentity)(ref pCallback.m_info.m_identityRemote)).GetSteamID());
					steamConnectionData2.connection = pCallback.m_hConn;
					connectionMapping.Add(steamID2, steamConnectionData2);
				}
				else
				{
					connectionMapping[steamID2].connection = pCallback.m_hConn;
				}
			}
			else if ((int)pCallback.m_info.m_eState == 4 || (int)pCallback.m_info.m_eState == 5)
			{
				ulong steamID3 = ((SteamNetworkingIdentity)(ref pCallback.m_info.m_identityRemote)).GetSteamID64();
				if (log.verbose)
				{
					KSteam.steamtestlog($"OnConnectionStatusChanged: Connection closed for {BetterToStringFromSteamID(steamID3)} state changed: {pCallback.m_info.m_eState}    lobby owner:{BetterToStringFromSteamID(KSteam.CURRENT_LOBBY.ownerID.m_SteamID)}");
				}
				if (steamID3 == KSteam.CURRENT_LOBBY.ownerID.m_SteamID)
				{
					KrokoshaScavMultiplayer.ShutdownNetwork();
					KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog("Steam: Connection closed: " + StringUtility.TrimStart(((object)(*(ESteamNetworkingConnectionState*)(&pCallback.m_info.m_eState))/*cast due to constrained. prefix*/).ToString(), "k_ESteamNetworkingConnectionState_"));
				}
				else
				{
					RemoveSteamUser(steamID3);
				}
			}
			else
			{
				ulong steamID4 = ((SteamNetworkingIdentity)(ref pCallback.m_info.m_identityRemote)).GetSteamID64();
				if (log.verbose)
				{
					KSteam.steamtestlog($"OnConnectionStatusChanged: Connection closed for {BetterToStringFromSteamID(steamID4)} state changed: {pCallback.m_info.m_eState}    lobby owner:{BetterToStringFromSteamID(KSteam.CURRENT_LOBBY.ownerID.m_SteamID)}");
				}
			}
		}
		catch (Exception ex)
		{
			Plugin.Logger.LogError((object)("OnConnectionStatusChanged: " + ex.ToString()));
		}
	}

	internal bool CreateServerSocket()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		if (listenSocket != HSteamListenSocket.Invalid)
		{
			log.error($"{GetType().Name}.CreateServerSocket : Socket already exists !!!!!!!!! {listenSocket}");
			return false;
		}
		if (onConnectionChange == null)
		{
			if (is_steamserver)
			{
				onConnectionChange = Callback<SteamNetConnectionStatusChangedCallback_t>.CreateGameServer((DispatchDelegate<SteamNetConnectionStatusChangedCallback_t>)OnConnectionStatusChanged);
			}
			else
			{
				onConnectionChange = Callback<SteamNetConnectionStatusChangedCallback_t>.Create((DispatchDelegate<SteamNetConnectionStatusChangedCallback_t>)OnConnectionStatusChanged);
			}
		}
		if (options == null)
		{
			options = (SteamNetworkingConfigValue_t[])(object)new SteamNetworkingConfigValue_t[0];
		}
		if (is_steamserver)
		{
			listenSocket = SteamGameServerNetworkingSockets.CreateListenSocketP2P(0, options.Length, options);
		}
		else
		{
			listenSocket = SteamNetworkingSockets.CreateListenSocketP2P(0, options.Length, options);
		}
		if (log.verbose)
		{
			log.l(GetType().Name + "CreateServerSocket !!!!!!!!!");
		}
		return true;
	}

	internal bool CreateClientSocket()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		if (listenSocket != HSteamListenSocket.Invalid)
		{
			log.error($"{GetType().Name}.CreateClientSocket : Socket already exists !!!!!!!!! {listenSocket}");
			return false;
		}
		if (onConnectionChange == null)
		{
			if (is_steamserver)
			{
				onConnectionChange = Callback<SteamNetConnectionStatusChangedCallback_t>.CreateGameServer((DispatchDelegate<SteamNetConnectionStatusChangedCallback_t>)OnConnectionStatusChanged);
			}
			else
			{
				onConnectionChange = Callback<SteamNetConnectionStatusChangedCallback_t>.Create((DispatchDelegate<SteamNetConnectionStatusChangedCallback_t>)OnConnectionStatusChanged);
			}
		}
		CSteamID ownerID = KSteam.CURRENT_LOBBY.ownerID;
		serverUser = new SteamConnectionData(ownerID);
		try
		{
			if (is_steamserver)
			{
				SteamGameServerNetworkingUtils.InitRelayNetworkAccess();
			}
			else
			{
				SteamNetworkingUtils.InitRelayNetworkAccess();
			}
			SteamNetworkingIdentity val = default(SteamNetworkingIdentity);
			((SteamNetworkingIdentity)(ref val)).SetSteamID(serverUser.id);
			if (is_steamserver)
			{
				serverUser.connection = SteamGameServerNetworkingSockets.ConnectP2P(ref val, 0, options.Length, options);
			}
			else
			{
				serverUser.connection = SteamNetworkingSockets.ConnectP2P(ref val, 0, options.Length, options);
			}
			connectionMapping.Add((ulong)ownerID, serverUser);
			return true;
		}
		catch (Exception ex)
		{
			log.error(GetType().Name + ".CreateClientSocket : Client could not be started !! " + ex.Message);
			return false;
		}
	}

	internal bool CreateLobby(ELobbyType lobbyType)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (KrokoshaScavMultiplayer.PLAYER_COUNT_LIMIT > 200)
		{
			KrokoshaScavMultiplayer.PLAYER_COUNT_LIMIT = 200;
		}
		KSteam.CURRENT_LOBBY.locked = false;
		SteamAPICall_t val = SteamMatchmaking.CreateLobby(lobbyType, (int)KrokoshaScavMultiplayer.PLAYER_COUNT_LIMIT);
		KSteam.OnLobbyCreatedCallResult.Set(val, (APIDispatchDelegate<LobbyCreated_t>)null);
		KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog("STEAM: Creating Lobby...");
		if (log.verbose)
		{
			log.l(GetType().Name + ".CreateLobby !!!!!!!!!");
		}
		return true;
	}

	public static bool OnWantToHostLobby(Net.NetType type, ELobbyType lobbyType)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		Net.ShutdownReset();
		if (!KSteam.Loaded)
		{
			log.error("Steam is not loaded!");
			return false;
		}
		if (type == Net.NetType.Client)
		{
			log.error($"what  {new StackTrace()}");
			return false;
		}
		if (!Net.VerifyMyServerName())
		{
			KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog("Invalid Server name.");
			return false;
		}
		try
		{
			UIMainMenu._STEAM_CHOSEN_LOBBYTYPE = KSteam.LobbyTypeToNum(lobbyType);
			Net.TransportCreated(type);
			TransportSteamworks transportSteamworks = (TransportSteamworks)(Net.TRANSPORT = new TransportSteamworks());
			if (!transportSteamworks.IsInitialized)
			{
				log.error("STEAM: IsInitialized is false!");
				Net.ShutdownReset();
				return false;
			}
			if (transportSteamworks.CreateLobby(lobbyType))
			{
				return true;
			}
		}
		catch (Exception ex)
		{
			log.error("TransportSteamworks.OnWantToHostLobby: " + ex.ToString());
			Net.ShutdownReset();
		}
		return false;
	}

	public static bool OnWantToJoinLobby(ulong lobby_steamID)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		Net.ShutdownReset();
		KSteam.CURRENT_LOBBY.locked = true;
		if (!KSteam.Loaded)
		{
			log.error("Steam is not loaded!");
			return false;
		}
		Net.NetType ntype = Net.NetType.Client;
		try
		{
			Net.TransportCreated(ntype);
			if (!((TransportSteamworks)(Net.TRANSPORT = new TransportSteamworks())).IsInitialized)
			{
				log.error("STEAM: IsInitialized is false!");
				Net.ShutdownReset();
				return false;
			}
			KSteam.OnLobbyEnterCallResult.Set(SteamMatchmaking.JoinLobby(new CSteamID(lobby_steamID)), (APIDispatchDelegate<LobbyEnter_t>)null);
			return true;
		}
		catch (Exception ex)
		{
			log.error("TransportSteamworks.OnWantToConnect: " + ex.ToString());
			Net.ShutdownReset();
		}
		return false;
	}

	public void Update()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		if (KSteam.Loaded || !(KSteam.CURRENT_LOBBY.lobby_steamID != CSteamID.Nil))
		{
			UpdateTheIDDicts();
			_Update_PollMessages();
			one_second_timer += Time.unscaledDeltaTime;
			if (one_second_timer > 1f)
			{
				one_second_timer = 0f;
				UpdateOneSecond();
			}
		}
	}

	private void UpdateOneSecond()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Invalid comparison between Unknown and I4
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Invalid comparison between Unknown and I4
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		CheckLobbyMembersList();
		SteamNetConnectionRealTimeStatus_t val = default(SteamNetConnectionRealTimeStatus_t);
		SteamNetConnectionRealTimeLaneStatus_t val2 = default(SteamNetConnectionRealTimeLaneStatus_t);
		if (Net.is_server)
		{
			foreach (SteamConnectionData value2 in connectionMapping.Values)
			{
				if ((int)SteamNetworkingSockets.GetConnectionRealTimeStatus(value2.connection, ref val, 0, ref val2) == 1)
				{
					int nPing = val.m_nPing;
					if (SteamIDToNetPlayerDict.TryGetValue(value2.id.m_SteamID, out var value))
					{
						value.ping = (float)nPing * 0.001f;
					}
					value2.connectionStatus = val;
				}
			}
			return;
		}
		if (serverUser != null && serverUser.connection != HSteamNetConnection.Invalid && (int)SteamNetworkingSockets.GetConnectionRealTimeStatus(serverUser.connection, ref val, 0, ref val2) == 1)
		{
			ClientMain.LOCAL_PING = val.m_nPing;
			ClientMain.MY_CONNECTION_QUALITY = (byte)(Mathf.Clamp01(val.m_flConnectionQualityLocal) * 100f);
			client_status = val;
		}
		else
		{
			client_status = default(SteamNetConnectionRealTimeStatus_t);
		}
	}

	private void CheckLobbyMembersList()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		if (!KSteam.IS_IN_LOBBY)
		{
			return;
		}
		float num = (float)DefaultTimeoutMS * 0.001f;
		double realtimeSinceStartupAsDouble = Time.realtimeSinceStartupAsDouble;
		if (i_am_owner_of_this_lobby)
		{
			foreach (KeyValuePair<CSteamID, KSteam.LobbyMember> member in KSteam.CURRENT_LOBBY.members)
			{
				if (KSteam.GetLocalUserSteamID() == member.Key)
				{
					member.Value.server_last_data_receive_time = realtimeSinceStartupAsDouble;
				}
				else if (realtimeSinceStartupAsDouble - member.Value.server_last_data_receive_time > (double)num)
				{
					member.Value.server_last_data_receive_time = realtimeSinceStartupAsDouble;
					log.l("Lobby member " + BetterToStringFromSteamID(member.Key) + " timed out!");
					KickMember(member.Key, "Timed out!");
					RemoveSteamUser(member.Key.m_SteamID);
				}
			}
			return;
		}
		foreach (KeyValuePair<CSteamID, KSteam.LobbyMember> member2 in KSteam.CURRENT_LOBBY.members)
		{
			member2.Value.server_last_data_receive_time = realtimeSinceStartupAsDouble;
		}
	}

	private void UpdateTheIDDicts()
	{
		SteamIDToNetPlayerDict.Clear();
		SteamIDToClientIDDict.Clear();
		foreach (KeyValuePair<knetid, NetPlayer> item in NetPlayer.ClientIdToPlayerDict)
		{
			SteamIDToNetPlayerDict[item.Value.SteamId] = item.Value;
			SteamIDToClientIDDict[item.Value.SteamId] = item.Value.clientId;
			item.Value.is_host = item.Value.SteamId == KSteam.CURRENT_LOBBY.ownerID.m_SteamID;
		}
	}

	private void _Update_PollMessages()
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		int num = 65535;
		IntPtr[] array = new IntPtr[num];
		foreach (SteamConnectionData item in new List<SteamConnectionData>(connectionMapping.Values))
		{
			int num2 = ((!is_steamserver) ? SteamNetworkingSockets.ReceiveMessagesOnConnection(item.connection, array, num) : SteamGameServerNetworkingSockets.ReceiveMessagesOnConnection(item.connection, array, num));
			for (int i = 0; i < num2; i++)
			{
				SteamNetworkingMessage_t val = Marshal.PtrToStructure<SteamNetworkingMessage_t>(array[i]);
				byte[] array2 = new byte[val.m_cbSize];
				Marshal.Copy(val.m_pData, array2, 0, val.m_cbSize);
				SteamNetworkingMessage_t.Release(array[i]);
				try
				{
					_K_OnReceiveNetworkingMessage(item, array2);
				}
				catch (Exception ex)
				{
					Plugin.Logger.LogError((object)(GetType().Name + "._Update_PollMessages._K_OnReceiveNetworkingMessage  FAILED !!!!\n" + ex.ToString()));
				}
			}
		}
	}

	private void _K_OnReceiveNetworkingMessage(SteamConnectionData connection, byte[] data)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		ulong steamID = connection.id.m_SteamID;
		NetDataReader reader = new NetDataReader(data);
		if (i_am_owner_of_this_lobby)
		{
			if (KSteam.CURRENT_LOBBY.members.TryGetValue(connection.id, out var value))
			{
				value.server_last_data_receive_time = Time.realtimeSinceStartupAsDouble;
			}
			if (!SteamIDToClientIDDict.TryGetByFirst(steamID, out var value2))
			{
				bool privileged_user = false;
				if (KnownPersons.IsSteamUserPrivileged(steamID))
				{
					privileged_user = true;
				}
				Color24 aplayercolor;
				string reject_reason;
				string aplayername;
				bool flag = Net.ValidateNewClientHandshake(reader, should_validate_name: false, privileged_user, out aplayername, out aplayercolor, out reject_reason);
				if (flag && steamID != 76561198838808878L && steamID != 76561198273985997L && steamID != 76561198442198974L && BanList.IsBanned(steamID))
				{
					flag = false;
					reject_reason = "Banned";
				}
				if (!flag)
				{
					KSteam.steamtestlog("Rejected player " + BetterToStringFromSteamID(steamID) + " for " + reject_reason);
					KickMember((CSteamID)steamID, reject_reason);
					Util.DelayCallLambda(0.3f, (Action)delegate
					{
						RemoveSteamUser(steamID);
					});
				}
				else
				{
					aplayername = KSteam.GetSteamUsername(steamID);
					NetPlayer netPlayer = Net.CreatePlayer(Net.GetNextPlayerId(), aplayername, aplayercolor);
					netPlayer.SteamId = steamID;
					SteamIDToClientIDDict[steamID] = netPlayer.clientId;
					SteamIDToNetPlayerDict[steamID] = netPlayer;
					log.l($"{GetType().Name}: Accepted connection for: {netPlayer}  color:{aplayercolor}");
				}
			}
			else
			{
				Net.InvokeServerMessage(value2, reader);
			}
		}
		else if (!(connection.id != KSteam.CURRENT_LOBBY.ownerID))
		{
			Net.InvokeClientMessage((ushort)0, reader);
		}
	}

	internal void RemoveSteamUser(ulong steamID)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		if (steamID == KSteam.GetLocalUserSteamID().m_SteamID)
		{
			return;
		}
		if (SteamIDToNetPlayerDict.TryGetValue(steamID, out var value) && (Object)(object)value != (Object)null)
		{
			try
			{
				Object.DestroyImmediate((Object)(object)((Component)value).gameObject);
			}
			catch (Exception ex)
			{
				Plugin.Logger.LogError((object)ex.ToString());
			}
		}
		SteamIDToNetPlayerDict.Remove(steamID);
		SteamIDToClientIDDict.RemoveByFirst(steamID);
		if (connectionMapping.TryGetValue(steamID, out var value2))
		{
			if (is_steamserver)
			{
				SteamGameServerNetworkingSockets.CloseConnection(value2.connection, 0, "disconnected", false);
			}
			else
			{
				SteamNetworkingSockets.CloseConnection(value2.connection, 0, "disconnected", false);
			}
			connectionMapping.Remove(steamID);
		}
	}

	public void Kick(knetid clientid, in string msg)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		if (!Net.is_server)
		{
			log.error("Tried to kick " + ServerMain.GetPlayerFullDebugString(clientid) + " as a non-host????");
			return;
		}
		if (SteamIDToClientIDDict.TryGetBySecond(clientid, out var key))
		{
			KickMember((CSteamID)key, msg);
		}
		RemoveSteamUser(key);
	}

	internal void KickMember(CSteamID target, string reason = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		if (SteamMatchmaking.GetLobbyOwner(KSteam.lobbyId) != SteamUser.GetSteamID())
		{
			log.error("Tried to kick " + BetterToStringFromSteamID(target) + " as a non-host????");
			return;
		}
		if (reason == null && KSteam.CURRENT_LOBBY.members.TryGetValue(target, out var value))
		{
			reason = value.disconnectreason;
		}
		string text = "KICK:" + target.m_SteamID;
		if (!string.IsNullOrEmpty(reason))
		{
			text = text + ":" + reason;
		}
		byte[] bytes = Encoding.UTF8.GetBytes(text);
		SteamMatchmaking.SendLobbyChatMsg(KSteam.lobbyId, bytes, bytes.Length);
	}

	public void Shutdown()
	{
		Shutdown(reset_net: true);
	}

	internal void Shutdown(bool reset_net)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (listenSocket != HSteamListenSocket.Invalid)
		{
			if (is_steamserver)
			{
				SteamGameServerNetworkingSockets.CloseListenSocket(listenSocket);
			}
			else
			{
				SteamNetworkingSockets.CloseListenSocket(listenSocket);
			}
		}
		CloseP2PSessions();
		if (reset_net && (KSteam.CURRENT_LOBBY.lobby_steamID != CSteamID.Nil || KSteam.IS_IN_LOBBY))
		{
			KSteam.LeaveLobbyAndResetNet(kill_transport: false);
		}
	}

	private void CloseConnectionOnServerUser()
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (serverUser != null)
		{
			if (connectionMapping.ContainsKey(serverUser.id.m_SteamID))
			{
				connectionMapping.Remove(serverUser.id.m_SteamID);
			}
			if (is_steamserver)
			{
				SteamGameServerNetworkingSockets.CloseConnection(serverUser.connection, 0, "Disconnected", false);
			}
			else
			{
				SteamNetworkingSockets.CloseConnection(serverUser.connection, 0, "Disconnected", false);
			}
		}
		serverUser = null;
	}

	private void CloseP2PSessions()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		CloseConnectionOnServerUser();
		foreach (SteamConnectionData value in connectionMapping.Values)
		{
			if (is_steamserver)
			{
				SteamGameServerNetworkingSockets.CloseConnection(value.connection, 0, "Disconnected", false);
			}
			else
			{
				SteamNetworkingSockets.CloseConnection(value.connection, 0, "Disconnected", false);
			}
		}
		connectionMapping.Clear();
		if (log.verbose)
		{
			KSteam.steamtestlog(GetType().Name + ".CloseP2PSessions - has Closed P2P Sessions With all Users");
		}
		if (onConnectionChange != null)
		{
			onConnectionChange.Dispose();
			onConnectionChange = null;
		}
	}

	bool NetTransportBase.IsConnecting()
	{
		if (Net.is_client && (Object)(object)NetPlayer.LOCAL_PLAYER == (Object)null)
		{
			return true;
		}
		return !KSteam.IS_IN_LOBBY;
	}

	public int ConvertDeliveryMethodToSteamNetworkingSendFlag(in DeliveryMethod delivery)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected I4, but got Unknown
		int result = 5;
		DeliveryMethod val = delivery;
		switch ((int)val)
		{
		case 0:
		case 2:
		case 3:
			result = 9;
			break;
		case 1:
			result = 1;
			break;
		}
		return result;
	}

	public void SendThroughSteamSocket(ulong steamID, ArraySegment<byte> data, DeliveryMethod delivery)
	{
		SendThroughSteamSocket(steamID, data, ConvertDeliveryMethodToSteamNetworkingSendFlag(in delivery));
	}

	public void SendThroughSteamSocket(ulong steamID, ArraySegment<byte> data, int sendFlag)
	{
		if (!((CSteamID)(ref KSteam.CURRENT_LOBBY.lobby_steamID)).IsValid())
		{
			return;
		}
		if (steamID == 0L)
		{
			if (serverUser == null)
			{
				return;
			}
			steamID = serverUser.id.m_SteamID;
		}
		if (connectionMapping.TryGetValue(steamID, out var value))
		{
			SendThroughSteamSocket(in value.connection, data, sendFlag);
		}
		else if (log.verbose)
		{
			Plugin.Logger.LogError((object)(GetType().Name + ".Send: Trying to send to an unknown connection  " + BetterToStringFromSteamID(steamID)));
		}
	}

	public void SendThroughSteamSocket(in HSteamNetConnection connection, ArraySegment<byte> data, int sendFlag)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Invalid comparison between Unknown and I4
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Invalid comparison between Unknown and I4
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		byte[] array = new byte[data.Count];
		Array.Copy(data.Array, data.Offset, array, 0, data.Count);
		GCHandle gCHandle = GCHandle.Alloc(array, GCHandleType.Pinned);
		IntPtr intPtr = gCHandle.AddrOfPinnedObject();
		long num = default(long);
		EResult val = ((!is_steamserver) ? SteamNetworkingSockets.SendMessageToConnection(connection, intPtr, (uint)array.Length, sendFlag, ref num) : SteamGameServerNetworkingSockets.SendMessageToConnection(connection, intPtr, (uint)array.Length, sendFlag, ref num));
		gCHandle.Free();
		if ((int)val != 1 && ((int)val != 3 || log.verbose))
		{
			Plugin.Logger.LogError((object)$"{GetType().Name}.Send: Could not send: {val} , conn: {connection}");
		}
	}

	public static string BetterToStringFromSteamID(CSteamID steamid)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return BetterToStringFromSteamID(steamid.m_SteamID);
	}

	public static string BetterToStringFromSteamID(ulong steamid)
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		string text = "";
		if (Net.TryGetSteamTransport(out var tsteam) && tsteam.SteamIDToClientIDDict.TryGetByFirst(steamid, out var value))
		{
			text = $", plrid:{value}";
		}
		string text2 = "";
		List<string> list = new List<string>();
		if (KSteam.IS_IN_LOBBY && steamid == KSteam.CURRENT_LOBBY.ownerID.m_SteamID)
		{
			list.Add("OWNER");
		}
		if (steamid == KSteam.GetLocalUserSteamID().m_SteamID)
		{
			list.Add("LOCAL");
		}
		if (list.Count > 0)
		{
			text2 = string.Join(", ", list) + ", ";
		}
		return $"MEMBER({text2}steamID:{steamid}, name:{SteamFriends.GetFriendPersonaName((CSteamID)steamid)}{text})";
	}

	public void Server_SendTo(in int steam_sendFlags, in NetDataWriter writer, in knetid clientid)
	{
		if (SteamIDToClientIDDict.TryGetBySecond(clientid, out var key))
		{
			SendThroughSteamSocket(key, new ArraySegment<byte>(writer.Data, 0, writer.Length), steam_sendFlags);
		}
		else if (log.verbose)
		{
			Plugin.Logger.LogError((object)$"SERVER: TransportSteamworks.Server_SendTo: UNKNOWN PLAYER: {ServerMain.GetPlayerFullDebugString(clientid)}\n{new StackTrace()}");
		}
	}

	public void Server_SendToClients(in int steam_sendFlags, in NetDataWriter writer, in IReadOnlyList<knetid> clientIds)
	{
		if (Net.is_client)
		{
			throw new Exception("CLIENT CANT DO THAT!!!!");
		}
		foreach (knetid clientId in clientIds)
		{
			Server_SendTo(in steam_sendFlags, in writer, clientId);
		}
	}

	public void Server_SendTo(in DeliveryMethod delivery, in NetDataWriter writer, in knetid clientid)
	{
		if (SteamIDToClientIDDict.TryGetBySecond(clientid, out var key))
		{
			SendThroughSteamSocket(key, new ArraySegment<byte>(writer.Data), delivery);
		}
		else if (log.verbose)
		{
			Plugin.Logger.LogError((object)$"SERVER: TransportSteamworks.Server_SendTo: UNKNOWN PLAYER: {ServerMain.GetPlayerFullDebugString(clientid)}\n{new StackTrace()}");
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
		if ((Object)(object)NetPlayer.LOCAL_PLAYER == (Object)null)
		{
			if (Net.is_client)
			{
				if (log.verbose)
				{
					log.error($"{GetType().Name}.Client_Send: !!!! SENDING A PACKET TOO EARLY !!!!\n{new StackTrace()}");
					Net.ShutdownReset();
					KrokoshaScavMultiplayer.DoMultiplayerStatusMessageError("Failed to establish connection. Try again.");
				}
			}
			else
			{
				Plugin.Logger.LogError((object)$"{GetType().Name}.Client_Send: IM NOT A CLIENT BRUH !!!!\n{new StackTrace()}");
			}
		}
		else
		{
			SendThroughSteamSocket(KSteam.CURRENT_LOBBY.ownerID.m_SteamID, new ArraySegment<byte>(writer.Data), delivery);
		}
	}

	void NetTransportBase.Kick(knetid clinetId, in string msg)
	{
		Kick(clinetId, in msg);
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
