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


		LevelAnchor anchor = GameObject.FindObjectOfType<LevelAnchor>();
		anchor.SpawnSystem = CurrentSave.SpawnSystem;
		anchor.ProfileName = GameManager.Inst.PlayerProgress.ProfileName;
	}

	public void CreateSaveInStation(StationBase station)
	{

	}

	public void SaveWorldData()
	{

		//save npc manager
		CurrentSave.LastUsedPartyNumber = GameManager.Inst.NPCManager.LastUsedPartyNumber;

		//save all macroAI parties
		CurrentSave.AllParties = new List<MacroAIPartySaveData>();
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

			CurrentSave.AllParties.Add(partyData);
		}

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

		GameManager.Inst.PlayerControl.SpawnStationID = CurrentSave.SpawnStationID;
		GameManager.Inst.PlayerControl.SpawnStationType = CurrentSave.SpawnStationType;

		GameManager.Inst.PlayerProgress.ProfileName = CurrentSave.ProfileName;

		return CurrentSave;
	}

	public void LoadSave()
	{
		if(CurrentSave == null)
		{
			return;
		}

		GameManager.Inst.NPCManager.LastUsedPartyNumber = CurrentSave.LastUsedPartyNumber;

		//loading all NPC and player parties
		Debug.Log("Loading AI Parties, there are " + CurrentSave.AllParties.Count);
		foreach(MacroAIPartySaveData partyData in CurrentSave.AllParties)
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
			party.LeaderLoadout = new Loadout(partyData.LeaderLoadout.ShipID, partyData.LeaderLoadout.ShipType);
			party.LeaderLoadout.WeaponJoints = new Dictionary<string, string>();

			for(int i=0; i<partyData.LeaderLoadout.WeaponJointNames.Count; i++)
			{
				party.LeaderLoadout.WeaponJoints.Add(partyData.LeaderLoadout.WeaponJointNames[i], partyData.LeaderLoadout.WeaponNames[i]);
			}

			party.FollowerLoadouts = new List<Loadout>();
			foreach(LoadoutSaveData loadoutData in partyData.FollowerLoadouts)
			{
				Loadout loadout = new Loadout(loadoutData.ShipID, loadoutData.ShipType);
				for(int i=0; i<loadoutData.WeaponJointNames.Count; i++)
				{
					loadout.WeaponJoints.Add(loadoutData.WeaponJointNames[i], loadoutData.WeaponNames[i]);
				}
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
	}

	public void LoadSaveInStation()
	{

	}

	public void LoadNewGame()
	{
		CurrentSave = null;
		GameManager.Inst.PlayerControl.SpawnStationID = "alexandria_station";
		GameManager.Inst.PlayerControl.SpawnStationType = StationType.Station;
	}




	private LoadoutSaveData CreateLoadoutSaveData(Loadout loadout)
	{
		if(loadout == null)
		{
			return null;
		}
		LoadoutSaveData loadoutData = new LoadoutSaveData();
		loadoutData.ShipID = loadout.ShipID;
		loadoutData.ShipType = loadout.ShipType;
		loadoutData.WeaponJointNames = new List<string>();
		loadoutData.WeaponNames = new List<string>();
		foreach(KeyValuePair<string, string> joint in loadout.WeaponJoints)
		{
			loadoutData.WeaponJointNames.Add(joint.Key);
			loadoutData.WeaponNames.Add(joint.Value);
		}

		return loadoutData;
	}
}
