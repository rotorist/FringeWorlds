using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MacroAI
{




	public void Initialize()
	{
		GameEventHandler.OnShipDeath -= OnShipDeath;
		GameEventHandler.OnShipDeath += OnShipDeath;
	}

	public void GenerateTestParty(string factionID)
	{
		MacroAIParty party = new MacroAIParty();
		party.FactionID = factionID;
		party.SpawnedShips = new List<ShipBase>();

		List<string> keyList = new List<string>(GameManager.Inst.WorldManager.AllSystems.Keys);

		//StarSystemData currentSystem = GameManager.Inst.WorldManager.AllSystems["washington_system"];
		StarSystemData currentSystem = GameManager.Inst.WorldManager.AllSystems[keyList[UnityEngine.Random.Range(0, keyList.Count)]];
		party.CurrentSystemID = currentSystem.ID;

		//StationData currentStation = currentSystem.GetStationByID("planet_colombia_landing");
		StationData currentStation = currentSystem.Stations[UnityEngine.Random.Range(0, currentSystem.Stations.Count)];
		party.DockedStationID = "planet_colombia_landing";
		Transform origin = GameObject.Find("Origin").transform;
		party.Location = new RelLoc(origin.position, currentStation.Location.RealPos, origin);
		party.PartyNumber = GameManager.Inst.NPCManager.LastUsedPartyNumber + 1;
		GameManager.Inst.NPCManager.LastUsedPartyNumber = party.PartyNumber;

		Item item1 = new Item(GameManager.Inst.ItemManager.GetItemStats("wc_VikoWeaponCapacitorMK1"));
		InvItemData itemData1 = new InvItemData();
		itemData1.Item = item1;
		itemData1.Quantity = 1;


		Item item2 = new Item(GameManager.Inst.ItemManager.GetItemStats("scn_RadianTekShortRangeScanner"));
		InvItemData itemData2 = new InvItemData();
		itemData2.Item = item2;
		itemData2.Quantity = 1;


		Item item3 = new Item(GameManager.Inst.ItemManager.GetItemStats("thr_StrelskyThrusterMK1"));
		InvItemData itemData3 = new InvItemData();
		itemData3.Item = item3;
		itemData3.Quantity = 1;


		Item item4 = new Item(GameManager.Inst.ItemManager.GetItemStats("shd_NCPTransporterShieldMK1"));
		InvItemData itemData4 = new InvItemData();
		itemData4.Item = item4;
		itemData4.Quantity = 1;


		Item item5 = new Item(GameManager.Inst.ItemManager.GetItemStats("dfs_CMDispenser"));
		InvItemData itemData5 = new InvItemData();
		itemData5.Item = item5;
		itemData5.Quantity = 1;
		itemData5.RelatedItemID = "ammo_LongDurationCM";


		Item item10 = new Item(GameManager.Inst.ItemManager.GetItemStats("wpn_SDCStomperAutocannon"));
		InvItemData itemData10 = new InvItemData();
		itemData10.Item = item10;
		itemData10.Quantity = 1;
		itemData10.RelatedItemID = "ammo_20mmTitaniumSlugs";


		Item item11 = new Item(GameManager.Inst.ItemManager.GetItemStats("wpn_StrelskyScreamerMissileLauncher"));
		InvItemData itemData11 = new InvItemData();
		itemData11.Item = item11;
		itemData11.Quantity = 1;
		itemData11.RelatedItemID = "ammo_StrelskySeekerMissile";


		Item item12 = new Item(GameManager.Inst.ItemManager.GetItemStats("wpn_AlFasadIonFluxTurretMK1"));
		InvItemData itemData12 = new InvItemData();
		itemData12.Item = item12;
		itemData12.Quantity = 1;


		Item item13 = new Item(GameManager.Inst.ItemManager.GetItemStats("wpn_CGGAnnihilatorTurretMK2"));
		InvItemData itemData13 = new InvItemData();
		itemData13.Item = item13;
		itemData13.Quantity = 1;


		Item item14 = new Item(GameManager.Inst.ItemManager.GetItemStats("shd_KeslerFighterShieldMK1"));
		InvItemData itemData14 = new InvItemData();
		itemData14.Item = item14;
		itemData14.Quantity = 1;

		Item item15 = new Item(GameManager.Inst.ItemManager.GetItemStats("wpn_AlFasadStingerPulseCannon"));
		InvItemData itemData15 = new InvItemData();
		itemData15.Item = item15;
		itemData15.Quantity = 1;


		Item item20 = new Item(GameManager.Inst.ItemManager.GetItemStats("ammo_LongDurationCM"));
		InvItemData itemData20 = new InvItemData();
		itemData20.Item = item20;
		itemData20.Quantity = UnityEngine.Random.Range(1, 3);


		Item item21 = new Item(GameManager.Inst.ItemManager.GetItemStats("ammo_20mmTitaniumSlugs"));
		InvItemData itemData21 = new InvItemData();
		itemData21.Item = item21;
		itemData21.Quantity = 500;


		Item item101 = new Item(GameManager.Inst.ItemManager.GetItemStats("wc_VikoWeaponCapacitorMK1"));
		InvItemData itemData101 = new InvItemData();
		itemData101.Item = item101;
		itemData101.Quantity = 1;


		Item item102 = new Item(GameManager.Inst.ItemManager.GetItemStats("scn_RadianTekShortRangeScanner"));
		InvItemData itemData102 = new InvItemData();
		itemData102.Item = item102;
		itemData102.Quantity = 1;


		Item item103 = new Item(GameManager.Inst.ItemManager.GetItemStats("thr_StrelskyThrusterMK1"));
		InvItemData itemData103 = new InvItemData();
		itemData103.Item = item103;
		itemData103.Quantity = 1;


		Item item104 = new Item(GameManager.Inst.ItemManager.GetItemStats("shd_NCPTransporterShieldMK1"));
		InvItemData itemData104 = new InvItemData();
		itemData104.Item = item104;
		itemData104.Quantity = 1;


		Item item105 = new Item(GameManager.Inst.ItemManager.GetItemStats("dfs_CMDispenser"));
		InvItemData itemData105 = new InvItemData();
		itemData105.Item = item105;
		itemData105.Quantity = 1;
		itemData105.RelatedItemID = "ammo_LongDurationCM";


		Item item110 = new Item(GameManager.Inst.ItemManager.GetItemStats("wpn_SDCStomperAutocannon"));
		InvItemData itemData110 = new InvItemData();
		itemData110.Item = item110;
		itemData110.Quantity = 1;
		itemData110.RelatedItemID = "ammo_20mmTitaniumSlugs";


		Item item111 = new Item(GameManager.Inst.ItemManager.GetItemStats("wpn_StrelskyScreamerMissileLauncher"));
		InvItemData itemData111 = new InvItemData();
		itemData111.Item = item111;
		itemData111.Quantity = 1;
		itemData111.RelatedItemID = "ammo_StrelskySeekerMissile";


		Item item112 = new Item(GameManager.Inst.ItemManager.GetItemStats("wpn_AlFasadIonFluxTurretMK1"));
		InvItemData itemData112 = new InvItemData();
		itemData112.Item = item112;
		itemData112.Quantity = 1;


		Item item113 = new Item(GameManager.Inst.ItemManager.GetItemStats("wpn_CGGAnnihilatorTurretMK2"));
		InvItemData itemData113 = new InvItemData();
		itemData113.Item = item113;
		itemData113.Quantity = 1;


		Item item114 = new Item(GameManager.Inst.ItemManager.GetItemStats("shd_KeslerFighterShieldMK1"));
		InvItemData itemData114 = new InvItemData();
		itemData114.Item = item114;
		itemData114.Quantity = 1;

		Item item115 = new Item(GameManager.Inst.ItemManager.GetItemStats("wpn_AlFasadStingerPulseCannon"));
		InvItemData itemData115 = new InvItemData();
		itemData115.Item = item115;
		itemData115.Quantity = 1;


		Item item120 = new Item(GameManager.Inst.ItemManager.GetItemStats("ammo_LongDurationCM"));
		InvItemData itemData120 = new InvItemData();
		itemData120.Item = item120;
		itemData120.Quantity = UnityEngine.Random.Range(1, 3);


		Item item121 = new Item(GameManager.Inst.ItemManager.GetItemStats("ammo_20mmTitaniumSlugs"));
		InvItemData itemData121 = new InvItemData();
		itemData121.Item = item121;
		itemData121.Quantity = 500;

		//generate loadout
		party.LeaderLoadout = new Loadout("Trimaran", ShipType.Transport);

		party.LeaderLoadout.HullAmount = GameManager.Inst.ItemManager.GetShipStats(party.LeaderLoadout.ShipID).Hull;
		party.LeaderLoadout.FuelAmount = GameManager.Inst.ItemManager.GetShipStats(party.LeaderLoadout.ShipID).MaxFuel;
		party.LeaderLoadout.LifeSupportAmount = GameManager.Inst.ItemManager.GetShipStats(party.LeaderLoadout.ShipID).LifeSupport;

		party.LeaderLoadout.Defensives = new List<InvItemData>();

		party.LeaderLoadout.ShipMods = new InvItemData[GameManager.Inst.ItemManager.GetShipStats(party.LeaderLoadout.ShipID).ModSlots];



		party.LeaderLoadout.WeaponCapacitor = itemData1;
		party.LeaderLoadout.Shield = itemData4;
		party.LeaderLoadout.Defensives.Add(itemData5);
		party.LeaderLoadout.Thruster = itemData3;
		party.LeaderLoadout.Scanner = itemData2;
		party.LeaderLoadout.AmmoBayItems.Add(itemData20);
		party.LeaderLoadout.AmmoBayItems.Add(itemData21);

		party.LeaderLoadout.WeaponJoints = new Dictionary<string, InvItemData>()
		{
			{ "GimballLeft", itemData10 },
			{ "GimballRight", itemData10 },
			{ "TurretLeft", itemData12 },
			{ "TurretRight", itemData12 },
			{ "TurretTop", itemData13 },
		};




		party.FollowerLoadouts = new List<Loadout>();
		for(int i=0; i<UnityEngine.Random.Range(1, 5); i++)
		{
			Loadout loadout = new Loadout("Spitfire", ShipType.Fighter);
			party.FollowerLoadouts.Add(loadout);
			loadout.WeaponJoints = new Dictionary<string, InvItemData>()
			{
				{ "GimballLeft", itemData115 },
				{ "GimballRight", itemData115 },
				{ "GimballFront", itemData111 },
			};

			loadout.HullAmount = GameManager.Inst.ItemManager.GetShipStats(loadout.ShipID).Hull;
			loadout.FuelAmount = GameManager.Inst.ItemManager.GetShipStats(loadout.ShipID).MaxFuel;
			loadout.LifeSupportAmount = GameManager.Inst.ItemManager.GetShipStats(loadout.ShipID).LifeSupport;

			loadout.Defensives = new List<InvItemData>();

			loadout.WeaponCapacitor = itemData1;
			if(UnityEngine.Random.value > 0.6f)
			{
				loadout.Defensives.Add(itemData5);
			}
			loadout.Shield = itemData114;
			loadout.Thruster = itemData103;
			loadout.Scanner = itemData102;
			loadout.AmmoBayItems.Add(itemData120);
			loadout.AmmoBayItems.Add(itemData121);

			loadout.ShipMods = new InvItemData[GameManager.Inst.ItemManager.GetShipStats(loadout.ShipID).ModSlots];



		}


		MacroAITask task = AssignMacroAITask(MacroAITaskType.None, party);

		party.IsInTradelane = false;
		//party.DestinationCoord = GameManager.Inst.WorldManager.AllNavNodes["cambridge_station"].Location;
		party.MoveSpeed = 10f;
		party.NextTwoNodes = new List<NavNode>();
		party.PrevNode = null;//CreateTempNode(party.Location, "tempstart", GameManager.Inst.WorldManager.AllSystems[party.CurrentSystemID]);

		LoadPartyTreeset(party);

		GameManager.Inst.NPCManager.AllParties.Add(party);
	}

	public MacroAIParty GeneratePlayerParty()
	{
		MacroAIParty party = new MacroAIParty();
		party.FactionID = "player";
		party.SpawnedShips = new List<ShipBase>();
		party.SpawnedShips.Add(GameManager.Inst.PlayerControl.PlayerShip);
		party.IsPlayerParty = true;

		StarSystemData currentSystem = GameManager.Inst.WorldManager.AllSystems[GameManager.Inst.WorldManager.CurrentSystem.ID];
		party.CurrentSystemID = currentSystem.ID;
		StationData currentStation = null;
		party.DockedStationID = "";
		Transform origin = GameObject.Find("Origin").transform;
		party.Location = new RelLoc(origin.position, GameManager.Inst.PlayerControl.PlayerShip.transform.position, origin);
		party.PartyNumber = 0;
		party.SpawnedShipsLeader = GameManager.Inst.PlayerControl.PlayerShip;
		party.ShouldEnableAI = true;

		//generate loadout
		party.LeaderLoadout = GameManager.Inst.PlayerProgress.ActiveLoadout;

		party.FollowerLoadouts = new List<Loadout>();
		/*
		for(int i=0; i<4; i++)
		{
			Loadout loadout = new Loadout("LightFighter", ShipType.Fighter);
			party.FollowerLoadouts.Add(loadout);
			loadout.WeaponJoints = new Dictionary<string, string>()
			{
				{ "GimballLeft", "Gun1" },
				{ "GimballRight", "Gun1" },
			};
		}*/

		MacroAITask task = null;

		party.IsInTradelane = false;
		//party.DestinationCoord = GameManager.Inst.WorldManager.AllNavNodes["cambridge_station"].Location;
		party.MoveSpeed = 10f;
		party.NextTwoNodes = new List<NavNode>();
		party.PrevNode = null;//CreateTempNode(party.Location, "tempstart", GameManager.Inst.WorldManager.AllSystems[party.CurrentSystemID]);

		LoadPartyTreeset(party);
		GenerateFormationForParty(party);

		GameManager.Inst.NPCManager.AllParties.Add(party);

		return party;
	}
	/*
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
		party.PartyNumber = GameManager.Inst.NPCManager.LastUsedPartyNumber + 1;
		GameManager.Inst.NPCManager.LastUsedPartyNumber = party.PartyNumber;

		//generate loadout
		party.LeaderLoadout = new Loadout("LightTransporter", ShipType.Transport);
		party.LeaderLoadout.WeaponJoints = new Dictionary<string, InvItemData>()
		{
			{ "GimballLeft", "Gun1" },
			{ "GimballRight", "Gun1" },
			{ "TurretLeft", "Turret1" },
			{ "TurretRight", "Turret1" },
		};

		party.FollowerLoadouts = new List<Loadout>();
		for(int i=0; i<UnityEngine.Random.Range(1, 5); i++)
		{
			Loadout loadout = new Loadout("Spitfire", ShipType.Fighter);
			party.FollowerLoadouts.Add(loadout);
			loadout.WeaponJoints = new Dictionary<string, string>()
			{
				{ "GimballLeft", "Gun1" },
				{ "GimballRight", "Gun1" },
			};
		}


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
	*/
	public void PerFrameUpdate()
	{
		//each frame update 1 party
		foreach(MacroAIParty party in GameManager.Inst.NPCManager.AllParties)
		{

			//if party is in the same system as player, and there's no leader ship spawned, then spawn a ship, and make it follow party location
			if(party.CurrentSystemID == GameManager.Inst.WorldManager.CurrentSystem.ID)
			{	
				
				//if(party.NextNode != null)
					//Debug.Log("next node : " + party.NextNode.ID);



				if(party.SpawnedShips.Count < party.FollowerLoadouts.Count + 1)
				{
					SpawnPartyMembers(party);
					GenerateFormationForParty(party);
				}



				if(!party.ShouldEnableAI)
				{
					foreach(ShipBase ship in party.SpawnedShips)
					{
						if(party.SpawnedShipsLeader == ship)
						{
							ship.transform.position = party.Location.RealPos;
							ship.transform.LookAt(party.Destination);
						}
						else
						{
							ship.transform.position = party.SpawnedShipsLeader.transform.TransformPoint(party.Formation[ship]);
							ship.transform.LookAt(party.Destination);
						}
					}
				}
				else
				{
					party.Location.RealPos = party.SpawnedShipsLeader.transform.position;
				}




				if(party.IsPlayerParty)
				{
					party.ShouldEnableAI = true;
				}
				else
				{
					//if one ship near (<1000) player then turn on AI for all party
					//if all party are too far (>1500) from player then turn off ai for all party
					//otherwise stay the same
					foreach(ShipBase ship in party.SpawnedShips)
					{
						float distFromPlayer = Vector3.Distance(ship.transform.position, GameManager.Inst.PlayerControl.PlayerShip.transform.position);
						if(distFromPlayer < 9999999)
						{
							party.ShouldEnableAI = true;
							break;
						}
						else if(distFromPlayer > 1500)
						{
							party.ShouldEnableAI = false;

						}
					}
				}

				bool needRepath = false;

				if(party.IsPlayerParty)
				{
					foreach(ShipBase ship in party.SpawnedShips)
					{
						if(ship != party.SpawnedShipsLeader)
						{
							ship.MyAI.Activate();
						}
					}
				}
				else
				{
					foreach(ShipBase ship in party.SpawnedShips)
					{
						AI ai = ship.GetComponent<AI>();
						if(party.ShouldEnableAI)
						{
							if(!ai.IsActive)
							{
								Debug.Log("Activating AI");
								ai.Activate();
								//here we need to place all members in formation

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

			/*
			if(party.CurrentTask == null)
			{
				Debug.Log("no task? " + (party.CurrentTask == null));
				continue;
			}
			*/

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
			if(UnityEngine.Random.value > 0.0f)
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
			
			if(!string.IsNullOrEmpty(party.DockedStationID) && UnityEngine.Random.value < 0.75f)
			{
				task.TaskType = MacroAITaskType.Trade;
				task.StayDuration = UnityEngine.Random.Range(10f, 30f);
				party.WaitTimer = 0;
				party.RunningNodeHist.UniquePush("Trading for " + task.StayDuration);
				Debug.LogError("Party " + party.PartyNumber + " Task has been assigned to party: Trade, current station" + party.DockedStationID);

				TradeCommodity(party, GameManager.Inst.WorldManager.DockableStationDatas[party.DockedStationID]);
			}
			else
			{
				task.TaskType = MacroAITaskType.Stay;
				task.StayDuration = UnityEngine.Random.Range(15f, 35f);
				party.WaitTimer = 0;
				party.RunningNodeHist.UniquePush("Stay for " + task.StayDuration);
				Debug.LogError("Party " + party.PartyNumber + " Task has been assigned to party: " + task.TaskType + " for " + task.StayDuration);
			}


		}
		else if(prevTaskType == MacroAITaskType.Stay || prevTaskType == MacroAITaskType.Trade)
		{
			
			task.TaskType = MacroAITaskType.Travel;
			List<string> keyList = new List<string>(GameManager.Inst.WorldManager.AllSystems.Keys);
			if(Time.time < 0)
			{
				Debug.LogError("new task for initial test");
				StarSystemData destSystem = GameManager.Inst.WorldManager.AllSystems[keyList[UnityEngine.Random.Range(0, keyList.Count)]];
				//StarSystemData destSystem = GameManager.Inst.WorldManager.AllSystems["washington_system"];
				//StarSystemData destSystem = GameManager.Inst.WorldManager.AllSystems["new_england_system"];
				task.TravelDestSystemID = destSystem.ID;

				task.TravelDestNodeID = destSystem.Stations[UnityEngine.Random.Range(0, destSystem.Stations.Count)].ID;
				//task.TravelDestNodeID = "annandale_station";//"planet_colombia_landing";//"bethesda_station";
				task.IsDestAStation = true;
				//task.TravelDestCoord = new RelLoc(destSystem.OriginPosition, new Vector3(-28.3f, 5f, 418.8f), null);
			}
			else
			{
				Debug.LogError("new task to random station in random system");
				StarSystemData destSystem = GameManager.Inst.WorldManager.AllSystems[keyList[UnityEngine.Random.Range(0, keyList.Count)]];
				//StarSystemData destSystem = GameManager.Inst.WorldManager.AllSystems["washington_system"];

				task.TravelDestSystemID = destSystem.ID;
				task.TravelDestNodeID = destSystem.Stations[UnityEngine.Random.Range(0, destSystem.Stations.Count)].ID;
				//task.TravelDestNodeID = "planet_colombia_landing";
				task.IsDestAStation = true;
			}

			party.WaitTimer = 0;
			Debug.LogError("Party " + party.PartyNumber + " Task has been assigned to party: " + task.TaskType + " to " + (task.IsDestAStation ? task.TravelDestNodeID : task.TravelDestCoord.ToString()));


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

	public void OnShipDeath(ShipBase ship)
	{
		MacroAIParty party = ship.MyAI.MyParty;
		if(party.SpawnedShipsLeader == ship)
		{
			//elect a new leader
			party.SpawnedShips.Remove(ship);
			if(party.Formation.ContainsKey(ship))
			{
				party.Formation.Remove(ship);
			}
			party.LeaderLoadout = null;

			if(party.SpawnedShips.Count > 0)
			{
				//randomly pick one ship
				ShipBase candidate = party.SpawnedShips[UnityEngine.Random.Range(0, party.SpawnedShips.Count)];
				party.SpawnedShipsLeader = candidate;
				party.Formation[candidate] = Vector3.zero;
				party.LeaderLoadout = candidate.MyLoadout;
				if(party.FollowerLoadouts.Contains(candidate.MyLoadout))
				{
					party.FollowerLoadouts.Remove(candidate.MyLoadout);
				}
			}
			else
			{
				//party is gone, remove party
				GameManager.Inst.NPCManager.AllParties.Remove(party);
			}
		}
		else if(party.SpawnedShips.Contains(ship))
		{
			//remove from spawned ships
			party.SpawnedShips.Remove(ship);
			//remove from formation
			if(party.Formation.ContainsKey(ship))
			{
				party.Formation.Remove(ship);
			}
			//remove from loadouts
			if(ship.MyLoadout != null && party.FollowerLoadouts.Contains(ship.MyLoadout))
			{
				party.FollowerLoadouts.Remove(ship.MyLoadout);
			}
		}
	}




	private void SpawnPartyMembers(MacroAIParty party)
	{
		if(!party.IsPlayerParty)
		{
			party.SpawnedShipsLeader = SpawnPartyMember(party, party.LeaderLoadout);
		}
		foreach(Loadout loadOut in party.FollowerLoadouts)
		{
			SpawnPartyMember(party, loadOut);
		}
	}

	private ShipBase SpawnPartyMember(MacroAIParty party, Loadout loadout)
	{
		
		ShipBase ship = GameManager.Inst.NPCManager.SpawnAIShip(loadout, party.FactionID, party);
		ship.transform.position = party.Location.RealPos + UnityEngine.Random.insideUnitSphere * 15f;
		party.SpawnedShips.Add(ship);
		AI ai = ship.GetComponent<AI>();
		ai.MyParty = party;
		if(ship.MyReference.ExhaustController != null)
		{
			ship.MyReference.ExhaustController.setExhaustState(ExhaustState.Normal);
		}

		if(party.SpawnedShipsLeader != null)
		{
			ai.Whiteboard.Parameters["FriendlyTarget"] = party.SpawnedShipsLeader;
		}
		ai.Deactivate();
		if(party.DockedStationID != "")
		{
			Debug.LogError("docked! at " + party.DockedStationID);
			ship.Hide();
			ai.IsDocked = true;
			ship.DockedStationID = party.DockedStationID;
		}
		ship.name = "AIShip-" + (GameManager.Inst.NPCManager.AllShips.Count+1);

		Debug.LogError("Spawning ship! " + ai.MyShip.name + " party " + party.PartyNumber);
		GameManager.Inst.NPCManager.AddExistingShip(ship);

		return ship;
	}

	private void DespawnParty(MacroAIParty party)
	{
		Debug.Log("Despawning party!");
		List<ShipBase> spawnedShipsCopy = new List<ShipBase>(party.SpawnedShips);
		foreach(ShipBase ship in spawnedShipsCopy)
		{
			GameManager.Inst.NPCManager.RemoveExistingShip(ship);
			party.SpawnedShips.Remove(ship);
			ship.GetComponent<AI>().Deactivate();
			GameObject.Destroy(ship.gameObject);
		}

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

	private void GenerateFormationForParty(MacroAIParty party)
	{
		party.Formation = new Dictionary<ShipBase, Vector3>();
		int currentTier = 1;
		int i = 0;
		foreach(ShipBase ship in party.SpawnedShips)
		{
			if(party.SpawnedShipsLeader == ship)
			{
				continue;
			}
			Vector3 disp = Vector3.zero + currentTier * new Vector3(0, 0, -7f);
			float width = 3f;
			if(i == 0)
			{
				disp.y = 1 * width;
			}
			else if(i == 1)
			{
				disp.x = -1 * width;
			}
			else if(i == 2)
			{
				disp.y = -1 * width;
			}
			else if(i == 3)
			{
				disp.x = 1 * width;
				i = 0;
				currentTier ++;
			}
			i++;
			party.Formation.Add(ship, disp);
		}
	}

	private void TradeCommodity(MacroAIParty party, DockableStationData stationData)
	{
		//check if there are any commodity goods in leader's cargo bay. if so, check if station is in demand for it
		//if so, sell it and trigger economy event. if demand level is less than 1, only a certain probability will sell it
		List<InvItemData> cargoCopy = new List<InvItemData>(party.LeaderLoadout.CargoBayItems);
		foreach(InvItemData invItem in cargoCopy)
		{
			if(invItem.Item.Type == ItemType.Commodity)
			{
				ResourceType resourceType = invItem.Item.GetResourceTypeAttribute();
				foreach(DemandResource demand in stationData.DemandResources)
				{
					if(demand.Type == resourceType || demand.ItemID == invItem.Item.ID)
					{
						float sellProbability = 1;
						if(demand.CurrentDemand <= 1)
						{
							sellProbability = 0.4f;
						}

						if(UnityEngine.Random.value < sellProbability)
						{
							party.LeaderLoadout.CargoBayItems.Remove(invItem);
							GameManager.Inst.EconomyManager.OnConvoySellCommodity(stationData, resourceType, invItem.Quantity);
						}
					}
				}
			}
		}

		//check if leader cargobay has room. if so, check if station has any commodity for sale and supply level is above 1
		//if so buy it and trigger economy event. if supply level is less than 1, only a certain probability will buy it
		float cargoBayUsage = party.LeaderLoadout.GetCargoBayUsage();
		float cargoBaySize = GameManager.Inst.ItemManager.GetShipStats(party.LeaderLoadout.ShipID).CargoBaySize;

		foreach(SaleItem saleItem in stationData.TraderSaleItems)
		{
			ItemStats stats = GameManager.Inst.ItemManager.GetItemStats(saleItem.ItemID);
			if(stats.Type == ItemType.Commodity)
			{
				float buyProbability = 1;
				if(saleItem.SupplyLevel <= 1)
				{
					buyProbability = 0.4f;
				}

				if(UnityEngine.Random.value < buyProbability && cargoBayUsage < cargoBaySize)
				{
					InvItemData itemData = new InvItemData();
					Item item = new Item(GameManager.Inst.ItemManager.GetItemStats(saleItem.ItemID));
					itemData.Item = item;
					itemData.Quantity = UnityEngine.Random.Range(1, Mathf.FloorToInt(cargoBaySize - cargoBayUsage));
					GameManager.Inst.ItemManager.AddItemtoInvItemDataList(party.LeaderLoadout.CargoBayItems, itemData, itemData.Quantity);
					GameManager.Inst.EconomyManager.OnConvoyBuyCommodity(stationData, itemData.Item.ID, itemData.Quantity);
				}
			}
		}

	}
}

