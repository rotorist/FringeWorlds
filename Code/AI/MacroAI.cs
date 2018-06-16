using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MacroAI
{


	private int _lastUsedPartyNumber;

	public void Initialize()
	{
		
	}

	public void GenerateTestParty()
	{
		MacroAIParty party = new MacroAIParty();
		party.FactionID = "otu";
		party.SpawnedShips = new List<ShipBase>();

		List<string> keyList = new List<string>(GameManager.Inst.WorldManager.AllSystems.Keys);
		StarSystemData currentSystem = GameManager.Inst.WorldManager.AllSystems["carolina_system"];
		party.CurrentSystemID = currentSystem.ID;
		StationData currentStation = currentSystem.GetStationByID("wilmington_station");
		party.DockedStationID = "wilmington_station";
		Transform origin = GameObject.Find("Origin").transform;
		party.Location = new RelLoc(origin.position, currentStation.Location.RealPos, origin);//Vector3.zero;//new Vector3(-100, 0.1f, 34);//
		party.PartyNumber = _lastUsedPartyNumber + 1;
		_lastUsedPartyNumber = party.PartyNumber;


		MacroAITask task = AssignMacroAITask(MacroAITaskType.None, party);

		party.IsInTradelane = false;
		//party.DestinationCoord = GameManager.Inst.WorldManager.AllNavNodes["cambridge_station"].Location;
		party.MoveSpeed = 10f;
		party.NextTwoNodes = new List<NavNode>();
		party.PrevNode = null;//CreateTempNode(party.Location, "tempstart", GameManager.Inst.WorldManager.AllSystems[party.CurrentSystemID]);

		LoadPartyTreeset(party);

		GameManager.Inst.NPCManager.AllParties.Add(party);
	}

	public void GenerateParties()
	{
		MacroAIParty party = new MacroAIParty();
		party.FactionID = "otu";
		party.SpawnedShips = new List<ShipBase>();

		List<string> keyList = new List<string>(GameManager.Inst.WorldManager.AllSystems.Keys);
		StarSystemData currentSystem = GameManager.Inst.WorldManager.AllSystems[keyList[UnityEngine.Random.Range(0, keyList.Count)]];
		party.CurrentSystemID = currentSystem.ID;
		StationData currentStation = currentSystem.Stations[UnityEngine.Random.Range(0, currentSystem.Stations.Count)];
		party.DockedStationID = currentStation.ID;
		if(currentSystem.ID == GameManager.Inst.WorldManager.CurrentSystem.ID)
		{
			party.Location = new RelLoc(currentSystem.OriginPosition, currentStation.Location.RealPos, GameObject.Find("Origin").transform);
		}
		else
		{
			party.Location = new RelLoc(currentSystem.OriginPosition, currentStation.Location.RealPos, null);
		}
		party.PartyNumber = _lastUsedPartyNumber + 1;
		_lastUsedPartyNumber = party.PartyNumber;


		MacroAITask task = AssignMacroAITask(MacroAITaskType.None, party);


		//party.DestinationStationID = "cambridge_station";
		//party.DestinationSystemID = "new_england_system";
		//party.IsDestinationAStation = false;
		party.IsInTradelane = false;
		//party.DestinationCoord = GameManager.Inst.WorldManager.AllNavNodes["cambridge_station"].Location;
		party.MoveSpeed = 10f;
		party.NextTwoNodes = new List<NavNode>();
		party.PrevNode = null;//CreateTempNode(party.Location, "tempstart", GameManager.Inst.WorldManager.AllSystems[party.CurrentSystemID]);

		LoadPartyTreeset(party);

		GameManager.Inst.NPCManager.AllParties.Add(party);
	}

	public void PerFrameUpdate()
	{
		//each frame update 1 party
		foreach(MacroAIParty party in GameManager.Inst.NPCManager.AllParties)
		{

			//if party is in the same system as player, and there's no leader ship spawned, then spawn a ship, and make it follow party location
			if(party.CurrentSystemID == GameManager.Inst.WorldManager.CurrentSystem.ID)
			{	
				
				if(party.NextNode != null)
					Debug.Log("next node : " + party.NextNode.ID);



				if(party.SpawnedShipsLeader == null)
				{
					
					party.SpawnedShipsLeader = GameManager.Inst.NPCManager.SpawnAIShip("LightFighter", ShipType.Fighter, party.FactionID, party);
					party.SpawnedShipsLeader.transform.position = party.Location.RealPos;
					party.SpawnedShips.Add(party.SpawnedShipsLeader);
					AI ai = party.SpawnedShipsLeader.GetComponent<AI>();
					ai.MyParty = party;
					ai.Deactivate();
					if(party.DockedStationID != "")
					{
						Debug.LogError("docked!");
						party.SpawnedShipsLeader.Hide();
						ai.IsDocked = true;
						ai.MyShip.DockedStationID = party.DockedStationID;
					}
					GameManager.Inst.NPCManager.AddExistingShip(ai.MyShip);
				}

				if(!party.ShouldEnableAI)
				{
					party.SpawnedShipsLeader.transform.position = party.Location.RealPos;
				}
				else
				{
					party.Location.RealPos = party.SpawnedShipsLeader.transform.position;
				}


				//if one ship near player then turn on AI for all party
				//if all party are too far from player then turn off ai for all party
				party.ShouldEnableAI = false;
				foreach(ShipBase ship in party.SpawnedShips)
				{
					if(Vector3.Distance(ship.transform.position, GameManager.Inst.PlayerControl.PlayerShip.transform.position) < 1000)
					{
						party.ShouldEnableAI = true;
					}
				}

				bool needRepath = false;

				foreach(ShipBase ship in party.SpawnedShips)
				{
					AI ai = ship.GetComponent<AI>();
					if(party.ShouldEnableAI)
					{
						if(!ai.IsActive)
						{
							Debug.Log("Activating AI");
							ai.Activate();
						}
					}
					else 
					{
						if(ai.IsActive)
						{
							Debug.Log("Deactivating AI");
							ai.Deactivate();
							//needRepath = true;
						}
					}
				}

				if(needRepath)
				{
					party.NextTwoNodes = new List<NavNode>();
					party.PrevNode = null;
				}
				
			}
			else
			{
				if(party.SpawnedShipsLeader != null)
				{
					DespawnParty(party);
				}
			}

			if(party.CurrentTask == null)
			{
				Debug.Log("no task? " + (party.CurrentTask == null));
				continue;
			}

			float deltaTime = Time.time - party.LastUpdateTime;
			party.WaitTimer += deltaTime;

			if(party.Destination != Vector3.zero)
			{
				float speed = party.IsInTradelane ? 100 : party.MoveSpeed;
				party.Location.RealPos = party.Location.RealPos + (party.Destination - party.Location.RealPos).normalized * deltaTime * speed;
			}

			if(!party.ShouldEnableAI)
			{
				party.TreeSet["MAIBaseBehavior"].Run();
			}

			/***
			if(party.CurrentTask.TaskType == MacroAITaskType.Travel && !party.ShouldEnableAI)
			{

				//if there's no prev node or next node, get nearest node to party current location and set nextnode to that
				//if there's no prev node but there is next node, check if is near next node, if not fly towards it
				//if close to next node, set prevnode as nextnode, and set next node as null
				//if there is prev node but no next node, calculate next node 
				//if there is next node and not near it, fly towards it
				//if there is next node and prev node, and is near next node, calculate using next node as start
				//now if reached destnode, fly towards destination coord
				if(!party.CurrentTask.IsDestAStation)
				{
					if(Vector3.Distance(party.Location.RealPos, party.CurrentTask.TravelDestCoord.RealPos) < Vector3.Distance(party.Location.RealPos, party.DestNode.Location.RealPos))
					{
						party.HasReachedDestNode = true;
					}
				}

				if(!party.CurrentTask.IsDestAStation && party.HasReachedDestNode)
				{

					//move towards dest coord
					float deltaTime = Time.time - party.LastUpdateTime;
					party.Location.RealPos = party.Location.RealPos + (party.CurrentTask.TravelDestCoord.RealPos - party.Location.RealPos).normalized * deltaTime * party.MoveSpeed;
					if(Vector3.Distance(party.Location.RealPos, party.CurrentTask.TravelDestCoord.RealPos) < 5)
					{
						//done travelling!
						Debug.Log("Party " + party.PartyNumber + " Done travelling to " + party.CurrentTask.TravelDestCoord.ToString());
						MacroAITask task = AssignMacroAITask(party.CurrentTask.TaskType, party);
						Debug.Log("Party " + party.PartyNumber + " new task: " + task.TaskType);
					}


				}
				else if(party.PrevNode == null && party.NextNode == null)
				{
					List<NavNode> nextTwoNodes = new List<NavNode>();
					NavNode nextNode = GameManager.Inst.NPCManager.MacroAI.GetClosestNodeToLocation(party.Location.RealPos, GameManager.Inst.WorldManager.AllSystems[party.CurrentSystemID]);
					List<NavNode> nextNextTwoNodes = GameManager.Inst.NPCManager.MacroAI.FindNextNavNode(nextNode, party.DestNode);
					nextTwoNodes.Add(nextNode);
					if(nextNextTwoNodes.Count > 0)
					{
						nextTwoNodes.Add(nextNextTwoNodes[0]);
					}
					party.NextTwoNodes = nextTwoNodes;
				}
				else if(party.PrevNode != null && party.NextNode == null)
				{
					party.NextTwoNodes = FindNextNavNode(party.PrevNode, party.DestNode);
				}
				else if(party.NextNode != null)
				{
					
					if(Vector3.Distance(party.Location.RealPos, party.NextNode.Location.RealPos) > 5f)
					{
						//move towards the next node
						float deltaTime = Time.time - party.LastUpdateTime;
						float speed = party.IsInTradelane ? 100 : party.MoveSpeed;
						party.Location.RealPos = party.Location.RealPos + (party.NextNode.Location.RealPos - party.Location.RealPos).normalized * deltaTime * speed;
						foreach(ShipBase ship in party.SpawnedShips)
						{
							if(ship.MyAI != null)
							{
								ship.DockedStationID = "";
								ship.MyAI.IsDocked = false;
							}
						}
						Debug.Log("moving party to " + party.NextNode.ID + " at speed " + speed);
						//GameObject.Find("Sphere").transform.position = party.Location;
						//Debug.Log("Travelling to next node: " + party.NextNode.ID + " Final dest " + (party.CurrentTask.IsDestAStation ? party.CurrentTask.TravelDestNodeID : party.CurrentTask.TravelDestCoord.ToString()));
					}
					else
					{
						if(party.NextNode == party.DestNode)
						{
							party.HasReachedDestNode = true;

							if(party.CurrentTask.IsDestAStation)
							{
								//done travelling!
								foreach(ShipBase ship in party.SpawnedShips)
								{
									if(ship.MyAI != null)
									{
										ship.MyAI.IsDocked = true;
										ship.DockedStationID = party.NextNode.ID;
									}
								}
								Debug.Log("Party " + party.PartyNumber + " Done travelling to " + party.CurrentTask.TravelDestNodeID);
								MacroAITask task = AssignMacroAITask(party.CurrentTask.TaskType, party);
								Debug.Log("Party " + party.PartyNumber + " new task: " + task.TaskType);
							}

						}
						else
						{
							foreach(ShipBase ship in party.SpawnedShips)
							{
								if(ship.MyAI != null)
								{
									ship.DockedStationID = "";
									ship.MyAI.IsDocked = false;
								}
							}
							if(party.NextNode.NavNodeType == NavNodeType.JumpGate)
							{
								//change party system
								JumpGateData jg = (JumpGateData)party.NextNode;
								JumpGateData otherJG = (JumpGateData)GameManager.Inst.WorldManager.AllNavNodes[jg.ExitGateID];
								party.CurrentSystemID = jg.TargetSystem;
								party.Location.RealPos = otherJG.Location.RealPos;
								party.PrevNode = otherJG;
								party.NextTwoNodes = new List<NavNode>();
							}
							else
							{

								party.PrevNode = party.NextNode;
								party.NextTwoNodes = FindNextNavNode(party.PrevNode, party.DestNode);
								Debug.Log("next node null? " + (party.NextNode == null));
								if(party.NextNode != null && party.NextNode.NavNodeType == NavNodeType.Tradelane && party.PrevNode.NavNodeType == NavNodeType.Tradelane)
								{
									party.IsInTradelane = true;
								}
								else
								{
									party.IsInTradelane = false;
								}

							}
						}
					}
				}
					
			}
			else if(party.CurrentTask.TaskType == MacroAITaskType.Stay)
			{
				//handle stay
				float deltaTime = Time.time - party.LastUpdateTime;
				party.WaitTimer += deltaTime;
				//Debug.Log("Waiting... " + (party.CurrentTask.StayDuration - party.WaitTimer));
				if(party.WaitTimer >= party.CurrentTask.StayDuration)
				{
					MacroAITask task = AssignMacroAITask(party.CurrentTask.TaskType, party);
					Debug.Log("Party " + party.PartyNumber + " Done waiting... new task: " + task.TaskType);
				}
			}
			*/
			party.LastUpdateTime = Time.time;
		}
	}

	public List<NavNode> FindNextNavNode(NavNode start, NavNode dest)
	{
		//is dest system same as current system? if not, need to find the next system towards the destination system, 
		//then find the jumpgate to the next system, 
		//finally find the next node towards the jumpgate
		if(start.SystemID == dest.SystemID)
		{

			return PathFind(start, dest);
		}
		else
		{
			StarSystemData nextSystem = (StarSystemData)(PathFind(GameManager.Inst.WorldManager.AllSystems[start.SystemID], GameManager.Inst.WorldManager.AllSystems[dest.SystemID])[0]);
			StarSystemData currentSystem = GameManager.Inst.WorldManager.AllSystems[start.SystemID];
			NavNode destJumpGate = null;
			foreach(JumpGateData jg in currentSystem.JumpGates)
			{
				if(jg.TargetSystem == nextSystem.ID)
				{
					destJumpGate = (NavNode)jg;
				}
			}
			Debug.Log("find next nav node");
			return PathFind(start, destJumpGate);
		}


	}

	public List<NavNode> PathFind(NavNode start, NavNode dest)
	{
		if(start == null || dest == null)
		{
			//Debug.Log("start is null? " + (start == null) + " dest is null? " + (dest == null) + " start==dest? " + (start == dest));
			return new List<NavNode>();
		}

		if(start == dest)
		{
			return new List<NavNode>(){dest};
		}

		//Debug.Log("Pathfinding start " + start.ID + start.Location + " -> " + dest.ID + dest.Location);
		List<NavNode> closedSet = new List<NavNode>();
		List<NavNode> openSet = new List<NavNode>();
		openSet.Add(start);


		LinkedList<NavNode> result = new LinkedList<NavNode>();
		LinkedListNode<NavNode> currentResultNode;

		Dictionary<NavNode,float> gScores = new Dictionary<NavNode, float>();
		Dictionary<NavNode,float> fScores = new Dictionary<NavNode, float>();

		List<NavNode> allNodes = null;

		if(start.NavNodeType == NavNodeType.System)
		{
			allNodes = new List<NavNode>(GameManager.Inst.WorldManager.GetAllSystemNavNodes());
		}
		else
		{
			allNodes = new List<NavNode>(GameManager.Inst.WorldManager.GetAllNavNodesInSystem(start.SystemID));
		}
		foreach(NavNode n in allNodes)
			
		{
			gScores.Add(n, Mathf.Infinity);
			fScores.Add(n, Mathf.Infinity);
		}

		gScores[start] = 0;
		fScores[start] = Vector3.Distance(start.Location.RealPos, dest.Location.RealPos);

		while(openSet.Count > 0)
		{
			NavNode bestNode = openSet[0];
			foreach(NavNode node in openSet)
			{
				if(fScores[node] < fScores[bestNode])
				{
					bestNode = node;
				}
			}

			if(bestNode == dest)
			{
				result.AddFirst(bestNode);
				break;
			}

			openSet.Remove(bestNode);
			closedSet.Add(bestNode);

			foreach(NavNode neighbor in bestNode.Neighbors)
			{
				if(closedSet.Contains(neighbor))
				{
					continue;
				}

				if(!openSet.Contains(neighbor))
				{
					openSet.Add(neighbor);
				}

				float tentativeGScore = gScores[bestNode] + Vector3.Distance(bestNode.Location.RealPos, neighbor.Location.RealPos);
				//Debug.Log("best node " + bestNode.name + " tentative g " + tentativeGScore + " neighbor g " + gScores[neighbor] + " " + neighbor.name);
				//Debug.Log("neighbor g is " + neighbor.ID);
				if(tentativeGScore >= gScores[neighbor])
				{
					continue;
				}

				if(!result.Contains(bestNode))
				{
					result.AddFirst(bestNode);
				}
				gScores[neighbor] = tentativeGScore;
				fScores[neighbor] = gScores[neighbor] + Vector3.Distance(neighbor.Location.RealPos, dest.Location.RealPos);

			}
		}


		currentResultNode = result.First;
		while(currentResultNode != null)
		{
			//Debug.Log("pathfinding result " + currentResultNode.Value.ID);
			currentResultNode = currentResultNode.Next;
		}

		List<NavNode> resultList = new List<NavNode>();
		resultList.Add(result.Last.Previous.Value);
		if(result.Last.Previous.Previous != null)
		{
			resultList.Add(result.Last.Previous.Previous.Value);
		}

		//Debug.Log("Going to return pathfinding result " + result.Last.Previous.Value.ID);
		//return result.Last.Previous.Value;
		return resultList;

	}


	public MacroAITask AssignMacroAITask(MacroAITaskType prevTaskType, MacroAIParty party)
	{
		MacroAITask task = new MacroAITask();

		if(prevTaskType == MacroAITaskType.None)
		{
			if(UnityEngine.Random.value > 0.1f)
			{
				prevTaskType = MacroAITaskType.Stay;
			}
			else
			{
				prevTaskType = MacroAITaskType.Travel;
			}
		}

		party.PrevNode = null;
		if(party.NextTwoNodes != null)
		{
			party.NextTwoNodes.Clear();
		}
		if(prevTaskType == MacroAITaskType.Travel)
		{

			task.TaskType = MacroAITaskType.Stay;
			task.StayDuration = UnityEngine.Random.Range(30f, 60f);
			party.WaitTimer = 0;
			Debug.Log("Party " + party.PartyNumber + " Task has been assigned to party: " + task.TaskType + " for " + task.StayDuration);

		}
		else if(prevTaskType == MacroAITaskType.Stay)
		{
			task.TaskType = MacroAITaskType.Travel;
			List<string> keyList = new List<string>(GameManager.Inst.WorldManager.AllSystems.Keys);
			if(Time.time < 5)
			{
				//StarSystemData destSystem = GameManager.Inst.WorldManager.AllSystems[keyList[UnityEngine.Random.Range(0, keyList.Count)]];
				StarSystemData destSystem = GameManager.Inst.WorldManager.AllSystems["washington_system"];

				task.TravelDestSystemID = destSystem.ID;
				//task.TravelDestNodeID = destSystem.Stations[UnityEngine.Random.Range(0, destSystem.Stations.Count)].ID;
				//task.TravelDestNodeID = "planet_colombia_landing";
				task.IsDestAStation = false;
				task.TravelDestCoord = new RelLoc(destSystem.OriginPosition, new Vector3(-28.3f, 5f, 418.8f), null);
			}
			else
			{
				StarSystemData destSystem = GameManager.Inst.WorldManager.AllSystems[keyList[UnityEngine.Random.Range(0, keyList.Count)]];
				//StarSystemData destSystem = GameManager.Inst.WorldManager.AllSystems["washington_system"];

				task.TravelDestSystemID = destSystem.ID;
				task.TravelDestNodeID = destSystem.Stations[UnityEngine.Random.Range(0, destSystem.Stations.Count)].ID;
				//task.TravelDestNodeID = "planet_colombia_landing";
				task.IsDestAStation = true;
				//task.TravelDestCoord = new RelLoc(destSystem.OriginPosition, new Vector3(-28.3f, 5f, 418.8f), null);
			}

			party.WaitTimer = 0;
			Debug.Log("Party " + party.PartyNumber + " Task has been assigned to party: " + task.TaskType + " to " + (task.IsDestAStation ? task.TravelDestNodeID : task.TravelDestCoord.ToString()));
		}

		party.CurrentTask = task;
		party.HasReachedDestNode = false;

		if(party.CurrentTask.TaskType == MacroAITaskType.Travel)
		{
			if(party.CurrentTask.IsDestAStation)
			{
				party.DestNode = GameManager.Inst.WorldManager.AllNavNodes[party.CurrentTask.TravelDestNodeID];
			}
			else
			{
				//party.DestNode = CreateTempNode(party.CurrentTask.TravelDestCoord, "tempdest", GameManager.Inst.WorldManager.AllSystems[party.CurrentTask.TravelDestSystemID]);
				party.DestNode = GetClosestNodeToLocation(party.CurrentTask.TravelDestCoord.RealPos, GameManager.Inst.WorldManager.AllSystems[party.CurrentTask.TravelDestSystemID]);
			}
		}



		return task;
	}

	public NavNode GetClosestNodeToLocation(Vector3 location, StarSystemData systemData)
	{
		//find the nearest navnode
		float lowDist = Mathf.Infinity;
		NavNode nearestNode = null;
		foreach(NavNode childNode in systemData.ChildNodes)
		{
			float dist = Vector3.Distance(childNode.Location.RealPos, location);
			if(dist < lowDist)
			{
				lowDist = dist;
				nearestNode = childNode;
			}
		}

		return nearestNode;
	}



	private void DespawnParty(MacroAIParty party)
	{
		GameManager.Inst.NPCManager.RemoveExistingShip(party.SpawnedShipsLeader);
		party.SpawnedShips.Remove(party.SpawnedShipsLeader);
		party.SpawnedShipsLeader.GetComponent<AI>().Deactivate();
		GameObject.Destroy(party.SpawnedShipsLeader.gameObject);
		party.SpawnedShipsLeader = null;
		party.ShouldEnableAI = false;
	}

	private void LoadPartyTreeset(MacroAIParty party)
	{
		party.TreeSet = new Dictionary<string, BehaviorTree>();
		party.TreeSet.Add("MAIBaseBehavior", GameManager.Inst.DBManager.XMLParserBT.LoadBehaviorTree("MAIBaseBehavior", null, party));
		party.TreeSet.Add("MAITravel", GameManager.Inst.DBManager.XMLParserBT.LoadBehaviorTree("MAITravel", null, party));
		party.TreeSet.Add("MAICombat", GameManager.Inst.DBManager.XMLParserBT.LoadBehaviorTree("MAICombat", null, party));
	}
}

