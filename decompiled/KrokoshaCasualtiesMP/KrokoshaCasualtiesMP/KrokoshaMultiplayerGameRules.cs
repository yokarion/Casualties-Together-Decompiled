using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using KrokoshaCasualtiesUtils;
using LiteNetLib.Utils;

namespace KrokoshaCasualtiesMP;

[Serializable]
public struct KrokoshaMultiplayerGameRules : INetSerializeByMemcpy, IDeltaPacketBase
{
	public bool sv_cheats = false;

	public bool AllowClientCheatCommands = false;

	[KrokoshaRuleByte(200, "")]
	public byte PLAYER_COUNT_LIMIT = 6;

	public bool ShowPlayerDirections = true;

	public bool EnableNametags = true;

	public bool EnableStatusIcons = true;

	public bool UnchippedHideNametags = true;

	public bool EnableChatbox = true;

	public bool OnlyProximityChat = false;

	public bool UnchippedProximityChat = true;

	public bool UnchippedIsIndividual = true;

	[KrokoshaRuleByte(100, "%")]
	public byte ScatterMinGroupSize = 51;

	[KrokoshaRuleFloat(0f, 1500f, "M")]
	public float ScatterPunishDistance = 80f;

	[KrokoshaRuleByte(101, "%")]
	public byte LayerFinishPlrPercent = 60;

	public bool LayerFinishKeepXOffset = true;

	[KrokoshaRuleByte(100, "%")]
	public byte StragglerRadlinePercent = 30;

	public bool NoInventoryLock = false;

	public bool EnableSleep = false;

	public bool EnableTimeManipulation = false;

	public bool SpeechImpairedChat = true;

	public bool HearingLossChat = true;

	public bool MindwipeDisablesChat = true;

	public bool DeadTextchat = true;

	public bool DeadVoicechat = true;

	public bool SleepingMute = false;

	public bool Permadeath = false;

	public bool ReviveOnNextLevel = false;

	public bool ReviveFromTrader = true;

	public bool RespawnKeepInventory = false;

	public bool RespawnKeepSkills = false;

	public bool AllowSpectatorFreecam = true;

	public bool AllowPush = true;

	public bool AlwaysAllowCarry = false;

	[KrokoshaRuleByte(254, "")]
	public byte PiggybackMaxStack = 1;

	[KrokoshaRuleFloat(0f, 2f, "")]
	public float PiggybackWeightMultiplier = 0.8f;

	public bool SpectateWhileUnconscious = false;

	public bool EnableMP3Sync = true;

	[KrokoshaRuleByte(4, "")]
	public byte VoicechatQuality = 4;

	public bool VoicechatEnabled = true;

	[KrokoshaRuleFloat(0f, 1500f, "M")]
	public float ProximityHearDistance = 55f;

	public bool CharacterYapPublic = true;

	public bool Teams = false;

	public bool PVP = false;

	public bool PVPCombatDismember = true;

	public float PVPMoodDebuff = 0.5f;

	public float PVPDamageMultiplier = 1f;

	public bool LateJoinAllowed = true;

	public bool LateJoinSpectate = false;

	public bool AmputateHealthyPlayers = true;

	public float AdditionalBrainRegen = 1f;

	public float AdditionalHealthRegen = 1f;

	public float AdditionalHealthDecay = 1f;

	public bool LastStandAllowed = true;

	public float SelfharmWitnessMoodDebuff = 3f;

	public bool SavePlayerState = true;

	public bool SavePlayerInventory = true;

	public bool SavePlayerPosition = true;

	public bool AutoContinue = false;

	public ushort AutoMinPlrsToStart = 2;

	public bool AutoExitWhenAllDied = true;

	public bool AutoExitWhenAllLeft = true;

	public static int bitset_size = 61;

	public bool PlayerScatterDisallowed
	{
		get
		{
			if (KrokoshaScavMultiplayer.rules.ScatterPunishDistance != 0f)
			{
				return KrokoshaScavMultiplayer.rules.ScatterPunishDistance < 449f;
			}
			return false;
		}
	}

	public bool AllowPiggyback => PiggybackMaxStack > 0;

