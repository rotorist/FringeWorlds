using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipStorage : MonoBehaviour 
{
	public float AmmoBaySize;
	public float CargoBaySize;
	public float AmmoBayUsage;

	public Dictionary<string, InvItemData> AmmoBayItems;
	public List<InvItemData> CargoBayItems;

	public void Initialize()
	{
		AmmoBayItems = new Dictionary<string, InvItemData>();
		CargoBayItems = new List<InvItemData>();
	}

	public void Initialize(List<InvItemData> ammoBayItems, List<InvItemData> cargoBayItems)
	{
		AmmoBayItems = new Dictionary<string, InvItemData>();
		foreach(InvItemData item in ammoBayItems)
		{
			AmmoBayItems.Add(item.Item.ID, item);
		}

		CargoBayItems = new List<InvItemData>();
		foreach(InvItemData item in cargoBayItems)
		{
			CargoBayItems.Add(item);
		}
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

	public Item TakeAmmo(string itemID, int quantity, string ammoType, bool isNPC)
	{
		//Debug.Log("taking ammo of type " + ammoType + " is itemID null " + (itemID == null));
		if(!string.IsNullOrEmpty(itemID) && AmmoBayItems.ContainsKey(itemID))
		{
			if(AmmoBayItems[itemID].Quantity >= quantity)
			{
				Item item = AmmoBayItems[itemID].Item;
				if(!isNPC)
				{
					AmmoBayItems[itemID].Quantity -=  quantity;
					AmmoBayUsage -= quantity * AmmoBayItems[itemID].Item.CargoUnits;
					if(AmmoBayItems[itemID].Quantity <= 0)
					{
						AmmoBayItems.Remove(itemID);

					}
				}

				return item;
			}
		}
		else if(!string.IsNullOrEmpty(ammoType))
		{
			//loop through each item and find one with same ammoType
			foreach(KeyValuePair<string, InvItemData> ammo in AmmoBayItems)
			{
				Debug.Log("checking ammo " + ammo.Value.Item.ID);
				if(ammo.Value.Item.GetStringAttribute("Ammo Type") == ammoType)
				{
					Debug.Log("Found ammo");
					if(ammo.Value.Quantity >= quantity)
					{
						Item item = ammo.Value.Item;
						if(!isNPC)
						{
							ammo.Value.Quantity -= quantity;
							AmmoBayUsage -= quantity * ammo.Value.Item.CargoUnits;
							if(ammo.Value.Quantity <= 0)
							{
								AmmoBayItems.Remove(ammo.Key);
							}
						}

						return ammo.Value.Item;
					}
				}
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
		float usage = 0;
		foreach(InvItemData item in CargoBayItems)
		{
			usage += item.Item.CargoUnits;
		}

		return CargoBaySize - usage;
	}
}
