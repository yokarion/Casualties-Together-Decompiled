using System.Runtime.InteropServices;
using KrokoshaCasualtiesUtils;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public struct CharacterLimbHealthState : INetSerializeByMemcpy
{
	private Bitset8 pb1 = default(Bitset8);

	private byte shrapnel = 0;

	private byte compressednumfurBloodAmount = 0;

	private byte compressednum6 = 0;

	private byte compressednum7 = 0;

	public float boneHealTimer = 0f;

	public float dislocationTimer = 0f;

	private byte compressednum9 = 0;

	private byte compressednum4 = 0;

	private byte compressednum5 = 0;

	public ushort disinfectionTime = 0;

	private byte bandageSlowAmount = 0;

	public bool dismembered
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

	public bool infected
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

	public bool has_splint
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

	public bool splint_type
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

	public bool has_tourniquet
	{
		get
		{
			return pb1[5];
		}
		set
		{
			pb1[5] = value;
		}
	}

	public float furBloodAmount
	{
		get
		{
			return (float)(int)compressednumfurBloodAmount / 25.5f;
		}
		set
		{
			compressednumfurBloodAmount = (byte)(Mathf.Clamp(value, 0f, 10f) * 25.5f);
		}
	}

	public float muscleHealth
	{
		get
		{
			return (float)(int)compressednum6 / 2.55f;
		}
		set
		{
			compressednum6 = (byte)(Mathf.Clamp(value, 0f, 100f) * 2.55f);
		}
	}

	public float skinHealth
	{
		get
		{
			return (float)(int)compressednum7 / 2.55f;
		}
		set
		{
			compressednum7 = (byte)(Mathf.Clamp(value, 0f, 100f) * 2.55f);
		}
	}

	public float infectionAmount
	{
		get
		{
			return (float)(int)compressednum9 / 2.55f;
		}
		set
		{
			compressednum9 = (byte)(Mathf.Clamp(value, 0f, 100f) * 2.55f);
		}
	}

	public float bleedAmount
	{
		get
		{
			return (float)(int)compressednum4 / 2.55f;
		}
		set
		{
			compressednum4 = (byte)(Mathf.Clamp(value, 0f, 100f) * 2.55f);
		}
	}

	public float pain
	{
		get
		{
			return (float)(int)compressednum5 / 2.55f;
		}
		set
		{
			compressednum5 = (byte)(Mathf.Clamp(value, 0f, 100f) * 2.55f);
		}
	}

	public CharacterLimbHealthState(Limb limb)
	{
		dismembered = limb.dismembered;
		shrapnel = (byte)limb.shrapnel;
		infected = limb.infected;
		muscleHealth = limb.muscleHealth;
		skinHealth = limb.skinHealth;
		furBloodAmount = limb.furBloodAmount;
		if (!limb.broken)
		{
			boneHealTimer = 0f;
		}
		else
		{
			boneHealTimer = limb.boneHealTimer;
		}
		if (!limb.dislocated)
		{
			dislocationTimer = 0f;
		}
		else
		{
			dislocationTimer = limb.dislocationTimer;
		}
		infectionAmount = limb.infectionAmount;
		bleedAmount = limb.bleedAmount;
		pain = limb.pain;
		disinfectionTime = (ushort)Mathf.Clamp(limb.disinfectionTime, 0f, 65535f);
		if (limb.bandageSlowAmount < 0.5f)
		{
			bandageSlowAmount = 0;
		}
		else
		{
			bandageSlowAmount = (byte)Mathf.Clamp(Mathf.Ceil(limb.bandageSlowAmount), 0f, 255f);
		}
		has_splint = limb.splinted;
		SplintLimb val = default(SplintLimb);
		if (((Component)limb).TryGetComponent<SplintLimb>(ref val))
		{
			splint_type = val.item == "carcasssplint";
		}
		TourniquetScript val2 = default(TourniquetScript);
		if (((Component)limb).TryGetComponent<TourniquetScript>(ref val2))
		{
			has_tourniquet = true;
		}
		else
		{
			has_tourniquet = false;
		}
	}

	public void Apply(Limb limb)
	{
		SplintLimb val2 = default(SplintLimb);
		if (has_splint)
		{
			SplintLimb val = ((Component)limb).GetComponent<SplintLimb>();
			if ((Object)(object)val == (Object)null)
			{
				val = ComponentHolderProtocol.AddComponent<SplintLimb>((Object)(object)limb);
				if (splint_type)
				{
					val.conditionLossMinute = 0.036f;
					val.item = "carcasssplint";
				}
				else
				{
					val.conditionLossMinute = 0.015f;
					val.item = "splint";
				}
			}
			val.condition = 1f;
		}
		else if (((Component)limb).TryGetComponent<SplintLimb>(ref val2))
		{
			Object.Destroy((Object)(object)val2);
		}
		limb.splinted = has_splint;
		TourniquetScript val3 = default(TourniquetScript);
		if (has_tourniquet)
		{
			ComponentHolderProtocol.GetOrAddComponent<TourniquetScript>((Object)(object)limb);
		}
		else if (((Component)limb).TryGetComponent<TourniquetScript>(ref val3))
		{
			val3.affectedLimbs.ForEach(delegate(Limb x)
			{
				x.blockedBleeding = false;
			});
			Object.Destroy((Object)(object)val3);
		}
		if (limb.dismembered != dismembered)
		{
			if (dismembered)
			{
				limb.Dismember();
			}
			else
			{
				limb.body.RegrowAllLimbs();
			}
		}
		if (dislocationTimer > 0f)
		{
			if (!limb.dislocated)
			{
				limb.Dislocate();
			}
		}
		else
		{
			limb.dislocated = false;
		}
		if (boneHealTimer > 0f)
		{
			if (!limb.broken)
			{
				limb.BreakBone();
			}
		}
		else
		{
			limb.MendBone();
		}
		limb.furBloodAmount = furBloodAmount;
		limb.dismembered = dismembered;
		limb.shrapnel = shrapnel;
		limb.infected = infected;
		limb.muscleHealth = muscleHealth;
		limb.skinHealth = skinHealth;
		limb.boneHealTimer = boneHealTimer;
		limb.dislocationTimer = dislocationTimer;
		limb.infectionAmount = infectionAmount;
		limb.bleedAmount = bleedAmount;
		limb.pain = pain;
		limb.disinfectionTime = (int)disinfectionTime;
		limb.bandageSlowAmount = (int)bandageSlowAmount;
	}

	public static int GetByteSize()
	{
		return Marshal.SizeOf(typeof(CharacterLimbHealthState));
	}
}
