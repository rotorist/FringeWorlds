using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Weapon
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

			GameObject o = GameObject.Instantiate(Resources.Load("Projectile")) as GameObject;
			Projectile projectile = o.GetComponent<Projectile>();
			projectile.Damage = new Damage();
			projectile.Damage.DamageType = DamageType.Photon;
			projectile.Damage.ShieldAmount = 25;
			projectile.Damage.HullAmount = 30;

			projectile.transform.position = Barrel.transform.position + Barrel.transform.forward * 2;
			Vector3 target = Barrel.transform.position + Barrel.transform.forward * 100;
			projectile.transform.LookAt(target);
			projectile.Fire(projectile.transform.forward * 30 + ParentShip.RB.velocity, Range, this.ParentShip);

			_isCooledDown = false;
			_coolDownTimer = 0;
		}
	}

}
