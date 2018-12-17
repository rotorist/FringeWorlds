using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Gun : Weapon
{
	public string ProjectilePrefab;
	public Damage Damage;
	public float ProjectileSpeed;

	public AIGunType AIGunType;

	private float _coolDownTimer;
	private bool _isCooledDown;
	private bool _isAmmoRequired;

	void Update()
	{
		if(!_isCooledDown)
		{
			_coolDownTimer += Time.deltaTime;
			if(_coolDownTimer >= 1 / this.FireRate)
			{
				_isCooledDown = true;
			}
		}
	}

	public override void Initialize (InvItemData itemData, string ammoID)
	{
		this.WeaponItem = itemData.Item;
		this.Class = itemData.Item.GetIntAttribute("Weapon Class");
		this.FireRate = itemData.Item.GetFloatAttribute("Fire Rate");
		this.Range = itemData.Item.GetFloatAttribute("Range");
		ProjectileSpeed = itemData.Item.GetFloatAttribute("Velocity");
		this.PowerConsumption = itemData.Item.GetFloatAttribute("Power Consumption");
		this.FiringSound = itemData.Item.GetStringAttribute("Firing Sound");
		this.RotationType = (WeaponRotationType)Enum.Parse(typeof(WeaponRotationType), itemData.Item.GetStringAttribute("Rotation Type"));
		ProjectilePrefab = itemData.Item.GetStringAttribute("Projectile ID");
		this.AmmoType = itemData.Item.GetStringAttribute("Ammo Type");
		this.AmmoID = ammoID;
		if(this.AmmoType != "")
		{
			_isAmmoRequired = true;
		}


	}

	public override void Rebuild ()
	{
		base.Rebuild();
		_isCooledDown = true;
	}



	public override void Fire()
	{
		if(_isCooledDown && ParentShip.WeaponCapacitor.IsPowerAvailable(PowerConsumption))
		{
			//TO DO: if gun requires ammo need to load ammo item from ammo bay
			GameObject o = null;
			Projectile projectile  = null;
			Item ammoItem = null;

			if(!_isAmmoRequired)
			{
				o = GameObject.Instantiate(Resources.Load(ProjectilePrefab)) as GameObject;
				projectile = o.GetComponent<Projectile>();
			}
			else
			{
				ammoItem = ParentShip.Storage.TakeAmmo(AmmoID, 1, this.AmmoType);
				if(ammoItem != null)
				{
					//just in case the ammo type has changed during take ammo, we always assign the item's id to ammoid
					AmmoID = ammoItem.ID;
					o = GameObject.Instantiate(Resources.Load(ProjectilePrefab)) as GameObject;
					projectile = o.GetComponent<Projectile>();
				}
				else
				{
					return;
				}

			}

			Damage = new Damage();
			Damage.DamageType = (DamageType)Enum.Parse(typeof(DamageType), this.WeaponItem.GetStringAttribute("Damage Type"));
			Damage.ShieldAmount = this.WeaponItem.GetFloatAttribute("Shield Damage");
			Damage.HullAmount = this.WeaponItem.GetFloatAttribute("Hull Damage");

			projectile.DamageMultiplier = ParentShip.WeaponPowerAlloc;
			projectile.Damage = Damage;
			if(ammoItem != null)
			{
				projectile.AdditionalShieldDamage = ammoItem.GetFloatAttribute("Additional Shield Damage");
				projectile.AdditionalHullDamage = ammoItem.GetFloatAttribute("Additional Hull Damage");
			}

			projectile.transform.position = Barrel.transform.position + Barrel.transform.forward * 2;
			Vector3 target = Barrel.transform.position + Barrel.transform.forward * 100;
			projectile.transform.LookAt(target);
			projectile.Fire(projectile.transform.forward * 50 + ParentShip.RB.velocity, Range, this.ParentShip);

			_isCooledDown = false;
			_coolDownTimer = 0;

			if(ParentShip == GameManager.Inst.PlayerControl.PlayerShip)
			{
				ParentShip.WeaponCapacitor.Consume(PowerConsumption);
			}

			if(Audio != null)
			{
				Audio.PlayOneShot(GameManager.Inst.SoundManager.GetClip(this.FiringSound));
			}
		}
	}

}

public enum AIGunType
{
	ForShield = 1,
	ForHull = 2,
	ForBoth = 3,
}

