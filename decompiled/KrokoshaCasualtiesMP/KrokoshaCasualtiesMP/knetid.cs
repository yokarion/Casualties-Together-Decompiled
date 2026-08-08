namespace KrokoshaCasualtiesMP;

public struct knetid
{
	public ushort id;

	public knetid(ushort id)
	{
		this.id = id;
	}

	public static implicit operator ushort(knetid id)
	{
		return id.id;
	}

	public static implicit operator knetid(ushort id)
	{
		return new knetid(id);
	}

	public override string ToString()
	{
		return id.ToString();
	}
}
