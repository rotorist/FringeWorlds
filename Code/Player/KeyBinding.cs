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
	public KeyCode FnKey;
	public KeyCode Key;
	public KeyInput SimilarInput;

	public bool Eval()
	{
		if(FnKey != KeyCode.None && Input.GetKey(FnKey) && Input.GetKeyDown(Key))
		{
			return true;
		}

		if(FnKey == KeyCode.None && Input.GetKeyDown(Key))
		{
			if(SimilarInput == null)
			{
				
				return true;
			}

			if(SimilarInput != null && SimilarInput.FnKey != KeyCode.None && !Input.GetKey(SimilarInput.FnKey))
			{
				return true;
			}

			return false;

		}

		return false;
	}

	public bool EvalKeyDown()
	{
		
		if(FnKey != KeyCode.None && Input.GetKey(FnKey) && Input.GetKey(Key))
		{
			//Debug.Log("true for " + FnKey.ToString() + " + " + Key.ToString());
			return true;
		}

		if(FnKey == KeyCode.None && Input.GetKey(Key))
		{
			if(SimilarInput == null)
			{
				return true;
			}

			if(SimilarInput != null && SimilarInput.FnKey != KeyCode.None && !Input.GetKey(SimilarInput.FnKey))
			{
				return true;
			}
			return false;
		}

		return false;
	}

	public bool IsAnyFnKeyDown()
	{
		if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.AltGr) || Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.RightControl))
		{
			return true;
		}
		else
		{
			return false;
		}
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
	MouseFlight,
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