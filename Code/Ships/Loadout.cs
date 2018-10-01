using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loadout
{
	public string ShipID;
	public ShipType ShipType;
	public Dictionary<string, string> WeaponJoints;
	public List<DefensiveType> Defensives;
	public List<string> DefensiveAmmoIDs;
	public List<InvItemData> AmmoBayItems;
	public List<InvItemData> CargoBayItems;

	public Loadout(string shipID, ShipType shipType)
	{
		ShipID = shipID;
		ShipType = shipType;
		WeaponJoints = new Dictionary<string, string>();
		AmmoBayItems = new List<InvItemData>();
		CargoBayItems = new List<InvItemData>();

	}

}

[System.Serializable]
public class LoadoutSaveData
{
	public string ShipID;
	public ShipType ShipType;
	public List<string> WeaponJointNames;
	public List<string> WeaponNames;
	public List<DefensiveType> Defensives;
	public List<string> DefensiveAmmoIDs;
	public List<InvItemData> AmmoBayItems;
	public List<InvItemData> CargoBayItems;

}