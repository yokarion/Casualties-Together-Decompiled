using LiteNetLib.Utils;

namespace KrokoshaCasualtiesMP;

public class ServerWorldStateSyncer : CoolSyncSubSystemStatic
{
	public new ServerWorldState cur_packet => (ServerWorldState)(object)base.cur_packet;

	public ServerWorldStateSyncer(byte systemid)
		: base(systemid)
	{
		SEND_FREQUENCY = 0.1f;
		base_packet = default(ServerWorldState);
		base.cur_packet = default(ServerWorldState);
	}

	protected override void PackPacket()
	{
		ClientMain.serverWorldState.Server_Serialize();
		base.cur_packet = ClientMain.serverWorldState;
	}

	protected override void Client_ReadData2(NetDataReader reader, ushort data2_len)
	{
		cur_packet.Deserialize();
	}
}
