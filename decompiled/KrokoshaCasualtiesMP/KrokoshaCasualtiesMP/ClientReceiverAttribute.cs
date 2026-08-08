using System;

namespace KrokoshaCasualtiesMP;

[AttributeUsage(AttributeTargets.Method)]
public class ClientReceiverAttribute : KrokoshaNetworkMessageReceiverAttribute
{
	public ushort message_id { get; private set; }

	public bool ignore_host { get; private set; }

	public ClientReceiverAttribute(ushort name, bool ignore_host = true)
	{
		message_id = name;
		this.ignore_host = ignore_host;
	}
}
