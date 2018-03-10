using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTParallelAnd : BTComposite
{
	public BTCompType CompNodeType;


	public override void Initialize ()
	{
		CompNodeType = BTCompType.ParallelAnd;

		for(int i=0; i<Children.Count; i++)
		{
			Children[i].Initialize();
		}
	}



	public override BTResult Process ()
	{
		//go through each child and process it
		//if anyone returns fail then return fail
		//only return success if all children are success
		BTResult finalResult = BTResult.Success;

		BTResult result = BTResult.Running;

		for(int i=0; i<Children.Count; i++)
		{
			result = Children[i].Process();


			if(result == BTResult.Fail)
			{
				return BTResult.Fail;
				this.IsRunning = false;
			}
			else if(result == BTResult.Running)
			{
				finalResult = BTResult.Running;
			}

		}

		return finalResult;
	}

}

