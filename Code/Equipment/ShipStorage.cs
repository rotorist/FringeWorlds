using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipStorage : MonoBehaviour 
{
	public float AmmoBaySize;
	public float CargoBaySize;
	public float AmmoBayUsage;
	public float CargoBayUsage;

	public Dictionary<string, InvItemData> AmmoBayItems;
	public Dictionary<string, InvItemData> CargoBayItems;

	public void Initialize()
	{
		AmmoBayItems = new Dictionary<string, InvItemData>();
		CargoBayItems = new Dictionary<string, InvItemData>();
	}

	public bool AddAmmo(InvItemData item)
	{
		if(AmmoBaySize - AmmoBayUsage < item.Quantity)
		{
			return false;
		}

		if(AmmoBayItems.ContainsKey(item.Item.ID))
		{
			AmmoBayItems[item.Item.ID].Quantity += item.Quantity;

		}
		else
		{
			AmmoBayItems.Add(item.Item.ID, item);
		}

		AmmoBayUsage += item.Quantity * item.Item.CargoUnits;

		return true;
	}

	public Item TakeAmmo(string itemID, int quantity)
	{
		if(AmmoBayItems.ContainsKey(itemID))
		{
			if(AmmoBayItems[itemID].Quantity >= quantity)
			{
				Item item = AmmoBayItems[itemID].Item;
				AmmoBayItems[itemID].Quantity -=  quantity;
				AmmoBayUsage -= quantity * AmmoBayItems[itemID].Item.CargoUnits;
				if(AmmoBayItems[itemID].Quantity <= 0)
				{
					AmmoBayItems.Remove(itemID);

				}

				return item;
			}
		}

		return null;
	}

	public int GetAmmoCount(string itemID)
	{
		if(AmmoBayItems.ContainsKey(itemID))
		{
			return AmmoBayItems[itemID].Quantity;
		}
		else
		{
			return 0;
		}
	}

	public float GetAmmoBaySpace()
	{
		return AmmoBaySize - AmmoBayUsage;
	}

	public float GetCargoBaySpace()
	{
		return CargoBaySize - CargoBayUsage;
	}
}
