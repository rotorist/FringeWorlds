using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager
{
	public Dictionary<string, ShipStats> AllShipStats { get { return _allShipStats; } }
	public Dictionary<string, ItemStats> AllItemStats { get { return _allItemStats; } }

	private Dictionary<string,ShipStats> _allShipStats;
	private Dictionary<string,ItemStats> _allItemStats;

	public void Initialize()
	{
		_allShipStats = GameManager.Inst.DBManager.JsonDataHandler.LoadAllShipStats();
		_allItemStats = GameManager.Inst.DBManager.JsonDataHandler.LoadAllItemStats();
	}

	public ItemStats GetItemStats(string id)
	{
		if(AllItemStats.ContainsKey(id))
		{
			return AllItemStats[id];
		}
		else
		{
			return null;
		}
	}

	public ShipStats GetShipStats(string id)
	{
		if(AllShipStats.ContainsKey(id))
		{
			return AllShipStats[id];
		}
		else
		{
			return null;
		}
	}

	public InvItemData GenerateInvitemData(string itemID, int quantity)
	{
		Item item = new Item(GameManager.Inst.ItemManager.GetItemStats(itemID));
		InvItemData itemData = new InvItemData();
		itemData.Item = item;
		itemData.Quantity = quantity;
		return itemData;
	}

	public void AddItemtoInvItemDataList(List<InvItemData> itemList, InvItemData target, int quantity)
	{
		bool itemExists = false;
		foreach(InvItemData item in itemList)
		{
			if(item.Item.ID == target.Item.ID)
			{
				Debug.Log("Adding item " + quantity + " to existing " + item.Quantity);
				item.Quantity += quantity;
				itemExists = true;
			}
		}

		if(!itemExists)
		{
			InvItemData newItem = new InvItemData();
			newItem.Item = new Item(GameManager.Inst.ItemManager.GetItemStats(target.Item.ID));
			newItem.Quantity = quantity;
			itemList.Add(newItem);
		}
	}

	public int TakeItemFromItemDataList(List<InvItemData> itemList, InvItemData target, int quantity)
	{
		List<InvItemData> listCopy = new List<InvItemData>(itemList);
		foreach(InvItemData item in listCopy)
		{
			if(item == target)
			{
				int newQuantity = item.Quantity - quantity;
				if(newQuantity <= 0)
				{
					int taken = item.Quantity;
					itemList.Remove(item);
					return taken;
				}
				else
				{
					target.Quantity = newQuantity;
					return quantity;
				}
			}
		}

		return 0;
	}
}
