using UnityEngine;

namespace KrokoshaCasualtiesUtils;

public static class Util_LimbExtensions
{
	public static byte GetIndex(this Limb limb)
	{
		for (byte b = 0; b < (byte)limb.body.limbs.Length; b++)
		{
			if ((Object)(object)limb == (Object)(object)limb.body.limbs[b])
			{
				return b;
			}
		}
		return 0;
	}

	public static Vector2 GetPosition(this Limb limb)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return Vector2.op_Implicit(((Component)limb).transform.position);
	}

	public static bool IsBodyLocal(this Limb limb)
	{
		return limb.body.IsBodyLocal();
	}
}
