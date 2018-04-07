using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarSystem
{
	public string ID;
	public string DisplayName;
	public List<Sun> Suns;
	public List<Planet> Planets;
	public List<StationBase> Stations;
	public List<GameObject> TradelaneSets;

	public StarSystem(string id, string displayName)
	{
		ID = id;
		DisplayName = displayName;
		Suns = new List<Sun>();
		Planets = new List<Planet>();
		Stations = new List<StationBase>();
		TradelaneSets = new List<GameObject>();
	}

}
