using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpGate : StationBase
{
	public string TargetSystem;
	public string ExitGateID;
	public Vector3 SpawnDisposition;

	public Transform Spinner;
	public ParticleSystem GateRing;
	public Renderer Horizon;
	public bool IsGateActive;
	public bool IsPortalReady;
	public float InactivityTimeout;
	public float MaxSpinnerSpeed;
	public DockingTrigger DockingTrigger;

	private float _spinnerSpeed;
	private ParticleSystem.EmissionModule _ringEmission;
	private float _activeTimer;
	private Material _horizonMatInst;

	private bool _isPlayerWarping;
	
	// Update is called once per frame
	void Update () 
	{
		if(IsGateActive)
		{
			//gradually increase spinner speed 
			_spinnerSpeed = Mathf.Clamp(_spinnerSpeed + Time.deltaTime * 60, 0, MaxSpinnerSpeed);
			if(_spinnerSpeed >= MaxSpinnerSpeed * 0.6f)
			{
				_ringEmission.enabled = true;
			}
			if(_spinnerSpeed >= MaxSpinnerSpeed * 0.9f)
			{
				SetHorizonAlpha(Mathf.Clamp(_horizonMatInst.color.a + Time.deltaTime * 1, 0.1f, 0.4f));
				EnablePortal();
			}
		}
		else
		{
			//gradually decrease spinner speed
			_spinnerSpeed = Mathf.Clamp(_spinnerSpeed - Time.deltaTime * 9, 0, MaxSpinnerSpeed);
		}

		Spinner.RotateAround(Spinner.position, Spinner.forward, _spinnerSpeed * Time.deltaTime);

		_activeTimer += Time.deltaTime;

		if(_activeTimer >= InactivityTimeout)
		{
			IsGateActive = false;
			DisablePortal();
			_ringEmission.enabled = false;
			SetHorizonAlpha(Mathf.Clamp(_horizonMatInst.color.a - Time.deltaTime * 1, 0.1f, 0.4f));
		}

		if(_isPlayerWarping)
		{
			GameManager.Inst.MainCamera.fieldOfView = Mathf.Lerp(GameManager.Inst.MainCamera.fieldOfView, 30f, Time.deltaTime * 3);
			GameManager.Inst.CameraController.FarCamera.fieldOfView = GameManager.Inst.MainCamera.fieldOfView;
		}
	}

	public override void Initialize ()
	{
		//find all children's station component and assign to this
		StationComponent [] components = transform.GetComponentsInChildren<StationComponent>();
		foreach(StationComponent comp in components)
		{
			comp.ParentStation = this;
		}

		_ringEmission = GateRing.emission;

		_ringEmission.enabled = false;
		_horizonMatInst = Horizon.material;
		SetHorizonAlpha(0.1f);
		DisablePortal();
	}

	public override DockRequestResult Dock (ShipBase requester, out DockSessionBase session)
	{
		Debug.Log("Begin docking at jumpgate " + this.ID);
		IsGateActive = true;
		_activeTimer = 0;
		session = null;
		return DockRequestResult.Accept;
	}

	public override DockRequestResult Undock (ShipBase requester, out DockSessionBase session)
	{
		Vector3 spawnLoc = DockingTrigger.transform.position + SpawnDisposition;
		requester.transform.position = spawnLoc;
		requester.transform.rotation = Quaternion.LookRotation(DockingTrigger.transform.up, Vector3.up);
		session = null;
		return DockRequestResult.Accept;
	}

	public override void OnDetectDocking (string triggerID, ShipBase requester)
	{
		if(requester == GameManager.Inst.PlayerControl.PlayerShip)
		{
			Debug.Log("Through the wormhole!");
			requester.IsInPortal = true;
			_isPlayerWarping = true;
			UIEventHandler.Instance.TriggerBeginDocking();
			GameManager.Inst.PlayerControl.DockComplete(this, StationType.JumpGate);
		}
		else
		{
			AI npcAI = requester.GetComponent<AI>();
			npcAI.MyParty.CurrentSystemID = TargetSystem;

		}


	}



	private void SetHorizonAlpha(float alpha)
	{
		_horizonMatInst.color = new Color(_horizonMatInst.color.r, _horizonMatInst.color.g, _horizonMatInst.color.b, alpha);
	}

	private void EnablePortal()
	{
		DockingTrigger.GetComponent<Collider>().enabled = true;
		IsPortalReady = true;
	}

	private void DisablePortal()
	{
		DockingTrigger.GetComponent<Collider>().enabled = false;
		IsPortalReady = false;
	}
}


public class JumpGateData : NavNode
{
	public string TargetSystem;
	public string ExitGateID;
	public string DisplayName;
	public Vector3 EulerAngles;
	public Vector3 SpawnDisposition;
}