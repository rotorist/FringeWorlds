using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour 
{
	public string DisplayName;
	public ShipBase ParentShip;
	public int Class;
	public float FireRate;
	public float Range;
	public GameObject TurretBase;
	public GameObject Barrel;
	public WeaponRotationType RotationType;
	public float GimballMax;
	public float GimballMin;
	public string AmmoID;
	public string AmmoType;
	public float PowerConsumption;
	public string FiringSound;
	public AudioSource Audio;
	public Item WeaponItem;

	public virtual void Initialize(InvItemData itemData, string ammoID)
	{

	}

	public virtual void Rebuild()
	{
		Audio = GetComponent<AudioSource>();
	}

	public virtual void Fire()
	{

	}

}

