using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentSheet : PanelBase
{
	public InventoryView EquipmentInventory;
	public InventoryView CargoEquipmentInventory;
	public EquipmentActionSheet ActionSheet;

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
	}

	public override void Hide ()
	{
		base.Hide();
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

	}

	public void OnRemoveButtonClick()
	{

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

		EquipmentInventory.Refresh();
	}



}
