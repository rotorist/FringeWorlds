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
	public ShipStorage Storage;
	public WeaponCapacitor WeaponCapacitor;
	public ShipModSlots ShipModSlots;

	public float TorqueModifier;
	public float HullCapacity;
	public float HullAmount;

	public float PowerSupply;

	public float MaxFuel;
	public float FuelAmount;
	public float MaxLifeSupport;
	public float LifeSupportAmount;

	public Vector3 CurrentPowerMgmtButton;
	public float WeaponPowerAlloc;
	public float ShieldPowerAlloc;
	public float EnginePowerAlloc;

	public bool IsInPortal;
	public StationType InPortalStationType;
	public float InPortalSpeed;
	public string DockedStationID;

	public List<GameObject> IncomingMissiles;
	public GameObject CurrentCountermeasure;


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
				else 
				{
					if(this != GameManager.Inst.PlayerControl.PlayerShip)
					{
						r.enabled = false;
					}
					else
					{
						r.enabled = true;
					}
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

		//create explosion
		GameObject explosion = GameObject.Instantiate(Resources.Load("Explosion" + UnityEngine.Random.Range(1, 5).ToString())) as GameObject;
		explosion.transform.position = this.transform.position;

		if(attacker == GameManager.Inst.PlayerControl.PlayerShip)
		{
			GameManager.Inst.SoundManager.UI.PlayOneShot(GameManager.Inst.SoundManager.GetClip("EnemyKilledUI"));
		}
			
		//destroy ship
		GameObject.Destroy(this.gameObject);


	}

	public virtual void SetVortex(float width, float length)
	{
		if(MyReference.VortexLeft != null)
		{
			MyReference.VortexLeft.startWidth = width;
			MyReference.VortexLeft.endWidth = width;
			MyReference.VortexLeft.time = length;
		}
		if(MyReference.VortexRight != null)
		{
			MyReference.VortexRight.startWidth = width;
			MyReference.VortexRight.endWidth = width;
			MyReference.VortexRight.time = length;
		}
	}

	public void ProcessHullDamage(Damage damage, ShipBase attacker)
	{
		if(IsInPortal)
		{
			return;
		}
		Debug.Log("Current Hull " + HullAmount);
		HullAmount = Mathf.Clamp(HullAmount - damage.HullAmount, 0, HullCapacity);
		Debug.Log("current hull " + HullAmount + " damage " + damage.HullAmount);
		if(HullAmount <= 0 && GameManager.Inst.PlayerControl.PlayerShip != this)
		{
			OnDeath(attacker);
		}
	}

	public float GetMaxWeaponRange()
	{
		float range = 0;
		foreach(WeaponJoint joint in MyReference.WeaponJoints)
		{
			if(joint.MountedWeapon != null && joint.MountedWeapon.Range > range)
			{
				range = joint.MountedWeapon.Range;
			}
		}

		return range;
	}

	void OnCollisionEnter(Collision collision)
	{
		//Debug.Log("I'm hitting " + collision.collider.name);
	}

	void OnParticleCollision(GameObject other)
	{
		ParticleSystem part = other.GetComponent<ParticleSystem>();
		List<ParticleCollisionEvent> collisionEvents =  new List<ParticleCollisionEvent>();
		int numCollisionEvents = part.GetCollisionEvents(this.gameObject, collisionEvents);

		int i = 0;

		while (i < numCollisionEvents)
		{
			Vector3 pos = collisionEvents[i].intersection;
			Damage damage = new Damage();
			damage.ShieldAmount = 10;
			damage.HullAmount = 0;
			damage.HitLocation = pos;
			damage.DamageType = DamageType.Kinetic;

			Shield.ProcessDamage(damage);
			i++;
		}
	}
		
}

public enum ShipType
{
	Fighter,
	Transport,
	Gunship,
	CargoShip,
	BattleCruiser,

}