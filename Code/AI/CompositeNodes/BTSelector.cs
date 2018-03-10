using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTSelector : BTComposite
{
	public BTCompType CompNodeType;

	private BTNode _currentRunningChild;

	public override void Initialize ()
	{
		CompNodeType = BTCompType.Selector;
		_currentRunningChild = null;

	}



	public override BTResult Process ()
	{
		//go through each child and process it
		//if it returns success, end with success, otherwise go to next child
		//if any return fail then return fail
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
				//done with processing
				_currentRunningChild = null;
				this.IsRunning = false;
				return BTResult.Success;
			}
			else if(result == BTResult.Fail)
			{
				_currentRunningChild = null;
			}
			else if(result == BTResult.Running)
			{
				_currentRunningChild = Children[i];
				return BTResult.Running;
			}

		}

		this.IsRunning = false;

		return BTResult.Fail;
	}

}

