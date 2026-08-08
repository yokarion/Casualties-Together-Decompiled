using UnityEngine;

namespace KrokoshaCasualtiesMP;

public class Krokosha_Heater_MultiplayerReplacementComponent : MonoBehaviour
{
	private Heater heater;

	private void Awake()
	{
		heater = ((Component)this).GetComponent<Heater>();
	}

	private void Update()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		if (!((Behaviour)heater).enabled || !(Time.time - heater.lastTime > 0.5f))
		{
			return;
		}
		heater.lastTime = Time.time;
		foreach (NetBody item in NetBody.GetBodiesInRadius(Vector2.op_Implicit(((Component)heater).transform.position), heater.maxDistance))
		{
			Body body = item.body;
			if ((heater.isHeater && body.temperature < heater.desiredTemp) || (!heater.isHeater && body.temperature > heater.desiredTemp))
			{
				body.temperature = Mathf.Lerp(body.temperature, heater.desiredTemp, heater.tempLerpStrength);
			}
		}
	}
}
