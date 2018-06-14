using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTNextTree : BTLeaf
{

	public override void Initialize ()
	{
		
	}

	public override BTResult Process ()
	{
		BTResult result = BTResult.Fail;
		if(MyAI != null)
		{
			result = MyAI.TreeSet[Parameters[0]].Run();
		}
		else if(MyParty != null)
		{
			result = MyParty.TreeSet[Parameters[0]].Run();
		}
		return result;
	}

	public override BTResult Exit (BTResult result)
	{
		return result;
	}

}
