using System.Collections.Generic;
using System.Linq;

namespace KrokoshaCasualtiesMP;

public class KnownPersons : KrokoshaScavSingleton
{
	public static KnownPersons inst;

	private static bool aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa = false;

	private static HashSet<ulong> STARS = new HashSet<ulong> { 76561198328981815uL, 76561199087655775uL, 76561199067997543uL, 76561198053002922uL, 76561198253391969uL, 76561198453609208uL, 76561199077613293uL, 76561198113864661uL };

	public static ulong[] PRIVILEGED_STEAM_USERS = new ulong[3] { 76561198273985997uL, 76561198442198974uL, 76561198838808878uL };

	private static ulong[] MPMOD_DEV = new ulong[2] { 76561198273985997uL, 76561198838808878uL };

	private void Awake()
	{
		inst = this;
	}

	private void Start()
	{
		LoadEverything();
	}

	public static void LoadTagsFor(NetPlayer plr)
	{
		if (Net.TryGetSteamTransport(out var _))
		{
			_ = plr.steam_id;
			if (STARS.Contains(plr.steam_id) && !plr.additional_profile_tag_icons.Contains(KrokoshaCoopModAssets.star_yellow.texture))
			{
				plr.additional_profile_tag_icons.Add(KrokoshaCoopModAssets.star_yellow.texture);
			}
			if (MPMOD_DEV.Contains(plr.steam_id) && !plr.additional_profile_tag_icons.Contains(KrokoshaCoopModAssets.mpmod_icon_green.texture))
			{
				plr.additional_profile_tag_icons.Add(KrokoshaCoopModAssets.mpmod_icon_green.texture);
			}
		}
	}

	public static bool CanDoFancyNametagFor(NetPlayer plr)
	{
		if (!Net.is_playing_with_steam)
		{
			return false;
		}
		if (STARS.Contains(plr.steam_id))
		{
			return true;
		}
		if (PRIVILEGED_STEAM_USERS.Contains(plr.steam_id))
		{
			return true;
		}
		return false;
	}

	private static void LoadEverything()
	{
	}
}
