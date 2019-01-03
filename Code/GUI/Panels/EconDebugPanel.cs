using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EconDebugPanel : PanelBase
{
	public Transform StationSelectAnchor;
	public List<DebugStationSelect> StationSelects;
	public UILabel SupplyCommodityList;
	public UILabel DemandCommodityList;
	public UILabel ConvoyTradeEvents;

	private string _selectedStationID;

	public override void Initialize ()
	{
		base.Initialize ();
	}

	public override void Show ()
	{
		base.Show ();
		RefreshStations();
	}

	public override void Hide ()
	{
		base.Hide ();
	}

	public override void PerFrameUpdate ()
	{
		if(!string.IsNullOrEmpty(_selectedStationID))
		{
			RefreshPrices();
		}
	}


	public void OnStationSelect(string stationID)
	{
		_selectedStationID = stationID;
	}

	public void SetConvoyTradeEvent(string description)
	{
		ConvoyTradeEvents.text = ConvoyTradeEvents.text + '\n' + description;
	}


	private void RefreshStations()
	{
		foreach(DebugStationSelect stationSelect in StationSelects)
		{
			GameObject.Destroy(stationSelect.gameObject);

		}

		StationSelects.Clear();
		int i = 0;
		foreach(KeyValuePair<string, DockableStationData> stationData in GameManager.Inst.WorldManager.DockableStationDatas)
		{
			GameObject stationSelectGO = GameObject.Instantiate(Resources.Load("DebugStationSelect")) as GameObject;
			DebugStationSelect stationSelect = stationSelectGO.GetComponent<DebugStationSelect>();
			stationSelect.transform.parent = StationSelectAnchor;
			stationSelect.transform.localPosition = new Vector3(0, 0 - (i * 20 + 5), 0);
			stationSelect.transform.localScale = new Vector3(1, 1, 1);
			stationSelect.StationIDLabel.text = stationData.Value.StationID;
			stationSelect.StationID = stationData.Value.StationID;
			StationSelects.Add(stationSelect);
			i++;
		}
	}

	private void RefreshPrices()
	{
		if(!string.IsNullOrEmpty(_selectedStationID))
		{
			string text = "";
			DockableStationData stationData = GameManager.Inst.WorldManager.DockableStationDatas[_selectedStationID];
			foreach(SaleItem saleItem in stationData.TraderSaleItems)
			{
				ItemStats stats = GameManager.Inst.ItemManager.GetItemStats(saleItem.ItemID);
				if(stats.Type == ItemType.Commodity)
				{
					text = text + stats.DisplayName + " Level " + saleItem.SupplyLevel + " Price " + Mathf.FloorToInt((stats.BasePrice * saleItem.SupplyLevel)).ToString() + '\n';
				}
			}

			SupplyCommodityList.text = text;

			text = "";
			foreach(DemandResource resource in stationData.DemandResources)
			{
				if(resource.Type == ResourceType.None)
				{
					ItemStats stats = GameManager.Inst.ItemManager.GetItemStats(resource.ItemID);
					text = text + stats.DisplayName + " Level " + resource.CurrentDemand + " Price " + Mathf.FloorToInt((stats.BasePrice * resource.CurrentDemand)).ToString() + '\n';
				}
				else
				{
					text = text + resource.Type + " Level " + resource.CurrentDemand + '\n';
				}
					
			}

			DemandCommodityList.text = text;
		}
	}
}
