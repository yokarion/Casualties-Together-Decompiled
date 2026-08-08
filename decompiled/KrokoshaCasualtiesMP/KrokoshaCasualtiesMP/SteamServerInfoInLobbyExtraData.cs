using System.Collections.Generic;
using System.Linq;

namespace KrokoshaCasualtiesMP;

public struct SteamServerInfoInLobbyExtraData
{
	public int deathcounter;

	public KrokoshaMultiplayerGameRules rules;

	public SteamServerInfoInLobbyExtraData(NetPublicServerInfo info)
	{
		deathcounter = 0;
		rules = info.rules;
	}

	public void Read(NetPublicServerInfo info)
	{
		info.rules = rules;
		Dictionary<string, string> defaults = new KrokoshaMultiplayerGameRules().ConvertToSerializableDict();
		IEnumerable<KeyValuePair<string, string>> enumerable = from x in rules.ConvertToSerializableDict()
			where !defaults[x.Key].Equals(x.Value)
			select x;
		if (enumerable.Count() > 0)
		{
			info.rules_as_string = Lang.Get("ruleschanged", false);
			{
				foreach (KeyValuePair<string, string> item in enumerable)
				{
					info.rules_as_string = info.rules_as_string + "\n" + item.Key + ": " + item.Value;
				}
				return;
			}
		}
		info.rules_as_string = Lang.Get("rulesdefault", false);
	}
}
