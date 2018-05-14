using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTCheck : BTLeaf
{
	
	public string Action;

	public override void Initialize ()
	{
		
	}

	public override BTResult Process ()
	{
		Debug.Log("Checking " + Action);
		switch(Action)
		{
		case "IsNearFriendlyTarget":
			{
				ShipBase target = (ShipBase)MyAI.Whiteboard.Parameters[Parameters[0]];
				if(target != null)
				{
					float dist = Vector3.Distance(MyAI.MyShip.transform.position, target.transform.position);
					if(dist <= (float)MyAI.Whiteboard.Parameters["FriendlyFollowDist"])
					{
						return BTResult.Success;
					}
					else
					{
						return BTResult.Fail;
					}
				}
				else
				{
					return BTResult.Fail;
				}
			}
			break;
		case "IsEnemyOnMyTail":
			{
				Debug.Log("Checking is enemy on my tail, param0 " + Parameters[0]);
				ShipBase target = (ShipBase)MyAI.Whiteboard.Parameters[Parameters[0]];
				if(target != null)
				{
					if(StaticUtility.CheckFighterOnTail(target.transform.position, MyAI.MyShip.transform.position, target.transform.forward, MyAI.MyShip.transform.forward))
					{
						Debug.Log("Got enemy on my tail!");
						return BTResult.Success;
					}
					else
					{
						Debug.Log("No enemy on my tail.");
						return BTResult.Fail;
					}
				}
				else
				{
					Debug.Log("No target enemy found!");
				}
			}
			break;
		case "IsThereDanger":
			{
				ShipBase target = (ShipBase)MyAI.Whiteboard.Parameters["TargetEnemy"];
				if(target != null)
				{
					float dist = Vector3.Distance(MyAI.MyShip.transform.position, target.transform.position);
					if(dist < (float)MyAI.Whiteboard.Parameters["MinEnemyRange"])
					{
						Debug.Log("found danger");
						return BTResult.Success;
					}
				}
				Debug.Log("no danger");
				return BTResult.Fail;
			}
			break;
		case "IsTargetInAttackFov":
			{
				ShipBase target = (ShipBase)MyAI.Whiteboard.Parameters[Parameters[0]];
				if(target != null)
				{
					//if angle between lead-target-me-los and my forward is less than 20 degrees
					Vector3 leadTarget = StaticUtility.FirstOrderIntercept(MyAI.transform.position, MyAI.RB.velocity, 30, target.transform.position, target.RB.velocity);
					Vector3 los = leadTarget - MyAI.MyShip.transform.position;
					float angleLosMyForward = Vector3.Angle(los, MyAI.MyShip.transform.forward);
					if(angleLosMyForward < 20)
					{
						return BTResult.Success;
					}
					else
					{
						return BTResult.Fail;
					}
				}
			}
			break;
		case "IsTargetInFiringRange":
			{
				ShipBase target = (ShipBase)MyAI.Whiteboard.Parameters[Parameters[0]];
				if(target != null)
				{
					if(Vector3.Distance(MyAI.MyShip.transform.position, target.transform.position) < (float)MyAI.Whiteboard.Parameters["FiringRange"])
					{
						return BTResult.Success;
					}
					else
					{
						return BTResult.Fail;
					}
				}
			}
			break;
		case "IsEnemyDetected":
			{
				if(MyAI.Whiteboard.Parameters["TargetEnemy"] == null)
				{
					return BTResult.Fail;
				}
				else
				{
					return BTResult.Success;
				}
			}
			break;
		case "IspartyLeader":
			{
				if(MyAI.MyParty.SpawnedShipsLeader == MyAI.MyShip)
				{
					return BTResult.Success;
				}
				else
				{
					return BTResult.Fail;
				}
			}
			break;
		case "HasDestination":
			{
				if(MyAI.MyParty == null || MyAI.MyParty.CurrentTask == null)
				{
					return BTResult.Fail;
				}

				if(MyAI.MyParty.CurrentTask.TaskType == MacroAITaskType.Travel)
				{
					return BTResult.Success;
				}
				else
				{
					return BTResult.Fail;
				}
			}
			break;
		case "HasReachedDestination":
			{
				if(!MyAI.MyParty.CurrentTask.IsDestAStation)
				{
					if(Vector3.Distance(MyAI.MyShip.transform.position, MyAI.MyParty.CurrentTask.TravelDestCoord) < 5)
					{
						return BTResult.Success;
					}
					else
					{
						return BTResult.Fail;
					}
				}
				else
				{
					if(MyAI.MyParty.DockedStationID == MyAI.MyParty.CurrentTask.TravelDestNodeID)
					{
						return BTResult.Success;
					}
					else
					{
						return BTResult.Fail;
					}
				}
			}
			break;
		}



		return BTResult.Success;
	}

	public override BTResult Exit (BTResult result)
	{
		return result;
	}
}
