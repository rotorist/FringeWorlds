using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticInventoryView : InventoryView
{


	//private List<InvItemData> _inventory;

	public override void Initialize (List<InvItemData> inventory)
	{
		//_inventory = inventory;
		Debug.Log("Item 0 is " + inventory[0].Item.DisplayName);
		ItemEntries[0].ItemData = inventory[0];
		ItemEntries[0].SetItemText(inventory[0].Item.DisplayName);
	}

	public override void Refresh ()
	{
		
	}




}
