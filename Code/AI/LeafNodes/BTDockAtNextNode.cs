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
		//Debug.Log("Initializing dock at next node");
		_currentSession = null;
	}

	public override BTResult Process ()
	{
		
		if(MyParty == null)
		{
			return Exit(BTResult.Fail);
		}

		if(MyParty.IsPlayerParty && !GameManager.Inst.PlayerControl.IsAutopilot)
		{
			Debug.Log("autopilot disabled!");
			return Exit(BTResult.Fail);
		}

		bool isFollowerDock = false;
		if(Parameters.Count > 0 && Parameters[0] == "LeaderDockedStation")
		{
			isFollowerDock = true;
		}

		if(!isFollowerDock)
		{
			if(MyParty.NextNode == null)
			{
				//Debug.Log("isdocked " + MyAI.IsDocked + " my party null? " + (MyParty == null) + " next node null? " + (MyParty.NextNode == null));
				return Exit(BTResult.Fail);
			}
		}

		if(isFollowerDock || MyParty.NextNode.NavNodeType == NavNodeType.Station)
		{
			NavNode dockNode;
			if(isFollowerDock)
			{
				//Debug.Log(MyParty.DockedStationID + " I am " + MyAI.MyShip.name);
				dockNode = GameManager.Inst.WorldManager.AllNavNodes[MyParty.DockedStationID];
				if(MyParty.SpawnedShipsLeader.DockedStationID == "")
				{
					return Exit(BTResult.Fail);
				}
			}
			else
			{
				dockNode = MyParty.NextNode;
				//if not destination then don't dock, just get close enough and move on to next node in GoTo
				if(MyParty.CurrentTask.TravelDestNodeID != dockNode.ID)
				{
					return Exit(BTResult.Fail);
				}
			}



			//GameObject.Find("Sphere").transform.position = (Vector3)MyAI.Whiteboard.Parameters["Destination"];
			//if too far away from station then go to node
			if(Vector3.Distance(MyAI.MyShip.transform.position, dockNode.Location.RealPos) > 40)
			{
				_dockingStage = 0;
				MyAI.Whiteboard.Parameters["Destination"] = dockNode.Location.RealPos;
				//Debug.Log("BTDockAtNextNode: running, going towards station position " + MyParty.NextNode.Location.RealPos);
				return BTResult.Running;
			}
			else if(_dockingStage == 0)
			{
				_dockingStage = 1;

			}
				

			if(MyAI.MyShip.DockedStationID == dockNode.ID)
			{
				MyAI.MyShip.Hide();
				MyParty.DockedStationID = dockNode.ID;

				//Debug.Log("Finished docking, " + MyParty.DockedStationID);

				if(MyAI.MyShip == MyParty.SpawnedShipsLeader)
				{
					bool allDocked = true;
					foreach(ShipBase member in MyAI.MyParty.SpawnedShips)
					{
						if(member != MyAI.MyShip && member.DockedStationID == "")
						{
							allDocked = false;
						}
					}
					if(allDocked)
					{
						Debug.LogError("Everyone has docked! " + MyParty.DockedStationID);
						return Exit(BTResult.Success);
					}
					else
					{
						//Debug.Log("Dock at station running");
						return BTResult.Running;
					}
				}
				else
				{
					return Exit(BTResult.Success);
				}
			}

			if(_currentSession == null)
			{
				DockRequestResult result = GameManager.Inst.WorldManager.CurrentSystem.GetStationByID(dockNode.ID).Dock(MyAI.MyShip, out _currentSession);
				if(result == DockRequestResult.Busy)
				{
					//Debug.Log("Dock at station running");
					MyAI.Whiteboard.Parameters["Destination"] = Vector3.zero;
					return BTResult.Running;
				}
				else if(result == DockRequestResult.Deny)
				{
					return Exit(BTResult.Fail);
				}
				else
				{
					//Debug.Log("BTDockAtNextNode: running");
					return BTResult.Running;
				}
			}
			else
			{
				//find dock start
				DockingSession session = (DockingSession)_currentSession;
				_dockStart = session.GetDockEnterTarget();

				if(!MyAI.MyShip.IsInPortal)
				{
					if(Vector3.Distance(MyAI.MyShip.transform.position, _dockStart) >= 3)
					{
						//fly towards docking target
						MyAI.Whiteboard.Parameters["Destination"] = _dockStart;
					}
					else
					{
						MyAI.Whiteboard.Parameters["Destination"] = Vector3.zero;

					}
				}
				//Debug.Log("BTDockAtNextNode: running");
				return BTResult.Running;
			}

		}
		else if(MyParty.NextNode.NavNodeType == NavNodeType.Tradelane)
		{
			if(MyParty.DestNode.ID == MyParty.NextNode.ID && MyParty.CurrentTLSession == null)
			{
				return Exit(BTResult.Fail);
			}

			if(MyAI.IsDocked)
			{
				return Exit(BTResult.Fail);
			}


			//if already in tradelane then do a midway dock on prevNode tradelane
			if(Vector3.Distance(MyAI.MyShip.transform.position, GameManager.Inst.PlayerControl.PlayerShip.transform.position) > 500 && MyParty.PrevNode != null 
				&& MyParty.PrevNode.NavNodeType == NavNodeType.Tradelane && MyParty.NextNode.NavNodeType == NavNodeType.Tradelane && !MyAI.MyShip.IsInPortal && MyParty.CurrentTLSession == null)
			{
				int direction = 0;
				TradelaneData prevTL = (TradelaneData)MyParty.PrevNode;
				TradelaneData nextTL = (TradelaneData)MyParty.NextNode;
				if(prevTL.NeighborAID == nextTL.ID)
				{
					direction = -1;
				}
				else if(prevTL.NeighborBID == nextTL.ID)
				{
					direction = 1;
				}

				if(direction != 0)
				{
					Tradelane currentLane = GameManager.Inst.WorldManager.CurrentSystem.GetTradelaneByID(prevTL.ID);
					DockSessionBase s;
					DockRequestResult result = currentLane.MidwayDock(MyAI.MyShip, out s, direction);

					if(result == DockRequestResult.Busy)
					{
						MyAI.Whiteboard.Parameters["Destination"] = Vector3.zero;
						//Debug.Log("Dock at station running");
						return BTResult.Running;
					}
					else if(result == DockRequestResult.Deny)
					{
						return Exit(BTResult.Fail);
					}
					else
					{
						//Debug.Log("BTDockAtNextNode: midway dock granted, running");
						MyParty.CurrentTLSession = (TLTransitSession)s;
						_dockingStage = 2;
						return BTResult.Running;
					}
				}

			}


			if(MyParty.CurrentTLSession == null)
			{
				//if too far away from station then go to
				if(Vector3.Distance(MyAI.MyShip.transform.position, MyParty.NextNode.Location.RealPos) > 40)
				{
					_dockingStage = 0;
					MyAI.Whiteboard.Parameters["Destination"] = MyParty.NextNode.Location.RealPos;
					//Debug.Log("BTDockAtNextNode: running next node " + MyParty.NextNode.ID);
					return BTResult.Running;
				}
				else if(_dockingStage == 0)
				{
					_dockingStage = 1;
				}

				Tradelane currentLane = GameManager.Inst.WorldManager.CurrentSystem.GetTradelaneByID(MyParty.NextNode.ID);

				if(_dockStart == Vector3.zero)
				{
					//need to decide if we want to dock here. find the next node after the tradelane, if it's 
					//a trade lane and is this tradelane's neighbor then dock. if not, return fail

					NavNode nextNextNode = MyParty.NextNextNode;

					if(nextNextNode != null && nextNextNode.NavNodeType == NavNodeType.Tradelane)
					{
						if(nextNextNode.ID == currentLane.NeighborAID)
						{
							_dockStart = currentLane.TriggerA.transform.position - (currentLane.TriggerA.transform.forward 
								+ new Vector3(UnityEngine.Random.Range(-0.3f, 0.3f), UnityEngine.Random.Range(-0.3f, 0.3f), UnityEngine.Random.Range(-0.3f, 0.3f))).normalized * UnityEngine.Random.Range(10f, 30f);
						}
						else if(nextNextNode.ID == currentLane.NeighborBID)
						{
							_dockStart = currentLane.TriggerB.transform.position - (currentLane.TriggerB.transform.forward 
								+ new Vector3(UnityEngine.Random.Range(-0.3f, 0.3f), UnityEngine.Random.Range(-0.3f, 0.3f), UnityEngine.Random.Range(-0.3f, 0.3f))).normalized * UnityEngine.Random.Range(10f, 30f);
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

				//GameObject.Find("Sphere").transform.position = _dockStart;

				if(_dockingStage == 1)
				{
					if(Vector3.Distance(_dockStart, MyAI.MyShip.transform.position) > 5)
					{
						MyAI.Whiteboard.Parameters["Destination"] = _dockStart;
						//Debug.Log("BTDockAtNextNode: going to dock start " + Vector3.Distance(_dockStart, MyAI.MyShip.transform.position));
						return BTResult.Running;
					}
					else
					{
						MyAI.Whiteboard.Parameters["Destination"] = Vector3.zero;
						_dockingStage = 2;
					}
				}
			}

			MyAI.Whiteboard.Parameters["IgnoreAvoidance"] = true;

			if(MyParty.CurrentTLSession != null && MyAI.MyShip.IsInPortal)
			{
				float distToNextNode = Vector3.Distance(MyAI.MyShip.transform.position, MyParty.NextNode.Location.RealPos);
				//Debug.Log(distToNextNode + " next node " + MyParty.NextNode.ID);
				if(distToNextNode < 10)
				{
					
					if(MyParty.NextNextNode == null || MyParty.NextNextNode.NavNodeType != NavNodeType.Tradelane)
					{
						MyParty.CurrentTLSession.Stage = TLSessionStage.Cancelling;
						//Debug.LogError("BTDockAtNextNode tradelane: cancel sent");
					}
					MyParty.PrevNode = MyParty.NextNode;
					Debug.LogError("BTDockAtNextNode tradelane: Successful " + MyParty.NextNode.ID);
					return Exit(BTResult.Success);
				}
				else
				{
					//Debug.Log("Dock at station running");
					return BTResult.Running;
				}
			}

			if(MyParty.CurrentTLSession == null)
			{
				Tradelane currentLane = GameManager.Inst.WorldManager.CurrentSystem.GetTradelaneByID(MyParty.NextNode.ID);
				DockSessionBase s;
				DockRequestResult result = currentLane.Dock(MyAI.MyShip, out s);

				if(result == DockRequestResult.Busy)
				{
					//Debug.Log("tradelane is busy");
					MyAI.Whiteboard.Parameters["Destination"] = Vector3.zero;
					return BTResult.Running;
				}
				else if(result == DockRequestResult.Deny)
				{
					return Exit(BTResult.Fail);
				}
				else
				{
					//Debug.Log("BTDockAtNextNode: dock request granted, running");
					MyParty.CurrentTLSession = (TLTransitSession)s;
					return BTResult.Running;
				}
			}
			else
			{
				if(!MyAI.MyShip.IsInPortal)
				{
					if(MyAI.MyShip.RB.velocity.magnitude < 0.1f || _dockingStage == 3)
					{
						Tradelane currentLane = GameManager.Inst.WorldManager.CurrentSystem.GetTradelaneByID(MyParty.NextNode.ID);
						if(currentLane.GetTLSessionDockingStage(MyParty.CurrentTLSession) >= 2)
						{
							//fly towards docking trigger
							Vector3 dockingTrigger = MyParty.CurrentTLSession.CurrentTrigger.transform.position;
							MyAI.Whiteboard.Parameters["Destination"] = dockingTrigger;
							_dockingStage = 3;
						}
					}
					else if(_dockingStage == 2)
					{
						MyAI.Whiteboard.Parameters["Destination"] = Vector3.zero;
					}
				}
				//Debug.Log("BTDockAtNextNode: running");
				return BTResult.Running;
			}
		}
		else if(MyParty.NextNode.NavNodeType == NavNodeType.JumpGate)
		{
			//if destination is in same system then don't dock
			if(MyParty.CurrentTask.TravelDestSystemID == GameManager.Inst.WorldManager.CurrentSystem.ID || MyAI.IsDocked)
			{
				return Exit(BTResult.Fail);
			}

			//Debug.Log("Trying to dock at jump gate");
			JumpGate jg = (JumpGate)GameManager.Inst.WorldManager.CurrentSystem.GetStationByID(MyParty.NextNode.ID);
			if(_waitDistance == 0)
			{
				_waitDistance = UnityEngine.Random.Range(25f, 40f);
			}
			_dockStart = jg.DockingTrigger.transform.position + jg.DockingTrigger.transform.up * 20;

			//if too far away from station then go to
			if(Vector3.Distance(MyAI.MyShip.transform.position, MyParty.NextNode.Location.RealPos) > _waitDistance)
			{
				MyAI.Whiteboard.Parameters["Destination"] = _dockStart;
				//Debug.Log("BTDockAtNextNode: running " + _waitDistance);
				return BTResult.Running;
			}

			MyParty.PrevNode = MyParty.NextNode;


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
					//Debug.Log("Going to dockstart");
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
					//Debug.Log("Going to dock trigger");
				}
				//GameObject.Find("Sphere").transform.position = (Vector3)MyAI.Whiteboard.Parameters["Destination"];
				//Debug.Log("BTDockAtNextNode: running");
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
				//Debug.Log("BTDOckAtNextNode: waiting for jumpgate to start");
				return BTResult.Running;
			}

		}


		return Exit(BTResult.Fail);
	}

	public override BTResult Exit (BTResult result)
	{
		//Debug.LogError("BTDockAtNextNode: " + result + " " + MyAI.MyShip.name);
		MyAI.Whiteboard.Parameters["IgnoreAvoidance"] = false;
		_currentSession = null;
		_waitDistance = 0;
		_dockingStage = 0;
		_dockStart = Vector3.zero;
		return result;
	}
}
