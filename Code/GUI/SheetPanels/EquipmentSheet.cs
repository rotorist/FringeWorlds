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

	public BarIndicator PowerUsage;
	public UILabel AvailablePower;

	private float _availablePower;
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
				//check if available power is larger or equal to the _selectedItem
				if(_availablePower < _selectedItem.Item.GetFloatAttribute("Power Required"))
				{
					//issue an error
					GameManager.Inst.UIManager.ErrorMessagePanel.DisplayMessage("Cannot install equipment. Not enough power.");
					return;
				}

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

		//here add all the mods to loadoutEquipment

		//now get power usage
		float powerUsage = CalculatePowerUsage(loadoutEquipment);
		float totalPower = GameManager.Inst.ItemManager.AllShipStats[playerLoadout.ShipID].PowerSupply;
		_availablePower = totalPower - powerUsage;
		PowerUsage.SetFillPercentage(powerUsage / totalPower);
		AvailablePower.text = _availablePower.ToString();

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

	public float CalculatePowerUsage(List<InvItemData> inventory)
	{
		float usage = 0;
		foreach(InvItemData item in inventory)
		{
			if(item != null)
			{
				usage += item.Item.GetFloatAttribute("Power Required");
			}
		}

		return usage;
	}
}
