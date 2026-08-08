using System.Collections.Generic;
using UnityEngine;

namespace KrokoshaCasualtiesUtils;

public static class Util_ContainerExtensions
{
	public static List<Item> GetAllItems(this Container container)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		List<Item> list = new List<Item>(((Component)container).transform.childCount);
		Item item = default(Item);
		foreach (Transform item2 in ((Component)container).transform)
		{
			if (((Component)item2).TryGetComponent<Item>(ref item))
			{
				list.Add(item);
			}
		}
		return list;
	}
}
