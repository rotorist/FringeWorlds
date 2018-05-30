using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour 
{
	public ShipBase MyShip;
	public bool IsDocked;
	public bool IsActive { get { return _isActive; } }

	public Faction myFaction;
	public MacroAIParty MyParty;

	//public ShipBase AttackTarget;
	public Rigidbody RB;
	public Whiteboard Whiteboard;
	public AvoidanceDetector AvoidanceDetector;

	public Dictionary<string,BehaviorTree> TreeSet;

	private bool _isActive;
	private bool _isEngineKilled;
	private Vector3 _avoidanceForce;

	
	// Update is called once per frame
	void Update () 
	{

		if(IsActive)
		{
			
			TreeSet["BaseBehavior"].Run();

			if(!IsDocked)
			{
				Turn();

				UpdateSensor();
			}
		}
	}

	void FixedUpdate()
	{
		if(IsActive)
		{
			if(!IsDocked)
			{
				Move();

				UpdateAvoidance();
			}
		}
	}

	// Use this for initialization
	public void Initialize() 
	{
		MyShip = transform.GetComponent<ShipBase>();
		AvoidanceDetector = MyShip.MyReference.AvoidanceDetector;
		AvoidanceDetector.ParentShip = MyShip;

		Whiteboard = new Whiteboard();
		Whiteboard.Initialize();

		//AttackTarget = GameManager.Inst.PlayerControl.PlayerShip;

		//Whiteboard.Parameters["TargetEnemy"] = AttackTarget;
		Whiteboard.Parameters["SpeedLimit"] = -1f;

		TreeSet = new Dictionary<string, BehaviorTree>();
		TreeSet.Add("BaseBehavior", GameManager.Inst.DBManager.XMLParserBT.LoadBehaviorTree("BaseBehavior", this));
		TreeSet.Add("Travel", GameManager.Inst.DBManager.XMLParserBT.LoadBehaviorTree("Travel", this));
		TreeSet.Add("FollowFriendly", GameManager.Inst.DBManager.XMLParserBT.LoadBehaviorTree("FollowFriendly", this));
		TreeSet.Add("FighterCombat", GameManager.Inst.DBManager.XMLParserBT.LoadBehaviorTree("FighterCombat", this));


	}

	public void Activate()
	{
		_isActive = true;
		Collider collider = MyShip.ShipModel.GetComponent<Collider>();
		collider.isTrigger = true;
	}

	public void Deactivate()
	{
		_isActive = false;
		Collider collider = MyShip.ShipModel.GetComponent<Collider>();
		collider.isTrigger = true;
	}


	private void Move()
	{
		if(!MyShip.IsInPortal)
		{
			_isEngineKilled = (bool)Whiteboard.Parameters["IsEngineKilled"];
			Vector3 dest = (Vector3)Whiteboard.Parameters["Destination"];
			bool isStopping = false;
			if(dest == Vector3.zero)
			{
				//try to stop
				isStopping = true;
			}
			Vector3 interceptDest = StaticUtility.FirstOrderIntercept(MyShip.transform.position, MyShip.RB.velocity, 0, dest, Vector3.zero);
			Whiteboard.Parameters["InterceptDest"] = interceptDest;
			Vector3 los = interceptDest - transform.position;
			if(isStopping)
			{
				los = RB.velocity * -1;
			}

			float force = 5;
			if((bool)Whiteboard.Parameters["IsThrusting"])
			{
				force = 14;

			}



			if(!_isEngineKilled)
			{
				if(Vector3.Angle(MyShip.transform.forward, los) < 30)
				{
					RB.AddForce(los.normalized * force);
				}
			}

			if(_avoidanceForce != Vector3.zero)
			{
				//Debug.Log(_avoidanceForce);
			}

			if((bool)Whiteboard.Parameters["IgnoreAvoidance"] == false)
			{
				RB.AddForce(_avoidanceForce);
			}

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

			//adjust max speed bASED on how close is to destination
			maxSpeed *= Mathf.Lerp(0.2f, 1f, Mathf.Clamp01(los.magnitude / 10));


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
				RB.AddForce(-1 * driftVelocity.normalized * driftVelocity.magnitude * 0.5f);

			}
		}
		else
		{
			RB.velocity = Vector3.zero;
		}

	}



	private void Turn()
	{
		if(MyShip.IsInPortal)
		{
			MyShip.RB.angularVelocity = Vector3.zero;
			return;
		}

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
		else
		{
			aimPoint = (Vector3)Whiteboard.Parameters["InterceptDest"];
		}

		Whiteboard.Parameters["AimPoint"] = aimPoint;

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
		if(aimPoint != Vector3.zero)
		{
			foreach(WeaponJoint joint in MyShip.MyReference.WeaponJoints)
			{
				joint.TargetPos = aimPoint;
			}
		}

		/*
		Fighter fighter = (Fighter)MyShip;
		fighter.LeftGun.transform.rotation = Quaternion.LookRotation(aimDir);
		fighter.RightGun.transform.rotation = Quaternion.LookRotation(aimDir);
		*/
	}

	private void AddLookTorque(Vector3 direction)
	{
		float angle = Vector3.Angle(direction, transform.forward);
		Vector3 cross = Vector3.Cross(transform.forward, direction).normalized;
		RB.AddTorque(cross * angle * 1f);
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

	private void UpdateSensor()
	{
		if(MyShip.Scanner != null)
		{
			//check if current target enemy is still valid or within range
			ShipBase currentTarget = (ShipBase)Whiteboard.Parameters["TargetEnemy"];
			if(currentTarget != null && Vector3.Distance(MyShip.transform.position, currentTarget.transform.position) < MyShip.Scanner.Range)
			{
				return;
			}
			else
			{
				Whiteboard.Parameters["TargetEnemy"] = null;
			}

			foreach(ShipBase ship in GameManager.Inst.NPCManager.AllShips)
			{
				if(Vector3.Distance(MyShip.transform.position, ship.transform.position) < MyShip.Scanner.Range)
				{
					Faction otherFaction;
					if(ship == GameManager.Inst.PlayerControl.PlayerShip)
					{
						otherFaction = GameManager.Inst.NPCManager.AllFactions["player"];
					}
					else
					{
						otherFaction = ship.GetComponent<AI>().myFaction;
					}

					float relationship = GameManager.Inst.NPCManager.GetFactionRelationship(myFaction, otherFaction);
					if(relationship < 0.4f)
					{
						Whiteboard.Parameters["TargetEnemy"] = ship;
						break;
					}
				}
			}
		}
	}
}
