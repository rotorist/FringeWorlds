﻿using UnityEngine;
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


}


