using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour 
{
	public ShipBase TargetShip;
	public ShipBase MyShip;
	public bool IsDocked;
	public bool IsActive { get { return _isActive; } }

	public Faction MyFaction;
	public MacroAIParty MyParty;
	public int MyPartyNumber;

	public MaxStack<string> RunningNodeHist;

	//public ShipBase AttackTarget;
	public Rigidbody RB;
	public Whiteboard Whiteboard;
	public AvoidanceDetector AvoidanceDetector;
	public AIWeaponControl WeaponControl;

	public float AimSkill;//0 to 1
	public bool IsEngineKilled;
	public bool IsThrusting;

	public Dictionary<string,BehaviorTree> TreeSet;

	private bool _isActive;
	private bool _isEngineKilled;
	private Vector3 _avoidanceForce;
	private float _targetLockTimer;
	private Vector3 _aimError;

	
	// Update is called once per frame
	public virtual void Update () 
	{
		if(IsActive)
		{
			
			TreeSet["BaseBehavior"].Run();

			if(!IsDocked)
			{
				Turn();
				UpdateAimError();
				UpdateSensor();
				WeaponControl.PerFrameUpdate();
				if(Whiteboard.Parameters["TargetEnemy"] != null)
				{
					AvoidanceDetector.State = AvoidanceState.Combat;
				}
				else
				{
					AvoidanceDetector.State = AvoidanceState.Travel;
				}

			}
		}
	}

	public virtual void FixedUpdate()
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
	public virtual void Initialize(MacroAIParty party, Faction faction) 
	{
		RunningNodeHist = new MaxStack<string>(20);

		AimSkill = 0.8f;
		MyShip = transform.GetComponent<ShipBase>();
		AvoidanceDetector = MyShip.MyReference.AvoidanceDetector;
		AvoidanceDetector.ParentShip = MyShip;

		Whiteboard = new Whiteboard();
		Whiteboard.Initialize();

		//AttackTarget = GameManager.Inst.PlayerControl.PlayerShip;

		//Whiteboard.Parameters["TargetEnemy"] = AttackTarget;
		Whiteboard.Parameters["SpeedLimit"] = -1f;

		MyParty = party;
		MyFaction = faction;
		MyPartyNumber = MyParty.PartyNumber;

		WeaponControl = new AIWeaponControl();
		WeaponControl.Initialize(this);

		TreeSet = new Dictionary<string, BehaviorTree>();
		TreeSet.Add("BaseBehavior", GameManager.Inst.DBManager.XMLParserBT.LoadBehaviorTree("BaseBehavior", this, party));
		TreeSet.Add("Travel", GameManager.Inst.DBManager.XMLParserBT.LoadBehaviorTree("Travel", this, party));
		TreeSet.Add("FollowFriendly", GameManager.Inst.DBManager.XMLParserBT.LoadBehaviorTree("FollowFriendly", this, party));
		TreeSet.Add("FighterCombat", GameManager.Inst.DBManager.XMLParserBT.LoadBehaviorTree("FighterCombat", this, party));
		TreeSet.Add("BigShipCombat", GameManager.Inst.DBManager.XMLParserBT.LoadBehaviorTree("BigShipCombat", this, party));

	}

	public virtual void OnTravelCompletion()
	{

	}

	public void Activate()
	{
		_isActive = true;
		Collider collider = MyShip.ShipModel.GetComponent<Collider>();
		collider.isTrigger = false;
	}

	public void Deactivate()
	{
		_isActive = false;
		Collider collider = MyShip.ShipModel.GetComponent<Collider>();
		collider.isTrigger = true;
	}





	protected void Move()
	{
		if(!MyShip.IsInPortal)
		{
			_isEngineKilled = (bool)Whiteboard.Parameters["IsEngineKilled"];
			Vector3 dest = (Vector3)Whiteboard.Parameters["Destination"];

			IsEngineKilled = _isEngineKilled;
			IsThrusting = (bool)Whiteboard.Parameters["IsThrusting"];

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
				los = RB.velocity * -1f;
			}
			else if(los.magnitude < 5f && RB.velocity.magnitude > 0.1f)
			{
				
				los = RB.velocity * -1f;
			}

			float force = 5;
			if(IsThrusting)
			{
				force = 14;

			}

			//adjust force based on how close is to destination
			force *= Mathf.Lerp(0.4f, 1f, Mathf.Clamp01(los.magnitude / 10));


			if(!_isEngineKilled || IsThrusting)
			{
				if(Vector3.Angle(MyShip.transform.forward, los) < 30 || los.magnitude < 10f || isStopping)
				{
					RB.AddForce(los.normalized * force);
				}
			}

			if(IsThrusting)
			{
				if(MyShip.MyReference.ExhaustController != null)
				{
					MyShip.MyReference.ExhaustController.setExhaustState(ExhaustState.Thruster);
				}
			}
			else
			{
				if(MyShip.MyReference.ExhaustController != null)
				{
					if(_isEngineKilled)
					{
						MyShip.MyReference.ExhaustController.setExhaustState(ExhaustState.Idle);
					}
					else
					{
						MyShip.MyReference.ExhaustController.setExhaustState(ExhaustState.Normal);
					}
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



	protected void Turn()
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
			
			if(RB.velocity.magnitude > 3f)
			{
				aimPoint = (Vector3)Whiteboard.Parameters["InterceptDest"];
			}
		}


		Whiteboard.Parameters["AimPoint"] = aimPoint;
		//Debug.Log("aimpoint " + aimPoint + " " + MyShip.name + " dest " + dest);
		Vector3 distToDest = dest - MyShip.transform.position;

		if(aimPoint != Vector3.zero)
		{
			//Quaternion rotation = Quaternion.LookRotation(aimPoint - MyShip.transform.position);
			//transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * turnRate);
			//Debug.Log("AI Turn state 1 " + MyShip.name);
			AddLookTorque(aimPoint - MyShip.transform.position);

			if(Vector3.Angle(aimPoint - MyShip.transform.position, MyShip.transform.forward) < 20)
			{
				aimDir = aimPoint - MyShip.transform.position;
			}
		}
		else if(dest != Vector3.zero && distToDest.magnitude > 5)
		{
			//Quaternion rotation = Quaternion.LookRotation(dest - MyShip.transform.position);
			//transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * turnRate);
			//Debug.Log("AI Turn state 2 " + MyShip.name);
			AddLookTorque(distToDest);
		}
		else
		{
			Vector3 velocity = RB.velocity;
			if(RB.velocity.magnitude > 1)
			{
				//Quaternion rotation = Quaternion.LookRotation(velocity);
				//transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 2f);
				//Debug.Log("AI Turn state 3 " + MyShip.name);
				AddLookTorque(velocity);
			}
			else
			{
				//Debug.Log("AI Turn state 4 " + MyShip.name);
				AddLookTorque(MyParty.SpawnedShipsLeader.transform.forward);
			}
		}

		//aim guns at target
		if(aimPoint != Vector3.zero)
		{
			foreach(WeaponJoint joint in MyShip.MyReference.WeaponJoints)
			{
				joint.TargetPos = aimPoint + _aimError;
			}
		}

		/*
		Fighter fighter = (Fighter)MyShip;
		fighter.LeftGun.transform.rotation = Quaternion.LookRotation(aimDir);
		fighter.RightGun.transform.rotation = Quaternion.LookRotation(aimDir);
		*/
	}

	protected void AddLookTorque(Vector3 direction)
	{
		float angle = Vector3.Angle(direction, transform.forward);
		Vector3 cross = Vector3.Cross(transform.forward, direction).normalized;
		RB.AddTorque(cross * angle * 0.2f);
		//get the angle between transform.right and direction projected on plane with up normal
		//Vector3 proj = Vector3.ProjectOnPlane(direction, transform.up);
		float horizontalAngle = Vector3.Angle(transform.right, Vector3.up);
		RB.AddTorque(transform.forward * (horizontalAngle - 90) * 0.02f);
	}

	protected void UpdateAvoidance()
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

		//check if any ships are too close to me
		float thres = 5f;
		float magnitude = 6f;
		if(AvoidanceDetector.State == AvoidanceState.Combat)
		{
			thres = 10f;
			magnitude = 15f;
		}

		foreach(ShipBase ship in GameManager.Inst.NPCManager.AllShips)
		{
			if(ship == GameManager.Inst.PlayerControl.PlayerShip || ship.MyAI.IsActive)
			{
				Vector3 dist = ship.transform.position - MyShip.transform.position;

				if(dist.magnitude < thres)
				{
					_avoidanceForce += Vector3.Lerp(dist.normalized * -1 * magnitude, Vector3.zero, (dist.magnitude / thres));
				}
			}
		}
		//Debug.DrawRay(MyShip.transform.position, _avoidanceForce, Color.green);
	}

	protected void UpdateSensor()
	{
		if(Whiteboard.Parameters["TargetEnemy"] != null)
		{
			TargetShip = (ShipBase)Whiteboard.Parameters["TargetEnemy"];
		}
		else
		{
			TargetShip = null;
		}

		if(MyShip.Scanner != null)
		{
			//check if current target enemy is still valid or within range
			ShipBase currentTarget = (ShipBase)Whiteboard.Parameters["TargetEnemy"];
			if(currentTarget != null && Vector3.Distance(MyShip.transform.position, currentTarget.transform.position) < MyShip.Scanner.Range)
			{
				if(_targetLockTimer <= 0)
				{
					_targetLockTimer = UnityEngine.Random.Range(3f, 8f);
					Whiteboard.Parameters["TargetEnemy"] = null;
				}
				else
				{
					return;
				}
			}
			else
			{
				Whiteboard.Parameters["TargetEnemy"] = null;
			}

			float closestDist = MyShip.Scanner.Range;
			ShipBase closestTarget = null;
			foreach(ShipBase ship in GameManager.Inst.NPCManager.AllShips)
			{
				if(ship.DockedStationID != "")
				{
					continue;
				}

				float dist = Vector3.Distance(MyShip.transform.position, ship.transform.position);
				if(dist < closestDist)
				{
					Faction otherFaction;
					if(ship == GameManager.Inst.PlayerControl.PlayerShip)
					{
						otherFaction = GameManager.Inst.NPCManager.AllFactions["player"];
					}
					else
					{
						otherFaction = ship.GetComponent<AI>().MyFaction;
					}

					float relationship = GameManager.Inst.NPCManager.GetFactionRelationship(MyFaction, otherFaction);
					//Debug.Log("relationship with " + otherFaction.DisplayName + " " + relationship);
					if(relationship < 0.4f)
					{
						closestTarget = ship;
						closestDist = dist;
					}
				}
			}

			Whiteboard.Parameters["TargetEnemy"] = closestTarget;
		}

		//if still no target enemy check if anyone else on the party has it then take that
		if(Whiteboard.Parameters["TargetEnemy"] == null)
		{
			foreach(ShipBase ship in MyParty.SpawnedShips)
			{
				if(ship.MyAI.Whiteboard.Parameters["TargetEnemy"] != null)
				{
					ShipBase targetShip = (ShipBase)ship.MyAI.Whiteboard.Parameters["TargetEnemy"];
					if(Vector3.Distance(ship.transform.position, targetShip.transform.position) < ship.Scanner.Range)
					{
						Whiteboard.Parameters["TargetEnemy"] = ship.MyAI.Whiteboard.Parameters["TargetEnemy"];
					}
				}
			}
		}

		if(Whiteboard.Parameters["TargetEnemy"] == null)
		{
			Whiteboard.Parameters["AimTarget"] = null;
		}

		_targetLockTimer = Mathf.Clamp(_targetLockTimer - Time.deltaTime, 0, 60);
	}

	private void UpdateAimError()
	{
		//calculate error magnitude based on target horizontal speed and aiming skill
		ShipBase currentTarget = (ShipBase)Whiteboard.Parameters["TargetEnemy"];
		if(currentTarget == null)
		{
			return;
		}

		float horizontalSpeed = Mathf.Abs(Vector3.Dot(currentTarget.RB.velocity, MyShip.transform.right));
		float factor = (Mathf.Clamp01(horizontalSpeed / 8)) * 0.4f + (1 - AimSkill) * 0.6f;
		float errorMagnitude = 40 * factor;

		if(_aimError.magnitude < 0.5f)
		{
			_aimError = errorMagnitude * UnityEngine.Random.insideUnitSphere;
		}

		_aimError = Vector3.Lerp(_aimError, Vector3.zero, AimSkill * 6f * Time.deltaTime);
		//Debug.Log(_aimError);
	}
}
