using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShipModSlots : EquipmentBase
{
	public List<ShipMod> ShipMods;
	public int NumberOfSlots;
	public ShipMod ActiveMod;
	public ShipBase ParentShip;

	void Update()
	{
		if(ActiveMod != null)
		{
			ActiveMod.PerFrameUpdate();
		}
	}

	public void Initialize(InvItemData [] shipMods, ShipBase parent)
	{
		ParentShip = parent;
		ShipMods = new List<ShipMod>();

		//find the active mod
		int activeModIndex = -1;
		for(int i=0; i<shipMods.Length && i<NumberOfSlots; i++)
		{
			if(shipMods[i] != null && shipMods[i].Item.GetStringAttribute("Equipment Type") == "ActiveShipMod")
			{
				activeModIndex = i;
				string className = shipMods[i].Item.GetStringAttribute("Active Mod Class Name");
				ActiveMod = (ShipMod)System.Activator.CreateInstance(System.Type.GetType(className));
				ActiveMod.Initialize(shipMods[i]);
				ShipMods.Add(ActiveMod);
				Debug.Log("Found ship mod " + ActiveMod.Type);
			}

		}

		//now add the passive mods. this ensures the active mod is always the first one in the list
		for(int i=0; i<shipMods.Length && i<NumberOfSlots; i++)
		{
			if(shipMods[i] != null && i != activeModIndex)
			{
				ShipModType type = (ShipModType)Enum.Parse(typeof(ShipModType), shipMods[i].Item.GetStringAttribute("Ship Mod Type"));
				ShipMod mod = CreateShipModByType(type);
				mod.Initialize(shipMods[i]);
				ShipMods.Add(mod);
				Debug.Log("Found ship mod " + mod.Type);
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


	private ShipMod CreateShipModByType(ShipModType type)
	{
		switch(type)
		{
		case ShipModType.Engine:
			return new ShipModEngine();
		case ShipModType.Hull:
			return new ShipModHull();
		case ShipModType.Shield:
			return new ShipModShield();
		}

		return null;
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