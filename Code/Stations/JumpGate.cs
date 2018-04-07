using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpGate : StationBase
{
	public string TargetSystem;
	public string ExitGateID;

	public Transform Spinner;
	public ParticleSystem GateRing;
	public Renderer Horizon;
	public bool IsGateActive;
	public float InactivityTimeout;
	public float MaxSpinnerSpeed;
	public DockingTrigger DockingTrigger;

	private float _spinnerSpeed;
	private ParticleSystem.EmissionModule _ringEmission;
	private float _activeTimer;
	private Material _horizonMatInst;
	
	// Update is called once per frame
	void Update () 
	{
		if(IsGateActive)
		{
			//gradually increase spinner speed 
			_spinnerSpeed = Mathf.Clamp(_spinnerSpeed + Time.deltaTime * 6, 0, MaxSpinnerSpeed);
			if(_spinnerSpeed >= MaxSpinnerSpeed * 0.6f)
			{
				_ringEmission.enabled = true;
			}
			if(_spinnerSpeed >= MaxSpinnerSpeed * 0.9f)
			{
				SetHorizonAlpha(Mathf.Clamp(_horizonMatInst.color.a + Time.deltaTime * 1, 0, 0.4f));
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
			_ringEmission.enabled = false;
			SetHorizonAlpha(Mathf.Clamp(_horizonMatInst.color.a - Time.deltaTime * 1, 0, 0.4f));
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
		SetHorizonAlpha(0);
		DisablePortal();
	}

	public override DockRequestResult Dock (ShipBase requester)
	{
		IsGateActive = true;
		_activeTimer = 0;

		return DockRequestResult.Accept;
	}

	public override void OnDetectDocking (string triggerID, ShipBase requester)
	{
		if(requester == GameManager.Inst.PlayerControl.PlayerShip)
		{
			Debug.Log("Through the wormhole!");
		}

	}



	private void SetHorizonAlpha(float alpha)
	{
		_horizonMatInst.color = new Color(_horizonMatInst.color.r, _horizonMatInst.color.g, _horizonMatInst.color.b, alpha);
	}

	private void EnablePortal()
	{
		DockingTrigger.GetComponent<Collider>().enabled = true;
	}

	private void DisablePortal()
	{
		DockingTrigger.GetComponent<Collider>().enabled = false;
	}
}
