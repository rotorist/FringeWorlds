using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour 
{
	public ShipBase ParentShip;
	public float FireRate;
	public float Range;
	public GameObject TurretBase;
	public GameObject Barrel;
	public WeaponRotationType RotationType;
	public int MaxMagazine;
	public int Magazine;
	public float GimballMax;
	public float GimballMin;

	public virtual void Rebuild()
	{

	}

	public virtual void Fire()
	{

	}
}

public enum WeaponRotationType
{
	Gimball,
	Turret,
	BallTurret,
	Fixed,
}