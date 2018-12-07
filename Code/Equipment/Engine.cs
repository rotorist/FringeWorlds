using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : EquipmentBase 
{
	public float MaxSpeed;
	public float CruiseSpeed;
	public float CruisePrepTime;
	public float Acceleration;

	public bool IsThrusting;
	public bool IsCruising;
	public bool IsPrepCruise;

	public float PrepPercent { get { return _cruisePrepTimer / CruisePrepTime; } }

	private float _cruisePrepTimer;

	void Update()
	{
		if(IsPrepCruise)
		{
			if(_cruisePrepTimer < CruisePrepTime)
			{
				_cruisePrepTimer += Time.deltaTime;
			}
			else
			{
				IsPrepCruise = false;
				IsCruising = true;
				IsThrusting = false;
			}
		}
	}

	public void Initialize(ShipStats stats)
	{
		MaxSpeed = stats.MaxSpeed;
		CruiseSpeed = stats.CruiseSpeed;
		CruisePrepTime = stats.CruisePrepTime;
		Acceleration = stats.Acceleration;
	}

	public void StartCruisePrep()
	{
		if(!IsCruising && !IsPrepCruise)
		{
			_cruisePrepTimer = 0;
			IsPrepCruise = true;
			IsThrusting = false;
		}
	}

	public void CancelCruise()
	{
		_cruisePrepTimer = 0;
		IsPrepCruise = false;
		IsCruising = false;
		GameManager.Inst.PlayerControl.PrimaryEngine.PlayOneShot(GameManager.Inst.SoundManager.GetClip("CruiseCancel"));
	}
}

