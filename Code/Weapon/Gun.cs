using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Weapon
{
	public string ProjectilePrefab;

	public AIGunType AIGunType;

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
		base.Rebuild();
		_isCooledDown = true;
	}



	public override void Fire()
	{
		if(_isCooledDown && ParentShip.WeaponCapacitor.IsPowerAvailable(PowerConsumption))
		{
			GameObject o = GameObject.Instantiate(Resources.Load(ProjectilePrefab)) as GameObject;
			Projectile projectile = o.GetComponent<Projectile>();
			projectile.DamageMultiplier = ParentShip.WeaponPowerAlloc;
			projectile.Damage = new Damage();
			projectile.Damage.DamageType = DamageType.Photon;
			projectile.Damage.ShieldAmount = 20;
			projectile.Damage.HullAmount = 30;

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
				Audio.PlayOneShot(GameManager.Inst.SoundManager.GetClip("Shot1"));
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

