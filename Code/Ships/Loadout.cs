using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loadout
{
	public string LoadoutID;
	public string ShipID;
	public ShipType ShipType;
	public Dictionary<string, string> WeaponJoints;
	public List<string> DefenseDeviceIDs;
	public List<DefensiveType> Defensives;
	public List<string> DefensiveAmmoIDs;
	public List<InvItemData> AmmoBayItems;
	public List<InvItemData> CargoBayItems;
	public string ShieldID;
	public string WeaponCapacitorID;
	public string ThrusterID;
	public string ScannerID;
	public string TeleporterID;
	public List<string> ShipModIDs;

	public Vector3 CurrentPowerMgmtButton;
	public float HullAmount;
	public float FuelAmount;
	public float LifeSupportAmount;



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
	public string LoadoutID;
	public string ShipID;
	public ShipType ShipType;
	public List<string> WeaponJointNames;
	public List<string> WeaponNames;
	public List<string> DefenseDeviceIDs;
	public List<DefensiveType> Defensives;
	public List<string> DefensiveAmmoIDs;
	public List<InvItemData> AmmoBayItems;
	public List<InvItemData> CargoBayItems;
	public string ShieldID;
	public string WeaponCapacitorID;
	public string ThrusterID;
	public string ScannerID;
	public string TeleporterID;
	public List<string> ShipModIDs;

	public SerVector3 CurrentPowerMgmtButton;
	public float HullAmount;
	public float FuelAmount;
	public float LifeSupportAmount;

}