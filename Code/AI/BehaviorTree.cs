using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorTree
{
	public string Name;
	public BTNode RootNode;

	public void Initialize()
	{
		RootNode.Initialize();
	}

	public BTResult Run()
	{
		BTResult result = RootNode.Process();
		return result;
	}



}
