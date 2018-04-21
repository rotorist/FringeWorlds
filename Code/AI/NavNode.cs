using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NavNode 
{
	public string ID;
	public Vector3 Location;
	public List<string> NeighborIDs;
	public List<NavNode> Neighbors;
	public NavNodeType NavNodeType;

}

public enum NavNodeType
{
	Tradelane,
	JumpGate,
	Station,
	System,
	Temp,
}

public class TempNode : NavNode
{
	public TempNode()
	{
		NeighborIDs = new List<string>();
		Neighbors = new List<NavNode>();
		NavNodeType = NavNodeType.Temp;
	}
}