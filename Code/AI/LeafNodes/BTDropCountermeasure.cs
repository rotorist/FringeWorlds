using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTDropCountermeasure : BTLeaf
{


	public override void Initialize ()
	{
		
	}

	public override BTResult Process ()
	{
		
		//check if missile is present and is close
		GameObject closeMissile = null;
		foreach(GameObject missile in MyAI.MyShip.IncomingMissiles)
		{
			if(missile != null)
			{
				if(Vector3.Distance(MyAI.MyShip.transform.position, missile.transform.position) < 30)
				{
					closeMissile = missile;
				}
			}
		}


		GameObject currentCM = MyAI.MyShip.CurrentCountermeasure;

		if(currentCM == null && closeMissile != null && closeMissile.GetComponent<Missile>().Stage == MissileStage.Chasing)
		{
			foreach(Defensive d in MyAI.MyShip.MyReference.Defensives)
			{
				if(d.Type == DefensiveType.Countermeasure)
				{
					CMDispenser dispenser = (CMDispenser)d;
					dispenser.DropCountermeasure();
					return Exit(BTResult.Success);
				}
			}
		}
		else
		{
			return Exit(BTResult.Fail);
		}

		return Exit(BTResult.Success);

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
