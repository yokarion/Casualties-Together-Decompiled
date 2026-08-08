using System.Runtime.InteropServices;
using KrokoshaCasualtiesUtils;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public struct CharacterHealthStateSyncPacket : INetSerializeByMemcpy
{
	private Bitset8 pb1;

	private CharacterSkillsStateSyncPacket skills;

	private byte compressedclawHealth;

	private byte compressednum3;

	public bool usedNeuralBooster;

	public float maxSpeed;

	public float moveForce;

	public float jumpSpeed;

	public float caffeinated;

	public float bloodVolume;

	public float bloodOxygen;

	public float bloodPressure;

	public float respiratoryRate;

	public float heartRate;

	public float bloodVesselSize;

	public float bloodViscosity;

	public float strokeAmount;

	public bool hasPulmonaryEmbolism;

	public float fibrillationProgress;

	public float venomCurrent;

	public float venomTotal;

	public float hunger;

	public float thirst;

	public float septicShock;

	public float temperature;

	public float sicknessAmount;

	public float consciousness;

	public float stamina;

	private byte compressednum4;

	public float happiness;

	public float weightOffset;

	public float radiationSickness;

	public float internalBleeding;

	public float hemothorax;

	public float traumaAmount;

	private byte compressednum2;

	private byte compressednum1;

	public float badSleepAmount;

	public float hearingLoss;

	public float antidepressantHappiness;

	public float opiateHappiness;

	public float antibioticImmunityTime;

	public float brainGrowSickness;

	public float lastStandTime;

	public float adrenaline;

	private byte compressednumb;

	public float stimulantMultiplier;

	private CharacterLimbHealthState limb0;

	private CharacterLimbHealthState limb1;

	private CharacterLimbHealthState limb2;

	private CharacterLimbHealthState limb3;

	private CharacterLimbHealthState limb4;

	private CharacterLimbHealthState limb5;

	private CharacterLimbHealthState limb6;

	private CharacterLimbHealthState limb7;

	private CharacterLimbHealthState limb8;

	private CharacterLimbHealthState limb9;

	private CharacterLimbHealthState limb10;

	private CharacterLimbHealthState limb11;

	private CharacterLimbHealthState limb12;

	private CharacterLimbHealthState limb13;

	private CharacterLimbHealthState limb14;

	public bool bothEyesGone
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

	public bool disfigured
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

	public bool eyeGone
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

	public bool triedRollingLastStand
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

	public bool succesfullyRolledLastStand
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

	public bool mindwiped
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

	public bool painkilled
	{
		get
		{
			return pb1[6];
		}
		set
		{
			pb1[6] = value;
		}
	}

	public bool sleeping
	{
		get
		{
			return pb1[7];
		}
		set
		{
			pb1[7] = value;
		}
	}

	public float clawHealth
	{
		get
		{
			return (float)(int)compressedclawHealth / 2.55f;
		}
		set
		{
			compressedclawHealth = (byte)(Mathf.Clamp(value, 0f, 100f) * 2.55f);
		}
	}

	public float brainHealth
	{
		get
		{
			return (float)(int)compressednum3 / 2.55f;
		}
		set
		{
			compressednum3 = (byte)(Mathf.Clamp(value, 0f, 100f) * 2.55f);
		}
	}

	public float energy
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

	public float dirtyness
	{
		get
		{
			return (float)(int)compressednum2 / 2.55f;
		}
		set
		{
			compressednum2 = (byte)(Mathf.Clamp(value, 0f, 100f) * 2.55f);
		}
	}

	public float wetness
	{
		get
		{
			return (float)(int)compressednum1 / 2.55f;
		}
		set
		{
			compressednum1 = (byte)(Mathf.Clamp(value, 0f, 100f) * 2.55f);
		}
	}

	public float burpTimer
	{
		get
		{
			return (float)(int)compressednumb / 2.55f / 0.9f - 100f;
		}
		set
		{
			compressednumb = (byte)(Mathf.Clamp((value + 100f) * 0.9f, 0f, 100f) * 2.55f);
		}
	}

	private void SetDefaults()
	{
		usedNeuralBooster = false;
		maxSpeed = 25f;
		moveForce = 4000f;
		jumpSpeed = 26f;
		caffeinated = 0f;
		bloodVolume = 100f;
		bloodOxygen = 100f;
		bloodPressure = 120f;
		respiratoryRate = 100f;
		heartRate = 70f;
		bloodVesselSize = 1f;
		bloodViscosity = 0f;
		strokeAmount = 0f;
		hasPulmonaryEmbolism = false;
		fibrillationProgress = 0f;
		venomCurrent = 0f;
		venomTotal = 0f;
		hunger = 109.5938f;
		thirst = 92.74957f;
		septicShock = 0f;
		temperature = 37f;
		sicknessAmount = 0f;
		consciousness = 100f;
		stamina = 100f;
		happiness = 0f;
		weightOffset = 0f;
		radiationSickness = 0f;
		internalBleeding = 0f;
		hemothorax = 0f;
		traumaAmount = 0f;
		badSleepAmount = 0f;
		hearingLoss = 0f;
		antidepressantHappiness = 0f;
		opiateHappiness = 0f;
		antibioticImmunityTime = 0f;
		brainGrowSickness = 0f;
		lastStandTime = -10000f;
		adrenaline = 0f;
		stimulantMultiplier = 0f;
	}

	public CharacterHealthStateSyncPacket(Body body)
	{
		pb1 = default(Bitset8);
		compressedclawHealth = 0;
		compressednum3 = 0;
		usedNeuralBooster = false;
		maxSpeed = 0f;
		moveForce = 0f;
		jumpSpeed = 0f;
		caffeinated = 0f;
		bloodVolume = 0f;
		bloodOxygen = 0f;
		bloodPressure = 0f;
		respiratoryRate = 0f;
		heartRate = 0f;
		bloodVesselSize = 0f;
		bloodViscosity = 0f;
		strokeAmount = 0f;
		hasPulmonaryEmbolism = false;
		fibrillationProgress = 0f;
		venomCurrent = 0f;
		venomTotal = 0f;
		hunger = 0f;
		thirst = 0f;
		septicShock = 0f;
		temperature = 0f;
		sicknessAmount = 0f;
		consciousness = 0f;
		stamina = 0f;
		compressednum4 = 0;
		happiness = 0f;
		weightOffset = 0f;
		radiationSickness = 0f;
		internalBleeding = 0f;
		hemothorax = 0f;
		traumaAmount = 0f;
		compressednum2 = 0;
		compressednum1 = 0;
		badSleepAmount = 0f;
		hearingLoss = 0f;
		antidepressantHappiness = 0f;
		opiateHappiness = 0f;
		antibioticImmunityTime = 0f;
		brainGrowSickness = 0f;
		lastStandTime = 0f;
		adrenaline = 0f;
		compressednumb = 0;
		stimulantMultiplier = 0f;
		limb0 = default(CharacterLimbHealthState);
		limb1 = default(CharacterLimbHealthState);
		limb2 = default(CharacterLimbHealthState);
		limb3 = default(CharacterLimbHealthState);
		limb4 = default(CharacterLimbHealthState);
		limb5 = default(CharacterLimbHealthState);
		limb6 = default(CharacterLimbHealthState);
		limb7 = default(CharacterLimbHealthState);
		limb8 = default(CharacterLimbHealthState);
		limb9 = default(CharacterLimbHealthState);
		limb10 = default(CharacterLimbHealthState);
		limb11 = default(CharacterLimbHealthState);
		limb12 = default(CharacterLimbHealthState);
		limb13 = default(CharacterLimbHealthState);
		limb14 = default(CharacterLimbHealthState);
		skills = new CharacterSkillsStateSyncPacket(body);
		burpTimer = body.burpTimer;
		bothEyesGone = body.bothEyesGone;
		disfigured = body.disfigured;
		eyeGone = body.eyeGone;
		triedRollingLastStand = body.triedRollingLastStand;
		succesfullyRolledLastStand = body.succesfullyRolledLastStand;
		clawHealth = body.clawHealth;
		brainHealth = body.brainHealth;
		usedNeuralBooster = body.usedNeuralBooster;
		maxSpeed = body.maxSpeed;
		moveForce = body.moveForce;
		jumpSpeed = body.jumpSpeed;
		caffeinated = body.caffeinated;
		bloodVolume = body.bloodVolume;
		bloodOxygen = body.bloodOxygen;
		bloodPressure = body.bloodPressure;
		respiratoryRate = body.respiratoryRate;
		heartRate = body.heartRate;
		bloodVesselSize = body.bloodVesselSize;
		bloodViscosity = body.bloodViscosity;
		strokeAmount = body.strokeAmount;
		hasPulmonaryEmbolism = body.hasPulmonaryEmbolism;
		fibrillationProgress = body.fibrillationProgress;
		venomCurrent = body.venomCurrent;
		venomTotal = body.venomTotal;
		hunger = body.hunger;
		thirst = body.thirst;
		septicShock = body.septicShock;
		temperature = body.temperature;
		sicknessAmount = body.sicknessAmount;
		consciousness = body.consciousness;
		stamina = body.stamina;
		energy = body.energy;
		happiness = body.happiness;
		weightOffset = body.weightOffset;
		radiationSickness = body.radiationSickness;
		internalBleeding = body.internalBleeding;
		hemothorax = body.hemothorax;
		traumaAmount = body.traumaAmount;
		dirtyness = body.dirtyness;
		wetness = body.wetness;
		badSleepAmount = body.badSleepAmount;
		hearingLoss = body.hearingLoss;
		antidepressantHappiness = body.antidepressantHappiness;
		opiateHappiness = body.opiateHappiness;
		antibioticImmunityTime = body.antibioticImmunityTime;
		brainGrowSickness = body.brainGrowSickness;
		lastStandTime = body.lastStandTime;
		adrenaline = body.adrenaline;
		sleeping = body.sleeping;
		mindwiped = body.IsMindwiped();
		stimulantMultiplier = body.stimulantMultiplier;
		Painkillers val = default(Painkillers);
		if (((Component)body).TryGetComponent<Painkillers>(ref val))
		{
			painkilled = true;
		}
		else
		{
			painkilled = false;
		}
		limb0 = new CharacterLimbHealthState(body.limbs[0]);
		limb1 = new CharacterLimbHealthState(body.limbs[1]);
		limb2 = new CharacterLimbHealthState(body.limbs[2]);
		limb3 = new CharacterLimbHealthState(body.limbs[3]);
		limb4 = new CharacterLimbHealthState(body.limbs[4]);
		limb5 = new CharacterLimbHealthState(body.limbs[5]);
		limb6 = new CharacterLimbHealthState(body.limbs[6]);
		limb7 = new CharacterLimbHealthState(body.limbs[7]);
		limb8 = new CharacterLimbHealthState(body.limbs[8]);
		limb9 = new CharacterLimbHealthState(body.limbs[9]);
		limb10 = new CharacterLimbHealthState(body.limbs[10]);
		limb11 = new CharacterLimbHealthState(body.limbs[11]);
		limb12 = new CharacterLimbHealthState(body.limbs[12]);
		limb13 = new CharacterLimbHealthState(body.limbs[13]);
		limb14 = new CharacterLimbHealthState(body.limbs[14]);
	}

	public void Apply(NetBody npc)
	{
		if (!(npc._last_sync_health_packet_receive_time > Time.realtimeSinceStartupAsDouble) || (brainHealth > 0f != npc.body.alive && npc.timeHasBeenDead > 1.0))
		{
			Apply(npc.body);
		}
	}

	public void Apply(Body body)
	{
		bool num = Util.IsBodyLocal(body);
		limb0.Apply(body.limbs[0]);
		limb1.Apply(body.limbs[1]);
		limb2.Apply(body.limbs[2]);
		limb3.Apply(body.limbs[3]);
		limb4.Apply(body.limbs[4]);
		limb5.Apply(body.limbs[5]);
		limb6.Apply(body.limbs[6]);
		limb7.Apply(body.limbs[7]);
		limb8.Apply(body.limbs[8]);
		limb9.Apply(body.limbs[9]);
		limb10.Apply(body.limbs[10]);
		limb11.Apply(body.limbs[11]);
		limb12.Apply(body.limbs[12]);
		limb13.Apply(body.limbs[13]);
		limb14.Apply(body.limbs[14]);
		body.clawHealth = clawHealth;
		body.bothEyesGone = bothEyesGone;
		body.disfigured = disfigured;
		body.eyeGone = eyeGone;
		body.brainHealth = brainHealth;
		body.usedNeuralBooster = usedNeuralBooster;
		body.maxSpeed = maxSpeed;
		body.moveForce = moveForce;
		body.jumpSpeed = jumpSpeed;
		body.caffeinated = caffeinated;
		body.bloodVolume = bloodVolume;
		body.bloodOxygen = bloodOxygen;
		body.bloodPressure = bloodPressure;
		body.respiratoryRate = respiratoryRate;
		body.heartRate = heartRate;
		body.bloodVesselSize = bloodVesselSize;
		body.bloodViscosity = bloodViscosity;
		body.strokeAmount = strokeAmount;
		body.hasPulmonaryEmbolism = hasPulmonaryEmbolism;
		body.fibrillationProgress = fibrillationProgress;
		body.venomCurrent = venomCurrent;
		body.venomTotal = venomTotal;
		body.hunger = hunger;
		body.thirst = thirst;
		body.septicShock = septicShock;
		body.temperature = temperature;
		body.sicknessAmount = sicknessAmount;
		body.consciousness = consciousness;
		body.stamina = stamina;
		body.energy = energy;
		body.happiness = happiness;
		body.weightOffset = weightOffset;
		body.radiationSickness = radiationSickness;
		body.internalBleeding = internalBleeding;
		body.hemothorax = hemothorax;
		body.traumaAmount = traumaAmount;
		body.dirtyness = dirtyness;
		body.wetness = wetness;
		body.badSleepAmount = badSleepAmount;
		body.hearingLoss = hearingLoss;
		body.antidepressantHappiness = antidepressantHappiness;
		body.opiateHappiness = opiateHappiness;
		body.antibioticImmunityTime = antibioticImmunityTime;
		body.brainGrowSickness = brainGrowSickness;
		body.lastStandTime = lastStandTime;
		body.adrenaline = adrenaline;
		body.sleeping = sleeping;
		if (num && succesfullyRolledLastStand && !body.succesfullyRolledLastStand)
		{
			Plugin.log.LogInfo((object)(KrokoshaScavMultiplayer.INPUT_USERNAME + ", rolled Last stand succesfully. "));
			((MonoBehaviour)PlayerCamera.main).StartCoroutine(PlayerCamera.main.LastStandSequence());
		}
		body.triedRollingLastStand = triedRollingLastStand;
		body.succesfullyRolledLastStand = succesfullyRolledLastStand;
		if (mindwiped && (Object)(object)body.mindWipe == (Object)null)
		{
			body.mindWipe = ComponentHolderProtocol.GetOrAddComponent<MindwipeScript>((Object)(object)body);
			body.mindWipe.active = true;
		}
		else if (!mindwiped && (Object)(object)body.mindWipe != (Object)null)
		{
			Object.Destroy((Object)(object)body.mindWipe);
		}
		Painkillers val = default(Painkillers);
		if (!painkilled && ((Component)body).TryGetComponent<Painkillers>(ref val))
		{
			val.opiateAmount = 0f;
			val.opiateTolerance = 0f;
		}
		body.stimulantMultiplier = stimulantMultiplier;
		body.burpTimer = burpTimer;
		skills.Apply(body);
		NetBody netBody = default(NetBody);
		if (((Component)body).TryGetComponent<NetBody>(ref netBody))
		{
			netBody.last_mood = happiness;
		}
	}

	public static int GetByteSize()
	{
		return Marshal.SizeOf(typeof(CharacterHealthStateSyncPacket));
	}
}
