using System;

namespace KrokoshaCasualtiesMP;

[AttributeUsage(AttributeTargets.Method)]
public class ServerReceiverAttribute : KrokoshaNetworkMessageReceiverAttribute
{
	public ushort message_id { get; private set; }

	public ServerReceiverAttribute(ushort name)
	{
		message_id = name;
	}
}
