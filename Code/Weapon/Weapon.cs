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

	public virtual void Rebuild()
	{

	}

	public virtual void Fire()
	{

	}
}
