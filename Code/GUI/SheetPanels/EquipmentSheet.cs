using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentSheet : PanelBase
{
	public StaticInventoryView EquipmentInventory;
	public InventoryView CargoEquipmentInventory;
	public EquipmentActionSheet ActionSheet;
	public ShipInventorySheet ShipInventorySheet;

	public UIButton InstallButton;
	public UIButton RemoveButton;

	private InvItemData _selectedItem;
	private InventoryView _selectedItemContainer;

	public override void Initialize ()
	{
		base.Initialize();

	}

	public override void Show ()
	{
		base.Show();
		ClearSelections();
	}

	public override void Hide ()
	{
		base.Hide();
		ClearSelections();
	}

	public override void OnItemSelect (InvItemData itemData, InventoryView container)
	{
		_selectedItem = itemData;
		_selectedItemContainer = container;
		EquipmentInventory.DeselectAll();
		CargoEquipmentInventory.DeselectAll();

		ActionSheet.SetItemTitle(itemData.Item.DisplayName);

		//build description with item description and attributes
		ActionSheet.SetItemDesc(itemData.Item.Description);
		ActionSheet.ListItemAttributes(itemData.Item.Attributes);

		if(container == EquipmentInventory)
		{
			InstallButton.isEnabled = false;
			RemoveButton.isEnabled = true;
		}
		else if(container == CargoEquipmentInventory)
		{
			InstallButton.isEnabled = true;
			RemoveButton.isEnabled = false;
		}
	}

	public void OnInstallButtonClick()
	{
		if(_selectedItem != null)
		{
			if(_selectedItemContainer == CargoEquipmentInventory)
			{
				//remove the invItemData from cargo and assign it to the slot in loadout
				//determine the slot by "EquipmentType" attribute of the item
				Loadout activeLoadout = GameManager.Inst.PlayerProgress.ActiveLoadout;
				if(activeLoadout.CargoBayItems.Contains(_selectedItem))
				{
					activeLoadout.CargoBayItems.Remove(_selectedItem);
				}

				string equipmentType = _selectedItem.Item.GetStringAttribute("Equipment Type");
				InvItemData tempItem = activeLoadout.GetInvItemFromEquipmentType(equipmentType);
				if(tempItem != null)
				{
					activeLoadout.CargoBayItems.Add(tempItem);
				}
				activeLoadout.SetEquipmentInvItem(_selectedItem);
				_selectedItem = null;
				tempItem = null;

				Refresh();

			}
		}
	}

	public void OnRemoveButtonClick()
	{
		if(_selectedItem != null)
		{
			if(_selectedItemContainer == EquipmentInventory)
			{
				//remove the item from slot and put it in cargo in loadout
				//determine the slot by "EquipmentType" attribute of the item
				Loadout activeLoadout = GameManager.Inst.PlayerProgress.ActiveLoadout;
				activeLoadout.ClearEquipment(_selectedItem);
				activeLoadout.CargoBayItems.Add(_selectedItem);

				Refresh();
			}

		}
	}

	public void Refresh()
	{
		Loadout playerLoadout = GameManager.Inst.PlayerProgress.ActiveLoadout;
		List<InvItemData> loadoutEquipment = new List<InvItemData>();
		loadoutEquipment.Add(playerLoadout.Shield);
		loadoutEquipment.Add(playerLoadout.WeaponCapacitor);
		loadoutEquipment.Add(playerLoadout.Thruster);
		loadoutEquipment.Add(playerLoadout.Scanner);
		loadoutEquipment.Add(playerLoadout.Teleporter);
		EquipmentInventory.Initialize(loadoutEquipment);

		ShipInventorySheet.Refresh();
		ClearSelections();
	}

	public void ClearSelections()
	{
		EquipmentInventory.DeselectAll();
		CargoEquipmentInventory.DeselectAll();
		_selectedItem = null;
		_selectedItemContainer = null;
		InstallButton.isEnabled = false;
		RemoveButton.isEnabled = false;
		ActionSheet.Clear();
	}


}
