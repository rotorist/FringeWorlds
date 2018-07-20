using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTSequence : BTComposite
{
	public BTCompType CompNodeType;

	private BTNode _currentRunningChild;

	public override void Initialize ()
	{
		CompNodeType = BTCompType.Sequence;
		_currentRunningChild = null;

	}



	public override BTResult Process ()
	{
		//go through each child and process it
		//if it returns success, go to next child
		//if it returns fail then return fail
		//if _currentRunningChild is null then start from beginning, otherwise skip to _currentRunningChild


		//start from first child
		this.IsRunning = true;
		BTResult result = BTResult.Running;

		for(int i=0; i<Children.Count; i++)
		{
			if(_currentRunningChild != null && Children[i] == _currentRunningChild)
			{
				result = _currentRunningChild.Process();

			}
			else if(_currentRunningChild != null)
			{
				continue;
			}
			else
			{
				Children[i].Initialize();
				result = Children[i].Process();
			}

			if(result == BTResult.Success)
			{
				//done with this node
				//Debug.Log("Done with child " + i);
				_currentRunningChild = null;
			}
			else if(result == BTResult.Fail)
			{
				//Debug.Log("Failed child " + i);
				return BTResult.Fail;
				this.IsRunning = false;
			}
			else if(result == BTResult.Running)
			{
				//Debug.Log("Running child " + i);
				_currentRunningChild = Children[i];
				return BTResult.Running;
			}

		}

		this.IsRunning = false;

		return BTResult.Success;
	}

}

