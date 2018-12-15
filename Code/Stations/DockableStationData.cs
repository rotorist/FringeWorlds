﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DockableStationData
{
	public string StationID;
	public List<SaleItem> TraderSaleItems;
	public List<SaleShip> ShipsForSale;
	public float FuelPrice;
	public float LifeSupportPrice;

	public List<MacroAIParty> DockedParties;

	public HomeStationData HomeStationData;

	public DockableStationData()
	{
		TraderSaleItems = new List<SaleItem>();
		ShipsForSale = new List<SaleShip>();
		DockedParties = new List<MacroAIParty>();

	}
}

[System.Serializable]
public class DockableStationSaveData
{
	public List<SaleItem> TraderSaleItems;
	public List<SaleShip> ShipsForSale;
	public float FuelPrice;
	public float LifeSupportPrice;
	public List<int> DockedPartiesNumbers;
	public HomeStationSaveData HomeStationSaveData;
}

[System.Serializable]
public class DockableStationInitialData
{
	public string StationID;
	public List<SaleItem> TraderSaleItems;
	public List<SaleShip> ShipsForSale;
	public float FuelPrice;
	public float LifeSupportPrice;

}
