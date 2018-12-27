using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DockableStationData
{
	public string StationID;
	public List<SaleItem> TraderSaleItems;
	public List<SaleShip> ShipsForSale;
	public List<DemandResource> DemandResources;
	public float FuelPrice;
	public float LifeSupportPrice;
	public float UndesiredItemPriceMultiplier;


	public List<MacroAIParty> DockedParties;

	public HomeStationData HomeStationData;

	public DockableStationData()
	{
		TraderSaleItems = new List<SaleItem>();
		ShipsForSale = new List<SaleShip>();
		DockedParties = new List<MacroAIParty>();

	}

	public float GetSaleItemPriceFactor(string itemID)
	{
		foreach(SaleItem saleItem in TraderSaleItems)
		{
			if(saleItem.ItemID == itemID)
			{
				return saleItem.PriceFactor;
			}
		}

		return 1;
	}
}

[System.Serializable]
public class DockableStationSaveData
{
	public string StationID;
	public List<SaleItem> TraderSaleItems;
	public List<SaleShip> ShipsForSale;
	public List<DemandResource> DemandResources;
	public float FuelPrice;
	public float LifeSupportPrice;
	public List<int> DockedPartiesNumbers;
	public HomeStationSaveData HomeStationSaveData;
	public float UndesiredItemPriceMultiplier;
}

[System.Serializable]
public class DockableStationInitialData
{
	public string StationID;
	public List<SaleItem> TraderSaleItems;
	public List<SaleShip> ShipsForSale;
	public List<DemandResource> DemandResources;
	public float FuelPrice;
	public float LifeSupportPrice;

}
