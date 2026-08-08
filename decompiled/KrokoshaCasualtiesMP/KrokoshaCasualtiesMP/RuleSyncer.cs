using System;
using System.Reflection;
using LiteNetLib.Utils;

namespace KrokoshaCasualtiesMP;

public class RuleSyncer : CoolSyncSubSystemStatic
{
	public new KrokoshaMultiplayerGameRules cur_packet => (KrokoshaMultiplayerGameRules)(object)base.cur_packet;

	public RuleSyncer(byte systemid)
		: base(systemid)
	{
		SEND_FREQUENCY = 0.1f;
		base_packet = new KrokoshaMultiplayerGameRules();
		base.cur_packet = new KrokoshaMultiplayerGameRules();
	}

	protected override void PackPacket()
	{
		base.cur_packet = KrokoshaScavMultiplayer.rules;
	}

	protected override void Client_ReadData2(NetDataReader reader, ushort data2_len)
	{
		int num = 0;
		string text = "";
		try
		{
			FieldInfo[] fields = typeof(KrokoshaMultiplayerGameRules).GetFields(BindingFlags.Instance | BindingFlags.Public);
			foreach (FieldInfo fieldInfo in fields)
			{
				if (fieldInfo.GetValue(KrokoshaScavMultiplayer.rules).ToString() != fieldInfo.GetValue(cur_packet).ToString())
				{
					text += $"\n{fieldInfo.Name} = {fieldInfo.GetValue(cur_packet)}";
					num++;
				}
			}
			text = $"Server changed {num} game rules:{text}";
		}
		catch (Exception ex)
		{
			log.error(ex.ToString());
		}
		KrokoshaScavMultiplayer.rules = cur_packet;
		KrokoshaScavMultiplayer.ApplyGameRules();
		if (log.verbose && !string.IsNullOrWhiteSpace(text) && num > 0)
		{
			KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog(text);
		}
	}
}
