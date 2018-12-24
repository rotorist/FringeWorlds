using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShipStats 
{
	public string ID;
	public string DisplayName;
	public string HUDName;
	public string Description;
	public float Hull;
	public float PowerSupply;
	public float TurnRate;
	public float Weight;
	public int ModSlots;
	public List<WeaponJointData> WeaponJoints;
	public int DefenseSlots;
	public float MaxSpeed;
	public float Acceleration;
	public float CruiseSpeed;
	public float CruisePrepTime;
	public float MaxFuel;
	public float LifeSupport;
	public ShieldClass ShieldClass;
	public float AmmoBaySize;
	public float CargoBaySize;


}

