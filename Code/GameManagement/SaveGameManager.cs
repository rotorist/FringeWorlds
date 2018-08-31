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
		Save();

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

		LevelAnchor anchor = GameObject.FindObjectOfType<LevelAnchor>();
		anchor.SpawnSystem = CurrentSave.SpawnSystem;
		anchor.ProfileName = GameManager.Inst.PlayerProgress.ProfileName;
	}

	public void CreateExitSave(StationBase station)
	{

	}

	public void Save()
	{
		CurrentSave = new SaveGame();

		//save all macroAI parties
		CurrentSave.AllParties = new List<MacroAIPartySaveData>();
		foreach(MacroAIParty party in GameManager.Inst.NPCManager.AllParties)
		{
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

	public void Load(string profileName)
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
			return false;
		}

		CurrentSave = (SaveGame)bf.Deserialize(file);

		//GameManager.Inst.PlayerControl.SpawnStationID = save.SpawnStationID;
		//GameManager.Inst.PlayerControl.SpawnStationType = save.SpawnStationType;
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
