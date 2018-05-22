using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTDockAtNextNode : BTLeaf
{
	private DockSessionBase _currentSession;
	private float _waitDistance;
	private int _dockingStage;
	private Vector3 _dockStart;

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
			if(Vector3.Distance(MyAI.MyParty.Location, MyAI.MyParty.NextNode.Location) > 40)
			{
				MyAI.Whiteboard.Parameters["Destination"] = MyAI.MyParty.NextNode.Location;
				Debug.Log("BTDockAtNextNode: running next node " + MyAI.MyParty.NextNode.ID);
				return BTResult.Running;
			}



			if(_dockingStage == 0 && _currentSession == null)
			{
				//need to decide if we want to dock here. find the next node after the tradelane, if it's 
				//a trade lane and is this tradelane's neighbor then dock. if not, return fail
				Tradelane currentLane = GameManager.Inst.WorldManager.CurrentSystem.GetTradelaneByID(MyAI.MyParty.NextNode.ID);
				NavNode nextNextNode = GameManager.Inst.NPCManager.MacroAI.FindNextNavNode(MyAI.MyParty.NextNode, MyAI.MyParty.DestNode);
				if(nextNextNode.NavNodeType == NavNodeType.Tradelane)
				{
					if(nextNextNode.ID == currentLane.NeighborAID)
					{

					}
					else if(nextNextNode.ID == currentLane.NeighborBID)
					{

					}
					else
					{
						return Exit(BTResult.Fail);
					}
				}
				else
				{
					return Exit(BTResult.Fail);
				}
			}



			if(MyAI.MyShip.IsInPortal)
			{
				if(Vector3.Distance(MyAI.MyParty.Location, MyAI.MyParty.NextNode.Location) < 10)
				{
					MyAI.MyParty.PrevNode = MyAI.MyParty.NextNode;
					Debug.LogError("BTDockAtNextNode tradelane: Successful");
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
				DockRequestResult result = currentLane.Dock(MyAI.MyShip, out _currentSession);
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
			Debug.Log("Trying to dock at jump gate");
			JumpGate jg = (JumpGate)GameManager.Inst.WorldManager.CurrentSystem.GetStationByID(MyAI.MyParty.NextNode.ID);
			if(_waitDistance == 0)
			{
				_waitDistance = UnityEngine.Random.Range(25f, 40f);
			}
			_dockStart = jg.DockingTrigger.transform.position + jg.DockingTrigger.transform.up * 20;

			//if too far away from station then go to
			if(Vector3.Distance(MyAI.MyParty.Location, MyAI.MyParty.NextNode.Location) > _waitDistance)
			{
				MyAI.Whiteboard.Parameters["Destination"] = _dockStart;
				Debug.Log("BTDockAtNextNode: running " + _waitDistance);
				return BTResult.Running;
			}

			MyAI.MyParty.PrevNode = MyAI.MyParty.NextNode;


			if(!jg.IsGateActive && !jg.IsPortalReady)
			{
				DockSessionBase session;
				jg.Dock(MyAI.MyShip, out session);

			}

			if(jg.IsGateActive && jg.IsPortalReady)
			{
				//first go to docking trigger Y + 10 and then towards docking trigger


				if(_dockingStage <= 0)
				{
					MyAI.Whiteboard.Parameters["Destination"] = _dockStart;
					if(Vector3.Distance(MyAI.MyShip.transform.position, _dockStart) <= 5)
					{
						_dockingStage = 1;
					}
					Debug.Log("Going to dockstart");
				}
				else if(_dockingStage == 1)
				{
					Debug.Log("Tring to stop at dock start " + MyAI.MyShip.RB.velocity.magnitude);
					if(MyAI.MyShip.RB.velocity.magnitude > 0.1f)
					{
						MyAI.Whiteboard.Parameters["Destination"] = Vector3.zero;
					}
					else
					{
						_dockingStage = 2;
					}
				}
				else if(_dockingStage >= 2)
				{
					MyAI.Whiteboard.Parameters["IgnoreAvoidance"] = true;
					MyAI.Whiteboard.Parameters["Destination"] = jg.DockingTrigger.transform.position;
					Debug.Log("Going to dock trigger");
				}
				//GameObject.Find("Sphere").transform.position = (Vector3)MyAI.Whiteboard.Parameters["Destination"];
				Debug.Log("BTDockAtNextNode: running");
				return BTResult.Running;
			}
			else
			{
				if(Vector3.Distance(MyAI.MyShip.transform.position, _dockStart) <= 5)
				{
					MyAI.Whiteboard.Parameters["Destination"] = Vector3.zero;
				}
				else
				{
					MyAI.Whiteboard.Parameters["Destination"] = _dockStart;
				}
				Debug.Log("BTDOckAtNextNode: waiting for jumpgate to start");
				return BTResult.Running;
			}

		}


		return Exit(BTResult.Fail);
	}

	public override BTResult Exit (BTResult result)
	{
		Debug.Log("BTDockAtNextNode: " + result);
		_currentSession = null;
		_waitDistance = 0;
		_dockingStage = 0;
		_dockStart = Vector3.zero;
		return result;
	}
}
