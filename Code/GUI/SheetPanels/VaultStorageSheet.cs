using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VaultStorageSheet : PanelBase
{
	public Loadout CurrentLoadout;
	public InventoryView VaultInventory;
	public InventoryView CargoAmmoInventory;
	public ShipInventorySheet ShipInventorySheet;
	public EquipmentActionSheet ActionSheet;
	public BarIndicator VaultSpace;
	public UILabel AvailableVaultSpace;
	public float AvailableVaultSpaceValue;

	public UITabSelection ItemTypeTabs;
	public UIButton StoreButton;
	public UIButton RetrieveButton;
	public UISlider QuantitySlider;
	public UILabel QuantityLabel;

	private List<ItemType> _itemTypeFilter;
	private InvItemData _selectedItem;
	private InventoryView _selectedItemContainer;
	private DockableStationData _currentStationData;
	private int _selectedQuantity;

	public override void Initialize ()
	{
		base.Initialize ();
		_itemTypeFilter = new List<ItemType>();
	}

	public override void Show ()
	{
		base.Show ();
		NGUITools.SetActive(ItemTypeTabs.gameObject, true);
		ItemTypeTabs.ForceSelectTab("Equipment");
		CargoAmmoInventory.SelectedItemHandler = this;

		StoreButton.isEnabled = false;
		StoreButton.GetComponent<UISprite>().alpha = 1;
		RetrieveButton.isEnabled = false;
		RetrieveButton.GetComponent<UISprite>().alpha = 1;

		QuantitySlider.alpha = 0;
		NGUITools.SetActive(QuantitySlider.gameObject, false);
	}

	public override void Hide ()
	{
		base.Hide ();
		NGUITools.SetActive(ItemTypeTabs.gameObject, false);

		StoreButton.isEnabled = false;
		StoreButton.GetComponent<UISprite>().alpha = 0;
		RetrieveButton.isEnabled = false;
		RetrieveButton.GetComponent<UISprite>().alpha = 0;
	}

	public override void OnItemSelect (InventoryItemEntry itemEntry, InventoryView container)
	{
		if(itemEntry != null)
		{
			_selectedItemContainer = container;
			_selectedItem = itemEntry.ItemData;
			ActionSheet.SetItemTitle(itemEntry.ItemData.Item.DisplayName);

			//build description with item description and attributes
			ActionSheet.SetItemDesc(itemEntry.ItemData.Item.Description);
			ActionSheet.ListItemAttributes(itemEntry.ItemData.Item);


			ActionSheet.SetItemSubnote("", "");


			VaultInventory.DeselectAll();
			CargoAmmoInventory.DeselectAll();

			if(container == VaultInventory)
			{
				StoreButton.isEnabled = false;
				RetrieveButton.isEnabled = true;
			}
			else if(container == CargoAmmoInventory && _currentStationData.HomeStationData != null && _currentStationData.HomeStationData.VaultSize > 0)
			{
				StoreButton.isEnabled = true;
				RetrieveButton.isEnabled = false;
			}

			if(_selectedItem.Quantity > 1)
			{
				QuantitySlider.alpha = 1;
				NGUITools.SetActive(QuantitySlider.gameObject, true);
				QuantitySlider.value = 1;
				_selectedQuantity = _selectedItem.Quantity;
				QuantityLabel.text = _selectedItem.Quantity.ToString();
			}
			else
			{
				QuantitySlider.alpha = 0;
				_selectedQuantity = 1;
				NGUITools.SetActive(QuantitySlider.gameObject, false);
			}
		}
	}

	public override void OnTabSelect (string tabName)
	{
		_itemTypeFilter.Clear();
		if(tabName == "Equipment")
		{
			_itemTypeFilter.Add(ItemType.Equipment);
			_itemTypeFilter.Add(ItemType.ShipMod);
		}
		else if(tabName == "Weapon")
		{
			_itemTypeFilter.Add(ItemType.Weapon);
			_itemTypeFilter.Add(ItemType.Defensives);
		}
		else if(tabName == "Ammo")
		{
			_itemTypeFilter.Add(ItemType.Ammo);
		}
		else if(tabName == "Commodity")
		{
			_itemTypeFilter.Add(ItemType.Commodity);
		}

		Refresh();
	}

	public void Refresh()
	{
		//make a list of all items in the vault
		string stationID = GameManager.Inst.PlayerProgress.SpawnStationID;
		ClearSelections();
		if(GameManager.Inst.WorldManager.DockableStationDatas.ContainsKey(stationID))
		{
			_currentStationData = GameManager.Inst.WorldManager.DockableStationDatas[stationID];

			ShipInventorySheet.InventoryItemTypes = _itemTypeFilter;
			if(_itemTypeFilter.Contains(ItemType.Ammo))
			{
				ShipInventorySheet.InventoryType = InventoryType.AmmoBay;
			}
			else
			{
				ShipInventorySheet.InventoryType = InventoryType.CargoBay;
			}
			ShipInventorySheet.Refresh();
			ShipInventorySheet.RefreshLoadButtons(null);

			if(_currentStationData.HomeStationData != null)
			{
				List<InvItemData> allVaultItems = _currentStationData.HomeStationData.ItemsInVault;
				List<InvItemData> displayedItems = new List<InvItemData>();
				foreach(InvItemData item in allVaultItems)
				{
					if(_itemTypeFilter.Contains(item.Item.Type))
					{
						displayedItems.Add(item);
					}
				}
				VaultInventory.Initialize(displayedItems);
				VaultInventory.RefreshLoadButtons();



				RefreshVaultSpace(_currentStationData.HomeStationData, allVaultItems);
			}

		}
	}

	public void OnRetrieveButtonClick()
	{
		if(_selectedItem != null && _currentStationData != null && _currentStationData.HomeStationData != null)
		{
			if(_selectedItemContainer == VaultInventory)
			{
				//remove item from vault and move to cargo
				List<InvItemData> dest = null;
				if(_selectedItem.Item.Type == ItemType.Ammo)
				{
					dest = CurrentLoadout.AmmoBayItems;
					if(ShipInventorySheet.AvailableAmmoSpaceValue < _selectedItem.Item.CargoUnits * _selectedQuantity)
					{
						GameManager.Inst.UIManager.ErrorMessagePanel.DisplayMessage("NOT ENOUGH SPACE IN AMMO BAY");
						return;
					}
				}
				else
				{
					dest = CurrentLoadout.CargoBayItems;
					if(ShipInventorySheet.AvailableCargoSpaceValue < _selectedItem.Item.CargoUnits * _selectedQuantity)
					{
						GameManager.Inst.UIManager.ErrorMessagePanel.DisplayMessage("NOT ENOUGH SPACE IN CARGO BAY");
						return;
					}
				}

				int itemsTaken = GameManager.Inst.UIManager.TakeItemFromItemDataList(_currentStationData.HomeStationData.ItemsInVault, _selectedItem, _selectedQuantity);
				GameManager.Inst.UIManager.AddItemtoInvItemDataList(dest, _selectedItem, itemsTaken);

				Refresh();
			}
		}
	}

	public void OnStoreButtonClick()
	{
		if(_selectedItem != null && _currentStationData != null && _currentStationData.HomeStationData != null)
		{
			
			if(_selectedItemContainer == CargoAmmoInventory)
			{
				List<InvItemData> source = null;
				if(_selectedItem.Item.Type == ItemType.Ammo)
				{
					source = CurrentLoadout.AmmoBayItems;
				}
				else
				{
					source = CurrentLoadout.CargoBayItems;
				}

				if(AvailableVaultSpaceValue < _selectedItem.Item.CargoUnits * _selectedQuantity)
				{
					GameManager.Inst.UIManager.ErrorMessagePanel.DisplayMessage("NOT ENOUGH SPACE IN VAULT");
					return;
				}

				//remove item from source and add to vault
				int itemsTaken = GameManager.Inst.UIManager.TakeItemFromItemDataList(source, _selectedItem, _selectedQuantity);
				GameManager.Inst.UIManager.AddItemtoInvItemDataList(_currentStationData.HomeStationData.ItemsInVault, _selectedItem, itemsTaken);

				Refresh();
			}
		}
	}

	public void OnSliderValueChange()
	{
		if(_selectedItem != null)
		{
			_selectedQuantity = Mathf.RoundToInt(Mathf.Lerp(1f, (float)_selectedItem.Quantity, QuantitySlider.value));
			QuantityLabel.text = _selectedQuantity.ToString();
		}
	}

	public void ClearSelections()
	{
		CargoAmmoInventory.DeselectAll();
		VaultInventory.DeselectAll();
		_selectedItem = null;
		_selectedItemContainer = null;
		StoreButton.isEnabled = false;
		RetrieveButton.isEnabled = false;
		QuantitySlider.alpha = 0;
		NGUITools.SetActive(QuantitySlider.gameObject, false);
		ActionSheet.Clear();
	}

	private void RefreshVaultSpace(HomeStationData homeStationData, List<InvItemData> vaultItems)
	{
		if(homeStationData == null)
		{
			AvailableVaultSpaceValue = 0;
			VaultSpace.SetFillPercentage(1);
			AvailableVaultSpace.text = "0";

			return;
		}

		int totalVaultSpace = homeStationData.VaultSize;
		float usedSpace = 0;
		foreach(InvItemData itemData in vaultItems)
		{
			usedSpace += itemData.Item.CargoUnits * itemData.Quantity;
		}

		AvailableVaultSpaceValue = Mathf.Clamp(totalVaultSpace - usedSpace, 0, homeStationData.VaultSize);
		VaultSpace.SetFillPercentage(Mathf.Clamp01(usedSpace / totalVaultSpace));
		AvailableVaultSpace.text = Mathf.CeilToInt(AvailableVaultSpaceValue).ToString();
	}

}
