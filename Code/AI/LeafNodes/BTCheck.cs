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
		
		BTResult result = BTResult.Success;
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
						result = BTResult.Success;
					}
					else
					{
						result = BTResult.Fail;
					}
				}
				else
				{
					result = BTResult.Fail;
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
						result = BTResult.Success;
					}
					else
					{
						result = BTResult.Fail;
					}
				}
				else
				{
					result = BTResult.Fail;
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
						result = BTResult.Success;
					}
				}
				result = BTResult.Fail;
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
						result = BTResult.Success;
					}
					else
					{
						result = BTResult.Fail;
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
						result = BTResult.Success;
					}
					else
					{
						result = BTResult.Fail;
					}
				}
			}
			break;
		case "IsEnemyDetected":
			{
				if(MyAI.Whiteboard.Parameters["TargetEnemy"] == null)
				{
					result = BTResult.Fail;
				}
				else
				{
					result = BTResult.Success;
				}
			}
			break;
		case "MAIIsNearEnemyParty":
			{
				return BTResult.Fail;
			}
			break;
		case "IspartyLeader":
			{
				if(MyParty.SpawnedShipsLeader == MyAI.MyShip)
				{
					result = BTResult.Success;
				}
				else
				{
					result = BTResult.Fail;
				}
			}
			break;
		case "HasDestination":
			{
				if(MyParty == null || MyParty.CurrentTask == null)
				{
					result = BTResult.Fail;
				}

				if(MyParty.CurrentTask.TaskType == MacroAITaskType.Travel)
				{
					result = BTResult.Success;
				}
				else
				{
					result = BTResult.Fail;
				}
			}
			break;
		case "HasReachedDestination":
			{
				result = HasReachedDestination();
			}
			break;
		case "MAIHasReachedDestination":
			{
				result = MAIHasReachedDestination();
			}
			break;
		case "HasNextNode":
			{
				if(MyParty == null || MyParty.NextNode == null)
				{
					result = BTResult.Fail;
				}
				else
				{
					result = BTResult.Success;
				}
			}
			break;
		case  "IsTaskCompleted":
			{
				if(MyParty == null)
				{
					result = BTResult.Fail;
				}
				else if(MyParty.CurrentTask == null || MyParty.CurrentTask.TaskType == MacroAITaskType.None)
				{
					result = BTResult.Success;
				}
				else
				{
					if(MyParty.CurrentTask.TaskType == MacroAITaskType.Stay)
					{
						if(MyParty.WaitTimer >= MyParty.CurrentTask.StayDuration)
						{
							result = BTResult.Success;
						}
						else
						{
							result = BTResult.Fail;
						}
					}
					else 
					{
						result = HasReachedDestination();
					}
				}

			}
			break;
		case "MAIIsTaskCompleted":
			{
				
				if(MyParty == null)
				{
					result = BTResult.Fail;
				}
				else if(MyParty.CurrentTask == null || MyParty.CurrentTask.TaskType == MacroAITaskType.None)
				{
					result = BTResult.Success;
				}
				else
				{
					if(MyParty.CurrentTask.TaskType == MacroAITaskType.Stay)
					{
						if(MyParty.WaitTimer >= MyParty.CurrentTask.StayDuration)
						{
							result = BTResult.Success;
						}
						else
						{
							result = BTResult.Fail;
						}
					}
					else 
					{
						result = MAIHasReachedDestination();
					}
				}
			}
			break;
		}

		//Debug.Log("Checking " + Action + " result " + result);

		return result;
	}

	public override BTResult Exit (BTResult result)
	{
		return result;
	}

	private BTResult HasReachedDestination()
	{
		if(!MyParty.CurrentTask.IsDestAStation)
		{
			if(Vector3.Distance(MyAI.MyShip.transform.position, MyParty.CurrentTask.TravelDestCoord.RealPos) < 30)
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
			if(MyAI.MyShip.DockedStationID == MyParty.CurrentTask.TravelDestNodeID)
			{
				MyParty.HasReachedDestNode = true;
				MyParty.NextTwoNodes.Clear();
				return BTResult.Success;
			}
			else
			{
				return BTResult.Fail;
			}
		}
	}

	private BTResult MAIHasReachedDestination()
	{
		Vector3 destination;
		if(!MyParty.CurrentTask.IsDestAStation)
		{
			destination = MyParty.CurrentTask.TravelDestCoord.RealPos;	

			if(Vector3.Distance(MyParty.Location.RealPos, destination) < 30)
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
			destination = GameManager.Inst.WorldManager.AllNavNodes[MyParty.CurrentTask.TravelDestNodeID].Location.RealPos;
			if(MyParty.DockedStationID == MyParty.CurrentTask.TravelDestNodeID)
			{
				MyParty.NextTwoNodes.Clear();
				MyParty.HasReachedDestNode = true;
				return BTResult.Success;
			}
			else
			{
				return BTResult.Fail;
			}
		}


	}
}
