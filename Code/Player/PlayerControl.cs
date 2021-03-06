﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl
{
	public ShipBase PlayerShip;

	public KeyBinding KeyBinding;



	public ParticleSystem SpaceDust;
	public ParticleSystem TradelaneDust;
	public ParticleSystem AsteroidDust;

	public Vector3 GimballTarget { get { return _gimballTarget; }}

	public object SelectedObject;
	public SelectedObjectType SelectedObjectType;
	

	public ShipBase TargetShip;

	public TLTransitSession CurrentTradelaneSession;

	public bool IsAutopilot;
	public Autopilot PlayerAutopilot;
	public MacroAIParty PlayerParty;

	public List<WeaponJoint> [] WeaponGroups;

	public AudioSource PrimaryEngine;
	public AudioSource SecondaryEngine;

	public float Throttle { get { return _throttle; } }
	public bool IsFAKilled { get { return _isFAKilled; } }
	public bool IsMouseFlight { get { return _isMouseFlight; } }
	public float YawForce { get { return _yawForce; } }
	public float PitchForce { get { return _pitchForce; } }
	public float RollForce { get { return _rollForce; } }
	public float ForwardForce { get { return _forwardForce; } }
	public float ThrusterForce { get { return _thruster; } }
	public bool IsGamePaused { get { return Time.timeScale <= 0; } }


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

	private float _cmTimer;


	public void Initialize()
	{
		KeyBinding = new KeyBinding();
		KeyBinding.Controls = GameManager.Inst.DBManager.UserPrefDataHandler.GetKeyBindings(false);

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

		o = SpaceDust.transform.Find("AsteroidDust").gameObject;
		AsteroidDust = o.GetComponent<ParticleSystem>();

		_isMouseFlight = true;
		_cmTimer = 4f;

		PrimaryEngine = PlayerShip.transform.Find("PrimaryEngineSound").GetComponent<AudioSource>();
		PrimaryEngine.loop = true;
		PrimaryEngine.clip = GameManager.Inst.SoundManager.GetClip("NormalEngine1");
		PrimaryEngine.Play();

		SecondaryEngine = PlayerShip.transform.Find("SecondaryEngineSound").GetComponent<AudioSource>();
		SecondaryEngine.loop = true;


		GameManager.Inst.NPCManager.AllShips.Add(PlayerShip);

		WeaponGroups = new List<WeaponJoint>[4];
		WeaponGroups[0] = new List<WeaponJoint>();
		WeaponGroups[1] = new List<WeaponJoint>();
		WeaponGroups[2] = new List<WeaponJoint>();
		WeaponGroups[3] = new List<WeaponJoint>();

		GameEventHandler.OnShipDeath -= OnNPCDeath;
		GameEventHandler.OnShipDeath += OnNPCDeath;

		Debug.Log("Initializing player control DONE");
	}

	public void LoadPlayerShip()
	{
		Debug.Log("Loading player ship");
		PlayerShip.RB.inertiaTensor = new Vector3(1, 1, 1);

		//use player loadout to spawn player ship here
		GameManager.Inst.NPCManager.SpawnPlayerShip(GameManager.Inst.PlayerProgress.ActiveLoadout, "player", PlayerParty);

		WeaponGroups[0].Add(PlayerShip.MyReference.WeaponJoints[2]);
		WeaponGroups[1].Add(PlayerShip.MyReference.WeaponJoints[0]);
		WeaponGroups[1].Add(PlayerShip.MyReference.WeaponJoints[1]);

		PlayerShip.MyReference.ExhaustController.setExhaustState(ExhaustState.Normal);
		PlayerShip.MyReference.ExhaustController.setExhaustLength(_throttle);

		Debug.Log("Loading player ship DONE");
	}

	public void CreatePlayerParty()
	{
		Debug.Log("Creating player party");
		//create MacroAIParty for player
		PlayerParty = GameManager.Inst.NPCManager.MacroAI.GeneratePlayerParty();
		PlayerAutopilot = PlayerShip.GetComponent<Autopilot>();
		PlayerAutopilot.Initialize(PlayerParty, GameManager.Inst.NPCManager.AllFactions["player"]);
		Debug.Log("Creating player party DONE");
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
		/*
		if(GameManager.Inst.SceneType == SceneType.SpaceTest)
		{
			UpdateCommandKeyInput();
			return;
		}

		if(GameManager.Inst.UIManager.HUDPanel.IsActive)
		{

			if(!IsAutopilot)
			{
				UpdateMovementKeyInput();
			}
			PlayerAutopilot.APUpdate();
			UpdateCommandKeyInput();
			UpdateMouseInput();

		}
		else
		{
			UpdateUIKeyInput();
		}
		*/

		UpdateWeaponAim();

		if(_cmTimer < 3f)
		{
			_cmTimer += Time.deltaTime;
		}

		//check if any incoming missiles are already destroyed
		PlayerShip.IncomingMissiles.RemoveAll(GameObject => GameObject == null);

	
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
		UpdateEngineSound();
		UpdateSpaceDust();
	}

	public void LateFrameUpdate()
	{
		
	}

	public void PauseGame(bool isPaused)
	{
		if(isPaused)
		{
			Time.timeScale = 0;
			GameManager.Inst.UIManager.HUDPanel.OnPauseGame();
		}
		else
		{
			Time.timeScale = 1;
			GameManager.Inst.UIManager.HUDPanel.OnUnpauseGame();
		}
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
		Debug.Log("Spawning in station " + GameManager.Inst.PlayerProgress.SpawnStationID);
		StationBase station = GameObject.Find(GameManager.Inst.PlayerProgress.SpawnStationID).GetComponent<StationBase>();
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

		InputEventHandler.Instance.InputState = InputState.InFlight;
	}

	public void SetMouseFlight(bool isOn)
	{
		_isMouseFlight = isOn;
	}

	public void UpdateSpaceTestInput()
	{
		if(Input.GetKeyDown(KeyCode.F12))
		{
			GameManager.Inst.DBManager.XMLParserWorld.GenerateSystemXML();
		}

	}

	public void UpdateUIKeyInput()
	{
		if(Input.GetKeyDown(KeyCode.F10))
		{
			if(GameManager.Inst.UIManager.KeyBindingPanel.IsActive)
			{
				UIEventHandler.Instance.TriggerCloseKeyBindingPanel();
				Time.timeScale = 1;
				GameManager.Inst.UIManager.HUDPanel.OnUnpauseGame();
			}
		}

		if(Input.GetKeyDown(KeyCode.BackQuote))
		{
			if(GameManager.Inst.UIManager.EconDebugPanel.IsActive)
			{
				UIEventHandler.Instance.TriggerCloseEconDebugPanel();
			}
		}

	}



	public void UpdateAutopilotKeyInput()
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			if(IsAutopilot)
			{
				CancelAutopilot();

			}
		}
	}


	public void UpdateInFlightKeyInput()
	{
		//power management
		if(KeyBinding.Controls[UserInputs.PowerManagement].EvalKeyDown())
		{
			UIEventHandler.Instance.TriggerOpenPowerManagement();
		}

		//select
		if(KeyBinding.Controls[UserInputs.Select].Eval())
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

		if(KeyBinding.Controls[UserInputs.Pause].Eval())
		{
			if(!GameManager.Inst.UIManager.KeyBindingPanel.IsActive)
			{
				UIEventHandler.Instance.TriggerOpenKeyBindingPanel();
				Time.timeScale = 0;
				GameManager.Inst.UIManager.HUDPanel.OnPauseGame();
			}
		}

		if(Input.GetKeyDown(KeyCode.BackQuote))
		{
			if(!GameManager.Inst.UIManager.EconDebugPanel.IsActive)
			{
				UIEventHandler.Instance.TriggerOpenEconDebugPanel();
			}
		}




		//toggle view
		if(Input.GetKeyDown(KeyCode.V))
		{
			GameManager.Inst.CameraController.SetView(!GameManager.Inst.CameraController.IsFirstPerson);
		}

		if(PlayerShip.IsInPortal)
		{
			return;
		}

		//autopilot goto
		if(Input.GetKeyDown(KeyCode.F2))
		{
			AutopilotGoTo();
		}

		//movement
		float rollSpeed = 1;
		float rollStopSpeed = 4;

		if(KeyBinding.Controls[UserInputs.FlightAssist].Eval())
		{
			_isFAKilled = !_isFAKilled;
			if(_isFAKilled && PlayerShip.Engine.IsCruising)
			{
				PlayerShip.Engine.CancelCruise();
			}
		}

		if(KeyBinding.Controls[UserInputs.MouseFlight].Eval())
		{
			_isMouseFlight = !_isMouseFlight;
		}


		if(KeyBinding.Controls[UserInputs.Cruise].Eval())
		{
			if(!PlayerShip.Engine.IsCruising && !PlayerShip.Engine.IsPrepCruise)
			{
				_isFAKilled = false;
				PlayerShip.Engine.StartCruisePrep();
			}
			else
			{
				PlayerShip.Engine.CancelCruise();
			}
		}




		//rolling
		if(KeyBinding.Controls[UserInputs.RollLeft].EvalKeyDown())
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
		if(KeyBinding.Controls[UserInputs.RollRight].EvalKeyDown())
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


		if(KeyBinding.Controls[UserInputs.ForwardThruster].EvalKeyDown())
		{
			_strafeHor = 0;
			_strafeVer = 0;
			_thruster = 1;

		}

		if(KeyBinding.Controls[UserInputs.ReverseThruster].EvalKeyDown())
		{
			_strafeHor = 0;
			_strafeVer = 0;
			_thruster = -0.2f;
		}


		

		//strafing


			

		if(KeyBinding.Controls[UserInputs.VectorLeft].EvalKeyDown())
		{
			_thruster = 0;
			_strafeHor = -1;
		}
		if(KeyBinding.Controls[UserInputs.VectorRight].EvalKeyDown())
		{
			_thruster = 0;
			_strafeHor = 1;
		}
		if(KeyBinding.Controls[UserInputs.VectorUp].EvalKeyDown())
		{
			_thruster = 0;
			_strafeVer = -1;
		}
		if(KeyBinding.Controls[UserInputs.VectorDown].EvalKeyDown())
		{
			_thruster = 0;
			_strafeVer = 1;
		}

		if(_strafeHor != 0 || _strafeVer != 0)
		{
			_rollValue = Mathf.Lerp(_rollValue, 0, Time.deltaTime * rollStopSpeed);
		}

		_rollForce = GameManager.Inst.Constants.RollCurve.Evaluate(_rollValue);


		if(!KeyBinding.Controls[UserInputs.ForwardThruster].EvalKeyDown() && !KeyBinding.Controls[UserInputs.ReverseThruster].EvalKeyDown())
		{
			_thruster = 0;

		}

		if(!KeyBinding.Controls[UserInputs.VectorLeft].EvalKeyDown() && !KeyBinding.Controls[UserInputs.VectorRight].EvalKeyDown())
		{
			_strafeHor = 0;
		}

		if(!KeyBinding.Controls[UserInputs.RollLeft].EvalKeyDown() && !KeyBinding.Controls[UserInputs.RollRight].EvalKeyDown())
		{
			_rollValue = Mathf.Lerp(_rollValue, 0, Time.deltaTime * rollStopSpeed);

		}

		if(!KeyBinding.Controls[UserInputs.VectorUp].EvalKeyDown() && !KeyBinding.Controls[UserInputs.VectorDown].EvalKeyDown())
		{
			_strafeVer = 0;
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
		if(!PlayerShip.Engine.IsCruising && !PlayerShip.Engine.IsPrepCruise)
		{
			if(KeyBinding.Controls[UserInputs.FireWeaponGroup1].EvalKeyDown())
			{
				foreach(WeaponJoint joint in WeaponGroups[0])
				{
					if(joint.MountedWeapon != null && joint.ControlMode == TurretControlMode.Manual)
					{
						joint.MountedWeapon.Fire();
					}
				}

			}

			if(KeyBinding.Controls[UserInputs.FireWeaponGroup2].EvalKeyDown())
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


		//weapon
		//countermeasure
		if(KeyBinding.Controls[UserInputs.Countermeasure].Eval())
		{
			DropCountermeasure();
		}

		//active ship mod
		if(KeyBinding.Controls[UserInputs.DeployShipMod].Eval())
		{
			Debug.Log("Deploy ship mod");
			DeployShipMod();
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

		//Mouse input
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


		Vector3 angularVelocity = PlayerShip.transform.InverseTransformDirection(PlayerShip.RB.angularVelocity);
		float engineKillBonus = _isFAKilled ? 1.2f : 1f;

		//Yaw
		float maxYawRate = 1.2f;

		PlayerShip.RB.AddTorque(PlayerShip.transform.up * _yawForce * PlayerShip.TorqueModifier * engineKillBonus, ForceMode.Acceleration);


		//Pitch
		float maxPitchRate = 2f;

		PlayerShip.RB.AddTorque(PlayerShip.transform.right * _pitchForce * PlayerShip.TorqueModifier * engineKillBonus, ForceMode.Acceleration);


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
			if(PlayerShip.Engine.IsCruising)
			{
				_forwardForce = PlayerShip.Engine.Acceleration * 2 * PlayerShip.EnginePowerAlloc;
			}
			else
			{
				_forwardForce = _throttle * PlayerShip.Engine.Acceleration * PlayerShip.EnginePowerAlloc;
			}


			float maxSpeed = PlayerShip.Engine.MaxSpeed;

			//PlayerShip.MyReference.ExhaustController.setExhaustState(ExhaustState.Thruster);

			//check if thruster is available
			Thruster thruster = PlayerShip.Thruster;
			if(thruster != null && !PlayerShip.Engine.IsCruising && !PlayerShip.Engine.IsPrepCruise)
			{
				if((_thruster != 0 || _strafeHor != 0 || _strafeVer != 0) && ((!PlayerShip.Engine.IsThrusting && thruster.CurrentFuel > thruster.MaxFuel * 0.1f) || (PlayerShip.Engine.IsThrusting && thruster.CurrentFuel > 0)))
				{
					

					if(_thruster > 0)
					{
						PlayerShip.Engine.IsThrusting = true;
						PlayerShip.RB.AddForce(PlayerShip.transform.forward * _thruster * 10);
						GameManager.Inst.CameraShaker.TriggerScreenShake(0.15f, 0.0065f, true);
					}
					else if(_thruster < 0)
					{
						PlayerShip.Engine.IsThrusting = true;
						PlayerShip.RB.AddForce(PlayerShip.transform.forward * _thruster * 10);
					}
					//strafe
					if(thruster.CanStrafe)
					{
						PlayerShip.RB.AddForce(PlayerShip.transform.right * _strafeHor * 3);
						PlayerShip.RB.AddForce(PlayerShip.transform.up * _strafeVer * -3);
					}

					float consumption = 0;
					if(_thruster > 0)
					{
						consumption = thruster.ConsumptionRate;
					}
					else if(_thruster < 0)
					{
						consumption = thruster.ConsumptionRate * 0.5f;
					}
					else if(thruster.CanStrafe && (Mathf.Abs(_strafeHor) > 0 || Mathf.Abs(_strafeVer) > 0))
					{
						consumption = thruster.ConsumptionRate * 0.3f;
					}

					thruster.CurrentFuel = Mathf.Clamp(thruster.CurrentFuel - consumption * Time.fixedDeltaTime, 0, thruster.MaxFuel);

				}
				else
				{
					PlayerShip.Engine.IsThrusting = false;
					thruster.CurrentFuel = Mathf.Clamp(thruster.CurrentFuel + thruster.RestoreRate * Time.fixedDeltaTime, 0, thruster.MaxFuel);

				}

			}

			if(PlayerShip.Engine.IsCruising)
			{
				PlayerShip.MyReference.ExhaustController.setExhaustState(ExhaustState.Cruise);
			}
			else if(PlayerShip.Engine.IsPrepCruise)
			{
				PlayerShip.MyReference.ExhaustController.setExhaustState(ExhaustState.Idle);
			}
			else if(PlayerShip.Engine.IsThrusting)
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

			PlayerShip.MyReference.ExhaustController.UpdateExhaustTrail(0);

			if(thruster != null && (_thruster != 0 || _isFAKilled))
			{
				maxSpeed = PlayerShip.Thruster.MaxSpeed;
			}

			if(PlayerShip.Engine.IsCruising)
			{
				maxSpeed = PlayerShip.Engine.CruiseSpeed;
			}

			if(!_isFAKilled && !PlayerShip.Engine.IsCruising)
			{
				
				maxSpeed = maxSpeed * _throttle + 0.01f;
			}


			if(velocity.magnitude < maxSpeed && !_isFAKilled && !PlayerShip.Engine.IsPrepCruise && _thruster >= 0)
			{
				PlayerShip.RB.AddForce(PlayerShip.transform.forward * _forwardForce);
			}




			//drag
			float drag = -0.2f;
			if(_thruster != 0)
			{
				drag = -1f;
			}
			//Debug.Log(velocity.magnitude);
			if(velocity.magnitude >= maxSpeed * 0.95f)
			{
				PlayerShip.RB.AddForce(drag * velocity * Mathf.Lerp(0.01f, 1f, (velocity.magnitude - maxSpeed * 0.95f) / (maxSpeed * 0.05f)));

			}
			else
			{
				PlayerShip.RB.AddForce(-1 * velocity * 0.01f);

			}

			//flight assist
			if((!_isFAKilled || _thruster != 0) && maxSpeed > 0)
			{
				Vector3 driftVelocity = velocity - Vector3.Dot(velocity, PlayerShip.transform.forward) * PlayerShip.transform.forward;
				float assistLevel = Mathf.Clamp(1 - Mathf.Clamp01(PlayerShip.RB.velocity.magnitude / maxSpeed), 0.6f, 1);
				PlayerShip.RB.AddForce(-1 * driftVelocity.normalized * driftVelocity.magnitude * assistLevel * PlayerShip.EnginePowerAlloc);
			}

		}
		else
		{
			PlayerShip.RB.velocity = Vector3.zero;
		}





	}

	private void UpdateEngineSound()
	{
		if(!_isFAKilled)
		{
			PrimaryEngine.pitch = 0.7f + 0.3f * _throttle;

		}
		else
		{
			PrimaryEngine.pitch = 0.6f;
		}

		ExhaustState state = PlayerShip.MyReference.ExhaustController.GetExhaustState();

		if(state == ExhaustState.Thruster)
		{
			SecondaryEngine.volume = Mathf.Lerp(SecondaryEngine.volume, 1, 30 * Time.fixedDeltaTime);
			SecondaryEngine.pitch = 1;
			if(SecondaryEngine.clip == null || SecondaryEngine.clip.name != "Afterburner")
			{
				SecondaryEngine.clip = GameManager.Inst.SoundManager.GetClip("Afterburner");
			}
			else if(!SecondaryEngine.isPlaying)
			{
				SecondaryEngine.Play();
			}
		}
		else if(state == ExhaustState.Normal || state == ExhaustState.Idle)
		{
			
			if(PlayerShip.Engine.IsPrepCruise)
			{
				/*
				if(PlayerShip.Engine.PrepPercent > 0.99f)
				{
					SecondaryEngine.volume = Mathf.Lerp(SecondaryEngine.volume, 0, 60 * Time.fixedDeltaTime);;
				}
				else
				{
					SecondaryEngine.volume = Mathf.Lerp(SecondaryEngine.volume, 1, 30 * Time.fixedDeltaTime);
				}

				if(SecondaryEngine.clip == null || SecondaryEngine.clip.name != "CruiseChargeUp")
				{
					
					SecondaryEngine.clip = GameManager.Inst.SoundManager.GetClip("CruiseChargeUp");
				}
				else if(!SecondaryEngine.isPlaying)
				{
					SecondaryEngine.Play();
				}
				*/

				SecondaryEngine.volume = Mathf.Lerp(SecondaryEngine.volume, 1f, 30 * Time.fixedDeltaTime);
				if(SecondaryEngine.clip == null || SecondaryEngine.clip.name != "Cruise")
				{
					SecondaryEngine.clip = GameManager.Inst.SoundManager.GetClip("Cruise");
					SecondaryEngine.volume = 0;
				}
				else if(!SecondaryEngine.isPlaying)
				{
					SecondaryEngine.Play();
				}
				SecondaryEngine.pitch = 0.4f + Mathf.Pow(PlayerShip.Engine.PrepPercent, 2) * 0.6f;

			}
			else
			{
				SecondaryEngine.pitch = 1;
				SecondaryEngine.volume = Mathf.Lerp(SecondaryEngine.volume, 0, 30 * Time.fixedDeltaTime);
			}
		}
		else if(state == ExhaustState.Cruise)
		{
			SecondaryEngine.volume = Mathf.Lerp(SecondaryEngine.volume, 1, 30 * Time.fixedDeltaTime);
			SecondaryEngine.pitch = 1.2f;
			if(SecondaryEngine.clip == null || SecondaryEngine.clip.name != "Cruise")
			{
				SecondaryEngine.clip = GameManager.Inst.SoundManager.GetClip("Cruise");
				SecondaryEngine.volume = 0;
			}
			else if(!SecondaryEngine.isPlaying)
			{
				SecondaryEngine.Play();
			}
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
			ParticleSystem.EmissionModule aEmission = AsteroidDust.emission;
			aEmission.enabled = false;
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
				ParticleSystem.MainModule sDnewMain = SpaceDust.main;
				sDnewMain.startSpeed = velocity.magnitude * 8 * -1;
				SpaceDust.transform.position = PlayerShip.transform.position - SpaceDust.transform.forward * 20f;

				ParticleSystem.EmissionModule sEmission = SpaceDust.emission;
				sEmission.enabled = true;
				ParticleSystem.EmissionModule aEmission = AsteroidDust.emission;
				aEmission.enabled = true;
				//find gradient from all asteroid fields
				float gradient = 0;
				foreach(AsteroidField field in GameManager.Inst.WorldManager.AsteroidFields)
				{
					if(field.Gradient > gradient)
					{
						gradient = field.Gradient;
					}
				}
				aEmission.rateOverTime = Mathf.Lerp(2f, 12f, gradient);

				if(velocity.magnitude > 4)
				{
					float velAngle = Vector3.Angle(PlayerShip.transform.forward, velocity);
					float velocityGradient = Mathf.Clamp01((velocity.magnitude - 4) / 8);
					PlayerShip.SetVortex(0.1f * gradient * (1 - Mathf.Clamp01(velAngle / 30f)) * velocityGradient, Mathf.Lerp(1.0f, 0.4f, velocityGradient));
				}
				else
				{
					PlayerShip.SetVortex(0, 0);
				}

				_isDustSettled = false;
			}
			else
			{
				if(!_isDustSettled)
				{
					SpaceDust.transform.position = PlayerShip.transform.position - SpaceDust.transform.forward * 20f;
					ParticleSystem.EmissionModule sEmission = SpaceDust.emission;
					sEmission.enabled = false;
					ParticleSystem.EmissionModule aEmission = AsteroidDust.emission;
					aEmission.enabled = false;
					_isDustSettled = true;
				}

				PlayerShip.SetVortex(0, 0);
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
		GameManager.Inst.UIManager.HUDPanel.OnClearSelectedObject();

		SelectedObjectType type = SelectedObjectType.Unknown;
		GameObject go = GameManager.Inst.CursorManager.SelectObject(out type);
		TargetShip = null;

		if(go != null)
		{
			GameManager.Inst.SoundManager.UI.PlayOneShot(GameManager.Inst.SoundManager.GetClip("Select"));

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

				InputEventHandler.Instance.InputState = InputState.Autopilot;
			}


		}
	}

	private void DropCountermeasure()
	{
		if(_cmTimer >= 3f)
		{
			foreach(Defensive d in PlayerShip.MyReference.Defensives)
			{
				if(d.Type == DefensiveType.Countermeasure)
				{
					CMDispenser cm = (CMDispenser)d;
					cm.DropCountermeasure();
				}
			}
		}

	}

	private void DeployShipMod()
	{
		if(PlayerShip.ShipModSlots.ActiveMod != null)
		{
			PlayerShip.ShipModSlots.ActiveMod.Deploy();
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

