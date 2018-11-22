using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipModSlots : EquipmentBase
{
	public List<ShipMod> ShipMods;
	public int NumberOfSlots;
	public ShipMod ActiveMod;
	public ShipBase ParentShip;

	public void Initialize(InvItemData [] shipMods, ShipBase parent)
	{
		ParentShip = parent;
		ShipMods = new List<ShipMod>();

		//find the active mod
		int activeModIndex = -1;
		for(int i=0; i<shipMods.Length && i<NumberOfSlots; i++)
		{
			if(shipMods[i].Item.GetStringAttribute("Equipment Type") == "ActiveShipMod")
			{
				activeModIndex = i;
				ActiveMod = new ShipMod();
				ActiveMod.Initialize(shipMods[i]);
				ShipMods.Add(ActiveMod);
			}

		}

		//now add the passive mods. this ensures the active mod is always the first one in the list
		for(int i=0; i<shipMods.Length && i<NumberOfSlots; i++)
		{
			if(i != activeModIndex)
			{
				ShipMod mod = new ShipMod();
				mod.Initialize(shipMods[i]);
				ShipMods.Add(mod);
			}
		}
	}

	public void ApplyMods()
	{
		if(ShipMods == null || ShipMods.Count <= 0)
		{
			return;
		}

		foreach(ShipMod mod in ShipMods)
		{
			mod.ApplyModToShip(ParentShip);
		}
	}
}

public enum ShipModType
{
	Active,
	Shield,
	Hull,
	Engine,
	Turn,
	Storage,
	Weapon,
}