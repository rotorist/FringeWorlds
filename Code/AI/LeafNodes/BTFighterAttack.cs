using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTFighterAttack : BTLeaf
{
	private float _lastProcessTime;
	private float _missileTimer;
	private float _missileDelay;

	public override void Initialize ()
	{
		_missileDelay = UnityEngine.Random.Range(3f, 6f);
		_missileTimer = 0;
		_lastProcessTime = Time.time;
	}

	public override BTResult Process ()
	{
		
		ShipBase target = (ShipBase)MyAI.Whiteboard.Parameters[Parameters[0]];
		if(target != null)
		{


			if(Vector3.Distance(MyAI.MyShip.transform.position, target.transform.position) < (float)MyAI.Whiteboard.Parameters["FiringRange"])
			{
				/*
				foreach(WeaponJoint joint in MyAI.MyShip.MyReference.WeaponJoints)
				{
					if(joint.MountedWeapon != null)
					{
						joint.MountedWeapon.Fire();
					}
				}
				*/

				MyAI.WeaponControl.FireGuns(AIGunSelection.All);

				//shoot missile and wait
				if(_missileTimer < _missileDelay)
				{
					float deltaTime = Time.time - _lastProcessTime;
					_missileTimer += deltaTime;
					_lastProcessTime = Time.time;
				}
				else
				{
					MyAI.WeaponControl.FireLaunchers(AILauncherSelection.Random);
					_missileDelay = UnityEngine.Random.Range(3f, 6f);
					_missileTimer = 0;
					_lastProcessTime = Time.time;
				}

				//Debug.Log("Processing Fighter Attack");
				return Running();
			}
			else
			{
				//Debug.Log("Target too far!");
				return Exit(BTResult.Fail);
			}
		}
		else
		{
			return Exit(BTResult.Fail);
		}

	}

	public override BTResult Exit (BTResult result)
	{
		return result;
	}

	public override BTResult Running ()
	{
		MyAI.RunningNodeHist.UniquePush("Fighter Attack");
		return BTResult.Running;
	}
}
