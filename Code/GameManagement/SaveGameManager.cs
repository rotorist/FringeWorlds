using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveGameManager
{
	public SaveGame CurrentSave;


	public void CreateAnchorSave(StationBase station, StationType type)
	{
		CurrentSave = new SaveGame();

		//create a regular save and then fill anchor with data

		if(type == StationType.JumpGate)
		{
			JumpGate gate = (JumpGate)station;
			CurrentSave.SpawnSystem = gate.TargetSystem;
			CurrentSave.SpawnStationID = gate.ExitGateID;
			CurrentSave.SpawnStationType = StationType.JumpGate;
		}
		else if(type == StationType.JumpHole)
		{

		}
		else if(type == StationType.Station)
		{
			CurrentSave.SpawnSystem = GameManager.Inst.WorldManager.CurrentSystem.ID;
			CurrentSave.SpawnStationID = station.ID;
			CurrentSave.SpawnStationType = type;
		}

		SaveWorldData();
		SavePlayerData(false);

		SerializeSave();

		LevelAnchor anchor = GameObject.FindObjectOfType<LevelAnchor>();
		anchor.SpawnSystem = CurrentSave.SpawnSystem;
		anchor.ProfileName = GameManager.Inst.PlayerProgress.ProfileName;
	}

	public void CreateSaveInStation()
	{
		Debug.Log("Saving game in station");

		CurrentSave = new SaveGame();
		CurrentSave.SpawnSystem = GameManager.Inst.PlayerProgress.SpawnSystemID;
		CurrentSave.SpawnStationID = GameManager.Inst.PlayerProgress.SpawnStationID;
		CurrentSave.SpawnStationType = GameManager.Inst.PlayerProgress.SpawnStationType;

		CurrentSave.AllNonPlayerParties = GameManager.Inst.NPCManager.PartySaveDatas;
		CurrentSave.LastUsedPartyNumber = GameManager.Inst.NPCManager.LastUsedPartyNumber;


		SavePlayerData(true);

		SerializeSave();

		LevelAnchor anchor = GameObject.FindObjectOfType<LevelAnchor>();
		anchor.SpawnSystem = CurrentSave.SpawnSystem;
		anchor.ProfileName = GameManager.Inst.PlayerProgress.ProfileName;
	}

	public void SavePlayerData(bool isInStation)
	{
		CurrentSave.PlayerLoadouts = new List<LoadoutSaveData>();
		if(!isInStation)
		{
			SyncLoadoutWithShip(GameManager.Inst.PlayerProgress.ActiveLoadout, GameManager.Inst.PlayerControl.PlayerShip);
		}

		Debug.Log(GameManager.Inst.PlayerProgress.ActiveLoadout.CurrentPowerMgmtButton);
		LoadoutSaveData loadout = CreateLoadoutSaveData(GameManager.Inst.PlayerProgress.ActiveLoadout);

		CurrentSave.PlayerLoadouts.Add(loadout);
		CurrentSave.PlayerActiveLoadoutID = loadout.LoadoutID;
		CurrentSave.ProfileName = GameManager.Inst.PlayerProgress.ProfileName;
	}

	public void SyncLoadoutWithShip(Loadout loadout, ShipBase ship)
	{
		loadout.AmmoBayItems = new List<InvItemData>();
		foreach(KeyValuePair<string,InvItemData> itemData in ship.Storage.AmmoBayItems)
		{
			loadout.AmmoBayItems.Add(itemData.Value);
		}

		loadout.CargoBayItems = new List<InvItemData>();
		foreach(InvItemData itemData in ship.Storage.CargoBayItems)
		{
			loadout.CargoBayItems.Add(itemData);
		}

		loadout.CurrentPowerMgmtButton = ship.CurrentPowerMgmtButton;
		loadout.HullAmount = ship.HullAmount;
		loadout.FuelAmount = ship.FuelAmount;
		loadout.LifeSupportAmount = ship.LifeSupportAmount;

		//sync weapon ammo assignment
		foreach(WeaponJoint joint in ship.MyReference.WeaponJoints)
		{
			if(joint.MountedWeapon != null && loadout.WeaponJoints.ContainsKey(joint.JointID))
			{
				loadout.WeaponJoints[joint.JointID].RelatedItemID = joint.MountedWeapon.AmmoID;
			}
		}
	}

	public void SaveWorldData()
	{

		//save npc manager
		CurrentSave.LastUsedPartyNumber = GameManager.Inst.NPCManager.LastUsedPartyNumber;

		//save all macroAI parties
		CurrentSave.AllNonPlayerParties = new List<MacroAIPartySaveData>();
		foreach(MacroAIParty party in GameManager.Inst.NPCManager.AllParties)
		{
			if(party.PartyNumber == 0)
			{
				continue;
			}

			MacroAIPartySaveData partyData = new MacroAIPartySaveData();
			partyData.Destination = new SerVector3(party.Destination);
			partyData.PartyNumber = party.PartyNumber;
			partyData.FactionID = party.FactionID;
			partyData.Location = new SerVector3(party.Location.Disposition);
			partyData.DockedStationID = party.DockedStationID;
			partyData.CurrentSystemID = party.CurrentSystemID;
			partyData.MoveSpeed = party.MoveSpeed;
			partyData.IsPlayerParty = party.IsPlayerParty;
			partyData.IsInTradelane = party.IsInTradelane;
			partyData.NextTwoNodesIDs = new List<string>();
			foreach(NavNode node in party.NextTwoNodes)
			{
				partyData.NextTwoNodesIDs.Add(node.ID);
			}
			partyData.PrevNodeID = (party.PrevNode == null)? "" : party.PrevNode.ID;
			partyData.DestNodeID = (party.DestNode == null)? "" : party.DestNode.ID;
			partyData.HasReachedDestNode = party.HasReachedDestNode;
			partyData.WaitTimer = party.WaitTimer;
			if(party.CurrentTask != null)
			{
				partyData.CurrentTask = new MacroAITaskSaveData();
				partyData.CurrentTask.TaskType = party.CurrentTask.TaskType;
				partyData.CurrentTask.StayDuration = party.CurrentTask.StayDuration;
				if(party.CurrentTask.TravelDestCoord != null)
				{
					partyData.CurrentTask.TravelDestCoord = new SerVector3(party.CurrentTask.TravelDestCoord.Disposition);
				}
				partyData.CurrentTask.TravelDestSystemID = party.CurrentTask.TravelDestSystemID;
				partyData.CurrentTask.TravelDestNodeID = party.CurrentTask.TravelDestNodeID;
				partyData.CurrentTask.IsDestAStation = party.CurrentTask.IsDestAStation;
			}
			partyData.LastUpdateTime = party.LastUpdateTime;
			partyData.ShouldEnableAI = party.ShouldEnableAI;
			partyData.FollowerLoadouts = new List<LoadoutSaveData>();
			foreach(Loadout loadout in party.FollowerLoadouts)
			{
				LoadoutSaveData loadoutData = CreateLoadoutSaveData(loadout);
				partyData.FollowerLoadouts.Add(loadoutData);
			}
			partyData.LeaderLoadout = CreateLoadoutSaveData(party.LeaderLoadout);
			partyData.TreeSetNames = new List<string>();
			foreach(KeyValuePair<string, BehaviorTree> tree in party.TreeSet)
			{
				partyData.TreeSetNames.Add(tree.Key);
			}

			CurrentSave.AllNonPlayerParties.Add(partyData);
		}




	}

	public void SerializeSave()
	{
		string fullPath = Application.persistentDataPath + "/" + GameManager.Inst.PlayerProgress.ProfileName + ".dat";

		BinaryFormatter bf = new BinaryFormatter();
		FileStream file;
		if(File.Exists(fullPath))
		{
			file = File.Open(fullPath, FileMode.Open);
		}
		else
		{
			file = File.Create(fullPath);
		}

		bf.Serialize(file, CurrentSave);
		file.Close();
		Debug.Log("Game has been saved");
	}


	public SaveGame GetSave(string profileName)
	{
		CurrentSave = null;

		Debug.Log("Loading save " + profileName);
		string fullPath = Application.persistentDataPath + "/" + profileName + ".dat";

		BinaryFormatter bf = new BinaryFormatter();
		FileStream file;
		if(File.Exists(fullPath))
		{
			file = File.Open(fullPath, FileMode.Open);
		}
		else
		{
			return null;
		}

		CurrentSave = (SaveGame)bf.Deserialize(file);

		file.Close();

		GameManager.Inst.PlayerProgress.SpawnStationID = CurrentSave.SpawnStationID;
		GameManager.Inst.PlayerProgress.SpawnStationType = CurrentSave.SpawnStationType;
		GameManager.Inst.PlayerProgress.SpawnSystemID = CurrentSave.SpawnSystem;

		GameManager.Inst.PlayerProgress.ProfileName = CurrentSave.ProfileName;

		return CurrentSave;
	}

	public void LoadSaveInSpace()
	{
		if(CurrentSave == null)
		{
			return;
		}

		GameManager.Inst.NPCManager.LastUsedPartyNumber = CurrentSave.LastUsedPartyNumber;

		//loading all NPC and player parties
		Debug.Log("Loading AI Parties, there are " + CurrentSave.AllNonPlayerParties.Count);
		foreach(MacroAIPartySaveData partyData in CurrentSave.AllNonPlayerParties)
		{
			Debug.Log("Loading Party " + partyData.PartyNumber);
			MacroAIParty party = new MacroAIParty();
			party.FactionID = partyData.FactionID;
			party.SpawnedShips = new List<ShipBase>();

			//List<string> keyList = new List<string>(GameManager.Inst.WorldManager.AllSystems.Keys);
			StarSystemData currentSystem = GameManager.Inst.WorldManager.AllSystems[partyData.CurrentSystemID];
			party.CurrentSystemID = currentSystem.ID;
			party.DockedStationID = partyData.DockedStationID;

			if(currentSystem.ID == GameManager.Inst.WorldManager.CurrentSystem.ID)
			{
				party.Location = new RelLoc(currentSystem.OriginPosition, partyData.Location.ConvertToVector3(), GameObject.Find("Origin").transform, 1);
			}
			else
			{
				party.Location = new RelLoc(currentSystem.OriginPosition, partyData.Location.ConvertToVector3(), null, 1);
			}
			party.PartyNumber = partyData.PartyNumber;

			//generate loadout
			party.LeaderLoadout = LoadLoadoutFromSave(partyData.LeaderLoadout);


			party.FollowerLoadouts = new List<Loadout>();
			foreach(LoadoutSaveData loadoutData in partyData.FollowerLoadouts)
			{
				Loadout loadout = LoadLoadoutFromSave(loadoutData);
				party.FollowerLoadouts.Add(loadout);
			}
				

			if(partyData.CurrentTask != null)
			{
				MacroAITask task = new MacroAITask();
				task.IsDestAStation = partyData.CurrentTask.IsDestAStation;
				task.StayDuration = partyData.CurrentTask.StayDuration;
				task.TaskType = partyData.CurrentTask.TaskType;

				if(partyData.CurrentTask.TravelDestCoord != null)
				{
					if(currentSystem.ID == GameManager.Inst.WorldManager.CurrentSystem.ID)
					{
						task.TravelDestCoord = new RelLoc(currentSystem.OriginPosition, partyData.CurrentTask.TravelDestCoord.ConvertToVector3(), GameObject.Find("Origin").transform, 1);
					}
					else
					{
						task.TravelDestCoord = new RelLoc(currentSystem.OriginPosition, partyData.CurrentTask.TravelDestCoord.ConvertToVector3(), null, 1);
					}
				}
				task.TravelDestNodeID = partyData.CurrentTask.TravelDestNodeID;
				task.TravelDestSystemID = partyData.CurrentTask.TravelDestSystemID;
				Debug.Log("Loading task type " + task.TaskType.ToString() + " " + task.TravelDestNodeID);
				party.CurrentTask = task;

				if(party.CurrentTask.TaskType == MacroAITaskType.Travel)
				{
					if(party.CurrentTask.IsDestAStation)
					{
						party.DestNode = GameManager.Inst.WorldManager.AllNavNodes[party.CurrentTask.TravelDestNodeID];
					}
					else
					{
						//party.DestNode = CreateTempNode(party.CurrentTask.TravelDestCoord, "tempdest", GameManager.Inst.WorldManager.AllSystems[party.CurrentTask.TravelDestSystemID]);
						party.DestNode = GameManager.Inst.NPCManager.MacroAI.GetClosestNodeToLocation(party.CurrentTask.TravelDestCoord.RealPos, GameManager.Inst.WorldManager.AllSystems[party.CurrentTask.TravelDestSystemID]);
					}
				}

			}


			party.IsInTradelane = partyData.IsInTradelane;
			party.MoveSpeed = partyData.MoveSpeed;
			party.NextTwoNodes = new List<NavNode>();
			foreach(string nodeID in partyData.NextTwoNodesIDs)
			{
				Debug.Log("Next node " + nodeID);
				party.NextTwoNodes.Add(GameManager.Inst.WorldManager.AllNavNodes[nodeID]);
			}
			if(partyData.PrevNodeID != "")
			{
				party.PrevNode = GameManager.Inst.WorldManager.AllNavNodes[partyData.PrevNodeID];
			}
			party.TreeSet = new Dictionary<string, BehaviorTree>();
			foreach(string treeSetName in partyData.TreeSetNames)
			{
				party.TreeSet.Add(treeSetName, GameManager.Inst.DBManager.XMLParserBT.LoadBehaviorTree(treeSetName, null, party));
			}


			GameManager.Inst.NPCManager.AllParties.Add(party);


		}

		//load player data now
		LoadPlayerData();

	}

	public void LoadPlayerData()
	{
		foreach(LoadoutSaveData loadoutData in CurrentSave.PlayerLoadouts)
		{
			if(loadoutData.LoadoutID == CurrentSave.PlayerActiveLoadoutID)
			{
				GameManager.Inst.PlayerProgress.ActiveLoadout = LoadLoadoutFromSave(loadoutData);

				//Debug.Log(GameManager.Inst.PlayerProgress.ActiveLoadout.CurrentPowerMgmtButton);
			}
		}
	}

	public void LoadSaveInStation()
	{
		if(CurrentSave == null)
		{
			return;
		}

		GameManager.Inst.NPCManager.LastUsedPartyNumber = CurrentSave.LastUsedPartyNumber;
		GameManager.Inst.NPCManager.PartySaveDatas = CurrentSave.AllNonPlayerParties;

		//load player data now
		LoadPlayerData();
	}

	public void LoadNewGameInSpace()
	{
		Debug.Log("Creating new game in space");
		CurrentSave = null;
		GameManager.Inst.PlayerProgress.Credits = 36100;
		GameManager.Inst.PlayerProgress.SpawnSystemID = "washington_system";
		GameManager.Inst.PlayerProgress.SpawnStationID = "planet_colombia_landing";
		GameManager.Inst.PlayerProgress.SpawnStationType = StationType.Station;
		GameManager.Inst.PlayerProgress.CreateInitialLoadout();
	}

	public void LoadNewGameInStation()
	{
		Debug.Log("Creating new game in station");
		CurrentSave = null;
		GameManager.Inst.PlayerProgress.Credits = 36100;
		GameManager.Inst.PlayerProgress.SpawnSystemID = "washington_system";
		GameManager.Inst.PlayerProgress.SpawnStationID = "planet_colombia_landing";
		GameManager.Inst.PlayerProgress.SpawnStationType = StationType.Station;
		GameManager.Inst.PlayerProgress.CreateInitialLoadout();
	}


	private LoadoutSaveData CreateLoadoutSaveData(Loadout loadout)
	{
		if(loadout == null)
		{
			return null;
		}
		LoadoutSaveData loadoutData = new LoadoutSaveData();
		loadoutData.LoadoutID = loadout.LoadoutID;
		loadoutData.ShipID = loadout.ShipID;
		loadoutData.ShipType = loadout.ShipType;
		loadoutData.WeaponJointNames = new List<string>();
		loadoutData.Weapons = new List<InvItemData>();
		foreach(KeyValuePair<string, InvItemData> joint in loadout.WeaponJoints)
		{
			loadoutData.WeaponJointNames.Add(joint.Key);
			loadoutData.Weapons.Add(joint.Value);
		}

		loadoutData.CurrentPowerMgmtButton = new SerVector3(loadout.CurrentPowerMgmtButton);
		loadoutData.HullAmount = loadout.HullAmount;
		loadoutData.FuelAmount = loadout.FuelAmount;
		loadoutData.LifeSupportAmount = loadout.LifeSupportAmount;

		loadoutData.Defensives = loadout.Defensives;
		loadoutData.AmmoBayItems = loadout.AmmoBayItems;
		loadoutData.CargoBayItems = loadout.CargoBayItems;

		loadoutData.Shield = loadout.Shield;
		loadoutData.WeaponCapacitor = loadout.WeaponCapacitor;
		loadoutData.Thruster = loadout.Thruster;
		loadoutData.Scanner = loadout.Scanner;
		loadoutData.Teleporter = loadout.Teleporter;

		loadoutData.ShipMods = loadout.ShipMods;

		return loadoutData;
	}

	private Loadout LoadLoadoutFromSave(LoadoutSaveData data)
	{
		Loadout loadout = new Loadout(data.ShipID, data.ShipType);
		for(int i=0; i<data.WeaponJointNames.Count; i++)
		{
			loadout.WeaponJoints.Add(data.WeaponJointNames[i], data.Weapons[i]);
		}
		loadout.LoadoutID = data.LoadoutID;
		loadout.CurrentPowerMgmtButton = data.CurrentPowerMgmtButton.ConvertToVector3();
		loadout.HullAmount = data.HullAmount;
		loadout.FuelAmount = data.FuelAmount;
		loadout.LifeSupportAmount = data.LifeSupportAmount;

		loadout.Defensives = data.Defensives;

		loadout.AmmoBayItems = data.AmmoBayItems;
		foreach(InvItemData itemData in loadout.AmmoBayItems)
		{
			if(itemData != null)
			{
				itemData.Item.PostLoad();
			}
		}

		loadout.CargoBayItems = data.CargoBayItems;
		foreach(InvItemData itemData in loadout.CargoBayItems)
		{
			if(itemData != null)
			{
				itemData.Item.PostLoad();
			}
		}

		Debug.Log("Cargo bay items count " + loadout.CargoBayItems.Count);
		loadout.Shield = data.Shield;
		if(loadout.Shield != null)
		{
			loadout.Shield.Item.PostLoad();
		}

		loadout.WeaponCapacitor = data.WeaponCapacitor;
		if(loadout.WeaponCapacitor != null)
		{
			loadout.WeaponCapacitor.Item.PostLoad();
		}

		loadout.Thruster = data.Thruster;
		if(loadout.Thruster != null)
		{
			loadout.Thruster.Item.PostLoad();
		}

		loadout.Scanner = data.Scanner;
		if(loadout.Scanner != null)
		{
			loadout.Scanner.Item.PostLoad();
		}

		loadout.Teleporter = data.Teleporter;
		if(loadout.Teleporter != null)
		{
			loadout.Teleporter.Item.PostLoad();
		}

		loadout.ShipMods = data.ShipMods;
		foreach(InvItemData itemData in loadout.ShipMods)
		{
			if(itemData != null)
			{
				itemData.Item.PostLoad();
			}
		}

		return loadout;
	}
}
