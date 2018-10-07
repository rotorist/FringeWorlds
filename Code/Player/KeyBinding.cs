using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBinding
{
	public Dictionary<UserInputs, KeyCode> Controls;

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

}