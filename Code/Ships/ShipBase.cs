using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipBase : MonoBehaviour 
{
	public string ShipModelID;
	public GameObject ShipModel;
	public ShipType ShipType;
	public Rigidbody RB;
	public Engine Engine;
	public ShieldBase Shield;
	public Thruster Thruster;


	public float HullCapacity;
	public float HullAmount;
	public float TotalPowerCapacity;
	public float WeaponPowerCapacity;
	public float WeaponPowerAmount;

	public bool IsInPortal;
	public float InPortalSpeed;
}

public enum ShipType
{
	Fighter,
	Transporter,
	Gunship,
	CargoShip,
	BattleCruiser,

}