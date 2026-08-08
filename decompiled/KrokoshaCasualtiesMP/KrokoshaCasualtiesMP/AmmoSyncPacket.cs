using System;
using System.Runtime.InteropServices;

namespace KrokoshaCasualtiesMP;

[Obsolete]
public struct AmmoSyncPacket : INetSerializeByMemcpy
{
	public byte rounds;

	public static int GetByteSize()
	{
		return Marshal.SizeOf(typeof(AmmoSyncPacket));
	}

	public AmmoSyncPacket(SyncInfo si)
	{
		AmmoScript ammo = si.ammo;
		rounds = (byte)ammo.rounds;
	}

	public void ApplyDirect(SyncInfo si)
	{
		if (si != null && !si.IsIgnored())
		{
			si.ammo.rounds = rounds;
		}
	}
}
