using System.Collections.Generic;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public static class CoopKeybinds
{
	private const string prefix = "krokosha_coop_";

	public const string pointfingerat = "krokosha_coop_pointfingerat";

	public const string chat = "krokosha_coop_chat";

	public const string voicechat = "krokosha_coop_voicechat";

	public const string woundview = "krokosha_coop_woundview";

	public const string push = "krokosha_coop_push";

	public const string carry = "krokosha_coop_carry";

	public const string inventory = "krokosha_coop_inventory";

	public const string piggyback = "krokosha_coop_piggyback";

	public const string showplrs = "krokosha_coop_showplrs";

	public static Dictionary<string, KeyCode> as_dict = new Dictionary<string, KeyCode>
	{
		{
			"krokosha_coop_pointfingerat",
			(KeyCode)325
		},
		{
			"krokosha_coop_chat",
			(KeyCode)47
		},
		{
			"krokosha_coop_voicechat",
			(KeyCode)118
		},
		{
			"krokosha_coop_woundview",
			(KeyCode)114
		},
		{
			"krokosha_coop_push",
			(KeyCode)102
		},
		{
			"krokosha_coop_piggyback",
			(KeyCode)112
		},
		{
			"krokosha_coop_carry",
			(KeyCode)111
		},
		{
			"krokosha_coop_inventory",
			(KeyCode)105
		},
		{
			"krokosha_coop_showplrs",
			(KeyCode)304
		}
	};

	public static bool IsWoundViewSameAsOG()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		return KeyBinds.GetBind("krokosha_coop_woundview") == KeyBinds.GetBind("woundview");
	}
}
