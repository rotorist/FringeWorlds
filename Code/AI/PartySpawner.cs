using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PartySpawner
{


	private List<string> _traderFactions;

	public void Initialize()
	{
		_traderFactions = new List<string>() { "otu", "freelancers" };
	}

	public void PerSecondUpdate()
	{
		//check if any trader party need to be added
		int currentTraderParties = 0;
		foreach(MacroAIParty party in GameManager.Inst.NPCManager.AllParties)
		{
			if(party.PartyType == MacroAIPartyType.Trader)
			{
				currentTraderParties ++;
			}
		}


		if(currentTraderParties < GameManager.Inst.Constants.MaxTraderParties)
		{
			//just create 1 trader party
			GenerateTraderParty(_traderFactions[UnityEngine.Random.Range(0, _traderFactions.Count)]);

		}
	}



	public void GenerateTraderParty(string factionID)
	{
		MacroAIParty party = new MacroAIParty();
		party.FactionID = factionID;
		party.SpawnedShips = new List<ShipBase>();

		List<string> keyList = new List<string>(GameManager.Inst.WorldManager.AllSystems.Keys);

		//StarSystemData currentSystem = GameManager.Inst.WorldManager.AllSystems["washington_system"];
		//StarSystemData currentSystem = GameManager.Inst.WorldManager.AllSystems[keyList[UnityEngine.Random.Range(0, keyList.Count)]];
		StationData currentStation = GameManager.Inst.WorldManager.GetRandomFriendlyDockableStation(party.FactionID, null);
		StarSystemData currentSystem = GameManager.Inst.WorldManager.AllSystems[currentStation.SystemID];
		party.CurrentSystemID = currentSystem.ID;

		//StationData currentStation = currentSystem.GetStationByID("planet_colombia_landing");
		//StationData currentStation = currentSystem.Stations[UnityEngine.Random.Range(0, currentSystem.Stations.Count)];
		party.DockedStationID = currentStation.ID;
		Transform origin = GameObject.Find("Origin").transform;
		party.Location = new RelLoc(origin.position, currentStation.Location.RealPos, origin);
		party.PartyNumber = GameManager.Inst.NPCManager.LastUsedPartyNumber + 1;
		GameManager.Inst.NPCManager.LastUsedPartyNumber = party.PartyNumber;
	}

	public void GenerateFactionLoadouts(Faction faction)
	{
		string shipID = faction.ShipPool[UnityEngine.Random.Range(0, faction.ShipPool.Count)];
		ShipStats shipStats = GameManager.Inst.ItemManager.GetShipStats(shipID);
		Loadout loadout = new Loadout(shipID, shipStats.ShipType);

		List<InvItemData> shieldCandidates = new List<InvItemData>();
		List<InvItemData> capacitorCandidates = new List<InvItemData>();
		List<InvItemData> thrusterCandidates = new List<InvItemData>();
		List<InvItemData> scannerCandidates = new List<InvItemData>();
		List<InvItemData> defensiveCandidates = new List<InvItemData>();
		List<InvItemData> weaponCandidates = new List<InvItemData>();

		foreach(string equipmentID in faction.EquipmentPool)
		{
			//find the shield with same class as the ship's shield class
			ItemStats itemStats = GameManager.Inst.ItemManager.GetItemStats(equipmentID);
			Item item = new Item(itemStats);
			if(item.Type == ItemType.Equipment)
			{
				InvItemData itemData = new InvItemData();
				itemData.Quantity = 1;
				itemData.Item = item;
				string equipmentType = item.GetStringAttribute("Equipment Type");
				if(equipmentType == "Shield")
				{
					ShieldClass shieldClass = (ShieldClass)Enum.Parse(typeof(ShieldClass), item.GetStringAttribute("Shield Class"));
					if(shieldClass == shipStats.ShieldClass)
					{
						shieldCandidates.Add(itemData);
					}
				}
				else if(equipmentType == "WeaponCapacitor")
				{
					capacitorCandidates.Add(itemData);
				}
				else if(equipmentType == "Thruster")
				{
					thrusterCandidates.Add(itemData);
				}
				else if(equipmentType == "Scanner")
				{
					scannerCandidates.Add(itemData);
				}
			}
			else if(item.Type == ItemType.Defensives)
			{
				if(shipStats.DefenseSlots > 0)
				{
					InvItemData itemData = new InvItemData();
					itemData.Quantity = 1;
					itemData.Item = item;
					defensiveCandidates.Add(itemData);
				}
			}
			else if(item.Type == ItemType.Weapon)
			{
				InvItemData itemData = new InvItemData();
				itemData.Quantity = 1;
				itemData.Item = item;
				weaponCandidates.Add(itemData);
			}
		}

		loadout.Shield = shieldCandidates[UnityEngine.Random.Range(0, shieldCandidates.Count)];
		loadout.WeaponCapacitor = capacitorCandidates[UnityEngine.Random.Range(0, capacitorCandidates.Count)];
		loadout.Thruster = thrusterCandidates[UnityEngine.Random.Range(0, thrusterCandidates.Count)];
		loadout.Scanner = scannerCandidates[UnityEngine.Random.Range(0, scannerCandidates.Count)];

		if(shipStats.DefenseSlots > 0)
		{
			loadout.Defensives = new List<InvItemData>();
			loadout.Defensives.Add(defensiveCandidates[UnityEngine.Random.Range(0, defensiveCandidates.Count)]);
		}

		//go through each weapon slot and find all the weapon candidates that can fit it, and then pick a random one


	}
}
