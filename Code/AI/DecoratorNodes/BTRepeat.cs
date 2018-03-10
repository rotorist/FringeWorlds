using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTRepeat : BTDecorator
{
	public override void Initialize ()
	{
		Child.Initialize();
	}

	public override BTResult Process ()
	{
		Child.Process();

		return BTResult.Running;
	}

}
