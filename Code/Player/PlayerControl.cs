using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl
{
	public ShipBase PlayerShip;

	public string SpawnStationID;
	public StationType SpawnStationType;

	public ParticleSystem SpaceDust;
	public ParticleSystem TradelaneDust;

	public Vector3 GimballTarget { get { return _gimballTarget; }}

	public object SelectedObject;
	public SelectedObjectType SelectedObjectType;
	

	public ShipBase TargetShip;

	public TLTransitSession CurrentTradelaneSession;

	public bool IsAutopilot;
	public Autopilot PlayerAutopilot;
	public MacroAIParty PlayerParty;

	public List<WeaponJoint> [] WeaponGroups;

	public float Throttle { get { return _throttle; } }
	public bool IsFAKilled { get { return _isFAKilled; } }
	public bool IsMouseFlight { get { return _isMouseFlight; } }
	public float YawForce { get { return _yawForce; } }
	public float PitchForce { get { return _pitchForce; } }
	public float RollForce { get { return _rollForce; } }
	public float ForwardForce { get { return _forwardForce; } }
	public float ThrusterForce { get { return _thruster; } }

	private Vector2 _mousePosNorm;
	private float _yawForce;
	private float _yawAngleV;

	private float _pitchForce;
	private float _pitchAngleV;

	private float _rollValue;
	private float _rollForce;
	private float _rollAngleV;

	private float _throttle;
	private float _forwardForce;
	private float _thruster;
	private float _strafeHor;
	private float _strafeVer;

	private bool _isMouseFlight;
	private bool _isFAKilled;
	private bool _isDustSettled;

	private Vector3 _gimballTarget;

	private Transform _testSphere;


	private Launcher _testLauncher;


	public void Initialize()
	{
		Debug.Log("Initializing player control");
		GameObject o = GameObject.Find("PlayerShip");

		if(o != null)
		{
			PlayerShip = o.GetComponent<ShipBase>();
		}
		else
		{
			Debug.Log("Can't find player ship");
		}

		o = GameObject.Find("SpaceDust");
		SpaceDust = o.GetComponent<ParticleSystem>();
		_testSphere = GameObject.Find("Sphere").transform;

		o = SpaceDust.transform.Find("TradelaneDust").gameObject;
		TradelaneDust = o.GetComponent<ParticleSystem>();

		_isMouseFlight = true;



		GameManager.Inst.NPCManager.AllShips.Add(PlayerShip);

		WeaponGroups = new List<WeaponJoint>[4];
		WeaponGroups[0] = new List<WeaponJoint>();
		WeaponGroups[1] = new List<WeaponJoint>();
		WeaponGroups[2] = new List<WeaponJoint>();
		WeaponGroups[3] = new List<WeaponJoint>();

		GameEventHandler.OnShipDeath -= OnNPCDeath;
		GameEventHandler.OnShipDeath += OnNPCDeath;

	}

	public void LoadPlayerShip()
	{
		PlayerShip.RB.inertiaTensor = new Vector3(1, 1, 1);

		//use player loadout to spawn player ship here
		GameManager.Inst.NPCManager.SpawnPlayerShip(GameManager.Inst.PlayerProgress.ActiveLoadout, "player", PlayerParty);

		WeaponGroups[0].Add(PlayerShip.MyReference.WeaponJoints[2]);
		WeaponGroups[1].Add(PlayerShip.MyReference.WeaponJoints[0]);
		WeaponGroups[1].Add(PlayerShip.MyReference.WeaponJoints[1]);

		PlayerShip.MyReference.ExhaustController.setExhaustState(ExhaustState.Normal);
		PlayerShip.MyReference.ExhaustController.setExhaustLength(_throttle);

	}

	public void CreatePlayerParty()
	{
		//create MacroAIParty for player
		PlayerParty = GameManager.Inst.NPCManager.MacroAI.GeneratePlayerParty();
		PlayerAutopilot = PlayerShip.GetComponent<Autopilot>();
		PlayerAutopilot.Initialize(PlayerParty, GameManager.Inst.NPCManager.AllFactions["player"]);
	}

	public int GetWeaponGroupNumber(WeaponJoint joint)
	{
		for(int i=0; i<4; i++)
		{
			if(WeaponGroups[i].Contains(joint))
			{
				return i;
			}
		}

		return -1;
	}

	public void PerFrameUpdate()
	{
		if(GameManager.Inst.SceneType == SceneType.SpaceTest)
		{
			return;
		}

		if(!IsAutopilot)
		{
			UpdateMovementKeyInput();
		}
		PlayerAutopilot.APUpdate();
		UpdateCommandKeyInput();
		UpdateMouseInput();
		UpdateWeaponAim();

	}

	public void FixedFrameUpdate()
	{
		if(GameManager.Inst.SceneType == SceneType.SpaceTest)
		{
			return;
		}

		if(IsAutopilot)
		{
			PlayerAutopilot.APFixedUpdate();
		}
		else
		{
			UpdateShipRotation();
			UpdateShipMovement();
		}

		UpdateSpaceDust();
	}

	public void LateFrameUpdate()
	{
		
	}


	public void OnNPCDeath(ShipBase ship)
	{
		if(TargetShip == ship)
		{
			GameManager.Inst.UIManager.HUDPanel.OnClearSelectedObject();
		}
	}

	public void DockComplete(StationBase dockedStation, StationType type)
	{
		GameManager.Inst.SaveGameManager.CreateAnchorSave(dockedStation, type);
		if(type == StationType.Station)
		{
			GameManager.Inst.UIManager.FadePanel.FadeOut(0.4f);
		}
		else if(type == StationType.JumpGate)
		{
			GameManager.Inst.UIManager.FadePanel.WhiteFadeOut(0.75f);
		}
	}

	public void SpawnPlayer()
	{
		Debug.Log(SpawnStationID);
		StationBase station = GameObject.Find(SpawnStationID).GetComponent<StationBase>();
		DockSessionBase session = null;
		station.Undock(PlayerShip, out session);
	}

	public void CancelAutopilot()
	{
		Debug.Log("cancelling autopilot");
		IsAutopilot = false;
		PlayerAutopilot.Deactivate();
		_isMouseFlight = true;
		PlayerParty.PrevNode = null;
		PlayerParty.NextTwoNodes.Clear();
		if(PlayerParty.CurrentTLSession != null)
		{
			PlayerParty.CurrentTLSession.Stage = TLSessionStage.Cancelling;

		}
	}

	public void UpdateCommandKeyInput()
	{
		//select
		if(Input.GetKeyDown(KeyCode.F))
		{
			SelectObject();
		}

		//dock
		if(Input.GetKeyDown(KeyCode.F3) && !PlayerShip.IsInPortal)
		{
			Dock();
		}

		//cancel
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			if(IsAutopilot)
			{
				CancelAutopilot();
			}
			else
			{
				if(CurrentTradelaneSession != null)
				{
					CurrentTradelaneSession.Stage = TLSessionStage.Cancelling;
				}
			}
		}

		//autopilot goto
		if(Input.GetKeyDown(KeyCode.F2))
		{
			AutopilotGoTo();
		}

		//toggle view
		if(Input.GetKeyDown(KeyCode.V))
		{
			GameManager.Inst.CameraController.SetView(!GameManager.Inst.CameraController.IsFirstPerson);
		}
	}

	public void UpdateMovementKeyInput()
	{
		float rollSpeed = 1;
		float rollStopSpeed = 4;

		if(Input.GetKeyDown(KeyCode.Tab))
		{
			_isFAKilled = !_isFAKilled;
		}

		if(Input.GetKeyDown(KeyCode.Space))
		{
			_isMouseFlight = !_isMouseFlight;
		}

		if(Input.GetKeyDown(KeyCode.LeftAlt))
		{
			if(Time.timeScale == 0)
			{
				Time.timeScale = 1;
			}
			else
			{
				Time.timeScale = 0;
			}

		}




		if(!Input.GetKey(KeyCode.LeftShift))
		{
			
			//rolling
			if(Input.GetKey(KeyCode.A))
			{
				if(_rollValue < 0)
				{
					_rollValue = Mathf.Clamp(_rollValue + Time.deltaTime * rollSpeed * (1 + 2 * (_rollValue * -1)), -1, 1);
				}
				else
				{
					_rollValue = Mathf.Clamp(_rollValue + Time.deltaTime * rollSpeed, -1, 1);
				}
			}
			if(Input.GetKey(KeyCode.D))
			{
				if(_rollValue > 0)
				{
					_rollValue = Mathf.Clamp(_rollValue - Time.deltaTime * rollSpeed * (1 + 2 * _rollValue), -1, 1);
				}
				else
				{
					_rollValue = Mathf.Clamp(_rollValue - Time.deltaTime * rollSpeed, -1, 1);
				}
			}




			//thruster
			_strafeHor = 0;
			_strafeVer = 0;

			if(Input.GetKey(KeyCode.W))
			{
				_thruster = 1;
			}

			if(Input.GetKey(KeyCode.S))
			{
				_thruster = -0.2f;
			}


		}

		//strafing
		if(Input.GetKey(KeyCode.LeftShift))
		{

			_thruster = 0;

			if(Input.GetKey(KeyCode.A))
			{
				_strafeHor = -1;
			}
			if(Input.GetKey(KeyCode.D))
			{
				_strafeHor = 1;
			}
			if(Input.GetKey(KeyCode.W))
			{
				_strafeVer = -1;
			}
			if(Input.GetKey(KeyCode.S))
			{
				_strafeVer = 1;
			}

			_rollValue = Mathf.Lerp(_rollValue, 0, Time.deltaTime * rollStopSpeed);
		}

		_rollForce = GameManager.Inst.Constants.RollCurve.Evaluate(_rollValue);


		if(!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
		{
			_thruster = 0;
			_strafeVer = 0;
		}

		if(!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
		{
			_rollValue = Mathf.Lerp(_rollValue, 0, Time.deltaTime * rollStopSpeed);
			_strafeHor = 0;
		}


	}

	private void UpdateMouseInput()
	{
		if(PlayerShip.IsInPortal)
		{
			return;
		}

		//get mouse position
		Vector2 mousePos = Input.mousePosition;
		if(_isMouseFlight)
		{
			_mousePosNorm = new Vector2((mousePos.x / Screen.width) - 0.5f, (mousePos.y / Screen.height) - 0.5f) * 2;
			_yawForce = GameManager.Inst.Constants.MouseYawCurve.Evaluate(_mousePosNorm.x);
			_pitchForce = GameManager.Inst.Constants.MousePitchCurve.Evaluate(_mousePosNorm.y) * -1;
		}
		else
		{
			_yawForce = Mathf.Lerp(_yawForce, 0, Time.deltaTime * 6);
			_pitchForce = Mathf.Lerp(_pitchForce, 0, Time.deltaTime * 6);
		}

		//mouse wheel throttle
		float wheelInput = Input.GetAxis("Mouse ScrollWheel");
		if(wheelInput > 0)
		{
			if(Input.GetKey(KeyCode.LeftControl))
			{
				
			}
			else
			{
				
				_throttle = Mathf.Clamp01(_throttle + 0.1f);
				PlayerShip.MyReference.ExhaustController.setExhaustLength(_throttle);
			}
		}
		else if(wheelInput < 0)
		{
			if(Input.GetKey(KeyCode.LeftControl))
			{
				
			}
			else
			{
				_throttle = Mathf.Clamp01(_throttle - 0.1f);
				PlayerShip.MyReference.ExhaustController.setExhaustLength(_throttle);
			}
		}



		//firing weapon

		if(Input.GetMouseButton(0))
		{
			foreach(WeaponJoint joint in WeaponGroups[0])
			{
				if(joint.MountedWeapon != null && joint.ControlMode == TurretControlMode.Manual)
				{
					joint.MountedWeapon.Fire();
				}
			}
				
		}

		if(Input.GetMouseButton(1))
		{
			foreach(WeaponJoint joint in WeaponGroups[1])
			{
				if(joint.MountedWeapon != null && joint.ControlMode == TurretControlMode.Manual)
				{
					joint.MountedWeapon.Fire();
				}
			}
		}

	}

	private void UpdateShipRotation()
	{
		if(PlayerShip.IsInPortal)
		{
			PlayerShip.RB.angularVelocity = Vector3.zero;
			_yawForce = 0;
			_pitchForce = 0;
			return;
		}

		Vector3 angularVelocity = PlayerShip.transform.InverseTransformDirection(PlayerShip.RB.angularVelocity);
		float engineKillBonus = _isFAKilled ? 2f : 1f;

		//Yaw
		float maxYawRate = 1.2f;
		if(Mathf.Abs(angularVelocity.y) < maxYawRate * engineKillBonus)
		{
			PlayerShip.RB.AddTorque(PlayerShip.transform.up * _yawForce * PlayerShip.TorqueModifier, ForceMode.Acceleration);
		}

		//Pitch
		float maxPitchRate = 2f;
		if(Mathf.Abs(angularVelocity.x) < maxPitchRate * engineKillBonus)
		{
			PlayerShip.RB.AddTorque(PlayerShip.transform.right * _pitchForce * PlayerShip.TorqueModifier);
		}

		//Roll is based on key press A and D and it lerps to 0
		float maxRollRate = 2f;
		if(Mathf.Abs(angularVelocity.z) < maxRollRate)
		{
			if(Mathf.Abs(_rollForce) > 0.02f)
			{
				PlayerShip.RB.AddTorque(PlayerShip.transform.forward * _rollForce * PlayerShip.TorqueModifier);
			}
			else
			{
				PlayerShip.RB.AddTorque(PlayerShip.transform.forward * angularVelocity.z * -0.5f * PlayerShip.TorqueModifier);
			}
		}
		else
		{
			PlayerShip.RB.AddTorque(PlayerShip.transform.forward * angularVelocity.z * -0.5f * PlayerShip.TorqueModifier);
		}

	}

	private void UpdateShipMovement()
	{
		



		Vector3 velocity = PlayerShip.RB.velocity;

		//if is in portal just stop the ship
		if(!PlayerShip.IsInPortal)
		{
			//main engine
			_forwardForce = _throttle * 1;
			float maxSpeed = PlayerShip.Engine.MaxSpeed;

			//PlayerShip.MyReference.ExhaustController.setExhaustState(ExhaustState.Thruster);

			if(_thruster != 0 || _isFAKilled)
			{
				maxSpeed = maxSpeed * 1.5f;
			}

			if(!_isFAKilled)
			{
				
				maxSpeed = maxSpeed * _throttle;
			}

			if(_thruster > 0)
			{
				PlayerShip.MyReference.ExhaustController.setExhaustState(ExhaustState.Thruster);
			}
			else
			{
				if(_isFAKilled)
				{
					PlayerShip.MyReference.ExhaustController.setExhaustState(ExhaustState.Idle);
				}
				else
				{
					PlayerShip.MyReference.ExhaustController.setExhaustState(ExhaustState.Normal);
				}
			}

			if(velocity.magnitude < maxSpeed && !_isFAKilled)
			{
				PlayerShip.RB.AddForce(PlayerShip.transform.forward * _forwardForce);
			}

			//thruster
			Thruster thruster = PlayerShip.Thruster;
			if(thruster != null)
			{
				if(thruster.CurrentFuel > 0)
				{
					PlayerShip.RB.AddForce(PlayerShip.transform.forward * _thruster * 10);
				}

				//strafe
				if(thruster.CanStrafe && thruster.CurrentFuel > 0)
				{
					PlayerShip.RB.AddForce(PlayerShip.transform.right * _strafeHor * 3);
					PlayerShip.RB.AddForce(PlayerShip.transform.up * _strafeVer * -3);
				}

				if(_thruster > 0 || (thruster.CanStrafe && (_strafeHor > 0 || _strafeVer > 0)))
				{
					thruster.CurrentFuel = Mathf.Clamp(thruster.CurrentFuel - thruster.ConsumptionRate * Time.fixedDeltaTime, 0, thruster.MaxFuel);
				}
				else
				{
					thruster.CurrentFuel = Mathf.Clamp(thruster.CurrentFuel + thruster.RestoreRate * Time.fixedDeltaTime, 0, thruster.MaxFuel);
				}
				
			}

			//drag

			//Debug.Log(velocity.magnitude);
			if(velocity.magnitude > maxSpeed)
			{
				PlayerShip.RB.AddForce(-1 * velocity * 1);
			}
			else
			{
				PlayerShip.RB.AddForce(-1 * velocity * 0.01f);
			}

			//flight assist
			if(!_isFAKilled || _thruster != 0)
			{
				Vector3 driftVelocity = velocity - Vector3.Dot(velocity, PlayerShip.transform.forward) * PlayerShip.transform.forward;
				PlayerShip.RB.AddForce(-1 * driftVelocity.normalized * driftVelocity.magnitude * 1f);
			}

		}
		else
		{
			PlayerShip.RB.velocity = Vector3.zero;
		}





	}

	private void UpdateSpaceDust()
	{
		Vector3 velocity = PlayerShip.RB.velocity;

		//space dust
		//Debug.DrawRay(PlayerShip.transform.position, velocity);
		if(PlayerShip.IsInPortal)
		{
			//disable dust
			ParticleSystem.EmissionModule sEmission = SpaceDust.emission;
			sEmission.enabled = false;
			//show tradelane dust according to InPortalSpeed
			if(PlayerShip.InPortalSpeed > 20)
			{
				ParticleSystem.EmissionModule tlEmission = TradelaneDust.emission;
				tlEmission.enabled = true;

				Quaternion rotation = Quaternion.LookRotation(PlayerShip.transform.forward);
				SpaceDust.transform.rotation = rotation;
				SpaceDust.transform.position = PlayerShip.transform.position + SpaceDust.transform.forward * 20f;
			}
			else
			{
				ParticleSystem.EmissionModule tlEmission = TradelaneDust.emission;
				tlEmission.enabled = false;
			}
		}
		else
		{
			//disable tradelane dust and show space dust
			ParticleSystem.EmissionModule tlEmission = TradelaneDust.emission;
			tlEmission.enabled = false;

			if(velocity.magnitude > 1f)
			{
				Quaternion rotation = Quaternion.LookRotation(velocity);
				SpaceDust.transform.rotation = rotation;
				ParticleSystem.MainModule newMain = SpaceDust.main;
				newMain.startSpeed = velocity.magnitude * 8 * -1;
				SpaceDust.transform.position = PlayerShip.transform.position - SpaceDust.transform.forward * 20f;

				ParticleSystem.EmissionModule emission = SpaceDust.emission;
				emission.enabled = true;

				_isDustSettled = false;
			}
			else
			{
				if(!_isDustSettled)
				{
					SpaceDust.transform.position = PlayerShip.transform.position - SpaceDust.transform.forward * 20f;
					ParticleSystem.EmissionModule emission = SpaceDust.emission;
					emission.enabled = false;
					_isDustSettled = true;
				}
			}
		}
	}

	private void UpdateWeaponAim()
	{
		

		Camera camera = Camera.main;
		Vector2 mousePos = new Vector2();

		mousePos = Input.mousePosition;
		float targetDist = 100;
		if(TargetShip != null)
		{
			targetDist = Mathf.Clamp(Vector3.Distance(camera.transform.position, TargetShip.transform.position), 30, 100);
		}
		Vector3 targetPos = camera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, targetDist));

		foreach(WeaponJoint joint in PlayerShip.MyReference.WeaponJoints)
		{
			joint.TargetPos = targetPos;
		}

		/*
		Fighter fighter = (Fighter)PlayerShip;

		Vector3 lookDirLeft = targetPos - fighter.LeftGun.position;
		Vector3 verticalLoSLeft = lookDirLeft - (fighter.LeftGunJoint.forward * 100);
		float angleLeft = Vector3.Angle(lookDirLeft, fighter.LeftGunJoint.forward);
		if(angleLeft > gimbalLimit)
		{
			verticalLoSLeft = verticalLoSLeft.normalized * (Mathf.Tan(Mathf.Deg2Rad * gimbalLimit) * 100);

			Vector3 newTarget = fighter.LeftGunJoint.position + fighter.LeftGunJoint.forward * 100 + verticalLoSLeft;
			lookDirLeft = newTarget - fighter.LeftGun.position;
			_gimballTarget = newTarget;
			//_testSphere.transform.position = newTarget;
		}
		else
		{
			_gimballTarget = Vector3.zero;
		}

		//Debug.Log(Vector3.Distance(targetPos, fighter.LeftGunJoint.position) + ", " + Vector3.Angle(lookDirLeft, fighter.LeftGunJoint.forward));
		Quaternion rotationLeft = Quaternion.LookRotation(lookDirLeft, fighter.LeftGunJoint.up);
		fighter.LeftGun.rotation = Quaternion.Lerp(fighter.LeftGun.rotation, rotationLeft, Time.deltaTime * 9);



		Vector3 lookDirRight = targetPos - fighter.RightGun.position;
		Vector3 verticalLoSRight = lookDirRight - (fighter.RightGunJoint.forward * 100);
		float angleRight = Vector3.Angle(lookDirRight, fighter.RightGunJoint.forward);
		if(angleRight > gimbalLimit)
		{
			Vector3 newVerticalLoS = verticalLoSRight.normalized * (Mathf.Tan(Mathf.Deg2Rad * gimbalLimit) * 100);
			Vector3 newTarget = fighter.RightGunJoint.position + fighter.RightGunJoint.forward * 100 + newVerticalLoS;
			lookDirRight = newTarget - fighter.RightGun.position;
		}
		Quaternion rotationRight = Quaternion.LookRotation(lookDirRight, fighter.RightGunJoint.up);
		fighter.RightGun.rotation = Quaternion.Lerp(fighter.RightGun.rotation, rotationRight, Time.deltaTime * 9);
		*/
	}


	/*
	private void ShiftShield(bool isToFront)
	{
		if(PlayerShip.Shield != null && PlayerShip.Shield.Type == ShieldType.Fighter)
		{
			FighterShield shield = (FighterShield)PlayerShip.Shield;
			float frontPortion = shield.FrontCapacity / shield.TotalCapacity;
			float totalAmount = shield.FrontAmount + shield.RearAmount;
			if(isToFront)
			{
				frontPortion = Mathf.Clamp01(frontPortion + 0.1f);

			}
			else
			{
				frontPortion = Mathf.Clamp01(frontPortion - 0.1f);
			
			}

			shield.FrontCapacity = shield.TotalCapacity * frontPortion;
			shield.RearCapacity = shield.TotalCapacity * (1 - frontPortion);
			shield.FrontAmount = totalAmount * frontPortion;
			shield.RearAmount = totalAmount * (1 - frontPortion);
		}
	}
	*/

	private void SelectObject()
	{
		SelectedObjectType type = SelectedObjectType.Unknown;
		GameObject go = GameManager.Inst.CursorManager.SelectObject(out type);
		TargetShip = null;

		if(go != null)
		{
			if(type == SelectedObjectType.Unknown)
			{
				//attempt to figure out what it is

				StationBase station = go.GetComponent<StationBase>();
				if(station != null)
				{
					SelectedObjectType = SelectedObjectType.Station;
					SelectedObject = station;
					GameManager.Inst.UIManager.HUDPanel.OnSelectPlanetOrStation(station.transform, station.DisplayName);
					return;
				}

				StationComponent stationComp = go.GetComponent<StationComponent>();
				if(stationComp != null)
				{
					SelectedObjectType = SelectedObjectType.Station;
					SelectedObject = stationComp.ParentStation;
					GameManager.Inst.UIManager.HUDPanel.OnSelectPlanetOrStation(stationComp.ParentStation.transform, stationComp.ParentStation.DisplayName);
					return;
				}

				Planet planet = go.GetComponent<Planet>();
				if(planet != null)
				{
					SelectedObjectType = SelectedObjectType.Planet;
					SelectedObject = planet;
					GameManager.Inst.UIManager.HUDPanel.OnSelectPlanetOrStation(planet.transform, planet.DisplayName);
					return;
				}

			}
			else if(type == SelectedObjectType.Ship)
			{
				ShipBase ship = go.GetComponent<ShipBase>();
				if(ship != null)
				{
					SelectedObjectType = SelectedObjectType.Ship;
					SelectedObject = ship;
					GameManager.Inst.UIManager.HUDPanel.OnSelectShip(ship);
					TargetShip = ship;
					return;
				}

			}
		}
		else
		{
			GameManager.Inst.UIManager.HUDPanel.OnClearSelectedObject();
		}
	}

	private void Dock()
	{
		if(SelectedObject != null && SelectedObjectType == SelectedObjectType.Station)
		{
			StationBase station = (StationBase)SelectedObject;
			if(station != null)
			{
				DockSessionBase session;
				station.Dock(PlayerShip, out session);
			}
		}
	}

	private void AutopilotGoTo()
	{
		if(SelectedObject != null)
		{
			Vector3 gotoDest = Vector3.zero;
			if(SelectedObjectType == SelectedObjectType.Planet)
			{
				Planet planet = (Planet)SelectedObject;
				Vector3 distance = planet.transform.position - PlayerShip.transform.position;
				gotoDest = planet.transform.position - distance.normalized * (40f + planet.OriginalScale.x / 2f);
			}
			else if(SelectedObjectType == SelectedObjectType.Station)
			{
				StationBase station = (StationBase)SelectedObject;
				Vector3 distance = station.transform.position - PlayerShip.transform.position;
				gotoDest = PlayerShip.transform.position + distance.normalized * (distance.magnitude - 20f);
			}

			if(gotoDest != Vector3.zero)
			{
				MacroAITask task = new MacroAITask();
				task.TaskType = MacroAITaskType.Travel;
				task.TravelDestSystemID = GameManager.Inst.WorldManager.CurrentSystem.ID;
				task.TravelDestNodeID = "";
				task.IsDestAStation = false;
				Transform origin = GameObject.Find("Origin").transform;
				task.TravelDestCoord = new RelLoc(origin.position, gotoDest, origin);

				IsAutopilot = true;
				PlayerParty.WaitTimer = 0;
				PlayerParty.CurrentTask = task;

				PlayerParty.HasReachedDestNode = false;
				PlayerParty.DestNode = GameManager.Inst.NPCManager.MacroAI.GetClosestNodeToLocation(task.TravelDestCoord.RealPos, GameManager.Inst.WorldManager.AllSystems[task.TravelDestSystemID]);
				Debug.Log("Autopilot dest node " + PlayerParty.DestNode.ID);
				PlayerAutopilot.Activate();
				_isMouseFlight = false;
			}


		}
	}


}

public enum SelectedObjectType
{
	Unknown,
	Ship,
	Station,
	Turret,
	Pickup,
	Planet,
}

