using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBinding
{
	public Dictionary<UserInputs, KeyInput> Controls;

	public KeyBinding()
	{
		Controls = new Dictionary<UserInputs, KeyInput>();

	}
}

public class KeyInput
{
	public bool IsFnSet;
	public KeyCode FnKey;
	public KeyCode Key;

	public bool Eval()
	{
		if(IsFnSet && Input.GetKey(FnKey) && Input.GetKeyDown(Key))
		{
			return true;
		}

		if(!IsFnSet && Input.GetKeyDown(Key))
		{
			return true;
		}

		return false;
	}
}

public enum UserInputs
{
	ForwardThruster,
	ReverseThruster,
	RollLeft,
	RollRight,
	VectorUp,
	VectorDown,
	VectorLeft,
	VectorRight,
	Cruise,
	FlightAssist,
	Countermeasure,
	Select,
	Pause,
	FireWeaponGroup1,
	FireWeaponGroup2,
	FireWeaponGroup3,
	FireWeaponGroup4,
	ThrottleUp,
	ThrottleDown,


	None,
}