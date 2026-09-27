using System.Collections;
using UnityEngine;

namespace Together;

public class HingeJointState : MonoBehaviour
{
	private HingeJoint2D hinge;

	private JointAngleLimits2D savedLimits;

	private JointMotor2D savedMotor;

	private bool savedUseLimits;

	private bool savedUseMotor;

	private Vector2 savedAnchor;

	private Vector2 savedConnectedAnchor;

	private bool savedAutoAnchor;

	private void Awake()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		hinge = ((Component)this).GetComponent<HingeJoint2D>();
		savedLimits = hinge.limits;
		savedMotor = hinge.motor;
		savedUseLimits = hinge.useLimits;
		savedUseMotor = hinge.useMotor;
		savedAnchor = ((AnchoredJoint2D)hinge).anchor;
		savedConnectedAnchor = ((AnchoredJoint2D)hinge).connectedAnchor;
		savedAutoAnchor = ((AnchoredJoint2D)hinge).autoConfigureConnectedAnchor;
	}

	private void OnEnable()
	{
		((MonoBehaviour)this).StartCoroutine(RestoreJoint());
	}

	private IEnumerator RestoreJoint()
	{
		yield return (object)new WaitForFixedUpdate();
		((AnchoredJoint2D)hinge).autoConfigureConnectedAnchor = savedAutoAnchor;
		((AnchoredJoint2D)hinge).anchor = savedAnchor;
		((AnchoredJoint2D)hinge).connectedAnchor = savedConnectedAnchor;
		hinge.limits = savedLimits;
		hinge.useLimits = savedUseLimits;
		hinge.motor = savedMotor;
		hinge.useMotor = savedUseMotor;
		if (Object.op_Implicit((Object)(object)((Joint2D)hinge).attachedRigidbody))
		{
			((Joint2D)hinge).attachedRigidbody.WakeUp();
		}
	}
}
