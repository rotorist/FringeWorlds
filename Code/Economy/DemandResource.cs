using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DemandResource
{
	public ResourceType Type;
	public string ItemID;
	public float DemandLevel;
	public float CurrentDemand;
}



public enum ResourceType
{
	None=0,
	Water=1,
	Methane=2,
	Oxygen=3,
	HFuel=4,
	Fertilizer=5,
	ConstructionMetal=6,
	VolatileMetal=7,
	PreciousMetal=8,
	RawMaterial=9,
	RareEarth=10,
	Food=11,
	LuxuryFood=12,
	Radioactive=13,
	Slaves=14,
	Refugees=15,
	Robotics=16,
	MachineComponent=17,
	WeaponComponent=18,
	EngineComponent=19,
	Chemicals=20,
}