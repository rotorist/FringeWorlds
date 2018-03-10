using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BTComposite : BTNode
{
	public List<BTNode> Children;

}

public enum BTCompType
{
	Sequence,
	Selector,
	ParallelAnd,
	ParallelOr,
	ParallelMain,
	Random,
}