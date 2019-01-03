using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;

public class JsonDataHandler : DataHandlerBase
{

	#region Public fields



	#endregion


	#region Private fields


	#endregion


	#region Constructor
	public JsonDataHandler()
	{
		Initialize();
	}

	#endregion


	#region Private methods
	private void Initialize()
	{
		this.Path = Application.dataPath + "/GameData/";
	}

	#endregion


	public override object LoadByName (string name, object[] param)
	{
		return null;
	}

	public override object[] LoadAll (object [] param)
	{
		return null;
	}

	public string LoadJsonString(string path)
	{
		string filePath = this.Path + path;
		if(File.Exists(filePath))
		{
			string dataAsJson = File.ReadAllText(filePath);
			return dataAsJson;
				/*
				JsonData loadedData = JsonUtility.FromJson<JsonData>(dataAsJson);
				
				for (int i = 0; i < loadedData.items.Length; i++) 
				{
					jsonDict.Add(loadedData.items[i].key, loadedData.items[i].value); 
					Debug.Log(loadedData.items[i].value.ToString());
				}
				*/


		}

		return "";

	}

	public ShipStats LoadShipStatsJson(string fileName)
	{
		Debug.Log("Loading ship from json " + fileName);
		
		string dataAsJson = LoadJsonString("Ships/" + fileName);
		ShipStats shipData = JsonUtility.FromJson<ShipStats>(dataAsJson);
		return shipData;
	}

	public Dictionary<string, ShipStats> LoadAllShipStats()
	{
		DirectoryInfo dirInfo = new DirectoryInfo(this.Path + "Ships/");
		FileInfo [] infos = dirInfo.GetFiles("*.json");
		Dictionary<string, ShipStats> ships = new Dictionary<string, ShipStats>();
		foreach(FileInfo info in infos)
		{
			ShipStats stats = LoadShipStatsJson(info.Name);
			ships.Add(stats.ID, stats);
		}


		return ships;
	}


	public ItemStats LoadItemStatsJson(string fileName)
	{
		Debug.Log("Loading item from json " + fileName);

		string dataAsJson = LoadJsonString("Items/" + fileName);
		ItemStats item = JsonUtility.FromJson<ItemStats>(dataAsJson);
		return item;
	}

	public Dictionary<string, ItemStats> LoadAllItemStats()
	{
		Dictionary<string, ItemStats> items = new Dictionary<string, ItemStats>();
		DirectoryInfo topDirInfo = new DirectoryInfo(this.Path + "Items/");
		DirectoryInfo [] dirInfos = topDirInfo.GetDirectories();
		foreach(DirectoryInfo dirInfo in dirInfos)
		{
			FileInfo [] infos = dirInfo.GetFiles("*.json");
			foreach(FileInfo info in infos)
			{
				ItemStats stats = LoadItemStatsJson(dirInfo.Name + "/" + info.Name);
				items.Add(stats.ID, stats);

				Debug.Log(stats.ID + " " + stats.Attributes[0].Name + " " + stats.Attributes[0].SerValue);
			}
		}

		return items;
	}


	public DockableStationData LoadDockableStationJson(string fileName)
	{
		string dataAsJson = LoadJsonString("Stations/" + fileName);
		DockableStationInitialData initialData = JsonUtility.FromJson<DockableStationInitialData>(dataAsJson);
		DockableStationData stationData = new DockableStationData();
		stationData.StationID = initialData.StationID;
		stationData.FactionID = initialData.FactionID;
		stationData.FuelPrice = initialData.FuelPrice;
		stationData.LifeSupportPrice = initialData.LifeSupportPrice;
		stationData.ShipsForSale = initialData.ShipsForSale;
		stationData.TraderSaleItems = initialData.TraderSaleItems;
		stationData.DemandResources = initialData.DemandResources;
		stationData.UndesiredItemPriceMultiplier = UnityEngine.Random.Range(0.1f, 0.4f);
		stationData.DemandNormalizeSpeed = initialData.DemandNormalizeSpeed;
		foreach(SaleItem saleItem in stationData.TraderSaleItems)
		{
			ItemStats stats = GameManager.Inst.ItemManager.GetItemStats(saleItem.ItemID);
			if(stats.Type == ItemType.Commodity)
			{
				saleItem.SupplyLevel = UnityEngine.Random.Range(0.3f, 1.6f);
			}
			else
			{
				saleItem.SupplyLevel = 1;
			}
			saleItem.Quantity = 1;
		}

		return stationData;
	}

	public Dictionary<string, DockableStationData> LoadAllDockableStations()
	{
		Debug.Log("Loading station datas");
		Dictionary<string, DockableStationData> stations = new Dictionary<string, DockableStationData>();
		DirectoryInfo topDirInfo = new DirectoryInfo(this.Path + "Stations/");
		FileInfo [] infos = topDirInfo.GetFiles("*.json");
		foreach(FileInfo info in infos)
		{
			Debug.Log("Loading station " + info.Name);
			DockableStationData stationData = LoadDockableStationJson(info.Name);
			stations.Add(stationData.StationID, stationData);
		}

		return stations;
	}

	public Faction LoadFactionJson(string fileName)
	{
		string dataAsJson = LoadJsonString("Factions/" + fileName);
		Faction faction = JsonUtility.FromJson<Faction>(dataAsJson);

		return faction;
	}

	public Dictionary<string, Faction> LoadAllFactions()
	{
		Debug.Log("Loading faction data");
		Dictionary<string, Faction> factions = new Dictionary<string, Faction>();
		DirectoryInfo topDirInfo = new DirectoryInfo(this.Path + "Factions/");
		FileInfo [] infos = topDirInfo.GetFiles("*.json");
		foreach(FileInfo info in infos)
		{
			if(info.Name != "relationships.json")
			{
				Debug.Log("Loading faction " + info.Name);
				Faction faction = LoadFactionJson(info.Name);
				factions.Add(faction.ID, faction);
			}
		}

		return factions;
	}

	public FactionRelationshipSaveData LoadFactionRelationships()
	{
		Debug.Log("Loading faction relationships");
		string dataAsJson = LoadJsonString("Factions/relationships.json");
		FactionRelationshipSaveData relationships = JsonUtility.FromJson<FactionRelationshipSaveData>(dataAsJson);

		return relationships;
	}
}



