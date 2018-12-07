using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSheet : PanelBase
{

	public Loadout CurrentLoadout;
	public StaticInventoryView WeaponInventory;
	public StaticInventoryView DefensivesInventory;
	public InventoryView CargoAmmoInventory;
	public EquipmentActionSheet ActionSheet;
	public ShipInventorySheet ShipInventorySheet;

	public UITabSelection CargoAmmoTabs;

	public UIButton InstallButton;
	public UIButton RemoveButton;

	private InvItemData _selectedWeaponItem;
	private InventoryView _selectedItemContainer;
	private int _selectedWeaponItemIndex;

	private InvItemData _selectedAmmoItem;
	private int _selectedAmmoItemIndex;

	private List<InvItemData> _installedDefensiveItems;

	private Dictionary<string, InventoryItemEntry> _weaponSlots;


	public override void Initialize ()
	{
		base.Initialize();
		NGUITools.SetActive(CargoAmmoTabs.gameObject, false);
	}

	public override void Show ()
	{
		base.Show();
		ClearSelections();
		NGUITools.SetActive(CargoAmmoTabs.gameObject, true);
		CargoAmmoTabs.ForceSelectTab("Cargo");
		CargoAmmoInventory.SelectedItemHandler = this;
	}

	public override void Hide ()
	{
		base.Hide();
		ClearSelections();
		CargoAmmoTabs.ForceSelectTab("Cargo");
		NGUITools.SetActive(CargoAmmoTabs.gameObject, false);
	}

	public override void OnItemSelect (InventoryItemEntry itemEntry, InventoryView container)
	{
		_selectedItemContainer = container;
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
		else
		{
			ActionSheet.SetItemSubnote("", "");
		}

		if(container == WeaponInventory || container == DefensivesInventory || itemEntry.ItemData.Item.Type == ItemType.Weapon)
		{
			_selectedWeaponItem = itemEntry.ItemData;
			_selectedWeaponItemIndex = itemEntry.InventoryIndex;

		}
		else if(container == CargoAmmoInventory)
		{
			_selectedAmmoItem = itemEntry.ItemData;
			_selectedAmmoItemIndex = itemEntry.InventoryIndex;

		}

		if(container == WeaponInventory || container == DefensivesInventory)
		{
			WeaponInventory.DeselectAll();
			DefensivesInventory.DeselectAll();
			if(ShipInventorySheet.InventoryType == InventoryType.CargoBay)
			{
				CargoAmmoInventory.DeselectAll();
			}
			InstallButton.isEnabled = false;
			RemoveButton.isEnabled = true;
		}
		else if(container == CargoAmmoInventory)
		{
			CargoAmmoInventory.DeselectAll();

			if(itemEntry.ItemData.Item.Type == ItemType.Weapon)
			{
				WeaponInventory.DeselectAll();
				DefensivesInventory.DeselectAll();
				InstallButton.isEnabled = true;
				RemoveButton.isEnabled = false;
			}
			else if(itemEntry.ItemData.Item.Type == ItemType.Ammo)
			{
				InstallButton.isEnabled = false;
				RemoveButton.isEnabled = false;
			}
		}

		ShipInventorySheet.RefreshLoadButtons(_selectedWeaponItem);
	}

	public override void OnTabSelect (string tabName)
	{
		if(tabName == "Cargo")
		{
			ShipInventorySheet.InventoryType = InventoryType.CargoBay;
			ShipInventorySheet.InventoryItemTypes.Add(ItemType.Weapon);
			ShipInventorySheet.InventoryItemTypes.Add(ItemType.Defensives);
		}
		else
		{
			ShipInventorySheet.InventoryType = InventoryType.AmmoBay;
			ShipInventorySheet.InventoryItemTypes.Add(ItemType.Ammo);
		}
		ShipInventorySheet.Refresh();
		ShipInventorySheet.RefreshLoadButtons(_selectedWeaponItem);
	}

	public void OnInstallButtonClick()
	{
		ShipStats shipStats = GameManager.Inst.ItemManager.AllShipStats[CurrentLoadout.ShipID];

		if(_selectedWeaponItem != null && _selectedWeaponItem.Item.Type == ItemType.Weapon)
		{
			bool slotFound = false;
			//find the first available weapon slot that >= weapon item's class
			for(int i=0; i<shipStats.WeaponJoints.Count; i++)
			{
				if(CurrentLoadout.WeaponJoints[shipStats.WeaponJoints[i].JointID] == null && shipStats.WeaponJoints[i].Class >= _selectedWeaponItem.Item.GetIntAttribute("Weapon Class"))
				{
					CurrentLoadout.SetWeaponInvItem(shipStats.WeaponJoints[i].JointID, _selectedWeaponItem);
					slotFound = true;
				}
			}

			if(!slotFound)
			{
				GameManager.Inst.UIManager.ErrorMessagePanel.DisplayMessage("NO WEAPON SLOTS AVAILABLE");
			}

		}

	}

	public void Refresh()
	{
		_weaponSlots = new Dictionary<string, InventoryItemEntry>();
		List<InvItemData> weaponItems = new List<InvItemData>();
		//see how many weapon slots ship has and hide slots not used
		//set hint text for visible slots
		//from current loadout create list of weapons and initialize weaponInventory
		ShipStats shipStats = GameManager.Inst.ItemManager.AllShipStats[CurrentLoadout.ShipID];
		for(int i=0; i<shipStats.WeaponJoints.Count; i++)
		{
			string hintText = "";
			if(shipStats.WeaponJoints[i].RotationType == WeaponRotationType.Gimball)
			{
				string jointClass = shipStats.WeaponJoints[i].Class.ToString();
				hintText = "Class " + jointClass + " Gun/Missile";
			}
			WeaponInventory.ItemEntries[i].SetHintText(hintText);
			_weaponSlots.Add(shipStats.WeaponJoints[i].JointID, WeaponInventory.ItemEntries[i]);

			if(CurrentLoadout.WeaponJoints[shipStats.WeaponJoints[i].JointID] == null)
			{
				weaponItems.Add(null);
			}
			else
			{
				weaponItems.Add(CurrentLoadout.WeaponJoints[shipStats.WeaponJoints[i].JointID]);
			}
		}

		WeaponInventory.Initialize(weaponItems);


		for(int i=shipStats.WeaponJoints.Count; i<WeaponInventory.ItemEntries.Count; i++)
		{
			NGUITools.SetActive(WeaponInventory.ItemEntries[i].gameObject, false);
		}

		//generate list for defensives
		List<InvItemData> defensiveItems = new List<InvItemData>();
		for(int i=0; i<shipStats.DefenseSlots; i++)
		{
			defensiveItems.Add(CurrentLoadout.Defensives[i]);

		}

		for(int i=shipStats.DefenseSlots; i<DefensivesInventory.ItemEntries.Count; i++)
		{
			NGUITools.SetActive(DefensivesInventory.ItemEntries[i].gameObject, false);
		}

		_installedDefensiveItems = defensiveItems;
		DefensivesInventory.Initialize(defensiveItems);

		//show cargo or ammo bay
		ShipInventorySheet.InventoryItemTypes.Clear();
		if(ShipInventorySheet.InventoryType == InventoryType.CargoBay)
		{
			ShipInventorySheet.InventoryItemTypes.Add(ItemType.Weapon);
		}
		else if(ShipInventorySheet.InventoryType == InventoryType.AmmoBay)
		{
			ShipInventorySheet.InventoryItemTypes.Add(ItemType.Ammo);
		}
		ShipInventorySheet.Refresh();
		ShipInventorySheet.RefreshLoadButtons(_selectedWeaponItem);
	}


	public void ClearSelections()
	{
		WeaponInventory.DeselectAll();
		CargoAmmoInventory.DeselectAll();
		DefensivesInventory.DeselectAll();
		_selectedWeaponItem = null;
		_selectedItemContainer = null;
		_selectedAmmoItem = null;
		InstallButton.isEnabled = false;
		RemoveButton.isEnabled = false;
		ActionSheet.Clear();
	}
}
