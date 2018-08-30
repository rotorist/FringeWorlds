using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigShipShield : ShieldBase
{


	private float _shieldFlashTimer;
	private float _fadeAlpha;
	private float _delayTimer;

	public override void Initialize ()
	{
		base.Initialize ();
		Amount = TotalCapacity;
		_delayTimer = RechargeDelay;
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
		Amount = Mathf.Clamp(Amount - damage.ShieldAmount * multiplier, 0, TotalCapacity);
		Debug.Log(multiplier + ", " + damage.ShieldAmount + ", " + Amount);

		float damageEval = 1;
		if(Amount > 0)
		{
			damageEval = 0;
		}
		else
		{
			damageEval = 1;
		}

		processedDamage.ShieldAmount = 0;
		processedDamage.HullAmount = damageEval * damage.HullAmount;

		return processedDamage;
	}

	// Update is called once per frame
	void Update () 
	{
		_shieldFlashTimer += Time.deltaTime;



		if(Amount < TotalCapacity)
		{
			if(_delayTimer < RechargeDelay)
			{
				_delayTimer += Time.deltaTime;
				Amount = 1;
			}
			else
			{
				if(Amount <= 0 && _delayTimer > RechargeDelay)
				{
					_delayTimer = 0;
				}
				else
				{
					Amount = Mathf.Clamp(Amount + RechargeRate * Time.deltaTime, 0, TotalCapacity);
				}
			}
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
