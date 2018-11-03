using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour 
{
	public string DisplayName;
	public ShipBase ParentShip;
	public float FireRate;
	public float Range;
	public GameObject TurretBase;
	public GameObject Barrel;
	public WeaponRotationType RotationType;
	public int MaxMagazine;
	public int Magazine;
	public float ReloadTime;
	public float GimballMax;
	public float GimballMin;
	public string AmmoID;
	public float PowerConsumption;
	public string FiringSound;
	public AudioSource Audio;

	public virtual void Rebuild()
	{
		Audio = GetComponent<AudioSource>();
	}

	public virtual void Fire()
	{

	}
}

