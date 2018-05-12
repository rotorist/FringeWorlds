using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTSwitch : BTComposite 
{
	public BTCompType CompNodeType;

	private bool _hasStarted = false;

	public override void Initialize ()
	{
		_hasStarted = false;
	}

	public override BTResult Process ()
	{
		if(!_hasStarted)
		{
			Children[0].Initialize();
			Children[1].Initialize();
			Children[2].Initialize();
			_hasStarted = true;
		}

		BTResult condition = Children[0].Process();
		if(condition == BTResult.Running)
		{
			return BTResult.Running;
		}
		else if(condition == BTResult.Success)
		{
			BTResult result = Children[1].Process();
			if(result != BTResult.Running)
			{
				_hasStarted = false;
			}

			return result;
		}
		else
		{
			BTResult result = Children[2].Process();
			if(result != BTResult.Running)
			{
				_hasStarted = false;
			}

			return result;
		}

	}


}
