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

	public List<string> HomeBaseIDs;

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

		ActiveLoadout.CurrentPowerMgmtButton = new Vector3(0, -20f, 0);
		ActiveLoadout.HullAmount = GameManager.Inst.ItemManager.GetShipStats(ActiveLoadout.ShipID).Hull;
		ActiveLoadout.FuelAmount = GameManager.Inst.ItemManager.GetShipStats(ActiveLoadout.ShipID).MaxFuel;
		ActiveLoadout.LifeSupportAmount = GameManager.Inst.ItemManager.GetShipStats(ActiveLoadout.ShipID).LifeSupport;

		ActiveLoadout.Defensives = new List<InvItemData>();

		ActiveLoadout.ShipMods = new InvItemData[GameManager.Inst.ItemManager.GetShipStats(ActiveLoadout.ShipID).ModSlots];
		{
			Item item = new Item(GameManager.Inst.ItemManager.GetItemStats("ammo_StrelskySeekerMissile"));
			InvItemData itemData = new InvItemData();
			itemData.Item = item;
			itemData.Quantity = 12;


			Item item2 = new Item(GameManager.Inst.ItemManager.GetItemStats("ammo_LongDurationCM"));
			InvItemData itemData2 = new InvItemData();
			itemData2.Item = item2;
			itemData2.Quantity = 10;

			//Debug.Log(ActiveLoadout.AmmoBayItems[1].Item.ID);

			Item item3 = new Item(GameManager.Inst.ItemManager.GetItemStats("shd_KeslerFighterShieldMK1"));
			InvItemData itemData3 = new InvItemData();
			itemData3.Item = item3;
			itemData3.Quantity = 1;


			Item item4 = new Item(GameManager.Inst.ItemManager.GetItemStats("shd_KeslerFighterShieldMK1"));
			InvItemData itemData4 = new InvItemData();
			itemData4.Item = item4;
			itemData4.Quantity = 1;


			Item item5 = new Item(GameManager.Inst.ItemManager.GetItemStats("wc_VikoWeaponCapacitorMK1"));
			InvItemData itemData5 = new InvItemData();
			itemData5.Item = item5;
			itemData5.Quantity = 1;


			Item item6 = new Item(GameManager.Inst.ItemManager.GetItemStats("thr_StrelskyThrusterMK1"));
			InvItemData itemData6 = new InvItemData();
			itemData6.Item = item6;
			itemData6.Quantity = 1;


			Item item7 = new Item(GameManager.Inst.ItemManager.GetItemStats("scn_RadianTekShortRangeScanner"));
			InvItemData itemData7 = new InvItemData();
			itemData7.Item = item7;
			itemData7.Quantity = 1;


			Item item8 = new Item(GameManager.Inst.ItemManager.GetItemStats("mod_ShieldConnectorTuningStage1"));
			InvItemData itemData8 = new InvItemData();
			itemData8.Item = item8;
			itemData8.Quantity = 1;


			Item item9 = new Item(GameManager.Inst.ItemManager.GetItemStats("mod_ShieldConnectorTuningStage2"));
			InvItemData itemData9 = new InvItemData();
			itemData9.Item = item9;
			itemData9.Quantity = 1;


			Item item10 = new Item(GameManager.Inst.ItemManager.GetItemStats("shd_NCPTransporterShieldMK1"));
			InvItemData itemData10 = new InvItemData();
			itemData10.Item = item10;
			itemData10.Quantity = 1;


			Item item11 = new Item(GameManager.Inst.ItemManager.GetItemStats("mod_ShieldCapacitorGrade1"));
			InvItemData itemData11 = new InvItemData();
			itemData11.Item = item11;
			itemData11.Quantity = 1;


			Item item12 = new Item(GameManager.Inst.ItemManager.GetItemStats("dfs_CMDispenser"));
			InvItemData itemData12 = new InvItemData();
			itemData12.Item = item12;
			itemData12.Quantity = 1;
			itemData12.RelatedItemID = "ammo_LongDurationCM";

			Item item15 = new Item(GameManager.Inst.ItemManager.GetItemStats("wpn_AlFasadStingerPulseCannon"));
			InvItemData itemData15 = new InvItemData();
			itemData15.Item = item15;
			itemData15.Quantity = 1;

			Item item16 = new Item(GameManager.Inst.ItemManager.GetItemStats("wpn_StrelskyScreamerMissileLauncher"));
			InvItemData itemData16 = new InvItemData();
			itemData16.Item = item16;
			itemData16.Quantity = 1;
			itemData16.RelatedItemID = "ammo_StrelskySeekerMissile";

			Item item17 = new Item(GameManager.Inst.ItemManager.GetItemStats("wpn_SDCStomperAutocannon"));
			InvItemData itemData17 = new InvItemData();
			itemData17.Item = item17;
			itemData17.Quantity = 1;

			Item item18 = new Item(GameManager.Inst.ItemManager.GetItemStats("wpn_AlFasadStingerPulseCannon"));
			InvItemData itemData18 = new InvItemData();
			itemData18.Item = item18;
			itemData18.Quantity = 1;

			Item item21 = new Item(GameManager.Inst.ItemManager.GetItemStats("ammo_20mmTitaniumSlugs"));
			InvItemData itemData21 = new InvItemData();
			itemData21.Item = item21;
			itemData21.Quantity = 500;

			ActiveLoadout.Shield = itemData4;
			ActiveLoadout.AmmoBayItems.Add(itemData);
			ActiveLoadout.AmmoBayItems.Add(itemData2);
			ActiveLoadout.AmmoBayItems.Add(itemData21);
			ActiveLoadout.CargoBayItems.Add(itemData17);
			ActiveLoadout.CargoBayItems.Add(itemData3);
			ActiveLoadout.CargoBayItems.Add(itemData5);
			ActiveLoadout.Thruster = itemData6;
			ActiveLoadout.CargoBayItems.Add(itemData7);
			ActiveLoadout.CargoBayItems.Add(itemData8);
			ActiveLoadout.CargoBayItems.Add(itemData9);
			ActiveLoadout.CargoBayItems.Add(itemData10);
			ActiveLoadout.CargoBayItems.Add(itemData11);
			ActiveLoadout.Defensives.Add(itemData12);

			ActiveLoadout.WeaponJoints = new Dictionary<string, InvItemData>()
			{
				{ "GimballLeft", itemData15 },
				{ "GimballRight", itemData18 },
				{ "GimballFront", null },
			};
		}

		//create a second loadout for testing
		Loadout StoredLoadout = new Loadout("Trimaran", ShipType.Transport);
		StoredLoadout.LoadoutID = "Player_Trimaran_1";
		StoredLoadout.CurrentPowerMgmtButton = new Vector3(0, -20f, 0);
		StoredLoadout.HullAmount = GameManager.Inst.ItemManager.GetShipStats(StoredLoadout.ShipID).Hull;
		StoredLoadout.FuelAmount = GameManager.Inst.ItemManager.GetShipStats(StoredLoadout.ShipID).MaxFuel;
		StoredLoadout.LifeSupportAmount = GameManager.Inst.ItemManager.GetShipStats(StoredLoadout.ShipID).LifeSupport;

		StoredLoadout.Defensives = new List<InvItemData>();

		StoredLoadout.ShipMods = new InvItemData[GameManager.Inst.ItemManager.GetShipStats(StoredLoadout.ShipID).ModSlots];
		{
			Item item = new Item(GameManager.Inst.ItemManager.GetItemStats("ammo_StrelskySeekerMissile"));
			InvItemData itemData = new InvItemData();
			itemData.Item = item;
			itemData.Quantity = 12;

			Item item2 = new Item(GameManager.Inst.ItemManager.GetItemStats("ammo_LongDurationCM"));
			InvItemData itemData2 = new InvItemData();
			itemData2.Item = item2;
			itemData2.Quantity = 10;

			//Debug.Log(ActiveLoadout.AmmoBayItems[1].Item.ID);

			Item item3 = new Item(GameManager.Inst.ItemManager.GetItemStats("shd_KeslerFighterShieldMK1"));
			InvItemData itemData3 = new InvItemData();
			itemData3.Item = item3;
			itemData3.Quantity = 1;


			Item item4 = new Item(GameManager.Inst.ItemManager.GetItemStats("shd_KeslerFighterShieldMK1"));
			InvItemData itemData4 = new InvItemData();
			itemData4.Item = item4;
			itemData4.Quantity = 1;


			Item item5 = new Item(GameManager.Inst.ItemManager.GetItemStats("wc_VikoWeaponCapacitorMK1"));
			InvItemData itemData5 = new InvItemData();
			itemData5.Item = item5;
			itemData5.Quantity = 1;


			Item item6 = new Item(GameManager.Inst.ItemManager.GetItemStats("thr_StrelskyThrusterMK1"));
			InvItemData itemData6 = new InvItemData();
			itemData6.Item = item6;
			itemData6.Quantity = 1;


			Item item7 = new Item(GameManager.Inst.ItemManager.GetItemStats("scn_RadianTekShortRangeScanner"));
			InvItemData itemData7 = new InvItemData();
			itemData7.Item = item7;
			itemData7.Quantity = 1;


			Item item8 = new Item(GameManager.Inst.ItemManager.GetItemStats("mod_ShieldConnectorTuningStage1"));
			InvItemData itemData8 = new InvItemData();
			itemData8.Item = item8;
			itemData8.Quantity = 1;


			Item item9 = new Item(GameManager.Inst.ItemManager.GetItemStats("mod_ShieldConnectorTuningStage2"));
			InvItemData itemData9 = new InvItemData();
			itemData9.Item = item9;
			itemData9.Quantity = 1;


			Item item10 = new Item(GameManager.Inst.ItemManager.GetItemStats("shd_NCPTransporterShieldMK1"));
			InvItemData itemData10 = new InvItemData();
			itemData10.Item = item10;
			itemData10.Quantity = 1;


			Item item11 = new Item(GameManager.Inst.ItemManager.GetItemStats("mod_ShieldCapacitorGrade1"));
			InvItemData itemData11 = new InvItemData();
			itemData11.Item = item11;
			itemData11.Quantity = 1;


			Item item12 = new Item(GameManager.Inst.ItemManager.GetItemStats("dfs_CMDispenser"));
			InvItemData itemData12 = new InvItemData();
			itemData12.Item = item12;
			itemData12.Quantity = 1;
			itemData12.RelatedItemID = "ammo_LongDurationCM";

			Item item15 = new Item(GameManager.Inst.ItemManager.GetItemStats("wpn_AlFasadStingerPulseCannon"));
			InvItemData itemData15 = new InvItemData();
			itemData15.Item = item15;
			itemData15.Quantity = 1;

			Item item16 = new Item(GameManager.Inst.ItemManager.GetItemStats("wpn_StrelskyScreamerMissileLauncher"));
			InvItemData itemData16 = new InvItemData();
			itemData16.Item = item16;
			itemData16.Quantity = 1;
			itemData16.RelatedItemID = "ammo_StrelskySeekerMissile";

			Item item17 = new Item(GameManager.Inst.ItemManager.GetItemStats("wpn_SDCStomperAutocannon"));
			InvItemData itemData17 = new InvItemData();
			itemData17.Item = item17;
			itemData17.Quantity = 1;

			Item item18 = new Item(GameManager.Inst.ItemManager.GetItemStats("wpn_AlFasadStingerPulseCannon"));
			InvItemData itemData18 = new InvItemData();
			itemData18.Item = item18;
			itemData18.Quantity = 1;

			Item item21 = new Item(GameManager.Inst.ItemManager.GetItemStats("ammo_20mmTitaniumSlugs"));
			InvItemData itemData21 = new InvItemData();
			itemData21.Item = item21;
			itemData21.Quantity = 500;

			StoredLoadout.Shield = itemData10;
			StoredLoadout.AmmoBayItems.Add(itemData2);
			StoredLoadout.AmmoBayItems.Add(itemData21);
			StoredLoadout.CargoBayItems.Add(itemData5);
			StoredLoadout.CargoBayItems.Add(itemData7);
			StoredLoadout.CargoBayItems.Add(itemData8);
			StoredLoadout.CargoBayItems.Add(itemData9);
			StoredLoadout.CargoBayItems.Add(itemData11);
			StoredLoadout.Defensives.Add(itemData12);

			StoredLoadout.WeaponJoints = new Dictionary<string, InvItemData>()
			{
				{ "GimballLeft", itemData15 },
				{ "GimballRight", itemData18 },
				{ "TurretLeft", null },
				{ "TurretRight", null },
				{ "TurretTop", null },
			};

		}

		//create a garage and vault in planet colombia
		HomeStationData homeStation = new HomeStationData();
		homeStation.HangarSize = 5;
		homeStation.VaultSize = 350;
		homeStation.ShipsInHangar.Add(StoredLoadout);

		GameManager.Inst.WorldManager.DockableStationDatas["planet_colombia_landing"].HomeStationData = homeStation;


		ProfileName = "Kurt";
	}
}
