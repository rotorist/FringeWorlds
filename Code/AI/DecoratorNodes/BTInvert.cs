using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTInvert : BTDecorator
{

	public override void Initialize ()
	{
		Child.Initialize();
	}

	public override BTResult Process ()
	{
		BTResult result = Child.Process();
		if(result == BTResult.Fail)
		{
			return BTResult.Success;
		}
		else if(result == BTResult.Success)
		{
			return BTResult.Fail;
		}

		return result;
	}
}
