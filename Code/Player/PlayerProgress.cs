﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProgress
{
	public string ProfileName;
	public Loadout ActiveLoadout;

	public void Initialize()
	{
		/*
		ActiveLoadout = new Loadout("Trimaran", ShipType.Transporter);
		ActiveLoadout.WeaponJoints = new Dictionary<string, string>()
		{
			{ "TurretLeft", "Class1Turret1" },
			{ "TurretRight", "Class1Turret1" },
			{ "TurretTop", "Class3Turret1" },
		}; 
*/
		ActiveLoadout = new Loadout("Spitfire", ShipType.Fighter);
		ActiveLoadout.WeaponJoints = new Dictionary<string, string>()
		{
			{ "GimballLeft", "Class2Gun1" },
			{ "GimballRight", "Class2Gun1" },
			{ "GimballFront", "Class1Launcher1" },
		};
		Item item = new Item();
		item.ID = "Class1Missile1";
		item.DisplayName = "Seeker Missile";
		item.Type = ItemType.Ammo;
		item.CargoUnits = 2;
		InvItemData itemData = new InvItemData();
		itemData.Item = item;
		itemData.Quantity = 5;
		ActiveLoadout.AmmoBayItems.Add(itemData);

		ProfileName = "Kurt";
	}
}