	public bool DisconnectShouldSaveAnything
	{
		get
		{
			if (!SavePlayerState && !SavePlayerInventory)
			{
				return SavePlayerPosition;
			}
			return true;
		}
	}

	public bool DisableSleep => !EnableSleep;

	public bool DisableTimeManipulation => !EnableTimeManipulation;

	public bool CanReviveOnNextLevel()
	{
		if (!Permadeath)
		{
			return ReviveOnNextLevel;
		}
		return false;
	}

	public bool CanReviveFromTrader()
	{
		if (!Permadeath)
		{
			return ReviveFromTrader;
		}
		return false;
	}

	public bool CanCommunicateWithTheDeadTC()
	{
		if (!DeadTextchat)
		{
			return Util.IsTutorialWorld();
		}
		return true;
	}

	public bool CanCommunicateWithTheDeadVC()
	{
		if (!DeadVoicechat)
		{
			return Util.IsTutorialWorld();
		}
		return true;
	}

	public bool CheckIfLayerContinueIsDisabled()
	{
		return LayerFinishPlrPercent > 100;
	}

	public Dictionary<string, string> ConvertToSerializableDict()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		CultureInfo currentCulture = CultureInfo.CurrentCulture;
		try
		{
			CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
			FieldInfo[] fields = GetType().GetFields();
			foreach (FieldInfo fieldInfo in fields)
			{
				object value = fieldInfo.GetValue(this);
				dictionary[fieldInfo.Name] = value.ToString();
			}
			return dictionary;
		}
		finally
		{
			CultureInfo.CurrentCulture = currentCulture;
		}
	}

	public static KrokoshaMultiplayerGameRules ConvertFromSerializableDict(Dictionary<string, string> r)
	{
		KrokoshaMultiplayerGameRules krokoshaMultiplayerGameRules = new KrokoshaMultiplayerGameRules();
		foreach (KeyValuePair<string, string> item in r)
		{
			string key = item.Key;
			string value = item.Value;
			FieldInfo field = krokoshaMultiplayerGameRules.GetType().GetField(key);
			if (field == null)
			{
				log.warn("RULE DECODER: UNKNOWN RULE: " + key + " (attempt to set it to: " + value + ")");
				continue;
			}
			object value2;
			try
			{
				value2 = ((!(field.FieldType == typeof(bool))) ? TypeDescriptor.GetConverter(field.FieldType).ConvertFromInvariantString(value) : ((object)Con.ParseBool01(value)));
			}
			catch (Exception ex)
			{
				log.warn("RULE DECODER: SET RULE, BUT VALUE IS INVALID:\nINPUT: " + key + " = " + value + "\n" + ex.Message);
				continue;
			}
			TypedReference obj = __makeref(krokoshaMultiplayerGameRules);
			field.SetValueDirect(obj, value2);
		}
		return krokoshaMultiplayerGameRules;
	}

	public void SetDefault()
	{
	}

	public KrokoshaMultiplayerGameRules()
	{
	}

	public void Write(NetDataWriter writer, List<bool> pack_bools, IDeltaPacketBase old)
	{
		KrokoshaMultiplayerGameRules obj = (KrokoshaMultiplayerGameRules)(object)old;
		bool flag = false;
		pack_bools.Add(sv_cheats);
		pack_bools.Add(AllowClientCheatCommands);
		flag = obj.PLAYER_COUNT_LIMIT != PLAYER_COUNT_LIMIT;
		pack_bools.Add(flag);
		if (flag)
		{
			writer.Put(PLAYER_COUNT_LIMIT);
		}
		pack_bools.Add(ShowPlayerDirections);
		pack_bools.Add(EnableNametags);
		pack_bools.Add(EnableStatusIcons);
		pack_bools.Add(UnchippedHideNametags);
		pack_bools.Add(EnableChatbox);
		pack_bools.Add(OnlyProximityChat);
		pack_bools.Add(UnchippedProximityChat);
		pack_bools.Add(UnchippedIsIndividual);
		flag = obj.ScatterMinGroupSize != ScatterMinGroupSize;
		pack_bools.Add(flag);
		if (flag)
		{
			writer.Put(ScatterMinGroupSize);
		}
		flag = obj.ScatterPunishDistance != ScatterPunishDistance;
		pack_bools.Add(flag);
		if (flag)
		{
			writer.Put(ScatterPunishDistance);
		}
		flag = obj.LayerFinishPlrPercent != LayerFinishPlrPercent;
		pack_bools.Add(flag);
		if (flag)
		{
			writer.Put(LayerFinishPlrPercent);
		}
		pack_bools.Add(LayerFinishKeepXOffset);
		flag = obj.StragglerRadlinePercent != StragglerRadlinePercent;
		pack_bools.Add(flag);
		if (flag)
		{
			writer.Put(StragglerRadlinePercent);
		}
		pack_bools.Add(NoInventoryLock);
		pack_bools.Add(EnableSleep);
		pack_bools.Add(EnableTimeManipulation);
		pack_bools.Add(SpeechImpairedChat);
		pack_bools.Add(HearingLossChat);
		pack_bools.Add(MindwipeDisablesChat);
		pack_bools.Add(DeadTextchat);
		pack_bools.Add(DeadVoicechat);
		pack_bools.Add(SleepingMute);
		pack_bools.Add(Permadeath);
		pack_bools.Add(ReviveOnNextLevel);
		pack_bools.Add(ReviveFromTrader);
		pack_bools.Add(RespawnKeepInventory);
		pack_bools.Add(RespawnKeepSkills);
		pack_bools.Add(AllowSpectatorFreecam);
		pack_bools.Add(AllowPush);
		pack_bools.Add(AlwaysAllowCarry);
		flag = obj.PiggybackMaxStack != PiggybackMaxStack;
		pack_bools.Add(flag);
		if (flag)
		{
			writer.Put(PiggybackMaxStack);
		}
		flag = obj.PiggybackWeightMultiplier != PiggybackWeightMultiplier;
		pack_bools.Add(flag);
		if (flag)
		{
			writer.Put(PiggybackWeightMultiplier);
		}
		pack_bools.Add(SpectateWhileUnconscious);
		pack_bools.Add(EnableMP3Sync);
		flag = obj.VoicechatQuality != VoicechatQuality;
		pack_bools.Add(flag);
		if (flag)
		{
			writer.Put(VoicechatQuality);
		}
		pack_bools.Add(VoicechatEnabled);
		flag = obj.ProximityHearDistance != ProximityHearDistance;
		pack_bools.Add(flag);
		if (flag)
		{
			writer.Put(ProximityHearDistance);
		}
		pack_bools.Add(CharacterYapPublic);
		pack_bools.Add(Teams);
		pack_bools.Add(PVP);
		pack_bools.Add(PVPCombatDismember);
		flag = obj.PVPMoodDebuff != PVPMoodDebuff;
		pack_bools.Add(flag);
		if (flag)
		{
			writer.Put(PVPMoodDebuff);
		}
		flag = obj.PVPDamageMultiplier != PVPDamageMultiplier;
		pack_bools.Add(flag);
		if (flag)
		{
			writer.Put(PVPDamageMultiplier);
		}
		pack_bools.Add(LateJoinAllowed);
		pack_bools.Add(LateJoinSpectate);
		pack_bools.Add(AmputateHealthyPlayers);
		flag = obj.AdditionalBrainRegen != AdditionalBrainRegen;
		pack_bools.Add(flag);
		if (flag)
		{
			writer.Put(AdditionalBrainRegen);
		}
		flag = obj.AdditionalHealthRegen != AdditionalHealthRegen;
		pack_bools.Add(flag);
		if (flag)
		{
			writer.Put(AdditionalHealthRegen);
		}
		flag = obj.AdditionalHealthDecay != AdditionalHealthDecay;
		pack_bools.Add(flag);
		if (flag)
		{
			writer.Put(AdditionalHealthDecay);
		}
		pack_bools.Add(LastStandAllowed);
		flag = obj.SelfharmWitnessMoodDebuff != SelfharmWitnessMoodDebuff;
		pack_bools.Add(flag);
		if (flag)
		{
			writer.Put(SelfharmWitnessMoodDebuff);
		}
		pack_bools.Add(SavePlayerState);
		pack_bools.Add(SavePlayerInventory);
		pack_bools.Add(SavePlayerPosition);
		pack_bools.Add(AutoContinue);
		flag = obj.AutoMinPlrsToStart != AutoMinPlrsToStart;
		pack_bools.Add(flag);
		if (flag)
		{
			writer.Put(AutoMinPlrsToStart);
		}
		pack_bools.Add(AutoExitWhenAllDied);
		pack_bools.Add(AutoExitWhenAllLeft);
	}

	public void Read(NetDataReader reader, BitArray bitset)
	{
		sv_cheats = bitset[0];
		AllowClientCheatCommands = bitset[1];
		if (bitset[2])
		{
			reader.Get(ref PLAYER_COUNT_LIMIT);
		}
		ShowPlayerDirections = bitset[3];
		EnableNametags = bitset[4];
		EnableStatusIcons = bitset[5];
		UnchippedHideNametags = bitset[6];
		EnableChatbox = bitset[7];
		OnlyProximityChat = bitset[8];
		UnchippedProximityChat = bitset[9];
		UnchippedIsIndividual = bitset[10];
		if (bitset[11])
		{
			reader.Get(ref ScatterMinGroupSize);
		}
		if (bitset[12])
		{
			reader.Get(ref ScatterPunishDistance);
		}
		if (bitset[13])
		{
			reader.Get(ref LayerFinishPlrPercent);
		}
		LayerFinishKeepXOffset = bitset[14];
		if (bitset[15])
		{
			reader.Get(ref StragglerRadlinePercent);
		}
		NoInventoryLock = bitset[16];
		EnableSleep = bitset[17];
		EnableTimeManipulation = bitset[18];
		SpeechImpairedChat = bitset[19];
		HearingLossChat = bitset[20];
		MindwipeDisablesChat = bitset[21];
		DeadTextchat = bitset[22];
		DeadVoicechat = bitset[23];
		SleepingMute = bitset[24];
		Permadeath = bitset[25];
		ReviveOnNextLevel = bitset[26];
		ReviveFromTrader = bitset[27];
		RespawnKeepInventory = bitset[28];
		RespawnKeepSkills = bitset[29];
		AllowSpectatorFreecam = bitset[30];
		AllowPush = bitset[31];
		AlwaysAllowCarry = bitset[32];
		if (bitset[33])
		{
			reader.Get(ref PiggybackMaxStack);
		}
		if (bitset[34])
		{
			reader.Get(ref PiggybackWeightMultiplier);
		}
		SpectateWhileUnconscious = bitset[35];
		EnableMP3Sync = bitset[36];
		if (bitset[37])
		{
			reader.Get(ref VoicechatQuality);
		}
		VoicechatEnabled = bitset[38];
		if (bitset[39])
		{
			reader.Get(ref ProximityHearDistance);
		}
		CharacterYapPublic = bitset[40];
		Teams = bitset[41];
		PVP = bitset[42];
		PVPCombatDismember = bitset[43];
		if (bitset[44])
		{
			reader.Get(ref PVPMoodDebuff);
		}
		if (bitset[45])
		{
			reader.Get(ref PVPDamageMultiplier);
		}
		LateJoinAllowed = bitset[46];
		LateJoinSpectate = bitset[47];
		AmputateHealthyPlayers = bitset[48];
		if (bitset[49])
		{
			reader.Get(ref AdditionalBrainRegen);
		}
		if (bitset[50])
		{
			reader.Get(ref AdditionalHealthRegen);
		}
		if (bitset[51])
		{
			reader.Get(ref AdditionalHealthDecay);
		}
		LastStandAllowed = bitset[52];
		if (bitset[53])
		{
			reader.Get(ref SelfharmWitnessMoodDebuff);
		}
		SavePlayerState = bitset[54];
		SavePlayerInventory = bitset[55];
		SavePlayerPosition = bitset[56];
		AutoContinue = bitset[57];
		if (bitset[58])
		{
			reader.Get(ref AutoMinPlrsToStart);
		}
		AutoExitWhenAllDied = bitset[59];
		AutoExitWhenAllLeft = bitset[60];
	}
}
