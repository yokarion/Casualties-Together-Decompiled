using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;

namespace KrokoshaCasualtiesMP;

public interface NetTransportBase
{
	bool IsConnecting();

	void Shutdown();

	void Update();

	void Kick(knetid clinetId, in string msg);

	void Server_SendTo(in DeliveryMethod delivery, in NetDataWriter writer, in knetid clientid);

	void Server_SendToClients(in DeliveryMethod delivery, in NetDataWriter writer, in IReadOnlyList<knetid> clientIds);

	void Client_Send(in DeliveryMethod delivery, in NetDataWriter writer);
}