public class MacroAIParty
{
	public Vector3 Destination;
	public int PartyNumber;
	public string FactionID;
	public RelLoc Location;
	public string DockedStationID;
	public string CurrentSystemID;
	public float MoveSpeed;

	public bool IsInTradelane;
	public TLTransitSession CurrentTLSession;
	public List<NavNode> NextTwoNodes;
	public NavNode NextNode 
	{ 
		get 
		{ 
			if(NextTwoNodes.Count > 0)
			{
				return NextTwoNodes[0]; 
			}
			else
			{
				return null;
			}
		} 
	}
	public NavNode NextNextNode 
	{ 
		get 
		{ 
			if(NextTwoNodes.Count > 1)
			{
				return NextTwoNodes[1]; 
			}
			else
			{
				return null;
			}
		} 
	}
	public NavNode PrevNode;
	public NavNode DestNode;
	public bool HasReachedDestNode;
	public float WaitTimer;
	public MacroAITask CurrentTask;

	public float LastUpdateTime;

	public GameObject TestSphere;
	public List<ShipBase> SpawnedShips;
	public ShipBase SpawnedShipsLeader;
	public bool ShouldEnableAI;

	public Dictionary<string,BehaviorTree> TreeSet;
}

public class MacroAITask
{
	public MacroAITaskType TaskType;
	public float StayDuration;
	public RelLoc TravelDestCoord;
	public string TravelDestSystemID;
	public string TravelDestNodeID;
	public bool IsDestAStation;
}

public enum MacroAITaskType
{
	None,
	Travel,
	Stay,
}