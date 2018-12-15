using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipHangarSheet : PanelBase
{
	public StaticInventoryView DockedShipInventory;
	public InventoryView HangarInventory;
	public ShipInfoSheet ShipInfoSheet;
	public ShipDataSheet ShipDataSheet;
	public BarIndicator HangarSpace;
	public UILabel AvailableHangarSpace;
	public float AvailableHangarSpaceValue;

	private InventoryItemEntry _selectedEntry;
	private DockableStationData _currentStationData;

	public override void Initialize ()
	{
		base.Initialize ();
	}

	public override void Show ()
	{
		base.Show ();
	}

	public override void Hide ()
	{
		base.Hide ();
	}

	public override void OnItemSelect (InventoryItemEntry itemEntry, InventoryView container)
	{
		DockedShipInventory.DeselectAll();
		HangarInventory.DeselectAll();
		_selectedEntry = itemEntry;

		Loadout selectedLoadout = null;
		//check if item is active loadout
		if(itemEntry.ItemData.Item.Description == GameManager.Inst.PlayerProgress.ActiveLoadout.LoadoutID)
		{
			selectedLoadout = GameManager.Inst.PlayerProgress.ActiveLoadout;
		}
		else
		{
			//check if item is in hangar
			selectedLoadout = FindLoadoutInHangar(itemEntry.ItemData.Item.Description);
		}
		ShipInfoSheet.CurrentLoadout = selectedLoadout;
		ShipInfoSheet.Refresh();
		ShipDataSheet.CurrentLoadout = selectedLoadout;
		ShipDataSheet.Refresh();
	}

	public override void OnItemSecButtonClick (InventoryItemEntry itemEntry, InventoryView container)
	{
		if(itemEntry.ItemData.Item.Type == ItemType.HangarItem)
		{
			OnSwitchToShip(itemEntry.ItemData);
			Refresh();
			OnItemSelect(itemEntry, container);

		}
	}

	public void OnSwitchToShip(InvItemData shipItem)
	{
		string loadoutID = shipItem.Item.Description;
		Loadout loadout = FindLoadoutInHangar(loadoutID);
		if(loadout != null)
		{
			//check if there's enough hangar space
			InvItemData activeShipItemData = this.GetShipItemDataFromLoadout(GameManager.Inst.PlayerProgress.ActiveLoadout);
			int activeShipUnits = activeShipItemData.Item.GetIntAttribute("Hangar Units");
			int newShipUnits = shipItem.Item.GetIntAttribute("Hangar Units");
			int hangarSpaceAfterSwitch = Mathf.FloorToInt(AvailableHangarSpaceValue + newShipUnits - activeShipUnits);
			if(hangarSpaceAfterSwitch >= 0)
			{
				_currentStationData.HomeStationData.ShipsInHangar.Remove(loadout);
				_currentStationData.HomeStationData.ShipsInHangar.Add(GameManager.Inst.PlayerProgress.ActiveLoadout);
				GameManager.Inst.PlayerProgress.ActiveLoadout = loadout;
				ShipInfoSheet.CurrentLoadout = loadout;
				ShipDataSheet.CurrentLoadout = loadout;
				GameManager.Inst.UIManager.ShipInfoPanel.ResetLoadout();
				ShipInfoSheet.Refresh();
				ShipDataSheet.Refresh();
			}
			else
			{
				GameManager.Inst.UIManager.ErrorMessagePanel.DisplayMessage("NOT ENOUGH SPACE IN HANGAR");
			}

		}

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
				List<InvItemData> shipHangarItems = new List<InvItemData>();
				foreach(Loadout loadout in _currentStationData.HomeStationData.ShipsInHangar)
				{	
					shipHangarItems.Add(GetShipItemDataFromLoadout(loadout));
				}

				HangarInventory.Initialize(shipHangarItems);
				HangarInventory.RefreshLoadButtons();
				RefreshHangarSpace(_currentStationData.HomeStationData, shipHangarItems);
			}
			else
			{
				RefreshHangarSpace(null, null);
			}
		}
		//fill the current ship inventory
		List<InvItemData> dockedShipItems = new List<InvItemData>();
		dockedShipItems.Add(GetShipItemDataFromLoadout(GameManager.Inst.PlayerProgress.ActiveLoadout));
		DockedShipInventory.Initialize(dockedShipItems);

	}

	private void RefreshHangarSpace(HomeStationData homeStationData, List<InvItemData> hangarItems)
	{
		if(homeStationData == null)
		{
			AvailableHangarSpaceValue = 0;
			HangarSpace.SetFillPercentage(1);
			AvailableHangarSpace.text = "0";

			return;
		}

		int totalHangarSpace = homeStationData.HangarSize;
		float usedSpace = 0;
		foreach(InvItemData itemData in hangarItems)
		{
			usedSpace += itemData.Item.GetIntAttribute("Hangar Units");
		}

		AvailableHangarSpaceValue = totalHangarSpace - usedSpace;
		HangarSpace.SetFillPercentage(usedSpace / totalHangarSpace);
		AvailableHangarSpace.text = AvailableHangarSpaceValue.ToString();
	}

	private Loadout FindLoadoutInHangar(string loadoutID)
	{
		if(loadoutID == GameManager.Inst.PlayerProgress.ActiveLoadout.LoadoutID)
		{
			return GameManager.Inst.PlayerProgress.ActiveLoadout;
		}

		if(_currentStationData.HomeStationData != null)
		{
			foreach(Loadout loadout in _currentStationData.HomeStationData.ShipsInHangar)
			{
				if(loadout.LoadoutID == loadoutID)
				{
					return loadout;
				}
			}
		}

		return null;
	}

	private InvItemData GetShipItemDataFromLoadout(Loadout loadout)
	{
		Debug.Log("Creating ship item " + loadout.ShipID);
		Item shipItem = new Item(GameManager.Inst.ItemManager.AllItemStats["ship_" + loadout.ShipID]);
		shipItem.DisplayName = GameManager.Inst.ItemManager.AllShipStats[loadout.ShipID].DisplayName;
		shipItem.Description = loadout.LoadoutID;
		InvItemData shipInvItem = new InvItemData();
		shipInvItem.Item = shipItem;
		shipInvItem.Quantity = 1;

		return shipInvItem;
	}
}
