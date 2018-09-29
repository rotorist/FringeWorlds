using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : Ammunition 
{
	public float TTL;

	public ShipBase Target;
	public float MaxSpeed;
	public Transform EngineFlameHolder;
	public ParticleSystem Smoke;

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
			if(_age > 0.3f)
			{
				_stage = MissileStage.Chasing;
				LoadEngineFlame();
			}
		}
		else if(_stage == MissileStage.Chasing)
		{
			if(Target == null)
			{
				_force = 5f;
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
					_force = 5f;
					//here we need to reduce torque so it stops turning
				}
				else
				{
					//AddLookTorque(los);
					Vector3 lookDir = Vector3.RotateTowards(transform.forward, los, 2f * Time.fixedDeltaTime, 0f);
					transform.rotation = Quaternion.LookRotation(lookDir);
					_force = 3f;
				}

				_rigidbody.AddForce(transform.forward * _force);
			}



			Vector3 driftVelocity = _rigidbody.velocity - Vector3.Dot(_rigidbody.velocity, transform.forward) * transform.forward;
			_rigidbody.AddForce(-1 * driftVelocity.normalized * driftVelocity.magnitude * 2f);
		}

		//keep under max speed
		if(_rigidbody.velocity.magnitude > MaxSpeed)
		{

			_rigidbody.AddForce(-1 * _rigidbody.velocity * 1);


		}

		if(_stage == MissileStage.Launched)
		{
			_rigidbody.AddForce(-1 * _rigidbody.velocity * 3.5f);
		}


		_age += Time.fixedDeltaTime;

		if(_age > TTL)
		{
			GameObject.Destroy(this.gameObject);
		}

	}

	public void Initialize(ShipBase target)
	{
		_stage = MissileStage.None;
		Target = target;
		MaxSpeed = 30;
		Damage = new Damage();
		Damage.DamageType = DamageType.Shock;
		Damage.ShieldAmount = 5;
		Damage.HullAmount = 80;
		_rigidbody = transform.GetComponent<Rigidbody>();
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
				Explode();
				GameObject.Destroy(gameObject);
			}
		}
		else
		{
			Explode();
			GameObject.Destroy(gameObject);
		}
	}


	private void LoadEngineFlame()
	{
		GameObject flame = GameObject.Instantiate(Resources.Load("EngineFlameMissile")) as GameObject;
		flame.transform.parent = EngineFlameHolder;
		flame.transform.localPosition = Vector3.zero;
		flame.transform.localScale = new Vector3(1, 1, 1);
		flame.transform.localEulerAngles = Vector3.zero;

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
	}

}

public enum MissileStage
{
	None,
	Launched,
	Chasing,
}