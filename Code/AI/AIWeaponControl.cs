using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIWeaponControl
{
	public AI MyAI;
	public List<Gun> Guns;
	public List<Launcher> Launchers;

	private float _gunBurstTimer;
	private bool _isFiringGuns;
	private List<Gun> _firingGuns;

	public void Initialize(AI myAI)
	{
		MyAI = myAI;
		Guns = new List<Gun>();
		Launchers = new List<Launcher>();
		foreach(WeaponJoint joint in MyAI.MyShip.MyReference.WeaponJoints)
		{
			if(joint.MountedWeapon != null)
			{
				Gun gun = joint.MountedWeapon.GetComponent<Gun>();
				if(gun != null)
				{
					Guns.Add(gun);
				}
				else
				{
					Launcher launcher = joint.MountedWeapon.GetComponent<Launcher>();
					if(launcher != null)
					{
						Launchers.Add(launcher);
					}
				}
			}
		}

		_gunBurstTimer = 0;
		_isFiringGuns = false;
	}

	public void FireGuns(AIGunSelection selection)
	{
		if(!_isFiringGuns)
		{
			_isFiringGuns = true;
			_firingGuns = new List<Gun>();
			foreach(Gun gun in Guns)
			{
				if((int)gun.AIGunType == (int)selection || gun.AIGunType == AIGunType.ForBoth || selection == AIGunSelection.All)
				{
					_firingGuns.Add(gun);
				}
			}
			//if can't find guns according to requested selection then just fire all guns
			if(_firingGuns.Count <= 0)
			{
				_firingGuns = Guns;
			}

		}


	}

	public void FireLaunchers(AILauncherSelection selection)
	{
		if(Launchers.Count <= 0)
		{
			return;
		}

		if(selection == AILauncherSelection.Random)
		{
			Launchers[UnityEngine.Random.Range(0, Launchers.Count)].Fire();
			return;
		}

		bool isLauncherFound = false;
		foreach(Launcher launcher in Launchers)
		{
			if((int)launcher.AILauncherType == (int)selection || launcher.AILauncherType == AILauncherType.ForBoth)
			{
				launcher.Fire();
				isLauncherFound = true;
			}
		}

		if(!isLauncherFound)
		{
			Launchers[UnityEngine.Random.Range(0, Launchers.Count)].Fire();
		}
	}

	public void PerFrameUpdate()
	{
		if(_gunBurstTimer <= 1 && _firingGuns != null && _isFiringGuns)
		{
			_gunBurstTimer += Time.deltaTime;
			foreach(Gun gun in _firingGuns)
			{
				gun.Fire();
			}

		}
		else
		{
			_gunBurstTimer = 0;
			_isFiringGuns = false;
		}
	}
}

public enum AIGunSelection
{
	All = 0,
	ForShield = 1,
	ForHull = 2,
	ForBoth = 3,
}

public enum AILauncherSelection
{
	Random = 0,
	ForShield = 1,
	ForHull = 2,
	ForBoth = 3,
	EngineDisruptor = 10,
}