using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTUntilFail : BTDecorator
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
			return BTResult.Fail;
		}

		return BTResult.Running;
	}

}
