using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager 
{
	public StarSystem CurrentSystem;
	public Sun [] Suns;
	public Dictionary<string, StarSystemData> AllSystems { get { return _allSystems; } }
	public Dictionary<string, NavNode> AllNavNodes { get { return _allNavNodes; } }
	public List<AsteroidField> AsteroidFields { get { return _asteroidFields; } }

	public int CurrentTime;//in hours

	public Dictionary<string, DockableStationData> DockableStationDatas;

	private List<AsteroidField> _asteroidFields;
	private Dictionary<string, StarSystemData> _allSystems;
	private Dictionary<string, NavNode> _allNavNodes;
	private int _secondsPerHour;
	private float _hourCounter;

	public void PreInitialize()
	{
		CurrentTime = 186698;
		DockableStationDatas = GameManager.Inst.DBManager.JsonDataHandler.LoadAllDockableStations();
		_allNavNodes = new Dictionary<string, NavNode>();
		_allSystems = GameManager.Inst.DBManager.XMLParserWorld.LoadAllSystemsFromXML();
	}


	public void InitializeDocked()
	{

		BuildNavMap();
		_secondsPerHour = 10;
	}

	public void InitializeSpace()
	{
		Suns = GameObject.FindObjectsOfType<Sun>();

		BuildNavMap();
		_secondsPerHour = 10;

		StationBase [] stations = GameObject.FindObjectsOfType<StationBase>();
		foreach(StationBase station in stations)
		{
			station.Initialize();
		}

		_asteroidFields = new List<AsteroidField>();
		AsteroidField [] fields = GameObject.FindObjectsOfType<AsteroidField>();
		foreach(AsteroidField field in fields)
		{
			field.Initialize();
			_asteroidFields.Add(field);
		}
	}

	public void PerFrameUpdate()
	{
		if(GameManager.Inst.PlayerControl.PlayerShip != null)
		{
			foreach(Sun sun in Suns)
			{
				sun.transform.LookAt(GameManager.Inst.PlayerControl.PlayerShip.transform);
			}
		}
			

		if(_asteroidFields.Count > 0)
		{
			UpdateAsteroids();
		}
	}

	public void PerSecondUpdate()
	{
		_hourCounter ++;
		if(_hourCounter > _secondsPerHour)
		{
			CurrentTime ++;
			_hourCounter = 0;
			GameEventHandler.Instance.TriggerOnHour();
		}
	}

	public List<NavNode> GetAllNavNodeList()
	{
		List<NavNode> nodeList = new List<NavNode>();
		foreach(KeyValuePair<string, NavNode> node in _allNavNodes)
		{
			nodeList.Add(node.Value);
		}

		return nodeList;
	}

	public List<NavNode> GetAllNavNodesInSystem(string systemID)
	{
		StarSystemData system = _allSystems[systemID];
		return system.ChildNodes;
	}

	public List<NavNode> GetAllSystemNavNodes()
	{
		List<NavNode> nodeList = new List<NavNode>();
		foreach(KeyValuePair<string, StarSystemData> node in _allSystems)
		{
			nodeList.Add((NavNode)node.Value);
		}

		return nodeList;
	}

	public string GetFormattedTime(int timeInHours)
	{
		int sol = Mathf.FloorToInt(timeInHours / (128f * 16f));
		int day = Mathf.FloorToInt((timeInHours % (128 * 16)) / 16f);
		int hour = (timeInHours % (128 * 16)) % 16;

		return "Sol " + sol.ToString() + " Day " + day.ToString() + " Hr " + hour;
	}

	public string GetFormattedCurrentTime()
	{
		return GetFormattedTime(CurrentTime);
	}

	public StationData GetRandomFriendlyDockableStation(string factionID, StationData excludeStation)
	{
		List<StationData> candidates = new List<StationData>();
		foreach(KeyValuePair<string, NavNode> navNode in _allNavNodes)
		{
			if(navNode.Value.NavNodeType == NavNodeType.Station)
			{
				StationData stationData = (StationData)navNode.Value;
				if(stationData != excludeStation && stationData.DockableStationData != null)
				{
					Faction requesterFaction = GameManager.Inst.NPCManager.AllFactions[factionID];
					Faction stationFaction = GameManager.Inst.NPCManager.AllFactions[stationData.DockableStationData.FactionID];
					float relationship = GameManager.Inst.NPCManager.GetFactionRelationship(requesterFaction, stationFaction);
					if(relationship >= 0.4f)
					{
						candidates.Add(stationData);
					}
				}
			}
		}

		if(candidates.Count > 0)
		{
			return candidates[UnityEngine.Random.Range(0, candidates.Count)];
		}
		else
		{
			return null;
		}
	}




	private void BuildNavMap()
	{
		_allNavNodes = new Dictionary<string, NavNode>();
		//foreach system, for each station, assign neighbor from neighbor ID
		//first build a dictionary of all navnodes
		foreach(KeyValuePair<string, StarSystemData> systemData in AllSystems)
		{
			foreach(StationData stationData in systemData.Value.Stations)
			{
				_allNavNodes.Add(stationData.ID, stationData);
				//now assign dockablestationdata to the station, if there is a dockablestationdata created for it
				foreach(KeyValuePair<string, DockableStationData> dockable in DockableStationDatas)
				{
					if(dockable.Value.StationID == stationData.ID)
					{
						stationData.DockableStationData = dockable.Value;
					}
				}
			}

			foreach(JumpGateData jumpGateData in systemData.Value.JumpGates)
			{
				_allNavNodes.Add(jumpGateData.ID, jumpGateData);
			}

			foreach(TradelaneData tradelaneData in systemData.Value.Tradelanes)
			{
				_allNavNodes.Add(tradelaneData.ID, tradelaneData);
			}

			//add systemData as a navnode too
			_allNavNodes.Add(systemData.Value.ID, systemData.Value);
		}



		//now go through each navnode and assign neighbors
		foreach(KeyValuePair<string, NavNode> node in _allNavNodes)
		{
			node.Value.Neighbors = new List<NavNode>();
			foreach(string neighborID in node.Value.NeighborIDs)
			{
				//Debug.Log(neighborID);
				node.Value.Neighbors.Add(_allNavNodes[neighborID]);
			}
		}
	}

	private void UpdateAsteroids()
	{
		Vector3 playerPos = GameManager.Inst.PlayerControl.PlayerShip.transform.position;
		foreach(AsteroidField field in _asteroidFields)
		{
			
			field.PerFrameUpdate();

		}
	}
}
