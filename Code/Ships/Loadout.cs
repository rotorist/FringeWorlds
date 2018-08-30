using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loadout
{
	public string ShipID;
	public ShipType ShipType;
	public Dictionary<string, string> WeaponJoints;

	public Loadout(string shipID, ShipType shipType)
	{
		ShipID = shipID;
		ShipType = shipType;
		WeaponJoints = new Dictionary<string, string>();


	}

}

[System.Serializable]
public class LoadoutSaveData
{
	public string ShipID;
	public ShipType ShipType;
	public List<string> WeaponJointNames;
	public List<string> WeaponNames;

}