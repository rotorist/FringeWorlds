using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTDockAtNextNode : BTLeaf
{
	private DockSessionBase _currentSession;


	public override void Initialize ()
	{
		_currentSession = null;
	}

	public override BTResult Process ()
	{
		if(MyAI.MyParty == null || MyAI.MyParty.NextNode == null)
		{
			return Exit(BTResult.Fail);
		}

		if(MyAI.MyParty.NextNode.NavNodeType == NavNodeType.Station)
		{
			//if not destination then don't dock, just get close enough and move on to next node in GoTo
			if(MyAI.MyParty.CurrentTask.TravelDestNodeID != MyAI.MyParty.NextNode.ID)
			{
				return Exit(BTResult.Fail);
			}


			//if too far away from station then go to node
			if(Vector3.Distance(MyAI.MyParty.Location, MyAI.MyParty.NextNode.Location) > 40)
			{
				MyAI.Whiteboard.Parameters["Destination"] = MyAI.MyParty.NextNode.Location;
				Debug.Log("BTDockAtNextNode: running");
				return BTResult.Running;
			}




			if(MyAI.MyShip.IsDocked)
			{
				return Exit(BTResult.Success);
			}

			if(_currentSession == null)
			{
				DockRequestResult result = GameManager.Inst.WorldManager.CurrentSystem.GetStationByID(MyAI.MyParty.NextNode.ID).Dock(MyAI.MyShip, out _currentSession);
				if(result == DockRequestResult.Busy)
				{
					return BTResult.Running;
				}
				else if(result == DockRequestResult.Deny)
				{
					return Exit(BTResult.Fail);
				}
				else
				{
					Debug.Log("BTDockAtNextNode: running");
					return BTResult.Running;
				}
			}
			else
			{
				if(!MyAI.MyShip.IsInPortal)
				{
					//fly towards docking target
					DockingSession session = (DockingSession)_currentSession;
					Vector3 dockTarget = session.GetDockEnterTarget();
					MyAI.Whiteboard.Parameters["Destination"] = dockTarget;


				}
				Debug.Log("BTDockAtNextNode: running");
				return BTResult.Running;
			}

		}
		else if(MyAI.MyParty.NextNode.NavNodeType == NavNodeType.Tradelane)
		{
			//if too far away from station then go to
			if(Vector3.Distance(MyAI.MyParty.Location, MyAI.MyParty.NextNode.Location) > 20)
			{
				MyAI.Whiteboard.Parameters["Destination"] = MyAI.MyParty.NextNode.Location;
				Debug.Log("BTDockAtNextNode: running");
				return BTResult.Running;
			}

			if(MyAI.MyShip.IsInPortal)
			{
				if(Vector3.Distance(MyAI.MyParty.Location, MyAI.MyParty.NextNode.Location) < 10)
				{
					MyAI.MyParty.PrevNode = MyAI.MyParty.NextNode;
					return Exit(BTResult.Success);
				}
				else
				{
					Debug.Log("BTDockAtNextNode: running");
					return BTResult.Running;
				}
			}

			if(_currentSession == null)
			{
				DockRequestResult result = GameManager.Inst.WorldManager.CurrentSystem.GetTradelaneByID(MyAI.MyParty.NextNode.ID).Dock(MyAI.MyShip, out _currentSession);
				if(result == DockRequestResult.Busy)
				{
					return BTResult.Running;
				}
				else if(result == DockRequestResult.Deny)
				{
					return Exit(BTResult.Fail);
				}
				else
				{
					Debug.Log("BTDockAtNextNode: running");
					return BTResult.Running;
				}
			}
			else
			{
				if(!MyAI.MyShip.IsInPortal)
				{
					//fly towards docking trigger
					TLTransitSession session = (TLTransitSession)_currentSession;
					Vector3 dockingTrigger = session.CurrentTrigger.transform.position;
					MyAI.Whiteboard.Parameters["Destination"] = dockingTrigger;
				}
				Debug.Log("BTDockAtNextNode: running");
				return BTResult.Running;
			}
		}
		else if(MyAI.MyParty.NextNode.NavNodeType == NavNodeType.JumpGate)
		{
			//if too far away from station then go to
			if(Vector3.Distance(MyAI.MyParty.Location, MyAI.MyParty.NextNode.Location) > 30)
			{
				MyAI.Whiteboard.Parameters["Destination"] = MyAI.MyParty.NextNode.Location;
				Debug.Log("BTDockAtNextNode: running");
				return BTResult.Running;
			}

			MyAI.MyParty.PrevNode = MyAI.MyParty.NextNode;

			JumpGate jg = (JumpGate)GameManager.Inst.WorldManager.CurrentSystem.GetStationByID(MyAI.MyParty.NextNode.ID);
			if(!jg.IsGateActive && !jg.IsPortalReady)
			{
				DockSessionBase session;
				jg.Dock(MyAI.MyShip, out session);
			}
			else if(jg.IsGateActive && jg.IsPortalReady)
			{
				//first go to docking trigger Y + 10 and then towards docking trigger
				if(Vector3.Angle((MyAI.MyShip.transform.position - jg.transform.position), jg.DockingTrigger.transform.up) > 10 && Vector3.Distance(MyAI.MyShip.transform.position, jg.transform.position) > 5)
				{
					MyAI.Whiteboard.Parameters["Destination"] = jg.DockingTrigger.transform.position + jg.DockingTrigger.transform.up * (UnityEngine.Random.Range(5f, 15f));
				}
				else
				{
					MyAI.Whiteboard.Parameters["Destination"] = jg.DockingTrigger.transform.position;
				}

				Debug.Log("BTDockAtNextNode: running");
				return BTResult.Running;
			}

		}


		return Exit(BTResult.Success);
	}

	public override BTResult Exit (BTResult result)
	{
		Debug.Log("BTDockAtNextNode: " + result);
		_currentSession = null;
		return result;
	}
}
