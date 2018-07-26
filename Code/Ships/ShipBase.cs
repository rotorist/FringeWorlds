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

	public float TorqueModifier = 1;
	public float HullCapacity;
	public float HullAmount;
	public float TotalPowerCapacity;
	public float WeaponPowerCapacity;
	public float WeaponPowerAmount;

	public bool IsInPortal;
	public float InPortalSpeed;
	public string DockedStationID;

	public Loadout MyLoadout;

	public virtual void Hide()
	{
		if(ShipModel != null)
		{
			//Debug.Log("HIDE SHIP " + this.name);
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
			//Debug.Log("SHOW SHIP " + this.name);
			Renderer shipRenderer = ShipModel.GetComponent<Renderer>();
			shipRenderer.enabled = true;
			foreach(Renderer r in ShipModel.GetComponentsInChildren<Renderer>())
			{
				if(r.tag != "Shield")
				{
					r.enabled = true;
				}
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

	public virtual void OnDeath(ShipBase attacker)
	{
		
		//process attacker

		//call death event
		GameEventHandler.Instance.TriggerShipDeath(this);

		//destroy ship
		GameObject.Destroy(this.gameObject);
	}

	public void ProcessHullDamage(Damage damage)
	{
		if(IsInPortal)
		{
			return;
		}

		HullAmount = Mathf.Clamp(HullAmount - damage.HullAmount, 0, HullCapacity);

		if(HullAmount <= 0 && GameManager.Inst.PlayerControl.PlayerShip != this)
		{
			OnDeath(this);
		}
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