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
}
