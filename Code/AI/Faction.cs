using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Faction 
{

	public string ID;
	public string DisplayName;
	public Dictionary<string, float> Relationships;

	public Faction()
	{
		Relationships = new Dictionary<string, float>();
	}
}
