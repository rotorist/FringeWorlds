using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Faction 
{

	public string ID;
	public string DisplayName;
	public string Description;

	public List<string> ShipPool;
	public List<string> EquipmentPool;

	public List<Loadout> FightersPool;
	public List<Loadout> FreightersPool;
	public List<Loadout> CapitalPool;

	public Faction()
	{
		
	}
}

[System.Serializable]
public class FactionRelationship
{
	public string Faction1;
	public string Faction2;
	public float Relationship;
}

[System.Serializable]
public class FactionRelationshipSaveData
{
	public List<FactionRelationship> Relationships;
}