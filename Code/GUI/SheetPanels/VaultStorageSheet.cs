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

	private List<ItemType> _itemTypeFilter;
	private InvItemData _selectedItem;
	private InventoryView _selectedItemContainer;
	private DockableStationData _currentStationData;

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

			string loadedAmmoID = itemEntry.ItemData.RelatedItemID;
			if(!string.IsNullOrEmpty(loadedAmmoID))
			{
				string ammoName = GameManager.Inst.ItemManager.GetItemStats(loadedAmmoID).DisplayName;
				ActionSheet.SetItemSubnote("Current Load", ammoName);
			}
			else if(!string.IsNullOrEmpty(itemEntry.ItemData.Item.GetStringAttribute("Ammo Type")) && (itemEntry.ItemData.Item.Type == ItemType.Weapon || itemEntry.ItemData.Item.Type == ItemType.Defensives ))
			{
				ActionSheet.SetItemSubnote("Current Load", "NONE");
			}
			else
			{
				ActionSheet.SetItemSubnote("", "");
			}

			VaultInventory.DeselectAll();
			CargoAmmoInventory.DeselectAll();

			if(container == VaultInventory)
			{
				StoreButton.isEnabled = false;
				RetrieveButton.isEnabled = true;
			}
			else if(container == CargoAmmoInventory)
			{
				StoreButton.isEnabled = true;
				RetrieveButton.isEnabled = false;
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
		//make a list of all ship items in the hangar
		string stationID = GameManager.Inst.PlayerProgress.SpawnStationID;

		if(GameManager.Inst.WorldManager.DockableStationDatas.ContainsKey(stationID))
		{
			_currentStationData = GameManager.Inst.WorldManager.DockableStationDatas[stationID];
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
			}

		}
	}

	public void OnRetrieveButtonClick()
	{

	}

	public void OnStoreButtonClick()
	{
		if(_selectedItem != null && _currentStationData != null && _currentStationData.HomeStationData != null)
		{
			if(_selectedItemContainer == VaultInventory)
			{
				//remove item from cargo and move to vault

			}
			else if(_selectedItemContainer == CargoAmmoInventory)
			{

			}
		}
	}

}
