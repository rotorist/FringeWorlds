using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticInventoryView : InventoryView
{


	private List<InvItemData> _staticInventory;

	public override void Initialize (List<InvItemData> inventory)
	{
		_staticInventory = inventory;
		//Debug.Log("Item 1 is " + inventory[0].Item.DisplayName + " " + inventory.Count);

		Refresh();
	}

	public override void Refresh ()
	{
		for(int i=0; i < _staticInventory.Count; i++)
		{
			if(_staticInventory[i] != null)
			{
				ItemEntries[i].ItemData = _staticInventory[i];
				ItemEntries[i].SetItemText(_staticInventory[i].Item.DisplayName);
				ItemEntries[i].MyButton.enabled = true;
			}
			else
			{
				ItemEntries[i].SetItemText("");
				ItemEntries[i].MyButton.enabled = false;
			}
		}
	}




}
