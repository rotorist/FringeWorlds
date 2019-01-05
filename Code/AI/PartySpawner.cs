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

		Faction faction = GameManager.Inst.NPCManager.AllFactions[party.FactionID];

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

		//pick 1 freighter loadout for the leader
		if(faction.FreightersPool.Count > 0)
		{
			party.LeaderLoadout = new Loadout(faction.FreightersPool[UnityEngine.Random.Range(0, faction.FreightersPool.Count)]);
		}
		else
		{
			party.LeaderLoadout = new Loadout(faction.FightersPool[UnityEngine.Random.Range(0, faction.FightersPool.Count)]);
		}

		//pick 1-4 fighter loadouts for followers
		party.FollowerLoadouts = new List<Loadout>();
		int numberOfFollowers = UnityEngine.Random.Range(1, 5);
		for(int i=0; i<numberOfFollowers; i++)
		{
			party.FollowerLoadouts.Add(new Loadout(faction.FightersPool[UnityEngine.Random.Range(0, faction.FightersPool.Count)]));

		}

		MacroAITask task = GameManager.Inst.NPCManager.MacroAI.AssignMacroAITask(MacroAITaskType.None, party);

		party.IsInTradelane = false;
		//party.DestinationCoord = GameManager.Inst.WorldManager.AllNavNodes["cambridge_station"].Location;
		party.MoveSpeed = 10f;
		party.NextTwoNodes = new List<NavNode>();
		party.PrevNode = null;//CreateTempNode(party.Location, "tempstart", GameManager.Inst.WorldManager.AllSystems[party.CurrentSystemID]);

		GameManager.Inst.NPCManager.MacroAI.LoadPartyTreeset(party);

		GameManager.Inst.NPCManager.AllParties.Add(party);
	}

	public void GenerateFactionLoadouts(Faction faction)
	{
		faction.FightersPool = new List<Loadout>();
		faction.FreightersPool = new List<Loadout>();
		faction.CapitalPool = new List<Loadout>();

		//create 3 random loadouts for each ship in the ship pool and add them to corresponding loadout pool
		int index = 0;
		foreach(string shipID in faction.ShipPool)
		{
			ShipStats shipStats = GameManager.Inst.ItemManager.GetShipStats(shipID);
			Loadout loadout1 = GenerateFactionLoadout(faction, shipID, index);
			index ++;
			Loadout loadout2 = GenerateFactionLoadout(faction, shipID, index);
			index ++;
			Loadout Loadout3 = GenerateFactionLoadout(faction, shipID, index);
			index ++;

			List<Loadout> targetList = null;
			if(shipStats.ShipType == ShipType.Fighter)
			{
				targetList = faction.FightersPool;
			}
			else if(shipStats.ShipType == ShipType.Transport || shipStats.ShipType == ShipType.CargoShip)
			{
				targetList = faction.FreightersPool;
			}
			else
			{
				targetList = faction.CapitalPool;
			}

			targetList.Add(loadout1);
			targetList.Add(loadout2);
			targetList.Add(Loadout3);
		}
	}

	public void RefillLoadoutAmmo(Loadout loadout)
	{
		foreach(KeyValuePair<string, InvItemData> weapon in loadout.WeaponJoints)
		{

		}
	}




	private Loadout GenerateFactionLoadout(Faction faction, string shipID, int loadoutIndex)
	{
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
			else if(item.Type == ItemType.Ammo)
			{
				InvItemData itemData = new InvItemData();
				itemData.Quantity = 1;
				itemData.Item = item;
				loadout.AmmoBayItems.Add(itemData);
				Debug.Log("Party Spawner Added ammo " + itemData.Item.ID);
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
		loadout.WeaponJoints = new Dictionary<string, InvItemData>();
		foreach(WeaponJointData jointData in shipStats.WeaponJoints)
		{
			List<InvItemData> slotWeaponCandidates = new List<InvItemData>();
			foreach(InvItemData weaponItem in weaponCandidates)
			{
				if(weaponItem.Item.GetIntAttribute("Weapon Class") <= jointData.Class && weaponItem.Item.GetStringAttribute("Rotation Type") == jointData.RotationType.ToString())
				{
					slotWeaponCandidates.Add(weaponItem);
				}
			}

			if(slotWeaponCandidates.Count > 0)
			{
				loadout.WeaponJoints.Add(jointData.JointID, slotWeaponCandidates[UnityEngine.Random.Range(0, slotWeaponCandidates.Count)]);
			}
		}

		loadout.ShipMods = new InvItemData[shipStats.ModSlots];


		loadout.CurrentPowerMgmtButton = new Vector3(0, -20f, 0);
		loadout.FuelAmount = shipStats.MaxFuel;
		loadout.HullAmount = shipStats.Hull;
		loadout.LifeSupportAmount = shipStats.LifeSupport;
		loadout.LoadoutID = faction.ID + "_" + loadoutIndex.ToString();


		return loadout;
	}
}
