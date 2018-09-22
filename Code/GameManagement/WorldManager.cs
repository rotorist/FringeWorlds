using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager 
{
	public StarSystem CurrentSystem;
	public StationBase CurrentDockedStation;
	public Sun [] Suns;
	public Dictionary<string, StarSystemData> AllSystems { get { return _allSystems; } }
	public Dictionary<string, NavNode> AllNavNodes { get { return _allNavNodes; } }
	public List<AsteroidField> AsteroidFields { get { return _asteroidFields; } }

	private List<AsteroidField> _asteroidFields;
	private Dictionary<string, StarSystemData> _allSystems;
	private Dictionary<string, NavNode> _allNavNodes;

	public void Initialize()
	{
		Suns = GameObject.FindObjectsOfType<Sun>();
		_allNavNodes = new Dictionary<string, NavNode>();
		_allSystems = GameManager.Inst.DBManager.XMLParserWorld.LoadAllSystemsFromXML();

		BuildNavMap();




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



		if(Input.GetKeyDown(KeyCode.F12))
		{
			GameManager.Inst.DBManager.XMLParserWorld.GenerateSystemXML();
		}

		if(_asteroidFields.Count > 0)
		{
			UpdateAsteroids();
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
