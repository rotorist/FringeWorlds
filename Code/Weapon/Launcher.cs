using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : Weapon 
{
	
	public string ProjectileName;

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

			GameObject o = GameObject.Instantiate(Resources.Load("Missile")) as GameObject;
			Missile missile = o.GetComponent<Missile>();

			Rigidbody target = null;
			if(ParentShip == GameManager.Inst.PlayerControl.PlayerShip)
			{
				if(GameManager.Inst.PlayerControl.TargetShip != null)
				{
					target = GameManager.Inst.PlayerControl.TargetShip.RB;
				}
			}
			else
			{
				ShipBase currentTarget = (ShipBase)ParentShip.MyAI.Whiteboard.Parameters["TargetEnemy"];
				if(currentTarget != null)
				{
					target = currentTarget.RB;
				}
			}
			missile.Initialize(target);

			missile.transform.position = Barrel.transform.position + Barrel.transform.forward * 2;
			Vector3 lookTarget = Barrel.transform.position + Barrel.transform.forward * 100;
			missile.transform.LookAt(lookTarget);
			missile.Fire(this.ParentShip, missile.transform.forward * 20 + ParentShip.RB.velocity);

			_isCooledDown = false;
			_coolDownTimer = 0;
		}
	}


}
