using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour 
{
	public ShipBase Attacker;
	public Vector3 Velocity;
	public float Range;
	public Damage Damage;

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

	void OnTriggerEnter(Collider other)
	{
		//detect shield hit
		//if hitting shield, process damage with shield, if penetration damage is 0 then destroy projectile
		//else keep going until it hits hull
		Debug.Log("Hit something " + other.name);
		if(other.tag == "Shield")
		{
			Debug.Log("Hit shield");
			ShieldBase shield = other.GetComponent<ShieldBase>();
			Damage.HitLocation = transform.position;
			Damage result = shield.ProcessDamage(Damage);
			if(result.HullAmount <= 1f)
			{
				GameObject.Destroy(this.gameObject);
			}

			return;
		}
	}

	void OnCollisionEnter(Collision collision) 
	{
		Debug.Log("Hit something " + collision.collider.name);

		ShipReference hitShip = null;
		//Debug.Log(collision.collider.name);






		hitShip = collision.collider.GetComponent<ShipReference>();



		if(hitShip != null && hitShip.ParentShip == Attacker)
		{
			return;
		}



		GameObject.Destroy(this.gameObject);
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

		Attacker = attacker;

		//GetComponent<TrailRenderer>().enabled = false;
	}
}
