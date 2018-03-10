using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTRandom : BTComposite
{
	public BTCompType CompNodeType;
	private BTNode _currentRunningChild;

	public override void Initialize ()
	{
		CompNodeType = BTCompType.Random;

		for(int i=0; i<Children.Count; i++)
		{
			Children[i].Initialize();
		}
	}



	public override BTResult Process ()
	{

		if(_currentRunningChild != null)
		{
			BTResult result = _currentRunningChild.Process();
			if(result != BTResult.Running)
			{
				_currentRunningChild = null;
			}

			return result;
		}
		else
		{
			//pick one random child to process
			int rand = UnityEngine.Random.Range(0, Children.Count);
			Children[rand].Initialize();
			BTResult result = Children[rand].Process();
			if(result == BTResult.Running)
			{
				_currentRunningChild = Children[rand];
			}

			return result;
		}



	}

}

