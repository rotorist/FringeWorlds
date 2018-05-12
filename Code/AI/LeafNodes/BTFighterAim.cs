using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTFighterAim : BTLeaf
{


	public override void Initialize ()
	{
		
	}

	public override BTResult Process ()
	{
		Debug.Log("Processing Fighter Aim");
		ShipBase target = (ShipBase)MyAI.Whiteboard.Parameters[Parameters[0]];
		if(target != null)
		{
			MyAI.Whiteboard.Parameters["AimTarget"] = target;
			Vector3 aimPoint = (Vector3)MyAI.Whiteboard.Parameters["AimPoint"];
			if(aimPoint == Vector3.zero)
			{
				return Exit(BTResult.Running);
			}
			if(aimPoint != Vector3.zero && Vector3.Angle(aimPoint - MyAI.MyShip.transform.position, MyAI.MyShip.transform.forward) > 25)
			{
				
				return BTResult.Fail;
			}

		}

		return Exit(BTResult.Fail);
	}

	public override BTResult Exit (BTResult result)
	{
		if(result == BTResult.Fail)
		{
			MyAI.Whiteboard.Parameters["AimTarget"] = null;
			MyAI.Whiteboard.Parameters["AimPoint"] = Vector3.zero;
		}
		return result;
	}
}
