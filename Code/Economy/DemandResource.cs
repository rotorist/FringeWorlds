using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DemandResource
{
	public ResourceType Type;
	public float DemandLevel;

}



public enum ResourceType
{
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
	Robotics,
	MachineComponent,
	WeaponComponent,
	EngineComponent,
	Chemicals,
}