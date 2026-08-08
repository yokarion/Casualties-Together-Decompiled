using System;

namespace KrokoshaCasualtiesMP;

[AttributeUsage(AttributeTargets.Method)]
public abstract class KrokoshaNetworkMessageReceiverAttribute : Attribute
{
	public enum ReceiverType : byte
	{
		Client,
		Server
	}

	public ushort GetMessageId()
	{
		if (this is ServerReceiverAttribute serverReceiverAttribute)
		{
			return serverReceiverAttribute.message_id;
		}
		if (this is ClientReceiverAttribute clientReceiverAttribute)
		{
			return clientReceiverAttribute.message_id;
		}
		throw new AccessViolationException();
	}

	public string GetMessageName()
	{
		return Net.UshortMsgIdToName(GetMessageId());
	}

	public ReceiverType GetReceiverType()
	{
		if (this is ServerReceiverAttribute)
		{
			return ReceiverType.Server;
		}
		if (this is ClientReceiverAttribute)
		{
			return ReceiverType.Client;
		}
		throw new AccessViolationException();
	}
}
