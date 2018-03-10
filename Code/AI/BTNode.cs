using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BTNode
{
	public bool IsRunning;
	public float LastRunTime;
	public abstract void Initialize();

	public abstract BTResult Process();

}

public enum BTResult
{
	Success,
	Fail,
	Running,
}