using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MacroAIParty
{

	public Vector3 Destination;
	public int PartyNumber;
	public string FactionID;
	public RelLoc Location;
	public string DockedStationID;
	public string CurrentSystemID;
	public float MoveSpeed;
	public Dictionary<ShipBase, Vector3> Formation;
	public bool IsPlayerParty;
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

	public List<Loadout> FollowerLoadouts;
	public Loadout LeaderLoadout;

	public Dictionary<string,BehaviorTree> TreeSet;

	public MaxStack<string> RunningNodeHist;

	public MacroAIParty()
	{
		RunningNodeHist = new MaxStack<string>(20);
	}
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

[System.Serializable]
public class MacroAITaskSaveData
{
	public MacroAITaskType TaskType;
	public float StayDuration;
	public SerVector3 TravelDestCoord;//distance from origin
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

[System.Serializable]
public class MacroAIPartySaveData
{
	public SerVector3 Destination;
	public int PartyNumber;
	public string FactionID;
	public SerVector3 Location;//distance from origin
	public string DockedStationID;
	public string CurrentSystemID;
	public float MoveSpeed;
	public bool IsPlayerParty;
	public bool IsInTradelane;
	public List<string> NextTwoNodesIDs;
	public string PrevNodeID;
	public string DestNodeID;
	public bool HasReachedDestNode;
	public float WaitTimer;
	public MacroAITaskSaveData CurrentTask;
	public float LastUpdateTime;
	public bool ShouldEnableAI;
	public List<LoadoutSaveData> FollowerLoadouts;
	public LoadoutSaveData LeaderLoadout;
	public List<string> TreeSetNames;

}