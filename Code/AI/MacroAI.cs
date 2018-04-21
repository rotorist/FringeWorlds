using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MacroAI
{
	public List<MacroAIParty> UnspawnedParties;
	public List<MacroAIParty> SpawnedParties;

	public void Initialize()
	{
		UnspawnedParties = new List<MacroAIParty>();
		SpawnedParties = new List<MacroAIParty>();
	}

	public void GenerateParties()
	{
		MacroAIParty party = new MacroAIParty();
		party.FactionID = "otu";
		party.CurrentSystemID = "washington_system";
		party.Location = Vector3.zero;
		party.DestinationStationID = "cambridge_station";
		party.DestinationSystemID = "new_england_system";
		party.IsDestinationAStation = false;
		party.IsInTradelane = false;
		party.DestinationCoord = GameManager.Inst.WorldManager.AllNavNodes["cambridge_station"].Location;
		party.MoveSpeed = 10f;
		party.NextNode = null;

		party.PrevNode = CreateTempNode(party.Location, "tempstart", GameManager.Inst.WorldManager.AllSystems[party.CurrentSystemID]);
		if(party.IsDestinationAStation)
		{
			party.DestNode = GameManager.Inst.WorldManager.AllNavNodes[party.DestinationStationID];
		}
		else
		{
			party.DestNode = CreateTempNode(party.DestinationCoord, "tempdest", GameManager.Inst.WorldManager.AllSystems[party.DestinationSystemID]);
		}



		UnspawnedParties.Add(party);
	}

	public void PerFrameUpdate()
	{
		//each frame update 1 party
		foreach(MacroAIParty party in UnspawnedParties)
		{
			if(party.DestNode == null)
			{
				continue;
			}

			//check if party is near next navnode

			if(party.NextNode != null && Vector3.Distance(party.Location, party.NextNode.Location) < 10f)
			{
				party.PrevNode = party.NextNode;
				if(party.NextNode == party.DestNode)
				{
					//done travelling
					if(party.IsDestinationAStation && party.DestNode.NavNodeType == NavNodeType.Station)
					{
						party.DockedStationID = party.DestinationStationID;
						party.DestNode = null;
						party.NextNode = null;
						//now do the trading and stuff before leaving

					}
				}
				else
				{
					//find next node towards dest node
					//is dest system same as current system? if not, need to find the next system towards the destination system, 
					//then find the jumpgate to the next system, 
					//finally find the next node towards the jumpgate
					if(party.DestinationSystemID == party.CurrentSystemID)
					{

						party.NextNode = FindNextNavNode(party.PrevNode, party.DestNode, party.CurrentSystemID);
					}
					else
					{
						
					}

					if(party.NextNode.NavNodeType == NavNodeType.Tradelane && party.PrevNode.NavNodeType == NavNodeType.Tradelane)
					{
						party.IsInTradelane = true;
					}
				}
			}
			else if(party.NextNode != null)
			{
				//move towards the next node
				float deltaTime = Time.time - party.LastUpdateTime;
				float speed = party.IsInTradelane ? 100 : party.MoveSpeed;
				party.Location = party.Location + (party.NextNode.Location - party.Location).normalized * deltaTime * speed;
				GameObject.Find("Sphere").transform.position = party.Location;
			}
			else
			{
				//find next node towards dest node
				party.NextNode = FindNextNavNode(party.PrevNode, party.DestNode, party.CurrentSystemID);
				if(party.NextNode.NavNodeType == NavNodeType.Tradelane && party.PrevNode.NavNodeType == NavNodeType.Tradelane)
				{
					party.IsInTradelane = true;
				}
			}

			party.LastUpdateTime = Time.time;
		}
	}


	public NavNode FindNextNavNode(NavNode start, NavNode dest, string systemID)
	{
		if(start == null || dest == null || start == dest)
		{
			Debug.Log("start is null? " + (start == null) + " dest is null? " + (dest == null) + " start==dest? " + (start == dest));
			return null;
		}

		Debug.Log("Pathfinding start " + start.ID + start.Location + " -> " + dest.ID + dest.Location);
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
			allNodes = GameManager.Inst.WorldManager.GetAllSystemNavNodes();
		}
		else
		{
			allNodes = GameManager.Inst.WorldManager.GetAllNavNodesInSystem(systemID);
		}
		foreach(NavNode n in allNodes)
			
		{
			gScores.Add(n, Mathf.Infinity);
			fScores.Add(n, Mathf.Infinity);
		}

		gScores[start] = 0;
		fScores[start] = Vector3.Distance(start.Location, dest.Location);

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

				float tentativeGScore = gScores[bestNode] + Vector3.Distance(bestNode.Location, neighbor.Location);
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
				fScores[neighbor] = gScores[neighbor] + Vector3.Distance(neighbor.Location, dest.Location);

			}
		}


		currentResultNode = result.First;
		while(currentResultNode != null)
		{
			Debug.Log("pathfinding result " + currentResultNode.Value.ID);
			currentResultNode = currentResultNode.Next;
		}

		Debug.Log("Going to return pathfinding result " + result.Last.Previous.Value.ID);
		return result.Last.Previous.Value;


	}



	private TempNode CreateTempNode(Vector3 location, string id, StarSystemData systemData)
	{
		TempNode tempNode = new TempNode();
		tempNode.ID = id;
		tempNode.Location = location;

		//find the nearest navnode
		float lowDist = Mathf.Infinity;
		NavNode nearestNode = null;
		foreach(NavNode childNode in systemData.ChildNodes)
		{
			float dist = Vector3.Distance(childNode.Location, location);
			if(dist < lowDist)
			{
				lowDist = dist;
				nearestNode = childNode;
			}
		}

		tempNode.NeighborIDs.Add(nearestNode.ID);
		tempNode.Neighbors.Add(nearestNode);

		return tempNode;
	}
}

public class MacroAIParty
{
	public string FactionID;
	public Vector3 Location;
	public string DockedStationID;
	public string CurrentSystemID;
	public float MoveSpeed;
	public bool IsDestinationAStation;
	public string DestinationSystemID;
	public Vector3 DestinationCoord;
	public string DestinationStationID;
	public bool IsInTradelane;
	public NavNode NextNode;
	public NavNode PrevNode;
	public NavNode DestNode;

	public float LastUpdateTime;
}