using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EquipmentSheet : PanelBase
{
	public Loadout CurrentLoadout;
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
		CargoEquipmentInventory.SelectedItemHandler = this;

		InstallButton.isEnabled = false;
		InstallButton.GetComponent<UISprite>().alpha = 1;
		RemoveButton.isEnabled = false;
		RemoveButton.GetComponent<UISprite>().alpha = 1;
	}

	public override void Hide ()
	{
		base.Hide();
		ClearSelections();

		InstallButton.isEnabled = false;
		InstallButton.GetComponent<UISprite>().alpha = 0;
		RemoveButton.isEnabled = false;
		RemoveButton.GetComponent<UISprite>().alpha = 0;
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
		ActionSheet.SetItemSubnote("", "");
		ActionSheet.ListItemAttributes(itemEntry.ItemData.Item);

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
					GameManager.Inst.UIManager.ErrorMessagePanel.DisplayMessage("CANNOT INSTALL EQUIPMENT. NOT ENOUGH POWER");
					return;
				}

				//remove the invItemData from cargo and assign it to the slot in loadout
				//determine the slot by "EquipmentType" attribute of the item

				string equipmentType = _selectedItem.Item.GetStringAttribute("Equipment Type");

				if(equipmentType != "PassiveShipMod" && equipmentType != "ActiveShipMod")
				{
					//if is shield, check shield class
					if(equipmentType == "Shield")
					{
						ShieldClass itemShieldClass = (ShieldClass)Enum.Parse(typeof(ShieldClass), _selectedItem.Item.GetStringAttribute("Shield Class"));
						if(itemShieldClass != GameManager.Inst.ItemManager.AllShipStats[CurrentLoadout.ShipID].ShieldClass)
						{
							//issue an error
							GameManager.Inst.UIManager.ErrorMessagePanel.DisplayMessage("SHIELD CLASS DOES NOT MATCH CURRENT SHIP");
							return;
						}
					}

					if(CurrentLoadout.CargoBayItems.Contains(_selectedItem))
					{
						CurrentLoadout.CargoBayItems.Remove(_selectedItem);
					}

					InvItemData tempItem = CurrentLoadout.GetInvItemFromEquipmentType(equipmentType);
					if(tempItem != null)
					{
						CurrentLoadout.CargoBayItems.Add(tempItem);
					}

					CurrentLoadout.SetEquipmentInvItem(_selectedItem, equipmentType);
					_selectedItem = null;
					tempItem = null;
				}
				else
				{
					//check if dependency mod is equiped
					string dependency = _selectedItem.Item.GetStringAttribute("Dependency");
					if(dependency != "" && CurrentLoadout.GetShipModFromID(dependency) == null)
					{
						GameManager.Inst.UIManager.ErrorMessagePanel.DisplayMessage("CANNOT INSTALL SHIP MOD. DEPENDENCY REQUIRED");
						return;
					}

					//try to install the mod
					bool result = CurrentLoadout.SetShipModInvItem(_selectedItem, equipmentType);
					if(result == false)
					{
						GameManager.Inst.UIManager.ErrorMessagePanel.DisplayMessage("NO SHIP MOD SLOTS AVAILABLE");
						return;
					}
					else
					{
						if(CurrentLoadout.CargoBayItems.Contains(_selectedItem))
						{
							CurrentLoadout.CargoBayItems.Remove(_selectedItem);
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
			//check if there's enough cargo space
			if(_selectedItem.Item.CargoUnits > ShipInventorySheet.AvailableCargoSpaceValue)
			{
				GameManager.Inst.UIManager.ErrorMessagePanel.DisplayMessage("NOT ENOUGH SPACE IN CARGO BAY");
				return;
			}

			if(_selectedItemContainer == EquipmentInventory)
			{
				//remove the item from slot and put it in cargo in loadout
				//determine the slot by "EquipmentType" attribute of the item
				CurrentLoadout.ClearEquipment(_selectedItem);
				CurrentLoadout.CargoBayItems.Add(_selectedItem);


			}
			else if(_selectedItemContainer == ShipModsInventory)
			{
				CurrentLoadout.CargoBayItems.Add(_selectedItem);
				CurrentLoadout.RemoveShipModByIndex(_selectedItemIndex);
				//also remove any dependencies
				//check if any mod depends on me
				List<InvItemData> dependencies = CurrentLoadout.GetModDependencies(_selectedItem.Item.ID);
				foreach(InvItemData dependency in dependencies)
				{
					CurrentLoadout.RemoveShipMod(dependency);
					CurrentLoadout.CargoBayItems.Add(dependency);
				}
			}

			Refresh();
		}
	}

	public void Refresh()
	{
		Debug.Log("Current loadout id: " + CurrentLoadout.LoadoutID);
		List<InvItemData> loadoutEquipment = new List<InvItemData>();
		loadoutEquipment.Add(CurrentLoadout.Shield);
		loadoutEquipment.Add(CurrentLoadout.WeaponCapacitor);
		loadoutEquipment.Add(CurrentLoadout.Thruster);
		loadoutEquipment.Add(CurrentLoadout.Scanner);
		loadoutEquipment.Add(CurrentLoadout.Teleporter);
		EquipmentInventory.Initialize(loadoutEquipment);

		//hide mod slots beyond what ship has
		int numberSlots = GameManager.Inst.ItemManager.AllShipStats[CurrentLoadout.ShipID].ModSlots;
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
		for(int i=0; i<CurrentLoadout.ShipMods.Length; i++)
		{
			loadoutShipMods.Add(CurrentLoadout.ShipMods[i]);
		}
		ShipModsInventory.Initialize(loadoutShipMods);

		//now get power usage
		float powerUsageEquipment = CalculatePowerUsage(loadoutEquipment);
		float powerUsageShipMods = CalculatePowerUsage(loadoutShipMods);
		float powerUsage = powerUsageEquipment + powerUsageShipMods;
		float totalPower = GameManager.Inst.ItemManager.AllShipStats[CurrentLoadout.ShipID].PowerSupply;
		_availablePower = totalPower - powerUsage;
		PowerUsage.SetFillPercentage(powerUsage / totalPower);
		AvailablePower.text = _availablePower.ToString();

		ShipInventorySheet.InventoryItemTypes.Clear();
		ShipInventorySheet.InventoryItemTypes.Add(ItemType.Equipment);
		ShipInventorySheet.InventoryItemTypes.Add(ItemType.ShipMod);
		ShipInventorySheet.Refresh();
		ShipInventorySheet.RefreshLoadButtons(null);
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
