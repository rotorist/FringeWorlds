using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipStorage : MonoBehaviour 
{
	public int AmmoBaySize;
	public int CargoBaySize;

	public Dictionary<string, InvItemData> AmmoBayItems;
	public Dictionary<string, InvItemData> CargoBayItems;

	public void Initialize()
	{
		AmmoBayItems = new Dictionary<string, InvItemData>();
		CargoBayItems = new Dictionary<string, InvItemData>();
	}

	public bool TakeAmmo(string itemID, int quantity)
	{
		if(AmmoBayItems.ContainsKey(itemID))
		{
			if(AmmoBayItems[itemID].Quantity >= quantity)
			{
				AmmoBayItems[itemID].Quantity -=  quantity;
				if(AmmoBayItems[itemID].Quantity <= 0)
				{
					AmmoBayItems.Remove(itemID);
				}

				return true;
			}
		}

		return false;
	}
}
