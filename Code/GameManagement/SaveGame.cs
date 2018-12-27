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

	public int LastUsedPartyNumber;
	public List<MacroAIPartySaveData> AllNonPlayerParties;

	public List<DockableStationSaveData> DockableStationDatas;

	public List<LoadoutSaveData> PlayerLoadouts;
	public string PlayerActiveLoadoutID;
	public int PlayerCredits; 
}
