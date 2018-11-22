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
		ActiveLoadout.HullAmount = GameManager.Inst.ItemManager.GetShipStats(ActiveLoadout.ShipID).Hull;
		ActiveLoadout.FuelAmount = GameManager.Inst.ItemManager.GetShipStats(ActiveLoadout.ShipID).MaxFuel;
		ActiveLoadout.LifeSupportAmount = GameManager.Inst.ItemManager.GetShipStats(ActiveLoadout.ShipID).LifeSupport;

		ActiveLoadout.DefenseDeviceIDs = new List<string>()
		{
			"dfs_CMDispenser",
		};
		ActiveLoadout.Defensives = new List<DefensiveType>()
		{
			DefensiveType.Countermeasure,
		};
		ActiveLoadout.DefensiveAmmoIDs = new List<string>()
		{
			"ammo_LongDurationCM",
		};

		ActiveLoadout.ShipMods = new InvItemData[GameManager.Inst.ItemManager.GetShipStats(ActiveLoadout.ShipID).ModSlots];

		Item item = new Item(GameManager.Inst.ItemManager.GetItemStats("ammo_Class1Missile1"));
		InvItemData itemData = new InvItemData();
		itemData.Item = item;
		itemData.Quantity = 12;
		ActiveLoadout.AmmoBayItems.Add(itemData);

		Item item2 = new Item(GameManager.Inst.ItemManager.GetItemStats("ammo_LongDurationCM"));
		InvItemData itemData2 = new InvItemData();
		itemData2.Item = item2;
		itemData2.Quantity = 10;
		ActiveLoadout.AmmoBayItems.Add(itemData2);
		//Debug.Log(ActiveLoadout.AmmoBayItems[1].Item.ID);

		Item item3 = new Item(GameManager.Inst.ItemManager.GetItemStats("shd_KeslerFighterShieldMK1"));
		InvItemData itemData3 = new InvItemData();
		itemData3.Item = item3;
		itemData3.Quantity = 1;
		ActiveLoadout.CargoBayItems.Add(itemData3);

		Item item4 = new Item(GameManager.Inst.ItemManager.GetItemStats("shd_KeslerFighterShieldMK1"));
		InvItemData itemData4 = new InvItemData();
		itemData4.Item = item4;
		itemData4.Quantity = 1;
		ActiveLoadout.Shield = itemData4;

		Item item5 = new Item(GameManager.Inst.ItemManager.GetItemStats("wc_NCPWeaponCapacitorMK1"));
		InvItemData itemData5 = new InvItemData();
		itemData5.Item = item5;
		itemData5.Quantity = 1;
		ActiveLoadout.CargoBayItems.Add(itemData5);

		Item item6 = new Item(GameManager.Inst.ItemManager.GetItemStats("thr_StrelSkyThrusterMK1"));
		InvItemData itemData6 = new InvItemData();
		itemData6.Item = item6;
		itemData6.Quantity = 1;
		ActiveLoadout.Thruster = itemData6;

		Item item7 = new Item(GameManager.Inst.ItemManager.GetItemStats("scn_RadianTekShortRangeScanner"));
		InvItemData itemData7 = new InvItemData();
		itemData7.Item = item7;
		itemData7.Quantity = 1;
		ActiveLoadout.CargoBayItems.Add(itemData7);

		Item item8 = new Item(GameManager.Inst.ItemManager.GetItemStats("mod_ShieldConnectorTuningStage1"));
		InvItemData itemData8 = new InvItemData();
		itemData8.Item = item8;
		itemData8.Quantity = 1;
		ActiveLoadout.CargoBayItems.Add(itemData8);

		Item item9 = new Item(GameManager.Inst.ItemManager.GetItemStats("mod_ShieldConnectorTuningStage2"));
		InvItemData itemData9 = new InvItemData();
		itemData9.Item = item9;
		itemData9.Quantity = 1;
		ActiveLoadout.CargoBayItems.Add(itemData9);

		ProfileName = "Kurt";
	}
}
