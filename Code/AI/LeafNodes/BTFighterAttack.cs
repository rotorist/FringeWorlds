using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTFighterAttack : BTLeaf
{


	public override void Initialize ()
	{

	}

	public override BTResult Process ()
	{
		
		ShipBase target = (ShipBase)MyAI.Whiteboard.Parameters[Parameters[0]];
		if(target != null)
		{
			/*
			Fighter fighter = (Fighter)MyAI.MyShip;
			fighter.LeftGun.GetComponent<Weapon>().Fire();
			fighter.RightGun.GetComponent<Weapon>().Fire();
			*/


			if(Vector3.Distance(MyAI.MyShip.transform.position, target.transform.position) < (float)MyAI.Whiteboard.Parameters["FiringRange"])
			{
				foreach(WeaponJoint joint in MyAI.MyShip.MyReference.WeaponJoints)
				{
					if(joint.MountedWeapon != null)
					{
						joint.MountedWeapon.Fire();
					}
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
