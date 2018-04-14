using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
		anchor.Save = CurrentSave;
	}

	public void CreateExitSave(StationBase station)
	{

	}

	public void Save()
	{
		CurrentSave = new SaveGame();
	}

	public void Load(SaveGame save)
	{
		CurrentSave = null;
		GameManager.Inst.PlayerControl.SpawnStationID = save.SpawnStationID;
		GameManager.Inst.PlayerControl.SpawnStationType = save.SpawnStationType;
	}

	public void LoadNewGame()
	{
		CurrentSave = null;
		GameManager.Inst.PlayerControl.SpawnStationID = "alexandria_station";
		GameManager.Inst.PlayerControl.SpawnStationType = StationType.Station;
	}



}
