using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterShield : ShieldBase
{

	private float _shieldFlashTimer;
	private float _fadeAlpha;

	public override void Initialize ()
	{
		base.Initialize ();
		Amount = TotalCapacity;
	}

	public override float GetShieldPercentage ()
	{
		return Amount / TotalCapacity;
	}

	public override Damage ProcessDamage (Damage damage)
	{
		Damage processedDamage = new Damage();
		processedDamage.DamageType = damage.DamageType;
		processedDamage.HitLocation = damage.HitLocation;


		/*
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
		*/
		float shieldFill = Amount / TotalCapacity;

		if(shieldFill > 0.2f)
		{
			if(ParentShip != GameManager.Inst.PlayerControl.PlayerShip)
			{
				//show shield flash
				this.MyRenderer = this.GetComponent<MeshRenderer>();
				this.MyRenderer.enabled = true;
				_shieldFlashTimer = 0;
			}
			else
			{
				_fadeAlpha = Mathf.Clamp01(_fadeAlpha + 0.1f);
			}

			//load shield hit mark
			GameObject hitMark = GameObject.Instantiate(Resources.Load("ShieldHitMark" + this.GetShieldHitEffectNumber())) as GameObject;
			hitMark.transform.position = damage.HitLocation;
			Quaternion lookRotation = Quaternion.LookRotation(damage.HitLocation - transform.position);
			hitMark.transform.rotation = lookRotation;
			hitMark.transform.parent = transform;


		}

		//get multiplier
		float multiplier = StaticUtility.GetShieldDamageMultiplier(this.Tech, damage.DamageType); 
		float powerMultiplier = 1 / ParentShip.ShieldPowerAlloc;
		Amount = Mathf.Clamp(Amount - damage.ShieldAmount * multiplier * powerMultiplier, 0, TotalCapacity);

		//float totalAmount = FrontAmount + RearAmount;
		//float frontPortion = FrontCapacity / TotalCapacity;
		//totalAmount = Mathf.Clamp(totalAmount - damage.ShieldAmount * multiplier, 0, TotalCapacity);
		//FrontAmount = totalAmount * frontPortion;
		//RearAmount = totalAmount * (1 - frontPortion);
		/*
		if(isFront)
		{
			FrontAmount = Mathf.Clamp(FrontAmount - damage.ShieldAmount * multiplier, 0, FrontCapacity);
		}
		else
		{
			RearAmount = Mathf.Clamp(RearAmount - damage.ShieldAmount * multiplier, 0, RearCapacity);
		}
		*/

		float damageEval = 1;
		if(damage.DamageType == DamageType.Shock)
		{
			damageEval = 1f;
		}
		else
		{
			if(ParentShip != GameManager.Inst.PlayerControl.PlayerShip)
			{
				damageEval = Mathf.Clamp01(GameManager.Inst.Constants.ShieldProtectionCurve.Evaluate(shieldFill));
			}
			else
			{
				GameManager.Inst.CameraShaker.TriggerScreenShake(0.07f, 0.015f, false);
				damageEval = Mathf.Clamp01(GameManager.Inst.Constants.PlayerShieldProtectionCurve.Evaluate(shieldFill));
			}
		}

		processedDamage.ShieldAmount = 0;
		processedDamage.HullAmount = damageEval * damage.HullAmount;

		return processedDamage;
	}

	// Update is called once per frame
	void Update () 
	{
		_shieldFlashTimer += Time.deltaTime;

		/*
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
		*/

		if(Amount < TotalCapacity)
		{
			float recharge = RechargeRate;
			Amount = Mathf.Clamp(Amount + recharge * ParentShip.ShieldPowerAlloc * Time.deltaTime, 0, TotalCapacity);
		}

		if(ParentShip == GameManager.Inst.PlayerControl.PlayerShip)
		{
			base.UpdateShieldFading(_fadeAlpha);
			_fadeAlpha = Mathf.Lerp(_fadeAlpha, 0, Time.deltaTime * 1);
		}
		else
		{
			if(this.MyRenderer != null && _shieldFlashTimer > 0.2f)
			{
				_shieldFlashTimer = 0;
				this.MyRenderer.enabled = false;
			}
		}
	}
}
