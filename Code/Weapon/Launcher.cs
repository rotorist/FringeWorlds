﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : Weapon 
{
	public AILauncherType AILauncherType;
	public string ProjectilePrefab;
	public string AmmoID;

	private float _coolDownTimer;
	private bool _isCooledDown;

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



	public override void Rebuild ()
	{

		_isCooledDown = true;
	}



	public override void Fire()
	{
		if(_isCooledDown)
		{
			//check if storage has it
			if(ParentShip.Storage.TakeAmmo(AmmoID, 1))
			{

				GameObject o = GameObject.Instantiate(Resources.Load(ProjectilePrefab)) as GameObject;
				Missile missile = o.GetComponent<Missile>();

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
				missile.Initialize(target);
				missile.Attacker = this.ParentShip;
				missile.transform.position = Barrel.transform.position + Barrel.transform.forward * 1f;
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