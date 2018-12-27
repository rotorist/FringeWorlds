using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraderInventorySheet : PanelBase
{
	public Loadout CurrentLoadout;
	public InventoryView TraderInventory;
	public InventoryView CargoAmmoInventory;
	public ShipInventorySheet ShipInventorySheet;
	public EquipmentActionSheet ActionSheet;



	public UITabSelection ItemTypeTabs;
	public UIButton SellButton;
	public UIButton BuyButton;
	public UISlider QuantitySlider;
	public UILabel QuantityLabel;
	public UILabel PlayerMoneyValue;
	public UILabel TotalPriceValue;

	private List<ItemType> _itemTypeFilter;
	private InventoryItemEntry _selectedItemEntry;
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

		SellButton.isEnabled = false;
		SellButton.GetComponent<UISprite>().alpha = 1;
		BuyButton.isEnabled = false;
		BuyButton.GetComponent<UISprite>().alpha = 1;

		QuantitySlider.alpha = 0;
		NGUITools.SetActive(QuantitySlider.gameObject, false);
		TotalPriceValue.alpha = 0;
	}

	public override void Hide ()
	{
		base.Hide ();
		NGUITools.SetActive(ItemTypeTabs.gameObject, false);

		SellButton.isEnabled = false;
		SellButton.GetComponent<UISprite>().alpha = 0;
		BuyButton.isEnabled = false;
		BuyButton.GetComponent<UISprite>().alpha = 0;
	}

	public override void OnItemSelect (InventoryItemEntry itemEntry, InventoryView container)
	{
		if(itemEntry != null)
		{
			_selectedItemContainer = container;
			_selectedItemEntry = itemEntry;
			ActionSheet.SetItemTitle(itemEntry.ItemData.Item.DisplayName);

			//build description with item description and attributes
			ActionSheet.SetItemDesc(itemEntry.ItemData.Item.Description);
			ActionSheet.ListItemAttributes(itemEntry.ItemData.Item);


			ActionSheet.SetItemSubnote("", "");


			TraderInventory.DeselectAll();
			CargoAmmoInventory.DeselectAll();

			if(container == TraderInventory)
			{
				SellButton.isEnabled = false;
				BuyButton.isEnabled = true;
			}
			else if(container == CargoAmmoInventory)
			{
				SellButton.isEnabled = true;
				BuyButton.isEnabled = false;
			}

			if(_selectedItemEntry.ItemData.Item.Type == ItemType.Ammo || _selectedItemEntry.ItemData.Item.Type == ItemType.Commodity)
			{
				QuantitySlider.alpha = 1;
				NGUITools.SetActive(QuantitySlider.gameObject, true);
				QuantitySlider.value = 0;
				_selectedQuantity = 1;
				QuantityLabel.text = "1";
				TotalPriceValue.alpha = 1;
				TotalPriceValue.text = "$" + Mathf.CeilToInt(_selectedItemEntry.Price * _selectedQuantity).ToString("#,#");
			}
			else if(_selectedItemEntry.ItemData.Quantity > 1)
			{
				QuantitySlider.alpha = 1;
				NGUITools.SetActive(QuantitySlider.gameObject, true);
				QuantitySlider.value = 1;
				_selectedQuantity = _selectedItemEntry.ItemData.Quantity;
				QuantityLabel.text = _selectedItemEntry.ItemData.Quantity.ToString();
				TotalPriceValue.alpha = 1;
				TotalPriceValue.text = "$" + Mathf.CeilToInt(_selectedItemEntry.Price * _selectedQuantity).ToString("#,#");
			}
			else
			{
				QuantitySlider.alpha = 0;
				_selectedQuantity = 1;
				NGUITools.SetActive(QuantitySlider.gameObject, false);
				TotalPriceValue.alpha = 1;
				TotalPriceValue.text = "$" + Mathf.CeilToInt(_selectedItemEntry.Price).ToString("#,#");
			}
		}
	}

	public override void OnTabSelect (string tabName)
	{
		
		if(tabName == "Equipment")
		{
			_itemTypeFilter.Clear();
			_itemTypeFilter.Add(ItemType.Equipment);
			_itemTypeFilter.Add(ItemType.ShipMod);
		}
		else if(tabName == "Weapon")
		{
			_itemTypeFilter.Clear();
			_itemTypeFilter.Add(ItemType.Weapon);
			_itemTypeFilter.Add(ItemType.Defensives);
		}
		else if(tabName == "Ammo")
		{
			_itemTypeFilter.Clear();
			_itemTypeFilter.Add(ItemType.Ammo);
		}
		else if(tabName == "Commodity")
		{
			_itemTypeFilter.Clear();
			_itemTypeFilter.Add(ItemType.Commodity);
		}

		Refresh();
	}

	public void Refresh()
	{
		//make a list of all items trader is selling
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

			if(_currentStationData != null && _currentStationData.TraderSaleItems != null)
			{
				List<InvItemData> displayedItems = new List<InvItemData>();
				foreach(SaleItem saleItem in _currentStationData.TraderSaleItems)
				{
					ItemStats itemStats = GameManager.Inst.ItemManager.GetItemStats(saleItem.ItemID);
					Item item = new Item(itemStats);
					InvItemData invItem = new InvItemData();
					invItem.Item = item;
					invItem.Quantity = saleItem.Quantity;
					if(_itemTypeFilter.Contains(itemStats.Type))
					{
						
						displayedItems.Add(invItem);
					}
				}

				TraderInventory.Initialize(displayedItems);
				TraderInventory.RefreshLoadButtons();

			}

			RefreshPrices();
			RefreshPlayerMoney();
		}
	}

	public void OnBuyButtonClick()
	{
		if(_selectedItemEntry != null && _currentStationData != null)
		{
			if(_selectedItemContainer == TraderInventory)
			{
				if(_selectedQuantity * _selectedItemEntry.Price > GameManager.Inst.PlayerProgress.Credits)
				{
					GameManager.Inst.UIManager.ErrorMessagePanel.DisplayMessage("NOT ENOUGH CREDITS");
					return;
				}

				//remove item from trader and move to cargo
				List<InvItemData> dest = null;
				if(_selectedItemEntry.ItemData.Item.Type == ItemType.Ammo)
				{
					dest = CurrentLoadout.AmmoBayItems;
					if(ShipInventorySheet.AvailableAmmoSpaceValue < _selectedItemEntry.ItemData.Item.CargoUnits * _selectedQuantity)
					{
						GameManager.Inst.UIManager.ErrorMessagePanel.DisplayMessage("NOT ENOUGH SPACE IN AMMO BAY");
						return;
					}


					GameManager.Inst.UIManager.AddItemtoInvItemDataList(dest, _selectedItemEntry.ItemData, _selectedQuantity);
				}
				else if(_selectedItemEntry.ItemData.Item.Type == ItemType.Commodity)
				{
					dest = CurrentLoadout.CargoBayItems;
					if(ShipInventorySheet.AvailableCargoSpaceValue < _selectedItemEntry.ItemData.Item.CargoUnits * _selectedQuantity)
					{
						GameManager.Inst.UIManager.ErrorMessagePanel.DisplayMessage("NOT ENOUGH SPACE IN CARGO BAY");
						return;
					}

					//find the sale item in station with this item ID and remove the quantity
					List<SaleItem> traderItemsCopy = new List<SaleItem>(_currentStationData.TraderSaleItems);
					foreach(SaleItem saleItem in traderItemsCopy)
					{
						if(saleItem.ItemID == _selectedItemEntry.ItemData.Item.ID)
						{
							saleItem.Quantity -= _selectedQuantity;
							if(saleItem.Quantity <= 0)
							{
								_currentStationData.TraderSaleItems.Remove(saleItem);
							}
						}
					}
					GameManager.Inst.UIManager.AddItemtoInvItemDataList(dest, _selectedItemEntry.ItemData, _selectedQuantity);
				}
				else
				{
					dest = CurrentLoadout.CargoBayItems;
					if(ShipInventorySheet.AvailableCargoSpaceValue < _selectedItemEntry.ItemData.Item.CargoUnits * _selectedQuantity)
					{
						GameManager.Inst.UIManager.ErrorMessagePanel.DisplayMessage("NOT ENOUGH SPACE IN CARGO BAY");
						return;
					}
					dest.Add(_selectedItemEntry.ItemData);
				}

				GameManager.Inst.PlayerProgress.Credits -= Mathf.FloorToInt(_selectedQuantity * _selectedItemEntry.Price);

				Refresh();
			}
		}
	}

	public void OnSellButtonClick()
	{
		if(_selectedItemEntry != null && _currentStationData != null)
		{

			if(_selectedItemContainer == CargoAmmoInventory)
			{
				List<InvItemData> source = null;
				if(_selectedItemEntry.ItemData.Item.Type == ItemType.Ammo)
				{
					source = CurrentLoadout.AmmoBayItems;
				}
				else
				{
					source = CurrentLoadout.CargoBayItems;
				}

				//remove item from source and add to vault
				int itemsTaken = GameManager.Inst.UIManager.TakeItemFromItemDataList(source, _selectedItemEntry.ItemData, _selectedQuantity);
				GameManager.Inst.PlayerProgress.Credits += Mathf.FloorToInt(itemsTaken * _selectedItemEntry.Price);

				//find the sale item in station with this item ID and remove the quantity
				List<SaleItem> traderItemsCopy = new List<SaleItem>(_currentStationData.TraderSaleItems);
				foreach(SaleItem saleItem in traderItemsCopy)
				{
					if(saleItem.ItemID == _selectedItemEntry.ItemData.Item.ID)
					{
						saleItem.Quantity += _selectedQuantity;
					}
				}

				Refresh();
			}
		}
	}

	public void OnSliderValueChange()
	{
		if(_selectedItemEntry != null && _currentStationData != null)
		{
			if((_selectedItemEntry.ItemData.Item.Type == ItemType.Ammo || _selectedItemEntry.ItemData.Item.Type == ItemType.Commodity) && _selectedItemContainer == TraderInventory)
			{
				int maxQuantity = Mathf.FloorToInt(GameManager.Inst.PlayerProgress.Credits / (_selectedItemEntry.Price));
				if(maxQuantity <= 1)
				{
					maxQuantity = 1;
				}
				_selectedQuantity = Mathf.RoundToInt(Mathf.Lerp(1f, maxQuantity, QuantitySlider.value));
				QuantityLabel.text = _selectedQuantity.ToString();
			}
			else
			{
				_selectedQuantity = Mathf.RoundToInt(Mathf.Lerp(1f, (float)_selectedItemEntry.ItemData.Quantity, QuantitySlider.value));
				QuantityLabel.text = _selectedQuantity.ToString();
			}

			if(_selectedItemContainer == TraderInventory)
			{
				ShipInventorySheet.RefreshProposedCargoUsage(_selectedQuantity * _selectedItemEntry.ItemData.Item.CargoUnits);
			}
			else
			{
				ShipInventorySheet.RefreshProposedCargoUsage(_selectedQuantity * _selectedItemEntry.ItemData.Item.CargoUnits * -1);
			}

			TotalPriceValue.alpha = 1;
			TotalPriceValue.text = "$" + Mathf.CeilToInt(_selectedItemEntry.Price * _selectedQuantity).ToString("#,#");
		}
	}

	public void ClearSelections()
	{
		CargoAmmoInventory.DeselectAll();
		TraderInventory.DeselectAll();
		_selectedItemEntry = null;
		_selectedItemContainer = null;
		SellButton.isEnabled = false;
		BuyButton.isEnabled = false;
		QuantitySlider.alpha = 0;
		TotalPriceValue.alpha = 0;
		NGUITools.SetActive(QuantitySlider.gameObject, false);
		ActionSheet.Clear();
	}



	private void RefreshPrices()
	{
		foreach(InventoryItemEntry itemEntry in CargoAmmoInventory.ItemEntries)
		{
			itemEntry.SetItemPrice(GameManager.Inst.EconomyManager.GetItemSellPrice(itemEntry.ItemData.Item, _currentStationData));
		}

		foreach(InventoryItemEntry itemEntry in TraderInventory.ItemEntries)
		{
			itemEntry.SetItemPrice(itemEntry.ItemData.Item.BasePrice * _currentStationData.GetSaleItemPriceFactor(itemEntry.ItemData.Item.ID));
		}
	}

	private void RefreshPlayerMoney()
	{
		PlayerMoneyValue.text = "$" + GameManager.Inst.PlayerProgress.Credits.ToString("#,#");
	}
}
