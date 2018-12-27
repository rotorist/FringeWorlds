using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EconomyManager
{


	public void Initialize()
	{
		RefreshStationSaleItems();
	}

	public float GetItemSellPrice(Item item, DockableStationData stationData)
	{
		if(item.Type == ItemType.Commodity)
		{
			//if the station has static needs for the resource, multiply base price with stations's demand item demand level
			//if item provides a resource that the station demands then multiply the base price with station's demand level for the resource
			//otherwise multiply base price with a small number between 0.1 and 0.4
			ResourceType resourceType = (ResourceType)Enum.Parse(typeof(ResourceType), item.GetStringAttribute("ResourceType"));
			foreach(DemandResource demandResource in stationData.DemandResources)
			{
				if(demandResource.ItemID == item.ID)
				{
					return item.BasePrice * demandResource.DemandLevel;
				}
				else if(demandResource.Type == resourceType)
				{
					return item.BasePrice * demandResource.DemandLevel;
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



	private void RefreshStationSaleItems()
	{
		foreach(KeyValuePair<string,DockableStationData> station in GameManager.Inst.WorldManager.DockableStationDatas)
		{
			foreach(SaleItem item in station.Value.TraderSaleItems)
			{
				//update quantity
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

