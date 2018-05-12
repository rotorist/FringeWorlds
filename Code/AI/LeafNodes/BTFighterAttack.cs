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
			foreach(WeaponJoint joint in MyAI.MyShip.MyReference.WeaponJoints)
			{
				joint.MountedWeapon.Fire();
			}

			if(Vector3.Distance(MyAI.MyShip.transform.position, target.transform.position) < (float)MyAI.Whiteboard.Parameters["FiringRange"])
			{
				//Debug.Log("Processing Fighter Attack");
				return BTResult.Running;
			}
			else
			{
				Debug.Log("Target too far!");
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
}
