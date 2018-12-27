using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DemandResource
{
	public ResourceType Type;
	public string ItemID;
	public float DemandLevel;

}



public enum ResourceType
{
	None,
	Water,
	Methane,
	Oxygen,
	HFuel,
	Fertilizer,
	ConstructionMetal,
	VolatileMetal,
	PreciousMetal,
	RawMaterial,
	RareEarth,
	Food,
	LuxuryFood,
	Radioactive,
	Slaves,
	Refugees,
	Robotics,
	MachineComponent,
	WeaponComponent,
	EngineComponent,
	Chemicals,
}