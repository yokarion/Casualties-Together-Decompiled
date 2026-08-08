using System.Collections.Generic;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public class DebugHelp : MonoBehaviour
{
	private class EventDisplayInfo
	{
		public Vector2 pos;

		public string text;

		public float duration;

		public Color color;

		public double start;
	}

	private static List<EventDisplayInfo> events = new List<EventDisplayInfo>();

	public static bool _DEV_VISUALISE_NET_EVENTS => DebugMenuSettings._DEV_VISUALISE_NET_EVENTS;

	public static void OnNetEvent(Vector2 pos, in string text, Color? color = null, float duration = 2f)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if (_DEV_VISUALISE_NET_EVENTS)
		{
			while (CheckOverlap(ref pos))
			{
			}
			events.Add(new EventDisplayInfo
			{
				pos = pos,
				text = text,
				duration = duration,
				color = (Color)(((_003F?)color) ?? Color.blue),
				start = Time.realtimeSinceStartupAsDouble
			});
		}
	}

	private static bool CheckOverlap(ref Vector2 pos)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		foreach (EventDisplayInfo @event in events)
		{
			if (KM.dist2dsqrcheck(in @event.pos, in pos, 1f))
			{
				pos += Vector2.up;
				return true;
			}
		}
		return false;
	}

	private void OnGUI()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		if (!_DEV_VISUALISE_NET_EVENTS || !KrokoshaScavMultiplayer.network_system_is_running)
		{
			return;
		}
		int fontSize = GUI.skin.label.fontSize;
		foreach (EventDisplayInfo @event in events)
		{
			Vector2 val = Vector2.op_Implicit(Camera.main.WorldToScreenPoint(Vector2.op_Implicit(@event.pos)));
			((Vector2)(ref val))._002Ector(val.x, (float)Screen.height - val.y);
			if (val.x > 0f && val.y > 0f && val.x < (float)Screen.width && val.y < (float)Screen.width)
			{
				GUI.skin.label.normal.textColor = @event.color;
				GUI.skin.label.fontSize = 10;
				GUI.Label(new Rect(val.x, val.y, 600f, 100f), @event.text);
			}
		}
		events.RemoveAll((EventDisplayInfo i) => Time.realtimeSinceStartupAsDouble - i.start > (double)i.duration);
		GUI.skin.label.normal.textColor = Color.white;
		GUI.skin.label.fontSize = fontSize;
	}

	public static void Server_SendAnEmptyDebugEvent(in Vector2 pos, in string msg, IReadOnlyList<knetid> target_clients = null)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (target_clients == null)
		{
			target_clients = ServerMain.AllClientIdsExceptHost;
		}
		NetDataWriter writer = Net.CreateWriter(10049);
		writer.Put(pos);
		writer.Put(msg, oneByteChars: true);
		DeliveryMethod delivery = (DeliveryMethod)0;
		IEnumerable<knetid> clientIds = target_clients;
		Net.Server_SendToClients(in delivery, in writer, in clientIds);
	}

	[ClientReceiver(10049, true)]
	private static void Client_EmptyDebugEvent(knetid _, ref NetDataReader reader)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		reader.Get(out Vector2 result);
		reader.Get(out var result2, oneByteChars: true);
		if (_DEV_VISUALISE_NET_EVENTS)
		{
			OnNetEvent(result, in result2);
		}
	}
}
