using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager
{
	public Dictionary<string, ShipStats> AllShipStats { get { return _allShipStats; } }


	private Dictionary<string,ShipStats> _allShipStats;

	public void Initialize()
	{
		_allShipStats = GameManager.Inst.DBManager.JsonDataHandler.LoadAllShipStats();
	}
}
