using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using KrokoshaCasualtiesUtils;
using LiteNetLib;
using LiteNetLib.Utils;
using Steamworks;
using TMPro;
using Together;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public class NetPlayer : MonoBehaviour
{
	public readonly Dictionary<string, object> CUSTOM_LOCAL_DATA = new Dictionary<string, object>();

	public Server_PlayerState server_plrstate;

	public ulong SteamId;

	public Texture2D profilepic_largeicon;

	public Texture2D profilepic_mediumicon;

	public Texture2D profilepic_smallicon;

	public List<Texture2D> KnownUserTagIcons = new List<Texture2D>();

	public Vector2 cursorpos = Vector2.zero;

	public Vector2 camerapos = Vector2.zero;

	public static Dictionary<Body, NetPlayer> BodyToPlayerDict = new Dictionary<Body, NetPlayer>();

	public static Dictionary<knetid, NetPlayer> ClientIdToPlayerDict = new Dictionary<knetid, NetPlayer>();

	public static List<NetPlayer> AllLivingPlayers = new List<NetPlayer>();

	public static List<NetPlayer> AllDeadPlayers = new List<NetPlayer>();

	public bool late_joined;

	public bool unchipped;

	public double levelPlayTime;

	public CoUtils personal_coutils_instance;

	public List<int> tosave_hascrafterbeforerecipes = new List<int>();

	[DoSync]
	public AnyObjectNetId minigame_targetobject;

	[DoSync]
	public knetid minigame_currentItem_syncid = (ushort)0;

	[AlwaysSync]
	public ushort minigame_session;

	[DoSync]
	public bool minigame_lmb_down;

	[AlwaysSync]
	public byte minigame_current_type = byte.MaxValue;

	[DoSync]
	public Vector2 minigame_handpos = Vector2.zero;

	[DoSync]
	public Vector2 minigame_mousepos = Vector2.zero;

	[DoSync]
	public byte minigame_handsprite = 2;

	[DoSync]
	public bool is_in_cmd;

	[DoSync]
	public bool is_alttab;

	[DoSync]
	public bool is_chatting;

	[DoSync]
	public bool is_crafting;

	[DoSync]
	public bool is_trading;

	[AlwaysSync]
	public ushort woundViewTargetNetBodyId;

	[DoSync]
	public bool server_mute_vc;

	[DoSync]
	public bool server_mute_tc;

	public const string plrnameprefix = "PlayerObject_";

	public const string plrnametagprefix = "PlayerNameTag_";

	public bool nameIsCustom;

	public string playername = "ERROR-NO-NAME";

	public bool _tutorial_finished;

	public bool finishedLayer;

	public double ping = -0.01;

	public GameObject locationPingCircle;

	public GameObject locationPingArrow;

	public Body body;

	public static NetPlayer LOCAL_PLAYER;

	private static TMP_FontAsset s_retroGamingFontAsset = null;

	public Color24 playerColor = Color.black;

	public static Color[] DEFAULT_TEAMS = (Color[])(object)new Color[4]
	{
		Color.red,
		Color.blue,
		Color.green,
		Color.yellow
	};

	public static Color[] NAMETAG_DEFAULT_COLORS = (Color[])(object)new Color[7]
	{
		Color.red,
		Color.green,
		Color.blue,
		Color.cyan,
		Color.yellow,
		Color.magenta,
		Color.white
	};

	public static Color[] NAMETAG_DEFAULT_COLORS_RANDOMIZED = (Color[])(object)new Color[7]
	{
		Color.red,
		Color.green,
		Color.blue,
		Color.cyan,
		Color.yellow,
		Color.magenta,
		Color.white
	};

	private static byte SERVER_NAMETAG_COLOR_PICKER_COUNTER = 0;

	public List<NetBody> visiblebodies = new List<NetBody>();

	private static NetDataWriter ragdollpacketwriter = Net.CreateWriter(10179);

	public const double FINGERPOINTINGTIME_MAX = 3.0;

	public bool is_pointingfingerat;

	public byte is_pointingfingeratTYPE;

	public double is_pointingfingeratTIME = -9990.0;

	public Vector2 is_pointingfingeratTARGET = Vector2.zero;

	public Texture2D profilepic_any_largetosmall => profilepic_largeicon ?? profilepic_mediumicon ?? profilepic_smallicon;

	public Texture2D profilepic_any_smalltolarge => profilepic_smallicon ?? profilepic_mediumicon ?? profilepic_largeicon;

	public knetid clientId { get; internal set; }

	public bool is_local { get; internal set; }

	public bool is_host { get; internal set; }

	internal VoicechatOutput vc_output { get; private set; }

	public bool minigame_is_in_a_minigame => minigame_current_type != byte.MaxValue;

	public SyncInfo minigame_currentItem_syncinfo
	{
		get
		{
			if (ItemSync.TryGetItem(minigame_currentItem_syncid, out var isi, out var _))
			{
				return isi;
			}
			return null;
		}
		set
		{
			minigame_currentItem_syncid = value.syncId;
		}
	}

	public Item minigame_currentItem
	{
		get
		{
			if (ItemSync.TryGetItem(minigame_currentItem_syncid, out var _, out var item))
			{
				return item;
			}
			return null;
		}
		set
		{
			if ((Object)(object)value == (Object)null || NetObjectRegistry.ObjectCanBeIgnoredForNetwork(((Component)value).gameObject))
			{
				minigame_currentItem_syncid = (ushort)0;
			}
			else if (Net.is_server)
			{
				SyncInfo syncInfo = NetObjectRegistry.Server_EnsureItemIsNetworkRegistered(((Component)value).gameObject);
				if (syncInfo != null)
				{
					minigame_currentItem_syncid = syncInfo.syncId;
				}
			}
		}
	}

	public Vector2 pos
	{
		get
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			if (IsAlive())
			{
				return playerbody.position;
			}
			return camerapos;
		}
		set
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)body != (Object)null)
			{
				playerbody.SetBodyPosition(value);
			}
		}
	}

	public double rtt_in_seconds_with_margin => ping * 4.1;

	public int ping_as_ms => (int)(ping * 1000.0);

	public GameObject chara => ((Component)((Component)body).transform.parent).gameObject;

	public NetBody playerbody
	{
		get
		{
			Body obj = body;
			if (obj == null)
			{
				return null;
			}
			return ((Component)obj).GetComponent<NetBody>();
		}
	}

	public string playername_as_hex => StringUtility.ToHexString(Encoding.UTF8.GetBytes(playername));

	public static event Action<NetPlayer> OnPlayerJoined;

	public static event Action<NetPlayer> OnPlayerLeft;

	public bool IsAlive()
	{
		if ((Object)(object)body == (Object)null)
		{
			return false;
		}
		return body.alive;
	}

	public bool IsUnchipped()
	{
		if ((Object)(object)body == (Object)null)
		{
			return unchipped;
		}
		if ((Object)(object)playerbody == (Object)null)
		{
			return unchipped;
		}
		return playerbody.unchipped;
	}

	public bool ImpairedSpeech()
	{
		if ((Object)(object)body == (Object)null)
		{
			return false;
		}
		if (body.alive)
		{
			return body.talker.impairedSpeech;
		}
		return false;
	}

	public bool IsAliveAndNotCriticallyDying()
	{
		if ((Object)(object)body == (Object)null)
		{
			return false;
		}
		if (body.alive)
		{
			return !body.isCriticallyDying;
		}
		return false;
	}

	public bool IsDeadOrCriticallyDying()
	{
		if ((Object)(object)body == (Object)null)
		{
			return false;
		}
		return body.IsDeadOrCriticallyDying();
	}

	public bool IsConscious()
	{
		Body obj = body;
		if (obj == null)
		{
			return false;
		}
		return obj.conscious;
	}

	public bool IsDead()
	{
		if ((Object)(object)body != (Object)null)
		{
			return !body.alive;
		}
		return false;
	}

	public bool IsAliveAndMindwiped()
	{
		if (IsAlive())
		{
			return body.IsMindwiped();
		}
		return false;
	}

	public bool IsAliveAndConscious()
	{
		if ((Object)(object)body == (Object)null)
		{
			return false;
		}
		if (body.alive)
		{
			return body.conscious;
		}
		return false;
	}

	public bool IsAliveAndNotConscious()
	{
		if ((Object)(object)body != (Object)null && body.alive)
		{
			return !body.conscious;
		}
		return false;
	}

	public bool IsAliveAndUnconsciousAndNotSleeping()
	{
		if ((Object)(object)body != (Object)null && body.alive)
		{
			if (!body.conscious)
			{
				return !body.sleeping;
			}
			return false;
		}
		return false;
	}

	public static List<NetBody> GetPlayerBodiesInRadius(Vector2 pos, float radius)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		radius *= radius;
		List<NetBody> list = new List<NetBody>();
		NetBody item = default(NetBody);
		foreach (Body key in BodyToPlayerDict.Keys)
		{
			if (KM.dist2dsqrcheck_presqr(Vector2.op_Implicit(((Component)key).transform.position), in pos, radius) && ((Component)key).TryGetComponent<NetBody>(ref item))
			{
				list.Add(item);
			}
		}
		return list;
	}

	public static List<NetPlayer> GetPlayersInRadius(Vector2 pos, float radius)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		radius *= radius;
		List<NetPlayer> list = new List<NetPlayer>();
		foreach (NetPlayer value in ClientIdToPlayerDict.Values)
		{
			if (KM.dist2dsqrcheck_presqr(value.pos, in pos, radius))
			{
				list.Add(value);
			}
		}
		return list;
	}

	public static (NetBody, float) GetNearestConsciousPlayerToThisBody(NetBody thisone)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		float num = float.PositiveInfinity;
		NetBody item = null;
		foreach (KeyValuePair<Body, NetPlayer> item2 in BodyToPlayerDict)
		{
			NetBody component = ((Component)item2.Key).GetComponent<NetBody>();
			if ((component.body.conscious || component.body.sleeping) && (Object)(object)thisone != (Object)(object)component)
			{
				float num2 = KM.dist2dsqr(Vector2.op_Implicit(((Component)component).transform.position), Vector2.op_Implicit(((Component)thisone).transform.position));
				if (num2 < num)
				{
					num = num2;
					item = ((Component)item2.Key).GetComponent<NetBody>();
				}
			}
		}
		return (item, num);
	}

	public static (NetPlayer, float) GetDistanceToNearestLivingPlayer(Vector2 target)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		float num = float.PositiveInfinity;
		NetPlayer item = null;
		foreach (NetPlayer allLivingPlayer in AllLivingPlayers)
		{
			float num2 = KM.dist2dsqr(Vector2.op_Implicit(((Component)allLivingPlayer.body).transform.position), in target);
			if (num2 < num)
			{
				num = num2;
				item = allLivingPlayer;
			}
		}
		return (item, num);
	}

	[Obsolete("probably?")]
	public void OnFinishTutorial(bool tp = true)
	{
		if (!_tutorial_finished)
		{
			_tutorial_finished = true;
			Chat.Server_ChatAnnouncement(playername + " just finished the tutorial.");
		}
	}

	public bool TryGetNetBody(out NetBody pb)
	{
		if ((Object)(object)this == (Object)null)
		{
			pb = null;
			return false;
		}
		if ((Object)(object)body != (Object)null && ((Component)body).TryGetComponent<NetBody>(ref pb))
		{
			return true;
		}
		pb = null;
		return false;
	}

	public static bool TryGetLocalNetBody(out NetBody nb)
	{
		if ((Object)(object)PlayerCamera.main != (Object)null && (Object)(object)PlayerCamera.main.body != (Object)null)
		{
			return ((Component)PlayerCamera.main.body).TryGetComponent<NetBody>(ref nb);
		}
		nb = null;
		return false;
	}

	public static NetBody GetLocalNetBodyNullable()
	{
		if ((Object)(object)PlayerCamera.main != (Object)null && (Object)(object)PlayerCamera.main.body != (Object)null)
		{
			return ((Component)PlayerCamera.main.body).GetComponent<NetBody>();
		}
		return null;
	}

	public override string ToString()
	{
		if ((Object)(object)this == (Object)null)
		{
			return "PLR(NULL)";
		}
		if (SteamId != 0L && Net.IsRunningSteam)
		{
			return $"PLR(ID:{clientId}, name:{playername}, SteamID: {SteamId})";
		}
		return $"PLR(ID:{clientId}, name:{playername})";
	}

	public static NetPlayer GetNetPlayerFromClientId(knetid clientid)
	{
		return GeneralExtensions.GetValueSafe<knetid, NetPlayer>(ClientIdToPlayerDict, clientid);
	}

	public static bool TryGetPlayerFromClientId(knetid clientid, out NetPlayer plr)
	{
		return ClientIdToPlayerDict.TryGetValue(clientid, out plr);
	}

	public static bool TryGetNetPlayerAndBodyFromClientId(knetid clientid, out NetPlayer plr, out Body body)
	{
		if (ClientIdToPlayerDict.TryGetValue(clientid, out plr) && (Object)(object)plr.body != (Object)null)
		{
			body = plr.body;
			return true;
		}
		plr = null;
		body = null;
		return false;
	}

	public static bool TryGetNetPlayerAndNetBodyFromClientId(knetid clientid, out NetPlayer plr, out NetBody pb)
	{
		if (ClientIdToPlayerDict.TryGetValue(clientid, out plr) && (Object)(object)plr.body != (Object)null)
		{
			pb = plr.playerbody;
			return true;
		}
		plr = null;
		pb = null;
		return false;
	}

	public static Body GetBodyFromClientId(knetid clientid)
	{
		NetPlayer netPlayerFromClientId = GetNetPlayerFromClientId(clientid);
		if ((Object)(object)netPlayerFromClientId != (Object)null)
		{
			return netPlayerFromClientId.body;
		}
		return null;
	}

	public static knetid GetClientIdFromBody(Body body)
	{
		return ((Component)body).GetComponent<NetBody>().netId;
	}

	public static NetPlayer GetNetPlayerFromBody(Body body)
	{
		return GeneralExtensions.GetValueSafe<Body, NetPlayer>(BodyToPlayerDict, body);
	}

	public void ApplyNameAndColor(string name, Color24 color)
	{
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		if (Net.TryGetSteamTransport(out var _))
		{
			if (!nameIsCustom && SteamId.ToString() == name)
			{
				name = KSteam.GetSteamUsername(SteamId);
			}
		}
		else
		{
			nameIsCustom = true;
		}
		if ((Object)(object)s_retroGamingFontAsset == (Object)null)
		{
			s_retroGamingFontAsset = (from x in Object.FindObjectsOfType<TextMeshProUGUI>()
				select ((TMP_Text)x).font).FirstOrDefault((TMP_FontAsset x) => (Object)(object)x != (Object)null && ((Object)x).name == "Retro GamingPix");
		}
		if ((Object)(object)s_retroGamingFontAsset != (Object)null)
		{
			name = FontUtils.ReplaceMissingCharacters(s_retroGamingFontAsset, name);
		}
		bool flag = nameIsCustom && playername != name;
		bool flag2 = playerColor != color;
		playername = name;
		playerColor = color;
		((Object)((Component)this).gameObject).name = "PlayerObject_" + name;
		if (KrokoshaScavMultiplayer.is_server && (flag || flag2))
		{
			List<knetid> list = ClientIdToPlayerDict.Keys.ToList();
			list.Remove((ushort)0);
			Server__ResponsePlayerName(list);
		}
		Color val = color;
		new Color(val.r, val.g, val.b, 0.9f);
		Color color2 = default(Color);
		((Color)(ref color2))._002Ector(val.r, val.g, val.b, 0f);
		if ((Object)(object)body != (Object)null)
		{
			playerbody.ApplyNameAndColor(name, color);
		}
		if (Object.op_Implicit((Object)(object)locationPingCircle))
		{
			((Object)locationPingCircle).name = "fingerpointercircle_" + playername;
			ComponentHolderProtocol.GetOrAddComponent<SpriteRenderer>((Object)(object)locationPingCircle).color = color2;
		}
		if (Object.op_Implicit((Object)(object)locationPingArrow))
		{
			((Object)locationPingArrow).name = "fingerpointerarrow_" + playername;
			ComponentHolderProtocol.GetOrAddComponent<SpriteRenderer>((Object)(object)locationPingArrow).color = color2;
		}
	}

	private void FixedUpdate()
	{
		if (KrokoshaScavMultiplayer.is_server && Util.IsInWorld() && !Util.IsWorldGenerated())
		{
			server_plrstate.is_loaded_in = false;
		}
		if (Util.IsWorldGenerated() && (is_local || KrokoshaScavMultiplayer.is_client || server_plrstate.is_loaded_in))
		{
			levelPlayTime += Time.fixedUnscaledDeltaTime;
		}
		else
		{
			levelPlayTime = 0.0;
		}
		Object.op_Implicit((Object)(object)body);
	}

	public bool IsInSameTeamAs(NetPlayer plr)
	{
		return playerColor == plr.playerColor;
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad((Object)(object)((Component)this).gameObject);
		if (KrokoshaScavMultiplayer.is_server)
		{
			server_plrstate = new Server_PlayerState(this);
		}
	}

	public static bool CheckIfPlrColorIsValid(Color24 c)
	{
		if (c.Sum() < 153)
		{
			return false;
		}
		return true;
	}

	private static Color24 PickNewPlayerColor()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < NAMETAG_DEFAULT_COLORS.Length; i++)
		{
			if (SERVER_NAMETAG_COLOR_PICKER_COUNTER >= NAMETAG_DEFAULT_COLORS.Length)
			{
				SERVER_NAMETAG_COLOR_PICKER_COUNTER = 0;
			}
			Color24 result = NAMETAG_DEFAULT_COLORS[SERVER_NAMETAG_COLOR_PICKER_COUNTER];
			SERVER_NAMETAG_COLOR_PICKER_COUNTER++;
			if (!ClientIdToPlayerDict.Values.Any((NetPlayer x) => x.playerColor == result))
			{
				return result;
			}
		}
		for (int num = 0; num < 200; num++)
		{
			Color24 result2 = new Color24((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255));
			if (!CheckIfPlrColorIsValid(result2))
			{
				result2[Random.Range(0, 3)] = (byte)Random.Range(200, 255);
			}
			if (!ClientIdToPlayerDict.Values.Any((NetPlayer x) => x.playerColor == result2))
			{
				return result2;
			}
		}
		log.error("COULDN'T PICK A NEW COLOR FOR A PLAYER AAAAAAAAA WTFFFFF!!?!?!?!?");
		return Utils.PickRandom<Color>(NAMETAG_DEFAULT_COLORS);
	}

	private void Start()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected O, but got Unknown
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Expected O, but got Unknown
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Expected O, but got Unknown
		locationPingCircle = new GameObject("fingerpointercircle");
		SpriteRenderer obj = locationPingCircle.AddComponent<SpriteRenderer>();
		obj.sprite = CoopModAssets.circlething;
		obj.color = new Color(0f, 0f, 0f, 0f);
		((Renderer)obj).sortingOrder = 6000;
		Object.DontDestroyOnLoad((Object)(object)locationPingCircle);
		locationPingCircle.SetActive(false);
		locationPingArrow = new GameObject("fingerpointerarrow");
		SpriteRenderer obj2 = locationPingArrow.AddComponent<SpriteRenderer>();
		obj2.sprite = CoopModAssets.arrowicon;
		obj2.color = new Color(0f, 0f, 0f, 0f);
		((Renderer)obj2).sortingOrder = 6000;
		locationPingArrow.transform.localScale = Vector3.one * 8f * 0.08f;
		Object.DontDestroyOnLoad((Object)(object)locationPingArrow);
		locationPingArrow.SetActive(false);
		bool flag = Net.TRANSPORT != null && Net.TRANSPORT is TransportSteamworks;
		if (flag)
		{
			KSteam.LoadPlayerProfilePics(this);
		}
		KnownPersons.LoadTagsFor(this);
		if (KrokoshaScavMultiplayer.is_server)
		{
			PlrSync.inst.Server_NewObject(this, clientId);
			if (!is_local)
			{
				try
				{
					ServerMain._Server_OnServerNameChange(new List<knetid> { clientId });
				}
				catch (Exception ex)
				{
					log.error(ex.ToString());
				}
			}
			if (!CheckIfPlrColorIsValid(playerColor))
			{
				playerColor = PickNewPlayerColor();
			}
			((Object)this).name = "PlayerObject_" + playername;
			string message = playername + " just joined the game!";
			int num = ClientIdToPlayerDict.Count + 1;
			if (KrokoshaScavMultiplayer.rules.AutoContinue && !Util.IsInWorld() && num <= KrokoshaScavMultiplayer.rules.AutoMinPlrsToStart)
			{
				message += $" {num}/{KrokoshaScavMultiplayer.rules.AutoMinPlrsToStart} minimum to start.";
			}
			Chat.Server_ChatAnnouncement(in message);
			if (KrokoshaScavMultiplayer.is_server)
			{
				string persistentId = GetPersistentId();
				if (ServerMain.server_lastplayerstates.TryGetValue(persistentId, out var value))
				{
					playerColor = value.plrcolor;
				}
			}
		}
		ClientIdToPlayerDict[clientId] = this;
		vc_output = ComponentHolderProtocol.GetOrAddComponent<VoicechatOutput>((Object)(object)this);
		if (Util.IsInWorld())
		{
			late_joined = true;
		}
		if (is_local)
		{
			((Behaviour)vc_output).enabled = false;
			personal_coutils_instance = CoUtils_instance_MultiplayerPatch.original_local_instance;
			if (!flag)
			{
				playername = KrokoshaScavMultiplayer.INPUT_USERNAME;
			}
			((Object)this).name = "PlayerObject_" + playername;
			LOCAL_PLAYER = this;
		}
		else
		{
			GameObject val = new GameObject("CoUtilsInstance_PerPlr");
			personal_coutils_instance = val.AddComponent<CoUtils>();
			val.transform.SetParent(((Component)this).transform);
			if (Util.IsInWorld() && Net.is_server)
			{
				CreateCharacter();
			}
		}
		if (KrokoshaScavMultiplayer.is_server && !is_local)
		{
			foreach (KeyValuePair<knetid, NetPlayer> item in ClientIdToPlayerDict)
			{
				item.Value.Server__ResponsePlayerName(clientId);
			}
		}
		ApplyNameAndColor(playername, playerColor);
		NetPlayer.OnPlayerJoined?.Invoke(this);
		((MonoBehaviour)this).InvokeRepeating("SlowUpdate", 1f, 1f);
		((MonoBehaviour)this).InvokeRepeating("SlowButFastUpdate", 1f, 0.1f);
		if (!KrokoshaScavMultiplayer.is_server)
		{
			return;
		}
		KrokoshaScavMultiplayer.ApplyGameRules();
		Util.DelayCallLambda(0.1f, (Action)delegate
		{
			if ((Object)(object)this != (Object)null && !is_local && Util.IsInWorld())
			{
				NetDataWriter writer = Net.CreateWriter(10021);
				writer.Put(ServerMain.LAST_STARTGAME_ANNOUNCEMENT_PACKET);
				writer.Put(WorldgenPatches.CompileRunSettings(), oneByteChars: true);
				IEnumerable<knetid> clientIds = new List<knetid> { clientId };
				Net.Server_SendToClientsVeryReliable(in writer, in clientIds);
				ServerMain.Server_AnnounceSeed(new List<knetid> { clientId });
			}
		});
	}

	public string GetPersistentId()
	{
		if (Net.IsRunningSteam)
		{
			return "STEAM_" + SteamId;
		}
		return "NAME_" + playername_as_hex;
	}

	private void SlowUpdate()
	{
		if (Net.is_server && (Object)(object)body != (Object)null && Util.IsWorldGenerated() && is_local)
		{
			server_plrstate.is_loaded_in = true;
			server_plrstate.did_give_spawn_location = true;
		}
	}

	private void SlowButFastUpdate()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		visiblebodies.Clear();
		if (Net.is_server && Util.IsWorldGenerated())
		{
			NetBody.GetBodiesInRadius(pos, 40f, visiblebodies);
			if (server_plrstate.is_loaded_in)
			{
				Server_SendRagdollPackets();
			}
		}
	}

	private void Server_SendRagdollPackets()
	{
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		if (is_local)
		{
			return;
		}
		NetDataWriter writer = ragdollpacketwriter;
		foreach (NetBody visiblebody in visiblebodies)
		{
			if (!visiblebody.body.standing && !((Object)(object)visiblebody.piggybacking_on != (Object)null) && (!((Object)(object)visiblebody.player == (Object)(object)this) || Server_HasForcedServerOwnershipOfBody()))
			{
				writer.ResetKeepMsgId();
				writer.Put((ushort)visiblebody.netId);
				int length = writer.Length;
				ClientMain.WriteRagdollPacket(ref writer, visiblebody.body);
				writer.CompressWriter(length);
				if (Net.TryGetSteamTransport(out var tsteam))
				{
					tsteam.Server_SendTo(0, in writer, clientId);
				}
				else
				{
					Net.Server_SendToClients((DeliveryMethod)4, in writer, clientId);
				}
			}
		}
	}

	public void Server__ResponsePlayerName(knetid requester_clientid)
	{
		Server__ResponsePlayerName(new List<knetid> { requester_clientid });
	}

	public void Server__ResponsePlayerName(IReadOnlyList<knetid> requester_clientid_list)
	{
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		if (requester_clientid_list.Count == 0)
		{
			return;
		}
		List<knetid> list = new List<knetid>(requester_clientid_list);
		NetDataWriter writer = Net.CreateWriter(10023);
		if (list.Remove(clientId))
		{
			writer.Put(true);
			writer.Put(is_host);
			writer.Put(nameIsCustom);
			if (nameIsCustom)
			{
				writer.Put(playername);
			}
			writer.Put(playerColor);
			writer.Put((ushort)clientId);
			writer.Put(SteamId);
			Net.Server_SendToClients((DeliveryMethod)2, in writer, clientId);
		}
		if (list.Count != 0)
		{
			writer.ResetKeepMsgId();
			writer.Put(false);
			writer.Put(is_host);
			writer.Put(nameIsCustom);
			if (nameIsCustom)
			{
				writer.Put(playername);
			}
			writer.Put(playerColor);
			writer.Put((ushort)clientId);
			writer.Put(SteamId);
			DeliveryMethod delivery = (DeliveryMethod)2;
			IEnumerable<knetid> clientIds = list;
			Net.Server_SendToClients(in delivery, in writer, in clientIds);
		}
	}

	[ClientReceiver(10170, true)]
	private static void ClientReceiver__PlayerLeft(knetid _, ref NetDataReader reader)
	{
		reader.Get(out knetid result);
		if (TryGetPlayerFromClientId(result, out var plr))
		{
			string msg = plr.playername + " disconnected.";
			Chat.LogMessage("*SERVER*", msg, false);
			Object.Destroy((Object)(object)plr);
		}
		else
		{
			log.warn($"CLIENT: Received PlayerLeft packet, but the player doesnt exist: {result}");
		}
	}

	[ClientReceiver(10023, true)]
	private static void ClientReceiver__PlayerNameResponse(knetid _, ref NetDataReader reader)
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		bool flag = default(bool);
		reader.Get(ref flag);
		bool flag2 = default(bool);
		reader.Get(ref flag2);
		bool flag3 = default(bool);
		reader.Get(ref flag3);
		string name = "NONAME";
		if (flag3)
		{
			reader.Get(ref name);
		}
		reader.Get(out Color24 result);
		reader.Get(out knetid result2);
		ulong num = default(ulong);
		reader.Get(ref num);
		if (Net.TryGetSteamTransport(out var _))
		{
			if (!KSteam.CURRENT_LOBBY.members.ContainsKey((CSteamID)num))
			{
				return;
			}
			if (!flag3)
			{
				name = KSteam.GetSteamUsername(num);
			}
		}
		if (!TryGetPlayerFromClientId(result2, out var plr))
		{
			plr = Net.CreatePlayer(result2, name, result);
		}
		plr.is_local = flag;
		plr.is_host = flag2;
		if (flag)
		{
			LOCAL_PLAYER = plr;
		}
		plr.SteamId = num;
		plr.ApplyNameAndColor(name, result);
		KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog($"Received others player name {plr} color: {plr.playerColor}  its_me: {flag}");
	}

	public bool HasGunEquipped(out GunScript gun)
	{
		GunScript val = default(GunScript);
		if ((Object)(object)body != (Object)null && body.conscious && body.HoldingItem(body.handSlot) && ((Component)body.GetItem(body.handSlot)).TryGetComponent<GunScript>(ref val))
		{
			gun = val;
			return true;
		}
		gun = null;
		return false;
	}

	public bool CanCommunicateWith_TextChat(NetPlayer target)
	{
		if ((Object)(object)target == (Object)(object)this)
		{
			return true;
		}
		if ((Object)(object)target != (Object)null)
		{
			if (Util.IsTutorialWorld())
			{
				return true;
			}
			bool flag = (Object)(object)target.body == (Object)null || target.body.alive;
			if (!((Object)(object)body == (Object)null) && !body.alive && flag && !KrokoshaScavMultiplayer.rules.DeadTextchat)
			{
				return false;
			}
			return true;
		}
		return false;
	}

	public void ResetEntropy()
	{
		if ((Object)(object)body != (Object)null)
		{
			body.ResetEntropy();
			playerbody.SetNetHealthSyncIgnoreTime(0.1f);
		}
	}

	public void CreateCharacter()
	{
		NetBody.CreateNewPlayerCharacter(this);
	}

	public bool Server_HasForcedServerOwnershipOfBody()
	{
		if ((Object)(object)body != (Object)null && !body.alive)
		{
			return true;
		}
		return false;
	}

	public void Server_DoAlertSingle(in string msg, bool reliable = true)
	{
		Server_DoAlertSingle(in msg, important: false, reliable);
	}

	public void Server_DoAlertSingle(in string msg, bool important, bool reliable)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		NetDataWriter writer = Net.CreateWriter(10006);
		writer.Put(msg);
		writer.Put(important);
		Net.Server_SendToClients((DeliveryMethod)(reliable ? 2 : 4), in writer, clientId);
	}

	public void Server_Kick(string message = "Kicked.")
	{
		if (Net.TryGetSteamTransport(out var _) && KnownPersons.IsSteamUserPrivileged(SteamId))
		{
			return;
		}
		try
		{
			if ((Object)(object)body != (Object)null)
			{
				body.Body_DropAllItems();
			}
		}
		catch (Exception)
		{
		}
		string message2 = "Kicked " + playername;
		Chat.Server_ChatAnnouncement(in message2);
		try
		{
			ServerMain.Server_SendMPLogMessage(message2 + ": " + message, false, new knetid[1] { clientId });
		}
		catch (Exception ex2)
		{
			log.error(ex2.ToString());
		}
		Net.Server_Kick(clientId, message);
	}

	public void Server_ReviveCharacter(bool remind_health_now = true, bool unmindwipe = false)
	{
		if (Util.IsInWorld())
		{
			CreateCharacter();
			if (!body.alive && !KrokoshaScavMultiplayer.rules.RespawnKeepSkills)
			{
				body.ResetMind();
			}
			body.ResetHealth(unmindwipe);
			if (remind_health_now)
			{
				MedicalSync.Server_SendCharacterHealth(playerbody);
			}
		}
	}

	public void Server_RemindPlayersCurrentState(bool keep_velocity = false, bool reliable = true)
	{
		if (TryGetNetBody(out var pb))
		{
			pb.Server_RemindPlayersCurrentState(keep_velocity, reliable);
		}
		else if (log.verbose)
		{
			log.warn($"Attempted to remind body state of {this} but theres no body.");
		}
	}

	public void Server_TeleportCharacter(Vector2 pos)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (Util.IsInWorld())
		{
			CreateCharacter();
			if (is_local)
			{
				UIInGame._DEV_FREECAM_CURPOS = pos;
			}
			playerbody.SetBodyPosition(pos);
			playerbody.last_sync_packet.pos = pos;
			if (!is_local)
			{
				WorldChunkSync.Server_CheckPlrPosChunk(this);
			}
			Server_RemindPlayersCurrentState();
		}
	}

	public void Server_RespawnCharacter(Vector2 pos, bool level_transition = false)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		if (Util.IsInWorld())
		{
			CreateCharacter();
			if (level_transition && !KrokoshaScavMultiplayer.rules.RespawnKeepInventory)
			{
				Server_DropAllInventory();
			}
			if (!KrokoshaScavMultiplayer.rules.RespawnKeepSkills)
			{
				body.ResetMind();
			}
			bool unmindwipe = !KrokoshaScavMultiplayer.rules.RespawnKeepSkills;
			playerbody.StopPiggyback();
			Server_ReviveCharacter(remind_health_now: true, unmindwipe);
			body.ForceStand();
			Server_TeleportCharacter(pos);
		}
	}

	public void Server_DropAllInventory()
	{
		body?.Body_DropAllItems();
	}

	public void Server_HealCharacter()
	{
		Server_ReviveCharacter(remind_health_now: false);
	}

	public bool IsAtTheEndOfLayer()
	{
		if ((Object)(object)body == (Object)null)
		{
			return false;
		}
		return ServerMain.DidHeFinishTheLayer(body);
	}

	public void PointFingerAt(Vector2 pos, byte type)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		if (Object.op_Implicit((Object)(object)body))
		{
			flag = body.conscious;
			body.overrideLookTime = 2.5f;
			body.overrideLookPos = pos;
		}
		is_pointingfingerat = KrokoshaScavMultiplayer.rules.CanCommunicateWithTheDeadTC() || flag;
		is_pointingfingeratTIME = Time.timeAsDouble;
		is_pointingfingeratTARGET = pos;
		is_pointingfingeratTYPE = type;
		SpriteRenderer component = locationPingCircle.GetComponent<SpriteRenderer>();
		switch (type)
		{
		case 0:
			component.sprite = CoopModAssets.circlething;
			break;
		case 1:
			component.sprite = CoopModAssets.alert2;
			break;
		default:
			component.sprite = CoopModAssets.circlething;
			break;
		}
	}

	private void Update()
	{
		if (IsAlive() && KrokoshaScavMultiplayer.is_server && !is_local && minigame_is_in_a_minigame && (Object)(object)minigame_currentItem != (Object)null && ItemSync.CheckIfBodyReachThisItem(minigame_currentItem, body) && minigame_currentItem.IsManualDefibrillator() && minigame_currentItem.battery.hasCharge)
		{
			minigame_currentItem.battery.DrainCharge(Time.deltaTime / 800f);
		}
	}

	private void LateUpdate()
	{
		_UpdateFingerPointVisual();
	}

	private void _UpdateFingerPointVisual()
	{
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		if (is_pointingfingerat && (!Util.IsInWorld() || Time.timeAsDouble - is_pointingfingeratTIME > 3.0))
		{
			is_pointingfingerat = false;
			locationPingCircle.SetActive(false);
			locationPingArrow.SetActive(false);
		}
		if (!((Object)(object)locationPingCircle != (Object)null) || !((Object)(object)locationPingArrow != (Object)null))
		{
			return;
		}
		double num = is_pointingfingeratTIME + Time.realtimeSinceStartupAsDouble;
		SpriteRenderer component = locationPingCircle.GetComponent<SpriteRenderer>();
		float num2 = Mathf.MoveTowards(component.color.a, (float)(is_pointingfingerat ? 1 : 0), Time.deltaTime * 2f);
		bool flag = num2 != 0f;
		if (locationPingCircle.activeSelf != flag)
		{
			locationPingCircle.SetActive(flag);
		}
		if (locationPingCircle.activeSelf)
		{
			Vector2 val = is_pointingfingeratTARGET;
			float num3 = (float)Math.Sin(num * 4.0);
			if (is_pointingfingeratTYPE == 1)
			{
				val += Vector2.up * (4f + num3 * 0.7f);
			}
			locationPingCircle.transform.position = KM.v3v2z(val, -2.601f);
			locationPingCircle.transform.localScale = Vector3.one * (1f + num3 * 0.3f) * 32f * 0.5f * 0.08f;
			component.color = playerColor.ToColorWithAlpha(num2);
		}
		bool flag2 = (Object)(object)body != (Object)null && flag && IsConscious() && locationPingCircle.activeSelf && !KM.dist2dsqrcheck(in is_pointingfingeratTARGET, pos, 8f);
		if (locationPingArrow.activeSelf != flag2)
		{
			locationPingArrow.SetActive(flag2);
		}
		if (locationPingArrow.activeSelf)
		{
			float magnitude;
			Vector2 val2 = KM.normal(in is_pointingfingeratTARGET, Vector2.op_Implicit(((Component)body).transform.position), out magnitude);
			locationPingArrow.transform.position = KM.v3v2z(Vector2.op_Implicit(((Component)body).transform.position) - val2 * 4f + val2 * (float)Math.Sin(num * 4.0) * 1f, -2.6f);
			locationPingArrow.transform.eulerAngles = new Vector3(0f, 0f, Vector2.SignedAngle(Vector2.right, val2) + 180f);
			locationPingArrow.GetComponent<SpriteRenderer>().color = component.color;
		}
	}

	public void DestroyCharacterIfNoLocal()
	{
		if ((Object)(object)body != (Object)null && (Object)(object)body != (Object)(object)PlayerCamera.main.body)
		{
			if ((Object)(object)body == (Object)(object)InvButton_get_body_MultiplayerPatch.focused_body)
			{
				InvButton_get_body_MultiplayerPatch.focused_body = null;
			}
			if ((Object)(object)WoundView.view != (Object)null && (Object)(object)WoundView.view.body == (Object)(object)body)
			{
				WoundView.view.body = PlayerCamera.main.body;
			}
			Server_DropAllInventory();
			BodyToPlayerDict.Remove(body);
			NetBody.DestroyNPC(body);
			body = null;
		}
	}

	public void OnDestroy()
	{
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (Util.IsInWorld() && (Object)(object)body != (Object)null && KrokoshaScavMultiplayer.rules.DisconnectShouldSaveAnything)
			{
				ServerMain.server_lastplayerstates[GetPersistentId()] = new PlayerSavedState(body);
			}
		}
		catch (Exception ex)
		{
			log.error("NetPlayer.OnDestroy: saving plr state: " + ex.ToString());
		}
		if (KrokoshaScavMultiplayer.is_server)
		{
			PlrSync.inst.Server_DeleteObject(clientId);
		}
		ClientIdToPlayerDict.Remove(clientId);
		ServerMain._UpdateSpecialPlayerLists();
		SharedMain.ForceUpdatePlayerLists();
		try
		{
			NetPlayer.OnPlayerLeft?.Invoke(this);
		}
		catch (Exception ex2)
		{
			log.error("NetPlayer.OnDestroy: OnPlayerLeft event: " + ex2.ToString());
		}
		try
		{
			if (Net.is_server && Net.running)
			{
				NetDataWriter writer = Net.CreateWriter(10170);
				writer.Put((ushort)clientId);
				writer.Put(SteamId);
				Net.Server_SendToClients((DeliveryMethod)2, in writer, (IEnumerable<knetid>)ServerMain.AllClientIdsExceptHost);
				string msg = playername + " disconnected.";
				KrokoshaScavMultiplayer.DoMultiplayerStatusMessageLog(msg);
				Chat.LogMessage("*SERVER*", msg, false);
				if (!is_local)
				{
					try
					{
						ServerMain._Server_OnServerNameChange(new List<knetid> { clientId });
					}
					catch (Exception ex3)
					{
						log.error(ex3.ToString());
					}
				}
			}
		}
		catch (Exception ex4)
		{
			log.error("NetPlayer.OnDestroy: disconnect handling: " + ex4.ToString());
		}
		DestroyCharacterIfNoLocal();
		if (Object.op_Implicit((Object)(object)locationPingCircle))
		{
			Object.Destroy((Object)(object)locationPingCircle);
		}
		if (Object.op_Implicit((Object)(object)locationPingArrow))
		{
			Object.Destroy((Object)(object)locationPingArrow);
		}
	}
}
