using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipInventorySheet : PanelBase
{
	public Loadout CurrentLoadout;
	public InventoryView Inventory;
	public List<ItemType> InventoryItemTypes;
	public BarIndicator CargoSpace;
	public BarIndicator ProposedCargoUsage;
	public UILabel AvailableCargoSpace;
	public InventoryType InventoryType;
	public UILabel Title;

	public float AvailableCargoSpaceValue;
	public float AvailableAmmoSpaceValue;

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
				Title.text = "CARGO BAY";
				RefreshCargoSpace(source, maxStorageSpace);
			}
			else
			{
				source = CurrentLoadout.AmmoBayItems;
				maxStorageSpace = GameManager.Inst.ItemManager.AllShipStats[CurrentLoadout.ShipID].AmmoBaySize;
				Title.text = "AMMO BAY";
				RefreshAmmoSpace(source, maxStorageSpace);
			}

			foreach(InvItemData itemData in source)
			{
				if(InventoryItemTypes.Contains(itemData.Item.Type))
				{
					itemList.Add(itemData);
				}
			}

			Inventory.Initialize(itemList);

			RefreshProposedCargoUsage(0);
		}
	}

	public void RefreshProposedCargoUsage(float additionalUsage)
	{
		if(ProposedCargoUsage != null)
		{
			UISprite indicatorSprite = ProposedCargoUsage.GetComponent<UISprite>();
			if(additionalUsage >= 0)
			{
				indicatorSprite.depth = 9;
			}
			else
			{
				indicatorSprite.depth = 11;
			}

			if(InventoryType == InventoryType.CargoBay)
			{
				float maxSpace = GameManager.Inst.ItemManager.AllShipStats[CurrentLoadout.ShipID].CargoBaySize;
				float newUsage = additionalUsage + (maxSpace - AvailableCargoSpaceValue);
				if(newUsage <= maxSpace)
				{
					indicatorSprite.color = new Color(1, 0.75f, 0);
					ProposedCargoUsage.SetFillPercentage(Mathf.Clamp01(newUsage / maxSpace));
				}
				else
				{
					indicatorSprite.color = new Color(1, 0, 0);
					ProposedCargoUsage.SetFillPercentage(1);
				}
			}
			else if(InventoryType == InventoryType.AmmoBay)
			{
				float maxSpace = GameManager.Inst.ItemManager.AllShipStats[CurrentLoadout.ShipID].AmmoBaySize;
				float newUsage = additionalUsage + (maxSpace - AvailableAmmoSpaceValue);
				if(newUsage <= maxSpace)
				{
					ProposedCargoUsage.GetComponent<UISprite>().color = new Color(1, 0.75f, 0);
					ProposedCargoUsage.SetFillPercentage(Mathf.Clamp01(newUsage / maxSpace));
				}
				else
				{
					ProposedCargoUsage.GetComponent<UISprite>().color = new Color(1, 0, 0);
					ProposedCargoUsage.SetFillPercentage(1);
				}
			}
		}
	}

	public void RefreshLoadButtons(InvItemData relatedItem)
	{

		if(relatedItem != null && (relatedItem.Item.Type == ItemType.Weapon || relatedItem.Item.Type == ItemType.Defensives))
		{
			Inventory.RelatedItem = relatedItem;
		}
		else
		{
			Inventory.RelatedItem = null;
		}

		Inventory.RefreshLoadButtons();
	}

	private void RefreshCargoSpace(List<InvItemData> source, float maxSpace)
	{
		float usage = 0;
		foreach(InvItemData itemData in source)
		{
			usage += itemData.Item.CargoUnits * itemData.Quantity;
		}

		AvailableCargoSpaceValue = Mathf.Clamp(maxSpace - usage, 0, maxSpace);
		AvailableCargoSpace.text = Mathf.CeilToInt(AvailableCargoSpaceValue).ToString();
		CargoSpace.SetFillPercentage(Mathf.Clamp01(usage / maxSpace));
	}

	private void RefreshAmmoSpace(List<InvItemData> source, float maxSpace)
	{
		float usage = 0;
		foreach(InvItemData itemData in source)
		{
			usage += itemData.Item.CargoUnits * itemData.Quantity;
		}

		AvailableAmmoSpaceValue = Mathf.Clamp(maxSpace - usage, 0, maxSpace);
		AvailableCargoSpace.text = Mathf.CeilToInt(AvailableAmmoSpaceValue).ToString();
		CargoSpace.SetFillPercentage(Mathf.Clamp01(usage / maxSpace));
	}

}

public enum InventoryType
{
	CargoBay,
	AmmoBay,
}