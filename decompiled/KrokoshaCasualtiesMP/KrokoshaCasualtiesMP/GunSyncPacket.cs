using System.Runtime.InteropServices;

namespace KrokoshaCasualtiesMP;

public struct GunSyncPacket : INetSerializeByMemcpy
{
	private Bitset8 pb1 = default(Bitset8);

	public byte roundsInMag = 0;

	public bool racked
	{
		get
		{
			return pb1[0];
		}
		set
		{
			pb1[0] = value;
		}
	}

	public bool safe
	{
		get
		{
			return pb1[1];
		}
		set
		{
			pb1[1] = value;
		}
	}

	public bool hasMag
	{
		get
		{
			return pb1[2];
		}
		set
		{
			pb1[2] = value;
		}
	}

	public bool roundInChamber
	{
		get
		{
			return pb1[3];
		}
		set
		{
			pb1[3] = value;
		}
	}

	public bool roundInChamber_is_casing
	{
		get
		{
			return pb1[4];
		}
		set
		{
			pb1[4] = value;
		}
	}

	public static int GetByteSize()
	{
		return Marshal.SizeOf(typeof(GunSyncPacket));
	}

	public GunSyncPacket(SyncInfo isync)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Invalid comparison between Unknown and I4
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Invalid comparison between Unknown and I4
		GunScript gun = isync.gun;
		racked = gun.racked;
		safe = gun.safe;
		hasMag = gun.hasMag;
		roundInChamber = (int)gun.roundInChamber != 2;
		roundInChamber_is_casing = (int)gun.roundInChamber == 1;
		roundsInMag = (byte)gun.roundsInMag;
	}

	public void ApplyDirect(SyncInfo si)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if (si != null && !si.IsIgnored())
		{
			GunScript gun = si.gun;
			gun.racked = racked;
			gun.safe = safe;
			gun.hasMag = hasMag;
			gun.roundInChamber = (RoundInChamber)((!roundInChamber) ? 2 : (roundInChamber_is_casing ? 1 : 0));
			gun.roundsInMag = roundsInMag;
		}
	}
}
