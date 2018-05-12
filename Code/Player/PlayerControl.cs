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




	public void Initialize()
	{
		GameObject o = GameObject.Find("PlayerShip");
		if(o != null)
		{
			PlayerShip = o.GetComponent<ShipBase>();
		}

		o = GameObject.Find("SpaceDust");
		SpaceDust = o.GetComponent<ParticleSystem>();
		_testSphere = GameObject.Find("Sphere").transform;

		o = SpaceDust.transform.Find("TradelaneDust").gameObject;
		TradelaneDust = o.GetComponent<ParticleSystem>();

		_isMouseFlight = true;

		/*
		o = GameObject.Find("AIShip");
		if(o != null)
		{
			TargetShip = o.GetComponent<ShipBase>();
		}
		*/
	}

	public void PerFrameUpdate()
	{

		UpdateKeyInput();
		UpdateMouseInput();


		UpdateWeaponAim();

	}

	public void FixedFrameUpdate()
	{
		UpdateShipRotation();
		UpdateShipMovement();
	}

	public void LateFrameUpdate()
	{
		
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
		station.Undock(PlayerShip);
	}



	private void UpdateKeyInput()
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
			if(CurrentTradelaneSession != null)
			{
				CurrentTradelaneSession.Stage = TLSessionStage.Cancelling;
			}
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
				ShiftShield(true);
			}
			else
			{
				_throttle += 0.1f;
			}
		}
		else if(wheelInput < 0)
		{
			if(Input.GetKey(KeyCode.LeftControl))
			{
				ShiftShield(false);
			}
			else
			{
				_throttle -= 0.1f;
			}
		}

		_throttle = Mathf.Clamp01(_throttle);

		//firing weapon
		Fighter fighter = (Fighter)PlayerShip;
		if(Input.GetMouseButton(0))
		{
			foreach(WeaponJoint joint in PlayerShip.MyReference.WeaponJoints)
			{
				if(joint.MountedWeapon != null)
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
			PlayerShip.RB.AddTorque(PlayerShip.transform.up * _yawForce * 1.2f);
		}

		//Pitch
		float maxPitchRate = 2f;
		if(Mathf.Abs(angularVelocity.x) < maxPitchRate * engineKillBonus)
		{
			PlayerShip.RB.AddTorque(PlayerShip.transform.right * _pitchForce * 1.2f);
		}

		//Roll is based on key press A and D and it lerps to 0
		float maxRollRate = 2f;
		if(Mathf.Abs(angularVelocity.z) < maxRollRate)
		{
			if(Mathf.Abs(_rollForce) > 0.02f)
			{
				PlayerShip.RB.AddTorque(PlayerShip.transform.forward * _rollForce * 1f);
			}
			else
			{
				PlayerShip.RB.AddTorque(PlayerShip.transform.forward * angularVelocity.z * -0.5f);
			}
		}
		else
		{
			PlayerShip.RB.AddTorque(PlayerShip.transform.forward * angularVelocity.z * -0.5f);
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

			if(_thruster != 0 || _isFAKilled)
			{
				maxSpeed = maxSpeed * 1.5f;
			}

			if(!_isFAKilled)
			{
				
				maxSpeed = maxSpeed * _throttle;
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
		float gimbalLimit = 10;

		Camera camera = Camera.main;
		Vector2 mousePos = new Vector2();

		mousePos = Input.mousePosition;
		Vector3 targetPos = camera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 100));

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



	private void ShiftShield(bool isToFront)
	{
		if(PlayerShip.Shield != null && PlayerShip.Shield.Type == ShieldType.Fighter)
		{
			FighterShield shield = (FighterShield)PlayerShip.Shield;
			if(isToFront)
			{
				shield.FrontCapacity = Mathf.Clamp(shield.FrontCapacity + shield.TotalCapacity * 0.15f, 0.1f, shield.TotalCapacity);
				shield.RearCapacity = Mathf.Clamp(shield.TotalCapacity - shield.FrontCapacity, 0.1f, shield.TotalCapacity);
				if(shield.RearAmount > shield.RearCapacity)
				{
					shield.FrontAmount = Mathf.Clamp(shield.FrontAmount + (shield.RearAmount - shield.RearCapacity), 0, shield.FrontCapacity);
					shield.RearAmount = shield.RearCapacity;
				}
			}
			else
			{
				shield.RearCapacity = Mathf.Clamp(shield.RearCapacity + shield.TotalCapacity * 0.15f, 0.1f, shield.TotalCapacity);
				shield.FrontCapacity = Mathf.Clamp(shield.TotalCapacity - shield.RearCapacity, 0.1f, shield.TotalCapacity);
				if(shield.FrontAmount > shield.FrontCapacity)
				{
					shield.RearAmount = Mathf.Clamp(shield.RearAmount + (shield.FrontAmount - shield.FrontCapacity), 0, shield.RearCapacity);
					shield.FrontAmount = shield.FrontCapacity;
				}
			}
		}
	}

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
				station.Dock(PlayerShip);
			}
		}
	}



}

public enum SelectedObjectType
{
	Unknown,
	Ship,
	Station,
	Pickup,
	Planet,
}

