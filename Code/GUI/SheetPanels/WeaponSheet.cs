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
	private InventoryItemEntry _selectedItemEntry;
	private InventoryView _selectedItemContainer;
	private int _selectedWeaponItemIndex;

	//private InvItemData _selectedAmmoItem;
	//private int _selectedAmmoItemIndex;

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

		InstallButton.isEnabled = false;
		InstallButton.GetComponent<UISprite>().alpha = 1;
		RemoveButton.isEnabled = false;
		RemoveButton.GetComponent<UISprite>().alpha = 1;
	}

	public override void Hide ()
	{
		base.Hide();
		ClearSelections();
		CargoAmmoTabs.ForceSelectTab("Cargo");
		NGUITools.SetActive(CargoAmmoTabs.gameObject, false);

		InstallButton.isEnabled = false;
		InstallButton.GetComponent<UISprite>().alpha = 0;
		RemoveButton.isEnabled = false;
		RemoveButton.GetComponent<UISprite>().alpha = 0;
	}

	public override void OnItemSecButtonClick (InventoryItemEntry itemEntry, InventoryView container)
	{
		if(itemEntry.ItemData.Item.Type == ItemType.Ammo && _selectedWeaponItem != null)
		{
			string ammoType = _selectedWeaponItem.Item.GetStringAttribute("Ammo Type");
			if(ammoType == itemEntry.ItemData.Item.GetStringAttribute("Ammo Type") && !string.IsNullOrEmpty(ammoType) && _selectedWeaponItem.Item.Type == ItemType.Weapon)
			{
				_selectedWeaponItem.RelatedItemID = itemEntry.ItemData.Item.ID;
				Refresh();
				OnItemSelect(_selectedItemEntry, _selectedItemContainer);
			}
		}
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
		else if(!string.IsNullOrEmpty(itemEntry.ItemData.Item.GetStringAttribute("Ammo Type")) && (itemEntry.ItemData.Item.Type == ItemType.Weapon || itemEntry.ItemData.Item.Type == ItemType.Defensives ))
		{
			ActionSheet.SetItemSubnote("Current Load", "NONE");
		}
		else
		{
			ActionSheet.SetItemSubnote("", "");
		}

		if(container == WeaponInventory || container == DefensivesInventory || itemEntry.ItemData.Item.Type == ItemType.Weapon || itemEntry.ItemData.Item.Type == ItemType.Defensives)
		{
			_selectedWeaponItem = itemEntry.ItemData;


		}
		else if(container == CargoAmmoInventory)
		{
			_selectedWeaponItem = itemEntry.ItemData;
			
		}

		_selectedItemEntry = itemEntry;

		WeaponInventory.DeselectAll();
		DefensivesInventory.DeselectAll();
		CargoAmmoInventory.DeselectAll();

		if(container == WeaponInventory || container == DefensivesInventory)
		{


			InstallButton.isEnabled = false;
			RemoveButton.isEnabled = true;
		}
		else if(container == CargoAmmoInventory)
		{
			if(itemEntry.ItemData.Item.Type == ItemType.Weapon || itemEntry.ItemData.Item.Type == ItemType.Defensives)
			{

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
					break;
				}
			}

			if(!slotFound)
			{
				GameManager.Inst.UIManager.ErrorMessagePanel.DisplayMessage("NO WEAPON SLOTS AVAILABLE");
			}
			else
			{
				CurrentLoadout.CargoBayItems.Remove(_selectedWeaponItem);
				ClearSelections();
				Refresh();	
			}


		}
		else if(_selectedWeaponItem != null && _selectedWeaponItem.Item.Type == ItemType.Defensives)
		{
			bool slotFound = false;
			for(int i=0; i<CurrentLoadout.Defensives.Count; i++)
			{
				if(CurrentLoadout.Defensives[i] == null)
				{
					//load the weapon
					CurrentLoadout.Defensives[i] = _selectedWeaponItem;
					slotFound = true;
				}
			}

			if(!slotFound)
			{
				GameManager.Inst.UIManager.ErrorMessagePanel.DisplayMessage("NO DEFENSIVE SLOTS AVAILABLE");
			}
			else
			{
				CurrentLoadout.CargoBayItems.Remove(_selectedWeaponItem);
				ClearSelections();
				Refresh();	
			}
		}




	}

	public void OnRemoveButtonClick()
	{
		if(_selectedWeaponItem != null && _selectedWeaponItem.Item.Type == ItemType.Weapon)
		{
			//find which joint this weapon is installed
			Dictionary<string, InvItemData> weaponJointsCopy = new Dictionary<string, InvItemData>(CurrentLoadout.WeaponJoints);
			foreach(KeyValuePair<string, InvItemData> weaponJoint in weaponJointsCopy)
			{
				if(weaponJoint.Value == _selectedWeaponItem)
				{
					//check if cargo has enough space
					if(_selectedWeaponItem.Item.CargoUnits <= ShipInventorySheet.AvailableCargoSpaceValue)
					{
						CurrentLoadout.CargoBayItems.Add(_selectedWeaponItem);
						CurrentLoadout.WeaponJoints[weaponJoint.Key] = null;
					}
					else
					{
						GameManager.Inst.UIManager.ErrorMessagePanel.DisplayMessage("NOT ENOUGH SPACE IN CARGO BAY");
					}
				}
			}
			ClearSelections();
			Refresh();
		}
		else if(_selectedWeaponItem != null && _selectedWeaponItem.Item.Type == ItemType.Defensives)
		{
			for(int i=0; i<CurrentLoadout.Defensives.Count; i++)
			{
				if(CurrentLoadout.Defensives[i] == _selectedWeaponItem)
				{
					if(_selectedWeaponItem.Item.CargoUnits <= ShipInventorySheet.AvailableCargoSpaceValue)
					{
						CurrentLoadout.CargoBayItems.Add(_selectedWeaponItem);
						CurrentLoadout.Defensives[i] = null;
					}
					else
					{
						GameManager.Inst.UIManager.ErrorMessagePanel.DisplayMessage("NOT ENOUGH SPACE IN CARGO BAY");
					}
				}
			}
			ClearSelections();
			Refresh();
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
			else if(shipStats.WeaponJoints[i].RotationType == WeaponRotationType.Turret)
			{
				string jointClass = shipStats.WeaponJoints[i].Class.ToString();
				hintText = "Class " + jointClass + " Turret";
			}
			NGUITools.SetActive(WeaponInventory.ItemEntries[i].gameObject, true);
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
			ShipInventorySheet.InventoryItemTypes.Add(ItemType.Defensives);
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
		//_selectedAmmoItem = null;
		InstallButton.isEnabled = false;
		RemoveButton.isEnabled = false;
		ActionSheet.Clear();
	}
}
