using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentSheet : PanelBase
{
	public InventoryView EquipmentInventory;



	private InvItemData _selectedItem;

	public override void Initialize ()
	{
		base.Initialize();

	}

	public override void Show ()
	{
		base.Show();
	}

	public override void Hide ()
	{
		base.Hide();
	}

	public override void OnItemSelect (InvItemData itemData)
	{
		_selectedItem = itemData;
		EquipmentInventory.DeselectAll();
	}

	public void Refresh()
	{

	}



}
