using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BTLeaf : BTNode
{
	public List<string> Parameters;
	public AI MyAI;
	public abstract BTResult Exit(BTResult result);
}
