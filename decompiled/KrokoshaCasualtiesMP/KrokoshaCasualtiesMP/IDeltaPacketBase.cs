using System.Collections;
using System.Collections.Generic;
using LiteNetLib.Utils;

namespace KrokoshaCasualtiesMP;

public interface IDeltaPacketBase
{
	void SetDefault();

	void Write(NetDataWriter writer, List<bool> pack_bools, IDeltaPacketBase old);

	void Read(NetDataReader reader, BitArray bitset);
}
