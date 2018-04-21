using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarMapNode : MonoBehaviour 
{
	public List<StarMapNode> Neighbors;

	void Update()
	{
		foreach(StarMapNode node in Neighbors)
		{
			Debug.DrawLine(transform.position, node.transform.position, Color.yellow);
		}
	}
}
