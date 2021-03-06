﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Ammunition 
{
	
	public Vector3 Velocity;
	public float Range;

	public float AdditionalShieldDamage;
	public float AdditionalHullDamage;

	private float _distTraveled;
	private float _destroyTimer;
	private bool _isDestroyed;
	private Rigidbody _rigidbody;
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		if(!_isDestroyed)
		{
			_distTraveled += Velocity.magnitude * Time.fixedDeltaTime;


			if(_distTraveled > 3)
			{
				//GetComponent<TrailRenderer>().enabled = true;
			}

			if(_distTraveled > Range)
			{
				GameObject.Destroy(this.gameObject);
			}
		}
		else
		{
			if(!_rigidbody.isKinematic)
			{
				_rigidbody.velocity = Vector3.zero;
				GetComponent<BoxCollider>().enabled = false;
				_rigidbody.isKinematic = true;
			}

			if(_destroyTimer < 0.5f)
			{
				_destroyTimer += Time.fixedDeltaTime;
			}
			else
			{
				GameObject.Destroy(this.gameObject);
			}
		}
	}

	public override void OnTriggerEnter(Collider other)
	{
		base.OnTriggerEnter(other);
	}

	public override void OnCollisionEnter(Collision collision) 
	{
		base.OnCollisionEnter(collision);

	}


	public void Fire(Vector3 velocity, float range, ShipBase attacker)
	{
		_distTraveled = 0;
		_destroyTimer = 0;
		_isDestroyed = false;

		Range = range;
		Velocity = velocity;

		_rigidbody = GetComponent<Rigidbody>();
		_rigidbody.velocity = velocity;

		Damage.ShieldAmount += AdditionalShieldDamage;
		Damage.HullAmount += AdditionalHullDamage;

		Attacker = attacker;

		//GetComponent<TrailRenderer>().enabled = false;
	}
}

