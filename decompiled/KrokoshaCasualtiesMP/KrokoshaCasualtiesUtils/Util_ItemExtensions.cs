namespace KrokoshaCasualtiesUtils;

public static class Util_ItemExtensions
{
	public static bool IsHandcrank(this Item item)
	{
		return item.id == "handcrank";
	}

	public static bool IsAED(this Item item)
	{
		return item.id == "aed";
	}

	public static bool IsManualDefibrillator(this Item item)
	{
		return item.id == "manualdefibrillator";
	}
}
