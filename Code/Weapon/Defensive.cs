using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defensive
{
	public DefensiveType Type;
	public ShipBase ParentShip;
	public string AmmoID;
}

public enum DefensiveType
{
	Countermeasure,
	SmokeScreen,
	Cloak,
}