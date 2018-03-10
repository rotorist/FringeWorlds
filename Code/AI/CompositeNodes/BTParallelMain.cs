using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTParallelMain : BTComposite
{
	public BTCompType CompNodeType;


	public override void Initialize ()
	{
		CompNodeType = BTCompType.ParallelMain;

		for(int i=0; i<Children.Count; i++)
		{
			Children[i].Initialize();
		}
	}



	public override BTResult Process ()
	{
		//go through each child and process it
		//only return the status of first child
		BTResult finalResult = BTResult.Success;

		BTResult result = BTResult.Running;

		for(int i=0; i<Children.Count; i++)
		{

			result = Children[i].Process();
			if(i == 0)
			{
				finalResult = result;
			}

		}

		return finalResult;
	}

}

