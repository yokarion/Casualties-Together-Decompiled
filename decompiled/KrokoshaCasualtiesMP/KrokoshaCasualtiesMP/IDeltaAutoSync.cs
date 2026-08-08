namespace KrokoshaCasualtiesMP;

public interface IDeltaAutoSync<T>
{
	void AutoSerialize(knetid netId, T real_obj);

	void AutoDeserialize(knetid netId, T real_obj);
}
