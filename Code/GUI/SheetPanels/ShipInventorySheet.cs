using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipInventorySheet : PanelBase
{
	public Loadout CurrentLoadout;
	public InventoryView Inventory;
	public List<ItemType> InventoryItemTypes;
	public BarIndicator CargoSpace;
	public UILabel AvailableCargoSpace;
	public InventoryType InventoryType;

	public float AvailableCargoSpaceValue;

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

	public void Refresh()
	{
		if(GameManager.Inst.SceneType == SceneType.Station)
		{
			List<InvItemData> itemList = new List<InvItemData>();
			List<InvItemData> source = null; 
			float maxStorageSpace = 0;
			if(InventoryType == InventoryType.CargoBay)
			{
				source = CurrentLoadout.CargoBayItems;
				maxStorageSpace = GameManager.Inst.ItemManager.AllShipStats[CurrentLoadout.ShipID].CargoBaySize;
			}
			else
			{
				source = CurrentLoadout.AmmoBayItems;
				maxStorageSpace = GameManager.Inst.ItemManager.AllShipStats[CurrentLoadout.ShipID].AmmoBaySize;
			}

			foreach(InvItemData itemData in source)
			{
				if(InventoryItemTypes.Contains(itemData.Item.Type))
				{
					itemList.Add(itemData);
				}
			}

			Inventory.Initialize(itemList);

			RefreshCargoSpace(source, maxStorageSpace);
		}
	}

	public void RefreshLoadButtons(InvItemData relatedItem)
	{
		Inventory.RelatedItem = relatedItem;
		Inventory.RefreshLoadButtons();
	}

	private void RefreshCargoSpace(List<InvItemData> source, float maxSpace)
	{
		float usage = 0;
		foreach(InvItemData itemData in source)
		{
			usage += itemData.Item.CargoUnits;
		}

		AvailableCargoSpaceValue = Mathf.Clamp(maxSpace - usage, 0, maxSpace);
		AvailableCargoSpace.text = Mathf.CeilToInt(AvailableCargoSpaceValue).ToString();
		CargoSpace.SetFillPercentage(Mathf.Clamp01(usage / maxSpace));
	}
}

public enum InventoryType
{
	CargoBay,
	AmmoBay,
}