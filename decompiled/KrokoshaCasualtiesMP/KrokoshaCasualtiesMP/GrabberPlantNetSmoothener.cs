using System;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

internal class GrabberPlantNetSmoothener : MonoBehaviour
{
	public Vector2 tipPos;

	public Vector2 prevtipPos;

	public double receivetime;

	public double prevreceivetime;

	private void Start()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		receivetime = Time.realtimeSinceStartupAsDouble;
		prevreceivetime = receivetime;
		tipPos = ((Component)this).GetComponent<GrabberPlant>().tipPos;
	}

	private void Update()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		GrabberPlant component = ((Component)this).GetComponent<GrabberPlant>();
		double num = receivetime - prevreceivetime;
		double num2 = Time.realtimeSinceStartupAsDouble - receivetime;
		if (num > 0.0)
		{
			float num3 = (float)Math.Min(num2 / num, 1.0);
			Vector2 val = Vector2.Lerp(prevtipPos, tipPos, num3);
			component.tipPos = Vector2.Lerp(component.tipPos, val, Time.unscaledTime * 4.7f);
		}
	}

	public void OnNewTipPosReceived(Vector2 vector2)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		prevtipPos = tipPos;
		tipPos = vector2;
		prevreceivetime = receivetime;
		receivetime = Time.realtimeSinceStartupAsDouble;
	}
}
