using Unity.VisualScripting;
using UnityEngine;

namespace KrokoshaCasualtiesMP;

public struct CharacterHealthPainkillerStateSyncPacket : INetSerializeByMemcpy
{
	public float opiateAmount;

	public float opiateTolerance;

	public float opiateReception;

	public float antagonistAmount;

	public float actualOpiateReception;

	public CharacterHealthPainkillerStateSyncPacket(Body body)
	{
		Painkillers component = ((Component)body).GetComponent<Painkillers>();
		opiateAmount = component.opiateAmount;
		opiateTolerance = component.opiateTolerance;
		opiateReception = component.opiateReception;
		antagonistAmount = component.antagonistAmount;
		actualOpiateReception = component.actualOpiateReception;
	}

	public void Apply(Body body)
	{
		Painkillers orAddComponent = ComponentHolderProtocol.GetOrAddComponent<Painkillers>((Object)(object)body);
		orAddComponent.opiateAmount = opiateAmount;
		orAddComponent.opiateTolerance = opiateTolerance;
		orAddComponent.opiateReception = opiateReception;
		orAddComponent.antagonistAmount = antagonistAmount;
		orAddComponent.actualOpiateReception = actualOpiateReception;
	}
}
