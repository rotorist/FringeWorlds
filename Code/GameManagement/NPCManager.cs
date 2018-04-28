using System.Collections;
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
			{facp.ID, 0.5f},
			{fac2.ID, 1},
			{fac3.ID, 1},
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
			{fac1.ID, 1},
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
		MacroAI.GenerateParties();

	}

	public void PerFrameUpdate()
	{
		MacroAI.PerFrameUpdate();
	}

	public void AddExistingShip(ShipBase ship)
	{
		if(!_allShips.Contains(ship))
		{
			_allShips.Add(ship);
		}
	}


}

public enum Factions
{
	Confederation,

}