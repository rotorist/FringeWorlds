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

}
