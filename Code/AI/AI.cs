using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour 
{
	public ShipBase MyShip;
	
	public ShipBase AttackTarget;
	public Rigidbody RB;
	public Whiteboard Whiteboard;
	public AvoidanceDetector AvoidanceDetector;

	public Dictionary<string,BehaviorTree> TreeSet;


	private bool _isEngineKilled;
	private Vector3 _avoidanceForce;

	
	// Update is called once per frame
	void Update () 
	{
		TreeSet["FighterCombat"].Run();

		Turn();
	}

	void FixedUpdate()
	{
		Move();
		UpdateAvoidance();
	}

	// Use this for initialization
	public void Initialize() 
	{
		Whiteboard = new Whiteboard();
		Whiteboard.Initialize();

		Whiteboard.Parameters["TargetEnemy"] = AttackTarget;
		Whiteboard.Parameters["SpeedLimit"] = -1f;

		TreeSet = new Dictionary<string, BehaviorTree>();
		TreeSet.Add("FighterCombat", GameManager.Inst.DBManager.XMLParserBT.LoadBehaviorTree("FighterCombat", this));


	}


	private void Move()
	{
		_isEngineKilled = (bool)Whiteboard.Parameters["IsEngineKilled"];
		Vector3 dest = (Vector3)Whiteboard.Parameters["Destination"];
		if(dest == Vector3.zero)
		{
			return;
		}

		Vector3 los = dest - transform.position;

		float force = 5;
		if((bool)Whiteboard.Parameters["IsThrusting"])
		{
			force = 14;

		}

		if(!_isEngineKilled)
		{
			RB.AddForce(los.normalized * force);

		}

		if(_avoidanceForce != Vector3.zero)
		{
			Debug.Log(_avoidanceForce);
		}
		RB.AddForce(_avoidanceForce);

		float strafeForce = (float)Whiteboard.Parameters["StrafeForce"];
		if(strafeForce != 0)
		{
			RB.AddForce(MyShip.transform.right * strafeForce);
		}

		//drag
		Vector3 velocity = RB.velocity;
		float maxSpeed = MyShip.Engine.MaxSpeed;
		//Debug.Log(velocity.magnitude);
		float speedLimit = (float)Whiteboard.Parameters["SpeedLimit"];
		if(speedLimit >= 0 && speedLimit < maxSpeed)
		{
			maxSpeed = speedLimit;
		}


		if(velocity.magnitude > maxSpeed)
		{
			RB.AddForce(-1 * velocity * 1);
		}
		else
		{
			RB.AddForce(-1 * velocity.normalized * 0.01f);
		}

		if(!_isEngineKilled)
		{
			Vector3 driftVelocity = velocity - Vector3.Dot(velocity, transform.forward) * transform.forward;
			RB.AddForce(-1 * driftVelocity.normalized * driftVelocity.magnitude * 0.1f);

		}


	}



	private void Turn()
	{
		Vector3 dest = (Vector3)Whiteboard.Parameters["Destination"];
		ShipBase aimTarget = (ShipBase)Whiteboard.Parameters["AimTarget"];
		Vector3 aimPoint = Vector3.zero;
		Vector3 aimDir = MyShip.transform.forward;
		float turnRate = 1.5f;

		if(aimTarget != null)
		{
			aimPoint = StaticUtility.FirstOrderIntercept(MyShip.transform.position, MyShip.RB.velocity,
															30, aimTarget.transform.position, aimTarget.RB.velocity);
		}
		if(aimPoint != Vector3.zero)
		{
			//Quaternion rotation = Quaternion.LookRotation(aimPoint - MyShip.transform.position);
			//transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * turnRate);
			AddLookTorque(aimPoint - MyShip.transform.position);

			if(Vector3.Angle(aimPoint - MyShip.transform.position, MyShip.transform.forward) < 20)
			{
				aimDir = aimPoint - MyShip.transform.position;
			}
		}
		else if(dest != Vector3.zero)
		{
			//Quaternion rotation = Quaternion.LookRotation(dest - MyShip.transform.position);
			//transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * turnRate);
			AddLookTorque(dest - MyShip.transform.position);
		}
		else
		{
			Vector3 velocity = RB.velocity;
			if(RB.velocity.magnitude > 0)
			{
				//Quaternion rotation = Quaternion.LookRotation(velocity);
				//transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 2f);
				AddLookTorque(velocity);
			}
		}

		//aim guns at target
		Fighter fighter = (Fighter)MyShip;
		fighter.LeftGun.transform.rotation = Quaternion.LookRotation(aimDir);
		fighter.RightGun.transform.rotation = Quaternion.LookRotation(aimDir);
	}

	private void AddLookTorque(Vector3 direction)
	{
		float angle = Vector3.Angle(direction, transform.forward);
		Vector3 cross = Vector3.Cross(transform.forward, direction).normalized;
		RB.AddTorque(cross * angle * 0.06f);
		//get the angle between transform.right and direction projected on plane with up normal
		Vector3 proj = Vector3.ProjectOnPlane(direction, transform.up);
		float horizontalAngle = Vector3.Angle(transform.right, proj);
		RB.AddTorque(transform.forward * (horizontalAngle - 90) * 0.01f);
	}

	private void UpdateAvoidance()
	{
		AvoidanceDetector.AvoidanceUpdate();
		if(AvoidanceDetector.Avoidance != Vector3.zero)
		{
			float cap = 30;
			float maxValue = StaticUtility.GetMaxElementV3(AvoidanceDetector.Avoidance);
			float forceMag = 5f + 10f * Mathf.Clamp01(maxValue / 30);

			Vector3 force1 = (AvoidanceDetector.transform.position - AvoidanceDetector.RaySource1.position).normalized * (AvoidanceDetector.Avoidance.x / maxValue);
			Vector3 force2 = (AvoidanceDetector.transform.position - AvoidanceDetector.RaySource2.position).normalized * (AvoidanceDetector.Avoidance.y / maxValue);
			Vector3 force3 = (AvoidanceDetector.transform.position - AvoidanceDetector.RaySource3.position).normalized * (AvoidanceDetector.Avoidance.z / maxValue);
			Vector3 force4 = Vector3.zero;

			if(AvoidanceDetector.Avoidance.x > cap && AvoidanceDetector.Avoidance.y > cap && AvoidanceDetector.Avoidance.z > cap)
			{
				force4 = (RB.velocity * -1).normalized;
			}

			_avoidanceForce = forceMag * (force1 + force2 + force3 + force4).normalized;
		}
		else
		{
			_avoidanceForce = Vector3.zero;
		}
	}
}
