using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipBase : MonoBehaviour 
{
	public string ShipModelID;
	public GameObject ShipModel;
	public ShipReference MyReference;
	public AI MyAI 
	{
		get { return GetComponent<AI>(); }
	}
	public Rigidbody RB;
	public Engine Engine;
	public ShieldBase Shield;
	public Thruster Thruster;
	public Scanner Scanner;

	public float HullCapacity;
	public float HullAmount;
	public float TotalPowerCapacity;
	public float WeaponPowerCapacity;
	public float WeaponPowerAmount;

	public bool IsInPortal;
	public float InPortalSpeed;
	public string DockedStationID;

	public virtual void Hide()
	{
		if(ShipModel != null)
		{
			Renderer shipRenderer = ShipModel.GetComponent<Renderer>();
			shipRenderer.enabled = false;
			foreach(Renderer r in ShipModel.GetComponentsInChildren<Renderer>())
			{
				r.enabled = false;
			}
			DisableColliders();
		}
	}

	public virtual void Show()
	{
		if(ShipModel != null)
		{
			Renderer shipRenderer = ShipModel.GetComponent<Renderer>();
			shipRenderer.enabled = true;
			foreach(Renderer r in ShipModel.GetComponentsInChildren<Renderer>())
			{
				r.enabled = true;
			}
			EnableColliders();
		}
	}

	public virtual void EnableColliders()
	{

	}

	public virtual void DisableColliders()
	{

	}
}

public enum ShipType
{
	Fighter,
	Transporter,
	Gunship,
	CargoShip,
	BattleCruiser,

}