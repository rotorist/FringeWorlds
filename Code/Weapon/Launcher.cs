using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Launcher : Weapon 
{
	public AILauncherType AILauncherType;
	//public string ProjectilePrefab;


	private float _coolDownTimer;
	private bool _isCooledDown;
	private bool _isNPC;

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
		this.PowerConsumption = itemData.Item.GetFloatAttribute("Power Consumption");
		this.FiringSound = itemData.Item.GetStringAttribute("Firing Sound");
		this.RotationType = (WeaponRotationType)Enum.Parse(typeof(WeaponRotationType), itemData.Item.GetStringAttribute("Rotation Type"));
		this.AmmoID = ammoID;
		this.AmmoType = itemData.Item.GetStringAttribute("Ammo Type");

		if(ParentShip.MyAI.MyFaction != null && ParentShip.MyAI.MyFaction.ID == "player")
		{
			_isNPC = false;
		}
		else
		{
			_isNPC = true;
		}

	}

	public override void Rebuild ()
	{
		base.Rebuild();
		_isCooledDown = true;
	}



	public override void Fire()
	{
		if(_isCooledDown)
		{
			//check if storage has it
			GameObject o = null;
			Missile missile  = null;
			Item ammoItem = null;

			ammoItem = ParentShip.Storage.TakeAmmo(AmmoID, 1, this.AmmoType, _isNPC);
			if(ammoItem != null)
			{
				//just in case the ammo type has changed during take ammo, we always assign the item's id to ammoid
				AmmoID = ammoItem.ID;
				o = GameObject.Instantiate(Resources.Load(ammoItem.GetStringAttribute("Weapon Prefab ID"))) as GameObject;
				missile = o.GetComponent<Missile>();
				Damage damage = new Damage();
				damage.DamageType = (DamageType)Enum.Parse(typeof(DamageType), ammoItem.GetStringAttribute("Damage Type"));
				damage.ShieldAmount = ammoItem.GetFloatAttribute("Shield Damage");
				damage.HullAmount = ammoItem.GetFloatAttribute("Hull Damage");
				missile.Damage = damage;
			}
			else
			{
				return;
			}



			if(missile != null)
			{

				Audio.PlayOneShot(GameManager.Inst.SoundManager.GetClip("MissileFire"));

				ShipBase target = null;
				if(ParentShip == GameManager.Inst.PlayerControl.PlayerShip)
				{
					if(GameManager.Inst.PlayerControl.TargetShip != null)
					{
						target = GameManager.Inst.PlayerControl.TargetShip;
						GameManager.Inst.PlayerControl.TargetShip.IncomingMissiles.Add(missile.gameObject);
					}
				}
				else
				{
					ShipBase currentTarget = (ShipBase)ParentShip.MyAI.Whiteboard.Parameters["TargetEnemy"];
					if(currentTarget != null)
					{
						target = currentTarget;
						currentTarget.IncomingMissiles.Add(missile.gameObject);
					}
				}

				missile.Initialize(target, ammoItem);
				missile.Attacker = this.ParentShip;
				missile.transform.position = Barrel.transform.position + Barrel.transform.forward * 2f;
				Vector3 lookTarget = Barrel.transform.position + Barrel.transform.forward * 100;
				missile.transform.LookAt(lookTarget);
				missile.Fire(this.ParentShip, missile.transform.forward * 9f + ParentShip.RB.velocity);

				_isCooledDown = false;
				_coolDownTimer = 0;
			}
		}
	}


}

public enum AILauncherType
{
	ForShield = 1,
	ForHull = 2,
	ForBoth = 3,
	EngineDisruptor = 10,
}