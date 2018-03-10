using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterShield : ShieldBase
{
	public float FrontCapacity;
	public float FrontAmount;
	public float RearCapacity;
	public float RearAmount;
	public float RechargeRate;


	public GameObject CurrentShieldFlash;

	private float _shieldFlashTimer;

	public override float GetShieldPercentage ()
	{
		return (FrontAmount + RearAmount) / TotalCapacity;
	}

	public override Damage ProcessDamage (Damage damage)
	{
		Damage processedDamage = new Damage();
		processedDamage.DamageType = damage.DamageType;
		processedDamage.HitLocation = damage.HitLocation;

		//determine whether it hit front or back
		float shieldFill = 1;
		bool isFront = true;
		float hitAngle = Vector3.Angle((damage.HitLocation - transform.position), transform.forward);
		if(hitAngle <= 90)
		{
			shieldFill = FrontAmount / FrontCapacity;
		}
		else
		{
			isFront = false;
			shieldFill = RearAmount / RearCapacity;
		}

		if(shieldFill > 0.2f)
		{
			if(ParentShip != GameManager.Inst.PlayerControl.PlayerShip)
			{
				//load a shield flash
				if(CurrentShieldFlash == null)
				{
					CurrentShieldFlash = GameObject.Instantiate(Resources.Load("ShieldFlash")) as GameObject;
				}

				CurrentShieldFlash.transform.parent = transform;
				CurrentShieldFlash.transform.localPosition = Vector3.zero;
				CurrentShieldFlash.transform.localScale = new Vector3(1, 1, 1);
				CurrentShieldFlash.transform.LookAt(damage.HitLocation);
				_shieldFlashTimer = 0;
			}

			//load shield hit mark
			GameObject hitMark = GameObject.Instantiate(Resources.Load("ShieldHitMark1")) as GameObject;
			hitMark.transform.position = damage.HitLocation;
			Quaternion lookRotation = Quaternion.LookRotation(damage.HitLocation - transform.position);
			hitMark.transform.rotation = lookRotation;
			hitMark.transform.parent = transform;


		}

		//get multiplier
		float multiplier = StaticUtility.GetShieldDamageMultiplier(this.Tech, damage.DamageType); 

		if(isFront)
		{
			FrontAmount = Mathf.Clamp(FrontAmount - damage.ShieldAmount * multiplier, 0, FrontCapacity);
		}
		else
		{
			RearAmount = Mathf.Clamp(RearAmount - damage.ShieldAmount * multiplier, 0, RearCapacity);
		}

		processedDamage.ShieldAmount = 0;
		processedDamage.HullAmount = Mathf.Clamp01(GameManager.Inst.Constants.ShieldProtectionCurve.Evaluate(shieldFill)) * damage.HullAmount;

		return processedDamage;
	}
	
	// Update is called once per frame
	void Update () 
	{
		_shieldFlashTimer += Time.deltaTime;
		if(_shieldFlashTimer > 0.1f)
		{
			_shieldFlashTimer = 0;
			if(CurrentShieldFlash != null)
			{
				GameObject.Destroy(CurrentShieldFlash.gameObject);
			}
			CurrentShieldFlash = null;
		}

		if(FrontAmount < FrontCapacity)
		{
			float recharge = RechargeRate;
			if(RearAmount < RearCapacity)
			{
				recharge = RechargeRate / 2f;
			}

			FrontAmount = Mathf.Clamp(FrontAmount + recharge * Time.deltaTime, 0, FrontCapacity);

		}

		if(RearAmount < RearCapacity)
		{
			float recharge = RechargeRate;
			if(FrontAmount < FrontCapacity)
			{
				recharge = RechargeRate / 2f;
			}

			RearAmount = Mathf.Clamp(RearAmount + recharge * Time.deltaTime, 0, RearCapacity);
		}
	}
}
