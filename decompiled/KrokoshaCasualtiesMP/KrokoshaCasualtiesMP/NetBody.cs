using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using KrokoshaCasualtiesUtils;
using LiteNetLib;
using LiteNetLib.Utils;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public class NetBody : MonoBehaviour
{
	public double _lasttime_nobraindamage_from_circulation = 100000000.0;

	public knetid netId;

	public double cpr_last_press_time;

	public int cpr_cur_perfect_press_count;

	public bool chip_punishment_protocol;

	public float irradiateIntensity;

	public float timeSinceRadUpdate;

	public static readonly List<NetBody> all_instances = new List<NetBody>();

	private static readonly Dictionary<knetid, NetBody> _PRIV_NetIdToNetBody = new Dictionary<knetid, NetBody>();

	public byte butchering_dropmeat_skip_counter;

	public bool _SERVER_healthsync_skip_frame;

	public double _last_sync_health_packet_receive_time;

	public double _last_sync_packet_receive_time;

	public NetBodySyncPacket last_sync_packet = new NetBodySyncPacket
	{
		standing = true,
		pos = (Vector2_4byte_512)new Vector2(0f, 0f),
		targetLookPos = (Vector2_4byte_512)new Vector2(0f, 0f),
		moveDir = new Vector2(0f, 0f)
	};

	private string _cur_bodyname = "EMPTY_STRING";

	protected Color24 _color;

	private static knetid _npc_counter_thingy = (ushort)999;

	public bool unchipped;

	public bool idle_anim_allowed;

	public bool useiteminhand;

	public bool[] localoverride_showInfection;

	public static float DistanceForNearBodies = 40f;

	public List<NetBody> NearBodies = new List<NetBody>();

	public bool NearBodiesIs5;

	protected bool last_alive = true;

	protected bool last_sleep;

	public const float default_body_temperature = 37f;

	public float last_mood;

	private bool prev_useiteminhand;

	public const float KG_to_encumburenceUnit = 0.17f;

	public NetBody piggybacking_on;

	public NetBody carrying_person;

	private static float PUSH_STAMINA_USE_MULTIPLIER = 1f;

	public bool is_local => Util.IsBodyLocal(body);

	public string playername => plr?.playername ?? bodyname ?? netId.ToString();

	public static IReadOnlyDictionary<knetid, NetBody> NetIdToNetBody => _PRIV_NetIdToNetBody;

	public bool is_player => (Object)(object)plr != (Object)null;

	public NetPlayer plr { get; internal set; }

	public NetPlayer player => plr;

	public CharStatusVisuals visual { get; protected set; }

	public Rigidbody2D rb => body.rb;

	public string bodyname
	{
		get
		{
			return _cur_bodyname;
		}
		set
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			ApplyNameAndColor(value, color);
		}
	}

	public Color color
	{
		get
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (is_player)
			{
				return plr.plrcolor;
			}
			return _color;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			_color = value;
		}
	}

	public bool alive => body.alive;

	public bool exercising => body.exercising;

	public Body body { get; private set; }

	public GameObject chara => ((Component)((Component)this).transform.parent).gameObject;

	public Limb head => body.limbs[0];

	public double timeOfDeath { get; protected set; }

	public double timeHasBeenDead => Time.unscaledTimeAsDouble - timeOfDeath;

	public Vector2 position => Vector2.op_Implicit(((Component)this).transform.position);

	public Vector2 pos
	{
		get
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			if (!body.standing)
			{
				return Vector2.op_Implicit(((Component)body.limbs[1]).transform.position);
			}
			return Vector2.op_Implicit(((Component)body).transform.position);
		}
	}

	public static event Action<NetPlayer> OnPlayerSleep;

	public static event Action<NetPlayer> OnPlayerDeath;

	public override string ToString()
	{
		if ((Object)(object)this == (Object)null)
		{
			return "NB(NULL)";
		}
		return (is_player ? "PB" : "NB") + $"(ID:{netId}, name:{playername})";
	}

	private void Awake()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		body = ((Component)this).GetComponent<Body>();
		body.moveDir = Vector2.zero;
		body.crouching = false;
		localoverride_showInfection = new bool[body.limbs.Length];
		visual = ComponentHolderProtocol.GetOrAddComponent<CharStatusVisuals>((Object)(object)this);
		idle_anim_allowed = true;
		plr = NetPlayer.GetNetPlayerFromBody(body);
		if ((Object)(object)plr != (Object)null)
		{
			OnFoundNetPlayerInitFinish();
		}
	}

	internal void OnFoundNetPlayerInitFinish()
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		if (Net.running)
		{
			netId = plr.clientId;
		}
		_PRIV_NetIdToNetBody[netId] = this;
		plr.body = body;
		ApplyNameAndColor(playername, plr.plrcolor);
		unchipped = plr.unchipped;
		NetPlayer.BodyToPlayerDict[body] = plr;
	}

	public void BackupLocalPlayerInit()
	{
		if ((Object)(object)plr == (Object)null && Net.running && Net.is_client_or_host)
		{
			plr = NetPlayer.LOCAL_PLAYER;
			if ((Object)(object)plr != (Object)null)
			{
				OnFoundNetPlayerInitFinish();
			}
		}
	}

	protected void Start()
	{
		if ((Object)(object)plr == (Object)null && is_local)
		{
			BackupLocalPlayerInit();
		}
		((MonoBehaviour)this).InvokeRepeating("SlowerUpdate", Random.value * 2f, 0.1f);
		((TMP_Text)body.talker.text).richText = false;
		if (Net.is_server && CharSync.inst != null)
		{
			CharSync.inst.Server_NewObject(this, netId);
		}
	}

	protected void OnDeath()
	{
		ServerMain._ded_server_switch_counter = 0f;
		try
		{
			if ((Object)(object)plr != (Object)null)
			{
				ServerMain.OnPlayerDeath(plr);
				if (KrokoshaScavMultiplayer.is_server)
				{
					MedicalSync.Server_QueueSendCharacterHealth(this, force: true);
				}
				plr.minigame_current_type = byte.MaxValue;
				plr.minigame_currentItem = null;
				NetBody.OnPlayerDeath?.Invoke(plr);
				plr.locationPingArrow.SetActive(false);
				plr.locationPingCircle.SetActive(false);
			}
		}
		catch (Exception ex)
		{
			log.error(ex.ToString());
		}
		timeOfDeath = Time.unscaledTimeAsDouble;
	}

	protected void OnSleep()
	{
		if (!KrokoshaScavMultiplayer.rules.DisableSleep && (Object)(object)plr != (Object)null)
		{
			ServerMain.OnPlayerSleep(plr);
			NetBody.OnPlayerSleep?.Invoke(plr);
		}
	}

	private void Update()
	{
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		if (timeSinceRadUpdate > 0.05f)
		{
			irradiateIntensity -= Time.unscaledDeltaTime * ((irradiateIntensity > 1f) ? 2f : 0.5f);
		}
		timeSinceRadUpdate += Time.unscaledDeltaTime;
		if ((Object)(object)plr == (Object)null && KrokoshaScavMultiplayer.network_system_is_running && is_local)
		{
			BackupLocalPlayerInit();
			if (log.verbose)
			{
				log.l("Still searching for local SCI wtf");
			}
		}
		if (!idle_anim_allowed || (Object)(object)carrying_person != (Object)null)
		{
			if (body.idleTime + Time.deltaTime > 11f)
			{
				body.idleTime = 11f;
			}
			if (body.bodyAnimator.GetCurrentAnimatorClipInfo(0).Length != 0 && ((Object)((AnimatorClipInfo)(ref body.bodyAnimator.GetCurrentAnimatorClipInfo(0)[0])).clip).name == "ExperimentSit")
			{
				body.bodyAnimator.Play("Grounded");
				body.standLerpTime = 0f;
			}
			if (body.armsAnimator.GetCurrentAnimatorClipInfo(0).Length != 0 && ((Object)((AnimatorClipInfo)(ref body.armsAnimator.GetCurrentAnimatorClipInfo(0)[0])).clip).name == "ArmsSit")
			{
				body.armsAnimator.Play("Grounded");
			}
		}
		if (!Util.IsBodyLocal(body) && body.conscious)
		{
			Item item = body.GetItem(body.handSlot);
			if (prev_useiteminhand)
			{
				if (!Object.op_Implicit((Object)(object)item) || !item.Stats.usableWithLMB || item.Stats.autoAttack)
				{
					body.UseItemInHand();
				}
			}
			else if (prev_useiteminhand != useiteminhand && (!Object.op_Implicit((Object)(object)item) || !item.Stats.usableWithLMB || !item.Stats.autoAttack))
			{
				body.UseItemInHand();
			}
			prev_useiteminhand = useiteminhand;
		}
		if (!body.alive)
		{
			body.halfMinuteCheckTime = 0f;
		}
		if (KrokoshaScavMultiplayer.is_server)
		{
			if (chip_punishment_protocol)
			{
				body.eyePanicTime = 1f;
				Body obj = body;
				obj.radiationSickness += Time.deltaTime;
			}
			if (!SharedMain.local_world_is_generated)
			{
				last_sync_packet.standing = true;
				last_sync_packet.moveDir = Vector2.zero;
				last_sync_packet.velocity = Vector2.zero;
				last_sync_packet.is_piggyback = false;
			}
			if (!CharSync.inst.IsRegisteted(netId))
			{
				CharSync.inst.Server_NewObject(this, netId);
			}
		}
	}

	protected void LateUpdate()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (KrokoshaScavMultiplayer.is_server && !SharedMain.local_world_is_generated)
		{
			last_sync_packet.standing = true;
			last_sync_packet.moveDir = Vector2.zero;
			last_sync_packet.velocity = Vector2.zero;
			last_sync_packet.is_piggyback = false;
		}
		if (!last_alive && !body.alive)
		{
			body.temperature = Mathf.Lerp(36.6f, WorldGeneration.world.ambientTemperature, Mathf.Clamp01((float)((Time.timeAsDouble - timeOfDeath) / 30.0)));
			body.reversedControls = false;
		}
		else
		{
			if (!SharedMain.local_world_is_generated)
			{
				return;
			}
			if (body.happiness < last_mood && NearBodies.Count > 0)
			{
				int num = 0;
				foreach (NetBody nearBody in NearBodies)
				{
					if (nearBody.body.happiness > body.happiness)
					{
						num++;
					}
				}
				float num2 = Mathf.Min((float)num / 8f, 0.5f);
				body.happiness = Mathf.Lerp(body.happiness, last_mood, num2);
			}
			last_mood = body.happiness;
		}
		_ = NearBodiesIs5;
	}

	public void HearinglossDistortMessage(NetBody yapper, ref string message, ref string chattag)
	{
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		string distortionChars = GlobalDark.main.distortionChars;
		StringBuilder stringBuilder = new StringBuilder(message);
		if (KrokoshaScavMultiplayer.rules.HearingLossChat)
		{
			float num = 0f;
			num = ((!Voicechat.VCRULE_mindwiperule || !body.IsMindwiped()) ? Mathf.Max(num, body.brainHealth.RemapClamped(85f, 0f, 0f, 0.8f)) : 0.94f);
			if (num > 0f)
			{
				for (int i = 0; i < message.Length; i++)
				{
					if (!char.IsWhiteSpace(stringBuilder[i]) && Random.value < num)
					{
						stringBuilder[i] = distortionChars[Random.Range(0, distortionChars.Length)];
					}
				}
			}
		}
		bool flag = KrokoshaScavMultiplayer.rules.OnlyProximityChat || (KrokoshaScavMultiplayer.rules.UnchippedProximityChat && unchipped);
		float num2 = 1.1f;
		float num3 = 10f;
		if (flag)
		{
			num3 = Vector2.Distance(yapper.pos, pos);
			if (num3 > Voicechat.MAX_HEAR_DISTANCE)
			{
				message = "";
				return;
			}
			num2 = Mathf.Clamp01(Body.Remap(num3, Voicechat.MAX_HEAR_DISTANCE * 0.4f, Voicechat.MAX_HEAR_DISTANCE, 1.01f, 0.001f));
		}
		if (KrokoshaScavMultiplayer.rules.HearingLossChat && body.hearingLoss > 30f)
		{
			num2 = Mathf.Min(num2, Mathf.Clamp01(Body.Remap(body.hearingLoss, 30f, 100f, 1f, 0f)));
		}
		if (num2 < 1f)
		{
			for (int j = 0; j < message.Length; j++)
			{
				if (Chat.CharCanBeReplacedWithBlank(stringBuilder[j]) && Random.value > num2)
				{
					stringBuilder[j] = '_';
				}
			}
		}
		message = stringBuilder.ToString();
		if (string.IsNullOrWhiteSpace(chattag) && flag && num3 > 60f)
		{
			chattag = Lang.MarkMsgAsLocaleKey("plr_chattag_far");
		}
	}

	public void DoMouseHoverTooltip(ref string tooltipName_text, ref string tooltipDescription_text)
	{
		tooltipName_text = bodyname;
		if (!body.alive)
		{
			tooltipName_text += Lang.Get("plr_hover_dead", false);
		}
		else if (body.sleeping)
		{
			tooltipName_text += Lang.Get("plr_hover_sleeping", false);
		}
		else if (!body.conscious)
		{
			tooltipName_text += Lang.Get("plr_hover_unconscious", false);
		}
		if (log.verbose)
		{
			if (is_player)
			{
				string obj = tooltipName_text;
				knetid knetid2 = netId;
				tooltipName_text = obj + " (PLR ID: " + knetid2.ToString() + " )";
			}
			else
			{
				string obj2 = tooltipName_text;
				knetid knetid2 = netId;
				tooltipName_text = obj2 + " (NPC ID: " + knetid2.ToString() + " )";
			}
			tooltipDescription_text = "";
			if (is_player)
			{
				tooltipDescription_text = "\n!!! " + (KrokoshaScavMultiplayer.is_client ? "CLIENT" : "SERVER") + " looking at " + (((ushort)netId != 0) ? "CLIENT" : "SERVER") + "\n";
			}
			tooltipDescription_text = tooltipDescription_text + "body.alive: " + body.alive + "\n";
			tooltipDescription_text = tooltipDescription_text + "body.conscious: " + body.conscious + "\n";
			tooltipDescription_text = tooltipDescription_text + "body.standing: " + body.standing + "\n";
			tooltipDescription_text = tooltipDescription_text + "body.isCriticallyDying: " + body.isCriticallyDying + "\n";
			tooltipDescription_text = tooltipDescription_text + "body.isDying: " + body.isDying + "\n";
			if (is_player)
			{
				Type minigame;
				if (plr.minigame_current_type == byte.MaxValue)
				{
					tooltipDescription_text += "plr.minigame: None (255)\n";
				}
				else if (MinigameMPManager.TryGetMinigameTypeFromId(plr.minigame_current_type, out minigame))
				{
					tooltipDescription_text += $"plr.minigame: {minigame.Name}  ({plr.minigame_current_type})\n";
				}
				else
				{
					tooltipDescription_text += "plr.minigame: UNKNOWN  ({this.plr.minigame_current_type})\n";
				}
				tooltipDescription_text = tooltipDescription_text + "pointing: " + plr.is_pointingfingerat + "\n";
				tooltipDescription_text = tooltipDescription_text + "healing: " + plr.woundViewTargetNetBodyId + "\n";
			}
			tooltipDescription_text = tooltipDescription_text + "carrying: " + ((object)carrying_person)?.ToString() + "\n";
			tooltipDescription_text = tooltipDescription_text + "piggyback: " + ((object)piggybacking_on)?.ToString() + "\n";
			tooltipDescription_text = tooltipDescription_text + "piggyback stack: " + CountPiggybackStackDown() + "\n";
			tooltipDescription_text = tooltipDescription_text + "temperature: " + Math.Round(body.temperature, 2) + "\n";
			if (is_player)
			{
				if (KrokoshaScavMultiplayer.is_server)
				{
					tooltipDescription_text = tooltipDescription_text + "did_give_spawn_location: " + plr.server_plrstate.did_give_spawn_location + "\n";
					tooltipDescription_text = tooltipDescription_text + "is_loaded_in: " + plr.server_plrstate.is_loaded_in + "\n";
				}
				tooltipDescription_text = tooltipDescription_text + "exist_time: " + (int)plr.levelPlayTime + "\n";
				if (plr.minigame_is_in_a_minigame)
				{
					tooltipDescription_text = tooltipDescription_text + "mg_session: " + plr.minigame_session + "\n";
					string obj3 = tooltipDescription_text;
					AnyObjectNetId minigame_targetobject = plr.minigame_targetobject;
					tooltipDescription_text = obj3 + "mg_obj: " + minigame_targetobject.ToString() + "\n";
					tooltipDescription_text = tooltipDescription_text + "mg_item: " + ((object)plr.minigame_currentItem)?.ToString() + "\n";
				}
			}
		}
		else
		{
			tooltipDescription_text = "<sprite index=20>Interact.";
		}
	}

	public void CameraShakeOverrideFunc(float num)
	{
		if (body.IsBodyLocal() && body.conscious)
		{
			PlayerCamera.main.shaker.Shake(num);
		}
	}

	private void OnDestroy()
	{
		_PRIV_NetIdToNetBody.Remove(netId);
		NetPlayer.BodyToPlayerDict.Remove(body);
		if ((Object)(object)MoodleManager.main != (Object)null && (Object)(object)MoodleManager.main.body == (Object)(object)body)
		{
			MoodleManager.main.body = PlayerCamera.main.body;
		}
		if (Net.is_server)
		{
			CharSync.inst.Server_DeleteObject(netId);
		}
	}

	public static (NetBody, float) GetNearestBody(Vector2 pos, bool must_be_alive = true, bool must_be_conscious = false)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		float num = float.PositiveInfinity;
		NetBody item = null;
		foreach (NetBody all_instance in all_instances)
		{
			if ((!must_be_alive || all_instance.body.alive) && (!must_be_conscious || all_instance.body.conscious))
			{
				float num2 = KM.dist2dsqr(Vector2.op_Implicit(((Component)all_instance).transform.position), in pos);
				if (num2 < num)
				{
					num = num2;
					item = all_instance;
				}
			}
		}
		return (item, num);
	}

	public static (NetBody, float) GetNearestConsciousBodyToThisBody(NetBody thisone)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		float num = float.PositiveInfinity;
		NetBody item = null;
		foreach (NetBody all_instance in all_instances)
		{
			if ((all_instance.body.conscious || all_instance.body.sleeping) && (Object)(object)thisone != (Object)(object)all_instance)
			{
				float num2 = KM.dist2dsqr(Vector2.op_Implicit(((Component)all_instance).transform.position), Vector2.op_Implicit(((Component)thisone).transform.position));
				if (num2 < num)
				{
					num = num2;
					item = all_instance;
				}
			}
		}
		return (item, num);
	}

	public static void GetBodiesInRadius(Vector2 pos, float radius, List<NetBody> list)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		radius *= radius;
		foreach (NetBody all_instance in all_instances)
		{
			if (KM.dist2dsqrcheck_presqr(Vector2.op_Implicit(((Component)all_instance).transform.position), in pos, radius))
			{
				list.Add(all_instance);
			}
		}
	}

	public static List<NetBody> GetBodiesInRadius(Vector2 pos, float radius)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		radius *= radius;
		List<NetBody> list = new List<NetBody>();
		foreach (NetBody all_instance in all_instances)
		{
			if (KM.dist2dsqrcheck_presqr(Vector2.op_Implicit(((Component)all_instance).transform.position), in pos, radius))
			{
				list.Add(all_instance);
			}
		}
		return list;
	}

	public static NetBody GetNetBodyFromId(knetid netId)
	{
		return GeneralExtensions.GetValueSafe<knetid, NetBody>(_PRIV_NetIdToNetBody, netId);
	}

	public static bool TryGetNetBodyFromId(knetid netId, out NetBody nb)
	{
		return _PRIV_NetIdToNetBody.TryGetValue(netId, out nb);
	}

	private void OnEnable()
	{
		all_instances.Add(this);
	}

	private void OnDisable()
	{
		all_instances.Remove(this);
	}

	public void SetNetIgnoreTime(float additionaltime = 0f)
	{
		if (KrokoshaScavMultiplayer.is_client)
		{
			float num = ClientMain.RTT_IN_SECONDS_with_margin;
			if (is_player && !plr.is_local)
			{
				num += (float)plr.rtt_in_seconds_with_margin;
			}
			_last_sync_packet_receive_time = Time.realtimeSinceStartupAsDouble + Math.Min(4.0, num + additionaltime);
		}
		else if (is_player)
		{
			_last_sync_packet_receive_time = Time.realtimeSinceStartupAsDouble + Math.Min(4.0, plr.rtt_in_seconds_with_margin + (double)additionaltime);
		}
		else
		{
			_last_sync_packet_receive_time = Time.realtimeSinceStartupAsDouble + Math.Min(4.0, additionaltime);
		}
	}

	public void SetNetHealthSyncIgnoreTime(float additionaltime = 0f)
	{
		if (KrokoshaScavMultiplayer.is_client)
		{
			float num = ClientMain.RTT_IN_SECONDS_with_margin;
			if (is_player && !plr.is_local)
			{
				num += (float)plr.rtt_in_seconds_with_margin;
			}
			_last_sync_health_packet_receive_time = Math.Max(_last_sync_health_packet_receive_time, Time.realtimeSinceStartupAsDouble + (double)num + (double)additionaltime);
		}
		else if (is_player)
		{
			_last_sync_health_packet_receive_time = Math.Max(_last_sync_health_packet_receive_time, Time.realtimeSinceStartupAsDouble + plr.rtt_in_seconds_with_margin + (double)additionaltime);
		}
		else
		{
			_last_sync_health_packet_receive_time = Math.Max(_last_sync_health_packet_receive_time, Time.realtimeSinceStartupAsDouble + (double)additionaltime);
		}
	}

	public void ApplyNameAndColor(string name, Color color)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		_cur_bodyname = name;
		this.color = color;
		((Object)body).name = "Body_" + bodyname;
		visual.ApplyNameAndColor();
	}

	private static knetid GetNextNPCBodyId()
	{
		knetid knetid2 = (ushort)0;
		while ((ushort)knetid2 < ushort.MaxValue)
		{
			_npc_counter_thingy = (ushort)((ushort)_npc_counter_thingy + 1);
			if ((ushort)_npc_counter_thingy > 65530)
			{
				_npc_counter_thingy = (ushort)1000;
			}
			if (all_instances.All((NetBody x) => (ushort)x.netId != (ushort)_npc_counter_thingy))
			{
				return _npc_counter_thingy;
			}
			knetid2 = (ushort)((ushort)knetid2 + 1);
		}
		throw new Exception("How many NPCs did u spawn bro?????????????");
	}

	public static NetBody CreateNewNPC(Vector2 pos, string name = null)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		knetid nextNPCBodyId = GetNextNPCBodyId();
		NetBody netBody = _Internal_CreateNetBody(nextNPCBodyId, pos, name);
		((Object)netBody.chara).name = $"NPC_ID_{nextNPCBodyId}_NAME_{name}";
		netBody.bodyname = name;
		netBody.netId = nextNPCBodyId;
		return netBody;
	}

	internal static NetBody CreateNewPlayerCharacter(NetPlayer plr)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)plr.body == (Object)null)
		{
			NetBody netBody = _Internal_CreateNetBody(plr.clientId, Body_PlaceBody_MultiplayerPatch.spawnlocation, plr.playername);
			plr.body = netBody.body;
			plr.body.targetLookPos = Vector2.op_Implicit(new Vector2(1000f, 460f));
			NetPlayer.BodyToPlayerDict.Add(plr.body, plr);
			netBody.plr = plr;
			netBody.netId = plr.clientId;
			netBody.last_sync_packet.pos = ((Component)plr.body).transform.position;
			netBody.ApplyNameAndColor(plr.playername, plr.plrcolor);
			if (KrokoshaScavMultiplayer.is_server)
			{
				if (plr.late_joined && KrokoshaScavMultiplayer.rules.LateJoinSpectate)
				{
					string persistentId = plr.GetPersistentId();
					if (!ServerMain.server_lastplayerstates.TryGetValue(persistentId, out var _))
					{
						if (log.verbose)
						{
							log.l($"Killing {plr} cuz he late-joined and LateJoinSpectate is on.");
						}
						plr.body.GetHead().Dismember();
						plr.body.GetLowerTorso().Dismember();
						plr.body.brainHealth = 0f;
						plr.body.heartRate = 0f;
						plr.body.bloodOxygen = 0f;
						plr.body.bloodPressure = 0f;
						MedicalSync.Server_QueueSendCharacterHealth(netBody, force: true);
						Util.CallLambdaWhen(() => !Util.IsInWorld() || plr.server_plrstate.is_loaded_in, (Action)delegate
						{
							Util.DelayCallLambda(1f, (Action)delegate
							{
								KrokoshaScavMultiplayer.Server_SendSimpleMessageToOneClient(NetmsgId.CLIENT_forcespectator, plr.clientId);
								plr.Server_DoAlertSingle("Late join, server put you in spectator mode.", important: true, reliable: true);
							});
						});
					}
				}
				Body val = plr.body;
				if (WorldGeneration.world.totalTraveled <= 0 && (int)WorldGeneration.world.biomeOverride == 0 && !SaveSystem.loadedRun)
				{
					switch (WorldGeneration.GetRunSettingInt("startingsupplies"))
					{
					case 1:
						val.PickUpItem(Utils.Create("emergencylight", Vector2.op_Implicit(((Component)val).transform.position), 0f).GetComponent<Item>(), 3, true);
						break;
					case 2:
						val.PickUpItem(Utils.Create("lantern", Vector2.op_Implicit(((Component)val).transform.position), 0f).GetComponent<Item>(), 3, true);
						val.PickUpItem(Utils.Create("dogfood", Vector2.op_Implicit(((Component)val).transform.position), 0f).GetComponent<Item>(), 4, true);
						val.PickUpItem(Utils.Create("waterbottle", Vector2.op_Implicit(((Component)val).transform.position), 0f).GetComponent<Item>(), 5, true);
						val.PickUpItem(Utils.Create("trashbag", Vector2.op_Implicit(((Component)val).transform.position), 0f).GetComponent<Item>(), 1, true);
						break;
					}
					if (DateTime.Now.Month == 12)
					{
						Utils.Create("present", Vector2.op_Implicit(((Component)val).transform.position) + Vector2.right * Random.Range(-2f, 2f), 0f);
					}
				}
			}
			return netBody;
		}
		return plr.playerbody;
	}

	internal static NetBody _Internal_CreateNetBody(knetid netid, Vector2 pos, string name = null)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		if (name == null)
		{
			name = "bot_" + netid.ToString();
		}
		GameObject obj = Object.Instantiate<GameObject>(ServerMain.CHARACTER_PREFAB, Vector3.zero, Quaternion.identity);
		((Object)obj).name = "Character_" + name;
		obj.SetActive(true);
		Body componentInChildren = obj.GetComponentInChildren<Body>();
		((Object)componentInChildren).name = "Body_" + name;
		componentInChildren.targetLookPos = Vector2.op_Implicit(new Vector2(1000f, 460f));
		((Component)componentInChildren).transform.position = Vector2.op_Implicit(pos);
		NetBody orAddComponent = ComponentHolderProtocol.GetOrAddComponent<NetBody>((Object)(object)componentInChildren);
		orAddComponent.netId = netid;
		_PRIV_NetIdToNetBody[netid] = orAddComponent;
		return orAddComponent;
	}

	public static void DestroyNPC(NetBody npc)
	{
		Object.Destroy((Object)(object)((Component)((Component)npc).transform.parent).gameObject);
	}

	public static void DestroyNPC(Body npc)
	{
		Object.Destroy((Object)(object)((Component)((Component)npc).transform.parent).gameObject);
	}

	public bool IsBodyLocal()
	{
		return body.IsBodyLocal();
	}

	public Vector2 GetPosition()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return Vector2.op_Implicit(((Component)this).transform.position);
	}

	public Vector2 GetHeadPos()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		return Vector2.op_Implicit(((Component)body.limbs[0]).transform.position);
	}

	private void SlowerUpdate()
	{
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < localoverride_showInfection.Length; i++)
		{
			if (!body.limbs[i].infected)
			{
				localoverride_showInfection[i] = false;
			}
			if (body.limbs[i].infectionAmount > 25f)
			{
				localoverride_showInfection[i] = true;
			}
		}
		if (KrokoshaScavMultiplayer.is_server)
		{
			if (this != null && is_player)
			{
				if (body.alive)
				{
					chip_punishment_protocol = SharedMain.CheckIfDispersionPunishmentProtocolRuleIsActive();
					if (chip_punishment_protocol)
					{
						chip_punishment_protocol = SharedMain.CheckIfShouldActivateDispersionPunishmentProtocol(plr, out var _, out var _, out var _);
					}
				}
				else
				{
					chip_punishment_protocol = false;
				}
			}
		}
		NearBodies.Clear();
		float max_squared = DistanceForNearBodies * DistanceForNearBodies;
		foreach (NetBody all_instance in all_instances)
		{
			if (all_instance != null && all_instance.is_player && (Object)(object)all_instance != (Object)(object)this && all_instance.alive && KM.dist2dsqrcheck_presqr(Vector2.op_Implicit(((Component)all_instance).transform.position), Vector2.op_Implicit(((Component)this).transform.position), max_squared))
			{
				NearBodies.Add(all_instance);
			}
		}
		UpdateBodySleepShare();
		UpdateBodyTempShare();
		if (body.alive != last_alive)
		{
			butchering_dropmeat_skip_counter = 0;
			FacialExpression val = default(FacialExpression);
			if (((Component)head).TryGetComponent<FacialExpression>(ref val))
			{
				((Behaviour)val.eyeLight).enabled = body.alive;
			}
			if (!body.alive && last_alive)
			{
				OnDeath();
			}
			else
			{
				if (Util.IsBodyLocal(body))
				{
					UIInGame.StopSpectatorMode();
				}
				CoUtils coUtilsInstance = body.GetCoUtilsInstance();
				if (coUtilsInstance != null)
				{
					coUtilsInstance.CancelAll();
				}
				if (KrokoshaScavMultiplayer.is_client && !KrokoshaScavMultiplayer.rules.RespawnKeepSkills)
				{
					body.ResetMind();
				}
			}
		}
		if (!KrokoshaScavMultiplayer.rules.DisableSleep && body.sleeping && !last_sleep)
		{
			OnSleep();
		}
		if (IsBodyLocal())
		{
			unchipped = Util.IsUnchipped();
		}
		last_sleep = body.sleeping;
		last_alive = body.alive;
	}

	private void UpdateBodySleepShare()
	{
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		if (KrokoshaScavMultiplayer.rules.DisableSleep || !body.alive || !body.sleeping || NearBodies.Count == 0)
		{
			return;
		}
		NearBodiesIs5 = false;
		float num = 1f / (float)NearBodies.Count;
		foreach (NetBody nearBody in NearBodies)
		{
			if (nearBody.body.energy - 5f > body.energy)
			{
				body.energy = Mathf.MoveTowards(body.energy, nearBody.body.energy, 0.5f * num);
				if (KM.dist2dsqrcheck_presqr(Vector2.op_Implicit(((Component)this).transform.position), Vector2.op_Implicit(((Component)nearBody).transform.position), 30f))
				{
					NearBodiesIs5 = true;
				}
			}
		}
	}

	private void UpdateBodyTempShare()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (!body.alive)
		{
			return;
		}
		BoxCollider2D component = ((Component)body).GetComponent<BoxCollider2D>();
		Collider2D[] array = Physics2D.OverlapBoxAll(Vector2.op_Implicit(((Component)component).transform.position), component.size, 0f);
		Limb val2 = default(Limb);
		foreach (Collider2D obj in array)
		{
			Body val = null;
			if (((Component)obj).TryGetComponent<Limb>(ref val2))
			{
				val = val2.body;
			}
			if ((Object)(object)val != (Object)null && (Object)(object)val != (Object)(object)body && val.alive && val.temperature > 20f)
			{
				float num = val.temperature - 37f;
				float num2 = body.temperature - 37f;
				float num3 = Math.Abs(num);
				float num4 = ((Math.Abs(num2) > num3) ? num : num2);
				float num5 = Mathf.Lerp(Mathf.Lerp(num, num2, 0.5f), num4, 0.33f) + 37f;
				val.temperature = Mathf.MoveTowards(val.temperature, num5, 0.012f);
				body.temperature = Mathf.MoveTowards(body.temperature, num5, 0.012f);
				break;
			}
		}
	}

	public void SetBodyPosition(Vector2 pos)
	{
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		if (!body.standing)
		{
			Rigidbody2D val = body.limbs[1].rb;
			List<(Vector2, Rigidbody2D)> list = new List<(Vector2, Rigidbody2D)>();
			Limb[] limbs = body.limbs;
			foreach (Limb val2 in limbs)
			{
				list.Add((val2.rb.position, val2.rb));
			}
			Vector2 val3 = pos - val.position;
			val.position = pos;
			{
				foreach (var item in list)
				{
					item.Item2.position = item.Item1 + val3;
				}
				return;
			}
		}
		((Component)body).transform.position = Vector2.op_Implicit(pos);
	}

	private void FixedUpdate()
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		HandlePiggybackUpdate();
		if (Net.running)
		{
			if (body.baseLimb.dismembered || ClientMain.IsInTemporaryWorldPause() || ((Component)body).transform.position.y > 6000f)
			{
				body.rb.velocity = Vector2.zero;
				body.rb.gravityScale = 0f;
			}
			else if (SharedMain.local_world_is_generating && !WorldgenPatches.client_technically_finished_worldgen_now_just_waiting_for_server)
			{
				body.ForceStand();
				body.rb.velocity = Vector2.zero;
				body.rb.gravityScale = 0f;
			}
		}
		if (!IsBodyLocal())
		{
			if (Time.realtimeSinceStartupAsDouble - _last_sync_packet_receive_time > 0.054999999701976776)
			{
				body.moveDir = Vector2.zero;
			}
			else if (Mathf.Abs(last_sync_packet.moveDir.x) < 0.1f)
			{
				body.moveDir = last_sync_packet.moveDir;
			}
			else if (Mathf.Abs(body.moveDir.x) > Mathf.Abs(last_sync_packet.moveDir.x) && Mathf.Sign(body.moveDir.x) == Mathf.Sign(last_sync_packet.moveDir.x))
			{
				body.moveDir = Vector2.MoveTowards(body.moveDir, last_sync_packet.moveDir, Time.fixedDeltaTime * 10f);
			}
			else
			{
				body.moveDir = Vector2.MoveTowards(body.moveDir, last_sync_packet.moveDir, Time.fixedDeltaTime * 30f);
			}
			body.IsRagdolling();
		}
	}

	public bool CanAttackThisGuy(Body target)
	{
		NetBody component = ((Component)target).GetComponent<NetBody>();
		if ((Object)(object)component != (Object)null)
		{
			if ((Object)(object)carrying_person == (Object)(object)component)
			{
				return false;
			}
			if ((Object)(object)piggybacking_on == (Object)(object)component)
			{
				return false;
			}
		}
		if (!target.alive)
		{
			return true;
		}
		if (!KrokoshaScavMultiplayer.rules.PVP)
		{
			return false;
		}
		if ((Object)(object)target == (Object)(object)body)
		{
			return false;
		}
		if ((Object)(object)component != (Object)null && component.is_player)
		{
			if (component.player.levelPlayTime < 1.0)
			{
				return false;
			}
			if (is_player && KrokoshaScavMultiplayer.rules.Teams && component.plr.plrcolor == plr.plrcolor)
			{
				return false;
			}
		}
		return true;
	}

	public int CountPiggybackStackUp()
	{
		NetBody netBody = this;
		int i;
		for (i = 0; i < 1000; i++)
		{
			if (!((Object)(object)netBody.carrying_person != (Object)null))
			{
				break;
			}
			netBody = netBody.carrying_person;
		}
		return i;
	}

	public int CountPiggybackStackDown()
	{
		NetBody netBody = this;
		int i;
		for (i = 0; i < 1000; i++)
		{
			if (!((Object)(object)netBody.piggybacking_on != (Object)null))
			{
				break;
			}
			netBody = netBody.piggybacking_on;
		}
		return i;
	}

	public NetBody GetPiggybackStackTop()
	{
		NetBody netBody = this;
		for (int i = 0; i < 1000; i++)
		{
			if (!((Object)(object)netBody.carrying_person != (Object)null))
			{
				break;
			}
			if ((Object)(object)netBody.carrying_person == (Object)(object)this)
			{
				log.error("PIGGYBACK GOT RECURSIVE !!!!!");
				netBody.StopPiggyback();
				break;
			}
			netBody = netBody.carrying_person;
		}
		return netBody;
	}

	public List<NetBody> GetPiggybackStackAll()
	{
		List<NetBody> list = new List<NetBody> { this };
		NetBody netBody = this;
		for (int i = 0; i < 1000; i++)
		{
			if (!((Object)(object)netBody.piggybacking_on != (Object)null))
			{
				break;
			}
			if ((Object)(object)netBody.piggybacking_on == (Object)(object)this)
			{
				log.error("PIGGYBACK GOT RECURSIVE !!!!! " + string.Join(" ", list) + " !!!");
				netBody.StopPiggyback();
				break;
			}
			netBody = netBody.piggybacking_on;
			list.Add(netBody);
		}
		netBody = this;
		for (int i = 0; i < 1000; i++)
		{
			if (!((Object)(object)netBody.carrying_person != (Object)null))
			{
				break;
			}
			if ((Object)(object)netBody.carrying_person == (Object)(object)this)
			{
				log.error("PIGGYBACK GOT RECURSIVE !!!!! " + string.Join(" ", list) + " !!!");
				netBody.StopPiggyback();
				break;
			}
			netBody = netBody.carrying_person;
			list.Add(netBody);
		}
		return list;
	}

	public bool IsPiggybackable()
	{
		if ((Object)(object)piggybacking_on != (Object)null)
		{
			if (CountPiggybackStackDown() < KrokoshaScavMultiplayer.rules.PiggybackMaxStack)
			{
				return true;
			}
			return false;
		}
		return body.standing;
	}

	public float GetRealFullEncumberanceOfBody()
	{
		float totalEncumberance = body.GetTotalEncumberance();
		float num = body.weightOffset * 0.34f + 50f;
		return totalEncumberance * 0.7f + num * 0.17f;
	}

	private void HandlePiggybackUpdate()
	{
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)carrying_person != (Object)null)
		{
			body.idleTime = 0f;
			if (KrokoshaScavMultiplayer.is_server && !IsPiggybackable())
			{
				carrying_person.StopPiggyback();
			}
		}
		if (!((Object)(object)piggybacking_on != (Object)null))
		{
			return;
		}
		body.idleTime = 0f;
		body.Ragdoll();
		Body val = piggybacking_on.body;
		if (!piggybacking_on.IsPiggybackable())
		{
			StopPiggyback();
			return;
		}
		if (val.isRight != body.isRight)
		{
			body.ForceSwitchDir();
		}
		Rigidbody2D val2 = val.limbs[1].rb;
		Rigidbody2D val3 = body.limbs[1].rb;
		Rigidbody2D val4 = val.GetLowerTorso().rb;
		Rigidbody2D val5 = body.GetUpperTorso().rb;
		float num = (body.isRight ? 1f : (-1f));
		Vector2 val6 = Vector2.up * (1f - piggybacking_on.body.crouchAmount);
		Vector2 val7 = val2.position + val6 + Vector2.left * num;
		if ((Object)(object)Physics2D.OverlapCircle(val7, 1f, LayerMask.GetMask(new string[1] { "Ground" })) != (Object)null)
		{
			val7 = Vector2.LerpUnclamped(piggybacking_on.pos + val6, val7, 0.1f);
		}
		body.crouchAmount = piggybacking_on.body.crouchAmount;
		SetBodyPosition(val7);
		Limb[] limbs = body.limbs;
		foreach (Limb val8 in limbs)
		{
			val8.rb.velocity = Vector2.Lerp(val8.rb.velocity, val.rb.velocity, 0.2f);
		}
		val3.velocity = val.rb.velocity;
		val5.velocity = val.rb.velocity;
		body.GetHead().rb.velocity = val.rb.velocity;
		val3.rotation = val2.rotation;
		val3.angularVelocity = val2.angularVelocity;
		if (Mathf.DeltaAngle(val4.rotation, val5.rotation) > 10f)
		{
			val5.rotation = Mathf.MoveTowardsAngle(val5.rotation, val4.rotation, Time.fixedDeltaTime * 100f);
		}
	}

	public bool CanBeCarriedBySomeone()
	{
		if (KrokoshaScavMultiplayer.rules.AlwaysAllowCarry)
		{
			return true;
		}
		return !body.standing;
	}

	public bool CanPiggyback(NetBody target, bool check_distance = false, bool force = false)
	{
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)piggybacking_on == (Object)(object)target || (Object)(object)body == (Object)null || (Object)(object)target.body == (Object)null)
		{
			return false;
		}
		if ((Object)(object)target.carrying_person != (Object)null)
		{
			return false;
		}
		if (!force && !target.IsPiggybackable())
		{
			return false;
		}
		int num = CountPiggybackStackUp();
		if (target.CountPiggybackStackDown() + num + 1 > KrokoshaScavMultiplayer.rules.PiggybackMaxStack)
		{
			return false;
		}
		if (check_distance)
		{
			if (is_local)
			{
				if (!KM.dist2dsqrcheck((Component)(object)body, (Component)(object)target.body, SharedMain.max_player_interaction_distance))
				{
					return false;
				}
			}
			else if (!KrokoshaScavMultiplayer.is_client && !KM.dist2dsqrcheck((Component)(object)body, (Component)(object)target.body, SharedMain.max_player_interaction_distance * 1.75f))
			{
				return false;
			}
		}
		if (is_local && !force && !Util.QuickRaycastInteractionCheck(Vector2.op_Implicit(((Component)body).transform.position), Vector2.op_Implicit(((Component)target.body).transform.position), do_effect: true))
		{
			PlayerCamera.main.DoAlert(Lang.Get("interact_obstructed", false), false);
		}
		return true;
	}

	public bool StartPiggyback(NetBody target, bool check_distance = false, bool force = false)
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		if (CanPiggyback(target, check_distance, force))
		{
			StopPiggyback(isinternal: true);
			if (log.verbose)
			{
				log.l($"{this} Started piggybacking {target}");
			}
			piggybacking_on = target;
			target.carrying_person = this;
			Sound.Play("backpack" + Random.Range(1, 3), Vector2.op_Implicit(((Component)target.carrying_person.body).transform.position), false, false, ((Component)target.carrying_person.body).transform, 1f, Random.Range(0.8f, 1.2f), false, false);
			target.body.rb.velocity = Vector2.Lerp(target.body.rb.velocity, body.rb.velocity, 0.5f);
			HandlePiggybackUpdate();
			if (is_player)
			{
				if (IsBodyLocal() && !force && KrokoshaScavMultiplayer.is_client)
				{
					ClientMain.Client_SendCharacterSyncPacket();
				}
				last_sync_packet.is_piggyback = true;
				if (KrokoshaScavMultiplayer.is_server)
				{
					if (!IsBodyLocal())
					{
						Server_RemindPlayersCurrentState();
					}
				}
				else
				{
					SetNetIgnoreTime();
				}
			}
			return true;
		}
		return false;
	}

	public bool StopPiggyback(bool isinternal = false)
	{
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)body != (Object)null && (Object)(object)piggybacking_on != (Object)null)
		{
			if (log.verbose)
			{
				log.l($"{this} Stopped piggybacking {piggybacking_on}");
			}
			NetBody netBody = piggybacking_on;
			piggybacking_on.carrying_person = null;
			piggybacking_on = null;
			body.shock = Mathf.Min(body.shock, 9f);
			body.Stand(true);
			if (Object.op_Implicit((Object)(object)netBody))
			{
				SetBodyPosition(Vector2.op_Implicit(((Component)netBody.body).transform.position));
			}
			if (is_player)
			{
				last_sync_packet.is_piggyback = false;
				if (!isinternal)
				{
					if (IsBodyLocal())
					{
						KrokoshaScavMultiplayer.Client_SendSimpleMessageToServer((ushort)10032);
					}
					if (KrokoshaScavMultiplayer.is_server && !IsBodyLocal())
					{
						Server_RemindPlayersCurrentState(keep_velocity: true);
					}
					SetNetIgnoreTime(0.1f);
				}
			}
			return true;
		}
		return false;
	}

	public void Push(NetBody victim)
	{
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)victim.piggybacking_on == (Object)(object)this || (Object)(object)victim.carrying_person == (Object)(object)this)
		{
			return;
		}
		float num = 15f * Mathf.Clamp(1f + body.skills.STRFrom10 * 0.1f, 0.2f, 3f);
		Body obj = body;
		obj.stamina -= 1f * PUSH_STAMINA_USE_MULTIPLIER;
		Body obj2 = body;
		obj2.temperature += 0.03f * PUSH_STAMINA_USE_MULTIPLIER;
		float volume;
		if (KrokoshaScavMultiplayer.is_client)
		{
			Body val = victim.body;
			string snd = "landsmall1";
			Vector2 val2 = victim.pos;
			volume = 1f;
			Util.PlayWorldSoundOnScreenIfInRange(in snd, in val2, in volume, 1f, 64f);
			val.SetVelocity(val.rb.velocity + KM.normal(body.rb.position, val.rb.position, out volume) * num);
			KrokoshaScavMultiplayer.Client_SendSimpleMessageToServer((ushort)10040, (ushort)victim.netId, true);
			return;
		}
		if (is_player)
		{
			plr.server_plrstate.last_push = Time.unscaledTimeAsDouble;
			if (victim.plr.minigame_is_in_a_minigame)
			{
				MinigameMPManager.Server_ForceEndMinigameForPlayer(victim.plr);
			}
		}
		victim.body.Ragdoll();
		victim.body.SetVelocity(victim.rb.velocity + KM.normal(rb.position, victim.rb.position, out volume) * num);
		victim.Server_RemindPlayersCurrentState(keep_velocity: true);
		Vector2 val3 = victim.body.rb.position;
		IReadOnlyList<knetid> to_who;
		if (!is_player)
		{
			to_who = ServerMain.AllClientIds;
		}
		else
		{
			IReadOnlyList<knetid> listOfClientIdsExceptThis = ServerMain.GetListOfClientIdsExceptThis(plr.clientId);
			to_who = listOfClientIdsExceptThis;
		}
		ServerMain.Server_AnnounceSound(val3, "landsmall1", to_who);
	}

	public void OnReceiveSyncPacket(in NetBodySyncPacket pack, bool force = false)
	{
		if (!(_last_sync_packet_receive_time > Time.realtimeSinceStartupAsDouble) || force || !ClientMain._last_reminderpack_while_generating_received)
		{
			_last_sync_packet_receive_time = Time.realtimeSinceStartupAsDouble;
			if ((Object)(object)body != (Object)null)
			{
				pack.Apply(this);
			}
		}
	}

	public void OnReceiveSyncPacket(in ClientToServer_NetBodySyncPacket pack, bool force = false)
	{
		if (!(_last_sync_packet_receive_time > Time.realtimeSinceStartupAsDouble) || force || !ClientMain._last_reminderpack_while_generating_received)
		{
			_last_sync_packet_receive_time = Time.realtimeSinceStartupAsDouble;
			if ((Object)(object)body != (Object)null)
			{
				pack.Apply(this);
			}
		}
	}

	public void Server_RemindPlayersCurrentState(bool keep_velocity = false, bool reliable = true)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		SetNetIgnoreTime();
		last_sync_packet = new NetBodySyncPacket(this);
		if (!keep_velocity)
		{
			last_sync_packet.velocity = Vector2.zero;
		}
		last_sync_packet.moveDir = Vector2.zero;
		NetDataWriter writer = Net.CreateWriter(10019);
		writer.Put((ushort)netId);
		writer.Put(last_sync_packet);
		Net.Server_SendToClients((DeliveryMethod)(reliable ? 2 : 4), in writer, (IEnumerable<knetid>)ServerMain.AllClientIdsExceptHost);
	}
}
