using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage
{
	public DamageType DamageType;
	public float ShieldAmount;
	public float HullAmount;
	public Vector3 HitLocation;

}

public enum DamageType
{
	EMP,
	Photon,
	Antimatter,
	Kinetic,
	Shock,
}