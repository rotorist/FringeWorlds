using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NPCManager
{
	public PartySpawner PartySpawner;
	public MacroAI MacroAI;
	public int LastUsedPartyNumber;
	public List<ShipBase> AllShips { get { return _allShips; } }
	public List<MacroAIParty> AllParties { get { return _allParties; } }

	public List<MacroAIPartySaveData> PartySaveDatas;

	public Dictionary<string, Faction> AllFactions { get { return _allFactions; } }

	private Dictionary<string, FactionRelationship> _factionRelationships;
	private List<ShipBase> _allShips;
	private Dictionary<string, Faction> _allFactions;
	private List<MacroAIParty> _allParties;


	public void InitializeDocked()
	{
		MacroAI = null;
		_allShips = new List<ShipBase>();
		_allFactions = new Dictionary<string, Faction>();
		_allParties = new List<MacroAIParty>();
		PartySaveDatas = new List<MacroAIPartySaveData>();
	}

	public void Initialize()
	{
		GameEventHandler.OnShipDeath -= OnShipDeath;
		GameEventHandler.OnShipDeath += OnShipDeath;

		_allShips = new List<ShipBase>();
		_allFactions = GameManager.Inst.DBManager.JsonDataHandler.LoadAllFactions();
		_allParties = new List<MacroAIParty>();

		_factionRelationships = new Dictionary<string, FactionRelationship>();
		FactionRelationshipSaveData relationshipSaveData = GameManager.Inst.DBManager.JsonDataHandler.LoadFactionRelationships();
		foreach(FactionRelationship relationship in relationshipSaveData.Relationships)
		{
			_factionRelationships.Add(relationship.Faction1 + relationship.Faction2, relationship);
		}

		/*
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
		*/

		MacroAI = new MacroAI();
		MacroAI.Initialize();

		PartySpawner = new PartySpawner();
		PartySpawner.Initialize();

		foreach(KeyValuePair<string, Faction> faction in _allFactions)
		{
			PartySpawner.GenerateFactionLoadouts(faction.Value);
		}
	}

	public void TestSpawn()
	{
		//spawn 1 party
		for(int i=0; i< 5; i++)
		{
			//MacroAI.GenerateParties();
		}
		Debug.Log("Generating test party");
		MacroAI.GenerateTestParty("otu");
		//MacroAI.GenerateTestParty("otu");
	}

	public void BuildShip(ShipBase ship, Loadout loadout, string factionID, MacroAIParty party)
	{
		string shipModelID = loadout.ShipID;
		ShipType shipType = loadout.ShipType;

		GameObject shipModel = GameObject.Instantiate(Resources.Load(shipModelID)) as GameObject;
		ShipStats stats = GameManager.Inst.ItemManager.GetShipStats(shipModelID);
		shipModel.transform.parent = ship.transform;
		shipModel.transform.localScale = new Vector3(1, 1, 1);
		shipModel.transform.localPosition = Vector3.zero;
		shipModel.transform.localEulerAngles = Vector3.zero;
		ship.ShipModel = shipModel;
		ship.ShipModelID = shipModelID;
		ship.MyReference = shipModel.GetComponent<ShipReference>();
		ship.MyReference.ParentShip = ship;
		ship.MyReference.Defensives = new List<Defensive>();
		ship.MyReference.ShipAudio = ship.GetComponent<AudioSource>();
		ship.HullCapacity = stats.Hull;
		ship.HullAmount = loadout.HullAmount;
		ship.MaxFuel = stats.MaxFuel;
		ship.FuelAmount = loadout.FuelAmount;
		ship.MaxLifeSupport = stats.LifeSupport;
		ship.LifeSupportAmount = loadout.LifeSupportAmount;
		ship.PowerSupply = stats.PowerSupply;
		ship.TorqueModifier = stats.TurnRate;
		ship.PowerSupply = stats.PowerSupply;
		ship.ShieldPowerAlloc = 1;
		ship.WeaponPowerAlloc = 1;
		ship.EnginePowerAlloc = 1;
		ship.Shield = ship.MyReference.Shield.GetComponent<ShieldBase>();
		ship.Shield.Initialize(loadout.Shield);
		ship.Shield.ParentShip = ship;
		ship.RB = ship.GetComponent<Rigidbody>();
		ship.RB.inertiaTensor = new Vector3(1, 1, 1);
		ship.Engine = shipModel.GetComponent<Engine>();
		ship.Engine.Initialize(stats);
		ship.Thruster = shipModel.GetComponent<Thruster>();
		ship.Thruster.Initialize(loadout.Thruster);
		ship.Scanner = shipModel.GetComponent<Scanner>();
		ship.Scanner.Initialize(loadout.Scanner);
		ship.Storage = shipModel.GetComponent<ShipStorage>();
		ship.Storage.Initialize();
		ship.Storage.AmmoBaySize = stats.AmmoBaySize;
		ship.Storage.CargoBaySize = stats.CargoBaySize;
		ship.WeaponCapacitor = shipModel.GetComponent<WeaponCapacitor>();
		ship.WeaponCapacitor.Initialize(loadout.WeaponCapacitor);

		ship.ShipModSlots = shipModel.GetComponent<ShipModSlots>();
		ship.ShipModSlots.NumberOfSlots = stats.ModSlots;
		ship.ShipModSlots.Initialize(loadout.ShipMods, ship);
		ship.ShipModSlots.ApplyMods();


		ship.MyLoadout = loadout;
	}

	public ShipBase SpawnAIShip(Loadout loadout, string factionID, MacroAIParty party)
	{
		ShipBase ship = (GameObject.Instantiate(Resources.Load("AIShip")) as GameObject).GetComponent<ShipBase>();

		BuildShip(ship, loadout, factionID, party);



		//load weapons
		foreach(WeaponJoint joint in ship.MyReference.WeaponJoints)
		{
			joint.ParentShip = ship;
			foreach(KeyValuePair<string, InvItemData> jointSetup in loadout.WeaponJoints)
			{
				if(jointSetup.Key == joint.JointID)
				{
					joint.LoadWeapon(jointSetup.Value);
				}
			}
		}

		for(int i=0; i<loadout.Defensives.Count; i++)
		{
			if(loadout.Defensives[i] != null && loadout.Defensives[i].Item.GetStringAttribute("Defensive Type") == "Countermeasure")
			{
				CMDispenser dispenser = new CMDispenser();
				dispenser.ParentShip = ship;
				dispenser.AmmoID = loadout.Defensives[i].RelatedItemID;
				dispenser.Type = DefensiveType.Countermeasure;
				ship.MyReference.Defensives.Add(dispenser);

			}
		}

		//load ammo bay
		ship.Storage.AmmoBayItems = new Dictionary<string, InvItemData>();
		foreach(InvItemData item in loadout.AmmoBayItems)
		{
			ship.Storage.AmmoBayItems.Add(item.Item.ID, item);
		}
		ship.Storage.CargoBayItems = new List<InvItemData>();
		foreach(InvItemData item in loadout.CargoBayItems)
		{
			ship.Storage.CargoBayItems.Add(item);
		}

		AI ai = ship.GetComponent<AI>();
		ai.Initialize(party, _allFactions[factionID]);

		return ship;
	}

	public ShipBase SpawnPlayerShip(Loadout loadout, string factionID, MacroAIParty party)
	{
		ShipBase ship = GameManager.Inst.PlayerControl.PlayerShip;

		BuildShip(ship, loadout, factionID, party);

		Autopilot pilot = GameManager.Inst.PlayerControl.PlayerAutopilot;
		pilot.AvoidanceDetector = ship.MyReference.AvoidanceDetector;
		pilot.AvoidanceDetector.ParentShip = ship;


		//load weapons
		foreach(WeaponJoint joint in ship.MyReference.WeaponJoints)
		{
			joint.ParentShip = ship;
			foreach(KeyValuePair<string, InvItemData> jointSetup in loadout.WeaponJoints)
			{
				if(jointSetup.Key == joint.JointID && jointSetup.Value != null)
				{
					joint.LoadWeapon(jointSetup.Value);
				}
			}
		}

		for(int i=0; i<loadout.Defensives.Count; i++)
		{
			if(loadout.Defensives[i] != null && loadout.Defensives[i].Item.GetStringAttribute("Defensive Type") == "Countermeasure")
			{
				CMDispenser dispenser = new CMDispenser();
				dispenser.ParentShip = ship;
				dispenser.AmmoID = loadout.Defensives[i].RelatedItemID;
				dispenser.Type = DefensiveType.Countermeasure;
				ship.MyReference.Defensives.Add(dispenser);

			}
		}

		//load ammo bay
		ship.Storage.AmmoBayItems = new Dictionary<string, InvItemData>();
		foreach(InvItemData item in loadout.AmmoBayItems)
		{
			ship.Storage.AmmoBayItems.Add(item.Item.ID, item);
		}

		ship.Storage.CargoBayItems = new List<InvItemData>();
		foreach(InvItemData item in loadout.CargoBayItems)
		{
			ship.Storage.CargoBayItems.Add(item);
		}
		//In$8177BB
		//load power management setting
		ship.CurrentPowerMgmtButton = loadout.CurrentPowerMgmtButton;


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

	public void PerSecondUpdate()
	{
		if(PartySpawner != null)
		{
			PartySpawner.PerSecondUpdate();
		}
	}

	public void OnShipDeath(ShipBase ship)
	{
		if(_allShips.Contains(ship))
		{
			_allShips.Remove(ship);
		}
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

	public MacroAIParty GetPartyByNumber(int partyNumber)
	{
		foreach(MacroAIParty party in _allParties)
		{
			if(partyNumber == party.PartyNumber)
			{
				return party;
			}
		}

		return null;
	}

	public float GetFactionRelationship(Faction faction1, Faction faction2)
	{
		//Debug.Log("Getting faction relationship for " + faction1.ID + " " + faction2.ID);
		if(faction1.ID == faction2.ID)
		{
			return 1;
		}

		string key1 = faction1.ID + faction2.ID;
		string key2 = faction2.ID + faction1.ID;

		if(_factionRelationships.ContainsKey(key1))
		{
			return _factionRelationships[key1].Relationship;
		}
		else if(_factionRelationships.ContainsKey(key2))
		{
			return _factionRelationships[key2].Relationship;
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