using UnityEngine;

namespace KrokoshaCasualtiesMP;

public struct SerializableRandomState : INetSerializeByMemcpy
{
	public State State;
}
