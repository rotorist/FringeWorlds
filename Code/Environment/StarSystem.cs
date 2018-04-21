﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarSystem
{
	public string ID;
	public string DisplayName;
	public List<Sun> Suns;
	public List<Planet> Planets;
	public List<StationBase> Stations;
	public List<Tradelane> Tradelanes;



	public StarSystem(string id, string displayName)
	{
		ID = id;
		DisplayName = displayName;
		Suns = new List<Sun>();
		Planets = new List<Planet>();
		Stations = new List<StationBase>();
		Tradelanes = new List<Tradelane>();


	}


}


public class StarSystemData : NavNode
{

	public string DisplayName;
	public string SkyboxName;
	public Color AmbientColor;
	public List<SunData> Suns;
	public List<PlanetData> Planets;
	public List<StationData> Stations;
	public List<JumpGateData> JumpGates;
	public List<TradelaneData> Tradelanes;
	public List<NavNode> ChildNodes;

	public List<MacroAIParty> Parties; //this is used for spawning NPC ships

	public StarSystemData(string id, string displayName)
	{
		ID = id;
		DisplayName = displayName;
		Suns = new List<SunData>();
		Planets = new List<PlanetData>();
		Stations = new List<StationData>();
		JumpGates = new List<JumpGateData>();
		Tradelanes  = new List<TradelaneData>();
		ChildNodes = new List<NavNode>();
		Parties = new List<MacroAIParty>();
		NeighborIDs = new List<string>();
		Neighbors = new List<NavNode>();
		NavNodeType = NavNodeType.System;
	}

}