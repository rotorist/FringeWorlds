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
	public InvItemData Shield;
	public InvItemData WeaponCapacitor;
	public InvItemData Thruster;
	public InvItemData Scanner;
	public InvItemData Teleporter;
	public InvItemData [] ShipMods;

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

	public InvItemData GetInvItemFromEquipmentType(string equipmentType)
	{
		switch(equipmentType)
		{
		case "Shield":
			return Shield;
		case "WeaponCapacitor":
			return WeaponCapacitor;
		case "Thruster":
			return Thruster;
		case "Scanner":
			return Scanner;
		case "Teleportor":
			return Teleporter;
			break;
		}
		return null;
	}
	
	public void SetEquipmentInvItem(InvItemData invItem, string equipmentType)
	{
		switch(equipmentType)
		{
		case "Shield":
			Shield  = invItem;
			break;
		case "WeaponCapacitor":
			WeaponCapacitor = invItem;
			break;
		case "Thruster":
			Thruster = invItem;
			break;
		case "Scanner":
			Scanner = invItem;
			break;
		case "Teleporter":
			Teleporter = invItem;
			break;
		}
	}

	public bool SetShipModInvItem(InvItemData invItem, string equipmentType)
	{
		if(equipmentType == "PassiveShipMod")
		{
			for(int i=0; i<ShipMods.Length; i++)
			{
				if(ShipMods[i] == null)
				{
					ShipMods[i] = invItem;
					return true;
				}
			}
			return false;
		}
		else if(equipmentType == "ActiveShipMod")
		{
			if(ShipMods.Length > 0 && ShipMods[0] == null)
			{
				ShipMods[0] = invItem;
				return true;
			}

			return false;
		}

		return false;
	}

	public InvItemData GetShipModFromSlotNumber(int slotNumber)
	{
		if(ShipMods.Length > slotNumber)
		{
			return ShipMods[slotNumber];
		}

		return null;
	}

	public void ClearEquipment(InvItemData invItem)
	{
		if(Shield == invItem)
		{
			Shield = null;
		}
		else if(WeaponCapacitor == invItem)
		{
			WeaponCapacitor = null;
		}
		else if(Thruster == invItem)
		{
			Thruster = null;
		}
		else if(Scanner == invItem)
		{
			Scanner = null;
		}
		else if(Teleporter == invItem)
		{
			Teleporter = null;
		}
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
	public InvItemData Shield;
	public InvItemData WeaponCapacitor;
	public InvItemData Thruster;
	public InvItemData Scanner;
	public InvItemData Teleporter;
	public InvItemData [] ShipMods;

	public SerVector3 CurrentPowerMgmtButton;
	public float HullAmount;
	public float FuelAmount;
	public float LifeSupportAmount;

}