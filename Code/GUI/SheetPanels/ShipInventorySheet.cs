using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipInventorySheet : PanelBase
{
	public InventoryView Inventory;
	public ItemType InventoryItemType;

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
			foreach(InvItemData itemData in GameManager.Inst.PlayerProgress.ActiveLoadout.CargoBayItems)
			{
				if(itemData.Item.Type == InventoryItemType)
				{
					itemList.Add(itemData);
				}
			}
			Inventory.Initialize(itemList);
		}
	}

}
