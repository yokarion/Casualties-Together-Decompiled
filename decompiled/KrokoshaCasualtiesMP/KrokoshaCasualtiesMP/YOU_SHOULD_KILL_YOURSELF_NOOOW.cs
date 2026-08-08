using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using HarmonyLib;
using KrokoshaCasualtiesUtils;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

[HarmonyPatch(typeof(SelfHarmer), "AttemptHarm")]
public static class YOU_SHOULD_KILL_YOURSELF_NOOOW
{
	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static LimbCondition _003C_003E9__2_0;

		public static LimbCondition _003C_003E9__2_1;

		internal bool _003CSelfHarm_Copypasted_003Eb__2_0(Limb l)
		{
			return l.skinHealth >= 95f;
		}

		internal bool _003CSelfHarm_Copypasted_003Eb__2_1(Limb l)
		{
			return l.skinHealth >= 95f;
		}
	}

	private static bool force_run;

	public static void DoMoodPenaltyInRadius(Body depressedguy, float emotional_damage, float maxdist)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		foreach (NetBody item in NetPlayer.GetPlayerBodiesInRadius(Vector2.op_Implicit(((Component)depressedguy).transform.position), maxdist))
		{
			if ((Object)(object)item.body != (Object)(object)depressedguy && item.body.alive)
			{
				_ = item.body.happiness;
				float num = emotional_damage;
				if (item.body.conscious && Util.AccurateRaycastInteractionCheckObstruction(depressedguy, item.body, do_effect: false))
				{
					num += emotional_damage * 0.5f;
				}
				Body body = item.body;
				body.happiness -= num * item.body.desensitizedMult;
			}
		}
	}

	public static void SelfHarmClawAnim(Body body)
	{
		SelfHarmer component = ((Component)body).GetComponent<SelfHarmer>();
		Util.GetMethod<SelfHarmer>("ClawAnim").Invoke(component, null);
	}

	public static void SelfHarm_Copypasted(byte type, Body body, ref Limb limb)
	{
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Expected O, but got Unknown
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Expected O, but got Unknown
		bool flag = type == 1;
		if (type == 2)
		{
			if (limb == null)
			{
				limb = body.limbs[Random.Range(1, body.limbs.Length)];
			}
			Limb obj = limb;
			obj.muscleHealth -= 13f;
			Limb obj2 = limb;
			obj2.skinHealth -= 35f;
			Limb obj3 = limb;
			obj3.bleedAmount += Random.Range(6f, 10f);
			Limb obj4 = limb;
			obj4.pain += Random.Range(20f, 30f);
			WoundView view = WoundView.view;
			Limb obj5 = limb;
			Sprite obj6 = Resources.Load<Sprite>("Special/selfHarmWound");
			object obj7 = _003C_003Ec._003C_003E9__2_0;
			if (obj7 == null)
			{
				LimbCondition val = (Limb l) => l.skinHealth >= 95f;
				_003C_003Ec._003C_003E9__2_0 = val;
				obj7 = (object)val;
			}
			view.AddImageToLimb(obj5, obj6, false, (LimbCondition)obj7);
			body.rb.AddForce(Random.insideUnitCircle * 500f, (ForceMode2D)1);
			body.talker.Talk(Locale.GetCharacter("selfharm"), (Limb)null, true, false);
			body.happiness += 4f;
			Sound.Play("selfharm", Vector2.op_Implicit(((Component)body).transform.position), false, true, (Transform)null, 1f, 1f, false, false);
			if (Util.IsBodyLocal(body))
			{
				Sound.Play("harmSting", Vector2.zero, true, false, (Transform)null, 0.7f, 1f, false, false);
			}
			SelfHarmClawAnim(body);
			DoMoodPenaltyInRadius(body, KrokoshaScavMultiplayer.rules.SelfharmWitnessMoodDebuff * 0.25f, 10f);
			return;
		}
		if (limb == null)
		{
			limb = (flag ? body.limbs[0] : ((Random.Range(0f, 1f) < 0.5f) ? body.limbs[4] : body.limbs[7]));
		}
		Limb obj8 = limb;
		obj8.muscleHealth -= 30f;
		Limb obj9 = limb;
		obj9.skinHealth -= 70f;
		Limb obj10 = limb;
		obj10.bleedAmount += Random.Range(45f, 60f);
		Limb obj11 = limb;
		obj11.pain += Random.Range(25f, 35f);
		body.adrenaline += 50f;
		body.rb.AddForce(Random.insideUnitCircle * 700f, (ForceMode2D)1);
		if (!flag)
		{
			body.talker.Talk(Locale.GetCharacter("suicide"), (Limb)null, true, false);
		}
		body.happiness += (flag ? 16f : 12.5f);
		Sound.Play("suicide", Vector2.op_Implicit(((Component)body).transform.position), false, true, (Transform)null, 1f, 1f, false, false);
		if (Util.IsBodyLocal(body))
		{
			MusicManager.main.PlaySong(Resources.Load<AudioClip>("Sounds/music/whathaveidone"));
			Sound.Play("harmSting", Vector2.zero, true, false, (Transform)null, 1f, 0.7f, false, false);
		}
		WoundView view2 = WoundView.view;
		Limb obj12 = limb;
		Sprite obj13 = Resources.Load<Sprite>("Special/suicideWound");
		object obj14 = _003C_003Ec._003C_003E9__2_1;
		if (obj14 == null)
		{
			LimbCondition val2 = (Limb l) => l.skinHealth >= 95f;
			_003C_003Ec._003C_003E9__2_1 = val2;
			obj14 = (object)val2;
		}
		view2.AddImageToLimb(obj12, obj13, false, (LimbCondition)obj14);
		if (flag)
		{
			body.RemoveEye();
		}
		SelfHarmClawAnim(body);
		DoMoodPenaltyInRadius(body, KrokoshaScavMultiplayer.rules.SelfharmWitnessMoodDebuff, 25f);
	}

	public static void Force(SelfHarmer __instance)
	{
		force_run = true;
		__instance.AttemptHarm();
		force_run = false;
	}

	public static bool Prefix(SelfHarmer __instance)
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		if (!KrokoshaScavMultiplayer.network_system_is_running)
		{
			return true;
		}
		if (!force_run)
		{
			Body component = ((Component)__instance).GetComponent<Body>();
			if (KrokoshaScavMultiplayer.is_client && !Util.IsBodyLocal(component))
			{
				__instance.cooldownTime = 120f;
				return false;
			}
			Item val = default(Item);
			Item val2 = default(Item);
			if (component.FindByIdThorough("plushie", ref val))
			{
				if (KrokoshaScavMultiplayer.IsNetworkActiveAndIsServer())
				{
					NetDataWriter writer = Net.CreateWriter(10064);
					writer.Put((byte)20);
					writer.Put(new LimbNetId(component.GetHead()));
					Net.Server_SendToClients((DeliveryMethod)0, in writer, (IEnumerable<knetid>)ServerMain.AllClientIdsExceptHost);
					return true;
				}
			}
			else if (component.totalHappiness > -90f)
			{
				if (Util.IsBodyLocal(component))
				{
					return true;
				}
			}
			else if (component.FindByTagThorough("gun", ref val2))
			{
				if (KrokoshaScavMultiplayer.IsNetworkActiveAndIsServer())
				{
					NetDataWriter writer2 = Net.CreateWriter(10064);
					writer2.Put((byte)10);
					writer2.Put(new LimbNetId(component.GetHead()));
					Net.Server_SendToClients((DeliveryMethod)0, in writer2, (IEnumerable<knetid>)ServerMain.AllClientIdsExceptHost);
					return true;
				}
			}
			else if (Util.IsBodyLocal(component))
			{
				return true;
			}
			__instance.cooldownTime = 120f;
			return false;
		}
		return true;
	}

	[ClientReceiver(10030, true)]
	private static void Client_ForceTalkerSay(knetid _, ref NetDataReader reader)
	{
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out knetid result);
		byte b = default(byte);
		reader.Get(ref b);
		string text = default(string);
		reader.Get(ref text);
		object obj = null;
		GameObject val = null;
		switch (b)
		{
		case 0:
		{
			if (NetObjectRegistry.TryGetSyncInfo(result, out var si))
			{
				val = si.go;
				obj = si;
			}
			break;
		}
		case 1:
		{
			if (NetPlayer.TryGetNetPlayerAndNetBodyFromClientId(result, out var _, out var pb))
			{
				val = ((Component)pb).gameObject;
				obj = pb;
			}
			break;
		}
		}
		if ((Object)(object)val != (Object)null && Util.TryGetTalkerOnObject(val, out var talker, out var _))
		{
			if (DebugHelp._DEV_VISUALISE_NET_EVENTS)
			{
				DebugHelp.OnNetEvent(Vector2.op_Implicit(val.transform.position), "C: " + obj.ToString() + " ForceTalkerSay: \"" + text + "\"");
			}
			talker.ForceNoSpeechImpairment(text, resetTalkTimer: true);
		}
	}

	[ClientReceiver(10064, true)]
	private static void Client_SelfHarmMinigameMinigameEnd(knetid servid, ref NetDataReader reader)
	{
		byte b = default(byte);
		reader.Get(ref b);
		reader.Get(out LimbNetId result);
		if (!result.TryGetNetBodyAndLimb(out var nb, out var limb))
		{
			return;
		}
		Body body = nb.body;
		switch (b)
		{
		case 20:
		{
			Item val = default(Item);
			if (body.FindByIdThorough("plushie", ref val))
			{
				Force(((Component)body).GetComponent<SelfHarmer>());
			}
			break;
		}
		case 10:
		{
			Item val2 = default(Item);
			if (body.FindByTagThorough("gun", ref val2))
			{
				((MonoBehaviour)((Component)body).GetComponent<SelfHarmer>()).StartCoroutine("Suicide");
			}
			break;
		}
		default:
			if (!Util.IsBodyLocal(body))
			{
				SelfHarm_Copypasted(b, body, ref limb);
			}
			break;
		}
	}
}
