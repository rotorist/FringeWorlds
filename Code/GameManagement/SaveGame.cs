using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveGame
{
	public string ProfileName;
	public string SpawnSystem;
	public string SpawnStationID;
	public StationType SpawnStationType;

	public List<MacroAIPartySaveData> AllParties;
}
