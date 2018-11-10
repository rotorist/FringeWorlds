using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Missile : Ammunition 
{
	public float TTL;

	public ShipBase Target;
	public float MaxSpeed;
	public float Acceleration;
	public Transform EngineFlameHolder;
	public ParticleSystem EngineFlameParticles;
	public GameObject EngineFlame;
	public float TurnRate;
	public float LaunchedStageDuration;
	public float BlastRadius;
	public float DetonationRadius;

	public MissileStage Stage { get { return _stage; } }

	private MissileStage _stage;
	private Rigidbody _rigidbody;
	private Vector3 _initialVelocity;
	private float _force;
	private float _age;


	void FixedUpdate () 
	{
		if(_stage == MissileStage.Launched)
		{
			//if(_age > 0.3f)
			if(_age > LaunchedStageDuration)
			{
				_stage = MissileStage.Chasing;
				LoadEngineFlame();
				Collider collider = transform.GetComponent<Collider>();
				if(collider != null)
				{
					collider.enabled = true;
				}
				AudioSource audio = GetComponent<AudioSource>();
				if(audio != null)
				{
					audio.Play();
				}
			}
		}
		else if(_stage == MissileStage.Chasing)
		{
			if(Target == null)
			{
				_force = Acceleration;//5f;
				_rigidbody.AddForce(transform.forward * _force);
			}
			else
			{
				Vector3 targetPos = Target.transform.position;
				if(Target.CurrentCountermeasure != null)
				{
					CounterMeasureFlares flares = Target.CurrentCountermeasure.GetComponentInChildren<CounterMeasureFlares>();
					if(flares != null && flares.IsEffective)
					{
						targetPos += flares.MissileDeviation;
					}
				}

				Vector3 aimPoint = StaticUtility.FirstOrderIntercept(transform.position, _rigidbody.velocity,
					_rigidbody.velocity.magnitude, targetPos, Target.RB.velocity);
				Vector3 los = aimPoint - transform.position;

				if(Vector3.Angle(los, transform.forward) < 10)
				{
					_force = Acceleration;//5f
					//here we need to reduce torque so it stops turning
				}
				else
				{
					//AddLookTorque(los);
					Vector3 lookDir = Vector3.RotateTowards(transform.forward, los, 1f * Time.fixedDeltaTime, 0f);
					transform.rotation = Quaternion.LookRotation(lookDir);
					_force = Acceleration * 0.6f;//3f;
				}

				_rigidbody.AddForce(transform.forward * _force);
			}



			Vector3 driftVelocity = _rigidbody.velocity - Vector3.Dot(_rigidbody.velocity, transform.forward) * transform.forward;
			_rigidbody.AddForce(-1 * driftVelocity.normalized * driftVelocity.magnitude * TurnRate);
		}

		//keep under max speed
		if(_rigidbody.velocity.magnitude > MaxSpeed)
		{

			_rigidbody.AddForce(-1 * _rigidbody.velocity * 1);


		}

		if(_stage == MissileStage.Launched)
		{
			_rigidbody.AddForce(-1 * _rigidbody.velocity * 2.5f);
		}


		_age += Time.fixedDeltaTime;

		if(_age > TTL)
		{
			Explode();
		}

	}

	public void Initialize(ShipBase target, Item missileItem)
	{
		_stage = MissileStage.None;
		Target = target;
		MaxSpeed = missileItem.GetFloatAttribute("MaxSpeed");
		Damage = new Damage();
		Damage.DamageType = (DamageType)Enum.Parse(typeof(DamageType), missileItem.GetStringAttribute("DamageType"));
		Damage.ShieldAmount = missileItem.GetFloatAttribute("ShieldDamage");
		Damage.HullAmount = missileItem.GetFloatAttribute("HullDamage");
		BlastRadius = missileItem.GetFloatAttribute("BlastRadius");
		DetonationRadius = missileItem.GetFloatAttribute("DetonationRadius");
		TurnRate = missileItem.GetFloatAttribute("TurnRate");
		Acceleration = missileItem.GetFloatAttribute("Acceleration");
		LaunchedStageDuration = missileItem.GetFloatAttribute("IgnitionDelay");
		_rigidbody = transform.GetComponent<Rigidbody>();

		Collider collider = transform.GetComponent<Collider>();
		if(collider != null)
		{
			collider.enabled = false;
		}
	}

	public void Fire(ShipBase attacker, Vector3 initialVelocity)
	{
		_stage = MissileStage.Launched;
		_initialVelocity = initialVelocity;
		_rigidbody.velocity = _initialVelocity;
	}
	/*
	public override void OnTriggerEnter(Collider other)
	{
		//Explode();
		//base.OnTriggerEnter(other);
		//GameObject.Destroy(gameObject);
	}
	*/

	public override void OnCollisionEnter(Collision collision) 
	{
		Debug.Log(collision.collider.name);
		ShipReference hitShip = collision.collider.GetComponent<ShipReference>();

		if(hitShip != null)
		{
			if(hitShip.ParentShip != Attacker)
			{
				//Debug.Log("Sending damage " + Damage.ShieldAmount + " to " + hitShip.ParentShip.name);
				Damage.HitLocation = collision.contacts[0].point;
				Damage = hitShip.ParentShip.Shield.ProcessDamage(Damage);

				if(Damage.HullAmount <= 1f)
				{
					Explode();
					return;
				}
				//Debug.Log("Hull amount " + Damage.HullAmount);
				hitShip.ParentShip.ProcessHullDamage(Damage, Attacker);
				Explode();
			}
		}
		else
		{
			Explode();
		}
	}


	private void LoadEngineFlame()
	{
		GameObject flame = GameObject.Instantiate(Resources.Load("EngineFlameMissile")) as GameObject;
		flame.transform.parent = EngineFlameHolder;
		flame.transform.localPosition = Vector3.zero;
		flame.transform.localScale = new Vector3(1, 1, 1);
		flame.transform.localEulerAngles = Vector3.zero;
		EngineFlameParticles = flame.GetComponent<ParticleSystem>();
		EngineFlame = flame.transform.Find("EngineFlames4").gameObject;

	}

	private void AddLookTorque(Vector3 direction)
	{
		float angle = Vector3.Angle(direction, transform.forward);
		Vector3 cross = Vector3.Cross(transform.forward, direction).normalized;
		_rigidbody.AddTorque(cross * angle * 0.1f);

	}



	private void Explode()
	{
		GameObject explosion = GameObject.Instantiate(Resources.Load("Explosion" + UnityEngine.Random.Range(1, 5).ToString())) as GameObject;
		explosion.transform.position = this.transform.position;
		Destroy();
	}

	private void Destroy()
	{
		EngineFlameParticles.transform.parent = null;
		EngineFlameParticles.Stop();
		EngineFlame.GetComponent<MeshRenderer>().enabled = false;
		EngineFlameParticles.GetComponent<FXSelfDestruct>().IsTTLEnabled = true;
		GameObject.Destroy(gameObject);
	}

}

public enum MissileStage
{
	None,
	Launched,
	Chasing,
}