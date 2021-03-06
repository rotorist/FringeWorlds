﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loadout
{
	public string LoadoutID;
	public string ShipID;
	public ShipType ShipType;
	public Dictionary<string, InvItemData> WeaponJoints;
	public List<InvItemData> Defensives;
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

	public Loadout(Loadout template)
	{
		LoadoutID = template.LoadoutID;
		ShipID = template.ShipID;
		ShipType = template.ShipType;
		WeaponJoints = new Dictionary<string, InvItemData>();
		foreach(KeyValuePair<string, InvItemData> weaponJoint in template.WeaponJoints)
		{
			WeaponJoints.Add(weaponJoint.Key, new InvItemData(weaponJoint.Value));
		}

		Defensives = new List<InvItemData>();
		foreach(InvItemData itemData in template.Defensives)
		{
			Defensives.Add(new InvItemData(itemData));
		}

		AmmoBayItems = new List<InvItemData>();
		foreach(InvItemData itemData in template.AmmoBayItems)
		{
			AmmoBayItems.Add(new InvItemData(itemData));
		}

		CargoBayItems = new List<InvItemData>();
		foreach(InvItemData itemData in template.CargoBayItems)
		{
			CargoBayItems.Add(new InvItemData(itemData));
		}

		if(template.Shield != null)
		{
			Shield = new InvItemData(template.Shield);
		}
		if(template.WeaponCapacitor != null)
		{
			WeaponCapacitor = new InvItemData(template.WeaponCapacitor);
		}
		if(template.Thruster != null)
		{
			Thruster = new InvItemData(template.Thruster);
		}
		if(template.Scanner != null)
		{
			Scanner = new InvItemData(template.Scanner);
		}
		if(template.Teleporter != null)
		{
			Teleporter = new InvItemData(template.Teleporter);
		}

		if(template.ShipMods != null)
		{
			ShipMods = new InvItemData[template.ShipMods.Length];
			for(int i=0; i<template.ShipMods.Length; i++)
			{
				if(template.ShipMods[i] != null)
				{
					ShipMods[i] = new InvItemData(template.ShipMods[i]);
				}
			}
		}
		else
		{
			ShipMods = new InvItemData[0];
		}

		CurrentPowerMgmtButton = template.CurrentPowerMgmtButton;
		HullAmount = template.HullAmount;
		FuelAmount = template.FuelAmount;
		LifeSupportAmount = template.LifeSupportAmount;
	}

	public Loadout(string shipID, ShipType shipType)
	{
		ShipID = shipID;
		ShipType = shipType;
		WeaponJoints = new Dictionary<string, InvItemData>();
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

	public void SetWeaponInvItem(string jointID, InvItemData invItem)
	{
		WeaponJoints[jointID] = invItem;
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

	public InvItemData GetShipModFromID(string itemID)
	{
		for(int i=0; i<ShipMods.Length; i++)
		{
			if(ShipMods[i] != null && ShipMods[i].Item.ID  == itemID)
			{
				return ShipMods[i];
			}
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

	public void RemoveShipModByIndex(int index)
	{
		ShipMods[index] = null;
	}

	public void RemoveShipMod(InvItemData invItem)
	{
		for(int i=0; i<ShipMods.Length; i++)
		{
			if(ShipMods[i] == invItem)
			{
				ShipMods[i] = null;
			}
		}
	}

	public List<InvItemData> GetModDependencies(string itemID)
	{
		List<InvItemData> dependencies = new List<InvItemData>();
		for(int i=0; i<ShipMods.Length; i++)
		{
			if(ShipMods[i] == null)
			{
				continue;
			}
			if(ShipMods[i].Item.GetStringAttribute("Dependency") == itemID)
			{
				dependencies.Add(ShipMods[i]);
			}
		}

		return dependencies;
	}

	public float GetCargoBayUsage()
	{
		float usage = 0;
		foreach(InvItemData item in CargoBayItems)
		{
			usage += item.Quantity * item.Item.CargoUnits;
		}

		return usage;
	}
}

[System.Serializable]
public class LoadoutSaveData
{
	public string LoadoutID;
	public string ShipID;
	public ShipType ShipType;
	public List<string> WeaponJointNames;
	public List<InvItemData> Weapons;
	public List<InvItemData> Defensives;
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