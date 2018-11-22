using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentSheet : PanelBase
{
	public StaticInventoryView EquipmentInventory;
	public StaticInventoryView ShipModsInventory;
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
	private int _selectedItemIndex;

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

	public override void OnItemSelect (InventoryItemEntry itemEntry, InventoryView container)
	{
		_selectedItem = itemEntry.ItemData;
		_selectedItemContainer = container;
		_selectedItemIndex = itemEntry.InventoryIndex;
		EquipmentInventory.DeselectAll();
		CargoEquipmentInventory.DeselectAll();
		ShipModsInventory.DeselectAll();

		ActionSheet.SetItemTitle(itemEntry.ItemData.Item.DisplayName);

		//build description with item description and attributes
		ActionSheet.SetItemDesc(itemEntry.ItemData.Item.Description);
		ActionSheet.ListItemAttributes(itemEntry.ItemData.Item.Attributes);

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
		else if(container == ShipModsInventory)
		{
			InstallButton.isEnabled = false;
			RemoveButton.isEnabled = true;
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

				string equipmentType = _selectedItem.Item.GetStringAttribute("Equipment Type");
				Loadout activeLoadout = GameManager.Inst.PlayerProgress.ActiveLoadout;

				if(equipmentType != "PassiveShipMod" && equipmentType != "ActiveShipMod")
				{
					

					if(activeLoadout.CargoBayItems.Contains(_selectedItem))
					{
						activeLoadout.CargoBayItems.Remove(_selectedItem);
					}

					InvItemData tempItem = activeLoadout.GetInvItemFromEquipmentType(equipmentType);
					if(tempItem != null)
					{
						activeLoadout.CargoBayItems.Add(tempItem);
					}

					activeLoadout.SetEquipmentInvItem(_selectedItem, equipmentType);
					_selectedItem = null;
					tempItem = null;
				}
				else
				{
					//check if dependency mod is equiped
					string dependency = _selectedItem.Item.GetStringAttribute("Dependency");
					if(dependency != "" && activeLoadout.GetShipModFromID(dependency) == null)
					{
						GameManager.Inst.UIManager.ErrorMessagePanel.DisplayMessage("Cannot install ship mod. Dependency required.");
						return;
					}

					//try to install the mod
					bool result = activeLoadout.SetShipModInvItem(_selectedItem, equipmentType);
					if(result == false)
					{
						GameManager.Inst.UIManager.ErrorMessagePanel.DisplayMessage("No Ship Mod slots available.");
						return;
					}
					else
					{
						if(activeLoadout.CargoBayItems.Contains(_selectedItem))
						{
							activeLoadout.CargoBayItems.Remove(_selectedItem);
						}
					}

					_selectedItem = null;
				}



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


			}
			else if(_selectedItemContainer == ShipModsInventory)
			{
				Loadout activeLoadout = GameManager.Inst.PlayerProgress.ActiveLoadout;
				activeLoadout.CargoBayItems.Add(_selectedItem);
				activeLoadout.RemoveShipModByIndex(_selectedItemIndex);
				//also remove any dependencies
				//check if any mod depends on me
				List<InvItemData> dependencies = activeLoadout.GetModDependencies(_selectedItem.Item.ID);
				foreach(InvItemData dependency in dependencies)
				{
					activeLoadout.RemoveShipMod(dependency);
					activeLoadout.CargoBayItems.Add(dependency);
				}
			}

			Refresh();
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

		//hide mod slots beyond what ship has
		int numberSlots = GameManager.Inst.ItemManager.AllShipStats[playerLoadout.ShipID].ModSlots;
		for(int i=0; i<ShipModsInventory.ItemEntries.Count; i++)
		{
			if(i < numberSlots)
			{
				NGUITools.SetActive(ShipModsInventory.ItemEntries[i].gameObject, true);
			}
			else
			{
				NGUITools.SetActive(ShipModsInventory.ItemEntries[i].gameObject, false);
			}
		}
		//here add all the mods to loadoutEquipment
		List<InvItemData> loadoutShipMods = new List<InvItemData>();
		for(int i=0; i<playerLoadout.ShipMods.Length; i++)
		{
			loadoutShipMods.Add(playerLoadout.ShipMods[i]);
		}
		ShipModsInventory.Initialize(loadoutShipMods);

		//now get power usage
		float powerUsageEquipment = CalculatePowerUsage(loadoutEquipment);
		float powerUsageShipMods = CalculatePowerUsage(loadoutShipMods);
		float powerUsage = powerUsageEquipment + powerUsageShipMods;
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
		ShipModsInventory.DeselectAll();
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
