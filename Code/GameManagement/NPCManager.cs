﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NPCManager
{
	public MacroAI MacroAI;

	public List<ShipBase> AllShips { get { return _allShips; } }
	public List<MacroAIParty> AllParties { get { return _allParties; } }

	public Dictionary<string, Faction> AllFactions { get { return _allFactions; } }


	private List<ShipBase> _allShips;
	private Dictionary<string, Faction> _allFactions;
	private List<MacroAIParty> _allParties;



	public void Initialize()
	{
		_allShips = new List<ShipBase>();
		_allFactions = new Dictionary<string, Faction>();
		_allParties = new List<MacroAIParty>();


		//for now manually create factions
		Faction facp = new Faction();
		facp.ID = "player";
		facp.DisplayName = "Player";
		_allFactions.Add(facp.ID, facp);

		Faction fac1 = new Faction();
		fac1.ID = "otu";
		fac1.DisplayName = "Orion Trade Union";
		_allFactions.Add(fac1.ID, fac1);


		Faction fac2 = new Faction();
		fac2.ID = "otu_patrol";
		fac2.DisplayName = "Union Patrol";
		_allFactions.Add(fac2.ID, fac2);

		Faction fac3 = new Faction();
		fac3.ID = "otu_civil_defense";
		fac3.DisplayName = "Union Civil Defense";
		_allFactions.Add(fac3.ID, fac3);

		Faction fac4 = new Faction();
		fac4.ID = "otu_navy";
		fac4.DisplayName = "Union Navy";
		_allFactions.Add(fac4.ID, fac4);

		Faction fac5 = new Faction();
		fac5.ID = "otu_task_force";
		fac5.DisplayName = "Union Task Force";
		_allFactions.Add(fac5.ID, fac5);

		facp.Relationships = new Dictionary<string, float>() 
		{	
			{fac1.ID, 0.5f},
			{fac2.ID, 0.5f},
			{fac3.ID, 0.5f},
			{fac4.ID, 0.5f},
			{fac5.ID, 0.5f}
		};

		fac1.Relationships = new Dictionary<string, float>() 
		{	
			{facp.ID, 1f},
			{fac2.ID, 1},
			{fac3.ID, 0},
			{fac4.ID, 1},
			{fac5.ID, 1}
		};

		fac2.Relationships = new Dictionary<string, float>() 
		{	
			{facp.ID, 0.5f},
			{fac1.ID, 1},
			{fac3.ID, 1},
			{fac4.ID, 1},
			{fac5.ID, 1}
		};

		fac3.Relationships = new Dictionary<string, float>() 
		{	
			{facp.ID, 0.5f},
			{fac1.ID, 0},
			{fac2.ID, 1},
			{fac4.ID, 1},
			{fac5.ID, 1}
		};

		fac4.Relationships = new Dictionary<string, float>() 
		{	
			{facp.ID, 0.5f},
			{fac1.ID, 1},
			{fac2.ID, 1},
			{fac3.ID, 1},
			{fac5.ID, 1}
		};

		fac5.Relationships = new Dictionary<string, float>() 
		{	
			{facp.ID, 0.5f},
			{fac1.ID, 1},
			{fac2.ID, 1},
			{fac3.ID, 1},
			{fac4.ID, 1}
		};

		MacroAI = new MacroAI();
		MacroAI.Initialize();

	}

	public void TestSpawn()
	{
		//spawn 1 party
		for(int i=0; i< 5; i++)
		{
			//MacroAI.GenerateParties();
		}
		//MacroAI.GenerateTestParty("otu_civil_defense");
		//MacroAI.GenerateTestParty("otu");
	}

	public ShipBase SpawnAIShip(Loadout loadout, string factionID, MacroAIParty party)
	{
		ShipBase ship = (GameObject.Instantiate(Resources.Load("AIShip")) as GameObject).GetComponent<ShipBase>();

		string shipModelID = loadout.ShipID;
		ShipType shipType = loadout.ShipType;

		GameObject shipModel = GameObject.Instantiate(Resources.Load(shipModelID)) as GameObject;
		shipModel.transform.parent = ship.transform;
		shipModel.transform.localScale = new Vector3(1, 1, 1);
		shipModel.transform.localPosition = Vector3.zero;
		shipModel.transform.localEulerAngles = Vector3.zero;
		ship.ShipModel = shipModel;
		ship.ShipModelID = shipModelID;
		ship.MyReference = shipModel.GetComponent<ShipReference>();
		ship.MyReference.ParentShip = ship;
		ship.Shield = ship.MyReference.Shield.GetComponent<ShieldBase>();
		ship.RB = ship.GetComponent<Rigidbody>();
		ship.RB.inertiaTensor = new Vector3(1, 1, 1);
		ship.Engine = shipModel.GetComponent<Engine>();
		ship.Thruster = shipModel.GetComponent<Thruster>();
		ship.Scanner = shipModel.GetComponent<Scanner>();

		AI ai = ship.GetComponent<AI>();
		ai.Initialize(party, _allFactions[factionID]);

		//load weapons
		foreach(WeaponJoint joint in ship.MyReference.WeaponJoints)
		{
			joint.ParentShip = ship;
			foreach(KeyValuePair<string, string> jointSetup in loadout.WeaponJoints)
			{
				if(jointSetup.Key == joint.JointID)
				{
					joint.LoadWeapon(jointSetup.Value);
				}
			}
		}

		return ship;
	}

	public void PerFrameUpdate()
	{
		if(GameManager.Inst.SceneType == SceneType.SpaceTest)
		{
			return;
		}

		MacroAI.PerFrameUpdate();
	}

	public void AddExistingShip(ShipBase ship)
	{
		if(!_allShips.Contains(ship))
		{
			_allShips.Add(ship);
		}
	}

	public void RemoveExistingShip(ShipBase ship)
	{
		if(_allShips.Contains(ship))
		{
			_allShips.Remove(ship);
		}
	}

	public float GetFactionRelationship(Faction faction1, Faction faction2)
	{
		//Debug.Log("Getting faction relationship for " + faction1.ID + " " + faction2.ID);
		if(faction1.ID == faction2.ID)
		{
			return 1;
		}

		else if(faction1.Relationships.ContainsKey(faction2.ID))
		{
			return faction1.Relationships[faction2.ID];
		}
		else
		{
			return 0;
		}
	}


}

public enum Factions
{
	Confederation,

}