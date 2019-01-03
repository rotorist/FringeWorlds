using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EconomyManager
{
	public float ConvoyBuyItemSupplyFactor = 0.001f;
	public float ConvoySellItemDemandFactor = 0.001f;

	public void Initialize()
	{
		RefreshStationSaleItems();
		SetInitialResourceDemand();
		GameEventHandler.OnHour -= PerHourUpdate;
		GameEventHandler.OnHour += PerHourUpdate;
	}


	public void PerHourUpdate()
	{
		foreach(KeyValuePair<string, DockableStationData> stationData in GameManager.Inst.WorldManager.DockableStationDatas)
		{
			RefreshDemandLevel(stationData.Value);
			RefreshSupplyLevel(stationData.Value);
		}
	}

	public void PerDayUpdate()
	{

	}

	public void RefreshDemandLevel(DockableStationData stationData)
	{
		//demand level slowly moves towards initial value
		//at the speed determined by the station data
		foreach(DemandResource resource in stationData.DemandResources)
		{
			//
			if(resource.CurrentDemand <= resource.DemandLevel)
			{
				resource.CurrentDemand = Mathf.Lerp(resource.CurrentDemand, resource.DemandLevel, stationData.DemandNormalizeSpeed);
			}
			else
			{
				resource.CurrentDemand += stationData.DemandNormalizeSpeed * (1 - Mathf.Clamp01((resource.CurrentDemand - resource.DemandLevel) / 1.5f));
			}

			if(resource.ItemID != null)
			{
				Debug.Log(resource.ItemID.ToString() + " , " + resource.CurrentDemand);
			}
		}
	}

	public void RefreshSupplyLevel(DockableStationData stationData)
	{
		//supply level for commodity slowly grows when there's no econ events affecting it
		//at the speed of DemandNormalizeSpeed
		//max at 4 for now
		foreach(SaleItem saleItem in stationData.TraderSaleItems)
		{
			ItemStats stats = GameManager.Inst.ItemManager.GetItemStats(saleItem.ItemID);
			if(stats.Type == ItemType.Commodity)
			{
				if(saleItem.SupplyLevel < 1)
				{
					saleItem.SupplyLevel += stationData.DemandNormalizeSpeed;
				}
				else
				{
					saleItem.SupplyLevel = saleItem.SupplyLevel + stationData.DemandNormalizeSpeed * Mathf.Clamp01( 1 - (saleItem.SupplyLevel - 1) / 2f);
				}
			}
		}
	}

	public float GetItemSellPrice(Item item, DockableStationData stationData)
	{
		if(item.Type == ItemType.Commodity)
		{
			//if the station has static needs for the resource, multiply base price with stations's demand item demand level
			//if item provides a resource that the station demands then multiply the base price with station's demand level for the resource
			//otherwise multiply base price with a small number between 0.1 and 0.4
			ResourceType resourceType = item.GetResourceTypeAttribute();
			foreach(DemandResource demandResource in stationData.DemandResources)
			{
				if(demandResource.ItemID == item.ID)
				{
					return item.BasePrice * demandResource.CurrentDemand;
				}
				else if(demandResource.Type == resourceType)
				{
					return item.BasePrice * demandResource.CurrentDemand;
				}
			}

			return item.BasePrice * stationData.UndesiredItemPriceMultiplier;
		}
		else
		{
			return item.BasePrice * 0.25f;
		}
		return 0;
	}




	public void OnConvoyBuyCommodity(DockableStationData station, string itemID, int quantity)
	{
		Debug.Log("Convoy bought commodity " + quantity.ToString() + "x " + itemID + " in station " + station.StationID);
		GameManager.Inst.UIManager.EconDebugPanel.SetConvoyTradeEvent("Convoy bought commodity " + quantity.ToString() + "x " + itemID + " in station " + station.StationID);
		foreach(SaleItem item in station.TraderSaleItems)
		{
			if(itemID == item.ItemID)
			{
				item.SupplyLevel -= quantity * ConvoyBuyItemSupplyFactor;
			}
		}
	}

	public void OnConvoySellCommodity(DockableStationData station, ResourceType resourceType, int quantity)
	{
		Debug.Log("Convoy sold commodity " + quantity.ToString() + "x " + resourceType + " in station " + station.StationID);
		GameManager.Inst.UIManager.EconDebugPanel.SetConvoyTradeEvent("Convoy sold commodity " + quantity.ToString() + "x " + resourceType + " in station " + station.StationID);
		foreach(DemandResource resource in station.DemandResources)
		{
			if(resource.Type == resourceType)
			{
				resource.DemandLevel -= quantity * ConvoySellItemDemandFactor;
			}
		}
	}


	//severity is determined by the number of ships involved and duration
	//a nearby combat will reduce both supply and demand
	private void OnNearStationCombat(StationData station, float distance, float severity)
	{

	}

	private void SetInitialResourceDemand()
	{
		foreach(KeyValuePair<string,DockableStationData> station in GameManager.Inst.WorldManager.DockableStationDatas)
		{
			foreach(DemandResource resource in station.Value.DemandResources)
			{
				resource.CurrentDemand = UnityEngine.Random.Range(0.1f, 1.9f);
				resource.DemandLevel = UnityEngine.Random.Range(0.7f, 1.5f);
			}
		}
	}

	private void RefreshStationSaleItems()
	{
		foreach(KeyValuePair<string,DockableStationData> station in GameManager.Inst.WorldManager.DockableStationDatas)
		{
			foreach(SaleItem item in station.Value.TraderSaleItems)
			{
				//update quantity
				Debug.Log("loading item " + item.ItemID + " in station " + station.Value.StationID);
				ItemStats stats = GameManager.Inst.ItemManager.GetItemStats(item.ItemID);
				if(stats.Type == ItemType.Commodity)
				{
					item.Quantity = UnityEngine.Random.Range(10, 500);

				}
				else
				{
					item.Quantity = 1;
				}
			}
		}
	}
}

public enum EconomyEventType
{
	ConvoySellItem,
	ConvoyBuyItem,
	NearStationCombat,

}