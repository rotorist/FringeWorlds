using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProgress
{
	public string ProfileName;
	public Loadout ActiveLoadout;

	public string SpawnStationID;
	public StationType SpawnStationType;
	public string SpawnSystemID;

	public void Initialize()
	{
		/*
		ActiveLoadout = new Loadout("Trimaran", ShipType.Transporter);

		ActiveLoadout.WeaponJoints = new Dictionary<string, string>()
		{
			{ "TurretLeft", "Class1Turret1" },
			{ "TurretRight", "Class1Turret1" },
			{ "TurretTop", "Class3Turret1" },
			{ "GimballLeft", "Class2Gun1" },
			{ "GimballRight", "Class2Gun1" },
		}; 
		*/


	}

	public void CreateInitialLoadout()
	{
		ActiveLoadout = new Loadout("Spitfire", ShipType.Fighter);
		ActiveLoadout.LoadoutID = "Player_Spitfire_1";
		ActiveLoadout.WeaponJoints = new Dictionary<string, string>()
		{
			{ "GimballLeft", "Class1Gun1" },
			{ "GimballRight", "Class1Gun1" },
			{ "GimballFront", "Class1Launcher1" },
		};

		ActiveLoadout.CurrentPowerMgmtButton = new Vector3(0, -20f, 0);
		ActiveLoadout.HullAmount = GameManager.Inst.ItemManager.AllShipStats[ActiveLoadout.ShipID].Hull;
		ActiveLoadout.FuelAmount = GameManager.Inst.ItemManager.AllShipStats[ActiveLoadout.ShipID].MaxFuel;
		ActiveLoadout.LifeSupportAmount = GameManager.Inst.ItemManager.AllShipStats[ActiveLoadout.ShipID].LifeSupport;

		ActiveLoadout.Defensives = new List<DefensiveType>()
		{
			DefensiveType.Countermeasure,
		};
		ActiveLoadout.DefensiveAmmoIDs = new List<string>()
		{
			"LongDurationCM",
		};
		Item item = new Item();
		item.ID = "Class1Missile1";
		item.DisplayName = "Seeker Missile";
		item.Type = ItemType.Ammo;
		item.CargoUnits = 2;
		InvItemData itemData = new InvItemData();
		itemData.Item = item;
		itemData.Quantity = 12;
		ActiveLoadout.AmmoBayItems.Add(itemData);

		Item item2 = new Item();
		item2.ID = "LongDurationCM";
		item2.DisplayName = "Long Duration Countermeasure";
		item2.Type = ItemType.Ammo;
		item2.CargoUnits = 2;
		InvItemData itemData2 = new InvItemData();
		itemData2.Item = item2;
		itemData2.Quantity = 10;
		ActiveLoadout.AmmoBayItems.Add(itemData2);

		ProfileName = "Kurt";
	}
}
