﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammunition : MonoBehaviour 
{
	public Damage Damage;
	public ShipBase Attacker;

	public virtual void OnTriggerEnter(Collider other)
	{
		//detect shield hit
		//if hitting shield, process damage with shield, if penetration damage is 0 then destroy projectile
		//else keep going until it hits hull
		//Debug.Log("Hit something " + other.name);
		if(other.tag == "Shield")
		{
			

			return;
		}
	}

	public virtual void OnCollisionEnter(Collision collision) 
	{
		//Debug.Log("Hit something " + collision.collider.name);

		ShipReference hitShip = null;
		//Debug.Log(collision.collider.name);


		hitShip = collision.collider.GetComponent<ShipReference>();



		if(hitShip != null)
		{
			if(hitShip.ParentShip == Attacker)
			{
				return;
			}
			else
			{
				//Debug.Log("Hit shield");
				ShieldBase shield = hitShip.Shield.GetComponent<ShieldBase>();
				if(shield.ParentShip == Attacker)
				{
					return;
				}

				Damage.HitLocation = collision.contacts[0].point;
				Damage = shield.ProcessDamage(Damage);
				if(Damage.HullAmount <= 1f)
				{
					GameObject.Destroy(this.gameObject);
					return;
				}

				hitShip.ParentShip.ProcessHullDamage(Damage);

				GameObject.Destroy(this.gameObject);
			}
		}
		else
		{
			GameObject.Destroy(this.gameObject);
		}


	}

}